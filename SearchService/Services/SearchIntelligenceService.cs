using System.Diagnostics;
using System.Text.RegularExpressions;
using Nest;
using SearchService.Models.Documents;
using SearchService.Models.RequestModels;
using SearchService.Models.SearchModels;

namespace SearchService.Services;

public interface ISearchIntelligenceService
{
    Task<SearchResultResponse<AutomotiveSearchDocument>> UnifiedSearchAsync(SearchQuery request);
    Task<AutocompleteResponse> AutocompleteAsync(AutocompleteRequest request);
}

public class SearchIntelligenceService : ISearchIntelligenceService
{
    private readonly IElasticClient _client;
    private readonly ISecurityService _securityService;
    private readonly ILogger<SearchIntelligenceService> _logger;
    private const string IndexName = "automotive_search";

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

    public async Task<SearchResultResponse<AutomotiveSearchDocument>> UnifiedSearchAsync(SearchQuery request)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Starting unified search for query: {Query}", request.Query);

            // Build the search descriptor
            var searchDescriptor = new SearchDescriptor<AutomotiveSearchDocument>()
                .Index(IndexName)
                .From((request.Page - 1) * request.PageSize)
                .Size(request.PageSize)
                .TrackTotalHits(true);

            // Apply security filtering
            var securityFilter = _securityService.BuildSecurityFilter(request.UserRole, request.UserId, "");
            
            // Build the main query
            var queryContainer = BuildQuery(request, securityFilter);
            searchDescriptor = searchDescriptor.Query(q => queryContainer);

            // Add aggregations for facets
            searchDescriptor = AddAggregations(searchDescriptor);

            // Add highlighting if enabled
            if (request.EnableHighlighting)
            {
                searchDescriptor = searchDescriptor.Highlight(h => h
                    .Fields(
                        f => f.Field(doc => doc.Make).PreTags("<em>").PostTags("</em>"),
                        f => f.Field(doc => doc.Model).PreTags("<em>").PostTags("</em>"),
                        f => f.Field(doc => doc.VIN).PreTags("<em>").PostTags("</em>"),
                        f => f.Field(doc => doc.SearchText).PreTags("<em>").PostTags("</em>"),
                        f => f.Field(doc => doc.City).PreTags("<em>").PostTags("</em>")
                    )
                );
            }

            // Execute the search
            var response = await _client.SearchAsync<AutomotiveSearchDocument>(searchDescriptor);

            if (!response.IsValid)
            {
                _logger.LogError("Search failed: {Error}", response.DebugInformation);
            return new SearchResultResponse<AutomotiveSearchDocument>
            {
                TotalHits = 0,
                Results = new List<SearchResult<AutomotiveSearchDocument>>(),
                Facets = new Dictionary<string, List<FacetResult>>(),
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
            }

            // Process results
            var results = response.Documents.Select((doc, index) =>
            {
                var hit = response.Hits.ElementAtOrDefault(index);
                return new SearchResult<AutomotiveSearchDocument>
                {
                    Document = doc,
                    Score = hit?.Score ?? 0,
                    Highlights = hit?.Highlight?.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value.ToList()
                    ) ?? new Dictionary<string, List<string>>()
                };
            }).ToList();

            // Process facets
            var facets = ProcessAggregations(response.Aggregations);

            stopwatch.Stop();

            return new SearchResultResponse<AutomotiveSearchDocument>
            {
                TotalHits = response.Total,
                MaxScore = response.MaxScore,
                Results = results,
                Facets = facets,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in unified search");
            stopwatch.Stop();

            return new SearchResultResponse<AutomotiveSearchDocument>
            {
                TotalHits = 0,
                Results = new List<SearchResult<AutomotiveSearchDocument>>(),
                Facets = new Dictionary<string, List<FacetResult>>(),
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
    }

    public async Task<AutocompleteResponse> AutocompleteAsync(AutocompleteRequest request)
    {
        try
        {
            _logger.LogInformation("Starting autocomplete for term: {Term}", request.Term);

            // Build autocomplete query
            var searchDescriptor = new SearchDescriptor<AutomotiveSearchDocument>()
                .Index(IndexName)
                .Size(request.MaxResults)
                .Source(src => src.Includes(i => i
                    .Field(f => f.Make)
                    .Field(f => f.Model)
                    .Field(f => f.VIN)
                    .Field(f => f.Year)
                    .Field(f => f.EntityType)
                ))
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .MultiMatch(mm => mm
                                .Query(request.Term)
                                .Type(TextQueryType.BoolPrefix)
                                .Fields(f => f
                                    .Field(doc => doc.Make, boost: 3.0)
                                    .Field(doc => doc.Model, boost: 3.0)
                                    .Field(doc => doc.VIN, boost: 2.0)
                                    .Field(doc => doc.SearchText, boost: 1.0)
                                )
                            )
                        )
                    )
                );

            var response = await _client.SearchAsync<AutomotiveSearchDocument>(searchDescriptor);

            if (!response.IsValid)
            {
                _logger.LogError("Autocomplete search failed: {Error}", response.DebugInformation);
                return new AutocompleteResponse { Suggestions = new List<AutocompleteSuggestion>() };
            }

            // Generate suggestions
            var suggestions = response.Documents
                .SelectMany((doc, index) => GenerateSuggestions(doc, request.Term)
                    .Select(text => new AutocompleteSuggestion 
                    { 
                        Text = text, 
                        Score = response.Hits.ElementAtOrDefault(index)?.Score ?? 0 
                    }))
                .Distinct()
                .Take(request.MaxResults)
                .ToList();

