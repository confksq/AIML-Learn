# Module 11.2 — Tokenization & Embeddings (Deep Dive)
**Part 3: Generative AI & LLMs | AI Solutions Architect Curriculum**

---

## What You Already Know (Recap)

From your previous sessions (May 4–17):
- Text is split into **tokens** before the model sees it
- Each token maps to a **token ID** (a number)
- Token IDs are converted into **embedding vectors** (meaning in numbers)
- Embeddings are also used **outside the LLM** for RAG search
- Embeddings capture **semantic similarity** (similar meaning = similar vector)

This chapter deepens all of that — and adds the two topics not yet covered:
- **SentencePiece & WordPiece** — alternative tokenizers used by BERT, T5, Gemini
- **Token limits and context windows** — the most practically important concept for architects

**Running example (used throughout):**
> *"My laptop crashed and I lost my report. Can I get it recovered before my meeting at 3pm?"*

---

## Part A — Tokenization (Deep Dive)

---

## 1. What is Tokenization?

**Tokenization = splitting raw text into smaller units (tokens) that the model can process.**

A token is not always a word. It can be:
- A full word: `laptop` → 1 token
- Part of a word: `recovered` → could be `recov` + `ered` (2 tokens)
- Punctuation: `?` → 1 token
- A space + word: ` my` → 1 token (space is included)
- A number: `3pm` → could be `3` + `pm` (2 tokens)

**Why split into sub-words instead of whole words?**

If you used whole words, your vocabulary would need millions of entries (every word in every language, every conjugation, every typo). That's unmanageable.

Sub-word tokenization finds a balance:
- Common words stay whole (`the`, `is`, `laptop`)
- Rare or complex words split into known parts (`tokenization` → `token` + `ization`)
- Unknown words are built from known pieces (even made-up words can be tokenized)

---

## 2. BPE — Byte Pair Encoding (GPT's approach)

**BPE = start with individual characters, then repeatedly merge the most common adjacent pairs.**

**How it's trained (simplified):**

```
Start: every character is its own token
  l  a  p  t  o  p  c  r  a  s  h  e  d ...

Step 1: Find most common adjacent pair → merge it
  'l' + 'a' → 'la' (if that's the most common pair)

Step 2: Repeat
  'la' + 'p' → 'lap'

Step 3: Repeat
  'lap' + 'top' → 'laptop'  ← full word because it appears very often
```

After thousands of merges, you end up with a vocabulary of ~50,000–100,000 tokens — a mix of common whole words and sub-word pieces.

**Applied to running example:**

```
"My laptop crashed and I lost my report. Can I get it recovered before my meeting at 3pm?"

Likely tokenization (GPT-style):
My | _laptop | _crashed | _and | _I | _lost | _my | _report | . 
Can | _I | _get | _it | _rec | overed | _before | _my | _meeting | _at | _3 | pm | ?

Total: ~23 tokens
```

*(The underscore `_` represents a leading space — GPT merges spaces into the next word)*

**Why this matters for architects:**
- `recovered` tokenizes as 2 tokens, not 1
- `3pm` tokenizes as 2 tokens: `3` and `pm`
- Your cost is based on token count, not word count

---

## 3. SentencePiece — Used by T5, LLaMA, Gemini

**SentencePiece = BPE (or unigram) applied directly to the raw byte stream, treating the sentence as a single stream without pre-tokenizing on spaces.**

Key difference from GPT's BPE:

| Feature | GPT BPE | SentencePiece |
|---|---|---|
| Treats spaces | Merges space into next token | Uses `▁` (underscore) to mark word starts |
| Works on | Pre-split words | Raw text (no pre-split) |
| Language handling | English-first | Designed for multilingual |
| Used by | GPT-2, GPT-3, GPT-4 | T5, LLaMA, Gemini, Mistral |

**SentencePiece example (same sentence):**

```
▁My ▁laptop ▁crashed ▁and ▁I ▁lost ▁my ▁report . 
▁Can ▁I ▁get ▁it ▁recover ed ▁before ▁my ▁meeting ▁at ▁3 pm ?
```

The `▁` marks where a new word starts in the original text.

**Why it's better for non-English:**
- Japanese/Chinese have no spaces between words — pre-splitting fails
- SentencePiece handles these languages natively

