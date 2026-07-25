# Part 1: Artificial Intelligence (Applied AI / LLMs)

**Teaching approach:** Feynman (analogy first) → How it works (internals) → PRIMM Code (Predict → Run → Modify → Make) → Interview Q&A

**Your level:** Intermediate | **Goal:** Interview-ready + Build real systems

---

## 1.1 Large Language Models – Architectures

### Analogy

Imagine a librarian who has read **every book, article, and website ever written**. When you ask them a question, they don't look up the answer — they *synthesize* from everything they've read and predict the most likely useful response word by word.

The **architecture** is how that librarian's brain is wired. Different architectures wire the brain differently:
- **GPT (decoder-only)** → Reads left to right, always predicting the next word. Like writing a story where each word depends on all previous words.
- **BERT (encoder-only)** → Reads the whole sentence at once, fills in blanks. Like a proofreader who sees the full context.
- **T5 (encoder-decoder)** → Reads input fully, then generates output. Like a translator who reads the full French sentence before writing English.

---

### How It Works Internally

#### Step 1 — Tokenization
Before any model sees text, it converts words into **tokens** (numbers). Tokens are not always full words.

```
"ChatGPT is amazing" → [9693, 402, 2898, 374, 8056] (example token IDs)
```

- **BPE (Byte Pair Encoding)** — GPT models. Merges frequent character pairs iteratively.
- **WordPiece** — BERT. Splits rare words into subwords (`playing` → `play` + `##ing`).
- **SentencePiece** — Language-agnostic, works on raw bytes (Llama, T5).

> **Why this matters:** Token count = cost + context limit. "ChatGPT" may be 1 token, but "antidisestablishmentarianism" is 6.

#### Step 2 — Embeddings
Each token ID is mapped to a **high-dimensional vector** (e.g., 4096 dimensions in GPT-4). Similar meanings → similar vectors.

```
"king" - "man" + "woman" ≈ "queen"  (classic word2vec demo, same idea)
```

#### Step 3 — Self-Attention (The Core Idea)
This is what makes Transformers powerful. Every token looks at every other token and decides: *"how much should I pay attention to you?"*

Think of it like a **meeting room**: every person (token) can talk to every other person simultaneously, and the most relevant conversations get amplified.

Mathematically:
```
Attention(Q, K, V) = softmax(QKᵀ / √d_k) × V
```
- **Q (Query)** = "What am I looking for?"
- **K (Key)** = "What do I contain?"
- **V (Value)** = "What do I actually give you?"

#### Step 4 — Multi-Head Attention
Instead of one attention pass, the model runs **N parallel attention heads** — each head learns to focus on different relationships (syntax, semantics, coreference, etc.).

#### Step 5 — Feed-Forward + Add & Norm
After attention, each token goes through a small neural network independently, then results are normalized and residual-connected (skip connections, like ResNet).

#### Step 6 — Causal Masking (GPT-style only)
Decoder-only models mask future tokens during training so the model can only see past tokens — forcing it to learn to predict the next one.

#### GPT vs BERT vs T5 at a glance

| Property | GPT (decoder) | BERT (encoder) | T5 (enc-dec) |
|-----------|--------------|----------------|--------------|
| Direction | Left → Right | Bidirectional | Both |
| Task | Text generation | Classification, NER | Translation, summarization |
| Training objective | Next token prediction | Masked token prediction | Text-to-text |
| Examples | GPT-4, Llama, Mistral | BERT, RoBERTa, DistilBERT | T5, FLAN-T5, mT5 |

#### Context Window
The maximum number of tokens the model can "see" at once. GPT-4 Turbo = 128K tokens (~300 pages). Beyond this, the model forgets earlier content.

Extended via: **RoPE** (Rotary Position Embedding, used in Llama), **ALiBi** (linear bias on attention scores), **sliding window attention** (Mistral).

#### Mixture of Experts (MoE)
Instead of all parameters activating for every token, MoE routes each token to only a **subset of expert sub-networks** (e.g., 2 out of 8 experts). This gives a large model capacity at lower compute cost.

> GPT-4 and Mixtral 8x7B use MoE. The "8x7B" means 8 experts of 7B params each, but only 2 activate per token → effectively 13B active params.

---

### Code — PRIMM Exercise

#### PREDICT first: What do you think this code outputs?

```python
from openai import OpenAI

client = OpenAI(api_key="your-api-key")

response = client.chat.completions.create(
    model="gpt-4o",
    messages=[
        {"role": "system", "content": "You are a helpful assistant."},
        {"role": "user", "content": "What is the capital of France?"}
    ],
    max_tokens=50,
    temperature=0.0
)

print(response.choices[0].message.content)
print(f"Tokens used: {response.usage.total_tokens}")
```

> **Predict:** What will `total_tokens` roughly be? (Hint: count the words in input + output)

#### RUN it → then look at the token count. Was your prediction close?

#### Now see tokenization directly:

```python
import tiktoken

enc = tiktoken.encoding_for_model("gpt-4o")

text = "Large Language Models are transforming AI applications in 2025."
tokens = enc.encode(text)

print(f"Text: {text}")
print(f"Token IDs: {tokens}")
print(f"Token count: {len(tokens)}")
print(f"Tokens decoded: {[enc.decode([t]) for t in tokens]}")
```

#### MODIFY — Try these one at a time and observe:
1. Change the text to a very technical word like `"antidisestablishmentarianism"` — how many tokens?
2. Try text in another language (e.g., Chinese) — more or fewer tokens than English?
3. Change `temperature=0.0` to `temperature=1.5` in the first snippet — what changes in the response?

#### MAKE — Your turn:
Build a small function that takes any string and returns: token count, estimated cost at $0.005/1K tokens, and a warning if token count exceeds 1000.

```python
def token_audit(text: str, model: str = "gpt-4o") -> dict:
    # your code here
    pass
```

---

### Interview Q&A

**Q1: What is the difference between encoder-only, decoder-only, and encoder-decoder models?**

> Encoder-only (BERT) reads bidirectionally — ideal for classification and NER where full context matters. Decoder-only (GPT) generates text autoregressively left-to-right — ideal for generation tasks. Encoder-decoder (T5) encodes input fully then decodes output — ideal for seq2seq tasks like translation or summarization.

