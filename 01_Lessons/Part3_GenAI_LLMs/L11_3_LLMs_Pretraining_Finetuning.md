# Module 11.3 — Pre-training & Fine-tuning
**Part 3: Generative AI & LLMs | AI Solutions Architect Curriculum**

---

## What You Already Know (Recap)

From your previous sessions:
- Text → **Tokens** → Token IDs → **Embeddings** → pass through **Transformer layers** (Attention + FFN)
- The model predicts the **next token** one at a time using the full context
- **RAG** retrieves external knowledge at query time and injects it into the prompt
- Context window is a **shared budget** — system prompt + history + RAG docs + response all share it

The missing piece: **where did the model's "knowledge" come from in the first place?**

And the architect question you'll face: **should you fine-tune, use RAG, or just engineer a better prompt?**

That's what this chapter covers.

---

**Running example (used throughout):**
> *"My laptop crashed and I lost my report. Can I get it recovered before my meeting at 3pm?"*

---

## Part A — Pre-training

---

## 1. What is Pre-training?

**Pre-training = teaching a model language itself, from scratch, on massive amounts of raw text.**

Before GPT can answer IT helpdesk questions, it needs to understand:
- What words mean
- How sentences are structured
- What facts are commonly true
- How to reason step by step

None of this is manually programmed. The model learns all of it by doing **one simple task billions of times:**

> **Predict the next token.**

That's it. Predict the next token — repeatedly, at scale, with self-correction — and the model gradually builds an internal representation of language and knowledge.

---

## 2. How Pre-training Works — Next-Token Prediction at Scale

**The training loop (simplified):**

```
Step 1: Take a sentence from the training data
  "The laptop recovery process requires submitting a ticket within 24 hours"

Step 2: Show the model a partial sequence
  Input:  "The laptop recovery process requires submitting a"
  Target: "ticket"

Step 3: Model predicts a probability distribution over all ~100,000 tokens
  "ticket"   → 34%   ← correct answer
  "form"     → 21%
  "request"  → 18%
  ...

Step 4: Compare prediction to actual next token
  Actual = "ticket"
  Model was 34% confident → loss calculated

Step 5: Backpropagation — adjust all the weights slightly to make "ticket" more likely
  (This is gradient descent — billions of small nudges)

Step 6: Repeat with the next example
  Input:  "The laptop recovery process requires submitting a ticket"
  Target: "within"

Repeat this process trillions of times across billions of documents.
```

**What the model learns by doing this:**
- Grammar and syntax (which words follow which patterns)
- Facts ("Paris is the capital of...") — because these patterns appear repeatedly
- Reasoning chains ("if X then Y") — because cause-and-effect patterns exist in text
- Style, tone, and domain knowledge — because training data spans the internet

**Key insight:** The model was never told any of this. It extracted all patterns by trying to predict the next word — over and over.

---

## 3. The Training Data

GPT-class models are pre-trained on a massive mix of internet text:

| Source | Examples |
|---|---|
| Web pages | Common Crawl (billions of pages) |
| Books | Books1, Books2, Project Gutenberg |
| Code | GitHub, StackOverflow |
| Wikipedia | All languages |
| News | News articles, journalism |
| Curated sets | WebText, C4, The Pile |

**GPT-3 training data:** ~300 billion tokens  
**GPT-4 training data:** not disclosed, estimated much larger

**Implication:** The model's "knowledge" is a compressed statistical pattern of all this text. It has seen more text than any human could read in thousands of lifetimes.

---

## 4. Why Models Have a Knowledge Cutoff Date

Pre-training data has a **snapshot date** — the date when the dataset was collected.

```
Training data collected → Model trained on it → Model deployed

Everything that happened AFTER the collection date = unknown to the model
```

**Example:**
- GPT-4's knowledge cutoff: April 2023
- If you ask about an event from June 2023 → the model doesn't know it
- The model may hallucinate a plausible-sounding but wrong answer

**For IT helpdesk at JM Family:**
```
User: "What does the new 2025 IT policy say about laptop recovery?"

GPT-4 (cutoff April 2023): ❌ No idea — hallucination risk
GPT-4 with RAG:            ✅ Retrieves the 2025 policy doc and answers correctly
```

