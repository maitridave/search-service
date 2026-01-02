namespace SearchService.Models.RequestModels;

public class SearchQuery
{
    public string Query { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<string> EntityTypes { get; set; } = new();
    public Dictionary<string, string> Filters { get; set; } = new();
    public bool EnableFuzzy { get; set; } = true;
    public int FuzzinessLevel { get; set; } = 1;
    public bool EnableHighlighting { get; set; } = true;
}

public class AutocompleteRequest
{
    public string Query { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int Size { get; set; } = 5;
}

public class BulkIndexRequest
{
    public string EntityType { get; set; } = string.Empty;
    public List<object> Documents { get; set; } = new();
}

public enum UserRole
{
    Seller,
    Buyer,
    Carrier,
    Agent
}
