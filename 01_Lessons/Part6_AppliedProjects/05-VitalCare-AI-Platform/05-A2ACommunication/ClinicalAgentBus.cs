// MODULE 08: Clinical Agent Bus — HIPAA Audit on Every Message
// HEALTHCARE EQUIVALENT OF: AgentBus.cs (JMA)
// KEY DIFFERENCE: 7-year HIPAA audit retention, PHI-safe logging

namespace VitalCare.A2ACommunication;

public class ClinicalAgentBus
{
    private readonly ILogger<ClinicalAgentBus> _logger;
    private readonly TelemetryClient _telemetry;

    public ClinicalAgentBus(ILogger<ClinicalAgentBus> logger, TelemetryClient telemetry)
    { _logger = logger; _telemetry = telemetry; }

    public async Task SendAsync<TPayload>(
        PAAgentMessage<TPayload> message,
        Func<PAAgentMessage<TPayload>, Task> handler)
    {
        // INTERVIEW: Log before delivery — HIPAA requires proof message was sent
        // 7-year retention on these logs (HIPAA requirement)
        LogHIPAAAudit("SENT", message);

        try
        {
            await handler(message);
            LogHIPAAAudit("DELIVERED", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "[CLINICAL A2A] FAILED | MsgId: {MsgId} | From: {From} → To: {To}",
                message.MessageId, message.SenderId, message.ReceiverId);

            // INTERVIEW: Dead letter — undelivered clinical messages are critical
            // Production: Azure Service Bus dead letter queue → clinical ops alert
            await DeadLetterAsync(message, ex.Message);
            throw;
        }
    }

    private void LogHIPAAAudit<TPayload>(string action, PAAgentMessage<TPayload> message)
    {
        // INTERVIEW: Every field here is non-PHI — safe to log
        // CorrelationId (RequestId) threads all messages for one PA request in audit log
        _logger.LogInformation(
            "[CLINICAL AUDIT A2A] {Action} | MsgId: {MsgId} | Correlation: {Corr} | " +
            "From: {From} → To: {To} | Type: {Type} | Schema: {Schema}",
            action, message.MessageId, message.CorrelationId,
            message.SenderId, message.ReceiverId, message.MessageType, message.SchemaVersion);

        _telemetry.TrackEvent($"ClinicalA2A_{action}", new Dictionary<string, string>
        {
            ["message_id"]     = message.MessageId,
            ["correlation_id"] = message.CorrelationId,
            ["sender"]         = message.SenderId,
            ["receiver"]       = message.ReceiverId,
            ["message_type"]   = message.MessageType
            // NO PHI fields — not member name, not diagnosis text
        });
    }

    private async Task DeadLetterAsync<TPayload>(PAAgentMessage<TPayload> message, string error)
    {
        _logger.LogCritical(
            "[CLINICAL DEAD LETTER] MsgId: {MsgId} | Correlation: {Corr} | Error: {Error}",
            message.MessageId, message.CorrelationId, error);
        await Task.CompletedTask;
    }
}
