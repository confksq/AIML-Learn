# Module 37 — Microsoft Fabric: OneLake, Lakehouse, Medallion and the AI Data Platform

**Part 8: Data Platform**
*Created: 2026-08-03 · Closes Roadmap gap #6 (Fabric & Data Engineering, ~5%) and `09_ML` tracker gap #73 (Medallion / lakehouse)*

> **Builds on `L06_AzureML`** (Delta/Parquet mentions, AutoML), **`L20_IntegrationPatterns`**
> (ADF, Synapse, Databricks), **`L09_AzureAISearch`** (indexers, hybrid retrieval) and
> **`L17_AzureAIFoundry`**. Those teach the AI layer. **This module teaches the data layer
> underneath it** — where the data actually lives, how it gets clean, and how an agent grounds
> on it.

---

## Why This Module Exists

The coverage check scored this area **~5% — the single largest hole in the library**:

| Sub-topic | Before this module |
|---|---|
| Microsoft Fabric | 1 passing mention in `L17` |
| OneLake | **0 hits** in lessons or questions |
| Lakehouse | 0 — already self-flagged as gap #73 |
| Medallion (Bronze/Silver/Gold) | 0 |
| Dataflows Gen2 | 0 |
| Fabric ↔ Foundry integration | 0 |
| Capacity / CU / cost governance | 0 |

Your own `09_ML/MLEngineer_Coverage_2026-07-26.md:260` prescribed the fix a week ago
("Data-engineering module — ADF/Databricks → Delta medallion → feature store") and it was never
built. This is that module.

**Why it matters for your target roles.** Every "AI Solutions Architect" loop eventually asks
*"where does the data come from?"* You can describe RAG, agents, MCP and evaluation in depth —
and then get stopped by *"how do you keep the index fresh from the source system?"* That question
is a data-platform question. Fabric is Microsoft's answer to it, and it is the layer directly
beneath the Document Intelligence + Azure AI Search RAG work you already do at JM Family.

**One honest framing to carry into the interview:** Fabric is a *packaging* decision, not a new
class of technology. Everything in it existed before — Spark, Delta Lake, Power Query, T-SQL,
Power BI. What changed is that they now share one storage layer, one billing meter and one
permission model. Say that, and you sound like someone who has evaluated it rather than someone
who read the marketing page.

---

## Section 1 — Fabric Architecture and the SaaS Model

### 1.1 The one-line definition

**Microsoft Fabric is a SaaS analytics platform that unifies data engineering, warehousing,
real-time analytics, data science and BI onto a single storage layer (OneLake) and a single
billing unit (Capacity Units).**

Three words carry the weight: **SaaS**, **single storage**, **single billing**.

### 1.2 Why it is not just Synapse rebranded

This is the most common interview probe on Fabric, and the honest answer wins.

| | Synapse Analytics (PaaS) | Fabric (SaaS) |
|---|---|---|
| **Provisioning** | You create a workspace, a Spark pool, a dedicated SQL pool, a storage account, link them | You buy a capacity; everything else already exists |
| **Storage** | Bring your own ADLS Gen2; each engine may keep its own copy | **OneLake — one per tenant, automatic, mandatory** |
| **Format** | Whatever each engine prefers; dedicated SQL pool used a proprietary internal format | **Delta-Parquet everywhere, for every engine** |
| **Billing** | Per-resource SKUs — Spark pool, SQL pool (DWU), pipeline activity runs, storage, each billed separately | **One capacity (CU) shared across every workload** |
| **Power BI** | Separate product, separate Premium capacity | **Same product, same capacity, Direct Lake on the same files** |
| **Tuning surface** | Large — node counts, DWUs, autoscale, pool configs | Small — pick an F-SKU |
| **Git / CI-CD** | Synapse workspace Git integration | Built-in Git integration + deployment pipelines per workspace |

**The real architectural change is the copy count.** In a classic Synapse estate, the same fact
table plausibly exists three times: as Parquet in the lake for Spark, loaded into the dedicated
SQL pool for T-SQL, and imported into a Power BI model for reporting. Three copies, three refresh
jobs, three chances to disagree. Fabric's design goal is **one copy, many engines**.

**What Fabric does not replace.** Be ready to say this — it signals judgement:

- **Databricks** — for heavy ML engineering, MLflow-centric workflows, advanced Spark tuning and
  multi-cloud, Databricks remains stronger. Fabric's Data Science experience is capable but
  younger.
- **Deep hybrid / on-prem-heavy orchestration** — Fabric Data Factory covers most of ADF, but
  large existing ADF estates with self-hosted integration runtimes and complex on-prem
  dependencies do not move for free.
- **Operational (OLTP) workloads** — Fabric is analytics. Cosmos DB and Azure SQL keep their jobs.
  (Fabric SQL database exists for operational-adjacent cases, but it is not a general OLTP
  replacement.)

### 1.3 The object hierarchy

```
Tenant  (one per organization — one OneLake, one Fabric admin portal)
 └── Capacity  (an F-SKU — the compute you pay for; F2 … F2048)
      └── Workspace  (unit of collaboration, permissions, Git integration, deployment)
           └── Item   (Lakehouse, Warehouse, Notebook, Pipeline, Dataflow Gen2,
                       Semantic model, Report, Eventhouse/KQL DB, Eventstream,
                       ML model, Experiment, Data agent …)
```

Two things follow from this shape and both show up in interviews:

1. **The workspace is the security and lifecycle boundary.** It is where roles are granted, where
   Git is wired, and what a deployment pipeline promotes dev → test → prod. So workspace design
   *is* your governance design. A single workspace holding Bronze, Silver and Gold means anyone
   who can read Gold can read raw PHI in Bronze.
2. **A workspace is assigned to exactly one capacity.** That is how cost is attributed. Put the
   data-science team's exploratory Spark in the same capacity as the executive Power BI reports
   and a runaway notebook will throttle the CFO's dashboard. (See §7 on smoothing and throttling.)

