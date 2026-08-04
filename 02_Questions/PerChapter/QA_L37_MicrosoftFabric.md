# Q&A — L37 Microsoft Fabric

*Created 2026-08-03 · Phase 2 of `Consolidation_and_Update_Plan_2026-08-03.md`*

> Q1–Q5 are the five questions the source material posed and never answered. Q6–Q17 are the ones an
> interviewer actually asks next. Answer format follows the house rule: **what it IS → why it works
> that way → a healthcare/JM Family example → the trade-off or when not to use it.**

---

**Q1. What is the Medallion architecture in Fabric?**

Three progressively refined layers over the same data. **Bronze** is raw, append-only, exactly as
received, with ingest metadata (`_ingest_ts`, `_source_file`, `_batch_id`) attached and nothing
edited. **Silver** is the trusted layer: typed, deduplicated, conformed to shared enums, joined to
reference data, SCD2 where history matters, with failures quarantined rather than dropped — one row
per real-world business entity. **Gold** is consumer-shaped: star schemas for BI, aggregates for
operations, wide denormalized rows for an agent to ground on.

The reason Bronze is append-only is **replay**: when Silver logic turns out to be wrong, you fix the
code and re-run from Bronze. If you had cleaned in place, the recovery path is asking the payer to
resend six months of EDI. In the VitalCare prior-auth case, Bronze holds raw EDI 278 files and raw
Document Intelligence OCR JSON (confidence scores included); Silver merges fax-sourced and
EDI-sourced requests into one `silver_pa_request` with a `source_channel` column; Gold produces
`fact_pa_decision` for the dashboard and `gold_pa_case_summary` for the agent.

**Trade-off:** three layers means storing the same information roughly three times and running two
transformation hops. For a small single-source, single-consumer dataset that is pure overhead — go
Bronze→Gold. Medallion earns its cost when you have multiple sources to conform, multiple consumers
wanting different shapes, or a regulatory need to prove what you originally received.

---

**Q2. How do you handle incremental refresh in Fabric?**

"Incremental" means four different mechanisms depending on the layer, and naming the right one is
the answer. **Dataflow Gen2 incremental refresh** — pick a DateTime column, a bucket size and a
look-back window; only recent buckets are re-queried, and it only works properly if the query folds
to the source. **Notebook watermark + `MERGE`** — store the high-water mark in a control table, read
only rows past it, merge on the business key. **Delta Change Data Feed** — read just the rows that
changed between two table versions, which is the efficient path for Silver→Gold aggregates.
**Structured Streaming with a checkpoint** — Spark tracks offsets so each run picks up only new
files or events.

It works this way because each layer has a different notion of "new": a connector has a date filter,
a lakehouse table has a transaction log, a stream has offsets. For semantic models, Import mode uses
date partitions — but **Direct Lake removes the question entirely**, since there is no refresh job
to make incremental.

At JM Family, the ADF equivalent you already run is the watermark pattern — a control table, a max
modified-date, and an upsert. It transfers to Fabric notebooks unchanged.

**Trade-off:** incremental is cheaper but carries state — a watermark, a checkpoint, a partition map
— that can drift, and late-arriving data silently slips past a naive high-water mark. Full reload is
expensive but always correct. Practical rule: incremental where volume forces it (Bronze→Silver),
full rebuild where it is cheap (small Gold aggregates), and always make the merge idempotent so a
re-run is harmless.

---

**Q3. What is the difference between a Lakehouse and a Warehouse in Fabric?**

Both store Delta-Parquet in OneLake — **the storage and format are identical**. The difference is the
write interface and the transaction model. A **Lakehouse** is Spark-first: you write via notebooks,
pipelines or shortcuts, it has a `Files/` area for unstructured content, and its SQL analytics
endpoint is **read-only**. A **Warehouse** is T-SQL-first: full `INSERT`/`UPDATE`/`DELETE`/`MERGE`,
stored procedures, and **multi-table transactions**, but tables only — no unstructured files.

It is built that way because the two audiences want different languages. Fabric's bet is that you
shouldn't have to copy data between them to serve both, so it unified the storage and left the
interfaces separate.

For the prior-auth platform: **Lakehouse for Bronze and Silver**, because the fax PDFs and OCR JSON
need `Files/` and the dedupe/SCD2/embedding logic is Python; **Warehouse for Gold**, because the BI
team writes T-SQL, wants constraints and stored procedures, and needs transactional loads across
fact and dimension tables.

