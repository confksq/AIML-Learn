# 05 — Resume Bullet

**Primary (concise):**
> Fine-tuned open-source LLMs with LoRA/QLoRA (Hugging Face PEFT) — training <1% of parameters on a free GPU and shipping few-MB adapters, complementing Azure OpenAI managed fine-tuning.

**Alternative (impact-oriented):**
> Delivered parameter-efficient fine-tuning (LoRA/QLoRA via PEFT) end to end — base-vs-tuned evaluation, loss-curve overfitting checks, and 4-bit QLoRA to fine-tune multi-billion-parameter models on consumer/Colab GPUs.

**Skills row additions:**
`LoRA · QLoRA · Hugging Face PEFT · parameter-efficient fine-tuning · quantization (NF4/GGUF/AWQ) · adapter serving`

**Talking point for interviews:**
"I fine-tune open-source models with PEFT LoRA/QLoRA — freeze the base, train tiny adapters (~0.1% of params), and own a few-MB artifact that runs on a free GPU — and I use Azure OpenAI managed fine-tuning for GPT-4o. Either way I fine-tune for behavior/format, not knowledge, and only when 100+ clean examples beat good prompting. I watch the loss curves for overfitting and evaluate base-vs-tuned before shipping."
