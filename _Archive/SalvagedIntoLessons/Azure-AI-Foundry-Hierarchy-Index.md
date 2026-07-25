# Azure AI Foundry — Complete Hierarchy Index

---

## ① Foundry Resource

### 1.1 Model Deployments
- OpenAI Family
  - GPT-5 / GPT-5-chat
  - GPT-4o / GPT-4.1 / GPT-4.1-nano
  - o3 / o4-mini
  - DALL·E / GPT-image-1
  - Whisper / gpt-4o-transcribe
- Microsoft Family
  - Phi-4
  - Phi-3.5
- Partner & Community
  - Meta Llama 3.1 / 3.2
  - Mistral Large / NeMo
  - Cohere Command R+ / Rerank v4
  - xAI Grok 3 / Grok 4 Fast
  - DeepSeek / Kimi / Moonshot
  - Hugging Face (10,000+ models)
- Model Router

### 1.2 Deployment Types
- Global Standard
- Provisioned Throughput (PTU)
- Serverless Endpoint (MaaS)
- Managed Compute

### 1.3 Security Settings
- Managed Identity
- RBAC Roles
  - Foundry Account Owner
  - Foundry Owner
  - Foundry User
  - Foundry Project Manager
- Network / Private Endpoints

### 1.4 Connections
- Azure Services
  - Azure AI Search
  - Azure Storage
  - Azure Cosmos DB
  - Azure OpenAI
  - Application Insights
  - Azure Key Vault
  - Azure Databricks
  - Azure APIM
- Microsoft Services
  - SharePoint
  - Microsoft Fabric
  - Foundry (another resource)
- Search & Web Grounding
  - Grounding with Bing Search
  - Grounding with Bing Custom Search
  - Serp
- External & Custom
  - OpenAI (direct)
  - Serverless Model
  - Model Gateway
  - API Key
  - Custom Key

---

## ② Foundry Project (New Gen)

### 2.1 Agent Service
- Agent Types
  - Prompt Agents
  - Hosted Agents
- Agent Core Components
  - System Prompt / Instructions
  - Model Deployment
  - Threads
  - Runs
  - Steps
- Agent Tools
  - Azure AI Search tool
  - Bing Web Search tool
  - Function Calling tool
  - Code Interpreter tool
  - File Search tool
  - MCP Server tool
  - Browser Automation
  - A2A Protocol tool
- Memory Store
  - Long-term memory across sessions
  - Automatic extraction + consolidation
- Multi-Agent Workflows
  - Visual builder
  - Agent-to-Agent (A2A) calls
  - Connected Agents / Sub-agent delegation

### 2.2 Playground
- Chat Playground
- Agents Playground
- Image Playground
- Audio Playground
- Compare Mode

### 2.3 Evaluations
- Groundedness score
- Relevance score
- Coherence score
- Fluency score
- Safety score
- Task Completion score
- Custom Evaluators

### 2.4 Observability
- Trace
  - End-to-end telemetry
  - Framework support (LangChain, AutoGen, OpenAI SDK)
  - OpenTelemetry (OTel) exporter
- Evaluate (in production)
  - Single-turn quality scoring
  - Multi-turn conversation scoring
- Monitor
  - Real-time issue detection
  - Alerts + dashboards
  - Azure Monitor
- Optimize
  - Production signal analysis
  - Ranked improvement suggestions

### 2.5 Fine-tuning Jobs
- LoRA
- QLoRA
- DPO
- Developer Tier

### 2.6 Files & Data
- Uploaded Files
- Vector Indexes
- Azure Blob Storage

### 2.7 Project Endpoint
- REST API (GA: 2025-05-01)

---

## ③ Hub-Based Project (Classic / AML)

### 3.1 Prompt Flow
- Standard Flow
- Chat Flow
- Evaluation Flow

### 3.2 Other Capabilities
- Fine-tuning
- Evaluations
- Managed Compute
- Connections

### 3.3 Auto-Created Resources
- Azure Storage Account
- Azure Key Vault
- Azure Application Insights
- Azure Container Registry

---

## ④ Foundry Tools (Prebuilt AI)

### 4.1 Speech
- Speech-to-Text (STT)
- Text-to-Speech (TTS)
- Real-time Translation

### 4.2 Vision
- Image Analysis
- OCR
- Face API
- Object Detection

### 4.3 Language
- Sentiment Analysis
- Named Entity Recognition (NER)
- Key Phrase Extraction
- PII Detection
- Language Detection
- Summarization
- Custom Question Answering (CQA)
- Conversational Language Understanding (CLU)

### 4.4 Document AI
- Form Recognizer
- Layout Analysis
- Prebuilt Models
  - Invoices
  - Receipts
  - W9s
  - IDs

### 4.5 Translator
- Real-time Translation
- Batch Document Translation

### 4.6 Content Safety
- Harmful Content Filter
- Jailbreak Detection
- Groundedness Detection

---

## ⑤ Developer Experience

### 5.1 Foundry Portal
- ai.azure.com

### 5.2 Foundry SDK (azure-ai-projects v2)
- Agents
- Inference
- Evaluations
- Memory
- Python
- .NET / C#
- JavaScript / TypeScript

### 5.3 REST API
- GA version: 2025-05-01

### 5.4 VS Code Extension — Foundry Toolkit
- Browse + Deploy Models
- Build + Deploy Hosted Agents
- Open in VS Code from Portal
- Generate Sample Code

### 5.5 Azure Developer CLI (azd)
- azd ai agent init
- azd up

### 5.6 Foundry Local
- Run models on-device
- Windows NPU / CPU
- CLI + SDK

---

## ⑥ Security & Governance

- Microsoft Entra ID
  - Identity + login (OAuth2 / OIDC)
  - RBAC role assignments
  - Conditional Access policies
- Azure Key Vault
  - Secrets, keys, certificates
  - BYO Vault support
- Private Endpoints + VNet
  - Network isolation
  - Azure Firewall / DDoS protection
  - Hub-spoke topology support
- Azure Policy
  - Governance rules + compliance
  - Resource tagging + locks
- Azure Monitor + Diagnostics
  - Platform-level logs
  - Cost Management + unified billing
- Responsible AI
  - Content Safety filters
  - Fairness evaluation
  - Transparency reports

---

## ⑦ GA vs Preview Status — 2025/2026

---

## ⑧ Key Rules Summary

---

## ⑨ Quick Reference Links
