# Section B — Azure AI Foundry & Azure OpenAI (15 Q, detailed)

**Created:** 2026-08-09
**Source list:** `_Archive/StaleTrackers/Interview_Prep_AI_Engineer_Complete.md` §B (Q16–Q30)
**Format:** per `00_PLAN_InterviewQA_2026-08-08.md` §5, extended to
**WHAT · WHY · WHEN · HOW** + your example + the trade-off.
**Sibling file:** `QA_Detail_A_RAG_Architecture_15Q.md`
**Companion revision layer:** `InterviewBank/02_Azure_AI_Platform.md` (18 Q, terse)
**Companion self-test:** `PerChapter/QA_L12_AzureOpenAI_Services.md` (35 Q) ·
`PerChapter/QA_L17_AzureAIFoundry.md` (30 Q)

---

## ⚠️ Read this before you drill

**Three things in this section need a decision from you before you speak them.**

1. **Two anchors describe candidates, not shipped systems.** Q24's anchor is *"JMA invoice
   extraction candidate"* and Q28's is *"JMA nightly ticket classification fit."* Both are
   things that *would* fit — not things you built. Say "that's where I'd use it, and here's
   why it fits" and you sound like an architect. Say "we use it" and one follow-up about
   throughput or schema versioning exposes it. **Phrasing is provided in each answer below.**

2. **Azure's product naming has churned.** Azure OpenAI Studio → Azure AI Studio → Azure AI
   Foundry, with further Microsoft rebranding since. Say the name you see in the portal
   *today* and add "formerly Azure AI Studio" — that reads as current, not out of date.
   Verify before any interview; this is a cheap way to sound plugged in or stale.

3. **`text-embedding-ada-002` is a liability on the resume.** The Interview Bible names it as
   the JM Family embedding model. It is a 2022 model on Microsoft's retirement track, and
   `text-embedding-3-large`/`-small` beat it on quality *and* price. Expect "why are you
   still on ada-002?" — the good answer is a migration story with a re-embedding cost
   estimate, not a shrug. **Covered in Q30.**

**Pricing, quota floors, and PTU minimums move constantly.** Where this file gives numbers it
says so. Never quote a specific price from memory in an interview — say the shape of the
ratio and that you'd check the current sheet.

---

## Q16. Azure OpenAI vs OpenAI direct — why does the enterprise pick Azure?

**Difficulty:** Easy · **Key terms:** data residency, Managed Identity, Private Link

**What they're testing:** whether you can articulate a procurement decision, not just a
technical preference. This is a question about compliance, and the interviewer usually knows
the answer already.

**60-second spoken answer:**
> It's rarely a model-quality decision — it's the same models. It's a control-plane decision,
> and it comes down to four things. Data residency: I choose the region the data is processed
> in, which matters the moment there's a regulator involved. Identity: it's Entra ID and
> Managed Identity with RBAC, so it lives inside the same access model as everything else we
> run, with no API key to leak. Network: Private Link means the traffic never touches the
> public internet. And contractually, prompts and completions aren't used to train the models.
> At JM Family those four things are exactly why we run Azure OpenAI — for finance and
> insurance documents, the OpenAI-direct posture wouldn't have cleared review.

### WHAT the actual differences are

| | Azure OpenAI | OpenAI direct |
|---|---|---|
| Identity | Entra ID, Managed Identity, RBAC | API key |
| Network | Private Endpoint, VNet, public access disabled | Public endpoint over TLS |
| Residency | You pick the region / data zone | Provider-controlled |
| Compliance | Inherits Azure's estate — SOC 2, ISO 27001, HIPAA BAA, FedRAMP via Azure Gov, PCI | Enterprise agreements available, smaller certified surface |
| Keys at rest | Customer-managed keys (CMK) supported | Provider-managed |
| Billing | On the existing Azure EA/MCA, counts toward commitment | Separate vendor, separate procurement |
| New models | Usually lands later | First |

### WHY these specific four matter to a regulated buyer
Each maps to a control an auditor will actually ask for:

- **Residency** answers "where is our data processed?" — a data-protection question with a
  legal answer, not a preference.
- **Managed Identity** removes the shared secret entirely. There's no key to rotate, no key
  in a config file, and every call is attributable to a principal in the Entra sign-in log.
- **Private Link** answers "can this data traverse the internet?" with "it cannot" rather
  than "it's encrypted in transit." Those are different assurances.
- **Not used for training** answers the question every legal team asks first.

The point to make: none of these are about the model. Buying Azure OpenAI is buying the
governance wrapper.

### The nuance most candidates miss — abuse monitoring
Azure OpenAI logs prompts and completions for abuse monitoring, retained for a bounded window
and reviewable by authorised Microsoft personnel on a flagged trigger. For sensitive
workloads you apply for **modified abuse monitoring / data-logging exemption**, which turns
that off.

Raising this unprompted is a strong signal. It says you've actually been through a
compliance review rather than read a comparison page — because "Microsoft doesn't see your
data" is the answer everyone gives, and it isn't quite true by default.

### WHEN OpenAI direct is the right call
Be willing to say this — a candidate who claims Azure always wins sounds like a salesperson:

- **You need a model on day one.** New models land on OpenAI first, sometimes by months.
- **You're a startup with no compliance surface** and the Azure setup cost is real overhead.
- **You need a capability Azure hasn't surfaced** — a preview API, a feature parity gap.
- **You're prototyping**, and the governance doesn't matter until it ships.

### HOW the enterprise setup actually looks
1. Azure OpenAI resource in the compliant region, `publicNetworkAccess: Disabled`.
2. Private Endpoint into the app VNet, Private DNS zone linked (see Q27).
3. `disableLocalAuth: true` so keys cannot be used at all (see Q26).
4. App identity granted **Cognitive Services OpenAI User**.
5. Diagnostic settings → Log Analytics for token usage, latency, and 429 rate.
6. Content filter policy configured per deployment; abuse-monitoring exemption filed if the
   data class requires it.

### Your example
JM Family runs Azure OpenAI for exactly these reasons. The corpus is finance and insurance
documents under a compliance regime where the network and residency posture is not
negotiable — the Private Link and Managed Identity setup is what made the platform
approvable, not a model preference.

### The trade-off
You pay for it in agility. Model availability lags, quota is regional and has to be requested
and managed, and the setup surface — networking, DNS, RBAC, content filter policy — is
genuinely more work than an API key. On a greenfield prototype with no compliance
requirement, that overhead buys you nothing.

**Follow-up probes:**
- *"Which compliance certifications?"* → It inherits the Azure estate: SOC 2, ISO 27001, HIPAA BAA where in scope, PCI DSS, FedRAMP High in Azure Government. The right answer is "it inherits Azure's" plus naming the one your regulator cares about.
- *"Is your data really never used for training?"* → Contractually yes, but note abuse-monitoring logging exists by default and requires an exemption to disable.
- *"When would you pick OpenAI direct?"* → Day-one model access, prototyping, or a feature Azure hasn't shipped.

**Red flag:** "Azure is more secure." It's the same models on the same weights — say *which
controls* you get, or the answer is marketing.

---

## Q17. What is a deployment in Azure OpenAI?

**Difficulty:** Easy · **Key terms:** deployment, TPM, model version

**What they're testing:** basic operational literacy. Getting this wrong signals you've only
used the playground.

**60-second spoken answer:**
> A deployment is your named instance of a model — it binds a specific model, a specific
> version, a deployment type, and a throughput quota to a name inside your resource. You call
> the deployment name in the URL, not the model name. That indirection is the useful part:
> the deployment name is the stable contract your application code depends on, so you can
> move the underlying model version behind it without touching a single caller. And the quota
> is attached to the deployment, so it's also your unit of throughput isolation. At JM Family
> we keep separate dev and prod deployments so a load test can't eat production's TPM.

### WHAT a deployment binds together

| Property | What it means |
|---|---|
| Deployment name | The string in your URL — the stable contract |
| Model | e.g. `gpt-4o`, `text-embedding-3-large` |
| Model version | Pinned (`2024-11-20`) or auto-update to default |
| Deployment type | Standard (regional), Global Standard, Data Zone, Provisioned (PTU), Batch |
| Capacity | TPM allocation drawn from your regional quota pool |
| Content filter policy | Which filter config applies to this deployment |

You call `POST {endpoint}/openai/deployments/{deployment-name}/chat/completions?api-version=...`.
The model name appears nowhere in the request path.

### WHY the indirection exists
It decouples the application from the model. Your code holds a deployment name in config;
the model version behind it is an infrastructure concern. That is what makes a version
migration a deployment change and a canary rather than a code release across every service
that calls the model. Q30 is built entirely on this property.

### WHY you pin the version — and when you don't
An auto-updating deployment silently moves you to a new model version. Model versions change
behaviour: output formatting drifts, refusal boundaries move, tool-call reliability shifts,
and few-shot prompts tuned to the old version regress. You find out from an eval failure or
a user complaint, and you can't correlate it to a deploy because there wasn't one.

**Pin in production.** Move versions deliberately: stand up a second deployment on the new
version, run the golden set against it, canary a traffic slice, then cut over.
**Auto-update is defensible in dev**, where you *want* early warning that the next version
breaks something.

The catch: pinning is not permanent. At retirement, a pinned deployment gets upgraded for you
whether or not you're ready. Pinning buys scheduling control, not immunity (Q30).

### WHEN to run multiple deployments of the same model
This is the senior version of the answer — deployments are a design tool, not just a config:

- **Environment isolation** — dev/test/prod, so nothing shares a quota pool.
- **Throughput isolation** — a batch job and the interactive path on separate deployments so
  the batch job can't starve users.
