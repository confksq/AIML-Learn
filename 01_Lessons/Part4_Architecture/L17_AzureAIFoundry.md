# Module 17 — Azure AI Foundry
**Part 3: Generative AI & LLMs | AI Solutions Architect Curriculum**
**[NEWLY ADDED — Career Roadmap]**

---

## What You Already Know (Recap)

From prior modules:
- **Module 12** — Azure OpenAI deployments, Chat Completions API
- **Module 13** — RAG pipelines, chunking, retrieval, evaluation need
- **Module 14** — Prompt Flow brief mention, SK vs managed tools
- **Module 15** — Fine-tuning: when and how
- **Module 16** — Prompt engineering, evaluation metrics (groundedness, relevance)

This module shows where ALL of that lives in one unified Microsoft platform — Azure AI Foundry.

---

**Running example (used throughout):**
> *JM Family builds an invoice Q&A assistant. Azure AI Foundry is where the team browses models, tests RAG pipelines visually, evaluates quality before going to production, and monitors everything after launch.*

---

## Topic 17.1 — What Is Azure AI Foundry

---

### 1. The One-Line Definition

```
Azure AI Foundry = Microsoft's unified platform for
                   building, evaluating, and deploying AI applications.
```

---

### 2. Why It Exists — The Problem Before Foundry

```
Before Azure AI Foundry (scattered tools):
  Model browsing     → Azure Marketplace (separate)
  Model deployment   → Azure OpenAI Studio (separate)
  RAG pipeline       → Build manually in code
  Evaluation         → Build manually in code
  Fine-tuning        → Azure OpenAI Studio (separate)
  Content Safety     → Azure Content Safety (separate)
  Monitoring         → App Insights (separate)

  Every tool in a different portal.
  No unified view.
  Hard to manage end to end.

After Azure AI Foundry (unified):
  Everything in ONE portal:
  ai.azure.com
  Model catalog → Prompt Flow → Evaluation → Deploy → Monitor
  All connected. All in one place.
```

---

### 3. Azure AI Foundry vs Azure OpenAI Studio

```
Many people confuse these two:

Azure OpenAI Studio (old):
  Only GPT models (OpenAI models)
  Chat playground, fine-tuning, deployments
  Being merged into Azure AI Foundry

Azure AI Foundry (new — current):
  ALL models — GPT-4o, Llama, Mistral, Phi, Cohere, etc.
  Everything Azure OpenAI Studio did PLUS:
    Prompt Flow visual builder
    Built-in evaluation framework
    Multi-model comparison
    Safety and content filtering management
    Agent building UI
  ← This is where you work now
  ← portal: ai.azure.com
```

---

### 4. Azure AI Foundry vs Semantic Kernel — Clarified

```
This was a common question in Module 14:

Azure AI Foundry:
  A PORTAL / PLATFORM — visual UI
  Where you browse, test, evaluate, deploy
  Good for: prototyping, evaluation, non-coders, demos
  You point and click

Semantic Kernel:
  A CODE SDK — C# / Python library
  Where you build production applications in code
  Good for: production apps, custom logic, enterprise systems
  You write code

They work TOGETHER:
  Use Azure AI Foundry to:
    → Find the right model
    → Test your RAG pipeline visually
    → Evaluate quality (groundedness score)
    → Deploy the model endpoint

  Use Semantic Kernel to:
    → Build the C# application that calls that endpoint
    → Add plugins, agents, memory in code
    → Deploy to your JM Family portal

JM Family path:
  Prototype and evaluate in Azure AI Foundry
  Build production app with Semantic Kernel
```

---

### 5. Azure AI Foundry Key Components

```
┌─────────────────────────────────────────────────────────────┐
│                    Azure AI Foundry                         │
│                    (ai.azure.com)                           │
│                                                             │
│  ┌─────────────┐  ┌──────────────┐  ┌───────────────────┐  │
│  │   Model     │  │   Prompt     │  │   Evaluation      │  │
│  │   Catalog   │  │   Flow       │  │   Framework       │  │
│  │             │  │              │  │                   │  │
│  │ 1,600+      │  │ Visual RAG   │  │ Groundedness      │  │
│  │ models      │  │ pipeline     │  │ Relevance         │  │
│  │ browse +    │  │ builder      │  │ Coherence         │  │
│  │ deploy      │  │ no-code      │  │ Fluency scores    │  │
│  └─────────────┘  └──────────────┘  └───────────────────┘  │
│                                                             │
│  ┌─────────────┐  ┌──────────────┐  ┌───────────────────┐  │
│  │  Fine-      │  │  Content     │  │   Monitoring      │  │
│  │  Tuning     │  │  Safety      │  │                   │  │
│  │             │  │              │  │ Token usage       │  │
│  │ UI-based    │  │ Filter +     │  │ Latency           │  │
│  │ no GPU      │  │ groundedness │  │ Error rates       │  │
│  │ setup       │  │ detection    │  │ Cost tracking     │  │
│  └─────────────┘  └──────────────┘  └───────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

---

## Topic 17.2 — Model Catalog

---

### 1. What Is the Model Catalog

```
A marketplace of 1,600+ AI models available in Azure.
Browse, compare, and deploy — all in one place.

