---
name: project-ai-assessment
description: "Enterprise AI Lead Assessment (Healthcare, Cloud-Agnostic) — competition the user is preparing to win; full breakdown of all 6 parts and 10 critical thinking questions"
metadata: 
  node_type: memory
  type: project
  originSessionId: d5d6585a-6ed6-4321-9af0-1d483592a176
---

User is preparing for a competitive "Enterprise AI Lead Assessment — Healthcare Edition (Cloud-Agnostic)" and wants to win it. Files (as of 2026-07-19 reorg) in `05_Assessments\` (repo root — personal `C:\pers\AIML-Learn\` or office `C:\Users\confksq\Project\AIML-Learn\`):
- `VitalCare_AssessmentBrief.txt` — the original brief
- `Assessment_Breakdown.md` — structured analysis
- `VitalCare_AI_Assessment_Response.md` — **the completed 101 KB submission**

**Why:** Personal competition with multiple participants; user wants deep understanding first, then build the solution together with Claude.

**How to apply:** Work through this assessment topic by topic. Always justify architecture decisions (why this, why not that). Never give generic AI answers — assessors explicitly reject them. Keep healthcare domain context (FHIR, HL7, HIPAA, prior auth, HEDIS) front and center.

---

## Assessment Meta

- **Role:** Enterprise AI Lead (7–10+ years)
- **Industry:** Healthcare — Provider, Payer, Digital Health
- **Client (fictional):** VitalCare Health Global — 180+ hospitals, 12M patients, multinational (US/EU/APAC)
- **Constraint:** Cloud-Agnostic — no AWS/Azure/GCP lock-in; every managed service needs a portable abstraction
- **Submission window:** 48 hours
- **Status:** ✅ **COMPLETE** — full response submitted (`VitalCare_AI_Assessment_Response.md`).
  A matching C# reference implementation exists at
  `01_Lessons/Part6_AppliedProjects/05-VitalCare-AI-Platform/` (9-layer agentic prior-auth platform).
  Retain this memory as a **reusable healthcare-AI architecture reference**, not as pending work.

---

## Evaluation Weights

| Area | Weight |
|---|---|
| Enterprise Architecture (cloud-agnostic rigor) | 20% |
| AI/GenAI Depth | 20% |
| Security, Privacy & Governance (HIPAA/GDPR/AI reg) | 15% |
| Business & Healthcare Domain Understanding | 10% |
| Scalability & Reliability | 10% |
| Cost Optimization | 10% |
| Critical Thinking & Tradeoffs | 10% |
| Deployment, DevOps & Portability | 5% |

---

## The 6 Parts

### Part 1 — Executive Summary (2 pages)
- Healthcare business understanding (provider, payer, digital health)
- Key pain points: 2hr/day charting, 3–7 day prior auth, expensive contact centers
- AI opportunity areas with patient-safety framing
- KPIs: -50% doc time, -30% contact center cost, prior auth <24hr for 80% cases, 60%+ self-service resolution
- Business/regulatory risks and assumptions
- Transformation roadmap
- **Trap:** Generic AI summaries get rejected

### Part 2 — Enterprise Architecture Design
**A. Business Architecture**
- Business domains: clinical, payer ops, member experience, research, corporate
- User personas: clinician, nurse, member, care manager, claims examiner, executive
- AI touchpoints across patient/member journey
- Workflows: encounter, prior auth, claims, appointment, triage
- Governance layers + PHI vs de-identified operational boundaries

**B. Logical Architecture Diagram (cloud-agnostic)**
- Data flow: HL7 v2, FHIR R4, X12 837/835/278, CDA
- AI orchestration, EHR/claims/CRM/telehealth/IVR integration
- Human feedback and clinical-review loops
- PHI containment zones clearly marked

**C. Cloud-Agnostic Deployment Architecture**
- Networking, zero-trust security, federated identity (SMART-on-FHIR for clinician SSO)
- AI inference (GPU-backed K8s, model servers), data services, streaming, CI/CD
- Disaster recovery, multi-region strategy with data residency awareness
- Every managed service must have a portable abstraction layer

---

### Part 3 — AI/GenAI Platform Strategy

**A. RAG Design**
- Chunking: semantic/structural/hierarchical for clinical guidelines, formularies, benefit docs
- Embedding: multilingual (14 languages), clinical/biomedical embeddings, refresh cadence
- Retrieval: dense vs hybrid vs graph, reranking, metadata filtering (jurisdiction/plan/specialty/date)
- Grounding: citations to source guidelines, deterministic retrieval contracts
- Hallucination mitigation: groundedness scoring, refusal patterns, confidence gates — critical since bad output = patient harm

**B. Model Strategy**
- Managed (GPT-4, Claude) vs open-weight (Llama, Mistral) vs healthcare-tuned (MedLM, Med-PaLM)
- Fine-tuning vs RAG vs prompt engineering vs LoRA/QLoRA
- PHI exposure risk — cannot send PHI to external APIs without a BAA
- Model routing: cheapest-fit-for-task; frontier only for high-stakes paths
- Latency: sub-second for in-consult; token streaming, speculative decoding

**C. Responsible AI & Clinical Safety**
- Guardrails: prompt injection, jailbreaks, toxicity, wrong dosing, hallucinated citations, PHI leakage in logs
- Bias monitoring: demographic, regional, payer cohort disparate impact

**D. Human-in-the-Loop**
- Escalation flows to nurse/clinician/licensed agent
- Confidence thresholds + uncertainty quantification
- Approval gates for: claim denials, dosing, referrals
- Full audit trail: prompt → retrieved context → model output → decision → reviewer identity

---

### Part 4 — Scalability & Operational Excellence

**Scalability:** Peak handling for flu season, open enrollment, telehealth surges; GPU autoscaling; global traffic routing

**Reliability:** 99.95% member-facing / 99.99% clinical; multi-region failover; graceful degradation chain: RAG → cached answers → rule-based → human; RPO/RTO per data class

**Cost Optimization:** Token compression, semantic caching (PHI-safe keying), model routing to cheapest fit, GPU batching/quantization, FinOps showback/chargeback

---

### Part 5 — Security & Compliance

| Area | Key Requirement |
|---|---|
| Regulatory | HIPAA, HITECH, GDPR, SOC 2 Type II, HITRUST, FDA SaMD/GMLP |
| Data Residency | US/EU/APAC regional isolation; cross-border PHI controls |
| PHI Protection | De-identification at ingest; redaction in prompts/responses; audit every PHI access |
| Encryption | At-rest + in-transit; customer-managed keys; envelope encryption |
| Secrets Management | Centralized store; short-lived creds; no secrets in prompts/notebooks |
| Network Security | Zero-trust; private endpoints; PHI zone isolation; service mesh (Istio/Linkerd) |
| Access Control | RBAC/ABAC; least privilege; just-in-time elevation; break-glass with full audit |
| AI Governance | Explainability, model versioning, evaluation gates, clinical sign-off |
| Supply Chain | Signed images, SBOMs, model provenance, dataset lineage |

---

### Part 6 — Delivery & Implementation Strategy

**Phased Roadmap:**
1. MVP — one use case (ambient doc), one region, narrow cohort
2. Pilot — expanded use cases, dual region, selected facilities
3. Enterprise Rollout — global, multi-region active-active, full residency compliance
4. Stabilization — FinOps, hardening, governance maturity, continuous clinical eval

**Risk Register categories:** Technical, Operational, Compliance, AI Governance, Adoption, Vendor lock-in

---

## 10 Mandatory Critical Thinking Questions

1. When NOT to use a frontier model API? (PHI sovereignty, latency, self-hosting)
2. Kubernetes vs Serverless for clinical workloads?
3. Vector search failure modes and clinical risk? (recall collapse, embedding drift, stale index, cold-start)
4. How to detect/monitor hallucinations in production? (groundedness scoring, citation verification, eval harnesses)
5. Four data tradeoffs: NoSQL vs PostgreSQL; data warehouse vs lakehouse (OMOP CDM); managed vs OSS vector DB; self-hosted vs managed LLM in HIPAA environment
6. Five failure scenario impacts: stream latency spike; LLM rate limits at peak; region outage mid-encounter; embedding drift vs updated guidelines; model deprecation mid-cycle
7. Redesign if PHI centralization becomes legally restricted (data localization laws)?
8. Top 5 hidden operational costs in healthcare GenAI?
9. When is agentic AI appropriate? (autonomy risk, rollback complexity, mandatory human checkpoints)
10. Rollback strategy for failed GenAI deployment (model, prompt, retrieval index, orchestration, side effects)

---

## Platform Capability Stack (must cover all with OSS + managed options)

Generative AI/LLMs, Agentic Orchestration, Healthcare Interoperability (FHIR/HL7/X12), Search & Retrieval (vector DB + hybrid), Compute (K8s + GPU inference), Storage (object + transactional + vector + lakehouse), Streaming (Kafka/FHIR events), Analytics (Iceberg/Delta/OMOP CDM), Feature Store/ML Platform, Security, AI Governance & Lineage, Identity (OIDC/SAML/SMART-on-FHIR), Monitoring (OpenTelemetry + LLM observability), API Management, Container Platform, Networking, Edge & Traffic Routing

---

## Win Conditions

- Show genuine healthcare domain knowledge — not AI buzzwords
- Every decision has "why this, why not that" justification
- Cloud-agnostic is real — portable abstractions, not just saying Kubernetes
- Clinical safety is woven through every layer
- Generic AI answers = disqualified

## Deliverables Required

- Executive Summary (mandatory, document)
- Business Architecture (mandatory, diagram)
- Logical Architecture (mandatory, diagram)
- Reference Deployment Architecture (mandatory, diagram with portable + managed variants)
- Security & Privacy Architecture (optional, diagram)
- AI Governance & Clinical Safety Framework (optional, document)
