# Module 11.1 — How LLMs Work: Attention & Transformer Architecture
**Part 3: Generative AI & LLMs | AI Solutions Architect Curriculum**

---

## What You Already Know (Recap)

From your previous sessions:
- Text → **Tokens** → Numbers (token IDs)
- Those numbers become **Embeddings** (vectors that capture meaning)
- The model predicts the **next token** one at a time
- **RAG** retrieves external context before sending to the LLM

The missing piece: **how does the model understand the relationship between tokens?**

That's what **Attention** and **Transformers** solve.

---

## 1. The Problem Attention Solves

Imagine this sentence:

> *"The bank by the river was steep, so we sat on the other bank."*

The word **"bank"** appears twice but means different things:
- First "bank" = river bank (geography)
- Second "bank" = also river bank (same context)

But consider:

> *"I deposited money at the bank."*

Here "bank" = financial institution.

**How does the model know which meaning to use?**

Old models (before Transformers) read left-to-right sequentially — by the time they got to "bank" they had forgotten early context. They were bad at long-range dependencies.

**Attention** fixes this: every token can **look at every other token** in the input and decide how much to focus on it.

---

## 2. What is Attention?

**Attention = a mechanism that lets each token ask: "which other tokens are most relevant to understanding me?"**

Simple example:

Sentence: `"The cat sat on the mat because it was tired"`

When the model processes the word **"it"**, attention lets it look back and ask:
- Is "it" referring to **cat**? → high relevance score
- Is "it" referring to **mat**? → low relevance score
- Is "it" referring to **sat**? → low relevance score

The model assigns a **weight** to each other token. Higher weight = more focus.

Result: the model understands **"it" = cat**, not mat.

---

## 3. How Attention Works (Simple Version)

For each token, attention computes three things:

| Name | What it means | Analogy |
|---|---|---|
| **Query (Q)** | "What am I looking for?" | A search query |
| **Key (K)** | "What do I offer to match against?" | A document title/tag |
| **Value (V)** | "What information do I actually carry?" | The document content |

**The process:**
1. Each token creates its own Q, K, V vectors
2. A token's **Query** is compared against every other token's **Key**
3. The comparison gives a **score** (how relevant is that token?)
4. Scores are normalized into **weights** (they sum to 1)
5. The final output is a **weighted sum of all Values**

In simple English:
> "I (Query) search all others (Keys), decide how relevant each one is (scores/weights), then blend their information (Values) based on relevance."

---

## 4. Self-Attention

The type used inside LLMs is called **Self-Attention** because the tokens are attending to **themselves** (within the same sequence).

Every token:
- Looks at all other tokens in the sentence
- Decides how much each one matters
- Updates its own representation based on that

This happens for **every token simultaneously** — not one at a time like old RNN models. That's why Transformers are fast and parallelizable.

---

## 5. Multi-Head Attention

Single attention captures one type of relationship. But language has many:

- Grammar relationships (subject → verb)
- Pronoun references (it → cat)
- Semantic similarity (river → water → bank)
- Position relationships (first word → last word)

**Multi-Head Attention = run attention multiple times in parallel, each "head" learning different relationships.**

Example with 8 heads:
- Head 1 might learn: subject-verb agreement
- Head 2 might learn: pronoun references
- Head 3 might learn: semantic word associations
- ...and so on

After all heads run, their outputs are **concatenated and combined** into one rich representation.

**Memory hook:**  
Single attention = one expert reading the sentence.  
Multi-head attention = 8 experts reading the same sentence, each noticing different things, then combining notes.

---

## 6. Positional Encoding

Attention looks at all tokens at once — but it loses track of **order**.

"Cat sat on mat" and "Mat on sat cat" would look the same to pure attention.

**Positional Encoding** fixes this by adding position information to each token's embedding before attention runs.

```
Token embedding  +  Position embedding  =  What the model actually uses
```

Position embeddings encode: "I am the 1st token", "I am the 5th token", etc.

So the model knows both:
- **What** the token means (from embedding)
- **Where** it appears (from positional encoding)

---

## 7. The Transformer Architecture

A **Transformer** is the full model architecture built on top of attention. Almost every modern LLM (GPT-4, Claude, Gemini) is a Transformer.

