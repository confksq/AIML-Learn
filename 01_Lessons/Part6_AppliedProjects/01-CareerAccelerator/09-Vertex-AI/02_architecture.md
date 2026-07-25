# 02 — Architecture: GCP Vertex AI

## The Vertex AI platform (mapped to Azure AI Foundry)

```
  ┌──────────────────────────────────────────────────────────────────────┐
  │ VERTEX AI  (managed, in your GCP project)     ≈ Azure AI Foundry      │
  │                                                                      │
  │  ┌────────────────┐  ┌──────────────────┐  ┌──────────────────────┐  │
  │  │ Gemini models   │  │ Vertex AI Search │  │ Agent Development Kit│  │
  │  │ (+ Model Garden:│  │ / Vector Search  │  │ (ADK) + Agent Engine │  │
  │  │  Claude, Llama, │  │ (managed RAG /   │  │  + Agent Builder     │  │
  │  │  Gemma, Mistral)│  │  vector DB)      │  │  (low-code)          │  │
  │  └────────────────┘  └──────────────────┘  └──────────────────────┘  │
  │  ┌────────────────┐  ┌──────────────────┐                            │
  │  │ Safety filters │  │ text-embedding-  │   auth: service acct + IAM │
  │  │ Responsible AI │  │ 004 (embeddings) │   logs: Cloud Logging      │
  │  └────────────────┘  └──────────────────┘                            │
  └──────────────────────────────────────────────────────────────────────┘
         ▲ google-genai SDK (vertexai=True)  /  vertexai SDK  /  ADK
         │
  ┌──────┴───────┐
  │ Your app     │  Python / Cloud Run / Agent Engine
  └──────────────┘
```

## RAG flow (Vertex AI Search) — mapped to Azure

```
  INGEST:  Docs in GCS / BigQuery ─▶ Vertex AI Search datastore
                                   ─▶ chunk + embed + index (managed)
  QUERY:   question ─▶ search.serving ─▶ retrieves + grounds with Gemini
                    ─▶ answer + citations
```
This mirrors Azure OpenAI "On Your Data" over Azure AI Search — Google manages chunk→embed→retrieve→ground. For a custom loop, use **Vertex AI Vector Search** (the AI Search vector index analog) and assemble the prompt yourself.

## Agent flow (ADK) — mapped to Semantic Kernel

```
  user ─▶ ADK Agent (Gemini + instruction)
            │  ReAct loop
            ├─▶ tool: get_inventory()      (Python fn ≈ [KernelFunction])
            ├─▶ tool: google_search        (built-in)
            └─▶ sub-agent: policy_agent    (agent-as-tool ≈ SK supervisor/specialist)
          ─▶ synthesized answer
     Runner manages session state/memory ≈ SK ChatHistory
     Deploy to Agent Engine (managed) or Cloud Run
```

## Component breakdown

| Component | Role | Azure equivalent |
|---|---|---|
| **google-genai SDK** | Call Gemini (vertexai=True routes via Vertex) | Azure OpenAI SDK |
| **Gemini** | Foundation model (multimodal, long context) | GPT-4o / o1 |
| **Model Garden** | Model catalog incl. Claude/Llama/Gemma | Foundry catalog / Bedrock access |
| **Vertex AI Search** | Managed RAG over GCS/BigQuery | AI Search + On Your Data |
| **Vertex Vector Search** | Managed vector DB (ANN) | Azure AI Search vector index |
| **text-embedding-004** | Embeddings | text-embedding-3 |
| **ADK Agent** | Agent framework (tools, multi-agent) | Semantic Kernel / Foundry Agents |
| **Agent Engine** | Managed agent runtime | (Foundry Agent hosting) |
| **Agent Builder** | Low-code agent console | Copilot Studio |
| **Safety filters** | Content filtering | Azure AI Content Safety |
| **Service account + IAM / ADC** | Auth | Managed Identity + RBAC |
| **Cloud Logging / Monitoring** | Observability | Azure Monitor / App Insights |

## Multi-cloud placement note

With modules 06 (Bedrock) and 09 (Vertex), you can now draw the **same** GenAI architecture — foundation model + managed RAG + agents + safety + IAM-style auth — on **all three clouds**. The differences are SDKs, model names, and storage (Blob vs S3 vs GCS). Vertex's standout: Gemini's ~1–2M-token context and native BigQuery integration.

---
*Next: `03_interview_qa.md`*