**Q2: Why does GPT use causal masking during training?**

> Without masking, the model could "cheat" by looking at future tokens when predicting the next one. Causal masking ensures the model only sees past tokens, forcing it to learn genuine next-token prediction — which makes it generative at inference time.

**Q3: What is temperature and how does it affect generation?**

> Temperature scales the logits before softmax. `temperature=0` → deterministic (always picks highest probability token). `temperature=1` → standard sampling. `temperature>1` → more random/creative. Internally: `logits = logits / temperature` before softmax.

**Q4: What is Mixture of Experts and why does it matter?**

> MoE routes each input token to a subset of specialized sub-networks ("experts") instead of activating all parameters. This decouples model capacity from compute — you get a large effective model with lower inference cost. Trade-off: requires more memory to store all experts even though only a few activate per token.

**Q5: Why does tokenization matter for cost and performance?**

> LLM APIs charge per token. Non-English languages, code, and rare words often tokenize into more tokens than plain English — the same meaning costs more. Context limits are also token-based, so inefficient tokenization = less space for useful content.

---

## 1.2 Prompt Engineering

### Analogy

Think of prompting like **briefing a very smart but extremely literal contractor**. If you say "build me a house," you'll get something — but probably not what you wanted. If you say "build a 3-bedroom house, brick exterior, open-plan kitchen, under $300K, here are 2 examples of what I like," you'll get something much closer to your vision.

The LLM is the contractor. The prompt is the brief. Better brief = better output.

---

### How It Works Internally

#### System vs User vs Assistant Roles

```
System   → Sets the persona, rules, and constraints. The LLM treats this as its "identity."
User     → The human's input each turn.
Assistant → The model's previous responses (used in multi-turn conversations).
```

The model sees all three as a concatenated token sequence. The "role" labels are just formatting conventions the model was trained to respect.

#### Zero-shot vs Few-shot vs Many-shot

| Type | What it means | When to use |
|------|--------------|-------------|
| Zero-shot | No examples given | Simple, well-understood tasks |
| Few-shot | 2–5 examples in the prompt | When output format matters |
| Many-shot | 10–100+ examples | Complex or domain-specific tasks |

**Why few-shot works:** Examples shift the probability distribution of the model's outputs toward the demonstrated pattern. It's in-context learning — no weights change.

#### Chain-of-Thought (CoT)
Adding `"Think step by step"` or showing reasoning steps in examples dramatically improves performance on multi-step reasoning tasks.

**Why it works:** Forces the model to generate intermediate reasoning tokens, which act as working memory and steer subsequent tokens toward correct conclusions.

```
Without CoT: "Roger has 5 balls. He buys 2 more cans of 3 balls each. How many?" → "11" (wrong)
With CoT:    "Roger starts with 5. 2 cans × 3 = 6. 5 + 6 = 11" → "11" (right, same model)
```

#### Tree-of-Thoughts (ToT)
Extends CoT by exploring **multiple reasoning paths** simultaneously and selecting the best one. Think of it as the model playing chess — evaluating several moves ahead before committing.

#### Self-Consistency
Run the same CoT prompt **multiple times** with `temperature > 0`, then take the **majority vote** across answers. Improves accuracy without changing the model.

#### Output Formatting
- **JSON mode** — forces structured output (OpenAI: `response_format={"type": "json_object"}`)
- **Logit bias** — increase/decrease probability of specific token IDs
- **Regex / grammar constraints** — tools like `outlines` or `guidance` constrain generation to valid formats

---

### Code — PRIMM Exercise

#### PREDICT: Which prompt will give a better structured response?

```python
from openai import OpenAI
import json

client = OpenAI(api_key="your-api-key")

# Prompt A - Zero-shot, vague
prompt_a = "Tell me about Python."

# Prompt B - Few-shot with format
prompt_b = """Extract the programming language info as JSON.

Example 1:
Input: "Java was created by James Gosling at Sun Microsystems in 1995."
Output: {"language": "Java", "creator": "James Gosling", "year": 1995, "company": "Sun Microsystems"}

Example 2:
Input: "Ruby was designed by Yukihiro Matsumoto and released in 1995."
Output: {"language": "Ruby", "creator": "Yukihiro Matsumoto", "year": 1995, "company": null}

Now extract:
Input: "Python was created by Guido van Rossum at CWI in 1991."
Output:"""

def call_llm(prompt, system="You are a helpful assistant."):
    response = client.chat.completions.create(
        model="gpt-4o",
        messages=[
            {"role": "system", "content": system},
            {"role": "user", "content": prompt}
        ],
        temperature=0.0
    )
    return response.choices[0].message.content

print("=== Prompt A ===")
print(call_llm(prompt_a))

print("\n=== Prompt B ===")
print(call_llm(prompt_b))
```

#### Chain-of-Thought example:

```python
# Without CoT
no_cot = """
A store has 48 apples. They sell 1/4 in the morning and 1/3 of the remainder in the afternoon.
How many apples are left?
Answer with just a number.
"""

# With CoT
with_cot = """
A store has 48 apples. They sell 1/4 in the morning and 1/3 of the remainder in the afternoon.
How many apples are left?
Think step by step, then give the final answer.
"""

print("Without CoT:", call_llm(no_cot))
print("With CoT:", call_llm(with_cot))
```

#### JSON mode (structured output):

```python
response = client.chat.completions.create(
    model="gpt-4o",
    messages=[
        {"role": "system", "content": "Extract entities and return valid JSON only."},
        {"role": "user", "content": "Elon Musk founded SpaceX in 2002 in Hawthorne, California."}
    ],
    response_format={"type": "json_object"},
    temperature=0.0
)

data = json.loads(response.choices[0].message.content)
print(data)
```

#### MODIFY — Try these:
1. Add a third few-shot example to Prompt B with a different language — does output quality improve?
2. In the CoT example, replace `"Think step by step"` with `"Think carefully"` — any difference?
3. Try `temperature=1.0` on the JSON mode call — does it still return valid JSON?

#### MAKE — Build a reusable prompt builder:

```python
def build_few_shot_prompt(task_description: str, examples: list[dict], new_input: str) -> str:
    """
    examples: [{"input": "...", "output": "..."}, ...]
    Returns a formatted few-shot prompt string.
    """
    # your code here
    pass
```

---

