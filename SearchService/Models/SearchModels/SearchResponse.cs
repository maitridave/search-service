namespace SearchService.Models.SearchModels;

public class SearchResultResponse<T>
{
    public long TotalHits { get; set; }
    public double MaxScore { get; set; }
    public List<SearchResult<T>> Results { get; set; } = new();
    public Dictionary<string, List<FacetResult>> Facets { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public long ElapsedMilliseconds { get; set; }
}

public class SearchResult<T>
{
    public T Document { get; set; } = default!;
    public double Score { get; set; }
    public Dictionary<string, List<string>> Highlights { get; set; } = new();
}

public class FacetResult
{
    public string Key { get; set; } = string.Empty;
    public long Count { get; set; }
}

public class AutocompleteResponse
{
    public List<AutocompleteSuggestion> Suggestions { get; set; } = new();
}

public class AutocompleteSuggestion
{
    public string Text { get; set; } = string.Empty;
    public double Score { get; set; }
}

public class HealthResponse
{
    public string Status { get; set; } = string.Empty;
    public Dictionary<string, string> Components { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class IndexResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
}
