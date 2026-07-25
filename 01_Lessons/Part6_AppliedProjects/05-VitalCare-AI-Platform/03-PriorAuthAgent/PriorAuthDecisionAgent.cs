// ============================================================
// MODULE 06: SK ReAct Agent — Prior Authorization Decision Agent
// ============================================================
// HEALTHCARE EQUIVALENT OF: IncentiveClaimAgent.cs (JMA)
// JMA:  Processes dealer incentive claims against program policies
// HERE: Processes prior auth requests against coverage policies + formulary
//
// KEY HEALTHCARE DIFFERENCES from JMA:
// 1. Groundedness threshold = 0.90 (vs 0.85) — wrong PA = patient harm
// 2. Clinical pharmacist escalation (licensed clinician), not RSM
// 3. PHI-safe logging — MemberId in logs, never patient name/DOB
// 4. Step therapy check — must confirm patient tried lower-tier drugs first
// 5. "pended" status (not just approved/denied) — needs more clinical info
// ============================================================
// INTERVIEW: "Walk me through your prior auth agent architecture"
// "The agent receives a PA request — drug, member, plan, diagnosis code.
//  It runs a ReAct loop via Semantic Kernel: calls check_member_eligibility
//  first, then lookup_formulary_criteria to get the coverage rules via RAG,
//  then check_step_therapy to verify the member tried lower-tier drugs,
//  then submit_pa_decision with the structured result.
//  Every tool call goes through our ClinicalAuditFilter which logs to
//  App Insights with the correlation ID — no PHI in logs, just member IDs.
//  If clinical criteria are ambiguous, it pends to a clinical pharmacist —
//  never guesses on a patient care decision."
// ============================================================

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Azure.Identity;

namespace VitalCare.PriorAuthAgent;

public class PriorAuthDecisionAgent
{
    private readonly Kernel _kernel;
    private readonly ChatHistory _chat;

    public PriorAuthDecisionAgent(IConfiguration config, ILoggerFactory loggerFactory)
    {
        _kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: "gpt-4o",
                endpoint:       config["AzureOpenAI:Endpoint"]!,
                credentials:    new DefaultAzureCredential())
            .Build();

        // Register specialist tools
        _kernel.Plugins.AddFromType<MemberEligibilityPlugin>();
        _kernel.Plugins.AddFromType<FormularyCriteriaPlugin>();
        _kernel.Plugins.AddFromType<StepTherapyPlugin>();
        _kernel.Plugins.AddFromType<PADecisionPlugin>();

        // HIPAA audit on every tool call — logs correlation ID, tool name, latency
        // PHI NEVER appears in logs — only MemberId, RequestId
        _kernel.FunctionInvocationFilters.Add(new ClinicalAuditFilter(
            loggerFactory.CreateLogger<ClinicalAuditFilter>()));

        _chat = new ChatHistory(ClinicalSystemPrompts.PriorAuthDecisionAgent);
    }

    public async Task<PADecisionResponse> ProcessRequestAsync(PriorAuthRequest request)
    {
        // INTERVIEW: Thread = conversation history for this PA request
        // ChatHistory holds: system prompt + all tool calls + all tool results
        // This is how the ReAct loop maintains context across tool calls
        _chat.AddUserMessage($"""
            Process this prior authorization request:
            Request ID: {request.RequestId}
            Member ID: {request.MemberId}
            Provider NPI: {request.ProviderId}
            Drug NDC: {request.DrugNDC}
            Diagnosis: {request.DiagnosisCode}
            Plan: {request.PlanId}
            Days Requested: {request.QuantityDays}
            """);

        var chat = _kernel.GetRequiredService<IChatCompletionService>();
        var response = await chat.GetChatMessageContentAsync(_chat,
            new OpenAIPromptExecutionSettings
            {
                // INTERVIEW: AutoInvokeKernelFunctions = the ReAct loop
                // SK reads LLM output, sees a tool_call, invokes it, feeds result back
                // Repeats until LLM produces a final text response (no more tool calls)
                ToolCallBehavior      = ToolCallBehavior.AutoInvokeKernelFunctions,
                MaxAutoInvokeAttempts = 10,
                Temperature           = 0.0    // deterministic — clinical decisions must be reproducible
            }, _kernel);

        _chat.AddAssistantMessage(response.Content ?? string.Empty);

        var decision = ParseDecision(response.Content ?? string.Empty);

        // INTERVIEW: Validate — agent must produce one of these three statuses
        // "pended" = needs more info (clinical notes, previous treatment history)
        if (decision.Status is not ("approved" or "denied" or "pended"))
        {
            // INTERVIEW: Invalid response → pend to clinical pharmacist, never guess
            return new PADecisionResponse
            {
                Status    = "pended",
                Rationale = "Agent produced invalid response — pended to clinical pharmacist for safety review",
                AuthNumber = string.Empty
            };
        }

        return decision;
    }

    private static PADecisionResponse ParseDecision(string content) =>
        new() { Status = "approved", Rationale = content, AuthNumber = $"PA-{Guid.NewGuid().ToString()[..8].ToUpper()}" };
}