### 1.4 The workloads (experiences)

| Experience | What it is | Closest thing you already know |
|---|---|---|
| **Data Factory** | Pipelines + Dataflows Gen2 | ADF — you use this daily at JM Family |
| **Data Engineering** | Lakehouse + Spark notebooks + Spark job definitions | Synapse Spark / Databricks |
| **Data Warehouse** | T-SQL warehouse, full DDL/DML | Synapse dedicated SQL pool / Azure SQL DW |
| **Data Science** | Notebooks, MLflow experiments, model registry | Azure ML (lighter) |
| **Real-Time Intelligence** | Eventstream → Eventhouse (KQL DB) → Activator alerts | Event Hubs + Data Explorer / ADX |
| **Power BI** | Semantic models, reports, dashboards | Power BI, unchanged but now Direct Lake capable |
| **Databases** | Fabric SQL database (operational, mirrored into OneLake) | Azure SQL, lightly |

All seven write to, or read from, the same OneLake.

---

## Section 2 — OneLake

### 2.1 What it is

**OneLake is a single, tenant-wide data lake, automatically provisioned, that every Fabric item
stores its data in.** Microsoft's own analogy is deliberate: *OneDrive for data*. You do not
create it, you cannot opt out of it, and there is exactly one per tenant.

Technically it is **ADLS Gen2** underneath — hierarchical namespace, same DFS API surface. That
detail is practically important: existing ADLS Gen2 tooling works against OneLake. Azure Storage
Explorer, `azcopy`, the ABFS driver, Databricks, and any SDK that speaks ADLS Gen2 can read and
write it, using a path of the shape:

```
abfss://<workspace>@onelake.dfs.fabric.microsoft.com/<item>.Lakehouse/Tables/<table>
abfss://<workspace>@onelake.dfs.fabric.microsoft.com/<item>.Lakehouse/Files/<path>
```

Every lakehouse has exactly two top-level areas:

- **`Tables/`** — managed Delta tables. These appear automatically in the SQL analytics endpoint
  and are queryable as tables.
- **`Files/`** — unmanaged files of any kind: raw JSON, CSV, PDFs, images, EDI drops.

> **The single most useful mental model:** `Files/` is your landing zone, `Tables/` is your
> curated model. Bronze usually starts in `Files/`, and everything from Silver onward lives in
> `Tables/`.

### 2.2 The one-copy principle

Every Fabric engine reads and writes **Delta-Parquet in OneLake**. Not "can export to" — *natively
stores as*.

The consequence, and this is the sentence to say out loud in an interview:

> *A Warehouse table and a Lakehouse table are the same file format in the same storage account.
> A Spark notebook, a T-SQL query and a Power BI Direct Lake model are three different readers of
> one physical copy — so there is no ETL between them and nothing to keep in sync.*

Contrast with the classic estate: Spark writes Parquet → a COPY INTO loads the SQL pool → a Power
BI import refresh copies it again. Three copies, two jobs, a refresh window, and a real chance the
dashboard disagrees with the warehouse at 8 a.m.

### 2.3 Shortcuts — the feature that makes adoption realistic

A **shortcut** is a pointer to data that lives somewhere else, presented inside OneLake as though
it were local. **No copy, no scheduled sync, no duplicate storage bill.**

Shortcut targets include:
- **ADLS Gen2** (your existing lake)
- **Amazon S3**, **Google Cloud Storage**, **S3-compatible**
- **Dataverse**
- **Another OneLake workspace** — the cross-team sharing mechanism
- **Mirrored databases** (Azure SQL, Cosmos DB, Snowflake — near-real-time replicas landed as
  Delta in OneLake, no pipeline to author)

**Why this matters at JM Family specifically.** You already have data in ADLS, in SharePoint-fed
processes, in Cosmos DB, and moving through ADF. The Fabric adoption story is *not* "migrate
everything." It is: create shortcuts over the ADLS containers that already exist, leave the data
physically where it is, and start building Silver/Gold on top. Migration becomes incremental
instead of a program.

**Two things to know about shortcut behaviour** (both are interview probes):

1. **Permissions.** For internal (OneLake-to-OneLake) shortcuts, access can be evaluated against
   the calling user's identity — a shortcut is not a permission bypass by default. For external
   shortcuts (S3, ADLS), you register a **connection with stored credentials**, and then anyone who
   can read the shortcut effectively uses those credentials. That delegation is exactly the kind of
   detail that turns into a security finding, so state it deliberately.
2. **Physics still applies.** A shortcut to S3 queried from a Fabric capacity in East US pays the
   latency and the egress. Shortcuts remove the copy, not the distance.

### 2.4 Delta-Parquet and V-Order

- **Parquet** — columnar file format: compressed, column-pruned, great for scans.
- **Delta Lake** — a transaction log (`_delta_log`) layered over Parquet files, adding ACID
  transactions, time travel, schema enforcement/evolution, MERGE and concurrent-writer safety.
  This is what makes a lake behave enough like a warehouse to be trusted.
- **V-Order** — a Microsoft write-time optimization (sorting, row-group tuning, encoding) applied
  to Parquet so that the Power BI VertiPaq engine can load it with minimal work. **V-Order is what
  makes Direct Lake fast.** It stays a valid, readable Parquet file for every other engine.

**The V-Order trade-off — worth knowing, because it is a real production decision.** V-Order costs
extra time and CU on *write* to buy speed on *read*. For a Gold table feeding executive dashboards,
that trade is obviously correct. For a high-volume Bronze ingest that no semantic model will ever
touch, it is wasted spend, and it can be disabled at the session, table or warehouse level.
Default behaviour has shifted across releases, so verify the current default in Microsoft Learn
before quoting one — but the *reasoning* above is stable and is what an interviewer is testing.

---

## Section 3 — Lakehouse vs Warehouse, and Direct Lake