### Interview Q&A

**Q1: What is the difference between zero-shot and few-shot prompting?**

> Zero-shot gives the model no examples — it relies entirely on pre-trained knowledge. Few-shot provides 2–5 input/output demonstrations in the prompt, shifting the model's output distribution toward the shown pattern through in-context learning. No weights are updated in either case.

**Q2: Why does Chain-of-Thought improve reasoning?**

> CoT forces the model to produce intermediate reasoning tokens before the final answer. These tokens serve as working memory — each step conditions subsequent tokens, reducing the probability of logical errors that arise from jumping directly to the answer in one step.

**Q3: What is the risk of few-shot prompting?**

> The model can become over-anchored to the format of examples and fail on edge cases not represented. Also, examples consume context window tokens — many-shot prompting can crowd out the actual content you want to process.

**Q4: When would you use self-consistency over a single CoT call?**

> When accuracy matters more than cost/latency. Self-consistency samples multiple reasoning paths (e.g., 10 runs at temperature=0.7) and majority-votes the answer. It's particularly effective for math and logic problems where there's a definitive correct answer.

**Q5: What is logit bias and when would you use it?**

> Logit bias adjusts the raw scores (logits) for specific token IDs before sampling, effectively banning or boosting certain tokens. Use case: force the model to respond only "Yes" or "No" by heavily penalizing all other tokens, without needing JSON mode or post-processing.

---

## 1.3 Retrieval-Augmented Generation (RAG)

### Analogy

Imagine you're taking an **open-book exam**. You (the LLM) are the student — smart, but your memory is frozen at training time. The vector database is your textbook. RAG is the process of:
1. Reading the exam question
2. Quickly finding the relevant pages in your textbook (retrieval)
3. Writing your answer based on both your knowledge and what you just read (generation)

Without RAG: the student answers purely from memory — risks hallucination for anything post-training or domain-specific.
With RAG: the student always has the right reference material in front of them.

---

### How It Works Internally

#### Full Pipeline

```
INDEXING (done once, offline):
  Raw documents
    → Chunking (split into pieces)
    → Embedding (convert to vectors)
    → Store in Vector DB

QUERYING (done at runtime, per query):
  User question
    → Embed question
    → Vector search (find similar chunks)
    → Rerank (optional)
    → Stuff chunks into LLM prompt
    → LLM generates grounded answer
```

#### Stage 1 — Chunking
Splitting documents into pieces the model can fit in its context and that contain coherent information.

| Strategy | How | Best for |
|----------|-----|----------|
| Fixed-size | Split every N tokens, overlap M tokens | Simple, works well for most docs |
| Recursive | Split on `\n\n`, then `\n`, then ` ` | Structured text, code |
| Semantic | Split at topic boundaries (embedding similarity drops) | Long mixed-topic documents |
| Paragraph | Split on double newlines | Articles, reports |

**Overlap** (e.g., 20% overlap between chunks) prevents context from being cut off at chunk boundaries.

#### Stage 2 — Embedding
Converting text chunks into dense vectors that capture semantic meaning.

- `text-embedding-3-small` (OpenAI) — 1536 dims, cheap, good
- `text-embedding-3-large` (OpenAI) — 3072 dims, better accuracy
- `BAAI/bge-large-en-v1.5` — open-source, strong performer
- `Cohere embed-v3` — multilingual, strong

**Why vectors?** Vectors let you find *semantically similar* content, not just keyword matches. "Car" and "automobile" have high cosine similarity even though they share no characters.

#### Stage 3 — Vector Database
Stores vectors and enables fast approximate nearest-neighbor (ANN) search.

| DB | Notes |
|----|-------|
| FAISS | Facebook, in-memory, no server needed — great for prototypes |
| Chroma | Local or server, simple API, good for dev |
| Qdrant | Production-ready, filters, hybrid search |
| Pinecone | Managed, serverless, production |
| Azure AI Search | Managed, hybrid (BM25 + vector), enterprise |

**HNSW** (Hierarchical Navigable Small World) — the graph-based index algorithm most vector DBs use. Trades tiny accuracy loss for massive speed gain over brute-force search.

#### Stage 4 — Hybrid Search
Pure vector search misses exact keyword matches. **Hybrid search** combines:
- **BM25** (keyword/sparse) — great for exact terms, product codes, names
- **Vector** (dense) — great for semantic meaning

Combine with **Reciprocal Rank Fusion (RRF)** to merge the two ranked lists.

#### Stage 5 — Reranking
First-stage retrieval (ANN) is fast but imprecise. A **cross-encoder reranker** takes each (query, chunk) pair and scores true relevance — much more accurate than vector similarity alone.

- `Cohere rerank-english-v3.0`
- `BAAI/bge-reranker-large` (open-source)

> Typical pattern: retrieve top 50 via ANN → rerank → keep top 5 → send to LLM.

#### Stage 6 — Generation with Grounding
The retrieved chunks are injected into the LLM prompt as context:

```
System: Answer only using the provided context. If the answer is not in the context, say "I don't know."

Context:
[Chunk 1 text]
[Chunk 2 text]
[Chunk 3 text]

Question: {user_question}
```

#### Advanced RAG Patterns

| Pattern | What it does |
|---------|-------------|
| **Self-RAG** | Model decides *when* to retrieve (not every query needs retrieval) |
| **CRAG (Corrective RAG)** | Evaluates retrieved docs; if low quality, falls back to web search |
| **Adaptive RAG** | Routes query to different retrieval strategies based on query type |
| **Multi-hop RAG** | Iteratively retrieves — answer from step 1 informs step 2 query |

---

### Code — PRIMM Exercise

#### PREDICT: What will the retrieved chunks be for the query "What is the refund policy?"

