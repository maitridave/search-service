using Nest;
using SearchService.Models.Events;
using SearchService.Models.Documents;

namespace SearchService.Services;

public interface IIndexingService
{
    Task<bool> IndexDocumentAsync<T>(T document, string indexName) where T : class;
    Task<bool> BulkIndexAsync<T>(IEnumerable<T> documents, string indexName) where T : class;
    Task<bool> DeleteDocumentAsync(string documentId, string indexName);
    Task<bool> ProcessOfferEvent(OfferEvent offerEvent);
    Task<bool> ProcessPurchaseEvent(PurchaseEvent purchaseEvent);
    Task<bool> ProcessTransportEvent(TransportEvent transportEvent);
}

public class IndexingService : IIndexingService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<IndexingService> _logger;
    private const string IndexName = "automotive_search";

    public IndexingService(IElasticClient elasticClient, ILogger<IndexingService> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }

    public async Task<bool> IndexDocumentAsync<T>(T document, string indexName) where T : class
    {
        try
        {
            var response = await _elasticClient.IndexDocumentAsync(document);
            
            if (response.IsValid)
            {
                _logger.LogInformation("Document indexed successfully in {IndexName}", indexName);
                return true;
            }
            
            _logger.LogError("Failed to index document: {Error}", response.DebugInformation);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing document");
            return false;
        }
    }

    public async Task<bool> BulkIndexAsync<T>(IEnumerable<T> documents, string indexName) where T : class
    {
        try
        {
            var bulkDescriptor = new BulkDescriptor();
            
            foreach (var document in documents)
            {
                bulkDescriptor.Index<T>(i => i
                    .Index(indexName)
                    .Document(document)
                );
            }
            
            var response = await _elasticClient.BulkAsync(bulkDescriptor);
            
            if (response.IsValid)
            {
                _logger.LogInformation("Bulk indexed {Count} documents successfully", documents.Count());
                return true;
            }
            
            _logger.LogError("Failed to bulk index documents: {Error}", response.DebugInformation);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk indexing documents");
            return false;
        }
    }

    public async Task<bool> DeleteDocumentAsync(string documentId, string indexName)
    {
        try
        {
            var response = await _elasticClient.DeleteAsync<AutomotiveSearchDocument>(documentId, d => d.Index(indexName));
            
            if (response.IsValid)
            {
                _logger.LogInformation("Document {DocumentId} deleted successfully from {IndexName}", documentId, indexName);
                return true;
            }
            
            _logger.LogWarning("Failed to delete document {DocumentId}: {Error}", documentId, response.DebugInformation);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId}", documentId);
            return false;
        }
    }

    public async Task<bool> ProcessOfferEvent(OfferEvent offerEvent)
    {
        try
        {
            _logger.LogInformation("Processing offer event: {EventType} for Offer {OfferId}", offerEvent.EventType, offerEvent.OfferId);

            if (offerEvent.EventType == EventTypes.OfferDeleted)
            {
                var documentId = $"offer_{offerEvent.OfferId}";
                return await DeleteDocumentAsync(documentId, IndexName);
            }

            var document = new AutomotiveSearchDocument
            {
                DocumentId = $"offer_{offerEvent.OfferId}",
                EntityType = EntityTypes.Offer,
                OfferId = offerEvent.OfferId,
                SellerId = offerEvent.SellerId,
                VIN = offerEvent.VIN,
                Make = offerEvent.Make,
                Model = offerEvent.Model,
                Trim = offerEvent.Trim,
                Year = offerEvent.Year,
                OfferAmount = offerEvent.OfferAmount,
                OfferStatus = offerEvent.OfferStatus,
                City = offerEvent.City,
                State = offerEvent.State,
                CreatedAt = offerEvent.CreatedAt,
                UpdatedAt = offerEvent.UpdatedAt,
                SearchText = GenerateSearchText(offerEvent),
                LastIndexed = DateTime.UtcNow
            };

            var response = await _elasticClient.IndexAsync(document, i => i
                .Index(IndexName)
                .Id(document.DocumentId)
            );

            if (response.IsValid)
            {
                _logger.LogInformation("Offer document indexed successfully: {DocumentId}", document.DocumentId);
                return true;
            }

            _logger.LogError("Failed to index offer document: {Error}", response.DebugInformation);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing offer event for Offer {OfferId}", offerEvent.OfferId);
            return false;
        }
    }

    public async Task<bool> ProcessPurchaseEvent(PurchaseEvent purchaseEvent)
    {
        try
        {
            _logger.LogInformation("Processing purchase event: {EventType} for Purchase {PurchaseId}", purchaseEvent.EventType, purchaseEvent.PurchaseId);

            if (purchaseEvent.EventType == EventTypes.PurchaseDeleted)
            {
                var documentId = $"purchase_{purchaseEvent.PurchaseId}";
                return await DeleteDocumentAsync(documentId, IndexName);
            }

            var document = new AutomotiveSearchDocument
            {
                DocumentId = $"purchase_{purchaseEvent.PurchaseId}",
                EntityType = EntityTypes.Purchase,
                PurchaseId = purchaseEvent.PurchaseId,
                BuyerId = purchaseEvent.BuyerId,
                OfferId = purchaseEvent.OfferId,
                SellerId = purchaseEvent.SellerId,
                VIN = purchaseEvent.VIN,
                Make = purchaseEvent.Make,
                Model = purchaseEvent.Model,
                Trim = purchaseEvent.Trim,
                Year = purchaseEvent.Year,
                BidAmount = purchaseEvent.BidAmount,
                PurchaseStatus = purchaseEvent.PurchaseStatus,
                City = purchaseEvent.City,
                State = purchaseEvent.State,
                CreatedAt = purchaseEvent.CreatedAt,
                UpdatedAt = purchaseEvent.UpdatedAt,
                SearchText = GenerateSearchText(purchaseEvent),
                LastIndexed = DateTime.UtcNow
            };

            var response = await _elasticClient.IndexAsync(document, i => i
                .Index(IndexName)
                .Id(document.DocumentId)
            );

            if (response.IsValid)
            {
                _logger.LogInformation("Purchase document indexed successfully: {DocumentId}", document.DocumentId);
                return true;
            }

            _logger.LogError("Failed to index purchase document: {Error}", response.DebugInformation);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing purchase event for Purchase {PurchaseId}", purchaseEvent.PurchaseId);
            return false;
        }
    }

    public async Task<bool> ProcessTransportEvent(TransportEvent transportEvent)
    {
        try
        {
            _logger.LogInformation("Processing transport event: {EventType} for Transport {TransportId}", transportEvent.EventType, transportEvent.TransportId);

            if (transportEvent.EventType == EventTypes.TransportDeleted)
            {
                var documentId = $"transport_{transportEvent.TransportId}";
                return await DeleteDocumentAsync(documentId, IndexName);
            }

            var cityInfo = $"{transportEvent.PickupCity} to {transportEvent.DeliveryCity}";
            var stateInfo = $"{transportEvent.PickupState} to {transportEvent.DeliveryState}";

            var document = new AutomotiveSearchDocument
            {
                DocumentId = $"transport_{transportEvent.TransportId}",
                EntityType = EntityTypes.Transport,
                TransportId = transportEvent.TransportId,
                CarrierId = transportEvent.CarrierId,
                PurchaseId = transportEvent.PurchaseId,
                BuyerId = transportEvent.BuyerId,
                SellerId = transportEvent.SellerId,
                OfferId = transportEvent.OfferId,
                VIN = transportEvent.VIN,
                Make = transportEvent.Make,
                Model = transportEvent.Model,
                Trim = transportEvent.Trim,
                Year = transportEvent.Year,
                BidAmount = transportEvent.BidAmount,
                TransportStatus = transportEvent.TransportStatus,
                City = cityInfo,
                State = stateInfo,
                CreatedAt = transportEvent.CreatedAt,
                UpdatedAt = transportEvent.UpdatedAt,
                SearchText = GenerateSearchText(transportEvent),
                LastIndexed = DateTime.UtcNow
            };

            var response = await _elasticClient.IndexAsync(document, i => i
                .Index(IndexName)
                .Id(document.DocumentId)
            );

            if (response.IsValid)
            {
                _logger.LogInformation("Transport document indexed successfully: {DocumentId}", document.DocumentId);
                return true;
            }

            _logger.LogError("Failed to index transport document: {Error}", response.DebugInformation);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing transport event for Transport {TransportId}", transportEvent.TransportId);
            return false;
        }
    }

    private string GenerateSearchText(OfferEvent offerEvent)
    {
        return $"{offerEvent.Year} {offerEvent.Make} {offerEvent.Model} {offerEvent.Trim} {offerEvent.VIN} offer vehicle {offerEvent.City} {offerEvent.State} {offerEvent.OfferStatus}".Trim();
    }

    private string GenerateSearchText(PurchaseEvent purchaseEvent)
    {
        return $"{purchaseEvent.Year} {purchaseEvent.Make} {purchaseEvent.Model} {purchaseEvent.Trim} {purchaseEvent.VIN} purchase bought {purchaseEvent.City} {purchaseEvent.State} {purchaseEvent.PurchaseStatus}".Trim();
    }

    private string GenerateSearchText(TransportEvent transportEvent)
    {
        return $"{transportEvent.Year} {transportEvent.Make} {transportEvent.Model} {transportEvent.Trim} {transportEvent.VIN} transport delivery shipping {transportEvent.PickupCity} {transportEvent.DeliveryCity} {transportEvent.TransportStatus}".Trim();
    }
}
