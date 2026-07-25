// ============================================================
// MODULE 07: Meta-Agent Hierarchy — PA Supervisor Agent
// ============================================================
// HEALTHCARE EQUIVALENT OF: SupervisorAgent.cs (JMA)
// JMA:  Supervisor → ClaimValidator + PolicyChecker + FraudDetector
// HERE: Supervisor → EligibilityChecker + FormularyChecker + ClinicalCriteriaAgent
//
// KEY HEALTHCARE DIFFERENCES:
// - "pended" status (clinical info needed) not just approved/denied/escalated
// - Clinical Criteria Agent checks diagnosis codes, age/gender rules, quantity limits
// - Escalation to LICENSED CLINICIAN (pharmacist), not business reviewer (RSM)
// - Higher stakes: wrong decision = patient harm, not just financial error
// ============================================================
// INTERVIEW: "Why use a supervisor pattern for prior auth?"
// "Prior auth requires three independent checks: eligibility, formulary, and
//  clinical criteria. Running them in a single monolithic agent means one
//  agent holds all the knowledge and prompts — harder to debug, worse accuracy.
//  The supervisor delegates: EligibilityChecker handles plan coverage,
//  FormularyChecker handles the drug's tier and PA requirements,
//  ClinicalCriteriaAgent evaluates diagnosis codes and step therapy.
//  Eligibility is fast and cheap — I run it first as a guard.
//  Formulary and clinical criteria are independent so they run in parallel.
//  The supervisor synthesizes all three results into the final decision."
// ============================================================

namespace VitalCare.MetaAgentOrchestration;

public class PASupervisorAgent
{
    private readonly EligibilityCheckerAgent   _eligibility;
    private readonly FormularyCheckerAgent     _formulary;
    private readonly ClinicalCriteriaAgent     _clinical;
    private readonly ILogger<PASupervisorAgent> _logger;

    public PASupervisorAgent(
        EligibilityCheckerAgent eligibility,
        FormularyCheckerAgent   formulary,
        ClinicalCriteriaAgent   clinical,
        ILogger<PASupervisorAgent> logger)
    {
        _eligibility = eligibility;
        _formulary   = formulary;
        _clinical    = clinical;
        _logger      = logger;
    }

    public async Task<SupervisorPADecision> OrchestrateAsync(PriorAuthRequest request)
    {
        _logger.LogInformation("[PA SUPERVISOR] Orchestrating PA {RequestId}", request.RequestId);

        // STEP 1: Eligibility first — fast guard
        // INTERVIEW: Run eligibility before expensive formulary/clinical checks
        // If member isn't covered, the rest is irrelevant — fail fast
        var eligibility = await _eligibility.CheckAsync(request);

        if (!eligibility.IsEligible)
        {
            return new SupervisorPADecision
            {
                RequestId = request.RequestId,
                Decision  = "denied",
                Reason    = $"Member not eligible: {eligibility.IneligibleReason}",
                Pend      = false
            };
        }

        // STEP 2: Formulary + Clinical in parallel
        // INTERVIEW: These are INDEPENDENT — formulary check doesn't depend on clinical result
        // Running them concurrently cuts latency roughly in half
        var (formularyResult, clinicalResult) = await (
            _formulary.CheckAsync(request),
            _clinical.EvaluateAsync(request)
        );

        _logger.LogInformation(
            "[PA SUPERVISOR] Formulary: {FRm} | Clinical: {Cl} | RequestId: {Id}",
            formularyResult.OnFormulary, clinicalResult.MeetsCriteria, request.RequestId);

        // STEP 3: Synthesize

        // Drug not on formulary → deny
        if (!formularyResult.OnFormulary)
        {
            return new SupervisorPADecision
            {
                RequestId = request.RequestId,
                Decision  = "denied",
                Reason    = $"Drug not on formulary for plan {request.PlanId}: {formularyResult.Reason}",
                PolicyRef = formularyResult.PolicyRef
            };
        }

        // Clinical criteria ambiguous → pend to pharmacist (healthcare-specific status)
        if (clinicalResult.IsAmbiguous)
        {
            _logger.LogWarning("[PA SUPERVISOR] Clinical criteria ambiguous — pending {ReqId}", request.RequestId);
            return new SupervisorPADecision
            {
                RequestId = request.RequestId,
                Decision  = "pended",
                Reason    = $"Clinical criteria ambiguous: {clinicalResult.AmbiguityReason}. Pharmacist review required.",
                Pend      = true   // INTERVIEW: "pended" = needs more info, not a final decision
            };
        }

        // Clinical criteria not met → deny with clinical evidence
        if (!clinicalResult.MeetsCriteria)
        {
            return new SupervisorPADecision
            {
                RequestId = request.RequestId,
                Decision  = "denied",
                Reason    = clinicalResult.DenialReason,
                PolicyRef = clinicalResult.PolicyRef
            };
        }

        // All criteria met → approve
        return new SupervisorPADecision
        {
            RequestId  = request.RequestId,
            Decision   = "approved",
            Reason     = $"Member eligible, drug on formulary ({formularyResult.PolicyRef}), clinical criteria met ({clinicalResult.PolicyRef})",
            PolicyRef  = clinicalResult.PolicyRef,
            AuthDays   = formularyResult.MaxDaysSupply
        };
    }
}

public record SupervisorPADecision
{
    public string RequestId { get; init; } = string.Empty;
    public string Decision  { get; init; } = string.Empty;   // approved | denied | pended
    public string Reason    { get; init; } = string.Empty;
    public string PolicyRef { get; init; } = string.Empty;
    public int    AuthDays  { get; init; }
    public bool   Pend      { get; init; }
}