```python
from openai import OpenAI
import numpy as np
import faiss

client = OpenAI(api_key="your-api-key")

# --- Sample documents (imagine these came from a PDF or website) ---
documents = [
    "Our return policy allows returns within 30 days of purchase with original receipt.",
    "Refunds are processed within 5-7 business days to the original payment method.",
    "We offer free shipping on orders over $50 across the continental United States.",
    "Customer support is available Monday through Friday, 9 AM to 6 PM EST.",
    "All electronics must be returned in original packaging to qualify for a refund.",
    "Gift cards are non-refundable and cannot be exchanged for cash.",
]

# --- Step 1: Embed all documents ---
def embed(texts: list[str]) -> np.ndarray:
    response = client.embeddings.create(
        model="text-embedding-3-small",
        input=texts
    )
    return np.array([r.embedding for r in response.data], dtype="float32")

doc_embeddings = embed(documents)

# --- Step 2: Build FAISS index ---
dim = doc_embeddings.shape[1]  # 1536
index = faiss.IndexFlatIP(dim)  # Inner product = cosine similarity (if normalized)
faiss.normalize_L2(doc_embeddings)
index.add(doc_embeddings)

# --- Step 3: Retrieve ---
def retrieve(query: str, top_k: int = 3) -> list[str]:
    q_emb = embed([query])
    faiss.normalize_L2(q_emb)
    scores, indices = index.search(q_emb, top_k)
    return [documents[i] for i in indices[0]]

# --- Step 4: Generate ---
def rag_answer(question: str) -> str:
    chunks = retrieve(question, top_k=3)
    context = "\n".join(f"- {c}" for c in chunks)

    response = client.chat.completions.create(
        model="gpt-4o",
        messages=[
            {
                "role": "system",
                "content": "Answer only using the provided context. If unsure, say 'I don't know'."
            },
            {
                "role": "user",
                "content": f"Context:\n{context}\n\nQuestion: {question}"
            }
        ],
        temperature=0.0
    )
    return response.choices[0].message.content

# --- Test it ---
question = "What is the refund policy?"
print("Retrieved chunks:")
for c in retrieve(question):
    print(f"  → {c}")
print("\nAnswer:", rag_answer(question))
```

#### MODIFY — Try these:
1. Ask `"Can I return a TV without the box?"` — does it retrieve the right chunks?
2. Add a new document: `"Laptops and tablets have a 15-day return window instead of 30 days."` — rebuild the index and ask about laptops.
3. Change `top_k=3` to `top_k=1` — does the answer quality drop?

#### MAKE — Add a chunking function:

```python
def chunk_text(text: str, chunk_size: int = 200, overlap: int = 50) -> list[str]:
    """
    Split text into overlapping chunks by word count.
    chunk_size: words per chunk
    overlap: words shared between consecutive chunks
    """
    # your code here
    pass

# Test with a long paragraph
long_text = """Large language models have transformed the way we interact with AI systems.
They are trained on massive amounts of text data and learn to predict the next token.
RAG systems extend these models by providing external knowledge at inference time.
This allows models to answer questions about documents they were never trained on.
The retrieval component finds relevant information, and the generation component synthesizes it."""

chunks = chunk_text(long_text, chunk_size=30, overlap=10)
for i, chunk in enumerate(chunks):
    print(f"Chunk {i}: {chunk}\n")
```

---

### Interview Q&A

**Q1: Why use RAG instead of just fine-tuning the model on your data?**

> RAG and fine-tuning solve different problems. RAG is for *knowledge* that changes frequently or needs citations — the model retrieves at runtime so the knowledge stays current without retraining. Fine-tuning is for *behavior* — changing how the model responds (tone, format, domain-specific reasoning style). RAG is also cheaper: no GPU training required. Use both together for best results: fine-tune for behavior, RAG for knowledge.

**Q2: What is the difference between vector search and keyword search? When does each fail?**

> Vector search uses semantic similarity (embedding distance) — finds conceptually related content even with different words. Fails for exact matches: product codes, names, acronyms. Keyword search (BM25) finds exact term matches — fails when the user uses different words than the document. Hybrid search (BM25 + vector + RRF) addresses both failure modes.

**Q3: What causes poor RAG retrieval and how do you fix it?**

> Common causes: (1) Chunks too large — dilute the relevant signal. Fix: smaller chunks with overlap. (2) Wrong embedding model — mismatch between query and document domain. Fix: use a domain-appropriate model. (3) Missing reranking — ANN retrieves approximately. Fix: add cross-encoder reranking. (4) No hybrid search — pure vector misses exact keywords. Fix: add BM25. (5) Query-document mismatch — query is a question, document is a statement. Fix: HyDE (generate a hypothetical answer, embed that instead).

**Q4: What is chunking overlap and why is it needed?**

> When splitting a document into chunks, important information can land at the boundary between two chunks — split across both but fully in neither. Overlap (e.g., 20% of chunk size repeated at the start of the next chunk) ensures boundary content is fully represented in at least one chunk.

**Q5: How do you evaluate a RAG system?**

> Retrieval quality: **hit rate** (is the answer chunk in top-k?), **MRR** (mean reciprocal rank — how high up is the right chunk?). Generation quality: **faithfulness** (is the answer grounded in retrieved context?), **answer relevance** (does it actually answer the question?), **context relevance** (are retrieved chunks relevant to the question?). RAGAS is the standard framework that automates all of these using an LLM judge.

---

## 1.4 Fine-Tuning & Parameter-Efficient Methods

### Analogy

Imagine a **general practitioner (GP) doctor** — trained on all of medicine, knows a bit about everything. Now imagine you need a **neurosurgeon**. You don't train a doctor from scratch — you take a GP and put them through a neurosurgery residency.

- **Pre-trained LLM** = the GP (expensive to train from scratch)
- **Fine-tuning** = the neurosurgery residency (adapt to specialist domain)
- **LoRA** = the residency, but only certain parts of the brain are re-wired (efficient, cheap)

---

### How It Works Internally

#### Supervised Fine-Tuning (SFT)
Train the model on labeled examples in (instruction, response) format:

```json
{"instruction": "Summarize this legal contract.", "input": "This agreement...", "output": "The parties agree to..."}
```

All model weights are updated. Requires GPUs, significant data (1K–100K examples), and time. Used to change the model's *behavior and style*, not just its knowledge.

#### LoRA (Low-Rank Adaptation)
Instead of updating all weights (billions of parameters), LoRA **freezes all original weights** and injects small trainable matrices at specific layers.

**The math:**
```
Original weight update: ΔW (huge matrix, e.g., 4096 × 4096 = 16M params)
LoRA approximation:     ΔW ≈ A × B
  where A is (4096 × r) and B is (r × 4096), r = rank (e.g., 8 or 16)
  Trainable params: 4096×8 + 8×4096 = 65,536 (0.4% of original!)
```

