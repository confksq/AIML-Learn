# Q&A — L20: Integration Patterns
**Source chapter:** `01_Lessons/Part4_Architecture/L20_IntegrationPatterns.md` | **Format:** self-study
**Questions:** 30 | *No overlap with the interview bank or the chapter's own recall quiz — these drill the chapter's specifics.*

---

## Azure Integration Services for AI

**Q1. Name the four categories of the AI integration stack with an example each.**
**Event-driven** (Event Grid, Service Bus, Event Hubs — trigger AI processing), **Compute** (Azure Functions, Container Apps, AKS — run AI workloads), **Orchestration** (Data Factory, Logic Apps, API Management — coordinate pipelines), **Storage** (Blob, Cosmos DB, SQL — pipeline state and results).

**Q2. Why are Azure Functions the most common AI compute layer at JMA? Five reasons.**
Triggered by many sources (Blob, Service Bus, Event Grid, HTTP, Timer); pay per execution (zero cost when idle); auto-scales for bursts; Managed Identity built-in (call AI services without keys); native C# support (Semantic Kernel, Azure OpenAI SDK, DI all work).

**Q3. In the EnterpriseSearch.Sync Function pattern, what are the four processing steps in order?**
(1) Extract with Document Intelligence → (2) **PII detection/removal** before indexing → (3) generate embedding → (4) push to AI Search (id, content, contentVector, fields, processedAt). It's a BlobTrigger Function firing on new documents.

**Q4. What does Event Grid do, and what can it route a BlobCreated event to?**
It's an event bus that routes events from Azure services to subscribers. A BlobCreated event can fan out to multiple subscribers simultaneously — an Azure Function (process the doc), a Logic App (notify the team), and a Service Bus queue (for rate-limited processing).

**Q5. Event Grid vs Service Bus — the core pattern difference and a JMA use for each.**
**Event Grid** — push, fire-and-forget, at-least-once, no ordering; JMA: "new PDF in blob → start DI processing." **Service Bus** — pull (consumer reads at its own pace), durable queue, FIFO ordering available, dead-letter; JMA: "rate-limit DI calls to 10/sec."

**Q6. How does Service Bus prevent 429 throttling — the without/with contrast?**
Without: 100 files uploaded at once → 100 simultaneous DI calls → ~30 get 429. With: 100 messages enqueued → a Function reads ~10/second → all 100 succeed, no throttling. The queue absorbs the burst and paces consumption.

**Q7. In the Service Bus Function, what do CompleteMessage, AbandonMessage, and DeadLetterMessage each do?**
`CompleteMessage` — success, remove from queue. `AbandonMessage` — on a 429, put it back for retry (with delay). `DeadLetterMessage` — on a permanent error, move to the dead-letter queue for manual review.

**Q8. As an AI gateway, what six things does APIM do in front of Azure OpenAI?**
Rate limiting (per team + total), semantic caching (cache similar queries), load balancing (round-robin across deployments), logging (every call to Log Analytics), auth (validate caller identity), and transformation (add headers, strip PII from logs).

**Q9. What three APIM policies appear in the chapter's Azure OpenAI config?**
`rate-limit-by-key` (per subscription/team), `azure-openai-semantic-cache-lookup` (score-threshold 0.85 on inbound), `set-backend-service` (load-balance pool) — with `azure-openai-semantic-cache-store` (duration 1800s) on outbound.

**Q10. ADF vs Azure Functions — trigger, volume, and JMA use for each.**
**ADF** — scheduled/manual/event trigger, millions of records, built-in orchestration (if/else, loops, error handling), visual runs UI; JMA: nightly batch re-enrichment. **Azure Functions** — event-driven/real-time, thousands per minute, code-based orchestration, App Insights; JMA: real-time doc processing on upload.