**As an architect:** You won't implement tokenizers. But knowing which tokenizer a model uses matters when you switch models or compare token counts across providers.

---

## 4. WordPiece — Used by BERT, Azure AI Language

**WordPiece = similar to BPE but uses a different merge criterion: it merges pairs that maximize the likelihood of the training data, not just frequency.**

Key characteristic: uses `##` prefix to mark continuation sub-words.

**WordPiece example:**

```
"recovered"  →  recover  ##ed

"tokenization"  →  token  ##ization

"3pm"  →  3  ##pm
```

The `##` means "this piece continues the previous word, no space."

| Tokenizer | Used by | Split marker |
|---|---|---|
| BPE | GPT-2, GPT-3, GPT-4, Claude | Space merged into token |
| SentencePiece | T5, LLaMA, Gemini, Mistral | `▁` marks word start |
| WordPiece | BERT, DistilBERT, Azure AI Language | `##` marks continuation |

**Practical implication:** If you call Azure AI Language Service (sentiment, NER, key phrases) alongside Azure OpenAI — they use different tokenizers. A piece of text might be 20 tokens in GPT but tokenizes differently in BERT. Token counts are **not portable across models**.

---

## 5. Token Limits and Context Windows ⭐ (Most Important for Architects)

This is the most operationally critical concept in this module.

### What is the Context Window?

**Context window = the maximum number of tokens the model can "see" at one time, across both input and output.**

```
┌─────────────────────────────────────────────────────┐
│              CONTEXT WINDOW  (e.g. 128,000 tokens)  │
│                                                     │
│  ┌──────────────────────────┐  ┌──────────────────┐ │
│  │     INPUT (prompt)       │  │  OUTPUT (response)│ │
│  │                          │  │                  │ │
│  │  System message: 200 tok │  │  Generated text  │ │
│  │  RAG docs: 10,000 tok    │  │  up to: X tokens │ │
│  │  Chat history: 5,000 tok │  │                  │ │
│  │  User question: 50 tok   │  │                  │ │
│  └──────────────────────────┘  └──────────────────┘ │
│                                                     │
│  Input + Output must fit within the total window    │
└─────────────────────────────────────────────────────┘
```

### Context Windows by Model

| Model | Context Window | Rough page equivalent |
|---|---|---|
| GPT-3.5 Turbo | 16,384 tokens | ~12 pages |
| GPT-4 | 8,192 tokens (base) / 128k (turbo) | ~6 / ~96 pages |
| GPT-4o | 128,000 tokens | ~96 pages |
| GPT-4o mini | 128,000 tokens | ~96 pages |
| Claude Sonnet | 200,000 tokens | ~150 pages |
| LLaMA 3 | 8,192–128k tokens | varies |

*Rule of thumb: 1 page ≈ 500 words ≈ 750 tokens*

### Why Context Window Matters for RAG

Your IT helpdesk assistant uses RAG. Here's the failure scenario:

```
User asks: "What is the laptop recovery policy?"

RAG retrieves: 25 relevant policy documents (total: 200,000 tokens)
You try to send all 25 to GPT-4 (128k window): ❌ EXCEEDS LIMIT

What actually happens:
- Your orchestrator must SELECT the top N documents
- Fit them inside the remaining context budget
- Leave room for the model's response
```

**Context budget formula:**
```
Available for RAG content = Context Window
                           - System message tokens
                           - Chat history tokens  
                           - User question tokens
                           - Reserved output tokens
                           - Safety buffer
```

**Example calculation for GPT-4o (128k):**
```
128,000 total
-    500 system message
-  5,000 chat history (last 10 turns)
-     50 user question
-  2,000 reserved for response
-    500 safety buffer
= 120,950 tokens available for RAG content
```

### The "Lost in the Middle" Problem

Research shows LLMs are **worse at using information in the middle** of a long context — they pay more attention to content at the start and end.

```
Start of context   ← model pays high attention
...
Middle of context  ← model pays LESS attention (risky!)
...
End of context     ← model pays high attention
```

**Architect implication:** Put the most critical retrieved document either first or last in your RAG prompt, not buried in the middle.

### Token Limits Affect Cost

Azure OpenAI charges **per token** (input + output separately):
- Input tokens: what you send
- Output tokens: what the model generates (usually more expensive)