At inference: `W_effective = W_frozen + A × B`

**Key hyperparameters:**
- `r` (rank) — controls capacity. Higher r = more expressive but more params. Typical: 8–64.
- `alpha` — scaling factor for the LoRA contribution. Often set to `2r`.
- `target_modules` — which layers to apply LoRA to (usually attention: `q_proj`, `v_proj`).

#### QLoRA
LoRA + **quantization**. The base model is loaded in 4-bit (NF4) precision, cutting memory by ~4x. Only the LoRA adapters (still in fp16) are trained.

This means you can fine-tune a **70B model on a single 48GB GPU** — previously impossible.

#### Decision Framework: When to use what?

| Scenario | Approach |
|----------|----------|
| Need latest info, data changes often | RAG |
| Need specific output format/style | Prompt engineering first |
| Model needs domain expertise + consistent behavior | Fine-tuning (SFT) |
| Limited GPU budget, large base model | LoRA / QLoRA |
| Need to align model with human preferences | RLHF or DPO |

#### RLHF vs DPO

**RLHF (Reinforcement Learning from Human Feedback):**
1. Collect human preference rankings between model outputs
2. Train a reward model to predict human preference scores
3. Use PPO (RL algorithm) to fine-tune the LLM to maximize reward
Complex, unstable, expensive.

**DPO (Direct Preference Optimization):**
Skips the reward model entirely. Directly trains on (chosen, rejected) pairs using a clever mathematical equivalence. Simpler, more stable, becoming the industry standard.

```
Input: (prompt, winning_response, losing_response)
Loss: maximize log P(winning) - log P(losing)  [simplified]
```

---

### Code — PRIMM Exercise

#### PREDICT: What do `r`, `lora_alpha`, and `target_modules` control?

```python
# pip install transformers peft accelerate bitsandbytes datasets

from transformers import AutoModelForCausalLM, AutoTokenizer, TrainingArguments, Trainer
from peft import LoraConfig, get_peft_model, TaskType
import torch

# --- Load base model (small example: GPT-2 for local testing) ---
model_name = "gpt2"
tokenizer = AutoTokenizer.from_pretrained(model_name)
tokenizer.pad_token = tokenizer.eos_token

model = AutoModelForCausalLM.from_pretrained(model_name)

# --- Define LoRA config ---
lora_config = LoraConfig(
    task_type=TaskType.CAUSAL_LM,
    r=8,                          # rank — controls adapter capacity
    lora_alpha=16,                # scaling: alpha/r = 2.0 (standard)
    target_modules=["c_attn"],    # GPT-2 attention projection layer
    lora_dropout=0.05,
    bias="none"
)

# --- Wrap model with LoRA ---
peft_model = get_peft_model(model, lora_config)

# --- See the difference in trainable params ---
peft_model.print_trainable_parameters()
# Output: trainable params: 294,912 || all params: 124,734,720 || trainable%: 0.2365
```

#### See which layers have LoRA adapters:

```python
for name, param in peft_model.named_parameters():
    if param.requires_grad:
        print(f"Trainable: {name} | Shape: {param.shape}")
```

#### For production (QLoRA on a real LLM):

```python
from transformers import BitsAndBytesConfig
from peft import LoraConfig, get_peft_model

# 4-bit quantization config
bnb_config = BitsAndBytesConfig(
    load_in_4bit=True,
    bnb_4bit_quant_type="nf4",        # NormalFloat4 — better than int4 for LLMs
    bnb_4bit_compute_dtype=torch.float16,
    bnb_4bit_use_double_quant=True    # quantize the quantization constants too
)

# Load a real model in 4-bit (requires GPU with ~10GB VRAM for 7B models)
# model = AutoModelForCausalLM.from_pretrained(
#     "meta-llama/Llama-3.1-8B",
#     quantization_config=bnb_config,
#     device_map="auto"
# )

lora_config = LoraConfig(
    r=16,
    lora_alpha=32,
    target_modules=["q_proj", "v_proj", "k_proj", "o_proj"],  # Llama attention layers
    lora_dropout=0.05,
    bias="none",
    task_type=TaskType.CAUSAL_LM
)
```

#### MODIFY — Try these:
1. Change `r=8` to `r=64` — how many more trainable parameters?
2. Add `"mlp"` to `target_modules` in GPT-2 (`c_fc`, `c_proj`) — does trainable % increase significantly?
3. Print the model architecture with `print(peft_model)` — can you spot where the LoRA layers are injected?

#### MAKE — Write a function to compare trainable parameters:

```python
def compare_lora_ranks(model_name: str, ranks: list[int]) -> None:
    """
    For each rank in ranks, apply LoRA to the model and print trainable param %.
    Helps you choose the right rank for your compute budget.
    """
    # your code here
    pass

compare_lora_ranks("gpt2", ranks=[4, 8, 16, 32, 64])
```

---

### Interview Q&A

**Q1: What problem does LoRA solve compared to full fine-tuning?**

> Full fine-tuning updates all model weights — for a 7B model that's 7 billion parameter updates requiring ~28GB of GPU memory just for the model, plus optimizer states (~3x more). LoRA freezes the original weights and trains tiny rank-decomposition matrices (A×B) at each layer. Trainable parameters drop to <1% of total, fitting fine-tuning on a single consumer GPU.

**Q2: What is the rank `r` in LoRA and how do you choose it?**

> Rank controls the "capacity" of the adapter — how expressive the weight update can be. Low rank (4–8): fast, cheap, good for simple stylistic changes. Higher rank (32–64): more expressive, needed for complex domain adaptation. Start at r=16, tune from there. If the model's performance plateaus, increase rank.

**Q3: What is the difference between RLHF and DPO?**

> RLHF trains a separate reward model from human preferences, then uses PPO (a reinforcement learning algorithm) to optimize the LLM against that reward — a two-stage, complex process prone to reward hacking and training instability. DPO reformulates the same objective as a supervised learning problem directly on (chosen, rejected) pairs, eliminating the reward model and RL stage entirely. DPO is simpler, more stable, and increasingly preferred.

**Q4: When would you NOT use fine-tuning?**

