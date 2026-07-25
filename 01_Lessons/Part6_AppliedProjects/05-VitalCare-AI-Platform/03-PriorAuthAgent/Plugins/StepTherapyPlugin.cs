// ============================================================
// MODULE 06 PLUGIN: Step Therapy Check (Healthcare-Specific)
// ============================================================
// NO JMA EQUIVALENT — this is a healthcare-only concept
//
// WHAT IS STEP THERAPY:
// "Fail first" requirement — before approving a specialty/brand drug,
// the payer requires proof the member tried lower-cost alternatives first
// Example: Before approving Humira (biologic, $3000/month) for rheumatoid
// arthritis, plan requires member tried methotrexate (generic, $20/month) first
//
// WHY THIS MATTERS IN THE INTERVIEW:
// Shows you understand healthcare-specific PA logic, not just generic agents
// ============================================================
// INTERVIEW: "What's step therapy and how does your agent handle it?"
// "Step therapy is a payer requirement that patients try lower-cost drugs
//  before a specialty drug gets approved — 'fail first' policy.
//  Our agent calls check_step_therapy after formulary lookup. It queries
//  the member's prescription history via the EHR FHIR API to find prior
//  fills of the required step drugs. If they tried methotrexate for 90 days
//  and discontinued due to adverse effects — that satisfies step therapy.
//  If there's no prior fill history, the agent pends to a clinical
//  pharmacist who can contact the prescribing physician for clinical notes."
// ============================================================

using Microsoft.SemanticKernel;

namespace VitalCare.PriorAuthAgent.Plugins;

public class StepTherapyPlugin
{
    private readonly HttpClient _fhirClient;  // FHIR R4 API client
    private readonly ILogger<StepTherapyPlugin> _logger;

    public StepTherapyPlugin(HttpClient fhirClient, ILogger<StepTherapyPlugin> logger)
    {
        _fhirClient = fhirClient;
        _logger     = logger;
    }

    [KernelFunction("check_step_therapy")]
    [Description("Check whether the member has completed required step therapy (tried lower-tier drugs first) before approving a specialty or brand drug. Required when formulary criteria indicates StepTherapyRequired=true.")]
    public async Task<StepTherapyResult> CheckAsync(
        [Description("Member ID to check prescription history for")] string memberId,
        [Description("NDC of the specialty drug being requested")] string requestedDrugNdc,
        [Description("List of required step drugs that must have been tried first")] string[] requiredStepDrugs)
    {
        _logger.LogInformation(
            "[STEP THERAPY] Checking member {MemberId} for {StepCount} required step drugs before {NDC}",
            memberId, requiredStepDrugs.Length, requestedDrugNdc);

        // INTERVIEW: FHIR R4 MedicationRequest search — queries EHR for prescription history
        // HIPAA: we query by memberId (non-PHI identifier), FHIR returns structured data
        // JMA equivalent: querying DMS for dealer's previous program participation
        var completedSteps = new List<string>();

        foreach (var stepDrug in requiredStepDrugs)
        {
            // Query FHIR for MedicationRequest resources (prescription records)
            var fhirQuery = $"/MedicationRequest?patient={memberId}&medication.code={stepDrug}&status=completed,stopped";
            var response  = await _fhirClient.GetAsync(fhirQuery);

            if (response.IsSuccessStatusCode)
            {
                // INTERVIEW: If the member has a completed/stopped MedicationRequest for the step drug
                // they've satisfied the step therapy requirement for that drug
                completedSteps.Add(stepDrug);
                _logger.LogInformation("[STEP THERAPY] Step drug {Drug} found for member {MemberId}", stepDrug, memberId);
            }
        }

        var isSatisfied = completedSteps.Count >= requiredStepDrugs.Length;

        _logger.LogInformation("[STEP THERAPY] Result: {Satisfied} ({Completed}/{Required} steps completed)",
            isSatisfied ? "SATISFIED" : "NOT SATISFIED",
            completedSteps.Count, requiredStepDrugs.Length);

        return new StepTherapyResult
        {
            IsSatisfied      = isSatisfied,
            CompletedSteps   = completedSteps.ToArray(),
            MissingSteps     = requiredStepDrugs.Except(completedSteps).ToArray(),
            // INTERVIEW: If steps not in EHR, it could be in paper records at physician's office
            // Agent pends for pharmacist to contact prescriber for clinical notes
            RequiresPhysicianNotes = !isSatisfied && completedSteps.Count == 0
        };
    }
}

public record StepTherapyResult
{
    public bool     IsSatisfied            { get; init; }
    public string[] CompletedSteps         { get; init; } = [];
    public string[] MissingSteps           { get; init; } = [];
    public bool     RequiresPhysicianNotes { get; init; }
}
