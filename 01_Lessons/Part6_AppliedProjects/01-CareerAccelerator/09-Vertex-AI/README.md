# 09 — GCP Vertex AI + Agent Development Kit

**Part of:** Career Accelerator portfolio · **PRD Feature L9** · **Phase (extension)**
**Skill:** Google Cloud Vertex AI — Gemini models, Vertex AI Search/embeddings, and the Agent Development Kit (ADK). Completes the **Azure + AWS + GCP** multi-cloud story.

---

## Why this module matters for the job search

A growing share of Senior AI / GenAI JDs ask for **multi-cloud** breadth, and "Vertex AI + Gemini" shows up alongside Azure and Bedrock. You already run the Azure equivalent of everything Vertex does, and module 06 mapped Azure ↔ AWS Bedrock. This module adds the **third cloud** — so you can say "I deliver the same GenAI architecture on Azure, AWS, *and* GCP" — a claim almost no Azure-only candidate can make.

---

## What you'll have after this module
- A `04_hands_on.py` that calls **Gemini on Vertex AI**, generates embeddings, and sketches a minimal **ADK agent** — all mapped to their Azure equivalents
- 15 senior-level interview Q&A on Vertex AI, Gemini, and multi-cloud AI
- A clean Azure ↔ GCP mental map you can recite

---

## Prerequisites
```bash
pip install -r requirements.txt
```
GCP access (free-tier / trial credits work for small tests):
- A **GCP project** with the **Vertex AI API enabled**
- Authentication via **Application Default Credentials**: `gcloud auth application-default login`
- Env: `GOOGLE_CLOUD_PROJECT=your-project`, `GOOGLE_CLOUD_LOCATION=us-central1`, `GOOGLE_GENAI_USE_VERTEXAI=True`
> No GCP account? The script is fully commented so the API shape and concepts stand alone.

---

## Quick start
```bash
pip install -r requirements.txt
gcloud auth application-default login
export GOOGLE_CLOUD_PROJECT=your-project GOOGLE_CLOUD_LOCATION=us-central1 GOOGLE_GENAI_USE_VERTEXAI=True
python 04_hands_on.py
```

---

## Files
| File | What it is |
|---|---|
| `01_concepts.md` | Vertex AI + Gemini + ADK, mapped 1:1 from Azure AI Foundry |
| `02_architecture.md` | Vertex platform architecture + RAG/agent flow |
| `03_interview_qa.md` | 15 senior-level interview Q&A (multi-cloud focus) |
| `04_hands_on.py` | Gemini generation + embeddings + minimal ADK agent |
| `05_resume_bullet.md` | Ready-to-paste resume bullet |
| `requirements.txt` | Python dependencies |

---
*Bridge: Azure AI Foundry → Vertex AI · Azure OpenAI (GPT-4o) → Gemini · Azure AI Search → Vertex AI Search / Vector Search · text-embedding-3 → text-embedding-004 · Semantic Kernel / Foundry Agents → Agent Development Kit (ADK) · Managed Identity → service accounts / ADC · Content Safety → Vertex safety filters*
