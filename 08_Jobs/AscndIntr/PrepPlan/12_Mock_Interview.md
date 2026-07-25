# Module 12 — Full Mock Interview + Terror Questions
**Interview:** Monday 2026-06-22 3:00 PM EST

---

## How to Use This Module

Read each question. Close the file. Answer out loud — full sentences, 60–90 seconds for technical answers, 3–4 minutes for the centerpiece question. Then come back and check your answer against the reference. Anything you stumble on = go back to that module.

**The interview format (from screener):**
- End-to-end agent workflow walkthrough (Module 06 centerpiece — 4-5 minutes)
- Framework choice justification (Module 04)
- MCP + APIM architecture (Module 05)
- VitalCare assessment defense (Module 11)
- Terror questions at the end

---

## ROUND 1 — Warmup (90 seconds each)

**Q1: Tell me about yourself in the context of AI architecture.**

Reference answer structure:
1. Current role + what you've actually built (JM Family — Document Intelligence pipeline, Azure AI Search RAG, Semantic Kernel agent)
2. Scale of the work (production, enterprise, Azure-native)
3. Why healthcare is a natural fit (document-heavy, compliance-driven, same patterns at different stakes)
4. What you're here for (architect-level platform design at 180-hospital scale)

---

**Q2: What is an AI agent and how is it different from a regular API call?**

Reference: An agent has a ReAct loop — it Reasons about what to do, Acts by calling a tool, Observes the result, and loops until it reaches an answer. A regular API call is deterministic: input in, output out, done. An agent is non-deterministic: it decides at runtime which tools to call, in what order, how many times — based on what it observes. The consequence is that agents are harder to test (you can't enumerate all execution paths) and harder to monitor (you need quality metrics, not just uptime).

---

**Q3: What is Semantic Kernel and why do you use it?**

Reference: Microsoft's production AI SDK. Plugin-based: you write C# methods decorated with `[KernelFunction]`, the LLM reads the function name and description and decides when to call it. Four core pieces: Kernel (container), KernelFunction (tool), ChatHistory (state), Planner (ReAct loop). You use it because: .NET-native (your team's language), Azure Managed Identity out of the box (no API keys), first-class connectors for Azure AI Search / Azure OpenAI / Content Safety, and `FunctionInvocationFilter` for interception at every tool call (your audit and guardrail hook).

---

## ROUND 2 — Architecture (2–3 minutes each)

**Q4: Walk me through an end-to-end prior authorization workflow — from how the request arrives to how the decision reaches the physician.**

This is Module 06 centerpiece. Hit all 11 steps: Receive → Reason → Plan → Retrieve → Tool Call → Observe → Loop → Generate → Validate → Respond → Monitor. Anchor each step to either JM Family or VitalCare. Time yourself — this answer should be 4 minutes.

---

**Q5: You have a Supervisor agent and three specialist agents. The PolicyChecker is timing out. What happens?**

Reference: Polly RetryPolicy catches the first transient timeout — retry 3x with exponential backoff. If PolicyChecker is consistently unavailable, CircuitBreaker opens after 5 failures and stops sending calls. Supervisor receives `CircuitOpenException`. Supervisor cannot synthesize a complete decision. Result: prior auth is PENDED (not failed, not dropped), routed to pharmacist review queue with the reason "PolicyChecker unavailable — manual policy review required." CorrelationId links the pended record to the full audit trail. When PolicyChecker recovers, the circuit half-opens, probe succeeds, circuit closes, replay worker re-processes the pended queue.

---

**Q6: How do you choose between MCP Hub and Azure APIM?**

Reference: They're not alternatives — you use both in a hybrid pattern. MCP Hub is AI-native: agents ask "what tools exist?", get back capability schemas the LLM understands, call tools via a standardized protocol. APIM is enterprise-native: throttling, OAuth, versioning, compliance logging for any HTTP client. MCP handles the agent side. APIM handles the backend API governance side. In a healthcare context: MCP Hub sits between the agent and all tools; APIM sits between the MCP Hub and each backend API (EHR, payer, lab). Every tool call that touches PHI goes through APIM so you have a single HIPAA audit gateway.

---

**Q7: A new developer on the team wants to use AutoGen for the prior auth workflow. How do you respond?**

Reference: Three reasons AutoGen is wrong for production healthcare. One — state is implicit: it's the message thread, no typed fields, so you can't audit what the agent knew at a specific step (HIPAA problem). Two — non-deterministic routing: the GroupChatManager uses an LLM to decide which agent speaks next; same input, different paths, different outcomes. A prior auth system where the fraud check sometimes runs before the policy check and sometimes after is not auditable. Three — no crash recovery: if the workflow crashes mid-conversation, you restart from scratch. AutoGen is excellent for research and exploration. For production PHI workflows, LangGraph (typed state, crash recovery) or Semantic Kernel (explicit ChatHistory, FunctionInvocationFilter) is the right answer.

---

## ROUND 3 — Deep Technical (2 minutes each)

**Q8: What is RAG and how does it differ from fine-tuning?**

Reference: RAG retrieves relevant documents at query time and injects them into the prompt as context — the LLM generates an answer grounded in that retrieved content. Fine-tuning bakes knowledge into model weights during training. Key differences: RAG knowledge stays current (you update the index, not the model); fine-tuning freezes at training time. RAG answers are auditable (you can cite which document); fine-tuning answers cannot be sourced. RAG is cheaper and faster to deploy (no training run). Use RAG when knowledge changes frequently or auditability is required. Use fine-tuning when you need consistent output format, domain-specific tone, or latency reduction at scale.

