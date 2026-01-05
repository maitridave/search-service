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
    /// Universal search API that handles both full search and autocomplete functionality
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchQuery request)
    {
        try
        {
            // If it's an autocomplete request (typically short query with high page size limit)
            if (request.IsAutocomplete || (!string.IsNullOrEmpty(request.Query) && request.Query.Length <= 3 && request.PageSize > 20))
            {
                // Convert to autocomplete request
                var autocompleteRequest = new AutocompleteRequest
                {
                    Term = request.Query,
                    MaxResults = request.PageSize > 0 ? request.PageSize : 10,
                    UserId = request.UserId,
                    UserRole = request.UserRole
                };
                
                var autocompleteResult = await _searchService.AutocompleteAsync(autocompleteRequest);
                return Ok(autocompleteResult);
            }
            
            // Regular unified search
            var result = await _searchService.UnifiedSearchAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}
