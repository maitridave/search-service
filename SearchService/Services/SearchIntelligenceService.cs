using System.Diagnostics;
using System.Text.RegularExpressions;
using Nest;
using SearchService.Models.EntityModels;
using SearchService.Models.RequestModels;
using SearchService.Models.SearchModels;

namespace SearchService.Services;

public interface ISearchIntelligenceService
{
    Task<SearchResultResponse<object>> UnifiedSearchAsync(SearchQuery request);
    Task<SearchResultResponse<OfferEntity>> SearchOffersAsync(SearchQuery request);
    Task<SearchResultResponse<PurchaseEntity>> SearchPurchasesAsync(SearchQuery request);
    Task<SearchResultResponse<TransportEntity>> SearchTransportsAsync(SearchQuery request);
    Task<AutocompleteResponse> AutocompleteAsync(AutocompleteRequest request);
}

public class SearchIntelligenceService : ISearchIntelligenceService
{
    private readonly IElasticClient _client;
    private readonly ISecurityService _securityService;
    private readonly ILogger<SearchIntelligenceService> _logger;

    // Regex patterns for entity detection
    private static readonly Regex VinPattern = new(@"[A-HJ-NPR-Z0-9]{17}", RegexOptions.Compiled);
    private static readonly Regex PhonePattern = new(@"\+?[\d\s\-\(\)]{10,}", RegexOptions.Compiled);

    public SearchIntelligenceService(
        IElasticClient client,
        ISecurityService securityService,
        ILogger<SearchIntelligenceService> logger)
    {
        _client = client;
        _securityService = securityService;
        _logger = logger;
    }

