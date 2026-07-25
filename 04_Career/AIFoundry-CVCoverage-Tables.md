# AI Foundry Coverage — Your CV Curriculum
**Source:** CV Curriculum + 66 Job Postings Analysis  
**Last Updated:** 2026-06-13

---

## Table 1 — AI Foundry Coverage vs CV Curriculum (Bullet by Bullet)

| Curriculum Area | Topic | AI Foundry | What You Need Instead |
|---|---|---|---|
| **RAG Pipeline** | RAG setup + Knowledge grounding | ✅ | — |
| **RAG Pipeline** | Embedding generation (text-embedding-3) | ✅ | — |
| **RAG Pipeline** | Grounded response with GPT-4o | ✅ | — |
| **RAG Pipeline** | Semantic Kernel SDK orchestration in C# | ❌ | Semantic Kernel learning path |
| **RAG Pipeline** | Document ingestion pipelines in code | ❌ | Semantic Kernel + C# code |
| **RAG Pipeline** | Hybrid retrieval (keyword + vector) tuning | ❌ | Azure AI Search hands-on |
| **Vector Search** | Azure AI Search integration (portal) | ✅ | — |
| **Vector Search** | Basic chunking (auto on file upload) | ✅ | — |
| **Vector Search** | HNSW indexing configuration | ❌ | Azure AI Search deep dive |
| **Vector Search** | Cosine similarity scoring tuning | ❌ | Azure AI Search deep dive |
| **Vector Search** | Fixed-size vs semantic vs paragraph chunking | ❌ | Azure AI Search hands-on |
| **Vector Search** | Azure AI Document Intelligence | ❌ | Separate Azure service |
| **Vector Search** | Token budget in chunk design | ❌ | SDK-level learning |
| **Agents** | Agent building — portal level | ✅ | — |
| **Agents** | Tools / Actions via OpenAPI spec | ✅ | — |
| **Agents** | Multi-agent concepts | ✅ | — |
| **Agents** | Semantic Kernel plugins in C# | ❌ | Semantic Kernel SDK |
| **Agents** | SK planners (sequential, stepwise) | ❌ | Semantic Kernel SDK |
| **Agents** | SK memory in code | ❌ | Semantic Kernel SDK |
| **Agents** | .NET-native agent loops | ❌ | Semantic Kernel SDK |
| **Prompt Engineering** | System prompt design (Instructions) | ✅ | — |
| **Prompt Engineering** | Temperature / Top-P settings | ✅ | — |
| **Prompt Engineering** | Few-shot and chain-of-thought prompting | ❌ | Practice + patterns |
| **Prompt Engineering** | Prompt chaining across calls | ❌ | SDK-level coding |
| **Prompt Engineering** | Streaming via IAsyncEnumerable | ❌ | C# SDK coding |
| **Prompt Engineering** | Prompt compression / token optimization | ❌ | SDK-level coding |
| **Prompt Engineering** | Model tier selection for cost | ❌ | Architectural practice |
| **AI Security** | Content Safety / Guardrails (portal) | ✅ | — |
| **AI Security** | Input and output filtering | ✅ | — |
| **AI Security** | Prompt injection defense in code | ❌ | Security patterns module |
| **AI Security** | Jailbreak detection | ❌ | Security patterns module |
| **AI Security** | PII detection and redaction | ❌ | Azure AI Content Safety SDK |
| **AI Security** | Grounding validation logic | ❌ | Code-level patterns |
| **Fine-tuning** | Supervised fine-tuning (portal) | ✅ | — |
| **Fine-tuning** | Fine-tune vs RAG vs prompt trade-offs | ✅ | — |
| **Fine-tuning** | Evaluation dataset design | ✅ | — |
| **LLMOps** | Model deployment and rollback | ✅ | — |
| **LLMOps** | Evaluation pipelines (groundedness etc.) | ✅ | — |
| **LLMOps** | Production monitoring via Azure Monitor | ✅ | — |
| **LLMOps** | Token consumption and cost tracking | ✅ | — |
| **LLMOps** | Prompt versioning strategy | ❌ | DevOps + tooling practices |
| **LLMOps** | Azure DevOps CI/CD for AI pipelines | ❌ | Azure DevOps module |
| **LLMOps** | Automated eval in CI/CD gates | ❌ | Azure DevOps module |
| **Foundations** | Transformer internals (attention etc.) | ❌ | Part 3 theory learning |
| **Foundations** | Tokenization (BPE, WordPiece, budgeting) | ❌ | Part 3 theory learning |
| **Foundations** | Embedding geometry (cosine, dimensions) | ❌ | Part 3 theory learning |
| **Foundations** | LoRA / PEFT / RLHF / instruction tuning | ❌ | Part 3 theory learning |

