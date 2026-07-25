# Azure AI Search — Indexers, Push vs Pull, Change Detection
## Deep Dive with JMA Real-World Code

---

## Two Ways Data Gets Into AI Search

```
WAY 1 — PULL (AI Search Indexer):
  Azure AI Search reaches out to the source on schedule
  and pulls data in automatically.
  ← AI Search owns the pipeline
  ← zero custom code

WAY 2 — PUSH (Your Code):
  Your application reads from source and pushes
  documents into AI Search via SDK.
  ← you own the pipeline
  ← full business logic control
```

---

## AI Search Indexer — Pull Pattern Explained

```
WHAT AN INDEXER IS:
  Index   = destination storage (like a database table)
             stores chunks + vectors
             always there, never moves

  Indexer = the ETL job that FILLS the index
             reads from source
             applies skills (chunk, embed, OCR)
             writes results to index
             runs on schedule
```

```
INDEXER LIFECYCLE:

  Blob Storage (new PDF arrives)
       │
       ▼
  INDEXER wakes up (on schedule)
       │
       ▼
  Reads new/changed files only (high-water mark)
       │
       ▼
  Applies SKILLSET:
   ├── Chunk skill    → splits doc into pieces
   ├── Embed skill    → calls text-embedding-3-large
   └── Merge skill    → combines results
       │
       ▼
  Writes chunks + vectors into INDEX
       │
       ▼
  Updates internal timestamp watermark
       │
       ▼
  Sleeps until next scheduled run
```

---

## How Indexer Detects Changes — High-Water Mark

```
Indexer does NOT watch Blob Storage in real time.
It polls on schedule and compares timestamps.

MECHANISM: High-Water Mark (timestamp tracking)
```

```
FIRST RUN (ever):
  Indexer scans ALL blobs in container
  Processes every file
  Saves internal state:
  "Last processed timestamp = 2026-06-14 09:00:00 UTC"
  Goes back to sleep

SECOND RUN (1 hour later):
  Asks Blob Storage:
  "Give me blobs where LastModified > 2026-06-14 09:00:00 UTC"

  Blob Storage returns ONLY changed/new files:
  ├── rav4-manual.pdf       ← not changed → SKIPPED
  ├── highlander-v2.pdf     ← modified 09:45 → PICKED UP ✅
  └── camry-new.pdf         ← added 10:15  → PICKED UP ✅

  Processes only 2 files (not all 100)
  Updates watermark to 10:00:00 UTC
```

```
WHAT POWERS THIS:
  Every blob has built-in metadata Azure sets automatically:
    LastModified: 2026-06-14 09:45:00 UTC
    ETag: "0x8DA1234..."

  Indexer reads LastModified from each blob.
  No code needed — Blob Storage tracks this automatically.
```

---

## Schedule Options

```
In Azure Portal → AI Search → Indexer → Schedule:

  None      ← run manually only (REST API call)
  Once      ← run now, never again
  5 minutes ← minimum polling interval
  Hourly    ← most common
  Daily     ← overnight batch

There is NO event-driven trigger built into the indexer.
It always polls on schedule — never "reacts" to an upload.
```

---

## Near Real-Time Option (Event-Driven, Extra Architecture)

```
Out of the box: blob uploaded → indexer finds it on NEXT scheduled run
                if hourly schedule → up to 60 min delay

For near real-time you add Event Grid:

  Blob Storage
       │ (fires event on every upload, within seconds)
       ▼
  Azure Event Grid
       │
       ▼
  Azure Function
       │ (calls AI Search REST API)
       ▼
  POST /indexers/my-indexer/run
       │
       ▼
  Indexer runs immediately, picks up new blob

This is extra architecture — not built into the indexer by default.
```

---

## What Gets Created in AI Search After Import Wizard

```
BEFORE import:
  Blob Storage → your PDFs
  AI Search   → (empty)

AFTER wizard completes:
  AI Search
  ├── INDEX created (you name it)
  │    ├── id            → "chunk-001"
  │    ├── content       → "RAV4 XLE $42,500 in Black..."
  │    ├── source        → "inventory.pdf"
  │    └── vector        → [-0.023, 0.061, 0.048, ...]
  │
  └── INDEXER created (auto-named)
       └── pipeline that ran to populate the index
           stays permanently, runs on schedule
```

---

## Import and Vectorize Data Wizard — Where to Configure

```
Azure Portal → your AI Search resource
 └── Overview page → "Import and vectorize data" button

WIZARD STEPS:

  Step 1: Connect your data
           ← Blob Storage / SharePoint / ADLS

  Step 2: Vectorize your text       ← EMBEDDING MODEL
           ├── Kind: Azure OpenAI
           └── Model: text-embedding-3-large

  Step 3: Vectorize images (skip if text only)

  Step 4: Advanced settings         ← CHUNKING
           ├── Chunk size: 512 tokens (slider)
           └── Chunk overlap: 10%

OR in AI Foundry:
  ai.azure.com → My assets → Indexes → + New index
  ← same settings, easier to find
```

---

## RAG vs File vs Multi RAG — Import Options

```
When you pick Blob Storage, wizard asks HOW to process:

  RAG
  ← Text-only documents (PDFs, Word, TXT)
  ← Chunks text → embeds text → stores text vectors
  ← Use for: vehicle data, warranty docs, policy docs

  File
  ← Treats each file as one document (no chunking)
  ← Good for structured files (JSON, CSV)
  ← No vector search, pure keyword

  Multi RAG (Multi-modal)
  ← Documents with BOTH text AND images
  ← PDFs with diagrams, charts, photos
  ← Creates text vectors AND image vectors
  ← Use for: technical manuals, brochures with photos
```

---

## Push vs Pull — When to Use Which

