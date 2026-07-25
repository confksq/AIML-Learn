# Module 8 — Document Intelligence
**Part 2: AI Engineering (AI-102 Level) | AI Solutions Architect Curriculum**

---

## What You Already Know (Recap)

From your JM Family work and prior modules:
- **Document Intelligence** — you use it in production for PDF invoice extraction
- **Azure Functions** — your pipeline trigger: Blob Storage → Function → Document Intelligence
- **Azure OpenAI** — downstream summarization after extraction
- **Managed Identity** — no API keys in code (`DefaultAzureCredential`)
- **Custom NER** (Module 7.3) — for entity types prebuilt models don't cover

This module explains *how Document Intelligence works* under the hood, which prebuilt models exist, when and how to build custom models, and the integration patterns you'd use in production.

---

**Running example (used throughout):**
> *JM Family's document processing pipeline: PDF invoices land in Blob Storage → Azure Function triggers → Document Intelligence extracts fields → Azure OpenAI summarizes → results stored in Cosmos DB / AI Search.*

Every concept maps to a real decision in this pipeline.

---

## Topic 8.1 — Document Intelligence Overview

---

### 1. What Is Azure AI Document Intelligence?

Document Intelligence (formerly Form Recognizer) is an AI service that extracts **structured data** from unstructured documents — PDFs, images, Word files, Excel files.

It does three things:
1. **Layout analysis** — identifies pages, lines, words, tables, checkboxes, selection marks
2. **Key-value extraction** — finds labeled fields ("Invoice Date: 2026-01-15")
3. **Model-based extraction** — uses trained models to find specific fields by document type

**Not the same as plain OCR:**
- OCR gives you a flat string of text
- Document Intelligence gives you structured JSON: field names, values, bounding boxes, confidence scores, table cells, page positions

---

### 2. How It Works Internally

```
PDF / Image
    ↓
[Layout Engine] — OCR + spatial analysis
    ↓
[Model Layer] — field extraction based on trained patterns
    ↓
JSON output: { fields, tables, key-value pairs, bounding boxes }
```

The layout engine runs first on every request regardless of model. The model layer then maps detected text to your expected fields.

**Two processing modes:**

| Mode | When to use |
|---|---|
| **Synchronous** | Small documents, < 2 pages, need result immediately |
| **Asynchronous (recommended)** | Multi-page PDFs, high volume, batch processing |

**Asynchronous flow (what your JM Family pipeline uses):**
```
POST /documentModels/{modelId}:analyze
    → returns Operation-Location header with operation ID

GET /documentModels/{modelId}/analyzeResults/{operationId}
    → poll until status = "succeeded"
    → then read result JSON
```

The C# SDK handles the polling for you with `WaitUntil.Completed`.

---

### 3. Resource Tiers

| Tier | Limit | Use |
|---|---|---|
| **Free (F0)** | 500 pages/month, 1 TPS | Dev/test only |
| **Standard (S0)** | Unlimited pages, 15 TPS | Production |

**Important:** Custom model training requires Standard tier. Free tier cannot train models.

---

### 4. Supported Input Formats

| Format | Max file size | Max pages |
|---|---|---|
| PDF | 500 MB | 2,000 |
| JPEG / PNG / BMP / TIFF | 500 MB | 1 per image |
| DOCX / XLSX / PPTX | 500 MB | Varies |

**Tips:**
- PDFs with embedded text (not scanned) give better accuracy than scanned image PDFs
- For scanned docs, 150 DPI minimum; 300 DPI recommended

---

## Topic 8.2 — Prebuilt Models

---

### 1. What Are Prebuilt Models?

Prebuilt models are Microsoft-trained models for common document types. You call them with no training required — Microsoft has already labeled millions of documents.

**Use prebuilt when:** The document type matches a supported category AND field accuracy is sufficient for your use case (> 85%).

---

### 2. Full Prebuilt Model List

