# Layer 3: Model Catalog

The **App Store for AI models** — where you pick the brain for your AI application.

---

## Position in Architecture

```
┌─────────────────────────────────────────────────┐
│           AI Foundry Portal (UI)                │
│  ══════════════════════════════════════════════ │
│  Layer 2 │ Hub & Projects                       │
│  ─────────────────────────────────────────────  │
│  Layer 3 │ Model Catalog               ◄─────── │  ← YOU ARE HERE
│  ─────────────────────────────────────────────  │
│  Layer 4 │ AI Services & Tools                  │
│  ─────────────────────────────────────────────  │
│  Layer 5 │ Deployment & Monitoring              │
└─────────────────────────────────────────────────┘
```

---

## What is the Model Catalog?

A library of **1,700+ models** from Microsoft, OpenAI, Meta, Mistral, Google, and others —
all available directly inside AI Foundry.

```
Model Catalog
 ├── OpenAI          → GPT-4o, GPT-4, GPT-3.5, DALL-E, Whisper
 ├── Meta            → Llama 3, Llama 2
 ├── Mistral         → Mistral Large, Mistral Small
 ├── Microsoft       → Phi-3, Phi-4 (small but powerful)
 ├── Google          → Gemma
 ├── Hugging Face    → thousands of open-source models
 └── Custom          → your own fine-tuned models
```

---

## Model Collections — 3 Categories

```
┌─────────────────────────────────────────────────┐
│  1. Frontier Models                             │
│     └── Best-in-class, large, powerful          │
│          GPT-4o, Claude, Llama 3 70B            │
├─────────────────────────────────────────────────┤
│  2. Open Models                                 │
│     └── Open source, flexible, cost-effective   │
│          Phi-3, Mistral, Gemma                  │
├─────────────────────────────────────────────────┤
│  3. Task-Specific Models                        │
│     └── Built for one job                       │
│          Whisper (speech), DALL-E (images),     │
│          Embeddings (text→vectors)              │
└─────────────────────────────────────────────────┘
```

---

## Two Deployment Types — Most Important Concept

```
┌──────────────────────────┬──────────────────────────┐
│  Serverless API          │  Managed Compute          │
│  (Pay-as-you-go)         │  (Dedicated)              │
├──────────────────────────┼──────────────────────────┤
│  No infrastructure       │  You own the compute      │
│  Microsoft manages it    │  You manage it            │
│  Pay per token           │  Pay per hour             │
│  Best for: most cases    │  Best for: high volume,   │
│                          │  data privacy needs       │
├──────────────────────────┼──────────────────────────┤
│  GPT-4o, Llama, Mistral  │  Any model                │
└──────────────────────────┴──────────────────────────┘
```

**.NET Analogy:**
- **Serverless** = Azure Functions (consumption plan) — pay per call
- **Managed Compute** = App Service (dedicated plan) — pay per hour

---

## Model Benchmarks

The catalog includes **built-in benchmarks** so you can compare models before picking:

```
Compare models by:
 ├── Quality score      ← how accurate/smart
 ├── Cost per token     ← how expensive
 ├── Latency            ← how fast
 ├── Context window     ← how much text it can handle
 └── Task performance   ← coding, reasoning, summarization
```

---

## How Model Catalog Connects to Other Layers

```
Model Catalog (Layer 3)
      │
      │  you pick + deploy a model
      ▼
Project (Layer 2)
      │
      │  model is now available inside your project
      ▼
Prompt Flow / RAG (Layer 4)
      │
      │  your tools call the model
      ▼
API Endpoint (Layer 5)
      │
      │  your app calls the endpoint
      ▼
Your Application (JMA chatbot, etc.)
```

---

## How to Pick a Model — Decision Tree

```
What do you need?

Text / Chat
 ├── Best quality, cost no concern  →  GPT-4o
 ├── Balance cost & quality        →  GPT-4o mini / Mistral Large
 ├── Fast & cheap                  →  Phi-3 / Mistral Small
 └── Data must stay private        →  Managed Compute deployment

Images
 └── DALL-E 3

Speech → Text
 └── Whisper

Text → Vectors (for RAG)
 └── text-embedding-ada-002 / text-embedding-3-large
```

---

## JMA Example — Vehicle Recommendation Chatbot

```
JMA Vehicle Recommendation Chatbot
 │
 ├── text-embedding-3-large    ← converts Toyota inventory docs
 │    (Embeddings Model)            into vectors for searching
 │
 └── GPT-4o                   ← reads search results + generates
      (Chat Model)                  the recommendation response
```

### Why Each Choice

| Model | Why |
|---|---|
| **text-embedding-3-large** | Best quality embeddings, great for product/inventory search where precision matters |
| **GPT-4o** | Best reasoning — understands customer intent and generates natural responses |

### How They Work Together (RAG Flow)

```
Customer: "I need a family SUV under $40k"
          │
          ▼
text-embedding-3-large           ← converts question to vector
          │
          ▼
Azure AI Search                  ← finds matching inventory vectors
          │
          ▼
GPT-4o  ◄── inventory results    ← reads results + customer question
          │
          ▼
"Here are 3 Toyota SUVs that match your needs..."
```

---

## One-Line Summary

> The Model Catalog is where you **browse, compare, and deploy AI models** into your Project —
> like an App Store where every app is a different AI brain.

---

## Navigation

| | |
|---|---|
| **Previous** | [02 — Layer 2: Hub & Projects](02-Layer2-Hub-and-Projects.md) |
| **Next** | [04 — Layer 4: AI Services & Tools](04-Layer4-AI-Services-and-Tools.md) |