**Trade-off / when not to:** don't pick Warehouse just because "warehouse sounds like production" —
if you have unstructured data or Python transformation logic, the Lakehouse is the correct home and
you'll otherwise end up with a second system beside it. Conversely, don't force a T-SQL team into
Spark notebooks for the sake of purity; the productivity loss is real and the storage is the same
either way. **Choose on team language and whether you need transactional writes — not on
performance.**

---

**Q4. How does Fabric integrate with Azure AI Foundry?**

Four patterns. **(1) Gold → Azure AI Search → RAG:** a notebook chunks and embeds Gold rows into a
`gold_pa_chunks` Delta table, an AI Search indexer builds the index over it, and the Foundry agent
retrieves from it — the `L09`/`L13` pipeline you know, but sourced from governed data instead of a
file share. **(2) SQL tool over the SQL analytics endpoint:** the agent gets a function-calling tool
that runs a constrained query against the Gold warehouse. **(3) Fabric data agent as a tool:** Fabric
exposes a natural-language agent over a lakehouse/warehouse/semantic model that already knows the
schema, and the Foundry orchestrator calls it as a delegated specialist — the `L28` meta-agent
pattern with a Microsoft-supplied domain expert. **(4) The full unstructured chain:** SharePoint/fax
→ Bronze → Document Intelligence → Silver → Gold → index → agent.

The reason this matters is grounding quality. An agent is only as trustworthy as its retrieval
corpus, and Gold is the only place the data is both clean and governed — so the medallion layer is
what separates a demo from something you put in front of a clinician.

**The key design rule, and the trade-off:** *anything with a correct numeric answer goes to SQL;
semantic and policy questions go to retrieval.* "How many prior auths did Payer X deny last
quarter?" is not a RAG question — vector search will return plausible passages and the model will
produce a plausible, wrong number, which is the agentic hallucination failure from `L24`. Don't
reach for RAG when the data is structured and the answer is computable.

---

**Q5. Describe the security model in Fabric — workspaces, RLS, OLS.**

Layered. **Workspace roles** are the coarse control: Admin (full, including access management),
Member (share and publish), Contributor (create/edit items), Viewer (view only — and notably a
Viewer does not automatically get SQL-endpoint access to the underlying data). Below that sit
**item-level permissions** per lakehouse/warehouse/model, and **OneLake data access roles** for
folder-level restriction inside a lakehouse. Then the fine-grained trio: **RLS** filters *rows*
("this clinician sees only their patients"), **OLS** hides *tables or columns entirely* so they
appear not to exist, and **CLS** denies specific *columns* via SQL `GRANT`/`DENY` in the warehouse.

The model is layered because the one-copy principle creates multiple doors into the same data.

**This is the trap, and it is the whole point of the question:** RLS defined on a semantic model
protects **Power BI only**. It does **not** protect the SQL analytics endpoint — which needs its own
`CREATE SECURITY POLICY` — and neither protects someone reading the Delta files directly from
OneLake. Three doors, one copy. In a PHI context you enumerate every door and secure each, or you
close the ones you are not using. Workspace design *is* security design: put Bronze (raw PHI) and
Gold in the same workspace and anyone who can read the dashboard source can read raw faxes.

---

**Q6. Why is Fabric not just Synapse rebranded?**

Three real changes. **Storage:** OneLake is tenant-wide, automatic and mandatory, rather than
bring-your-own ADLS per workspace. **Format:** every engine natively stores Delta-Parquet, so the
same physical file serves Spark, T-SQL and Power BI — where Synapse's dedicated SQL pool kept its own
internal copy. **Billing:** one Capacity Unit meter across all workloads, instead of separate SKUs
for Spark pools, DWUs, pipeline runs and Power BI Premium.

The measurable difference is the **copy count**. A fact table in a classic Synapse estate plausibly
exists three times — lake Parquet for Spark, loaded into the SQL pool, imported into a Power BI model
— with three refresh jobs and three chances to disagree at 8 a.m. Fabric's design goal is one copy,
many engines.

**Where the honesty helps you:** Fabric does not replace Databricks for heavy ML engineering or
advanced Spark tuning, does not painlessly absorb a large ADF estate with self-hosted integration
runtimes, and is not an OLTP store. Saying that unprompted reads as evaluation rather than
enthusiasm.

---

