# Module 2 — Azure AI Platform
**Source plan:** `AIML-Learn/04_Career/00_PRD.md` §4–5, `01_EXECUTION_PLAN.md`
**Format:** WHY / HOW / WHEN / SCALE / DEPLOY
**Question count:** 18

---

### Q1. Multi-service Azure AI Services resource vs individual per-service resources — how do you decide?

- **WHY:** Multi-service = one endpoint, one key, simpler billing/management. Individual = isolated RBAC, quotas, and billing per service.
- **HOW:** Same base endpoint (`*.cognitiveservices.azure.com`) routes to different capabilities by API path for multi-service; individual resources each get their own endpoint.
- **WHEN:** Multi-service for dev/test or tightly coupled pipelines with one owning team. Individual resources once different services have different owners, different compliance boundaries, or need independent quota/scaling.
- **SCALE:** Individual resources scale/quota independently — a Document Intelligence spike doesn't starve Speech's quota. Multi-service shares one quota pool across all services on it.
- **DEPLOY:** Local/dev — multi-service is fine everywhere. Single/multi-region production — split to individual resources once per-service RBAC, monitoring, or regional placement diverges (e.g., Document Intelligence must stay in-region for data residency while Speech doesn't need to).

**Follow-up probe:** "Your dev multi-service resource works fine, then production Document Intelligence starts throttling and takes down Speech calls too — what happened and what's the fix?" (Shared quota pool on the multi-service resource; fix is splitting to individual resources with independent TPS budgets.)

---

### Q2. API keys vs Managed Identity — which do you standardize on, and why?

- **WHY:** Managed Identity eliminates secrets to rotate, leak, or accidentally commit; API keys are simpler to start with but are a standing liability.
- **HOW:** `DefaultAzureCredential()` resolves identity from the compute resource (Function App system-assigned MI, etc.) with zero secrets in code or config.
- **WHEN:** API keys are acceptable for local dev/quick prototyping only. Managed Identity is the production standard — non-negotiable once real data flows through it.
- **SCALE:** Key rotation at scale (many services, many consumers) is an operational burden that grows linearly with resource count; Managed Identity removes that scaling cost entirely.
- **DEPLOY:** Local dev can use keys via user secrets/env vars (never committed). Single-region and multi-region production both standardize on Managed Identity + RBAC — this doesn't change by deployment tier, it's a baseline.

**Follow-up probe:** "Security flags an app still using API keys in production — walk the exact remediation steps." (Assign system-assigned MI to the compute resource → grant Cognitive Services User role → swap `AzureKeyCredential` for `DefaultAzureCredential()` → delete the key-based env vars → rotate/revoke the old key.)

---

### Q3. Cognitive Services User vs Contributor — who gets which role?

- **WHY:** Least-privilege — inference-only callers shouldn't be able to modify or delete the resource.
- **HOW:** **User** = call APIs only. **Contributor** = manage resource config + call APIs. **Owner** = full control including billing/access.
- **WHEN:** Apps/Functions/web APIs → User. CI/CD service principals and developers who provision/configure → Contributor. Admins only → Owner.
- **SCALE:** As the number of consuming services grows, User-role assignment is what keeps the blast radius of a compromised app identity small — it can't reconfigure or delete the resource even if compromised.
- **DEPLOY:** Same role model at every deployment tier — role assignment doesn't change with region count, but multi-region means assigning User to each region's compute identity against each region's resource (or a central identity if using cross-region private networking).

**Follow-up probe:** "A compromised Function App identity has Contributor instead of User — what's the actual damage difference?" (Contributor can change network rules, diagnostic settings, or delete the resource entirely — not just misuse it for inference. That's the whole point of the distinction.)

---

### Q4. VNet Service Endpoints + firewall rules vs Private Endpoint — when do you need the stronger option?

- **WHY:** Service Endpoints restrict *who* can reach a still-public endpoint (IP/VNet allow-list). Private Endpoint removes the public endpoint entirely — traffic never leaves the VNet.
- **HOW:** Service Endpoint = `networkAcls` with `defaultAction: Deny` + VNet/IP rules, traffic still transits the public address space logically. Private Endpoint = a private IP inside your VNet, public access disabled.
- **WHEN:** Service Endpoints for a fast, low-cost restriction when "public address, but locked down" is acceptable. Private Endpoint when compliance mandates no public exposure at all (PHI, financial documents, contracts) — matches JMA's Document Intelligence handling contracts/financial docs.
- **SCALE:** Private Endpoint doesn't materially change scaling behavior of the AI service itself, but adds DNS/routing complexity that must be replicated per region in a multi-region design.
- **DEPLOY:** Local/dev — neither typically needed. Single-region prod — Service Endpoint is often sufficient. Multi-region/global prod handling regulated data — Private Endpoint per region, with private DNS zones linked across the network topology so cross-region calls still resolve correctly.

**Follow-up probe:** "You add a Private Endpoint and DNS resolution now returns the old public IP for some clients — what's wrong?" (Private DNS zone isn't linked to the VNet those clients are in, or DNS caching — clients need to resolve via the private DNS zone, not the public one.)

---

### Q5. What do you monitor on every Azure AI Services resource, and why those specific things?

- **WHY:** Metrics tell you *what's happening now*; diagnostic logs tell you *what happened to a specific request* (audit trail); alerts tell you *before a human notices*.
- **HOW:** Azure Monitor Metrics (SuccessfulCalls, TotalErrors, ClientErrors 4xx, ServerErrors 5xx, Latency) + Diagnostic Logs to Log Analytics (Audit, RequestResponse categories) + Alert rules on error-rate/latency thresholds.
- **WHEN:** From day one of any production resource — not something retrofitted after an incident.
- **SCALE:** As call volume grows, raw log volume grows with it — need sampling/retention policy decisions in Log Analytics to control cost, and alert thresholds need to be relative (error rate %) not absolute (error count) so they stay meaningful as traffic scales.
- **DEPLOY:** Single-region — one Log Analytics workspace suffices. Multi-region/global — decide centralized (all regions log to one workspace for a unified view) vs regional (data residency may force logs to stay in-region) — this is itself a data-residency decision, not just an observability one.

**Follow-up probe:** "A healthcare client says logs can't leave the region — how does that change your multi-region monitoring design?" (Regional Log Analytics workspaces per region instead of centralized; cross-region visibility via aggregated *metrics* (not raw request/response logs) shipped to a central dashboard, since metrics don't carry the regulated payload.)

---

### Q6. How do you handle 429 throttling, and why doesn't "just add more instances" work?

- **WHY:** TPM/TPS quota is enforced per-resource, not per-caller — more callers hitting the same resource still share the same ceiling.
- **HOW:** Polly (or equivalent) exponential backoff honoring `Retry-After`; this buys resilience, not more capacity.
- **WHEN:** Backoff/retry is baseline hygiene everywhere. Quota increase requests or a second resource are needed once sustained traffic approaches the quota ceiling, not just for occasional bursts.
- **SCALE:** More Function instances hitting the same AI resource still share one quota — scaling compute horizontally does nothing for a quota-bound bottleneck. Real fixes: request quota increase, round-robin across multiple resources (doubles effective TPM), or queue/pace ingestion upstream (Service Bus) to smooth bursts.
- **DEPLOY:** Single-region — one resource with a queue in front is often enough. Multi-region — round-robin across regional resources both increases effective quota *and* gives you regional failover for free, which is the deployment-scale answer, not just the throttling answer.

**Follow-up probe:** "Your team lead insists more Function instances will fix the 429s — how do you explain why, and what do you propose instead?" (Quota is resource-scoped, not caller-scoped, so instance count is irrelevant to the ceiling; propose quota increase + Service Bus-fronted pacing, or multi-resource round-robin if regional failover is also a goal.)

---

### Q7. Azure OpenAI: PTU (Provisioned Throughput) vs pay-as-you-go — how do you choose?

- **WHY:** PAYG bills per token, shared capacity, subject to dynamic throttling under load. PTU reserves dedicated throughput capacity for predictable, guaranteed latency/throughput at a fixed cost.
- **HOW:** PTU is purchased/reserved in throughput units ahead of time (often with a commitment term); PAYG scales automatically with usage, billed per token consumed.
- **WHEN:** PAYG for variable/unpredictable/low-to-moderate volume workloads, prototyping, or spiky traffic. PTU once you have predictable high-sustained volume and latency SLAs that PAYG's dynamic throttling can't guarantee.
- **SCALE:** PAYG effective throughput ceiling is shared/dynamic — you can be throttled by *other tenants'* demand on the shared pool during peak periods. PTU throughput is dedicated — your ceiling doesn't move regardless of what other customers are doing.
- **DEPLOY:** Local/dev — always PAYG. Single-region production with real SLAs — PTU. Multi-region/global — PTU purchased per region where guaranteed throughput matters, PAYG as overflow/burst capacity behind the same routing layer (hybrid model).

**Follow-up probe:** "Your production endpoint has predictable Monday-morning traffic spikes and PAYG throttles every time — what's the fix, and is PTU alone sufficient?" (PTU sized to the sustained baseline plus a PAYG overflow path for the spike above baseline — full PTU sized to peak wastes money the rest of the week; this is the pricing/cost-optimization theme that gets developed further in Module 5.)

---

### Q8. How do you handle Azure OpenAI model version deprecation in a production architecture?

- **WHY:** Model versions are retired on a schedule (Microsoft publishes deprecation dates) — an architecture hard-coded to a specific version will break on a date you don't control unless designed for it.
- **HOW:** Deploy behind an abstraction (a named deployment, or an internal routing layer) rather than referencing a raw model version directly in application code; monitor Microsoft's deprecation announcements as an operational input, not a surprise.
- **WHEN:** Design for this from the first production deployment — retrofitting version-agnostic routing after a deprecation deadline is a fire drill, not an architecture decision.
- **SCALE:** The more services/teams directly reference a specific model version, the more coordinated the migration effort — centralizing the model reference behind one routing layer turns an org-wide migration into a single config change.
- **DEPLOY:** Applies uniformly regardless of region count — but multi-region adds the requirement that the *new* model version must actually be available in every region you're deployed to before you can cut over, which needs to be checked, not assumed.

**Follow-up probe:** "A model version deprecates in 60 days and it's not yet available in one of your three deployed regions — what do you do?" (Escalate/track regional availability with Microsoft, plan a temporary cross-region routing fallback for that region's traffic, or fall back to PAYG in a region where the new version is available while capacity catches up — don't wait until day 60.)

---

### Q9. Azure AI Search: keyword vs vector vs hybrid — when is pure keyword actually the right call?

- **WHY:** Keyword search is exact-match/lexical (good for known terms, IDs, codes); vector search is semantic (good for conceptual/paraphrased queries); hybrid combines both, typically outperforming either alone.
- **HOW:** Hybrid = BM25 keyword scoring + vector cosine similarity, fused (often reciprocal rank fusion) into one ranked result set, optionally with a semantic re-ranker on top.
- **WHEN:** Pure keyword is still right when queries are precise/structured (SKU lookup, exact dealer code, invoice number) — vector search adds cost and can actually *hurt* precision on exact-match queries by surfacing "semantically similar but wrong" results.
- **SCALE:** Vector search costs more per query and more storage (embeddings) than keyword-only; at high query volume with a large index, that cost difference compounds — a real reason JMA's current keyword-only prod index hasn't yet moved to hybrid.
- **DEPLOY:** Same trade-off applies at every deployment tier — this is a query-pattern decision, not a topology decision. What changes across regions is embedding-model availability and any latency added by cross-region vector index replication.

**Follow-up probe:** "JMA's production search index is keyword-only and staging is vector-enabled — what's your recommendation and rollout plan for closing that gap?" (Recommend hybrid, not vector-only, given exact-match query patterns likely still exist; stage rollout via the existing EnterpriseSearch.Sync Push API pipeline, validate relevance on real query logs before flipping prod traffic.)

---

### Q10. Push API vs indexer (pull) model for Azure AI Search — how do you decide?

- **WHY:** Push API gives your application full control over exactly what/when gets indexed (real-time, event-driven). Indexers pull from a supported data source on a schedule or via change detection, with less custom code but less control.
- **HOW:** Push = your app calls the Search REST/SDK API directly with documents (this is JMA's EnterpriseSearch.Sync pattern). Indexer = configured against a supported source (Blob, SQL, Cosmos) with a schedule or change-feed trigger.
- **WHEN:** Push API when you need real-time freshness, custom enrichment/transformation before indexing, or your source isn't a natively supported indexer data source. Indexer when the source is natively supported and near-real-time (not real-time) freshness is acceptable — less code to maintain.
- **SCALE:** Push API scales with your application's own throughput/reliability engineering (retry, batching); indexers scale via the platform's own scheduling but are bounded by indexer run frequency and source-side change detection latency.
- **DEPLOY:** Local/dev — either works. Multi-region — Push API gives you the control to explicitly fan out writes to regional replica indexes; indexers would need per-region indexer configs pointed at regional (or replicated) data sources.

**Follow-up probe:** "Why did JMA choose Push API for EnterpriseSearch.Sync instead of an indexer?" (Likely: custom enrichment/transformation logic before indexing, and/or the source isn't a natively supported indexer data source, and/or real-time freshness requirements the indexer's schedule couldn't meet.)

---

### Q11. Document Intelligence: prebuilt models vs custom — what's the actual decision threshold?

- **WHY:** Prebuilt models (invoice, receipt, layout, W-2, etc.) are maintained and improve automatically at no extra training cost; custom models require labeled data, training, and ongoing maintenance as your document formats evolve.
- **HOW:** Prebuilt = call the model directly against a document. Custom = label 5-15+ sample documents (varies by complexity) in the Document Intelligence Studio, train, evaluate, deploy.
- **WHEN:** Use prebuilt whenever it covers the fields you need. Go custom only when required fields aren't in the prebuilt schema (e.g., JMA-specific `VehicleMake`, `DealerCode`) — don't build custom for fields prebuilt already extracts.
- **SCALE:** Prebuilt scales with zero additional operational burden as document volume grows. Custom models add a maintenance burden that scales with how often the source document format changes — a vendor changing their invoice template can silently degrade a custom model's accuracy.
- **DEPLOY:** Cloud endpoint for most cases. Containerized Document Intelligence (Layout, Read, and a growing set of prebuilt models) for on-prem/data-sovereignty requirements — but note Azure OpenAI itself cannot be containerized, which caps how far a fully-offline document pipeline can go if it also needs generative summarization downstream.

**Follow-up probe:** "Prebuilt Invoice model gives you Vendor/Total/Date but you need VehicleMake and DealerCode too — do you replace it with a custom model or something else?" (Run both — prebuilt Invoice for the fields it already covers well, plus a custom extraction model or Custom NER layer specifically for the JMA-specific fields, rather than reinventing what prebuilt already does well.)

---

### Q12. What does Azure AI Foundry unify, and when do you actually need it vs raw resource management?

- **WHY:** Foundry centralizes model deployment, evaluation, tracing, Content Safety configuration, and agent management in one portal/SDK surface instead of managing each Azure AI resource independently via the Azure portal.
- **HOW:** Foundry projects wrap underlying Azure AI Services/OpenAI resources — you still have the same underlying resources and billing, but a unified control plane on top.
- **WHEN:** Valuable once you're managing multiple models/deployments/agents and need centralized evaluation and tracing — overkill for a single simple integration against one Azure OpenAI deployment.
- **SCALE:** The unified control plane's value grows with the number of models, agents, and evaluation pipelines you're coordinating — it's an operational-complexity multiplier reducer, not a raw-throughput lever.
- **DEPLOY:** Foundry projects can span regions the same way the underlying resources do; the portal/control-plane layer itself doesn't add a deployment-topology constraint beyond what the underlying Azure AI Services already have.

**Follow-up probe:** "Your team manages one Azure OpenAI deployment and one AI Search index — do you recommend adopting Foundry now?" (Not yet — the coordination overhead Foundry solves doesn't exist yet at that scale; revisit once evaluation pipelines, multiple agents, or multiple models enter the picture.)

---

### Q13. Content Safety: what are you actually configuring, and where does groundedness detection fit?

- **WHY:** Content Safety protects against harmful/inappropriate content (hate, violence, self-harm, sexual categories with severity levels) on both input and output; groundedness detection is a *separate* capability that checks whether a generated answer is actually supported by the provided grounding context — distinct problem (hallucination, not harmful content).
- **HOW:** Category + severity threshold configuration per category (0-Safe through higher severity tiers), applied as an input filter, output filter, or both; groundedness detection runs post-generation against the retrieved context to flag ungrounded claims.
- **WHEN:** Content Safety on every production GenAI endpoint without exception — non-negotiable baseline. Groundedness detection specifically wherever RAG/grounded answers are being produced and hallucination risk has real consequence (any regulated or customer-facing content).
- **SCALE:** Both add per-call latency and cost — at high volume, this becomes a real line item, which is why threshold tuning (not blanket maximum strictness) matters; over-blocking legitimate content at scale has its own cost (support tickets, user friction).
- **DEPLOY:** Same configuration applies regardless of deployment tier, but multi-region/global deployments handling regulated content (e.g., healthcare) should treat Content Safety + groundedness thresholds as a compliance-mandated baseline, not a tunable per-region.

**Follow-up probe:** "Your RAG answers are well-grounded in retrieved documents but Content Safety still flags them — what's actually happening?" (Content Safety and groundedness are separate systems checking separate things — grounded-but-flagged means the *content itself* (even if accurate/grounded) trips a harm category, e.g., a medical document legitimately containing clinical detail that trips a severity threshold; requires threshold tuning per use case, not disabling the check.)

---

### Q14. When do Azure AI Services containers make sense over the cloud endpoint?

- **WHY:** Containers solve disconnected/air-gapped environments, data sovereignty, ultra-low latency, and regulatory on-prem requirements — not cost savings at typical volume (cloud is usually cheaper below very high scale).
- **HOW:** Pull from MCR, run with `ApiKey`/`Billing`/`Eula` env vars — the container still phones home to Azure for billing even when "disconnected"; raw request data stays local, billing metadata doesn't.
- **WHEN:** Only when one of the hard constraints applies — otherwise cloud endpoint is simpler and always current. Azure OpenAI specifically cannot be containerized at all, which caps this option for any pipeline needing generative capability, not just extraction/classification.
- **SCALE:** Containers shift the scaling model from "Azure manages capacity" to "you provision and scale Kubernetes/compute yourself" — budget 2-4GB RAM and 1-2 cores per instance, and you now own the autoscaling/sizing problem the cloud endpoint used to hide.
- **DEPLOY:** This *is* the deployment-tier question — local/on-prem container for the disconnected tier, cloud endpoint everywhere else. A hybrid design (containers at edge sites, cloud endpoint for everything else) is common when only some sites have the hard constraint.

**Follow-up probe:** "A factory-floor site has no reliable internet — Document Intelligence container solves extraction, but the pipeline also needs Azure OpenAI summarization. What's the architecture?" (Extraction runs locally in the container; summarization either queues for processing once connectivity returns, or the container-based extraction output is batched and sent to the cloud Azure OpenAI endpoint asynchronously — Azure OpenAI's non-containerizability forces a hybrid/asynchronous design at disconnected sites.)

---

### Q15. In a shared multi-service resource, how do you maintain a security boundary between different consuming teams/services?

- **WHY:** A single multi-service resource means one set of keys/identity permissions grants access to *every* service on it — a team needing only Language access could technically call Vision too, unless boundaries are enforced elsewhere.
- **HOW:** RBAC alone can't scope access to a *subset* of services within one multi-service resource — the real boundary comes from splitting into individual per-service resources once teams/trust boundaries diverge, or fronting the shared resource with an API gateway (APIM) that enforces per-consumer service-level authorization.
- **WHEN:** Acceptable to share when all consumers are within the same trust boundary (one team, one app). Split or gateway-front once consumers cross team/trust/compliance boundaries.
- **SCALE:** As more independent teams onboard to a shared resource, the coordination and blast-radius cost compounds — this is a governance problem that gets harder to unwind the longer it's deferred.
- **DEPLOY:** Applies identically across deployment tiers — but a multi-region deployment with regionally-scoped teams is a natural forcing function to finally split into per-region, per-team resources rather than one shared global one.

**Follow-up probe:** "Two teams share one multi-service resource; one team's compromised key now has implicit access to the other team's service surface — how do you explain this happened given RBAC was correctly configured?" (RBAC on the resource grants access to *the resource*, not to a service *within* it — this is exactly the multi-service resource's structural limitation; the fix is splitting to individual resources or adding a gateway layer, not tightening RBAC further.)

---

### Q16. Semantic ranker vs a custom reranking model in Azure AI Search — when do you build your own?

- **WHY:** Azure AI Search's built-in semantic ranker is a managed, general-purpose re-ranking layer — good default, zero training required. A custom reranker (fine-tuned cross-encoder) can outperform it on domain-specific relevance judgments the general model wasn't trained on.
- **HOW:** Semantic ranker re-scores the top-N candidates from initial retrieval using a Microsoft-maintained model. Custom reranking requires training/fine-tuning a cross-encoder on your own labeled relevance data and hosting/calling it as an extra pipeline stage.
- **WHEN:** Start with semantic ranker always. Only build custom once you have labeled relevance data showing the built-in ranker systematically mis-ranks your domain's queries, and the relevance gap is worth the build/maintenance cost.
- **SCALE:** Custom reranking adds a real inference cost/latency step per query on top of retrieval — justified only when the relevance lift is large enough to matter at your query volume.
- **DEPLOY:** Semantic ranker is a managed service feature — no deployment-topology decision. A custom reranker adds its own hosting/scaling/regional-deployment question, effectively becoming another model you now operate.

**Follow-up probe:** "How would you prove a custom reranker is actually worth building before investing in it?" (Offline evaluation: collect labeled relevance judgments for representative queries, measure NDCG/MRR of semantic ranker alone vs semantic ranker + candidate custom reranker on held-out data — build only if the lift clears a threshold that justifies the ongoing hosting/maintenance cost.)

---

### Q17. What's a sound disaster-recovery strategy for an Azure AI Search index?

- **WHY:** Azure AI Search doesn't offer automatic cross-region failover for a single service the way some other Azure data services do — the index itself lives in one region unless you explicitly design for redundancy.
- **HOW:** Options: (1) re-index from source-of-truth on demand into a secondary-region service (works well when Push API/indexer pipeline can reliably rebuild the index), (2) maintain a warm secondary index kept in sync via the same Push API writes fanned out to both regions, (3) periodic index backup/restore for slower-RPO scenarios.
- **WHEN:** Warm secondary (near-zero RPO) for search that's business-critical/customer-facing. Re-index-on-demand is acceptable when the source-of-truth is durable and re-indexing time is within your RTO tolerance.
- **SCALE:** Re-index-on-demand's RTO scales with index size and source-retrieval throughput — fine for a modest index, potentially hours for a very large one, which pushes toward the warm-secondary approach as data volume grows.
- **DEPLOY:** This *is* the multi-region question directly — active-passive (secondary idle until failover) is simpler and cheaper; active-active (both regions serving live traffic, both kept in sync) gives better RTO/latency but doubles ongoing cost and requires the write path (Push API) to reliably fan out to both.

**Follow-up probe:** "Your source-of-truth documents live only in one region's storage — does that block an active-active search deployment?" (Not necessarily, but it does mean the search layer's regional redundancy is only as good as the source data's — a full DR design needs source-data replication addressed too, not just the index; this is where the search DR question connects back to the whole pipeline's region strategy.)

---

### Q18. How would you architect for a future Azure AI service deprecation or breaking API change beyond model versions (e.g., an SDK major version bump)?

- **WHY:** SDKs and REST API versions evolve; a tightly-coupled integration (direct SDK calls scattered across the codebase) makes a breaking change an org-wide, high-risk migration.
- **HOW:** Wrap Azure AI Service calls behind an internal abstraction/interface layer specific to your domain (e.g., an `IDocumentExtractionService` interface, not raw `DocumentAnalysisClient` calls scattered through business logic) — the SDK/API version becomes an implementation detail behind one boundary.
- **WHEN:** Design this in from the start of any non-trivial integration — retrofitting an abstraction layer after a breaking change has already forced a rewrite is far more expensive than building it up front.
- **SCALE:** The abstraction's value compounds with the number of call sites across the codebase — for a single call site it's arguably overhead; for a production pipeline touched by many features, it's what keeps a version bump a single-file change instead of a multi-team migration.
- **DEPLOY:** Same principle regardless of deployment topology — but multi-region deployments benefit further since a rollout of the new SDK/API version can be staged region-by-region behind the abstraction, rather than requiring a global simultaneous cutover.

**Follow-up probe:** "Microsoft announces a breaking REST API version bump with a 6-month deadline — walk through your rollout plan given the abstraction layer already exists." (Implement and test the new API version behind the same interface in a feature-branch/staging environment → validate against the same test suite the old implementation used → stage rollout region-by-region or behind a feature flag → deprecate the old implementation once all regions are confirmed on the new version, well before the deadline.)

---

*Module 2 of 6 — GenAI Architect Interview Prep. Next: Module 3 — RAG Architecture.*