| Model ID | Document type | Key fields extracted |
|---|---|---|
| `prebuilt-invoice` | Invoices | VendorName, CustomerName, InvoiceDate, DueDate, InvoiceTotal, LineItems |
| `prebuilt-receipt` | Receipts (retail/restaurant) | MerchantName, TransactionDate, Total, Items |
| `prebuilt-idDocument` | Passports, driver's licenses | FirstName, LastName, DocumentNumber, DateOfBirth, Address |
| `prebuilt-businessCard` | Business cards | ContactNames, JobTitles, Emails, PhoneNumbers, Addresses |
| `prebuilt-w2` | US W-2 tax forms | Employee, Employer, Wages, FederalTax, SocialSecurity |
| `prebuilt-healthInsuranceCard.us` | US health insurance cards | Member, MemberId, Payer, Deductible |
| `prebuilt-taxUsW2` | IRS W-2 | Same as w2, more structured |
| `prebuilt-contract` | Contracts (general) | Parties, Dates, PaymentTerms |
| `prebuilt-layout` | Any document | Tables, paragraphs, key-value pairs, selection marks (no field semantics) |
| `prebuilt-read` | Any document | Plain text extraction, language detection, handwriting |
| `prebuilt-document` | Any document | Key-value pairs + entities + layout (general-purpose) |

**Three "catch-all" models explained:**

| Model | When to use |
|---|---|
| `prebuilt-read` | You just need text out of a document. Fastest, cheapest. |
| `prebuilt-layout` | You need table structure + positions but no field semantics |
| `prebuilt-document` | You want key-value pairs extracted without knowing the doc type |

---

### 3. Calling a Prebuilt Model — C# Example

```csharp
var client = new DocumentAnalysisClient(
    new Uri("https://<endpoint>.cognitiveservices.azure.com/"),
    new DefaultAzureCredential()
);

// Analyze from URL
var operation = await client.AnalyzeDocumentFromUriAsync(
    WaitUntil.Completed,
    "prebuilt-invoice",
    new Uri("https://mystorageaccount.blob.core.windows.net/invoices/invoice001.pdf")
);

var result = operation.Value;

// Extract fields
foreach (var document in result.Documents)
{
    if (document.Fields.TryGetValue("VendorName", out var vendor))
        Console.WriteLine($"Vendor: {vendor.Content} (confidence: {vendor.Confidence:P0})");

    if (document.Fields.TryGetValue("InvoiceTotal", out var total))
        Console.WriteLine($"Total: {total.Content}");
}

// Extract tables
foreach (var table in result.Tables)
{
    Console.WriteLine($"Table: {table.RowCount} rows x {table.ColumnCount} cols");
    foreach (var cell in table.Cells)
        Console.WriteLine($"  [{cell.RowIndex},{cell.ColumnIndex}]: {cell.Content}");
}
```

---

### 4. Understanding the Response JSON

```json
{
  "status": "succeeded",
  "analyzeResult": {
    "documents": [
      {
        "docType": "invoice",
        "fields": {
          "VendorName": {
            "type": "string",
            "content": "Contoso Ltd.",
            "confidence": 0.98,
            "boundingRegions": [{ "pageNumber": 1, "polygon": [...] }]
          },
          "InvoiceTotal": {
            "type": "currency",
            "content": "$1,234.56",
            "valueCurrency": { "amount": 1234.56, "currencySymbol": "$" },
            "confidence": 0.95
          },
          "Items": {
            "type": "array",
            "valueArray": [
              {
                "valueObject": {
                  "Description": { "content": "Part #XY-42" },
                  "Quantity": { "content": "5" },
                  "UnitPrice": { "content": "$12.00" }
                }
              }
            ]
          }
        }
      }
    ],
    "tables": [...],
    "keyValuePairs": [...],
    "pages": [...]
  }
}
```

**Confidence scores:**
- `> 0.90` — reliable, use directly
- `0.70 – 0.90` — flag for review
- `< 0.70` — likely wrong, route to human review queue

---

### 5. JM Family Gap: What Prebuilt Invoice Doesn't Give You

The `prebuilt-invoice` model extracts general invoice fields. It will NOT give you:
- `VehicleMake`, `VehicleModel`, `VehicleVIN`
- `DealerCode`, `DealerName` (JM Family-specific)
- `PackCode`, `iPacketId`
- Custom line-item classifications specific to automotive

For these → **Custom Models** (Topic 8.3).

---

## Topic 8.3 — Custom Models

---

### 1. When to Use Custom Models

| Situation | Action |
|---|---|
| Prebuilt model covers your doc type | Use prebuilt |
| Prebuilt accuracy < 85% on your data | Custom model |
| Fields don't exist in any prebuilt | Custom model |
| Multiple document subtypes | Composed model |
| Need to process several doc types in one call | Composed model |

---

### 2. Custom Model Types

