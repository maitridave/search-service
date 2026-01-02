using Nest;

namespace SearchService.Models.EntityModels;

public class PurchaseEntity
{
    [Keyword]
    public string PurchaseId { get; set; } = string.Empty;

    [Keyword]
    public string BuyerId { get; set; } = string.Empty;

    [Keyword]
    public string OfferId { get; set; } = string.Empty;

    [Date]
    public DateTime PurchaseDate { get; set; }

    [Number]
    public decimal Amount { get; set; }

    [Keyword]
    public string Status { get; set; } = string.Empty;

    [Object]
    public BuyerDetails BuyerDetails { get; set; } = new();

    [Keyword]
    public string PaymentMethod { get; set; } = string.Empty;

    [Date]
    public DateTime CreatedAt { get; set; }

    [Date]
    public DateTime UpdatedAt { get; set; }

    [Completion]
    public CompletionField Suggest { get; set; } = new();
}

public class BuyerDetails
{
    [Text]
    public string Name { get; set; } = string.Empty;

    [Keyword]
    public string Contact { get; set; } = string.Empty;
}
