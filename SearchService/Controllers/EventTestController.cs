using Microsoft.AspNetCore.Mvc;
using SearchService.Models.Events;
using SearchService.EventHandlers;

namespace SearchService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventTestController : ControllerBase
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<EventTestController> _logger;

    public EventTestController(IEventPublisher eventPublisher, ILogger<EventTestController> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    /// <summary>
    /// Test endpoint to publish offer events
    /// </summary>
    [HttpPost("offer")]
    public async Task<IActionResult> PublishOfferEvent([FromBody] OfferEvent offerEvent)
    {
        try
        {
            offerEvent.Source = "test-service";
            offerEvent.EventTimestamp = DateTime.UtcNow;
            
            await _eventPublisher.PublishAsync(offerEvent, offerEvent.EventType);
            
            return Ok(new { message = "Offer event published successfully", eventId = offerEvent.EventId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing offer event");
            return StatusCode(500, new { error = "Failed to publish event" });
        }
    }

    /// <summary>
    /// Test endpoint to publish purchase events
    /// </summary>
    [HttpPost("purchase")]
    public async Task<IActionResult> PublishPurchaseEvent([FromBody] PurchaseEvent purchaseEvent)
    {
        try
        {
            purchaseEvent.Source = "test-service";
            purchaseEvent.EventTimestamp = DateTime.UtcNow;
            
            await _eventPublisher.PublishAsync(purchaseEvent, purchaseEvent.EventType);
            
            return Ok(new { message = "Purchase event published successfully", eventId = purchaseEvent.EventId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing purchase event");
            return StatusCode(500, new { error = "Failed to publish event" });
        }
    }

    /// <summary>
    /// Test endpoint to publish transport events
    /// </summary>
    [HttpPost("transport")]
    public async Task<IActionResult> PublishTransportEvent([FromBody] TransportEvent transportEvent)
    {
        try
        {
            transportEvent.Source = "test-service";
            transportEvent.EventTimestamp = DateTime.UtcNow;
            
            await _eventPublisher.PublishAsync(transportEvent, transportEvent.EventType);
            
            return Ok(new { message = "Transport event published successfully", eventId = transportEvent.EventId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing transport event");
            return StatusCode(500, new { error = "Failed to publish event" });
        }
    }

    /// <summary>
    /// Bulk test data generator
    /// </summary>
    [HttpPost("generate-test-data")]
    public async Task<IActionResult> GenerateTestData([FromQuery] int count = 10)
    {
        try
        {
            var random = new Random();
            var makes = new[] { "Toyota", "Honda", "Ford", "Chevrolet", "BMW", "Audi", "Mercedes", "Nissan", "Hyundai", "Kia" };
            var models = new[] { "Sedan", "SUV", "Truck", "Coupe", "Hatchback", "Wagon" };
            var cities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose" };
            var states = new[] { "NY", "CA", "IL", "TX", "AZ", "PA", "TX", "CA", "TX", "CA" };

            for (int i = 1; i <= count; i++)
            {
                var make = makes[random.Next(makes.Length)];
                var model = models[random.Next(models.Length)];
                var year = random.Next(2015, 2025);
                var vin = GenerateVIN();
                var city = cities[random.Next(cities.Length)];
                var state = states[random.Next(states.Length)];

                // Create offer event
                var offerEvent = new OfferEvent
                {
                    EventType = EventTypes.OfferCreated,
                    Source = "test-service",
                    OfferId = i,
                    SellerId = random.Next(100, 1000),
                    VIN = vin,
                    Make = make,
                    Model = model,
                    Trim = "Standard",
                    Year = year,
                    OfferAmount = random.Next(15000, 50000),
                    OfferStatus = "available",
                    City = city,
                    State = state,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                    UpdatedAt = DateTime.UtcNow
                };

                await _eventPublisher.PublishAsync(offerEvent, offerEvent.EventType);

                // Sometimes create purchase and transport events
                if (random.Next(1, 4) == 1) // 33% chance
                {
                    var purchaseEvent = new PurchaseEvent
                    {
                        EventType = EventTypes.PurchaseCreated,
                        Source = "test-service",
                        PurchaseId = i,
                        BuyerId = random.Next(1000, 2000),
                        OfferId = i,
                        SellerId = offerEvent.SellerId,
                        VIN = vin,
                        Make = make,
                        Model = model,
                        Trim = "Standard",
                        Year = year,
                        BidAmount = offerEvent.OfferAmount - random.Next(1000, 3000),
                        PurchaseStatus = "completed",
                        City = city,
                        State = state,
                        PurchaseDate = DateTime.UtcNow.AddDays(-random.Next(0, 15)),
                        CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 15)),
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _eventPublisher.PublishAsync(purchaseEvent, purchaseEvent.EventType);

                    // Create transport event
                    var transportEvent = new TransportEvent
                    {
                        EventType = EventTypes.TransportCreated,
                        Source = "test-service",
                        TransportId = i,
                        CarrierId = random.Next(2000, 3000),
                        PurchaseId = i,
                        BuyerId = purchaseEvent.BuyerId,
                        SellerId = offerEvent.SellerId,
                        OfferId = i,
                        VIN = vin,
                        Make = make,
                        Model = model,
                        Trim = "Standard",
                        Year = year,
                        BidAmount = purchaseEvent.BidAmount,
                        TransportStatus = "in_progress",
                        PickupCity = city,
                        PickupState = state,
                        DeliveryCity = cities[random.Next(cities.Length)],
                        DeliveryState = states[random.Next(states.Length)],
                        ScheduleDate = DateTime.UtcNow.AddDays(random.Next(1, 10)),
                        CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 10)),
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _eventPublisher.PublishAsync(transportEvent, transportEvent.EventType);
                }

                // Small delay to prevent overwhelming
                await Task.Delay(10);
            }

            return Ok(new { message = $"Generated {count} test events successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating test data");
            return StatusCode(500, new { error = "Failed to generate test data" });
        }
    }

    private string GenerateVIN()
    {
        const string chars = "ABCDEFGHJKLMNPRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 17)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
