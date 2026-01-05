namespace SearchService.Models.Events;

public abstract class BaseEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime EventTimestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
}

public class OfferEvent : BaseEvent
{
    public int OfferId { get; set; }
    public int SellerId { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Trim { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal OfferAmount { get; set; }
    public string OfferStatus { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PurchaseEvent : BaseEvent
{
    public int PurchaseId { get; set; }
    public int BuyerId { get; set; }
    public int OfferId { get; set; }
    public int SellerId { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Trim { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal BidAmount { get; set; }
    public string PurchaseStatus { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class TransportEvent : BaseEvent
{
    public int TransportId { get; set; }
    public int CarrierId { get; set; }
    public int PurchaseId { get; set; }
    public int BuyerId { get; set; }
    public int SellerId { get; set; }
    public int OfferId { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Trim { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal BidAmount { get; set; }
    public string TransportStatus { get; set; } = string.Empty;
    public string PickupCity { get; set; } = string.Empty;
    public string PickupState { get; set; } = string.Empty;
    public string DeliveryCity { get; set; } = string.Empty;
    public string DeliveryState { get; set; } = string.Empty;
    public DateTime ScheduleDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public static class EventTypes
{
    public const string OfferCreated = "offer.created";
    public const string OfferUpdated = "offer.updated";
    public const string OfferDeleted = "offer.deleted";
    
    public const string PurchaseCreated = "purchase.created";
    public const string PurchaseUpdated = "purchase.updated";
    public const string PurchaseDeleted = "purchase.deleted";
    
    public const string TransportCreated = "transport.created";
    public const string TransportUpdated = "transport.updated";
    public const string TransportDeleted = "transport.deleted";
}
