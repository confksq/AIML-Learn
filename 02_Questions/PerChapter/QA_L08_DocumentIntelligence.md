# Q&A — L08: Document Intelligence
**Source chapter:** `01_Lessons/Part2_AzureAIServices/L08_DocumentIntelligence.md` | **Format:** self-study
**Questions:** 28 | *No overlap with the interview bank or the chapter's own self-test — these test the chapter's factual content directly.*

---

## Overview & Internals

**Q1. What three things does Document Intelligence do that plain OCR doesn't?**
(1) **Layout analysis** — pages, lines, words, tables, checkboxes, selection marks; (2) **key-value extraction** — labeled fields ("Invoice Date: 2026-01-15"); (3) **model-based extraction** — trained models mapping text to specific fields by document type. OCR returns a flat string; DI returns structured JSON with field names, values, bounding boxes, confidence scores, and table cells.

**Q2. What always runs first inside Document Intelligence, regardless of which model you call?**
The **layout engine** (OCR + spatial analysis) runs on every request. The model layer then maps the detected text to expected fields on top of that layout output.

**Q3. Sync vs async processing — when is each appropriate, and which does production use?**
**Synchronous** — small documents (<2 pages) needing an immediate result. **Asynchronous** — multi-page PDFs, high volume, batch (recommended, and what the JMA pipeline uses). Async flow: POST `:analyze` → get `Operation-Location` header with operation ID → poll `analyzeResults/{operationId}` until `succeeded`. The C# SDK hides the polling via `WaitUntil.Completed`.
*Memory hook: "Async always for production — POST, poll, read."*

**Q4. Compare the Free and Standard tiers — and what can Free NOT do?**
**F0:** 500 pages/month, 1 TPS — dev/test only. **S0:** unlimited pages, 15 TPS — production. Custom model **training requires Standard** — Free tier cannot train models.

**Q5. What are the input format limits, and what improves accuracy on scanned documents?**
PDF up to 500 MB / 2,000 pages; JPEG/PNG/BMP/TIFF (1 page per image); DOCX/XLSX/PPTX supported. PDFs with embedded (digital) text beat scanned-image PDFs. For scans: 150 DPI minimum, **300 DPI recommended**.

---

## Prebuilt Models

**Q6. Name six specific-document prebuilt models and one key field each extracts.**
`prebuilt-invoice` (InvoiceTotal, LineItems), `prebuilt-receipt` (MerchantName, Total), `prebuilt-idDocument` (DocumentNumber, DateOfBirth), `prebuilt-businessCard` (Emails, PhoneNumbers), `prebuilt-w2` (Wages, FederalTax), `prebuilt-healthInsuranceCard.us` (MemberId, Payer). Also: `prebuilt-contract` (Parties, PaymentTerms).

**Q7. Distinguish the three "catch-all" prebuilt models.**
| Model | Gives you | Use when |
|---|---|---|
| `prebuilt-read` | Plain text + language detection + handwriting | You just need text out — fastest, cheapest |
| `prebuilt-layout` | Tables, paragraphs, positions, selection marks — no field semantics | You need structure but not field meaning |
| `prebuilt-document` | Key-value pairs + entities + layout | You want KV pairs without knowing the doc type |
*Memory hook: "read = text, layout = structure, document = key-value."*

**Q8. In the C# prebuilt call, how do you read one field's value and its confidence?**
`document.Fields.TryGetValue("VendorName", out var vendor)` → `vendor.Content` for the extracted text, `vendor.Confidence` for the 0–1 score. Tables come from `result.Tables` with `RowCount`/`ColumnCount` and per-cell `RowIndex`/`ColumnIndex`/`Content`.

**Q9. In the response JSON, what's the difference between `content` and typed value properties like `valueCurrency`?**
`content` is the raw extracted text as it appeared ("$1,234.56"); typed properties parse it into usable data (`valueCurrency: { amount: 1234.56, currencySymbol: "$" }`). Line items arrive as an `array` type whose `valueArray` holds `valueObject` entries (Description, Quantity, UnitPrice per line).

**Q10. What are the chapter's three confidence bands and the action for each?**
`> 0.90` — reliable, use directly. `0.70–0.90` — flag for review. `< 0.70` — likely wrong, route to human review / never auto-process.

**Q11. Which JMA-specific fields will `prebuilt-invoice` never give you?**
`VehicleMake`/`VehicleModel`/`VehicleVIN`, `DealerCode`/`DealerName`, `PackCode`/`iPacketId`, and automotive-specific line-item classifications — the gap that motivates custom models.

---

## Custom Models

**Q12. Template vs Neural custom model — state the one-line mental model for each.**
Template: "InvoiceDate is always at position (x=120, y=200) on page 1" — learns **fixed positions** on consistent layouts. Neural: "InvoiceDate is the date following 'Invoice Date:' wherever it appears" — learns **field semantics** across varied layouts.
*Memory hook: "Template = fixed layout, Neural = variable layout."*

**Q13. Why is Custom Neural the right choice for JMA's automotive invoices?**
Invoices come from many different vendors, each with a different layout — template models break on layout variation; neural models generalize across it. (2026 note: neural also now converges faster in training and is recommended over template for most new projects.)