- **Quota aggregation** — several deployments across regions behind a router to exceed a
  single region's quota ceiling (Q20).
- **Version migration** — old and new side by side during a canary.
- **Differentiated content filters** — a stricter policy on the customer-facing deployment
  than on an internal tool.

### Your example
JM Family runs separate dev and prod deployments. The immediate driver was throughput
isolation — a load test against a shared deployment throttles production, because quota lives
on the deployment and nothing else protects it.

### The trade-off
Every deployment consumes quota from the regional pool, so proliferation fragments capacity:
five deployments at 10K TPM each is worse under a spike than one at 50K, because unused
headroom can't be shared. Deployments also multiply the config surface — each needs its own
filter policy, monitoring and version-tracking. Split for a reason, not by default.

**Follow-up probes:**
- *"Why pin the version?"* → Versions change behaviour; an unpinned deployment gives you a silent regression with no deploy to correlate it to.
- *"What does a deployment cost when idle?"* → Standard/PAYG: nothing, you pay per token. Provisioned: the full hourly rate regardless of traffic.
- *"How do you migrate versions with no downtime?"* → Second deployment on the new version, golden-set eval, canary a traffic slice, cut over, retire the old.

**Red flag:** using "deployment" and "model" interchangeably.

---

## Q18. What is Azure AI Foundry?

**Difficulty:** Easy · **Key terms:** model catalog, Prompt Flow, evaluation, Hub/Project

**What they're testing:** whether you know the platform beyond the chat playground — and
whether your terminology is current.

**60-second spoken answer:**
> Foundry is Microsoft's unified platform for building GenAI applications — it's what
> replaced Azure OpenAI Studio and then Azure AI Studio. The mental model is a Hub, which is
> the shared team-level resource holding connections, compute and security, and Projects
> underneath it, which are the individual workstreams. Inside that you get the model catalog
> — not just OpenAI but Meta, Mistral, Cohere, Phi and Hugging Face models — plus deployments,
> Prompt Flow for orchestration, an evaluation framework, Content Safety, the Agent Service,
> and tracing. The way I use it is prototype in Foundry, productionize in code — Foundry gets
> you to a working evaluated prototype fast, and Semantic Kernel or the SDK is where the
> production service actually lives.

### WHAT the components are, and when you touch each

| Component | What it is | When you touch it |
|---|---|---|
| **Hub** | Team-level parent: connections, storage, Key Vault, compute, network | Once, at setup — this is the governance boundary |
| **Project** | A workstream under a Hub, with its own assets | Per initiative |
| **Model catalog** | OpenAI + Meta, Mistral, Cohere, Phi, Hugging Face | Model selection and comparison |
| **Deployments** | Serverless API or managed compute endpoints | Every model you serve |
| **Prompt Flow** | Visual/YAML orchestration DAG with built-in tracing | Prototyping chains and RAG flows |
| **Evaluations** | Built-in evaluators — groundedness, relevance, coherence, fluency, similarity, safety | Before every release, and in CI |
| **Content Safety** | Hate/violence/sexual/self-harm filters, jailbreak and prompt-shield detection, groundedness detection | Configured per deployment |
| **Agent Service** | Managed agent runtime — threads, tools, state | Agentic workloads |
| **Connections** | Credential-bearing links to AI Search, storage, other services | Setup, then rarely |
| **Tracing** | Per-request spans across the flow | Debugging quality and latency |
| **Compute** | Managed instances/clusters for fine-tuning and hosted models | Fine-tuning, open-model hosting |

### WHY the Hub/Project split exists
It separates the things that need central governance from the things teams iterate on. The
Hub owns the network configuration, the customer-managed keys, the storage account, the
connections and who can use them. Projects inherit that and can't route around it. So a
platform team stands up one compliant Hub, and five product teams get Projects inside it
without each re-litigating the security review. Get this split right and Foundry scales
across an org; get it wrong and you have five Hubs and five different security postures.

### Foundry vs Semantic Kernel — the follow-up
They aren't alternatives, and saying so cleanly is the whole answer:

- **Foundry is a platform** — a hosting, catalog, evaluation and governance surface. It runs
  outside your process.
- **Semantic Kernel is an SDK** — an in-process orchestration library that runs *inside* your
  service, composing plugins, planners and memory in C# or Python.

You deploy a model on Foundry and call it from a service built with Semantic Kernel. The real
decision is Prompt Flow vs SK for orchestration: Prompt Flow is a managed DAG that's superb
for iteration, evaluation and tracing, but it puts your orchestration in a hosted flow. SK is
plain code — testable, debuggable, deployable in your own container, and versioned in your
own repo. Hence prototype in one, productionize in the other.

### WHEN to use Foundry versus going straight to the SDK
| Use Foundry's surface | Go straight to code |
|---|---|
| Comparing models across vendors | Model already chosen |
| Building an evaluation baseline | Eval suite already in CI |
| Non-engineers need to see and tune the flow | Engineering-owned service |
| Fine-tuning or hosting open models | Inference-only against a deployment |
| Agent runtime you don't want to operate | Existing orchestration in-process |

### Your example
At JM Family the pattern is prototype in Foundry, productionize in Semantic Kernel. Foundry
gets a flow evaluated and demonstrable quickly — the model catalog and built-in evaluators
compress the "which model, is it good enough" loop. Once the shape is settled, the production
path is code in our own repo and our own release pipeline, calling Foundry deployments.

### The trade-off
Foundry is a fast-moving surface with real churn — the product has been renamed twice, the
Hub/Project model has shifted, and features move between preview and GA. Building a
production system deep inside the managed surface means absorbing that churn. It's also
another RBAC and networking surface to secure, and Prompt Flow's convenience comes at the
cost of orchestration you can't unit-test the way you can test code.

**Follow-up probes:**
- *"Foundry vs Semantic Kernel?"* → Platform vs in-process SDK. Not competitors — you call Foundry deployments from an SK service.
- *"Hub vs Project?"* → Hub is the governance boundary — network, keys, connections. Projects are workstreams that inherit it.
- *"Would you run Prompt Flow in production?"* → For a stable business-owned flow, defensible. For a core service, I want orchestration as testable code in my own repo.

**Red flag:** calling it "Azure OpenAI Studio." The name has moved twice since; using the old
one dates you instantly.

---

## Q19. PTU vs pay-as-you-go — how do you choose?

**Difficulty:** Medium · **Key terms:** provisioned throughput, TPM, break-even, utilisation

**What they're testing:** capacity planning and whether you can defend a committed spend to a
finance stakeholder.

**60-second spoken answer:**
> Pay-as-you-go bills per token on shared capacity — zero commitment, but your latency is
> whatever the shared pool gives you that minute, and under load you get throttled. Provisioned
> throughput reserves dedicated capacity: you pay a fixed hourly rate whether you use it or
> not, and you get deterministic latency and a guaranteed throughput floor. So it's a
> utilisation question. If your load is predictable and sustained, PTU is cheaper per token
> and the latency is stable. If it's spiky or low, PTU is money burning on idle capacity. The
> shape that usually wins in production is PTU sized to the steady-state baseline with
> pay-as-you-go handling the overflow — you buy predictability for the load you can predict
> and elasticity for the load you can't. That's how JM Family is sized.

### WHAT you're actually buying

| | Pay-as-you-go (Standard) | Provisioned (PTU) |
|---|---|---|
| Billing | Per token consumed | Per PTU per hour, reserved |
| Capacity | Shared pool | Dedicated to you |
| Latency | Variable, no guarantee | Deterministic, predictable TTFT/throughput |
| Under load | 429 against your TPM quota | Runs at your provisioned rate; excess 429s or spills over |
| Idle cost | Zero | Full rate |
| Commitment | None | Hourly, or discounted monthly/yearly reservation |

### WHY PTU produces better latency
It isn't a faster model — it's the absence of a queue. On shared capacity your request
competes with every other tenant in that region, so time-to-first-token varies with someone
else's traffic. Provisioned capacity is reserved to you, so throughput and TTFT hold steady
under your own load. For an interactive product with a latency SLO, that predictability is
often worth more than the token-price arbitrage.

### HOW you do the break-even, concretely
The calculation an interviewer wants to hear:

1. **Measure the steady state.** Tokens per minute at p50 and p95 over a representative week,
   input and output separately.
2. **Convert to PTU.** Throughput per PTU varies by model and deployment type — use
   Microsoft's current capacity calculator, don't estimate from memory.
3. **Price both.** PTU: units × hourly rate × 730 hours. PAYG: monthly tokens × per-token
   rate, input and output priced separately.
4. **Find the utilisation crossover.** PTU wins above a sustained utilisation threshold —
   commonly cited around two-thirds, but compute it for your own numbers rather than quoting
   a rule of thumb.
5. **Apply reservations.** A monthly or yearly PTU reservation discounts substantially and
   moves the crossover down. It also deepens the commitment.
6. **Size to baseline, not peak.** This is the part people get wrong — see below.

### WHY you size to baseline and not peak
Sizing PTU to peak means paying for peak capacity 24 hours a day to serve a spike that lasts
two. Size to the steady state, and let overflow go to a pay-as-you-go deployment — either via
Azure's spillover configuration or your own router that catches the 429 and retries against
the PAYG deployment. You get predictable latency on the bulk of traffic and elastic capacity
on the tail, and you only pay peak prices during the peak.

The Monday-morning spike question is exactly this: PTU carries the baseline, overflow lands
on PAYG, and if the spike is *predictable* you can also pre-warm by scheduling additional
capacity.

