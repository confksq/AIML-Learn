# High-Level Prep — Memory · Tokenization Efficiency · Scaling · Agents

**Created:** 2026-07-19 · **Altitude:** architect interview, not implementation
**Companion to:** `02_Questions/InterviewBank/`

> Purpose: the four things you can be asked at any depth without warning. Each section gives the
> 30-second answer you must be able to say cold, the decision framework behind it, and the probes
> that follow. Section 1 (Memory) also fills a genuine gap in `01_Lessons/` — it was only ever a
> subtopic inside L16.

---

# 1. Memory

**The gap-filler. Read this one properly.**

## The 30-second answer

> "Memory in an LLM system is everything the model knows at inference time that isn't in its weights.
> The context window is only the working set — the part you're paying for on every single call.
> Around it you need a memory architecture: what you keep, what you summarize, what you push to
> durable storage, and what you retrieve back. LLMs are stateless; memory is entirely the
> application's job."

## The single most important distinction

**Context window ≠ memory.**

The context window is *working memory* — rebuilt from scratch on every API call, and re-billed every
time. Memory is the *strategy* for deciding what goes into that window. Conflating the two is the
mistake that marks someone as never having run this in production.

## Four layers

| Layer | Lives | Lifetime | Azure home |
|---|---|---|---|
| **Working** — the assembled prompt | Context window | One call | — |
| **Session** — this conversation | App/cache | Minutes–hours | SK `ChatHistory`, Redis |
| **Long-term** — facts about the user/domain | Vector or document store | Indefinite | AI Search, Cosmos DB, Foundry Memory Store |
| **State/scratchpad** — agent's intermediate work | Orchestrator | One task | LangGraph state, SK context |

Interviewers probe whether you know these are **different problems**. Session memory is a
truncation problem. Long-term memory is a retrieval problem. Conflating them produces a system that
either forgets constantly or costs 10× what it should.

## Five strategies — with the trade-off that matters

| Strategy | How | Cost | Loses |
|---|---|---|---|
| **Full buffer** | Send everything | Grows quadratically | Nothing — until you hit the window and it breaks |
| **Sliding window** | Keep last N turns | Flat, predictable | Anything older, silently |
| **Summarization / compaction** | LLM-compress old turns | Extra call per compaction | Detail; compounds errors over time |
| **Vector retrieval** | Embed turns, retrieve relevant | Embedding + search | Continuity — retrieves facts, not narrative flow |
| **Hybrid** | Recent verbatim + older summarized + vector for facts | Highest complexity | Least — the production answer |

**Say this:** *"Sliding window for short transactional chats, hybrid for anything long-running.
Summarization alone is a trap — errors compound because you're summarizing summaries."*

## What to evict when the window fills

The ranking, best to worst:
1. **Oldest middle turns** — models attend weakly to the middle anyway (lost-in-the-middle)
2. **Tool call *outputs*** once acted on — usually the biggest tokens in the window
3. **Retrieved chunks from prior turns** — re-retrieve if needed
4. **Never**: the system prompt, or the most recent 2–3 turns

## Failure modes to name

| Failure | What it looks like |
|---|---|
| **Context rot** | Long conversations degrade — contradictory summaries accumulate |
| **Lost in the middle** | Facts placed mid-context get ignored; position matters as much as presence |
| **Stale memory** | Long-term store says one thing, retrieved docs say another; model picks arbitrarily |
| **Cost blowup** | Full-buffer memory — token cost grows quadratically with conversation length |
| **Memory poisoning** | Injected content persisted into long-term memory, re-served as fact later. *Security issue* |

That last one is worth raising unprompted — it connects memory to prompt injection and shows you
think about both.

## Multi-agent memory

When Agent A hands to Agent B, what transfers? Three options:
- **Full history** — expensive, and B inherits A's confusion
- **Summary only** — cheap, lossy, most common
- **Shared state object** — structured fields both read/write. Best, and what LangGraph formalizes

