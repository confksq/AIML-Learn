# Module 3 — Computer Vision Fundamentals
**Part 1: AI Fundamentals | AI Solutions Architect Curriculum**
*Created: 2026-06-30*

---

## Why This Module Matters

Computer Vision is one of the five core AI workload types (alongside NLP, Speech, Knowledge Mining, and GenAI). As an Azure AI Architect, you may not build CV systems daily at JM Family, but you need to:

- Know which Azure service to recommend when a CV requirement arrives
- Understand what Custom Vision is and when to use it over the prebuilt API
- Know the OCR landscape — critical because it connects directly to Document Intelligence (Module 8) and your JMA form recognition work
- Handle Responsible AI questions around Face API (it has a Limited Access policy)

**Key connection:** OCR (this module) → Document Intelligence (Module 8) → AI Search (Module 9) → RAG (Module 13). You've built the downstream — this fills in the upstream.

---

**Running example:**
> *JM Family wants to process vehicle inspection photos, extract text from dealer agreements, and verify identity documents submitted by dealers.*

---

## Topic 3.1 — Computer Vision Concepts

---

### 1. What Is Computer Vision?

Computer Vision is the field of AI that enables machines to interpret and understand visual information from images and video.

```
Human sees:   photo → brain → "that's a Ford F-150 with a dent on the rear bumper"
CV model sees: photo → pixels → feature maps → "vehicle, truck, damage_detected, location: rear"
```

The model does not "understand" the image the way a human does — it detects patterns in pixel values that correlate with the labels it was trained on.

---

### 2. Core CV Tasks

| Task | What It Does | Example |
|---|---|---|
| **Image Classification** | Assigns a label to the whole image | "This is a damaged vehicle" |
| **Object Detection** | Finds objects + draws bounding boxes | "Ford logo at [x:120, y:45, w:80, h:30]" |
| **Semantic Segmentation** | Labels every pixel by category | "These pixels = car, these = road, these = sky" |
| **OCR / Text Reading** | Reads text in an image | "Invoice #INV-2024-8823" extracted from photo |
| **Face Detection** | Finds faces in an image | "3 faces detected at coordinates..." |

---

### 3. Classification vs Detection vs Segmentation

```
CLASSIFICATION:
┌─────────────────────────────────┐
│                                 │
│  [photo of damaged car]         │   → Label: "damaged_vehicle"
│                                 │      One label for the whole image
└─────────────────────────────────┘

OBJECT DETECTION:
┌─────────────────────────────────┐
│  ┌──────┐         ┌──────────┐  │
│  │FORD  │         │  dent    │  │   → [vehicle, 0.97, (10,20,200,150)]
│  │ logo │         │  damage  │  │   → [logo, 0.91, (15,25,80,40)]
│  └──────┘         └──────────┘  │   → [damage, 0.85, (210,90,120,80)]
└─────────────────────────────────┘

SEMANTIC SEGMENTATION:
┌─────────────────────────────────┐
│ ████████████████████████████    │   → each pixel labeled:
│ ████car████████car█████sky█████ │      car = blue pixels
│ ████car████████car████████████  │      road = gray pixels
│ ████████road█████████road██████ │      sky = white pixels
└─────────────────────────────────┘
```

**Architect decision:** For JM Family vehicle inspection photos → object detection. For dealer document images → OCR. For "is this a valid document type" → classification.

---

### 4. How CV Models Learn

Same as any ML model — supervised learning with labeled training data:

```
Training data: 10,000 vehicle images
Labels: [{"category": "truck", "bbox": [x,y,w,h]}, {"category": "dent", "bbox": [...]}]

Model learns: what pixel patterns = "truck", what patterns = "dent"

At inference: new photo → model outputs predictions with confidence scores
```

More labeled data = better model. Azure Custom Vision makes this accessible without ML expertise.

---

## Topic 3.2 — Azure AI Vision Service

---

### 1. What Azure AI Vision Does

