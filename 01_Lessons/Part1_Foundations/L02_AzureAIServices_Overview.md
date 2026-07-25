# Module 2 — Azure AI Services Overview
**Part 1: AI Fundamentals | AI Solutions Architect Curriculum**
*Created: 2026-06-30*

---

## Why This Module Matters

You already know Azure OpenAI, AI Search, Document Intelligence, Semantic Kernel — the deep internals. Module 2 is the **map** you were missing. It answers:

- What is the full Azure AI landscape — which service does what?
- How do you provision and manage these services?
- What are the security and compliance patterns?

Since you already know the deep content (Modules 7–19), this module will feel fast. Think of it as putting labels on a map you already know how to navigate.

---

**Running example (used throughout):**
> *JM Family is building an AI platform. An architect must choose the right Azure AI service for each use case — and provision, secure, and govern them correctly.*

---

## Topic 2.1 — Azure AI Platform Introduction

---

### 1. The Azure AI Services Landscape

Azure AI is a family of services, not one product. Every service is a REST API backed by a pre-trained model hosted by Microsoft.

```
AZURE AI SERVICES LANDSCAPE (2026)
────────────────────────────────────────────────────────────

VISION                    LANGUAGE                 SPEECH
─────────────────         ────────────────         ──────────────────
Azure AI Vision           Azure AI Language        Azure AI Speech
  • Image Analysis          • Sentiment analysis     • Speech-to-Text
  • OCR / Read              • Entity recognition     • Text-to-Speech
  • Object detection        • PII detection          • Speech Translation
  • Face detection          • Text classification    • Custom Speech
Custom Vision               • Summarization
                           Azure OpenAI Service
                             • GPT-4o, GPT-4o mini
                             • Embeddings
                             • DALL-E 3, Whisper

SEARCH & KNOWLEDGE        DOCUMENT                 DECISION
──────────────────        ─────────────────        ─────────────────
Azure AI Search             Azure Document          Azure Content Safety
  • Full-text BM25           Intelligence             • Harm detection
  • Vector search            • Invoice model          • Prompt injection
  • Hybrid + semantic        • Layout model           • Groundedness
  • AI Enrichment            • Custom models
                             • Read model (OCR)
```

---

### 2. Which Service for Which Job — Decision Table

| You Need To... | Use This |
|---|---|
| Extract text from a scanned PDF | Azure Document Intelligence (Read model) |
| Extract structured fields from invoices | Azure Document Intelligence (Invoice model) |
| Search documents with natural language | Azure AI Search (semantic + vector) |
| Answer questions using your own documents | Azure OpenAI + Azure AI Search (RAG) |
| Detect sentiment in dealer feedback | Azure AI Language (Sentiment) |
| Detect PII before sending to LLM | Azure AI Language (PII detection) |
| Transcribe a call recording | Azure AI Speech (Speech-to-Text) |
| Detect objects in vehicle images | Azure AI Vision (Image Analysis 4.0) |
| Train a custom image classifier | Azure AI Custom Vision |
| Generate text, summarize, answer questions | Azure OpenAI Service |
| Detect harmful content in AI outputs | Azure Content Safety |
| Detect faces in photos | Azure AI Face Service |

---

### 3. Multi-Service vs Single-Service Resources

When provisioning, you have a choice:

**Multi-service resource (Azure AI Services)**
- One resource, one key, one endpoint
- Covers: Vision, Language, Speech, Face, Translator
- Does NOT include: Azure OpenAI, AI Search, Document Intelligence (separate)
- Best for: dev/test, small projects, prototyping

**Single-service resources**
- One resource per service (e.g., one for Language, one for Vision)
- Independent scaling, billing, monitoring per service
- Best for: production — gives isolation, separate quotas, cleaner cost tracking

```
JM Family recommendation:
  Development: multi-service resource → fast setup
  Production: separate resources per service
              cog-jma-prod-vision, cog-jma-prod-language, etc.
              Reason: separate Private Endpoints, quotas, monitoring alerts
```

---

### 4. Azure AI Foundry — The Unified Portal (2026)

Prior to 2025, each service had its own portal/studio. Now there is one:

```
ai.azure.com — Azure AI Foundry
  ├── Model catalog (1,600+ models)
  ├── Azure OpenAI deployments
  ├── AI Agents builder
  ├── Prompt Flow
  ├── Evaluation pipelines
  ├── Content Safety / Guardrails
  ├── Fine-tuning
  └── Monitoring / Tracing
```

