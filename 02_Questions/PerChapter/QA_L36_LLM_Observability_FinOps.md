# Q&A — L36 LLM Observability, Tracing and FinOps

*Created 2026-07-26 · FDE-Prep*

---

**Q1. Why is HTTP 200 an insufficient definition of success for an agent?**

Because an agent can be fully available, sub-second, and confidently wrong. A hallucinated answer or
a wrong tool call returns 200. Traditional APM measures whether the system responded; it cannot
measure whether the response was right.

---

**Q2. Trace, span, attribute — and what does one agent request produce?**

Trace = one end-to-end request. Span = one operation inside it, nested into a tree. Attribute =
key/value metadata on a span. One agent request produces a trace containing a span per LLM call, per
tool call, and per guardrail check — typically 5–15 spans for a multi-step loop.

---

**Q3. Why OpenTelemetry rather than a vendor SDK? What does the Collector let you change without
touching application code?**

OTel is vendor-neutral: instrument once, export anywhere. The Collector sits between your app and the
backends, so you can switch from Jaeger to Tempo to Dynatrace, or fan out to several at once, by
editing collector configuration — no redeploy of the application.

---

**Q4. Two agents, disconnected traces. What is missing?**

Context propagation. The trace context (trace ID and parent span ID) must be injected into the
message envelope by the sender and extracted by the receiver. Without it each agent starts a new
root trace and you cannot see the request end to end.

---

**Q5. LangSmith vs Langfuse — when does compliance force the choice, and why?**

When prompts contain regulated data. LangSmith is primarily SaaS, so prompts and completions leave
your boundary; Langfuse is self-hostable, so they do not. Since the sensitive content *is* the prompt
text, hosting model is a data-residency decision, not a preference.

---

**Q6. What is Arize Phoenix uniquely good at?**

The embedding layer — visualising embedding clusters, detecting drift between the indexed corpus and
live queries, and surfacing retrieval failures where a query returns nothing relevant. That makes it
the RAG-debugging tool rather than a general tracer.

---

**Q7. Four things LiteLLM gives an architect. The honest downside?**

Provider abstraction (swap Azure→Bedrock without code change); automatic fallback on 429 or outage;
model version pinning with config-based rollback; per-team virtual keys with budgets and unified cost
accounting. Downside: another network hop to run highly available, and it lags provider-specific
features. Overkill for one model on one cloud.

---

**Q8. Why is `api_version: "2024-10-01"` better than an alias?**

An alias silently moves. For regulated or clinical AI, a model change is a new deployment requiring
re-validation — you cannot have that happen without a deploy. Pinning also makes rollback a config
change rather than a code release.

---

**Q9. Semantic caching at 0.90 similarity — describe the failure using the cancellation example.**

*"Customer wants to cancel"* and *"customer does NOT want to cancel"* embed almost identically —
negation barely moves the vector. At a 0.90 threshold the second query hits the first's cached
answer and the system confidently returns the exact opposite of the truth. Start at 0.95+ and
evaluate against real query pairs.

---

**Q10. Why do output tokens deserve more attention than input tokens, and why do agent loops break
linear cost intuition?**

Output tokens typically cost 3–5× input tokens, so verbose answers are disproportionately expensive.
Agent loops resend growing context on every iteration, so total tokens grow roughly quadratically
with loop length rather than linearly — which is why capping iterations is a cost control, not just
a safety control.

---

**Q11. Showback vs chargeback.**

Showback reports what each team consumed — visibility without billing. Chargeback puts the cost on
the team's budget. Showback changes awareness; chargeback changes behaviour.

---

**Q12. Which metric would you put in front of a CFO, and why not cost per token?**

Cost per business unit — *"$0.04 per prior-auth processed versus $12 of analyst time."* Cost per
token is an implementation detail with no comparator; cost per transaction can be set against the
manual alternative, which is the only comparison that justifies the spend.

---

**Q13. Name three alerts a traditional APM would never fire that an agent platform needs.**

Groundedness score dropping week-on-week; average loop iterations rising above baseline (the agent is
thrashing); and human-escalation rate rising (model or data drift). A fourth: cache hit-rate collapse,
which predicts a cost spike before the invoice shows it.

---

**Q14. What are the three observability layers, and which one do most teams skip?**

Infrastructure (pods, CPU, restarts), AI service (latency, TTFT, tokens, cost, 429s), and quality
(groundedness, tool accuracy, task success). Most teams skip **quality** — and it is the layer where
the failures that damage the business actually appear.

---

**Q15. Ranked: the FinOps levers with the best return, and the one architectural change with the
biggest impact.**

Semantic caching first (30–60% on repetitive traffic, low effort), then model tiering, prompt
compression, iteration caps, RAG context trimming, batching, self-hosting. The biggest architectural
lever is **model tiering** — a small model classifies and answers the easy majority, escalating only
on low confidence, at roughly a twentieth of the cost per call.

---

**Q16. What is the trap in logging prompts and completions?**

It is what makes agent debugging possible and simultaneously the fastest way to move regulated data
into an observability platform that was never scoped for it. It is a data-classification decision:
full text in dev, redacted or hashed in prod, with a documented break-glass path.

---

## Scoring

| Score | Read |
|---|---|
| 14–16 | Rows 46–52 are green. |
| 10–13 | Re-read §3 (tracing platforms) and §6 (FinOps). |
| < 10 | Re-read `L36`, then `L31` §4–5 for the three-layer model it builds on. |
