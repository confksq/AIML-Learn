# Layer 4: AI Services & Tools

The **build layer** — where you take your deployed model and actually make it do something useful.

---

## Position in Architecture

```
┌─────────────────────────────────────────────────┐
│           AI Foundry Portal (UI)                │
│  ══════════════════════════════════════════════ │
│  Layer 2 │ Hub & Projects                       │
│  ─────────────────────────────────────────────  │
│  Layer 3 │ Model Catalog                        │
│  ─────────────────────────────────────────────  │
│  Layer 4 │ AI Services & Tools         ◄─────── │  ← YOU ARE HERE
│  ─────────────────────────────────────────────  │
│  Layer 5 │ Deployment & Monitoring              │
└─────────────────────────────────────────────────┘
```

---

## 6 Tools Inside Layer 4

```
Layer 4: AI Services & Tools
 │
 ├── 1. Prompt Flow       ← orchestrate your AI logic
 ├── 2. RAG & Grounding   ← connect your own data
 ├── 3. Evaluation        ← measure quality
 ├── 4. Fine-tuning       ← customize a model
 ├── 5. Content Safety    ← guard rails
 └── 6. Azure AI Search   ← powers RAG under the hood
```

---

## One-Line Purpose of Each

| Tool | One Line |
|---|---|
| **Prompt Flow** | Visually chain prompts, logic, and tools into a workflow |
| **RAG & Grounding** | Give the model YOUR data to answer from |
| **Evaluation** | Score and measure how good your AI responses are |
| **Fine-tuning** | Train the model on your own data to customize behavior |
| **Content Safety** | Block harmful inputs and outputs |
| **Azure AI Search** | The search engine that finds relevant chunks for RAG |

---

## How They Relate to Each Other

```
Your Data
    │
    ▼
Azure AI Search ──indexes──► RAG & Grounding
                                    │
                              injects context
                                    │
                                    ▼
Prompt Flow ◄────────────── GPT-4o (Layer 3)
    │
    ▼
Content Safety ── filters response
    │
    ▼
Evaluation ── scores quality
    │
    ▼
Layer 5 (Deploy & Monitor)
```

---

## Tool 1: Prompt Flow

### What is Prompt Flow?

A **visual / low-code orchestration tool** built inside AI Foundry that lets you chain
prompts, logic, and tools into a workflow — without writing application code.

```
Prompt Flow
 ├── Visual drag-and-drop flow builder
 ├── YAML based under the hood
 ├── Nodes = individual steps (LLM call, Python, Search, API)
 ├── Connections between nodes = data passing
 └── Output = deployable API endpoint
```

### Prompt Flow Node Types

```
Flow Nodes:
 ├── LLM Node       ← calls GPT-4o or any model
 ├── Python Node    ← custom logic / data transformation
 ├── Search Node    ← queries Azure AI Search
 ├── API Node       ← calls external REST APIs
 └── Prompt Node    ← defines prompt templates
```

### Example — JMA Vehicle Chatbot Flow

```
[Input: customer question]
        │
        ▼
[Embedding Node]              ← text-embedding-3-large converts question
        │
        ▼
[Search Node]                 ← Azure AI Search finds matching inventory
        │
        ▼
[Prompt Node]                 ← builds prompt with context + question
        │
        ▼
[LLM Node: GPT-4o]            ← generates recommendation
        │
        ▼
[Content Safety Node]         ← filters response
        │
        ▼
[Output: recommendation]
```

---

## Q&A: Prompt Flow vs Semantic Kernel

**Q: Is Prompt Flow where Semantic Kernel (SK) comes in?**

Great question — yes and no. They are complementary, not the same thing:

```
┌─────────────────────────────────────────────────┐
│  Prompt Flow                                    │
│  └── Visual / low-code orchestration tool       │
│       INSIDE AI Foundry Portal                  │
│       No coding required                        │
│       YAML based flows                          │
│       Designed for: AI Engineers, Data Scientists│
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│  Semantic Kernel (SK)                           │
│  └── Code-first SDK (C#, Python, Java)          │
│       You write it in your application          │
│       Designed for: Software Developers         │
│       Connects TO AI Foundry endpoints          │
└─────────────────────────────────────────────────┘
```

### Where They Sit in the 5 Layers

```
Layer 3 │ Model Catalog
─────────────────────────────────────────────────
Layer 4 │ AI Services & Tools
        │  └── Prompt Flow  ← visual orchestration
        │                      built IN AI Foundry
─────────────────────────────────────────────────
Layer 5 │ Deployment & Monitoring
        │  └── API Endpoint  ◄── Prompt Flow deploys here
                │
                │  SK calls this endpoint
                ▼
        Your C# Application
        └── Semantic Kernel  ← code orchestration
                               built OUTSIDE AI Foundry
```

### Three Integration Patterns

```
Option A — Prompt Flow only:
  AI Foundry Prompt Flow
        │ deploys as endpoint
        ▼
  Your C# App calls the endpoint
  (SK not needed for orchestration)

Option B — Semantic Kernel only:
  Your C# App
        └── Semantic Kernel orchestrates
             ├── calls GPT-4o directly
             ├── calls your RAG index
             └── chains logic in C# code
             (Prompt Flow not needed)

Option C — Both together (Enterprise pattern):
  Prompt Flow handles AI workflow
        │ deployed as endpoint
        ▼
  Semantic Kernel calls that endpoint
  + handles app-level orchestration
```

