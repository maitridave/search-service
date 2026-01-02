namespace SearchService.Models.Events
{
    public class OfferCreatedEvent
    {
        public int OfferId { get; set; }
        public int VehicleId { get; set; }
        public int SellerId { get; set; }
        public int? BuyerId { get; set; }
        public int? CarrierId { get; set; }
        public decimal OfferAmount { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string? Trim { get; set; }
        public string VIN { get; set; } = string.Empty;
    }

    public class OfferUpdatedEvent
    {
        public int OfferId { get; set; }
        public int VehicleId { get; set; }
        public int SellerId { get; set; }
        public int? BuyerId { get; set; }
        public int? CarrierId { get; set; }
        public decimal OfferAmount { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string? Trim { get; set; }
        public string VIN { get; set; } = string.Empty;
    }

    public class PurchaseCreatedEvent
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public int OfferId { get; set; }
        public int? TransportId { get; set; }
        public DateTime? AssignedAt { get; set; }
        public decimal BidAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class PurchaseUpdatedEvent
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public int OfferId { get; set; }
        public int? TransportId { get; set; }
        public DateTime? AssignedAt { get; set; }
        public decimal BidAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }

    public class TransportCreatedEvent
    {
        public int Id { get; set; }
        public int CarrierId { get; set; }
        public int PurchaseId { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string DeliveryLocation { get; set; } = string.Empty;
        public DateTime? ScheduledDate { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public string CarrierPhone { get; set; } = string.Empty;
        public decimal TransportFee { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public int EstimatedDays { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class TransportUpdatedEvent
    {
        public int Id { get; set; }
        public int CarrierId { get; set; }
        public int PurchaseId { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string DeliveryLocation { get; set; } = string.Empty;
        public DateTime? ScheduledDate { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public string CarrierPhone { get; set; } = string.Empty;
        public decimal TransportFee { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public int EstimatedDays { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }
}