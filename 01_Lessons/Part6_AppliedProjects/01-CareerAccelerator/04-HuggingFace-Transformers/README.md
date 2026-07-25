# 04 — Hugging Face Transformers

**Part of:** Career Accelerator portfolio · **PRD Feature L4** · **Phase 2 (Week 2)**
**Skill:** The Hugging Face ecosystem — Hub, `transformers`, `pipeline()`, embeddings, and open-source RAG.

---

## Why this module matters for the job search

**~35% of Senior AI Engineer JDs mention Hugging Face — the single highest-frequency gap skill** in the analysis. It's the lingua franca of open-source AI: the model registry, the library, and the toolkit that data scientists and ML engineers assume you know. You already understand what these models *do* (you use Azure OpenAI daily) — this module proves you can work them in the open-source stack recruiters screen hardest for.

---

## What you'll have after this module
- Four runnable demo scripts covering the four things HF is used for: **text generation, embeddings, classification, and a full local RAG**
- All runnable **without a paid API** (models download from the Hub and run locally)
- 15 senior-level interview Q&A on tokenizers, the `pipeline()` API, the Model Hub, and HF vs Azure OpenAI

---

## Prerequisites
```bash
pip install -r requirements.txt
```
First run downloads models from the Hugging Face Hub (a few hundred MB). No API key needed for the local models used here.
> On a corporate proxy (Zscaler), Hub downloads may be blocked — run from a non-proxied network or set `HF_HOME` to a pre-downloaded cache.

---

## Quick start
```bash
pip install -r requirements.txt
python 04a_text_generation.py     # generate text with a small local model
python 04b_embeddings.py          # embeddings + cosine similarity semantic search
python 04c_classification.py      # zero-shot classification with BART
python 04d_rag_with_hf.py         # full local RAG: HF embeddings + FAISS + local LLM
```

---

## Files
| File | What it demonstrates |
|---|---|
| `01_concepts.md` | HF ecosystem, bridged from Azure OpenAI / AI Foundry |
| `02_architecture.md` | How the pieces (Hub, Transformers, pipelines) fit together |
| `03_interview_qa.md` | 15 senior-level interview Q&A |
| `04a_text_generation.py` | `pipeline("text-generation")` — temperature/top_p |
| `04b_embeddings.py` | sentence-transformers, cosine similarity, semantic search |
| `04c_classification.py` | zero-shot classification with `facebook/bart-large-mnli` |
| `04d_rag_with_hf.py` | End-to-end RAG using only HF + FAISS (no paid API) |
| `05_resume_bullet.md` | Ready-to-paste resume bullet |
| `requirements.txt` | Python dependencies |

---
*Bridge: Azure OpenAI Service → Hugging Face Hub · text-embedding-3 → sentence-transformers · Azure AI Foundry model catalog → HF Model Hub · Content Understanding → HF task pipelines*
