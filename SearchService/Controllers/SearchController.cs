using Microsoft.AspNetCore.Mvc;
using SearchService.Models.RequestModels;
using SearchService.Services;

namespace SearchService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchIntelligenceService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ISearchIntelligenceService searchService, ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Universal search across all entity types
    /// </summary>
    [HttpGet("unified")]
    public async Task<IActionResult> UnifiedSearch([FromQuery] SearchQuery request)
    {
        try
        {
            var result = await _searchService.UnifiedSearchAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing unified search");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Search offers with role-based filtering
    /// </summary>
    [HttpGet("offers")]
    public async Task<IActionResult> SearchOffers([FromQuery] SearchQuery request)
    {
        try
        {
            var result = await _searchService.SearchOffersAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching offers");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Search purchases with role-based filtering
    /// </summary>
    [HttpGet("purchases")]
    public async Task<IActionResult> SearchPurchases([FromQuery] SearchQuery request)
    {
        try
        {
            var result = await _searchService.SearchPurchasesAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching purchases");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Search transports with role-based filtering
    /// </summary>
    [HttpGet("transports")]
    public async Task<IActionResult> SearchTransports([FromQuery] SearchQuery request)
    {
        try
        {
            var result = await _searchService.SearchTransportsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching transports");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Autocomplete suggestions
    /// </summary>
    [HttpGet("autocomplete")]
    public async Task<IActionResult> Autocomplete([FromQuery] AutocompleteRequest request)
    {
        try
        {
            var result = await _searchService.AutocompleteAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting autocomplete suggestions");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get aggregated facets for filtering
    /// </summary>
    [HttpGet("aggregations")]
    public async Task<IActionResult> GetAggregations([FromQuery] SearchQuery request)
    {
        try
        {
            // Set page size to 0 to only get aggregations
            request.PageSize = 0;
            var result = await _searchService.SearchOffersAsync(request);
            return Ok(new { facets = result.Facets });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting aggregations");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}