Categories:
  OpenAI models:    GPT-4o, GPT-4o mini, o1, o3
  Microsoft models: Phi-3, Phi-4 (small but powerful)
  Meta models:      Llama 3, Llama 3.1, Llama 3.2
  Mistral models:   Mistral Large, Mistral Small
  Cohere models:    Command R, Command R+
  Stability AI:     Stable Diffusion (image generation)
  And many more...
```

---

### 2. Two Deployment Options for Catalog Models

```
SERVERLESS API (pay per token):
  No GPU to manage
  Microsoft hosts the model
  You pay only when you call it
  Best for: low-medium volume, prototyping
  Works like: Azure OpenAI today

  JM Family use:
    Testing Llama vs GPT-4o quality
    Without committing to a deployment

MANAGED COMPUTE (dedicated GPU):
  You provision a GPU cluster
  Model runs on YOUR compute
  Fixed cost (running 24/7)
  Best for: high volume, consistent latency needs
  Gives you: full control over the model runtime

  JM Family use:
    Production fine-tuned model at high call volume
```

---

### 3. How to Choose the Right Model

```
QUESTION 1: Does it need to stay in Azure OpenAI?
  Yes (compliance, existing integration) → GPT-4o, GPT-4o mini
  No (open to any model) → evaluate catalog options

QUESTION 2: Cost vs Quality trade-off
  Highest quality:   GPT-4o, Claude (via Azure)
  Good quality/cost: GPT-4o mini, Phi-4, Mistral Small
  Budget option:     Phi-3 mini, Llama 3.1 8B

QUESTION 3: Task type
  Document Q&A, RAG:      GPT-4o mini → good balance
  Complex reasoning:      GPT-4o, o1
  Code generation:        GPT-4o, Phi-4
  Simple classification:  Phi-3 mini → cheap and fast
  Image understanding:    GPT-4o (multimodal)

JM Family recommendation:
  Invoice assistant (RAG + agents): GPT-4o mini
  Executive summary drafting:       GPT-4o
  High-volume classification:       Fine-tuned GPT-4o mini
```

---

### 4. Model Comparison in Azure AI Foundry

```
Azure AI Foundry lets you compare models side by side:

  Same question → GPT-4o mini vs Llama 3.1 vs Phi-4
  See: response quality, token count, latency, cost
  Make data-driven decision — not just follow the hype

This is what an architect does:
  Not "use GPT-4o because it is the most famous"
  But "compare options, measure for our specific task,
       choose the best cost-quality fit"
```

---

## Topic 17.3 — Prompt Flow

---

### 1. What Is Prompt Flow

```
Prompt Flow = visual drag-and-drop pipeline builder
              for RAG and AI workflows

No code required to build a basic RAG pipeline.
Each step is a "node" on a canvas.
Connect nodes with arrows.
Test, iterate, deploy — all in the UI.
```

---

### 2. The RAG Pipeline in Prompt Flow

```
Visual canvas — left to right flow:

[Input]
  ↓
  User question arrives
  ↓
[Embed Query Node]
  ↓
  Calls text-embedding-3-small
  Converts question to vector
  ↓
[Vector Search Node]
  ↓
  Searches Azure AI Search index
  Returns top 5 relevant chunks
  ↓
[Prompt Template Node]
  ↓
  Combines: system instructions + chunks + question
  Builds the full augmented prompt
  ↓
[LLM Node]
  ↓
  Calls GPT-4o mini
  Generates answer based on augmented prompt
  ↓
[Output]
  ↓
  Returns answer + source citations to user
```

---

### 3. Node Types in Prompt Flow

```
Node Type          What It Does                    When to Use
────────────────────────────────────────────────────────────────
LLM Node           Calls any deployed model        Generate text, classify
                   GPT-4o, Phi-3, Llama, etc.

Embedding Node     Converts text to vector         Before vector search

Search Node        Queries Azure AI Search         Retrieve relevant chunks

Prompt Template    Builds the prompt string        Combine context + question

Python Node        Runs any Python code            Custom logic, calculations,
                   Custom transformation           data formatting

Condition Node     If/else branching               Route based on intent
                   Route flow differently          Different paths for
                   based on result                 different question types

Input/Output       Define what goes in/out         Start and end of flow
```

---

### 4. Prompt Flow vs Semantic Kernel — Decision Guide

```
USE PROMPT FLOW when:
  ✓ Prototyping — quickly test a RAG pipeline
  ✓ Showing stakeholders — visual is easier to explain
  ✓ Non-developers need to maintain it
  ✓ Standard RAG pattern — no complex custom logic
  ✓ Want built-in evaluation connected to the pipeline
  ✓ Deploy as a REST endpoint quickly for testing

USE SEMANTIC KERNEL when:
  ✓ Production C# application
  ✓ Complex business logic (cannot express in Prompt Flow nodes)
  ✓ Need full .NET ecosystem integration (DI, logging, auth)
  ✓ Multi-step agents with custom plugins
  ✓ JM Family dealer portal — production enterprise app
  ✓ Need Managed Identity, App Insights, Private Endpoints in code

