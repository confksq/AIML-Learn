# Q&A — L11_1: LLMs — Attention & Transformer Architecture
**Source chapter:** `01_Lessons/Part3_GenAI_LLMs/L11_1_LLMs_Attention_Transformer.md` | **Format:** self-study
**Questions:** 26 | *No overlap with the interview bank (01_Fundamentals covers the architect-level versions) or the chapter's own mini quiz — these drill the chapter's specifics.*

---

## Attention Mechanics

**Q1. Using the chapter's "bank" example, what specifically goes wrong in pre-Transformer sequential models?**
Reading strictly left-to-right and compressing history into a rolling state, older models had **forgotten early context by the time an ambiguous word arrived** — they were bad at long-range dependencies, so disambiguating "bank" (river vs. financial) from distant context failed.

**Q2. In "The cat sat on the mat because it was tired," describe what attention computes when processing "it."**
"it" scores every other token for relevance: **cat → high weight**, mat → low, sat → low. The weights are normalized (sum to 1), and "it"'s updated representation blends the other tokens' information proportionally — resolving "it" = cat.

**Q3. Give the Q/K/V analogy and the one-sentence summary of the attention process.**
**Query** = the search query ("what am I looking for?"), **Key** = a document's title/tag ("what do I offer to match against?"), **Value** = the document's content ("what information do I carry?"). Summary: *"I (Query) search all others (Keys), score their relevance, then blend their information (Values) by those weights."*
*Memory hook: "Query searches, Key matches, Value delivers."*

**Q4. List the 5 steps of the attention computation.**
(1) Each token creates its own Q, K, V vectors → (2) a token's Query is compared against every other token's Key → (3) each comparison yields a relevance score → (4) scores normalize into weights summing to 1 → (5) output = the weighted sum of all Values.

**Q5. Why is it called *self*-attention, and what's the parallelism payoff?**
Tokens attend to other tokens **within the same sequence** (themselves collectively). All tokens do this **simultaneously**, not one-at-a-time like RNNs — which is exactly why Transformers train fast and parallelize on GPUs.

**Q6. What kinds of distinct relationships might different attention heads learn? Give four.**
Grammar/subject-verb agreement, pronoun references (it → cat), semantic word associations (river → water → bank), positional relationships (first word ↔ last word). After all heads run, outputs are **concatenated and combined** into one representation.
*Memory hook: "Multi-head = 8 experts reading the same sentence, comparing notes."*

**Q7. What would pure attention conclude about "Cat sat on mat" vs "Mat on sat cat," and what fixes it?**
They'd look identical — attention is order-blind (a set operation). **Positional encoding** fixes it: a position embedding is *added* to each token embedding before attention, so the model knows both *what* the token means and *where* it sits.
*Memory hook: "not good" ≠ "good not."*

---

## Transformer Architecture

**Q8. Map the three Transformer variants to purpose and examples.**
| Variant | Purpose | Examples |
|---|---|---|
| Encoder-only | Understanding text | BERT, sentence classifiers |
| Decoder-only | Generating text | GPT-4, Claude, Llama |
| Encoder-Decoder | Transform text (translate/summarize) | T5, BART |
Azure OpenAI GPT models = **decoder-only**.

**Q9. Recite the layer order of one decoder block.**
Token embeddings + positional encoding → **Masked multi-head self-attention** → Add & Normalize → **Feed-forward network** → Add & Normalize → output (richer token representations). That whole unit is one Transformer layer; GPT stacks dozens.

**Q10. Why must decoder attention be *masked*?**
During generation the model must only attend to **past** tokens — the future ones don't exist yet (and during training, peeking at them would be cheating on the next-token prediction task). Masking hides future positions.

**Q11. What does the FFN do that attention doesn't, and what may be stored there?**
Attention relates tokens **to each other**; the FFN does deeper **per-token** processing (two linear layers with a non-linearity between). Research suggests much of the model's stored factual "knowledge" lives in FFN weights, not attention.

**Q12. What's the division of labor across layer depth?**
Early layers capture surface structure (grammar, syntax); deeper layers capture meaning, facts, and reasoning. More stacked layers = more abstraction capacity — a key reason bigger/deeper models reason better (at higher cost).
*Memory hook: "Shallow = grammar, deep = reasoning."*

