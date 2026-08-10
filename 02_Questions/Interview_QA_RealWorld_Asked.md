# Interview Q&A — Actually Asked

**Built:** 2026-08-08 · **Phase 2** of `00_PLAN_InterviewQA_2026-08-08.md`
**Source:** the questions put to Bala across his last five interviews.

> **Why this file is the highest-value one in the repo.** Everything else is a prediction
> about what might be asked. These fourteen were asked. Several appeared more than once,
> which means they are not idiosyncratic to one interviewer — they are what the market is
> currently testing for in a Lead AI Engineer / FDE.
>
> Four of them — **1M-document search design, context compression, KEDA for AI, and PII
> management** — had **no adequate source anywhere in this repo** when it was swept
> (1,335 questions across 131 files). Those four carry a companion deep-dive question each.

---

## Ownership — read this before you drill

This file and `Interview_QA_Resume_Based.md` deliberately touch some of the same topics.
They are **not** duplicates and you need both, because interviewers ask both forms:

| | This file owns | The resume file owns |
|---|---|---|
| Framing | *"How would you…"* — the general framework | *"How did you…"* — your project, with numbers |
| Example | Q13 owns the **generic agent lifecycle** | `Resume Q15` owns **your JM Family agent topology** |
| Example | Q11 owns the **token-saving lever list** | `Resume Q29` owns **the $150K story** |
| Example | Q10 owns the **PII decision framework** | `Resume Q37` owns **where redaction sat in your pipeline** |

If an interviewer asks the general form, answer from here and *then* offer the project as
evidence. If they ask about your experience, answer from the resume file and offer the
framework as depth. Answering the wrong form is the most common way to sound rehearsed.

---

## ⚠️ Overlap with `Interview_Bible_77Q_FDE_AI_Lead.md` — declared split

**Added 2026-08-10.** The Bible's **Section B — "The Real-Time Inquisition"** (Q48–Q60)
covers the *same fourteen questions* as this file. Four are word-identical. This is not
accidental duplication — both derive from the same source, your last five interviews.

**The declared split, so you always know which to open:**

| | `Interview_Bible` Section B | **This file** |
|---|---|---|
| Layer | **Spoken answer** — the words, in your voice, dense with specifics | **Drill layer** — what they're testing, whiteboard, follow-up probes, red flags |
| Use it for | Rehearsing the delivery | Surviving the *second* and *third* question |
| Unique to it | Deep-dive extensions Q61–Q77 (DocIntel labelling, HNSW tuning, CDC, Neo4j schema) | Companions Q15–Q18 · the trade-off in every answer · what a weak answer sounds like |

**Rule: rehearse from the Bible, prepare from here.** The Bible gives you the two minutes.
This file gives you the ten minutes after, which is where interviews are actually decided.

### Question mapping

| This file | Bible Section B | Bible deep-dive extensions |
|---|---|---|
| Q1 memory | Q48 `:1060` | — |
| Q2 DocIntel training | Q49 `:1069` | **Q61–Q65** `:1193–1242` — model types, labelling, layout variation, extraction failure, continuous improvement |
| Q3 1M-doc search ⚠ | Q50 `:1078` | **Q66** `:1257` tier choice · **Q68** `:1281` partitioning · **Q67** `:1267` HNSW · **Q69** `:1296` CDC |
| Q4 model choice | Q51 `:1087` | **Q71** `:1327` |
| Q5 context window | Q52 `:1096` | **Q72** `:1344` sliding window vs map-reduce |
| Q6 compression ⚠ | Q52 `:1096` | **Q73** `:1356` LLMLingua |
| Q7 RAG lifecycle | Q53 `:1104` | — |
| Q8 KEDA ⚠ | Q54 `:1116` | — |
| Q9 chunking | Q55 `:1124` | **Q75** `:1396` 200-page PDF |
| Q10 PII ⚠ | Q56 `:1133` | — |
| Q11 token saving | Q57 `:1142` | — |
| Q12 A2A | Q58 `:1152` ⚠ *see note* | — |
| Q13 agent process | Q59 `:1163` | — |
| Q14 Foundry components | Q60 `:1176` | — |
| Q15 index topology | — | **Q68** `:1281` · **Q70** `:1312` Cosmos vs AI Search |
| Q16 when compression hurts | — | **Q73** `:1356` |
| Q17 quotas / PTU / backpressure | — | *nothing — this file is the only source* |
| Q18 PII side channels | — | *nothing — this file is the only source* |

> ⚠️ **Bible Q58 is framed wrongly.** It asks *"Why A2A instead of a single monolith
> agent?"* — which conflates A2A with multi-agent decomposition. Those are different
> things: decomposing one system into several agents is **orchestration**; A2A is for agents
> across **organisational boundaries** that neither party controls. Answering the Bible's
> framing will walk you straight into the trap **Q12 below** exists to prevent. Use Q12.

> **Before rehearsing from the Bible**, read the audit findings in
> `Interview_Bible_77Q_AUDIT_2026-08-10.md`. Several of its answers carry confident
> technical claims and specific numbers that do not survive scrutiny.

---

## Answer format

Same seven-part structure as the resume file, with one addition: a **`⏱ ASKED`** banner
carrying the question as it was actually put to you. The phrasing matters — real questions
are shorter and vaguer than textbook ones, and part of the skill is deciding what they
meant before you answer.

---

# Q1. How is memory managed?

> **⏱ ASKED:** *"How is memory managed?"*
>
> Note how open this is. It could mean the context window, agent memory, or conversation
> state. **Scope it out loud before answering** — "in an agent system, or do you mean the
> context window specifically?" That single clarifying sentence reads as senior, and it
> stops you answering the wrong question for ninety seconds.

**What they're testing:** Whether you know that "memory" is four different things with
four different lifetimes. Almost every candidate answers "conversation history" and stops,
which is one quarter of the answer.

**60-second spoken answer:**

> "The first thing I'd separate is memory from the context window, because they get
> conflated. The context window is working space — it's what the model can see on this one
> call, and it's paid for on every call. Memory is what you retrieve *into* that space. A
> model has no memory between calls; everything that looks like memory is something your
> system stored and re-supplied.
>
> Then there are four kinds, with different lifetimes.
>
> **Working memory** — the current task state. What step we're on, what's been done. Lives
> for one run.
>
> **Conversation memory** — the dialogue history. Grows with the session, so it's managed:
> recent turns kept verbatim, older ones summarised.
>
> **Semantic memory** — durable facts. The user's role, their entitlements, their
> preferences. Stored externally, retrieved when relevant rather than carried.
>
> **Episodic memory** — what happened in past interactions. 'Last week this user asked
> about X and the answer was wrong.' Distinct from semantic because it's about events, not
> facts.
>
> The engineering discipline is that memory is a retrieval problem, not a storage problem.
> The hard part isn't keeping things — it's deciding what to bring back."

**Depth — the four-point rule:**

1. **What it IS** — four memory types (working, conversation, semantic, episodic), each with
   a distinct lifetime and store, all external to the model and retrieved into the context
   window on demand.
2. **Why it works that way** — the model is stateless between calls. Every apparent memory
   is a system responsibility. Once you accept that, memory design becomes a retrieval and
   eviction problem, which is a tractable engineering problem with known trade-offs.
3. **Your example** — JM Family: LangGraph checkpointed working state, windowed and
   summarised conversation history, Cosmos DB for durable facts. Full detail in
   `Resume Q18`.
4. **The trade-off** — every strategy loses something. Summarisation loses detail
   unpredictably. Retrieval-based memory can miss relevant facts because relevance was
   judged by similarity. Unbounded history is the only lossless option and it is
   unaffordable. There is no free choice, and saying so is the senior position.

**Whiteboard:**

```
   ┌──────────────────── CONTEXT WINDOW ────────────────────┐
   │  working space — paid for on EVERY call                 │
   │  system prompt · retrieved memory · current turn        │
   └────────────────────────▲────────────────────────────────┘
                            │ retrieved into
   ┌────────────────────────┴────────────────────────────────┐
   │  WORKING      current task state       one run          │
   │  CONVERSATION dialogue history         session, windowed│
   │  SEMANTIC     durable facts            permanent, ext.  │
   │  EPISODIC     what happened before     permanent, ext.  │
   └─────────────────────────────────────────────────────────┘
        ⚠ the model itself holds NOTHING between calls
```

**The eviction ladder — what goes first when the window fills:**

```
   NEVER evict     system prompt · current user turn · safety instructions
        │
        ▼          oldest conversation turns  → summarise, don't delete
        │
        ▼          lowest-ranked retrieved chunks → drop
        │
        ▼          older tool results → replace with a reference
        │
        ▼          mid-conversation detail → compress (see Q6)
```