| Type | How it works | When to use |
|---|---|---|
| **Custom template** (form) | Learns fixed-position fields on structured forms | Forms with consistent layout (same fields, same positions) |
| **Custom neural** | Learns semantic field meaning across varied layouts | Documents with variable layouts, different vendors |
| **Composed** | Wraps multiple custom models, auto-routes by document type | Mixed document types in one batch |

**Template vs Neural — key difference:**
```
Template model: "InvoiceDate is always at position (x=120, y=200) on page 1"
Neural model:   "InvoiceDate is the date that follows 'Invoice Date:' regardless of where it appears"
```

**For JM Family:** Automotive invoices from different vendors have different layouts → **Custom Neural** is the right choice.

---

### 3. Training a Custom Model — Step by Step

```
Step 1: Gather training data
    - Minimum: 5 documents (15-20 recommended, 50+ for neural)
    - Must be representative of the variation you'll see in production
    - Label both positive examples AND edge cases

Step 2: Upload to Azure Blob Storage
    - One container for training documents
    - Keep labeled documents and unlabeled documents separate

Step 3: Label in Document Intelligence Studio
    - studio.ai.azure.com → Document Intelligence
    - Create project, connect to your storage account
    - For each document: draw bounding boxes, assign field names
    - For tables: label column headers + row data

Step 4: Train
    - Document Intelligence Studio → Train
    - Template: minutes | Neural: 20-60 minutes
    - Produces a Model ID (GUID)

Step 5: Evaluate
    - Studio shows per-field accuracy
    - Target: > 85% confidence on each key field

Step 6: Deploy
    - Custom models are immediately callable by Model ID
    - No separate deployment step (unlike Azure OpenAI)

Step 7: Call from your app
    - Use Model ID exactly like a prebuilt model ID
```

---

### 4. Labeling Tips

- **More documents > more fields** — if accuracy is low, add more training documents before adding more fields
- **Label every occurrence** — if a field appears 3 times in a document, label all 3
- **Include variation** — different vendors, fonts, date formats, currencies
- **Negative examples** — include documents where a field is absent (set it to empty)
- **Table labeling** — label the whole table, not just individual cells

---

### 5. Custom Model — C# Call

```csharp
// Exactly the same API as prebuilt — only the model ID changes
var operation = await client.AnalyzeDocumentFromUriAsync(
    WaitUntil.Completed,
    "my-custom-model-id-guid-here",  // your trained model ID
    new Uri("https://mystorageaccount.blob.core.windows.net/invoices/new-invoice.pdf")
);

var result = operation.Value;
foreach (var document in result.Documents)
{
    if (document.Fields.TryGetValue("VehicleMake", out var make))
        Console.WriteLine($"Vehicle Make: {make.Content}");

    if (document.Fields.TryGetValue("DealerCode", out var dealer))
        Console.WriteLine($"Dealer: {dealer.Content} ({dealer.Confidence:P0})");
}
```

---

### 6. Composed Models

When you have multiple document types in one batch, use a **composed model**:

```
Composed Model (ID: "jmfamily-docs-composed")
    ├── Custom Model A: "jmfamily-invoice-v2"     → routes if doc looks like invoice
    ├── Custom Model B: "jmfamily-contract-v1"    → routes if doc looks like contract
    └── Custom Model C: "jmfamily-po-v1"          → routes if doc looks like PO
```

You call the composed model ID. Document Intelligence classifies the document type and routes to the right sub-model automatically.

```csharp
// Call composed model — same API, composed model ID
var operation = await client.AnalyzeDocumentFromUriAsync(
    WaitUntil.Completed,
    "jmfamily-docs-composed",
    documentUri
);

var result = operation.Value;
Console.WriteLine($"Detected type: {result.Documents[0].DocType}");
// → "jmfamily-invoice-v2" or "jmfamily-contract-v1" etc.
```

**Limit:** Max 100 component models per composed model.

---

### 7. Model Lifecycle — Versioning

```
v1 model (Model ID: abc-123)  ← in production
    ↓ retrain with more data
v2 model (Model ID: def-456)  ← test in staging

// A/B test: route 10% traffic to v2, validate accuracy
// When confident → update your app config to use def-456
// Delete abc-123 after cutover
```

There's no built-in versioning in Document Intelligence — you manage it yourself by keeping old model IDs until you're confident in the new one.

---

## Topic 8.4 — Integration Patterns