**Q13. Walk the 5-step generation loop for "What is Azure AI Search?"**
(1) **Tokenize** → token IDs; (2) **embed + add positions**; (3) pass through all Transformer layers (masked attention → add+norm → FFN → add+norm each); (4) the last token's final representation is projected to vocabulary size → **softmax** → probability over every possible next token → pick/sample one; (5) **append the new token and run the whole sequence again** — repeat until end-of-sequence or max length.

**Q14. What is streaming, mechanically, given the generation loop?**
Just the tokens being sent to the client as they're generated, one at a time — streaming isn't a different mode of model operation; it's exposing the token-by-token loop instead of buffering the full response.

---

## Key Numbers & Architect Relevance

**Q15. Fill in the classic GPT-3 architecture numbers.**
Parameters: **175B**; layers: **96**; attention heads: **96 per layer**. GPT-4's counts are undisclosed but larger; GPT-4o's context window is **128k tokens** (input + output combined).

**Q16. Connect three Transformer concepts to their direct architect implication.**
Context window limit → you must chunk documents for RAG (can't send a 1000-page PDF). Token-by-token generation → streaming UX is possible/natural. Layers = depth → more reasoning = higher cost (GPT-4 vs 3.5 pricing). (Also: decoder-only = built to generate; masked attention = can't peek ahead.)

**Q17. Correct these misconceptions: "attention = the model thinking" and "context window = memory."**
Attention is a **mathematical weighting mechanism**, not cognition — it scores and blends token representations. The context window is a **sliding window**, not persistent memory — nothing outside the current window exists for the model; persistence requires external memory architecture.

**Q18. Correct: "GPT memorizes facts" and "more parameters = always better."**
The model **compresses patterns** from training data — facts are statistically encoded, not stored verbatim, which is exactly why they can hallucinate. And parameter count alone doesn't determine quality — training-data quality (and matching data volume to size) matters equally.

---

## 2026 Updates

**Q19. Why is GPT-4o fast despite being large, per the MoE update?**
It's a **Mixture-of-Experts** model — only a subset of expert layers activates per token, so inference cost tracks the *active* parameters, not the total. Architect takeaway: total parameter count is no longer comparable across MoE and dense models.

**Q20. Compare current context windows across the big three model families.**
GPT-4o: **128k**; Claude (Sonnet-class): **200k**; Gemini 2.0: **1M**. And regardless of size, "lost in the middle" persists — critical content belongs at the start or end of the context.

**Q21. How do o1/o3 reasoning models differ from a standard forward pass?**
They generate **hidden chain-of-thought reasoning tokens internally before answering** — slower and costlier per query, but more accurate on complex multi-step tasks. It's a different usage profile, not just a bigger model.

**Q22. How do multimodal Transformers ingest images, per the update?**
A **ViT (Vision Transformer) encoder** converts image patches into embeddings that enter the same Transformer alongside text tokens — one architecture handling text, images, and audio natively (GPT-4o, Claude, Gemini).

**Q23. What distinguishes Claude's training approach in one line?**
Same decoder-only Transformer architecture as GPT, trained differently — **Constitutional AI + RLHF** (alignment via an explicit principle set plus human feedback).

---

## Applied (from the chapter's exercises, answered)

**Q24. Sketch a 128k token budget for a JMA dealer support agent.**
Example allocation: system prompt ~500 · RAG context (top-5 × 512-token chunks) ~2,560 · conversation history (last 10 turns) ~4,000–5,000 · user query ~50–100 · output reservation ~2,000 · safety buffer ~500 → total ~10k of 128k used. Lesson: a well-designed prompt uses a *fraction* of the window — filling it costs money and triggers lost-in-the-middle degradation.

**Q25. Model selection: match the four JMA tasks to GPT-4o / GPT-4o mini / o1-class.**
High-volume 5-category ticket classification (speed+cost) → **GPT-4o mini**. Complex multi-step contract analysis with legal reasoning → **o1/o3-class reasoning model**. 2-sentence call summary → **GPT-4o mini**. Multi-step math verification of an invoice total → **o1-class** (or 4o with tool use for arithmetic).

**Q26. MoE (GPT-4o) vs dense (GPT-4o mini): when does mini win despite lower capability?**
When the task is simple enough that mini's quality is equivalent — classification, short summaries, formatting — its lower per-token price and latency win outright. Capability advantages only matter on tasks that actually exercise them.

---

*Curriculum Q&A Batch B — file 3 of 4. Next: QA_L11_2 (Tokenization & Embeddings).*