```
                    AI SEARCH INDEXER (Pull)      CUSTOM CODE (Push)
────────────────────────────────────────────────────────────────────
Data source         Blob / SQL / Cosmos           Any source via code
Custom filtering    ❌ not possible               ✅ full IF/ELSE logic
Retention rules     ❌ not possible               ✅ date cutoff in code
Multi-library       ❌ one source per indexer     ✅ multiple sources
Stale doc cleanup   ⚠️ soft-delete workaround     ✅ custom diff logic
Code required       ❌ zero code                  ✅ full SDK project
Maintenance         Low — Azure manages           Higher — your code
Good for            Simple uniform data           Complex business rules

USE PULL when:
  ├── Source is Blob / SQL / Cosmos
  ├── No custom field filtering needed
  ├── No retention rules
  └── You want zero-code RAG indexing fast

USE PUSH when:
  ├── Source requires Graph API (SharePoint with column filtering)
  ├── Business rules on which documents to include
  ├── Custom retention / date logic
  └── Multiple sources merged into one index
```

---

## JMA Real-World — EnterpriseSearch.Sync (Push Pattern)

```
Project: C:\Users\confksq\source\repos\JMA-Apps\docmgmt\Azure\
         AppServices\app-jma-docmgmt-aisearch

Two projects:
  EnterpriseSearch.Api   ← ASP.NET Core Web API (search/read)
  EnterpriseSearch.Sync  ← .NET BackgroundService (the "WebJob")
```

```
WHY JMA USES PUSH (not AI Search indexer) — from code:

REASON 1 — Retention filtering (GraphReaderService.cs:57)
  var retentionCutoff = DateTimeOffset.UtcNow.AddMonths(-RetentionMonths);
  ← skip documents older than configured months
  ← built-in SharePoint indexer cannot do retention logic

REASON 2 — JobSource column filtering (GraphReaderService.cs:89)
  var allowedJobSources = _sharePointOptions.GetAllowedJobSources();
  ← only index documents where SharePoint column matches allowed values
  ← built-in indexer pulls everything, cannot filter by column

REASON 3 — Multi-library support (GraphReaderService.cs:65)
  foreach (var libraryContext in siteContext.LibraryContexts)
  ← iterates multiple SharePoint drives/libraries
  ← built-in indexer is one data source = one library

REASON 4 — Stale document cleanup (IndexWriterService.cs:38)
  await _indexWriterService.DeleteMissingDocumentsAsync(activeDocumentIds)
  ← custom diff: what is in index but no longer in SharePoint → delete
  ← built-in indexer has no equivalent

REASON 5 — Schema validation on startup (SearchIndexProvisioningService.cs:135)
  ValidateExistingIndex(existingResponse.Value)
  ← throws if index schema doesn't match expected definition
  ← prevents silent schema drift in shared environments
```

```
SYNC FLOW (Worker.cs):

  RunSyncAsync() every run:
   │
   ├── EnsureIndexAsync()
   │    ← create index if missing
   │    ← validate schema if exists
   │
   ├── GraphReaderService.ProcessDocumentsAsync()
   │    ← calls graph.microsoft.com via Microsoft Graph API
   │    ← $top=200 per page
   │    ← filters by JobSource + RetentionMonths
   │    ← skips files with no contractNumber
   │    ← batches into groups of 100
   │
   ├── IndexWriterService.UploadDocumentsAsync()
   │    ← SearchClient.UploadDocumentsAsync(batch)  ← PUSH API
   │    ← no AI Search indexer involved
   │
   └── IndexWriterService.DeleteMissingDocumentsAsync()
        ← removes stale docs no longer in SharePoint
```

```
SCHEDULE (WorkerScheduleOptions.cs):
  Default mode:     Daily
  Default time:     09:00
  Default timezone: Eastern Standard Time
  RunOnStartup:     false (configurable)

  Modes:
  WorkerScheduleMode.Daily    ← once per day at DailyRunTime
  WorkerScheduleMode.Interval ← every IntervalMinutes (default 1440 = 24hrs)
```

```
AUTHENTICATION (dual strategy from code):
  SharePoint (Graph):
    → ConfidentialClientApplication if ClientId + ClientSecret configured
    → DefaultAzureCredential (Managed Identity) otherwise

  AI Search:
    → AzureKeyCredential if ApiKey configured
    → ClientSecretCredential if ClientId + Secret + TenantId configured
    → DefaultAzureCredential otherwise
```

---

## JMA Staging — No Indexers (Confirmed)

```
srch-jma-stg-indexer investigation:
  Indexers:     0  ← confirmed empty (200 OK, value: [])
  Data Sources: 0  ← no SharePoint connection
  Skillsets:    0
  Indexes:      1  ← documents-stg exists

WHY: Staging doesn't connect to real SharePoint (safety).
     Test data is loaded via Push API from deployment pipeline.
     No live SharePoint connection = no indexer needed in staging.
```

---

## Index Schema — What Gets Defined in Code

```csharp
// SearchIndexProvisioningService.cs — BuildIndexDefinition()
// No vectors, no embeddings — pure keyword/filter lookup:

  id              → filterable only (key)
  sharePointItemId→ filterable only
  sharePointDriveId→filterable only
  sourceLibrary   → filterable only
  contractNumber  → filterable only (NOT searchable)
  jobSource       → filterable only
  fileName        → SEARCHABLE ← only keyword-searchable field
  completedDate   → filterable + sortable
  scannedDate     → filterable + sortable
```

---

## Navigation

| | |
|---|---|
| **Previous** | [09 — RAG Deep Dive](09-RAG-Deep-Dive.md) |
| **Next** | [10 — Semantic Kernel](10-Semantic-Kernel.md) |