JM Family typical path:
  Week 1:  Build RAG prototype in Prompt Flow (visual, fast)
           Show stakeholders — get sign-off
  Week 2+: Rebuild in Semantic Kernel for production
           Add enterprise features (auth, logging, security)
```

---

### 5. Deploying a Prompt Flow as REST Endpoint

```
Once your Prompt Flow is tested and working:

  1. Click "Deploy" in Azure AI Foundry
  2. Choose compute (serverless or managed)
  3. Azure creates a REST endpoint automatically

  Endpoint:
    POST https://jmf-ai-foundry.eastus.inference.ml.azure.com/score
    Body: {"question": "Is invoice JMF-ATL-001 overdue?"}
    Response: {"answer": "...", "sources": [...]}

  Your C# app calls this endpoint:
    No SK needed for simple scenarios
    Just an HttpClient POST

  When to use deployed Prompt Flow endpoint:
    Simple RAG — no agents, no complex logic
    Prototype promoted to production quickly
    Non-.NET teams (Python, JavaScript frontend)
```

---

## Topic 17.4 — Evaluation Flows

---

### 1. Why Evaluation Matters

```
Problem without evaluation:
  You build a RAG pipeline
  It seems to work in testing
  You deploy to production
  Users report wrong answers
  You have no data to diagnose the problem

Problem with evaluation:
  You measure quality BEFORE deploying
  You know exactly: groundedness 3.2/5, relevance 4.1/5
  You iterate until scores are acceptable
  You deploy with confidence
  You have baseline to compare future changes against
```

---

### 2. The Five Evaluation Metrics

```
GROUNDEDNESS (most important for RAG):
  Is the answer supported by the retrieved documents?
  Score: 1-5 (5 = fully grounded in source docs)

  Score 5: Answer is directly stated in the retrieved chunks
  Score 3: Answer is partially supported
  Score 1: Answer is hallucinated — not in any retrieved chunk

  JM Family: critical — invoice data must be grounded in actual records

RELEVANCE:
  Does the answer actually address the question asked?
  Score: 1-5

  Score 5: Answer directly and completely answers the question
  Score 3: Answer is related but misses key parts of question
  Score 1: Answer is off-topic

COHERENCE:
  Is the answer logically structured and well-reasoned?
  Score: 1-5
  Does it flow logically? Are conclusions supported by reasoning?

FLUENCY:
  Is the language natural and grammatically correct?
  Score: 1-5
  Would a human write it this way?

SIMILARITY:
  How close is the answer to the known correct answer?
  Score: 0-1 (cosine similarity)
  Requires ground truth answers to compare against
```

---

### 3. Running an Evaluation in Azure AI Foundry

```
Step 1: Prepare evaluation dataset
  A set of question + expected answer pairs
  Example:
    Q: "What is the penalty for late invoice submission?"
    A: "2% per month as per clause 3.2 of dealer agreement"

  Need minimum 20-50 pairs for meaningful results
  JM Family: use real historical Q&A from employees

Step 2: Connect your RAG pipeline
  Point evaluation at your Prompt Flow or SK endpoint
  Azure AI Foundry calls your pipeline with each question
  Collects the actual answers your system returns

Step 3: Run evaluation
  Azure AI Foundry uses GPT-4o as the evaluator
  GPT-4o reads: question + retrieved context + your answer
  Scores each metric 1-5 for every question
  Returns aggregate scores across all test cases

Step 4: Read the results
  Groundedness avg: 4.2/5  ← good
  Relevance avg:    3.8/5  ← acceptable
  Coherence avg:    4.5/5  ← good
  Fluency avg:      4.7/5  ← excellent

Step 5: Iterate
  Low groundedness → fix chunking or retrieval (more/better chunks)
  Low relevance → fix prompt template or system prompt
  Low coherence → add chain of thought to prompt
  Repeat until scores meet your threshold
```

---

### 4. Minimum Quality Bar for Production

```
JM Family recommended thresholds:

  Groundedness:  ≥ 4.0/5   → answers must be in source docs
  Relevance:     ≥ 4.0/5   → must answer what was asked
  Coherence:     ≥ 3.5/5   → must be logical
  Fluency:       ≥ 3.5/5   → must be readable

  If below threshold → do not deploy
  Fix the pipeline and re-evaluate

  Never deploy without evaluation:
    A score gives you evidence
    "Our RAG pipeline scored 4.2 groundedness" is
    something you can show stakeholders and leadership
```

---

### 5. C# — Calling the Evaluation API

```csharp
using Azure.AI.Projects;
using Azure.Identity;

// Connect to Azure AI Foundry project
var projectClient = new AIProjectClient(
    new Uri("https://jmf-ai-foundry.eastus.api.azureml.ms"),
    "your-subscription-id",
    "jmf-resource-group",
    "jmf-ai-foundry-project",
    new DefaultAzureCredential()
);

var evaluationClient = projectClient.GetEvaluationsClient();