This is one of the strongest arguments for RAG — you can keep the knowledge current without retraining the model.

---

## 5. What Pre-training Produces — The Base Model

After pre-training, you have a **base model** (also called a foundation model):
- GPT-3 base, LLaMA 3 base, Mistral base, etc.

A base model:
- Knows language very well
- Has broad factual knowledge up to its cutoff
- Is **not** particularly helpful or safe as an assistant

If you ask a base model "How do I recover my laptop files?", it might:
- Continue your sentence like an autocomplete engine
- Write a Wikipedia-style article
- Not give you a direct answer in assistant format

**Base models are the raw material. They need further training to become assistants.**

---

## Part B — Fine-tuning

---

## 6. What is Fine-tuning?

**Fine-tuning = taking a pre-trained base model and training it further on a smaller, specific dataset to improve its behavior for a particular purpose.**

```
Pre-trained model (general language knowledge)
           ↓
Fine-tuning on task-specific data
           ↓
Fine-tuned model (better at specific tasks / domain / behavior)
```

Fine-tuning is **much cheaper** than pre-training because:
- You start from a model that already understands language
- You only train for a few epochs on a small dataset (thousands, not billions of examples)
- You're adjusting weights slightly, not learning from scratch

---

## 7. Types of Fine-tuning

### Type 1 — Instruction Fine-tuning (Supervised Fine-tuning / SFT)

**Goal:** Teach the model to follow instructions in a helpful assistant format.

**Training data format:**
```json
{
  "instruction": "What should I do if my laptop crashes and I lose files?",
  "response": "If your laptop crashes and you lose files, here are the steps to recover them: 
               1. Submit a recovery ticket in the IT portal within 24 hours.
               2. Include your asset tag and a list of lost files.
               3. IT will restore from the last backup taken within 48 hours."
}
```

Thousands of these instruction-response pairs → model learns to behave as a helpful assistant.

**This is how GPT-3 base became InstructGPT, and later ChatGPT.**

### Type 2 — Domain Fine-tuning

**Goal:** Make the model better at a specific domain — not general assistant behavior, but domain knowledge and vocabulary.

**Example:** Fine-tune on JM Family's internal IT documentation so the model:
- Knows your specific ticket system names
- Uses the right internal terminology
- Understands your specific processes

**Training data:** Your company's documents, manuals, past tickets, resolved cases.

### Type 3 — Task-specific Fine-tuning

**Goal:** Optimize for a specific output format or task type.

Examples:
- Fine-tune for classification (is this ticket: hardware / software / network?)
- Fine-tune for structured output (always respond in JSON)
- Fine-tune for tone (formal internal IT communication style)

---

## 8. RLHF — Reinforcement Learning from Human Feedback

**RLHF = the technique used to make models safe, helpful, and aligned with what humans actually want.**

This is how base GPT-4 became the helpful, harmless assistant you interact with.

**The RLHF process:**

```
Step 1: Supervised Fine-tuning (SFT)
  Human trainers write ideal responses → model trained on them
  Result: model that can follow instructions

Step 2: Reward Model Training
  Human raters rank model outputs ("which response is better?")
  A separate "reward model" is trained to predict human preference scores

Step 3: PPO (Proximal Policy Optimization)
  The SFT model generates responses
  The reward model scores them
  The SFT model's weights are updated to maximize the reward score
  Repeat thousands of times

Result: A model that generates responses humans prefer — helpful, harmless, honest
```

**Applied to IT helpdesk running example:**

```
Model response A: "Submit ticket #1234 for laptop recovery."
Model response B: "I'm sorry to hear about your laptop crash. To recover your files, 
                   you'll need to submit a recovery request in the IT portal..."

Human raters prefer B → reward model learns this → PPO pushes model toward B-style responses
```

**You as an architect don't run RLHF.** This happens at the foundation model level (OpenAI, Anthropic, Meta). What you need to know:
- RLHF is why models are helpful and follow instructions
- RLHF is why models refuse harmful requests
- RLHF can sometimes make models overly cautious or verbose (a side effect)

---

## 9. LoRA and QLoRA — Efficient Fine-tuning

Full fine-tuning updates **all** model parameters. For GPT-3 (175B parameters) — that's extremely expensive.

