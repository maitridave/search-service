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
    /// Universal search across all entity types with optional aggregations
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

    // /// <summary>
    // /// Get aggregated facets for filtering from unified index
    // /// </summary>
    // [HttpGet("aggregations")]
    // public async Task<IActionResult> GetAggregations([FromQuery] SearchQuery request)
    // {
    //     try
    //     {
    //         // Set page size to 0 to only get aggregations
    //         request.PageSize = 0;
    //         var result = await _searchService.UnifiedSearchAsync(request);
    //         return Ok(new { facets = result.Facets });
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting aggregations");
    //         return StatusCode(500, new { error = "Internal server error" });
    //     }
    // }

    /// <summary>
    /// Autocomplete suggestions from unified index
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
}
