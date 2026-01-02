using SearchService.Models.EntityModels;

namespace SearchService.Services;

public interface IIndexingService
{
    Task<bool> IndexOfferAsync(OfferEntity offer);
    Task<bool> IndexPurchaseAsync(PurchaseEntity purchase);
    Task<bool> IndexTransportAsync(TransportEntity transport);
    Task<bool> BulkIndexOffersAsync(IEnumerable<OfferEntity> offers);
    Task<bool> BulkIndexPurchasesAsync(IEnumerable<PurchaseEntity> purchases);
    Task<bool> BulkIndexTransportsAsync(IEnumerable<TransportEntity> transports);
    Task<bool> DeleteOfferAsync(string offerId);
    Task<bool> DeletePurchaseAsync(string purchaseId);
    Task<bool> DeleteTransportAsync(string transportId);
}

public class IndexingService : IIndexingService
{
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<IndexingService> _logger;

    public IndexingService(IElasticsearchService elasticsearchService, ILogger<IndexingService> logger)
    {
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    public async Task<bool> IndexOfferAsync(OfferEntity offer)
    {
        try
        {
            // Prepare suggestion field
            offer.Suggest = new Nest.CompletionField
            {
                Input = new[] { offer.Make, offer.Model, offer.VIN, $"{offer.Make} {offer.Model}" }
            };

            return await _elasticsearchService.IndexDocumentAsync(offer, "offers");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing offer {OfferId}", offer.OfferId);
            return false;
        }
    }

    public async Task<bool> IndexPurchaseAsync(PurchaseEntity purchase)
    {
        try
        {
            // Prepare suggestion field
            purchase.Suggest = new Nest.CompletionField
            {
                Input = new[] { purchase.PurchaseId, purchase.BuyerDetails.Name }
            };

            return await _elasticsearchService.IndexDocumentAsync(purchase, "purchases");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing purchase {PurchaseId}", purchase.PurchaseId);
            return false;
        }
    }

    public async Task<bool> IndexTransportAsync(TransportEntity transport)
    {
        try
        {
            // Prepare suggestion field
            transport.Suggest = new Nest.CompletionField
            {
                Input = new[] 
                { 
                    transport.TransportId, 
                    transport.VehicleDetails.VIN,
                    transport.PickupLocation,
                    transport.DeliveryLocation
                }
            };

            return await _elasticsearchService.IndexDocumentAsync(transport, "transports");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing transport {TransportId}", transport.TransportId);
            return false;
        }
    }

    public async Task<bool> BulkIndexOffersAsync(IEnumerable<OfferEntity> offers)
    {
        try
        {
            // Prepare suggestion fields for all offers
            foreach (var offer in offers)
            {
                offer.Suggest = new Nest.CompletionField
                {
                    Input = new[] { offer.Make, offer.Model, offer.VIN, $"{offer.Make} {offer.Model}" }
                };
            }

            return await _elasticsearchService.BulkIndexDocumentsAsync(offers, "offers");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk indexing offers");
            return false;
        }
    }

    public async Task<bool> BulkIndexPurchasesAsync(IEnumerable<PurchaseEntity> purchases)
    {
        try
        {
            // Prepare suggestion fields for all purchases
            foreach (var purchase in purchases)
            {
                purchase.Suggest = new Nest.CompletionField
                {
                    Input = new[] { purchase.PurchaseId, purchase.BuyerDetails.Name }
                };
            }

            return await _elasticsearchService.BulkIndexDocumentsAsync(purchases, "purchases");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk indexing purchases");
            return false;
        }
    }

    public async Task<bool> BulkIndexTransportsAsync(IEnumerable<TransportEntity> transports)
    {
        try
        {
            // Prepare suggestion fields for all transports
            foreach (var transport in transports)
            {
                transport.Suggest = new Nest.CompletionField
                {
                    Input = new[] 
                    { 
                        transport.TransportId, 
                        transport.VehicleDetails.VIN,
                        transport.PickupLocation,
                        transport.DeliveryLocation
                    }
                };
            }

            return await _elasticsearchService.BulkIndexDocumentsAsync(transports, "transports");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk indexing transports");
            return false;
        }
    }

    public async Task<bool> DeleteOfferAsync(string offerId)
    {
        return await _elasticsearchService.DeleteDocumentAsync(offerId, "offers");
    }

    public async Task<bool> DeletePurchaseAsync(string purchaseId)
    {
        return await _elasticsearchService.DeleteDocumentAsync(purchaseId, "purchases");
    }

    public async Task<bool> DeleteTransportAsync(string transportId)
    {
        return await _elasticsearchService.DeleteDocumentAsync(transportId, "transports");
    }
}