```
A query that sends 10,000 tokens of RAG context × 1,000 users/day
= 10,000,000 input tokens/day
At $0.005/1k tokens (GPT-4o mini) = $50/day just for input
```

**This is why chunking strategy in RAG is a cost decision, not just a quality decision.**

---

## Part B — Embeddings (Deep Dive)

---

## 6. What Are Embeddings (Revisited)?

**Embedding = a list of numbers (a vector) that captures the meaning of a piece of text.**

You've seen this before. What's new here: understanding **why** they work and the difference between types.

A word like `laptop` becomes something like:
```
[0.23, -0.84, 0.12, 0.67, -0.31, ... ]   (typically 768–3072 numbers)
```

These numbers aren't random — they encode meaning. Words used in similar contexts end up with similar vectors.

### Why similar meaning = similar vector

During embedding model training, the model learns:
- `laptop` and `computer` appear in similar sentences → similar vectors
- `laptop` and `pizza` appear in very different sentences → different vectors

The result: **geometric distance in the vector space = semantic distance in meaning**.

---

## 7. Word vs Sentence Embeddings

### Word Embeddings (older — Word2Vec, GloVe)
- Each word gets a single fixed vector
- `bank` always has the same vector regardless of context
- Problem: "river bank" and "savings bank" get the same embedding

### Contextual Embeddings (modern — BERT, OpenAI)
- Each **occurrence** of a word gets a vector based on its full context
- `bank` in "river bank" → different vector than `bank` in "savings bank"
- This is what Azure OpenAI's embedding models produce

### Sentence / Chunk Embeddings (what RAG uses)
- Instead of one word, an entire chunk of text becomes one vector
- The vector represents the **overall meaning** of the chunk
- Used in RAG: each document chunk → one vector stored in Azure AI Search

**Applied to running example:**

```
Chunk 1 (IT policy doc):
"To recover a file lost due to hardware failure, submit a recovery 
request via the IT portal within 24 hours of the incident."

→ Embedding: [0.45, -0.23, 0.87, ...]  ← vector captures: recovery, hardware, IT, portal

User query:
"My laptop crashed and I lost my report. Can I get it recovered before my meeting at 3pm?"

→ Embedding: [0.41, -0.19, 0.83, ...]  ← similar vector!

Cosine similarity ≈ 0.92  →  HIGH MATCH  →  Chunk retrieved
```

---

## 8. Embedding Dimensions and Similarity

### Dimensions

**Dimension = how many numbers are in each vector.**

| Model | Dimensions | Use case |
|---|---|---|
| text-embedding-ada-002 | 1,536 | General purpose (legacy) |
| text-embedding-3-small | 512–1,536 (configurable) | Cost-efficient |
| text-embedding-3-large | 256–3,072 (configurable) | High accuracy |
| BERT base | 768 | Classification tasks |

More dimensions = more expressive = higher accuracy but more storage and compute cost.

**Azure AI Search vector field:** You set the dimension when you create the index. It must match the embedding model you use — you can't mix models.

### Similarity Measures

How do you compare two vectors to see if they're similar?

**Cosine Similarity (most common in RAG):**
```
Measures the angle between two vectors, not their magnitude.
Score: -1 (opposite) to +1 (identical)
Score > 0.85 = very similar meaning
Score < 0.50 = unrelated
```

**Why cosine, not Euclidean distance?**
Cosine similarity ignores vector length — only direction matters. A short sentence and a long sentence about the same topic will have similar cosine similarity even though their raw magnitudes differ.

```
"laptop failure recovery"        → [0.45, -0.23, 0.87, ...]
"procedure for recovering lost    
 files after laptop hardware      → [0.41, -0.19, 0.83, ...]
 failure"

Cosine similarity = 0.94  ✅  (same concept, different length)
Euclidean distance would see these as "far apart" due to length difference ❌
```

**Dot Product:** Used when vectors are normalized (length = 1). Equivalent to cosine similarity in that case. Azure AI Search supports both.

---

## 9. Using Embeddings for Semantic Search

This is where Part A and Part B connect — how tokenization and embeddings work together in your RAG pipeline.

### The Full RAG Embedding Flow