            return new AutocompleteResponse { Suggestions = suggestions };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in autocomplete");
            return new AutocompleteResponse { Suggestions = new List<AutocompleteSuggestion>() };
        }
    }

    private QueryContainer BuildQuery(SearchQuery request, QueryContainer? securityFilter)
    {
        var queries = new List<QueryContainer>();

        // Main search query
        if (!string.IsNullOrEmpty(request.Query))
        {
            // Detect if it's a VIN
            if (VinPattern.IsMatch(request.Query))
            {
                queries.Add(Query<AutomotiveSearchDocument>.Term(t => t.Field(f => f.VIN).Value(request.Query)));
            }
            else
            {
                // Multi-field search
                var multiMatchQuery = Query<AutomotiveSearchDocument>.MultiMatch(m => m
                    .Query(request.Query)
                    .Type(TextQueryType.BestFields)
                    .Fields(f => f
                        .Field(doc => doc.Make, boost: 2.0)
                        .Field(doc => doc.Model, boost: 2.0)
                        .Field(doc => doc.VIN, boost: 3.0)
                        .Field(doc => doc.SearchText, boost: 1.0)
                        .Field(doc => doc.City, boost: 1.0)
                        .Field(doc => doc.State, boost: 1.0)
                    )
                    .Fuzziness(request.EnableFuzzy ? Fuzziness.Auto : Fuzziness.EditDistance(0))
                );

                queries.Add(multiMatchQuery);
            }
        }
        else
        {
            // Match all if no specific query
            queries.Add(Query<AutomotiveSearchDocument>.MatchAll());
        }

        // Apply filters
        var filters = new List<QueryContainer>();
        
        // Add security filter if exists
        if (securityFilter != null)
        {
            filters.Add(securityFilter);
        }
        
        // Entity type filter
        if (request.EntityTypes.Any())
        {
            filters.Add(Query<AutomotiveSearchDocument>.Terms(t => t.Field(f => f.EntityType).Terms(request.EntityTypes)));
        }

        // Custom filters
        foreach (var filter in request.Filters)
        {
            filters.Add(Query<AutomotiveSearchDocument>.Term(t => t.Field(filter.Key).Value(filter.Value)));
        }

        // Combine queries and filters
        return Query<AutomotiveSearchDocument>.Bool(b => b
            .Must(queries.ToArray())
            .Filter(filters.ToArray())
        );
    }

    private SearchDescriptor<AutomotiveSearchDocument> AddAggregations(SearchDescriptor<AutomotiveSearchDocument> searchDescriptor)
    {
        return searchDescriptor.Aggregations(a => a
            .Terms("by_entity_type", t => t.Field(f => f.EntityType).Size(10))
            .Terms("by_make", t => t.Field(f => f.Make.Suffix("keyword")).Size(10))
            .Terms("by_year", t => t.Field(f => f.Year).Size(10))
            .Terms("by_offer_status", t => t.Field(f => f.OfferStatus).Size(10))
            .Terms("by_purchase_status", t => t.Field(f => f.PurchaseStatus).Size(10))
            .Terms("by_transport_status", t => t.Field(f => f.TransportStatus).Size(10))
            .Terms("by_state", t => t.Field(f => f.State).Size(20))
        );
    }

    private Dictionary<string, List<FacetResult>> ProcessAggregations(IReadOnlyDictionary<string, IAggregate> aggregations)
    {
        var facets = new Dictionary<string, List<FacetResult>>();

        foreach (var agg in aggregations)
        {
            if (agg.Value is BucketAggregate bucketAgg)
            {
                var facetResults = bucketAgg.Items
                    .OfType<KeyedBucket<object>>()
                    .Select(bucket => new FacetResult
                    {
                        Key = bucket.Key?.ToString() ?? "",
                        Count = bucket.DocCount ?? 0
                    })
                    .Where(f => f.Count > 0)
                    .OrderByDescending(f => f.Count)
                    .ToList();

                if (facetResults.Any())
                {
                    facets[agg.Key] = facetResults;
                }
            }
        }

        return facets;
    }


    private List<string> GenerateSuggestions(AutomotiveSearchDocument doc, string term)
    {
        var suggestions = new List<string>();
        var lowerTerm = term.ToLower();

        // Vehicle suggestions
        if (!string.IsNullOrEmpty(doc.Make) && doc.Make.ToLower().Contains(lowerTerm))
            suggestions.Add(doc.Make);

        if (!string.IsNullOrEmpty(doc.Model) && doc.Model.ToLower().Contains(lowerTerm))
            suggestions.Add(doc.Model);

        if (!string.IsNullOrEmpty(doc.Make) && !string.IsNullOrEmpty(doc.Model))
            suggestions.Add($"{doc.Year} {doc.Make} {doc.Model}");

        // VIN suggestions
        if (!string.IsNullOrEmpty(doc.VIN) && doc.VIN.ToLower().Contains(lowerTerm))
            suggestions.Add(doc.VIN);

        return suggestions.Distinct().ToList();
    }

    private string DetectEntityType(string query)
    {
        if (string.IsNullOrEmpty(query)) return "";

        var lowerQuery = query.ToLower();

        // Transportation keywords
        if (lowerQuery.Contains("transport") || lowerQuery.Contains("delivery") || 
            lowerQuery.Contains("shipping") || lowerQuery.Contains("carrier"))
            return "transport";

        // Purchase keywords
        if (lowerQuery.Contains("purchase") || lowerQuery.Contains("buy") || 
            lowerQuery.Contains("bought") || lowerQuery.Contains("buyer"))
            return "purchase";

        // Offer keywords (default)
        return "offer";
    }
}