**Q14. What are the training-data minimums for custom models?**
Absolute minimum 5 documents; 15–20 recommended; **50+ for neural** — and they must represent the real variation production will see (different vendors, fonts, date formats, currencies), including edge cases.

**Q15. Walk the 7-step custom model training workflow.**
(1) Gather representative training docs → (2) upload to Blob Storage (labeled and unlabeled kept separate) → (3) label in **Document Intelligence Studio** (draw bounding boxes, assign field names, label tables) → (4) train (template: minutes; neural: 20–60 min) → produces a **Model ID (GUID)** → (5) evaluate per-field accuracy (target >85%) → (6) no separate deployment step — the model is immediately callable by ID → (7) call it exactly like a prebuilt model, just with your Model ID.

**Q16. Give four of the chapter's five labeling tips.**
(1) More documents beats more fields — add docs before adding fields when accuracy is low; (2) label **every** occurrence of a field, not just the first; (3) include variation across vendors/fonts/formats; (4) include negative examples (documents where a field is absent, set empty); (5) label whole tables, not individual cells.

**Q17. What is a composed model, and what does the response tell you that a single custom model doesn't?**
A wrapper around multiple custom models (max **100** components) — you call one composed model ID, DI classifies the incoming document, routes to the right sub-model automatically, and the response's `DocType` tells you which sub-model matched (e.g., "jmfamily-invoice-v2").
*Memory hook: "Composed = one call, many doc types."*

**Q18. Document Intelligence has no built-in model versioning — what's the manual pattern?**
Train the new model → it gets a **new Model ID** → validate in staging → switch the model ID in app configuration (env var / App Config — no code deploy) → keep the old model ID alive through an observation period → delete only after the new one is proven.

---

## Integration Patterns

**Q19. Draw the JMA standard pipeline from blob to storage, including the confidence routing.**
Blob Storage (PDF arrives) → BlobTrigger fires Azure Function → `AnalyzeDocumentFromUriAsync` via Managed Identity → Document Intelligence returns JSON → Function validates confidence → **>0.90** auto-process to Cosmos DB / AI Search; **0.70–0.90** to human review queue (Service Bus); **<0.70 / failed** to dead-letter queue + alert → optional Azure OpenAI summarization downstream.

**Q20. What two Managed Identity role assignments does the Function need in the no-keys pattern?**
**Storage Blob Data Reader** on the storage account (read the PDFs) and **Cognitive Services User** on the Document Intelligence resource (call the API) — identity chains replace both SAS URLs and API keys.

**Q21. SAS URL vs streaming for private blobs — trade-off?**
SAS URL (`GenerateSasUri`) is simpler but carries an expiry and is a bearer credential; streaming the blob content directly (`OpenReadAsync` → `AnalyzeDocumentAsync`) avoids any URL-based credential — better for private blobs in a locked-down setup.

**Q22. What's the fan-out/fan-in batch pattern for thousands of documents?**
ADF pipeline or **Durable Function**: fan out — submit N documents in parallel (async, collect operation IDs) → poll for completion → fan in — aggregate results → bulk insert to Cosmos DB / AI Search. Throughput ceiling: at 15 TPS Standard tier, roughly ~900 docs/minute best case.

**Q23. In the error-handling pattern, how are 429, 400, and 5xx each treated differently?**
**429** (throttled) — rethrow and let Polly retry with exponential backoff. **400** (bad document — unsupported format, corrupted, password-protected) — log and move to dead-letter; retrying will never fix a bad document. **5xx** (service-side transient) — rethrow to retry. Also: zero documents extracted isn't an exception — log a warning and route to manual review.

**Q24. Why index Document Intelligence output into Azure AI Search with both structured fields AND a content vector?**
Structured fields (`vendorName`, `dealerCode`, `invoiceTotal`) enable **faceted filtering** ("all Ford invoices from dealer FL-042"); the raw text + its embedding (`contentVector`) enable **semantic search** ("find invoices similar to this one") — one index serving both query styles.

---

## 2026 Updates & Edge Facts

**Q25. What is Markdown output, and why does it matter for RAG?**
DI can now emit extracted content as **Markdown** (tables become markdown tables, headings preserved) instead of raw JSON/text — LLMs handle markdown-structured tables far better in prompts, improving downstream RAG/summarization quality. Set `outputContentFormat = "markdown"`.

**Q26. Where does Document Intelligence now surface in Azure AI Foundry?**
Under **"Content Understanding"** — same API, new portal entry point, with a no-code extraction-pipeline wizard. Content Understanding also extends beyond documents to structured extraction from **video, audio, and images**.

**Q27. Which new prebuilt models arrived in the 2026 wave?**
Tax forms beyond W-2 (1098, 1099), US mortgage forms (1003/1008), health insurance card, marriage certificate — expanding past the classic invoice/receipt/ID set. The Studio URL also moved to documentintelligence.ai.azure.com.

**Q28. A multi-page invoice counts how, against quota and billing?**
Each **page** counts as a transaction for capacity math — a 10-page PDF isn't one unit of work. When sizing batch jobs (like the 50,000-invoice overnight run), compute against actual page counts, and verify the tier's per-call vs per-page billing model.

---

*Curriculum Q&A Batch A — file 3 of 3 (L06, L07, L08 complete). Next batch: L09, L10, L11_1, L11_2.*
