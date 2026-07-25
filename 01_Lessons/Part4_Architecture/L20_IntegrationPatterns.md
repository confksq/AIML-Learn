# Module 20 — Integration Patterns
**Part 4: Enterprise AI Solutions | AI Solutions Architect Curriculum**
*Created: 2026-06-30*

---

## Why This Module Matters

You now know every individual Azure AI service in depth. Integration Patterns is about **connecting them into production systems** — wiring Azure Functions, Logic Apps, Service Bus, Event Grid, M365 Copilot, SharePoint, and enterprise data sources into coherent AI workflows.

This is the final architecture layer — the "how does it all work together in a real enterprise?" module.

**JM Family is already doing this:** EnterpriseSearch.Sync (Azure Functions + AI Search Push API), Document Intelligence pipeline, SharePoint integration. This module gives you the vocabulary and patterns to describe, extend, and architect these systems confidently.

---

**Running example:**
> *JM Family builds an end-to-end AI platform: dealer documents ingested from SharePoint → Document Intelligence extracts fields → AI Search indexes content → Azure OpenAI RAG answers dealer questions via Teams bot → managers see analytics in Power BI.*

---

## Topic 20.1 — Azure Integration Services for AI

---

### 1. The AI Integration Stack

```
AZURE INTEGRATION SERVICES FOR AI
──────────────────────────────────────────────────────────────────

EVENT-DRIVEN (triggers AI processing)
  Azure Event Grid      → event bus, triggers on blob created/updated
  Azure Service Bus     → durable message queue, async AI processing
  Azure Event Hubs      → high-volume streaming (IoT, telemetry → AI)

COMPUTE (runs AI workloads)
  Azure Functions       → stateless event-driven, per-call billing
  Azure Container Apps  → containerized AI services, auto-scale to 0
  Azure Kubernetes (AKS)→ long-running AI inference at scale

ORCHESTRATION (coordinates AI pipelines)
  Azure Data Factory    → batch data movement + AI enrichment
  Logic Apps            → low-code workflow, 400+ connectors
  Azure API Management  → AI gateway, rate limiting, caching

STORAGE (AI pipeline state and results)
  Azure Blob Storage    → raw documents, audio, images
  Azure Cosmos DB       → AI processing results, conversation state
  Azure SQL             → structured results, reporting tables
```

---

### 2. Azure Functions as the AI Processing Engine

Azure Functions is the most common compute layer for AI pipelines at JM Family:

```
Why Azure Functions for AI:
  ✓ Triggered by: Blob Storage, Service Bus, Event Grid, HTTP, Timer
  ✓ Pay per execution (zero cost when idle)
  ✓ Scales automatically to handle bursts
  ✓ Managed Identity built-in → call AI services without keys
  ✓ C# support → Semantic Kernel, Azure OpenAI SDK, DI all work natively
```

**JMA EnterpriseSearch.Sync pattern (simplified):**

```csharp
[Function("ProcessNewDocument")]
public async Task Run(
    [BlobTrigger("raw-docs/{name}", Connection = "StorageConnection")] Stream document,
    string name,
    FunctionContext executionContext)
{
    var logger = executionContext.GetLogger<ProcessNewDocument>();

    // Step 1: Extract with Document Intelligence
    var extractedFields = await _documentIntelligence.AnalyzeAsync(document);
    logger.LogInformation("DI extracted {fieldCount} fields from {name}", extractedFields.Count, name);

    // Step 2: PII detection before indexing
    var sanitizedContent = await _languageService.RemovePiiAsync(extractedFields.Content);

    // Step 3: Generate embedding
    var embedding = await _openAI.GetEmbeddingAsync(sanitizedContent);

    // Step 4: Push to AI Search
    await _searchClient.IndexDocumentAsync(new SearchDocument
    {
        ["id"] = Path.GetFileNameWithoutExtension(name),
        ["content"] = sanitizedContent,
        ["contentVector"] = embedding,
        ["fields"] = extractedFields,
        ["processedAt"] = DateTimeOffset.UtcNow
    });

    logger.LogInformation("Document {name} indexed successfully", name);
}
```

---

### 3. Event Grid — Event-Driven AI Triggers

Event Grid routes events from Azure services to your Functions/Logic Apps:

```
Blob Storage (new file uploaded)
    │ BlobCreated event
    ▼
Event Grid Topic
    │ routes to subscribers
    ├──► Azure Function (ProcessNewDocument)
    ├──► Logic App (notify processing team)
    └──► Service Bus (queue for rate-limited AI processing)
```