---

### 1. The Standard Pipeline Pattern (Your JM Family Architecture)

```
Blob Storage (PDF arrives)
    ↓ BlobTrigger
Azure Function
    ↓ AnalyzeDocumentFromUriAsync (Managed Identity)
Document Intelligence
    ↓ Extracted JSON
Azure Function (map fields, validate confidence)
    ↓
    ├── High confidence (> 0.90) → Cosmos DB / AI Search
    ├── Low confidence (0.70-0.90) → Human Review Queue (Service Bus)
    └── Failed / < 0.70 → Dead Letter Queue + alert
    ↓
Azure OpenAI (optional: summarize extracted content)
```

---

### 2. Handling Low-Confidence Results

Never auto-process all results blindly. Build a confidence routing layer:

```csharp
const double AutoProcessThreshold = 0.90;
const double ReviewThreshold = 0.70;

foreach (var field in document.Fields)
{
    var confidence = field.Value.Confidence ?? 0;

    if (confidence >= AutoProcessThreshold)
        autoProcessed.Add(field.Key, field.Value.Content);
    else if (confidence >= ReviewThreshold)
        needsReview.Add(field.Key, field.Value.Content);
    else
        failed.Add(field.Key, field.Value.Content);
}

if (needsReview.Any())
    await serviceBusClient.SendMessageAsync(new ServiceBusMessage(
        JsonSerializer.Serialize(new { DocumentId = docId, Fields = needsReview })
    ));
```

---

### 3. Analyzing from Blob Storage (Managed Identity Pattern)

```csharp
// Option A: Pass SAS URL (simpler but has expiry)
var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
var operation = await docClient.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, modelId, sasUri);

// Option B: Stream content directly (better for private blobs)
var blobStream = await blobClient.OpenReadAsync();
var operation = await docClient.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, blobStream);
```

**Best practice for JM Family (Private Endpoint setup):**
- Function has Managed Identity → access to Blob Storage (Storage Blob Data Reader role)
- Function has Managed Identity → access to Document Intelligence (Cognitive Services User role)
- No SAS URLs, no keys — just identity chains

---

### 4. Batch Processing Pattern

For processing thousands of documents:

```
ADF Pipeline (or Durable Function)
    ↓
Fan-out: submit N documents in parallel
    ↓
Document Intelligence (async — get operation IDs)
    ↓
Poll / wait for completion
    ↓
Fan-in: aggregate results
    ↓
Bulk insert to Cosmos DB / AI Search
```

**Rate limits to know:**
- Standard tier: 15 TPS (transactions per second)
- At 15 TPS with 2-second processing per doc → ~900 docs/minute max
- For higher volume: request quota increase, or use multiple Document Intelligence resources

---

### 5. Error Handling Pattern

```csharp
try
{
    var operation = await client.AnalyzeDocumentFromUriAsync(
        WaitUntil.Completed, modelId, documentUri);

    if (operation.Value.Documents.Count == 0)
    {
        logger.LogWarning("No documents extracted from {Uri}", documentUri);
        // Route to manual review — not an error, but needs attention
        return;
    }

    // process...
}
catch (RequestFailedException ex) when (ex.Status == 429)
{
    // Throttled — exponential backoff (Polly)
    throw; // let Polly handle retry
}
catch (RequestFailedException ex) when (ex.Status == 400)
{
    // Bad document — unsupported format, corrupted, password protected
    logger.LogError("Invalid document {Uri}: {Message}", documentUri, ex.Message);
    await MoveToDeadLetterAsync(documentUri);
}
catch (RequestFailedException ex) when (ex.Status >= 500)
{
    // Service-side error — transient, retry
    throw;
}
```

---

### 6. Document Intelligence + Azure AI Search (Your RAG Pipeline)

After extracting fields from Document Intelligence, you index them into Azure AI Search:

```
Document Intelligence output:
{
  "VendorName": "Ford Motor Co.",
  "InvoiceDate": "2026-01-15",
  "InvoiceTotal": 1234.56,
  "VehicleMake": "Ford",
  "DealerCode": "FL-042",
  "RawText": "..."   ← full text for semantic search
}
    ↓
Azure AI Search index:
{
  "id": "invoice-001",
  "vendorName": "Ford Motor Co.",
  "invoiceDate": "2026-01-15T00:00:00Z",
  "invoiceTotal": 1234.56,
  "vehicleMake": "Ford",
  "dealerCode": "FL-042",
  "content": "...",        ← searchable text
  "contentVector": [...]   ← embedding for semantic search
}
```

