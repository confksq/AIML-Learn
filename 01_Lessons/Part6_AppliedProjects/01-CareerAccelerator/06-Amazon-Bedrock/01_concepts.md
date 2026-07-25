# 01 — Concepts: Amazon Bedrock

> **Bridge from what you already know:** Amazon Bedrock is **AWS's Azure AI Foundry**. Every Bedrock concept has a direct Azure equivalent you already use. You're learning a new vendor's names for the same architecture.

---

## 1. The one-sentence mental model

**Bedrock is a managed, serverless API to foundation models (Claude, Titan, Llama, Mistral) plus RAG (Knowledge Bases), agents (Bedrock Agents), and guardrails — the AWS mirror of Azure AI Foundry / Azure OpenAI.** You call it with `boto3` instead of the Azure SDK.

| You know (Azure AI Foundry) | Amazon Bedrock | What it does |
|---|---|---|
| Azure OpenAI Service | **Bedrock model invocation API** | Call a foundation model |
| A GPT-4o **deployment** | A **model ID** (e.g. `anthropic.claude-3-sonnet-...`) | Which model you call |
| Azure AI Search (RAG) | **Bedrock Knowledge Bases** | Managed RAG vector store + retrieval |
| Semantic Kernel / agents | **Bedrock Agents** (action groups + Lambda) | Agent orchestration + tool use |
| Azure AI Content Safety | **Bedrock Guardrails** | Content filtering, PII, denied topics |
| Azure AI Foundry Hub | **Bedrock** (the platform) | The unified GenAI platform layer |
| Managed Identity / RBAC | **IAM roles** | Auth |
| Azure Monitor | **CloudWatch** | Observability |

---

## 2. Models available on Bedrock

Bedrock is multi-model, like Foundry's catalog:

| Provider | Models | Note |
|---|---|---|
| **Anthropic** | Claude 3 (Haiku/Sonnet/Opus), Claude 3.5 | The flagship models on Bedrock |
| **Amazon** | Titan (text, embeddings), Nova | AWS's own models |
| **Meta** | Llama 3 | Open-weight |
| **Mistral** | Mistral, Mixtral | Open-weight |
| **Cohere** | Command, Embed | Text + embeddings |

Model access is **opt-in per model** — you request access in the Bedrock console once before you can invoke a model (the AWS analog of deploying a model in Foundry).

---

## 3. Calling a model — boto3

Two client types matter:
- `bedrock` — management (list models, manage access).
- `bedrock-runtime` — **invocation** (the one you call to generate).

```python
import boto3, json

client = boto3.client("bedrock-runtime", region_name="us-east-1")

# Newer, model-agnostic API: Converse (recommended — uniform across providers)
resp = client.converse(
    modelId="anthropic.claude-3-sonnet-20240229-v1:0",
    messages=[{"role": "user", "content": [{"text": "What is RAG?"}]}],
    inferenceConfig={"maxTokens": 300, "temperature": 0.2},
)
print(resp["output"]["message"]["content"][0]["text"])
```

- **`converse` API** = the modern, uniform, provider-agnostic call (messages in, message out) — closest to the Azure OpenAI chat shape. Prefer it.
- **`invoke_model` API** = older, provider-specific JSON bodies (each model expects a different payload). You'll see it in older code.

**Model IDs vs Azure deployment names:** Bedrock uses a fixed provider-qualified model ID (`anthropic.claude-3-sonnet-...`); Azure uses a deployment name you choose. Same idea — the string that selects the model.

---

## 4. Bedrock Knowledge Bases (managed RAG)

The Azure AI Search + "On Your Data" equivalent — AWS manages the whole RAG loop:

```
Documents in S3  ─▶  Knowledge Base  ─▶  (chunk + embed with Titan/Cohere)
                                     ─▶  vector store (OpenSearch Serverless / Aurora pgvector)
Query ─▶ RetrieveAndGenerate API ─▶ retrieves chunks + calls the model + returns cited answer
```

- You point a Knowledge Base at an **S3** bucket (the Blob Storage equivalent), pick an embedding model and a vector store, and Bedrock handles chunking, embedding, indexing, and retrieval.
- `retrieve` returns chunks; `retrieve_and_generate` returns a grounded, cited answer in one call — the exact analog of Azure OpenAI "On Your Data."

---

## 5. Bedrock Agents

The Bedrock equivalent of Semantic Kernel agents / Azure AI Foundry Agents:
- **Action groups** — the tools the agent can call, defined by an OpenAPI schema and backed by an **AWS Lambda** function (your tool code). This is Bedrock's `[KernelFunction]`/tool equivalent.
- **Knowledge Bases** attach to an agent as its retrieval source.
- The agent plans (ReAct), calls action groups/KBs, and synthesizes — same orchestration model you know from SK.

---

## 6. Bedrock Guardrails

The Azure AI Content Safety equivalent: configurable content filters (hate/violence/sexual/misconduct), denied topics, PII detection/redaction, and word filters — applied to inputs and outputs, decoupled from the model. Includes contextual grounding checks (the groundedness-detection analog).

---

## 7. When to use Bedrock vs Azure AI Foundry

| Factor | Bedrock (AWS) | Azure AI Foundry |
|---|---|---|
| Best fit | Org is AWS-native; wants Claude/Titan; S3/Lambda ecosystem | Org is Azure-native; wants GPT-4o/o1; M365 integration |
| Flagship models | Claude 3.x (Anthropic), Titan, Nova | GPT-4o, o1/o3, Phi |
| RAG | Knowledge Bases (+ OpenSearch/Aurora) | Azure AI Search + On Your Data |
| Agents | Bedrock Agents (+ Lambda action groups) | Foundry Agents / Semantic Kernel |
| Auth / ops | IAM, CloudWatch | Managed Identity, Azure Monitor |

**The senior / multi-cloud answer:** "The choice usually follows the org's existing cloud — Bedrock for AWS-native shops (S3, Lambda, IAM already in place), Azure AI Foundry for Azure-native ones (M365, Managed Identity). The GenAI architecture is the same on both — foundation model + managed RAG + agents + guardrails — so I map cleanly between them. I'd pick per the client's existing footprint and which flagship models they need (Claude on Bedrock, GPT-4o/o1 on Azure)."

---
*Next: `02_architecture.md` · full 15-dimension comparison in `azure_vs_bedrock_comparison.md`*
