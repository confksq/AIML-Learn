# 01 — Concepts: RAGAS Evaluation

> **Bridge from what you already know:** at JM Family you already do groundedness checks and A/B testing on your RAG answers. RAGAS just turns those instincts into **four standardized, measurable metrics** — the difference between "it seems grounded" and "faithfulness = 0.87."

---

## 1. Why evaluate RAG at all?

A RAG pipeline has (at least) **two independent failure points** — retrieval and generation — and "the answer looked right" is not a metric. Without measurement you can't tell whether a change helped or hurt, you can't set a quality gate in CI/CD, and you can't show leadership evidence. RAGAS gives you numbers per stage so you can diagnose *which* stage is weak.

This is the same argument as Azure AI Foundry's evaluation flows (groundedness/relevance/coherence) — RAGAS is the popular open-source, framework-agnostic version that JDs name explicitly.

---

## 2. The 4 core metrics, in plain English

RAGAS scores each on **0 to 1** (higher is better). Each targets a specific part of the pipeline:

### Faithfulness — *"Did the answer stick to the retrieved context?"*
The hallucination check. It breaks the answer into individual claims and checks how many are actually supported by the retrieved chunks. **Low faithfulness = the model is making things up** beyond what retrieval gave it.
- This is exactly your JMA **groundedness** check, formalized.

### Answer Relevance — *"Does the answer actually address the question?"*
Measures whether the answer is on-topic and complete for the question asked (independent of whether it's factually grounded). **Low answer relevance = the model answered a different or partial question.**

### Context Recall — *"Did retrieval find the chunks needed to answer?"*
Of the information needed to produce the ground-truth answer, how much was actually present in the retrieved context. **Low context recall = a retrieval problem — the right chunks weren't retrieved.** (Requires ground-truth answers.)

### Context Precision — *"Are the retrieved chunks actually useful?"*
Of the chunks that were retrieved, how many are relevant (and are the relevant ones ranked at the top). **Low context precision = retrieval is returning noise** — you're stuffing the prompt with irrelevant chunks, wasting tokens and risking distraction.

---

## 3. Which metric points at which fix

This is the senior-level payoff — RAGAS tells you *where* to look:

| Low metric | Root cause | Fix |
|---|---|---|
| **Faithfulness** | Model hallucinating beyond context | Tighten the grounding prompt; lower temperature; add a "say I don't know" instruction |
| **Answer Relevance** | Model off-topic / incomplete | Improve the prompt template; clarify the question; check the model isn't rambling |
| **Context Recall** | Retrieval missed the right chunks | Better chunking, better embeddings, hybrid search, higher top-K |
| **Context Precision** | Retrieval returning noise | Re-ranking (cross-encoder), better query, lower top-K, metadata filtering |

Notice: **Recall + Precision are retrieval problems; Faithfulness + Relevance are generation problems.** RAGAS isolates the two halves of RAG the same way you'd diagnose "the chunk wasn't retrieved" vs "the chunk was retrieved but ignored" in Azure.

---

## 4. Score interpretation (rules of thumb)

| Score | Reading |
|---|---|
| **≥ 0.90** | Strong |
| **0.80 – 0.90** | Acceptable for many production uses |
| **0.70 – 0.80** | Needs attention — investigate the weakest metric |
| **< 0.70** | Not production-ready — fix before shipping |

These mirror the JMA thresholds you'd set in Azure AI Foundry (groundedness ≥ 4.0/5 etc.). Exact thresholds are use-case-specific — regulated content demands higher bars.

---

## 5. How RAGAS actually computes these — "LLM-as-judge"

RAGAS uses an LLM (the "judge" or "critic" model, e.g. GPT-4o) to score each metric — e.g., it asks the judge to extract claims from the answer and check each against the context (faithfulness). This is the same **LLM-as-judge** pattern you saw in the LLMOps curriculum:
- The judge model must be **pinned** — changing it shifts every score.
- The judge should be **calibrated** against human judgment on a sample before you trust it at scale.
- Judge calls cost money — evaluation is itself a metered pipeline.

---

## 6. RAGAS vs TruLens vs Azure AI Evaluation

| Tool | Ecosystem | Strengths |
|---|---|---|
| **RAGAS** | Framework-agnostic (LangChain, LlamaIndex, custom) | The 4 canonical RAG metrics; the one JDs name most; easy to drop into any pipeline |
| **TruLens** | Framework-agnostic, observability-focused | "Feedback functions," tracing, dashboards; good for continuous monitoring |
| **Azure AI Evaluation** (Foundry) | Azure-native | Groundedness/relevance/coherence/fluency; integrated with your Azure deployments and CI/CD |

**The senior answer:** "I use Azure AI Foundry's evaluators natively in my Azure stack, and RAGAS when I need a framework-agnostic, open-source evaluation harness — for example over a LlamaIndex or local pipeline, or when a JD specifically standardizes on RAGAS. They measure the same underlying qualities; RAGAS's four metrics map cleanly to Foundry's groundedness/relevance plus retrieval-side recall/precision."

---

## 7. Ground truth — what you need to provide

- **Faithfulness, Answer Relevance, Context Precision** can be computed from the question + answer + retrieved contexts alone (no reference answer strictly required for all versions).
- **Context Recall** needs a **ground-truth answer** to check what information *should* have been retrieved.

So build a small **golden dataset** (question + ideal answer) — 10+ pairs to start — exactly the golden-dataset practice from the LLMOps module. `sample_questions.json` in this module is that dataset.

---
*Next: `02_architecture.md` — the evaluation flow.*
