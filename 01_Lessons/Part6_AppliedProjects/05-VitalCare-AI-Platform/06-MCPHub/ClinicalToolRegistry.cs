// ============================================================
// MODULE 05: MCP Hub — Clinical Tool Registry
// ============================================================
// HEALTHCARE EQUIVALENT OF: MCPToolRegistry.cs (JMA)
// JMA tools registered: DMS, Policy Search, DI
// HERE tools registered: FHIR EHR, Payer API, Formulary Search, Lab System
//
// KEY HEALTHCARE DIFFERENCE:
// Every tool access to PHI must be logged (HIPAA)
// MCP Hub is the single place to enforce PHI access logging
// across ALL agents — audit happens once here, not in each agent
// ============================================================
// INTERVIEW: "What tools are registered in your clinical MCP Hub?"
// "Four categories: EHR tools (read patient data via FHIR R4),
//  Payer tools (eligibility check, PA submission, formulary lookup),
//  Clinical reference tools (drug interaction checker, clinical guidelines RAG),
//  and notification tools (pharmacist escalation, provider notification).
//  The hub enforces PHI access logging on every tool call — one place,
//  not duplicated across 12 agents. APIM sits in front for JWT validation,
//  rate limiting, and HIPAA audit trail that meets 7-year retention."
// ============================================================

namespace VitalCare.MCPHub;

public class ClinicalToolRegistry
{
    private readonly Dictionary<string, ClinicalToolDefinition> _tools = new();
    private readonly ILogger<ClinicalToolRegistry> _logger;

    public ClinicalToolRegistry(ILogger<ClinicalToolRegistry> logger) => _logger = logger;

    public void RegisterTool(ClinicalToolDefinition tool)
    {
        _tools[tool.Name] = tool;
        _logger.LogInformation("[CLINICAL MCP] Registered: {Name} | PHI: {PHI} | Category: {Cat}",
            tool.Name, tool.AccessesPHI, tool.Category);
    }

    public IReadOnlyList<ClinicalToolDefinition> DiscoverTools(string? category = null) =>
        _tools.Values.Where(t => category == null || t.Category == category).ToList();

    public async Task<ClinicalToolResult> InvokeAsync(ClinicalToolCall call)
    {
        if (!_tools.TryGetValue(call.ToolName, out var tool))
            return ClinicalToolResult.Failure($"Tool '{call.ToolName}' not registered");

        // INTERVIEW: PHI access audit — log BEFORE invoking any PHI-accessing tool
        // This is the single enforcement point across all agents
        if (tool.AccessesPHI)
        {
            _logger.LogInformation(
                "[CLINICAL MCP PHI ACCESS] Tool: {Tool} | Agent: {Agent} | Correlation: {Corr} | Time: {Time}",
                call.ToolName, call.AgentId, call.CorrelationId, DateTime.UtcNow);
            // Production: write to HIPAA audit log with 7-year retention
        }

        try
        {
            var result = await tool.Handler(call.Parameters);
            return ClinicalToolResult.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CLINICAL MCP] Tool failed: {Tool}", call.ToolName);
            return ClinicalToolResult.Failure(ex.Message);
        }
    }

    // Called at startup to register all clinical tools
    public void RegisterDefaultTools()
    {
        // FHIR tool — accesses PHI
        RegisterTool(new ClinicalToolDefinition
        {
            Name        = "fhir.read_patient",
            Description = "Read patient demographics and active conditions from EHR via FHIR R4",
            Category    = "ehr",
            AccessesPHI = true,   // INTERVIEW: flag = PHI audit required
            Handler     = _ => Task.FromResult<object>(new { patient = "fhir_data" })
        });

        // Formulary search — no PHI
        RegisterTool(new ClinicalToolDefinition
        {
            Name        = "formulary.search",
            Description = "Search formulary coverage policies and tier information by drug NDC and plan ID",
            Category    = "formulary",
            AccessesPHI = false,
            Handler     = _ => Task.FromResult<object>(new { tier = 3, requiresPA = true })
        });

        // Payer eligibility — accesses PHI
        RegisterTool(new ClinicalToolDefinition
        {
            Name        = "payer.eligibility",
            Description = "Check member eligibility and benefits with the insurance payer",
            Category    = "payer",
            AccessesPHI = true,
            Handler     = _ => Task.FromResult<object>(new { eligible = true })
        });

        // Pharmacist notification — no PHI in message body (uses request ID)
        RegisterTool(new ClinicalToolDefinition
        {
            Name        = "notify.pharmacist",
            Description = "Escalate PA request to clinical pharmacist for review",
            Category    = "notification",
            AccessesPHI = false,
            Handler     = _ => Task.FromResult<object>(new { ticketId = $"PH-{Guid.NewGuid().ToString()[..8]}" })
        });
    }
}

public record ClinicalToolDefinition
{
    public string   Name        { get; init; } = string.Empty;
    public string   Description { get; init; } = string.Empty;
    public string   Category    { get; init; } = string.Empty;
    public bool     AccessesPHI { get; init; }   // INTERVIEW: drives PHI audit logging
    public Func<Dictionary<string, object>, Task<object>> Handler { get; init; } = _ => Task.FromResult<object>(new { });
}

public record ClinicalToolCall
{
    public string AgentId       { get; init; } = string.Empty;
    public string ToolName      { get; init; } = string.Empty;
    public string CorrelationId { get; init; } = string.Empty;
    public Dictionary<string, object> Parameters { get; init; } = new();
}

public record ClinicalToolResult
{
    public bool   Success { get; init; }
    public object Data    { get; init; } = new { };
    public string Error   { get; init; } = string.Empty;
    public static ClinicalToolResult Success(object data)  => new() { Success = true,  Data = data };
    public static ClinicalToolResult Failure(string error) => new() { Success = false, Error = error };
}
