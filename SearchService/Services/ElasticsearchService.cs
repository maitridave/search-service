using Nest;
using SearchService.Models.EntityModels;

namespace SearchService.Services;

public interface IElasticsearchService
{
    Task<bool> IndexDocumentAsync<T>(T document, string index) where T : class;
    Task<bool> BulkIndexDocumentsAsync<T>(IEnumerable<T> documents, string index) where T : class;
    Task<bool> DeleteDocumentAsync(string id, string index);
    Task<T?> GetDocumentAsync<T>(string id, string index) where T : class;
    Task<bool> UpdateDocumentAsync<T>(string id, T document, string index) where T : class;
}

public class ElasticsearchService : IElasticsearchService
{
    private readonly IElasticClient _client;
    private readonly ILogger<ElasticsearchService> _logger;

    public ElasticsearchService(IElasticClient client, ILogger<ElasticsearchService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<bool> IndexDocumentAsync<T>(T document, string index) where T : class
    {
        try
        {
            var response = await _client.IndexAsync(document, idx => idx.Index(index));
            
            if (!response.IsValid)
            {
                _logger.LogError("Failed to index document: {Error}", response.DebugInformation);
                return false;
            }

            _logger.LogInformation("Document indexed successfully in {Index}", index);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing document in {Index}", index);
            return false;
        }
    }

    public async Task<bool> BulkIndexDocumentsAsync<T>(IEnumerable<T> documents, string index) where T : class
    {
        try
        {
            var bulkDescriptor = new BulkDescriptor();

            foreach (var document in documents)
            {
                bulkDescriptor.Index<T>(i => i
                    .Index(index)
                    .Document(document)
                );
            }

            var response = await _client.BulkAsync(bulkDescriptor);

            if (!response.IsValid || response.Errors)
            {
                _logger.LogError("Bulk indexing failed: {Error}", response.DebugInformation);
                return false;
            }

            _logger.LogInformation("Bulk indexed {Count} documents in {Index}", documents.Count(), index);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk indexing documents in {Index}", index);
            return false;
        }
    }

    public async Task<bool> DeleteDocumentAsync(string id, string index)
    {
        try
        {
            var response = await _client.DeleteAsync(new DocumentPath<object>(id), d => d.Index(index));

            if (!response.IsValid)
            {
                _logger.LogError("Failed to delete document {Id} from {Index}: {Error}", id, index, response.DebugInformation);
                return false;
            }

            _logger.LogInformation("Document {Id} deleted from {Index}", id, index);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {Id} from {Index}", id, index);
            return false;
        }
    }

    public async Task<T?> GetDocumentAsync<T>(string id, string index) where T : class
    {
        try
        {
            var response = await _client.GetAsync<T>(id, g => g.Index(index));

            if (!response.IsValid || !response.Found)
            {
                _logger.LogWarning("Document {Id} not found in {Index}", id, index);
                return null;
            }

            return response.Source;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document {Id} from {Index}", id, index);
            return null;
        }
    }

    public async Task<bool> UpdateDocumentAsync<T>(string id, T document, string index) where T : class
    {
        try
        {
            var response = await _client.UpdateAsync<T>(id, u => u
                .Index(index)
                .Doc(document)
                .DocAsUpsert(true)
            );

            if (!response.IsValid)
            {
                _logger.LogError("Failed to update document {Id} in {Index}: {Error}", id, index, response.DebugInformation);
                return false;
            }

            _logger.LogInformation("Document {Id} updated in {Index}", id, index);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document {Id} in {Index}", id, index);
            return false;
        }
    }
}
