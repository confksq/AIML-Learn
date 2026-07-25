# 02 — Architecture: RAGAS Evaluation Flow

## The evaluation flow

```
  ┌──────────────────────────┐
  │ Golden dataset           │   sample_questions.json
  │ question + ground_truth  │   (10 Q&A pairs)
  └────────────┬─────────────┘
               │ for each question
               ▼
  ┌──────────────────────────────────────────────────────────────┐
  │ RAG PIPELINE (the system under test)                          │
  │   embed question ─▶ FAISS/vector search ─▶ top-K contexts     │
  │                          │                                    │
  │                          ▼                                    │
  │   build prompt (contexts + question) ─▶ LLM ─▶ answer         │
  └────────────┬───────────────────────────────────┬─────────────┘
               │ answer                             │ retrieved contexts
               ▼                                    ▼
  ┌──────────────────────────────────────────────────────────────┐
  │ EVALUATION RECORD (per question)                              │
  │   { question, answer, contexts[], ground_truth }              │
  └────────────┬─────────────────────────────────────────────────┘
               │  collect all records into a dataset
               ▼
  ┌──────────────────────────────────────────────────────────────┐
  │ RAGAS EVALUATOR  (LLM-as-judge, e.g. GPT-4o)                  │
  │   ┌───────────────┐ ┌────────────────┐ ┌──────────────┐       │
  │   │ Faithfulness  │ │ Answer         │ │ Context      │       │
  │   │ (grounding)   │ │ Relevance      │ │ Recall       │       │
  │   └───────────────┘ └────────────────┘ └──────────────┘       │
  │                     ┌────────────────┐                        │
  │                     │ Context        │   all scored 0–1       │
  │                     │ Precision      │                        │
  │                     └────────────────┘                        │
  └────────────┬─────────────────────────────────────────────────┘
               ▼
  ┌──────────────────────────────────────────────────────────────┐
  │ SCORE TABLE + interpretation                                  │
  │   Faithfulness 0.87 | Answer Rel 0.82 | Recall 0.74 | Prec 0.91│
  │   → weakest = Context Recall → retrieval problem → fix chunking│
  └──────────────────────────────────────────────────────────────┘
```

## Component breakdown

| Component | Role | Azure equivalent |
|---|---|---|
| **Golden dataset** | Questions + ground-truth answers to evaluate against. | Foundry evaluation dataset / golden set |
| **RAG pipeline** | The system under test — produces answers + retrieved contexts. | Your JMA RAG pipeline |
| **Evaluation record** | Per-question bundle: question, answer, contexts, ground_truth. | The input each Foundry evaluator scores |
| **RAGAS evaluator** | LLM-as-judge scoring the 4 metrics. | Azure AI Foundry evaluators (GPT-4o judge) |
| **Score table** | Aggregated metrics + which is weakest. | Foundry evaluation results view |

## Data flow notes

- **You must capture the retrieved contexts, not just the final answer.** RAGAS needs the actual chunks the pipeline retrieved to compute faithfulness/precision/recall. Instrument your pipeline to return `contexts` alongside `answer` — the same "log the retrieved chunks per query" discipline from the LLMOps module.
- **Ground truth is only strictly needed for Context Recall.** The other three can be computed without a reference answer, but providing ground truth improves and enables the full metric set.
- **The judge model is metered and must be pinned.** Every metric is an LLM call; changing the judge model invalidates trend comparisons.

## Where this plugs into CI/CD

Run RAGAS as a **quality gate** in your pipeline: on any change to chunking, embeddings, prompt, or model, re-run the golden dataset and fail the build if faithfulness (or any metric) drops below threshold — exactly the AI CI/CD quality-gate pattern from the LLMOps curriculum, using RAGAS as the evaluator instead of Foundry.

---
*Next: `03_interview_qa.md`*