The structured fields enable **faceted filtering** ("show all Ford invoices from dealer FL-042") while the vector field enables semantic search ("find invoices similar to this one").

---

## Module 8 — Architecture Summary

```
┌──────────────────────────────────────────────────────────────────────┐
│                   Azure AI Document Intelligence                      │
│                                                                        │
│  Input formats: PDF, JPG, PNG, TIFF, DOCX, XLSX                      │
│  Processing: Async (recommended) | Sync (small docs)                  │
│                                                                        │
│  Model types:                                                          │
│  ┌─────────────────┐  ┌───────────────────┐  ┌──────────────────┐   │
│  │    Prebuilt      │  │   Custom Template  │  │  Custom Neural   │   │
│  │ invoice, receipt │  │  Fixed-layout      │  │ Variable layout  │   │
│  │ ID, layout, read │  │  forms             │  │ multi-vendor     │   │
│  └─────────────────┘  └───────────────────┘  └──────────────────┘   │
│                                    ↓                                   │
│                           Composed Model                               │
│                    (auto-route by document type)                       │
│                                                                        │
│  Output: Fields + Confidence + Tables + Bounding Boxes + Key-Value    │
│                                                                        │
│  Integration pattern:                                                  │
│  Blob → Function → DocIntelligence → Confidence routing →             │
│       → Auto process (>0.90) | Review queue (0.70-0.90) | DLQ        │
│                                                                        │
│  Auth: Managed Identity (Cognitive Services User role)                │
│  Networking: Private Endpoint for financial/contract docs             │
└──────────────────────────────────────────────────────────────────────┘
```

---

## Recall — Module 8 Self-Test Questions

Try answering before checking the answers below.

**Q1.** Your JM Family pipeline processes invoices from 40 different automotive vendors. Each vendor has a different invoice layout. The `prebuilt-invoice` model extracts `VendorName` and `Total` but misses `VehicleMake`, `VehicleVIN`, and `DealerCode`. What model type do you use and why?

**Q2.** You have a mix of invoices, purchase orders, and contracts all landing in the same Blob Storage container. You want one Document Intelligence call to handle all three types. What do you set up?

**Q3.** Your Document Intelligence call returns `InvoiceTotal` with confidence `0.62`. Your app auto-processes it and writes `$0.00` to Cosmos DB (the field was blank in that doc). What went wrong architecturally, and how do you fix it?

**Q4.** You're analyzing a scanned PDF. The Read model returns garbled text for some lines. What's the most likely cause, and what do you recommend?

**Q5.** Your Document Intelligence resource is at Standard tier (15 TPS). You need to process 50,000 invoices overnight (8 hours). Can you do it? Show the math.

**Q6.** Your team wants to retrain the custom invoice model with 200 new labeled documents. You don't want to break the production pipeline during retraining. How do you handle versioning?

---

<details>
<summary>Answers (expand after attempting)</summary>

**A1.** Use **Custom Neural model**. Template models require consistent layouts — 40 different vendor formats will break a template model. Neural models understand field semantics across variable layouts ("VehicleMake is the vehicle make regardless of where it appears"). Train with a representative sample from multiple vendors (~50+ documents covering the layout variations).

**A2.** Train three separate custom models (one per document type), then create a **Composed model** wrapping all three. Call the composed model ID — Document Intelligence classifies the incoming document and routes to the right sub-model automatically. The response includes `DocType` telling you which sub-model matched.

**A3.** No confidence threshold check. The app blindly used the extracted value even when confidence was low. Fix: add a confidence routing layer — anything below 0.70 goes to a dead-letter queue or human review, never auto-processed. Never write low-confidence values to the database without flagging them.

**A4.** Most likely cause: scan resolution too low (below 150 DPI) or skewed/rotated scan. Recommendation: enforce minimum 300 DPI at document ingestion, add a pre-processing step to deskew/rotate if needed, or request the original digital PDF instead of the scanned copy.

**A5.** Yes, easily. 15 TPS × 3600 sec × 8 hrs = **432,000 transactions** available. 50,000 invoices fits with room to spare. Caveat: if each invoice is multi-page, each page counts as a transaction — check actual page counts. Also, if each "transaction" is one document call (not one page), verify your tier's per-call vs per-page billing model.