---

## Focus Summary — The ❌ Clusters

```
Biggest gap:  Semantic Kernel SDK in C#     → 7 missing topics
Second gap:   Azure AI Search deep dive     → 4 missing topics
Third gap:    Foundations / Theory          → 4 missing topics
Fourth gap:   AI Security patterns         → 4 missing topics
Fifth gap:    LLMOps / DevOps              → 3 missing topics

Semantic Kernel is the single largest gap — it touches RAG, Agents,
Prompt Engineering, and Streaming all at once. One SK learning track
closes the most ❌ boxes fastest.
```

---

## Table 2 — ❌ Items That Have a Portal or Tool (No Code Needed)

| Topic | Available | Where |
|---|---|---|
| Hybrid retrieval tuning | ✅ Portal | Azure AI Search portal — index configuration |
| HNSW indexing | ✅ Portal | Azure AI Search portal — vector config |
| Cosine similarity scoring | ✅ Portal | Azure AI Search — relevance tuning |
| Chunking strategies | ✅ Portal | Azure AI Search — index wizard chunking settings |
| Document Intelligence | ✅ Portal | AI Foundry → Content Understanding |
| Few-shot / chain-of-thought prompting | ✅ Portal | AI Foundry Playground — test directly, no code |
| Prompt chaining | ✅ Portal | AI Foundry Agents — multi-step instructions |
| Model tier selection for cost | ✅ Portal | AI Foundry → Models + endpoints pricing view |
| Prompt injection / jailbreak defense | ✅ Portal | Azure AI Content Safety Studio |
| PII detection and redaction | ✅ Portal | Azure AI Language Studio |
| Automated evaluation pipelines | ✅ Portal | AI Foundry → Protect & govern → Evaluation |
| CI/CD for AI | ✅ Portal | Azure DevOps — pipeline UI, no code needed to start |
| Prompt versioning | ✅ Tool | GitHub — version your prompt files like code |
| Tokenization mechanics | ✅ Tool | platform.openai.com/tokenizer |
| LoRA / PEFT / fine-tuning adaptation | ✅ Portal | AI Foundry → Fine-tuning portal |
| Token consumption monitoring | ✅ Portal | AI Foundry → Monitoring + Azure Monitor |

---

## Table 3 — ❌ Items That Require Code

| Topic | Requires Code | Language |
|---|---|---|
| Semantic Kernel orchestration | ❌ Must code | C# |
| SK plugins and planners | ❌ Must code | C# |
| SK memory in code | ❌ Must code | C# |
| .NET-native agent loops | ❌ Must code | C# |
| Streaming via IAsyncEnumerable | ❌ Must code | C# |
| Prompt compression in code | ❌ Must code | C# |
| Grounding validation logic | ❌ Must code | C# |
| Document ingestion pipelines | ❌ Must code | C# |
| Transformer / attention theory | ❌ Theory | Reading / courses |
| Embedding geometry theory | ❌ Theory | Reading / courses |

```
❌ items breakdown:
  ├── 16 topics → have a portal or tool you can use NOW
  └── 10 topics → require C# coding or theory reading
```

---

## Table 4 — Master Coverage Table (Job Market + AI Foundry + CV)
> Based on 66 job postings analysis