**Q7. What is OneLake, and what is a shortcut?**

**OneLake** is a single tenant-wide data lake, provisioned automatically, that every Fabric item
stores data in — "OneDrive for data." It is ADLS Gen2 underneath with the same DFS API surface, so
`azcopy`, Storage Explorer, the ABFS driver and Databricks all work against it. A **shortcut** is a
pointer to data living elsewhere — ADLS Gen2, S3, GCS, Dataverse, another OneLake workspace, or a
mirrored database — surfaced inside OneLake as if it were local, with **no copy and no sync job**.

Shortcuts exist because otherwise adoption would require migration, and migration programs stall. At
JM Family the practical path is: shortcut the ADLS containers that already exist, leave the data
physically where it is, and start building Silver on top. Adoption becomes incremental.

**Two trade-offs to name:** for external shortcuts you register a **connection with stored
credentials**, so anyone who can read the shortcut effectively uses that identity — a real
delegation concern in a PHI estate. And a shortcut removes the copy, not the distance: a shortcut to
S3 queried from an East US capacity still pays the latency and the egress.

---

**Q8. What is Direct Lake and why is it fast?**

A Power BI storage mode where the VertiPaq engine reads **Delta-Parquet files in OneLake directly**,
paging columns into memory on demand, rather than loading a copy at refresh time (Import) or querying
the source per visual (DirectQuery). You get near-Import performance with live data and **no refresh
job at all**.

It works because Import mode's speed comes from VertiPaq's compressed columnar in-memory layout, and
**V-Ordered** Parquet is already close to that layout — so Direct Lake skips the translation step. As
soon as a Delta transaction commits in Gold, the model reflects it.

For the prior-auth dashboard this removes the 6 a.m. refresh window entirely: the Silver→Gold
notebook finishes and the executive report is current, with no orchestration between them.

**The trade-off you must volunteer — fallback.** When a model hits something unsupported (layered
views, certain security configurations, or exceeding the SKU's memory guardrails) it silently
**falls back to DirectQuery** and gets much slower — users report "the report got slow this week."
Discipline: keep Direct Lake models on plain Delta tables not views, monitor fallback in the
Capacity Metrics app, right-size the SKU, and consider **disabling fallback** so unsupported queries
fail loudly. A silent 10× slowdown is harder to diagnose than an error.

---

**Q9. Dataflow Gen2, pipeline, or notebook — how do you choose?**

**Pipeline** = orchestration: control flow, scheduling, parameters, retries, Copy activity. It is ADF
inside Fabric. **Dataflow Gen2** = low-code transformation in Power Query M with 300+ connectors,
built for analysts. **Notebook** = code-first Spark for real transformation logic.

The production pattern is **pipeline orchestrates, notebook transforms, dataflow only where an
analyst owns the logic** — because the three differ on cost, testability and reviewability. Dataflow
Gen2 is generally the most CU-expensive way to move a given volume; you cannot meaningfully
unit-test a Power Query mashup, whereas notebook logic imports as a module and gets a pytest suite;
and a notebook diff is readable in a PR while a Dataflow diff is a wall of generated M.

For regulated healthcare data, *"how do you test your transformations?"* has a real answer only on
the notebook path — which is why Silver logic belongs there.

**When Dataflows genuinely win:** an analyst owns a reference spreadsheet or a niche SaaS connector
and needs it in the lakehouse weekly without waiting on the engineering backlog. Scope the tool,
don't dismiss it. Also worth naming: **Mirroring** replicates an operational database (Azure SQL,
Cosmos DB, Snowflake) into OneLake as Delta with no pipeline to author — often the better answer to
"get our Cosmos collection into the lake."

---

**Q10. Explain Capacity Units, bursting, smoothing and throttling.**

A **Capacity Unit** is Fabric's single abstract compute meter — every Spark job, warehouse query,
dataflow refresh and Direct Lake load draws from the same pool. **Bursting** lets one job temporarily
exceed the capacity's baseline so heavy work finishes fast. **Smoothing** then spreads that consumed
CU over a following window: roughly 24 hours for background operations (pipelines, refreshes),
minutes for interactive ones (a user clicking a report).

It works this way so short spikes are absorbed instead of failing. **But smoothing defers usage, it
does not forgive it** — sustained over-consumption accumulates as **carry-forward debt** and the
capacity throttles in stages: interactive delay → interactive rejection → background rejection.
Note the order: user-facing degradation comes *first*, deliberately, so the problem is visible.