For services NOT in Foundry, use their dedicated portals:
- Document Intelligence → documentintelligence.ai.azure.com
- Custom Vision → customvision.ai
- Language Studio → language.cognitive.azure.com
- Vision Studio → portal.vision.cognitive.azure.com

---

### 5. Choosing the Right Service — Architect Decision Flow

```
New requirement arrives:
  │
  ├─ Is it about understanding/generating TEXT?
  │   ├─ Structured extraction from docs → Document Intelligence
  │   ├─ Search and retrieval → AI Search
  │   ├─ Summarize / answer questions → Azure OpenAI (+ RAG if grounded)
  │   └─ Classify / detect sentiment / PII → Azure AI Language
  │
  ├─ Is it about IMAGES or VIDEO?
  │   ├─ Read text in image → Vision (OCR) or Document Intelligence
  │   ├─ Detect objects, tag image → Azure AI Vision
  │   └─ Custom categories → Custom Vision
  │
  ├─ Is it about AUDIO?
  │   └─ Speech-to-Text / TTS / Translation → Azure AI Speech
  │
  └─ Is it about SAFETY / CONTENT?
      └─ Azure Content Safety
```

---

## Topic 2.2 — Provisioning Azure AI Resources

---

### 1. Creating Azure AI Services Resources

Every Azure AI service is provisioned as an Azure resource with:
- A **resource group** (logical container)
- A **region** (where the compute runs — affects latency and model availability)
- A **pricing tier** (F0 = free, S0 = standard, custom for OpenAI)
- A **name** (becomes part of the endpoint URL)

```
Endpoint format:
  https://{resource-name}.cognitiveservices.azure.com/

Example:
  https://cog-jma-dev-language.cognitiveservices.azure.com/
```

---

### 2. Keys and Endpoints

Every AI service has two access methods:

**Method 1: API Key** (simpler, avoid in production)
```
Key1 and Key2 (two keys for zero-downtime rotation)
Header: Ocp-Apim-Subscription-Key: {key}

Risk: key in code → leaked to Git → anyone can call your service
```

**Method 2: Managed Identity + Azure AD** (production standard)
```csharp
// No key in code — identity is the credential
var credential = new DefaultAzureCredential();
var client = new TextAnalyticsClient(endpoint, credential);
```

JM Family standard: **Managed Identity always in production.** API keys only in local dev with user secrets, never committed.

---

### 3. Pricing Tiers

| Tier | Cost | Use |
|---|---|---|
| **F0 (Free)** | $0 but throttled (5 calls/min, low limits) | Learning, demos |
| **S0 (Standard)** | Pay per call, no throttle | Production |
| **Commitment tiers** | Monthly flat fee at high volume | Enterprise scale |

Azure OpenAI has different pricing — per token (not per call). You covered this in Module 12.

---

### 4. ARM Templates and Bicep for AI Resources

In production, never click to create resources — use Infrastructure as Code:

**Bicep example — provision a Language service:**
```bicep
resource languageService 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: 'cog-jma-prod-language'
  location: 'eastus2'
  kind: 'TextAnalytics'
  sku: {
    name: 'S0'
  }
  properties: {
    publicNetworkAccess: 'Disabled'   // VNet only
    customSubDomainName: 'cog-jma-prod-language'
  }
  identity: {
    type: 'SystemAssigned'            // Managed Identity
  }
}
```

**Why Bicep over portal clicks:**
- Repeatable across dev/stg/prod
- Version controlled in Git
- Enforces security settings (no accidental public access)
- Required for CI/CD pipeline deployments

---

### 5. Managing with Azure CLI

Common CLI commands for AI services:

```bash
# List all AI service resources
az cognitiveservices account list --resource-group rg-jma-ai-prod

# Get keys (avoid — use Managed Identity instead)
az cognitiveservices account keys list \
  --name cog-jma-prod-language \
  --resource-group rg-jma-ai-prod

# Create a multi-service resource
az cognitiveservices account create \
  --name cog-jma-dev-multi \
  --resource-group rg-jma-ai-dev \
  --kind CognitiveServices \
  --sku S0 \
  --location eastus2

# Check quota / usage
az cognitiveservices account usage list \
  --name cog-jma-prod-language \
  --resource-group rg-jma-ai-prod
```