// Run evaluation on your RAG pipeline output
var evaluation = await evaluationClient.CreateAsync(
    new EvaluationSchedule
    {
        DisplayName = "JMF Invoice RAG Evaluation - May 2026",
        Data = new InputDataset
        {
            // Your test dataset: questions + ground truth answers
            Id = "jmf-invoice-eval-dataset"
        },
        Evaluators = new Dictionary<string, EvaluatorConfiguration>
        {
            ["groundedness"] = new EvaluatorConfiguration
            {
                Id = EvaluatorIds.Groundedness
            },
            ["relevance"] = new EvaluatorConfiguration
            {
                Id = EvaluatorIds.Relevance
            },
            ["coherence"] = new EvaluatorConfiguration
            {
                Id = EvaluatorIds.Coherence
            }
        }
    }
);

Console.WriteLine($"Evaluation started: {evaluation.Value.Id}");
```

---

## Topic 17.5 — Fine-tuning in Azure AI Foundry

---

### 1. Fine-tuning UI — No Code Required

```
You learned fine-tuning in Module 15 — the C# SDK way.
Azure AI Foundry gives you a UI to do the same thing:

  Step 1: Go to Fine-tuning section in ai.azure.com
  Step 2: Choose base model (GPT-4o mini)
  Step 3: Upload training JSONL file (drag and drop)
  Step 4: Upload validation JSONL file (optional)
  Step 5: Set hyperparameters (or leave as auto)
  Step 6: Click "Start training job"
  Step 7: Monitor loss curves in the UI
  Step 8: Deploy fine-tuned model from the same UI

  No C# SDK code needed.
  Same result as the code approach from Module 15.
  UI approach is faster for first-time fine-tuning.
```

---

### 2. Monitoring Training in the UI

```
Azure AI Foundry shows live training charts:

  Training loss curve:     should decrease each epoch
  Validation loss curve:   should track training loss
  Token count:             how many tokens processed so far
  Estimated time:          when training will complete

  Visual overfitting detection:
    If validation loss starts rising → stop training
    UI makes this easy to spot — no log parsing needed
```

---

### 3. Fine-tuning → Evaluate → Deploy Flow

```
Best practice in Azure AI Foundry:

  1. Fine-tune model (UI)
  2. Create deployment of fine-tuned model
  3. Run evaluation against fine-tuned model
     Compare scores: fine-tuned vs base model
  4. If fine-tuned scores better → promote to production
  5. If not → adjust training data, retrain

  This loop is all inside Azure AI Foundry.
  No separate tools needed.
```

---

## Topic 17.6 — Content Safety and Responsible AI

---

### 1. Content Safety Built Into Every Deployment

```
When you deploy ANY model in Azure AI Foundry:
  Content Safety filters are ON by default

What it filters (four categories from Module 11.4):
  Hate speech:      racist, discriminatory content
  Violence:         threats, graphic violence
  Sexual content:   explicit material
  Self-harm:        content promoting harm

Severity levels:
  0 = safe
  2 = low severity
  4 = medium severity
  6 = high severity (blocked by default)

You control the threshold:
  Strict:    block severity 2+ (most conservative)
  Balanced:  block severity 4+ (default)
  Lenient:   block severity 6+ (least conservative)
  JM Family: use balanced — enterprise standard
```

---

### 2. Groundedness Detection — New in Foundry

```
Beyond content filtering, Azure AI Foundry adds:

Groundedness Detection:
  Checks if the LLM answer is supported by the retrieved context
  Catches hallucinations BEFORE they reach the user

How it works:
  Your RAG pipeline returns: context chunks + LLM answer
  Groundedness filter compares them
  If answer contains claims NOT in the chunks → flagged
  You can: block the response OR return a safe fallback

JM Family use:
  Invoice amounts and statuses must be grounded in real data
  If LLM invents a figure → groundedness filter catches it
  Returns: "I could not find reliable data for this query"
  Instead of: a hallucinated invoice amount

Configuration in Azure AI Foundry:
  Enable groundedness filter on your deployment
  Set threshold: 0.5-0.8 (higher = stricter)
  Monitor flagged responses in the dashboard
```

---

### 3. Responsible AI Dashboard

```
Azure AI Foundry Responsible AI dashboard shows:

  Fairness metrics:
    Is the model performing equally across demographic groups?
    Are invoice classifications fair across dealer regions?

  Error analysis:
    Where does the model fail most often?
    Which question types get low scores?

  Data exploration:
    What patterns exist in your evaluation dataset?
    Are there gaps in coverage?

  Causal analysis:
    What features drive model decisions?
    Why did the model classify this invoice as high risk?

When to use:
  Before launching a model that affects people or decisions
  Required for enterprise AI governance in most companies
  JM Family: run before deploying dealer-facing assistant