```
INDEXING TIME (runs once or periodically):

Document text
    ↓ Tokenize (split into tokens)
    ↓ Embed (tokens → full-chunk vector via embedding model)
    ↓ Store vector in Azure AI Search index

─────────────────────────────────────────────────────────

QUERY TIME (runs every user request):

User query text
    ↓ Tokenize
    ↓ Embed (query → vector)
    ↓ Cosine similarity search against all stored vectors
    ↓ Top K most similar chunks returned
    ↓ Inject into LLM prompt as context
    ↓ LLM generates response
```

### Hybrid Search (Keyword + Semantic)

Pure semantic search misses exact matches. Pure keyword search misses paraphrases.

**Hybrid = run both, combine scores:**

```
Query: "laptop recovery before 3pm"

Keyword search (BM25):
  Match 1: "laptop recovery procedure" (score: 0.85)  ← exact keyword match
  Match 2: "file restoration process" (score: 0.20)   ← keyword miss

Semantic search (vector):
  Match 1: "laptop recovery procedure" (score: 0.92)
  Match 2: "file restoration process" (score: 0.87)   ← semantic hit!

Hybrid (RRF fusion):
  Match 1: "laptop recovery procedure" (top in both)
  Match 2: "file restoration process" (semantic hit rescued it)
```

Azure AI Search's **Reciprocal Rank Fusion (RRF)** merges both rankings without needing to tune weights.

---

## 10. Embedding Happens in Two Places (Clarification)

You've seen this before but it's worth cementing with a complete diagram:

```
YOUR APP (orchestrator)                    AZURE OPENAI (LLM)
─────────────────────────────────          ─────────────────────────────────
Indexing:                                  Processing input:
  Document text                              Prompt text
    ↓                                          ↓
  Embedding API call ←── EXTERNAL            [Tokenizer]
  (text-embedding-3)                            ↓
    ↓                                          [Embedding layer] ←── INTERNAL
  Vector stored in                               ↓
  Azure AI Search                            [Attention layers]
                                                 ↓
Querying:                                    [FFN layers]
  User query                                     ↓
    ↓                                        [Output: next token]
  Embedding API call ←── EXTERNAL
    ↓
  Similarity search
    ↓
  Top K chunks → into prompt
```

| Embedding location | Why it runs | Who controls it |
|---|---|---|
| **Outside LLM** (Embeddings API) | To convert text to vectors for search | You (the architect) |
| **Inside LLM** (first layer) | To convert token IDs to vectors for processing | The model, internally |

Same word. Different purpose. Different model weights. Not interchangeable.

---

## 11. Why This Matters for You as an Architect

| Concept | Architect implication |
|---|---|
| **Token count ≠ word count** | Estimate tokens accurately for cost planning. Use tiktoken library or Azure's token counter |
| **Different tokenizers per model** | Don't assume GPT token count matches BERT. Test with the actual model |
| **Context window is shared** | System prompt + history + RAG docs + response all share the budget. Design your prompt template with this in mind |
| **Lost in the middle** | Put critical context at the start or end of RAG-injected content, not buried in the middle |
| **Embedding model = index schema** | Once you index with text-embedding-3-large (3072 dims), you can't switch to ada-002 (1536 dims) without re-indexing |
| **Cosine similarity threshold** | You need to tune this. Too low = irrelevant results. Too high = miss relevant matches. ~0.75–0.85 is a common starting range |
| **Hybrid search** | Almost always better than pure semantic for enterprise RAG. Use it by default in Azure AI Search |
| **Cost of embeddings** | Embedding API calls are cheap but add up at scale. Cache embeddings for frequently asked queries |

---

## 12. Numbers to Know

| Fact | Value |
|---|---|
| GPT-4o context window | 128,000 tokens |
| Rule of thumb | 1 page ≈ 750 tokens |
| text-embedding-3-large dimensions | Up to 3,072 |
| text-embedding-3-small dimensions | Up to 1,536 |
| Cosine similarity range | -1 to +1 |
| Good similarity threshold (starting point) | 0.75–0.85 |
| GPT vocabulary size (BPE) | ~100,000 tokens |
| BERT vocabulary size (WordPiece) | 30,522 tokens |

---

## 13. Common Misconceptions

| Misconception | Reality |
|---|---|
| "Token = word" | A word can be 1–4+ tokens. Always count tokens, not words |
| "Bigger context window = always use it all" | Filling the window increases cost and triggers lost-in-the-middle problem |
| "The embedding model inside the LLM is the same as the Embeddings API" | No — the internal LLM embedding layer is task-specific; the Embeddings API uses a separate model designed for retrieval |
| "Switching embedding models is easy" | You must re-embed and re-index all documents if you change the model |
| "Cosine similarity of 0.7 is good enough" | Depends on your data — always evaluate with real queries |
| "All tokenizers are the same" | BPE, WordPiece, SentencePiece behave differently — token counts will differ across models |