---

## Topic 2.3 — Security and Compliance

---

### 1. Authentication Options

```
THREE WAYS TO AUTHENTICATE TO AZURE AI SERVICES
─────────────────────────────────────────────────

1. API KEY (avoid in production)
   Header: Ocp-Apim-Subscription-Key: abc123...
   Risk: key exposure, no identity tracking, no RBAC

2. AZURE AD TOKEN (manual — avoid)
   Get token from Azure AD → pass as Bearer token
   Better than key but complex to manage

3. MANAGED IDENTITY (production standard)
   Your Azure resource (Function App, AKS) has an identity
   Identity gets RBAC role on the AI service
   No credentials in code, auto-rotated, fully audited
   ← This is what JMA should use everywhere
```

**RBAC roles for AI services:**

| Role | Access |
|---|---|
| Cognitive Services User | Call the APIs |
| Cognitive Services Contributor | Full management |
| Cognitive Services Reader | Read metadata, no API calls |

```csharp
// C# — Managed Identity, works in Azure, no key needed
var credential = new DefaultAzureCredential();

// Azure OpenAI
var openAIClient = new AzureOpenAIClient(
    new Uri("https://oai-jma-prod.openai.azure.com/"),
    credential);

// Language service
var languageClient = new TextAnalyticsClient(
    new Uri("https://cog-jma-prod-language.cognitiveservices.azure.com/"),
    credential);
```

---

### 2. Network Security — VNets and Private Endpoints

Default: Azure AI services accept traffic from the public internet.
Production requirement: **restrict to VNet only.**

```
NETWORK SECURITY PATTERN
─────────────────────────────────────────────────────────

Public Internet ──✕── (blocked by firewall rules)

Azure VNet (jma-vnet-prod)
  ├── Subnet: app-subnet
  │     Azure Function App (jma-func-aisync)
  │         │
  │         └──── Private Endpoint ──── cog-jma-prod-language
  │                                     (private IP: 10.0.1.5)
  │
  └── Subnet: ai-subnet
        Private DNS Zone: cognitiveservices.azure.com
        Routes all .cognitiveservices.azure.com to private IPs
```

**Key concepts:**
- **Private Endpoint:** assigns a private IP to the AI service inside your VNet
- **Private DNS Zone:** ensures `cog-jma-prod-language.cognitiveservices.azure.com` resolves to the private IP, not the public one
- **Service Endpoint vs Private Endpoint:** Private Endpoint is the current standard (gives a private IP); Service Endpoints are older and weaker

---

### 3. Managed Identity for AI Services — The Full Pattern

```
JM Family EnterpriseSearch.Sync — full Managed Identity chain:

Azure Function App (jma-func-aisync)
  System-assigned Managed Identity → "jma-func-aisync"
  │
  ├── RBAC: "Cognitive Services User" on cog-jma-prod-language
  ├── RBAC: "Search Index Data Contributor" on srch-jma-prod-indexer
  └── RBAC: "Storage Blob Data Reader" on st-jma-prod-docs

No keys anywhere. Identity is the credential.
Rotate nothing. Azure AD handles it.
Full audit trail in Azure Monitor: who called what, when.
```

---

### 4. Data Privacy and Compliance

**Key principle:** Azure AI services do **not** use your data to train Microsoft's models.

| Service | Data handling |
|---|---|
| Azure OpenAI | Your prompts/completions stay in your tenant, not used for training |
| Azure AI Language | Requests processed, not stored by default |
| Azure Document Intelligence | Documents processed in-memory, not retained |
| Azure AI Search | Your index lives in your storage — fully isolated |

**For JMA healthcare/finance context:**
- Enable **Customer Managed Keys (CMK)** if data at rest must be encrypted with your own key (KeyVault)
- Enable **diagnostic logging** — required for compliance audits
- Use **Azure Policy** to enforce: no public network access, Managed Identity required

---

### 5. Azure AI Content Safety — Overview

Sits in front of any AI service that processes user input or generates output:

```
User Input
    │
    ▼
Azure Content Safety (pre-check)
    │ checks: Hate, Sexual, Violence, Self-harm
    │ checks: Prompt injection / Prompt shields
    │
    ▼ (if safe)
Azure OpenAI / AI Service
    │
    ▼
Azure Content Safety (post-check)
    │ checks: Groundedness (is output grounded in sources?)
    │ checks: Protected material detection
    │
    ▼
Response to User
```

