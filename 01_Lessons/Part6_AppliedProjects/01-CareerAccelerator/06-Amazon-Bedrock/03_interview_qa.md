# 03 — Interview Q&A: Amazon Bedrock & Multi-Cloud AI (15 questions, senior level)

---

**Q1. What is Amazon Bedrock, and what's its Azure equivalent?**
A managed, serverless AWS service providing API access to foundation models (Claude, Titan, Llama, Mistral) plus managed RAG (Knowledge Bases), agents (Bedrock Agents), and guardrails. It's the AWS mirror of Azure AI Foundry / Azure OpenAI — same GenAI architecture, different vendor primitives; you call it with boto3.

**Q2. How do you invoke a model on Bedrock?**
Via the `bedrock-runtime` boto3 client. The modern way is the `converse` API — provider-agnostic messages-in/message-out, uniform across Claude/Titan/Llama, closest to the Azure OpenAI chat shape. The older `invoke_model` API takes provider-specific JSON bodies. I prefer `converse` because it avoids per-model payload differences.

**Q3. What are Bedrock Knowledge Bases?**
Bedrock's managed RAG — the Azure AI Search + "On Your Data" equivalent. You point a Knowledge Base at an S3 bucket, pick an embedding model and vector store (OpenSearch Serverless or Aurora pgvector), and Bedrock handles chunking, embedding, indexing, and retrieval. `retrieve_and_generate` returns a grounded, cited answer in one call.

**Q4. What are Bedrock Agents and action groups?**
Bedrock Agents are the Semantic Kernel / Foundry Agents equivalent — they plan (ReAct), call tools, and use Knowledge Bases for retrieval. Tools are "action groups," each defined by an OpenAPI schema and backed by an AWS Lambda function that runs your tool code. Action group = Bedrock's `[KernelFunction]`/tool.

**Q5. What are Bedrock Guardrails?**
The Azure AI Content Safety equivalent: configurable content filters (hate/violence/sexual/misconduct), denied topics, PII detection/redaction, word filters, and contextual grounding checks — applied to inputs and outputs, decoupled from the model. Same defense-in-depth layer as Content Safety plus groundedness detection.

**Q6. Model IDs vs Azure deployment names — what's the difference?**
Bedrock uses a fixed, provider-qualified model ID (`anthropic.claude-3-sonnet-20240229-v1:0`); Azure uses a deployment name you choose for a model. Both are just the string that selects which model you invoke — same concept, different naming convention.

**Q7. How is auth different on Bedrock vs Azure?**
Bedrock uses AWS IAM roles/policies; Azure uses Managed Identity + Azure AD RBAC. Same principle — grant the compute identity least-privilege access to the AI service, no static keys in code. On Azure I'd use a system-assigned Managed Identity with the Cognitive Services OpenAI User role; on AWS an IAM role with a Bedrock invoke policy.

**Q8. When would you choose Bedrock over Azure AI Foundry?**
It usually follows the org's existing cloud. Bedrock for AWS-native shops where S3, Lambda, and IAM are already in place, or when they specifically want Claude/Titan. Azure AI Foundry for Azure-native orgs (M365, Managed Identity) or when they want GPT-4o/o1. The architecture is identical; I choose per footprint and flagship-model needs.

**Q9. You need to enable a model before using it — explain.**
Bedrock model access is opt-in per model: you request access in the Bedrock console once (accepting the provider's terms), after which you can invoke it. It's the AWS analog of deploying a model in Foundry — a one-time enablement gate before invocation.

**Q10. How would you build RAG on Bedrock end to end?**
Put documents in S3, create a Knowledge Base pointing at that bucket with Titan/Cohere embeddings and an OpenSearch Serverless vector store. At query time, call `retrieve_and_generate` with the model ID and knowledge base ID to get a grounded, cited answer — or `retrieve` for just the chunks if I want to control prompt assembly myself. It's the same flow as Azure OpenAI On Your Data over AI Search.

**Q11. What's the multi-cloud value you bring as an Azure engineer learning Bedrock?**
I can deliver the same GenAI architecture on either cloud and map cleanly between them: Azure OpenAI ↔ Bedrock model API, Azure AI Search ↔ Knowledge Bases, Semantic Kernel ↔ Bedrock Agents, Content Safety ↔ Guardrails, Managed Identity ↔ IAM. Most Azure-only candidates can't, so I can serve AWS-native and multi-cloud clients without a re-architecture.

**Q12. Claude on Bedrock vs GPT-4o on Azure — how do you decide?**
By capability fit and org constraints. Claude 3.5 Sonnet and GPT-4o are both strong general models; I'd benchmark both on the actual task (like Foundry's model comparison) rather than assume. Constraint-wise: if the org is AWS-native or wants Anthropic specifically, Claude on Bedrock; if Azure-native or needs o1/o3 reasoning or M365 integration, GPT-4o/o1 on Azure.

**Q13. How does cost/throughput management compare?**
Both offer on-demand (per-token) and reserved throughput — Azure's PTU maps to Bedrock's Provisioned Throughput. For predictable high volume with latency guarantees, reserve capacity on either. For variable/bursty load, on-demand. Same PTU-vs-pay-as-you-go reasoning I apply on Azure, applied to Bedrock.

**Q14. What are the risks of a multi-cloud AI strategy?**
Operational complexity (two sets of SDKs, auth, monitoring, IaC), duplicated evaluation/guardrail config, data-residency and egress cost when data crosses clouds, and skill spread. Mitigate by abstracting the model call behind an internal interface, standardizing evaluation/guardrails as policy, and keeping data local to its cloud. Multi-cloud is justified by resilience, best-of-breed models, or client mandate — not by default.

**Q15. Design a portable GenAI architecture that can run on either Azure or Bedrock.**
Wrap the model invocation and RAG behind an internal interface (e.g., `IGenerationService` / `IRetrievalService`) with two implementations — one over Azure OpenAI + AI Search, one over Bedrock + Knowledge Bases. Keep prompts, evaluation (RAGAS-style), and guardrail policy vendor-neutral. Then the same application runs on either cloud by swapping the implementation — the same abstraction-layer discipline I use to survive Azure SDK/model version changes.

---
*Frame every answer as "the Azure equivalent is X, here's the Bedrock primitive, and here's how I choose" — that's the multi-cloud signal.*