Azure AI Vision (Image Analysis 4.0 — current version) is Microsoft's prebuilt computer vision API. No training required — it uses a foundation model already trained on billions of images.

```
POST https://cog-jma-dev-vision.cognitiveservices.azure.com/computervision/imageanalysis:analyze

Input: image URL or binary
Output: JSON with detected objects, tags, captions, text, etc.
```

---

### 2. Key Capabilities — Image Analysis 4.0

| Feature | What It Returns | JM Family Use |
|---|---|---|
| **Caption** | One-sentence description of the image | "A white pickup truck with front damage" |
| **Dense Captions** | Captions for each region of the image | Caption per detected area |
| **Tags** | Labels with confidence scores | ["vehicle", "truck", "damage", "outdoor"] |
| **Object Detection** | Objects with bounding boxes | Vehicle parts, logos |
| **OCR / Read** | Text extracted from image | Dealer sticker text, VIN plate |
| **Smart Cropping** | Best thumbnail crop suggestion | Thumbnail for vehicle listings |
| **Background Removal** | Image without background | Product photos |
| **People Detection** | Detects people in the scene | Dealership photos |

---

### 3. Calling Vision API in C#

```csharp
using Azure;
using Azure.AI.Vision.ImageAnalysis;

// Setup
var endpoint = new Uri("https://cog-jma-dev-vision.cognitiveservices.azure.com/");
var credential = new DefaultAzureCredential(); // Managed Identity
var client = new ImageAnalysisClient(endpoint, credential);

// Analyze image
var result = await client.AnalyzeAsync(
    ImageUrl: new Uri("https://jmastorage.blob.core.windows.net/vehicles/truck-001.jpg"),
    visualFeatures: VisualFeatures.Caption | VisualFeatures.Tags | VisualFeatures.Objects | VisualFeatures.Read,
    new ImageAnalysisOptions { Language = "en" }
);

// Caption
Console.WriteLine($"Caption: {result.Value.Caption.Text} ({result.Value.Caption.Confidence:P})");

// Tags
foreach (var tag in result.Value.Tags.Values)
    Console.WriteLine($"Tag: {tag.Name} ({tag.Confidence:P})");

// Objects
foreach (var obj in result.Value.Objects.Values)
    Console.WriteLine($"Object: {obj.Tags[0].Name} at [{obj.BoundingBox}]");

// Text (OCR)
foreach (var line in result.Value.Read.Blocks.SelectMany(b => b.Lines))
    Console.WriteLine($"Text: {line.Text}");
```

---

### 4. What Image Analysis 4.0 Cannot Do

| Limitation | Solution |
|---|---|
| Doesn't know your specific categories (e.g., "JMA inspection grade A/B/C") | Use Custom Vision |
| OCR from complex multi-page documents | Use Document Intelligence |
| Structured field extraction from forms | Use Document Intelligence |
| Real-time video analysis | Use Video Indexer (separate service) |

---

## Topic 3.3 — Azure AI Custom Vision

---

### 1. When to Use Custom Vision

Use Custom Vision when the prebuilt Vision API doesn't know your specific categories:

```
Prebuilt Vision API knows: "vehicle", "truck", "car", "damage"
Does NOT know: "JMA Grade A inspection", "JMA Grade B inspection", "dealer logo - Ford vs Toyota"

Custom Vision: you teach it YOUR categories with YOUR labeled images
```

---

### 2. Two Project Types

| Type | Task | Example |
|---|---|---|
| **Classification** | Whole image gets a label | "This inspection photo = Grade A / B / C" |
| **Object Detection** | Detect + locate objects with bounding boxes | "Find the VIN plate in this photo, draw a box around it" |

---

### 3. The Custom Vision Workflow

