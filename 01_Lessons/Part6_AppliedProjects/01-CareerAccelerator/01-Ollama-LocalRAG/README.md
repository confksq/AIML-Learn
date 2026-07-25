# 01 — Ollama + Local LLMs (Local RAG)

**Part of:** Career Accelerator portfolio · **PRD Feature L1** · **Phase 1 (Week 1)**
**Skill:** Running open-source LLMs locally with Ollama, and building a fully local RAG pipeline (no paid API).

---

## Why this module matters for the job search

~15% of Senior AI Engineer JDs ask for **local / open-source LLM** experience — driven by:
- **Regulated / air-gapped environments** (healthcare, finance, defense) where data cannot leave the building
- **Cost control** at high volume (no per-token bill)
- **Data privacy** — prompts and documents never leave your machine

You already run this pattern in the cloud at JM Family (Azure OpenAI + Azure AI Search RAG). This module proves you can deliver the **same architecture on open-source infrastructure** — the exact signal a KFORCE-type "local AI" role is looking for.

---

## What you'll have after this module
- A running **Ollama** server hosting LLaMA 3 / Mistral locally
- A single-file **end-to-end RAG pipeline** (`04_hands_on.py`): ingest → chunk → embed → FAISS → local LLM → cited answer
- 15 senior-level interview Q&A on local vs cloud, model serving, quantization, and FAISS

---

## Prerequisites

**Install Ollama:**
```bash
# Linux / WSL / Mac
curl -fsSL https://ollama.com/install.sh | sh

# Windows: download the installer from https://ollama.com/download
```
> Note: on a corporate network behind a proxy (e.g. Zscaler), the install URL may be blocked. Install from a non-proxied network, or use the offline installer.

**Pull a model:**
```bash
ollama pull llama3        # ~4.7 GB, general purpose
ollama pull mistral       # ~4.1 GB, fast, strong reasoning
```

**Python deps:**
```bash
pip install -r requirements.txt
```

---

## Quick start (3 commands)
```bash
ollama serve &                       # start the local model server on :11434
ollama pull llama3                   # download the model (one time)
python 04_hands_on.py                # run the full local RAG pipeline
```

Expected output: the script ingests the sample text, retrieves the relevant chunks for a question, sends them to LLaMA 3 running locally, and prints a grounded answer **with source references** — all with zero cloud calls.

---

## Files
| File | What it is |
|---|---|
| `01_concepts.md` | Theory, bridged from your Azure OpenAI / AI Search knowledge |
| `02_architecture.md` | ASCII architecture + component breakdown |
| `03_interview_qa.md` | 15 senior-level interview Q&A |
| `04_hands_on.py` | Single-file, end-to-end local RAG (heavily commented) |
| `05_resume_bullet.md` | Ready-to-paste resume bullet |
| `requirements.txt` | Python dependencies |

---
*Bridge: Azure OpenAI endpoint → Ollama server · Azure AI Search → FAISS · text-embedding-3 → sentence-transformers*
