# Audit — `Interview_Bible_77Q_FDE_AI_Lead.md`

**Date:** 2026-08-10
**Scope:** all 77 answers, read in full
**Method:** technical claims checked against platform behaviour; numbers cross-checked
against the resume, `Interview_QA_Resume_Based.md`, and against each other
**Changes made to the Bible:** **none.** This is a findings report only.

---

## Verdict

The Bible is **well-structured, well-written, and the most fluent rehearsal material in the
repo.** The code samples are largely correct, the architectural narratives are coherent, and
several answers — the OIDC federation setup, the LangGraph state machine, the strangler-fig
migration — are genuinely strong.

It also contains **six factual errors that a platform-competent interviewer will catch, four
internal contradictions, and roughly twenty unfalsifiable precise numbers.** The density of
confident specifics is the core risk: the file answers every question as though every figure
were measured, and a single "how did you measure that?" on a fabricated number contaminates
every other number you quote — including the true ones.

**Recommendation: usable for structure and phrasing, not safe to quote verbatim until
Tier 1 and Tier 2 below are resolved.**

---

## TIER 1 — Factually wrong. Fix before speaking these.

These are not judgment calls. They will be caught.

### 1.1 — Azure AI Search tier limits are invented ⚠️ highest risk

**Q66 `:1261-1264`** gives a decision matrix:

> *"S1: Up to 15M documents, 3 partitions… S2: Up to 50M documents, 12 partitions…
> S3: Up to 150M documents, 36 partitions… S4: Up to 500M documents, 60 partitions."*

Problems:
- **There is no S4 tier.** Azure AI Search Standard tiers are S1, S2, S3, plus S3 HD, with
  Storage Optimized L1/L2 alongside. Naming a tier that does not exist ends the credibility
  of the answer instantly.
- **Partition counts are wrong.** Standard tiers cap at **12 partitions**, not 3 / 12 / 36 / 60.
- **Document limits don't work that way.** Capacity is storage-driven per partition, not a
  fixed document ceiling per tier.

**And it contradicts itself.** `Q50 :1083` says *"Standard S2 tier (100M documents max, 200
partitions)"* — a different wrong number for the same tier, sixteen questions earlier.

> **Fix:** replace the matrix with the sizing *method* — chunks × dimensions × bytes, then
> partitions for storage and replicas for QPS. `RealWorld` Q3 has this. A method you can
> reason from beats a table you can misremember.

### 1.2 — Azure AI Search has no partition key or routing key

**Q68 `:1283-1288`**:

> *"Azure AI Search automatically shards across partitions, but you need to design the
> partition key manually… we used doc_id as the routing key so that documents belonging to
> the same doc_id always landed on the same partition."*

Partition keys and routing keys are **Cosmos DB** concepts. Azure AI Search partitions are
transparent scale units — you cannot control document placement, and there is no partition
key to design.

The *conclusion* of the answer is sound (separate indexes per document type give logical
isolation and shrink the search space). The mechanism attributed to it is not.

> **Fix:** keep "one index per document type" and "filter first to shrink the candidate
> set." Drop the partition-key mechanism entirely.

### 1.3 — Embedding storage maths understates by ~10×

**Q66 `:1265`**: *"1M docs × 10KB/embeddings = ~10GB storage."*

A 1536-dimension float32 vector is ~6KB **per chunk**, and a document produces many chunks.
At ~20 chunks per document that is 20M vectors ≈ **120GB**, before text, metadata and index
overhead. The Bible's figure is off by an order of magnitude, and this is the exact
arithmetic the 1M-document question exists to test.

> **Fix:** `RealWorld` Q3 whiteboard has the correct calculation.

### 1.4 — MCP is described as a data-governance standard

**Q17 `:483`**: *"MCP (Model Context Protocol) is the emerging standard for enforcing data
boundaries and context governance."* The answer then describes building "MCP middleware"
that tags chunks with classification and clearance levels and filters them against Entra ID.

That is **security trimming**, and it is good engineering — but it is not MCP. MCP is an open
protocol for connecting models to tools and data sources. Anyone who has read the spec will
know within one sentence.

This is the same error as the resume line flagged in `Resume_Based` Q35 — but where the
resume merely implies it, the Bible builds a whole answer on it.

> **Fix:** `Resume_Based` Q35 has the correct definition and the honest governance
> connection. Describe the tagging work as security trimming, which is what it is, and is
> impressive on its own.

### 1.5 — A2A framed as an alternative to a monolithic agent

**Q58 `:1152`**: *"Why A2A (Agent-to-Agent) instead of a single monolith agent?"*

This conflates A2A with multi-agent decomposition. Splitting one system into several agents
is **orchestration** — you own both sides. A2A is for agents across organisational boundaries
where neither party controls the other's implementation. Answering the Bible's framing walks
into precisely the trap `RealWorld` Q12 was written to prevent.

