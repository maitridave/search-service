using MassTransit;
using SearchService.Models.Events;
using SearchService.Services;

namespace SearchService.Consumers
{
    public class TransportCreatedConsumer : IConsumer<TransportCreatedEvent>
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<TransportCreatedConsumer> _logger;

        public TransportCreatedConsumer(
            IElasticsearchService elasticsearchService,
            ILogger<TransportCreatedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TransportCreatedEvent> context)
        {
            try
            {
                var transportEvent = context.Message;
                _logger.LogInformation("Processing TransportCreated event for Transport ID: {TransportId}", transportEvent.Id);

                var searchDocument = new
                {
                    id = $"transport_{transportEvent.Id}",
                    entityType = "transport",
                    transportId = transportEvent.Id,
                    carrierId = transportEvent.CarrierId,
                    purchaseId = transportEvent.PurchaseId,
                    pickupLocation = transportEvent.PickupLocation,
                    deliveryLocation = transportEvent.DeliveryLocation,
                    scheduledDate = transportEvent.ScheduledDate,
                    carrierName = transportEvent.CarrierName,
                    carrierPhone = transportEvent.CarrierPhone,
                    transportFee = transportEvent.TransportFee,
                    vehicleType = transportEvent.VehicleType,
                    estimatedDays = transportEvent.EstimatedDays,
                    status = transportEvent.Status,
                    createdAt = transportEvent.CreatedAt,
                    searchableText = $"transport {transportEvent.Id} carrier {transportEvent.CarrierName} {transportEvent.PickupLocation} {transportEvent.DeliveryLocation}"
                };

                await _elasticsearchService.IndexDocumentAsync("automotive_search", searchDocument);
                _logger.LogInformation("Successfully indexed Transport ID: {TransportId} to Elasticsearch", transportEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TransportCreated event for Transport ID: {TransportId}", context.Message?.Id);
                throw;
            }
        }
    }

    public class TransportUpdatedConsumer : IConsumer<TransportUpdatedEvent>
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<TransportUpdatedConsumer> _logger;

        public TransportUpdatedConsumer(
            IElasticsearchService elasticsearchService,
            ILogger<TransportUpdatedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TransportUpdatedEvent> context)
        {
            try
            {
                var transportEvent = context.Message;
                _logger.LogInformation("Processing TransportUpdated event for Transport ID: {TransportId}", transportEvent.Id);

                var searchDocument = new
                {
                    id = $"transport_{transportEvent.Id}",
                    entityType = "transport",
                    transportId = transportEvent.Id,
                    carrierId = transportEvent.CarrierId,
                    purchaseId = transportEvent.PurchaseId,
                    pickupLocation = transportEvent.PickupLocation,
                    deliveryLocation = transportEvent.DeliveryLocation,
                    scheduledDate = transportEvent.ScheduledDate,
                    carrierName = transportEvent.CarrierName,
                    carrierPhone = transportEvent.CarrierPhone,
                    transportFee = transportEvent.TransportFee,
                    vehicleType = transportEvent.VehicleType,
                    estimatedDays = transportEvent.EstimatedDays,
                    status = transportEvent.Status,
                    createdAt = transportEvent.CreatedAt,
                    lastModifiedAt = transportEvent.LastModifiedAt,
                    searchableText = $"transport {transportEvent.Id} carrier {transportEvent.CarrierName} {transportEvent.PickupLocation} {transportEvent.DeliveryLocation}"
                };

                await _elasticsearchService.UpdateDocumentAsync("automotive_search", $"transport_{transportEvent.Id}", searchDocument);
                _logger.LogInformation("Successfully updated Transport ID: {TransportId} in Elasticsearch", transportEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TransportUpdated event for Transport ID: {TransportId}", context.Message?.Id);
                throw;
            }
        }
    }
}