**Event Grid vs Service Bus — which to use:**

| | Event Grid | Service Bus |
|---|---|---|
| **Pattern** | Push (fire and forget) | Pull (consumer reads at own pace) |
| **Delivery** | At-least-once, no ordering | At-least-once, FIFO ordering available |
| **Retry** | Built-in with dead-letter | Built-in with dead-letter |
| **Use for AI** | Trigger AI processing on file arrival | Rate-limit AI calls (prevent throttling) |
| **JMA use** | "New PDF in blob → start DI processing" | "Rate-limit DI calls to 10/sec" |

---

### 4. Service Bus — Rate-Limiting AI Calls

Azure AI Services have throttle limits (429 Too Many Requests). Service Bus smooths the flow:

```
WITHOUT Service Bus:
  100 files uploaded at once
  100 DI calls fired simultaneously
  → 70 succeed, 30 get 429 throttle errors

WITH Service Bus:
  100 files uploaded at once
  100 messages enqueued in Service Bus
  Azure Function reads 1 message per 100ms (10/second)
  → All 100 succeed, no throttling
```

```csharp
[Function("ProcessDocumentQueue")]
public async Task Run(
    [ServiceBusTrigger("ai-processing-queue",
     Connection = "ServiceBusConnection",
     MaxMessageCount = 1)] // process one at a time to control rate
    ServiceBusReceivedMessage message,
    ServiceBusMessageActions messageActions,
    FunctionContext context)
{
    try
    {
        var documentPath = message.Body.ToString();
        await ProcessDocumentAsync(documentPath);
        await messageActions.CompleteMessageAsync(message); // success
    }
    catch (RequestFailedException ex) when (ex.Status == 429)
    {
        // Re-queue with delay for retry
        await messageActions.AbandonMessageAsync(message);
    }
    catch (Exception ex)
    {
        await messageActions.DeadLetterMessageAsync(message, ex.Message);
    }
}
```

---

### 5. Azure API Management as AI Gateway

APIM sits in front of Azure OpenAI as a gateway — critical for enterprise:

```
Client (Teams Bot / Web App / Azure Function)
    │
    ▼
Azure API Management (AI Gateway)
    ├── Rate limiting: 100 RPM per team, 1000 RPM total
    ├── Semantic caching: cache similar queries 30 min
    ├── Load balancing: round-robin across 3 OpenAI deployments
    ├── Logging: every AI call logged to Log Analytics
    ├── Auth: validate caller identity before forwarding to OpenAI
    └── Transformation: add system prompt headers, strip PII from logs
    │
    ▼
Azure OpenAI Service (3 deployments for load balance)
```

**APIM policies for Azure OpenAI (Bicep):**
```xml
<policies>
  <inbound>
    <!-- Rate limit per subscription key (team) -->
    <rate-limit-by-key calls="100" renewal-period="60"
                       counter-key="@(context.Subscription.Id)" />
    <!-- Semantic cache lookup -->
    <azure-openai-semantic-cache-lookup score-threshold="0.85"
                                         embeddings-backend-id="oai-embeddings" />
    <!-- Load balance across deployments -->
    <set-backend-service backend-id="oai-pool-backend" />
  </inbound>
  <outbound>
    <!-- Cache the response -->
    <azure-openai-semantic-cache-store duration="1800" />
  </outbound>
</policies>
```

---

### 6. Azure Data Factory — Batch AI Enrichment

ADF orchestrates large-scale batch AI processing:

```
ADF Pipeline: Monthly Document Re-Enrichment
──────────────────────────────────────────────

Activity 1: Get Files (Lookup)
  → Query Cosmos DB for documents older than 90 days
  → Returns list of blob paths

Activity 2: For Each (parallel batches of 50)
  └── Web Activity: call Azure Function
        → Document Intelligence re-analyzes (new model version)
        → New embedding generated (new embedding model)
        → AI Search document updated

Activity 3: Verify (Wait + Lookup)
  → Check AI Search document count matches expected
  → Alert if count differs

Activity 4: Notify (Web Activity)
  → POST to Teams webhook: "Re-enrichment complete: 5,842 docs updated"
```

**When to use ADF vs Azure Functions:**

| | ADF | Azure Functions |
|---|---|---|
| **Trigger** | Schedule, manual, event | Event-driven (real-time) |
| **Volume** | Millions of records | Thousands per minute |
| **Orchestration** | Built-in (if/else, loops, error handling) | Code-based |
| **Monitoring** | Visual pipeline runs UI | Application Insights |
| **JMA use** | Nightly batch re-enrichment | Real-time doc processing on upload |