    public async Task<SearchResultResponse<object>> UnifiedSearchAsync(SearchQuery request)
    {
        var stopwatch = Stopwatch.StartNew();
        var results = new List<SearchResult<object>>();
        var facets = new Dictionary<string, List<FacetResult>>();

        try
        {
            // Detect entity type from query
            var detectedEntity = DetectEntityType(request.Query);
            
            // Search across all entity types or specific ones
            var entityTypes = request.EntityTypes.Any() ? request.EntityTypes : new List<string> { "offers", "purchases", "transports" };

            foreach (var entityType in entityTypes)
            {
                object? entityResults = entityType.ToLower() switch
                {
                    "offers" => await SearchOffersInternalAsync(request),
                    "purchases" => await SearchPurchasesInternalAsync(request),
                    "transports" => await SearchTransportsInternalAsync(request),
                    _ => null
                };

                if (entityResults != null)
                {
                    if (entityResults is SearchResultResponse<OfferEntity> offerResults)
                    {
                        results.AddRange(offerResults.Results.Select(r => new SearchResult<object>
                        {
                            Document = r.Document,
                            Score = r.Score,
                            Highlights = r.Highlights
                        }));
                    }
                    else if (entityResults is SearchResultResponse<PurchaseEntity> purchaseResults)
                    {
                        results.AddRange(purchaseResults.Results.Select(r => new SearchResult<object>
                        {
                            Document = r.Document,
                            Score = r.Score,
                            Highlights = r.Highlights
                        }));
                    }
                    else if (entityResults is SearchResultResponse<TransportEntity> transportResults)
                    {
                        results.AddRange(transportResults.Results.Select(r => new SearchResult<object>
                        {
                            Document = r.Document,
                            Score = r.Score,
                            Highlights = r.Highlights
                        }));
                    }
                }
            }

            stopwatch.Stop();

            return new SearchResultResponse<object>
            {
                TotalHits = results.Count,
                MaxScore = results.Any() ? results.Max(r => r.Score) : 0,
                Results = results.OrderByDescending(r => r.Score).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Facets = facets,
                Suggestions = new List<string>(),
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing unified search");
            throw;
        }
    }

    public async Task<SearchResultResponse<OfferEntity>> SearchOffersAsync(SearchQuery request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await SearchOffersInternalAsync(request);
        stopwatch.Stop();
        result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        return result;
    }

    public async Task<SearchResultResponse<PurchaseEntity>> SearchPurchasesAsync(SearchQuery request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await SearchPurchasesInternalAsync(request);
        stopwatch.Stop();
        result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        return result;
    }

    public async Task<SearchResultResponse<TransportEntity>> SearchTransportsAsync(SearchQuery request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await SearchTransportsInternalAsync(request);
        stopwatch.Stop();
        result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        return result;
    }

    public async Task<AutocompleteResponse> AutocompleteAsync(AutocompleteRequest request)
    {
        try
        {
            var indexName = GetIndexNameFromEntityType(request.EntityType);
            
            var searchResponse = await _client.SearchAsync<dynamic>(s => s
                .Index(indexName)
                .Size(request.Size)
                .Suggest(su => su
                    .Completion("autocomplete", c => c
                        .Field("suggest")
                        .Prefix(request.Query)
                        .Fuzzy(f => f.Fuzziness(Fuzziness.Auto))
                        .Size(request.Size)
                    )
                )
            );

            var suggestions = new List<AutocompleteSuggestion>();
            
            if (searchResponse.Suggest != null && searchResponse.Suggest.ContainsKey("autocomplete"))
            {
                var completionSuggestions = searchResponse.Suggest["autocomplete"];
                
                foreach (var suggestion in completionSuggestions)
                {
                    foreach (var option in suggestion.Options)
                    {
                        suggestions.Add(new AutocompleteSuggestion
                        {
                            Text = option.Text,
                            Score = option.Score
                        });
                    }
                }
            }

            return new AutocompleteResponse { Suggestions = suggestions };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing autocomplete");
            throw;
        }
    }

    private async Task<SearchResultResponse<OfferEntity>> SearchOffersInternalAsync(SearchQuery request)
    {
        var securityFilter = _securityService.BuildSecurityFilter(request.UserRole, request.UserId, "offers");

        var searchDescriptor = new SearchDescriptor<OfferEntity>()
            .Index("offers")
            .From((request.Page - 1) * request.PageSize)
            .Size(request.PageSize)
            .Query(q => BuildOfferQuery(q, request, securityFilter))
            .Aggregations(a => BuildAggregations(a))
            .TrackTotalHits();

        if (request.EnableHighlighting)
        {
            searchDescriptor.Highlight(h => h
                .Fields(
                    f => f.Field(ff => ff.Make).PreTags("<em>").PostTags("</em>"),
                    f => f.Field(ff => ff.Model).PreTags("<em>").PostTags("</em>"),
                    f => f.Field(ff => ff.VIN).PreTags("<em>").PostTags("</em>")
                )
            );
        }

        var response = await _client.SearchAsync<OfferEntity>(searchDescriptor);

        return MapSearchResponse(response);
    }

    private async Task<SearchResultResponse<PurchaseEntity>> SearchPurchasesInternalAsync(SearchQuery request)
    {
        var securityFilter = _securityService.BuildSecurityFilter(request.UserRole, request.UserId, "purchases");

        var searchDescriptor = new SearchDescriptor<PurchaseEntity>()
            .Index("purchases")
            .From((request.Page - 1) * request.PageSize)
            .Size(request.PageSize)
            .Query(q => BuildPurchaseQuery(q, request, securityFilter))
            .TrackTotalHits();

        if (request.EnableHighlighting)
        {
            searchDescriptor.Highlight(h => h
                .Fields(
                    f => f.Field(ff => ff.BuyerDetails.Name).PreTags("<em>").PostTags("</em>"),
                    f => f.Field(ff => ff.PurchaseId).PreTags("<em>").PostTags("</em>")
                )
            );
        }

        var response = await _client.SearchAsync<PurchaseEntity>(searchDescriptor);

        return MapSearchResponse(response);
    }

    private async Task<SearchResultResponse<TransportEntity>> SearchTransportsInternalAsync(SearchQuery request)
    {
        var securityFilter = _securityService.BuildSecurityFilter(request.UserRole, request.UserId, "transports");

        var searchDescriptor = new SearchDescriptor<TransportEntity>()
            .Index("transports")
            .From((request.Page - 1) * request.PageSize)
            .Size(request.PageSize)
            .Query(q => BuildTransportQuery(q, request, securityFilter))
            .TrackTotalHits();

        if (request.EnableHighlighting)
        {
            searchDescriptor.Highlight(h => h
                .Fields(
                    f => f.Field(ff => ff.VehicleDetails.VIN).PreTags("<em>").PostTags("</em>"),
                    f => f.Field(ff => ff.PickupLocation).PreTags("<em>").PostTags("</em>"),
                    f => f.Field(ff => ff.DeliveryLocation).PreTags("<em>").PostTags("</em>")
                )
            );
        }

        var response = await _client.SearchAsync<TransportEntity>(searchDescriptor);

        return MapSearchResponse(response);
    }

    private QueryContainer BuildOfferQuery(QueryContainerDescriptor<OfferEntity> q, SearchQuery request, QueryContainer securityFilter)
    {
        var queries = new List<Func<QueryContainerDescriptor<OfferEntity>, QueryContainer>>();

        // Main search query with fuzzy matching
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            queries.Add(mq => mq.MultiMatch(mm => mm
                .Query(request.Query)
                .Fields(f => f
                    .Field(ff => ff.Make, boost: 2.0)
                    .Field(ff => ff.Model, boost: 2.0)
                    .Field(ff => ff.VIN, boost: 3.0)
                    .Field(ff => ff.Location.City)
                    .Field(ff => ff.Location.State)
                )
                .Fuzziness(request.EnableFuzzy ? Fuzziness.Auto : Fuzziness.EditDistance(0))
                .Type(TextQueryType.BestFields)
            ));
        }

        // Apply filters
        foreach (var filter in request.Filters)
        {
            queries.Add(fq => fq.Term(filter.Key, filter.Value));
        }

        var query = q.Bool(b =>
        {
            var boolQuery = b.Must(queries.ToArray());
            
            if (securityFilter != null)
            {
                boolQuery = boolQuery.Filter(f => securityFilter);
            }

            return boolQuery;
        });

        return query;
    }

