# Module 1 — GenAI/LLM Fundamentals
**Source plan:** `AIML-Learn/04_Career/00_PRD.md` §4, `01_EXECUTION_PLAN.md`
**Format:** Concept-check (Tier 1 — DEPLOY/SCALE lenses don't meaningfully apply to pure fundamentals)
**Question count:** 15

---

### Q1. What is attention, and why did it replace RNN/LSTM architectures for sequence modeling?

**Answer:**
Attention lets a model weigh how relevant every other token in a sequence is to the token currently being processed, and pull information from the most relevant ones regardless of distance. RNNs/LSTMs process tokens sequentially and compress everything seen so far into a fixed-size hidden state — long-range dependencies decay ("vanishing gradient"-style information loss) and the sequential nature blocks parallelization during training.

Attention (via the Transformer) removes the sequential bottleneck: every token can attend to every other token in a single matrix operation, so training parallelizes across the full sequence on GPUs, and long-range dependencies don't decay with distance the way they do in a recurrent hidden state.

**Follow-up probe:** "What's the computational cost of that — and why does it matter for context window size?"
(Expected: self-attention is O(n²) in sequence length — this is exactly why context window scaling is expensive and why techniques like sparse/sliding-window attention and FlashAttention exist.)

---

### Q2. Explain self-attention vs cross-attention — where is each used?

**Answer:**
- **Self-attention:** queries, keys, and values all come from the *same* sequence. Used to let a sequence build context-aware representations of itself (e.g., a decoder attending over the tokens it has generated so far).
- **Cross-attention:** queries come from one sequence, keys/values from a *different* sequence. Classic use: an encoder-decoder model where the decoder's queries attend over the encoder's output (e.g., translation — the decoder attends over the source-language encoding while generating the target language).

Most modern decoder-only LLMs (GPT-style, and what Azure OpenAI serves) use masked self-attention only — no separate encoder, no cross-attention layer, since the "context" is just the preceding tokens in the same stream (including anything injected via RAG/system prompt).

**Follow-up probe:** "If everything is decoder-only self-attention, how does RAG-retrieved context get 'attended to' differently than the original prompt?"
(Expected: it doesn't get architecturally special treatment — it's just tokens in the context window. Any distinction is prompt-engineering/positioning, not a separate attention mechanism. This is exactly why prompt structure and retrieval placement matter for grounding quality.)

---

### Q3. Walk through the Transformer architecture end to end.

**Answer:**
Input text → **tokenization** → **embedding** (each token maps to a dense vector) → **positional encoding** added (since attention has no inherent notion of order) → stack of **Transformer blocks**, each containing:
1. Multi-head self-attention (multiple attention "heads" learn different relationship types in parallel, outputs concatenated)
2. Add & layer-norm (residual connection + normalization for training stability)
3. Feed-forward network (position-wise MLP, typically 4x the hidden dimension)
4. Add & layer-norm again

→ final layer projects hidden states to vocabulary-size logits → softmax → probability distribution over next token → sampling strategy picks the actual next token → repeat autoregressively.

**Follow-up probe:** "Why do we need positional encoding if attention already looks at the whole sequence?"
(Expected: attention is permutation-invariant by design — without positional encoding, "the dog bit the man" and "the man bit the dog" would produce identical attention patterns since the *set* of tokens is the same, only order differs.)

---

### Q4. What is tokenization, and why does it matter architecturally — not just linguistically?

**Answer:**
Tokenization breaks text into sub-word units (most Azure OpenAI models use a BPE — Byte-Pair Encoding — variant, e.g. `cl100k_base`/`o200k_base`) rather than whole words or characters, balancing vocabulary size against sequence length. It matters architecturally because:
- **Cost:** billing is per-token (input + output), so token-inefficient text (e.g. non-English languages, code, or unusual formatting) directly costs more
- **Context window:** the window is a token budget, not a character or word budget — the same content can consume very different amounts of context depending on tokenizer efficiency for that content type
- **Latency:** more tokens = more autoregressive generation steps = higher latency for output tokens specifically

**Follow-up probe:** "A client complains their non-English document pipeline burns through context window twice as fast as expected — why, and what's the architectural fix?"
(Expected: BPE tokenizers are trained predominantly on English-heavy corpora, so non-Latin-script or non-English text often tokenizes less efficiently — more tokens per unit of meaning. Fixes: chunk sizing calibrated per-language rather than a flat character count, monitor actual token counts not word counts, consider whether a model with a tokenizer better suited to the target language is available.)

---

### Q5. What are embeddings, and how is similarity between them measured?

**Answer:**
An embedding is a dense vector representation of text (word, sentence, or document) in a continuous space where geometric proximity approximates semantic proximity — texts with similar meaning map to vectors that are close together. Generated by a dedicated embedding model (e.g. `text-embedding-3-large` on Azure OpenAI), separate from the generative/chat model.

Similarity is most commonly measured via **cosine similarity** (the cosine of the angle between two vectors, ranging -1 to 1, insensitive to magnitude — only direction matters) rather than Euclidean distance, because embedding magnitude isn't semantically meaningful; direction is.

**Follow-up probe:** "Why cosine similarity specifically, and when would Euclidean distance be the wrong choice?"
(Expected: embedding vectors aren't normalized to unit length by all models, and raw magnitude can vary for reasons unrelated to meaning — e.g., text length or model-specific scaling. Cosine similarity normalizes that out. Euclidean distance would penalize magnitude differences that have nothing to do with semantic difference, which is exactly the failure mode you don't want in a retrieval system.)

---

### Q6. What determines a model's context window, and what are the trade-offs of a larger one?

**Answer:**
Context window size is a training-time architectural decision (positional encoding scheme, attention pattern, and training data sequence lengths) — it's not something you can arbitrarily extend at inference time without retraining or specific extension techniques (e.g. RoPE scaling, ALiBi). Azure OpenAI models ship with fixed context windows per model/version (e.g. 128K for GPT-4o-class models).

Trade-offs of a larger window:
- **Cost:** you pay per input token — a larger window invites stuffing more context in, which directly increases cost per call
- **Latency:** attention is O(n²) — processing a longer input increases prefill latency
- **Quality — "lost in the middle":** empirically, models attend less reliably to information in the middle of a very long context than to the beginning or end, so a bigger window doesn't guarantee better grounding; poorly-curated long context can perform worse than well-curated short context

**Follow-up probe:** "Given 'lost in the middle,' how does that change your RAG chunk-retrieval and context-assembly strategy?"
(Expected: retrieve fewer, higher-relevance chunks rather than maximizing how much fits; place the most critical retrieved content near the start or end of the prompt; re-rank before assembly rather than relying on raw vector-similarity order.)

---

### Q7. Differentiate pretraining, fine-tuning, and RLHF.

**Answer:**
- **Pretraining:** self-supervised next-token prediction over a massive, broad corpus. This is where the model learns language structure, world knowledge, and reasoning patterns. Extremely compute-expensive; done once by the model provider.
- **Fine-tuning:** further supervised training on a smaller, task/domain-specific labeled dataset, adjusting model weights to specialize behavior (e.g. Azure OpenAI fine-tuning on a JSONL dataset of prompt/completion pairs for a specific format or domain vocabulary).
- **RLHF (Reinforcement Learning from Human Feedback):** a distinct alignment stage — human raters rank model outputs, a reward model is trained on those rankings, and the base model is further trained (typically via PPO or similar) to maximize outputs humans prefer. This is what turns a raw pretrained model into something that follows instructions, refuses harmful requests, and behaves conversationally rather than just completing text statistically.

**Follow-up probe:** "If fine-tuning specializes the model and RLHF aligns it, why would you ever need both — walk through a scenario where you'd use one but not the other."
(Expected: fine-tuning changes *what* the model knows/how it formats output — e.g., adopting a company's document schema; it doesn't fix *how* the model behaves conversationally, which is what RLHF already handled at the base-model level. Most Architect-level work only touches fine-tuning — RLHF is a provider-side stage you consume, not one you typically run yourself, unless building a fully custom model.)

---

### Q8. What problem does RLHF solve that pretraining alone doesn't?

**Answer:**
Pretraining optimizes purely for next-token prediction likelihood over training data — it has no notion of "helpful," "harmless," or "follows instructions." A raw pretrained model will often continue a prompt in the most statistically likely way (e.g., completing a question with more questions, mimicking whatever style is prevalent in similar training data) rather than answering it directly. RLHF explicitly optimizes for human-preferred behavior — instruction-following, refusing harmful requests, matching a helpful conversational tone — which is a fundamentally different objective than "predict the next token accurately."

**Follow-up probe:** "Does RLHF eliminate hallucination?"
(Expected: no — hallucination is rooted in the underlying probabilistic generation process (Q9), not a behavioral misalignment RLHF is designed to fix. RLHF can reduce *some* hallucination patterns by rewarding calibrated uncertainty, but it doesn't structurally prevent a model from generating plausible-sounding incorrect content. That's why grounding techniques like RAG and groundedness detection exist as a separate mitigation layer.)

---

### Q9. Why do LLMs hallucinate — what's the fundamental cause?

**Answer:**
An LLM generates text by sampling from a probability distribution over the next token, conditioned on everything before it — it has no built-in fact-verification step, no notion of "I don't know," and no grounding in an external source of truth unless one is explicitly provided (e.g., via RAG). When the model's training data doesn't contain the answer, or when the prompt asks for something outside its knowledge, it still produces the most statistically plausible continuation — which can be fluent, confident, and entirely fabricated, because fluency and factuality are not the same optimization target.

**Follow-up probe:** "Your RAG pipeline is grounded in retrieved documents and still hallucinates — what are the two most likely root causes?"
(Expected: (1) retrieval failure — the relevant chunk wasn't retrieved at all, so the model is answering from parametric memory instead of the provided context; (2) the model ignoring/overriding provided context in favor of its own "knowledge" — mitigated by explicit grounding instructions, groundedness detection/scoring post-generation, and citation-forcing prompt patterns.)

---

### Q10. What do temperature, top-p, and top-k control, and how would you tune them for different use cases?

**Answer:**
All three control how the next token is sampled from the model's output probability distribution:
- **Temperature:** scales the distribution before sampling — lower (near 0) sharpens it toward the highest-probability token (more deterministic, repetitive); higher flattens it (more random/creative, higher hallucination risk)
- **Top-p (nucleus sampling):** samples only from the smallest set of tokens whose cumulative probability exceeds p — dynamically adjusts how many candidates are considered based on the model's confidence at that step
- **Top-k:** restricts sampling to the k highest-probability tokens, regardless of their cumulative probability mass

Tuning guidance:
- **Structured/factual output (extraction, classification, code generation, RAG answers):** low temperature (0–0.3), often combined with a tight top-p — you want determinism and low hallucination risk
- **Creative/brainstorming tasks:** higher temperature (0.7–1.0) — variety is the goal, occasional incoherence is an acceptable trade-off

**Follow-up probe:** "For a production RAG endpoint answering customer questions from grounded documents, what temperature would you set, and why not just set it to 0?"
(Expected: low, e.g. 0.1–0.2, not always literal 0 — temperature 0 is fully deterministic given identical input, which sounds ideal for reproducibility, but some providers/architectures still exhibit minor non-determinism at temp 0 due to floating-point/batching effects, and a very slight temperature can help the model avoid degenerate repetition loops on edge-case prompts. The key point the interviewer wants: near-zero, not necessarily exactly zero, and the reasoning why.)

---

### Q11. What are scaling laws, and what actually determines how capable a model is?

**Answer:**
Empirically (Kaplan et al., Chinchilla), model capability scales predictably with three levers: **parameter count**, **training data volume**, and **compute budget** — and critically, they must scale *together*. The Chinchilla finding specifically showed many early large models were "over-parameterized, under-trained" — a smaller model trained on proportionally more data outperformed a larger model trained on too little data for its size, at the same compute cost. This is why newer model families emphasize data quality/volume, not just parameter count, when claiming capability gains.

**Follow-up probe:** "A vendor pitches you a model with 2x the parameters as an upgrade — what do you ask before believing it's actually better?"
(Expected: was it trained compute-optimally for that parameter count, i.e., proportionally more data — or just scaled up on the same data? Also ask for benchmark deltas, not parameter count, since parameter count alone isn't a capability proxy.)

---

### Q12. When would you use zero-shot, few-shot prompting, versus fine-tuning?

**Answer:**
- **Zero-shot:** no examples in the prompt — relies entirely on the model's pretrained/RLHF-aligned general capability. Use when the task is common enough to be well-represented in training data (e.g., "summarize this," "extract the vendor name").
- **Few-shot:** a handful of example input/output pairs included directly in the prompt to demonstrate the desired format/pattern, no weight changes. Use when the task has an unusual output format or narrow domain convention that zero-shot doesn't reliably produce, but you don't want the overhead of fine-tuning.
- **Fine-tuning:** actual weight updates on a labeled dataset. Use when few-shot isn't reliable enough at scale (i.e., you'd need so many examples per call that it blows the context budget), when the task is high-volume enough that fine-tuning's fixed cost pays off, or when you need consistent behavior a prompt alone can't guarantee.

**Follow-up probe:** "Your few-shot prompt works great in testing but degrades in production — what's the most likely cause?"
(Expected: production inputs are more varied than the handful of examples covered — few-shot generalizes only as well as the examples chosen represent the real input distribution. This is the actual signal that it's time to move to fine-tuning.)

---

### Q13. What is Mixture of Experts (MoE), and why does it matter for cost/latency?

**Answer:**
MoE architectures replace a single dense feed-forward block with multiple "expert" sub-networks, plus a router that selects only a small subset (e.g., 2 of 8) of experts to activate per token. Total parameter count can be very large (more capacity/knowledge), while the *active* parameters per forward pass stay small — so inference cost and latency track the active parameter count, not the total.

**Follow-up probe:** "What's the architectural trade-off MoE makes to get that efficiency?"
(Expected: memory footprint — all experts must be loaded/available even though only a few activate per token, so MoE models are memory-hungry to serve even though they're compute-cheap per token. This is a deployment/infrastructure consideration, not just a modeling one.)

---

### Q14. At the architecture level, how do multimodal models combine image and text?

**Answer:**
Broadly: an image is passed through a vision encoder (often a ViT — Vision Transformer) that produces a sequence of image "tokens" (patch embeddings), which are then projected into the same embedding space as text tokens and fed into the same (or a connected) Transformer alongside the text tokens — attention then operates across both modalities jointly. This is why multimodal capability isn't "a separate image model bolted on" — the image content participates in the same attention mechanism as the text.

**Follow-up probe:** "Why does that architectural choice mean image inputs also consume your token budget/context window?"
(Expected: because images are tokenized into a sequence of patch embeddings that occupy positions in the same context window as text — a high-resolution image can consume a meaningful chunk of the context window, which is a real cost/context-budget consideration when architecting a vision + RAG pipeline, e.g. for Document Intelligence-adjacent multimodal use cases.)

---

### Q15. What is model distillation, and when would you deploy a distilled model instead of the full-size one?

**Answer:**
Distillation trains a smaller "student" model to mimic a larger "teacher" model's outputs (often matching the teacher's output probability distribution, not just the final answer, which transfers more nuance than hard labels alone). The result is a smaller, cheaper, faster model that retains a meaningful fraction of the teacher's capability on the tasks it was distilled for.

**When to deploy the distilled version:** high-volume, latency-sensitive, or cost-sensitive production paths where the task is narrow enough that the capability gap doesn't matter (e.g., a classification/routing step ahead of a more expensive call) — use the full model only where the task genuinely needs its broader reasoning capability.

**Follow-up probe:** "Design a two-tier architecture using this — where does the distilled model go, where does the full model go?"
(Expected: a model-routing/tiering pattern — a cheap, fast, distilled model handles simple/high-volume requests or an initial classification/triage step; requests it flags as complex, ambiguous, or high-stakes get escalated to the full model. This is a direct preview of the pricing/cost-optimization content in Module 5.)

---

*Module 1 of 6 — GenAI Architect Interview Prep. Next: Module 2 — Azure AI Platform.*