> When (1) the knowledge you need changes frequently — fine-tuned weights are static, use RAG instead. (2) You have limited labeled data (<500 examples) — prompt engineering or few-shot will likely outperform. (3) You need interpretable reasoning — fine-tuned models are harder to audit than prompted ones. (4) You're in a cost-constrained environment — fine-tuning requires GPU infrastructure and ongoing maintenance.

**Q5: What is QLoRA and why is it significant?**

> QLoRA combines quantization (loading the base model in 4-bit NF4 format, reducing memory ~4x) with LoRA (training only small adapter matrices in full precision). This made it possible to fine-tune 65B+ parameter models on a single 48GB GPU — previously requiring clusters of 8×80GB GPUs. It democratized LLM fine-tuning for researchers and smaller companies.

---

## 1.5 AI Agents & Function Calling

### Analogy

Think of an AI agent like a **detective solving a case**. A detective doesn't know everything upfront — they:
1. **Reason** about what they know ("The suspect was in London on Monday...")
2. **Take an action** (call a witness, search a database)
3. **Observe** the result
4. **Reason again** with new information
5. Repeat until the case is solved

That loop — **Reason → Act → Observe → Repeat** — is exactly the ReAct agent pattern. The LLM is the detective's brain. The tools (search, calculator, database) are their investigative resources.

---

### How It Works Internally

#### ReAct (Reason + Act) Pattern

```
User: "What is the current stock price of Apple and how has it changed this week?"

Thought: I need to look up Apple's current stock price.
Action: search_stock(ticker="AAPL")
Observation: AAPL = $189.50 (as of today)

Thought: Now I need the price from 7 days ago.
Action: search_stock(ticker="AAPL", date="7_days_ago")
Observation: AAPL = $182.30

Thought: I have both prices. Change = (189.50 - 182.30) / 182.30 = +3.9%
Action: FINISH
Answer: Apple's stock is $189.50 today, up 3.9% from $182.30 a week ago.
```

The key insight: the LLM generates **both the reasoning text AND the action calls** as tokens. The runtime intercepts action calls, executes the tool, and feeds the result back as an observation.

#### Function Calling (OpenAI)
OpenAI's implementation of tool use. You define tools as JSON schema. The model outputs structured JSON when it decides to call a tool.

```
You → Model: "What's the weather in Paris?" + [weather_tool_schema]
Model → You: {"tool_call": {"name": "get_weather", "arguments": {"city": "Paris"}}}
You execute the function → get_weather("Paris") → "18°C, Cloudy"
You → Model: [previous messages] + "Tool result: 18°C, Cloudy"
Model → You: "The weather in Paris is 18°C and cloudy."
```

The model never actually runs code — it outputs a structured request, your code runs it, you feed back the result.

#### Agent Memory Types

| Memory Type | What it stores | Duration |
|-------------|---------------|----------|
| Short-term buffer | Conversation history (last N messages) | Per session |
| Summary memory | Compressed summary of earlier conversation | Per session |
| Entity memory | Key facts about entities mentioned | Per session |
| Long-term (vector) | Embeddings of past conversations/facts | Persistent across sessions |

#### Multi-Agent Patterns

**Supervisor pattern:** One orchestrator LLM breaks the task and delegates to specialist agents, collects results, synthesizes final answer.

```
User Query → Supervisor
              ├── Research Agent (web search)
              ├── Code Agent (writes + runs code)
              └── Writer Agent (formats final output)
            → Supervisor synthesizes → Final Answer
```

**Handoff pattern:** Agents pass control to each other directly based on task type (used in OpenAI Swarm, LangGraph).

---

### Code — PRIMM Exercise

#### PREDICT: How many tool calls will the agent make for "What is 15% of 847, and what is the square root of that result?"

```python
from openai import OpenAI
import json, math

client = OpenAI(api_key="your-api-key")

# --- Define tools ---
tools = [
    {
        "type": "function",
        "function": {
            "name": "calculate",
            "description": "Perform a mathematical calculation. Supports: percent, sqrt, add, multiply.",
            "parameters": {
                "type": "object",
                "properties": {
                    "operation": {
                        "type": "string",
                        "enum": ["percent", "sqrt", "add", "multiply"]
                    },
                    "a": {"type": "number", "description": "First number"},
                    "b": {"type": "number", "description": "Second number (not needed for sqrt)"}
                },
                "required": ["operation", "a"]
            }
        }
    },
    {
        "type": "function",
        "function": {
            "name": "get_weather",
            "description": "Get current weather for a city.",
            "parameters": {
                "type": "object",
                "properties": {
                    "city": {"type": "string"}
                },
                "required": ["city"]
            }
        }
    }
]

# --- Tool execution (your code, not the LLM) ---
def execute_tool(name: str, args: dict) -> str:
    if name == "calculate":
        op = args["operation"]
        a = args["a"]
        b = args.get("b", 0)
        if op == "percent":
            return str(a * b / 100)
        elif op == "sqrt":
            return str(math.sqrt(a))
        elif op == "add":
            return str(a + b)
        elif op == "multiply":
            return str(a * b)
    elif name == "get_weather":
        # Mock response
        return f"Weather in {args['city']}: 22°C, Sunny"
    return "Unknown tool"

# --- Agent loop ---
def run_agent(user_message: str) -> str:
    messages = [{"role": "user", "content": user_message}]
    step = 0

    while True:
        step += 1
        print(f"\n--- Step {step} ---")

        response = client.chat.completions.create(
            model="gpt-4o",
            messages=messages,
            tools=tools,
            tool_choice="auto"
        )

        msg = response.choices[0].message
        messages.append(msg)

        # No tool calls → final answer
        if not msg.tool_calls:
            print(f"Final answer: {msg.content}")
            return msg.content

        # Execute each tool call
        for tool_call in msg.tool_calls:
            fn_name = tool_call.function.name
            fn_args = json.loads(tool_call.function.arguments)
            print(f"Tool call: {fn_name}({fn_args})")

            result = execute_tool(fn_name, fn_args)
            print(f"Result: {result}")

            messages.append({
                "role": "tool",
                "tool_call_id": tool_call.id,
                "content": result
            })

# --- Run it ---
run_agent("What is 15% of 847, and what is the square root of that result?")
```

#### MODIFY — Try these:
1. Ask `"What is the weather in Paris and Tokyo?"` — does it make parallel tool calls?
2. Add a `"search"` tool that just returns a mock result — ask the agent a knowledge question.
3. Add a step counter limit (max 5 steps) to prevent infinite loops.