**Probe you'll get:** *"How do agents share memory without corrupting each other's state?"*
Answer: a typed shared state with explicit write ownership per field — not a free-for-all
conversation log.

## Your JMA hook

Your DealerSource/EnterpriseSearch RAG is effectively **stateless** — each query retrieves fresh with
no session memory. That's a legitimate design choice for search, and saying so demonstrates you know
memory is a *cost*, not a virtue. The upgrade path: session memory for multi-turn follow-ups
("what about the other dealer?") and long-term memory for user preferences.

---

## Episodic memory — the fourth type they'll ask about

*Added 2026-07-26 · FDE-Prep. The JD wording is "Memory systems — short-term, long-term vector, episodic."*

**The 20-second answer:** episodic memory is what the agent remembers about **specific past
interactions as events** — what happened, when, with whom, and how it turned out — as opposed to
facts or documents.

### The four kinds, distinguished

| Kind | Stores | Question it answers | Where it lives |
|---|---|---|---|
| **Short-term / working** | this conversation's turns | "what did we just say?" | context window |
| **Semantic** | facts, documents, policy | "what is the cancellation rule?" | vector store (RAG) |
| **Episodic** | **past episodes as events** | **"what happened last time I handled this dealer?"** | event store + vector index over summaries |
| **Procedural** | learned how-to / skills | "what's my proven sequence for this task?" | prompts, tools, `L32`-style Skills |

The split people miss: **semantic memory is knowledge, episodic memory is experience.** RAG gives
you the former. Most "agent memory" products are really doing the latter.

### What an episode record looks like

```json
{
  "episode_id": "ep-88213-2026-07-14",
  "actor":      "CancellationAgent",
  "subject":    "dealer:4471",
  "when":       "2026-07-14T09:12:00Z",
  "task":       "VSC cancellation, trade-in",
  "steps":      ["lookup_contract", "check_open_claims", "escalate_human"],
  "outcome":    "held — open claim CLM-4471",
  "summary":    "Trade-in cancellation blocked by a pending claim; adjuster resolved in 3 days.",
  "embedding":  [...]
}
```

Retrieval is **hybrid**: filter structurally (`subject = dealer:4471`, last 90 days), then rank the
survivors by embedding similarity to the current task. Pure vector search over episodes retrieves
things that merely *sound* similar; the structural filter is what makes it useful.

### Why it matters

| Without episodic | With |
|---|---|
| Agent re-derives the same conclusion every time | Recalls "this dealer's trade-ins usually have open claims — check first" |
| No learning from failure | "Last time I escalated too early; the claim auto-resolved" |
| Cannot answer "have we seen this before?" | Can |
| Every run costs full reasoning | Precedent shortens the loop |

### The three failure modes to name

1. **Unbounded growth.** Episodes accumulate forever. Summarise old ones, keep raw for N days,
   then compress. Same eviction discipline as the context window.
2. **Poisoned precedent.** One wrong episode retrieved repeatedly becomes an entrenched wrong
   habit — memory poisoning with a longer half-life, because it looks like experience.
3. **Privacy.** Episodes are, by construction, a behavioural record of real people and accounts.
   Under HIPAA/GDPR they are personal data with retention and deletion obligations, and cross-tenant
   leakage here is worse than in semantic memory because it is specific.

### The one-liner for interview

> "Short-term memory is the conversation, semantic memory is knowledge in a vector store, and
> episodic memory is *experience* — what happened, when, and how it turned out. I'd store episodes
> as structured events with an embedded summary, and retrieve them with a structural filter first
> and similarity second, so I get relevant precedent rather than things that merely read similarly.
> The two things I'd design for up front are compaction, because episodes grow without bound, and
> retention, because an episode log is a behavioural record of real customers."

---

# 2. Tokenization Efficiency

## The 30-second answer

> "Tokens are the billing and the constraint. Efficiency isn't about clever prompt-shortening — it's
> about architecture: model choice, retrieval size, and caching. Those three move the needle by
> multiples. Prompt wordsmithing moves it by percent."

