# Q&A — L11_2: LLMs — Tokenization & Embeddings
**Source chapter:** `01_Lessons/Part3_GenAI_LLMs/L11_2_LLMs_Tokenization_Embeddings.md` | **Format:** self-study
**Questions:** 30 | *No overlap with the interview bank or the chapter's own mini quiz — these drill the chapter's specifics.*

---

## Tokenization

**Q1. A token is not always a word — give five forms a token can take.**
A full word (`laptop`), part of a word (`recovered` → `recov`+`ered`), punctuation (`?`), a space+word combined (` my` as one token), a number split (`3pm` → `3`+`pm`).
*Memory hook: "Token ≠ word — always count tokens."*

**Q2. Why sub-word tokenization instead of whole-word vocabularies?**
Whole words would need millions of vocabulary entries (every word, language, conjugation, typo). Sub-words balance it: common words stay whole, rare words split into known parts, and even made-up words can be built from known pieces — with a vocabulary of only ~50k–100k tokens.

**Q3. Describe how BPE is trained, in three steps.**
Start with every character as its own token → repeatedly find the **most common adjacent pair** and merge it into a new token (`l`+`a`→`la` → `la`+`p`→`lap` → eventually `laptop` becomes whole because it's frequent) → after thousands of merges, the vocabulary is a mix of common whole words and sub-word pieces.

**Q4. How does GPT's BPE handle spaces, and what does the `_` convention mean in tokenization displays?**
GPT merges the leading space **into the next token** — ` laptop` is one token including its space, shown as `_laptop`.

**Q5. In the chapter's running example sentence, which two everyday strings tokenize as 2 tokens each — and why does that matter?**
`recovered` → `rec`+`overed` and `3pm` → `3`+`pm`. It matters because **billing is per token, not per word** — real token counts run higher than word counts, especially for uncommon words, numbers, and codes.

**Q6. How does SentencePiece differ from GPT's BPE on the same sentence?**
SentencePiece operates on the **raw byte stream with no pre-splitting on spaces**, marking word starts with `▁` (`▁laptop`, `▁recover ed`). GPT BPE pre-splits and merges spaces into tokens. SentencePiece is designed multilingual-first.

**Q7. Why does SentencePiece handle Japanese/Chinese better?**
Those languages have **no spaces between words** — tokenizers that rely on pre-splitting by whitespace fail. SentencePiece never pre-splits, so it handles unspaced scripts natively.

**Q8. What's WordPiece's merge criterion and its continuation marker?**
It merges pairs that **maximize training-data likelihood** (not raw frequency like BPE), and marks continuation sub-words with `##` — `recovered` → `recover` + `##ed`, meaning "continues the previous word, no space."

**Q9. Match tokenizer → models → marker.**
| Tokenizer | Used by | Marker |
|---|---|---|
| BPE | GPT-2/3/4, Claude | Space merged into token |
| SentencePiece | T5, LLaMA, Gemini, Mistral | `▁` word start |
| WordPiece | BERT, DistilBERT, Azure AI Language | `##` continuation |

**Q10. Why are token counts "not portable across models," and where does this bite in an Azure stack?**
Different tokenizers split the same text differently — 20 GPT tokens ≠ 20 BERT tokens. It bites when you use Azure AI Language (WordPiece-based) alongside Azure OpenAI (BPE) — never assume one service's token count predicts the other's limits or costs.

**Q11. Compare the vocabulary sizes: GPT's BPE vs BERT's WordPiece.**
GPT BPE: ~**100,000** tokens. BERT WordPiece: **30,522** tokens.

---

## Context Windows

**Q12. State the context-window definition precisely — what's the common misunderstanding?**
The maximum tokens the model can see at once **across input AND output combined** — not just the prompt. A 128k window with a 120k prompt leaves only ~8k for the response.
*Memory hook: "Context window = shared budget — system + history + RAG + output from one pot."*

**Q13. Write the context budget formula.**
Available for RAG content = Context Window − system message − chat history − user question − reserved output tokens − safety buffer. (Chapter example on 128k: minus 500+5,000+50+2,000+500 → ~120,950 available.)

**Q14. What's the pages-to-tokens rule of thumb?**
1 page ≈ 500 words ≈ **750 tokens**. So a 128k window ≈ ~96 pages; Claude's 200k ≈ ~150 pages.

**Q15. RAG retrieves 25 relevant documents totaling 200k tokens; the model window is 128k. What must the orchestrator do?**
It cannot send them all — it must **select** the top-N documents/chunks that fit the remaining budget (after system/history/question/output reservations), which is why retrieval ranking quality matters: the orchestrator's selection is only as good as the ranking.

**Q16. State the "lost in the middle" finding and its prompt-design consequence.**
LLMs use information at the **start and end** of a long context more reliably than the middle. Consequence: place the most critical retrieved document first or last in the RAG prompt — never buried in the middle.

**Q17. Do the chapter's cost math: 10k input tokens/query × 1,000 users/day at GPT-4o-mini input pricing.**
10,000,000 input tokens/day ≈ **$50/day at $0.005/1k tokens** — just for input. The lesson: chunking strategy and context-size discipline are **cost decisions**, not just quality decisions.

**Q18. Input vs output token pricing — which usually costs more?**
**Output** tokens are usually more expensive per token than input. Budget both separately.

**Q19. From the 2026 updates: what are the input/output splits for GPT-4o, Claude Sonnet-class, and Gemini 2.0 Flash?**
GPT-4o: 128k in / **16k out**; Claude Sonnet: 200k in / 8k out; Gemini 2.0 Flash: 1M in / 8k out. And the architect rule: **don't fill the window** — performance degrades approaching the limit.

---

## Embeddings

**Q20. Why do similar meanings end up with similar vectors — what does training exploit?**
Embedding training exploits context co-occurrence: `laptop` and `computer` appear in similar sentences → their vectors converge; `laptop` and `pizza` don't → vectors diverge. Result: **geometric distance ≈ semantic distance**.

**Q21. Word2Vec/GloVe vs contextual embeddings — what's the "bank" problem?**
Old word embeddings gave each word ONE fixed vector — "river bank" and "savings bank" got identical embeddings. Contextual embeddings (BERT, OpenAI) embed each **occurrence** based on its surrounding context, so the two "bank"s get different vectors.

**Q22. What do RAG systems actually embed — words, sentences, or something else?**
**Chunks** — an entire chunk of text becomes one vector representing its overall meaning; those chunk vectors are what's stored and searched in Azure AI Search.

**Q23. Give the embedding dimensions for ada-002, 3-small, 3-large, and BERT base.**
ada-002: 1,536 (fixed, legacy). 3-small: up to 1,536 (configurable down to 512). 3-large: up to **3,072** (configurable down to 256). BERT base: 768.

**Q24. Why cosine similarity over Euclidean distance — use the chapter's short-vs-long sentence example.**
Cosine measures the **angle** (direction) between vectors, ignoring magnitude. "laptop failure recovery" and a long sentence describing the same procedure differ in length/magnitude but point the same direction — cosine ≈ 0.94 ✅, while Euclidean distance would call them far apart due to length ❌.

**Q25. When is dot product equivalent to cosine similarity?**
When vectors are **normalized to length 1** — then dot product = cosine similarity. Azure AI Search supports both.

**Q26. What similarity threshold does the chapter suggest as a starting range, and what's the tuning trade-off?**
**~0.75–0.85** starting range. Too low → irrelevant results pass; too high → relevant matches missed. Always tune against real queries — "0.7 is good enough" is a misconception; it depends on your data.

**Q27. Contrast the indexing-time and query-time embedding flows.**
**Indexing (once/periodic):** document text → tokenize → embed chunk → store vector in the index. **Query (every request):** user query → tokenize → embed → cosine similarity against stored vectors → top-K chunks → injected into the LLM prompt → generate. Same embedding model on both sides — mandatory.

**Q28. Explain "embedding happens in two places" — and why they're not interchangeable.**
**Outside the LLM:** the Embeddings API converts text to vectors for search/retrieval — you control this. **Inside the LLM:** the first layer converts token IDs to vectors for the model's own processing — internal, task-specific, a black box. Different models, different weights, different purposes — the internal layer can't be used for retrieval and vice versa.

**Q29. Your colleague wants to switch the index from 3-large to 3-small to save cost — what's the full consequence chain?**
Different model = incompatible vectors AND different dimensions (3072 vs 1536) — the index's vector field dimension no longer matches, queries embedded with the new model can't be compared against old vectors, so it requires **re-embedding every document and re-indexing** (or a new index + cutover). Never a config-only change.

**Q30. What two tools does the chapter name for counting tokens before sending, and why bother?**
The **OpenAI Tokenizer** web tool (platform.openai.com/tokenizer) and the **tiktoken** Python library — count tokens ahead of API calls for cost estimation and context-budget planning. Also from the updates: cache embeddings for frequently-asked queries — embedding calls are cheap individually but add up at scale.

---

*Curriculum Q&A Batch B — file 4 of 4 (L09, L10, L11_1, L11_2 complete). Next batch: L11_3, L11_4, L12, L13.*
