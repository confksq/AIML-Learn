# Layer 5: Deployment & Monitoring

The **ship it and watch it** layer — where your AI goes live and stays healthy.

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
│  Layer 4 │ AI Services & Tools                  │
│  ─────────────────────────────────────────────  │
│  Layer 5 │ Deployment & Monitoring     ◄─────── │  ← YOU ARE HERE
└─────────────────────────────────────────────────┘
```

---

## Two Halves of Layer 5

```
Layer 5
 ├── DEPLOYMENT    ← get it live (endpoints, versioning, rollback)
 └── MONITORING    ← keep it healthy (tracing, metrics, alerts)
```

---

## HALF 1: Deployment

### What Gets Deployed

```
From Layer 4                    Deployed as
─────────────────────────────────────────────
Prompt Flow          ──────►   REST API Endpoint
Fine-tuned Model     ──────►   Model Endpoint
RAG Pipeline         ──────►   REST API Endpoint
Base Model           ──────►   Model Endpoint
```

### Two Endpoint Types

```
┌──────────────────────────┬──────────────────────────┐
│  Real-time Endpoint      │  Batch Endpoint           │
│  (Online)                │  (Offline)                │
├──────────────────────────┼──────────────────────────┤
│  Instant response        │  Process large files      │
│  One request at a time   │  Thousands at once        │
│  Low latency             │  High throughput          │
│                          │                           │
│  JMA chatbot             │  Bulk document analysis   │
│  Vehicle recommendation  │  Overnight batch jobs     │
└──────────────────────────┴──────────────────────────┘
```

### Deployment Pipeline — How It Works

```
Layer 4 (Build)
    │
    │  Dev tests pass
    ▼
Project Dev Environment
    │
    │  Azure DevOps / GitHub Actions pipeline
    ▼
Project Staging Environment     ← test with real traffic
    │
    │  Evaluation scores pass threshold
    ▼
Project Production Environment  ← live endpoint
    │
    └── Your C# app / Angular app calls this endpoint
```

### Blue-Green Deployment (Safe Rollout)

```
Current live:   Endpoint v1  ──► 100% traffic
                     │
New version:    Endpoint v2  ──► 0% traffic (warm up)
                     │
After testing:  Endpoint v1  ──► 10% traffic
                Endpoint v2  ──► 90% traffic
                     │
Full rollout:   Endpoint v2  ──► 100% traffic
                Endpoint v1  ──► retired
```

> Same pattern as Azure App Service deployment slots — you already know this from .NET deployments.

---

## HALF 2: Monitoring

### 4 Things You Monitor

```
Monitoring
 ├── 1. Performance    ← latency, throughput, errors
 ├── 2. Quality        ← response groundedness, relevance
 ├── 3. Safety         ← harmful content detection
 └── 4. Cost           ← token usage, spend per endpoint
```

### Where You Monitor (Portal Left Nav)

```
Observe and optimize:
 ├── Tracing    ← see every step of every request end-to-end
 └── Monitoring ← dashboards, metrics, alerts

Protect and govern:
 ├── Risks + alerts   ← threshold alerts (cost spike, quality drop)
 └── Evaluation       ← continuous quality scoring in production
```

### Tracing — Most Powerful Tool

```
User asks: "What SUVs are under $40k?"
    │
    ▼  [Tracing captures every step]
    ├── Step 1: Input received          2ms
    ├── Step 2: Content Safety check    45ms   ✅ clean
    ├── Step 3: Embedding generated     120ms
    ├── Step 4: AI Search query         89ms   → 5 results
    ├── Step 5: Prompt built            3ms
    ├── Step 6: GPT-4o called           1.2s   → response
    ├── Step 7: Content Safety output   40ms   ✅ clean
    └── Step 8: Response returned       12ms

Total: 1.5s  |  Tokens: 1,847  |  Cost: $0.002
```

> Like **Application Insights** but for AI requests — you already know App Insights from .NET!

### Quality Monitoring in Production

```
Not just "is it up?" but "is it GOOD?"

Metrics tracked continuously:
 ├── Groundedness score    ← is it answering from your data?
 ├── Relevance score       ← is it on topic?
 ├── Token usage           ← cost control
 ├── Latency p50/p95/p99   ← response time percentiles
 └── Safety violations     ← how many blocked?

Alert examples:
 ├── Groundedness drops below 80%  → trigger re-evaluation
 ├── Latency p95 > 3s              → scale up compute
 └── Monthly tokens > budget       → alert team
```

---

## LLMOps Lives Here

```
Layer 4                          Layer 5
────────────────────────────────────────────────
Build Prompt Flow      ──────►  Deploy as endpoint
Run Evaluation         ──────►  Gate deployment (pass/fail)
Fine-tune model        ──────►  Deploy new model version
Update RAG index       ──────►  Swap index in endpoint
                                      │
                                      ▼
                               Monitor quality drift
                                      │
                                      ▼
                               Trigger re-evaluation
                                      │
                                      ▼
                               Redeploy if needed
                                 (full LLMOps loop)
```

---

## How Layer 5 Connects Your App

```
Your C# App (JMA)
 └── Semantic Kernel
      └── calls REST endpoint
               │
               ▼
      AI Foundry Endpoint (Layer 5)
               │
               ▼
      Prompt Flow (Layer 4)
               │
               ▼
      GPT-4o + RAG (Layers 3 & 4)
               │
               ▼
      Response back to your app
               │
               ▼
      Monitoring captures everything
```

---

## One-Line Summary

> **Deployment** turns your AI workflow into a live API endpoint.
> **Monitoring** ensures it stays fast, accurate, safe, and within budget
> — this is where LLMOps runs in production.

---

## Knowledge Check

**Q: Your JMA vehicle chatbot goes live. Next week GPT-4o gets updated by Microsoft
and response quality drops. Which Layer 5 feature catches this first and what do you do?**

<details>
<summary>Answer</summary>

**Monitoring / Evaluation** catches it first:
- Groundedness and relevance scores drop below threshold
- Alert fires automatically

**What you do:**
1. Check Tracing to compare before/after request traces
2. Run Evaluation pipeline against test dataset to confirm quality drop
3. Either update your Prompt Flow to compensate for new model behavior
4. Or pin to previous model version via Model Catalog
5. Redeploy via Azure DevOps pipeline
6. Monitor scores recover before full rollout

This is the **LLMOps loop** in action.

</details>

---

## Navigation

| | |
|---|---|
| **Previous** | [04 — Layer 4: AI Services & Tools](../04-Layer4-AI-Services-and-Tools.md) |
| **Next** | `06-AI-Agents-Deep-Dive.md` *(coming soon)* |