    private QueryContainer BuildPurchaseQuery(QueryContainerDescriptor<PurchaseEntity> q, SearchQuery request, QueryContainer securityFilter)
    {
        var queries = new List<Func<QueryContainerDescriptor<PurchaseEntity>, QueryContainer>>();

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            queries.Add(mq => mq.MultiMatch(mm => mm
                .Query(request.Query)
                .Fields(f => f
                    .Field(ff => ff.PurchaseId, boost: 3.0)
                    .Field(ff => ff.BuyerDetails.Name, boost: 2.0)
                    .Field(ff => ff.BuyerDetails.Contact)
                )
                .Fuzziness(request.EnableFuzzy ? Fuzziness.Auto : Fuzziness.EditDistance(0))
            ));
        }

        var query = q.Bool(b =>
        {
            var boolQuery = b.Must(queries.ToArray());
            
            if (securityFilter != null)
            {
                boolQuery = boolQuery.Filter(f => securityFilter);
            }

            return boolQuery;
        });

        return query;
    }

    private QueryContainer BuildTransportQuery(QueryContainerDescriptor<TransportEntity> q, SearchQuery request, QueryContainer securityFilter)
    {
        var queries = new List<Func<QueryContainerDescriptor<TransportEntity>, QueryContainer>>();

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            queries.Add(mq => mq.MultiMatch(mm => mm
                .Query(request.Query)
                .Fields(f => f
                    .Field(ff => ff.TransportId, boost: 3.0)
                    .Field(ff => ff.VehicleDetails.VIN, boost: 3.0)
                    .Field(ff => ff.PickupLocation, boost: 2.0)
                    .Field(ff => ff.DeliveryLocation, boost: 2.0)
                )
                .Fuzziness(request.EnableFuzzy ? Fuzziness.Auto : Fuzziness.EditDistance(0))
            ));
        }

        var query = q.Bool(b =>
        {
            var boolQuery = b.Must(queries.ToArray());
            
            if (securityFilter != null)
            {
                boolQuery = boolQuery.Filter(f => securityFilter);
            }

            return boolQuery;
        });

        return query;
    }

    private AggregationContainerDescriptor<OfferEntity> BuildAggregations(AggregationContainerDescriptor<OfferEntity> a)
    {
        return a
            .Terms("by_make", t => t.Field(f => f.Make.Suffix("keyword")).Size(10))
            .Terms("by_status", t => t.Field(f => f.Status).Size(10))
            .Terms("by_condition", t => t.Field(f => f.Condition).Size(10));
    }

    private SearchResultResponse<T> MapSearchResponse<T>(ISearchResponse<T> response) where T : class
    {
        var results = new List<SearchResult<T>>();

        foreach (var hit in response.Hits)
        {
            var highlights = new Dictionary<string, List<string>>();
            
            if (hit.Highlight != null)
            {
                foreach (var highlight in hit.Highlight)
                {
                    highlights[highlight.Key] = highlight.Value.ToList();
                }
            }

            results.Add(new SearchResult<T>
            {
                Document = hit.Source,
                Score = hit.Score ?? 0,
                Highlights = highlights
            });
        }

        var facets = new Dictionary<string, List<FacetResult>>();
        
        if (response.Aggregations != null)
        {
            foreach (var agg in response.Aggregations)
            {
                if (agg.Value is BucketAggregate bucketAgg)
                {
                    var facetResults = bucketAgg.Items
                        .OfType<KeyedBucket<object>>()
                        .Select(b => new FacetResult
                        {
                            Key = b.Key.ToString() ?? string.Empty,
                            Count = b.DocCount ?? 0
                        })
                        .ToList();

                    facets[agg.Key] = facetResults;
                }
            }
        }

        return new SearchResultResponse<T>
        {
            TotalHits = response.Total,
            MaxScore = response.MaxScore,
            Results = results,
            Facets = facets,
            Suggestions = new List<string>()
        };
    }

    private string DetectEntityType(string query)
    {
        if (VinPattern.IsMatch(query))
            return "vin";
        if (PhonePattern.IsMatch(query))
            return "phone";
        
        return "text";
    }

    private string GetIndexNameFromEntityType(string entityType)
    {
        return entityType.ToLower() switch
        {
            "offer" or "offers" => "offers",
            "purchase" or "purchases" => "purchases",
            "transport" or "transports" => "transports",
            _ => "offers"
        };
    }
}
