// GAP TOPIC: HNSW Vector Search for Clinical Documents
// HEALTHCARE EQUIVALENT OF: HNSWVectorSearch.cs (JMA)
// Indexes: formulary PDFs, clinical coverage policies, clinical guidelines

using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Azure.Identity;

namespace VitalCare.RAGSearch;

public class FormularyVectorSearch
{
    private readonly SearchIndexClient _indexClient;
    private readonly SearchClient      _searchClient;
    private const string IndexName = "vitalcare-clinical-index";

    public FormularyVectorSearch(string searchEndpoint)
    {
        var cred     = new DefaultAzureCredential();
        _indexClient = new SearchIndexClient(new Uri(searchEndpoint), cred);
        _searchClient = new SearchClient(new Uri(searchEndpoint), IndexName, cred);
    }

    // INTERVIEW: "What's in your clinical HNSW index?"
    // "Three document types: formulary entries (drug + tier + PA requirements),
    //  clinical coverage policies (diagnosis criteria, step therapy rules),
    //  and clinical guidelines (evidence-based treatment standards).
    //  The HNSW parameters are the same as JMA — m=4, efConstruction=400.
    //  But I add clinical metadata fields: icd10Codes, ndcCodes, documentType.
    //  These become search filters — if I'm looking up coverage for diagnosis Z00.01,
    //  I filter to chunks tagged with that ICD-10 code first, then do HNSW.
    //  Pre-filtering reduces the search space dramatically for clinical queries."
    public async Task CreateIndexAsync()
    {
        var index = new SearchIndex(IndexName)
        {
            Fields =
            {
                new SimpleField("id",           SearchFieldDataType.String)  { IsKey = true },
                new SearchableField("text")     { IsFilterable = false },
                new SimpleField("documentId",   SearchFieldDataType.String)  { IsFilterable = true },
                new SimpleField("documentType", SearchFieldDataType.String)  { IsFilterable = true },  // formulary | clinical-guideline | coverage-policy
                new SimpleField("planId",       SearchFieldDataType.String)  { IsFilterable = true },  // filter by insurance plan
                new SimpleField("icd10Codes",   SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsFilterable = true },  // pre-filter by diagnosis
                new SimpleField("ndcCodes",     SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsFilterable = true },  // pre-filter by drug
                new VectorSearchField("embedding", dimensions: 1536, vectorSearchProfileName: "clinical-vector-profile")
            },
            VectorSearch = new VectorSearch
            {
                Algorithms = { new HnswAlgorithmConfiguration("clinical-hnsw") { Parameters = new HnswParameters { M = 4, EfConstruction = 400, EfSearch = 500, Metric = VectorSearchAlgorithmMetric.Cosine } } },
                Profiles   = { new VectorSearchProfile("clinical-vector-profile", "clinical-hnsw") }
            },
            SemanticSearch = new SemanticSearch
            {
                Configurations = { new SemanticConfiguration("clinical-semantic", new SemanticPrioritizedFields { ContentFields = { new SemanticField("text") } }) }
            }
        };

        await _indexClient.CreateOrUpdateIndexAsync(index);
    }

    public async Task<List<ClinicalSearchChunk>> SearchAsync(float[] queryEmbedding, string? planId = null, string? icd10 = null, int topK = 5)
    {
        var filterParts = new List<string>();
        if (planId != null) filterParts.Add($"planId eq '{planId}'");
        if (icd10  != null) filterParts.Add($"icd10Codes/any(c: c eq '{icd10}')");

        var options = new SearchOptions
        {
            VectorSearch = new VectorSearchOptions
            {
                Queries = { new VectorizedQuery(queryEmbedding) { KNearestNeighborsCount = topK * 2, Fields = { "embedding" } } }
            },
            QueryType     = SearchQueryType.Semantic,
            SemanticSearch = new SemanticSearchOptions { ConfigurationName = "clinical-semantic" },
            Filter        = filterParts.Any() ? string.Join(" and ", filterParts) : null,
            Size          = topK
        };

        var results = await _searchClient.SearchAsync<ClinicalSearchChunk>("*", options);
        var chunks  = new List<ClinicalSearchChunk>();
        await foreach (var r in results.Value.GetResultsAsync())
            chunks.Add(r.Document with { Score = r.Score ?? 0.0, SemanticScore = r.SemanticSearch?.RerankerScore ?? 0.0 });

        return chunks;
    }
}

public record ClinicalSearchChunk
{
    public string   Id           { get; init; } = string.Empty;
    public string   Text         { get; init; } = string.Empty;
    public string   DocumentId   { get; init; } = string.Empty;
    public string   DocumentType { get; init; } = string.Empty;
    public double   Score        { get; init; }
    public double   SemanticScore { get; init; }
}
