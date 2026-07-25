# Module 5 — Solution & Deployment Architecture
**Source plan:** `AIML-Learn/04_Career/00_PRD.md` §4–5, `01_EXECUTION_PLAN.md`
**Format:** WHY / HOW / WHEN / SCALE / DEPLOY
**Question count:** 35 — this is the deployment-scale centerpiece module, walking the full ladder: local → single-region → multi-region → global, plus caching and pricing as first-class architectural decisions.

---

## 5a. Local / Dev Deployment (5)

### Q1. What does "local deployment" actually mean for a GenAI pipeline, given Azure OpenAI can't be containerized?

- **WHY:** Local/dev iteration needs to be fast and cheap, but the generative model itself is a hard constraint — Azure OpenAI has no local/container option (Module 2 Q14), unlike Document Intelligence or AI Search components.
- **HOW:** Everything containerizable (Document Intelligence, some AI Search-adjacent tooling) runs locally; the Azure OpenAI call itself still hits the real cloud endpoint even in "local dev," typically against a dedicated low-cost/low-quota dev deployment, not production capacity.
- **WHEN:** Local dev for everything except the LLM call itself — accept that generative dev/test always has a live-cloud-dependency, and design dev workflows (mocking, response recording) around that reality rather than pretending full offline dev is possible.
- **SCALE:** Not a scale concern at dev stage by definition — but dev-tier quota needs to be sized so a whole team iterating concurrently doesn't throttle each other on a shared low-quota dev deployment.
- **DEPLOY:** This is the bottom rung of the deployment ladder — establishes that even "local" has a cloud dependency for generation, which matters when discussing disconnected/air-gapped requirements later (Q4, Module 2 Q14).

**Follow-up probe:** "A developer wants a fully offline dev environment, no cloud calls at all — what do you tell them?" (Not fully achievable for the generative step specifically — offer response-recording/mocking (Q3) for iteration without live calls most of the time, but be upfront that true generation testing requires the cloud endpoint.)

---

### Q2. How do you isolate dev/test from production to avoid cross-contamination?

- **WHY:** Shared resources between dev and prod risk dev traffic consuming production quota, dev bugs writing test data into production indexes, or dev experiments affecting production monitoring/alerting signal quality.
- **HOW:** Fully separate Azure resources per environment (separate Azure OpenAI deployments, separate AI Search services, separate resource groups) with environment-scoped RBAC — not just separate app config pointing at shared infra.
- **WHEN:** From the first production deployment onward — retrofitting isolation after dev and prod have been sharing resources is a much bigger migration than starting isolated.
- **SCALE:** Isolation cost is roughly fixed (a second set of resources) regardless of production scale — cheap insurance relative to the cost of a dev bug affecting production data or quota.
- **DEPLOY:** Dev/test environments typically live in a single region regardless of how many regions production spans — no need to replicate the full multi-region topology in dev.

**Follow-up probe:** "A dev experiment accidentally wrote test documents into what turned out to be the production AI Search index — how did the isolation fail, and what's the fix?" (Shared index/resource across environments, likely via shared config/connection string — fix is fully separate resources per environment with distinct credentials, so a misconfigured dev pointer physically cannot reach production data.)

---

### Q3. What's a cost-zero (or near-zero) iteration strategy for GenAI development?

- **WHY:** Every live LLM call during active development iteration costs money and adds latency to the dev feedback loop — neither is necessary for most of the iteration cycle (prompt structure, code logic, UI).
- **HOW:** Record real responses once, then replay/mock them for repeated local iteration on everything downstream of the actual model call; reserve live calls for validating actual prompt/model-behavior changes, not every code change.
- **WHEN:** Default practice for any non-trivial GenAI development workflow — live-call-every-iteration is a default worth actively avoiding, not a neutral choice.
- **SCALE:** This has an outsized impact on developer velocity and dev-tier cost as team size grows — the savings compound with every developer and every iteration cycle across a team, not just a single person's workflow.
- **DEPLOY:** Purely a dev-tier practice — not applicable once in production, where live calls are the actual product behavior being served.

**Follow-up probe:** "How do you make sure mocked/recorded responses don't drift from real model behavior undetected?" (Periodically re-validate against live calls — e.g., a scheduled or pre-merge check that re-records and diffs against current live behavior — mocking accelerates iteration but needs a real-call checkpoint to catch model/prompt drift, not become a permanently stale fiction.)

---

### Q4. How do you handle local/lightweight search-index development without standing up full production-scale AI Search?

- **WHY:** Full-scale AI Search infrastructure is unnecessary overhead for iterating on chunking/retrieval logic during development — but the retrieval *behavior* still needs to be testable locally.
- **HOW:** A small-scale AI Search instance (lower tier) or an in-memory/lightweight vector store for unit-level iteration, reserving the full production-configuration index for integration/staging testing before release.
- **WHEN:** Lightweight local index for rapid unit-level chunking/retrieval logic iteration; full-config staging index before anything ships, since retrieval behavior (relevance ranking, hybrid search tuning) doesn't always transfer perfectly from a lightweight substitute.
- **SCALE:** Not a production scale concern — this is purely about keeping the dev feedback loop fast without paying for or waiting on full-scale infrastructure during early iteration.
- **DEPLOY:** Dev-tier only — staging/production always use the real, fully-configured service to catch configuration-specific behavior differences before release.

**Follow-up probe:** "Retrieval behaves differently in staging than it did against your lightweight local index — why might that be, and is it a problem?" (Full-scale service configuration — semantic ranker settings, actual production-scale index size affecting ranking behavior — can genuinely differ from a lightweight local substitute; this is expected and exactly why a staging validation pass against the real configuration is still required before shipping, not a sign the lightweight dev setup was wrong to use.)

---

### Q5. How do you test a GenAI pipeline in CI without making live, costly Azure AI calls on every commit?