```
Step 1: CREATE PROJECT
  Portal: customvision.ai → New Project
  Choose: Classification or Object Detection
  Choose: domain (General, Food, Retail, Compact for edge export)

Step 2: UPLOAD & LABEL IMAGES
  Minimum: 15 images per tag/class (more = better)
  Classification: upload images, assign tag to each
  Object Detection: upload images, draw bounding boxes per object

Step 3: TRAIN
  Click "Train" → Quick Training (minutes) or Advanced Training (hours)
  Model evaluates on a held-out set automatically

Step 4: EVALUATE
  Precision, Recall, AP (Average Precision) per tag
  Iterate: add more images for low-performing tags

Step 5: PUBLISH
  Publish to prediction endpoint
  Call via REST API (same pattern as Vision API)

Step 6: EXPORT (optional)
  Export as ONNX → run on-device (no network needed)
  Export as TensorFlow → Python apps
  Export as Docker container → Azure Container Apps
```

---

### 4. Custom Vision API Call (after publishing)

```csharp
var predictionClient = new CustomVisionPredictionClient(
    new ApiKeyServiceClientCredentials("prediction-key"),
    new HttpClient(),
    true)
{
    Endpoint = "https://cog-jma-dev-customvision.cognitiveservices.azure.com/"
};

// Classify an image
var result = await predictionClient.ClassifyImageUrlAsync(
    projectId: Guid.Parse("your-project-id"),
    publishedModelName: "JMAInspectionGrades",
    new ImageUrl("https://storage.jmfamily.com/inspections/photo-001.jpg")
);

foreach (var prediction in result.Predictions.OrderByDescending(p => p.Probability))
    Console.WriteLine($"{prediction.TagName}: {prediction.Probability:P}");
// Output:
// Grade A: 87.3%
// Grade B: 10.1%
// Grade C: 2.6%
```

---

### 5. Exporting to ONNX for Edge Deployment

```
Use case: JMA dealer service bay — no reliable internet
Solution: export Custom Vision model as ONNX, embed in tablet app

Export → ONNX → runs locally on Windows tablet
No API call needed
Inspection grading works offline
```

---

## Topic 3.4 — Azure AI Face Service

---

### 1. What Face Service Does

| Capability | Description |
|---|---|
| **Face Detection** | Find faces in image, return bounding boxes |
| **Face Attributes** | Age estimate, emotion, glasses, head pose, blur, exposure |
| **Face Verification** | Is face A the same person as face B? (1:1 matching) |
| **Face Identification** | Which person in the group is this? (1:many matching) |
| **Face Grouping** | Group unknown faces by similarity |
| **Find Similar** | Find faces that look like this one |

---

### 2. Limited Access Policy — Important

> **Microsoft requires an application form to access Face Identification, Verification, and Grouping features.**

This is part of Microsoft's Responsible AI commitment. Face recognition has high potential for misuse (surveillance, unauthorized tracking).

```
Who needs to apply:
  - Any company using Face Identification (who is this person?)
  - Any company using Verification for access control / identity verification

Who can use freely:
  - Face detection (just finding faces, no identity)
  - Face attributes (emotion, glasses, age estimate)
```

**JM Family context:** If you need to verify dealer representative identity from a photo ID, you must apply for Limited Access. Expect 10-business-day review.

---

### 3. Responsible AI for Face Service

```
WHAT YOU MUST NOT DO:
  ✗ Build surveillance systems that track people without consent
  ✗ Use emotion recognition for employment decisions
  ✗ Build law enforcement facial recognition without authorization
  ✗ Identify individuals in public without clear disclosure

WHAT IS ACCEPTABLE:
  ✓ Verify a user's identity for account access (with consent)
  ✓ Detect whether a photo contains faces (for content moderation)
  ✓ Group similar faces in a personal photo app
```

---

## Topic 3.5 — Reading Text with OCR

---

### 1. OCR vs Read API — Key Distinction

There are two ways to extract text with Azure Vision:

| | OCR (legacy) | Read API (current) |
|---|---|---|
| **Best for** | Simple, single-page, printed text | Multi-page, handwritten, complex layouts |
| **Async?** | Synchronous (returns immediately) | Asynchronous (poll for result) |
| **Handwriting?** | Limited | Yes — good quality |
| **Multi-page docs?** | No | Yes (PDF, TIFF) |
| **Status** | Deprecated — use Read | Current standard |

