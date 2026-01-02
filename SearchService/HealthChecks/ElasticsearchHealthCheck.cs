using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nest;

namespace SearchService.HealthChecks;

public class ElasticsearchHealthCheck : IHealthCheck
{
    private readonly IElasticClient _elasticClient;

    public ElasticsearchHealthCheck(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var pingResponse = await _elasticClient.PingAsync(ct: cancellationToken);
            
            if (pingResponse.IsValid)
            {
                // Also check cluster health
                var healthResponse = await _elasticClient.Cluster.HealthAsync(ct: cancellationToken);
                
                if (healthResponse.IsValid)
                {
                    var statusString = healthResponse.Status.ToString().ToLower();
                    var data = new Dictionary<string, object>
                    {
                        ["cluster_name"] = healthResponse.ClusterName,
                        ["status"] = statusString,
                        ["number_of_nodes"] = healthResponse.NumberOfNodes,
                        ["active_primary_shards"] = healthResponse.ActivePrimaryShards,
                        ["active_shards"] = healthResponse.ActiveShards
                    };

                    return statusString switch
                    {
                        "green" => HealthCheckResult.Healthy("Elasticsearch cluster is healthy", data),
                        "yellow" => HealthCheckResult.Degraded("Elasticsearch cluster is in yellow state", null, data),
                        "red" => HealthCheckResult.Unhealthy("Elasticsearch cluster is in red state", null, data),
                        _ => HealthCheckResult.Unhealthy("Unknown Elasticsearch cluster state", null, data)
                    };
                }
                else
                {
                    return HealthCheckResult.Unhealthy($"Elasticsearch cluster health check failed: {healthResponse.DebugInformation}");
                }
            }
            else
            {
                return HealthCheckResult.Unhealthy($"Elasticsearch ping failed: {pingResponse.DebugInformation}");
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Elasticsearch health check threw exception: {ex.Message}", ex);
        }
    }
}