#### MAKE — Build a simple memory-enabled agent:

```python
class SimpleAgent:
    def __init__(self, tools: list):
        self.tools = tools
        self.memory = []  # stores (role, content) tuples

    def chat(self, user_message: str) -> str:
        """
        Run the agent loop for one user turn.
        Maintain memory across calls so the agent remembers previous turns.
        """
        # your code here
        pass

agent = SimpleAgent(tools=tools)
print(agent.chat("My name is Alex."))
print(agent.chat("What is my name?"))  # Should remember "Alex"
```

---

### Interview Q&A

**Q1: What is the ReAct pattern and why is it effective?**

> ReAct interleaves reasoning ("Thought") and action ("Action") steps, with observations from tool results fed back in. The key insight is that generating reasoning tokens before each action conditions the model to take more relevant actions — and tool results in the context condition more accurate subsequent reasoning. It's effective because it mirrors how humans solve multi-step problems: think, act, observe, repeat.

**Q2: How does function calling work under the hood?**

> The model is trained to recognize when a tool would help and output a structured JSON object describing the tool name and arguments. Critically, the model does NOT execute any code — it produces a structured token sequence that your application runtime parses and dispatches to the appropriate function. The result is then appended to the message history as a "tool" role message, and the model continues generation with that context.

**Q3: What are the failure modes of agents and how do you mitigate them?**

> (1) **Infinite loops** — agent never decides to stop. Fix: max step limit, explicit stopping conditions. (2) **Wrong tool selection** — model calls the wrong tool. Fix: clear tool descriptions, few-shot examples of tool use in the system prompt. (3) **Hallucinated tool arguments** — model passes invalid args. Fix: strict JSON schema validation, argument type checking before execution. (4) **Context overflow** — long agent runs fill the context window. Fix: summarize older steps, use memory compression.

**Q4: When would you use a multi-agent system vs a single agent?**