### WHEN each is right
| Choose PTU | Choose PAYG |
|---|---|
| Sustained, predictable volume | Spiky, seasonal, or low volume |
| Latency SLO to meet | Best-effort latency acceptable |
| Chronic 429s despite quota increases | Quota is comfortable |
| Cost predictability required by finance | Variable cost preferred |
| Prototype/dev | Always PAYG — never provision dev |

### Your example
JM Family sizes to baseline plus overflow. The workload has a clear steady state — business
users across the working day — with predictable peaks, which is exactly the profile where
provisioned baseline plus elastic overflow beats either option alone. This sits inside the
same cost-engineering programme as the model-tier routing (Q29) that produced the ~30% /
~$150K inference cost reduction.

### The trade-off
PTU is committed spend, and idle PTU is pure waste — a reservation you outgrow or undershoot
is money you can't recover. It also reduces flexibility: PTU is allocated per model and
region, so a model migration or a region change means re-planning capacity, and a reserved
commitment can outlive the model you bought it for. Under-provisioning is worse than PAYG,
because you now pay a fixed cost *and* get throttled.

**Follow-up probes:**
- *"Monday-morning spike is throttling us — what do you do?"* → PTU for baseline, spillover to PAYG for the peak. Don't size PTU to peak; you'd pay for it around the clock.
- *"What's the break-even?"* → A sustained-utilisation crossover you compute from your own p50/p95 token rate against current PTU and token pricing. Quoting a fixed percentage from memory is a bluff.
- *"Does PTU make the model faster?"* → No — it removes queueing. Same model, predictable throughput.

**Red flag:** "PTU is cheaper" with no utilisation figure. It's cheaper above a crossover and
much more expensive below it.

---

## Q20. How do you handle 429 throttling?

**Difficulty:** Medium · **Key terms:** 429, Retry-After, TPM quota, backoff with jitter

**What they're testing:** whether you understand that this is a *quota* problem, not a
*capacity* problem — the "add more instances" trap is deliberately baited in the follow-up.

**60-second spoken answer:**
> First, understand what the 429 means: quota in Azure OpenAI is tokens-per-minute and
> requests-per-minute attached to the deployment, drawn from a regional pool. It is not about
> how much compute your app has. So the immediate handling is exponential backoff with jitter,
> honouring the Retry-After header the service sends — and critically, adding application
> instances makes it worse, because all those instances draw on the same deployment quota and
> you've just increased the arrival rate against a fixed ceiling. The real fixes are upstream:
> request a quota increase, spread across multiple deployments and regions behind a router,
> pace non-interactive work through a queue, move batch work to the Batch API, or buy
> provisioned throughput. At JM Family we queue the Document Intelligence and embedding calls
> specifically so ingestion can't throttle the interactive path.

### WHAT a 429 actually is
Azure OpenAI enforces two limits per deployment, both derived from the capacity you assigned:

- **TPM** — tokens per minute, counting prompt tokens plus a projection of completion tokens.
- **RPM** — requests per minute, derived from TPM.

Both are evaluated over short sliding windows, so a burst can trip the limit even when your
per-minute average is well under. The response carries `Retry-After` telling you how long to
wait. On a provisioned deployment you get a 429 when you exceed your provisioned rate rather
than a quota figure — same status, different cause.

### WHY adding instances makes it worse
This is the trap, and the answer needs to be crisp. Scaling out increases the *arrival rate*
against a ceiling that didn't move. Ten pods against a 10K TPM deployment have exactly 10K
TPM between them — you've just made them collide more often, and you've added retry storms on
top. Horizontal scaling solves compute-bound problems. This is a *quota-bound* problem, and
the two look identical on a dashboard until you know where the limit lives.

If a team lead says "add instances", the reply is: the constraint is on the deployment, not
the pods; here's the TPM figure and here's our token rate; scaling the app increases pressure
on the same ceiling. Then offer the real options.

### HOW to handle it — client side (necessary, not sufficient)
1. **Honour `Retry-After`.** Don't invent your own interval when the service told you one.
2. **Exponential backoff with jitter.** Without jitter, every throttled caller retries in
   lockstep and you build a thundering herd.
3. **Cap retries and fail gracefully.** Infinite retry converts throttling into an outage.
4. **Circuit-break.** After sustained 429s, stop trying for a cooldown rather than hammering.
5. **Token-aware client-side throttling.** Estimate tokens before sending and self-limit —
   cheaper than discovering the ceiling by hitting it.

### HOW to fix it — architecture side (the actual answer)
| Fix | When it's right |
|---|---|
| **Quota increase** | Always try first. Free, often granted, sometimes the whole answer. |
| **Multiple deployments + router** | You've hit the regional ceiling. Round-robin or least-loaded across deployments/regions. Check residency before crossing regions. |
| **APIM as a smart gateway** | Multi-backend load balancing, circuit breaking, token metering and per-consumer quota in one place. The standard enterprise pattern — and there's an APIM transcript already in this repo. |
| **Queue + paced workers** | Ingestion, embedding, batch classification. Decouple arrival rate from processing rate. |
| **Batch API** | Non-interactive work, ~50% cheaper, separate quota pool (Q28). |
| **PTU** | Chronic throttling with predictable load (Q19). |
| **Semantic caching** | Cuts calls at the source — repeated questions never reach the model. |
| **Model tiering** | Route simple queries to a cheaper model on a different deployment with its own quota (Q29). |

The last two are worth naming because they reduce demand rather than chase supply — that's
the cost-conscious framing a hiring manager notices.

### WHEN to separate interactive from batch traffic
Always, once you have both. They have opposite requirements: interactive needs low latency
and tolerates low throughput; batch needs high throughput and tolerates latency. Sharing a
deployment means an ingestion run degrades the user-facing path at exactly the moment it's
least acceptable. Separate deployments — or better, Batch API for the batch half.

### Your example
JM Family queues Document Intelligence and embedding calls to avoid throttling. Ingestion at
500K+ documents generates enormous burst pressure; pacing it through a queue keeps it from
competing with interactive retrieval for the same quota. That's the same separation-of-paths
principle behind the event-driven-with-controlled-concurrency ingestion design.

### The trade-off
Every mitigation costs something. Backoff adds tail latency. Multi-region routing adds
residency and consistency complexity and possibly a compliance conversation. Queuing breaks
the synchronous contract, so the UX becomes "we'll notify you" instead of an answer.
PTU is committed spend. There is no free way to exceed a quota ceiling — you either raise it,
spread it, defer the work, or need less of it.

**Follow-up probes:**
- *"Your team lead says just add more instances. Respond."* → Quota is on the deployment, not the pods. More instances raise the arrival rate against the same ceiling and add retry storms. Show the TPM figure against the measured token rate.
- *"You're already at max quota in your region."* → Multiple deployments across regions behind APIM, subject to a residency check. Then Batch/caching to cut demand.
- *"Why jitter?"* → Without it, throttled callers retry in lockstep and re-throttle each other.

**Red flag:** stopping at "retry with exponential backoff." That's table stakes — the
question is what you change so you stop hitting it.

---

## Q21. What is "On Your Data"?

**Difficulty:** Medium · **Key terms:** managed RAG, `data_sources`, strictness, citations

**What they're testing:** whether you know the managed shortcut exists — and whether you can
say precisely why you didn't use it.

**60-second spoken answer:**
> It's Azure OpenAI's built-in managed RAG. You add a `data_sources` block to the chat
> completion call pointing at an Azure AI Search index, and Azure does the retrieval, prompt
> assembly and citation formatting for you. One API call instead of a pipeline. It's genuinely
> good for getting a grounded chatbot over an existing index in a day, and the citations come
> back structured. What you give up is control of the parts that determine RAG quality:
> chunking, hybrid tuning, custom re-ranking, query rewriting, and the ability to evaluate
> retrieval and generation as separate stages. At JM Family we run custom RAG precisely
> because chunking control is what got retrieval accuracy where it needed to be — table-aware
> chunking on dealer forms isn't something a managed pipeline will do for you.

### WHAT it does
You pass a `data_sources` array on the chat completions request naming a supported store —
Azure AI Search, Cosmos DB, Elasticsearch, or blob-backed indexes. Azure then:
1. Takes the user message and retrieves from the store.
2. Assembles a grounded prompt from the results.
3. Calls the model.
4. Returns the answer plus a structured citations payload.

