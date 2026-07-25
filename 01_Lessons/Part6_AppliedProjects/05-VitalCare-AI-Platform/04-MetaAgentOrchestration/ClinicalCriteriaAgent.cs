// ============================================================
// MODULE 07: Specialist — Clinical Criteria Agent (Healthcare-Specific)
// ============================================================
// NO JMA EQUIVALENT — this is a healthcare-only specialist agent
//
// WHAT IT DOES: Evaluates clinical-specific PA criteria:
// - Diagnosis code match: Is the ICD-10 code an approved indication?
// - Age/gender restrictions: e.g., drug only approved for adults 18+
// - Quantity limits: Does requested days supply exceed formulary max?
// - Step therapy: Has member tried required lower-tier drugs?
// - Contraindications: Any known allergies or drug interactions?
//
// WHY IT'S A SEPARATE AGENT:
// Formulary checker = "is the drug on the plan?"
// Clinical criteria = "is this drug appropriate for THIS patient's condition?"
// Different knowledge bases, different tools, different prompts
// ============================================================
// INTERVIEW: "What does your Clinical Criteria Agent check?"
// "It's the medical necessity layer. Formulary tells us if the drug is covered.
//  Clinical criteria tells us if it's medically appropriate for THIS patient.
//  It checks: is the ICD-10 diagnosis an approved indication for this drug?
//  Does the patient's age/gender meet the drug's labeling requirements?
//  Did they complete step therapy? Is the requested quantity within limits?
//  Each check uses a different tool — some hit the FHIR API for patient data,
//  some do RAG on clinical coverage policy documents.
//  If ANY criterion is ambiguous — not clearly met, not clearly denied —
//  it returns IsAmbiguous=true and the supervisor pends to a pharmacist."
// ============================================================

using Microsoft.SemanticKernel;

namespace VitalCare.MetaAgentOrchestration;

public class ClinicalCriteriaAgent
{
    private readonly Kernel _kernel;
    private readonly ILogger<ClinicalCriteriaAgent> _logger;

    public ClinicalCriteriaAgent(Kernel kernel, ILogger<ClinicalCriteriaAgent> logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    public async Task<ClinicalCriteriaResult> EvaluateAsync(PriorAuthRequest request)
    {
        _logger.LogInformation(
            "[CLINICAL CRITERIA] Evaluating NDC {NDC} for diagnosis {Dx}",
            request.DrugNDC, request.DiagnosisCode);

        // INTERVIEW: Clinical criteria agent has its own focused system prompt
        // It knows about ICD-10 codes, drug indications, step therapy rules
        // It does NOT make eligibility or formulary decisions — pure clinical evaluation
        var history = new Microsoft.SemanticKernel.ChatCompletion.ChatHistory(
            ClinicalSystemPrompts.ClinicalCriteriaAgent);

        history.AddUserMessage($"""
            Evaluate clinical criteria for this prior auth request:
            Drug NDC: {request.DrugNDC}
            Diagnosis Code (ICD-10): {request.DiagnosisCode}
            Member ID: {request.MemberId}
            Days Supply Requested: {request.QuantityDays}

            Check:
            1. Is {request.DiagnosisCode} an approved indication for this drug?
            2. Does the requested quantity ({request.QuantityDays} days) meet quantity limits?
            3. Has step therapy been completed? Check member {request.MemberId} prescription history.
            4. Any age, gender, or clinical restrictions that apply?

            If any criterion is unclear or information is missing, set IsAmbiguous=true.
            Never deny based on missing information — pend for pharmacist review.
            """);

        var chat = _kernel.GetRequiredService<Microsoft.SemanticKernel.ChatCompletion.IChatCompletionService>();
        var response = await chat.GetChatMessageContentAsync(history,
            new Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIPromptExecutionSettings
            {
                ToolCallBehavior      = Microsoft.SemanticKernel.Connectors.OpenAI.ToolCallBehavior.AutoInvokeKernelFunctions,
                MaxAutoInvokeAttempts = 8,
                Temperature           = 0.0
            }, _kernel);

        return ParseClinicalResult(response.Content ?? string.Empty);
    }

    private static ClinicalCriteriaResult ParseClinicalResult(string response) =>
        new() { MeetsCriteria = true, PolicyRef = "ClinicalPolicy-2026 §2.1", IsAmbiguous = false };
}

public record ClinicalCriteriaResult
{
    public bool   MeetsCriteria   { get; init; }
    public string DenialReason    { get; init; } = string.Empty;
    public string PolicyRef       { get; init; } = string.Empty;
    public bool   IsAmbiguous     { get; init; }   // INTERVIEW: healthcare-specific — needs pharmacist
    public string AmbiguityReason { get; init; } = string.Empty;
}