> Single agents work for focused tasks with a clear sequence of steps. Multi-agent is better when: tasks require deep specialization (a research agent + coding agent are each better than one general agent), tasks are parallelizable (multiple agents working simultaneously), or reliability is critical (agents can verify each other's work). Trade-off: multi-agent adds orchestration complexity and cost.

**Q5: What is the difference between short-term and long-term memory in agents?**

> Short-term memory is the conversation history in the context window — it's fast but limited (context window size) and lost when the session ends. Long-term memory persists across sessions, typically stored in a vector database — relevant past memories are retrieved and injected into the context as needed. Long-term memory enables agents to remember users, past decisions, and learned facts across many interactions.

---

## 1.6 LLM Evaluation & Benchmarks

### Analogy

Evaluating an LLM is like **quality control at a factory**. You need different inspectors for different defects:
- One inspector checks if the product matches the spec (faithfulness)
- One checks if the customer actually wanted this product (relevance)
- One checks overall quality against industry standards (benchmarks)
- One monitors the assembly line in real-time (production metrics)

No single metric tells the full story — you need a dashboard of inspectors.

---

### How It Works Internally

#### Offline Evaluation (before deployment)

**Reference-based metrics** (compare model output to a ground-truth reference):

| Metric | What it measures | Limitation |
|--------|-----------------|------------|
| BLEU | n-gram overlap with reference | Penalizes valid paraphrases |
| ROUGE | Recall of n-grams from reference | Only for summarization |
| METEOR | BLEU + stemming + synonyms | Better but still surface-level |
| Perplexity | How "surprised" the model is by test text | Only for language modeling |

**LLM-as-a-Judge** (use GPT-4 to evaluate outputs):
More flexible than reference-based — can assess coherence, helpfulness, tone. Prompt GPT-4 with a rubric and have it score 1–5. Correlation with human judgments is high (>0.8 in many studies).

**G-Eval:** Structured LLM evaluation with chain-of-thought scoring. More reliable than single-score LLM-as-judge.

#### Benchmarks (standardized test suites)

| Benchmark | Tests | What it signals |
|-----------|-------|----------------|
| MMLU | 57 academic subjects, multiple choice | General knowledge breadth |
| HumanEval | Python coding problems | Code generation ability |
| MBPP | 374 Python tasks | Basic programming |
| GSM8K | Grade-school math word problems | Multi-step reasoning |
| TruthfulQA | Factual accuracy, avoiding false beliefs | Hallucination tendency |
| HELM | Holistic: accuracy, calibration, fairness, efficiency | Multi-dimensional quality |

> **Important:** Benchmark scores can be gamed by training on benchmark data. Always check if a model's training data excludes the benchmarks it's being evaluated on.

#### RAG-Specific Evaluation (RAGAS)

RAGAS evaluates RAG pipelines on 4 dimensions using an LLM judge:

| Metric | Measures | Formula idea |
|--------|----------|-------------|
| **Faithfulness** | Is the answer grounded in context? | Claims in answer ÷ claims supported by context |
| **Answer Relevance** | Does the answer address the question? | Similarity of generated question from answer to original |
| **Context Precision** | Are retrieved chunks relevant? | Relevant chunks ÷ total retrieved chunks |
| **Context Recall** | Were all needed chunks retrieved? | Claims in ground truth covered by context |

#### Production Metrics (post-deployment)

| Metric | What | How to measure |
|--------|------|----------------|
| TTFT | Time To First Token | Latency monitoring |
| TPOT | Time Per Output Token | Throughput monitoring |
| Token cost | $ per query | API usage logs |
| Groundedness | Answer stays within retrieved context | Automated NLI check |
| Safety violation rate | Harmful outputs / total | Content safety filter logs |
| Drift | Quality degrading over time | Periodic eval set scoring |

---

### Code — PRIMM Exercise

#### PREDICT: Which response will score higher on faithfulness?

```python
from openai import OpenAI

client = OpenAI(api_key="your-api-key")

context = """
The Eiffel Tower is located in Paris, France. It was built between 1887 and 1889
as the entrance arch for the 1889 World's Fair. It stands 330 meters tall.
Gustave Eiffel's company designed and built the tower.
"""

question = "Who built the Eiffel Tower and when?"

response_a = "The Eiffel Tower was built by Gustave Eiffel's company between 1887 and 1889."
response_b = "The Eiffel Tower was built in the 19th century by French engineers and is one of the most visited monuments in the world, attracting millions of tourists annually."

# --- LLM-as-a-Judge for Faithfulness ---
def evaluate_faithfulness(context: str, question: str, answer: str) -> dict:
    prompt = f"""You are evaluating whether an answer is faithful to the given context.
Faithful means: every claim in the answer is directly supported by the context.

Context: {context}
Question: {question}
Answer: {answer}

Evaluate step by step:
1. List each claim in the answer.
2. For each claim, check if it is supported by the context (yes/no).
3. Calculate: supported claims / total claims = faithfulness score (0.0 to 1.0)

Return JSON: {{"claims": [...], "supported": [...], "score": float, "reasoning": str}}"""

    response = client.chat.completions.create(
        model="gpt-4o",
        messages=[{"role": "user", "content": prompt}],
        response_format={"type": "json_object"},
        temperature=0.0
    )

    import json
    return json.loads(response.choices[0].message.content)

print("=== Response A ===")
import json
print(json.dumps(evaluate_faithfulness(context, question, response_a), indent=2))

print("\n=== Response B ===")
print(json.dumps(evaluate_faithfulness(context, question, response_b), indent=2))
```

#### Simple BLEU score (no API needed):

```python
from nltk.translate.bleu_score import sentence_bleu, SmoothingFunction
import nltk
nltk.download('punkt', quiet=True)

reference = "The Eiffel Tower was built by Gustave Eiffel between 1887 and 1889".split()
candidate_a = "Gustave Eiffel built the Eiffel Tower from 1887 to 1889".split()
candidate_b = "A famous tower in Paris was constructed in the late 1800s".split()

smoother = SmoothingFunction().method1
score_a = sentence_bleu([reference], candidate_a, smoothing_function=smoother)
score_b = sentence_bleu([reference], candidate_b, smoothing_function=smoother)

print(f"Candidate A BLEU: {score_a:.3f}")
print(f"Candidate B BLEU: {score_b:.3f}")
```

#### MODIFY — Try these:
1. Add an **Answer Relevance** evaluator: does the answer actually address the question?
2. Create a response that is `faithful=1.0` but `relevance=0.0` — can you construct one?
3. Modify the faithfulness prompt to also output a letter grade (A/B/C/D/F).

#### MAKE — Build a mini evaluation pipeline:

```python
def evaluate_rag_response(
    context: str,
    question: str,
    answer: str
) -> dict:
    """
    Returns a dict with:
    - faithfulness: 0.0 to 1.0
    - answer_relevance: 0.0 to 1.0
    - overall: weighted average
    - verdict: "Pass" if overall >= 0.7 else "Fail"
    """
    # your code here
    pass

# Test it
result = evaluate_rag_response(
    context=context,
    question=question,
    answer=response_a
)
print(result)
```

---

### Interview Q&A

**Q1: Why is BLEU insufficient for evaluating LLM outputs?**

> BLEU measures n-gram overlap with a reference answer. It penalizes valid paraphrases that use different words, cannot assess factual accuracy (a fluent hallucination scores well if it shares words with the reference), and doesn't capture coherence or helpfulness. For generative tasks with multiple valid answers, BLEU is a poor proxy for actual quality. LLM-as-a-judge with a rubric is far more aligned with human judgment.

**Q2: What is faithfulness in RAG evaluation and why does it matter?**

> Faithfulness measures whether every claim in the generated answer is supported by the retrieved context. A model can generate a fluent, plausible-sounding answer that contradicts or extends beyond the context — that's a hallucination. Faithfulness catches this. In production RAG systems (especially in healthcare, legal, finance), faithfulness is critical — answers must be traceable to source documents.

**Q3: What is the difference between context precision and context recall?**

> Context precision asks: "of the chunks I retrieved, how many were actually relevant?" — it penalizes noisy retrieval. Context recall asks: "of all the information needed to answer the question, how much did I retrieve?" — it penalizes missed information. You need both: high precision (clean retrieval) AND high recall (complete retrieval) for a strong RAG system.

**Q4: What is TTFT and why do users care about it?**

> TTFT (Time To First Token) is the latency between sending a request and receiving the first token of the response. Users perceive TTFT as "how long before it starts responding" — even if total generation takes 10 seconds, a 200ms TTFT makes the system feel responsive because the user sees output starting immediately. Streaming is the standard pattern to expose low TTFT to users.

**Q5: How do you detect and respond to model drift in production?**

> Monitor a held-out evaluation set periodically (e.g., daily) and track faithfulness, answer relevance, and safety scores over time. Alert when scores drop by >10% from baseline. Also track: topic distribution of queries (if user questions are shifting, your knowledge base may be stale), cost per query (anomalies indicate prompt injection or unexpected usage), and groundedness scores. When drift is detected: update the knowledge base (RAG), trigger a re-evaluation of prompts, or consider fine-tuning if behavioral drift is confirmed.

---

## Summary — Part 1 at a Glance

| Section | Core Concept | Key Interview Term |
|---------|-------------|-------------------|
| 1.1 LLM Architectures | Transformer self-attention, tokenization, causal masking | Decoder-only, MoE, RoPE |
| 1.2 Prompt Engineering | In-context learning, CoT, output formatting | Few-shot, self-consistency, logit bias |
| 1.3 RAG | Chunk → Embed → Index → Retrieve → Generate | Hybrid search, reranking, RAGAS |
| 1.4 Fine-Tuning | LoRA rank decomposition, QLoRA 4-bit, DPO | ΔW ≈ A×B, trainable params %, DPO vs RLHF |
| 1.5 Agents | ReAct loop, function calling, tool execution | Thought/Action/Observation, parallel tool calls |
| 1.6 Evaluation | Faithfulness, BLEU limits, LLM-as-judge | RAGAS, TTFT, groundedness |

---

## What's Next

Once you're comfortable with Part 1, move to:
- **Part 2** — The math and classical ML that explains *why* these techniques work
- **Part 3** — How to deploy all of the above on Azure (Azure OpenAI, AI Search, Semantic Kernel)

---

*Teaching model used: Feynman (analogy) + PRIMM (code) + Scaffolding (progressive depth)*
*File: Part1-AI-LLMs.md*