---

## Topic 20.2 — Microsoft 365 Integration

---

### 1. The Microsoft Copilot Ecosystem

```
MICROSOFT COPILOT ECOSYSTEM (2026)
──────────────────────────────────────────────────────

Microsoft 365 Copilot
  ├── Built into: Word, Excel, PowerPoint, Outlook, Teams
  ├── Knows: your emails, documents, meetings, chats
  └── Extensible via: Copilot Plugins, Declarative Agents

Copilot Studio (low-code)
  ├── Build custom agents without code
  ├── Connect to your APIs and data sources
  └── Publish to: Teams, M365 Copilot, Web, mobile

Azure AI Foundry Agents (pro-code)
  ├── Full SK/Python control
  └── Publish to channels via Azure Bot Service

Microsoft Graph
  └── API for all M365 data (emails, files, calendar, Teams messages)
```

---

### 2. Microsoft Graph — Accessing M365 Data for AI

Microsoft Graph is the API that gives your AI access to company data in M365:

```
What Graph exposes:
  /me/messages          → user's emails
  /me/drive/root        → OneDrive files
  /me/calendar/events   → calendar
  /sites/{id}/lists     → SharePoint lists
  /teams/{id}/channels  → Teams channel messages
  /users/{id}/manager   → org chart
```

**C# — call Graph from an Azure Function using Managed Identity:**

```csharp
// Requires Microsoft.Graph NuGet package
var credential = new DefaultAzureCredential();
var graphClient = new GraphServiceClient(credential);

// Get SharePoint documents modified in last 24 hours
var files = await graphClient.Sites[siteId]
    .Drive.Root
    .Children
    .GetAsync(config =>
    {
        config.QueryParameters.Filter = $"lastModifiedDateTime ge {DateTime.UtcNow.AddDays(-1):O}";
        config.QueryParameters.Select = new[] { "id", "name", "lastModifiedDateTime", "webUrl" };
    });

foreach (var file in files.Value)
{
    // Download and process with Document Intelligence
    var stream = await graphClient.Sites[siteId]
        .Drive.Items[file.Id].Content.GetAsync();
    await ProcessWithDocumentIntelligenceAsync(stream, file.Name);
}
```

**JMA SharePoint integration:** Instead of a manual indexer, use Graph change notifications (webhooks) → triggered when SharePoint files change → calls your Azure Function → processes new/updated documents.

---

### 3. Building a Declarative Agent for M365 Copilot

Declarative Agents extend M365 Copilot with your custom knowledge and tools:

```json
// manifest.json (Teams App Manifest)
{
  "copilotAgents": {
    "declarativeAgents": [{
      "id": "jma-dealer-support-agent",
      "file": "jma-dealer-support-agent.json"
    }]
  }
}
```

```json
// jma-dealer-support-agent.json
{
  "name": "JMA Dealer Support",
  "description": "Answers dealer questions using JM Family knowledge base",
  "instructions": "You are a JM Family dealer support specialist. Answer questions using the provided knowledge base. Always cite your sources. If you don't know, say so.",
  "capabilities": [
    {
      "name": "WebSearch",
      "sites": [{"url": "https://jmfamily.com/dealer-portal"}]
    },
    {
      "name": "OneDriveAndSharePoint",
      "items_by_sharepoint_ids": [
        {"site_id": "jma-sharepoint-site-id"}
      ]
    }
  ],
  "actions": [
    {
      "id": "get-order-status",
      "file": "openapi.json"
    }
  ]
}
```

This agent appears inside M365 Copilot chat — users invoke it with `@JMA Dealer Support` in Teams or Outlook.

---

### 4. Power Platform AI Builder

For non-developer JMA teams, Power Platform AI Builder offers no-code AI:

| Feature | What It Does | JMA Use |
|---|---|---|
| **Document Processing** | Extract fields from forms (wraps DI) | Invoice processing in Power Automate |
| **Text Classification** | Classify emails/tickets | Route support tickets in Power Automate |
| **Sentiment Analysis** | Detect email sentiment | Flag negative dealer emails |
| **Object Detection** | Find objects in images | Detect damage in vehicle photos |
| **Azure OpenAI** | GPT-4o in Power Automate flows | Summarize emails, draft responses |

**When to use Power Platform vs Azure SDK:**
- Non-developer team that owns the process → Power Platform
- Complex custom logic, SLA requirements, JMA IT ownership → Azure SDK