**Q11. Walk the four activities of the ADF monthly re-enrichment pipeline.**
(1) **Get Files** (Lookup — Cosmos DB for docs older than 90 days). (2) **For Each** (parallel batches of 50 → Web Activity calls a Function that re-analyzes with a new DI model, re-embeds, updates AI Search). (3) **Verify** (check AI Search doc count matches expected, alert if not). (4) **Notify** (POST to Teams webhook with completion summary).

---

## Microsoft 365 Integration

**Q12. Name the four parts of the Microsoft Copilot ecosystem (2026).**
M365 Copilot (built into Word/Excel/PowerPoint/Outlook/Teams; extensible via plugins/declarative agents), Copilot Studio (low-code custom agents), Azure AI Foundry Agents (pro-code SK/Python, published via Bot Service), and Microsoft Graph (API for all M365 data).

**Q13. What is Microsoft Graph, and name four endpoints it exposes.**
The API giving AI access to M365 company data. Endpoints: `/me/messages` (emails), `/me/drive/root` (OneDrive files), `/me/calendar/events` (calendar), `/sites/{id}/lists` (SharePoint), `/teams/{id}/channels` (Teams messages), `/users/{id}/manager` (org chart).

**Q14. How does the Graph Function example authenticate and find recent SharePoint files?**
Uses `DefaultAzureCredential()` → `GraphServiceClient` (Managed Identity, no keys). Queries the site's drive with a filter `lastModifiedDateTime ge {yesterday}`, selecting id/name/lastModified/webUrl, then downloads each file's content and processes it with Document Intelligence.

**Q15. What's the better JMA SharePoint integration than a manual indexer?**
**Graph change notifications (webhooks)** — fire when SharePoint files change → trigger an Azure Function → process only new/updated documents, instead of polling everything on a schedule.

**Q16. What is a Declarative Agent, and how do users invoke it?**
A no-code extension of M365 Copilot with custom knowledge/tools — defined in a JSON manifest (name, instructions, capabilities like SharePoint sites/WebSearch, and API actions). It appears inside M365 Copilot chat; users invoke it with `@JMA Dealer Support` in Teams or Outlook.

**Q17. When would a JMA manager use a Declarative Agent vs a full Foundry Agent?**
**Declarative Agent** for the fastest no-code path — point it at the SharePoint site with dealer agreements, deploy as a Teams App, no IT build. **Full Foundry Agent** for production with custom business logic (API calls, complex routing) needing pro-code control.

**Q18. What is Power Platform AI Builder, and name three of its no-code AI features?**
No-code AI for non-developer teams. Features: Document Processing (wraps DI — invoice extraction in Power Automate), Text Classification (route tickets), Sentiment Analysis (flag negative dealer emails), Object Detection (vehicle damage photos), Azure OpenAI (GPT-4o in flows). Use Power Platform when a non-dev team owns the process; Azure SDK when there's complex logic/SLA/IT ownership.

---

## Enterprise Data Integration

**Q19. Name five enterprise data sources and how each connects into the AI pipeline.**
SharePoint → Microsoft Graph → Function; Blob Storage → BlobTrigger; SQL Database → SQL Trigger; Email (Exchange) → Graph; SAP/Dynamics 365 → API; IoT/Telemetry → Event Hubs → Anomaly Detector. All converge → Document Intelligence → AI Search (indexed) → Azure OpenAI RAG (queryable).

**Q20. Contrast the real-time and batch pipeline flows.**
**Real-time (event-driven):** SharePoint upload → Graph webhook → Event Grid → Function fires immediately → DI extracts (<30s) → AI Search updated (<1min total) → dealer can query within a minute. **Batch (scheduled):** nightly 2am ADF → query SQL for unvectorized invoices → batch-embed (Batch API, 50% cost) → bulk upload to AI Search → update Cosmos status → log metrics.

**Q21. When do you choose real-time vs batch?**
Real-time when the dealer needs to query newly uploaded documents immediately. Batch for historical backfill, cost optimization, and non-urgent enrichment where overnight latency is acceptable.