> **Fix:** use `RealWorld` Q12.

### 1.6 — "Confidence interval (0-100)" on LLM output

**Q20 `:568`** describes surfacing *"a confidence interval (0-100)"* per response.

Two problems: a confidence interval is a statistical range, not a 0–100 score; and LLMs do
not emit calibrated confidence. Your own `L24_Hallucination_Mitigation.md` makes exactly this
point — a model at 40% and one at 95% produce equally confident prose.

> **Fix:** say "a groundedness score from our evaluation layer," which is real and defensible.

### Also worth correcting

| Ref | Claim | Problem |
|---|---|---|
| `Q50 :1084` | *"Pure vector search fails at 1M because of the curse of dimensionality"* | Not the reason. Hybrid wins because exact identifiers carry almost no semantic signal to embed — that's true at 1K documents too |
| `Q10 :304` | *"prefix length exactly 2,000 tokens so the cache key aligned perfectly"* | Prompt caching matches on prefix with a minimum-token threshold; there's no alignment requirement to engineer |
| `Q29 :824` | Titan Text listed as *"Embedding generation"* | Titan Text and Titan Embeddings are different models |
| `Q34 :882` | *"RTO < 2 min, RPO = 4 hours. Met SOC 2 compliance"* | SOC 2 attests to controls; it doesn't specify RTO/RPO thresholds |
| `Q1 :46`, `Q4 :124` | `text-embedding-ada-002` as the primary embedding model | Legacy. Quoting it dates the work; the `text-embedding-3` family superseded it |

---

## TIER 2 — Unfalsifiable precise numbers

Every one of these invites *"how did you measure that?"* You have one good answer to that
question — the recall@k methodology in `Resume_Based` Q8. You do not have twenty.

| Ref | Claim |
|---|---|
| `Q9 :267` | *"saved JM Family **exactly $152,300** annually"* |
| `Q1/Q3` | retrieval accuracy *"78% → 95%"* |
| `Q4 :134` | fine-tuned embeddings gave *"6% relative lift in recall@10"* |
| `Q6 :191` | *"92% task completion rate"* |
| `Q8 :261` | agent failure rates *"15% → under 3% within a month"* |
| `Q8 :253` | token consumption *"8,000 → 3,000 per query"* |
| `Q9 :275` | cache hit rate *"65%"* |
| `Q10 :311-312` | prompt cache hit *"~70%"*, latency *"1.2s → 200ms"* |
| `Q11 :337` | *"95% accuracy across 50+ deployments"* |
| `Q12 :361` | *"80% of the quality of the cloud LLM"* |
| `Q13 :386` | KEDA *"slashed infrastructure costs by 40% ($300K)"* |
| `Q28 :815` | *"15% cost benefit by arbitraging"* |
| `Q29 :827` | *"saved ~12% over six months"* |
| `Q32 :855-856` | Azure *"~500ms P95"* vs Bedrock *"~1.2s P95"* |
| `Q33 :871` | *"saving ~$8K/month"*, GPT-4o *"5% more reliable"* on JSON |
| `Q49 :1075-1076` | *"92% F1… up from 70%… improved to 96% over six months"* |
| `Q67 :1279` | *"m=32, efConstruction=200 gave 95% recall, ~4 hours for 1M vectors"* |
| `Q68 :1293` | latency *"800ms → 300ms"* |
| `Q70 :1325` | *"3x better latency (200ms vs 600ms)"* |
| `Q72 :1354` | *"5 minutes → 45 seconds"* |

**`$152,300` is the most dangerous single figure in the file.** Nobody knows annual inference
savings to the nearest hundred dollars. False precision reads as invention to anyone who has
built a cost model — and your resume says "~$150K+", which is the defensible form.

### Three claims that cannot be true as stated

| Ref | Claim | Why |
|---|---|---|
| `Q12 :361` | *"zero hallucination in air-gapped environments"* | Not achievable. Directly contradicts `Resume_Based` Q12, where you correct exactly this phrasing on your own resume |
| `Q18 :532` | *"Zero successful prompt injection attacks over 12 months"* | You can only claim zero **detected**. Claiming zero successful attacks asserts knowledge of undetected ones |
| `Q19 :551` | *"The vector index contained zero PII"* | PII detection has false negatives. `RealWorld` Q10 says assume they exist and control access accordingly |

---

## TIER 3 — Internal contradictions

The Bible disagrees with itself. Any of these surfacing in one interview is a problem.

**3.1 — The same 78%→95% has two different owners.**
`Q1 :50` — *"This cross-encoder step was the secret sauce — it boosted our retrieval accuracy
from 78% to 95%."*
`Q3 :116` — *"[structure-aware chunking] Our retrieval accuracy jumped from 78% to 95%."*
Both cannot own the same 17-point gain.

