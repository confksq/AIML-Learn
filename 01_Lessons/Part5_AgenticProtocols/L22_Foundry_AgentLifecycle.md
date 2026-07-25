# Module 01 — Azure AI Foundry: Platform, Agent Lifecycle, and Healthcare Architecture

---

## Why This Module Matters

The interviewer confirmed focus on "entire AI agent workflow end-to-end." Azure AI Foundry IS the platform where that workflow lives. Every other module connects back to this one. You will be asked:
- "Walk me through how Azure AI Foundry works."
- "How do you go from prototype to production in Foundry?"
- "What's the difference between a Hub and a Project?"

Your anchor: You have built and deployed agents in AI Foundry at JM Family. You've published to Teams and M365 Copilot.

---

## Section 1 — What Azure AI Foundry IS

**Azure AI Foundry** (portal: ai.azure.com) is Microsoft's unified platform for building, testing, evaluating, and deploying AI applications and agents. It replaced Azure OpenAI Studio and Azure Machine Learning Studio as the single entry point for enterprise AI work.

**The mental model:** Think of AI Foundry like an **aircraft carrier**. The carrier (Foundry) is the platform. The planes (agents, models, pipelines) are what you build and launch from it. The carrier provides fuel (compute), weapons (models), navigation (evaluation), and communication (deployment endpoints). You don't build agents in isolation — you build them on the carrier.

**What Foundry gives you:**
- Access to 1,600+ models (GPT-4o, Llama, Mistral, Phi, DALL-E, Whisper, and more)
- Agent builder with tools, knowledge, memory, and guardrails
- Evaluation framework (groundedness, relevance, coherence, fluency)
- Fine-tuning UI (no-code, monitors loss curves)
- Prompt Flow (visual pipeline builder — now largely replaced by Agents UI)
- Content Safety (built-in filters, groundedness detection)
- LLMOps (prompt versioning, A/B testing, model rollback)

---

## Section 2 — The Hierarchy: Hub → Project → Resources

This is the most common confusion point. Burn this into memory.

```
Azure AI Foundry Hub        ← Azure resource (what you see in portal.azure.com)
    └── Project             ← workspace where you do your work (ai.azure.com)
            ├── Agents      ← your AI agents
            ├── Models      ← model deployments (GPT-4o, embeddings, etc.)
            ├── Knowledge   ← files, indexes, data sources
            ├── Evaluations ← test runs, quality scores
            └── Tools       ← MCP tools, functions, APIs
```

**Hub** = the Azure infrastructure layer. It holds shared compute, networking, storage, and security config. One hub can serve multiple teams/projects. You create this in portal.azure.com.

**Project** = the workspace. This is where you actually build. Each project has its own agents, deployments, and evaluations. A project inherits the hub's infrastructure but has its own access controls. You work here at ai.azure.com.

**Healthcare example:**
- Hub: `vitalcare-foundry-hub` — shared across the entire hospital network (180 hospitals)
- Project 1: `prior-auth-team` — builds the Prior Auth agent
- Project 2: `ambient-docs-team` — builds the Ambient Documentation agent
- Project 3: `member-selfservice-team` — builds the Member Self-Service chatbot
- Each team has isolated access but shares the same GPT-4o deployment and Azure AI Search index

**JM Family anchor:**
"At JM Family I have `aiml-learn-resource` as my hub, with `ai-learn` as the project inside it. All my agents — JMAVehicleIQA and others — live in that project."

> **⚙️ Config or Code?**
> - **Portal Config only:** Create Hub (portal.azure.com) → create Project (ai.azure.com) → assign RBAC roles → set shared resources (Key Vault, Storage, AI Search)
> - **Custom Code:** Nothing — hierarchy setup is 100% portal clicks
> - **Both:** Connecting SK to a Project model deployment (portal deploys the model, code builds the connection via KernelBuilder + endpoint URL)

---

## Section 3 — The 8 Building Blocks of AI Foundry

When the interviewer asks "how does AI Foundry work?" — walk them through these 8 blocks.

| Building Block | What it is | Healthcare example |
|---|---|---|
| **Agents** | AI agents with tools, knowledge, memory | Prior Auth agent, Ambient Doc agent |
| **Models** | Deployed models (GPT-4o, embeddings, Whisper) | GPT-4o for generation, text-embedding-3-large for search |
| **Tools** | Functions, APIs, MCP tools the agent can call | FHIR API, Payer eligibility API, Lab system |
| **Knowledge** | Files, Azure AI Search indexes, SharePoint | Clinical guidelines, formulary, policy docs |
| **Memory** | Short-term (conversation) + long-term (vector store) | Patient context across sessions |
| **Evaluations** | Groundedness, relevance, coherence scoring | Run before every deployment — quality gate |
| **Fine-tuning** | Custom model training on your data | Train GPT-4o mini on prior auth approval patterns |
| **Guardrails** | Content Safety filters, groundedness detection | Block hallucinated drug names before output |

