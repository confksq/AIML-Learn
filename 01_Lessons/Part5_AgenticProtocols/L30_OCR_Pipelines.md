# Module 09 — OCR Pipelines: Azure Document Intelligence vs John Snow Labs


---

> **⚙️ Config or Code? — Quick Reference for This Module**
> | Stage | Portal Config | Custom Code |
> |---|---|---|
> | Pre-processing (de-noise, de-skew, binarize) | None ❌ | 100% Code (OpenCV / ImageSharp) ✅ |
> | DI model selection (prebuilt) | Select model in portal ✅ | API call (few lines) |
> | DI Custom Template — label + train | Document Intelligence Studio (portal) ✅ | None for training |
> | DI API call | None ❌ | SDK code (`StartAnalyzeDocumentAsync`) ✅ |
> | Confidence routing | None ❌ | 100% Code (if/else on confidence score) ✅ |
> | Post-processing validation | None ❌ | 100% Code (format, cross-field, business rules) ✅ |
> | JSL de-identification pipeline | None ❌ | Python code (`MedicalNerModel`, `DeIdentification`) ✅ |
> | Blob Storage trigger (Azure Function) | Portal (event trigger config) ✅ | Function code body ✅ |

## Why This Module Matters

The job description asks you to "Evaluate and select OCR solutions aligned to accuracy and scale requirements" and "Design OCR pre-processing pipelines (de-noise, de-skew, binarization) and post-processing validation workflows." You've built this at JM Family — you have production credibility here. You will be asked:
- "How does your document processing pipeline handle poor-quality scans?"
- "When would you use John Snow Labs over Azure Document Intelligence?"
- "What happens when OCR confidence is low?"

Your anchor: JM Family uses `cog-jma-dev-frm-recognizer` (Azure Document Intelligence) for dealer form extraction. You built the confidence routing: >0.90 auto-process, 0.70–0.90 human review queue, <0.70 dead-letter.

---

## Section 1 — What OCR Is Solving (the real problem)

OCR (Optical Character Recognition) is not just "read text from an image." In an enterprise context, OCR is a **structured data extraction pipeline**: you receive unstructured documents (PDFs, scans, photos) and you need to produce machine-readable structured data that downstream systems can process.

The challenge: documents are messy. Scanned at an angle. Printed on colored paper. Faxed across three systems. Stamped with "COPY" diagonally. Your OCR pipeline must handle all of that before the model even sees the text.

**The mental model:** Think of OCR like a **hospital admissions form triage**. Before a clerk can enter data into the EHR, they: straighten the paper (de-skew), make sure they can read it (de-noise), and confirm it's the right form (classification). Then they extract the data field by field. Your pipeline does the same steps — just automated.

---

## Section 2 — Pre-Processing Pipeline (before OCR runs)

Pre-processing transforms a raw image into something the model can read accurately. Three core steps:

### Step 1 — De-Noise
Remove artifacts that are not part of the original document content.

**What it removes:**
- Salt-and-pepper noise (random black/white pixels from scanning)
- Background speckle from aged or faxed documents
- JPEG compression artifacts

**How:** Gaussian blur, median filter, or adaptive thresholding via OpenCV / Azure Vision preprocessing.

**Healthcare example:** A patient brings in a handwritten medication list faxed from another hospital. The fax machine introduced horizontal line artifacts. De-noise removes them so the model doesn't misread "metformin" as "rnetformin."

### Step 2 — De-Skew
Straighten documents that were placed at an angle in the scanner.

**Why it matters:** OCR models trained on horizontal text perform poorly on text rotated even 3-5 degrees. De-skew detects the angle and rotates the image back to horizontal before extraction.

**How:** Hough transform to detect the dominant line angle, then rotate the image to correct it.

**JM Family anchor:** Dealer forms scanned at dealerships often arrive tilted 2-8 degrees. Before we added de-skew, our confidence scores were averaging 0.78. After de-skew, they jumped to 0.91.

### Step 3 — Binarization
Convert the image to pure black-and-white (binary pixel values).

**Why:** Color and grayscale images carry noise that confuses character recognition. Binarization makes text crisp and background pure white — exactly what OCR models expect.

**How:** Adaptive thresholding (not global) — global thresholding fails on documents with uneven lighting (one corner darker than another). Adaptive thresholding calculates the threshold locally per region.

**Pre-processing flow:**
```
Raw scan / PDF page
      ↓
Convert to grayscale
      ↓
De-noise (median filter)
      ↓
De-skew (Hough + rotate)
      ↓
Adaptive binarization
      ↓
Feed to OCR model
```

---

## Section 3 — Azure Document Intelligence (What You Use)

**Azure Document Intelligence** (formerly Form Recognizer) is Microsoft's managed OCR + structured extraction service. It's not just character recognition — it understands document structure: tables, key-value pairs, checkboxes, signatures.

**Three model tiers:**

| Model | What it does | When to use |
|-------|-------------|-------------|
| **Read** | Raw text extraction only | Extracting text from any document with no structure expectations |
| **Layout** | Text + tables + structure | Documents where table structure matters |
| **Prebuilt** | Domain-specific (Invoice, Receipt, ID, Tax W2) | Standard document types — no training needed |
| **Custom Template** | Fixed-layout forms (same field positions every time) | Your own forms with consistent layout |
| **Custom Neural** | Variable-layout documents (same fields, different layouts) | Multi-vendor forms with same fields but different formatting |

**JM Family production:**
"We use Custom Template models for JM Family dealer forms — each incentive program has its own form template, so field positions are fixed. Training requires 5+ labeled samples per template in Document Intelligence Studio. Confidence scores are per-field — we route on the minimum confidence across all required fields."