**Rule:** Always use **Read API** (now part of Image Analysis 4.0). The old OCR endpoint is being retired.

---

### 2. OCR vs Document Intelligence — Critical Distinction for JMA

This is a common architect confusion:

```
READ API (Azure AI Vision)
  ├── Input: image file (JPG, PNG, PDF)
  ├── Output: raw text, line by line, with bounding boxes
  ├── Use: "get the words out of this image"
  └── Does NOT extract structured fields

DOCUMENT INTELLIGENCE (Azure AI Document Intelligence)
  ├── Input: image or PDF
  ├── Output: structured JSON — named fields with values
  ├── Invoice model → {VendorName, TotalAmount, InvoiceDate, LineItems[]}
  ├── Use: "extract specific fields from this form type"
  └── Internally USES OCR — but adds field extraction on top
```

**JM Family rule:** For dealer agreements and invoice extraction → Document Intelligence (Module 8), not raw Vision OCR.

---

### 3. Read API in C# (Part of Image Analysis 4.0)

```csharp
// Text extraction is part of Image Analysis — same client
var result = await client.AnalyzeAsync(
    ImageUrl: new Uri("https://storage.jmfamily.com/docs/dealer-agreement-scan.jpg"),
    visualFeatures: VisualFeatures.Read
);

// Navigate the result hierarchy:
// Result → Read → Blocks → Lines → Words
foreach (var block in result.Value.Read.Blocks)
{
    foreach (var line in block.Lines)
    {
        Console.WriteLine($"Line: '{line.Text}'");
        Console.WriteLine($"  at: [{string.Join(", ", line.BoundingPolygon)}]");

        foreach (var word in line.Words)
            Console.WriteLine($"    Word: '{word.Text}' confidence: {word.Confidence:P}");
    }
}
```

---

### 4. Multi-Language and Handwriting

```
Supported: 164 languages for printed text
Handwriting: English, Chinese Simplified, French, German, Italian, Japanese, Korean, Portuguese, Spanish

For JM Family: English primary, Spanish for dealer communications
```

---

### 5. OCR Integration Pattern — Connecting to the JMA Pipeline

```
JMA Document Pipeline (OCR layer):

Dealer uploads scanned agreement
        │
        ▼
Azure Blob Storage (raw-docs container)
        │
        ▼ (Event Grid trigger)
Azure Function (JmaDocProcessor)
        │
        ├── Azure Document Intelligence → extract fields
        │   (Invoice model or custom template model)
        │         │
        │         ▼
        │   Structured JSON → Cosmos DB
        │
        └── Azure AI Vision (Read) → full text
                  │
                  ▼
            AI Search Push API → indexed for search
```

You built downstream parts of this at JMA. The OCR layer (Vision Read / Document Intelligence) is the entry point.

---

## Topic R3 — Recall: Module 3 Review & Quiz

---

**Q1.** A JM Family dealer takes a photo of a VIN plate on a vehicle. You need to extract the VIN number text from the photo. Which service and which feature?

> **A:** Azure AI Vision — Image Analysis 4.0 with the Read feature (VisualFeatures.Read). The Read feature extracts text from images with bounding box coordinates. If it were a structured form (like a vehicle registration document with specific named fields), you'd use Document Intelligence instead.

---

**Q2.** JM Family wants to automatically classify vehicle inspection photos into 3 grades: A (excellent), B (minor damage), C (major damage). Which service?

> **A:** Azure AI Custom Vision — Image Classification project. The prebuilt Vision API doesn't know JMA's grading system. You train Custom Vision with labeled inspection photos (minimum 15 per grade). After training, publish and call the prediction endpoint.

---

**Q3.** What is the difference between Image Classification and Object Detection in Custom Vision?

> **A:** Classification assigns one label to the whole image ("this photo = Grade B"). Object Detection finds and locates multiple objects within the image with bounding boxes ("damage detected at [x:120, y:45, w:80, h:60], VIN plate at [x:200, y:300, w:150, h:40]"). Use detection when you need to know WHERE something is, not just IF it's there.