---

## Topic 20.3 — Enterprise Data Integration

---

### 1. Connecting Enterprise Data Sources to AI

```
ENTERPRISE DATA SOURCES → AI PIPELINE
──────────────────────────────────────────────────────────────

SharePoint           ─── Microsoft Graph ──► Azure Function
                                              │
Blob Storage         ─── BlobTrigger ────────┤
                                              │
SQL Database         ─── SQL Trigger ─────────┤
                                              ▼
Email (Exchange)     ─── Graph ──────► Document Intelligence
                                              │
SAP / Dynamics 365   ─── API ───────────────┤
                                              ▼
IoT / Telemetry      ─── Event Hubs ────► Azure AI + Anomaly Detector
                                              │
                                              ▼
                                        Azure AI Search (indexed)
                                              │
                                              ▼
                                    Azure OpenAI RAG (queryable)
```

---

### 2. Real-Time vs Batch Data Pipelines

**Real-time (event-driven):**
```
New invoice uploaded to SharePoint
    → Graph webhook fires → Event Grid event
    → Azure Function triggers immediately
    → Document Intelligence extracts fields (< 30 seconds)
    → AI Search updated (< 1 minute total latency)
    → Dealer can query the new invoice via RAG within 1 minute
```

**Batch (scheduled):**
```
Nightly at 2am: ADF pipeline runs
    → Query SQL for all invoices not yet vectorized
    → Batch embed with Azure OpenAI (batch API, 50% cost)
    → Bulk upload vectors to AI Search
    → Update Cosmos DB processing status
    → Log completion metrics
```

**Choose based on:**
- Real-time: dealer needs to query newly uploaded documents immediately
- Batch: historical backfill, cost optimization, non-urgent enrichment

---

### 3. Data Governance for AI

AI pipelines that process company data need governance:

```
DATA GOVERNANCE CHECKLIST FOR JMA AI PIPELINES
───────────────────────────────────────────────

Classification
  ✓ Classify documents: Public / Internal / Confidential / Restricted
  ✓ Restricted docs → never sent to external LLM, only internal models

PII Handling
  ✓ PII detected and redacted before indexing in AI Search
  ✓ PII not stored in AI pipeline logs
  ✓ Personal data purged from AI Search when GDPR/CCPA requested

Access Control
  ✓ AI Search row-level security: `filter=department eq '{userDept}'`
  ✓ Azure OpenAI prompts do not expose cross-department data
  ✓ Audit log: who queried what, when

Retention
  ✓ Azure OpenAI: requests not stored (confirm in Azure Portal)
  ✓ AI Search: documents deleted when source document deleted
  ✓ Conversation history: purged after 90 days

Compliance
  ✓ All AI processing stays in Azure East US 2 (data residency)
  ✓ Customer-managed keys for AI Search index encryption
  ✓ Monthly compliance review with JMA legal team
```

---

### 4. Azure Synapse + AI — Analytics at Scale

For JMA BI and analytics use cases:

```
Azure Synapse Analytics + AI:
  ├── Synapse Serverless SQL reads AI Search index (external table)
  ├── Synapse Spark runs batch embedding generation at scale
  ├── Synapse ML runs model training on large datasets
  └── Power BI connects to Synapse for AI-enriched analytics

JMA use case:
  Daily: Synapse ingests all dealer support tickets
  Synapse Spark: batch classify + sentiment score all tickets
  Synapse SQL: aggregate by dealer, region, issue type
  Power BI: "Dealer Sentiment Dashboard" shows weekly trends
```

---

## Topic R20 — Recall: Module 20 Review & Quiz

---

**Q1.** JM Family uploads 500 dealer agreements to SharePoint in one batch. Each needs Document Intelligence processing. Without any throttle control, what happens? How do you fix it?

> **A:** DI gets 500 simultaneous requests → most return 429 (throttled). Fix: use Event Grid to capture the SharePoint upload events → route them to a Service Bus queue → Azure Function reads one message at a time (or 5-10 per second based on your DI quota) → processes sequentially at a sustainable rate. Service Bus dead-letter queue catches any permanent failures for manual review.

---

**Q2.** What is the difference between Event Grid and Service Bus? Give a JMA use case for each.

> **A:** Event Grid is a push event bus — fires events when something happens (file uploaded, resource changed), routes to multiple subscribers simultaneously. JMA use: "New PDF in Blob → trigger DI processing immediately." Service Bus is a durable message queue — producer enqueues, consumer reads at its own pace, guaranteed ordering, retry/dead-letter built in. JMA use: "Rate-limit AI Search indexing — process at most 10 documents per second even during bulk uploads."

