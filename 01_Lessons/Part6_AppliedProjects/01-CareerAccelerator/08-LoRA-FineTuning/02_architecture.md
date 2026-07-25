# 02 — Architecture: LoRA / QLoRA Fine-Tuning

## Where LoRA sits inside the model

```
  A transformer layer's weight matrix W (FROZEN):

        input x
          │
          ├──────────────▶ [ W ]  (frozen, huge)  ──┐
          │                                          ├──▶ output = W·x + A·B·x
          └──▶ [ A ] ──▶ [ B ]  (tiny, TRAINED) ─────┘
               (d×r)     (r×d)     r = 8 (rank)

  Only A and B train (~0.1–1% of params). W never changes.
  QLoRA additionally stores W in 4-bit (NF4) to save ~3× memory.
```

## The training flow (what the notebook does)

```
  ┌────────────────────┐   ┌──────────────────────┐   ┌───────────────────────┐
  │ Base model + tok    │──▶│ Apply LoRA (PEFT)     │──▶│ Train adapters only   │
  │ from HF Hub         │   │ LoraConfig(r, alpha,  │   │ (HF Trainer, N epochs)│
  │ (TinyLlama / GPT-2) │   │  target_modules)      │   │ on ~15 examples       │
  └────────────────────┘   └──────────────────────┘   └───────────┬───────────┘
        (optional QLoRA:                                          │
         4-bit BitsAndBytesConfig on the base)                    │
                                                                  ▼
  ┌──────────────────────────────────────────────────────────────────────────┐
  │ RESULTS                                                                    │
  │   • base-model output   vs   fine-tuned output   (same prompt)            │
  │   • training-loss curve (should fall; watch for val-loss rising=overfit)  │
  │   • save_pretrained -> adapter only (a few MB, NOT the whole model)       │
  └──────────────────────────────────────────────────────────────────────────┘
```

## Component breakdown

| Component | Role | Your Azure equivalent |
|---|---|---|
| **Base model** (HF Hub) | The frozen pretrained weights | The model behind an Azure OpenAI deployment |
| **Tokenizer** | Text ↔ token IDs (matched to the model) | Same |
| **LoraConfig** | Defines the adapters (r, alpha, target_modules) | Azure fine-tuning hyperparameters (managed) |
| **get_peft_model** | Wraps the base with trainable adapters | — (Azure does this internally) |
| **Trainer / SFTTrainer** | Runs the training loop | Azure OpenAI fine-tuning job |
| **BitsAndBytesConfig** | 4-bit quantization for QLoRA | — (Azure abstracts it) |
| **Adapter (save_pretrained)** | The tiny trained artifact you own | Azure's hosted fine-tuned model |
| **Loss curve** | Diagnoses fit / overfit | Foundry fine-tuning loss chart |

## Inference after fine-tuning

```
  load base model  +  load adapter (PeftModel.from_pretrained)  ─▶  fine-tuned behavior
  (optionally merge_and_unload() to bake the adapter into the weights for deployment)
```
You ship the small adapter alongside (or merged into) the base — the same "base + adapter" pattern that makes LoRA storage-cheap.

## Why this runs on free Colab

Full fine-tuning a 7B model needs ~28 GB+ of GPU memory (a data-center card). LoRA drops it to ~14 GB by freezing the base; QLoRA drops it to ~5 GB by 4-bit quantizing the frozen base — which fits on Colab's free **T4 (16 GB)**. The notebook uses a small model + LoRA so it runs comfortably even without QLoRA, and shows where you'd add QLoRA for larger models.

---
*Next: `03_interview_qa.md`*
