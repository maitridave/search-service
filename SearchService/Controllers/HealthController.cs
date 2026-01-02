using Microsoft.AspNetCore.Mvc;
using Nest;
using SearchService.Models.SearchModels;

namespace SearchService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<HealthController> _logger;

    public HealthController(IElasticClient elasticClient, ILogger<HealthController> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var response = new HealthResponse
        {
            Status = "Healthy",
            Components = new Dictionary<string, string>(),
            Timestamp = DateTime.UtcNow
        };

        try
        {
            // Check Elasticsearch connection
            var pingResponse = await _elasticClient.PingAsync();
            response.Components["Elasticsearch"] = pingResponse.IsValid ? "Healthy" : "Unhealthy";

            if (!pingResponse.IsValid)
            {
                response.Status = "Degraded";
                _logger.LogWarning("Elasticsearch is unhealthy");
            }

            // Check indices
            var offerIndexExists = await _elasticClient.Indices.ExistsAsync("offers");
            var purchaseIndexExists = await _elasticClient.Indices.ExistsAsync("purchases");
            var transportIndexExists = await _elasticClient.Indices.ExistsAsync("transports");

            response.Components["Indices"] = 
                (offerIndexExists.Exists && purchaseIndexExists.Exists && transportIndexExists.Exists) 
                ? "Healthy" : "Degraded";

            response.Components["API"] = "Healthy";

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            response.Status = "Unhealthy";
            response.Components["Error"] = ex.Message;
            return StatusCode(503, response);
        }
    }

    /// <summary>
    /// Get performance metrics
    /// </summary>
    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics()
    {
        try
        {
            var stats = await _elasticClient.Cluster.StatsAsync();
            
            return Ok(new
            {
                cluster = new
                {
                    name = stats.ClusterName,
                    status = stats.Status.ToString(),
                    nodes = stats.Nodes?.Count,
                    indices = stats.Indices?.Count
                },
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metrics");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
