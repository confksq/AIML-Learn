// MODULE 05: HIPAA Gateway (APIM + PHI Audit)
// HEALTHCARE EQUIVALENT OF: APIMGateway.cs (JMA)
// KEY DIFFERENCE: HIPAA audit on every request, mTLS between services

namespace VitalCare.MCPHub;

public class HIPAAGateway
{
    private readonly HttpClient _http;
    private readonly ClinicalToolRegistry _registry;
    private readonly ILogger<HIPAAGateway> _logger;

    public HIPAAGateway(HttpClient http, ClinicalToolRegistry registry, ILogger<HIPAAGateway> logger)
    { _http = http; _registry = registry; _logger = logger; }

    public async Task<ClinicalToolResult> HandleRequestAsync(ClinicalToolCall call, string jwtToken)
    {
        // APIM Policy 1: JWT validation
        if (!ValidateToken(jwtToken, out var agentId))
            return ClinicalToolResult.Failure("Unauthorized");

        // APIM Policy 2: Rate limit (protect EHR/payer systems from overload)
        if (!await CheckRateLimitAsync(agentId))
            return ClinicalToolResult.Failure("Rate limit exceeded: max 60 clinical calls per minute");

        // INTERVIEW: HIPAA audit on EVERY request through the gateway
        // This is separate from the PHI-specific audit in the registry
        // Gateway audit = "who called what when"; Registry audit = "was PHI accessed?"
        _logger.LogInformation(
            "[HIPAA GATEWAY] Agent: {Agent} | Tool: {Tool} | Correlation: {Corr} | Time: {Time}",
            agentId, call.ToolName, call.CorrelationId, DateTime.UtcNow);

        return await _registry.InvokeAsync(call);
    }

    private bool ValidateToken(string jwt, out string agentId) { agentId = "agent"; return !string.IsNullOrEmpty(jwt); }
    private async Task<bool> CheckRateLimitAsync(string id) { await Task.CompletedTask; return true; }
}