You covered this in detail in Modules 11.4 and 18 — this is the overview-level connection.

---

## Topic R2 — Recall: Module 2 Review & Quiz

---

**Q1.** JM Family needs to extract structured fields (vendor name, total amount, invoice date) from scanned PDF invoices. Which Azure AI service? Which specific model within it?

> **A:** Azure Document Intelligence — Invoice prebuilt model. It returns structured JSON with named fields. Not Azure AI Vision (that's general image analysis) and not Azure OpenAI alone (expensive and slow for structured extraction at scale).

---

**Q2.** For production, should you use a multi-service or single-service Azure AI resource? Why?

> **A:** Single-service resources in production. Reasons: separate quotas (one noisy service doesn't throttle another), separate Private Endpoints, separate monitoring alerts, cleaner cost tracking per service, independent RBAC.

---

**Q3.** A developer hardcoded the Azure AI Language API key in the appsettings.json file and committed it to Git. What's the correct fix?

> **A:** Revoke and regenerate the key immediately. Replace key authentication with Managed Identity. Add the key pattern to .gitignore. Use Azure Key Vault for any secrets that must remain as keys. Enable Azure Defender for DevOps to catch future secret leaks.

---

**Q4.** What is the difference between a Service Endpoint and a Private Endpoint for an Azure AI service?

> **A:** Service Endpoint routes traffic from your VNet subnet over the Azure backbone to the service's public IP — it's still a public IP. Private Endpoint gives the service a private IP inside your VNet — all traffic stays on private network, never touches the public internet. Private Endpoint is the current production standard.

---

**Q5.** Name the three things every Azure AI service endpoint URL contains.

> **A:** `https://{resource-name}.cognitiveservices.azure.com/` — resource name (your custom name), service subdomain (cognitiveservices.azure.com), and the REST path follows after. For Azure OpenAI it's slightly different: `https://{resource-name}.openai.azure.com/`.

---

## Memory Hooks

- **"Multi-service = dev, Single-service = prod"**
- **"Managed Identity = no keys in code, ever"**
- **"Private Endpoint = private IP inside your VNet"**
- **"Azure OpenAI does NOT use your data to train Microsoft models"**
- **"F0 = free/throttled, S0 = pay-per-call production"**
- **"Bicep for AI resources = repeatable, auditable, CI/CD-ready"**
- **"Content Safety wraps your AI service — pre and post"**

---

## Interactive Learning Ideas

### Exercise 1 — Service Mapping (10 min)
Without looking, write down which Azure AI service handles each JMA use case:
- Read text from a scanned dealer agreement PDF
- Detect if a dealer support chat message contains PII
- Answer "What is our return policy?" from internal docs
- Transcribe a recorded dealer call
- Detect inappropriate content in a dealer-submitted image

### Exercise 2 — Portal Walkthrough (15 min)
Go to portal.azure.com → Resource Groups → find your JMA AI resources. For each one:
- What kind (single or multi-service)?
- What pricing tier?
- Is public network access enabled or disabled?
- Does it have a Managed Identity?

### Exercise 3 — Bicep Deploy (20 min)
Write a Bicep file that provisions an Azure AI Language service with:
- Public network access disabled
- System-assigned Managed Identity
- Location: eastus2
- SKU: S0

Deploy to your dev resource group. Verify in portal.

### Exercise 4 — CLI Practice (10 min)
Using Azure CLI, list all cognitiveservices accounts in your JMA resource group. Find the endpoint URL for each one. Note which ones have `publicNetworkAccess: Disabled`.

### Exercise 5 — Security Audit Question
For JMA's `cog-jma-dev-frm-recognizer` (Document Intelligence resource, manually deployed 2023): 
- Is it using Managed Identity or API keys?
- Is public network access enabled?
- What would you change to bring it to production security standard?

---

*Previous: Module 1 — Introduction to AI*
*Next: Module 3 — Computer Vision Fundamentals*
*Also connects to: Module 7 (Azure AI Services Deep Dive), Module 8 (Document Intelligence), Module 9 (Azure AI Search), Module 12 (Azure OpenAI)*