The failure story to tell: a data-science team runs a large exploratory Spark job Friday afternoon on
the shared production capacity. It bursts, succeeds, everyone goes home. Consumption smooths across
the weekend, Monday's scheduled refreshes stack on top of the carry-forward, and by 9 a.m. the
executive dashboards are throttled. Nobody did anything obviously wrong.

**Mitigation and trade-off:** separate capacities for engineering and BI (costs more, but isolation
is the point), Spark autoscale billing so bursty jobs bill serverless, and the **Fabric Capacity
Metrics app** to see CU by item and the carry-forward timeline. Over-provisioning one big shared
capacity is usually more expensive *and* less reliable than two right-sized ones.

---

**Q11. How would you cut Fabric cost on a real project?**

**Pause non-production capacities on a schedule** — F-SKUs are Azure resources billed per second and
pausable, so a dev capacity paused outside working hours is roughly a 70% saving for zero
architectural effort. Automate it with a runbook, Logic App or Function against the capacity API.
Then: right-size the SKU from Capacity Metrics rather than a guess, separate engineering from BI
capacity, prefer notebooks over Dataflows Gen2 for volume, disable **V-Order** where nothing reads
via Direct Lake (typically Bronze), run `OPTIMIZE`/`VACUUM` table maintenance, and watch Direct Lake
fallback.

The V-Order point is the nuanced one: V-Order costs extra write time and CU to buy read speed. Right
for a Gold table feeding dashboards; wasted on a high-volume Bronze ingest no semantic model will
ever touch.

**The catch on pausing:** while a capacity is paused everything in its workspaces is unavailable —
reports don't render, scheduled refreshes fail. Pause dev and test freely; pause production only if
you truly have no overnight consumers, and remember batch pipelines *are* consumers. Also note
**P-SKUs (legacy Power BI Premium) cannot be paused** — only Azure F-SKUs can.

---

**Q12. Your Gold table has row-level security. Does the agent respect it?**

**Only if you designed for it — by default, almost certainly not.** If the agent queries through a
**service principal** with blanket read access, RLS is bypassed entirely: the SP sees every row. And
if the content was **copied into an Azure AI Search index**, the index has no idea RLS ever existed —
chunks are just documents. That second case is the more dangerous one, because the copy silently
discards the security model rather than failing.

Mitigations in order: **(1) identity passthrough** — query on-behalf-of the user so RLS/OLS evaluate
as designed, the right default for the SQL-tool pattern; **(2) security trimming** — store the
permission key (member ID, care-team ID) as a filterable field on every chunk and have the
application inject a **mandatory server-side filter** from the caller's token; **(3) physical
separation** — separate indexes per trust boundary when boundaries are few and stable; **(4) layer
the enforcement**, since semantic-model RLS doesn't cover the SQL endpoint and neither covers direct
OneLake file access.

**The rule that must not bend:** never let the model choose the security filter. A filter the LLM can
influence is not a security control — it is a prompt-injection target (`L18` §18.3).

**Trade-off:** identity passthrough is the strongest option but costs you result caching and
complicates batch/service-to-service scenarios; security trimming scales better but is only as good
as the discipline that keeps the permission key correct on every chunk at index time.

---

**Q13. What is the small-file problem and why does it matter here?**

Frequent micro-batch writes produce thousands of tiny Parquet files. Every read then pays per-file
overhead — open, parse footer, plan — so queries get slow and expensive even though the total data
volume is small. It is the classic lakehouse killer.

It happens because Delta appends files rather than rewriting them; a pipeline running every five
minutes on a modest stream generates ~288 files a day per partition, and it compounds.

Fix: run **`OPTIMIZE`** to compact small files into right-sized ones and **`VACUUM`** to remove files
past the retention window, on a scheduled maintenance job. In a medallion, Bronze is where this bites
hardest because it is append-only and high-frequency.

**Trade-off:** `OPTIMIZE` costs CU and rewrites data, and `VACUUM` **destroys time-travel** beyond
its retention threshold — so setting retention too aggressively removes your ability to roll back or
audit a prior version. In a regulated context, set `VACUUM` retention from the compliance
requirement, not from the storage bill.

---

**Q14. Where does Document Intelligence sit in a Fabric medallion, and what must you not throw away?**