**Follow-up probes:**
- *"What's the difference between semantic and episodic memory?"* → Semantic is a fact
  ("this user works in claims"). Episodic is an event ("on 3rd June this user asked about
  policy renewals and rejected the answer"). Episodic is what lets a system improve per
  user; most systems have none.
- *"How do you decide what's worth remembering?"* → Explicit rules beat model judgment here.
  Store what has a defined use — entitlements, stated preferences, corrections. Letting a
  model decide what to remember produces a store full of trivia that costs money to search.
- *"What happens when memory contradicts retrieved context?"* → Retrieved source documents
  win for facts about the world; memory wins for facts about the user. Having a precedence
  rule at all is the answer — most systems have an accidental one.
- *"Multi-agent — does each agent have its own memory?"* → Shared working state so agents do
  not re-retrieve (a real cost saving), private scratchpads for reasoning. The distinction
  matters: shared state is a contract between agents and needs a schema.

**Red flag:** answering "we keep the conversation history and truncate when it gets long."
It is one memory type and the crudest management strategy, and it tells the interviewer
you have built a chatbot rather than a system.

---

# Q2. How do you train Document Intelligence on documents?

> **⏱ ASKED:** *"How you train document intelligence documents?"*
>
> They are asking about **custom model training** in Azure AI Document Intelligence — the
> labelling, training, and evaluation loop.

**What they're testing:** Whether you have run the training loop or only consumed prebuilt
models. The details that prove it: how many documents, how labelling works, and what you
do when accuracy is poor.

**60-second spoken answer:**

> "The loop is: decide you need custom at all, label, train, evaluate, then own the
> retraining lifecycle.
>
> **First, don't.** If a prebuilt model covers the document type — invoices, receipts, IDs,
> or general layout — use it. Custom training is only for genuinely proprietary document
> structures where the fields you need aren't in any prebuilt schema.
>
> **Labelling.** You use Document Intelligence Studio to label fields on sample documents —
> you're drawing boxes and naming fields, and the tool produces the label files alongside
> the documents in blob storage. The critical thing is *layout diversity*, not volume. Five
> documents of the same layout teach it one layout. You want examples covering every layout
> variant you'll see in production.
>
> **Model type.** Template models are fast to train and rigid — they expect fields in
> consistent positions. Neural models generalise across layout variation, cost more to
> train, and are what you want when documents vary.
>
> **Evaluate on held-out documents**, not on the training set, looking at per-field
> accuracy. Per-field, because an overall number hides that one important field is failing.
>
> **Then the part people skip** — production monitoring. Watch the confidence-score
> distribution. Drifting downward means new layouts are arriving and it's time to label
> more and retrain."

**Depth — the four-point rule:**

1. **What it IS** — a supervised extraction model trained on labelled samples: label in
   Studio, train (template or neural), evaluate per-field on held-out data, monitor
   confidence in production, retrain on drift.
2. **Why it works that way** — the model learns the association between field semantics and
   position/context within a layout. That is why layout diversity dominates raw count — you
   are teaching it the space of layouts, not the space of values.
3. **Your example** — KPMG contract extraction at 500K/year. See `Resume Q50–Q52` for the
   pipeline architecture and the prebuilt-vs-custom decision.
4. **The trade-off** — custom models are accurate on what they have seen and degrade
   silently on what they have not. There is no error; there is a confidence score you have
   to be watching. The alternative — layout analysis plus an LLM for extraction — is more
   expensive per document and far more tolerant of unseen layouts. High volume with stable
   layouts favours custom; a long tail of varied documents favours the LLM path.

**Whiteboard:**

```
   0. PREBUILT FIRST — invoice · receipt · ID · layout
        └── covers it at acceptable accuracy? → STOP. use it.

   1. LABEL          Document Intelligence Studio
                     ⚠ LAYOUT DIVERSITY > document count
                     docs + label files → blob storage

   2. CHOOSE TYPE    template  → fast, rigid, consistent positions
                     neural    → generalises across layouts, costlier

   3. TRAIN

   4. EVALUATE       per-FIELD accuracy on HELD-OUT docs
                     ⚠ overall accuracy hides one failing critical field

   5. PRODUCTION     monitor confidence-score DISTRIBUTION
                     drifting down = new layouts arriving = relabel + retrain
```

**Follow-up probes:**
- *"How many documents do you need?"* → A small number gets a model training — on the order
  of five per layout — but a robust production model needs enough to cover layout variety,
  which is usually a much larger and more diverse set. Answer with the principle: it is
  driven by how many distinct layouts exist, not by a fixed number.
- *"What if accuracy is poor on one field?"* → Diagnose before adding data. Is the field
  genuinely ambiguous in the source? Is it inconsistently labelled? Label noise on one field
  is a more common cause than insufficient data, and adding more badly-labelled examples
  makes it worse.
- *"Composed models?"* → Where you have several custom models for different document types
  and need automatic routing between them. Worth naming — it is the answer to "we have
  fifteen contract formats."
- *"How does this interact with the LLM?"* → Extraction gives values with locations and
  confidence; the LLM interprets. `Resume Q52` covers the boundary.
- *"What about handwriting?"* → Supported at lower confidence. Route low-confidence
  handwriting to human review, never to an LLM, which will fill the gap confidently.

**Red flag:** describing it as "we upload documents and it learns." Skips labelling, which
is the entire cost of the exercise, and signals you have not done it.

---

# Q3. You have one million documents. How do you design Azure AI Search?

> **⏱ ASKED:** *"1 million documents how you design AI search?"*
>
> ⚠️ **GAP QUESTION.** Swept against all 1,335 existing questions in this repo, the closest
> match was a *10-million-chunk latency troubleshooting* question in `L09:700` — diagnosing
> a slow index, not sizing one. There was no source for this. See also the companion,
> **Q15**.

**What they're testing:** Capacity planning. Anyone can describe an index; this question is
about whether you can size one, and whether you know that **one million documents is not
one million search units of anything** — it is a chunk-count problem.

**60-second spoken answer:**

> "The first thing I'd do is convert the question, because a million documents isn't the
> number that matters. What matters is chunks and their dimensionality.
>
> A million documents at, say, twenty chunks each is twenty million vectors. At 1536
> dimensions in float32 that's about 6KB per vector before overhead — roughly 120GB of
> vector data, and the index needs it resident to serve low-latency vector queries. That
> single calculation drives the entire design, and it's the calculation most people skip.
>
> From there: **service tier** sized on total index size and required queries per second.
> **Partitions** for storage — you add partitions when the data doesn't fit. **Replicas**
> for query throughput and availability — you add replicas when QPS or SLA demands it.
> Those are two independent dials and conflating them is the classic mistake.
>
> **Reduce the vector footprint** — this is where the real leverage is. Dimension reduction
> via Matryoshka truncation on models that support it, and scalar or binary quantisation,
> which cuts storage several-fold for a small recall cost.
>
> **Ingestion** is its own design — indexers with change detection for incremental updates,
> not full rebuilds.
>
> And the honest caveat: at that scale I'd want a proof-of-concept measurement on a
> representative slice before committing to a tier, because the estimate has real error
> bars."

**Depth — the four-point rule:**

1. **What it IS** — a sizing exercise driven by chunk count × dimensionality, then tier
   selection, then partitions for storage and replicas for throughput, with quantisation as
   the primary cost lever.
2. **Why it works that way** — vector search requires the index resident for low latency.
   Storage is therefore the binding constraint at scale, and storage is a function of
   vectors × dimensions × bytes-per-dimension. Every lever that matters — chunk size,
   dimension count, quantisation — moves one of those three terms.
3. **Your example** — JM Family at 500K documents. Say what you actually ran and be clear
   that a million is a different scale requiring re-measurement, not extrapolation.
4. **The trade-off** — every storage saving costs recall. Quantisation loses precision in
   distance calculations. Dimension truncation loses semantic nuance. Bigger chunks mean
   fewer vectors and worse retrieval precision. You are trading recall for cost and the
   right point is found by measuring on your own eval set, not from a vendor table.

**Whiteboard:**

```
   STEP 1 — CONVERT THE QUESTION            ⚠ this is what people skip
   1,000,000 docs × ~20 chunks     = 20,000,000 vectors
   20M × 1536 dims × 4 bytes       ≈ 120 GB raw vector data
                                     + text + metadata + index overhead

   STEP 2 — TIER          sized on total index size AND target QPS

   STEP 3 — TWO INDEPENDENT DIALS
   partitions ──▶ STORAGE      (data doesn't fit → add partitions)
   replicas   ──▶ QPS + SLA    (queries too slow / need HA → add replicas)
   ⚠ conflating these is the classic sizing error

   STEP 4 — SHRINK THE VECTORS      ← the real leverage
   quantisation (scalar / binary)   large storage cut, small recall cost
   dimension truncation (Matryoshka) 3072 → 1024 where model supports it
   chunk size ↑                      fewer vectors, worse precision

   STEP 5 — INGESTION
   indexers + change detection → incremental, never full rebuild
```

**Follow-up probes:**
- *"How do you keep query latency low at that size?"* → ANN indexing (HNSW) rather than
  exhaustive search, aggressive metadata filtering to reduce the candidate space before
  vector comparison, and replicas for concurrency. Filtering first is the biggest practical
  win and it is why metadata design matters as much as vector design.
- *"What's the cost driver?"* → Storage and the service tier, not query volume — which
  surprises people who reason from LLM pricing. Ingestion embedding cost is a large one-off
  plus a smaller ongoing cost on new documents.
- *"Would you shard across multiple indexes?"* → Legitimate at extreme scale or where a
  natural partition key exists — by business unit, by year. Costs you cross-shard queries
  and adds routing logic. Worth it when the partition matches how people actually query.
- *"What if it doesn't fit the largest tier?"* → Then you shard, or you reduce the vector
  footprint, or you reconsider whether everything needs to be in the vector index at all.
  Cold archival content behind a keyword-only index is a legitimate design.
- *"How long does initial indexing take?"* → Dominated by embedding throughput and your
  quota, not by the search service. Do the arithmetic out loud — it is the same reasoning
  as `Resume Q10`.

**Red flag:** answering with the index schema and no numbers. The question contains a
figure — one million — and an answer that never does arithmetic has not engaged with it.

---

# Q4. Which models do you choose, and why?

> **⏱ ASKED:** *"Which models you choose why?"*

**What they're testing:** Whether you have a decision framework or a favourite. The good
answer is a table of criteria, not a model name.

**60-second spoken answer:**

> "I'd answer with the criteria rather than a model, because the model changes every few
> months and the criteria don't.
>
> **Task class first.** Is this constrained output — classification, routing, extraction
> against a schema — or open-ended reasoning? Constrained tasks go to the smallest model
> that passes evaluation. Reasoning goes to the frontier model. That split alone is usually
> the largest cost lever in a system.
>
> **Context window.** How much do you actually need to fit? Long-context models cost more
> per token and there's a quality cost to very long contexts too, so it's not free capacity.
>
> **Latency budget.** Interactive path or batch? Smaller models are faster, and for
> something a user is waiting on, time-to-first-token often matters more than total quality.
>
> **Compliance and deployment.** Which models are available in the regions you're allowed to
> use, with the data-handling terms your business requires? This constraint eliminates more
> options than quality does, in enterprise.
>
> **Then cost**, which is a function of the first four rather than an independent axis.
>
> And the discipline underneath all of it: evaluate candidates on *your* task with *your*
> data. Public benchmarks tell you very little about performance on your extraction schema."

**Depth — the four-point rule:**

1. **What it IS** — a five-criterion framework: task class, context requirement, latency
   budget, compliance/availability, then cost — validated by evaluation on your own data.
2. **Why it works that way** — model quality is not one-dimensional. A model that is better
   on reasoning benchmarks may be worse at strict JSON adherence, which for an extraction
   pipeline is the only property that matters. Task-specific evaluation is the only reliable
   signal.
3. **Your example** — JM Family tiering: small models for routing, classification, and query
   rewriting; GPT-4o for final synthesis. See `Resume Q31`. At ADP/Assurant, Bedrock and
   Azure OpenAI evaluated against each other — `Resume Q58`.
4. **The trade-off** — every additional model in production is another prompt set, another
   version to pin, another eval baseline, and another thing that changes under you. Tiering
   saves money and costs maintenance. Below a certain volume the maintenance exceeds the
   saving.

**Whiteboard:**

```
   1. TASK CLASS      constrained (classify/route/extract) → smallest that passes eval
                      open-ended reasoning                 → frontier

   2. CONTEXT         how much do you ACTUALLY need? long context ≠ free

   3. LATENCY         interactive → smaller/faster, TTFT matters
                      batch       → optimise for cost

   4. COMPLIANCE  ⚠   region availability · data-handling terms
                      eliminates more options than quality does

   5. COST            a FUNCTION of 1-4, not an independent axis

   ── then EVALUATE candidates on YOUR task with YOUR data ──
      ⚠ public benchmarks say little about your extraction schema
```

**Follow-up probes:**
- *"GPT-4o versus a smaller model — where's the line?"* → Where the eval set says it is, on
  your task. Refusing to give a universal answer is correct here, and following it with
  "here's how I'd find the line" is what makes it a strong answer rather than a dodge.
- *"Open weight versus hosted?"* → Hosted for capability and zero ops. Open weight for data
  boundary requirements, cost at very high stable volume, or air-gapped operation. See
  `Resume Q42`.
- *"How often do you re-evaluate?"* → On a cadence, and whenever a materially better or
  cheaper model ships. The eval set makes this cheap, which is an underrated argument for
  having one.
- *"Would you use different models for different customers?"* → Only if compliance forces
  it. Otherwise the maintenance cost multiplies for no benefit.

**Red flag:** naming a model as the answer. "We use GPT-4o" answers a different question and
suggests the choice was never made.

---

# Q5. How do you manage the context window if it grows?

> **⏱ ASKED:** *"How you manage context window if it grows?"*

**What they're testing:** Whether you have a strategy or just truncate. Also whether you
know that a bigger context window is not the answer.

**60-second spoken answer:**

> "The instinct is to reach for a bigger window and that's usually wrong, for two reasons.
> Cost scales with what you send on every single call, so a growing context is a growing
> bill on every turn. And quality degrades in long contexts — the lost-in-the-middle
> effect, where information in the middle of a long context is attended to less reliably
> than information at either end. A bigger window can produce worse answers.
>
> So the strategy is to keep the window small deliberately. Five things, roughly in order:
>
> **Retrieve, don't stuff.** The main reason a context grows is putting whole documents in
> when you needed three paragraphs. Retrieval is context management.
>
> **Rolling window on conversation** — keep the last N turns verbatim.
>
> **Summarisation buffer** — older turns get compressed into a running summary rather than
> dropped.
>
> **Externalise** — durable facts live in a store and get retrieved when relevant, not
> carried in every call.
>
> **Priority-ordered eviction** when it still doesn't fit. System prompt never goes. The
> current turn never goes. Lowest-ranked retrieved chunks go first, then older tool results,
> then mid-conversation detail.
>
> And a hard rule: never silently truncate. If the content won't fit, that's a condition to
> handle explicitly, because a model that lost content doesn't know it lost content and
> will answer confidently anyway."

**Depth — the four-point rule:**

1. **What it IS** — a five-strategy ladder: retrieve rather than stuff, rolling window,
   summarisation buffer, externalised memory, priority eviction — with explicit failure
   rather than silent truncation.
2. **Why it works that way** — two independent pressures. Cost is linear in input tokens on
   every call, so context growth compounds across a session. Quality is non-monotonic in
   context length because attention over very long sequences degrades in the middle. Both
   argue for the same discipline.
3. **Your example** — JM Family: token-budget middleware enforcing exactly this priority
   order before every call. See `Resume Q30`.
4. **The trade-off** — every reduction loses information, and you cannot know in advance
   which loss will matter. A user referring back to something from thirty turns ago may hit
   a summary that dropped it. You are trading recall of history for cost and quality on the
   current turn.

**Whiteboard:**

```
   ✗ WRONG INSTINCT: "use a bigger context window"
       cost      linear in input tokens, on EVERY call
       quality   lost-in-the-middle — long contexts attend worse in the middle

   ✓ THE LADDER
   1. RETRIEVE, don't stuff       ← biggest win. retrieval IS context management
   2. rolling window              last N turns verbatim
   3. summarisation buffer        older turns → running summary, not deleted
   4. externalise                 durable facts in a store, retrieved on relevance
   5. priority eviction           system prompt ✗ never · current turn ✗ never
                                  lowest-ranked chunks → old tool results → detail

   ⚠ NEVER silently truncate — the model doesn't know it lost content
     and will answer confidently anyway
```

**Follow-up probes:**
- *"What is lost-in-the-middle?"* → Retrieval quality within the context is position-
  dependent: content at the start and end is used more reliably than content in the middle.
  Practical consequence — put the most important retrieved chunk first, not fifth.
- *"When would you use a long-context model?"* → When a single document genuinely must be
  reasoned over whole and cannot be usefully chunked — some legal and clinical documents
  qualify. Not as a substitute for retrieval.
- *"How do you summarise without losing what matters?"* → Structured summarisation against a
  schema — decisions made, entities mentioned, open questions — rather than free-form
  prose. Free-form summarisation drops whatever the summariser found uninteresting.
- *"What if the user asks about something that was summarised away?"* → Either the system
  retrieves the original turn from the thread store, or it says it does not have it. The
  failure mode to avoid is answering from a lossy summary as though it were complete.

**Red flag:** "we truncate the oldest messages." It is the crudest strategy, it is
lossy in an uncontrolled way, and it does not mention retrieval at all.

---

# Q6. What type of compression do you use?

> **⏱ ASKED:** *"What type of compression you use?"*
>
> ⚠️ **GAP QUESTION.** Nothing in the repo covered this — best lexical match across 1,335
> questions scored 0.22 and was unrelated. Companion deep-dive at **Q16**.

**What they're testing:** Whether "compression" means anything specific to you. It is a
vague question and the strong move is to enumerate the distinct kinds, because most
candidates name one.

**60-second spoken answer:**

> "There are several distinct things called compression in an LLM system and they're worth
> separating, because they operate at different layers.
>
> **Conversation compression** — summarising older dialogue turns into a running summary.
> Lossy by design, and the standard approach to unbounded history.
>
> **Context compression at retrieval time** — this is the one I'd emphasise. After
> retrieving chunks, you reduce them before they reach the prompt. Two flavours: extractive,
> where you select the sentences within a chunk that actually relate to the query and drop
> the rest, and abstractive, where you summarise the chunk. Extractive is safer because it
> preserves original wording, which matters when you're citing sources.
>
> **Prompt compression** — techniques that remove low-information tokens from a prompt,
> LLMLingua being the well-known family. Real compression ratios, and the risk is that
> what looks low-information to the compressor may be load-bearing.
>
> **Semantic deduplication** — when retrieval returns three chunks saying the same thing,
> you're paying three times for one fact. Dedup before the prompt.
>
> **And reranking as compression**, which is the one people don't label as compression at
> all — going from ten retrieved chunks to three is a 70% reduction in retrieved tokens and
> it improves quality rather than degrading it. That's the cheapest compression available
> and most systems already have it."

**Depth — the four-point rule:**

1. **What it IS** — five distinct mechanisms: conversation summarisation, retrieval-time
   context compression (extractive or abstractive), prompt compression, semantic dedup, and
   reranking-as-compression.
2. **Why it works that way** — each targets a different source of bloat. History grows with
   the session; retrieved context is bloated per call; prompts accumulate boilerplate;
   retrieval returns redundancy. One technique cannot address all four, which is why "what
   compression do you use" has a list answer rather than a single one.
3. **Your example** — JM Family: reranking from top-10 to top-3 was a substantial prompt-
   token reduction with no measurable quality loss, and summarisation on conversation
   history. See `Resume Q29`.
4. **The trade-off** — the ordering by risk matters and is worth stating. Reranking is
   safe — it improves quality. Dedup is nearly safe. Extractive compression is moderate
   risk. Abstractive and prompt compression are the riskiest, because the compressor is
   making a judgment about what matters and it does not know your task. Anything lossy needs
   evaluation before and after, not assumption.

**Whiteboard:**

```
   MECHANISM                  TARGETS              RISK      NOTE
   ─────────                  ───────              ────      ────
   reranking (top-k ↓)        retrieved bloat      NONE ✓    improves quality too
                                                              cheapest, usually already there
   semantic dedup             redundant chunks     low       3 chunks, 1 fact
   extractive compression     within-chunk noise   moderate  preserves wording → citable
   conversation summarisation history growth       moderate  lossy by design
   abstractive compression    within-chunk noise   higher    rewrites — citation breaks
   prompt compression         prompt boilerplate   HIGHEST   compressor doesn't know
   (LLMLingua family)                                        what's load-bearing

   ⚠ anything lossy → evaluate before/after. never assume.
```

**Follow-up probes:**
- *"Which do you reach for first?"* → Reranking, because it is the only one that improves
  quality while reducing tokens. Then dedup. Lossy techniques last.
- *"Doesn't extractive compression break citations?"* → No, and that is the point —
  extractive keeps original sentences, so a citation still points at real source text.
  Abstractive rewrites, which is precisely why it is riskier in a grounded system.
- *"What compression ratio is realistic?"* → Depends entirely on redundancy in the source.
  The honest answer is that you measure it on your corpus rather than quoting a published
  figure from a different one.
- *"Would you compress the system prompt?"* → Rarely worth it — it is stable, and prompt
  caching at the provider level addresses the same cost far more safely. Compressing the
  instructions that define behaviour is a poor risk-reward trade.

**Red flag:** naming only conversation summarisation. It is one of five and the question
was plural in intent.

---

# Q7. Design the lifecycle of RAG.

> **⏱ ASKED:** *"Design the life cycle of RAG."*
>
> Note "lifecycle," not "architecture." They want the **operational loop** — including what
> happens after it ships — not just the request path. Answering with only the request path
> is answering a different question.

**What they're testing:** Whether you think of RAG as a running system with maintenance, or
as a pipeline you build once.

**60-second spoken answer:**

> "I'd split it into a build phase, a serve phase, and an operate loop — and the operate
> loop is the part that distinguishes a production system.
>
> **Build:** source and ingest documents. Extract, handling scans, tables and layout. Chunk
> — structure-aware, with overlap. Redact PII *before* anything is persisted. Embed. Index,
> with metadata for filtering and access control.
>
> **Serve:** receive the query. Rewrite it if it's conversational, so it stands alone.
> Retrieve — hybrid, keyword plus vector. Rerank to the top few. Assemble the prompt with
> grounding instructions and citation requirements. Generate. Validate the output —
> groundedness and content safety. Return with citations.
>
> **Operate — the loop that most designs omit:** evaluate continuously against a fixed set;
> gate every prompt, retrieval or model change on that evaluation; monitor cost, latency and
> quality together; keep the index fresh as source documents change; and refresh the
> evaluation set itself from real production queries, because the questions people actually
> ask drift away from the ones you anticipated.
>
> If I had to name the single thing that separates a RAG demo from a RAG system, it's that
> third phase existing at all."

**Depth — the four-point rule:**

1. **What it IS** — three phases: build (ingest → chunk → redact → embed → index), serve
   (rewrite → retrieve → rerank → ground → generate → validate), operate (evaluate → gate →
   monitor → refresh index → refresh eval set).
2. **Why it works that way** — RAG systems decay. Documents change, so the index goes stale.
   Query patterns drift, so the eval set stops representing reality. Model versions change
   underneath you. Without the operate loop, quality degrades invisibly, because nothing in
   a RAG system alerts you that answers got worse.
3. **Your example** — JM Family, 500K documents with RAGAS-gated changes. Full architecture
   in `Resume Q7`; ingestion and failure handling in `Resume Q10`.
4. **The trade-off** — the operate loop is ongoing cost with no visible feature output. It is
   the first thing cut under delivery pressure and the reason systems degrade six months
   after launch. Being able to say that out loud — naming it as the thing that gets cut — is
   what makes the answer sound experienced rather than diagrammed.

**Whiteboard:**

```
   ┌─ BUILD ────────────────────────────────────────────────────┐
   │ source → extract (OCR/layout/tables) → chunk (structure-    │
   │ aware + overlap) → ⚠ PII redact → embed → index + metadata  │
   └───────────────────────────┬────────────────────────────────┘
                               ▼
   ┌─ SERVE ────────────────────────────────────────────────────┐
   │ query → rewrite → hybrid retrieve → rerank → ground+cite    │
   │ → generate → validate (groundedness + safety) → return      │
   └───────────────────────────┬────────────────────────────────┘
                               ▼
   ┌─ OPERATE ⚠ the phase most designs omit ────────────────────┐
   │ evaluate on fixed set → GATE every change → monitor cost/   │
   │ latency/quality → refresh index on doc change → refresh the │
   │ EVAL SET from real production queries                       │
   └────────────────────────────────────────────────────────────┘
                               │
                               └──────▶ feeds back into BUILD and SERVE
```

**Follow-up probes:**
- *"How do you keep the index fresh?"* → Event-driven on document change with deterministic
  chunk IDs so updates are upserts. See `Resume Q10`.
- *"When would you rebuild rather than update?"* → Chunking strategy change or embedding
  model change. Both invalidate existing vectors. `Resume Q45` covers the side-by-side
  rebuild.
- *"How do you refresh the eval set?"* → Sample from production query logs, including — and
  especially — queries that failed or were re-asked. Failed queries are the best source of
  new eval cases.
- *"Where does CAG fit?"* → Cache-augmented generation preloads a stable, bounded corpus
  into the context rather than retrieving per query. Viable when the corpus is small enough
  to fit and rarely changes. At 500K documents it is not an option. See `L23_CAG_vs_RAG.md`.

**Red flag:** describing only the request path. The word "lifecycle" was in the question and
an answer that stops at "generate" has not heard it.

---

# Q8. Why and when do you use AKS with KEDA autoscaling for AI?

> **⏱ ASKED:** *"Why and when you use AKS keda auto scale for AI?"*
>
> ⚠️ **GAP QUESTION.** The weakest coverage of all fourteen — best match across the repo
> scored **0.12** and was unrelated. `L34` teaches Kubernetes but never connects it to AI
> workloads. Companion deep-dive at **Q17**.

**What they're testing:** Whether you understand why AI workloads break standard
autoscaling. The answer is a property of the workload, not a preference for a tool.

**60-second spoken answer:**

> "Because for AI workloads, CPU is not the demand signal — and CPU is what the standard
> Kubernetes autoscaler scales on.
>
> An AI worker spends most of its life blocked on a network call: waiting for an embedding
> endpoint, a model completion, a document extraction service. It's I/O bound. So a thousand
> documents can pile up in a queue while CPU utilisation sits low, and the HPA looks at a
> healthy cluster and does nothing. The backlog grows and nothing scales.
>
> KEDA scales on external metrics — queue depth, event rate, custom metrics. The number of
> pending messages *is* the demand, measured directly rather than inferred from a proxy
> that doesn't correlate.
>
> The second reason is scale-to-zero. AI workloads are frequently bursty — a heavy ingestion
> run, then hours of nothing. The HPA's floor is one replica; KEDA can go to zero and start
> on the first event.
>
> **When not to:** anything on an interactive path. Scale-to-zero means a cold start, and
> for a user waiting on a response that's unacceptable. The rule I'd apply is KEDA with
> scale-to-zero for queue-driven asynchronous work, and a warm floor for anything
> synchronous."

**Depth — the four-point rule:**

1. **What it IS** — event-driven autoscaling on external metrics with scale-to-zero, versus
   HPA's resource-utilisation model with a floor of one replica.
2. **Why it works that way** — AI workloads are I/O bound almost by definition. The worker is
   waiting on a remote model, not computing. Every resource-based autoscaler misreads that
   as idle capacity. Scaling on backlog measures demand directly.
3. **Your example** — JM Family ingestion workers on Service Bus queue depth. See
   `Resume Q41` for the implementation and the quota-ceiling detail.
4. **The trade-off** — scale-to-zero buys a cold start, and for anything that loads a model
   locally the cold start is measured in tens of seconds. That makes it excellent for async
   ingestion and unusable on an interactive path. The decision rule follows directly from
   that.

**Whiteboard:**

```
   WHY CPU FAILS AS A SIGNAL FOR AI WORK
   worker lifecycle:  ▓ compute (small)  ░░░░░░░ blocked on network (most of it)
                      → CPU looks idle while 1,000 messages queue up
                      → HPA sees a healthy cluster, does nothing

   HPA                              KEDA
   ───                              ────
   CPU / memory                     external metric — QUEUE DEPTH
   floor = 1 replica                floor = 0 (scale to zero)
   infers demand from a proxy       measures demand directly

   WHEN                             WHEN NOT
   ────                             ────────
   queue-driven ingestion   ✓       interactive / user-waiting   ✗
   batch document processing ✓      anything loading a local model ✗
   bursty, idle much of the day ✓     (cold start = tens of seconds)
   → scale to zero                  → keep a warm floor

   ⚠ ALWAYS cap maxReplicaCount below your downstream QUOTA
     scaling past the quota turns a backlog into a wall of 429s
```

**Follow-up probes:**
- *"What metric specifically?"* → Service Bus active message count with a target
  messages-per-replica, or any KEDA scaler matching your event source.
- *"What stops it scaling infinitely?"* → `maxReplicaCount`, and it must be set against the
  downstream quota rather than against cluster capacity. This is the detail that proves
  production experience — more workers past the quota just move the bottleneck and generate
  retry storms.
- *"KEDA for GPU inference?"* → Economics are stronger because GPU nodes are expensive, cold
  start is much worse because of node provisioning plus model loading. Usually a warm pool
  with KEDA scaling above it rather than true scale-to-zero.
- *"Why AKS at all rather than Functions or Container Apps?"* → Good question and the honest
  answer is that Functions or Container Apps are often the better choice for exactly this
  workload. AKS wins when you already run a cluster, need specific networking, GPU node
  pools, or fine-grained control. Choosing AKS by default is a red flag in itself.

**Red flag:** answering "KEDA scales better." It is not a reason. The reason is that the
demand signal for an I/O-bound AI workload is queue depth, and CPU-based autoscaling cannot
see it.

---

# Q9. Which chunking strategy is best, and which do you choose?

> **⏱ ASKED:** *"Which chunking strategy is best and which one to choose?"*
>
> The question contains a trap — "which is best" has no answer, and saying so is part of
> the answer.

**What they're testing:** Whether you will name a favourite or give a decision rule. Also
whether you know chunking is evaluated, not chosen.

**60-second spoken answer:**

> "There isn't a best one, and I'd say that first — the choice is driven by document
> structure and by what queries need to be answered.
>
> **Fixed-size** splits by token count. Simplest, fastest, and it cuts through the middle of
> sentences and tables. Fine for uniform prose, poor for anything structured.
>
> **Recursive** tries progressively smaller structural boundaries — sections, then
> paragraphs, then sentences — falling back only when it must. This is my default for
> production. It respects document structure without the cost of semantic analysis.
>
> **Semantic** places boundaries where the meaning shifts, detected by embedding similarity
> between adjacent segments. Better coherence, meaningfully more expensive to compute, and
> the cost recurs on every re-index.
>
> **Document-structure-aware** — using the actual layout: headings, sections, and critically
> keeping tables atomic. For contracts, forms, and clinical documents this beats all of the
> above, because the structure carries meaning.
>
> My decision rule: recursive as the default, structure-aware when documents have real
> structure worth preserving, semantic only when I've measured that it justifies its cost.
> And whatever I choose, chunk size and overlap get tuned against a retrieval eval set
> rather than picked from a blog post."

**Depth — the four-point rule:**

1. **What it IS** — four strategies with different structure-awareness and cost, chosen by
   document type and validated by retrieval evaluation.
2. **Why it works that way** — chunking determines what can be retrieved *at all*. If the
   answer to a question is split across two chunks, no retrieval strategy, no reranker, and
   no prompt recovers it. Chunking is the upstream constraint on everything downstream,
   which is why it deserves measurement rather than a default.
3. **Your example** — JM Family: recursive, structure-aware, roughly 500 tokens with ~15%
   overlap, tables kept atomic because insurance forms are heavily tabular. See
   `Resume Q7`.
4. **The trade-off** — small chunks give precise retrieval and lose surrounding context;
   large chunks preserve context and dilute the embedding, so retrieval gets less precise
   and you pay more tokens per retrieved chunk. Overlap mitigates boundary loss and costs
   storage and duplicate retrieval. Every dial trades against another.

**Whiteboard:**

```
   STRATEGY            RESPECTS STRUCTURE   COST      USE WHEN
   ────────            ──────────────────   ────      ────────
   fixed-size          none                 lowest    uniform prose, prototypes
   recursive           boundaries           low       ← DEFAULT for production
   semantic            meaning shifts       high      measured to justify the cost
   structure-aware     layout + tables      medium    contracts · forms · clinical

   SIZE vs OVERLAP
   small chunks  → precise retrieval, loses surrounding context
   large chunks  → keeps context, dilutes embedding, costs more per retrieval
   overlap       → protects boundary facts, costs storage + duplicate hits

   ⚠ TUNE AGAINST A RETRIEVAL EVAL SET. never pick from a blog post.
   ⚠ if the answer spans two chunks, NOTHING downstream recovers it
```

**Follow-up probes:**
- *"Do you chunk a 3-page and a 300-page document the same way?"* → No. A short document may
  be one chunk. A long one needs hierarchical treatment — section summaries alongside
  detailed chunks, so a query can hit the right section before the right paragraph.
- *"What chunk size?"* → No universal number. Roughly 200–800 tokens is where most systems
  land, and the actual figure comes from evaluation on your corpus. Giving a number without
  that caveat suggests it was copied.
- *"How do you handle tables?"* → Keep them atomic — never split a table across chunks — and
  consider a separate extraction path that preserves table structure. Splitting tables is
  one of the most common silent quality killers in document RAG.
- *"What about code, or nested documents?"* → Structure-aware by language or schema. The
  general principle holds: split at boundaries the content itself defines.
- *"Would you change chunking after launch?"* → It means re-embedding the whole corpus, so
  it is an expensive decision to revisit. That is the argument for evaluating chunking
  properly before the first full index build.

**Red flag:** naming one strategy as best. The question invited it and the correct answer
refuses it. Second red flag: quoting a chunk size with no mention of evaluation.

---

# Q10. How do you manage PII?

> **⏱ ASKED:** *"How pii management?"*
>
> ⚠️ **GAP QUESTION.** Best match across all 1,335 existing questions scored **0.17** and was
> about LlamaIndex evaluation — i.e. nothing. Companion deep-dive at **Q18**.

**What they're testing:** Whether you know that PII in an AI system is a *placement*
problem, not a tooling problem. Naming a detection service is the easy quarter of the
answer.

**60-second spoken answer:**

> "The decision that matters is *where in the pipeline* it happens, and the answer is: as
> early as possible, before anything is persisted.
>
> The reason is that a RAG system makes copies. The moment content is embedded, it's in the
> vector store. The moment a prompt is sent, it's in your logs and possibly the provider's.
> The moment an answer is cached, it's in the cache. Redacting on the way out to the user
> doesn't undo any of that.
>
> So the order is: extract, detect, then redact or tokenise, *then* embed and index.
> Detection is Azure AI Language for standard entity types — names, addresses, national IDs,
> financial identifiers — plus custom patterns for domain identifiers that generic detection
> doesn't recognise, like internal policy or account formats.
>
> Then the redact-versus-tokenise decision. **Redaction** removes the value permanently —
> one-way, and it destroys the ability to answer questions about that entity.
> **Tokenisation** replaces it with a stable token and keeps the mapping in a separately
> secured store, so downstream processes can re-identify through a controlled, audited path.
>
> And the part most people miss: the same discipline has to apply to logs, caches, and
> evaluation sets, because those are copies too."

**Depth — the four-point rule:**

1. **What it IS** — detect-then-redact-or-tokenise placed before embedding and persistence,
   with the same treatment extended to every derived copy: logs, caches, eval sets.
2. **Why it works that way** — redaction after persistence is not redaction, it is deletion
   work. Placing the control before the first write means the unsafe state never exists,
   which is the same structural-control principle as the token-budget choke point and the
   security filter — controls that fail closed rather than relying on someone remembering.
3. **Your example** — JM Family: redaction as a mandatory ingestion pipeline stage before
   embedding. See `Resume Q37` for the pipeline placement and `Resume Q40` for how it was
   enforced structurally.
4. **The trade-off** — redaction destroys capability. A corpus with names removed cannot
   answer questions about named individuals, which is sometimes exactly what the business
   needs. Tokenisation preserves the capability and creates a re-identification path, which
   is a new attack surface with its own access controls to get right. There is no free
   option — you are choosing which risk to hold.

**Whiteboard:**

```
   extract ──▶ DETECT ──▶ REDACT or TOKENISE ──▶ embed ──▶ index
                              ▲
                              └── ⚠ MUST be here — before the FIRST write

   ✗ redact at query time  → already in the vector store
   ✗ redact on output      → already in logs, cache, provider request records

   DETECT   Azure AI Language (standard entities)
          + custom patterns (policy numbers, internal account formats)

   REDACT      one-way · original not retained · destroys capability
   TOKENISE    stable token + mapping in SEPARATE secured store
               reversible through an audited path · new attack surface

   ⚠ THE COPIES PEOPLE FORGET  →  see Q18
     logs · caches · evaluation sets · provider retention
```

**Follow-up probes:**
- *"How do you handle false negatives in detection?"* → You assume they exist. Detection is a
  model and it misses things, especially unusual formats and non-English content. Mitigation
  is defence in depth: restrict access to the index as though it still contains PII, rather
  than treating redaction as making the data public-safe.
- *"What about PII the model infers?"* → A real and underappreciated risk — a model can
  reconstruct or infer an identity from non-identifying details. Redaction addresses stated
  PII, not inferred. Naming this is a strong differentiator.
- *"Does the provider retain your prompts?"* → Depends on deployment and configuration. Know
  your Azure OpenAI data-handling terms and whether abuse-monitoring retention was disabled
  for the workload. Compliance-minded interviewers ask this specifically.
- *"Right to erasure?"* → Requires enumerating every derived copy via a document-to-chunk
  mapping. Full answer in `Resume Q38`.
- *"Would you let PII into the context window at all?"* → Sometimes you must — a legitimate
  query about a specific customer needs their data. The control then is access, not
  redaction: the right user, the right scope, and audit. Recognising that redaction is not
  always the answer is the mature position.

**Red flag:** naming Azure AI Language PII detection and stopping. The tool is the easy
part; placement is the design.

---

# Q11. How can we save tokens?

> **⏱ ASKED:** *"How token saving we ca do?"*

**What they're testing:** Whether you have a ranked list of levers with real magnitudes, or
one tip. This is a cost question in disguise and the ranking is the answer.

**60-second spoken answer:**

> "I'd rank them by actual impact, because they're not close to equal.
>
> **Retrieval discipline is the biggest by a distance.** Prompt tokens dominate cost in a RAG
> system, and retrieved context dominates prompt tokens. Going from ten chunks to three
> after reranking is a large cut, and it usually *improves* quality. That's the first thing
> I'd look at in any system.
>
> **Model tiering** — route constrained tasks to a small model, keep the frontier model for
> final synthesis. Typically the largest saving after retrieval.
>
> **Caching** — exact-match on repeated queries. Enterprise query distributions are heavily
> repetitive, so this pays more than people expect.
>
> **Prompt caching** at the provider level, where a stable prefix is billed at a reduced
> rate. Nearly free if your prompt is already structured with the stable part first.
>
> **Prompt hygiene** — trimming verbose system prompts, removing few-shot examples that a
> fine-tune or a clearer instruction made unnecessary, and not re-sending context that
> hasn't changed.
>
> **Output limits** — `max_tokens` and instructions to be concise. Real, but output is
> usually the smaller half of the bill.
>
> The thing I'd say last but mean first: you can't rank any of this without per-call
> telemetry. Optimising before you can attribute spend means optimising the wrong thing."

**Depth — the four-point rule:**

1. **What it IS** — a ranked list: retrieval discipline, model tiering, response caching,
   prompt caching, prompt hygiene, output limits — gated on having per-call cost telemetry.
2. **Why it works that way** — in a RAG system the input side dominates, and within the
   input, retrieved context dominates. Most people optimise output length because it is
   visible, and it is the smallest term. Following the arithmetic rather than the intuition
   is the whole answer.
3. **Your example** — JM Family: this ranking is what produced the 30% / ~$150K reduction.
   The full story, including which lever contributed most, is `Resume Q29`; the middleware
   that enforced it is `Resume Q30`.
4. **The trade-off** — each lever has a quality cost that must be measured. Fewer chunks can
   drop the answer. A smaller model can miss nuance. A cache can serve stale content.
   Without an eval gate you are not saving cost, you are trading quality for cost blind.

**Whiteboard:**

```
   RANKED BY IMPACT          MECHANISM                        QUALITY RISK
   ────────────────          ─────────                        ────────────
   1. retrieval discipline   top-k 10 → 3 after rerank        ↑ often IMPROVES
   2. model tiering          small for constrained tasks      measure per task
   3. response caching       exact-match on repeats           staleness
   4. prompt caching         stable prefix, reduced rate      none
   5. prompt hygiene         trim system prompt, drop         low
                             unnecessary few-shot
   6. output limits          max_tokens, "be concise"         truncation

   ⚠ INPUT dominates cost in RAG. within input, RETRIEVED CONTEXT dominates.
     most people optimise output first — it's the smallest term.

   ⚠ PREREQUISITE: per-call telemetry tagged by feature.
     no attribution → you optimise the wrong thing.
```

**Follow-up probes:**
- *"What's the single biggest lever?"* → Retrieval discipline, and it is the one that
  improves quality at the same time.
- *"Does a shorter prompt hurt quality?"* → Sometimes. Removing genuine instruction hurts;
  removing boilerplate does not. The eval gate is what distinguishes them.
- *"How do you count tokens accurately?"* → The model family's tokenizer, `tiktoken` for
  OpenAI. Character heuristics are wrong in different directions for code and non-English
  text.
- *"What about fine-tuning to shorten prompts?"* → Legitimate — a fine-tuned model needs less
  instruction and fewer examples, so prompt tokens drop on every call. Pays at sustained
  volume on a narrow task. See `Resume Q56`.

**Red flag:** starting with "we limit max_tokens." It is the smallest lever and starting
there signals you have not looked at where the spend actually is.

---

# Q12. Why A2A?

> **⏱ ASKED:** *"Why A2A?"*
>
> Two words. They are testing whether you can articulate the problem it solves — and
> whether you will confuse it with MCP, which is the most common error.

**What they're testing:** Whether you understand the *boundary* A2A operates across.
Candidates routinely describe their internal multi-agent orchestration as A2A, which it is
not.

**60-second spoken answer:**

> "A2A solves agent interoperability *across organisational boundaries*, and that's the
> distinction I'd lead with — because it's routinely confused with two things it isn't.
>
> It isn't MCP. MCP connects a model to tools and data — it's vertical, model down to
> capability. A2A connects agents to each other — it's horizontal, peer to peer. They
> compose; they don't compete.
>
> And it isn't internal orchestration. If I have a supervisor agent calling specialist
> agents inside one system that I own, that's orchestration. I control both sides, I can
> define the interface however I like, and a framework handles it. A2A isn't needed and adds
> nothing.
>
> A2A matters when the agents are owned by different parties. A provider's agent needs to
> talk to a payer's agent for a prior authorisation. Neither controls the other's
> implementation, neither can dictate the interface, and they need a common way to discover
> what the other can do, delegate a task, and track it to completion. That's what the
> protocol standardises — capability advertisement through agent cards, task delegation, and
> a state model for long-running work.
>
> So: why A2A? Because without it, every cross-organisation agent integration is a bespoke
> contract, and that doesn't scale past a handful of partners."

**Depth — the four-point rule:**

1. **What it IS** — an open protocol for agent-to-agent interoperability across trust and
   organisational boundaries: capability discovery via agent cards, task delegation, and a
   lifecycle model for long-running asynchronous tasks.
2. **Why it works that way** — the N×M integration problem again, one layer up. MCP solved it
   for model-to-tool; A2A solves it for agent-to-agent. Without a standard, every partner
   integration is a bespoke API contract with its own auth, schema, and error semantics.
3. **Your example** — healthcare is the clearest: provider agent to payer agent for prior
   authorisation, where the two organisations have no shared codebase and no ability to
   mandate each other's implementation. Your own multi-agent work at JM Family is
   orchestration, not A2A — and saying so explicitly is what proves you understand the
   difference. See `Resume Q15`.
4. **The trade-off** — protocol overhead for a problem you may not have. Inside one system,
   direct calls are simpler, faster, and easier to debug. A2A also introduces trust
   questions that internal orchestration never has: how do you verify a remote agent's
   claimed capability, and what happens when it lies about a task's completion?

**Whiteboard:**

```
   VERTICAL — MCP                      HORIZONTAL — A2A
   model ──▶ tools & data              agent ◀──▶ agent
   one owner                           DIFFERENT owners, different trust domains

   ┌──────────────┐                    ┌──────────────┐
   │ Provider     │ ── agent card ───▶ │ Payer        │
   │ agent        │ ◀── capability ─── │ agent        │
   │              │ ── delegate task ─▶│              │
   │              │ ◀── task state ─── │              │
   └──────┬───────┘                    └──────┬───────┘
          │ MCP                               │ MCP
          ▼                                   ▼
      EHR · FHIR                          claims system

   ✗ NOT A2A: supervisor → specialist inside ONE system you own
              that's orchestration. a framework handles it.
   ✓ IS A2A:  neither side controls the other's implementation
```

**Follow-up probes:**
- *"What's an agent card?"* → A machine-readable advertisement of what an agent can do, its
  endpoint, and its authentication requirements — the discovery mechanism that lets one
  agent find and evaluate another without a prearranged contract.
- *"How is this different from just calling an API?"* → An API exposes fixed operations. A2A
  assumes the other side is an agent — it can reason, it may take time, it may need to come
  back with clarifying questions. Hence task state and long-running lifecycle rather than
  request/response.
- *"Do you trust a remote agent's output?"* → No, and this is the sharp question. Same
  discipline as any untrusted input: validate against your own systems of record, do not
  let a remote agent's assertion trigger a consequential action unverified.
- *"Have you used it in production?"* → `[CONFIRM]`. Be honest. Understanding the problem it
  solves is credible; claiming production use you do not have is not, and the follow-up
  about failure handling will find it.
- *"MCP and A2A together?"* → Yes — the normal picture. Each agent uses MCP to reach its own
  tools and A2A to reach peer agents. Drawing that is the strongest version of this answer.

**Red flag:** describing your internal supervisor/worker topology as A2A. It is the single
most common error on this question and it reveals the boundary was never understood.

---

# Q13. Explain the entire agent process you implemented.

> **⏱ ASKED:** *"Explain entire agent process you implemented."*
>
> This is a 4–5 minute answer, not 60 seconds. It is the centrepiece question and it is
> asked in almost every agentic interview.
>
> **Ownership:** this entry owns the **generic lifecycle** — the eleven stages any
> production agent goes through. `Resume Q15` owns **your specific JM Family topology**.
> Interviewers ask both forms. The right move is to give the lifecycle *anchored in* your
> system, which is what the answer below does.

**What they're testing:** Everything at once — architecture, failure handling, grounding,
observability, and whether you can hold a structured five-minute answer without rambling.

**The structure to deliver — eleven stages:**

> **1 — RECEIVE.** The agent is triggered. In our case an HTTP POST from the .NET API layer
> carrying the query, a session ID, and context identifiers. Other triggers: a Teams
> message, an Event Grid event on document arrival, a queue message, a timer for batch work.
>
> **2 — REASON.** The model reads the system prompt, the task, and relevant history, and
> decides what needs to happen. This is where the system prompt does its work — it defines
> the agent's scope, its tools, and its constraints.
>
> **3 — PLAN.** It selects which tools to call and in what order. Dynamic function calling —
> the tools aren't hard-coded into the flow, they're offered as schemas and the model
> chooses. Tool descriptions are the selection signal, so they're written as carefully as
> prompts.
>
> **4 — RETRIEVE.** Where the task needs knowledge, RAG runs — hybrid search, reranked,
> scoped by the user's access at query time, never after.
>
> **5 — TOOL CALL.** The agent invokes external systems. Every invocation passes through a
> filter that checks the allow-list, validates arguments against the schema, and logs inputs
> and outputs.
>
> **6 — OBSERVE.** It reads the result and updates its reasoning. A structured error here is
> recoverable — the agent adapts rather than failing.
>
> **7 — LOOP.** Steps 3 to 6 repeat until the task is done. Bounded — a hard iteration cap
> and a cumulative token budget for the run. Hitting either is a logged failure, not a
> silent truncation.
>
> **8 — GENERATE.** Final response, grounded in retrieved context, with citations, and
> instructed to say it doesn't know where the context doesn't support an answer.
>
> **9 — VALIDATE.** Guardrails before the user sees anything: Content Safety on the output,
> and a groundedness check that the claims are supported by the retrieved sources.
>
> **10 — RESPOND.** Validated output returned with its citations and its provenance.
>
> **11 — MONITOR.** Every stage emits to App Insights under one correlation ID, so any run
> is fully reconstructable — which agent ran, which tools, with what arguments, what came
> back, what the guardrails decided.

**Depth — the four-point rule:**

1. **What it IS** — an eleven-stage bounded ReAct loop with retrieval, allow-listed tool
   invocation, output validation, and end-to-end tracing.
2. **Why it works that way** — the loop is what makes it an agent rather than a pipeline; the
   *bounds* are what make it production-safe. Every constraint in the description —
   iteration cap, token budget, allow-list, schema validation, groundedness check — exists
   because of a specific failure mode, and being able to attach the failure to each control
   is what makes this answer land.
3. **Your example** — JM Family document workflows. Topology, permissions, and framework
   split in `Resume Q15`–`Q17`; failure handling in `Resume Q19`; observability in
   `Resume Q46`.
4. **The trade-off** — every bound reduces capability. The iteration cap fails some
   legitimately complex tasks. The allow-list blocks some legitimate actions. You are
   choosing predictability over capability, and for a regulated client that is right —
   but it is a choice, not a free win.

**Whiteboard:**

```
   [1] RECEIVE     HTTP · Teams · Event Grid · queue · timer
        ▼
   [2] REASON      system prompt + task + history
        ▼
   [3] PLAN        dynamic function calling — tool descriptions ARE the signal
        ▼
   [4] RETRIEVE    hybrid + rerank, access-scoped AT QUERY TIME
        ▼
   [5] TOOL CALL   ── filter: allow-list · schema validation · log in/out
        ▼
   [6] OBSERVE     structured error → agent adapts
        ▼
   [7] LOOP ───────┘  ⚠ BOUNDED: iteration cap + cumulative token budget
        ▼              hitting either = LOGGED FAILURE, not silent truncation
   [8] GENERATE    grounded · cited · "say I don't know"
        ▼
   [9] VALIDATE    Content Safety + groundedness  ← before the user sees it
        ▼
  [10] RESPOND     with citations and provenance
        ▼
  [11] MONITOR     one correlation ID through every stage → run is reconstructable
```

**Follow-up probes:**
- *"What happens at step 7 if it never converges?"* → The cap fires, the run fails explicitly
  with partial results, and it is logged as a capped run so the rate is visible as a metric.
- *"How do you debug a wrong answer?"* → Walk the trace backwards. Was the right content
  retrieved? If no, retrieval problem. If yes, generation problem. That bisection is the
  reason retrieval and generation are measured separately.
- *"Where does it fail most in production?"* → Tool selection when tool descriptions overlap,
  and retrieval on documents that extracted badly. Both are silent failures — the system
  produces a confident answer either way.
- *"Would you build this as an agent again?"* → For parts. Naming the part that should have
  been a deterministic pipeline is the strongest possible close to this answer. See
  `Resume Q20`.

**Red flag:** narrating a happy path with no bounds, no validation, and no observability.
The stages that make it production-grade are 5, 7, 9 and 11 — an answer without them
describes a demo.

---

# Q14. Explain each component of Azure AI Foundry.

> **⏱ ASKED:** *"Explain each component of azure foundry."*
>
> ⚠️ **GAP QUESTION.** `L17` and `L22` teach the platform, but nothing in the repo enumerates
> the components one by one with what each is *for*. "Each component" means they want the
> list — and they want to hear which ones you have actually touched.

**What they're testing:** Platform depth. This question is easy to fake at surface level and
impossible to fake one layer down, which is why interviewers like it. Say what each thing
is, when you touch it, and what it costs you.

**60-second spoken answer** *(this one runs long — the component walk is the answer)*:

> "I'd group them into five layers: the workspace layer, the model layer, the build layer,
> the run layer, and the govern layer.
>
> **Hub** — the top-level workspace. It's where shared infrastructure lives: connections to
> data sources and services, compute, security configuration, and the storage and key vault
> the projects use. You set it up once per team or per environment.
>
> **Project** — a working container inside a Hub. This is where an individual solution
> lives — its deployments, its data, its evaluations. Projects inherit the Hub's connections
> and security, which is the point: configure once, reuse across projects.
>
> **Model Catalog** — the model selection surface. Azure OpenAI models, open-weight models,
> Microsoft's own, and third-party. This is where the model choice from Q4 actually gets
> made.
>
> **Deployments** — a model made callable at an endpoint, with a chosen capacity model.
> That's where you decide between pay-as-you-go and provisioned throughput, which is a
> significant cost and latency decision, not an afterthought.
>
> **Agent Service** — the managed agent runtime: agent definitions, tool registration,
> thread and state management. It's the hosted alternative to running your own orchestration
> loop.
>
> **Prompt Flow** — visual authoring and orchestration for prompt-based flows, with
> evaluation built in. Good for iteration; I'd still keep the production artefact in Git.
>
> **Evaluations** — built-in evaluators for groundedness, relevance, coherence, safety, run
> against datasets. The platform-native counterpart to running RAGAS yourself.
>
> **Content Safety** — the harm-category filtering layer, configurable per deployment.
>
> **Connections** — managed, credential-holding links to data sources and services: AI
> Search, storage, other Azure resources. This is what keeps keys out of application code.
>
> **Compute** — the underlying instances for anything you run yourself, like fine-tuning or
> a hosted flow.
>
> **Tracing and monitoring** — request-level observability with App Insights integration.
>
> The two I'd emphasise as most consequential in practice are Connections, because it's the
> security boundary, and Deployments, because the capacity choice drives both your cost
> curve and your latency profile."

**Depth — the four-point rule:**

1. **What it IS** — five layers: workspace (Hub, Project, Connections, Compute), model
   (Catalog, Deployments), build (Prompt Flow, Agent Service), evaluate (Evaluations,
   Tracing), govern (Content Safety, RBAC through the Hub).
2. **Why it works that way** — the Hub/Project split exists so security and connectivity are
   configured once at the boundary an enterprise actually governs, and consumed many times
   by teams that should not each be configuring their own credentials. Understanding that
   is the difference between listing components and understanding the design.
3. **Your example** — JM Family: Foundry SDK for the platform-facing concerns while
   application composition stayed in LangChain/LangGraph. See `Resume Q9` for the layer
   boundary and why it evolved that way.
4. **The trade-off** — the managed surface is convenient and opinionated. Agent Service
   handles the runtime for you and gives you less control over the loop than writing it
   yourself. Prompt Flow is excellent for iteration and weaker as a production source of
   truth than Git. Choosing managed components trades control for governance and speed,
   and knowing which ones you would *not* use is the mark of having actually built on it.

**Whiteboard:**

```
   ┌─ WORKSPACE ────────────────────────────────────────────────────┐
   │ HUB          shared infra · connections · compute · security   │
   │   └─ PROJECT working container · inherits Hub config           │
   │ CONNECTIONS  ⚠ credential-holding links → keys OUT of code     │
   │ COMPUTE      instances for fine-tuning / hosted flows          │
   └────────────────────────────────────────────────────────────────┘
   ┌─ MODEL ────────────────────────────────────────────────────────┐
   │ MODEL CATALOG   OpenAI · open-weight · Microsoft · third-party │
   │ DEPLOYMENTS  ⚠  endpoint + CAPACITY MODEL (PAYG vs provisioned)│
   │                 drives cost curve AND latency profile          │
   └────────────────────────────────────────────────────────────────┘
   ┌─ BUILD ────────────────────────────────────────────────────────┐
   │ AGENT SERVICE   managed runtime · tools · threads · state      │
   │ PROMPT FLOW     visual authoring + eval — iterate here,        │
   │                 keep the production artefact in Git            │
   └────────────────────────────────────────────────────────────────┘
   ┌─ EVALUATE / GOVERN ────────────────────────────────────────────┐
   │ EVALUATIONS     groundedness · relevance · coherence · safety  │
   │ TRACING         request-level, App Insights                    │
   │ CONTENT SAFETY  harm categories, per deployment                │
   └────────────────────────────────────────────────────────────────┘
```

**Follow-up probes:**
- *"Hub versus Project — why the split?"* → Governance boundary. Connections, security and
  compute configured once at Hub level; projects consume them. It is the answer to "how do
  I let five teams build without five sets of credentials."
- *"Provisioned throughput versus pay-as-you-go?"* → PTU gives reserved capacity, predictable
  latency, and a fixed cost regardless of usage. PAYG is variable cost with shared capacity
  and rate limits. PTU pays off at high sustained volume or where latency consistency is a
  requirement. This is a genuinely senior detail — see also **Q17**.
- *"Foundry Evaluations or RAGAS?"* → Both are defensible. Foundry's are integrated and
  governed; RAGAS in your own pipeline gives more control and portability across providers,
  which matters if you might not stay on Azure. `Resume Q23` covers what RAGAS computes.
- *"Which have you actually used?"* → Answer honestly and specifically. Claiming depth on
  all eleven invites a probe on the one you have not touched.
- *"How does this differ from Azure ML?"* → Azure ML is the broader ML platform — training,
  pipelines, model registry for classical ML. Foundry is the generative-AI and agent
  surface. They overlap on compute and registry concepts.

**Red flag:** listing components without saying what each is *for* or when you touch it.
The question says "explain each" and a glossary recital answers only half of it.

---
---

# Companion Deep-Dives — the four gap areas

> These four extend the gap questions above. They are the follow-ups a strong interviewer
> asks *after* you answer Q3, Q6, Q8 and Q10 well — and the repo had nothing on any of
> them.

---

# Q15. Companion to Q3 — one index or many? How do you handle multi-tenancy at scale?

**What they're testing:** Index topology. Once you have sized an index, the next real
decision is how many of them there are — and it is a decision with security, cost, and
operational consequences.

**60-second spoken answer:**

> "Three options, and the choice is driven by isolation requirements first and cost second.
>
> **One shared index with a tenant filter.** Cheapest, simplest to operate, one schema to
> maintain. Every query carries a mandatory tenant filter applied server-side from the
> authenticated identity. The risk is that isolation depends on a filter always being
> applied correctly — one code path that forgets it is a cross-tenant leak.
>
> **Index per tenant.** Hard isolation, easy to explain to an auditor, easy to delete a
> tenant entirely. Costs more, and you hit service limits on index count. It also means N
> schemas to migrate when the schema changes, which is genuinely painful at scale.
>
> **Hybrid** — shared index for the many small tenants, dedicated indexes for the few large
> or high-sensitivity ones. This is usually where real systems land.
>
> My rule: if a cross-tenant leak is an existential business event, pay for separate
> indexes for those tenants. If tenants are internal business units under one compliance
> regime, a shared index with enforced filtering is proportionate.
>
> The other axis is functional sharding — splitting by document type or by year rather than
> by tenant, which helps when queries naturally scope that way and hurts when they don't."

**Depth — the four-point rule:**

1. **What it IS** — shared-with-filter, index-per-tenant, or hybrid; plus functional sharding
   as an orthogonal option.
2. **Why it works that way** — isolation is enforced either by *code* (a filter that must
   always be applied) or by *structure* (separate indexes that cannot see each other).
   Structural isolation is more expensive and cannot be defeated by a bug, which is why it
   is the answer where the consequence of a leak is severe.
3. **Your example** — JM Family, security trimming by Entra ID group on a shared index — see
   `Resume Q13`. That is the shared-with-filter model applied to users rather than tenants.
4. **The trade-off** — index-per-tenant multiplies operational surface: schema migrations,
   index warm-up, per-index capacity, and service quota limits on index count. Shared
   multiplies risk concentration. Hybrid multiplies complexity but usually costs least
   overall.

**Whiteboard:**

```
   SHARED + FILTER            INDEX PER TENANT         HYBRID
   ───────────────            ────────────────         ──────
   cheapest                   hard isolation           small tenants → shared
   one schema                 easy to audit            large / sensitive → own
   ⚠ isolation = code         easy tenant deletion     ← where most systems land
     one missed filter        ⚠ N schemas to migrate
     = cross-tenant leak      ⚠ service limit on count

   DECISION RULE
   leak = existential?         → structural isolation, pay for it
   internal business units,
   one compliance regime?      → shared + enforced filter is proportionate

   ORTHOGONAL: functional sharding (by doc type, by year)
   helps when queries scope that way · hurts when they cross shards
```

**Follow-up probes:**
- *"How do you enforce the filter can't be missed?"* → Never let application code construct
  the query directly. A repository layer that takes the identity and constructs the filter
  means there is no path that omits it — the same structural-control principle as elsewhere.
- *"What about noisy neighbours?"* → A shared index means one tenant's heavy query load
  affects others. Replicas help; per-tenant rate limiting helps more.
- *"How do you delete a tenant?"* → Trivial with a dedicated index. With a shared index it is
  a filtered bulk delete plus verification — and verification is the part that matters for
  proving it happened.

**Red flag:** defaulting to one shared index without asking about isolation requirements.
It is usually right and it must be a decision, not an assumption.

---

# Q16. Companion to Q6 — when does compression hurt, and how do you know?

**What they're testing:** Whether you would deploy a lossy optimisation without measuring
it. This is the question that separates people who read about LLMLingua from people who
tried it.

**60-second spoken answer:**

> "Compression hurts in three specific ways, and they're not all obvious.
>
> **It drops load-bearing detail.** A compressor decides what's low-information based on
> general language statistics, not on your task. A policy number, a negation, a date
> qualifier — these look like noise and change the answer entirely. Negations are the
> classic case: dropping 'not' from a clause inverts its meaning and nothing downstream
> catches it.
>
> **It breaks citation.** Abstractive compression rewrites the source. If your system quotes
> and cites, the quoted text no longer exists in the source document, which is worse than
> unhelpful in a regulated setting — it looks like fabrication.
>
> **It costs what it saves.** Some compression techniques require a model call to compress.
> If you spend a small-model call to save prompt tokens on a small-model call, you may have
> gained nothing. The arithmetic has to be done, not assumed.
>
> How you know: A/B the compressed and uncompressed paths against the same evaluation set,
> and look at faithfulness and answer correctness — not at the compression ratio, which is
> the number the technique advertises and the number that doesn't matter.
>
> And I'd treat compression as an optimisation of last resort. Reranking and dedup get you
> most of the way with no quality risk. Reach for lossy compression only when those are
> exhausted."

**Depth — the four-point rule:**

1. **What it IS** — three failure modes: dropping load-bearing tokens, breaking citation
   provenance, and negative net cost. Detected by evaluating both paths on the same eval set.
2. **Why it works that way** — a compressor optimises an objective that is not your
   objective. It preserves general informativeness; your task may depend on a single token.
   Nothing in the compression step knows which.
3. **Your example** — JM Family: reranking from ten chunks to three delivered a large
   reduction with no quality loss, which is why the lossy techniques were never needed. See
   `Resume Q29`.
4. **The trade-off** — measuring compression properly costs a full eval run per configuration,
   which is real time and money. The alternative is deploying it blind, and the failure is
   silent — a system with over-compressed context does not error, it just answers slightly
   worse, forever.

**Whiteboard:**

```
   HOW COMPRESSION HURTS
   1. drops LOAD-BEARING tokens
      ⚠ negations — "not" removed inverts the clause, nothing catches it
      ⚠ identifiers, dates, qualifiers — look like noise, ARE the answer
   2. breaks CITATION
      abstractive rewrites source → quoted text no longer exists in the document
      → in a regulated setting this reads as fabrication
   3. costs what it saves
      model call to compress, to save tokens on a model call — do the arithmetic

   HOW YOU KNOW
   A/B compressed vs uncompressed on the SAME eval set
   measure: faithfulness + answer correctness
   ✗ NOT the compression ratio — that's the vendor's metric, not yours

   ORDER OF REACH
   rerank → dedup → (only then) extractive → abstractive / prompt compression
   ▲ no quality risk                        ▲ measure or don't ship
```

**Follow-up probes:**
- *"Would you ever compress in a regulated domain?"* → Extractive yes, since original wording
  survives and citation holds. Abstractive, very reluctantly, and never on the path that
  produces a cited answer.
- *"What's a safe compression ratio?"* → There is no safe ratio independent of content. Two
  corpora at the same ratio can have completely different quality outcomes. Measure per
  corpus.
- *"How would you catch a compression regression in production?"* → Faithfulness monitoring
  on live traffic, and the fact that this is hard to attribute is itself an argument for
  proving it offline first.

**Red flag:** citing a compression ratio as evidence that compression works. The ratio
measures how much you removed, not whether you removed the right things.

---

# Q17. Companion to Q8 — scaling the inference layer: quotas, PTU, and backpressure.

**What they're testing:** Whether you know that scaling AI workloads has a ceiling your
autoscaler cannot see. This is the question behind the question in Q8, and very few
candidates get here.

**60-second spoken answer:**

> "Scaling workers is the easy half. The half that bites you is that the model endpoint has
> a quota, and no amount of Kubernetes scaling changes it.
>
> Azure OpenAI capacity is expressed in tokens per minute and requests per minute. Exceed it
> and you get 429s. So if KEDA scales your workers on queue depth without a ceiling, you
> convert a backlog into a wall of rate-limit errors, and the retries make the congestion
> worse. The backlog doesn't clear faster — it clears slower.
>
> So three things. **Cap `maxReplicaCount` against the quota**, not against cluster capacity.
> **Implement backpressure** — retry with exponential backoff and jitter, and a circuit
> breaker so a sustained rate-limit condition stops the flood rather than amplifying it.
> And **decide your capacity model**: pay-as-you-go gives you variable cost on shared
> capacity with rate limits; provisioned throughput reserves capacity, gives you predictable
> latency, and costs the same whether you use it or not.
>
> PTU is right for high sustained volume or where latency consistency is a requirement.
> PAYG is right for bursty and unpredictable load. A common pattern is PTU sized to steady
> state with PAYG spillover for peaks.
>
> The general principle: in an AI system the bottleneck is almost never your compute. It's a
> quota somewhere, and your scaling design has to know where it is."

**Depth — the four-point rule:**

1. **What it IS** — quota as the true scaling ceiling, enforced by capping replicas, absorbed
   by backpressure, and shaped by the PTU-versus-PAYG capacity decision.
2. **Why it works that way** — autoscalers scale the thing they control. The constraint lives
   in a service they do not control, so it is invisible to them. Unless the ceiling is
   configured explicitly, the autoscaler will happily scale straight through it.
3. **Your example** — JM Family: `maxReplicaCount` set against the Document Intelligence and
   embedding quotas rather than cluster capacity. See `Resume Q41` and `Resume Q19` for the
   retry and circuit-breaker design.
4. **The trade-off** — PTU is reserved capacity you pay for whether or not you use it. Size
   it to peak and you waste money most of the day; size it to average and you rate-limit at
   peak. Hybrid — PTU for steady state, PAYG spillover — costs more per peak token and is
   usually the right answer.

**Whiteboard:**

```
   ⚠ THE CEILING YOUR AUTOSCALER CANNOT SEE

   KEDA scales workers on queue depth ──▶ more workers ──▶ more calls
                                                             │
                                            model endpoint QUOTA (TPM / RPM)
                                                             │
                                                        429 · 429 · 429
                                                             │
                                            retries ──▶ MORE congestion
                                            backlog clears SLOWER, not faster

   THREE CONTROLS
   1. cap maxReplicaCount   against the QUOTA, not cluster capacity
   2. backpressure          exponential backoff + JITTER + circuit breaker
   3. capacity model:
        PAYG  variable cost · shared capacity · rate limits    → bursty load
        PTU   reserved · predictable latency · fixed cost      → sustained volume
        hybrid PTU steady state + PAYG spillover               → usual answer

   PRINCIPLE: the bottleneck is a QUOTA, not your compute. know where it is.
```

**Follow-up probes:**
- *"How do you find the quota ceiling?"* → Load test to it deliberately in a non-production
  environment. Discovering it in production during a bulk load is the common alternative.
- *"What do you do when you hit it legitimately?"* → Request an increase — with lead time —
  and in the meantime shape the load: queue depth is your friend here, because async work
  can simply take longer without failing.
- *"Does this apply to embeddings too?"* → Yes, and embeddings are usually where you hit it
  first during ingestion, because ingestion generates far more calls than querying does.
- *"How do you monitor for it?"* → 429 rate as a first-class metric with an alert. A rising
  429 rate is the earliest signal that your scaling design and your quota have diverged.

**Red flag:** describing autoscaling with no mention of a downstream ceiling. It is the
failure that actually happens, and an answer without it describes a system that has not run
at load.

---

# Q18. Companion to Q10 — PII in the places nobody redacts: logs, caches, and eval sets.

**What they're testing:** Whether your PII discipline extends past the happy path. Almost
every system redacts the index and leaks through a side channel.

**60-second spoken answer:**

> "The pipeline gets redacted because that's where everyone looks. The leaks are in the
> copies nobody thinks of as copies.
>
> **Logs.** Prompts and completions are logged for debugging, and the prompt contains the
> retrieved context — so your logs contain the source data verbatim, often with wider access
> than the source system has. This is the most common real leak in production AI systems.
> Either scrub before writing, or apply source-equivalent access controls to the log store.
>
> **Caches.** A cached answer is a persisted copy of generated content derived from source
> documents. It also has an access-control dimension: if the cache key is just the question,
> you will serve one user's permitted answer to a user who isn't permitted to see it. The
> cache key has to include the access scope.
>
> **Evaluation sets.** Built from real documents and real questions, versioned in Git,
> readable by the whole engineering team. That's frequently the widest-access copy of
> sensitive content in the entire organisation and nobody classifies it that way.
>
> **Traces.** Same problem as logs, with the agent's full reasoning and tool arguments
> attached.
>
> **And the provider.** Depending on deployment and configuration, prompts may be retained
> for abuse monitoring. That is a data flow to a third party and it needs to be a documented
> decision, not a default.
>
> The discipline: enumerate every place content lands, and apply the same classification to
> each. If you can't list them, you don't know where your data is."

**Depth — the four-point rule:**

1. **What it IS** — five commonly-missed copies: logs, caches, evaluation sets, traces, and
   provider retention — each requiring source-equivalent classification and controls.
2. **Why it works that way** — an AI pipeline is a copy machine. Every stage that persists
   anything creates a new location with its own access model, and the access models
   diverge — engineering can read logs, everyone can read the repo, the cache has no ACL at
   all by default.
3. **Your example** — JM Family: this is the substance behind treating the responsible-AI
   controls as structural. See `Resume Q37` for pipeline placement and `Resume Q38` for the
   erasure implications.
4. **The trade-off** — scrubbing logs makes debugging materially harder. The prompt content
   is exactly what you need to diagnose a bad answer. The workable compromise is usually
   restricted-access full logs with a short retention window rather than scrubbed logs
   retained forever — but that is a decision to take explicitly.

**Whiteboard:**

```
   THE PIPELINE GETS REDACTED. THESE DON'T.

   LOGS         prompt + completion = source content verbatim
                ⚠ usually WIDER access than the source system
                → scrub before write, or source-equivalent ACLs

   CACHES       persisted generated content
                ⚠ key on question alone → serves A's answer to B
                → cache key MUST include access scope

   EVAL SETS    real documents + real questions, in Git, team-readable
                ⚠ often the WIDEST-access copy of sensitive content anywhere

   TRACES       logs + agent reasoning + tool arguments

   PROVIDER     retention for abuse monitoring, depending on config
                → a third-party data flow. document it as a decision.

   TEST: can you list every place content lands?
         if not, you don't know where your data is.
```

**Follow-up probes:**
- *"How do you scrub logs without losing debuggability?"* → Restricted access plus short
  retention is usually better than scrubbing. Alternatively log identifiers and hashes
  rather than content, and accept that some debugging requires elevated, audited access.
- *"What about the eval set specifically?"* → Either synthesise it, or classify and
  access-control the repo holding it at the same level as the source data. Most teams do
  neither and it is worth raising unprompted — it lands as genuine operational awareness.
- *"How would you audit this?"* → Enumerate the persistence points from the architecture
  and check the access model on each. The artefact is the enumeration; if nobody can produce
  it, the answer is that nobody knows.

**Red flag:** confident PII answers that stop at the index. The pipeline is the part
everyone secures; the side channels are where the incidents come from.

---
---

# Appendix — Drill order

These fourteen were asked. Drill them in this order, which is by a combination of
recurrence and how much damage a weak answer does.

| Priority | Q | Why first |
|---|---|---|
| 1 | **Q13** agent process | Centrepiece. 4–5 minutes, asked in nearly every agentic interview |
| 2 | **Q7** RAG lifecycle | Second centrepiece. Note it asks for the *operate* loop, not the request path |
| 3 | **Q14** Foundry components | Enumerable, easy to fail on detail, and pure preparation closes it |
| 4 | **Q1** memory | Four types. Most candidates give one |
| 5 | **Q3** 1M-doc search ⚠ | Gap. Requires arithmetic out loud |
| 6 | **Q9** chunking | Asked often, contains a trap ("which is best") |
| 7 | **Q11** token saving | Your strongest proof point sits behind it — the $150K story |
| 8 | **Q10** PII ⚠ | Gap. Disqualifying in regulated interviews if weak |
| 9 | **Q4** model choice | Framework, not a model name |
| 10 | **Q5** context window | Connects to Q6 and Q11 |
| 11 | **Q8** KEDA ⚠ | Gap. Weakest coverage of all fourteen |
| 12 | **Q6** compression ⚠ | Gap. List answer — most candidates name one of five |
| 13 | **Q2** DocIntel training | Straightforward if you have run the loop |
| 14 | **Q12** A2A | Short answer, one trap: do not call your orchestration A2A |

**Then the companions** — Q15–Q18 — which are the follow-ups to the four gap questions and
the places a strong interviewer will push after a good first answer.

---

**END OF FILE — Phase 2 complete. 14 asked questions + 4 companion deep-dives.**

Next: Phase 3 — fill the 44 `[FILL:]` placeholders in
`InterviewBank/07_Behavioral_Leadership.md`, and build `00_DRILL_INDEX.md` over the 1,161
existing unique questions.