**Q22. Recite the five areas of the AI data-governance checklist.**
**Classification** (Public/Internal/Confidential/Restricted; Restricted never to external LLMs), **PII Handling** (detect/redact before indexing, not in logs, purge on GDPR/CCPA request), **Access Control** (row-level security filter, no cross-department exposure, audit log), **Retention** (OpenAI requests not stored, AI Search docs deleted with source, conversation history purged after 90 days), **Compliance** (data residency in-region, customer-managed keys, monthly legal review).

**Q23. What does Azure Synapse + AI enable, and what's the JMA dashboard use case?**
Synapse Serverless SQL reads the AI Search index as an external table; Synapse Spark runs batch embedding at scale; Synapse ML trains on large datasets; Power BI connects for AI-enriched analytics. JMA: nightly ingest all support tickets → Spark batch-classifies + sentiment-scores them → SQL aggregates by dealer/region/issue → Power BI "Dealer Sentiment Dashboard" shows weekly trends.

---

## Applied (Recall Quiz & Exercises)

**Q24. 500 dealer agreements uploaded to SharePoint at once, each needing DI — what happens without throttle control and how do you fix it?**
DI gets 500 simultaneous requests → most return 429. Fix: **Event Grid captures the upload events → routes to a Service Bus queue → a Function reads one (or ~5–10/sec) at a time → processes at a sustainable rate**, with a dead-letter queue catching permanent failures for manual review.

**Q25. Why does JMA specifically need APIM given multiple teams share one Azure OpenAI resource?**
Without APIM, one team's spike can **exhaust the shared quota and throttle everyone else**. APIM enforces per-team rate limits, plus semantic caching (cost), load balancing across deployments (resilience), and per-call logging (compliance audit) — turning a shared resource into a governed one.

**Q26. Fastest path for a manager to ask M365 Copilot about SharePoint dealer agreements without IT building an app?**
A **Declarative Agent** for M365 Copilot pointed at the SharePoint site — no code, deploys as a Teams App, appears as `@JMA Dealer Support`, Copilot retrieves and cites automatically. Upgrade to a Foundry Agent only if custom business logic is later needed.

**Q27. Full pipeline design (Exercise 1) — list the trigger-to-index chain and where dead-lettering goes.**
Trigger: dealer uploads PDF to SharePoint → **Graph webhook → Event Grid → Service Bus → Azure Function → Document Intelligence → PII strip → embed → AI Search push**. Dead-letter at each step (Service Bus DLQ for processing failures) so nothing is silently lost; monitor a health metric at each stage (queue depth, DI success rate, index count delta).

**Q28. APIM rate-limiting setup (Exercise 2) — what policy, what limit, and what do you observe testing 70 rapid calls?**
Add a `rate-limit-by-key` policy of 60 calls/minute per subscription key on the Azure OpenAI API (Consumption tier). Sending 70 quick calls: the first 60 pass, the remaining **~10 return 429**; APIM analytics show the call distribution and throttling.

**Q29. Data-governance audit (Exercise 5) — what four checks do you run against EnterpriseSearch.Sync?**
Is PII stripped before indexing (check the code)? Does AI Search have row-level security filtering (check index schema)? Are Azure OpenAI requests logged for audit (check diagnostic settings)? Are documents deleted from AI Search when removed from SharePoint (check cleanup logic)? Identify the gaps and fix the highest-risk one first.

**Q30. Recite the module's memory hooks in one line each.**
Functions = real-time AI trigger, ADF = batch orchestrator. Event Grid = push fire-and-forget, Service Bus = pull with durability. APIM = AI gateway (rate limit + cache + load balance + log). Microsoft Graph = API for all M365 data. Declarative Agent = M365 Copilot extension, no code. Governance = classify → PII strip → access control → retain → comply.

---

*Curriculum Q&A Batch F — file 1 of 2. Next: QA_L21 (Python for AI) — the final file.*