**The async processing flow:**
```csharp
// 1. Submit document
var operation = await _client.StartAnalyzeDocumentAsync(
    "prebuilt-invoice", documentStream);

// 2. Poll for completion (not synchronous)
var result = await operation.WaitForCompletionAsync();

// 3. Extract with confidence routing
foreach (var field in result.Documents[0].Fields)
{
    if (field.Value.Confidence < 0.70f)
        await _deadLetterQueue.SendAsync(documentId, field.Name);
    else if (field.Value.Confidence < 0.90f)
        await _reviewQueue.SendAsync(documentId, field.Name);
    else
        processedFields[field.Name] = field.Value.Content;
}
```

---

## Section 4 — John Snow Labs (The Healthcare Alternative)

**John Snow Labs** is a specialized NLP company whose **Spark NLP** and **Healthcare NLP** libraries are purpose-built for clinical and medical document processing. It is NOT a general-purpose OCR service — it's a medical language understanding platform.

**When John Snow Labs wins over Azure DI:**

| Scenario | Why JSL wins |
|----------|-------------|
| Clinical notes (SOAP notes, discharge summaries) | JSL has medical NER models that extract diagnoses, medications, dosages, procedures with clinical accuracy |
| ICD-10 / CPT code extraction | JSL models trained on clinical coding — Azure DI has no concept of medical codes |
| De-identification of PHI | JSL's de-identification pipeline removes 18 HIPAA identifiers from text with >99% recall |
| Medication normalization | JSL maps drug brand names to RxNorm codes — Azure DI just extracts text |
| Radiology reports | JSL's radiology models understand anatomical terms, measurement patterns |

**When Azure DI wins:**

| Scenario | Why Azure DI wins |
|----------|-----------------|
| Structured forms (invoices, insurance cards, tax forms) | DI's prebuilt models handle standard layouts out of the box |
| Fixed-layout proprietary forms | Custom Template model, no ML training needed |
| Integration with Azure ecosystem | Native connection to Azure AI Search, Blob Storage, Logic Apps |
| Teams without Python/Spark expertise | DI is a REST API — any language, no cluster needed |

**The key distinction:**
Azure DI = **form structure** expert. John Snow Labs = **medical language** expert.

A hospital admissions form → Azure DI.
A physician's SOAP note → John Snow Labs.

---

## Section 5 — Post-Processing Validation

After OCR extracts the text, you validate the extracted data before it enters any downstream system.

**Three validation layers:**

**Layer 1 — Format Validation**
Does the extracted value match the expected format?
```csharp
// Date field extracted as "O1/15/2025" (OCR misread 0 as O)
// Format validation catches this before it reaches the database
if (!DateTime.TryParse(extractedDate, out _))
    flagForReview(fieldName, "Invalid date format", extractedDate);
```

**Layer 2 — Cross-Field Consistency**
Do related fields agree with each other?
```
Admission date: 2025-01-15
Discharge date: 2025-01-12   ← discharge BEFORE admission — OCR error
```
Cross-field validation catches logical impossibilities that format validation misses.

**Layer 3 — Business Rule Validation**
Does the extracted data satisfy domain rules?
```
Policy number extracted: "XYZ-12345"
Expected format for BlueCross MA: "BCB-[6 digits]"  ← mismatch — wrong insurer's form?
```

**Combined routing:**
```
Extraction complete
      ↓
Format validation pass? → if fail → human review queue
      ↓
Cross-field consistency? → if fail → human review queue
      ↓
Business rules pass? → if fail → human review queue (with reason)
      ↓
All pass + all confidence > 0.90 → auto-process
```

---

## Section 6 — Confidence Routing (Your Production Pattern)

The confidence routing table from JM Family production:

| Confidence | Action | Reason |
|-----------|--------|--------|
| > 0.90 | Auto-process | High confidence — no human needed |
| 0.70 – 0.90 | Human review queue | Uncertain — flag specific fields for reviewer |
| < 0.70 | Dead-letter | Too low to trust — re-scan or manual entry required |

**Healthcare adjustment (VitalCare):**
In clinical settings, the thresholds are tighter because PHI errors have patient safety consequences:
- > 0.95 → Auto-process
- 0.80 – 0.95 → Pharmacist review queue
- < 0.80 → Dead-letter + alert

---

## Quick-Reference Interview Answers

**Q: How do you handle poor-quality document scans before OCR?**
"Three-stage pre-processing: de-noise (remove scan artifacts and speckle), de-skew (straighten documents placed at an angle — even 3 degrees kills accuracy), and adaptive binarization (convert to black-and-white using local thresholds to handle uneven lighting). At JM Family, adding de-skew alone moved our average confidence score from 0.78 to 0.91 on dealer forms."

**Q: When would you recommend John Snow Labs instead of Azure Document Intelligence?**
"When the document is clinical narrative, not form structure. Azure DI is exceptional at extracting structured fields from fixed or variable-layout forms — invoices, insurance cards, lab order forms. John Snow Labs wins when you need to understand medical language: extracting diagnoses from SOAP notes, normalizing medications to RxNorm codes, de-identifying 18 HIPAA identifiers from discharge summaries. In a healthcare platform, I'd use both: DI extracts the form data, JSL interprets the clinical text embedded in those forms."

**Q: What happens when your OCR confidence is low?**
"Low confidence never silently fails. Our pipeline has three tiers: above threshold, auto-process; middle band, route to human review queue with the specific low-confidence fields flagged so the reviewer knows exactly what to check; below minimum, dead-letter with an alert. In a healthcare context, the thresholds are tighter and the human review queue routes to a licensed professional — a pharmacist for medication data, a clinical coder for diagnosis codes. No PHI data is modified or discarded automatically below the confidence threshold."
