# 01 — Concepts: GCP Vertex AI + Agent Development Kit

> **Bridge from what you already know:** Vertex AI is **Google's Azure AI Foundry**. You mapped Azure ↔ AWS Bedrock in module 06; this is the same exercise for GCP. Learn Google's names for the architecture you already run.

---

## 1. The one-sentence mental model

**Vertex AI is a managed platform for foundation models (Gemini), managed RAG (Vertex AI Search / Vector Search), agents (Agent Development Kit + Agent Builder), and embeddings — the GCP mirror of Azure AI Foundry / Azure OpenAI.** You call it with the `google-genai` (or `vertexai`) Python SDK.

| You know (Azure AI Foundry) | GCP Vertex AI | What it does |
|---|---|---|
| Azure OpenAI Service | **Vertex AI** model API | Call a foundation model |
| GPT-4o / o1 | **Gemini** (2.0/2.5 Flash, Pro) | The flagship models |
| A GPT-4o **deployment** | A **model name** (`gemini-2.0-flash`) | Which model you call |
| Azure AI Search (RAG) | **Vertex AI Search** / **Vector Search** | Managed RAG / vector store |
| Semantic Kernel / Foundry Agents | **Agent Development Kit (ADK)** + Agent Builder | Agent orchestration |
| Azure AI Content Safety | **Vertex safety filters / Responsible AI** | Content filtering |
| Managed Identity / RBAC | **Service accounts + IAM / ADC** | Auth |
| Azure Monitor | **Cloud Logging / Monitoring** | Observability |
| Azure Blob Storage | **Google Cloud Storage (GCS)** | Source documents |

---

## 2. Gemini — the model family

Google's flagship, natively multimodal (text, image, audio, video):

| Model | Best for |
|---|---|
| **Gemini 2.0/2.5 Flash** | Fast, cost-efficient, high-volume — the GPT-4o-mini analog |
| **Gemini 2.0/2.5 Pro** | Complex reasoning, long context — the GPT-4o analog |
| **text-embedding-004 / gecko** | Embeddings for retrieval — the text-embedding-3 analog |

Gemini's headline feature is a **very long context window** (up to ~1–2M tokens), larger than GPT-4o's 128k — relevant when you're weighing "stuff more context" vs RAG.

---

## 3. Two ways to call Gemini (know both)

1. **Gemini API (AI Studio)** — quick, API-key based, consumer/prototyping (like calling api.openai.com).
2. **Vertex AI** — enterprise: runs in your GCP project, IAM auth, data governance, VPC-SC — the equivalent of choosing Azure OpenAI over OpenAI-direct.

The modern **`google-genai` SDK** targets both; set `GOOGLE_GENAI_USE_VERTEXAI=True` to route through Vertex.

```python
from google import genai
client = genai.Client(vertexai=True, project="my-proj", location="us-central1")
resp = client.models.generate_content(model="gemini-2.0-flash", contents="What is RAG?")
print(resp.text)
```

The enterprise reasons to prefer Vertex over the raw Gemini API are the same reasons you prefer Azure OpenAI over OpenAI: data residency, IAM, compliance, VPC.

---

## 4. RAG on Vertex — two options

- **Vertex AI Search** (formerly Enterprise Search / Agent Builder datastores) — fully managed RAG: point it at GCS/BigQuery/websites, it chunks, embeds, indexes, and serves grounded, cited answers. The **Azure "On Your Data" + AI Search** equivalent.
- **Vertex AI Vector Search** (formerly Matching Engine) — a managed vector database (ANN) when you want to build the RAG loop yourself. The **Azure AI Search vector index** equivalent.

Plus a **RAG Engine** (managed RAG orchestration) and grounding-with-Google-Search as a built-in tool.

---

## 5. Agent Development Kit (ADK)

Google's open-source **agent framework** (the PRD's "+ Agent Development Kit") — the Vertex-native counterpart to Semantic Kernel / crewAI:

- **Agent** — an LLM with instructions + tools (like an SK agent / crewAI Agent).
- **Tools** — Python functions the agent can call (like `[KernelFunction]`), plus built-in tools (Google Search, code exec) and other agents as tools.
- **Multi-agent** — compose agents hierarchically (sub-agents), the ADK version of a supervisor/specialist pattern.
- **Runner / sessions** — manage state and memory across turns.
- Deploy to **Vertex AI Agent Engine** (managed runtime) or Cloud Run.

```python
from google.adk.agents import Agent

def get_inventory(model: str) -> str:
    """Look up vehicle inventory by model."""
    return f"3 {model} in stock"

agent = Agent(
    name="dealer_agent",
    model="gemini-2.0-flash",
    instruction="You are a JMA dealer support agent. Use tools to answer.",
    tools=[get_inventory],
)
```

Same mental model as SK: define tools, give the agent instructions, it plans (ReAct) and calls them. **Agent Builder** is the low-code console layer on top (the Copilot-Studio analog).

---

## 6. Auth — service accounts + ADC (vs Managed Identity)

GCP uses **service accounts** with IAM roles; locally you authenticate via **Application Default Credentials** (`gcloud auth application-default login`); on GCP compute the attached service account is used automatically — the direct analog of Azure Managed Identity + `DefaultAzureCredential`. No keys in code.

---

## 7. When Vertex AI vs Azure AI Foundry vs Bedrock

| Factor | Vertex AI (GCP) | Azure AI Foundry | Bedrock (AWS) |
|---|---|---|---|
| Flagship models | Gemini (+ Claude, Llama via Model Garden) | GPT-4o, o1/o3 | Claude, Titan, Llama |
| Managed RAG | Vertex AI Search | AI Search + On Your Data | Knowledge Bases |
| Agents | ADK + Agent Engine | Foundry Agents / SK | Bedrock Agents |
| Long context | Gemini ~1–2M tokens | 128k–200k | model-dependent |
| Best fit | GCP-native, data on BigQuery/GCS, wants Gemini | Azure-native, M365, GPT-4o | AWS-native, wants Claude |

**The senior / multi-cloud answer:** "The choice follows the org's cloud and preferred models. GCP-native shops on BigQuery/GCS with Gemini → Vertex AI; Azure-native → Foundry with GPT-4o; AWS-native → Bedrock with Claude. The GenAI architecture — foundation model + managed RAG + agents + safety — is the same on all three, so I map cleanly between them. Vertex's differentiators are Gemini's very long context and tight BigQuery integration."

---

## 8. Model Garden

Vertex AI **Model Garden** is the Foundry-catalog / Bedrock-model-access equivalent — 150+ models including Gemini, plus **Claude (Anthropic), Llama, Mistral, and Gemma** available through Vertex. So "which cloud has Claude?" is: both Bedrock and Vertex. Naming this signals real breadth.

---
*Next: `02_architecture.md`*