```

---

### 4. Complete JM Family AI Foundry Workflow

```
┌──────────────────────────────────────────────────────────────┐
│               Azure AI Foundry (ai.azure.com)                │
│                                                              │
│  PHASE 1: DISCOVER                                           │
│    Model Catalog → compare GPT-4o mini vs Phi-4             │
│    Pick GPT-4o mini for invoice assistant                    │
│                                                              │
│  PHASE 2: BUILD PROTOTYPE                                    │
│    Prompt Flow → visual RAG pipeline                         │
│    Connect Azure AI Search (invoice index)                   │
│    Add LLM node (GPT-4o mini deployment)                     │
│    Test in UI playground                                     │
│                                                              │
│  PHASE 3: EVALUATE                                           │
│    Upload 50 Q&A test pairs                                  │
│    Run evaluation → groundedness, relevance scores           │
│    Iterate until groundedness ≥ 4.0/5                        │
│                                                              │
│  PHASE 4: FINE-TUNE (if needed)                              │
│    Upload 200 JSONL training examples                        │
│    Train GPT-4o mini for format consistency                  │
│    Re-evaluate fine-tuned model                              │
│    Compare scores: fine-tuned vs base                        │
│                                                              │
│  PHASE 5: DEPLOY                                             │
│    Deploy as REST endpoint (Prompt Flow)                     │
│    OR hand off to Semantic Kernel team for C# production app │
│    Enable Content Safety + Groundedness filters              │
│                                                              │
│  PHASE 6: MONITOR                                            │
│    Track token usage, latency, error rates                   │
│    Alert on groundedness score drops                         │
│    Re-evaluate monthly                                       │
└──────────────────────────────────────────────────────────────┘
```

---

## Module 17 — Self-Test Questions

**Q1.** What is the difference between Azure AI Foundry and Azure OpenAI Studio? Why does this matter in interviews?

> **A:** Azure OpenAI Studio is the older portal supporting only OpenAI models (GPT-4o, embeddings). Azure AI Foundry is the new unified platform (ai.azure.com) that replaced and expanded it — supporting 1,600+ models from OpenAI, Meta, Microsoft, Mistral, and others, plus adding Prompt Flow, built-in evaluation, fine-tuning UI, Content Safety management, and monitoring all in one place. In interviews, saying "Azure OpenAI Studio" for current work signals you are behind — the correct current answer is Azure AI Foundry.

---

**Q2.** A JM Family stakeholder asks you to demonstrate the invoice RAG assistant before approving budget. What is the fastest way to build it?

> **A:** Build a prototype in Azure AI Foundry Prompt Flow — no code required. Create a visual pipeline: Input node → Embedding node (text-embedding-3-small) → Azure AI Search node (invoice index) → Prompt Template node → GPT-4o mini LLM node → Output node. Test in the built-in playground. Deploy as a REST endpoint in one click. The entire prototype can be ready in hours, not days, and is visual enough for stakeholders to understand without technical knowledge.

---

**Q3.** Your RAG pipeline scores groundedness 2.8/5 in Azure AI Foundry evaluation. What does this mean and how do you fix it?

> **A:** Groundedness 2.8/5 means the LLM is frequently generating answers not supported by the retrieved document chunks — it is hallucinating. Root causes: (1) Chunks are too small — not enough context in each chunk. (2) Retrieval is returning irrelevant chunks — wrong content is being sent to the LLM. (3) The prompt template is not instructing the model strongly enough to stay grounded. Fix in order: first improve chunking (larger chunks or parent-child), then improve retrieval (hybrid search, re-ranking), then tighten the prompt ("Answer only using the provided context. If the answer is not in the context, say I don't know").

---

**Q4.** When would you use a serverless API deployment vs managed compute deployment in the Azure AI Foundry model catalog?

> **A:** Serverless API — pay per token, no infrastructure to manage, Microsoft handles scaling. Use for prototyping, low-medium volume, or when trying a new model before committing. Managed compute — dedicated GPU cluster, fixed cost running 24/7, full control over latency and scaling. Use for high-volume production workloads where consistent low latency is needed and the per-token cost of serverless becomes more expensive than dedicated compute. JM Family would start serverless (prototype phase) and evaluate whether volume justifies managed compute.

---

**Q5.** What is groundedness detection in Azure AI Foundry Content Safety and how is it different from the groundedness evaluation metric?

> **A:** The evaluation metric (Topic 17.4) measures groundedness offline — you run a batch test against a dataset to understand average quality before deployment. Groundedness detection in Content Safety (Topic 17.6) works in real time — it checks every live response as it happens, comparing the LLM answer against the retrieved context, and blocks or flags responses that contain claims not supported by the source documents. Evaluation is a quality gate before launch. Content Safety groundedness detection is a live safety net in production.

---

**Q6.** What is the recommended JM Family path: build everything in Prompt Flow or Semantic Kernel?

> **A:** Both, in sequence. Use Prompt Flow to prototype quickly — visual, fast, stakeholder-friendly, no code. Run evaluation in Foundry to validate quality. Once the design is proven, rebuild in Semantic Kernel for production — because Semantic Kernel gives full C# control, enterprise integration (Managed Identity, App Insights, Private Endpoints), custom plugin logic, multi-step agents, and the ability to embed in JM Family's existing .NET application. Prompt Flow is the design and validation environment. Semantic Kernel is the production implementation.

---

## Memory Hooks

- **"Azure AI Foundry = ai.azure.com — one portal for everything"**
- **"Foundry replaced Azure OpenAI Studio — say Foundry in interviews"**
- **"Model catalog = 1,600+ models — compare before committing"**
- **"Serverless = pay per token. Managed compute = pay per hour."**
- **"Prompt Flow = visual prototype. Semantic Kernel = production code."**
- **"Groundedness < 4.0 = fix chunking or retrieval before deploying"**
- **"Evaluation uses GPT-4o as the judge — AI evaluating AI"**
- **"Content Safety is ON by default on every Foundry deployment"**
- **"Groundedness detection = real-time hallucination blocker in production"**
- **"JM Family path: Prompt Flow prototype → evaluate → SK production"**

---

---

## 2026 Updates

| Topic | Update |
|---|---|
| **Agents GA** | AI Foundry Agents builder is now GA (was preview when this module was written). Build, test, publish agents without code. Connect to AI Search (Knowledge), Azure Functions (Tools), Code Interpreter built-in |
| **Connected Agents (multi-agent)** | Foundry now supports Connected Agents — one agent calls another as a sub-agent via standard API. Build orchestrator + specialist pattern fully in portal |
| **Content Understanding** | New capability — structured extraction from documents, images, video, audio in one API. Wraps Document Intelligence + Vision + Speech into a unified extraction pipeline |
| **Evaluation GA** | Foundry Evaluation pipelines now GA. Built-in evaluators: Groundedness, Relevance, Coherence, Fluency, Violence/Hate/Sexual content. Run against a golden dataset before promoting a prompt or model |
| **Tracing GA** | Azure AI Foundry Tracing — see every LLM call, tool call, retrieval, latency, and token usage in a visual trace. Integrated with OpenTelemetry. Essential for debugging agent behavior |
| **Model routing** | New feature — route queries to different models based on complexity. Simple queries → GPT-4o mini, complex queries → GPT-4o or o1. Cost-efficient without sacrificing quality |

---

## Interactive Learning Ideas

### Exercise 1 — Build a JMA Agent in Foundry (30 min)
In ai.azure.com → Agents → Build:
- System prompt: JMA dealer support assistant
- Knowledge: connect your AI Search index (srch-jma-stg-indexer)
- Tool: add Code Interpreter (for invoice calculations)
- Test: ask a question that requires both document retrieval AND calculation
- Publish to web app — note the URL and share with yourself

### Exercise 2 — Evaluation Pipeline (20 min)
In AI Foundry → Evaluate:
- Create a golden dataset: 10 questions + ideal answers from your JMA RAG content
- Run a Groundedness evaluation against your deployed agent
- Run a Relevance evaluation
- Check scores — which questions fail? Why?
- What would you change in the system prompt or knowledge to fix failures?

### Exercise 3 — Tracing a Failed Agent Response (15 min)
Intentionally ask your Foundry agent a question it should fail on (something not in your knowledge base).
- Go to Tracing in Foundry
- Find the trace for that call
- Trace: what did it retrieve? What was the retrieval score? What did GPT-4o generate?
- At which step did it go wrong? (retrieval miss vs generation hallucination)

### Exercise 4 — Foundry vs SK Decision
JMA wants to build two AI features this quarter:
1. **Dealer FAQ chatbot** — answers common questions, used by 500 dealers, needs to go live in 2 weeks, non-technical PM is building it
2. **Automated invoice validation agent** — 10 custom business rules, integrates with SAP API, needs to run 10,000 invoices/night, JMA IT team owns it

For each: AI Foundry portal approach OR Semantic Kernel C# code approach? Justify.

---

*Previous: Module 16 — Prompt Engineering*
*Next: Module 18 — AI Solution Architecture*
*Updated: 2026-06-30*

---

## Appendix — Merged from Legacy Notes

> Consolidated 2026-07-18 during library reorganization. Source: `07b-Agents-Tool-vs-Knowledge-vs-FineTune.md`, `07-AI-Agents-JMA-RealWorld.md`, `Azure-AI-Foundry-Hierarchy-Index.md`.

### 1. Tool vs Knowledge (RAG) vs Fine-Tune — The Deciding Factor

Foundry lets you attach all three to an agent. The question architects get stuck on is *which one for which data*. The answer is **speed of change**:

```
SPEED OF CHANGE decides everything:

Changes every few MONTHS/YEARS    →  Fine-tune
 └── Model behavior, tone, style
     "Always respond like a Toyota advisor"

Changes every few DAYS/WEEKS      →  RAG (Knowledge)
 └── Documents, specs, policies
     "RAV4 2024 specs PDF"

Changes every MINUTE/HOUR/SECOND  →  Tool (API)
 └── Live transactional data
     "Current inventory count"
     "Available slots right now"
```

| Question | Answer |
|---|---|
| How should it **behave**? | Fine-tune |
| What should it **know**? | RAG / Knowledge |
| What is happening **right now**? | Tool |

---

### 2. Why RAG Cannot Replace a Tool for Live Data

RAG *can* technically be refreshed constantly — but every update runs a pipeline:

```
New document / data arrives
        ▼
Step 1: Chunk the document        ← takes time
        ▼
Step 2: Embed each chunk          ← costs money per token
        ▼
Step 3: Index into AI Search      ← takes time
        ▼
Step 4: Available to Agent

Total time: seconds to minutes per update
```

| Update frequency | Viable? |
|---|---|
| Every few months | Perfect |
| Every few weeks | Great |
| Every few days | Fine |
| Every few hours | Possible — watch embedding costs |
| Every few minutes | Possible — heavy pipeline |
| Every few seconds | Use a Tool instead |
| Real-time / live | Definitely use a Tool |

```
Wrong:  Inventory DB → re-index RAG every second → Agent searches RAG
Right:  Inventory DB → REST API → Agent calls Tool → gets live count
```

At a re-index cost of ~30 seconds per run, a per-second data source produces a permanent backlog: the index is always stale, cost climbs continuously, and the agent sees contradictory values across turns.

---

### 3. Decision Tree — Which Capability to Attach

```
Is the data LIVE / real-time?
 ├── YES → TOOL (API call)
 └── NO  → Is it a calculation or action?
            ├── YES → TOOL (function call)
            └── NO  → Is it static reference text?
                       ├── YES → KNOWLEDGE (RAG)
                       └── NO  → Is it about HOW to behave?
                                  ├── YES → FINE-TUNE
                                  └── NO  → System prompt