**A6.** Train the new model → gets a new Model ID (e.g., `invoice-v2`). Test it in a staging environment. When accuracy is validated, update the model ID in your app configuration (environment variable / App Config) without code deployment. Keep `invoice-v1` alive until you confirm `invoice-v2` is working in production. Delete `invoice-v1` only after a safe observation period (e.g., 1 week).

</details>

---

## Memory Hooks

- **"prebuilt-read = text out, prebuilt-layout = structure out, prebuilt-document = key-value out"** — three catch-alls, different purposes
- **"Template = fixed layout, Neural = variable layout"** — choose by vendor diversity
- **"Composed model = one call, many doc types"** — auto-routes, returns DocType
- **"Confidence < 0.70 = never auto-process"** — always route to review queue
- **"15 TPS Standard tier"** — matters for batch sizing; request increase if needed
- **"Model versioning = two Model IDs, one config switch"** — no built-in versioning, manage it yourself
- **"Async always for production"** — POST → get operation ID → poll → read result

---
---

## 2026 Updates

| Topic | Update |
|---|---|
| **New prebuilt models** | W-2, 1098, 1099, health insurance card, US mortgage (1003/1008), marriage certificate — expanding beyond invoice/receipt/ID |
| **Content Understanding (AI Foundry)** | Document Intelligence is now accessible via Azure AI Foundry under "Content Understanding" — same API, new portal entry point. AI Foundry wizard wraps DI for no-code extraction pipelines |
| **Markdown output** | DI now supports outputting extracted content as Markdown (tables → markdown tables, headings preserved) — better for feeding into LLMs for RAG |
| **Custom Neural Model improvements** | Custom Neural model now handles mixed-format documents better. Faster training convergence. Recommended over Custom Template for most new projects |
| **Document Intelligence Studio** | documentintelligence.ai.azure.com replaces the old Form Recognizer Studio URL. All projects and models carry over |
| **AI Foundry Content Understanding** | New capability — processes video, audio, and images with structured extraction, not just documents |

---

## Interactive Learning Ideas

### Exercise 1 — Prebuilt Model Comparison (20 min)
Go to documentintelligence.ai.azure.com → try these prebuilt models on sample documents:
- Invoice model → a sample invoice PDF
- Layout model → a multi-column document
- Read model → a handwritten note photo
Compare JSON output structure. Which fields does each return?

### Exercise 2 — Markdown Output Test (15 min)
Take a PDF with tables (any financial report). Run it through DI with `outputContentFormat = "markdown"`. Then paste the markdown output into an Azure OpenAI prompt: "Summarize the key figures from this table." Compare how LLM handles markdown vs raw JSON text. Notice the improvement.

### Exercise 3 — Confidence Routing Implementation (20 min)
Write a C# method `RouteByConfidence(AnalyzeResult result)` that:
- Checks the confidence of each extracted field
- Routes to `AutoProcess` if all fields > 0.90
- Routes to `ReviewQueue` if any field is 0.70–0.90
- Routes to `DeadLetter` if any field < 0.70
- Logs which fields triggered the routing decision

### Exercise 4 — JMA Pipeline Trace (15 min)
Look at `cog-jma-dev-frm-recognizer` in the Azure portal:
- What models are deployed on it?
- What pricing tier?
- Is public network access restricted?
- Trace one document through the JMA pipeline: where does it come from, what DI model processes it, where does the output go?

### Exercise 5 — Custom Template vs Neural Decision
For each JMA document type, decide: Custom Template or Custom Neural model?
- Standard JMA invoice (always same layout, same printer)
- Dealer agreements (same fields but different law firms format them differently)
- Vehicle inspection reports (handwritten checkboxes + printed text)
- Lease contracts from 15 different manufacturers

---

*Chapter file for: AI Solutions Architect Curriculum | Part 2 Module 8*
*Written: 2026-05-27 | Updated: 2026-06-30*

---

## Appendix — Merged from Legacy Notes

> Consolidated 2026-07-18 during library reorganization. Source: `11-DocumentIntelligence-vs-AISearch.md`.

### 1. Document Intelligence vs Azure AI Search — Reader vs Finder

Both services deal with documents, both are Azure AI services, and both appear in JMA production (`cog-jma-dev-frm-recognizer` and `srch-jma-dev-indexer`). They do completely different jobs:

