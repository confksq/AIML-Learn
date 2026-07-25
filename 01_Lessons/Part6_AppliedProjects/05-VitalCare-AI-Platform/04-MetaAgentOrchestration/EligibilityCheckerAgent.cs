// MODULE 07: Specialist — Member Eligibility Checker
// HEALTHCARE EQUIVALENT OF: ClaimValidatorAgent.cs (JMA)
// Checks member is actively enrolled and has drug benefit coverage

namespace VitalCare.MetaAgentOrchestration;

public class EligibilityCheckerAgent
{
    private readonly ILogger<EligibilityCheckerAgent> _logger;
    public EligibilityCheckerAgent(ILogger<EligibilityCheckerAgent> logger) => _logger = logger;

    public async Task<EligibilityCheckResult> CheckAsync(PriorAuthRequest request)
    {
        _logger.LogInformation("[ELIGIBILITY] Checking member {MemberId} on plan {PlanId}",
            request.MemberId, request.PlanId);

        // Rule-based checks first (fast, no LLM needed)
        var failures = new List<string>();
        if (string.IsNullOrEmpty(request.MemberId))   failures.Add("Missing MemberId");
        if (string.IsNullOrEmpty(request.PlanId))     failures.Add("Missing PlanId");
        if (string.IsNullOrEmpty(request.DrugNDC))    failures.Add("Missing Drug NDC");
        if (string.IsNullOrEmpty(request.DiagnosisCode)) failures.Add("Missing Diagnosis Code (ICD-10)");
        if (request.QuantityDays <= 0)                failures.Add("Invalid days supply");
        if (request.RequestDate > DateTime.UtcNow.AddDays(30)) failures.Add("Request date too far in future");

        if (failures.Any())
            return new EligibilityCheckResult { IsEligible = false, IneligibleReason = string.Join("; ", failures) };

        // Production: query payer eligibility API (270/271 EDI or REST)
        return await Task.FromResult(new EligibilityCheckResult
        {
            IsEligible        = true,
            PlanName          = "BlueCross PPO Gold",
            DrugBenefitActive = true
        });
    }
}

public record EligibilityCheckResult
{
    public bool   IsEligible        { get; init; }
    public string IneligibleReason  { get; init; } = string.Empty;
    public string PlanName          { get; init; } = string.Empty;
    public bool   DrugBenefitActive { get; init; }
}