Tunable knobs include `strictness` (1–5, how relevant a result must be to be used),
`topNDocuments`, `inScope` (refuse when the data doesn't cover it), and role/prompt overrides.

### WHY it exists
It collapses the entire query pipeline into one call. For teams without RAG expertise — or
teams with an existing AI Search index and a two-week deadline — it removes the pipeline,
the orchestration code, and the citation plumbing. The `inScope` and `strictness` settings
give you a grounding dial without writing a grounding check.

### WHY you'd move off it
Map it against the failure points from Section A and the limits become concrete:

| RAG lever | On Your Data | Custom |
|---|---|---|
| Chunking strategy | Whatever indexed the data; no table-awareness | Full control (A-Q3) |
| Hybrid / RRF tuning | Limited | Full (A-Q4) |
| Re-ranking | Semantic ranker if configured on the index | Any re-ranker, any candidate depth (A-Q5) |
| Query rewriting / HyDE | Built-in rewriting, not yours | Full control (A-Q10, A-Q12) |
| Stage-separated eval | Hard — retrieval is inside the black box | Native (A-Q15) |
| Prompt assembly | Template with limited override | Yours |
| Multi-index routing / graph | No | Yes |

The evaluation row is the one that matters most. Section A's entire diagnostic method depends
on separating retrieval failures from generation failures. When retrieval happens inside a
managed call, you can't cleanly dump the top-K, so you lose the first debugging step.

### WHEN On Your Data is genuinely the right answer
Don't dismiss it — dismissing a managed service you haven't justified dismissing reads as
NIH:

- **Proof of concept or demo** — days matter, quality tuning doesn't yet.
- **An AI Search index already exists** and is well-built.
- **The corpus is clean prose** with no tables, forms or identifiers.
- **Small team, no RAG specialist** — a managed pipeline beats a badly-built custom one.
- **Internal low-stakes tooling** where "good enough" is genuinely good enough.

Switch to custom when quality plateaus and you need a lever the managed path doesn't expose,
when you need per-stage evaluation gates, or when the document class needs layout-aware
processing.

### Your example
JM Family uses custom RAG for chunking control. The corpus is dealer forms and financial
documents where table-aware chunking is decisive — a table split mid-row produces retrieved
chunks of numbers with no column headers. That, plus the need to run RAGAS against retrieval
and generation separately, is what put us on a custom pipeline.

### The trade-off
Custom RAG is a pipeline you own forever: ingestion, chunking, embedding, index lifecycle,
freshness, re-ranking, eval, and the on-call for all of it. On Your Data is one call
Microsoft operates. If you can't articulate a quality lever you actually need, the managed
option is the better engineering decision and the honest answer.

**Follow-up probes:**
- *"When would you use custom RAG instead?"* → When you need chunking control, custom re-ranking, or stage-separated eval. Concretely: table-heavy documents and a groundedness gate.
- *"What does `strictness` do?"* → Sets the relevance bar for including a retrieved result — higher means fewer, more relevant chunks and more refusals.
- *"Could you have started on it and migrated?"* → Yes, and that's often the right sequencing — prove the use case managed, then rebuild the pipeline where quality demands it.

**Red flag:** not knowing it exists, or dismissing it without naming the specific lever you
needed.

---

## Q22. `finish_reason = length` — what happened?

**Difficulty:** Easy · **Key terms:** `max_tokens`, truncation, context window, `content_filter`

**What they're testing:** basic API debugging. Fast, correct, and move on — but there's one
non-obvious case worth knowing.

**60-second spoken answer:**
> The generation hit a ceiling and was cut off mid-output — either your `max_tokens` setting,
> or the context window itself, because prompt plus completion has to fit in the total window.
> The fix is to raise `max_tokens`, shorten the prompt, or ask for a shorter output. The case
> that catches people out is reasoning models: their hidden reasoning tokens count against the
> completion budget, so if `max_completion_tokens` is too low the model can spend the entire
> allowance thinking and return an empty string with `finish_reason=length` — it looks like a
> failure but it's a budget problem. We monitor finish reasons in App Insights at JM Family,
> because a rising truncation rate is an early signal that prompts are growing.

### WHAT the finish reasons mean

| `finish_reason` | Meaning | Action |
|---|---|---|
| `stop` | Completed normally, or hit a stop sequence | None |
| `length` | Hit `max_tokens` or the context ceiling | Raise the budget or shorten input/output |
| `content_filter` | Content Safety blocked it | Inspect `content_filter_results` |
| `tool_calls` | Model wants a tool invoked | Execute and continue the loop |

### WHY it happens — three distinct causes
1. **`max_tokens` too small for the requested output.** The common case. You asked for a
   summary of a long document and allowed 200 tokens.
2. **Context exhaustion.** Prompt plus completion must fit the model's total window. A
   retrieval pipeline that grew its top-K silently eats the completion budget — this is the
   RAG-specific version and it correlates with A-Q7.
3. **Reasoning tokens (o-series).** Hidden chain-of-thought is billed and budgeted as
   completion tokens. Too small a `max_completion_tokens` and reasoning consumes it all,
   leaving nothing for the visible answer. You get an empty response with
   `finish_reason=length`, which reads like a bug rather than a budget.

### HOW to handle it properly
- **Detect it, always.** Treat `finish_reason != "stop"` as an error path, not a success. A
  truncated answer that your code returns as if complete is a correctness bug — the user sees
  a confident half-answer.
- **Budget explicitly.** Count prompt tokens before sending, subtract from the window, and
  set `max_tokens` from what's left with headroom.
- **For structured output, truncation is fatal** — truncated JSON won't parse. This is a
  strong argument for Structured Outputs (Q24) plus generous budget.
- **If the output is legitimately long**, don't just raise the ceiling — restructure. Chunk
  the task, stream, or use map-reduce summarisation.
- **Monitor the rate.** A rising truncation percentage means prompts are growing — usually
  top-K creep or history accumulation.

### The `content_filter` follow-up
Different failure entirely: Azure Content Safety blocked the request or the response. The
response carries `content_filter_results` with the category — hate, sexual, violence,
self-harm, plus jailbreak/prompt-shield and protected-material detections — and a severity.

Diagnose by checking whether the block was on the **prompt** or the **completion**:
- **Prompt blocked** → the user's input tripped a filter. Often a false positive on
  legitimate domain content — insurance claims describing injuries, medical records, legal
  documents about violent crime.
- **Completion blocked** → the model produced something filtered.

Remediation: adjust the content-filter policy severity thresholds on that deployment if your
use case justifies it (this requires approval for some categories), or apply for a modified
filter configuration. Log the category and severity — you need the evidence to make the case.
Never silently swallow it; return a distinguishable error so support can tell "blocked" from
"broken".

### Your example
Finish reasons are monitored in JM Family's App Insights. Tracking the distribution rather
than just error rate is what turns truncation from a user complaint into a metric — a rising
`length` share means context growth, and a rising `content_filter` share on insurance
documents means the filter is catching legitimate claim descriptions.

### The trade-off
Raising `max_tokens` costs money on every call — output tokens are the expensive ones — and
raises latency. Blindly setting it high across the board is a cost regression. Set it per
task from the expected output size, not once globally.

**Follow-up probes:**
- *"What about `finish_reason=content_filter`?"* → Content Safety blocked prompt or completion. Check `content_filter_results` for category and severity, determine which side, then tune policy or file for a modified configuration.
- *"You raised `max_tokens` and it still truncates."* → You're hitting the context window, not the parameter. Prompt plus completion must fit — shrink the prompt.
- *"Empty response, `finish_reason=length`, on o1?"* → Reasoning tokens consumed the whole completion budget. Raise `max_completion_tokens`.

**Red flag:** returning a truncated answer to the user as if it completed.

---

## Q23. Function calling — who executes the function?

**Difficulty:** Medium · **Key terms:** tool calls, argument validation, confused deputy

**What they're testing:** a security instinct. There's exactly one right answer and it's about
trust boundaries.

**60-second spoken answer:**
> Your code does. Always. The model only decides *what* it wants called and with *what*
> arguments — it returns a structured tool-call object and stops. Nothing executes unless your
> code chooses to execute it. That distinction is the entire security model, because the
> model's output is untrusted input: it's been influenced by the user's message and by any
> document you retrieved, either of which may be adversarial. So between the tool call and the
> execution there has to be a gate — validate the arguments against a schema, check the
> *user's* permissions rather than the app's, confirm the tool exists in the registry, and log
> it. At JM Family we validate dealer codes against the source of truth before any tool
> executes, because the model will occasionally produce one that looks perfectly plausible and
> doesn't exist.

### WHAT the loop actually is
1. You send messages plus tool definitions (name, description, JSON-schema parameters).
2. The model responds with `finish_reason: tool_calls` and a structured call — a name and a
   JSON arguments object.
3. **Your code decides.** Validate, authorise, execute — or refuse.
4. You append the result as a tool message and call the model again.
5. It either calls another tool or produces the final answer.

The model has no execution capability. It emits a request.

### WHY this is a security boundary, not a formality
The tool call is **user-influenced output from an untrusted component**. Two attack paths:

- **Direct injection** — the user talks the model into calling a tool with parameters they
  shouldn't be able to reach: another customer's account, a broader query, a destructive
  operation.
- **Indirect injection** — a retrieved document contains instructions. Your RAG pipeline
  pulled it in, the model read it as context, and it says "call `delete_records` for all
  accounts." The user never typed anything malicious.

The classic failure is the **confused deputy**: the app holds broad permissions, the model
requests an action, the app executes with *its own* privileges, and the user has just
performed an action they were never authorised to perform. The fix is authorising against the
end user's identity — not the service principal's — at the execution gate.

### HOW to build the gate
1. **Tool exists?** Look the name up in a registry. Models invent tool names; reject unknowns
   and return a structured error the model can recover from.
2. **Schema-validate the arguments.** Types, enums, ranges, formats. Use enums wherever the
   domain is closed — it constrains generation as well as validating it.
3. **Validate against reality.** A well-formed dealer code is not a real dealer code. Check
   the source of truth before acting.
4. **Authorise as the user.** Pass the caller's identity through and check entitlement for
   *this* record. Never rely on the app's own permissions.
5. **Classify by side effect.** Reads execute freely. Writes and destructive operations get
   confirmation, an approval step, or a human in the loop.
6. **Bound it.** Rate-limit tool calls per session, cap iterations, make writes idempotent so
   a retry can't double-charge.
7. **Log everything** — the call, the arguments, the decision, the result. This is your audit
   trail and your debugging record.

### HOW to handle a hallucinated argument
Don't throw. **Return the error to the model as a tool result** — "dealer code DLR-9999 not
found; valid codes for this user are …" — and let it correct itself. The self-correction loop
is what makes tool use robust, and it's the same pattern as the Validator node in the
LangGraph state machine. Bound the retries so a persistent failure escalates instead of
looping.

Prevention beats correction: enums in the schema for closed domains, Structured Outputs
strict mode on tool schemas (Q24), and tool descriptions specific enough that the model isn't
guessing.

### WHEN to require a human
Any irreversible or outward-facing action — payments, deletions, sending communications,
anything a regulator would want a name attached to. The pattern is the model proposes, a
person disposes, and the approval is logged with the proposed arguments visible.

### Your example
JM Family validates dealer codes before executing. Codes are a closed domain with a source of
truth, and a model asked for one it hasn't seen will produce something that has the right
shape and doesn't exist. Validating at the gate turns a wrong answer into a correction loop
rather than a query against a nonexistent entity.

### The trade-off
The gate adds latency per tool call and real code to maintain — a registry, schemas,
authorisation checks, audit logging. Strict validation also raises failure rate: calls that
would have half-worked now get rejected, and you need the correction loop to keep the UX
acceptable. That's the right trade for anything touching customer data, and probably
over-engineering for a read-only internal search tool.

**Follow-up probes:**
- *"The model hallucinates an argument — what happens?"* → Validation rejects it, the error goes back as a tool result, the model corrects. Bounded retries, then escalate.
- *"What's the confused deputy problem here?"* → App executes with its own broad permissions on behalf of a user who lacks them. Authorise as the end user at the gate.
- *"A retrieved document tells the model to call a destructive tool."* → Indirect prompt injection. Side-effect classification plus user-identity authorisation stops it; the gate doesn't care where the instruction came from.

**Red flag:** "the model calls the function." It cannot. If you say this, the security
follow-up is coming.

---

## Q24. What are Structured Outputs?

**Difficulty:** Medium · **Key terms:** `json_schema`, strict mode, constrained decoding

**What they're testing:** whether you know the difference between *guaranteed shape* and
*guaranteed correctness* — the distinction most people blur.

**60-second spoken answer:**
> It's a mode where you hand the model a JSON schema and the output is guaranteed to conform
> to it — not "usually valid JSON" but structurally guaranteed, because decoding is constrained
> to tokens the schema permits. It replaces the old approach of asking nicely in the prompt and
> writing a retry loop around a parse failure. The distinction I'd stress is that it guarantees
> *shape*, not *truth*: you'll always get a well-formed object with the right fields and types,
> and the values inside can still be wrong or hallucinated. So it eliminates a class of
> integration bugs, not a class of accuracy bugs — you still need validation. It's the obvious
> fit for the invoice extraction work at JM Family, where a downstream system needs a strict
> contract.

### WHAT it is, versus the alternatives

| Approach | Guarantee |
|---|---|
| "Respond in JSON" in the prompt | None. Prose preambles, markdown fences, trailing commas. |
| `response_format: json_object` | Syntactically valid JSON. **Not your schema** — any keys, any shape. |
| `response_format: json_schema` + `strict: true` | Conforms to your schema: keys, types, nesting, enums. |

### WHY constrained decoding gives a hard guarantee
It's not better instruction-following. At each decoding step the sampler is masked to tokens
that can still lead to a valid instance of the schema. A token that would break the structure
has its probability zeroed — it's not that the model chooses not to emit it, it *can't*. That
is why the guarantee is structural rather than probabilistic, and why it's categorically
different from prompting.

### WHAT it doesn't guarantee — say this unprompted
The schema constrains structure. It says nothing about whether `"invoice_total": 4820.00` is
the number on the invoice. Every hallucination failure mode survives Structured Outputs
intact — it just arrives well-formed now, which arguably makes it *more* dangerous, because
well-formed output invites downstream trust. You still need field-level validation, a
confidence signal, and reconciliation against a source of truth. Being the candidate who
volunteers this is the difference between having read about the feature and having shipped it.

### The strict-mode constraints (know these)
- Every property must be listed in `required` — model optionality with a union type including
  `null`, not by omitting the field.
- `additionalProperties: false` is mandatory on every object.
- Only a subset of JSON Schema is supported — no arbitrary `pattern`/`format` enforcement,
  bounded nesting depth and total property count.
- The first request with a new schema pays a preparation cost; it's cached afterward, so
  schema churn has a latency price.

### WHEN to use it
| Use it | Don't |
|---|---|
| Extraction feeding a downstream system | Free-form conversational answers |
| Tool/function argument generation | Long-form summaries or explanations |
| Classification into a fixed label set (enums) | Exploratory or creative output |
| Anything a parser consumes | Where the shape genuinely varies per input |

### HOW you'd apply it to invoice extraction
> **Phrase this as a design, not a deployment.** The source note marks it a *candidate* at JM
> Family. "That's exactly where I'd apply it, and here's the design" is a strong answer.
> "We use it in production" invites a throughput and schema-versioning follow-up you'd have to
> improvise.

1. Schema per document type — invoice, claim, policy — with enums for closed fields
   (currency, document type, status) and nullable unions for genuinely optional ones.
2. Extract with strict mode, one document at a time, temperature 0.
3. **Validate beyond the schema**: totals reconcile against line items, dates are plausible,
   dealer codes exist (Q23), amounts within expected ranges.
4. Attach provenance — page and bounding box from Document Intelligence — so every field is
   auditable back to the source.
5. Route low-confidence or reconciliation-failing extractions to human review rather than
   straight through.
6. Version the schema explicitly; downstream consumers pin a version.

Step 3 is the one that matters. Structured Outputs got you a valid object; only reconciliation
tells you it's the right object.

### Your example
Invoice extraction at JM Family is the natural fit — the output feeds a downstream enterprise
system that needs a strict contract, and the current approach relies on prompt-level JSON
instructions with parse-failure handling. Structured Outputs removes that entire failure mode.
It's a design I'd defend rather than a system I've run.

### The trade-off
The schema becomes a coupling point: change it and you must version it and coordinate with
every consumer. Strict mode's constraints — everything required, no `additionalProperties` —
force awkward modelling for genuinely optional data. Over-constraining also hurts quality:
force a model to emit a field it has no basis for and it will invent a value rather than
leave it out, because the schema demands it. That's a real failure mode — make genuinely
absent fields nullable and mean it.

**Follow-up probes:**
- *"How is this different from `json_object` mode?"* → `json_object` guarantees valid JSON. `json_schema` + strict guarantees *your* JSON — keys, types, enums.
- *"Does it stop hallucination?"* → No. It guarantees shape, not truth. Values can still be wrong, and now they're well-formed and easier to trust by mistake.
- *"What happens to a field the document doesn't contain?"* → If it's required, the model invents something. Make it a nullable union — that's the whole reason the union pattern exists.

**Red flag:** "it makes the model output JSON." So does asking nicely, most of the time. The
answer is the *guarantee* and its precise limits.

---

## Q25. o1/o3 reasoning models — when do you use them, and how do they differ in the API?

**Difficulty:** Hard · **Key terms:** reasoning tokens, hidden chain-of-thought, `reasoning_effort`

**What they're testing:** currency, plus whether you know the counter-intuitive prompting
guidance. The "don't say think step by step" follow-up is the real question.

**60-second spoken answer:**
> Reasoning models generate a hidden chain of thought before the visible answer. Those
> reasoning tokens are billed as output and count against your completion budget, which is the
> main operational surprise. The API differs in specific ways: no temperature or top-p — you
> can't sample-tune them — `max_completion_tokens` instead of `max_tokens`, and a
> `reasoning_effort` control to trade depth against cost and latency. They're for genuinely
> hard multi-step problems: complex analysis, tricky code, multi-constraint planning. They're
> badly wrong for classification, extraction, or summarisation, where you pay several times
> the cost and multiples of the latency for no quality gain. At JM Family we reserve o1 for
> complex analysis and everything routine goes to the 4o tier.

### WHAT is different at the API level

| | Standard (GPT-4o) | Reasoning (o-series) |
|---|---|---|
| Hidden reasoning | No | Yes — billed as output, not returned |
| `temperature` / `top_p` | Yes | Not supported |
| Token budget param | `max_tokens` | `max_completion_tokens` |
| Effort control | — | `reasoning_effort`: low / medium / high |
| System message | `system` | `developer` (varies by version) |
| Latency | Sub-second to seconds | Seconds to minutes |
| Cost per task | Baseline | Substantially higher — reasoning tokens dominate |

Feature support (streaming, tools, structured outputs, images) has varied across o-series
versions — check the current matrix rather than assuming.

### WHY you don't tell them to "think step by step"
This is the interesting part, and it's counter-intuitive enough to be a good discriminator.

The model already produces an extended internal chain of thought — that's the architecture,
reinforced during training. Explicit CoT prompting on top of that is at best redundant and
measurably harmful in practice: it constrains a reasoning process that was trained to find
its own structure, and it burns tokens restating a strategy the model would have chosen.
OpenAI's own guidance is to prompt these models *simply* and directly.

The corollary is that heavy few-shot prompting also tends to hurt. With a standard model,
examples are how you communicate the pattern. With a reasoning model, a clear statement of
the goal and the constraints usually beats a wall of examples — zero-shot or minimal-shot is
the recommended starting point. So your accumulated prompt-engineering instincts partially
*invert* here, which is exactly why the question gets asked.

### WHY the budget gotcha bites
Reasoning tokens consume `max_completion_tokens`. Set it at 500 for a hard problem and the
model can spend all 500 thinking, leaving nothing for the answer — you get an empty response
with `finish_reason: length` (Q22). It reads like an API failure and it's a budgeting error.
Budget generously for these models and monitor the reasoning-token share.

### WHEN to use them
| Reasoning model | Standard model |
|---|---|
| Multi-step analysis with interacting constraints | Classification, extraction, tagging |
| Hard debugging, non-trivial algorithm design | Summarisation, rewriting |
| Complex planning where a wrong step compounds | RAG answer generation |
| Ambiguous problems needing decomposition | High-volume, latency-sensitive paths |
| Evaluating or judging other models' hard outputs | Anything on an interactive hot path |

A useful production pattern: **escalation**. Run the standard model, detect low confidence or
a validation failure, escalate that fraction to the reasoning model. You get reasoning
quality on the cases that need it at a small share of the cost.

### Your example
JM Family reserves o1 for complex analysis. The routine RAG and extraction traffic runs on
the 4o tier — putting it on a reasoning model would multiply cost and latency for no quality
gain, which is the opposite of the cost programme in Q19 and Q29.

### The trade-off
Cost and latency, both by multiples, and the latency is the harder constraint — a response
measured in tens of seconds rules them out of interactive UX without a progress affordance.
You also lose sampling control, so techniques that depend on temperature — self-consistency
sampling, controlled variation — aren't available. And the hidden reasoning is genuinely
hidden: you're billed for tokens you can't inspect, which complicates debugging and cost
attribution.

**Follow-up probes:**
- *"Why not add 'think step by step'?"* → It already does, internally and better. Explicit CoT constrains a trained process and wastes tokens; guidance is to prompt simply.
- *"Empty response with `finish_reason: length` — why?"* → Reasoning tokens consumed the whole `max_completion_tokens` budget.
- *"How do you use one without the cost?"* → Escalation. Standard model first, escalate only low-confidence or validation-failing cases.

**Red flag:** describing them as "just better GPT-4" — the API surface, cost profile, and
prompting guidance are all different.

---

## Q26. Managed Identity vs API keys for Azure OpenAI.

**Difficulty:** Easy · **Key terms:** `DefaultAzureCredential`, RBAC, `disableLocalAuth`

**What they're testing:** production security hygiene. The remediation follow-up is where the
real answer is.

**60-second spoken answer:**
> Managed Identity in anything that runs in Azure, keys only for local development. With
> Managed Identity the platform issues and rotates the token — there is no secret in a config
> file, no secret in Key Vault to manage, and nothing to leak. You grant the identity the
> Cognitive Services OpenAI User role, use DefaultAzureCredential in code, and every call is
> attributable to a named principal in the Entra sign-in logs, which is what turns access into
> an audit trail. The step people forget is disabling local auth on the resource entirely —
> otherwise the keys still work and your Managed Identity migration is a convention rather
> than a control. JM Family's Functions run on Managed Identity.

### WHAT the difference is

| | API key | Managed Identity |
|---|---|---|
| What it is | Shared secret, two per resource | Entra identity bound to the Azure resource |
| Rotation | Manual, coordinated across consumers | Platform-handled, transparent |
| Attribution | None — every caller looks identical | Per-principal in Entra sign-in logs |
| Granularity | All-or-nothing | RBAC roles, scopable |
| Revocation | Rotate and update every consumer | Remove the role assignment |
| Leak blast radius | Full access until noticed and rotated | No secret exists to leak |
| Works outside Azure | Yes | Needs federation |

Relevant roles: **Cognitive Services OpenAI User** for inference, **Cognitive Services OpenAI
Contributor** for managing deployments. Grant the narrower one to workloads.

### WHY "we store the key in Key Vault" isn't the same thing
It's better than a config file, and it's still a shared secret. It has no per-caller
attribution, it needs a rotation process someone owns, and anything that can read the vault
secret has the full key. Key Vault moves the secret; Managed Identity removes it. The
question to ask of any design is "what's the blast radius if this leaks" — with MI there's
nothing to leak.

### HOW it works
`DefaultAzureCredential` walks a chain of credential sources: environment variables, workload
identity, managed identity, Azure CLI login, and so on. The same code path therefore works in
production (managed identity), in CI (workload identity federation), and on a developer laptop
(`az login`) with no branching. That uniformity is why it's the recommended pattern — the code
doesn't know or care which mechanism supplied the token.

### HOW to remediate a key-in-code finding
The follow-up, and the order matters:

1. **Rotate the key immediately.** It's compromised the moment it's in a repo — assume
   disclosure. Rotate first, investigate second.
2. **Check for use.** Diagnostic logs and Entra sign-in logs for calls you can't account for.
3. **Migrate the caller** to Managed Identity: assign the role, switch to
   `DefaultAzureCredential`, deploy, verify.
4. **Disable local auth** — `disableLocalAuth: true` on the resource. This is the control. Until
   you do this, keys still work and nothing structurally prevents the next occurrence.
5. **Purge from history.** A deleted line is still in git history and in any fork or clone.
   History rewrite, and if the repo is public, assume permanent disclosure regardless.
6. **Prevent recurrence** — secret scanning with push protection, a pre-commit hook, and a
   policy check that flags resources with local auth enabled.

Steps 4 and 5 are what distinguish a real remediation from a cleanup. Most people stop at 3.

### WHEN keys are still acceptable
- Local development against a non-production resource.
- A caller genuinely outside Azure with no federation path — though workload identity
  federation covers GitHub Actions and most CI today, so this shrinks every year.
- Short-lived spikes and demos.

Never in a deployed workload, and never in a repo.

### Your example
JM Family's Azure Functions authenticate to Azure OpenAI with Managed Identity. Combined with
the Private Link posture in Q27, that's the pair of controls that made the platform
approvable for financial documents — no secret to leak and no public network path to use it
on.

### The trade-off
Managed Identity is Azure-native, so a hybrid or multi-cloud caller needs federation, which
is more setup than pasting a key. Local development requires `az login` and a role assignment
on a dev resource, which is real friction for a new joiner. Token acquisition also adds a
small first-call latency, though the credential caches. All small prices; none justify a key
in a deployed service.

**Follow-up probes:**
- *"Security flags a key in code. Walk me through remediation."* → Rotate → check logs for misuse → migrate to MI → **disable local auth** → purge git history → add push protection.
- *"Why not just put the key in Key Vault?"* → Still a shared secret: no attribution, needs rotation, full access to anything that can read it. MI removes the secret rather than relocating it.
- *"How does local dev work?"* → `DefaultAzureCredential` falls through to `az login`. Same code, different credential source.

**Red flag:** "we keep the key in Key Vault" offered as the secure answer. It's the
second-best answer, and the interviewer is waiting for the first.

---

## Q27. What's the production network posture for Azure OpenAI?

**Difficulty:** Medium · **Key terms:** Private Link, VNet, private DNS zone, network isolation

**What they're testing:** whether you've actually deployed this. The DNS follow-up is
unmistakable — only people who've debugged it know the answer.

**60-second spoken answer:**
> Public network access disabled on the resource, a Private Endpoint into the application's
> VNet, and the private DNS zone linked so the endpoint's hostname resolves to the private IP.
> The result is that the service has no reachable public path at all and traffic stays on the
> Microsoft backbone — that's a different assurance from "encrypted in transit," and it's the
> one a regulator asks for. Combined with Managed Identity, there's no key to steal and no
> public endpoint to use it against. For JM Family's financial documents this wasn't optional.

### WHAT the configuration is
1. `publicNetworkAccess: Disabled` on the Azure OpenAI resource.
2. A **Private Endpoint** — a NIC with a private IP in your VNet subnet.
3. The **private DNS zone** `privatelink.openai.azure.com`, containing an A record for your
   resource, **linked to the VNet**.
4. NSGs and, typically, egress through Azure Firewall for the rest of the workload.
5. Optionally customer-managed keys for data at rest, if the compliance regime requires
   control of the key.

### WHY DNS is the part that breaks
Your code calls `https://myresource.openai.azure.com`. That public hostname resolves via CNAME
to `myresource.privatelink.openai.azure.com`. Whether you reach the private endpoint depends
entirely on who answers that second name:

- **Private DNS zone linked to the VNet** → resolves to the private IP. Works.
- **Not linked, or resolved by a DNS server that doesn't know about it** → resolves to the
  public IP. And since public access is disabled, the connection fails — or worse, in a
  partially-configured environment, silently egresses over the internet.

### HOW to fix "DNS returns the public IP"
The diagnostic sequence, which is the answer they want:

1. `nslookup myresource.openai.azure.com` **from inside the VNet** — a laptop tells you
   nothing.
2. Public IP returned → resolution isn't reaching the private zone.
3. **Check the virtual network link.** The private DNS zone must have a link to *this* VNet.
   Creating the zone isn't enough; the link is a separate object and is the single most
   common omission.
4. **Check for custom DNS.** If the VNet uses custom DNS servers — a domain controller, an
   on-prem forwarder — those servers must conditionally forward to Azure-provided DNS at
   `168.63.129.16`. Without that, they answer from public DNS and you get the public IP.
5. **Hub-and-spoke:** the zone is usually linked to the hub VNet, and spokes resolve through
   it. A new spoke that was never linked is the recurring version of this bug.
6. **Check the A record exists** in the zone for your resource.
7. **Then check connectivity** — NSG rules, endpoint approval state.

Saying "check the private DNS zone is linked to the VNet, and if there's custom DNS, that it
forwards to 168.63.129.16" is the sentence that proves you've done this.

### WHY this is more than encryption in transit
TLS protects the payload from reading in flight. It doesn't change the fact that the traffic
traverses the public internet and the service has a public endpoint any credential holder can
reach from anywhere. Private Link removes the path. For a control framework, "the service is
not reachable from the internet" is a categorically stronger statement than "the traffic is
encrypted" — and it's the one that closes the finding.

### WHEN full isolation isn't warranted
Be able to say this — reflexively maximum security signals inexperience with cost:

- Dev and test environments with synthetic data — Private Link adds cost and friction for no
  protected asset.
- Public-data workloads with no confidentiality requirement.
- Where a service endpoint or IP allowlist meets the actual control requirement.

Private Endpoints cost per hour plus data processed, and they add DNS complexity to every
environment. Apply them where the data class justifies it.

### Your example
Private Link is required for JM Family's financial documents. Public access disabled, Private
Endpoint into the application VNet, private DNS zone linked. Together with Managed Identity
(Q26), that's the network-plus-identity pair the compliance review actually assessed.

### The trade-off
Operational complexity, concentrated in DNS — which is also where it fails, usually at
environment-creation time when someone forgets the zone link. It complicates local
development (you can't reach the endpoint from a laptop without VPN or a jump host), CI
(build agents need VNet integration), and any external integration. Budget for the DNS
troubleshooting; every team hits it at least once.

**Follow-up probes:**
- *"DNS returns the public IP. Why?"* → Private DNS zone not linked to the VNet, or custom DNS not forwarding to 168.63.129.16. Test resolution from inside the VNet.
- *"Isn't TLS enough?"* → TLS protects the payload; Private Link removes the internet path and the public endpoint. Different controls, different assurances.
- *"How do developers work against this?"* → VPN or bastion into the VNet, or a separate non-isolated dev resource with non-production data.

**Red flag:** listing Private Endpoint without mentioning DNS. The endpoint is the easy half.

---

## Q28. What's the Batch API for?

**Difficulty:** Easy · **Key terms:** batch inference, ~50% discount, enqueued-token quota

**What they're testing:** cost awareness, and whether you match workload shape to service tier.

**60-second spoken answer:**
> Asynchronous bulk processing at roughly half the price of the synchronous path, with a
> 24-hour target turnaround. You submit a file of requests in JSONL, it processes offline, and
> you collect the results. The reason it's cheaper is that you're giving up latency
> guarantees — Microsoft schedules it into spare capacity. The other benefit people miss is
> that it draws on a separate enqueued-token quota, so a large batch job doesn't consume the
> TPM your interactive path depends on. That solves the throttling problem from a different
> direction. Nightly classification of support tickets is the textbook fit, and it's a
> workload shape JM Family has.

### WHAT it is
- Submit a JSONL file, one request object per line, to a Global Batch deployment.
- The service processes asynchronously against a 24-hour target.
- Results come back as a file; failures are reported per line.
- Priced at a substantial discount to the synchronous equivalent — commonly cited around 50%,
  but check current pricing.
- Consumes a **separate enqueued-token quota**, not your standard TPM.

### WHY it's cheaper
You're selling latency flexibility back to the provider. Synchronous inference must be served
now, which requires capacity held ready for peak. Batch work can be scheduled into troughs,
so it's served from capacity that would otherwise sit idle. The discount is the price of that
scheduling freedom — the same economics as spot compute.

### WHY the separate quota matters more than the discount
This is the point that gets overlooked. A large embedding backfill or classification run on
your standard deployment competes directly with interactive traffic for TPM, and the batch
job wins on volume — so users get throttled while a background job runs. Batch draws from a
different pool. It's architectural isolation between interactive and bulk paths, which is the
same principle as separate deployments in Q17 and queue-pacing in Q20, achieved without
running the queue yourself.

### WHEN to use it
| Good fit | Bad fit |
|---|---|
| Nightly/periodic classification | Anything a user is waiting on |
| Embedding backfill or re-embedding a corpus | Interactive chat |
| Bulk summarisation of a document set | Real-time extraction in a workflow |
| Evaluation runs over a golden set | Anything with an SLA under 24 hours |
| Synthetic data generation | Sub-hour freshness requirements |

The clean test: **would a user notice if this took 20 hours?** No → batch.

### HOW you'd design around it
1. Accumulate work into a batch window rather than firing per event.
2. Generate the JSONL with a stable `custom_id` per line so results reconcile back to source
   records — you get results out of order and partially failed.
3. Submit, poll for completion, retrieve.
4. **Handle partial failure and expiry.** Individual lines fail; whole jobs can miss the
   window and expire. You need per-line retry logic and an escalation path for expired jobs —
   the 24 hours is a target, not a guarantee.
5. Reconcile by `custom_id`, persist, alert on unreconciled records.
6. Keep a synchronous fallback for anything that becomes urgent.

Step 4 is where naive implementations break. Treat batch as an unreliable channel that's
cheap, not a reliable channel that's slow.

### Your example
> **Phrase as a fit, not a deployment.** The source marks this a candidate. "That's the
> workload shape it's for, and here's how I'd run it" is defensible; claiming it in
> production invites questions about reconciliation and expiry handling.

Nightly ticket classification at JM Family is exactly the profile — high volume, no
interactive consumer, a natural overnight window. Running it on the interactive deployment
would consume TPM the daytime path needs, so the separate quota is as much of the argument
as the discount. Re-embedding after a model change is the other obvious candidate, given the
500K-document corpus.

### The trade-off
You surrender latency control entirely — no SLA, jobs can expire, and you need the
reconciliation and retry machinery to run it safely. The file-based interface is clumsier
than an API call, and debugging a failed line inside a large job is harder than debugging a
request. It's the right tool for genuinely deferrable work and a liability for anything else.

**Follow-up probes:**
- *"Is 24 hours acceptable?"* → It's the test for whether batch fits. If nobody notices a 20-hour turnaround, yes — and it halves the bill and protects interactive quota.
- *"What if a job doesn't complete in the window?"* → It can expire. You need per-line reconciliation by `custom_id`, retry, and an escalation path to the synchronous deployment.
- *"Does it use my TPM quota?"* → No — a separate enqueued-token quota. That isolation is often worth more than the discount.

**Red flag:** knowing only the discount and not the separate quota pool.

---

## Q29. GPT-4o vs GPT-4o-mini — how do you choose?

**Difficulty:** Medium · **Key terms:** model routing, cost tiering, cascade, eval-driven selection

**What they're testing:** cost engineering with evidence. The "use the best model for
everything" counter is the real question.

**60-second spoken answer:**
> Empirically, per task, against an eval set — not by reputation. The method I use is: build
> on the strong model first to establish what good looks like and get a quality ceiling, then
> take that eval set and run the cheap model against it task by task. On narrow structured
> work — classification, extraction, straightforward summarisation, routine RAG answers —
> mini typically lands close enough to be indistinguishable, at a fraction of the price and
> half the latency. Where it falls down is multi-step reasoning and instruction-heavy prompts
> with many interacting constraints. So you route: cheap by default, escalate what needs it.
> That routing is a meaningful share of the ~30% inference cost reduction at JM Family, and
> the reason it was defensible is that the eval numbers were on the table.

### WHAT the actual difference is
Mini is roughly an order of magnitude cheaper per token and materially faster, with quality
that holds up on narrow tasks and degrades on hard ones. **Don't quote a precise multiple in
an interview** — pricing changes and being wrong on a number you volunteered is worse than
saying "about an order of magnitude, I'd check the current sheet."

Where mini reliably holds:
- Classification into a defined label set
- Structured extraction with a schema
- Summarisation of a bounded input
- Straightforward RAG answers over good retrieved context
- Query rewriting and coreference resolution (A-Q10)

Where it degrades:
- Multi-step reasoning where an early error compounds
- Prompts with many interacting constraints
- Long-context synthesis across many documents
- Ambiguous extraction needing judgement
- Agentic loops where tool-selection quality drives everything downstream

### WHY "build on the strong model, then downgrade" is the right order
Building on the cheap model first confounds two questions: is the *task design* wrong, or is
the *model* insufficient? You can't tell, and you'll spend days prompt-engineering around a
capability gap. Establish the ceiling on the strong model, get the eval set and the target
number, then treat downgrading as a measured experiment with a pass/fail bar. It's also the
argument that survives contact with a stakeholder: "mini scores within one point of 4o on
this task's eval, at a tenth the cost" is a decision. "Mini felt fine" is not.

### HOW to route
| Strategy | Mechanism | Trade-off |
|---|---|---|
| **Static by task** | Extraction → mini; agent planning → 4o | Simple, predictable. Ignores per-query difficulty. |
| **Rule-based** | Length, intent classification, document type | Cheap. Rules drift from reality. |
| **Classifier** | A small model predicts required tier | Adapts. Another component to train and monitor. |
| **Cascade** | Try mini; validate; escalate on failure or low confidence | Best cost/quality. Escalated queries pay twice and take longer. |

Cascade is usually the strongest production answer, provided you have a cheap, reliable
signal for "this answer isn't good enough" — schema validation failure, a groundedness score
below threshold, a refusal, or an explicit confidence field.

### HOW to counter "just use GPT-4o for everything"
Three arguments, in order of force:

1. **Evidence.** "Here's the eval on our own golden set: mini matches 4o on this task class
   within the noise band. Here's the cost delta at our volume." A number beats an opinion.
2. **Latency.** Mini's speed is a *product* argument, not just a cost one. On an interactive
   path, halving response time is a UX improvement you'd pay for — here you get paid for it.
3. **Headroom.** Routing simple traffic off the 4o deployment frees TPM for the queries that
   genuinely need it, which directly reduces throttling (Q20).

And concede the real risk honestly: routing adds a component that can send a hard query to a
weak model. You mitigate with a conservative default, cascade escalation, and monitoring the
quality metric per tier — not by pretending the risk isn't there.

### Your example
JM Family routes by query complexity. Model-tier selection sits alongside semantic caching
and token-budget management in the programme that produced the ~30% inference cost reduction
(~$150K/year). The tiering was defensible because it was eval-backed per task class rather
than a blanket downgrade.

### The trade-off
Routing is a component with its own failure mode: misroute a hard query and the user gets a
worse answer, and the router's mistakes are invisible unless you monitor quality per tier.
It also multiplies evaluation work — every task now needs eval on two models, and every model
version change means re-validating both. And it fragments quota across deployments. Below a
certain volume the engineering cost exceeds the savings; do the arithmetic before building it.

**Follow-up probes:**
- *"Someone says use GPT-4o for everything. Counter it."* → Eval numbers on our own golden set, plus the latency win, plus the quota headroom. Concede that routing adds a misroute risk and say how you monitor it.
- *"How do you know mini is good enough?"* → It scores within a defined band of 4o on that task's golden set. Not vibes — a gate.
- *"What breaks when you downgrade?"* → Multi-step reasoning and instruction-dense prompts first. Structured extraction and classification hold up.

**Red flag:** picking a model by reputation with no eval. This question is about method.

---

## Q30. How do you handle Azure OpenAI model deprecation?

**Difficulty:** Medium · **Key terms:** deprecation vs retirement, version-agnostic routing, region availability

**What they're testing:** whether you plan for the lifecycle, or get surprised by it. This is
an operations-maturity question.

**60-second spoken answer:**
> Three things. First, architecture: never let a model version reach application code — the
> deployment name is the contract, the version lives in infrastructure config, so a migration
> is a deployment change and a canary rather than a release across every service. Second,
> process: track Microsoft's retirement notices, and understand there are two dates —
> deprecation, when you can't create new deployments, and retirement, when existing ones stop.
> A pinned version gets force-upgraded at retirement whether you're ready or not, so pinning
> buys scheduling control, not immunity. Third, validation: before cutover, confirm the target
> version exists in every region you deploy to, run the golden set against it, canary a slice,
> then move. At JM Family that abstraction layer is in the services already.

### WHAT the two dates mean

| Event | What happens |
|---|---|
| **Deprecation** | No new deployments on that version. Existing ones keep serving. |
| **Retirement** | Existing deployments stop, or are force-upgraded to a successor. |

Microsoft publishes both and has generally given extended notice for GA models — commonly
cited as around twelve months, though timelines have varied and preview models move faster.
Treat the published date as the fact and the norm as a planning assumption.

### WHY this is an architecture question, not a calendar question
If model versions are hard-coded across a dozen services, a retirement is a coordinated
multi-team release under a deadline you don't control. If the deployment name is the only
thing in code, it's an infrastructure change with a canary. The mitigation is designed in
long before the notice arrives — which is why the question is really "did you build the
indirection?"

The layered version:
1. **Code** knows only a logical name — `"chat-primary"`, `"embedding-primary"`.
2. **Config** maps that to a deployment name per environment.
3. **Infrastructure** binds the deployment to a model and version.

Now a version move touches layer 3 only.

### HOW the migration actually runs
1. **Notice arrives.** Log the retirement date; work backwards to a cutover with slack.
2. **Check region availability first.** The new version may not exist yet in every region you
   deploy to. This is the constraint that most often forces a plan change, so check it before
   planning anything else.
3. **Stand up a parallel deployment** on the new version.
4. **Run the golden set** against it — both retrieval and generation metrics (A-Q15). Model
   versions change output format, refusal boundaries, and tool-call reliability.
5. **Diff the behaviour** on a sample of real traffic: format drift, refusal rate, latency,
   token consumption. A new version that's 15% more verbose is a cost regression that no
   quality metric catches.
6. **Canary** a traffic slice with the quality metric watched.
7. **Cut over**, keep the old deployment briefly for rollback, then retire it.

### HOW to handle "the new version isn't in one of your regions"
Options in the order you'd consider them:

1. **Wait**, if the retirement date allows — availability usually spreads. Track it.
2. **Route that region's traffic to a compliant region** that has it. Check data residency
   before you do — this can be the thing you cannot do, and it's the answer they're probing
   for.
3. **Change deployment type.** Global or Data Zone deployments have different availability
   footprints than regional Standard, and a Data Zone deployment may satisfy residency while
   giving you availability.
4. **Move to a different model** available everywhere, with eval to confirm it holds.
5. **Escalate to Microsoft** — account teams can give region roadmap detail and sometimes
   accelerate.

Naming the residency constraint unprompted is the mark of someone who's done this in a
regulated environment.

### The `ada-002` problem — raise this yourself
Your resume and the Interview Bible name `text-embedding-ada-002` as the JM Family embedding
model. It's a 2022 model on the retirement track, and `text-embedding-3-large` and `-small`
beat it on quality and price. Expect the question.

The strong answer is a migration plan with the hard part acknowledged: **changing embedding
models invalidates the entire index.** Vectors from different models aren't comparable, so
migration means re-embedding all 500K+ documents — a batch cost (Q28 is exactly the vehicle),
a parallel index, a re-run of the retrieval golden set to confirm the new model is actually
better on *your* corpus, and an atomic index alias flip. That is a project, not a config
change, and being able to size it is the answer. "We're on ada-002" with no plan is the weak
version.

Note the `-3` models support `dimensions` truncation, so you can trade a little quality for
smaller vectors and lower storage — a genuine lever at 500K documents.

### Your example
JM Family's services sit behind an abstraction layer, so the model version is infrastructure
config rather than application code. That's what makes a version migration a canary instead
of a coordinated release. The open item is the ada-002 embedding migration, which is sized as
a re-embedding project rather than a version bump.

### The trade-off
The indirection has a cost: another config layer, another thing that can be misconfigured,
and an added hop between "what model is this call using" and the code — which makes debugging
marginally harder. Running parallel deployments during migration doubles the quota footprint
and, on PTU, doubles the committed spend for the overlap. Both are cheap relative to an
unplanned forced upgrade.

**Follow-up probes:**
- *"The new version isn't available in one of your regions."* → Wait if the timeline allows; otherwise cross-region routing subject to a residency check, a Global/Data Zone deployment type, or a different model. Residency is usually the binding constraint.
- *"How do you know the new version didn't break anything?"* → Golden set on both stages, plus a behavioural diff on real traffic for format drift, refusal rate, and token consumption.
- *"Why not just use auto-update?"* → It moves you silently with no deploy to correlate a regression to. Fine in dev, not in production.

**Red flag:** "Microsoft gives twelve months' notice" as the whole answer. The question is
what you built so the notice is routine.

---

## Drill sheet — the one-line version of each

| # | Question | The sentence that must appear |
|---:|---|---|
| 16 | Azure vs OpenAI direct | Same models, different control plane: residency, Managed Identity, Private Link, no training on your data |
| 17 | What is a deployment | Named instance binding model + version + quota; the name is the stable contract |
| 18 | What is Foundry | Hub/Project platform: catalog, Prompt Flow, evals, Content Safety, agents. Prototype here, productionize in code |
| 19 | PTU vs PAYG | A utilisation question. PTU for the baseline, PAYG for the overflow |
| 20 | 429 handling | Quota lives on the deployment — more instances make it worse |
| 21 | On Your Data | Managed RAG in one call; you trade away chunking, re-ranking and stage-separated eval |
| 22 | `finish_reason=length` | Hit `max_tokens` or the context ceiling — and reasoning tokens count against the budget |
| 23 | Function calling | **Your code executes.** The model only requests. That gap is the security boundary |
| 24 | Structured Outputs | Guarantees shape, not truth. Constrained decoding, not better prompting |
| 25 | Reasoning models | Hidden CoT billed as output; don't say "think step by step" — it already does |
| 26 | Managed Identity | No secret to leak — and disable local auth, or keys still work |
| 27 | Network posture | Public access off, Private Endpoint, **private DNS zone linked to the VNet** |
| 28 | Batch API | ~50% cheaper, 24h target, and a separate quota pool that protects interactive traffic |
| 29 | 4o vs mini | Build on the strong model, downgrade against an eval, route or cascade |
| 30 | Deprecation | Version lives in infra config, never in code. Two dates: deprecation and retirement |

---

## Cross-references

| This question | Goes deeper in |
|---|---|
| Q16, Q26, Q27 | `InterviewBank/06_Responsible_AI_LLMOps.md`; `PerChapter/QA_L12_AzureOpenAI_Services.md` |
| Q17, Q19, Q20, Q28 | `PerChapter/QA_L12_AzureOpenAI_Services.md`; the APIM gateway transcript in `07_ChatHistory/` |
| Q18 | `PerChapter/QA_L17_AzureAIFoundry.md`; `08_Jobs/AscndIntr/PrepPlan/Module01_AzureAIFoundry_HubProject_AgentTools_2026-06-27.md` |
| Q21 | `QA_Detail_A_RAG_Architecture_15Q.md` Q2, Q3, Q15; `PerChapter/QA_L13_RAG_DeepDive.md` |
| Q22, Q24, Q25 | `PerChapter/QA_L15_PromptEngineering.md`; `HighLevelPrep/HLP01_Memory_Tokens_Scaling_Agents.md` §2 |
| Q23 | `InterviewBank/04_Agent_Orchestration.md`; `PerChapter/QA_L16_AIOrchestration_SK_Agents.md` |
| Q29, Q19 | `PerChapter/QA_L36_LLM_Observability_FinOps.md`; `Interview_Bible_77Q_FDE_AI_Lead.md` Q9–Q11 |
| Q30 | `QA_Detail_A_RAG_Architecture_15Q.md` Q13 (index freshness, alias flip pattern) |
