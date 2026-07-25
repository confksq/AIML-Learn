// ============================================================
// MODULE 06 PLUGIN: Submit PA Decision + Escalate
// ============================================================
// HEALTHCARE EQUIVALENT OF: ClaimDecisionPlugin.cs (JMA)
// JMA:  submit_claim_decision → write to DMS + generate auth number
// HERE: submit_pa_decision → write to EHR/payer system + generate PA number
//       escalate_to_pharmacist → route to licensed clinical pharmacist
// ============================================================

using Microsoft.SemanticKernel;

namespace VitalCare.PriorAuthAgent.Plugins;

public class PADecisionPlugin
{
    private readonly HttpClient _http;
    private readonly FaultTolerance.ClinicalEscalationService _escalation;
    private readonly ILogger<PADecisionPlugin> _logger;

    public PADecisionPlugin(
        HttpClient http,
        FaultTolerance.ClinicalEscalationService escalation,
        ILogger<PADecisionPlugin> logger)
    {
        _http       = http;
        _escalation = escalation;
        _logger     = logger;
    }

    [KernelFunction("submit_pa_decision")]
    [Description("Submit the final prior authorization decision (approved or denied) to the payer system. Only call after verifying eligibility AND formulary criteria. Include the policy evidence in the rationale.")]
    public async Task<PASubmissionResult> SubmitDecisionAsync(
        [Description("Request ID from the original PA request")] string requestId,
        [Description("Decision: must be 'approved' or 'denied' only")] string decision,
        [Description("Clinical rationale citing specific formulary section and diagnosis criteria")] string rationale,
        [Description("Source document reference for the policy criteria used")] string policyRef,
        [Description("Authorized days supply — required if decision is approved")] int authorizedDays = 0)
    {
        // INTERVIEW: Validate decision before writing — never write invalid status
        if (decision is not ("approved" or "denied"))
        {
            _logger.LogError("[PA DECISION] Invalid decision '{Decision}' for request {ReqId}", decision, requestId);
            throw new ArgumentException($"Decision must be 'approved' or 'denied', got: '{decision}'");
        }

        // INTERVIEW: Approved decisions require an authorized days supply
        if (decision == "approved" && authorizedDays <= 0)
        {
            _logger.LogWarning("[PA DECISION] Approved decision missing authorizedDays — pending to pharmacist");
            return await EscalateToClinicalAsync(requestId,
                "Approved decision submitted without authorized days supply — clinical pharmacist review required");
        }

        var authNumber = decision == "approved"
            ? $"PA-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}"
            : string.Empty;

        _logger.LogInformation("[PA DECISION] Submitting {Decision} for request {ReqId} | Auth: {Auth}",
            decision, requestId, authNumber);

        // Write to payer system (EHR / prior auth platform)
        var payload = new { requestId, decision, rationale, policyRef, authNumber, authorizedDays };
        await _http.PostAsJsonAsync("/pa/v1/decisions", payload);

        return new PASubmissionResult
        {
            RequestId  = requestId,
            Decision   = decision,
            AuthNumber = authNumber,
            AuthDays   = authorizedDays,
            PolicyRef  = policyRef
        };
    }

    [KernelFunction("escalate_to_pharmacist")]
    [Description("Escalate the prior authorization to a licensed clinical pharmacist for review. Call when: eligibility data unavailable, step therapy history unclear, clinical criteria ambiguous, or any uncertainty about the clinical decision.")]
    public async Task<PASubmissionResult> EscalateToClinicalAsync(
        [Description("Request ID to escalate")] string requestId,
        [Description("Clear description of why clinical pharmacist review is needed")] string reason)
    {
        _logger.LogWarning("[PA ESCALATION] Request {ReqId} escalated: {Reason}", requestId, reason);

        var ticket = await _escalation.EscalateToPharmacistAsync(new FaultTolerance.ClinicalEscalationRequest
        {
            RequestId   = requestId,
            Reason      = reason,
            Priority    = reason.Contains("urgent", StringComparison.OrdinalIgnoreCase) ? "URGENT" : "ROUTINE"
        });

        return new PASubmissionResult
        {
            RequestId  = requestId,
            Decision   = "pended",
            TicketId   = ticket.TicketId,
            AuthNumber = string.Empty
        };
    }
}

public record PASubmissionResult
{
    public string RequestId  { get; init; } = string.Empty;
    public string Decision   { get; init; } = string.Empty;
    public string AuthNumber { get; init; } = string.Empty;
    public string TicketId   { get; init; } = string.Empty;
    public string PolicyRef  { get; init; } = string.Empty;
    public int    AuthDays   { get; init; }
}
