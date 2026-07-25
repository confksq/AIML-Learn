# VitalCare AI Platform

> **The healthcare translation of JMA Dealer Intelligence Platform.**
> Every module, every concept, every pattern — re-expressed in clinical terms.
> This is the project you describe in the Ascendion interview.

---

## Why This Project Exists

The Ascendion role is at a **healthcare AI company**. Your background is at JMA (automotive distribution). The bridge is this project.

**Learning strategy:**
1. You built JMA Dealer Intelligence first (domain you know deeply)
2. This project re-maps every component to healthcare terminology
3. In the interview, you describe *this* project and use JMA internally to check your understanding

---

## What This System Does

```
Prior Auth PDF Form
       |
       v
[Azure Document Intelligence]   ← extracts structured prior auth request
       |
       v
[Prior Auth Intake Agent]        ← validates: patient, provider, drug info
       |
       v
[Supervisor Agent]               ← breaks into sub-tasks, delegates
    /     |     \
   /      |      \
[Eligibility  [Formulary    [Clinical Criteria
  Checker]     Checker]       Agent]
       \      |      /
        \     |     /
         \    v    /
    [Prior Auth Decision]        ← approved | denied | pended
          |
          v
    [Groundedness Monitor]       ← HIPAA-compliant audit + quality scoring
```

**The business problem:**
Payers (insurance companies) receive thousands of Prior Authorization (PA) requests per day.
Each PA request asks: "Is this drug/procedure covered for this patient under their plan?"
Manual review takes 24-72 hours. VitalCare automates this to minutes while maintaining
clinical accuracy and HIPAA compliance.

---

## Domain Mapping: JMA → Healthcare

| JMA Concept | Healthcare Equivalent | Why the Same |
|---|---|---|
| Dealer Incentive Claim | Prior Authorization Request | Both: structured request for approval against policy criteria |
| DMS (Dealer Mgmt System) | EHR / EMR (Epic, Cerner) | Both: system of record for the entity |
| Incentive Policy PDF | Clinical Coverage Policy / Formulary | Both: policy docs that determine eligibility |
| Regional Sales Manager | Clinical Pharmacist / Medical Director | Both: human reviewer for escalated/unclear cases |
| Dealer Eligibility | Patient Eligibility (insurance coverage) | Both: "is this entity in good standing?" |
| VIN (Vehicle ID) | NPI (National Provider Identifier) | Both: unique ID for the subject of the request |
| Program Code (e.g., GC-2026-Q1) | NDC / CPT Code | Both: code identifying the specific program/drug/procedure |
| Fraud Detection | Clinical Anomaly Detection | Both: outlier patterns that suggest abuse |
| Confidence Routing (>0.90 auto) | PA Confidence Routing | Both: auto-approve high confidence, human review for uncertain |
| Circuit Breaker → RSM | Circuit Breaker → Clinical Pharmacist | Both: service outage → human escalation |

---

## Module Coverage Map

| Directory | Module | Interview Concept | Healthcare Specifics |
|---|---|---|---|
| `01-DocumentPipeline/` | 09 | OCR/Document Intelligence | PHI extraction, HIPAA field handling |
| `01-DocumentPipeline/` | Gap | Chunking Strategies | Clinical guideline paragraph chunking |
| `02-RAGSearch/` | Gap | HNSW Vector Search | Formulary + coverage policy retrieval |
| `02-RAGSearch/` | Gap | Hybrid Retrieval + RRF | Drug code (exact) + clinical meaning (semantic) |
| `03-PriorAuthAgent/` | 06 | SK ReAct Agent Loop | Clinical tool call sequence |
| `04-MetaAgentOrchestration/` | 07 | Meta-Agent Hierarchy | Supervisor → Eligibility + Formulary + Clinical |
| `05-A2ACommunication/` | 08 | A2A Protocol | HIPAA-compliant typed messages between agents |
| `06-MCPHub/` | 05 | MCP Hub + APIM | HIPAA audit on every tool call |
| `07-FaultTolerance/` | 10 | Retry + Circuit Breaker | Payer API outage → clinical escalation |
| `08-PromptEngineering/` | Gap | System Prompt Design | PHI-safe prompts, clinical scope constraints |
| `09-LLMOps/` | Gap+11 | Eval + Groundedness | Clinical quality gate, pharmacist sign-off |

---

## Key Healthcare-Specific Differences from JMA

### 1. PHI (Protected Health Information)
Every document, every message, every log must handle PHI:
- **No PHI in logs** — mask patient names, MRNs, DOBs
- **Encrypted in transit and at rest** — Azure encryption, always
- **Audit trail** — who accessed what PHI and when (HIPAA requirement)
- **Data minimization** — only extract the PHI fields you actually need

```csharp
// WRONG — PHI in log
_logger.LogInformation("Processing PA for patient {Name} DOB {DOB}", patient.Name, patient.DOB);

// RIGHT — use non-PHI identifiers in logs
_logger.LogInformation("Processing PA {RequestId} for member {MemberId}", request.Id, request.MemberId);
```

### 2. HIPAA Compliance
- All inter-agent messages must be logged (audit requirement)
- A2A messages use HIPAA-compliant identifiers (member IDs, not names)
- Groundedness monitoring is a **clinical safety feature**, not just quality
- Human escalation to **clinical pharmacist** (licensed clinician) not RSM

