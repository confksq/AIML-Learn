# Module 11 — Defend the VitalCare Assessment



---

## Why This Module Matters

The interviewer HAS READ your 43-page VitalCare AI Platform assessment. They did not invite you to discuss a hypothetical — they invited you to defend a specific architecture you already proposed. This module is not new content. It's your bridge between what you submitted and what you know from JM Family production.

The key insight: **every VitalCare architecture decision maps directly to something you've built or designed at JM Family.** Your answers are not theoretical — they're "I built this, here's what I changed for the clinical context."

---

## Section 1 — The JMA → VitalCare Domain Mapping

When the interviewer probes a VitalCare decision, anchor it to JM Family first, then explain the healthcare delta.

| JM Family Concept | VitalCare Equivalent | Healthcare Delta |
|------------------|---------------------|-----------------|
| Dealer form extraction (DI) | Prior auth form extraction (DI) | Confidence threshold: 0.85 → 0.95 (PHI consequence) |
| Azure AI Search hybrid RAG | Clinical knowledge base RAG | Semantic re-ranking required (clinical specificity matters) |
| Semantic Kernel agent loop | Prior Auth Agent (SK) | StepTherapyPlugin added (no JMA equivalent) |
| ChatHistory → Cosmos DB | Session state → Cosmos DB | TTL shortened (PHI retention minimization) |
| App Insights groundedness | Groundedness monitoring | Threshold: 0.85 → 0.90 |
| `FunctionInvocationFilter` | `FunctionInvocationFilter` + PHI masking | Member ID only in logs — no name, DOB, diagnosis |
| SharePoint document feed | FHIR R4 API + EHR integration | Real-time patient data vs batch document sync |
| Incentive claim = APPROVED/DENIED | Prior auth = APPROVED/DENIED/PENDED | "Pended" is clinical ambiguity — no JMA equivalent |
| Escalation → RSM (regional sales manager) | Escalation → Pharmacist | License requirement for clinical review |
| RBAC on AI Search | RBAC + row-level security | Patient-specific record isolation (HIPAA minimum necessary) |
| No audit log retention SLA | 7-year HIPAA audit retention | Append-only immutable audit store |

---

## Section 2 — The Key Decisions You Must Defend

These are the decisions the interviewer is most likely to probe. Know the WHY for each.

### Decision 1 — Why Azure Document Intelligence over John Snow Labs for form extraction?

**Your answer:**
"Prior auth forms are structured — fixed field positions, checkboxes, signature lines. Azure DI's Custom Template model handles this with >95% accuracy out of the box after training on 10-15 labeled samples per form type. John Snow Labs wins on clinical narrative — SOAP notes, discharge summaries — where medical NER matters. The forms going into prior auth are structured, not narrative. DI is the right tool. If we add clinical note processing to the pipeline (ambient documentation, discharge planning), JSL enters at that stage."

### Decision 2 — Why Semantic Kernel for agent orchestration over LangGraph?

**Your answer:**
"The platform is Azure-native and the development team is predominantly .NET. Semantic Kernel gives us Managed Identity, Content Safety, and Azure AI Search as first-class connectors — not bolted-on integrations. LangGraph is Python-only, which would split the team. The one place LangGraph would win is the prior auth step therapy decision tree — it has complex conditional branching with human-in-the-loop interrupts and crash recovery. For that specific workflow, I'd use LangGraph in isolation. The broader orchestration platform is Semantic Kernel."

### Decision 3 — Why RAG over fine-tuning for the clinical knowledge base?

**Your answer:**
"Three reasons. First, clinical policies change constantly — formulary updates, payer rule changes, new treatment guidelines. Fine-tuned models have a knowledge cutoff at training time; RAG retrieves current documents every call. Second, audibility — a RAG system can cite which document supported which answer. In a HIPAA audit, 'the AI used document X version 3.2 to make this recommendation' is defensible. 'The model learned it during training' is not. Third, cost and speed — fine-tuning GPT-4o for this domain would cost tens of thousands and take weeks. RAG with Azure AI Search is production-ready in days."

### Decision 4 — Why hybrid MCP + APIM over direct API calls?

**Your answer:**
"180 hospitals, 12 agent types, each hospital with potentially different EHR versions. Direct API calls means 2,160 custom integrations to maintain. MCP Hub provides tool discovery — each agent asks 'what tools do I have?' and the Hub returns current tool schemas. When Hospital X upgrades from Epic 2023 to 2024, we update one MCP connector, not 12 agents. APIM sits in front of backend APIs for HIPAA audit logging, rate limiting (prevents a runaway agent from exhausting payer API quotas), and centralized OAuth. The combination is the only pattern that scales to 180 hospitals."

### Decision 5 — Why the Supervisor → Specialist hierarchy over a monolithic agent?

