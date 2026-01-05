using Nest;

namespace SearchService.Models.Documents;

[ElasticsearchType(IdProperty = nameof(DocumentId))]
public class AutomotiveSearchDocument
{
    [Keyword]
    public string DocumentId { get; set; } = string.Empty;
    
    [Keyword]
    public string EntityType { get; set; } = string.Empty; // "offer", "purchase", "transport"
    
    // Common IDs
    [Number]
    public int? SellerId { get; set; }
    
    [Number]
    public int? BuyerId { get; set; }
    
    [Number]
    public int? CarrierId { get; set; }
    
    [Number]
    public int? PurchaseId { get; set; }
    
    [Number]
    public int? TransportId { get; set; }
    
    [Number]
    public int? OfferId { get; set; }
    
    // Vehicle Information
    [Keyword]
    public string VIN { get; set; } = string.Empty;
    
    [Text(Analyzer = "autocomplete_analyzer")]
    public string Make { get; set; } = string.Empty;
    
    [Text(Analyzer = "autocomplete_analyzer")]
    public string Model { get; set; } = string.Empty;
    
    [Text(Analyzer = "autocomplete_analyzer")]
    public string Trim { get; set; } = string.Empty;
    
    [Number]
    public int Year { get; set; }
    
    // Financial Information
    [Number]
    public decimal? OfferAmount { get; set; }
    
    [Number]
    public decimal? BidAmount { get; set; }
    
    // Status Fields
    [Keyword]
    public string? OfferStatus { get; set; }
    
    [Keyword]
    public string? PurchaseStatus { get; set; }
    
    [Keyword]
    public string? TransportStatus { get; set; }
    
    // Location Information
    [Text(Analyzer = "autocomplete_analyzer")]
    public string City { get; set; } = string.Empty;
    
    [Keyword]
    public string State { get; set; } = string.Empty;
    
    // Search Text for comprehensive searching
    [Text(Analyzer = "autocomplete_analyzer")]
    public string SearchText { get; set; } = string.Empty;
    
    // Timestamps
    [Date]
    public DateTime CreatedAt { get; set; }
    
    [Date]
    public DateTime UpdatedAt { get; set; }
    
    [Date]
    public DateTime LastIndexed { get; set; } = DateTime.UtcNow;
}

public static class EntityTypes
{
    public const string Offer = "offer";
    public const string Purchase = "purchase";
    public const string Transport = "transport";
}
