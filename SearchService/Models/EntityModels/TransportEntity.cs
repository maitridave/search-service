using Nest;

namespace SearchService.Models.EntityModels;

public class TransportEntity
{
    [Keyword]
    public string TransportId { get; set; } = string.Empty;

    [Keyword]
    public string CarrierId { get; set; } = string.Empty;

    [Keyword]
    public string PurchaseId { get; set; } = string.Empty;

    [Text]
    public string PickupLocation { get; set; } = string.Empty;

    [Text]
    public string DeliveryLocation { get; set; } = string.Empty;

    [Date]
    public DateTime ScheduleDate { get; set; }

    [Date]
    public DateTime? ActualPickupDate { get; set; }

    [Date]
    public DateTime? ExpectedDeliveryDate { get; set; }

    [Keyword]
    public string Status { get; set; } = string.Empty;

    [Object]
    public VehicleDetails VehicleDetails { get; set; } = new();

    [Date]
    public DateTime CreatedAt { get; set; }

    [Date]
    public DateTime UpdatedAt { get; set; }

    [Completion]
    public CompletionField Suggest { get; set; } = new();
}

public class VehicleDetails
{
    [Keyword]
    public string VIN { get; set; } = string.Empty;

    [Text]
    public string Make { get; set; } = string.Empty;

    [Text]
    public string Model { get; set; } = string.Empty;

    [Number]
    public int Year { get; set; }
}
