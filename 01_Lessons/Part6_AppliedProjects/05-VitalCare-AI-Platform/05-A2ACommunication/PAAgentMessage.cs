// ============================================================
// MODULE 08: A2A Protocol — HIPAA-Compliant Typed Messages
// ============================================================
// HEALTHCARE EQUIVALENT OF: AgentMessage.cs (JMA)
// KEY HIPAA DIFFERENCE: Message payloads use member IDs only
//                       No patient name, DOB, SSN in message fields
//                       Full PHI lives in EHR — agents exchange IDs only
// ============================================================
// INTERVIEW: "How does A2A differ in a HIPAA environment?"
// "The message schema is designed with HIPAA in mind from the start.
//  Payloads only carry non-PHI identifiers — member ID, request ID,
//  NDC code, ICD-10 code. No patient names, no DOBs, no SSNs in
//  agent messages. The full PHI stays in the EHR; agents exchange
//  identifiers and use the FHIR API to look up PHI only when needed.
//  Every message is logged to a HIPAA audit trail with 7-year retention.
//  The envelope carries a correlation ID that threads all messages
//  for one PA request together in the audit log."
// ============================================================

namespace VitalCare.A2ACommunication;

// Same envelope pattern as JMA — CorrelationId + MessageId + Schema version
public record PAAgentMessage<TPayload>
{
    public string   MessageId     { get; init; } = Guid.NewGuid().ToString();
    public string   CorrelationId { get; init; } = string.Empty;   // = RequestId for full audit trail
    public string   SchemaVersion { get; init; } = "1.0";
    public string   SenderId      { get; init; } = string.Empty;
    public string   ReceiverId    { get; init; } = string.Empty;
    public string   MessageType   { get; init; } = string.Empty;
    public DateTime SentAt        { get; init; } = DateTime.UtcNow;
    public TPayload Payload       { get; init; } = default!;
}

// INTERVIEW: HIPAA-compliant payload — IDs only, never PHI field values
public record EligibilityResultPayload
{
    public string RequestId         { get; init; } = string.Empty;
    public string MemberId          { get; init; } = string.Empty;   // non-PHI identifier
    public bool   IsEligible        { get; init; }
    public string IneligibleReason  { get; init; } = string.Empty;
    // WHAT'S NOT HERE: patient name, DOB, SSN, address — those stay in EHR
}

public record FormularyResultPayload
{
    public string   RequestId       { get; init; } = string.Empty;
    public string   DrugNdc         { get; init; } = string.Empty;   // NDC = drug code, not PHI
    public bool     OnFormulary     { get; init; }
    public string   PolicyRef       { get; init; } = string.Empty;
    public string[] EvidenceChunks  { get; init; } = [];
}

public record ClinicalCriteriaPayload
{
    public string RequestId      { get; init; } = string.Empty;
    public string DiagnosisCode  { get; init; } = string.Empty;   // ICD-10 code, not diagnosis text
    public bool   MeetsCriteria  { get; init; }
    public bool   IsAmbiguous    { get; init; }
    public string PolicyRef      { get; init; } = string.Empty;
}

public record PharmacistEscalationPayload
{
    public string RequestId    { get; init; } = string.Empty;
    public string Reason       { get; init; } = string.Empty;
    public string Priority     { get; init; } = string.Empty;   // ROUTINE | URGENT
    public string TicketId     { get; init; } = string.Empty;
    // WHAT'S NOT HERE: patient contact info — pharmacist retrieves from EHR using RequestId
}

public enum PAMessageType
{
    EligibilityRequest,
    EligibilityResult,
    FormularyCheckRequest,
    FormularyCheckResult,
    ClinicalCriteriaRequest,
    ClinicalCriteriaResult,
    SupervisorDecision,
    PharmacistEscalation,
    EscalationAcknowledged
}
