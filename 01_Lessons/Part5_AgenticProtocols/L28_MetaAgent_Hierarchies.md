# Module 07 — Meta-Agent Hierarchies: Agents of Agents

> **⚙️ Config or Code? — This Module**
> - **Portal Config only:** Azure Service Bus queue + dead-letter settings (portal), Container Apps deployment of each specialist agent (portal/Bicep), RBAC roles between agents (portal)
> - **Custom Code:** Supervisor reasoning logic (SK Planner or custom orchestration), `Task.WhenAll()` for parallel specialist calls (C#), LangGraph fan-out/fan-in graph (Python), typed message contracts between Supervisor and Specialists, failure handling per specialist (fail-fast vs partial vs fallback logic), dead-letter message handler
> - **Both:** Service Bus (create queue + dead-letter config = portal; publish/subscribe code = SDK)

---

## Why This Module Matters

The screener confirmed agent workflow as a core focus. Meta-agent hierarchies are the architecture pattern that separates "I built a chatbot" from "I designed an autonomous AI platform." You will be asked:
- "How would you break a complex clinical decision into multiple agents?"
- "How does a supervisor agent know which specialist to call?"
- "What happens when a sub-agent fails in a hierarchy?"

Your anchor: The VitalCare platform uses a Supervisor agent that delegates to three specialists — ClaimValidator, PolicyChecker, and FraudDetector — running in parallel where possible.

---

## Section 1 — What a Meta-Agent Hierarchy IS

A **Meta-Agent (Agent of Agents)** is a pattern where one agent — the Supervisor — does not do the work itself. Its only job is to **decompose a complex task, assign sub-tasks to specialist agents, collect results, and synthesize a final decision**.

**The mental model:** Think of it like a **hospital department chief**. When a complex case comes in, the Chief doesn't personally run labs, read the ECG, and check drug interactions. The Chief reads the chart, says "Cardiology — look at this ECG," "Pharmacy — check the drug interactions," "Lab — run a CBC." Each specialist reports back. The Chief synthesizes all inputs into the treatment plan.

The Chief is the Meta-Agent. The specialists are the sub-agents. The treatment plan is the final output.

---

## Section 2 — The Three-Layer Architecture

```
TASK comes in
      ↓
[SUPERVISOR AGENT]   ← Layer 1: decompose, delegate, synthesize
  /       |       \
[Validator] [PolicyChecker] [FraudDetector]   ← Layer 2: specialists
      ↓         ↓              ↓
[Tools/RAG] [Tools/RAG]   [Tools/RAG]   ← Layer 3: capabilities
```

**Layer 1 — Supervisor:**
- Receives the top-level task
- Decides which specialists are needed
- Calls them (sequentially or in parallel depending on dependencies)
- Synthesizes all results into a final decision
- The Supervisor NEVER calls external APIs directly — only other agents

**Layer 2 — Specialists:**
- Each specialist owns one domain: validation, policy, fraud, clinical, etc.
- Has its own system prompt, its own tools, its own RAG index
- Returns a structured result (not free text) — typed response the Supervisor can reason over

**Layer 3 — Capabilities:**
- The actual tools: FHIR queries, payer API calls, Azure AI Search lookups
- Specialists call these, never the Supervisor

---

## Section 3 — Healthcare Example: Prior Auth Meta-Agent

**The task:** A physician submits a prior authorization for semaglutide for a patient with Type 2 diabetes.

**Supervisor receives:**
```json
{
  "patient_id": "P-10234",
  "requested_medication": "semaglutide 1mg",
  "diagnosis_code": "E11.9",
  "insurer": "BlueCross-MA-Plan-7"
}
```

**Supervisor's reasoning:**
> "This requires three independent checks. ClaimValidator confirms the diagnosis supports the medication. PolicyChecker retrieves this insurer's formulary rules. FraudDetector checks if this physician has anomalous prescription patterns. All three can run in parallel — no dependencies between them."

**Parallel execution:**
```
ClaimValidator   → FHIR read → diagnosis E11.9 confirms semaglutide eligibility → APPROVE
PolicyChecker    → RAG index lookup → BlueCross-MA-Plan-7 requires step therapy → FLAG
FraudDetector    → anomaly model → physician's pattern is within normal range → CLEAR
```

**Supervisor synthesizes:**
> "Two of three approve. One flags step therapy requirement. Result: PENDED — route to pharmacist for step therapy documentation."

**The outcome is not any one specialist's answer — it's the Supervisor's synthesis of all three.**

---

## Section 4 — Parallel vs Sequential Execution

The Supervisor must decide: run specialists in parallel or in sequence?

| Pattern | When to use | Example |
|---------|------------|---------|
| **Parallel** | Specialists are independent — no output feeds into the next | ClaimValidator, PolicyChecker, FraudDetector all need the same patient data |
| **Sequential** | Output of one specialist feeds the next | First validate the patient exists → then check eligibility → then check fraud |
| **Conditional** | Route to different specialists based on earlier result | If diagnosis is oncology → route to OncologySpecialist; if cardiology → CardioSpecialist |

**In Semantic Kernel:**
```csharp
// Parallel execution
var tasks = new[]
{
    _claimValidator.ValidateAsync(request),
    _policyChecker.CheckPolicyAsync(request),
    _fraudDetector.CheckPatternAsync(request)
};
var results = await Task.WhenAll(tasks);
```

**In LangGraph (Python):**
```python
# Parallel fan-out from supervisor node
graph.add_edge("supervisor", "claim_validator")
graph.add_edge("supervisor", "policy_checker")
graph.add_edge("supervisor", "fraud_detector")
# Fan-in: all three must complete before synthesize node
graph.add_edge(["claim_validator", "policy_checker", "fraud_detector"], "synthesize")
```

---

## Section 5 — Failure Propagation in Hierarchies

**The trap question:** "What happens when one sub-agent fails?"

This is where architects are separated from developers. Three strategies:

| Strategy | Behavior | Use when |
|----------|----------|----------|
| **Fail-fast** | Supervisor immediately fails the whole task | All specialists are required — no partial result is usable |
| **Partial result** | Supervisor synthesizes what it has, flags the gap | Some specialists are informational — decision can proceed with caveats |
| **Retry + fallback** | Supervisor retries the failed specialist; if still failing, uses a fallback | Specialist is critical but a fallback (simpler rule) exists |

**Healthcare example — Failure in the PolicyChecker:**
- PolicyChecker times out (payer API is down)
- Supervisor cannot determine step therapy requirement
- **Fail-fast would be wrong** — the agent would silently fail and the prior auth would disappear
- **Correct behavior:** Supervisor returns `PENDED — PolicyChecker unavailable; route to manual review`
- The physician knows a human needs to check the policy — no PHI is lost, no decision is silently dropped

**The rule:** In healthcare, silent failure is never acceptable. Every partial or failed result must produce an actionable outcome — approve, deny, pend, or escalate. Never disappear.

---

## Section 6 — JM Family Anchor

"At JM Family I have the same pattern in our incentive claim processing. The Supervisor receives a dealer incentive claim and delegates to three validators — an EligibilityValidator (is this dealer enrolled in this program?), a DataQualityValidator (does the submission have all required fields?), and a DuplicateDetector (has this exact claim been submitted before?). The validators run in parallel. The Supervisor synthesizes: if all three pass → auto-approve; if EligibilityValidator fails → deny; if DataQuality fails → return to dealer for correction; if Duplicate detected → flag for manual review. The key is that the Supervisor never calls the database or the SharePoint index directly — it only speaks to its specialists."

---

## Quick-Reference Interview Answers

**Q: What is a Meta-Agent and when would you use one?**
"A Meta-Agent is a Supervisor agent whose job is decomposition and synthesis — not direct execution. You use the pattern when a task requires multiple specialist domains that can't all live in one agent's context. In healthcare: a prior auth decision requires eligibility checking, policy rule lookup, and fraud detection simultaneously. One monolithic agent would have a confused system prompt and competing tools. Three specialists, each expert in one domain, supervised by a coordinator — that's architecturally clean and clinically defensible."

**Q: How do you handle failure in a multi-agent hierarchy?**
"The Supervisor owns the failure contract. Before I build any hierarchy, I define what a partial result means for each specialist: is this specialist's output required, informational, or fallback-eligible? For required specialists, the Supervisor pends the task and routes to human review on failure — never silently drops. For informational specialists, the Supervisor flags the gap in the response and proceeds. In healthcare, every outcome must be actionable — approve, deny, pend, or escalate."

**Q: How do agents communicate in your hierarchy?**
"Through typed message contracts — not free text. The Supervisor sends a typed request object to each specialist, and each specialist returns a typed response with a status, a reason, and a confidence score. That structure is what lets the Supervisor synthesize cleanly — it's not parsing natural language, it's reading structured fields. I'll walk through the A2A protocol in detail next — that's the standard that governs this contract."