**LoRA = Low-Rank Adaptation — fine-tune only a small set of additional parameters, keep the original weights frozen.**

### How LoRA Works

```
Original model weights: W  (175B parameters — FROZEN, never changed)

LoRA adds two small matrices:
  A  (small)
  B  (small)

During fine-tuning, only A and B are updated.
During inference: output = W·x + A·B·x

A and B together might be 0.1% of the total parameter count.
```

**Why this works:** Language models have highly redundant weight matrices. The "fine-tuning signal" lives in a low-dimensional subspace. LoRA captures that signal without touching the bulk of the model.

**Practical result:**
- Full fine-tune of GPT-3: requires ~5TB of GPU memory, weeks of compute
- LoRA fine-tune: runs on a single A100 GPU, hours to days
- Performance: often within 1–3% of full fine-tune quality

### QLoRA — Quantized LoRA

**QLoRA = LoRA + quantization (compress the frozen base model weights to use less memory)**

```
Standard LoRA:  frozen weights stored at 16-bit precision
QLoRA:          frozen weights stored at 4-bit precision  ← uses 4x less GPU memory
                LoRA adapters still trained at 16-bit
```

**Result:** Fine-tuning a 70B parameter model on a single consumer GPU (24GB VRAM) — previously impossible.

### LoRA in Azure OpenAI

When you fine-tune a model in Azure OpenAI:
- Under the hood, Azure uses LoRA-style parameter-efficient fine-tuning
- You upload JSONL training data
- Azure handles the training infrastructure
- You get a fine-tuned deployment endpoint to call

```
Your training file (JSONL format):
{"messages": [
  {"role": "system", "content": "You are an IT helpdesk assistant for JM Family."},
  {"role": "user", "content": "My laptop crashed. What do I do?"},
  {"role": "assistant", "content": "Please submit a recovery ticket in ServiceNow within 24 hours..."}
]}
```

---

## 10. Fine-tuning vs RAG vs Prompt Engineering — The Decision Framework ⭐

This is the most important architect decision in this entire module.

### The Three Levers

| Approach | What it does | Cost | When to use |
|---|---|---|---|
| **Prompt Engineering** | Shape model behavior with instructions in the system prompt | Zero — just text | Default starting point. Always try first. |
| **RAG** | Inject real-time knowledge from external sources at query time | Low-medium (retrieval + embedding infrastructure) | When answers require up-to-date, company-specific, or large-volume knowledge |
| **Fine-tuning** | Retrain model weights on your specific data | High (training compute + data prep) | When behavior/format needs to change, or prompt space is too limited |

### Decision Tree

```
Can I solve this with a well-written system prompt?
    YES → Use prompt engineering. Done.
    NO  ↓

Does the model need access to facts/docs it doesn't know?
    YES → Use RAG. (Knowledge problem — not a behavior problem)
    NO  ↓

Is the issue behavior, tone, format, or domain vocabulary?
    YES → Fine-tune.
    NO  ↓

Does the task require a fundamentally different capability?
    YES → Consider a specialized model or different approach
```

### When Fine-tuning is the Wrong Answer

| Scenario | Wrong approach | Right approach |
|---|---|---|
| Model doesn't know your company's current IT policy | Fine-tune on policy docs | RAG — policies change, retraining is slow |
| Model gives too long answers | Fine-tune | Add "Be concise. Answer in 2-3 sentences." to system prompt |
| Model doesn't know your ticket system name | Fine-tune | Put it in the system prompt: "Our ticket system is ServiceNow." |
| Model needs to look up live ticket status | Fine-tune | Function calling / tool use |

### When Fine-tuning IS the Right Answer

| Scenario | Why fine-tuning wins |
|---|---|
| Consistent output format (always JSON, always structured) | Prompt can be overridden; fine-tuning locks in behavior |
| Highly domain-specific vocabulary the model doesn't know | Can't inject a glossary via RAG — needs to be in weights |
| Reduce latency/cost by removing a long system prompt | Fine-tune the behavior in — shorter prompts |
| Small, fast model needs to behave like a big model | Fine-tune a 7B model on GPT-4 outputs (distillation) |
| Strict tone/persona that must never break | RLHF/SFT can enforce it more robustly than prompting |

