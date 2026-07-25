# 03 — Interview Q&A: LoRA / QLoRA Fine-Tuning (20 questions, senior level)

> This module has 20 Q&A — fine-tuning is one of the most-grilled topics in Senior AI interviews.

---

**Q1. When do you fine-tune instead of prompting or RAG?**
Fine-tune for a behavior/format/tone/vocabulary change; RAG for up-to-date or private knowledge; prompting as the default first try. "Fine-tune for BEHAVIOR, RAG for KNOWLEDGE." And only fine-tune when you have 100+ clean examples and prompting genuinely can't solve it — fine-tuning adds cost and maintenance.

**Q2. What does LoRA actually do differently from full fine-tuning?**
Full fine-tuning updates all weights — for a 7B model that's 7 billion numbers and ~28 GB of GPU memory. LoRA freezes the base weights and trains only two small matrices (A and B) whose product approximates the weight change, ~0.1–1% of the parameters. Same task quality within a few percent, ~95% less memory, and the saved artifact is a few MB instead of ~14 GB.

**Q3. Explain the LoRA low-rank idea in plain English.**
Fine-tuning learns a change ΔW to a big weight matrix. LoRA's insight is that ΔW is low-rank — expressible as two small matrices A (d×r) and B (r×d) multiplied together, where r is small (8). So instead of training a full-size ΔW you train A and B, and at inference the output is W·x + (A·B)·x. The tiny addition captures most of the fine-tuning signal because LM weight updates live in a low-dimensional subspace.

**Q4. What is the rank (r) hyperparameter and how do you choose it?**
r sets the size of the A and B matrices — the adapter's capacity. r=8 is a good default; 4 for simple tasks, 16 for more expressive ones, higher approaches full fine-tuning cost. Bigger r = more capacity and memory. I start at 8 and only raise it if the task underfits.

**Q5. What is lora_alpha?**
A scaling factor applied to the adapter's output (effective scaling ≈ alpha/r). Common practice is alpha = 2×r (r=8 → alpha=16). It controls how strongly the adapter influences the frozen base's output.

**Q6. What are target_modules?**
The layers LoRA attaches adapters to — commonly the attention query/value projections (q_proj, v_proj). More target modules = more capacity but more memory. Attention projections are the standard sweet spot; you can add more (k_proj, o_proj, MLP layers) for harder tasks.

**Q7. What is QLoRA and how much memory does it save?**
QLoRA = LoRA plus 4-bit quantization (NF4) of the frozen base weights, while training the adapters in higher precision. It cuts memory roughly 3× more than LoRA — a 7B model goes from ~14 GB (LoRA) to ~5 GB (QLoRA) — with essentially the same quality. It's what makes fine-tuning a 7B model on a free Colab T4 realistic.

**Q8. Why does QLoRA not hurt quality much despite 4-bit weights?**
The 4-bit quantization is only on the frozen base weights, which aren't being updated — and NF4 is designed to preserve the distribution of neural-net weights well. The adapters that actually learn are kept in higher precision (bf16). So the trainable part is full-precision; only the static backbone is compressed.

**Q9. What is PEFT?**
Hugging Face's Parameter-Efficient Fine-Tuning library — it implements LoRA, QLoRA, and other adapter methods. You wrap a base model with `get_peft_model(model, LoraConfig(...))`, train with the standard Trainer, and `save_pretrained` writes just the adapter. It's the standard open-source fine-tuning toolkit.

**Q10. What gets saved after a LoRA fine-tune, and how big is it?**
Only the adapter matrices — typically a few MB — not the multi-GB base model. At inference you load the base model and apply the adapter (PeftModel.from_pretrained), or merge the adapter into the weights for deployment. This is why LoRA is storage- and distribution-cheap: one base, many small task adapters.