## The budget formula

```
total = system + memory/history + retrieved context + user query + output
```

Output tokens typically cost **3–4× input**. Most people optimize the wrong end.

## The levers, ranked by actual impact

| Lever | Impact | Notes |
|---|---|---|
| **Model choice** | **~17×** | GPT-4o → GPT-4o mini. The single biggest lever, by far |
| **Top-K reduction** | 2–5× | Retrieving 20 chunks when 5 suffice is the most common waste |
| **Caching** (exact + semantic) | 2–10× on repeat traffic | Depends entirely on query repetition rate |
| **Prompt caching** | Up to ~90% off cached prefix | Big static system prompts amortize well |
| **Memory strategy** | 2–3× on long conversations | See §1 |
| **Output constraints** | 1.5–2× | Cap `max_tokens`; structured output beats prose |
| **Prompt wordsmithing** | ~5–10% | Real, but last |

**Say this:** *"I'd route by task complexity before I'd touch the prompt. Cheap model for
extraction and classification, expensive model only for synthesis and reasoning."*

## Tokenizer facts worth knowing

- ~4 characters ≈ 1 token in English; **code and non-English are far worse**
- Token IDs are **model-specific** — not portable across model families
- BPE (GPT) · SentencePiece (LLaMA, Gemini) · WordPiece (BERT)
- Embedding models have **their own** limits, independent of your chat model

## The trap question

*"How do you reduce cost?"* — a weak answer talks about shortening prompts. A strong answer:
**measure first**. Where are the tokens actually going? Usually retrieved context, not the prompt.
Then: right-size the model, cut top-K, cache. Instrument with App Insights before optimizing
anything.

---

# 3. Scaling AI

## The 30-second answer

> "Scaling a GenAI system means four different things and they trade against each other: throughput,
> latency, cost, and reliability. The bottleneck is almost never your compute — it's the model
> provider's quota. So you design around TPM limits, not CPU."

**That last sentence is the whole insight.** Traditional scaling instincts mislead here.

## Four dimensions

| Dimension | Constraint | Levers |
|---|---|---|
| **Throughput** | TPM/RPM quota | Multiple deployments, PTU, regional spread, queue + backpressure |
| **Latency** | Model inference time | Streaming, smaller model, reduce top-K, semantic cache, parallel retrieval |
| **Cost** | Token volume | See §2 |
| **Reliability** | Provider outages, throttling | Retry with backoff, circuit breaker, fallback model, graceful degradation |

## The quota point

- **PAYG / Standard** — shared capacity, subject to throttling under load
- **PTU (Provisioned Throughput)** — reserved capacity, predictable latency, expensive; justified only at steady high volume
- **429s are normal**, not exceptional — exponential backoff is mandatory, not optional
- Spread across **multiple deployments/regions** to multiply effective quota

## Caching — three distinct kinds

| Type | Key | Hit rate | Risk |
|---|---|---|---|
| **Exact** | Query hash | Low | None |
| **Semantic** | Embedding similarity | Much higher | **Serving a wrong-but-similar answer** — needs a tight threshold |
| **Embedding** | Text hash → vector | Very high | None. Free win, underused |

Embedding cache is the easy one people forget: the same document re-embedded repeatedly is pure waste.

## Streaming

Doesn't reduce *actual* latency — it reduces **perceived** latency, and that's usually what matters.
Time-to-first-token becomes the metric, not total time. Worth saying explicitly; it shows you
distinguish user experience from system performance.

## Degradation ladder

Under load, degrade in this order rather than failing: full model → smaller model → cached answer →
retrieval-only, no generation → queue with honest wait message.

---

# 4. AI Agents

## The 30-second answer

> "An agent is an LLM in a loop with tools and a termination condition. RAG retrieves once and
> generates. An agent decides *whether* to retrieve, *which* tool to call, and *when* it's done.
> That autonomy is the value and the entire risk surface."

