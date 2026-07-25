// GAP TOPIC + MODULE 11: Clinical LLMOps — Evaluation Pipeline
// HEALTHCARE EQUIVALENT OF: EvaluationPipeline.cs (JMA)
// KEY DIFFERENCES:
// - Groundedness threshold: 0.90 (vs 0.85 for JMA) — patient safety bar
// - Decision accuracy threshold: 0.95 (vs 0.90) — clinical decisions matter more
// - Golden dataset reviewed by clinical pharmacist, not just engineers
// - Pharmacist sign-off required before any prompt change reaches production

namespace VitalCare.LLMOps;

public class ClinicalEvalPipeline
{
    private readonly PriorAuthAgent.PriorAuthDecisionAgent _agent;
    private readonly ClinicalEvaluatorLLM _evaluator;
    private readonly ILogger<ClinicalEvalPipeline> _logger;

    // INTERVIEW: "Why is your clinical groundedness threshold higher than JMA?"
    // "In JMA, a wrong incentive claim decision costs money.
    //  In healthcare, a wrong PA decision can delay patient treatment or
    //  cause inappropriate denial of care. The stakes are different.
    //  We set groundedness at 0.90 and decision accuracy at 0.95 —
    //  and the golden dataset is reviewed by a clinical pharmacist,
    //  not just software engineers. They validate that the expected answers
    //  are clinically correct, not just logically consistent."
    private const double GroundednessThreshold  = 0.90;  // higher than JMA (0.85)
    private const double RelevanceThreshold     = 0.85;
    private const double DecisionAccuracyThreshold = 0.95;  // higher than JMA (0.90)

    public ClinicalEvalPipeline(
        PriorAuthAgent.PriorAuthDecisionAgent agent,
        ClinicalEvaluatorLLM evaluator,
        ILogger<ClinicalEvalPipeline> logger)
    { _agent = agent; _evaluator = evaluator; _logger = logger; }

    public async Task<ClinicalEvalReport> RunAsync(List<ClinicalGoldenCase> goldenDataset)
    {
        _logger.LogInformation("[CLINICAL EVAL] Running on {Count} golden PA cases", goldenDataset.Count);
        var scores = new List<ClinicalCaseScore>();

        foreach (var testCase in goldenDataset)
        {
            var response = await _agent.ProcessRequestAsync(testCase.PARequest);
            var score    = await _evaluator.ScoreAsync(testCase, response);

            scores.Add(new ClinicalCaseScore
            {
                CaseId        = testCase.Id,
                Groundedness  = score.Groundedness,
                Relevance     = score.Relevance,
                DecisionMatch = response.Status == testCase.ExpectedDecision,
                // INTERVIEW: "pended when should have approved" is less bad than "approved when should have denied"
                // False negatives (over-pend) vs false positives (approve what should deny) have different clinical weights
                FalseApproval = response.Status == "approved" && testCase.ExpectedDecision == "denied"
            });

            _logger.LogInformation("[CLINICAL EVAL] Case {Id}: G={G:F2} R={R:F2} Decision={D}{FP}",
                testCase.Id, score.Groundedness, score.Relevance,
                response.Status == testCase.ExpectedDecision ? "✓" : "✗",
                scores.Last().FalseApproval ? " ⚠️ FALSE APPROVAL" : "");
        }

        var report = new ClinicalEvalReport
        {
            RunAt              = DateTime.UtcNow,
            TotalCases         = scores.Count,
            AvgGroundedness    = scores.Average(s => s.Groundedness),
            AvgRelevance       = scores.Average(s => s.Relevance),
            DecisionAccuracy   = scores.Count(s => s.DecisionMatch) / (double)scores.Count,
            FalseApprovalRate  = scores.Count(s => s.FalseApproval) / (double)scores.Count,
            PassesGate         = scores.Average(s => s.Groundedness) >= GroundednessThreshold &&
                                  scores.Average(s => s.Relevance)   >= RelevanceThreshold    &&
                                  scores.Count(s => s.DecisionMatch) / (double)scores.Count >= DecisionAccuracyThreshold &&
                                  scores.Count(s => s.FalseApproval) / (double)scores.Count == 0  // zero false approvals allowed
        };

        if (report.FalseApprovalRate > 0)
            _logger.LogCritical("[CLINICAL EVAL] FALSE APPROVALS DETECTED: {Rate:P1} — DEPLOYMENT BLOCKED regardless of other scores.", report.FalseApprovalRate);
        else if (!report.PassesGate)
            _logger.LogCritical("[CLINICAL EVAL] QUALITY GATE FAILED — G={G:F2}, Acc={A:F2}. Deployment blocked.", report.AvgGroundedness, report.DecisionAccuracy);
        else
            _logger.LogInformation("[CLINICAL EVAL] Quality gate PASSED. Pharmacist sign-off next.");

        return report;
    }
}

public record ClinicalGoldenCase
{
    public string         Id               { get; init; } = string.Empty;
    public PriorAuthRequest PARequest       { get; init; } = new();
    public string         PolicyContext    { get; init; } = string.Empty;
    public string         ExpectedDecision { get; init; } = string.Empty;
    public string         ClinicalReviewedBy { get; init; } = string.Empty;  // pharmacist who validated this case
}

public record ClinicalEvalReport
{
    public DateTime RunAt             { get; init; }
    public int      TotalCases        { get; init; }
    public double   AvgGroundedness   { get; init; }
    public double   AvgRelevance      { get; init; }
    public double   DecisionAccuracy  { get; init; }
    public double   FalseApprovalRate { get; init; }
    public bool     PassesGate        { get; init; }
}

public record ClinicalCaseScore { public string CaseId = ""; public double Groundedness; public double Relevance; public bool DecisionMatch; public bool FalseApproval; }
public record ClinicalEvalScore { public double Groundedness; public double Relevance; }
public class ClinicalEvaluatorLLM { public Task<ClinicalEvalScore> ScoreAsync(ClinicalGoldenCase c, PADecisionResponse r) => Task.FromResult(new ClinicalEvalScore { Groundedness = 0.92, Relevance = 0.88 }); }