**Q11. LoRA/QLoRA vs Azure OpenAI fine-tuning — when each?**
PEFT LoRA/QLoRA for open-source models (Llama, Phi, Mistral) — I own the adapter, run it on Colab/Azure ML, and control quantization and hyperparameters. Azure OpenAI fine-tuning for GPT-4o/mini — managed, hosted, pay per token, less control. The decision follows which base model you need and how much control vs convenience you want.

**Q12. Distinguish GGUF, AWQ, EXL2, and NF4.**
GGUF is the serving format for llama.cpp/Ollama (local inference). AWQ (activation-aware) and EXL2 are GPU-inference quantization formats. NF4 is the 4-bit training quantization used by QLoRA for the frozen base. Key distinction: GGUF/AWQ/EXL2 are for serving; NF4 is for training.

**Q13. How do you know if fine-tuning is working — what do you watch?**
The loss curves. Training loss should fall steadily. If validation loss also falls and both plateau together, that's a good fit. If training loss keeps falling while validation loss rises, that's overfitting — the model is memorizing. I also compare base-model vs fine-tuned output on held-out prompts to confirm the behavior actually changed as intended.

**Q14. What is overfitting in fine-tuning and how do you fix it?**
The model memorizes the training examples instead of learning the general pattern — training loss keeps dropping while validation loss rises, and it fails on new inputs. Fixes: stop at fewer epochs (before divergence), add more and more-varied training examples, and reduce learning rate. Tiny datasets (10–20 examples) overfit easily.

**Q15. How many examples do you need, and how important is quality?**
Technically as few as ~10, but 50–100 minimum for reliable results, 200–500 for production quality — with a held-out validation set. Quality dominates: 500 clean, diverse, consistent examples beat 50,000 noisy ones. Bad data teaches the model to be reliably wrong.

**Q16. What's the difference between fine-tuning for knowledge vs behavior — why does it matter here?**
Fine-tuning changes behavior/format/vocabulary, not live knowledge — it bakes patterns into weights at training time and can't access data that changes afterward. Trying to fine-tune in facts that change (a policy, prices) is the classic mistake; that's a RAG problem. LoRA is a behavior tool; pair it with RAG when you also need current knowledge.

**Q17. What is catastrophic forgetting and how does LoRA help?**
When fine-tuning on a narrow task degrades the model's general capabilities (it "forgets" what it knew). LoRA mitigates this because the base weights are frozen — you're adding a small adapter rather than overwriting the pretrained knowledge — so the general capability is preserved and you can even swap adapters per task. It's not immune, but it's more resistant than full fine-tuning.

**Q18. Can you serve multiple fine-tunes efficiently with LoRA?**
Yes — because each fine-tune is just a small adapter over the same frozen base, you can host one base model and hot-swap or even batch multiple LoRA adapters (multi-LoRA serving). That's far cheaper than hosting a full fine-tuned copy per task, and a real production advantage of the adapter approach.

**Q19. Walk through your Colab LoRA notebook.**
Load a small base model + tokenizer from the HF Hub, apply LoRA via PEFT LoraConfig (r=8, alpha=16, target q_proj/v_proj) and confirm only ~0.06% of params are trainable, fine-tune on ~15 instruction/response examples with the HF Trainer for a few epochs, then compare base-model vs fine-tuned output on the same prompt and plot the training-loss curve. Finally save the adapter (a few MB). It runs on a free Colab T4.

**Q20. Design a fine-tuning strategy for a JMA use case — consistent JSON invoice extraction at high volume.**
This is a behavior/format problem, so fine-tuning fits. I'd curate 200–500 clean examples (invoice text → exact target JSON schema), split 80/20, and fine-tune GPT-4o-mini via Azure OpenAI managed fine-tuning for the production path — or a small open-source model via QLoRA on Azure ML if I need to own the weights/run on-prem. Validate format compliance (target 99%+) against the held-out set, gate on it, and keep RAG for the policy knowledge that changes. Fine-tune only the format; never the facts.

---
*Fine-tuning gets grilled — be fluent on the LoRA math intuition, the memory numbers, QLoRA, and the fine-tune-vs-RAG boundary.*