## The distinction they're testing

| | Decides what to do | Loops |
|---|---|---|
| **RAG** | No — fixed pipeline | No |
| **Function calling** | Model picks the tool, your code executes | Usually single-shot |
| **Agent** | Yes | Yes — until termination |

**Critical nuance:** in function calling the model decides *what*, your code decides *whether* to
execute. That control point is where you put authorization and guardrails.

## ReAct

Reason → Act → Observe → repeat. Every agent framework is a variation on this.

## When NOT to use an agent

Strong architects volunteer this. Agents add latency, cost, and non-determinism. Don't use one when:
- The workflow is **known and fixed** — use a pipeline
- The task is **single-step retrieval** — use RAG
- You need **deterministic, auditable** behavior — regulated decisions
- **Latency budget is tight** — agents multiply round-trips

*"Would you use an agent here?"* is often a trap. Sometimes the answer is no.

## Framework landscape

| Framework | Model | Language |
|---|---|---|
| **Semantic Kernel** | Plugins + planner, implicit loop | C#, Python |
| **LangGraph** | Explicit state graph, cycles as first-class | Python, JS |
| **AutoGen** | Conversational multi-agent | Python |
| **crewAI** | Role-playing crew | Python |
| **Foundry Agent Service** | Managed, portal-configured | Azure-native |

⚠️ **Your gap:** LangGraph and AutoGen aren't in `01_Lessons/` at all. The one-line version —
*"SK's loop is implicit and you hope it terminates; LangGraph makes the loop an explicit state
machine, so retries and cycles like Corrective RAG become first-class and bounded."* That single
contrast covers most of what a high-level question needs.

## Multi-agent patterns

- **Supervisor / orchestrator** — one routes to specialists. Most common, most defensible
- **Sequential pipeline** — fixed handoff chain
- **Peer / A2A** — agents negotiate. Powerful, hard to debug, rarely justified

**Say this:** *"I default to supervisor. Peer-to-peer is usually complexity without payoff unless
agents genuinely need to negotiate."*

## Guardrails — name these

Iteration caps · tool allow-lists · human-in-the-loop for high-stakes actions · output validation ·
audit logging of every tool call · timeout + circuit breaker per tool · cost ceiling per task

Your `01_Lessons/Part6_AppliedProjects/02-DealerIntelligence-Platform/07-FaultTolerance/` implements several of these —
worth referencing as something you've actually built.

## Production concerns

Non-determinism makes testing hard — golden datasets + LLM-as-judge, not assertions. Debugging needs
full trace logging of every reason/act/observe step. Cost is unbounded by default: cap iterations.

---

# Rapid self-check

Answer aloud in under a minute each. If you stall, reread that section.

1. Why isn't the context window the same as memory?
2. When the window fills, what do you evict first — and what do you never evict?
3. What's memory poisoning and why is it a security problem?
4. Rank the cost levers. Where does prompt shortening land?
5. Why is TPM quota the scaling bottleneck rather than compute?
6. Semantic cache vs exact cache — what's the risk of the former?
7. Does streaming reduce latency?
8. Give a case where you'd refuse to use an agent.
9. SK vs LangGraph in one sentence.
10. Function calling: who decides what runs, and who decides whether it runs?

---

## Cross-references

- Tokenization depth → `01_Lessons/Part3_GenAI_LLMs/L11_2_LLMs_Tokenization_Embeddings.md`
- Scaling depth → `01_Lessons/Part4_Architecture/L18_AISolutionArchitecture.md`
- Agents depth → `01_Lessons/Part3_GenAI_LLMs/L16_AIOrchestration_SK_Agents.md`
- MCP / A2A / CAG → `01_Lessons/Part5_AgenticProtocols/` (L23, L26, L28, L29)
- Framework comparison → `01_Lessons/Part5_AgenticProtocols/L25_AgentFramework_Comparison.md`