```

| Data Type | Example | Solution | Why |
|---|---|---|---|
| Live inventory | Cars in stock right now | Tool | Changes every hour |
| Live pricing | Today's APR rate | Tool | Changes weekly |
| Live slots | Test drive availability | Tool | Changes every minute |
| Vehicle specs | RAV4 engine, mpg, features | RAG | Changes yearly |
| Warranty terms | Coverage details | RAG | Changes occasionally |
| Dealer policies | Trade-in rules | RAG | Changes rarely |
| Brand tone | Professional, friendly | Fine-tune | Trained once |
| Domain language | Toyota terminology | Fine-tune | Trained once |
| Response style | Always upsell warranty | Fine-tune | Trained once |

> **Fine-tune** shapes HOW the model talks. **RAG** gives it WHAT to read.
> **Tool** gives it WHAT IS HAPPENING RIGHT NOW.

---

### 4. JMA Worked Example — Dealer Support Agent End-to-End

A production-shaped Foundry agent showing the Think → Act → Observe loop across four JMA APIs.

**The request** — a dealer rep pastes a customer call into the JMA Dealer Portal:

> *"Sarah Johnson has a 2021 Toyota Camry to trade in. She wants a RAV4 hybrid, budget around $45,000. What would her monthly payments be, and when can she come in for a test drive?"*

**The agent loop:**

```
THINK: I need 4 things:
       1. Trade-in value for 2021 Camry
       2. RAV4 Hybrid inventory availability
       3. Monthly payment calculation
       4. Test drive availability

STEP 1
THINK:  Get trade-in value first
ACT:    → TradeIn Tool  { "make":"Toyota","model":"Camry","year":2021,"zip":"33301" }
OBSERVE:{ "trade_in_value":"$18,500","condition":"Good","market_demand":"High" }

STEP 2
THINK:  Check RAV4 Hybrid inventory
ACT:    → Inventory Search Tool  { "model":"RAV4 Hybrid","max_price":45000,
                                   "dealer":"Fort Lauderdale" }
OBSERVE:{ "results":[
            { "vin":"JTMRWRFV4PD123456","model":"RAV4 Hybrid XLE",
              "color":"Midnight Black","msrp":42500,"status":"In Stock" },
            { "vin":"JTMRWRFV4PD789012","model":"RAV4 Hybrid XLE Premium",
              "color":"Blueprint","msrp":44800,"status":"In Stock" } ] }

STEP 3
THINK:  Net price = $42,500 − $18,500 = $24,000
ACT:    → Finance Calculator Tool  { "vehicle_price":42500,"trade_in":18500,
                                     "credit_score_range":"good","term_months":60 }
OBSERVE:{ "monthly_payment":"$487","apr":"4.9%","total_cost":"$29,220" }

STEP 4
THINK:  Check test drive availability
ACT:    → Scheduling Tool  { "dealer":"Fort Lauderdale",
                             "vehicle_vin":"JTMRWRFV4PD123456","available_days":7 }
OBSERVE:{ "slots":["Mon 10:00 AM","Mon 2:00 PM","Tue 11:00 AM"] }

THINK:  I have everything. Compose final response.
STOP
```

**The agent definition behind it:**

```
Agent Name:    JMA Dealer Support Agent
Brain:         GPT-4o
Deployed on:   AI Foundry → Build & customize → Agents

Instructions (System Prompt):
  "You are a JMA Southeast Toyota dealer support agent.
   When a dealer submits a customer request:
   1. Always check trade-in value if customer has a vehicle
   2. Always search current inventory matching customer needs
   3. Always calculate financing with trade-in applied
   4. Always show next 3 available test drive slots
   Never make up inventory or pricing — always use tools."

Tools Connected:
 ├── TradeIn Tool        → JMA pricing API
 ├── Inventory Search    → Azure AI Search (toyota-inventory-index)
 ├── Finance Calculator  → JMA finance API
 └── Scheduling Tool     → JMA appointment API

Knowledge:
 └── RAG Index           → Toyota specs, features, comparison docs
```

**Why it matters — the business case:**

```
WITHOUT AGENT (today):               WITH AGENT:
─────────────────────                ───────────
Dealer opens 4 tabs:                 Dealer types one sentence
 Tab 1: Trade-in tool                Agent does everything
 Tab 2: Inventory system             Response in 8 seconds
 Tab 3: Finance calculator           One screen, complete answer
 Tab 4: Scheduling system            Dealer just clicks Book

