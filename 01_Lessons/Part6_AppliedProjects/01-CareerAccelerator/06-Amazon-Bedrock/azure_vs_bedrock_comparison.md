# Azure AI Foundry ↔ Amazon Bedrock — 15-Dimension Comparison

A side-by-side for the "can you work multi-cloud AI?" interview question. The architecture is the same on both clouds; the vendor primitives differ.

| # | Dimension | Azure AI Foundry / Azure OpenAI | Amazon Bedrock |
|---|---|---|---|
| 1 | **Platform layer** | Azure AI Foundry (ai.azure.com) | Amazon Bedrock (console + API) |
| 2 | **Model invocation** | Azure OpenAI SDK / REST | `boto3` `bedrock-runtime` (`converse` / `invoke_model`) |
| 3 | **Flagship models** | GPT-4o, o1/o3, GPT-4o-mini, Phi | Claude 3.x (Anthropic), Titan/Nova (Amazon), Llama, Mistral, Cohere |
| 4 | **Model selection** | Deployment name (you choose) | Model ID (provider-qualified, fixed) |
| 5 | **Model access** | Deploy a model in Foundry | Opt-in per model in the Bedrock console |
| 6 | **Embeddings** | text-embedding-3-small/large | Titan Embeddings, Cohere Embed |
| 7 | **Managed RAG** | Azure AI Search + "On Your Data" | Knowledge Bases (`retrieve_and_generate`) |
| 8 | **Vector store** | Azure AI Search (HNSW) | OpenSearch Serverless / Aurora pgvector |
| 9 | **Source storage** | Azure Blob Storage | Amazon S3 |
| 10 | **Agents** | Foundry Agents / Semantic Kernel | Bedrock Agents (action groups + Lambda) |
| 11 | **Tools / functions** | `[KernelFunction]` plugins / function calling | Action groups (OpenAPI schema + Lambda) |
| 12 | **Content safety** | Azure AI Content Safety (+ groundedness, Prompt Shields) | Bedrock Guardrails (+ contextual grounding, PII) |
| 13 | **Auth** | Managed Identity + Azure AD RBAC | IAM roles/policies |
| 14 | **Observability** | Azure Monitor + Application Insights | CloudWatch |
| 15 | **Fine-tuning** | Azure OpenAI fine-tuning (GPT-4o) | Bedrock custom models (fine-tune Titan/others) + provisioned throughput |

## Bonus dimensions (nice to name)

| Dimension | Azure | Bedrock |
|---|---|---|
| Throughput reservation | PTU (Provisioned Throughput Units) | Provisioned Throughput |
| Batch | Azure OpenAI Batch API | Bedrock batch inference |
| Networking | Private Link / VNet | VPC endpoints (PrivateLink) |
| Evaluation | Foundry Evaluation (RAGAS-style) | Bedrock model evaluation |

## The one-liner for interviews

> "Bedrock and Azure AI Foundry are mirror platforms — foundation model + managed RAG + agents + guardrails. I map cleanly between them: Azure OpenAI ↔ Bedrock model API, Azure AI Search ↔ Knowledge Bases, Semantic Kernel ↔ Bedrock Agents, Content Safety ↔ Guardrails, Managed Identity ↔ IAM. I'd choose per the client's existing cloud footprint and which flagship models they need — Claude on Bedrock, GPT-4o/o1 on Azure."