At the **Bronze boundary**. The pipeline copies the raw PDF into `lh_bronze/Files/pa_fax/` untouched,
then a notebook calls Azure AI Document Intelligence and lands the **raw extraction JSON** — including
per-field **confidence scores** — in `bronze_pa_fax_ocr`. The raw PDF stays, because the OCR model
will be upgraded and you will want to re-extract without re-acquiring the source.

The thing people discard and regret is the **confidence scores**. Silver needs them to route
low-confidence extractions to human review (`needs_human_review = true`) rather than silently
promoting a mis-read diagnosis code into a clinical decision. Throwing away confidence at ingest
means the downstream system cannot distinguish a certain extraction from a guess.

This is your JM Family `cog-jma-dev-frm-recognizer` pattern with a governed platform under it.

**Trade-off:** keeping raw PDFs plus raw JSON plus derived tables is real storage cost, and PHI in
Bronze raises the governance bar — which is exactly why layer-per-workspace separation (§4.4) is
worth its overhead in healthcare.

---

**Q15. How do you get Fabric work into source control and out of click-ops?**

**Git integration** binds a workspace to an Azure DevOps or GitHub branch — notebooks, pipelines,
semantic models and lakehouse metadata serialize to the repo, so changes are diffable and
reviewable. **Deployment pipelines** then promote dev → test → prod across workspaces, with
parameterized deployment rules swapping connection strings and data-source paths per stage.

It matters because the default Fabric experience is a browser and a Save button, which in a regulated
environment is an audit problem, not just an engineering preference. This is the same discipline as
`L34` GitOps, applied to a SaaS analytics platform.

**Trade-off:** item serialization quality varies by item type — notebooks diff beautifully,
Dataflows Gen2 and some model artifacts serialize as large generated blobs that are technically
versioned but not meaningfully reviewable. That is a second, independent reason to keep real logic in
notebooks.

---

**Q16. When would you tell a client *not* to adopt Fabric?**

When their centre of gravity is somewhere Fabric doesn't reach. **Heavy ML engineering** on MLflow
with advanced Spark tuning and multi-cloud requirements — Databricks is more mature. **A large ADF
estate** with self-hosted integration runtimes and deep on-prem dependencies — the migration cost
exceeds the benefit until those dependencies shrink. **OLTP workloads** — Fabric is analytics; Cosmos
DB and Azure SQL keep their jobs. **A single small dataset with one consumer** — a lakehouse,
medallion and capacity are overhead you cannot justify against a table in Azure SQL and a Power BI
report.

The general test: Fabric's value comes from *unification* — many sources, many engines, many
consumers, one governed copy. If a client has one source and one consumer, they are buying the
overhead without the benefit.

**Say this deliberately in an interview.** Naming where a Microsoft product loses reads as
architectural judgement, and it is the difference between sounding like a consultant and sounding
like a brochure.

---

**Q17. Give the 60-second version.**

Fabric is Microsoft's SaaS analytics platform, and the architectural idea is **one copy of the
data** — everything lands in OneLake as Delta-Parquet and Spark, T-SQL and Power BI all read that
same copy. That's what makes **Direct Lake** possible: Import-mode speed with live data and no
refresh job, watching for DirectQuery fallback.

Structure it as a **medallion** — Bronze append-only so you can always replay, Silver conformed and
deduped as the trusted layer, Gold shaped per consumer. **Pipelines orchestrate, notebooks
transform, Dataflows only where an analyst owns the logic**, mainly for cost and testability.

Gold is what you **ground agents on**: text questions to an AI Search index built off Gold, anything
numeric to a **SQL tool** against the warehouse, because that's where RAG hallucinates. And copying
Gold into a vector index **detaches it from row-level security**, so you re-attach it with security
trimming enforced server-side.

On cost it's **one CU meter** with bursting and smoothing — forgiving of spikes, but it accumulates
carry-forward debt and throttles interactive users first. So isolate engineering from BI capacity and
pause non-prod on a schedule.

---

## Related

- Lesson: `01_Lessons/Part8_DataPlatform/L37_MicrosoftFabric.md`
- `L09_AzureAISearch` · `L13_RAG_DeepDive` — the index this Gold layer feeds
- `L24_Hallucination_Mitigation` — why numeric questions go to SQL, not retrieval
- `L18_AISolutionArchitecture` §18.3 — the threat model behind Q12
- `L36_LLM_Observability_FinOps` — same FinOps discipline, different meter