---

**Q3.** What is Azure API Management doing when used as an AI gateway, and why does JM Family need it?

> **A:** APIM sits between callers (bot, web app) and Azure OpenAI. It handles: rate limiting per team (prevents one team from exhausting quota), semantic caching (serves cached responses for similar queries — saves cost), load balancing across OpenAI deployments (avoids single-endpoint failures), logging all AI calls for compliance audit, and transforming requests (adding security headers, stripping PII from logs). JMA needs it because multiple teams use the same Azure OpenAI resource — without APIM, one team can throttle everyone else.

---

**Q4.** A JMA manager wants to ask M365 Copilot questions about dealer agreements stored in SharePoint without IT building a custom app. What's the fastest path?

> **A:** Build a Declarative Agent for M365 Copilot — configure it to access the SharePoint site containing dealer agreements. No code required, deploys as a Teams App, appears as `@JMA Dealer Support` in M365 Copilot chat. Users can ask questions and Copilot retrieves from the SharePoint site automatically. For production use with custom business logic (API calls, complex routing), upgrade to a full Azure AI Foundry Agent.

---

**Q5.** What is the difference between real-time and batch AI data pipelines? When would you use each at JM Family?

> **A:** Real-time: event-triggered, low latency (< 1 min), higher cost per document, uses Azure Functions + Event Grid. Use when: dealer needs to query a newly uploaded document immediately. Batch: scheduled (nightly/weekly), high throughput, lower cost (Batch API 50% discount), uses ADF + Synapse. Use when: enriching historical documents, generating weekly analytics, cost-sensitive enrichment where overnight latency is acceptable.

---

## Memory Hooks

- **"Functions = real-time AI trigger, ADF = batch AI orchestrator"**
- **"Event Grid = push fire-and-forget, Service Bus = pull with durability"**
- **"APIM = AI gateway: rate limit + cache + load balance + log"**
- **"Microsoft Graph = API for all M365 data (email, SharePoint, Teams, calendar)"**
- **"Declarative Agent = M365 Copilot extension, no code needed"**
- **"Service Bus smooths AI throttle spikes — queue absorbs bursts"**
- **"Governance: classify → PII strip → access control → retain → comply"**

---

## Interactive Learning Ideas

### Exercise 1 — Full Pipeline Design (30 min)
Design JMA's complete document AI pipeline on paper:
- Trigger: dealer uploads PDF to SharePoint
- Steps: Graph webhook → Event Grid → Service Bus → Azure Function → DI → PII strip → embed → AI Search push
- Error handling: dead-letter at each step
- Monitoring: what metric at each step tells you the pipeline is healthy?

### Exercise 2 — APIM Rate Limiting Setup (20 min)
Configure Azure API Management in front of your Azure OpenAI resource:
- Create an APIM instance (Consumption tier — pay per call)
- Import Azure OpenAI API spec
- Add rate-limit-by-key policy: 60 calls per minute per subscription
- Test: send 70 calls quickly — observe which ones get 429
- Check APIM analytics to see call distribution

### Exercise 3 — Graph Integration (20 min)
Write a C# Azure Function that:
- Triggers on a timer (every 5 minutes)
- Calls Microsoft Graph to list SharePoint files modified in the last 5 minutes
- For each new file: downloads content and calls Document Intelligence
- Logs results

### Exercise 4 — Declarative Agent Build (20 min)
Build a Declarative Agent for M365 Copilot:
- Create a Teams App manifest with a declarativeAgents entry
- Point it at a SharePoint site with test documents
- Sideload in Teams Developer Mode
- Test: ask it a question that requires the SharePoint documents
- Does it correctly retrieve and cite?

### Exercise 5 — Data Governance Audit
For JMA's current AI pipeline (EnterpriseSearch.Sync):
- Is PII being stripped before indexing in AI Search? (check the code)
- Does AI Search have row-level security filtering? (check index schema)
- Are Azure OpenAI requests logged for audit? (check diagnostic settings)
- Are SharePoint documents deleted from AI Search when removed from SharePoint? (check cleanup logic)
- Where are the gaps? What would you fix first?

---

*Previous: Module 19 — MLOps / LLMOps*
*This is the final module — next step: Module 19 (Hands-On Projects) — build a full JMA AI solution end-to-end*
*Updated: 2026-06-30*
