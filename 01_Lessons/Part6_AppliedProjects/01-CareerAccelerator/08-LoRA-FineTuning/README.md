# 08 — LoRA / QLoRA Fine-Tuning

**Part of:** Career Accelerator portfolio · **PRD Feature L8** · **Phase 3 (Week 5)**
**Skill:** Hands-on parameter-efficient fine-tuning (LoRA / QLoRA) with Hugging Face PEFT — running an actual fine-tune, not just the theory.

---

## Why this module matters for the job search

~20% of JDs — including an "AI/ML Engineer — Embedding Model Fine-Tuning" role in the inbox. This is a **high-interview-frequency** topic, which is why it has **20 Q&A** (more than the other modules). You already know the *theory* (curriculum L11_3 / L14) and Azure OpenAI fine-tuning. This module closes the gap that matters most: **doing an actual open-source fine-tune** — loading a base model, applying LoRA adapters with PEFT, training on a small dataset, and comparing base vs fine-tuned output — all on **free Google Colab**.

---

## What you'll have after this module
- A **Colab-compatible notebook** (`04_lora_finetune.ipynb`) that fine-tunes a small model with LoRA end to end and shows base-vs-fine-tuned output + a loss curve
- A concepts doc that turns the LoRA math into plain English (no PhD required)
- **20 senior-level interview Q&A** — the highest count in the track, because this topic gets grilled

---

## Prerequisites
- **Google Colab (free tier)** — the notebook is written to run there with a free GPU (Runtime → Change runtime type → T4 GPU). Or run locally with a GPU.
- No paid API — the base model (TinyLlama / GPT-2) downloads from the Hugging Face Hub.
```bash
pip install -r requirements.txt   # if running locally instead of Colab
```

---

## Quick start
1. Open `04_lora_finetune.ipynb` in Google Colab (upload it, or File → Open notebook → GitHub → this repo).
2. Runtime → Change runtime type → **T4 GPU**.
3. Run all cells top to bottom. It installs deps, loads a base model, applies LoRA, fine-tunes on ~15 examples, and prints base-vs-fine-tuned output plus the training-loss curve.

---

## Files
| File | What it is |
|---|---|
| `01_concepts.md` | LoRA/QLoRA in plain English, bridged from your fine-tuning theory |
| `02_architecture.md` | Where LoRA sits in the model + the training flow |
| `03_interview_qa.md` | **20** senior-level interview Q&A (highest count) |
| `04_lora_finetune.ipynb` | Colab notebook: LoRA fine-tune end to end + base-vs-tuned comparison |
| `05_resume_bullet.md` | Ready-to-paste resume bullet |
| `requirements.txt` | Python dependencies |

---
*Bridge: Azure OpenAI fine-tuning (managed) → PEFT LoRA (open-source, you own the adapter) · QLoRA quantization → the GGUF/4-bit idea from module 01 · fine-tune vs RAG (curriculum L14) → the decision framework revisited*