### 3.1 The two items

| | **Lakehouse** | **Warehouse** |
|---|---|---|
| Primary engine | Spark (notebooks, job definitions) | T-SQL (Polaris distributed SQL engine) |
| Write path | Spark, pipelines, dataflows, shortcuts | **T-SQL DML** — `INSERT`/`UPDATE`/`DELETE`/`MERGE`, stored procedures |
| SQL surface | **SQL analytics endpoint — read-only** | Full read/write T-SQL |
| Transactions | Delta ACID, single-table | **Multi-table transactions** (`BEGIN TRAN` across tables) |
| Unstructured data | **Yes — `Files/` holds anything** | No — tables only |
| Schema handling | Flexible; schema-on-read possible in `Files/` | Enforced, defined up front |
| Natural owner | Data engineers, Python/Scala skills | SQL developers, BI teams |
| Storage | Delta-Parquet in OneLake | **Delta-Parquet in OneLake — the same** |

Both store the same format in the same lake. **The difference is the write interface and the
transaction model, not the data.** And because it is the same lake, a Warehouse query can join
across to a Lakehouse table, and vice versa, without moving anything.

### 3.2 Decision criteria

Choose **Lakehouse** when:
- The data includes unstructured or semi-structured content (PDFs, faxes, images, JSON, EDI).
- Your transformation logic is Python/Spark, or needs libraries SQL cannot express — including
  **chunking and embedding for RAG** (§6).
- The team is data engineers.
- You want everything version-controlled as notebook code.

Choose **Warehouse** when:
- The team is T-SQL-native and you want them productive without learning Spark.
- You need multi-table transactional writes, or genuine `UPDATE`/`DELETE` semantics as a first-class
  operation.
- You are migrating an existing SQL DW / Synapse dedicated pool and want the stored procedures to
  mostly survive.
- The consumer is BI and the model is dimensional.

**A common and defensible production answer:** Lakehouse for Bronze and Silver (ingest and
transform, where flexibility and Python matter), Warehouse for Gold (serve, where SQL skills,
constraints and dimensional modelling matter). One estate, both items, no copy between them.

> **If you say only one thing about this in an interview:** *"They're the same storage and the same
> format — I pick based on the team's language and whether I need transactional T-SQL writes,
> not based on performance."*

### 3.3 Direct Lake — the headline feature

Power BI has three storage modes. Knowing all three, and where Direct Lake fits, is the highest-value
Fabric detail you can carry.

| Mode | How it works | Freshness | Speed | Cost |
|---|---|---|---|---|
| **Import** | Data copied into the VertiPaq in-memory engine at refresh time | **Stale** — as of last refresh | **Fastest** | Refresh compute + memory; refresh windows |
| **DirectQuery** | Every visual generates a query to the source at render time | **Live** | **Slowest** — network + source latency per interaction | Load on the source system |
| **Direct Lake** | VertiPaq reads the **Delta-Parquet files in OneLake directly**, paging columns into memory on demand | **Live** — sees committed Delta writes | **Near-Import** | No refresh job at all |

**Why it works.** Import mode's speed comes from VertiPaq's compressed columnar in-memory format —
and V-Ordered Parquet is already close to that layout. So Direct Lake skips the translation: it
loads (transcodes) the columns it needs straight from OneLake, on first access, and caches them.
No copy, no refresh schedule, and the model reflects the lake as soon as the Delta transaction
commits.