> **⚙️ Config or Code?**
> - **Portal Config only:** Agents (UI builder), Models (deploy from catalog), Knowledge (upload files), Evaluations (run in Foundry), Fine-tuning (no-code UI), Guardrails (toggle Content Safety filters)
> - **Custom Code:** Tools (write the Azure Function / API the tool calls), Memory (connect Cosmos DB or Redis in SK code)
> - **Both:** Knowledge (upload files = Config; connect AI Search index to SK = Code)

---

## Section 4 — Agent Lifecycle in AI Foundry

This is what "end-to-end agent workflow" means at the platform level.

```
1. BUILD       → Create agent in Foundry Agents UI
                 Set system prompt, attach tools, add knowledge files
                 
2. TEST        → Playground — chat with the agent, test edge cases
                 Does it hallucinate? Does it use the right tool?
                 
3. EVALUATE    → Run evaluation dataset through the agent
                 Score groundedness ≥ 0.85, relevance ≥ 0.80
                 Quality gate: if scores pass → proceed; if fail → fix prompt
                 
4. DEPLOY      → Publish as REST endpoint OR publish to Teams/M365 Copilot
                 Blue-green deployment: v2 gets 10% traffic, monitor, then 100%
                 
5. MONITOR     → Azure Monitor + App Insights
                 Track: token usage, latency, groundedness drift, cost
                 Alert: if groundedness drops below 0.85 → auto-rollback
                 
6. ITERATE     → Update system prompt → re-evaluate → re-deploy
                 Prompt versioned in Git, evaluation history tracked in Foundry
```

**Healthcare example — Ambient Documentation agent lifecycle:**
1. **Build**: System prompt = "You are a clinical documentation assistant. Draft SOAP notes from physician dictation. Never invent clinical findings. If unsure, flag for physician review."
2. **Test**: Playground — feed 20 sample dictations, check if SOAP structure is correct
3. **Evaluate**: Groundedness score on 100 test cases. Threshold: ≥ 0.90 (clinical = higher bar than normal)
4. **Deploy**: Publish to Teams channel for pilot physicians at 2 hospitals
5. **Monitor**: Track latency (target <3s), groundedness drift weekly, token cost per note
6. **Iterate**: Physicians flag that ICD-10 codes are sometimes wrong → update system prompt → re-evaluate → redeploy

---

## Section 5 — Foundry vs Semantic Kernel: When to Use Which

This is the decision you make every time you start a new AI project.

| | Azure AI Foundry | Semantic Kernel |
|---|---|---|
| **Best for** | Prototype, evaluate, fine-tune, no-code deploy | Production C# app, custom orchestration |
| **Who uses it** | AI engineers, business analysts, PMs | Software engineers (.NET) |
| **Agent building** | Visual UI, drag and drop tools | Code — KernelFunction, Planner, ChatHistory |
| **Evaluation** | Built-in automated eval with scoring | Manual — you write eval harness |
| **Deployment** | One-click to Teams, M365, REST endpoint | You build the API, host it, wire the auth |
| **Flexibility** | Less — constrained to Foundry patterns | More — full control of every call |
| **PHI control** | Platform handles some guardrails | You implement all guardrails in code |

**⚙️ Config or Code?**
- **Portal Config only:** BUILD (Agents UI — system prompt, tools, knowledge), TEST (Playground), DEPLOY (publish to Teams / M365 / REST endpoint), MONITOR dashboards
- **Custom Code:** Dataset prep for EVALUATE (write JSONL test cases), CI/CD pipeline YAML (trigger eval on prompt change), auto-rollback script
- **Both:** EVALUATE (run in Foundry = Config; CI/CD quality gate = Code)

**The JM Family roadmap (and your interview answer):**
> "We use Foundry to prototype and evaluate. Once the agent passes our quality gate (groundedness ≥ 0.85), we re-implement it in Semantic Kernel for production. Foundry gives us the fast feedback loop — we can iterate on prompts and test with real clinical data in hours. SK gives us production control — Managed Identity, FunctionInvocationFilter for audit logging, custom error handling, and integration with our existing .NET service layer."

---

## Section 6 — Evaluation Deep Dive (Clinical = Higher Bar)

Evaluation is what separates toy agents from production agents. In healthcare, the bar is higher than standard enterprise.

**The 4 standard metrics:**
1. **Groundedness** — is every claim in the response supported by the retrieved context? (most important for clinical)
2. **Relevance** — does the response answer the actual question asked?
3. **Coherence** — is the response logically structured and readable?
4. **Fluency** — is the language natural and grammatically correct?