### Applied to JM Family IT Helpdesk

```
Current state: GPT-4o + RAG on IT policy docs + system prompt

Is fine-tuning worth it?
├── Policy answers: NO → RAG handles this better (policies update frequently)
├── Tone/format: MAYBE → try system prompt first ("Always respond formally")
├── Helpdesk terminology: MAYBE → add to system prompt first
└── Consistent JSON output for ticket creation: YES → fine-tuning can help here

Recommendation: Start with RAG + prompt engineering.
Fine-tune only if you have 1,000+ labeled examples AND a behavior problem
that prompting cannot solve.
```

---

## 11. Transfer Learning — Why You Don't Train From Scratch

**Transfer learning = take a model trained on one large general task, and adapt it to a new specific task.**

This is why fine-tuning works:

```
Pre-training learned:
✅ What "recovery" means
✅ What "IT ticket" means
✅ How to write a helpful numbered list
✅ English grammar and syntax
✅ Thousands of domain patterns

Fine-tuning adds:
✅ JM Family's specific IT process
✅ Your ServiceNow ticket format
✅ Internal terminology

You get the combination — without paying for pre-training again.
```

**The analogy:** Hiring a doctor and training them in your hospital's specific EMR system is transfer learning. You didn't need to re-teach them medicine — just the hospital-specific procedures on top of their existing knowledge.

---

## 12. Why This Matters for You as an Architect

| Concept | Architect implication |
|---|---|
| **Pre-training = knowledge cutoff** | Build systems that don't depend on the model's internal knowledge for facts. Use RAG for anything that changes. |
| **Base model ≠ assistant** | Always deploy fine-tuned or instruction-tuned models, not raw base models |
| **Fine-tune vs RAG decision** | Default to RAG + prompting. Fine-tune only when you have a behavior problem, not a knowledge problem |
| **LoRA in Azure** | You don't need massive GPU clusters to fine-tune. Azure OpenAI fine-tuning is accessible |
| **RLHF = why models are helpful** | Safety filters and refusals come from RLHF. You can't override them with prompting alone (nor should you) |
| **Training data quality > quantity** | 500 high-quality fine-tuning examples beats 50,000 noisy ones |
| **Fine-tune is not a shortcut** | Curating 1,000 labeled Q&A pairs for your domain takes significant effort. Budget for it. |

---

## 13. Numbers to Know

| Fact | Value |
|---|---|
| GPT-3 training data | ~300 billion tokens |
| GPT-3 parameters | 175 billion |
| Minimum fine-tuning examples (Azure OpenAI) | 10 (recommended: 50–1,000+) |
| LoRA trainable parameters vs full model | ~0.1–1% of total parameters |
| QLoRA precision for frozen weights | 4-bit (NF4 quantization) |
| Fine-tuning epochs (typical) | 3–5 |
| Rule of thumb: when to fine-tune | When you have 100+ labeled examples AND prompting can't solve it |

---

## 14. Common Misconceptions

| Misconception | Reality |
|---|---|
| "Fine-tuning makes the model smarter" | It changes behavior, not intelligence. The model doesn't gain new reasoning — it learns your format and domain |
| "RAG and fine-tuning are interchangeable" | No — RAG solves knowledge problems; fine-tuning solves behavior problems. Often you need both |
| "Fine-tuning is expensive" | Full fine-tuning is. LoRA/QLoRA on small models is now accessible. Azure OpenAI fine-tuning is pay-per-token |
| "The model remembers fine-tuning data exactly" | It learns patterns, not verbatim text. Fine-tuning doesn't create a lookup table |
| "Prompt engineering is a workaround until you fine-tune" | It's a legitimate, maintainable approach. Many production systems never need fine-tuning |
| "RLHF is just filtering bad responses" | It's a full training loop with a reward model — much more sophisticated than post-filtering |

---

## 15. Mini Quiz (Test Yourself)