**The catch you must mention — fallback.** Direct Lake cannot serve every scenario. When the model
hits something unsupported (certain view/complex-SQL constructs, some security configurations, or
a model exceeding the capacity's memory guardrails for the SKU), it **falls back to DirectQuery**
for that query — silently, and much slower. Users experience it as "the report got slow this
week." Production discipline is therefore:

1. Keep Direct Lake models on **plain Delta tables**, not layered views.
2. Watch fallback in the **Capacity Metrics app** and in the semantic model's refresh/query telemetry.
3. Set the fallback behaviour deliberately — you can disable fallback so unsupported queries fail
   loudly instead of degrading quietly. **Failing loudly is usually the right call in production**,
   because a silent 10× slowdown is harder to diagnose than an error.
4. Right-size the SKU: Direct Lake memory limits scale with the F-SKU.

**Say this in an interview:** *"Direct Lake gives you Import-mode performance with DirectQuery
freshness, by reading V-Ordered Delta files straight into VertiPaq — the thing to watch for is
DirectQuery fallback, which is a silent performance cliff."* That one sentence covers the feature
and its failure mode.

---

## Section 4 — The Medallion Architecture, Worked Through

Bronze/Silver/Gold is not a Fabric invention (it comes from Databricks/Delta practice), but Fabric
is where you will implement it. The pattern is **progressive refinement with replayability**.

Everyone can recite "raw / cleaned / aggregated." Interviews separate candidates on **what
transformation actually happens at each hop, and why the boundary sits where it does.**

### 4.0 The worked example

Use the domain you already have material in — **VitalCare prior authorization** (`05-VitalCare-AI-Platform/`).
Three sources:

1. **EDI 278** prior-auth request/response transactions from payers (structured, high volume)
2. **Faxed / scanned PA forms** arriving as PDFs (unstructured — the Document Intelligence path you
   run at JM Family)
3. **Member and provider reference data** from an operational system (slowly changing)

### 4.1 Bronze — land it exactly as it arrived

**Rule: Bronze is append-only and never edited. It is the system of replay.**

| Aspect | Bronze behaviour |
|---|---|
| Fidelity | Byte-for-byte as received. Do not fix, do not reshape. |
| Schema | Minimal — parse the envelope, keep the payload |
| Writes | Append only. **No updates, no deletes.** |
| Partitioning | By ingest date, almost always |
| Retention | Long — this is what you re-run from |

What lands:

```
Lakehouse: lh_bronze
  Files/
    edi278/ingest_date=2026-08-03/batch_0417.x12        ← raw EDI, untouched
    pa_fax/ingest_date=2026-08-03/fax_88213.pdf         ← raw PDF, untouched
  Tables/
    bronze_edi278_raw     (segment-level rows + metadata)
    bronze_pa_fax_ocr     (Document Intelligence JSON output + metadata)
```

**Every Bronze row carries ingest metadata.** This is the part people skip and then regret:

```python
from pyspark.sql import functions as F

bronze = (
    raw_df
    .withColumn("_ingest_ts",   F.current_timestamp())
    .withColumn("_source_file", F.input_file_name())
    .withColumn("_batch_id",    F.lit(batch_id))
    .withColumn("_source_system", F.lit("payer_edi_sftp"))
)
bronze.write.format("delta").mode("append").save(bronze_path)
```

**Why append-only matters — the argument to make.** If Silver logic has a bug — a mis-parsed
diagnosis code, a wrong date format — you fix the code and **re-run Silver from Bronze**. If Bronze
had been "cleaned in place," the original is gone and the only recovery is asking the payer to
resend six months of transactions. Bronze is cheap storage buying you the right to be wrong.

For the fax path, note where the AI sits: **Document Intelligence runs at the Bronze boundary**,
and its raw JSON output — confidence scores included — is what lands. Do not throw away confidence
scores at ingest; Silver needs them to route low-confidence extractions to human review.

### 4.2 Silver — make it correct, one row per real-world thing

**Rule: Silver is the trusted, conformed, business-entity layer. If a number is wrong here, it is
wrong everywhere downstream.**

What actually happens at this hop:

| Transformation | Concretely, in this example |
|---|---|
| **Type casting** | EDI strings → `date`, `decimal`, typed enums |
| **Deduplication** | Payer resends the same 278; dedupe on `(transaction_control_number, payer_id)` keeping latest by `_ingest_ts` |
| **Validation / quarantine** | Rows failing rules go to `silver_pa_request_quarantine`, **not silently dropped** |
| **Conforming** | Payer A's `"APPR"`, Payer B's `"A1"`, the fax form's `"Approved"` → one `status = 'APPROVED'` enum |
| **Joins to reference** | Attach member and provider dimensions; resolve NPI |
| **SCD Type 2** | Member eligibility changes over time — keep history with `valid_from`/`valid_to` |
| **Unstructured merge** | Fax-derived requests and EDI-derived requests land in the **same** `silver_pa_request` table, distinguished by `source_channel` |
| **PHI handling** | Tokenize or mask direct identifiers not needed downstream; this is the layer where the policy is enforced |
| **Confidence routing** | OCR fields below the confidence threshold → `needs_human_review = true` |

```sql
-- Silver merge: idempotent, so re-running is safe
MERGE INTO silver_pa_request AS tgt
USING staged_pa_request AS src
  ON tgt.request_id = src.request_id
WHEN MATCHED AND src._ingest_ts > tgt._ingest_ts THEN UPDATE SET *
WHEN NOT MATCHED THEN INSERT *
```

**The design rule for the Bronze→Silver boundary:** Silver must be reproducible from Bronze by
re-running code alone. If a Silver value depends on something not in Bronze — a manual correction,
a lookup that has since changed — you have broken replayability, and that is the bug to catch in
review.

**Idempotency is the operational requirement.** Pipelines get re-run: retries, backfills, a fixed
bug. `MERGE` on a stable business key makes a re-run harmless. `INSERT` makes it a data-quality
incident.

### 4.3 Gold — shape it for how it will be consumed

**Rule: Gold is consumer-shaped. There is usually more than one Gold table for the same Silver
data, because there is more than one consumer.**

| Consumer | Gold shape |
|---|---|
| Power BI executive dashboard | Star schema — `fact_pa_decision` + `dim_member`, `dim_provider`, `dim_date`, pre-aggregated |
| Operations turnaround report | Daily aggregate: approvals, denials, median decision hours, by payer |
| **An AI agent (§6)** | Denormalized, wide, text-rich rows — or a table of chunks + embeddings |
| A downstream ML model | Feature table at a defined grain, point-in-time correct |

```
Warehouse: wh_gold
  fact_pa_decision          (grain: one row per decision event)
  dim_member                (SCD2)
  dim_provider
  dim_date
  agg_pa_turnaround_daily
  gold_pa_case_summary      ← wide, text-rich, denormalized, for agent grounding
```

**Point that separates senior answers:** Gold is where you *deliberately* denormalize and duplicate.
Silver is normalized and correct; Gold is redundant and fast. Duplicating a metric across two Gold
tables is fine — provided both derive from the same Silver source, so they cannot disagree.

**The "one row per decision event" discipline.** Always state the **grain** of a fact table. Most
production reporting bugs are grain bugs: a join fans out a fact table and every measure doubles.
Interviewers who have shipped BI will notice you naming the grain unprompted.

### 4.4 How to lay this out in Fabric

Three viable layouts — the choice is a **governance** choice, not a technical one:

| Layout | When |
|---|---|
| One workspace, one lakehouse, three schemas | Small team, one trust boundary, simplest |
| One workspace, three lakehouses (`lh_bronze`/`lh_silver`/`lh_gold`) | Clear separation, still one permission boundary |
| **Three workspaces, one per layer** | **Different people should see different layers** |

**For the healthcare case, the third.** Bronze holds raw PHI at full fidelity. Analysts need Gold.
Since the workspace is the permission boundary, layer-per-workspace is what actually stops an
analyst reading raw fax PDFs. Cross-workspace access is then via **OneLake shortcuts**, granted
deliberately.

### 4.5 What each hop runs on

| Hop | Engine | Why |
|---|---|---|
| Source → Bronze | **Pipeline** (Copy activity) or Mirroring | Movement, not transformation |
| Fax → Bronze | Pipeline → **Azure AI Document Intelligence** → notebook writes JSON | The AI call sits at the boundary |
| Bronze → Silver | **Spark notebook** | Real logic: dedupe, SCD2, conform, quarantine |
| Silver → Gold | **Notebook or T-SQL stored procedure** | Aggregation — SQL is often the better fit |
| Gold → BI | **Direct Lake semantic model** | No refresh job |
| Gold → Agent | AI Search indexer, or a SQL tool (§6) | — |

---

## Section 5 — Dataflows Gen2 vs Pipelines vs Notebooks

The three ways to move and transform data. Choosing between them is the most frequent practical
decision in a Fabric project, and there is a decision table an interviewer wants to hear.

### 5.1 What each one is

**Pipelines** — orchestration. Activities, control flow (`ForEach`, `If`, `Until`), parameters,
scheduling, dependency chaining, retries, failure branches. This is **ADF, inside Fabric** — the
authoring experience you already know from JM Family. Pipelines *move* data (Copy activity) and
*invoke* other things. They are not where transformation logic belongs.

**Dataflows Gen2** — low-code transformation via **Power Query (M)**. 300+ connectors, visual step
editor, incremental refresh, and an explicit output destination (Lakehouse, Warehouse, KQL DB, Azure
SQL). Built for analysts who know Excel/Power Query, not Spark.

**Notebooks** — code-first Spark (PySpark, Spark SQL, Scala, R) or Python. Full library access,
arbitrary logic, unit-testable, diffable in Git.

### 5.2 The decision table

| | **Pipeline** | **Dataflow Gen2** | **Notebook** |
|---|---|---|---|
| **Purpose** | Orchestrate & move | Transform, low-code | Transform, code |
| **Language** | Config / JSON | Power Query M | PySpark, Spark SQL, Python |
| **Author** | Data engineer | **Analyst / citizen dev** | Data engineer |
| **Scale** | N/A (moves data) | Small–medium | **Large — distributed Spark** |
| **CU cost profile** | Low | **High per unit of data** | Efficient at scale; startup cost per session |
| **Testability** | Poor | **Poor — no real unit testing** | **Good — pytest, importable modules** |
| **Git diff quality** | Verbose JSON | **Poor — M script in a large blob** | **Excellent — plain code** |
| **Connectors** | Many | **Most — 300+** | Fewer built-in; code your own |
| **Complex logic** | No | Limited | **Yes — anything** |
| **Best at** | Scheduling, control flow, Copy | Analyst-owned small ingest, quick shaping | Silver/Gold logic, ML, embeddings |

### 5.3 The production pattern

> **Pipeline orchestrates. Notebook transforms. Dataflow only where an analyst owns the logic.**

```
Pipeline: pl_pa_daily
 ├── Copy activity        : payer SFTP  → lh_bronze/Files/edi278/
 ├── Copy activity        : fax share   → lh_bronze/Files/pa_fax/
 ├── Notebook             : nb_doc_intelligence_extract   (PDF → OCR JSON → Bronze)
 ├── Notebook             : nb_bronze_to_silver           (dedupe, conform, SCD2, quarantine)
 ├── Notebook             : nb_silver_to_gold             (facts + dims + agent table)
 ├── Notebook             : nb_embed_and_index            (chunk + embed → Gold; §6)
 └── On failure           : Teams/email alert + write to run-log table
```

**Why not Dataflows for the heavy lifting?** Three reasons, and they are the ones to give:

1. **Cost.** Dataflow Gen2 is generally the most CU-expensive way to move a given volume. At scale
   the same work in a notebook costs materially less.
2. **Testability.** You cannot meaningfully unit-test a Power Query mashup. Notebook logic imports
   as a module and gets a pytest suite. For regulated healthcare data, "how do you test your
   transformations?" has a real answer only in the notebook path.
3. **Reviewability.** A notebook diff in a PR is readable. A Dataflow diff is a wall of generated M.

**Where Dataflows genuinely win:** an analyst who owns a reference spreadsheet or a niche SaaS
connector and needs it in the lakehouse weekly, without waiting on the engineering backlog. That is
a real and valuable use case — do not dismiss the tool, scope it.

### 5.4 Two adjacent options worth naming

- **Mirroring** — near-real-time replication of an operational database (Azure SQL, Cosmos DB,
  Snowflake, PostgreSQL) into OneLake as Delta, with **no pipeline to author and no CU cost for the
  replication itself**. If the ask is "get our Cosmos DB collection into the lake," mirroring often
  beats writing a pipeline. Naming this shows current knowledge.
- **Copy job** — a simplified, managed copy experience (full and incremental) sitting between a raw
  Copy activity and a full pipeline.

### 5.5 Incremental processing — the four mechanisms

"How do you handle incremental refresh?" is a stock Fabric question, and the honest answer is that
**"incremental" means four different mechanisms depending on the layer.** Naming which one you mean
is the answer.

| Layer | Mechanism | How it works |
|---|---|---|
| **Dataflow Gen2** | **Incremental refresh** | Configure a `DateTime` column, a bucket size (day/month) and a look-back window. Only recent buckets are re-queried; older ones are left alone. Query folding to the source is what makes it actually incremental rather than "filter after loading everything." |
| **Notebook / Silver** | **Watermark + `MERGE`** | Track the high-water mark (max `_ingest_ts` or a source LSN) in a control table, read only rows beyond it, `MERGE` on the business key. **Idempotent** — safe to re-run. |
| **Delta** | **Change Data Feed (CDF)** | Enable `delta.enableChangeDataFeed`; read only the inserts/updates/deletes a table saw between two versions, instead of diffing the whole table. Ideal for Silver→Gold when Gold is an aggregate. |
| **Streaming** | **Structured Streaming + checkpoint** | Spark tracks offsets in a checkpoint location, so each micro-batch processes only new files/events. `Trigger.AvailableNow` gives batch-style runs with streaming's bookkeeping. |
| **Semantic model** | **Incremental refresh (partitions)** | Import-mode models partition by date and refresh only recent partitions. **Direct Lake makes this moot** — there is no refresh to make incremental. |

**The trade-off to state:** incremental is cheaper but carries state (a watermark, a checkpoint, a
partition map) that can drift or corrupt. Full reload is expensive but always correct. In the
prior-auth case, Bronze→Silver is incremental by watermark; the daily aggregate in Gold is a small
full rebuild, because rebuilding a summary table is cheap and removes a whole class of "the numbers
drifted" incidents. **Be incremental where volume forces it, full-reload where you can afford
correctness.**

### 5.6 Real-Time Intelligence, briefly

For streaming rather than batch, the chain is:

```
Eventstream  →  Eventhouse (KQL database)  →  Activator (alerts / triggers)
   ingest         store + KQL query              act on a condition
```

The mapping to what you know: **Eventstream ≈ Event Hubs/Stream Analytics ingest**, **Eventhouse ≈
Azure Data Explorer (ADX)**, queried with **KQL** — the same language as Log Analytics, which you
have already met in `L31`/`L36` observability. Eventhouse data can be surfaced into OneLake in
Delta form, so it joins the same medallion story. In the prior-auth example, this is where you would
put "alert when a payer's decision latency exceeds SLA" rather than waiting for tomorrow's batch.

---

## Section 6 — Fabric ↔ Azure AI Foundry: Grounding Agents on OneLake

**This is the section that matters most for your target roles.** It is the joint between everything
in Parts 3–5 (RAG, agents, MCP) and everything above.

### 6.1 The framing

An agent is only as good as what it can retrieve. In a real enterprise, the answer to *"what should
it retrieve from?"* is almost never "a folder of PDFs someone uploaded." It is **the Gold layer** —
because Gold is the only place the data is both clean and governed.

> **Say it this way:** *"Fabric is where I make the data trustworthy; Foundry is where I make it
> answerable. The medallion architecture is what stands between a demo and a system I'd put in front
> of a clinician."*

### 6.2 Four integration patterns

**Pattern 1 — Gold → Azure AI Search → RAG (unstructured/text questions)**

```
lh_gold.gold_pa_case_summary  (wide, text-rich rows)
      ↓  chunk + embed in a Spark notebook
gold_pa_chunks (chunk_text, embedding, case_id, source_ref, effective_date)
      ↓  Azure AI Search indexer (over OneLake / ADLS shortcut)
srch-index  →  hybrid + semantic retrieval  →  Foundry agent
```

This is exactly the `L09` + `L13` pipeline you already know — the change is that its **source is a
governed Gold table instead of a raw file share.** Freshness becomes a data-platform property:
the indexer runs after the Gold notebook, so index freshness is a pipeline dependency, not a hope.

Embedding in a notebook is a natural fit — it is Python, it is distributed, and it belongs with the
rest of your transformation code:

```python
# nb_embed_and_index — Gold chunk + embed
chunks = (spark.table("gold_pa_case_summary")
    .transform(chunk_by_section, max_tokens=512, overlap=64))

embedded = chunks.mapInPandas(embed_batch, schema=EMBED_SCHEMA)   # Azure OpenAI embeddings

(embedded.write.format("delta").mode("overwrite")
    .save(f"{gold_path}/gold_pa_chunks"))
```

**Pattern 2 — Direct SQL tool over the SQL analytics endpoint (structured questions)**

This is the one people miss, and it is the stronger architectural answer half the time.

*"How many prior authorizations did Payer X deny last quarter?"* is **not a RAG question.** Vector
search over text chunks will retrieve plausible passages and the model will produce a plausible
number — which is the textbook agentic hallucination failure from `L24`. The correct design gives
the agent a **tool that runs SQL against the Gold warehouse** and returns an exact count.

```
Foundry agent
  ├── tool: search_clinical_guidelines(query)  → AI Search   (unstructured — "what does policy say?")
  └── tool: query_pa_metrics(sql|params)       → SQL endpoint (structured — "how many, what trend?")
```

**The rule:** *aggregations, counts, and anything with a correct numeric answer go to SQL. Semantic
and policy questions go to retrieval.* An agent that can do both, and a router that knows which is
which, is the design worth describing. (This is the same local-vs-global instinct as GraphRAG's two
query modes, and the same "don't retrieve what you can compute" point as `L23` CAG vs RAG.)

Guard the SQL tool properly: parameterized or schema-constrained queries, a read-only identity, a
row limit, a query timeout, and an allow-list of tables. A free-form text-to-SQL tool pointed at a
PHI warehouse is a finding waiting to happen.

**Pattern 3 — Fabric data agent as a tool**

Fabric can expose a **data agent** over a lakehouse, warehouse or semantic model — a natural-language
interface that generates queries against that specific data, grounded in the model's schema and
whatever instructions you give it. A Foundry agent can then call the Fabric data agent as one of
its tools, delegating "questions about our analytics data" to something that already knows the
schema.

This is the multi-agent delegation pattern from `L28` (meta-agent hierarchies) with a Microsoft-
supplied specialist: the orchestrator owns the conversation and the routing; the Fabric data agent
owns the data domain.

**Pattern 4 — The unstructured pipeline end to end**

```
SharePoint / fax intake
   → Pipeline Copy → lh_bronze/Files/pa_fax/          (raw PDF preserved)
   → Azure AI Document Intelligence                    (OCR + field extraction, confidence scores)
   → bronze_pa_fax_ocr                                 (raw JSON, nothing discarded)
   → Silver: validate, conform, route low-confidence to human review
   → Gold:   gold_pa_case_summary + gold_pa_chunks
   → AI Search index → Foundry agent → clinician-facing answer with citations
```

This is your JM Family stack — Document Intelligence, AI Search, SharePoint sources — **with a
governed data platform underneath it.** If you can draw this whiteboard diagram end to end, you have
covered the roadmap's Fabric area and its agentic area in one answer.

### 6.3 The governance question interviewers use to separate candidates

> *"Your Gold table has row-level security so a clinician only sees their own patients. Does the
> agent respect it?"*

**Only if you designed for it. By default, almost certainly not.**

- If the agent queries through a **service principal** with blanket read access, **RLS is bypassed**
  — the SP sees every row, and the agent will happily answer about any patient.
- If content was **copied into an Azure AI Search index**, the index has *no idea* RLS ever existed.
  Chunks are just documents. This is the more dangerous case because the copy silently discards the
  security model.

Real mitigations, in the order you should offer them:

1. **Identity passthrough** — query with the user's identity (on-behalf-of), so RLS/OLS evaluate
   as designed. Strongest, and the right default for the SQL-tool pattern.
2. **Security trimming in the index** — store the permission key (member ID, care-team ID, provider
   group) as a filterable field on every chunk, and have the application add a **mandatory,
   server-side** filter derived from the caller's token. Never let the model choose that filter —
   a filter the LLM can influence is not a security control.
3. **Physical separation** — separate indexes or separate Gold tables per trust boundary, when the
   boundaries are few and stable.
4. **Layer the enforcement.** Semantic-model RLS does **not** protect the SQL analytics endpoint,
   and neither protects direct OneLake file access. Three doors — secure all three, or close the
   ones you are not using.

Being able to say *"copying data into a vector index detaches it from its security model, so I
re-attach it with security trimming and enforce the filter server-side"* is a genuinely senior
answer, and it connects directly to the threat-model work in `L18` §18.3.

---

## Section 7 — Governance, Capacity and Cost

### 7.1 Capacity Units and F-SKUs

**Capacity Unit (CU)** is Fabric's single abstract unit of compute. Every operation — a Spark job, a
warehouse query, a dataflow refresh, a Direct Lake model load — consumes CU-seconds from the same
pool. One meter for the whole platform. That is the SaaS bargain: you stop sizing seven services and
start sizing one number.

| SKU | Relative size | Typical use |
|---|---|---|
| **F2** | smallest | Dev/test, learning, tiny workloads |
| **F8 – F32** | small–mid | Departmental production |
| **F64** | the notable step | **Threshold where Power BI free-license users may consume content** — the historic P1 equivalent, and the usual "real production" starting point |
| **F128 – F2048** | large | Enterprise |

Two more things to know:

- **F-SKUs are Azure resources** — bought in the Azure portal, billable per second, **pausable**.
  **P-SKUs** are the legacy Power BI Premium capacities, bought via Microsoft 365, and **not**
  pausable. The F64 licensing threshold is the detail people get wrong.
- **Autoscale billing for Spark** can be enabled so Spark jobs bill serverless rather than
  competing for the shared capacity pool — useful when bursty engineering work is starving
  interactive BI.

### 7.2 Pause and resume — the biggest cost lever

An F-SKU can be **paused**, and while paused **you are not billed for compute** (OneLake storage is
billed separately and continues).

A dev capacity paused outside working hours is roughly a 70% saving on that capacity for zero
architectural effort. Automate it — an Azure Automation runbook, a Logic App, or a scheduled Azure
Function against the Fabric capacity API.

**The catch to state:** while a capacity is paused, everything in its workspaces is unavailable —
reports will not render, scheduled refreshes fail. So pause dev and test freely; pause production
only if you genuinely have no overnight consumers, and remember that batch pipelines are consumers.

### 7.3 Smoothing, bursting and throttling — the mechanic to actually understand

This is the most-asked Fabric operations question and the most-misunderstood feature.

- **Bursting** — a single job may temporarily consume *more* CU than the capacity's baseline, so a
  heavy Spark job finishes fast instead of being clamped to the SKU size.
- **Smoothing** — the CU consumed is then spread over a following time window rather than charged
  instantaneously. **Background operations** (pipelines, refreshes, notebook jobs) smooth over a
  long window — roughly 24 hours. **Interactive operations** (a user clicking a report) smooth over
  minutes.

Together these mean short spikes are absorbed instead of failing. **But smoothing defers usage, it
does not forgive it.** If you consistently consume more than the capacity provides, the deferred
usage accumulates as **carry-forward debt**, and the capacity begins to **throttle** in stages:

| Stage | Effect |
|---|---|
| **Interactive delay** | User-triggered operations are delayed — reports feel sluggish |
| **Interactive rejection** | User-triggered operations are **rejected** — reports fail to render |
| **Background rejection** | Scheduled jobs are rejected — pipelines and refreshes stop running |

The order matters and it is deliberately user-visible first. **The practical failure story:** a
data-science team runs a large exploratory Spark job on Friday afternoon on the shared production
capacity. It bursts, it succeeds, everyone goes home. The consumption smooths across the weekend,
Monday's scheduled refreshes stack on top of the carry-forward, and by Monday 9 a.m. the executive
dashboards are throttled. **Nobody did anything obviously wrong** — which is exactly why capacity
isolation is a design decision, not an afterthought.

**Mitigations:** separate capacities for engineering and BI; Spark autoscale billing; the **Fabric
Capacity Metrics app** (the tool to name — it shows CU by item, by operation, and the carry-forward
timeline); and surge protection to cap background consumption.

### 7.4 Security and governance model

**Workspace roles** (coarse — this is the main control):

| Role | Can |
|---|---|
| **Admin** | Everything, including managing access and deleting the workspace |
| **Member** | Add others, publish, share |
| **Contributor** | Create and edit items |
| **Viewer** | **View only — cannot see underlying data via SQL endpoint unless separately granted** |

**Item-level permissions** — finer, per lakehouse/warehouse/model, including whether the grantee
gets the SQL endpoint, the OneLake files, or only the semantic model.

**OneLake data access roles** — folder-level security within a lakehouse, so a Bronze folder can be
restricted even inside a workspace someone can otherwise read.

**RLS, OLS and CLS** — the trio the stub asked about and never explained:

| | What it hides | Where it is enforced |
|---|---|---|
| **RLS** (row-level) | **Rows** — "this clinician sees only their patients" | Semantic model (DAX filter) **or** SQL endpoint (`CREATE SECURITY POLICY`) — **these are different implementations** |
| **OLS** (object-level) | **Tables or columns entirely** — the object does not appear to exist | Semantic model |
| **CLS** (column-level) | **Columns** — table visible, column denied | Warehouse / SQL, via `GRANT`/`DENY` |

**The trap, again, because it is the highest-value thing in this section:** RLS defined on the
semantic model protects **Power BI**, not the SQL analytics endpoint, and neither protects someone
reading the Delta files directly from OneLake. Multiple doors into one copy of the data is the price
of the one-copy principle. Enumerate the doors and secure each.

**Wider governance surface:**

- **Domains** — group workspaces by business area (Claims, Underwriting, Clinical) for federated
  ownership and discovery.
- **Sensitivity labels** — Microsoft Purview Information Protection labels applied to Fabric items;
  they **flow downstream**, including into exported files, which is a genuinely strong story for
  PHI/PII.
- **Lineage view** — visual item-to-item dependency graph. This is your answer to *"if this source
  changes, what breaks?"* and to audit questions.
- **Endorsement** — mark items as **Promoted** or **Certified** so consumers can tell the governed
  Gold model from someone's experiment.
- **Git integration + deployment pipelines** — workspace bound to an Azure DevOps/GitHub branch;
  deployment pipelines promote dev → test → prod with parameterized rules. This is how Fabric work
  stops being click-ops, and it connects straight to `L34` (GitOps).

### 7.5 Cost discipline checklist

1. **Pause non-production capacities** on a schedule — largest single saving.
2. **Right-size the SKU** from Capacity Metrics data, not from a guess.
3. **Separate capacities** for engineering and BI so exploration cannot throttle executives.
4. **Prefer notebooks over Dataflows Gen2** for volume — materially cheaper per unit of data.
5. **Disable V-Order where nothing reads via Direct Lake** (typically Bronze).
6. **Run table maintenance** — `OPTIMIZE` (compaction) and `VACUUM` (retention). The **small-file
   problem** is the classic lakehouse killer: thousands of tiny Parquet files from frequent
   micro-batches make every read slow and every query expensive.
7. **Watch Direct Lake fallback** — a model silently on DirectQuery burns far more CU than one
   staying in Direct Lake.
8. **Set retention on Bronze.** Append-only forever is a real storage bill; append-only for a
   defined, compliance-driven window is a policy.

---

## Section 8 — How This Connects to Everything Else

| This module | Connects to |
|---|---|
| Medallion Gold → chunk/embed → index | `L13_RAG_DeepDive`, `L09_AzureAISearch` |
| SQL tool vs retrieval tool routing | `L24_Hallucination_Mitigation`, `L23_CAG_vs_RAG` |
| Fabric data agent as a delegated tool | `L28_MetaAgent_Hierarchies`, `L26_MCP` |
| RLS bypass via service principal / index copy | `L18_AISolutionArchitecture` §18.3 threat model |
| Capacity CU, smoothing, throttling | `L36_LLM_Observability_FinOps` (same FinOps discipline, different meter) |
| Git integration, deployment pipelines | `L34_Kubernetes_Helm_GitOps` |
| Pipelines as ADF-in-Fabric | `L20_IntegrationPatterns` — your JM Family ADF work transfers directly |
| Document Intelligence at the Bronze boundary | JM Family `cog-jma-dev-frm-recognizer` work |

**Certification note.** The relevant certs are **DP-600** (Fabric Analytics Engineer) and **DP-700**
(Fabric Data Engineer). DP-700 is the closer match to this module's content. Neither is required for
your target roles — but if a JD names Fabric, DP-600 is the cheapest possible credibility signal
given you already hold AI-102.

---

## Section 9 — The 60-Second Interview Answer

When asked *"what do you know about Fabric?"*, this is the shape:

> "Fabric is Microsoft's SaaS analytics platform. The architectural idea is one copy of the data —
> everything lands in OneLake as Delta-Parquet, and Spark, T-SQL and Power BI all read that same
> copy instead of each keeping their own. That's what makes Direct Lake possible: Power BI reads the
> Delta files straight into memory, so you get Import-mode speed with live data and no refresh job —
> though you have to watch for DirectQuery fallback.
>
> I'd structure it as a medallion — Bronze append-only and never edited so I can always replay,
> Silver conformed and deduped as the trusted layer with one row per business entity, Gold shaped
> per consumer. Orchestration in pipelines, real transformation in notebooks, Dataflows only where an
> analyst owns the logic — mainly for cost and testability.
>
> The part I care most about is what sits on top: Gold is what I ground agents on. Text questions go
> to an AI Search index built off Gold; anything with a numeric answer goes to a SQL tool against the
> warehouse, because that's where RAG hallucinates. The thing to watch is that copying Gold into a
> vector index detaches it from row-level security, so you re-attach it with security trimming and
> enforce the filter server-side.
>
> On cost, it's one Capacity Unit meter for everything, with bursting and smoothing — which is
> forgiving of spikes but accumulates carry-forward debt and eventually throttles interactive users
> first. So I'd isolate engineering from BI capacity and pause non-prod on a schedule."

That answer covers all seven sections, names the failure modes rather than the features, and lands
on AI and cost — which is where your value is.

---

## Related

- Q&A drill: `02_Questions/PerChapter/QA_L37_MicrosoftFabric.md`
- Plan of record: `04_Career/Consolidation_and_Update_Plan_2026-08-03.md` (Phase 2)
- Coverage baseline: `04_Career/Roadmap_Coverage_Check_2026-08-03.md` §6
- ML gap tracker: `09_ML/MLEngineer_Coverage_2026-07-26.md` gap #73
