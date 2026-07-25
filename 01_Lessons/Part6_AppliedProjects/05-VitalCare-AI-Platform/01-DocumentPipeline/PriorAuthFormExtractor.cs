// MODULE 09: Prior Auth Form Extractor
// HEALTHCARE EQUIVALENT OF: DealerFormExtractor.cs (JMA)
// Extracts structured PA data from PDF forms submitted by providers
// KEY DIFFERENCES:
// - PHI handling: extracted fields classified as PHI vs non-PHI
// - Higher confidence threshold: 0.92 (clinical > financial)
// - Dead letter: never auto-deny — route to data entry for reprocessing

using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.Identity;

namespace VitalCare.DocumentPipeline;

public class PriorAuthFormExtractor
{
    private readonly DocumentAnalysisClient _diClient;
    private readonly ILogger<PriorAuthFormExtractor> _logger;

    // INTERVIEW: Higher threshold than JMA (0.90) because clinical = higher stakes
    private const double AutoProcessThreshold = 0.92;
    private const double ReviewThreshold      = 0.75;

    public PriorAuthFormExtractor(string diEndpoint, ILogger<PriorAuthFormExtractor> logger)
    {
        // DefaultAzureCredential = Managed Identity — no stored secrets accessing PHI
        _diClient = new DocumentAnalysisClient(new Uri(diEndpoint), new DefaultAzureCredential());
        _logger   = logger;
    }

    public async Task<PAExtractionResult> ExtractAsync(Stream pdfStream, string documentId)
    {
        _logger.LogInformation("[PA DI] Starting extraction for document {DocId}", documentId);

        // INTERVIEW: "vitalcare-prior-auth" = custom model trained on PA forms
        // PA forms vary by payer and provider — custom model learns the JMA equivalent of "where is the VIN"
        var operation = await _diClient.AnalyzeDocumentAsync(
            WaitUntil.Completed,
            modelId: "vitalcare-prior-auth",
            document: pdfStream);

        var result   = operation.Value;
        var document = result.Documents.FirstOrDefault();

        if (document == null)
            return PAExtractionResult.DeadLetter(documentId, "No document found in DI result");

        // INTERVIEW: PHI field classification — know which extracted fields are PHI
        // MemberId = non-PHI (it's an identifier, not the patient's name)
        // PatientName = PHI — encrypt immediately, don't log
        var fields = document.Fields.ToDictionary(
            kvp => kvp.Key,
            kvp => (Value: kvp.Value.Content ?? string.Empty, Confidence: kvp.Value.Confidence ?? 0.0));

        var requiredFields = new[] { "MemberId", "DrugNDC", "DiagnosisCode", "ProviderNPI", "DaysRequested" };
        var minConfidence  = requiredFields
            .Where(f => fields.ContainsKey(f))
            .Select(f => fields[f].Confidence)
            .DefaultIfEmpty(0.0).Min();

        _logger.LogInformation("[PA DI] Document {DocId}: min confidence={Conf:P0}", documentId, minConfidence);

        if (minConfidence >= AutoProcessThreshold)
            return PAExtractionResult.AutoProcess(documentId, BuildPARequest(fields), minConfidence);
        else if (minConfidence >= ReviewThreshold)
            return PAExtractionResult.NeedsReview(documentId, BuildPARequest(fields), minConfidence);
        else
        {
            _logger.LogWarning("[PA DI] Dead letter: {DocId} confidence {Conf:P0}", documentId, minConfidence);
            return PAExtractionResult.DeadLetter(documentId, $"Extraction confidence {minConfidence:P0} below minimum. Provider must resubmit with clearer form.");
        }
    }

    private static PriorAuthRequest BuildPARequest(Dictionary<string, (string Value, double Confidence)> fields)
    {
        bool Get(string key, out string val) { val = fields.TryGetValue(key, out var f) ? f.Value : ""; return !string.IsNullOrEmpty(val); }
        Get("MemberId", out var member); Get("DrugNDC", out var ndc);
        Get("DiagnosisCode", out var dx); Get("ProviderNPI", out var npi);
        Get("DaysRequested", out var days);

        return new PriorAuthRequest
        {
            RequestId     = Guid.NewGuid().ToString(),
            MemberId      = member,
            DrugNDC       = ndc,
            DiagnosisCode = dx,
            ProviderId    = npi,
            QuantityDays  = int.TryParse(days, out var d) ? d : 0,
            RequestDate   = DateTime.UtcNow
        };
    }
}

public record PAExtractionResult
{
    public string         DocumentId  { get; init; } = string.Empty;
    public string         Route       { get; init; } = string.Empty;
    public PriorAuthRequest? Request  { get; init; }
    public double         Confidence  { get; init; }
    public string         ErrorReason { get; init; } = string.Empty;

    public static PAExtractionResult AutoProcess(string id, PriorAuthRequest req, double conf) =>
        new() { DocumentId = id, Route = "auto", Request = req, Confidence = conf };
    public static PAExtractionResult NeedsReview(string id, PriorAuthRequest req, double conf) =>
        new() { DocumentId = id, Route = "review", Request = req, Confidence = conf };
    public static PAExtractionResult DeadLetter(string id, string reason) =>
        new() { DocumentId = id, Route = "dead_letter", ErrorReason = reason };
}