### 3. FHIR (Fast Healthcare Interoperability Resources)
- Healthcare data exchange standard
- Patient data comes from EHR via FHIR R4 API
- PA requests may reference FHIR MedicationRequest or ServiceRequest resources

```csharp
// Example: reading patient eligibility from EHR FHIR API
// JMA equivalent: reading dealer enrollment from DMS API
var patient = await _fhirClient.ReadAsync<Patient>($"Patient/{memberId}");
```

### 4. Clinical Decision Support (CDS) Rules
- **Never fine-tune medical knowledge** — RAG only, always citable
- **Step therapy** — must try lower-cost alternatives first
- **Quantity limits** — max days supply, max units per authorization
- **Age/gender requirements** — some drugs only appropriate for certain populations

### 5. Groundedness = Patient Safety
- In JMA: groundedness failure = wrong dealer decision → financial risk
- In Healthcare: groundedness failure = wrong PA decision → patient harm
- Clinical groundedness threshold: **0.90** (vs 0.85 for JMA)
- Groundedness drops alert clinical quality officer, not just on-call eng

---

## Tech Stack

```
Azure OpenAI (GPT-4o)            ← Clinical reasoning LLM
Azure OpenAI (text-embedding-3-large) ← Embed formulary + coverage docs
Azure AI Search (HNSW)           ← Formulary/guideline vector retrieval
Azure Document Intelligence      ← PA form OCR + PHI extraction
Semantic Kernel (C#)             ← Agent orchestration + ReAct loop
Azure API Management             ← HIPAA audit gateway
Azure Service Bus                ← A2A message routing
Azure Key Vault                  ← PHI encryption keys
Microsoft.Identity (Managed)     ← DefaultAzureCredential, no stored secrets
Polly                            ← Retry + circuit breaker for payer APIs
Application Insights             ← Groundedness monitoring + PHI-safe audit logs
```

---

## Interview Bridge Quote

> "At JMA I designed and built a multi-agent incentive claim system — agents
> that process dealer program eligibility through RAG on policy documents,
> with HNSW-indexed vector search, A2A typed communication between specialist
> agents, and a full LLMOps pipeline with automated groundedness scoring as
> the deployment quality gate.
>
> I mapped that entire architecture to the prior authorization problem in VitalCare —
> same patterns, same modules, but with HIPAA constraints on every component:
> PHI-masked logging, FHIR-compliant data access, and a clinical pharmacist
> escalation path instead of RSM. The groundedness threshold is 0.90 in clinical
> because a poorly-grounded PA decision is a patient safety event, not just a
> financial error."

---

## Project Structure

```
05-VitalCare-AI-Platform/
├── README.md                          ← This file — read before the interview
│
├── 01-DocumentPipeline/
│   ├── PriorAuthFormExtractor.cs      ← Azure DI, PHI-safe extraction, confidence routing
│   └── ClinicalGuidelineChunker.cs   ← Paragraph chunking for clinical PDFs
│
├── 02-RAGSearch/
│   ├── FormularyVectorSearch.cs       ← HNSW index for formulary + coverage policies
│   └── HybridClinicalRetrieval.cs    ← Hybrid: drug code (keyword) + clinical meaning (vector)
│
├── 03-PriorAuthAgent/
│   ├── PriorAuthDecisionAgent.cs      ← SK ReAct agent — clinical tool call sequence
│   └── ClinicalAuditFilter.cs        ← FunctionInvocationFilter with HIPAA audit logging
│
├── 04-MetaAgentOrchestration/
│   ├── PASupervisorAgent.cs           ← Delegates: Eligibility + Formulary + ClinicalCriteria
│   ├── EligibilityCheckerAgent.cs    ← Checks patient coverage and plan eligibility
│   ├── FormularyCheckerAgent.cs      ← Checks if drug is on formulary, tier, restrictions
│   └── ClinicalCriteriaAgent.cs     ← Evaluates clinical criteria (step therapy, age, diagnosis)
│
├── 05-A2ACommunication/
│   ├── PAAgentMessage.cs             ← HIPAA-compliant typed messages (member IDs, not PHI)
│   └── ClinicalAgentBus.cs          ← HIPAA audit on every inter-agent message
│
├── 06-MCPHub/
│   ├── ClinicalToolRegistry.cs       ← Registers: FHIR tool, Payer API, Formulary tool
│   └── HIPAAGateway.cs               ← APIM + HIPAA audit + PHI access logging
│
├── 07-FaultTolerance/
│   ├── PayerAPIRetryPolicy.cs        ← Retry with backoff for payer eligibility APIs
│   ├── PayerCircuitBreaker.cs        ← Circuit opens → all PAs escalate to pharmacist
│   └── ClinicalEscalationService.cs ← Routes to licensed clinical pharmacist, not RSM
│
├── 08-PromptEngineering/
│   └── ClinicalSystemPrompts.cs     ← PHI-safe, scope-restricted clinical agent prompts
│
└── 09-LLMOps/
    ├── ClinicalEvalPipeline.cs       ← 100 golden PA test cases, threshold 0.90
    ├── ClinicalGroundednessMonitor.cs ← Patient safety alert when groundedness drops
    └── ClinicalPromptVersioning.cs  ← Prompts in Git, pharmacist sign-off required
```