Takes: 10-15 minutes                 Takes: 8 seconds
Risk: manual errors                  Risk: near zero
```

Note how the three capability types divide cleanly in this agent: the four tools are all live data, the RAG index is all slow-changing reference text, and the "always upsell warranty / JMA brand voice" behavior sits in the system prompt (and would move to a fine-tune only if the prompt proved insufficient at scale).

---

### 5. Azure AI Foundry — Complete Capability Taxonomy

A reference map of everything that exists under a Foundry resource. Useful as an interview checklist and for scoping architecture reviews.

**① Foundry Resource**

| Area | Options |
|---|---|
| **Model families** | OpenAI (GPT-5/5-chat, GPT-4o, GPT-4.1/4.1-nano, o3/o4-mini, DALL·E, GPT-image-1, Whisper); Microsoft (Phi-4, Phi-3.5); Partner (Llama 3.1/3.2, Mistral Large/NeMo, Cohere Command R+/Rerank v4, xAI Grok 3/Grok 4 Fast, DeepSeek, Kimi/Moonshot, Hugging Face 10,000+); Model Router |
| **Deployment types** | Global Standard, Provisioned Throughput (PTU), Serverless Endpoint (MaaS), Managed Compute |
| **Security** | Managed Identity, Network / Private Endpoints, RBAC roles: Foundry Account Owner, Foundry Owner, Foundry User, Foundry Project Manager |
| **Connections — Azure** | AI Search, Storage, Cosmos DB, Azure OpenAI, Application Insights, Key Vault, Databricks, APIM |
| **Connections — Microsoft** | SharePoint, Microsoft Fabric, another Foundry resource |
| **Connections — grounding** | Grounding with Bing Search, Bing Custom Search, Serp |
| **Connections — external** | OpenAI direct, Serverless Model, Model Gateway, API Key, Custom Key |

**② Foundry Project (New Gen)**

```
Agent Service
 ├── Agent types:      Prompt Agents | Hosted Agents
 ├── Core components:  System Prompt · Model Deployment · Threads · Runs · Steps
 ├── Agent tools:      Azure AI Search · Bing Web Search · Function Calling ·
 │                     Code Interpreter · File Search · MCP Server ·
 │                     Browser Automation · A2A Protocol
 ├── Memory Store:     long-term memory across sessions,
 │                     automatic extraction + consolidation
 └── Multi-agent:      visual workflow builder · Agent-to-Agent (A2A) calls ·
                       Connected Agents / sub-agent delegation

Playground        Chat · Agents · Image · Audio · Compare Mode
Evaluations       Groundedness · Relevance · Coherence · Fluency ·
                  Safety · Task Completion · Custom Evaluators
Observability     Trace (OTel exporter, LangChain/AutoGen/OpenAI SDK support)
                  Evaluate in production (single-turn + multi-turn scoring)
                  Monitor (real-time detection, alerts, Azure Monitor)
                  Optimize (production signal analysis, ranked suggestions)
Fine-tuning Jobs  LoRA · QLoRA · DPO · Developer Tier
Files & Data      Uploaded Files · Vector Indexes · Azure Blob Storage
Project Endpoint  REST API (GA: 2025-05-01)
```

**③ Hub-Based Project (Classic / AML)** — the older shape you will still meet in existing subscriptions. Provides Prompt Flow (Standard / Chat / Evaluation flows), fine-tuning, evaluations, managed compute, and connections. Creating one **auto-provisions four resources**: Azure Storage Account, Key Vault, Application Insights, Container Registry. New-gen Foundry projects do not require this.

**④ Foundry Tools (prebuilt AI)** — Speech (STT, TTS, real-time translation); Vision (image analysis, OCR, Face API, object detection); Language (sentiment, NER, key phrase, PII, language detection, summarization, CQA, CLU); Document AI (Form Recognizer, layout, prebuilt invoice/receipt/W9/ID); Translator (real-time + batch document); Content Safety (harmful content filter, jailbreak detection, groundedness detection).

**⑤ Developer Experience**

| Surface | Notes |
|---|---|
| Foundry Portal | ai.azure.com |
| Foundry SDK | `azure-ai-projects` v2 — Agents, Inference, Evaluations, Memory. Python, .NET/C#, JS/TS |
| REST API | GA version 2025-05-01 |
| VS Code extension (Foundry Toolkit) | Browse + deploy models, build + deploy hosted agents, "Open in VS Code" from portal, generate sample code |
| Azure Developer CLI | `azd ai agent init` → `azd up` |
| Foundry Local | Run models on-device — Windows NPU / CPU, CLI + SDK |

**⑥ Security & Governance** — Entra ID (OAuth2/OIDC identity, RBAC assignments, Conditional Access); Key Vault (secrets/keys/certs, BYO vault); Private Endpoints + VNet (isolation, Azure Firewall/DDoS, hub-spoke); Azure Policy (governance rules, tagging, locks); Azure Monitor + Diagnostics (platform logs, Cost Management unified billing); Responsible AI (Content Safety filters, fairness evaluation, transparency reports).