| Skill | Job Demand | AI Foundry Covers | How | Next Action |
|---|---|---|---|---|
| **Azure AI Foundry** | ████████████ Very High | ✅ Full portal | Portal | You are here — continue |
| **AI Agents** | ████████████ Very High | ✅ Agents portal | Portal | Already built one today |
| **Generative AI / LLMs** | ████████████ Very High | ✅ Model Catalog + Playground | Portal | AI Foundry Playground |
| **RAG / Vector Search** | ██████████ High | ✅ Knowledge + AI Search | Portal | AI Foundry Knowledge section |
| **Azure AI Services** | █████████ High | ✅ Content Understanding | Portal | AI Foundry → Content Understanding |
| **Fine-tuning / Embeddings** | ████ Medium | ✅ Fine-tuning portal | Portal | AI Foundry → Fine-tuning |
| **Document Intelligence / OCR** | █████ Medium | ✅ Content Understanding | Portal | AI Foundry → Content Understanding |
| **Evaluation pipelines** | (CV item) | ✅ Evaluation portal | Portal | AI Foundry → Protect & govern → Evaluation |
| **Content Safety / Guardrails** | (CV item) | ✅ Guardrails portal | Portal | AI Foundry → Guardrails + controls |
| **LLMOps / Monitoring** | (CV item) | ✅ Monitoring portal | Portal | AI Foundry → Monitoring |
| **Prompt injection / Jailbreak** | (CV item) | ✅ Content Safety Studio | Portal | Azure AI Content Safety Studio |
| **PII detection** | (CV item) | ✅ Language Studio | Portal | Azure AI Language Studio |
| **Chunking / HNSW / Indexing** | (CV item) | ✅ AI Search portal | Portal | Azure AI Search portal |
| **CI/CD for AI** | (CV item) | ✅ Azure DevOps | Portal | Azure DevOps pipeline UI |
| **Tokenization mechanics** | (CV item) | ✅ External tool | Tool | platform.openai.com/tokenizer |
| **Semantic Kernel (C#)** | ██████ Medium | ❌ Must code | C# SDK | SK GitHub samples + docs |
| **.NET / C# + AI** | ████████ Medium-High | ❌ Must code | C# SDK | Semantic Kernel learning path |
| **SK Plugins / Planners / Memory** | (CV item) | ❌ Must code | C# SDK | Semantic Kernel learning path |
| **Streaming / Token optimization** | (CV item) | ❌ Must code | C# SDK | Semantic Kernel learning path |
| **Grounding validation in code** | (CV item) | ❌ Must code | C# SDK | Semantic Kernel learning path |
| **LangChain (Python)** | ███████ Medium | ❌ Separate framework | Python | LangChain docs + Python practice |
| **Python for AI** | ███████████ High | ❌ Not covered | Python | Python + LangChain together |
| **Amazon Bedrock** | █████ Medium | ❌ Different platform | Portal | AWS Bedrock console |
| **Microsoft Fabric** | ███ Growing | ❌ Separate platform | Portal | Microsoft Fabric portal |
| **Vertex AI** | ███ Growing | ❌ Different platform | Portal | Google Cloud console |
| **Graph / Vector Datastores** | ██ Niche | ❌ Partial (AI Search only) | Portal + Code | CosmosDB vector + AI Search |
| **Transformer theory** | (CV item) | ❌ Theory only | Reading | Your Part 3 learning |
| **LoRA / PEFT / RLHF** | (CV item) | ❌ Theory only | Reading | Your Part 3 learning |

---

## Priority Focus — What Closes the Most Jobs Fastest

```
RIGHT NOW (portal — no code needed):
  1. AI Foundry Agents        ← very high demand, built today
  2. RAG deep dive            ← high demand, portal work in AI Foundry
  3. Content Understanding    ← covers Document Intelligence gap
  4. AI Foundry Evaluation    ← LLMOps + CV gap, same portal
  5. Content Safety Studio    ← security gap, portal-based

NEXT TRACK (C# code — one focused path):
  6. Semantic Kernel SDK      ← closes 8 CV gaps + medium-high job demand

PARALLEL TRACK (new platform):
  7. Python + LangChain       ← high job demand, biggest uncovered gap
```

---

*File generated: 2026-06-13*  
*Sources: claude-AIFoundryQA-imp.md + gmailreq.md + AI-LearningRoadmap.md*