- **WHY:** Live calls in CI on every commit are slow, costly at scale (many commits × many PR runs), and non-deterministic (model output isn't guaranteed identical run to run) — bad fit for fast, deterministic CI gates.
- **HOW:** Contract/schema tests against recorded responses for the bulk of CI (matching Q3's dev-loop pattern, applied to CI), with a smaller, separate scheduled or pre-release suite of real live-call integration tests that don't block every commit.
- **WHEN:** Recorded/mocked tests on every PR/commit as the fast gate; live-call integration tests on a slower cadence (nightly, or pre-release) as the real-behavior validation layer.
- **SCALE:** As commit frequency and team size grow, the cost/time savings of not live-calling on every commit compounds significantly — this is a direct CI cost and velocity lever.
- **DEPLOY:** CI/test infrastructure is independent of production deployment topology — but the live-call integration suite should target the same regions/configurations production actually uses, not just one arbitrary test region.

**Follow-up probe:** "Your fast CI suite is all green but a live-call integration test catches a real regression days later — is the CI strategy still sound?" (Yes, by design — fast recorded-response tests catch code/logic/schema regressions immediately; live-call tests catch actual model-behavior regressions on a slower cadence; the trade-off is intentional (fast feedback for most changes, slower but real validation for the parts recorded tests structurally can't catch) — the fix is making sure the live-call suite runs frequently enough that a regression's time-to-detection stays acceptable, not eliminating the layered approach.)

---

## 5b. Single-Region Production (6)

### Q6. How do you design for high availability within a single region for a GenAI pipeline?

- **WHY:** A single point of failure within one region (one AI Search replica, one App Service instance) turns any transient issue into a full outage — HA within a region is the baseline before even considering multi-region.
- **HOW:** Multiple replicas/instances behind a load balancer for compute (App Service/Container Apps scale-out), multiple AI Search replicas for query availability, and Azure OpenAI's managed service-level redundancy underneath the deployment you're calling.
- **WHEN:** Baseline requirement for any production workload — not an advanced/optional pattern, the starting point before multi-region is even discussed.
- **SCALE:** Replica/instance count scales with load — but HA's purpose (surviving a single instance failure) is orthogonal to raw scale; even a low-traffic production service needs at least 2 instances for HA, not just enough instances for throughput.
- **DEPLOY:** This is the single-region rung specifically — availability zones within the region (not yet cross-region) are the mechanism, distinct from the multi-region failover discussed in 5c.

**Follow-up probe:** "You have 3 App Service instances but they're all in the same availability zone — what's actually protected, and what isn't?" (Protected against a single-instance failure; not protected against a zone-level outage — true within-region HA needs instances spread across availability zones, not just multiple instances in the same zone.)

---

### Q7. How do you load-balance across multiple Azure OpenAI deployments within a region?

- **WHY:** A single deployment has a fixed quota ceiling (Module 2 Q6) — spreading load across multiple deployments (or multiple resources) within a region increases effective throughput and adds resilience if one deployment degrades.
- **HOW:** An application-layer or gateway-layer (APIM) round-robin or least-latency routing across multiple Azure OpenAI deployment endpoints, with health-check-aware routing that skips a degraded deployment.
- **WHEN:** Once sustained load approaches a single deployment's quota ceiling, or once you need resilience against a single deployment's degradation independent of overall load level.
- **SCALE:** Effective regional throughput scales roughly linearly with deployment count (each adds its own quota pool) — this is the within-region analog of the multi-resource round-robin pattern from Module 2 Q6.
- **DEPLOY:** Purely single-region here — the multi-region version of this same load-balancing idea is covered in 5c/5d as failover/global routing, a related but distinct problem (resilience across deployments vs resilience across regions).

**Follow-up probe:** "How is load-balancing across deployments within a region different from the multi-region failover pattern in 5c?" (Within-region load balancing is about throughput/resilience across peers with similar latency characteristics to the user; multi-region failover is about surviving a regional outage, with real latency/data-residency trade-offs the within-region case doesn't have — they solve different problems and are often both present in a mature architecture.)

---

### Q8. How do you design quota budget across multiple internal services calling one shared Azure OpenAI deployment?

- **WHY:** Without explicit budgeting, one high-volume internal consumer can starve another's quota on a shared deployment — a "noisy neighbor" problem entirely internal to your own organization.
- **HOW:** Either logical quota partitioning enforced at a gateway layer (APIM policies capping each consumer's rate), or separate deployments per consumer/team if isolation matters more than shared-capacity efficiency.
- **WHEN:** Once more than one internal team/service shares a deployment — a single-consumer deployment doesn't need this, but that's rarely the steady state in a growing organization.
- **SCALE:** Gateway-enforced partitioning scales better administratively than manually coordinating "please don't use too much" between teams — as consumer count grows, explicit enforced budgets are what prevent this from becoming a recurring incident.
- **DEPLOY:** Single-region concern primarily — but if consumers are spread across regions calling a single regional deployment, this compounds with the cross-region latency question from 5c/5d.

**Follow-up probe:** "Team A's batch job silently exhausts the shared quota and Team B's real-time customer-facing feature starts throttling — how do you prevent recurrence?" (Enforce per-consumer rate limits at a gateway layer (APIM) so no single consumer can exceed its budgeted share regardless of intent, and consider separating batch/bulk workloads onto a distinct deployment or PTU allocation from latency-sensitive real-time workloads entirely — mixing batch and real-time on one shared quota pool is the root design issue, not just a missing limit.)

---

### Q9. How do you roll out a new model version or prompt change safely in production?

- **WHY:** A new model version or prompt change can silently degrade quality in ways that aren't caught by pre-release testing alone — a full-traffic cutover on day one risks a broad quality regression with no easy rollback signal until damage is done.
- **HOW:** Canary/blue-green rollout — route a small percentage of production traffic to the new version, compare quality/safety metrics (Module 3 Q13's evaluation framework, plus groundedness/Content Safety pass rates) against the existing version before ramping to full traffic.
- **WHEN:** Every non-trivial model version or prompt change in a production system with real user impact — not just for major version bumps.
- **SCALE:** The canary percentage and evaluation window should scale with traffic volume — high-volume systems can validate a canary statistically significant within minutes; low-volume systems need a longer window to gather enough signal before ramping confidently.
- **DEPLOY:** Applies per-region independently — a canary validated in one region doesn't guarantee identical behavior in another if regional model version availability or data characteristics differ (Module 2 Q8's regional availability point).

**Follow-up probe:** "Your canary's automated metrics look fine but a support ticket surfaces a subtle quality regression the metrics didn't catch — what does this tell you about your rollout process?" (Automated metrics don't capture everything human judgment does — this argues for a human-review sample alongside automated canary metrics, not replacing manual spot-checks entirely with automation, especially for subjective quality dimensions the golden-set evaluation didn't anticipate.)

---

### Q10. What does disaster recovery look like within a single region (before considering multi-region)?

- **WHY:** Not every organization needs or can justify multi-region DR — within-region DR (surviving an availability-zone failure, data corruption, or accidental deletion) is a real, lower-cost tier of resilience worth designing explicitly rather than skipping straight to "we need multi-region" or nothing at all.
- **HOW:** Availability-zone-spread compute (Q6), AI Search index backup/restore capability, point-in-time recovery for any transactional data stores in the pipeline, and tested restore procedures (not just backups that have never been restored from).
- **WHEN:** Baseline for any production system — multi-region DR (5c) is an additional, higher-cost tier layered on top of this, not a replacement for it.
- **SCALE:** Backup/restore time scales with data volume — an index backup/restore RTO needs to be measured against actual production data size, not assumed acceptable from a small-scale test.
- **DEPLOY:** This is explicitly the single-region rung of the DR ladder — the natural next question is whether business requirements justify the added cost of the multi-region tier in 5c.

**Follow-up probe:** "You have automated nightly backups of the search index but have never tested a restore — what's the actual risk?" (Untested backups are not a reliable recovery mechanism — corruption, incomplete backups, or restore-procedure gaps only surface during an actual restore attempt; the real DR posture is unknown until a restore has been tested end-to-end, ideally as a scheduled game-day exercise, Q17.)

---

### Q11. How do you size PTU/quota capacity for expected single-region production load?

- **WHY:** Under-provisioning causes throttling during real traffic; over-provisioning wastes budget on unused reserved capacity — sizing needs to be grounded in actual expected load patterns, not guesswork.
- **HOW:** Model expected load (peak concurrent requests, tokens per request, time-of-day/weekly patterns) against PTU throughput-per-unit benchmarks (or PAYG's dynamic-but-shared ceiling), sizing PTU to sustained/predictable baseline load with PAYG overflow for above-baseline spikes (Module 2 Q7's hybrid pattern).
- **WHEN:** Before committing to a PTU reservation term — sizing based on real usage data from a PAYG pilot period is far more reliable than sizing from an upfront estimate alone.
- **SCALE:** Capacity planning needs to account for organic growth over the PTU commitment term, not just current-day load — under-sizing for a 6-12 month commitment based on today's traffic alone is a common, costly mistake.
- **DEPLOY:** This is the single-region capacity question specifically — multi-region capacity planning (5c/5d) compounds this with the added question of how load distributes across regions, not just total load.

**Follow-up probe:** "You sized PTU for current average load and it throttles every Monday morning during a predictable weekly spike — what did the sizing model miss?" (Sizing to average load rather than the actual peak/pattern — PTU should be sized to the sustained pattern including known predictable peaks, with PAYG overflow absorbing anything above that baseline, exactly the pattern flagged in Module 2 Q7.)

---

## 5c. Multi-Region — Active-Passive / Active-Active (7)

### Q12. Active-passive vs active-active multi-region — how do you decide?

- **WHY:** Active-passive (secondary region idle/standby until failover) is simpler and cheaper; active-active (both regions serving live traffic simultaneously) gives better RTO and can improve latency for geographically distributed users, at roughly double the steady-state cost and added consistency complexity.
- **HOW:** Active-passive: secondary region's infrastructure exists but doesn't serve production traffic until a failover event triggers cutover. Active-active: both regions continuously serve real traffic, requiring both to stay in sync (Q13) at all times, not just during a failover.
- **WHEN:** Active-passive when the primary driver is DR/business-continuity risk mitigation and cost matters. Active-active when you also need improved latency for geographically distributed users, or when RTO requirements are so tight that "spin up and cut over" isn't fast enough.
- **SCALE:** Active-active's synchronization overhead grows with write volume and geographic distance between regions — the consistency mechanism (Q16) has to work harder as both traffic and distance increase.
- **DEPLOY:** This is the defining decision of the multi-region tier — everything else in this section (data sync, failover mechanics, RPO/RTO) flows from which of these two patterns you pick.

**Follow-up probe:** "Leadership asks for active-active 'for better resilience' without understanding the cost — how do you frame the actual trade-off?" (Active-active roughly doubles steady-state infrastructure cost and adds real consistency-management complexity — resilience-wise, active-passive already covers most DR scenarios; active-active's added value is primarily latency for distributed users and tighter RTO, not resilience alone — frame the decision around which specific business requirement is actually driving the ask.)

---

### Q13. How do you keep a vector search index in sync across regions?

- **WHY:** Both active-passive (secondary must be reasonably current to be useful on failover) and active-active (both regions must serve consistent results) require some index synchronization strategy — an out-of-sync secondary defeats the purpose of having it.
- **HOW:** Fan out writes from the same ingestion pipeline to both regions' indexes simultaneously (JMA's Push API pattern naturally supports this — write to both regional indexes as part of the same ingestion event), rather than trying to replicate the index itself after the fact.
- **WHEN:** From the moment a secondary region exists — a secondary index that's only populated during a failover event (rather than kept continuously current) reintroduces a large RTO/RPO gap that defeats an "always current" DR goal.
- **SCALE:** Dual/multi-region writes roughly double ingestion-side write cost and add a new failure mode (one region's write succeeds, the other's fails) that needs explicit handling — not free, and needs its own reliability engineering (retry, reconciliation).
- **DEPLOY:** This is the core multi-region data question — the answer differs meaningfully from single-region DR (Q10), where backup/restore of one index was sufficient; here, two live indexes must be kept consistent continuously.

**Follow-up probe:** "A write succeeds in the primary region's index but fails in the secondary — how do you detect and recover from that silently-diverged state?" (Needs an explicit reconciliation mechanism — periodic consistency checks comparing document counts/checksums between regions, and either automatic retry-to-converge or alerting for manual reconciliation; silent write-failure asymmetry is exactly how active-passive secondaries quietly become stale without anyone noticing until a failover reveals it.)

---

### Q14. How does failover traffic cutover actually work mechanically?

- **WHY:** "Failover" isn't automatic just because a secondary region exists — traffic routing needs an explicit mechanism to detect the primary's failure and redirect users to the secondary.
- **HOW:** Azure Front Door or Traffic Manager with health-probe-based routing — health probes continuously check the primary region's endpoint; on failure detection, traffic automatically routes to the secondary (for active-passive) or the routing weight shifts (for active-active already serving traffic).
- **WHEN:** Configure and test this before you need it — failover mechanics validated only in theory (never actually triggered end-to-end) are a common source of DR plans that don't work when actually needed (see Q17).
- **SCALE:** Health-probe interval and failure-threshold tuning trade off false-positive failovers (probe too sensitive) against slow failover detection (probe too lax) — needs tuning against real failure-mode data, not default settings assumed to be correct.
- **DEPLOY:** This is the mechanical glue connecting the multi-region tier to actual user-facing behavior — everything else in 5c (sync, RPO/RTO) is meaningless without a working cutover mechanism to actually route users to the healthy region.

**Follow-up probe:** "Your health probe checks whether the App Service responds, but the actual failure was Azure OpenAI throttling downstream — does failover trigger?" (Likely not — a shallow health check (is the endpoint reachable) misses a downstream dependency failure; health probes need to reflect actual service health including critical downstream dependencies, not just process liveness, or failover won't trigger when it actually should.)

---

### Q15. How do RPO and RTO requirements drive the active-passive vs active-active decision?

- **WHY:** RPO (how much data loss is acceptable — measured in time) and RTO (how long until service is restored) are business requirements that should drive the technical design, not the reverse — picking an architecture first and backfilling an RPO/RTO claim is backwards.
- **HOW:** Near-zero RPO requires continuous synchronous or near-real-time replication (pushes toward active-active or a tightly-synced active-passive); a longer acceptable RPO (e.g., 15 minutes of data loss tolerable) allows for periodic/async replication, which is cheaper and simpler.
- **WHEN:** Establish RPO/RTO requirements with the business stakeholders *before* designing the multi-region architecture — this conversation should happen at the requirements stage, not be inferred after the fact from whatever was built.
- **SCALE:** Tighter RPO/RTO requirements generally cost more to achieve (more synchronous replication, more standby capacity) — this cost/requirement trade-off should be made visible to whoever sets the requirement, not absorbed silently by engineering.
- **DEPLOY:** This is the requirement that determines which pattern (Q12) and which sync mechanism (Q13) are actually necessary — a business that can tolerate 30 minutes of RPO doesn't need the cost/complexity of true active-active synchronous replication.

**Follow-up probe:** "The business says 'zero data loss, instant failover' without understanding the cost — how do you have that conversation?" (Translate the abstract ask into concrete architecture and cost — 'zero data loss, instant failover' means active-active with synchronous cross-region replication, roughly 2x infrastructure cost plus real engineering complexity for consistency guarantees; present that trade-off explicitly and let the business decide if the stated requirement is actually worth that cost, or if a slightly relaxed RPO/RTO is acceptable at much lower cost.)

---

### Q16. What is split-brain risk in an active-active design, and how do you avoid it?

- **WHY:** If both regions accept writes independently without coordination, a network partition between regions can let both sides make conflicting changes to the same logical data — when connectivity is restored, there's no automatically-correct way to reconcile two divergent, individually-valid histories.
- **HOW:** Either avoid true multi-master writes entirely (route all writes to one region even in an active-active *read* topology — "active-active for reads, single-writer for writes" is a common pragmatic pattern), or use conflict-resolution strategies (last-write-wins with careful timestamp handling, or application-level conflict resolution) if true multi-region writes are unavoidable.
- **WHEN:** This risk only exists if both regions can accept independent writes — a read-active/write-single-region pattern sidesteps it entirely, which is why many "active-active" GenAI architectures are actually active-active for serving/reads with a single write path.
- **SCALE:** Split-brain risk and reconciliation complexity grow with write volume and the length of any network partition — a brief partition with low write volume is a minor reconciliation task; a long partition with high write volume can produce a genuinely difficult-to-reconcile divergence.
- **DEPLOY:** This is the sharpest edge of the active-active pattern — it's exactly why Q12's simpler active-passive pattern remains the more common choice unless the business genuinely needs true multi-region write availability.

**Follow-up probe:** "Why might a GenAI RAG pipeline specifically be a good candidate for 'active-active reads, single-writer' rather than true multi-master writes?" (The write path — document ingestion into the search index — is typically a controlled, centralized pipeline (Module 3's ingestion flow) rather than arbitrary user-generated writes from anywhere; centralizing that one write path while serving reads/queries active-active from both regions gets most of active-active's latency/availability benefit without taking on split-brain risk that a pipeline like this doesn't actually need.)

---

### Q17. How do you actually test multi-region failover before you need it for real?

- **WHY:** A failover mechanism that has never been triggered end-to-end is unverified — DR plans that look correct on paper frequently fail in ways only a real (or realistic simulated) failover event reveals (Q14's health-probe gap is a concrete example).
- **HOW:** Scheduled game-day exercises — deliberately trigger a controlled failover (or simulate the primary region's failure) in a non-production or carefully-scoped production window, and validate the full chain: health-probe detection, traffic cutover, secondary region actually serving correctly, data consistency post-failover.
- **WHEN:** Regularly, not just once at initial launch — infrastructure and dependencies change over time, and a failover mechanism validated a year ago may have silently broken due to an unrelated change since.
- **SCALE:** Game-day exercises need to be scoped carefully as production traffic grows — testing failover against full production load has real risk; a well-designed test isolates the blast radius (a percentage of traffic, or a non-critical time window) while still exercising the real mechanism.
- **DEPLOY:** This is the validation step that makes every other answer in 5c actually trustworthy — an unverified failover design is a false sense of security, not real resilience.

**Follow-up probe:** "Leadership is nervous about testing failover against real production traffic — how do you propose testing safely?" (Start with testing in staging/a non-production environment that mirrors the real topology; graduate to a scoped production game day — a small traffic percentage or a low-traffic time window — once staging tests are clean; the goal is incrementally building confidence without betting the whole production system on an untested mechanism's first real trigger being an actual outage.)

---

### Q18. How do you justify the cost of multi-region to leadership against the risk of not having it?

- **WHY:** Multi-region roughly doubles infrastructure cost (more for active-active) — this needs to be justified against a quantified risk, not assumed as automatically worth it.
- **HOW:** Frame the cost against the business cost of an outage — expected outage frequency/duration without multi-region × cost-per-hour-of-downtime (lost revenue, SLA penalties, reputational cost) vs. the multi-region's ongoing incremental cost — a straightforward expected-value comparison leadership can evaluate.
- **WHEN:** This justification conversation should happen explicitly, ideally before building multi-region "because it seems like best practice" — best-practice architecture patterns still need to be justified against actual business risk tolerance and budget.
- **SCALE:** The justification threshold shifts as the business grows — a workload that couldn't justify multi-region cost at low scale/low criticality may clearly justify it once it becomes business-critical or reaches a scale where an outage's cost is much higher.
- **DEPLOY:** This cost-benefit framing is what should determine *whether* you climb to the multi-region rung of the deployment ladder at all, not just *how* once the decision is made.

**Follow-up probe:** "A cost-conscious stakeholder asks 'can we just skip multi-region and accept the risk' — how do you respond as the architect, not just defer to their call?" (Present the actual quantified risk clearly — expected outage cost vs. multi-region's cost — and give a clear recommendation, but ultimately this is a legitimate business risk-tolerance decision, not a purely technical one; the architect's job is making the trade-off visible and well-quantified, not overriding a business decision within acceptable risk tolerance with a purely technical 'best practice' preference.)

---

## 5d. Global Scale-Out (6)

### Q19. How do you route users to the nearest/best-performing region globally?

- **WHY:** Without geo-aware routing, all users hit one region regardless of their physical location, adding avoidable latency for users far from that region — global scale-out's core value proposition is reducing that latency.
- **HOW:** Azure Front Door with latency-based or geo-based routing rules, directing each user's request to the nearest healthy regional deployment rather than a fixed single endpoint.
- **WHEN:** Once you have genuinely global users where latency differences across regions are large enough to matter for the use case (real-time conversational UX cares more about this than an async batch pipeline).
- **SCALE:** Routing infrastructure itself scales trivially (it's a managed service) — what doesn't scale for free is having enough regional deployments to actually route *to*, which is the real cost driver, not the routing layer.
- **DEPLOY:** This is the mechanism that makes "global" actually mean something beyond "multi-region" — multi-region alone (5c) was about resilience; global routing specifically optimizes for user-proximity latency on top of that resilience foundation.

**Follow-up probe:** "A user in Asia is being routed to a US region despite you having deployed an Asia-Pacific region — what's likely misconfigured?" (Front Door routing rules/priority weights not correctly reflecting the new region, or the new region failing health probes silently — verify the routing configuration explicitly includes and correctly prioritizes the new region, and that its health probes are passing.)

---

### Q20. How do you architect around data residency and sovereignty constraints in a global deployment?

- **WHY:** Some jurisdictions (EU, and healthcare/PHI contexts specifically) legally require certain data to stay within defined geographic/regulatory boundaries — a global architecture that freely routes/replicates data anywhere violates this regardless of how well-engineered the routing/replication is.
- **HOW:** Region-pinned data storage/processing for constrained data (a user's data physically stored and processed only in their required region), with global routing (Q19) directing that user's *traffic* to their compliant region specifically, not just the nearest one.
- **WHEN:** Any deployment serving EU users (GDPR), healthcare data (HIPAA/PHI, regional health-data laws), or any jurisdiction with explicit data-localization requirements — this needs to be a design input from the start, not a retrofit.
- **SCALE:** Data-residency constraints can force a departure from pure latency-optimized routing (Q19) — a user's nearest region isn't always their compliant region, which means the routing and replication strategy has to encode compliance rules, not just latency optimization.
- **DEPLOY:** This is the constraint that most directly shapes the "global scale-out" tier's actual design — global doesn't mean "one interchangeable pool of regions," it means "the right regions for the right data, routed correctly."

**Follow-up probe:** "An EU user's nearest low-latency region is actually outside the EU — how do you resolve the conflict between latency-optimal and compliance-required routing?" (Compliance constraints override latency optimization when they conflict — the EU user's traffic and data must route to an EU-compliant region even if it's not the absolute lowest-latency option; this is a hard constraint, not a trade-off to be optimized away.)

---

### Q21. Why can't GenAI inference be "edge cached" the way static assets can via a CDN?

- **WHY:** CDN edge caching works for content that's identical for every requester (a static image, a JS bundle) — LLM inference is dynamic and context-dependent per request (different prompt, different retrieved context, often different user) and generally can't be pre-computed and cached at the edge the same way.
- **HOW:** Static assets (UI, images, documentation) genuinely benefit from CDN/edge caching in a GenAI application's surrounding infrastructure — but the inference call itself has to reach an actual model-serving region, not an edge node.
- **WHEN:** Use CDN/edge caching for everything in the application that *is* static/cacheable (which is often a meaningful portion of a full application's traffic) — don't conflate that with the inference path itself.
- **SCALE:** This distinction matters more as global user count grows — the static-asset portion of the architecture scales beautifully via CDN edge caching (cheap, fast, global by design); the inference portion doesn't get that same free scaling and needs the actual regional-deployment strategy (Q19, Q20) instead.
- **DEPLOY:** A mature global architecture uses CDN/edge for what it's good at (static assets) and regional model-serving deployments + smart routing for what edge caching structurally can't do (dynamic generation) — these are complementary, not competing, layers.

**Follow-up probe:** "A stakeholder asks why you can't just 'put the AI at the edge like the CDN' for lower latency everywhere — how do you explain the limitation clearly?" (Edge caching serves identical pre-computed content instantly from many locations; inference computes a new, context-specific result per request and requires the actual model — running full inference capacity at every edge location isn't the same problem as caching a static file, and isn't what current edge infrastructure is built for; semantic/prompt caching, Q26, is the closer analog but still isn't the same as edge-caching static content.)

---

### Q22. What does the true cost model look like for a global, multi-region deployment?

- **WHY:** Global cost isn't just "multi-region cost × region count" — it includes cross-region data transfer costs, per-region reserved capacity (PTU) if used, redundant storage/index costs per region, and the routing/monitoring infrastructure overhead — easy to underestimate by focusing only on the obvious compute line item.
- **HOW:** Build a full cost model covering: per-region compute/PTU, per-region storage (indexes, embeddings), cross-region data transfer/replication bandwidth, and the monitoring/routing layer (Front Door, cross-region Log Analytics if centralized) — not just multiplying single-region cost by region count.
- **WHEN:** Before committing to a global topology — this cost model should directly inform the Q18-style cost-benefit justification, made concrete with real numbers rather than a rough multiplier.
- **SCALE:** Cross-region data transfer cost specifically scales with sync frequency and data volume (Q13's index-sync writes are a direct cost line here) — a chatty, frequently-synced multi-region design costs more in transfer fees than a coarser-grained sync strategy, a real lever worth modeling explicitly.
- **DEPLOY:** This is the total-cost-of-ownership view across the entire deployment ladder — useful for comparing the actual cost delta between staying single-region, going multi-region for DR (5c), and going fully global (5d) with concrete numbers rather than intuition.

**Follow-up probe:** "Your global cost estimate came in at exactly 3x the single-region cost for a 3-region deployment — what did the estimate likely miss?" (Probably missed cross-region data transfer/replication costs and the routing/monitoring layer overhead — a naive per-region multiplication misses the costs that are specific to the *coordination* between regions, not just the regions' individual infrastructure.)

---

### Q23. How do you handle a new model version being available in some regions before others?

- **WHY:** Microsoft doesn't necessarily roll out new Azure OpenAI model versions to every region simultaneously — a global architecture assuming uniform model availability across all regions will hit a real gap at some point (this connects directly to Module 2 Q8's deprecation-timeline problem, but for *new* version rollout instead of old version retirement).
- **HOW:** Query/track actual regional model availability as an operational input (not an assumption), and design the routing/deployment-version logic to handle a period where different regions legitimately run different model versions.
- **WHEN:** Any time a new model version is being adopted in a global deployment — treat regional availability gaps as the expected case during rollout, not an exception to handle reactively.
- **SCALE:** The coordination complexity of managing version consistency (or intentional inconsistency) grows with region count — more regions means more potential availability-gap combinations to reason about during any version transition.
- **DEPLOY:** This is a direct consequence of operating at the global tier specifically — single-region deployments don't have this problem at all, which is worth naming explicitly as one of global's real operational costs.

**Follow-up probe:** "A new model version is available in your primary region but not yet in a secondary region your users are routed to — what are your options?" (Either delay the version rollout globally until all regions have it (simplest, but delays the improvement everywhere for the sake of one lagging region), or accept a temporary intentional version discrepancy with monitoring to ensure both versions still meet your quality bar, or route affected users' traffic preferentially to a region that does have the new version if latency impact is acceptable — the right choice depends on how consequential the version difference actually is for user experience.)

---

### Q24. How do you design global load balancing that accounts for both latency and AI-specific quality/SLA differences across regions?

- **WHY:** Traditional global load balancing optimizes for latency/availability alone — a GenAI-specific concern is that "healthy and fast" isn't the same as "meeting quality SLA," since a region could be technically up and fast but serving a degraded model version or experiencing groundedness/safety-check failures.
- **HOW:** Extend health-probe/routing criteria beyond simple uptime/latency to include AI-specific signals — error rates on Content Safety/groundedness checks, model version consistency, and observed quality-metric trends per region — feeding into routing decisions, not just infrastructure health.
- **WHEN:** Once quality-monitoring infrastructure (Module 3 Q13's evaluation framework, applied continuously in production, not just pre-release) exists per region to actually produce these signals — a prerequisite, not automatic.
- **SCALE:** This is a more sophisticated routing capability that pays off as region count and criticality grow — for two regions, manual awareness might suffice; for many regions, automated quality-aware routing becomes necessary to catch a regional quality degradation before it affects a meaningful fraction of global traffic.
- **DEPLOY:** This is the most mature/advanced version of the global routing question, synthesizing Q19 (latency routing) with production quality monitoring — a natural "if you had more time" extension in an interview answer.

**Follow-up probe:** "How would you detect a region that's technically healthy (fast, low error rate) but producing systematically lower-quality answers?" (Continuous production sampling against groundedness/quality checks (Module 3 Q6, Q13) broken out per region, with alerting on a per-region quality-metric divergence — this is a signal traditional infrastructure health checks structurally can't see, since the region is 'up' by every conventional metric while still degrading in the dimension that actually matters for a GenAI product.)

---

## 5e. Caching Strategy (5)

### Q25. What does prompt caching actually cache, and when does it help?

- **WHY:** Repeated large static portions of a prompt (a long system prompt, a large set of few-shot examples, or a large retrieved-context block reused across similar queries) are expensive to reprocess on every call — prompt caching avoids reprocessing the identical prefix.
- **HOW:** The provider caches the model's internal processing of a prompt prefix that's identical across calls, so subsequent calls sharing that prefix skip reprocessing it, reducing both cost and latency for the cached portion.
- **WHEN:** High-value when a large, static prompt component (system instructions, a large stable context block) repeats across many calls — low value for prompts that are mostly unique/dynamic content per call.
- **SCALE:** The cost/latency savings scale with both cache-hit rate and the size of the cached prefix — a large static system prompt reused across thousands of daily calls is exactly the profile that benefits most.
- **DEPLOY:** Prompt caching is typically scoped per-deployment/region — a cache warmed in one region's deployment doesn't transfer to another region's deployment, which matters for multi-region designs relying on this optimization (cache needs to be warmed independently per region).

**Follow-up probe:** "Your system prompt is 2000 tokens and called on every request — is prompt caching worth implementing, and how would you validate it?" (Worth investigating — measure actual cache-hit rate and the cost/latency delta in a staging environment before assuming the benefit; the win is real but its magnitude depends on actual traffic patterns and cache-hit consistency, not just prompt size alone.)

---

### Q26. How does semantic caching differ from exact-match response caching?

- **WHY:** Exact-match caching only helps when the identical query repeats verbatim — real user queries rarely repeat exactly, even when they're asking the same underlying question in different words, which is where semantic caching adds value exact-match can't.
- **HOW:** Semantic caching embeds incoming queries and checks similarity against previously-answered queries' embeddings — a sufficiently similar past query's cached answer is served (or used as a strong prior) instead of a fresh generation call, using the same embedding-similarity mechanism RAG uses for retrieval.
- **WHEN:** High-value for domains with a genuinely repetitive underlying question set phrased differently by different users (FAQ-style support, common policy questions) — low value where questions are genuinely unique/context-specific per user.
- **SCALE:** Cache-hit rate (and therefore cost/latency savings) scales with how repetitive the real query distribution actually is — needs to be measured against real traffic, not assumed; a highly diverse query distribution won't benefit much regardless of cache sophistication.
- **DEPLOY:** Semantic cache storage/lookup has the same regional-consistency questions as any vector index (Module 3, Q13 of this module) — a multi-region deployment needs to decide whether the semantic cache is shared/replicated or independently warmed per region.

**Follow-up probe:** "Semantic caching serves a cached answer for a query that's similar but not identical to the original — what's the risk, and how do you bound it?" (Risk is serving a subtly wrong/stale answer for a query that seemed similar but actually needed a different answer — bound it with a conservative similarity threshold, and treat cache hits as high confidence but not certain, potentially with periodic cache-entry revalidation rather than treating cached answers as permanently correct.)

---

### Q27. When would you deploy CAG (Module 3 Q9) specifically as a deployment-tier caching strategy, not just a RAG alternative?

- **WHY:** Revisiting CAG from a deployment-architecture lens rather than a pure RAG-alternative lens: CAG's precomputed KV-cache is itself a caching strategy — the "cache" being the model's processed representation of a static knowledge base, avoiding repeated context-processing cost per query the same way prompt caching (Q25) does, but at a larger scale (whole knowledge base, not just a prompt prefix).
- **HOW:** Deploy CAG when the knowledge base is bounded and stable enough to justify precomputing and maintaining that cache per deployment/region, treating cache warm-up and invalidation (on knowledge-base updates) as an explicit operational process (Module 3 Q9's weekly-formulary example).
- **WHEN:** As a deployment optimization specifically when query volume against a bounded, infrequently-changing knowledge base is high enough that the cache-maintenance overhead is clearly worth the per-query latency/cost savings.
- **SCALE:** Like prompt caching (Q25), CAG's cache must be warmed independently per regional deployment in a multi-region design — this is a real operational cost that scales with region count, on top of the knowledge-base-update-triggered rebuild cost.
- **DEPLOY:** This connects Module 3's RAG-vs-CAG framing directly to this module's deployment ladder — CAG is a caching-architecture decision with the same regional-consistency and invalidation questions as any other cache discussed in this section, not a separate category.

**Follow-up probe:** "How is deciding to use CAG here different from just calling it 'RAG vs CAG' the way Module 3 framed it?" (Same underlying trade-off, different lens — Module 3 asked 'which retrieval paradigm fits the knowledge base'; here the question is 'given CAG, how do you operate it across regions, warm it, and invalidate it as a caching system' — the deployment/operational questions (Q25-Q29's caching section) apply to CAG's cache the same way they apply to prompt or semantic caches.)

---

### Q28. What makes cache invalidation the hardest part of caching in a GenAI pipeline specifically?

- **WHY:** "There are only two hard problems in computer science: cache invalidation and naming things" applies with extra force here — a stale cached answer in a GenAI system isn't just outdated, it's delivered with the same fluent confidence as a fresh, correct answer, making staleness invisible to the end user (the same failure mode as a stale RAG index, Module 3 Q12, but now multiplied across every caching layer in this section).
- **HOW:** Tie cache invalidation to the same source-of-truth change signals driving RAG index freshness (Module 3 Q12) wherever possible — event-driven invalidation when underlying source content changes, rather than relying purely on time-based TTL expiration, which either invalidates too eagerly (losing cache benefit) or too late (serving stale answers).
- **WHEN:** Any cache layer (prompt, semantic, CAG) sitting in front of content that changes — the invalidation strategy needs to be designed alongside the caching strategy from the start, not treated as an afterthought once staleness incidents start occurring.
- **SCALE:** Invalidation complexity compounds across every caching layer active simultaneously (prompt cache + semantic cache + CAG, potentially all present in one architecture) — each needs its own invalidation trigger tied to what it's actually caching, and a change to source content may need to propagate invalidation across multiple cache layers at once.
- **DEPLOY:** In multi-region deployments, invalidation signals need to propagate to every region's independently-warmed cache (Q25-Q27's regional-cache-independence point) — a source update invalidating the cache in one region but not another produces the same silent-divergence problem as Q13's index-sync failure mode, applied to caches instead of the primary index.

**Follow-up probe:** "A policy document updates, the RAG index re-indexes correctly (Module 3 Q12), but a semantic cache still serves the old cached answer for a week — what was missed?" (The index-freshness pipeline and the semantic-cache invalidation pipeline were treated as independent when they should have been triggered by the same source-of-truth change event — any caching layer sitting between the index and the user needs to subscribe to the same freshness signal the index itself does, not be assumed automatically consistent with it.)

---

### Q29. How do you maintain cache consistency across regions in a multi-region deployment?

- **WHY:** Independently-warmed regional caches (the default, per Q25-Q27) can diverge — one region's cache reflects an update, another's doesn't yet, producing the uncomfortable situation where the same question gets a different answer depending on which region happened to serve it.
- **HOW:** Either accept eventual consistency across regional caches with an explicit, bounded propagation-lag SLA for invalidation signals (Q28) reaching every region, or centralize the cache (accepting the cross-region latency cost of a shared cache) if strict consistency matters more than the latency benefit caching was meant to provide in the first place.
- **WHEN:** Explicitly decide which trade-off you're accepting (eventual consistency with bounded lag, vs. centralized/consistent but higher latency) rather than leaving it as an unexamined side effect of "each region just caches independently."
- **SCALE:** The operational burden of guaranteeing tight cross-region cache consistency grows with region count and update frequency of the underlying content — for infrequently-changing content, eventual consistency with a modest lag SLA is usually an acceptable, much simpler trade-off than engineering strict global cache consistency.
- **DEPLOY:** This closes the loop on the caching section within the deployment ladder — every caching decision (Q25-Q28) ultimately needs an explicit multi-region consistency answer once the architecture actually spans regions, not just a single-region design assumption carried forward unexamined.

**Follow-up probe:** "Is perfectly consistent caching across regions actually necessary for most GenAI use cases, or is that over-engineering?" (For most use cases, no — a bounded eventual-consistency window (e.g., invalidation propagates within minutes) is an acceptable trade-off given caching's latency/cost benefit; true strict cross-region consistency is worth the engineering cost only for use cases where even brief inconsistency has real consequence, e.g., safety-critical or compliance-sensitive answers — the right call depends on the specific content's consequence, mirroring Module 4 Q17's tiering-by-consequence approach to memory staleness.)

---

## 5f. Pricing & Cost-Optimization Best Practices (4)

### Q30. PTU vs PAYG as a solution-architecture decision — what's the commitment-risk trade-off?

- **WHY:** Revisiting Module 2 Q7 at the solution-architecture level: PTU requires committing to a reservation term (cost certainty, but locked in even if actual usage comes in lower than forecast) — PAYG has no commitment risk but no throughput/latency guarantee and is subject to shared-pool dynamic throttling.
- **HOW:** Model the break-even point — at what sustained usage level does PTU's fixed cost become cheaper than PAYG's per-token cost — and weigh that against forecast confidence (how sure are you usage will actually reach and sustain that level for the commitment term).
- **WHEN:** PTU once usage forecasting is confident and sustained (not just a hopeful projection), and latency/throughput guarantees are a real business requirement, not a nice-to-have — committing to PTU on optimistic, unvalidated forecasts is a real financial risk.
- **SCALE:** As usage grows and stabilizes, the case for PTU strengthens (more confidently above the break-even point, guarantee value increases with more users depending on consistent latency) — early-stage or highly variable workloads are more often better served by PAYG's flexibility despite the shared-pool risk.
- **DEPLOY:** This decision can be made per-region independently — a mature, high-traffic primary region might justify PTU while a newer, lower-traffic secondary region stays on PAYG until its usage pattern is established, rather than a uniform global commitment.

**Follow-up probe:** "Finance asks you to commit to a 12-month PTU reservation based on a 3-month usage trend — how do you respond as the architect?" (Flag the forecasting risk explicitly — 3 months may not capture seasonality or genuine growth-vs-plateau uncertainty; propose either a shorter initial commitment term if available, a hybrid approach (PTU sized to the confident baseline, PAYG for the uncertain margin), or explicitly quantify the financial downside of over-committing before recommending a 12-month lock-in.)

---

### Q31. How do you measure the true cost per user interaction, not just per API call?

- **WHY:** A single "user interaction" in a RAG/agentic system often triggers multiple underlying calls (query rewriting, embedding, retrieval, reranking, generation, groundedness check, possibly multi-hop/agentic loops) — costing only the final generation call dramatically undercounts true cost, and undermines any pricing/ROI decision built on that undercount.
- **HOW:** Instrument cost attribution across the full pipeline per logical interaction (tag/trace all sub-calls belonging to one user interaction and sum their actual cost), not just the headline generation call.
- **WHEN:** Before making any cost-driven architecture decision (model tiering, PTU sizing, feature scoping) — a decision based on an undercounted cost baseline will be wrong in a predictable direction (underestimating true cost).
- **SCALE:** This undercounting risk compounds specifically in the more sophisticated architectures covered elsewhere in this plan — multi-hop agentic RAG (Module 3 Q10), multi-agent systems (Module 4), and heavy caching/reranking pipelines all add sub-call cost that's easy to lose track of without deliberate instrumentation.
- **DEPLOY:** True per-interaction cost may differ by region if regional pricing, PTU-vs-PAYG mix, or caching hit-rates differ — a global cost model (Q22) needs this per-interaction granularity to be meaningful, not just an aggregate regional total.

**Follow-up probe:** "Leadership approved a feature based on 'cost per generation call' and actual production cost came in 4x higher — what was the estimation gap?" (The estimate almost certainly only counted the final generation call, missing embedding, retrieval, reranking, and any groundedness/safety-check calls that fire on every interaction — the fix going forward is full-pipeline cost instrumentation and using true per-interaction cost for any future estimate, not just the most visible call.)

---

### Q32. Design a model-tiering/routing architecture pattern — cheap model for triage, expensive model for complex cases.

- **WHY:** Not every request needs the most capable (and most expensive) model — routing simple/high-volume requests to a cheaper, faster model and reserving the expensive model for genuinely complex cases can dramatically reduce average cost per interaction without sacrificing quality where it matters (this is the practical production application of Module 1 Q15's distillation/tiering preview).
- **HOW:** A lightweight classification/triage step (itself often a small, cheap model call, or even rule-based) assesses request complexity/category first, then routes to the appropriately-sized model — simple/common queries to a small or distilled model, complex/ambiguous/high-stakes queries escalated to the full-capability model.
- **WHEN:** Once you have production traffic data showing a meaningful share of requests are simple enough that a cheaper model would perform equivalently — validated with real evaluation data (Module 3 Q13's framework, applied per tier), not assumed.
- **SCALE:** The cost savings scale directly with the fraction of traffic that's genuinely simple enough to route to the cheaper tier — the architecture's value is proportional to how skewed your real request distribution is toward the "simple" end, which needs measurement, not assumption.
- **DEPLOY:** This tiering logic can be deployed consistently across regions (same routing rules everywhere) or regionally tuned if request complexity distribution genuinely differs by market — start with consistent global rules and only regionally diverge if data shows it's warranted.

**Follow-up probe:** "How do you prevent the cheap triage model itself from becoming a quality bottleneck — misrouting a complex request to the cheap tier?" (Continuously evaluate triage accuracy against ground truth (did the cheap tier's answer actually meet quality bar, or should it have been escalated) — treat the triage classifier as its own model requiring the same evaluation discipline as the main generative models, not a fire-and-forget rule; bias the triage threshold conservatively toward escalation when uncertain, since a wrongly-escalated simple request costs more but a wrongly-under-routed complex request costs quality.)

---

### Q33. How do you design cost attribution/chargeback for a shared platform serving multiple internal teams or tenants?

- **WHY:** A shared GenAI platform without cost attribution makes it impossible to answer "which team/tenant is driving cost" — this blocks informed budget decisions, makes the noisy-neighbor problem (Module 5b Q8) financially invisible, and removes any incentive for consuming teams to be cost-conscious.
- **HOW:** Tag every request at the point of entry (API key, JWT claim, or gateway-enforced identifier) with the consuming team/tenant, and aggregate true per-interaction cost (Q31) by that tag for chargeback/showback reporting.
- **WHEN:** From the point a platform serves more than one internal consumer — retrofitting attribution after the fact requires reconstructing historical cost data that may not be recoverable if tagging wasn't in place from the start.
- **SCALE:** Attribution granularity needs matter more as consumer count grows — coarse team-level attribution may suffice for a handful of internal teams; a platform serving many external tenants likely needs attribution down to the individual tenant/customer level for accurate billing or cost-based product pricing.
- **DEPLOY:** Attribution tagging needs to be consistent across regions in a global deployment — a consumer's cost shouldn't become invisible just because their traffic happened to route to a different region's infrastructure.

**Follow-up probe:** "A shared platform's total cost is known but no one can say which team is driving 60% of it — what's the remediation path, and can historical cost be recovered?" (Implement attribution tagging going forward immediately — historical cost is likely not recoverable at the granularity needed unless coarse proxies exist (e.g., request logs with enough metadata to approximate attribution retroactively); the real fix is making attribution a non-negotiable requirement for any new consumer onboarding to the platform from this point forward.)

---

## 5g. Multi-Tenant & Cost/Security Trade-offs (2)

### Q34. Shared capacity vs dedicated capacity per tenant — how do you decide the isolation model?

- **WHY:** Shared capacity (multiple tenants on the same underlying deployment/index) is more cost-efficient but creates noisy-neighbor risk (Module 5b Q8) and requires rigorous access-control enforcement (Module 3 Q15); dedicated capacity per tenant eliminates both risks but at meaningfully higher cost, scaling linearly with tenant count rather than sharing economies of scale.
- **HOW:** Shared capacity with strong logical isolation (metadata-filtered retrieval, per-tenant rate limiting/quota enforcement, Module 3 Q15's access-control pattern) as the default; dedicated capacity reserved for tenants with contractual/regulatory requirements that shared infrastructure genuinely can't satisfy (specific compliance certifications, guaranteed performance SLAs incompatible with shared-pool dynamics).
- **WHEN:** Default to shared with strong isolation controls for cost efficiency; escalate to dedicated only for tenants whose requirements explicitly demand it — not as a default premium tier offered without a genuine technical/compliance driver.
- **SCALE:** Shared capacity's cost-efficiency advantage grows with tenant count (better utilization of pooled capacity); dedicated capacity's cost grows linearly per tenant regardless of overall platform scale — the crossover point where dedicated becomes justified is usually a specific tenant's requirement, not a general scale threshold.
- **DEPLOY:** In a multi-region global platform, this decision compounds with data-residency requirements (Q20) — a tenant requiring both dedicated capacity and specific regional data residency needs dedicated infrastructure in their specific required region(s), not just dedicated capacity anywhere.

**Follow-up probe:** "A tenant demands dedicated capacity 'for better performance' without a specific compliance driver — how do you evaluate the request?" (Investigate whether the actual performance concern is a real noisy-neighbor symptom (fixable with better shared-tenancy rate limiting/QoS controls, cheaper than dedicating) or a genuine sustained-load requirement that shared capacity structurally can't meet — don't default to the more expensive dedicated option without first confirming cheaper isolation improvements within the shared model wouldn't resolve it.)

---

### Q35. Synthesizing the whole module — where does the security boundary sit at each deployment tier, and how does it change as you climb the ladder?

- **WHY:** This is the module's closing synthesis question — security boundaries aren't a single fixed line, they shift and multiply as the architecture climbs from local to global, and an interviewer asking this wants to see you hold the whole deployment ladder coherently, the same way Module 3 Q18 closed out the RAG module.
- **HOW:** **Local/dev** — boundary is environment isolation (Q2) keeping dev entirely separate from anything real. **Single-region production** — boundary is RBAC/Managed Identity (Module 2 Q2-Q3) and network isolation (Module 2 Q4) around the resource. **Multi-region** — boundary extends to securing replication/sync traffic between regions (Q13) and ensuring failover doesn't create a temporary security gap (a secondary region's controls must be as rigorous as primary's, not an afterthought). **Global** — boundary now includes data-residency enforcement (Q20) as a compliance-driven boundary, not just a technical one, plus multi-tenant isolation (Q34) if serving multiple tenants across that global footprint.
- **WHEN:** This synthesis should be rehearsed as a coherent narrative, not just a list — an interviewer probing "how does security change as you scale" is testing whether you see these as one evolving system, not unrelated checklist items per tier.
- **SCALE:** Every rung up the ladder adds a security surface the previous rung didn't have (replication traffic, cross-region identity federation, data-residency enforcement, multi-tenant boundaries) — security complexity compounds with deployment complexity, not linearly but genuinely additively at each tier.
- **DEPLOY:** This question *is* the deployment ladder, viewed through the security lens specifically — the honest answer is that there's no single "the security boundary," there's a boundary appropriate to each tier that must all hold simultaneously in a mature global deployment.

**Follow-up probe:** "You're asked to design security for a healthcare client's global GenAI platform from scratch — walk the security boundary at each tier in under 2 minutes." (Structured answer hitting all four tiers concisely: dev isolation with no real PHI ever touching non-prod → Managed Identity/RBAC/Private Endpoints as the single-region baseline given Module 2 Q4's healthcare-relevance note → encrypted, access-controlled replication traffic plus consistent controls on the secondary region for multi-region → data-residency-enforced routing and storage, plus tenant isolation if applicable, for the global tier — delivered as one coherent narrative, not four disconnected answers, which is exactly what this question is testing for.)

---

*Module 5 of 6 — GenAI Architect Interview Prep. Next: Module 6 — Responsible AI, LLMOps & Governance.*
