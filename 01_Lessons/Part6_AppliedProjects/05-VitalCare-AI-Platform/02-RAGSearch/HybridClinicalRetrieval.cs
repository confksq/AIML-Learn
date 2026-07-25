// GAP TOPIC: Hybrid Retrieval for Clinical Documents
// HEALTHCARE EQUIVALENT OF: HybridRetrieval.cs (JMA)
// KEY DIFFERENCE: Drug codes (NDC) and diagnosis codes (ICD-10) need EXACT keyword match
//                 Clinical meaning needs semantic (vector) search
//                 Hybrid is even more important in healthcare than JMA

using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Azure.Identity;

namespace VitalCare.RAGSearch;

public class HybridClinicalRetrieval
{
    private readonly SearchClient _searchClient;
    private readonly IEmbeddingService _embedder;
    private readonly ILogger<HybridClinicalRetrieval> _logger;

    public HybridClinicalRetrieval(string searchEndpoint, IEmbeddingService embedder, ILogger<HybridClinicalRetrieval> logger)
    {
        _searchClient = new SearchClient(new Uri(searchEndpoint), "vitalcare-clinical-index", new DefaultAzureCredential());
        _embedder     = embedder;
        _logger       = logger;
    }

    // INTERVIEW: "Why is hybrid even more important in healthcare than JMA?"
    // "Drug codes and diagnosis codes are exact — NDC '00071-0156-23' and
    //  'lisinopril 10mg' mean the same thing but BM25 won't match them semantically.
    //  At the same time, 'heart failure with reduced ejection fraction' and
    //  'HFrEF' are clinically identical but keyword search won't connect them.
    //  Hybrid gives you exact code matching (BM25) + clinical synonym matching (HNSW).
    //  In healthcare, missing the right clinical criterion because of a keyword
    //  mismatch isn't a retrieval quality problem — it's a patient safety problem."
    public async Task<List<ClinicalRetrievedChunk>> RetrieveAsync(
        string query,
        string? planFilter  = null,
        string? icd10Filter = null,
        string? ndcFilter   = null,
        int     topK        = 5)
    {
        _logger.LogInformation("[CLINICAL HYBRID] Query: '{Query}' | Plan: {Plan} | ICD10: {DX} | NDC: {NDC}",
            query, planFilter, icd10Filter, ndcFilter);

        var queryEmbedding = await _embedder.EmbedAsync(query);

        var filterParts = new List<string>();
        if (planFilter  != null) filterParts.Add($"planId eq '{planFilter}'");
        if (icd10Filter != null) filterParts.Add($"icd10Codes/any(c: c eq '{icd10Filter}')");
        if (ndcFilter   != null) filterParts.Add($"ndcCodes/any(c: c eq '{ndcFilter}')");

        var options = new SearchOptions
        {
            VectorSearch = new VectorSearchOptions
            {
                Queries = { new VectorizedQuery(queryEmbedding) { KNearestNeighborsCount = topK * 2, Fields = { "embedding" } } }
            },
            QueryType     = SearchQueryType.Semantic,
            SemanticSearch = new SemanticSearchOptions { ConfigurationName = "clinical-semantic" },
            Filter        = filterParts.Any() ? string.Join(" and ", filterParts) : null,
            Select        = { "id", "text", "documentId", "documentType" },
            Size          = topK
        };

        var results = await _searchClient.SearchAsync<ClinicalSearchChunk>(query, options);
        var chunks  = new List<ClinicalRetrievedChunk>();

        await foreach (var r in results.Value.GetResultsAsync())
        {
            chunks.Add(new ClinicalRetrievedChunk
            {
                Id           = r.Document.Id,
                Text         = r.Document.Text,
                DocumentId   = r.Document.DocumentId,
                DocumentType = r.Document.DocumentType,
                HybridScore  = r.Score ?? 0.0,
                SemanticScore = r.SemanticSearch?.RerankerScore ?? 0.0,
                KeyPhrases   = r.SemanticSearch?.Captions.Select(c => c.Text).ToArray() ?? []
            });
        }

        _logger.LogInformation("[CLINICAL HYBRID] Retrieved {Count} chunks", chunks.Count);
        return chunks;
    }
}

public record ClinicalRetrievedChunk
{
    public string   Id           { get; init; } = string.Empty;
    public string   Text         { get; init; } = string.Empty;
    public string   DocumentId   { get; init; } = string.Empty;
    public string   DocumentType { get; init; } = string.Empty;
    public double   HybridScore  { get; init; }
    public double   SemanticScore { get; init; }
    public string[] KeyPhrases   { get; init; } = [];
}

public interface IEmbeddingService { Task<float[]> EmbedAsync(string text); }