### Side-by-Side Comparison

| | Prompt Flow | Semantic Kernel |
|---|---|---|
| **Where** | Inside AI Foundry | Inside your C# app |
| **Who** | AI Engineers | Software Developers |
| **How** | Visual / YAML | Code (C#/Python) |
| **Orchestrates** | AI steps | App + AI steps |
| **Your role at JMA** | Configure it | Consume it |

### One-Line Summary

> **Prompt Flow** is the visual AI orchestrator inside AI Foundry.
> **Semantic Kernel** is the C# SDK you use in your app to call AI —
> they complement each other, SK doesn't replace Prompt Flow and vice versa.

---

---

## Tool 2: RAG & Grounding

### Problem it Solves

```
GPT-4o knows the world up to its training cutoff
It does NOT know your Toyota inventory, dealer manuals, or JMA policies

Solution:
  RAG = Retrieval Augmented Generation
  └── Fetch YOUR data → inject into prompt → model answers from it
```

> **One line:** Give the model YOUR data to answer from — without retraining it.

---

## Tool 3: Azure AI Search

### The Engine Under RAG

```
Your docs → chunked → converted to vectors → stored in AI Search index
                                                      │
Customer question → vectorized → AI Search finds ────┘
the most relevant chunks → passed to GPT-4o
```

> **One line:** The search engine that finds the right chunks of your data for RAG.

---

## Tool 4: Evaluation

### Problem it Solves

```
How do you know if your AI responses are actually good?
You can't manually read 10,000 responses

Solution:
  Evaluation runs automated scoring across test datasets

Scores:
  ├── Groundedness   ← is the answer based on your data?
  ├── Relevance      ← does it answer the question?
  ├── Coherence      ← is it well written?
  ├── Fluency        ← is it natural language?
  └── Safety         ← does it contain harmful content?
```

> **One line:** Automated quality scoring for your AI responses — your AI unit tests.

---

## Tool 5: Fine-tuning

### Problem it Solves

```
GPT-4o is general purpose
You need it to talk like a Toyota dealer, follow JMA tone,
or understand domain-specific terms

Solution:
  Fine-tuning = additional training on YOUR examples
  └── You provide: input → ideal output pairs (training data)
  └── Result: a customized version of the base model

When to use:
  ├── Prompt engineering not enough          → fine-tune
  ├── Need consistent tone/style            → fine-tune
  ├── Domain-specific language              → fine-tune
  └── Just need company data in answers     → RAG (cheaper!)
```

> **One line:** Customize a base model's behavior by training it on your own examples.

---

## Tool 6: Content Safety

### Problem it Solves

```
Users can send harmful prompts
Models can generate harmful responses
Jailbreaks, prompt injections, toxic content

Solution:
  Content Safety = guard rails on both input AND output

Filters:
  ├── Hate speech
  ├── Violence
  ├── Sexual content
  ├── Self-harm
  ├── Jailbreak attempts    ← "ignore your instructions and..."
  └── Prompt injection      ← malicious data trying to hijack the model
```

> **One line:** Filters that block harmful content going IN to and coming OUT of your model.

---

## How All 6 Work Together — Full Flow

```
User Input
    │
    ▼
┌─ Content Safety ─────────────────────────────┐
│  (block harmful input)                       │
└──────────────────────────────────────────────┘
    │
    ▼
┌─ Prompt Flow ────────────────────────────────┐
│  orchestrates the steps below:               │
│                                              │
│  Azure AI Search ──► RAG & Grounding         │
│       (find chunks)    (inject into prompt)  │
│                               │              │
│                               ▼              │
│                    GPT-4o (Layer 3)          │
│                    (fine-tuned if needed)    │
└──────────────────────────────────────────────┘
    │
    ▼
┌─ Content Safety ─────────────────────────────┐
│  (block harmful output)                      │
└──────────────────────────────────────────────┘
    │
    ▼
┌─ Evaluation ─────────────────────────────────┐
│  (score quality continuously)                │
└──────────────────────────────────────────────┘
    │
    ▼
Response to User
```

---

## All 6 Tools — Summary Table

| Tool | Solves | When to Use |
|---|---|---|
| **Prompt Flow** | AI logic orchestration | Always — it's the glue |
| **RAG & Grounding** | Model doesn't know your data | Your data changes frequently |
| **Azure AI Search** | Finding relevant data chunks | Always with RAG |
| **Evaluation** | Measuring response quality | Before every deployment |
| **Fine-tuning** | Model behavior/tone/style | When RAG + prompts aren't enough |
| **Content Safety** | Harmful content | Always in production |

---

## Progress in Layer 4

| Tool | Status |
|---|---|
| ✅ Prompt Flow | Covered — deep dive done |
| ✅ RAG & Grounding | Covered — high level done |
| ✅ Azure AI Search | Covered — high level done |
| ✅ Evaluation | Covered — high level done |
| ✅ Fine-tuning | Covered — high level done |
| ✅ Content Safety | Covered — high level done |

---

## Navigation

| | |
|---|---|
| **Previous** | [03 — Layer 3: Model Catalog](03-Layer3-Model-Catalog.md) |
| **Next** | [05 — Layer 5: Deployment & Monitoring](05-Layer5-Deployment-and-Monitoring.md) |
