using Nest;

namespace SearchService.Models.EntityModels;

public class OfferEntity
{
    [Keyword]
    public string OfferId { get; set; } = string.Empty;

    [Keyword]
    public string SellerId { get; set; } = string.Empty;

    [Keyword]
    public string VIN { get; set; } = string.Empty;

    [Text(Analyzer = "standard")]
    public string Make { get; set; } = string.Empty;

    [Text(Analyzer = "standard")]
    public string Model { get; set; } = string.Empty;

    [Number]
    public int Year { get; set; }

    [Number]
    public decimal OfferAmount { get; set; }

    [Object]
    public LocationInfo Location { get; set; } = new();

    [Keyword]
    public string Condition { get; set; } = string.Empty;

    [Keyword]
    public string Status { get; set; } = string.Empty;

    [Date]
    public DateTime CreatedAt { get; set; }

    [Date]
    public DateTime UpdatedAt { get; set; }

    [Completion]
    public CompletionField Suggest { get; set; } = new();
}

public class LocationInfo
{
    [Text]
    public string City { get; set; } = string.Empty;

    [Text]
    public string State { get; set; } = string.Empty;

    [Text]
    public string Country { get; set; } = string.Empty;
}
