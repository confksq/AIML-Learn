// MODULE 10: Clinical Escalation Service
// HEALTHCARE EQUIVALENT OF: EscalationService.cs (JMA)
// JMA: Escalates to RSM (business reviewer)
// HERE: Escalates to LICENSED CLINICAL PHARMACIST
// KEY DIFFERENCE: Pharmacist is a licensed clinician — legal requirement
//                 for certain PA decisions. RSM is a business role.

namespace VitalCare.FaultTolerance;

public class ClinicalEscalationService
{
    private readonly IEmailService  _email;
    private readonly ILogger<ClinicalEscalationService> _logger;

    public ClinicalEscalationService(IEmailService email, ILogger<ClinicalEscalationService> logger)
    { _email = email; _logger = logger; }

    // INTERVIEW: "Who reviews escalated PAs?"
    // "A licensed clinical pharmacist. Not a business reviewer, not an on-call engineer —
    //  a clinician. For PA decisions involving clinical criteria, a licensed pharmacist
    //  must be in the loop. That's both a legal requirement in many states and a
    //  patient safety requirement. The agent routes to them, they have full context
    //  via the ticket, and they can approve/deny/request physician notes."
    public async Task<ClinicalTicket> EscalateToPharmacistAsync(ClinicalEscalationRequest request)
    {
        var ticket = new ClinicalTicket
        {
            TicketId   = $"CRX-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            RequestId  = request.RequestId,
            Reason     = request.Reason,
            Priority   = request.Priority,
            CreatedAt  = DateTime.UtcNow
        };

        _logger.LogWarning(
            "[CLINICAL ESCALATION] {TicketId} | RequestId: {ReqId} | Priority: {Pri} | Reason: {Reason}",
            ticket.TicketId, ticket.RequestId, ticket.Priority, ticket.Reason);

        // Notify clinical pharmacist — they retrieve patient details from EHR using RequestId
        // PHI is NOT in the notification body — pharmacist looks up in their clinical system
        await _email.SendAsync(
            "pharmacist-oncall@vitalcare.com",
            $"[{ticket.Priority}] PA {ticket.RequestId} requires clinical review — {ticket.TicketId}",
            $"""
            A prior authorization request requires clinical pharmacist review.

            Ticket: {ticket.TicketId}
            Request ID: {ticket.RequestId}
            Priority: {ticket.Priority}
            Reason for escalation: {ticket.Reason}

            Please review in the clinical portal using Request ID {ticket.RequestId}.
            Do NOT reply with PHI to this email.
            """);

        return ticket;
    }
}

public record ClinicalEscalationRequest
{
    public string RequestId { get; init; } = string.Empty;
    public string Reason    { get; init; } = string.Empty;
    public string Priority  { get; init; } = "ROUTINE";   // ROUTINE | URGENT
}

public record ClinicalTicket
{
    public string   TicketId  { get; init; } = string.Empty;
    public string   RequestId { get; init; } = string.Empty;
    public string   Reason    { get; init; } = string.Empty;
    public string   Priority  { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public interface IEmailService { Task SendAsync(string to, string subject, string body); }
