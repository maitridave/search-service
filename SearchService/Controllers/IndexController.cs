using Microsoft.AspNetCore.Mvc;
using SearchService.Models.EntityModels;
using SearchService.Models.RequestModels;
using SearchService.Models.SearchModels;
using SearchService.Services;

namespace SearchService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IndexController : ControllerBase
{
    private readonly IIndexingService _indexingService;
    private readonly ILogger<IndexController> _logger;

    public IndexController(IIndexingService indexingService, ILogger<IndexController> logger)
    {
        _indexingService = indexingService;
        _logger = logger;
    }

    /// <summary>
    /// Index a single offer document
    /// </summary>
    [HttpPost("offers")]
    public async Task<IActionResult> IndexOffer([FromBody] OfferEntity offer)
    {
        try
        {
            var success = await _indexingService.IndexOfferAsync(offer);
            
            return success 
                ? Ok(new IndexResponse { Success = true, Message = "Offer indexed successfully", DocumentId = offer.OfferId })
                : StatusCode(500, new IndexResponse { Success = false, Message = "Failed to index offer" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing offer");
            return StatusCode(500, new IndexResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Index a single purchase document
    /// </summary>
    [HttpPost("purchases")]
    public async Task<IActionResult> IndexPurchase([FromBody] PurchaseEntity purchase)
    {
        try
        {
            var success = await _indexingService.IndexPurchaseAsync(purchase);
            
            return success 
                ? Ok(new IndexResponse { Success = true, Message = "Purchase indexed successfully", DocumentId = purchase.PurchaseId })
                : StatusCode(500, new IndexResponse { Success = false, Message = "Failed to index purchase" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing purchase");
            return StatusCode(500, new IndexResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Index a single transport document
    /// </summary>
    [HttpPost("transports")]
    public async Task<IActionResult> IndexTransport([FromBody] TransportEntity transport)
    {
        try
        {
            var success = await _indexingService.IndexTransportAsync(transport);
            
            return success 
                ? Ok(new IndexResponse { Success = true, Message = "Transport indexed successfully", DocumentId = transport.TransportId })
                : StatusCode(500, new IndexResponse { Success = false, Message = "Failed to index transport" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing transport");
            return StatusCode(500, new IndexResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Bulk index multiple documents
    /// </summary>
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkIndex([FromBody] BulkIndexRequest request)
    {
        try
        {
            bool success = false;

            switch (request.EntityType.ToLower())
            {
                case "offers":
                    var offers = System.Text.Json.JsonSerializer.Deserialize<List<OfferEntity>>(
                        System.Text.Json.JsonSerializer.Serialize(request.Documents));
                    success = await _indexingService.BulkIndexOffersAsync(offers ?? new List<OfferEntity>());
                    break;

                case "purchases":
                    var purchases = System.Text.Json.JsonSerializer.Deserialize<List<PurchaseEntity>>(
                        System.Text.Json.JsonSerializer.Serialize(request.Documents));
                    success = await _indexingService.BulkIndexPurchasesAsync(purchases ?? new List<PurchaseEntity>());
                    break;

                case "transports":
                    var transports = System.Text.Json.JsonSerializer.Deserialize<List<TransportEntity>>(
                        System.Text.Json.JsonSerializer.Serialize(request.Documents));
                    success = await _indexingService.BulkIndexTransportsAsync(transports ?? new List<TransportEntity>());
                    break;

                default:
                    return BadRequest(new IndexResponse { Success = false, Message = "Invalid entity type" });
            }

            return success 
                ? Ok(new IndexResponse { Success = true, Message = $"Bulk indexed {request.Documents.Count} documents" })
                : StatusCode(500, new IndexResponse { Success = false, Message = "Bulk indexing failed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing bulk index");
            return StatusCode(500, new IndexResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a document by ID and type
    /// </summary>
    [HttpDelete("{type}/{id}")]
    public async Task<IActionResult> DeleteDocument(string type, string id)
    {
        try
        {
            bool success = false;

            switch (type.ToLower())
            {
                case "offers":
                    success = await _indexingService.DeleteOfferAsync(id);
                    break;
                case "purchases":
                    success = await _indexingService.DeletePurchaseAsync(id);
                    break;
                case "transports":
                    success = await _indexingService.DeleteTransportAsync(id);
                    break;
                default:
                    return BadRequest(new IndexResponse { Success = false, Message = "Invalid entity type" });
            }

            return success 
                ? Ok(new IndexResponse { Success = true, Message = "Document deleted successfully" })
                : StatusCode(500, new IndexResponse { Success = false, Message = "Failed to delete document" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document");
            return StatusCode(500, new IndexResponse { Success = false, Message = ex.Message });
        }
    }
}