**Your answer:**
"A monolithic prior auth agent would have a 2,000-token system prompt trying to be an eligibility checker, a formulary expert, and a fraud detector simultaneously. The LLM's attention is diluted across all three domains, and the tools from all three domains compete in the same context window. The Supervisor pattern gives each specialist a focused 200-token system prompt for exactly one domain. Accuracy improves because the model isn't context-switching. Maintainability improves because changing fraud detection rules touches only the FraudDetector agent, not a monolithic prompt. And specialists run in parallel — the prior auth result arrives in the time of the slowest specialist, not the sum of all three."

---

## Section 3 — The Terror Questions (VitalCare Specific)

**"GPU costs tripled last month. CTO is questioning ROI. Defend your architecture's cost model."**

"We're using Azure OpenAI serverless endpoints — no GPU provisioning, no idle costs. We pay per token. Three controls are in place. First, GPT-4o mini handles all classification and routing tasks (triage, duplicate detection, format validation) — it's 17x cheaper than GPT-4o full and sufficient for structured tasks. GPT-4o is reserved for final synthesis and answer generation. Second, semantic caching via Redis — if the same prior auth question with the same policy has been answered in the last 24 hours, we return the cached response without calling the LLM. Cache hit rate is typically 25-30% for formulary questions. Third, top-K reduction in RAG retrieval — we return 3 context chunks, not 10. Fewer tokens in the prompt = lower cost per call. The ROI case: 180 hospitals × average 200 prior auths/day × 12 minutes saved per auth = 432,000 minutes/day = 7,200 clinician-hours per day freed from manual policy lookup. At a conservative $80/hour clinical staff cost, that's $576,000/day in recovered clinical time."

**"A physician claims your agent gave a wrong prior auth recommendation that delayed a patient's treatment. How do you investigate?"**

"Every prior auth decision has a CorrelationId that links the full audit trail: which documents were retrieved, which specialist agents ran, what confidence score each returned, what the Supervisor's synthesis said, and what the final output was. We retrieve the full audit log for that CorrelationId from App Insights. We check the groundedness score — if it was below 0.90, that's a process gap (the decision should have been pended, not auto-approved). We check whether the retrieval returned the correct formulary version for that insurer and date. We check the Content Safety filter log — did any output flag get suppressed? This investigation typically takes under 10 minutes because every decision is fully logged. We share the audit report with the clinical team and — depending on the root cause — either fix the retrieval index (outdated policy document), adjust the confidence threshold, or escalate the agent type for that insurer to mandatory pharmacist review."

---

## Section 4 — PHI and HIPAA Specifics You Must Know Cold

These will come up — especially with a healthcare client.

| Requirement | How VitalCare Implements It |
|------------|---------------------------|
| No PHI in logs | `FunctionInvocationFilter` masks PII — logs member ID only, never name/DOB/diagnosis |
| 7-year audit retention | Append-only audit store in Azure Cosmos DB with legal hold policy |
| Minimum necessary access | RBAC + row-level security in AI Search — prior auth agent sees only its patient's records |
| Breach notification | Azure Defender for Cloud alert → incident response runbook |
| Right to explanation | Every decision has an audit trail with source documents cited |
| Business Associate Agreement | Azure signs BAA — required before storing or processing PHI |

**The PHI in logs trap:** Candidates often say "we log all agent calls for debugging." Wrong answer for healthcare. You log: member ID, CorrelationId, agent name, tool called, confidence score, timestamp. Never: patient name, date of birth, diagnosis code, medication name. Those are PHI — they cannot appear in application logs.

---

## Quick-Reference Interview Answers

**Q: Walk me through the key architecture decisions in your assessment.**
"The platform has five load-bearing decisions. RAG over fine-tuning — clinical policies change weekly, RAG stays current; fine-tuning freezes knowledge at training time. Azure DI for structured form extraction — 95%+ accuracy on fixed-layout forms without JSL's NLP overhead. Semantic Kernel for agent orchestration — .NET-native, Azure-integrated, Managed Identity throughout. Hybrid MCP + APIM for tool integration — scales to 180 hospitals by updating one MCP connector, not 12 agents. Supervisor + specialist hierarchy — parallel execution, focused system prompts, clean failure isolation. All five decisions connect back to either clinical safety, PHI compliance, or operational scale."

**Q: What would you change in your assessment if you were rebuilding it today?**
"One thing I'd add is LangGraph for the step therapy decision subtree. The current Semantic Kernel implementation handles the main ReAct loop well, but the step therapy pathway has 7 conditional branches with physician interrupts at two points. LangGraph's built-in Checkpointer and `interrupt_before` primitive would handle that sub-workflow more cleanly than manual state management in SK. I'd keep SK as the main orchestration layer and use LangGraph only for that isolated, graph-shaped sub-problem."
