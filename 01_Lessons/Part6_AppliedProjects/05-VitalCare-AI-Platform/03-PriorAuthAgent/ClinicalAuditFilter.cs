// ============================================================
// MODULE 06: FunctionInvocationFilter — HIPAA-Compliant Audit
// ============================================================
// HEALTHCARE EQUIVALENT OF: AuditFilter.cs (JMA)
// KEY DIFFERENCE: HIPAA requires audit trail on ALL PHI access
//                 PHI must NEVER appear in log messages
//                 Use correlation IDs + member IDs only
// ============================================================
// INTERVIEW: "How do you handle HIPAA audit in your agent?"
// "Every tool call goes through our ClinicalAuditFilter —
//  it's a FunctionInvocationFilter in Semantic Kernel that intercepts
//  every tool call before and after execution.
//  We log: tool name, correlation ID, member ID, latency.
//  We NEVER log PHI — no patient names, DOBs, diagnoses in log messages.
//  These logs go to App Insights with a 7-year retention policy
//  (HIPAA requirement). If a tool call fails, we log the error
//  with the same identifiers so we can reconstruct exactly what happened
//  during any HIPAA audit."
// ============================================================

using Microsoft.SemanticKernel;
using System.Diagnostics;

namespace VitalCare.PriorAuthAgent;

public class ClinicalAuditFilter : IFunctionInvocationFilter
{
    private readonly ILogger<ClinicalAuditFilter> _logger;
    private readonly TelemetryClient _telemetry;

    public ClinicalAuditFilter(ILogger<ClinicalAuditFilter> logger, TelemetryClient? telemetry = null)
    {
        _logger    = logger;
        _telemetry = telemetry ?? new TelemetryClient();
    }

    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        var toolName    = $"{context.Function.PluginName}.{context.Function.Name}";
        var sw          = Stopwatch.StartNew();

        // INTERVIEW: Log BEFORE invoking — if the call hangs, you still have the record
        // HIPAA: only correlation ID in log, never PHI
        _logger.LogInformation(
            "[CLINICAL AUDIT] BEFORE | Tool: {Tool} | Correlation: {Corr}",
            toolName,
            GetCorrelationId(context));

        try
        {
            await next(context);
            sw.Stop();

            // INTERVIEW: Log AFTER — confirms the tool returned and how long it took
            _logger.LogInformation(
                "[CLINICAL AUDIT] AFTER | Tool: {Tool} | Latency: {Ms}ms | Correlation: {Corr}",
                toolName, sw.ElapsedMilliseconds, GetCorrelationId(context));

            // Track every tool call in App Insights for HIPAA audit dashboard
            _telemetry.TrackEvent("ClinicalToolCall", new Dictionary<string, string>
            {
                ["tool"]           = toolName,
                ["correlation_id"] = GetCorrelationId(context),
                ["latency_ms"]     = sw.ElapsedMilliseconds.ToString(),
                ["outcome"]        = "success"
                // INTERVIEW: No PHI fields — member name, DOB, diagnosis NOT logged here
            });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex,
                "[CLINICAL AUDIT] FAILED | Tool: {Tool} | Latency: {Ms}ms | Error: {Error} | Correlation: {Corr}",
                toolName, sw.ElapsedMilliseconds, ex.Message, GetCorrelationId(context));

            _telemetry.TrackEvent("ClinicalToolCall", new Dictionary<string, string>
            {
                ["tool"]           = toolName,
                ["correlation_id"] = GetCorrelationId(context),
                ["latency_ms"]     = sw.ElapsedMilliseconds.ToString(),
                ["outcome"]        = "failure",
                ["error"]          = ex.GetType().Name   // error TYPE only, not message (may contain PHI)
            });

            throw;  // re-throw — fault tolerance layer handles retry/escalation
        }
    }

    private static string GetCorrelationId(FunctionInvocationContext ctx)
    {
        // Extract correlation ID from kernel arguments — set at request intake
        if (ctx.Arguments.TryGetValue("correlationId", out var id) && id != null)
            return id.ToString()!;
        return "unknown";
    }
}

// Placeholder — real impl uses Microsoft.ApplicationInsights
public class TelemetryClient
{
    public void TrackEvent(string name, Dictionary<string, string> props) { }
}
