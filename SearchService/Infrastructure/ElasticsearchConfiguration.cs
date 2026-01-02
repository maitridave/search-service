using Elasticsearch.Net;
using Nest;

namespace SearchService.Infrastructure;

public class ElasticsearchConfiguration
{
    public static IElasticClient CreateClient(IConfiguration configuration)
    {
        var uri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
        var defaultIndex = "automotive_search";
        
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


        return new ElasticClient(connectionSettings);
    }

    public static async Task CreateIndicesAsync(IElasticClient client)
    {
        await CreateAutomotiveSearchIndexAsync(client);
    }

    private static async Task CreateAutomotiveSearchIndexAsync(IElasticClient client)
    {
        var indexName = "automotive_search";
        var existsResponse = await client.Indices.ExistsAsync(indexName);

        if (!existsResponse.Exists)
        {
            var createIndexResponse = await client.Indices.CreateAsync(indexName, c => c
                .Settings(s => s
                    .NumberOfShards(4)
                    .NumberOfReplicas(1)
                    .Analysis(a => a
                        .Analyzers(an => an
                            .Custom("autocomplete_analyzer", ca => ca
                                .Tokenizer("autocomplete_tokenizer")
                                .Filters("lowercase", "asciifolding")
                            )
                        )
                        .Tokenizers(t => t
                            .EdgeNGram("autocomplete_tokenizer", e => e
                                .MinGram(2)
                                .MaxGram(10)
                                .TokenChars(TokenChar.Letter, TokenChar.Digit)
                            )
                        )
                    )
                )
                .Map(m => m
                    .Properties(p => p
                        .Number(n => n.Name("seller_id").Type(NumberType.Integer))
                        .Number(n => n.Name("buyer_id").Type(NumberType.Integer))
                        .Number(n => n.Name("carrier_id").Type(NumberType.Integer))
                        .Number(n => n.Name("purchase_id").Type(NumberType.Integer))
                        .Number(n => n.Name("transport_id").Type(NumberType.Integer))
                        .Number(n => n.Name("offer_id").Type(NumberType.Integer))
                        .Keyword(k => k.Name("vin"))
                        .Text(t => t.Name("make").Analyzer("autocomplete_analyzer"))
                        .Text(t => t.Name("model").Analyzer("autocomplete_analyzer"))
                        .Text(t => t.Name("trim").Analyzer("autocomplete_analyzer"))
                        .Number(n => n.Name("year").Type(NumberType.Integer))
                        .Number(n => n.Name("offer_amount").Type(NumberType.Double))
                        .Number(n => n.Name("bid_amount").Type(NumberType.Double))
                        .Keyword(k => k.Name("offer_status"))
                        .Keyword(k => k.Name("purchase_status"))
                        .Keyword(k => k.Name("transport_status"))
                        .Text(t => t.Name("city").Analyzer("autocomplete_analyzer"))
                        .Keyword(k => k.Name("state"))
                        .Text(t => t.Name("search_text").Analyzer("autocomplete_analyzer"))
                    )
                )
            );

            if (!createIndexResponse.IsValid)
            {
                throw new Exception($"Failed to create index {indexName}: {createIndexResponse.DebugInformation}");
            }
        }
    }
}