---

## 14. Mini Quiz (Test Yourself)

1. Your RAG app uses GPT-4o (128k context). Your system prompt is 300 tokens, chat history is 4,000 tokens, and each retrieved chunk is 500 tokens. How many chunks can you fit if you reserve 2,000 tokens for the response?
2. Why does `recovered` tokenize as 2 tokens instead of 1?
3. What is the key difference between BPE and WordPiece?
4. Why is cosine similarity preferred over Euclidean distance for embedding comparison?
5. You indexed documents using `text-embedding-3-large`. A colleague wants to switch to `text-embedding-3-small` to save cost. What's the consequence?
6. What is the "lost in the middle" problem and how do you mitigate it in your RAG prompt design?

*(Ask these in your Claude Code window for discussion)*

---

## Memory Hooks

- **Token ≠ word** — `recovered` = 2 tokens, `3pm` = 2 tokens, always count tokens
- **Context window = shared budget** — system + history + RAG + output all come from the same pot
- **BPE** = GPT's way (space merged in), **WordPiece** = BERT's way (`##` continuation), **SentencePiece** = LLaMA/Gemini's way (`▁` word start)
- **Embeddings outside LLM** = for search and retrieval (you control this)
- **Embeddings inside LLM** = for model processing (black box, not your concern)
- **Cosine similarity** = angle between vectors = semantic closeness
- **Hybrid search** = keyword + semantic, almost always better than either alone
- **Lost in the middle** = put the most important RAG content first or last

---

## What Comes Next (Module 11.3)

**11.3 — Pre-training and Fine-tuning**
- How GPT learned from the internet (next-token prediction at scale)
- Why models have a knowledge cutoff date
- What transfer learning means for your architecture decisions
- Fine-tuning vs RAG vs prompt engineering — the decision framework
- LoRA and QLoRA — what they are and when Azure uses them

---
---

## 2026 Updates

| Topic | Update |
|---|---|
| **Matryoshka embeddings** | text-embedding-3-large now supports Matryoshka Representation Learning (MRL) — you can request smaller dimensions (256, 512, 1536) from the same model. 256-dim = 6x cheaper storage, ~3% accuracy loss vs full 1536-dim |
| **Structured token limits** | GPT-4o: 128k input, 16k output. Claude Sonnet 4.6: 200k input, 8k output. Gemini 2.0 Flash: 1M input, 8k output. Architect rule: don't fill the context window — performance degrades as you approach limits |
| **Embedding model versions** | text-embedding-3-small and text-embedding-3-large are current (released early 2024, still current). text-embedding-ada-002 is legacy — don't use for new projects. Cannot mix embedding models in same index |
| **Sparse + dense hybrid** | Azure AI Search now natively supports hybrid retrieval with RRF combining BM25 (sparse) + HNSW vector (dense). No need to implement RRF yourself — built into Search API |
| **Tokenizer tools** | OpenAI Tokenizer (platform.openai.com/tokenizer) and tiktoken (Python library) let you count tokens before sending to API. Essential for budget planning |

---

## Interactive Learning Ideas

### Exercise 1 — Tokenizer Hands-On (10 min)
Go to platform.openai.com/tokenizer:
- Paste a JMA dealer support ticket
- Count the tokens
- Now paste your typical system prompt
- Calculate: if you do 1,000 calls/day with this prompt, what's the monthly token cost at GPT-4o pricing?

### Exercise 2 — Embedding Dimension Trade-off
Using Azure OpenAI text-embedding-3-large:
- Embed 10 JMA-relevant sentences at 1536 dimensions
- Embed the same 10 sentences at 256 dimensions (add `dimensions: 256` to API call)
- Run cosine similarity comparison queries on both
- Is the accuracy difference noticeable for your use case?

### Exercise 3 — Cosine Similarity Calculator (15 min)
Write a C# method `float CosineSimilarity(float[] a, float[] b)`.
Test it: embed "F-150 delivery delay" and "truck shipment late" — should be high similarity.
Then embed "F-150 delivery delay" and "dental insurance claim" — should be low.
What threshold would you use to decide "these are related"?

