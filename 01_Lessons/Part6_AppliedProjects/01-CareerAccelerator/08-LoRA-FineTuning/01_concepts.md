# 01 — Concepts: LoRA / QLoRA Fine-Tuning

> **Bridge from what you already know:** you know the *theory* (curriculum L11_3, L14) and Azure OpenAI fine-tuning. This module is the **hands-on open-source version** — running the fine-tune yourself with Hugging Face PEFT, owning the resulting adapter.

---

## 1. When fine-tuning (not prompting, not RAG)

The decision framework you already know, restated for this module:

| Need | Use |
|---|---|
| Up-to-date / private facts in the answer | **RAG** (fine-tuning bakes knowledge at train time; it can't access live data) |
| A behavior / format / tone / vocabulary change | **Fine-tuning** |
| A one-off or quick task | **Prompt engineering** (default; try first) |

**"Fine-tune for BEHAVIOR, RAG for KNOWLEDGE."** LoRA/QLoRA is *how* you fine-tune open-source models cheaply.

---

## 2. Full fine-tuning vs LoRA vs QLoRA (memory + compute)

| | Full fine-tuning | LoRA | QLoRA |
|---|---|---|---|
| What trains | ALL weights | Small adapter matrices only | Small adapters only |
| Base weights | Updated | **Frozen** (16-bit) | **Frozen + 4-bit quantized** |
| Trainable params | 100% | ~0.1–1% | ~0.1–1% |
| GPU memory (7B model) | ~28 GB+ | ~14 GB | **~5 GB** |
| Runs on | Data-center GPU | One A100 / good GPU | Consumer GPU / free Colab T4 |
| Quality | Baseline | ~within 1–3% of full | ~same as LoRA |

The progression is a memory-reduction story: full → LoRA (freeze the base) → QLoRA (also 4-bit the frozen base).

---

## 3. LoRA math, in plain English (no PhD required)

Fine-tuning normally learns a change `ΔW` to a big weight matrix `W`. `ΔW` is as big as `W` — expensive.

**LoRA's insight:** that change `ΔW` is "low-rank" — it can be approximated by multiplying two *small* matrices, `A` and `B`:

```
  W  (frozen, huge, e.g. 4096 × 4096 = 16.7M numbers)
  +
  A × B   (tiny: 4096×r and r×4096, with r=8 → only ~65K numbers)

  At inference:  output = W·x + (A·B)·x
                          └frozen┘  └the only part that trained┘
```

- `r` (rank) is a small number (4, 8, 16). It sets how big `A` and `B` are.
- You train **only A and B** (~0.1–1% of the model). The huge `W` never changes.
- Because language-model weight updates really do live in a low-dimensional subspace, this tiny addition captures most of the fine-tuning signal — quality within a few % of full fine-tuning.

**The payoff:** you fine-tune a 7B model by training ~a few million numbers instead of 7 billion, so it fits on a free Colab GPU, and the saved artifact (the adapter) is a few MB instead of ~14 GB.

---

## 4. LoRA key hyperparameters

| Param | Controls | Typical |
|---|---|---|
| `r` (rank) | Adapter size / capacity | 8 (start here); 4 for simple, 16 for more |
| `lora_alpha` | Scaling of the adapter output | 2 × r (e.g. r=8 → alpha=16) |
| `target_modules` | Which layers get adapters | attention projections (`q_proj`, `v_proj`) |
| `lora_dropout` | Regularization | 0.05 |

---

## 5. QLoRA = LoRA + 4-bit quantized frozen base

QLoRA takes LoRA further by storing the **frozen** base weights in **4-bit** (NF4) instead of 16-bit, cutting memory ~3× more — while still training the adapters in higher precision. Same quality as LoRA, ~1/3 the memory. This is the same quantization idea as GGUF/Ollama from module 01, applied to *training* instead of *serving*.

- Enabled via `BitsAndBytesConfig(load_in_4bit=True, bnb_4bit_quant_type="nf4", ...)`.
- QLoRA is what makes fine-tuning a 7B model on a single consumer/Colab GPU realistic.

---

## 6. PEFT — the Hugging Face toolkit

**PEFT** (Parameter-Efficient Fine-Tuning) is the library that implements LoRA/QLoRA:

```python
from peft import LoraConfig, get_peft_model, TaskType

config = LoraConfig(task_type=TaskType.CAUSAL_LM, r=8, lora_alpha=16,
                    target_modules=["q_proj", "v_proj"], lora_dropout=0.05)
model = get_peft_model(base_model, config)
model.print_trainable_parameters()   # e.g. "trainable: 0.06% of all params"
```

You then train with the standard HF `Trainer` / `SFTTrainer`, and `save_pretrained` writes just the **adapter** (a few MB) — not the whole model. At inference you load the base model + apply the adapter.

---

## 7. Quantization formats (name these in interviews)

| Format | Used for | Note |
|---|---|---|
| **GGUF** | Local serving (llama.cpp / Ollama) | The module-01 format |
| **AWQ** | GPU inference | Activation-aware quantization |
| **EXL2** | GPU inference (ExLlama) | High-throughput |
| **NF4** | QLoRA training | 4-bit normal-float for frozen weights |

GGUF/AWQ/EXL2 are *serving* quantization; NF4 is *training* quantization (QLoRA). Knowing the distinction signals depth.

---

## 8. LoRA/QLoRA vs Azure OpenAI fine-tuning

| | PEFT LoRA/QLoRA | Azure OpenAI fine-tuning |
|---|---|---|
| Model | Open-source (Llama, Phi, Mistral, GPT-2) | GPT-4o / GPT-4o-mini |
| Where it runs | Your GPU / Colab / Azure ML | Managed by Microsoft |
| Output | An adapter you own (few MB) | A hosted fine-tuned deployment |
| Cost | Compute only (free on Colab) | Pay per training token + hosting |
| Control | Full — weights, quantization, hyperparams | Managed, less control |

**The senior answer:** "For open-source models I fine-tune with PEFT LoRA/QLoRA — I own the adapter, run it on Colab or Azure ML, and control quantization and hyperparameters. For GPT-4o I use Azure OpenAI's managed fine-tuning. Either way I only fine-tune for a behavior/format/vocabulary problem — RAG for knowledge — and I need 100+ clean examples before it beats good prompting."

---

## 9. Overfitting (the thing to watch)

Training loss falling while **validation loss rises** = memorizing, not learning. Fixes: fewer epochs (stop before divergence), more/varied examples. With tiny datasets (10–20 examples) overfitting is easy — the notebook uses a small set to *show* the mechanics, not to produce a production model.

---
*Next: `02_architecture.md`*