**How evaluation works in Foundry:**
- You provide a golden dataset: `{question, context, expected_answer}`
- Foundry sends each row through your agent
- GPT-4o acts as the judge — scores each response 1-5 on each metric
- Foundry aggregates scores and shows a dashboard

**Clinical thresholds (what you'd set):**
- Groundedness ≥ 0.90 (clinical — higher than standard 0.85)
- Relevance ≥ 0.80
- Coherence ≥ 0.75
- Fluency ≥ 0.80

**Quality gate in CI/CD:**
```
Prompt change committed to Git
        ↓
CI pipeline runs eval dataset (100 test cases)
        ↓
If groundedness ≥ 0.90 → deploy to staging
If groundedness < 0.90 → pipeline fails, alert engineer
        ↓
Staging: 10% traffic (blue-green)
        ↓
If no alerts after 24h → 100% traffic
```

---

## Section 7 — Content Safety and Guardrails

Every agent you deploy in healthcare must have guardrails. Foundry has two layers built in.

**Layer 1 — Input/Output filters (Content Safety):**
- Hate, violence, sexual, self-harm — blocked at configurable severity levels
- PII detection — flag or redact patient identifiers before they reach the LLM
- Prompt injection detection — blocks attempts to override your system prompt

**Layer 2 — Groundedness detection (real-time hallucination blocker):**
- Before the response goes to the user, Foundry checks: is every claim in the response supported by the retrieved documents?
- If a claim has no source → Foundry blocks or flags it
- This is the most important guardrail for clinical use — it catches hallucinated drug names, invented lab values, fabricated clinical findings

**⚙️ Config or Code?**
- **Portal Config only:** Enable Content Safety (toggle in Foundry project settings), set severity thresholds (portal sliders), enable groundedness detection (Foundry agent settings toggle), PII detection (portal category selection)
- **Custom Code:** Custom output validator in SK (`FunctionInvocationFilter`), ICD-10 format regex check, structured output schema validation
- **Both:** Prompt injection defense (Content Safety = Config in portal; system prompt hardening = you write the prompt text)

**What you say in the interview:**
> "In Foundry I enable groundedness detection on every clinical agent. It runs before output — if the agent claims a patient has a penicillin allergy but that's not in the retrieved EHR data, Foundry catches it before it reaches the physician. Combined with Azure Content Safety for PII and prompt injection, those are my two platform-level guardrails. Then in the SK production layer I add a FunctionInvocationFilter that logs every tool call and a custom output validator that checks for ICD-10 code format before the note is saved."

---

---

## Section 8 — CV SKILL: Fine-Tuning vs RAG vs Prompt Engineering

> **CV anchor:** "Applied fine-tuning and model adaptation strategies using Azure AI Foundry — supervised fine-tuning, evaluation dataset design, selecting between fine-tuning versus RAG versus prompt engineering based on latency, cost, data volume, and update-frequency trade-offs"

### The Three Adaptation Options

```
PROMPT ENGINEERING          RAG                     FINE-TUNING
──────────────────          ───                     ───────────
Change what you ASK         Change what you GIVE    Change the MODEL ITSELF
No model change             External knowledge      Train on your data
No training cost            Retrieval cost/call     High upfront training cost
Instant change              Update index = live     Re-train to update
Best for: behavior rules    Best for: knowledge     Best for: style/domain
```

### Decision Framework — when to use each

**Use Prompt Engineering when:**
```
├── Behavior change needed (tone, format, rules, persona)
├── Fast iteration required — change in minutes
├── No domain knowledge gap — model already knows the content
└── Example: "Always output JSON. Never invent clinical findings. 
             Flag uncertainty with 'Recommend physician review.'"
```

**Use RAG when:**
```
├── Knowledge gap — private enterprise data not in training
├── Knowledge changes frequently (drug formulary, payer policies)
├── PHI involved — cannot pre-load patient records
├── Auditability required — cite which source grounded the answer
└── Example: retrieve patient FHIR record + payer policy at query time
```

**Use Fine-Tuning when:**
```
├── Specific OUTPUT FORMAT needed that prompt engineering cannot reliably enforce
│   └── Example: SOAP note always in exact clinical structure
├── Specific VOCABULARY or DOMAIN STYLE needed
│   └── Example: radiology report terminology, ICD-10 coding patterns
├── Latency critical — RAG round-trip too slow
│   └── Example: real-time clinical decision support (<500ms required)
├── Large volume of labeled examples available (1000+ examples minimum)
└── Knowledge is STABLE — not updated frequently
    └── Fine-tuning for something that changes monthly = constant retraining cost
```

### The Decision Matrix

| Factor | Prompt Engineering | RAG | Fine-Tuning |
|---|---|---|---|
| **Update frequency** | Instant | Same day (re-index) | Days-weeks (retrain) |
| **Training cost** | Zero | Zero | High ($100s-$1000s) |
| **Per-query cost** | Lowest | Medium (retrieval + LLM) | Low (no retrieval) |
| **Data required** | None | Documents | 1000+ labeled examples |
| **PHI handling** | Safe | Retrieve only what needed | Risky — PHI in training data |
| **Auditability** | Low | High (cite chunks) | Low |
| **Best outcome** | Behavior rules | Current knowledge | Style/format consistency |

### Supervised Fine-Tuning in Azure AI Foundry

```
Process:
1. Prepare dataset: JSONL format
   {"prompt": "Draft SOAP note from: [dictation text]", 
    "completion": "[perfect SOAP note example]"}
   Minimum: 50 examples. Production quality: 500-1000+

2. Upload to Foundry → Fine-tuning → Select base model (GPT-4o-mini recommended)

3. Training runs → Foundry shows loss curve
   └── Loss should decrease steadily — if it spikes → data quality issue

4. Evaluate fine-tuned model vs base model on held-out test set
   └── Compare groundedness, format accuracy, clinical accuracy

5. Deploy fine-tuned model → A/B test against base model
   └── 10% traffic to fine-tuned → compare quality metrics → promote if better
```

**Healthcare example — when fine-tuning was the right call:**
> "At JM Family we evaluated fine-tuning vs prompt engineering for our SOAP note generation. Prompt engineering produced variable output structure — sometimes the Assessment section came before Plan, sometimes notes were in prose not SOAP format. We fine-tuned GPT-4o-mini on 800 physician-approved SOAP note examples. Result: 100% correct SOAP structure, 40% latency reduction (no RAG round-trip for format), and the fine-tuned model consistently used clinical shorthand the base model never used. The trade-off: we cannot update it in real-time — a change to SOAP format requires a retraining cycle."

**⚙️ Config or Code?**
- **Portal Config only:** Upload dataset to Foundry (drag and drop), select base model (dropdown), start training job (button), view loss curve (dashboard), deploy fine-tuned model (button), A/B traffic split (slider)
- **Custom Code:** Prepare JSONL dataset (Python script to format `{"prompt": ..., "completion": ...}`), evaluate fine-tuned vs base model on test set (Python eval script)
- **Both:** Monitor training (loss curve = Config in portal; custom eval metrics = Code)

### Interview Answer

**Q: When would you choose fine-tuning over RAG?**
> "Fine-tuning is the right choice in three specific scenarios. First, when output format consistency is non-negotiable and prompt engineering cannot reliably enforce it — we fine-tuned for SOAP note structure because the base model with even a detailed system prompt produced variable formats. Second, when latency is critical and you cannot afford the RAG retrieval round-trip — fine-tuning bakes the knowledge in, no retrieval step needed. Third, when the domain vocabulary or style is highly specialized — radiology reports, clinical coding — and you have 500+ labeled examples. The disqualifying factors are: knowledge that changes frequently (monthly payer policy updates mean constant retraining), PHI in training data (huge compliance risk), and small datasets (below 500 examples, prompt engineering will outperform fine-tuning)."

---

## Quick-Reference Interview Answers

**Q: What is Azure AI Foundry and how does it fit in your architecture?**
"AI Foundry is Microsoft's unified platform for building, evaluating, and deploying AI agents and applications. In my architecture it's the prototype and evaluation layer — I build agents there, run them against a golden evaluation dataset, and only promote to production in Semantic Kernel once they pass my quality gate. The hub holds shared infrastructure, the project is the isolated workspace, and inside the project I have agents, model deployments, knowledge files, and evaluation runs."

**Q: How do you go from idea to production with an agent?**
"Build in Foundry Agents UI → test in Playground → evaluate with automated scoring → quality gate → deploy to staging with blue-green → monitor groundedness and latency → promote to 100% traffic. For clinical agents I set groundedness threshold at 0.90, not 0.85 — patient safety requires a higher bar. Once stable in Foundry, I re-implement the orchestration in Semantic Kernel for the production .NET layer where I need full control over auth, logging, and error handling."

**Q: What's the difference between a Hub and a Project in AI Foundry?**
"The hub is the Azure infrastructure resource — shared compute, networking, security config. One hub serves multiple teams. The project is the workspace inside the hub — isolated per team, with its own agents, deployments, and evaluations. Think of the hub as the building and the project as a department floor. Each department works independently but shares the building's power, security, and network."

**Q: How do you prevent hallucination in a Foundry-deployed agent?**
"Two platform-level guardrails: Content Safety for PII and prompt injection, and groundedness detection for factual accuracy — both run in Foundry before output reaches the user. In the system prompt I explicitly instruct the agent to only use retrieved context and flag uncertainty rather than guess. In production SK I add a FunctionInvocationFilter that logs every tool call and an output validator. In the evaluation pipeline, groundedness is my primary quality gate — if it drops below threshold, the deployment is blocked."
