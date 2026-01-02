using MassTransit;
using SearchService.Models.Events;
using SearchService.Services;

namespace SearchService.Consumers
{
    public class PurchaseCreatedConsumer : IConsumer<PurchaseCreatedEvent>
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<PurchaseCreatedConsumer> _logger;

        public PurchaseCreatedConsumer(
            IElasticsearchService elasticsearchService,
            ILogger<PurchaseCreatedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PurchaseCreatedEvent> context)
        {
            try
            {
                var purchaseEvent = context.Message;
                _logger.LogInformation("Processing PurchaseCreated event for Purchase ID: {PurchaseId}", purchaseEvent.Id);

                var searchDocument = new
                {
                    id = $"purchase_{purchaseEvent.Id}",
                    entityType = "purchase",
                    purchaseId = purchaseEvent.Id,
                    buyerId = purchaseEvent.BuyerId,
                    offerId = purchaseEvent.OfferId,
                    transportId = purchaseEvent.TransportId,
                    assignedAt = purchaseEvent.AssignedAt,
                    bidAmount = purchaseEvent.BidAmount,
                    status = purchaseEvent.Status,
                    createdAt = purchaseEvent.CreatedAt,
                    searchableText = $"purchase {purchaseEvent.Id} offer {purchaseEvent.OfferId} buyer {purchaseEvent.BuyerId}"
                };

                await _elasticsearchService.IndexDocumentAsync("automotive_search", searchDocument);
                _logger.LogInformation("Successfully indexed Purchase ID: {PurchaseId} to Elasticsearch", purchaseEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PurchaseCreated event for Purchase ID: {PurchaseId}", context.Message?.Id);
                throw;
            }
        }
    }

    public class PurchaseUpdatedConsumer : IConsumer<PurchaseUpdatedEvent>
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<PurchaseUpdatedConsumer> _logger;

        public PurchaseUpdatedConsumer(
            IElasticsearchService elasticsearchService,
            ILogger<PurchaseUpdatedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PurchaseUpdatedEvent> context)
        {
            try
            {
                var purchaseEvent = context.Message;
                _logger.LogInformation("Processing PurchaseUpdated event for Purchase ID: {PurchaseId}", purchaseEvent.Id);

                var searchDocument = new
                {
                    id = $"purchase_{purchaseEvent.Id}",
                    entityType = "purchase",
                    purchaseId = purchaseEvent.Id,
                    buyerId = purchaseEvent.BuyerId,
                    offerId = purchaseEvent.OfferId,
                    transportId = purchaseEvent.TransportId,
                    assignedAt = purchaseEvent.AssignedAt,
                    bidAmount = purchaseEvent.BidAmount,
                    status = purchaseEvent.Status,
                    createdAt = purchaseEvent.CreatedAt,
                    lastModifiedAt = purchaseEvent.LastModifiedAt,
                    searchableText = $"purchase {purchaseEvent.Id} offer {purchaseEvent.OfferId} buyer {purchaseEvent.BuyerId}"
                };

                await _elasticsearchService.UpdateDocumentAsync("automotive_search", $"purchase_{purchaseEvent.Id}", searchDocument);
                _logger.LogInformation("Successfully updated Purchase ID: {PurchaseId} in Elasticsearch", purchaseEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PurchaseUpdated event for Purchase ID: {PurchaseId}", context.Message?.Id);
                throw;
            }
        }
    }
}