### Exercise 4 — Token Budget Audit
Take the JMA EnterpriseSearch RAG pipeline prompt. Run it through the tokenizer:
- How many tokens does the system prompt use?
- How many tokens does a typical retrieved context use (top-3 chunks)?
- How much room is left for conversation history?
- What would you cut if you needed to reduce cost by 30%?

---

*File: Part3_Module11_2_Tokenization_Embeddings.md | AI Solutions Architect Curriculum*
*Updated: 2026-06-30*

---
---

## 15. Setting `max_tokens` & Token-Saving Techniques — Cost Control (added 2026-08-01)

This module teaches what tokens *are* and how the context window is *budgeted*. It stops short of
the operational question: once you know your budget, how do you actually control spend? That's
covered elsewhere in the library — this section is the cross-reference, not a duplicate.

### Setting `max_tokens`

An API request parameter that caps the **response length only** — it does not cap input, and it is
not itself a savings technique, just a safety ceiling.

```python
response = client.chat.completions.create(
    model="gpt-4o",
    messages=messages,
    max_tokens=800   # response is cut off here, whatever it costs to that point
)
```

- Recommended range for typical use: **500–1000**
- `finish_reason == "length"` in the response means you hit the cap mid-answer — raise `max_tokens`
  or shrink the input side of the budget (§5 above) to leave more room
- **o1/o3 reasoning models use a different parameter name** — `max_completion_tokens`, not
  `max_tokens` — and drop `temperature` entirely
- Full detail: `L12_AzureOpenAI_Services.md` (~lines 148, 165, 205, 601, 695, 699)

### Token-saving techniques (ranked by return, not alphabetically)

| Technique | Typical savings | Where taught |
|---|---|---|
| **Model tiering/routing** — cheap model handles easy cases, escalate to GPT-4o only when needed | Biggest single lever | `L36_LLM_Observability_FinOps.md` §229–260, §351–355 · `HLP01` §2 |
| **Semantic caching** — skip the LLM call for near-duplicate queries | 20–60% | `L13_RAG_DeepDive.md`:1324 · `L36`:295–320 |
| **Prompt caching** — Azure OpenAI caches repeated system-prompt prefixes | ~50–90% off the cached portion | `L15_PromptEngineering.md`:742 |
| **Context/RAG trimming** — reduce top-K, truncate chunks, summarize history | 20–40% | `L13`:868–897 (the context-budget formula in §5 above, applied) |
| **Prompt compression** — trim a bloated system prompt | 10–30% | `L15`:596–619 (worked example: 65→30 tokens, 54% cut) |
| **Batch API** — async batch completions for non-realtime jobs | Flat 50% | `L12`:699 |
| **Cap `max_tokens` / agent iteration limits** | 1.5–2× | `L36`:384 |

### The decision framework — which lever first

Two ranked tables in the library answer "which technique do I reach for first," and both agree:
**don't start with prompt wordsmithing — it's the weakest lever for the effort.**

- `L36` §342–348 ("levers in order of return"): semantic caching → model tiering → prompt
  compression → cap iterations → trim RAG context → batch/provisioned throughput → self-host
- `HLP01_Memory_Tokens_Scaling_Agents.md` §2 (ranked by magnitude): model choice (~17×) > top-K
  reduction (2–5×) > caching (2–10×) > prompt caching (~90% off cached prefix) > memory strategy
  (2–3×) > output constraints incl. `max_tokens` cap (1.5–2×) > prompt wordsmithing (~5–10%, **last**)

**Rule of thumb both converge on:** route by task complexity before touching the prompt — ask
"does this need GPT-4o at all?" (tiering), then "have we seen this before?" (caching), then "are we
sending more context than needed?" (trimming), and only then micro-optimize wording.

This connects §5's context-budget formula (line 205 above) to the FinOps decision layer — the
budget formula tells you *how much room you have*; this section tells you *what to cut first* when
you're over.

> ⚠️ **Do not insert further content between here and line 578 above.** `00_INDEX.md` carries ~18
> line-number citations into this file (e.g. `L11_2:401`, `L11_2:157`), all at or below line 538.
> Any edit inside the original body shifts those citations silently. Append new material after this
> point only, or regenerate the index.
