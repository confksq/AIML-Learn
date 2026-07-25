# 03 — RAGAS Evaluation

**Part of:** Career Accelerator portfolio · **PRD Feature L3** · **Phase 1 (Week 1)**
**Skill:** Measuring RAG quality with RAGAS — faithfulness, answer relevance, context recall, context precision.

---

## Why this module matters for the job search

~30% of Senior AI Engineer JDs ask for **RAG evaluation** experience, and Azure AI Foundry JDs explicitly name **RAGAS** and **TruLens**. You already do groundedness checks and A/B testing at JM Family — RAGAS formalizes that instinct into four measurable, industry-standard metrics you can put a number on. "Our RAG scored 0.87 faithfulness" is exactly the evidence hiring managers want to hear.

---

## What you'll have after this module
- A working RAG pipeline instrumented with **RAGAS** (`04_hands_on.py`)
- A score table across all **4 core metrics** with plain-English interpretation
- `sample_questions.json` — 10 realistic Q&A + ground-truth pairs to evaluate against
- 15 senior-level interview Q&A on why/how to evaluate RAG

---

## Prerequisites
```bash
pip install -r requirements.txt
```
Backend: RAGAS uses an LLM as a judge. Set `OPENAI_API_KEY` (or Azure OpenAI vars), or point it at a local Ollama model (see `04_hands_on.py` config).

---

## Quick start
```bash
pip install -r requirements.txt
export OPENAI_API_KEY=...          # or configure Azure / Ollama in the script
python 04_hands_on.py
```
The script builds a small RAG pipeline over a sample document, runs the 10 questions from `sample_questions.json`, collects answers + retrieved contexts, then runs RAGAS and prints a scored table with interpretation of the weakest metric.

---

## Files
| File | What it is |
|---|---|
| `01_concepts.md` | The 4 metrics in plain English, bridged from your JMA groundedness work |
| `02_architecture.md` | Evaluation flow diagram |
| `03_interview_qa.md` | 15 senior-level interview Q&A |
| `04_hands_on.py` | RAG pipeline + RAGAS evaluation + score table |
| `sample_questions.json` | 10 Q&A + ground-truth pairs |
| `05_resume_bullet.md` | Ready-to-paste resume bullet |
| `requirements.txt` | Python dependencies |

---
*Bridge: your JMA groundedness checks & A/B testing → RAGAS formalizes them into faithfulness / answer relevance / context recall / context precision.*
