# 02 — Architecture: Amazon Bedrock

## The Bedrock platform (mapped to Azure AI Foundry)

```
  ┌──────────────────────────────────────────────────────────────────────┐
  │ AMAZON BEDROCK  (managed, serverless)      ≈ Azure AI Foundry         │
  │                                                                      │
  │  ┌────────────────┐  ┌──────────────────┐  ┌──────────────────────┐  │
  │  │ Foundation     │  │ Knowledge Bases  │  │ Agents               │  │
  │  │ Models         │  │ (managed RAG)    │  │ (action groups +     │  │
  │  │ Claude/Titan/  │  │ S3 -> chunk/embed│  │  Lambda + KBs)       │  │
  │  │ Llama/Mistral  │  │ -> vector store  │  │                      │  │
  │  └────────────────┘  └──────────────────┘  └──────────────────────┘  │
  │  ┌────────────────┐  ┌──────────────────┐                            │
  │  │ Guardrails     │  │ Model access     │   auth: IAM roles          │
  │  │ (content safety│  │ (opt-in per model)│  logs: CloudWatch         │
  │  │  + grounding)  │  └──────────────────┘                            │
  │  └────────────────┘                                                  │
  └──────────────────────────────────────────────────────────────────────┘
         ▲ boto3 (bedrock-runtime.converse / retrieve_and_generate)
         │
  ┌──────┴───────┐
  │ Your app     │  Python (boto3) / Lambda / any AWS compute
  └──────────────┘
```

## RAG via Knowledge Bases — data flow

```
  INGEST:  Documents in S3 ─▶ Knowledge Base ─▶ chunk + embed (Titan/Cohere)
                                              ─▶ vector store (OpenSearch Serverless / Aurora pgvector)

  QUERY:   user question ─▶ retrieve_and_generate(modelId, knowledgeBaseId)
                          ─▶ Bedrock retrieves chunks + calls the model
                          ─▶ grounded answer + citations
```
This is the AWS mirror of Azure OpenAI "On Your Data" over Azure AI Search — you configure the KB once and Bedrock owns chunk→embed→retrieve→generate.

## Component breakdown

| Component | Role | Azure equivalent |
|---|---|---|
| **bedrock-runtime** client | Invoke models (`converse`, `retrieve_and_generate`) | Azure OpenAI SDK |
| **Model ID** | Selects the model (`anthropic.claude-3-sonnet-...`) | Deployment name |
| **Knowledge Base** | Managed RAG: S3 → embed → vector store → retrieve | Azure AI Search + On Your Data |
| **S3 bucket** | Source documents | Azure Blob Storage |
| **Vector store** | OpenSearch Serverless / Aurora pgvector | Azure AI Search vectors |
| **Bedrock Agent** | Plans + calls action groups (Lambda) + KBs | Foundry Agents / Semantic Kernel |
| **Action group** | A tool, defined by OpenAPI schema + Lambda | `[KernelFunction]` plugin |
| **Guardrails** | Content filtering, PII, grounding checks | Azure AI Content Safety |
| **IAM role** | Auth to Bedrock | Managed Identity + RBAC |
| **CloudWatch** | Metrics/logs | Azure Monitor |

## Two invocation APIs (know both)

- **`converse`** — modern, provider-agnostic (uniform messages in/out across Claude, Titan, Llama). Prefer this; it's closest to the Azure OpenAI chat shape and avoids per-model JSON bodies.
- **`invoke_model`** — older, provider-specific request/response JSON (each model family expects a different body). You'll encounter it in existing code; know it exists but reach for `converse`.

## Multi-cloud placement note

The whole point of this module: the **architecture is identical** to your Azure stack (foundation model + managed RAG + agents + guardrails). What changes is the vendor's SDK (`boto3` vs Azure SDK), auth (IAM vs Managed Identity), storage (S3 vs Blob), and flagship models (Claude/Titan vs GPT-4o). An architect who can draw this same diagram on both clouds is the "multi-cloud AI" hire.

---
*Next: `03_interview_qa.md`*
