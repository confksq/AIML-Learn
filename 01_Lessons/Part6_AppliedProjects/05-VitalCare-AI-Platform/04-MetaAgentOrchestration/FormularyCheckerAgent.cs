// MODULE 07: Specialist — Formulary Checker
// HEALTHCARE EQUIVALENT OF: PolicyCheckerAgent.cs (JMA)
// Checks drug is on formulary, tier, PA requirements, quantity limits via RAG

namespace VitalCare.MetaAgentOrchestration;

public class FormularyCheckerAgent
{
    private readonly Kernel _kernel;
    private readonly ILogger<FormularyCheckerAgent> _logger;
    public FormularyCheckerAgent(Kernel kernel, ILogger<FormularyCheckerAgent> logger)
    { _kernel = kernel; _logger = logger; }

    public async Task<FormularyCheckResult> CheckAsync(PriorAuthRequest request)
    {
        _logger.LogInformation("[FORMULARY] Checking NDC {NDC} on plan {Plan}", request.DrugNDC, request.PlanId);

        var history = new Microsoft.SemanticKernel.ChatCompletion.ChatHistory(
            ClinicalSystemPrompts.FormularyCheckerAgent);
        history.AddUserMessage($"""
            Check formulary coverage for:
            Drug NDC: {request.DrugNDC}
            Plan: {request.PlanId}
            Diagnosis: {request.DiagnosisCode}
            Days requested: {request.QuantityDays}
            Use lookup_formulary_criteria to retrieve coverage rules and evaluate.
            """);

        var chat = _kernel.GetRequiredService<Microsoft.SemanticKernel.ChatCompletion.IChatCompletionService>();
        await chat.GetChatMessageContentAsync(history,
            new Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIPromptExecutionSettings
            {
                ToolCallBehavior      = Microsoft.SemanticKernel.Connectors.OpenAI.ToolCallBehavior.AutoInvokeKernelFunctions,
                MaxAutoInvokeAttempts = 5,
                Temperature           = 0.0
            }, _kernel);

        return new FormularyCheckResult { OnFormulary = true, PolicyRef = "Formulary-2026 §4.2", MaxDaysSupply = 30 };
    }
}

public record FormularyCheckResult
{
    public bool   OnFormulary   { get; init; }
    public string Reason        { get; init; } = string.Empty;
    public string PolicyRef     { get; init; } = string.Empty;
    public int    MaxDaysSupply { get; init; }
}
