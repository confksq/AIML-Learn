# Q&A — L17: Azure AI Foundry
**Source chapter:** `01_Lessons/Part4_Architecture/L17_AzureAIFoundry.md` | **Format:** self-study
**Questions:** 30 | *No overlap with the interview bank or the chapter's own self-test — these drill the chapter's specifics.*

---

## What It Is

**Q1. Give the one-line definition and the portal URL.**
Azure AI Foundry = Microsoft's **unified platform for building, evaluating, and deploying AI applications** — one portal at **ai.azure.com** replacing the old scattered tools (Marketplace, OpenAI Studio, manual RAG/eval, separate Content Safety, App Insights).

**Q2. Foundry vs Azure OpenAI Studio — the difference, and why it matters in interviews.**
Azure OpenAI Studio (old) supported **only OpenAI models** (GPT-4o, embeddings). Foundry (new) supports **1,600+ models** from OpenAI, Meta, Microsoft, Mistral, Cohere, etc., plus Prompt Flow, built-in evaluation, fine-tuning UI, Content Safety management, and monitoring. Saying "Azure OpenAI Studio" for current work signals you're behind — the correct current answer is **Azure AI Foundry**.

**Q3. Foundry vs Semantic Kernel — how do they relate?**
Foundry is a **portal/platform** (visual UI — browse, test, evaluate, deploy; good for prototyping/non-coders). SK is a **code SDK** (C#/Python — build production apps, custom logic). They work together: use Foundry to find the model, test the RAG pipeline, evaluate, and deploy the endpoint; use SK to build the C# production app that calls it. JMA path: **prototype/evaluate in Foundry → build production in SK.**

**Q4. Name Foundry's six key components.**
Model Catalog (1,600+ models), Prompt Flow (visual RAG builder), Evaluation Framework (groundedness/relevance/coherence/fluency), Fine-Tuning (UI, no GPU setup), Content Safety (filter + groundedness detection), Monitoring (token usage, latency, error rates, cost).

---

## Model Catalog

**Q5. What is the Model Catalog, and name six model families in it.**
A marketplace of 1,600+ AI models to browse, compare, and deploy in one place: OpenAI (GPT-4o, o1/o3), Microsoft (Phi-3, Phi-4), Meta (Llama 3.x), Mistral (Large/Small), Cohere (Command R/R+), Stability AI (Stable Diffusion).

**Q6. Serverless API vs Managed Compute deployment — the trade-off.**
**Serverless API** — pay per token, no GPU to manage, Microsoft hosts; best for low-medium volume and prototyping. **Managed Compute** — you provision a dedicated GPU cluster, fixed cost running 24/7, full runtime control; best for high volume with consistent-latency needs. JMA starts serverless, moves to managed compute only if volume justifies it.

**Q7. Match model to task: document Q&A/RAG, complex reasoning, simple classification, image understanding.**
Document Q&A/RAG → GPT-4o mini (good balance). Complex reasoning → GPT-4o or o1. Simple classification → Phi-3 mini (cheap/fast). Image understanding → GPT-4o (multimodal). JMA: invoice assistant → GPT-4o mini; executive summaries → GPT-4o; high-volume classification → fine-tuned GPT-4o mini.

**Q8. What's the architect's point about model comparison in Foundry?**
Foundry lets you compare models side by side (same question → GPT-4o mini vs Llama vs Phi-4; see quality, tokens, latency, cost). The architect's job isn't "use GPT-4o because it's famous" — it's **compare options, measure for your specific task, choose the best cost-quality fit** with data.

---

## Prompt Flow

**Q9. What is Prompt Flow, and what's its building block?**
A **visual drag-and-drop pipeline builder** for RAG/AI workflows — no code for a basic pipeline. Each step is a **node** on a canvas; connect nodes with arrows; test, iterate, deploy in the UI.

**Q10. List the RAG node sequence in Prompt Flow.**
Input → **Embed Query** (text-embedding-3-small → vector) → **Vector Search** (Azure AI Search → top-5 chunks) → **Prompt Template** (system + chunks + question) → **LLM Node** (GPT-4o mini generates) → Output (answer + citations).

**Q11. Name six Prompt Flow node types and what each does.**
LLM Node (calls a deployed model), Embedding Node (text→vector), Search Node (queries AI Search), Prompt Template (builds the prompt string), Python Node (custom code/logic/transforms), Condition Node (if/else branching to route by intent), Input/Output (flow start/end).

**Q12. Prompt Flow vs Semantic Kernel — when each, and the typical JMA path?**
Prompt Flow: prototyping, stakeholder demos, non-developer maintainers, standard RAG, built-in evaluation, quick REST endpoint. SK: production C# apps, complex business logic beyond nodes, full .NET integration (DI/logging/auth), multi-step agents with custom plugins, Managed Identity/App Insights/Private Endpoints. Path: **Week 1 prototype in Prompt Flow → Week 2+ rebuild in SK for production.**

**Q13. How do you deploy a Prompt Flow, and when is the deployed endpoint enough (no SK)?**
Click Deploy → choose compute (serverless/managed) → Azure auto-creates a REST endpoint (`POST .../score` with a question, returns answer + sources). Your C# app can just `HttpClient` POST — no SK needed for **simple RAG** (no agents, no complex logic), quick prototype-to-production, or non-.NET frontends.

---

## Evaluation

**Q14. Why evaluate before deploying, in one contrast?**
Without evaluation: it "seems to work," you deploy, users report wrong answers, and you have **no data to diagnose**. With evaluation: you measure quality first (groundedness 3.2/5, relevance 4.1/5), iterate until acceptable, deploy with confidence, and keep a baseline for future comparison.

**Q15. Name the five evaluation metrics and their scales.**
Groundedness (answer supported by retrieved docs? 1–5, most important for RAG), Relevance (addresses the question? 1–5), Coherence (logically structured? 1–5), Fluency (natural language? 1–5), Similarity (vs known-correct answer, 0–1 cosine, needs ground truth).

**Q16. Walk the 5 steps of running an evaluation in Foundry.**
(1) Prepare an evaluation dataset (20–50+ question/expected-answer pairs, ideally real historical Q&A) → (2) connect your RAG pipeline (Prompt Flow or SK endpoint) → (3) run — **Foundry uses GPT-4o as the evaluator**, scoring each metric per question → (4) read aggregate scores → (5) iterate (low groundedness → fix chunking/retrieval; low relevance → fix prompt template).

**Q17. What are JMA's recommended production thresholds, and the golden rule?**
Groundedness ≥ 4.0/5, Relevance ≥ 4.0/5, Coherence ≥ 3.5/5, Fluency ≥ 3.5/5 — below threshold, don't deploy; fix and re-evaluate. Golden rule: **never deploy without evaluation** — a score is evidence you can show leadership ("our RAG scored 4.2 groundedness").

**Q18. A pipeline scores groundedness 2.8/5 — what does it mean and the fix order?**
The LLM is frequently generating answers **not supported by retrieved chunks** — hallucinating. Causes: chunks too small, retrieval returning irrelevant chunks, or a weak prompt. Fix in order: **improve chunking** (larger/parent-child) → **improve retrieval** (hybrid + re-ranking) → **tighten the prompt** ("answer only from context; if not present, say I don't know").

---

## Fine-Tuning & Content Safety in Foundry

**Q19. What's the Foundry fine-tuning UI flow, vs the SK-code approach from L14?**
Same result, no code: go to Fine-tuning → choose base model (GPT-4o mini) → drag-drop training JSONL (and optional validation) → set hyperparameters (or auto) → Start training job → monitor loss curves in the UI (visual overfitting detection — validation loss rising = stop) → deploy from the same UI. Best practice loop: **fine-tune → deploy → evaluate vs base → promote if better, else adjust data and retrain** — all inside Foundry.

**Q20. What Content Safety is on by default, and what are the threshold presets?**
On every Foundry deployment: filters for **Hate, Violence, Sexual, Self-harm** on a 0–6 severity scale. Presets: Strict (block 2+), **Balanced (block 4+, default)**, Lenient (block 6+). JMA uses Balanced (enterprise standard).

**Q21. Groundedness detection (Content Safety) vs groundedness evaluation metric — the difference.**
The **evaluation metric** measures groundedness **offline** in a batch test against a dataset — a quality gate before launch. **Groundedness detection** (Content Safety) runs in **real time** on every live response, comparing the answer against retrieved context and blocking/flagging unsupported claims — a live hallucination safety net in production. Configure with a threshold 0.5–0.8 (higher = stricter).

**Q22. What does the Responsible AI dashboard show, and when do you run it?**
Fairness (equal performance across groups/regions), Error analysis (where the model fails most), Data exploration (dataset patterns/gaps), Causal analysis (what drives decisions). Run it **before launching any model that affects people or decisions** — typically required for enterprise AI governance.

**Q23. Recite the six phases of the JMA Foundry workflow.**
(1) **Discover** (Model Catalog — compare, pick GPT-4o mini) → (2) **Build prototype** (Prompt Flow visual RAG + AI Search) → (3) **Evaluate** (50 Q&A pairs, iterate to groundedness ≥ 4.0) → (4) **Fine-tune** if needed (200 JSONL examples, re-evaluate vs base) → (5) **Deploy** (REST endpoint or hand to SK team; enable Content Safety + groundedness) → (6) **Monitor** (token usage, latency, errors; alert on groundedness drops; re-evaluate monthly).

---

## 2026 Updates & Applied

**Q24. What are Foundry Agents and Connected Agents (2026 GA)?**
**Agents builder** is GA — build/test/publish agents **without code**, connecting AI Search (Knowledge), Azure Functions (Tools), and built-in Code Interpreter. **Connected Agents** support multi-agent — one agent calls another as a sub-agent via standard API, building the orchestrator+specialist pattern fully in the portal.

**Q25. What is Foundry Tracing, and why is it essential?**
A visual trace of **every LLM call, tool call, retrieval, latency, and token usage** — integrated with OpenTelemetry. Essential for **debugging agent behavior**: you can see exactly which step (retrieval miss vs generation hallucination) caused a failure.

**Q26. What is Content Understanding (2026)?**
A new capability for **structured extraction from documents, images, video, and audio in one API** — wrapping Document Intelligence + Vision + Speech into a unified extraction pipeline.

**Q27. What is model routing in Foundry (2026)?**
Route queries to different models by complexity — simple queries → GPT-4o mini, complex → GPT-4o or o1 — cost-efficient without sacrificing quality (the tiering pattern, now a portal feature).

**Q28. In the Foundry-vs-SK decision (Exercise 4): a 2-week dealer FAQ chatbot built by a non-technical PM, vs an invoice-validation agent with 10 business rules integrating SAP at 10k/night owned by IT — which each?**
Dealer FAQ chatbot → **Foundry portal** (fast, visual, non-technical maintainer, standard RAG, tight timeline). Invoice-validation agent → **Semantic Kernel** (10 custom business rules, SAP API integration, high-volume batch, owned by IT — needs full code control and enterprise integration).

**Q29. Tracing a failed agent response (Exercise 3) — what do you inspect and what distinction are you making?**
Open the trace for the failed call and inspect: what did it retrieve, what was the retrieval score, what did GPT-4o generate. You're determining **at which step it went wrong** — a **retrieval miss** (right chunk never retrieved) vs a **generation hallucination** (chunk retrieved but the model ignored/misused it) — because the fixes differ (retrieval/chunking vs prompt/grounding).

**Q30. Building a JMA agent in Foundry (Exercise 1) — what three things do you wire up?**
A **system prompt** (JMA dealer support assistant), **Knowledge** (connect the AI Search index, e.g. srch-jma-stg-indexer), and a **Tool** (e.g. Code Interpreter for invoice calculations) — then test with a question needing both retrieval AND calculation, and publish to a web app.

---

*Curriculum Q&A Batch E — file 1 of 3. Next: QA_L18 (AI Solution Architecture).*