1. What is the single training task used during pre-training, and why does it result in broad language knowledge?
2. You're building an IT helpdesk assistant using GPT-4o. Your manager asks whether to fine-tune the model on past helpdesk tickets. What questions do you ask before deciding?
3. What is the knowledge cutoff problem, and how does RAG address it?
4. What does LoRA do differently from full fine-tuning, and why is it practical for enterprise use?
5. A colleague says "let's fine-tune the model so it knows our IT policy." What's the problem with this plan?
6. What does RLHF produce that instruction fine-tuning alone doesn't?

*(Ask these in your Claude Code window for discussion)*

---

## Memory Hooks

- **Pre-training** = predict next token, trillions of times → language + knowledge compressed into weights
- **Knowledge cutoff** = model doesn't know what happened after its training data was collected → use RAG for current facts
- **Base model** = knows language, not an assistant → needs fine-tuning to become helpful
- **Fine-tune = behavior**, **RAG = knowledge** — solve different problems
- **LoRA** = freeze the big model, train only tiny adapter matrices → cheap fine-tuning
- **QLoRA** = LoRA + 4-bit compression of frozen weights → fine-tune 70B models on one GPU
- **Default order:** Prompt Engineering → RAG → Fine-tuning (try in that order)

---

## What Comes Next (Module 11.4)

**11.4 — RLHF and Alignment**
- Why safety training matters and how it works in depth
- Constitutional AI (Anthropic's approach — used in Claude)
- What "alignment" means and why it's hard
- Prompt injection and jailbreaking — what they are and why they work
- What Azure Content Safety does and where it fits in your architecture
- As an architect: how to design AI systems that stay within safe boundaries

---
---

## 2026 Updates

| Topic | Update |
|---|---|
| **GPT-4o fine-tuning GA** | GPT-4o and GPT-4o mini fine-tuning now GA in Azure OpenAI. Not just GPT-3.5 anymore. Use JSONL format, same workflow |
| **Reasoning model training** | o1/o3 models (OpenAI) use a different training regime — they generate chain-of-thought reasoning tokens internally. This is not standard fine-tuning — it's a separate training approach not yet publicly exposable |
| **Vision fine-tuning** | GPT-4o fine-tuning now supports image inputs in training data — can teach the model to interpret domain-specific images (vehicle inspection photos, document layouts) |
| **LoRA via AI Foundry** | Azure AI Foundry model catalog supports LoRA fine-tuning on open-source models (Llama, Phi, Mistral) directly in the portal — no GPU cluster management required |
| **Phi-4 models** | Microsoft Phi-4 (3.8B parameters) achieves near GPT-4 performance on many benchmarks at 10x lower cost. Key insight: model size ≠ intelligence. Quality training data matters more than parameter count |

---

## Interactive Learning Ideas

### Exercise 1 — Fine-tune Decision Drill (10 min)
For each JMA scenario, decide: Fine-tune / RAG / Prompt Engineering / None?
- "Answer dealer questions using our internal policy documents" (documents change monthly)
- "Always respond in formal business English, never casual" (style requirement)
- "Classify dealer tickets into 5 categories using consistent labels"
- "Know that 'iPacket' is our digital retailing tool, not a generic term"
- "Answer questions about live inventory levels"

### Exercise 2 — JSONL Fine-tune Dataset (20 min)
Create a 10-example JSONL fine-tuning dataset for teaching GPT-4o to respond in JMA's customer service voice:
```jsonl
{"messages": [{"role": "system", "content": "..."}, {"role": "user", "content": "..."}, {"role": "assistant", "content": "..."}]}
```
Make each example show the contrast: what GPT-4o normally says vs what JMA voice sounds like.

### Exercise 3 — Training Cost Estimate
If you fine-tune GPT-4o mini with:
- 500 training examples
- Average 300 tokens per example
- 3 epochs
Calculate: total training tokens, cost at current Azure OpenAI fine-tuning pricing, and compare to 90 days of inference cost savings (assume 1,000 calls/day, 50-token shorter prompts post fine-tune).

### Exercise 4 — Loss Curve Analysis
Research what overfitting looks like in a fine-tuning loss curve:
- What does training loss vs validation loss look like when overfitting?
- What epoch count should you stop at?
- What's the difference between underfitting (loss still falling) vs good fit vs overfit?

---

*File: Part3_Module11_3_Pretraining_Finetuning.md | AI Solutions Architect Curriculum*
*Updated: 2026-06-30*