**3.2 — The same $300K has two different owners, three questions apart.**
`Q13 :386` — KEDA autoscaling *"slashed our infrastructure costs by 40% ($300K annually)"*.
`Q14 :410` — the monolith migration delivered *"40% infrastructure cost reduction ($300K
annually)"*.
Your resume attributes it to the migration. Pick one.

**3.3 — The routing story uses two different cheap models.**
`Q9 :278-280` — simple queries → **GPT-3.5-Turbo**, *"70% of queries"*.
`Q71 :1331-1335` — simple queries → **GPT-4o-mini**; GPT-3.5-Turbo is a *"legacy fallback"* —
and is priced *higher* ($0.50 vs $0.30) than the model it supposedly falls back from.

**3.4 — Two different production incidents.**
`Q22 :612` — the incident is an **AI Search index corruption** from a schema update, resolved
in a 5-minute minute-by-minute drill.
`InterviewBank/07` Q5 — the incident is an **Azure OpenAI TPM quota exhaustion**.
Both are told as *the* production incident you owned. Choose one and retire the other; being
asked "you mentioned a different outage earlier" is unrecoverable.

**3.5 — Cost percentages that sum to 100% of the total.**
`Q9 :269-284` — caching *"40% reduction – ~$60K"*, routing *"30% – ~$45K"*, truncation
*"30% – ~$45K"*. The percentages sum to 100 and the dollars to $150K, but the headline is
*"30% of our original inference budget"*. So the percentages are shares **of the saving**,
not of spend — while being written as though they were reductions in spend. An interviewer
doing the arithmetic will ask, and the labels don't survive it.

---

## TIER 4 — Conflicts with your other materials

| Bible | Conflicts with | Note |
|---|---|---|
| *"exactly $152,300"* | Resume: *"~$150K+"* | Resume phrasing is the defensible one |
| *"78% → 95%"* baseline | Resume: *"95% retrieval accuracy"*, no baseline | The baseline is a new claim requiring its own methodology |
| *"zero hallucination"* `Q12` | `Resume_Based` Q12 | Q12 exists to talk you *out* of this phrasing |
| MCP as governance standard `Q17` | `Resume_Based` Q35 | Q35 corrects it |
| Index corruption incident `Q22` | `InterviewBank/07` Q5 | Two competing incidents |
| GPT-3.5 as primary cheap tier `Q9` | `Resume_Based` Q31 | Q31 describes tiering without naming dated models |

---

## What is genuinely strong — keep this

Not everything here needs fixing. These are good and I would rehearse from them directly:

- **`Q16` / `Q31` — Azure Managed Identity → OIDC federation → AWS IAM.** Correct, specific,
  and exactly the detail that proves multi-cloud experience. The trust-policy JSON is right.
- **`Q26` — the LangGraph state machine.** Correct API usage, sensible conditional edge, and
  the iteration guard is real engineering.
- **`Q14` — strangler-fig migration** with weighted routing and a CDC rollback window.
  Coherent, and it matches how these actually run.
- **`Q13` — the KEDA `ScaledObject`.** Correct trigger config and the right reasoning about
  HPA's floor of 1. *(Only the $300K attribution is contested — see 3.2.)*
- **`Q8` — the three agent failure modes**, especially state hashing to detect cycles. That
  is a detail people only know from having been burned.
- **`Q24` — the two-week bank MVP.** The best FDE answer in the file; the "ugly-but-functional
  first" instinct is exactly the role.
- **`Q20` — pivoting from accuracy to verifiability.** Strong framing, and it matches
  `Resume_Based` Q49. *(Only the "confidence interval" wording needs fixing — 1.6.)*

---

## Suggested order of work

1. **Resolve Tier 3 first (30 min).** The contradictions are free to fix — you're choosing
   between two stories you already have, not creating anything.
2. **Fix the six Tier 1 errors (30 min).** Each has a correct replacement already written in
   `RealWorld` or `Resume_Based`; cross-references are in the tables above.
3. **Triage Tier 2 (the long one).** For each number: can you say how it was measured? If
   yes, keep it and prepare the methodology. If no, round it, qualify it, or drop it. Round
   and defensible beats precise and invented — every time.
4. **Then rehearse.** The Bible is the best delivery material you have once the claims behind
   it are ones you can stand on.

> The underlying issue is one habit, not seventy-seven mistakes: **the file answers every
> question as though every number were measured.** Your resume file takes the opposite
> approach — 22 explicit `[CONFIRM:]` markers where the honest answer is "I'd have to check."
> That posture is not weakness. It is the thing that makes the numbers you *do* quote
> believable.

---

**Related:** `Interview_QA_RealWorld_Asked.md` (declared split + question mapping) ·
`Interview_QA_Resume_Based.md` Appendix A (the `[CONFIRM:]` checklist) ·
`00_DRILL_INDEX.md` (what to open, when)
