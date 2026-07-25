// GAP TOPIC + MODULE 11: Clinical Groundedness Monitoring
// HEALTHCARE EQUIVALENT OF: GroundednessMonitor.cs (JMA)
// KEY DIFFERENCE: Groundedness drop = patient safety event, not just quality issue
//                 Alerts go to clinical quality officer, not just on-call engineer

namespace VitalCare.LLMOps;

public class ClinicalGroundednessMonitor
{
    private readonly ClinicalEvaluatorLLM _evaluator;
    private readonly TelemetryClient      _telemetry;
    private readonly ILogger<ClinicalGroundednessMonitor> _logger;

    private const double AlertThreshold    = 0.85;
    private const double CriticalThreshold = 0.75;  // patient safety alert

    public ClinicalGroundednessMonitor(ClinicalEvaluatorLLM ev, TelemetryClient tel, ILogger<ClinicalGroundednessMonitor> log)
    { _evaluator = ev; _telemetry = tel; _logger = log; }

    // INTERVIEW: "What triggers your clinical groundedness alert?"
    // "Two thresholds. At 0.85 we alert the engineering on-call — investigate
    //  potential drift in the formulary index or prompt quality.
    //  At 0.75 we alert the clinical quality officer AND trigger an immediate
    //  review of all PA decisions made in the last hour. That's a patient
    //  safety response — not just an engineering alert. In healthcare,
    //  groundedness monitoring is a clinical quality function, not just DevOps."
    public async Task MonitorAsync(ClinicalProductionInput input)
    {
        var score = await _evaluator.ScoreAsync(
            new ClinicalGoldenCase { PolicyContext = input.RetrievedClinicalEvidence },
            new PADecisionResponse { Status = input.Decision, Rationale = input.Rationale });

        _telemetry.TrackMetric("pa.groundedness", score.Groundedness);
        _telemetry.TrackMetric("pa.relevance",    score.Relevance);
        _telemetry.TrackEvent("PARatedInProduction", new Dictionary<string, string>
        {
            ["request_id"]   = input.RequestId,
            ["decision"]     = input.Decision,
            ["groundedness"] = score.Groundedness.ToString("F3")
            // NO PHI — member ID not logged here either
        });

        if (score.Groundedness < CriticalThreshold)
        {
            _logger.LogCritical(
                "[CLINICAL GROUNDEDNESS CRITICAL] RequestId: {ReqId} | Score: {Score:F2} | " +
                "PATIENT SAFETY ALERT — notifying clinical quality officer. " +
                "All PA decisions in last 60 min queued for pharmacist review.",
                input.RequestId, score.Groundedness);
            // Production: PagerDuty → on-call eng + clinical quality officer
            // Auto-queue last 60 min of PA decisions for pharmacist review
        }
        else if (score.Groundedness < AlertThreshold)
        {
            _logger.LogWarning(
                "[CLINICAL GROUNDEDNESS WARNING] RequestId: {ReqId} | Score: {Score:F2} | " +
                "Alerting engineering — investigate formulary index or prompt quality.",
                input.RequestId, score.Groundedness);
        }
    }
}

public record ClinicalProductionInput
{
    public string RequestId             { get; init; } = string.Empty;
    public string RetrievedClinicalEvidence { get; init; } = string.Empty;
    public string Rationale             { get; init; } = string.Empty;
    public string Decision              { get; init; } = string.Empty;
}
