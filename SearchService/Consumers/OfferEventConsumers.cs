using MassTransit;
using SearchService.Models.Events;
using SearchService.Services;
using SearchService.Models.EntityModels;

namespace SearchService.Consumers
{
    public class OfferCreatedConsumer : IConsumer<OfferCreatedEvent>
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<OfferCreatedConsumer> _logger;

        public OfferCreatedConsumer(
            IElasticsearchService elasticsearchService,
            ILogger<OfferCreatedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OfferCreatedEvent> context)
        {
            try
            {
                var offerEvent = context.Message;
                _logger.LogInformation("Processing OfferCreated event for Offer ID: {OfferId}", offerEvent.OfferId);

                // Map event to search document
                var searchDocument = new
                {
                    id = $"offer_{offerEvent.OfferId}",
                    entityType = "offer",
                    offerId = offerEvent.OfferId,
                    vehicleId = offerEvent.VehicleId,
                    sellerId = offerEvent.SellerId,
                    buyerId = offerEvent.BuyerId,
                    carrierId = offerEvent.CarrierId,
                    offerAmount = offerEvent.OfferAmount,
                    city = offerEvent.City,
                    state = offerEvent.State,
                    country = offerEvent.Country,
                    status = offerEvent.Status,
                    createdAt = offerEvent.CreatedAt,
                    make = offerEvent.Make,
                    model = offerEvent.Model,
                    year = offerEvent.Year,
                    trim = offerEvent.Trim,
                    vin = offerEvent.VIN,
                    searchableText = $"{offerEvent.Make} {offerEvent.Model} {offerEvent.Year} {offerEvent.VIN} {offerEvent.City} {offerEvent.State}"
                };

                await _elasticsearchService.IndexDocumentAsync("automotive_search", searchDocument);
                _logger.LogInformation("Successfully indexed Offer ID: {OfferId} to Elasticsearch", offerEvent.OfferId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing OfferCreated event for Offer ID: {OfferId}", context.Message?.OfferId);
                throw;
            }
        }
    }

    public class OfferUpdatedConsumer : IConsumer<OfferUpdatedEvent>
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<OfferUpdatedConsumer> _logger;

        public OfferUpdatedConsumer(
            IElasticsearchService elasticsearchService,
            ILogger<OfferUpdatedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OfferUpdatedEvent> context)
        {
            try
            {
                var offerEvent = context.Message;
                _logger.LogInformation("Processing OfferUpdated event for Offer ID: {OfferId}", offerEvent.OfferId);

                var searchDocument = new
                {
                    id = $"offer_{offerEvent.OfferId}",
                    entityType = "offer",
                    offerId = offerEvent.OfferId,
                    vehicleId = offerEvent.VehicleId,
                    sellerId = offerEvent.SellerId,
                    buyerId = offerEvent.BuyerId,
                    carrierId = offerEvent.CarrierId,
                    offerAmount = offerEvent.OfferAmount,
                    city = offerEvent.City,
                    state = offerEvent.State,
                    country = offerEvent.Country,
                    status = offerEvent.Status,
                    createdAt = offerEvent.CreatedAt,
                    lastModifiedAt = offerEvent.LastModifiedAt,
                    make = offerEvent.Make,
                    model = offerEvent.Model,
                    year = offerEvent.Year,
                    trim = offerEvent.Trim,
                    vin = offerEvent.VIN,
                    searchableText = $"{offerEvent.Make} {offerEvent.Model} {offerEvent.Year} {offerEvent.VIN} {offerEvent.City} {offerEvent.State}"
                };

                await _elasticsearchService.UpdateDocumentAsync("automotive_search", $"offer_{offerEvent.OfferId}", searchDocument);
                _logger.LogInformation("Successfully updated Offer ID: {OfferId} in Elasticsearch", offerEvent.OfferId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing OfferUpdated event for Offer ID: {OfferId}", context.Message?.OfferId);
                throw;
            }
        }
    }
}