---

**Q4.** A developer says "I'll use Azure AI Vision OCR to extract invoice fields from dealer PDF invoices." What's wrong with this?

> **A:** Vision OCR (Read API) extracts raw text only — it doesn't know what "vendor name" or "invoice total" means. For structured field extraction from invoices, use Azure Document Intelligence with the Invoice prebuilt model. It returns named JSON fields (VendorName, TotalAmount, InvoiceDate, LineItems) not just raw text.

---

**Q5.** Why does Face Identification require a Microsoft Limited Access application?

> **A:** Face Identification (1:many — who is this person?) carries high risk of misuse: unauthorized surveillance, tracking individuals without consent, discriminatory use. Microsoft's Responsible AI commitment requires organizations to apply for access and agree to usage terms. Basic face detection (finding faces in an image) and face attributes (emotion, age estimate) are freely available without the application.

---

**Q6.** A JM Family service bay has unreliable internet. They want to use AI to classify vehicle damage in real time from a tablet app. How?

> **A:** Train a Custom Vision object detection model, then export it as ONNX. Embed the ONNX model in the tablet app (Windows or iOS). The model runs locally — no network call needed. Results are generated on-device and synced to the cloud when internet is available.

---

## Memory Hooks

- **"Classification = whole image label, Detection = where are objects, Segmentation = every pixel"**
- **"Vision API = prebuilt, Custom Vision = your categories"**
- **"Read API for OCR, Document Intelligence for structured fields"**
- **"Face Identification = Limited Access required — Responsible AI"**
- **"ONNX export = run Custom Vision offline on edge device"**
- **"Vision feeds into Document Intelligence feeds into AI Search feeds into RAG"**
- **"Image Analysis 4.0 = caption + tags + objects + OCR in one API call"**

---

## Interactive Learning Ideas

### Exercise 1 — Vision Studio Hands-On (15 min)
Go to portal.vision.cognitive.azure.com → Image Analysis → try with a JMA vehicle photo or any car image:
- What caption does it generate?
- What tags and confidence scores?
- Does it detect any objects?
- Can it read any text visible in the image?

### Exercise 2 — OCR vs Document Intelligence Comparison (15 min)
Take a scanned invoice PDF. Run it through:
1. Azure AI Vision Read API → note: raw text output, no field names
2. Azure Document Intelligence Invoice model → note: structured JSON with VendorName, TotalAmount, etc.

Compare the outputs. This is the clearest way to understand the distinction.

### Exercise 3 — Custom Vision Project (30 min)
Create a Custom Vision project at customvision.ai:
- Task: Image Classification
- Tags: 2 categories of your choice (e.g., "sedan" vs "truck")
- Upload 5+ images per tag from Google Images
- Train and check Precision/Recall
- Test with a new image

### Exercise 4 — Architect Decision Quiz
For each JMA scenario, choose the right service (Vision API / Custom Vision / Document Intelligence / Face Service):

| Scenario | Service |
|---|---|
| Read text from a scanned dealer agreement photo | ? |
| Classify vehicle inspection photos into A/B/C grades | ? |
| Extract vendor name + total from invoice PDF | ? |
| Detect faces in dealership event photos | ? |
| Verify that a dealer rep's selfie matches their ID photo | ? |
| Add image captions to vehicle listing photos | ? |

### Exercise 5 — Connect to JMA Pipeline (10 min)
Look at JMA's `cog-jma-dev-frm-recognizer` (Document Intelligence resource). Trace the full flow:
- Where does the document come from? (Blob Storage)
- Which DI model processes it? (custom template or invoice?)
- Where does the output go? (Cosmos DB? AI Search?)
- Where does Vision OCR fit vs where does DI fit in this pipeline?

---

*Previous: Module 2 — Azure AI Services Overview*
*Next: Module 4 — Natural Language Processing*
*Connects to: Module 8 (Document Intelligence), Module 9 (AI Search), Module 13 (RAG)*
