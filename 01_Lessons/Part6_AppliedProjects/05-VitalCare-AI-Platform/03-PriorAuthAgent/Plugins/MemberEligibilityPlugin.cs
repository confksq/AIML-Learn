// ============================================================
// MODULE 06 PLUGIN: Member Eligibility Check
// ============================================================
// HEALTHCARE EQUIVALENT OF: DealerEligibilityPlugin.cs (JMA)
// JMA:  check_dealer_eligibility → DMS API → dealer active/enrolled?
// HERE: check_member_eligibility → Payer API → member covered by plan?
//
// KEY DIFFERENCES:
// - Payer API (insurance company's eligibility service) not DMS
// - Checks: plan active, drug benefit included, deductible met
// - Returns coverage tier (copay vs. coinsurance) for approved drugs
// ============================================================

using Microsoft.SemanticKernel;

namespace VitalCare.PriorAuthAgent.Plugins;

public class MemberEligibilityPlugin
{
    private readonly HttpClient _http;
    private readonly ILogger<MemberEligibilityPlugin> _logger;

    public MemberEligibilityPlugin(HttpClient http, ILogger<MemberEligibilityPlugin> logger)
    {
        _http   = http;
        _logger = logger;
    }

    // INTERVIEW: [KernelFunction] + [Description] — LLM reads the description
    // to decide when to call this tool. Make descriptions precise and action-oriented.
    [KernelFunction("check_member_eligibility")]
    [Description("Check whether a member is actively enrolled in their health plan and has drug benefit coverage. Call this FIRST before any formulary or clinical criteria checks.")]
    public async Task<MemberEligibilityResult> CheckEligibilityAsync(
        [Description("Member's insurance ID number — not their name")] string memberId,
        [Description("Insurance plan identifier")] string planId,
        [Description("Date of service in YYYY-MM-DD format")] string requestDate)
    {
        _logger.LogInformation("[ELIGIBILITY] Checking member {MemberId} on plan {PlanId}", memberId, planId);

        // INTERVIEW: Payer eligibility API — every major insurer exposes one
        // 270/271 EDI transaction (healthcare standard) or REST API
        // DefaultAzureCredential = Managed Identity, no stored credentials
        var response = await _http.GetAsync(
            $"/eligibility/v1/members/{memberId}/plans/{planId}?date={requestDate}");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("[ELIGIBILITY] Payer API returned {Status} for member {MemberId}",
                response.StatusCode, memberId);
            // INTERVIEW: API failure → return unknown, agent will escalate to pharmacist
            return new MemberEligibilityResult
            {
                MemberId     = memberId,
                IsEligible   = false,
                IneligibleReason = $"Payer API returned {(int)response.StatusCode} — unable to verify eligibility"
            };
        }

        // Simulate successful eligibility check
        return new MemberEligibilityResult
        {
            MemberId       = memberId,
            IsEligible     = true,
            PlanName       = "BlueCross PPO Gold",
            DrugBenefitActive = true,
            DeductibleMet  = true
        };
    }
}

public record MemberEligibilityResult
{
    public string MemberId            { get; init; } = string.Empty;
    public bool   IsEligible          { get; init; }
    public string IneligibleReason    { get; init; } = string.Empty;
    public string PlanName            { get; init; } = string.Empty;
    public bool   DrugBenefitActive   { get; init; }
    public bool   DeductibleMet       { get; init; }
}
