# 03 — Interview Q&A: RAGAS Evaluation (15 questions, senior level)

---

**Q1. Why do you need to evaluate a RAG pipeline at all?**
Because "the answer looked right" isn't a metric, and RAG has at least two independent failure points — retrieval and generation. Without measurement you can't tell if a change helped or hurt, can't gate deploys on quality, and can't give leadership evidence. RAGAS gives per-stage numbers so you can diagnose which stage is weak and prove quality with a score.

**Q2. What are the four core RAGAS metrics?**
Faithfulness (does the answer stick to the retrieved context — hallucination check), Answer Relevance (does the answer address the question), Context Recall (did retrieval find the chunks needed to answer), and Context Precision (are the retrieved chunks actually useful/well-ranked). All scored 0–1, higher is better.

**Q3. Which metrics are retrieval problems vs generation problems?**
Context Recall and Context Precision are retrieval-side — they measure whether the right chunks were found and whether noise was avoided. Faithfulness and Answer Relevance are generation-side — whether the model stayed grounded and answered the question. This split is exactly "the chunk wasn't retrieved" vs "the chunk was retrieved but ignored."

**Q4. Faithfulness scores 0.6 — what's happening and how do you fix it?**
The model is hallucinating — generating claims not supported by the retrieved context. Fixes: tighten the grounding instruction ("answer only from the context"), lower temperature, add an explicit "say I don't know if it's not in the context" clause, and force citations so ungrounded claims are visible. Faithfulness is the formalized version of a groundedness check.

**Q5. Context Recall is low but Faithfulness is high — interpret that.**
The model is faithfully answering from whatever it retrieved, but retrieval isn't finding all the information needed for a complete/correct answer. It's a retrieval problem, not a generation problem — fix chunking, embeddings, use hybrid search, or raise top-K. The model is being honest about incomplete context.

**Q6. Context Precision is low — what does that cost you and how do you fix it?**
Retrieval is returning irrelevant chunks (or ranking relevant ones low). That wastes prompt tokens, raises cost, and can distract the model into worse answers. Fix with a cross-encoder re-ranker, a better query (query rewriting), lower top-K, or metadata filtering — get fewer, higher-quality chunks.

**Q7. Which metrics require ground-truth answers?**
Context Recall specifically needs a ground-truth answer to determine what information *should* have been retrieved. Faithfulness, Answer Relevance, and Context Precision can be computed from the question, answer, and retrieved contexts. That's why you build a small golden dataset of question + ideal-answer pairs.

**Q8. How does RAGAS actually compute these scores?**
LLM-as-judge: it uses a critic model (e.g., GPT-4o) to, for example, extract atomic claims from the answer and check each against the retrieved context for faithfulness. The judge model must be pinned (changing it shifts all scores), calibrated against human judgment before trusting at scale, and its calls are metered — evaluation is itself a cost.

**Q9. What's a good RAGAS score?**
Rules of thumb: ≥0.90 strong, 0.80–0.90 acceptable for many production uses, 0.70–0.80 needs attention, below 0.70 not production-ready. Exact thresholds are use-case-specific — regulated or high-consequence content demands higher bars. I set the threshold per use case, not a universal number.

**Q10. RAGAS vs TruLens vs Azure AI Evaluation — when do you use each?**
Azure AI Foundry evaluators natively in my Azure stack (integrated with deployments and CI/CD). RAGAS when I need a framework-agnostic open-source harness — over a LlamaIndex, local, or custom pipeline, or when a JD standardizes on it. TruLens when I want observability/feedback-function-style continuous monitoring with dashboards. They measure the same underlying qualities.

**Q11. How do you use RAGAS in CI/CD?**
As a quality gate: on any change to chunking, embeddings, prompt, or model, re-run the golden dataset through RAGAS and fail the build if faithfulness (or any metric) drops below threshold. This is the AI CI/CD quality-gate pattern — non-deterministic LLM output means you gate on measured quality scores, not just unit tests.

**Q12. What's the risk of trusting LLM-as-judge scores blindly?**
Judges have known biases — self-preference (favoring outputs like their own), verbosity bias (longer = higher), position bias in pairwise comparisons, and leniency drift. Mitigate by calibrating the judge against human labels on a sample, pinning the judge version, and periodically re-checking agreement. The score is evidence, not gospel.

**Q13. How is RAGAS related to what you already do at JM Family?**
I already do groundedness checks and A/B testing on RAG answers at JMA. RAGAS formalizes that into standardized, comparable metrics — faithfulness is essentially my groundedness check with a number attached, and it adds retrieval-side recall/precision I was reasoning about informally. It turns "seems grounded" into "faithfulness = 0.87," which is what a quality gate and a stakeholder need.

**Q14. Your end-to-end answer quality dropped after a change. How do you localize the cause with RAGAS?**
Re-run the golden dataset and look at which metric moved. If Context Recall/Precision dropped, the regression is in retrieval (chunking/embeddings/index). If Faithfulness/Answer Relevance dropped while retrieval metrics held, it's generation (prompt/model). RAGAS isolates the failing half so you don't debug blind.

**Q15. How big should the golden dataset be, and who writes it?**
Start with 10+ representative question + ideal-answer pairs; grow toward ~100 for a stable production gate, stratified across easy, inference-required, out-of-scope, and adversarial cases. Domain experts write the ideal answers so ground truth is trustworthy. Refresh it as the corpus and query patterns drift, and feed production failures back into it.

---
*Frame answers as "this formalizes the groundedness/A-B-testing I already do at JMA" — it shows the concept isn't new to you, only the tool.*
