using Elasticsearch.Net;
using Nest;

namespace SearchService.Infrastructure;

public class ElasticsearchConfiguration
{
    public static IElasticClient CreateClient(IConfiguration configuration)
    {
        var uri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
        var defaultIndex = configuration["Elasticsearch:DefaultIndex"] ?? "search-service";
        
        var pool = new SingleNodeConnectionPool(new Uri(uri));
        var connectionSettings = new ConnectionSettings(pool)
            .DefaultIndex(defaultIndex)
            .EnableApiVersioningHeader()
            .DisableDirectStreaming()
            .PrettyJson()
            .RequestTimeout(TimeSpan.FromSeconds(60))
            .MaximumRetries(5)
            .MaxRetryTimeout(TimeSpan.FromSeconds(120))
            .DeadTimeout(TimeSpan.FromSeconds(60))
            .PingTimeout(TimeSpan.FromSeconds(30))
            .SniffOnConnectionFault(false)
            .SniffOnStartup(false)
            .ThrowExceptions(false) // Don't throw exceptions, let us handle them
            .OnRequestCompleted(details =>
            {
                if (details.DebugInformation != null)
                {
                    Console.WriteLine($"Elasticsearch request: {details.DebugInformation}");
                }
            });

        // Register index mappings
        connectionSettings.DefaultMappingFor<Models.EntityModels.OfferEntity>(m => m
            .IndexName("offers")
            .IdProperty(p => p.OfferId)
        );

        connectionSettings.DefaultMappingFor<Models.EntityModels.PurchaseEntity>(m => m
            .IndexName("purchases")
            .IdProperty(p => p.PurchaseId)
        );

        connectionSettings.DefaultMappingFor<Models.EntityModels.TransportEntity>(m => m
            .IndexName("transports")
            .IdProperty(p => p.TransportId)
        );

        return new ElasticClient(connectionSettings);
    }

    public static async Task CreateIndicesAsync(IElasticClient client)
    {
        await CreateOfferIndexAsync(client);
        await CreatePurchaseIndexAsync(client);
        await CreateTransportIndexAsync(client);
    }

    private static async Task CreateOfferIndexAsync(IElasticClient client)
    {
        var indexName = "offers";
        var existsResponse = await client.Indices.ExistsAsync(indexName);

        if (!existsResponse.Exists)
        {
            var createIndexResponse = await client.Indices.CreateAsync(indexName, c => c
                .Settings(s => s
                    .NumberOfShards(3)
                    .NumberOfReplicas(1)
                    .Analysis(a => a
                        .Analyzers(an => an
                            .Custom("autocomplete_analyzer", ca => ca
                                .Tokenizer("standard")
                                .Filters("lowercase", "edge_ngram_filter")
                            )
                            .Custom("synonym_analyzer", sa => sa
                                .Tokenizer("standard")
                                .Filters("lowercase", "synonym_filter")
                            )
                        )
                        .TokenFilters(tf => tf
                            .EdgeNGram("edge_ngram_filter", e => e
                                .MinGram(2)
                                .MaxGram(20)
                            )
                            .Synonym("synonym_filter", sy => sy
                                .Synonyms("car, vehicle, auto, automobile")
                            )
                        )
                    )
                )
                .Map<Models.EntityModels.OfferEntity>(m => m
                    .AutoMap()
                    .Properties(p => p
                        .Text(t => t.Name(n => n.Make).Analyzer("synonym_analyzer").Fields(f => f.Keyword(k => k.Name("keyword"))))
                        .Text(t => t.Name(n => n.Model).Analyzer("synonym_analyzer").Fields(f => f.Keyword(k => k.Name("keyword"))))
                        .Number(n => n.Name(nn => nn.OfferAmount).Type(NumberType.ScaledFloat).ScalingFactor(100))
                    )
                )
            );

            if (!createIndexResponse.IsValid)
            {
                throw new Exception($"Failed to create index {indexName}: {createIndexResponse.DebugInformation}");
            }
        }
    }

    private static async Task CreatePurchaseIndexAsync(IElasticClient client)
    {
        var indexName = "purchases";
        var existsResponse = await client.Indices.ExistsAsync(indexName);

        if (!existsResponse.Exists)
        {
            var createIndexResponse = await client.Indices.CreateAsync(indexName, c => c
                .Settings(s => s
                    .NumberOfShards(3)
                    .NumberOfReplicas(1)
                )
                .Map<Models.EntityModels.PurchaseEntity>(m => m
                    .AutoMap()
                    .Properties(p => p
                        .Number(n => n.Name(nn => nn.Amount).Type(NumberType.ScaledFloat).ScalingFactor(100))
                    )
                )
            );

            if (!createIndexResponse.IsValid)
            {
                throw new Exception($"Failed to create index {indexName}: {createIndexResponse.DebugInformation}");
            }
        }
    }

    private static async Task CreateTransportIndexAsync(IElasticClient client)
    {
        var indexName = "transports";
        var existsResponse = await client.Indices.ExistsAsync(indexName);

        if (!existsResponse.Exists)
        {
            var createIndexResponse = await client.Indices.CreateAsync(indexName, c => c
                .Settings(s => s
                    .NumberOfShards(3)
                    .NumberOfReplicas(1)
                )
                .Map<Models.EntityModels.TransportEntity>(m => m
                    .AutoMap()
                )
            );

            if (!createIndexResponse.IsValid)
            {
                throw new Exception($"Failed to create index {indexName}: {createIndexResponse.DebugInformation}");
            }
        }
    }
}