---

**Q9: What is CAG and when would you use it instead of RAG?**

Reference: CAG (Cache-Augmented Generation) preloads a document set into the model's context window at session start — rather than retrieving at query time. The entire knowledge base is in context for every question. Use CAG when: the knowledge set is small (fits in the context window), the content is static (doesn't change between sessions), and latency matters (no retrieval step = faster response). Use RAG when: knowledge set is large (millions of documents), content updates frequently, or you need citation-level traceability. In healthcare: clinical policy FAQs → CAG (100-page document, updated monthly, always the same per insurer). Full formulary + EHR records → RAG (millions of patient records, changes in real time).

---

**Q10: How do you detect and prevent hallucination in a clinical agent?**

Reference: Three layers. Prevention: RAG grounding — answer is generated from retrieved context, not from model memory. The system prompt instructs: "Answer only from the provided context. If the context does not contain the answer, say so." Detection: groundedness evaluation — Azure AI Foundry evaluator compares the answer to the retrieved context and scores factual grounding (0 to 1). Responses below 0.90 in healthcare are flagged for human review. Blocking: Azure Content Safety groundedness detection runs in real time — if the output contains claims not supported by the retrieved context, the response is blocked before it reaches the physician. The key distinction: factual hallucination (claims a drug exists that doesn't) vs agentic hallucination (claims it performed a tool call that never happened). Both require separate defenses.

---

**Q11: What is A2A Protocol?**

Reference: Open standard (2025, Google-initiated, now multi-vendor) for agent-to-agent communication. Defines: how agents discover each other's capabilities, how they send tasks to each other, how auth works between agents, how failures propagate. Key implementation: typed message envelope (MessageId, CorrelationId, SchemaVersion, HmacSignature) + AgentBus that validates schema, verifies HMAC, logs audit, routes to specialist, dead-letters on failure. Why it matters: without it, every agent-to-agent call is a custom HTTP call with custom auth — works at two agents, breaks at twenty. A2A makes any compliant agent interoperable with any other, regardless of the underlying framework.

---

## ROUND 4 — Terror Questions

**"Your groundedness score has been 0.91 for three months. Yesterday it dropped to 0.71. What do you do?"**

Step 1: Check what changed yesterday — git log on prompts, index updates, model version changes, new document ingested. Step 2: Identify the correlation — did a new policy document get added? Did a prompt version deploy? Step 3: Pull 20 sample low-groundedness responses — look at what was retrieved vs what was answered. Are the retrieved chunks correct but the answer contradicts them? (prompt drift) Or are the retrieved chunks wrong? (index issue) Step 4: If prompt drift — roll back to previous prompt version, redeploy. If index issue — identify the bad document, remove it, re-evaluate. Step 5: Alert clinical team that prior auth decisions from the last 24 hours should be reviewed — don't assume only future decisions are affected.

---

**"Healthcare AI regulation is tightening. How does your VitalCare architecture adapt?"**

The architecture is built for adaptation, not point-in-time compliance. Prompt versioning in Git means regulation-triggered prompt changes are traceable. The audit log with 7-year retention already exceeds current HIPAA requirements — future mandates for longer retention require only a policy change, not an architecture change. The Supervisor + specialist pattern means a new regulatory check (e.g., a new CMS rule requiring an additional validation) = add one new specialist agent, not rebuild the monolith. Content Safety filters are configurable — new prohibited content categories are a configuration update. The most fragile point is the confidence thresholds — if regulators mandate specific accuracy SLAs, we'd need a formal evaluation protocol to certify the thresholds, not just set them empirically.

---

**"I don't see how your system handles 180 hospitals simultaneously. Walk me through scaling."**

Compute: Azure Functions with consumption plan for agent orchestration — scales to thousands of concurrent instances automatically, no pre-provisioning. Queue: Azure Service Bus Premium — partitioned queues, each hospital gets its own partition, no noisy neighbor. LLM: Azure OpenAI with PTU (provisioned throughput) for predictable latency at scale — not shared TPM limits that degrade under load. AI Search: Standard tier with 12 replicas for read scale. The coordination layer (MCP Hub) is stateless — it routes but holds no state, so horizontal scaling is trivial. The single architecture concern at 180-hospital scale is the APIM rate limits per payer API — we'd negotiate enterprise SLAs with each payer's IT team and implement per-hospital quota management in APIM so one hospital's burst doesn't consume another hospital's quota.

---

## Pre-Interview Checklist

- [ ] Can you deliver Module 06 (centerpiece) in 4 minutes from memory?
- [ ] Do you know your 5 VitalCare architecture decisions cold (Module 11 Section 2)?
- [ ] Can you explain MCP vs APIM in 60 seconds?
- [ ] Do you know the Polly circuit breaker states (closed → open → half-open)?
- [ ] Can you say "groundedness threshold 0.90 in healthcare" without hesitation?
- [ ] Do you have your JM Family `cog-jma-dev-frm-recognizer` and `oai-jma-dev-shared-mcp` names ready as anchors?
- [ ] Can you name all three reasons AutoGen is wrong for production PHI workflows?
- [ ] GitHub portfolio ready: `github.com/confksq/AI-RandD` with `05-VitalCare-AI-Platform/`