### Two variants:

| Type | Has | Used for | Examples |
|---|---|---|---|
| **Encoder-only** | Encoder | Understanding text | BERT, sentence classifiers |
| **Decoder-only** | Decoder | Generating text | GPT-4, Claude, Llama |
| **Encoder-Decoder** | Both | Translate/summarize | T5, BART |

**For Azure OpenAI (GPT models), you're dealing with Decoder-only.**

### Decoder block (what GPT uses) — one layer:

```
Input tokens
    ↓
[Token Embeddings + Positional Encoding]
    ↓
[Masked Multi-Head Self-Attention]   ← tokens attend to previous tokens only
    ↓
[Add & Normalize]
    ↓
[Feed-Forward Network]               ← each token processed independently
    ↓
[Add & Normalize]
    ↓
Output (richer token representations)
```

This entire block is called a **Transformer layer**. GPT-4 has many dozens of these stacked on top of each other.

**Why "masked" attention in decoder?**  
During generation, the model should only look at **past tokens**, not future ones (it hasn't generated those yet). Masking hides future tokens.

---

## 8. Feed-Forward Network (FFN)

After attention, each token's representation goes through a **Feed-Forward Network** — two linear layers with a non-linear activation between them.

Think of it as: attention figures out **relationships between tokens**, the FFN does **deeper processing on each token individually**.

The FFN is where a lot of the model's "knowledge" is stored (some research suggests stored facts live here, not just in attention).

---

## 9. Stacking Layers — How Depth Helps

One Transformer layer = one pass of attention + FFN.

Modern LLMs stack **many layers** (GPT-3 has 96 layers, GPT-4 has more):

```
Layer 1: basic patterns (grammar, syntax)
Layer 2: slightly more complex patterns
...
Layer N: high-level reasoning, facts, relationships
```

Early layers handle surface structure. Later layers handle meaning and reasoning. That's why **bigger models with more layers** are generally smarter.

---

## 10. GPT Architecture Walkthrough — End to End

Here's how GPT generates a response to: `"What is Azure AI Search?"`

```
Step 1: Tokenize
  "What is Azure AI Search?" → [1234, 318, 7592, 9552, 9622, 30]

Step 2: Embed + Position
  Each token ID → embedding vector
  Add positional encoding to each

Step 3: Pass through Transformer layers (e.g., 96 layers)
  Each layer: Masked Attention → Add+Norm → FFN → Add+Norm
  After all layers: each token has a rich, context-aware representation

Step 4: Predict next token
  The last token's representation is projected to vocabulary size
  Softmax gives probabilities for every possible next token
  The model picks (or samples) the highest probability token

Step 5: Append and repeat
  The generated token is added to the sequence
  The whole sequence goes through the model again to predict the next token
  Repeat until end-of-sequence token or max length
```

---

## 11. Key Numbers to Know (for Azure Architect context)

| Concept | What to remember |
|---|---|
| **Context window** | Total tokens (input + output) the model can handle at once. GPT-4: 128k tokens |
| **Parameters** | The learned weights in all layers. GPT-3: 175B, GPT-4: estimated much larger |
| **Attention heads** | GPT-3: 96 heads per layer |
| **Layers** | GPT-3: 96 layers |
| **Embedding dimension** | Size of each token's vector. Larger = more expressive |

---

## 12. Why This Matters for You as an Architect

| Transformer concept | Architect implication |
|---|---|
| **Context window limit** | You must chunk documents in RAG — can't send 1000-page PDF |
| **Token-by-token generation** | Streaming responses are just tokens arriving one at a time |
| **Multi-head attention** | Each head learns different things — explains why models are good at many tasks |
| **Layers = depth** | More layers = better reasoning = higher cost (GPT-4 vs GPT-3.5) |
| **Decoder-only = generation** | GPT models are for generating, not just classifying |
| **Positional encoding** | Order matters — "not good" ≠ "good not" |
| **Masked attention** | Model can't cheat by looking at future tokens during generation |

---

## 13. Common Misconceptions (for interviews)

| Misconception | Reality |
|---|---|
| "The model understands language like humans" | It predicts next tokens based on patterns |
| "Attention = the model thinking" | Attention is a mathematical weighting mechanism |
| "More parameters = always better" | Quality of training data matters equally |
| "GPT memorizes facts" | It compresses patterns; facts can hallucinate |
| "Context window = memory" | It's a sliding window, not permanent memory |

---

## 14. Mini Quiz (Test Yourself)

1. What problem does attention solve that older models had?
2. What are Q, K, V in attention — and what does each represent?
3. Why does a decoder use "masked" attention?
4. What does positional encoding add and why is it needed?
5. Why do LLMs stack many Transformer layers instead of just one?
6. If a GPT model has a 128k token context window, what does that mean for your RAG app?

*(Ask these questions in your Claude Code window for answers and discussion)*

---

## Memory Hooks

- **Attention** = every token asks "who else matters to understand me?"
- **Q/K/V** = Query searches, Key matches, Value delivers
- **Multi-head** = multiple experts each noticing different relationships
- **Positional encoding** = tells the model WHERE each token sits
- **Decoder-only** = GPT generates, doesn't encode for classification
- **Layers** = shallow = grammar, deep = reasoning

---

## What Comes Next (Module 11.2+)

After this chapter:
1. **Tokenization deep dive** — BPE, SentencePiece, token limits
2. **Pre-training and Fine-tuning** — how models learn, LoRA, QLoRA
3. **RLHF and Alignment** — how models are made safe and helpful
4. **Model capabilities & limitations** — hallucinations, reasoning, bias
5. **Azure OpenAI Service** (Module 12) — where you use all of this in practice

---
---

## 2026 Updates

| Topic | Update |
|---|---|
| **Mixture of Experts (MoE)** | GPT-4o and Mistral use MoE — not all parameters activate per token. Only a subset of "expert" layers activate per forward pass. This is why GPT-4o is fast despite being large. Architects need to know this when comparing model sizes |
| **Extended context windows** | GPT-4o: 128k tokens. Claude Sonnet 4.6: 200k tokens. Gemini 2.0: 1M tokens. The "lost-in-the-middle" problem remains — important content should be at start or end of context |
| **Reasoning models (o1/o3)** | OpenAI o1/o3 use chain-of-thought reasoning internally before answering. Different from standard transformer forward pass — model generates hidden reasoning tokens first. Slower but more accurate for complex tasks |
| **Multimodal transformers** | GPT-4o, Claude, and Gemini are natively multimodal — same transformer handles text, images, audio. Vision uses a ViT (Vision Transformer) encoder that converts image patches to embeddings |
| **Claude architecture** | Claude (Anthropic) uses Constitutional AI + RLHF. Same transformer decoder architecture as GPT but trained differently. Sonnet 4.6 is the current production model (August 2025 knowledge cutoff) |

---

## Interactive Learning Ideas

### Exercise 1 — Attention on Paper (15 min)
Take a 5-word sentence. Draw the attention matrix manually:
- Rows = query tokens, Columns = key tokens
- Fill in which pairs you'd expect high attention scores
- Show how the word "bank" would attend differently in "river bank" vs "bank account"
- This is self-attention — every token queries every other token

### Exercise 2 — Context Window Budget Planning (10 min)
Design the token budget for a JMA dealer support agent (128k context window):
- System prompt: how many tokens?
- RAG retrieved context (top-5 chunks at 512 tokens each): how many?
- Conversation history (last 10 turns): how many?
- User query: how many?
- Output reservation: how many?
- Total: do you fit? What do you cut if you don't?

### Exercise 3 — Model Selection Decision
For each JMA use case, pick the right model (GPT-4o / GPT-4o mini / o1 / o3):
- Classify a dealer support ticket into one of 5 categories (high volume, needs speed + low cost)
- Write a complex multi-step contract analysis with legal reasoning
- Generate a 2-sentence summary of a call transcript
- Solve a multi-step math problem to verify a dealer's invoice total

### Exercise 4 — MoE vs Dense Model Comparison
Research: GPT-4o is an MoE model. GPT-4o mini is a dense model. What does this mean for:
- Speed of inference?
- Cost per token?
- Total parameter count vs active parameter count?
- When would you choose mini over full 4o despite lower capability?

---

*File: Part3_Module11_1_Attention_Transformer.md | AI Solutions Architect Curriculum*
*Updated: 2026-06-30*
