# Q&A — L14: Fine-Tuning LLMs
**Source chapter:** `01_Lessons/Part3_GenAI_LLMs/L14_FineTuning.md` (internally "Module 15") | **Format:** self-study
**Questions:** 26 | *No overlap with the interview bank or the chapter's own self-test — these drill the chapter's concrete practice. Note: L11_3 covered fine-tuning theory; this chapter is the practical Azure workflow.*

---

## When to Fine-Tune

**Q1. Name the four legitimate reasons to fine-tune, with a JMA example each.**
(1) **Format consistency** — always the same JSON schema (invoice extraction). (2) **Domain vocabulary** — model correctly uses "curtailment," "floorplan," "dealer reserve." (3) **Style/tone** — consistent brand voice without long per-prompt instructions. (4) **Cost optimization at scale** — fine-tune GPT-4o mini to match GPT-4o on a specific task (~10x cheaper per call for high-volume classification).

**Q2. Name the four situations where you must NOT fine-tune, and the right tool for each.**
Need current/private data → **RAG** (fine-tuning bakes knowledge at train time, can't access live data). Fewer than 50–100 examples → **few-shot prompting** (tiny datasets overfit). Task changes frequently → **prompt engineering** (re-fine-tuning per change is expensive; prompt updates are instant/free). GPT-4o already does it well → **don't fine-tune** (adds cost, complexity, maintenance).

**Q3. From the decision table, which tool wins for: private-doc Q&A, always-JSON, summarize a document, reduce cost on high volume, match brand voice?**
Private-doc Q&A → RAG. Always-JSON → fine-tune. Summarize → prompt engineering. Reduce cost on high volume → fine-tune (a smaller model). Match brand voice → fine-tune.

**Q4. Walk the cost break-even for fine-tuning.**
Training is cheap (~$5–8 for 3–5 epochs on 1,000 GPT-4o-mini examples). The real cost is **dedicated deployment hosting** (~$1.70/hr ≈ $40/day just to keep it running). Inference is ~16x cheaper per call than standard GPT-4o. Break-even: **10,000+ calls/day** makes fine-tuning economical; ~100 calls/day and hosting cost exceeds the savings.

---

## Azure OpenAI Fine-Tuning

**Q5. Which models can and cannot be fine-tuned in Azure OpenAI?**
Can: GPT-4o mini (best for most scenarios), GPT-4o (limited, when quality is critical), GPT-3.5-turbo (legacy, being phased out). Cannot: **embedding models** — use them as-is or change chunking strategy instead.

**Q6. What is the training-data format, and what does one line contain?**
**JSONL** — one JSON object per line, no commas between lines, UTF-8, max 512 MB. Each line is one complete conversation: a `messages` array with system + user + assistant. The assistant message is the target the model learns to produce.

**Q7. State the example-count tiers: technical minimum, practical minimum, production ideal.**
Technical minimum: **10** (Azure allows it, quality is poor). Practical minimum: **50–100**. Production ideal: **200–500** diverse, clean, representative examples, with ~20% held out as a validation set.

**Q8. What makes training data good vs bad?**
**Good:** diverse inputs (many phrasings of the same question), consistent outputs (identical format every time), representative (covers production cases), clean (no typos/wrong answers), balanced (not 90% one type). **Bad:** duplicate inputs (memorization not generalization), inconsistent outputs (conflicting patterns learned), only easy cases (fails on edge cases), wrong answers (learns to be reliably wrong).

**Q9. List the 7 steps of the Azure OpenAI fine-tuning workflow.**
(1) Prepare training data (JSONL) → (2) upload the file → (3) create the fine-tuning job → (4) monitor training progress → (5) deploy the fine-tuned model → (6) test and evaluate → (7) use in your application.

**Q10. In the C# SDK, what `purpose` do you set when uploading, and what does the `Suffix` do?**
`FileUploadPurpose.FineTune` on upload. The `Suffix` (e.g., "jmf-invoice-v1") names the resulting fine-tuned model so you can identify it among deployments.

**Q11. What does passing `null` for the hyperparameters (NEpochs, BatchSize, LearningRateMultiplier) mean?**
Azure **auto-selects** them — the recommended starting point; only override once you have a reason from evaluation results.

**Q12. What's the minimum production bar when evaluating a fine-tuned model?**
Fine-tuned accuracy > base-model accuracy; **format compliance 99%+** (the thing you fine-tuned for); and a **regression check** — the fine-tuned model must not be *worse* on general tasks it should still handle.

---

## Loss Curves & Overfitting

**Q13. What do training loss and validation loss each measure?**
Training loss — how well the model fits the training data (should decrease steadily). Validation loss — how well it generalizes to unseen examples (should decrease roughly tracking training loss).

**Q14. What's the overfitting signal, and four fixes?**
Signal: **training loss keeps falling while validation loss starts rising** — the model is memorizing, not learning the pattern. Fixes: stop at the epoch before divergence, add more training examples, add more variety to examples, reduce epochs on future runs.

**Q15. What's the epoch guidance: underfit / optimal / overfit-risk ranges?**
1–2 epochs → underfitting (hasn't learned enough). 3–4 epochs → usually optimal. 5+ epochs → overfitting risk, watch validation loss carefully.

---

## LoRA / QLoRA (Practical)

**Q16. Do the LoRA parameter math: a 4096×4096 matrix (16.7M params), rank r=16.**
LoRA trains A (4096×16) + B (16×4096) = 65,536 + 65,536 = **131,072 parameters** ≈ **0.4% of the full matrix's 16.7M** (chapter's Exercise 3). At inference: `output = W₀(input) + A(B(input))` — the frozen result plus the adapter result.

**Q17. What do the LoRA hyperparameters rank (r) and alpha (α) control, and the common defaults?**
`r` — adapter size/expressiveness (4=simple/fast, 8=common default, 16=more expressive, 64≈full fine-tune). `α` — output scaling factor; common practice is **α = 2×r** (r=8, α=16 → scaling 2). `target_modules` — which layers get adapters (commonly q_proj and v_proj).

**Q18. QLoRA vs LoRA — memory for a 7B model, and when to pick QLoRA?**
LoRA keeps frozen weights in float16 (~14 GB for 7B). QLoRA quantizes frozen weights to **4-bit** (~5 GB) while training adapters in bfloat16 — same quality, ~3x less memory. Pick QLoRA when GPU memory is the constraint (8–12 GB) or to fine-tune larger models on smaller GPUs.

**Q19. LoRA/QLoRA vs Azure OpenAI fine-tuning — when each?**
**Azure OpenAI fine-tuning** — your app already uses Azure OpenAI (GPT-4o/mini), want managed infra, need enterprise compliance, small-medium dataset — the JMA production choice. **LoRA/QLoRA** — fine-tuning an **open-source** model (Llama/Mistral/Phi), want to own the weights, very large dataset, research. Note: you **cannot** LoRA GPT-4o — its weights aren't accessible.

**Q20. In the PEFT Python example, what does `print_trainable_parameters()` reveal, and how big is the saved artifact?**
It shows ~**0.055%** of parameters are trainable (e.g., 2.1M of 3.8B for Phi-3 mini). The saved **LoRA adapters are only ~8 MB** vs the full ~7 GB model — you store and share just the adapters; at inference you load the base model + apply adapters.

---

## Misconceptions & 2026

**Q21. Recite the core one-line rule of this chapter.**
**"Fine-tune for BEHAVIOR — RAG for KNOWLEDGE."** Fine-tuning changes how the model formats/speaks/behaves; RAG injects retrievable, changeable facts.

**Q22. Correct: "the AI team wants to fine-tune a model to answer questions about internal invoice policies."**
Wrong tool — policies change (new rules, penalties, agreements), so every update would force expensive re-fine-tuning. Use **RAG**: store policies in Azure AI Search, retrieve at query time, answer from current documents. Fine-tuning is for behavior/format, not changing knowledge.

**Q23. What is DPO (Direct Preference Optimization) from the 2026 updates?**
**Preference fine-tuning** — an alternative to RLHF for alignment. You provide pairs of (preferred response, rejected response) per prompt; the model learns to prefer the better one. Simpler than full RLHF, often comparable results.

**Q24. What is distillation as an Azure OpenAI capability, and design the JMA pipeline (Exercise 4)?**
Use a large model's outputs as fine-tuning data for a small model — bake GPT-4o's behavior into GPT-4o mini. Pipeline: (1) generate ~1,000 dealer-support responses with GPT-4o (teacher) → (2) fine-tune GPT-4o mini (student) on them → (3) evaluate whether mini now matches 4o on the test set → (4) compute savings over 12 months at 4o vs mini pricing.

**Q25. What changed for GPT-4o and Phi-4 fine-tuning in 2026?**
Full **GPT-4o fine-tuning is GA** (not just mini — higher capability ceiling, ~$25/1M training tokens, vision fine-tuning supported). **Phi-4 (3.8B)** is now fine-tuneable — cheaper inference, competitive quality; a good JMA candidate for automotive/financial vocabulary. LoRA on Llama 3/Phi-4/Mistral is available directly in **AI Foundry** (no GPU cluster setup, outputs a small adapter file).

**Q26. Decision drill (Exercise 1) — pick the tool for each: formal-only language, current inventory, always-JSON-with-exact-fields, expert in contract terminology, "ATL = Southeast region."**
Formal-only language → system prompt (fine-tune if prompting insufficient). Current inventory → **tool/function calling** (live data). Always-JSON exact fields → **fine-tune**. Expert in contract terminology → **fine-tune** (domain vocabulary). "ATL = Southeast region" (single fact) → **system prompt** (one term doesn't justify training).

---

*Curriculum Q&A Batch D — file 1 of 3. Next: QA_L15 (Prompt Engineering).*