```
Azure AI Document Intelligence  =  READER
                                    "What is written in this document?"

Azure AI Search                 =  FINDER
                                    "Which documents match this question?"
```

| | Document Intelligence | Azure AI Search |
|---|---|---|
| **Input** | Raw document — PDF, image, scan, photo | Chunks of text + vectors (already extracted) |
| **Output** | Structured data extracted from it | Matching results for a user query |
| **Does NOT** | Store anything. Search anything. | Read raw documents. Perform OCR. |
| **Billing** | Per page processed | Per search unit (tier-based) |

Document Intelligence reads *one* document and returns what is written in it. AI Search needs content **already extracted** — then it indexes and finds it. Neither can substitute for the other, which is why a full RAG pipeline over scanned documents needs three separate resources with three separate bills:

```
┌──────────────────────────────────────────────────────────────┐
│  1. Azure AI Document Intelligence                           │
│     READ — extract text + fields from raw documents          │
│     Billing: per page processed                              │
├──────────────────────────────────────────────────────────────┤
│  2. Azure OpenAI (text-embedding-3-large)                    │
│     CONVERT — turn extracted text into vectors               │
│     Billing: per token embedded                              │
├──────────────────────────────────────────────────────────────┤
│  3. Azure AI Search                                          │
│     STORE + SEARCH — index vectors, find matching content    │
│     Billing: per search unit (tier-based)                    │
└──────────────────────────────────────────────────────────────┘
Each is a separate Azure resource. Separate billing. Separate SDK.
```

---

### 2. When NOT to Use Document Intelligence

Topic 8.2 covers when to reach for prebuilt vs custom models. The prior question — *should this document touch Document Intelligence at all?* — is answered here:

```
USE Document Intelligence when:
  ├── Documents are scanned images (PDFs from scanner, photos)
  ├── Documents have structured fields you need to extract
  ├── You need to read tables from PDFs programmatically
  ├── Source is handwritten forms or mixed-format documents
  └── You need key-value pairs extracted, not just raw text

DO NOT USE when:
  ├── Document is already machine-readable text (Word, TXT, JSON)
  │    → read it directly, you are paying per page for nothing
  ├── You just need to search through text
  │    → use AI Search directly
  └── Document is structured data
       → use a database
```

The most common waste pattern is routing born-digital PDFs and Office documents through DI out of habit, when the text layer is already available for free.

---

### 3. JMA Production — Current State and the RAG Gap

```
cog-jma-dev-frm-recognizer
  ← Azure AI Document Intelligence resource (dev)
  ← Service: Form Recognizer / Document Intelligence
  ← Manually deployed 2023-08-18, no CI/CD
  ← Owner: Matt Waterman
  ← Reads/extracts from scanned forms and documents

srch-jma-dev-indexer (documents-dev index)
  ← Azure AI Search resource (dev)
  ← Stores extracted fields: contractNumber, fileName, dates
  ← NO vectors, NO embeddings — pure keyword + filter lookup
  ← EnterpriseSearch.Sync WebJob pushes data here via Graph API
```

**Current flow — no AI, no RAG:**

```
SharePoint documents → WebJob → AI Search index
  ← contractNumber used as a filter to find the file
  ← fileName is the only keyword-searchable field
```

Note what is *missing*: the two services are both deployed but are not connected to each other. DI extracts from scanned forms; the search index is populated separately from SharePoint metadata. There is no pipeline joining extraction output to the retrieval layer.

**Future opportunity — closing the gap:**

```
Scanned contract → Document Intelligence (extract full text)
  → chunk → embed → AI Search (vector index)
  → user asks in natural language → RAG answer with citations
```

Two flags on the current state worth raising in any architecture review: `cog-jma-dev-frm-recognizer` was **manually deployed with no CI/CD**, so its configuration is not reproducible; and the index carries no vector field, so no amount of prompt work on top of it will produce semantic retrieval.

---

### 4. Content Understanding — Document Intelligence Inside AI Foundry

```
AI Foundry → Content Understanding
  ← this is Document Intelligence exposed through the AI Foundry portal
  ← same service, same models, different UI entry point
  ← lets you test extraction on sample documents without code
  ← connects output to the Knowledge / Data section for RAG ingestion
```

The practical value is the last line: from Content Understanding you can wire extraction output straight into a Foundry Knowledge source, which is the no-code version of the DI → chunk → embed → AI Search pipeline described above.
