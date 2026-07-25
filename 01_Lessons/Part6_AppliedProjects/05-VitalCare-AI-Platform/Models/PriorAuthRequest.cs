// Shared model types used across all VitalCare modules
// HEALTHCARE: Equivalent of ClaimRequest in JMA Dealer Intelligence

namespace VitalCare;

public record PriorAuthRequest
{
    public string  RequestId      { get; init; } = string.Empty;
    public string  MemberId       { get; init; } = string.Empty;  // NOT patient name — HIPAA
    public string  ProviderId     { get; init; } = string.Empty;  // NPI number
    public string  DrugNDC        { get; init; } = string.Empty;  // NDC = National Drug Code
    public string  DiagnosisCode  { get; init; } = string.Empty;  // ICD-10 code
    public string  PlanId         { get; init; } = string.Empty;  // Insurance plan identifier
    public int     QuantityDays   { get; init; }                   // Days supply requested
    public DateTime RequestDate   { get; init; }
}

// JMA → Healthcare mapping:
// ClaimId     → RequestId
// DealerId    → MemberId (patient's insurance member ID, non-PHI identifier)
// VehicleVin  → DrugNDC (unique code for the specific drug + dose)
// ProgramCode → PlanId (insurance plan determines coverage rules)
// ClaimAmount → QuantityDays (what's being authorized)
// SaleDate    → RequestDate

public record PADecisionResponse
{
    public string Status      { get; init; } = string.Empty;  // approved | denied | pended
    public string Rationale   { get; init; } = string.Empty;
    public string PolicyRef   { get; init; } = string.Empty;
    public string AuthNumber  { get; init; } = string.Empty;
    public int    AuthDays    { get; init; }                   // approved days supply
}
