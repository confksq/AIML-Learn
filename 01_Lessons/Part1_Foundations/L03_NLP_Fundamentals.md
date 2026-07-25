# Module 4 — Natural Language Processing
**Part 1: AI Fundamentals | AI Solutions Architect Curriculum**
*Created: 2026-06-30*

---

## Why This Module Matters

NLP is the foundation of everything in Part 3 — LLMs, RAG, Semantic Kernel, AI Agents — all build on NLP concepts. Since you already know how LLMs work, this module will connect the classical NLP building blocks to what you already understand at the deep level.

**Key connections you'll see:**
- Tokenization here → BPE/WordPiece you learned in Module 11.2
- NER here → Custom NER you saw in Module 7
- Sentiment here → used in RAG output validation
- CLU (intents/entities) here → the predecessor to modern AI Agents
- Question Answering here → simplified RAG (retrieval from a fixed KB)

---

**Running example:**
> *JM Family wants to analyze dealer support tickets — detect sentiment, extract key entities (VehicleMake, DealerCode), classify intent (complaint vs inquiry vs escalation), and answer common FAQs automatically.*

---

## Topic 4.1 — NLP Concepts

---

### 1. What Is NLP?

Natural Language Processing is the field of AI that enables computers to understand, interpret, and generate human language.

```
Human writes: "The Ford F-150 delivery was 3 weeks late and the wrong color."

NLP tasks on this sentence:
  Language Detection   → English
  Sentiment            → Negative (0.12 positive, 0.88 negative)
  Key Phrases          → ["Ford F-150 delivery", "3 weeks late", "wrong color"]
  Named Entities       → [Ford = Organization, F-150 = Product]
  PII Detection        → none found
  Summarization        → "Late delivery of wrong-color F-150"
  Intent (CLU)         → Complaint
  Entities (CLU)       → {VehicleModel: "F-150", Issue: "delivery_delay"}
```

One sentence → multiple NLP tasks, each providing a different insight.

---

### 2. Classical NLP Pipeline (Pre-LLM)

Before transformers and LLMs, NLP required explicit pipeline steps:

```
Raw Text
    │
    ▼ Tokenization
"The"  "Ford"  "F-150"  "delivery"  "was"  "3"  "weeks"  "late"
    │
    ▼ Stop Word Removal
"Ford"  "F-150"  "delivery"  "3"  "weeks"  "late"
    │
    ▼ Stemming/Lemmatization
"ford"  "f-150"  "deliveri"  "3"  "week"  "late"
    │
    ▼ POS Tagging
ford(NOUN)  f-150(NOUN)  deliveri(NOUN)  3(NUM)  week(NOUN)  late(ADJ)
    │
    ▼ NER
[ORG: ford]  [PRODUCT: f-150]  [TIME: 3 weeks]
    │
    ▼ Feature Vector → ML Model → Intent/Sentiment/Category
```

**Why you need to know this:** Azure AI Language still exposes these steps as explicit API features. Understanding what each step does helps you choose which API to call and debug unexpected outputs.

---

### 3. Key NLP Concepts

#### Tokenization (Classical vs LLM)

| | Classical NLP | LLM (BPE/WordPiece) |
|---|---|---|
| **Unit** | Word or character | Subword piece |
| **"unhappiness"** | ["unhappiness"] | ["un", "happiness"] or ["un", "hap", "pi", "ness"] |
| **Purpose** | Split text into words | Split text into model vocabulary items |
| **You saw this in** | This module | Module 11.2 |

Classical tokenization = split on spaces/punctuation. LLM tokenization = learned subword splits.

#### Stop Words
Common words with little meaning: "the", "is", "at", "which", "on". Removed before feature extraction in classical ML — irrelevant to meaning, just noise.

#### Stemming vs Lemmatization

| | Stemming | Lemmatization |
|---|---|---|
| **What** | Crude cut (remove suffixes) | Returns dictionary form |
| **"running"** | "runn" | "run" |
| **"better"** | "better" | "good" |
| **Speed** | Fast | Slower (needs vocabulary) |

#### Part-of-Speech (POS) Tagging
Label each word with its grammatical role: NOUN, VERB, ADJ, ADV, PREP, NUM, etc.
Used by NER to identify that "Ford" (NOUN, followed by another NOUN) is likely an entity.

#### Named Entity Recognition (NER)
Identify and classify real-world entities in text:

| Entity Type | Example |
|---|---|
| Person | "John Smith" |
| Organization | "Ford Motor Company", "JM Family" |
| Location | "Atlanta, Georgia" |
| Date/Time | "March 15, 2026", "3 weeks" |
| Product | "F-150", "iPhone" |
| Money | "$47,000" |
| Phone | "+1-404-555-1234" |
| Email | "dealer@ford.com" |

#### Sentiment Analysis
Assigns a polarity score: positive, negative, neutral (and mixed).

```
"The delivery was fast but the vehicle had scratches."
→ Overall:  Mixed
→ Sentence 1: Positive (delivery was fast)
→ Sentence 2: Negative (vehicle had scratches)
```

Opinion Mining goes further — extracts the opinion target:
```
"The delivery was fast but the vehicle had scratches."
→ [fast → delivery] positive
→ [scratches → vehicle] negative
```

---

## Topic 4.2 — Azure AI Language Service

---

### 1. What Azure AI Language Covers

One service, many capabilities:

| Capability | What It Does |
|---|---|
| **Sentiment Analysis** | Positive/Negative/Neutral + opinion mining |
| **Key Phrase Extraction** | Most important phrases in text |
| **Named Entity Recognition** | Standard entity types (Person, Org, Location, etc.) |
| **Entity Linking** | Links entities to Wikipedia (disambiguates "Apple" = company vs fruit) |
| **Language Detection** | Which language is this text? (confidence score) |
| **PII Detection** | Finds and redacts personal information |
| **Text Summarization** | Abstractive or extractive summary |
| **Custom NER** | Your own entity types (covered in Module 7) |
| **Question Answering** | FAQ-style Q&A from a knowledge base |
| **CLU** | Conversational Language Understanding (intents + entities) |
| **Text Translation** | Part of Azure AI Translator (separate service) |

---

### 2. Calling the Language Service in C#

```csharp
using Azure;
using Azure.AI.TextAnalytics;

var endpoint = new Uri("https://cog-jma-prod-language.cognitiveservices.azure.com/");
var credential = new DefaultAzureCredential(); // Managed Identity
var client = new TextAnalyticsClient(endpoint, credential);

string dealerTicket = "The Ford F-150 delivery was 3 weeks late and the paint was scratched.";
```

---

### 3. Sentiment Analysis

```csharp
var response = await client.AnalyzeSentimentAsync(dealerTicket, options: new AnalyzeSentimentOptions
{
    IncludeOpinionMining = true
});

var result = response.Value;
Console.WriteLine($"Overall: {result.Sentiment} (pos:{result.ConfidenceScores.Positive:P0}, neg:{result.ConfidenceScores.Negative:P0})");

foreach (var sentence in result.Sentences)
{
    Console.WriteLine($"  Sentence: '{sentence.Text}' → {sentence.Sentiment}");
    foreach (var opinion in sentence.Opinions)
    {
        Console.WriteLine($"    {opinion.Target.Text}: {opinion.Target.Sentiment}");
        foreach (var assessment in opinion.Assessments)
            Console.WriteLine($"      Assessment: {assessment.Text} ({assessment.Sentiment})");
    }
}

// Output:
// Overall: Negative (pos:8%, neg:92%)
//   Sentence: "The Ford F-150 delivery was 3 weeks late..." → Negative
//     delivery: Negative
//       Assessment: late (Negative)
//   Sentence: "...the paint was scratched." → Negative
//     paint: Negative
//       Assessment: scratched (Negative)
```

---

### 4. Key Phrase Extraction

```csharp
var keyPhraseResponse = await client.ExtractKeyPhrasesAsync(dealerTicket);

Console.WriteLine("Key Phrases:");
foreach (var phrase in keyPhraseResponse.Value)
    Console.WriteLine($"  - {phrase}");

// Output:
// Key Phrases:
//   - Ford F-150 delivery
//   - 3 weeks
//   - scratched paint
```

---

### 5. Named Entity Recognition

```csharp
var nerResponse = await client.RecognizeEntitiesAsync(dealerTicket);

foreach (var entity in nerResponse.Value)
    Console.WriteLine($"{entity.Category} ({entity.SubCategory}): '{entity.Text}' — {entity.ConfidenceScore:P0}");

// Output:
// Organization: 'Ford' — 95%
// Product: 'F-150' — 88%
// Quantity (Duration): '3 weeks' — 97%
```

---

### 6. PII Detection — Critical for JMA

Before sending any text to Azure OpenAI or external services, run PII detection:

```csharp
var piiResponse = await client.RecognizePiiEntitiesAsync(dealerTicket);

// Check for PII
if (piiResponse.Value.Any())
{
    Console.WriteLine("PII found — redacting before LLM call:");
    Console.WriteLine($"Redacted text: {piiResponse.Value.RedactedText}");

    foreach (var entity in piiResponse.Value)
        Console.WriteLine($"  PII: {entity.Category} — '{entity.Text}'");
}
else
{
    // Safe to send to LLM
    await SendToAzureOpenAIAsync(dealerTicket);
}
```

**JMA use case:** Dealer support tickets may contain dealer employee names, phone numbers, email addresses. Strip these before sending to Azure OpenAI for summarization.

---

### 7. Text Summarization

```csharp
// Extractive summarization — returns original sentences
var extractiveSummaryOp = await client.ExtractiveSummarizeAsync(
    WaitUntil.Completed,
    new List<string> { longDealerReport }
);

// Abstractive summarization — generates new summary text
var abstractiveSummaryOp = await client.AbstractiveSummarizeAsync(
    WaitUntil.Completed,
    new List<string> { longDealerReport },
    options: new AbstractiveSummarizeOptions { SentenceCount = 2 }
);
```

**Extractive:** picks the most important original sentences. Fast, factually safe.
**Abstractive:** writes a new summary (like a human would). More natural, but can hallucinate slightly.

For JMA financial/legal documents → use extractive. For dealer communications → abstractive is fine.

---

### 8. Batch Processing — Analyze Multiple Documents

```csharp
// Process multiple tickets at once — more efficient than one-at-a-time
var documents = new List<string>
{
    "Dealer ATL-001: F-150 delivered late, paint damaged.",
    "Dealer CHI-042: Excellent service, truck arrived early.",
    "Dealer MIA-019: Wrong vehicle model delivered, need immediate resolution."
};

// Batch sentiment
var batchResponse = await client.AnalyzeSentimentBatchAsync(documents);
foreach (var doc in batchResponse.Value)
    Console.WriteLine($"Doc {doc.Id}: {doc.DocumentSentiment.Sentiment}");
```

**Batch limit:** 25 documents per request, 5,120 characters per document. For larger volumes, chunk and batch.

---

## Topic 4.3 — Question Answering

---

### 1. What Question Answering Is

Azure AI Language Question Answering lets you build a FAQ knowledge base and query it with natural language questions.

```
Knowledge Base:
  Q: What are your delivery timeframes?
  A: Standard delivery is 5-7 business days. Expedited is 2-3 business days.

  Q: How do I report a damaged vehicle?
  A: Submit a damage report within 24 hours of delivery via the dealer portal.

User asks: "My truck arrived damaged, what should I do?"
  → QA finds best matching answer
  → Returns: "Submit a damage report within 24 hours..."
  → Confidence score: 0.87
```

This is **simplified RAG** — retrieval from a fixed, structured knowledge base. Not as flexible as full RAG (Module 13), but simpler to set up and maintain.

---

### 2. QnA Maker vs Question Answering

| | QnA Maker (legacy) | Question Answering (current) |
|---|---|---|
| **Status** | Retired March 2025 | Current standard |
| **Location** | Separate resource | Part of Azure AI Language |
| **Multi-turn** | Yes | Yes (improved) |
| **Active learning** | Yes | Yes (improved) |

Always use **Question Answering** (inside Azure AI Language). QnA Maker is gone.

---

### 3. Creating a Knowledge Base

```
Language Studio → Question Answering → Create project

Data sources:
  - Upload FAQ documents (Word, PDF, Excel, URL)
  - Enter Q&A pairs manually
  - Import from existing URL (extracts Q&A from web pages)

Multi-turn conversations:
  Q: How do I return a vehicle?
  A: We handle returns within 30 days. [Follow-up prompts:]
    → "What documents do I need?"
    → "Is there a restocking fee?"
    → "How long does the process take?"
```

---

### 4. Calling Question Answering from C#

```csharp
var client = new QuestionAnsweringClient(endpoint, credential);
var project = new QuestionAnsweringProject("jmf-dealer-faq", "production");

var response = await client.GetAnswersAsync(
    "My vehicle arrived damaged, what do I do?",
    project
);

foreach (var answer in response.Value.Answers.OrderByDescending(a => a.Confidence))
{
    Console.WriteLine($"Answer: {answer.Answer}");
    Console.WriteLine($"Confidence: {answer.Confidence:P0}");
    Console.WriteLine($"Source: {answer.Source}");
    break; // Take top answer
}

// Output:
// Answer: Submit a damage report within 24 hours of delivery via the dealer portal.
// Confidence: 87%
// Source: DealerHandbook.pdf
```

---

### 5. When to Use QA vs Full RAG

| | Question Answering | Full RAG (Module 13) |
|---|---|---|
| **Content** | Fixed FAQ pairs | Any documents (unstructured) |
| **Updates** | Manual additions | Ingest and re-index |
| **Answer style** | Fixed text answers | LLM generates answer |
| **Scale** | Hundreds of Q&A pairs | Thousands of documents |
| **Setup** | Simple — Language Studio | Complex — AI Search + OpenAI |
| **JMA use** | Simple dealer FAQ bot | Enterprise document Q&A |

---

## Topic 4.4 — Conversational Language Understanding (CLU)

---

### 1. What CLU Does

CLU understands the **intent** (what does the user want?) and **entities** (what specific things are they talking about?) from natural language input.

```
User says: "I want to check the status of my F-150 order for dealer ATL-001"

CLU extracts:
  Intent:  CheckOrderStatus       (confidence: 0.95)
  Entities:
    VehicleModel: "F-150"
    DealerCode: "ATL-001"
```

Your application then takes the intent and entities and calls the right function:
```csharp
if (intent == "CheckOrderStatus")
    await orderService.GetStatusAsync(vehicleModel, dealerCode);
```

---

### 2. Intents and Entities

**Intent** = what the user wants to do
- CheckOrderStatus
- ReportDamage
- RequestQuote
- CancelOrder
- TrackDelivery

**Entity** = specific data values extracted from the utterance

| Entity Type | What it is | Example |
|---|---|---|
| **Learned** | Model learns from examples | VehicleModel → learns "F-150", "Silverado", "Ram 1500" |
| **List** | Fixed vocabulary | DealerRegion → ["Northeast", "Southeast", "Midwest"] |
| **Prebuilt** | Azure handles it | DateTime → "next Tuesday", "March 15" |
| **Regex** | Pattern-based | DealerCode → `[A-Z]{3}-\d{3}` |

---

### 3. CLU Workflow

```
Step 1: Language Studio → Conversational Language Understanding → New Project

Step 2: Define Intents
  Add utterances (example sentences) per intent:
  CheckOrderStatus:
    - "What's the status of my order?"
    - "Check order for dealer {DealerCode}"
    - "Where is my {VehicleModel} delivery?"
    - "Track my F-150 for ATL-001"
    (minimum 15 utterances per intent — more is better)

Step 3: Tag Entities in Utterances
  "Check order for dealer [ATL-001]"
                              ↑ tag as DealerCode entity

Step 4: Train → Evaluate
  Precision/Recall per intent
  Confusion matrix (which intents get confused with each other)

Step 5: Deploy → Production slot

Step 6: Call from app
```

---

### 4. Calling CLU from C#

```csharp
var client = new ConversationAnalysisClient(endpoint, credential);

var data = new
{
    analysisInput = new
    {
        conversationItem = new
        {
            text = "I want to check the status of my F-150 order for dealer ATL-001",
            id = "1",
            participantId = "user"
        }
    },
    parameters = new
    {
        projectName = "jmf-dealer-assistant",
        deploymentName = "production",
        stringIndexType = "Utf16CodeUnit"
    },
    kind = "Conversation"
};

var response = await client.AnalyzeConversationAsync(RequestContent.Create(data));
var result = response.Content.ToObjectFromJson<JsonElement>();

var topIntent = result.GetProperty("result").GetProperty("prediction").GetProperty("topIntent").GetString();
Console.WriteLine($"Intent: {topIntent}");

// Route to appropriate handler
switch (topIntent)
{
    case "CheckOrderStatus":
        await HandleCheckOrderStatus(result);
        break;
    case "ReportDamage":
        await HandleDamageReport(result);
        break;
}
```

---

### 5. CLU vs AI Agents (Modern Perspective)

You've already learned about AI Agents (Module 14). Here's how CLU relates:

| | CLU | AI Agent (Module 14) |
|---|---|---|
| **Intent detection** | Explicit — you define intents | Implicit — LLM infers intent |
| **Entity extraction** | Rule-based + learned | LLM extracts naturally |
| **Flexibility** | Only handles defined intents | Handles anything |
| **Cost** | Very cheap per call | More expensive (LLM tokens) |
| **Predictability** | High — finite intent set | Lower — LLM can surprise you |
| **JMA use** | Simple, fixed-intent chatbot | Complex agentic workflows |

**CLU is still valid** for high-volume, cost-sensitive, well-defined intent recognition. AI Agents are better for open-ended, complex conversational flows.

---

## Topic 4.5 — Text Translation

---

### 1. Azure AI Translator Service

Azure AI Translator is a **separate service** from Azure AI Language — it handles translation between 100+ languages.

```
POST https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to=es&to=fr

Body: [{"text": "The F-150 delivery is delayed by 3 weeks."}]

Response:
[{
  "detectedLanguage": {"language": "en", "score": 1.0},
  "translations": [
    {"text": "La entrega de la F-150 se retrasa 3 semanas.", "to": "es"},
    {"text": "La livraison du F-150 est retardée de 3 semaines.", "to": "fr"}
  ]
}]
```

---

### 2. Translator Capabilities

| Feature | Description |
|---|---|
| **Text Translation** | Translate to 1+ target languages in one call |
| **Language Detection** | Auto-detect source language |
| **Transliteration** | Convert script without translating (Arabic → Roman alphabet) |
| **Dictionary Lookup** | Alternative translations with examples |
| **Document Translation** | Translate entire Word/PDF/PowerPoint files (async) |
| **Custom Translator** | Domain-specific vocabulary (automotive terms, legal) |

---

### 3. Custom Translator — When to Use

Standard Translator knows general language. Custom Translator learns your domain vocabulary:

```
Standard Translator:
  "The vehicle's MSRP with iPacket digital retailing tool..."
  → Translates "iPacket" as "iPaquete" (wrong — it's a brand name)

Custom Translator (trained on JMA documents):
  → Keeps "iPacket" unchanged (learned it's a proper noun)
  → Correctly translates industry terms like "floorplan" in automotive context
```

Build a Custom Translator model when you have automotive/financial/legal terms that need consistent handling across languages.

---

### 4. C# Translation Call

```csharp
using Azure.AI.Translation.Text;

var client = new TextTranslationClient(
    new AzureKeyCredential("<key>"),
    region: "eastus"
);

var response = await client.TranslateAsync(
    targetLanguages: new[] { "es", "fr", "de" },
    content: new[] { "The F-150 delivery is delayed." }
);

foreach (var translation in response.Value[0].Translations)
    Console.WriteLine($"{translation.TargetLanguage}: {translation.Text}");
```

---

## Topic R4 — Recall: Module 4 Review & Quiz

---

**Q1.** A JM Family dealer support ticket arrives in Spanish. Your system needs to detect the language, translate it to English, then analyze sentiment. Which Azure services and in which order?

> **A:** (1) **Azure AI Translator** — detect language (auto-detection built in) and translate to English. (2) **Azure AI Language** — run sentiment analysis on the English text. Run them in sequence: Translator first, then Language. You could also use Azure AI Language's language detection feature, but Translator does both in one call (detect + translate).

---

**Q2.** What is the difference between extractive and abstractive summarization?

> **A:** Extractive summarization selects and returns the most important original sentences from the document — nothing is invented, all words come from the source. Abstractive summarization generates new text that captures the key points — more natural reading but can slightly rephrase or simplify. For JMA legal/financial documents, use extractive (factually safe). For dealer communications, abstractive is fine.

---

**Q3.** Your CLU model correctly identifies 90% of "CheckOrderStatus" intents but confuses "CancelOrder" with "ModifyOrder" 40% of the time. What do you do?

> **A:** Add more distinct utterances to both "CancelOrder" and "ModifyOrder" intents — focusing on phrases that clearly differentiate them. Review the confusion matrix to find overlapping phrases and rewrite them. If the concepts are genuinely similar in your domain, consider merging into one intent with an entity that distinguishes the action (Action: "cancel" vs "modify").

---

**Q4.** A dealer's ticket says "John Smith at dealer JMF-ATL-001 wants a refund for his F-150." Before sending this to Azure OpenAI for summarization, what should you do and how?

> **A:** Run PII detection using Azure AI Language (`RecognizePiiEntitiesAsync`). "John Smith" is a Person entity (PII). Use the `RedactedText` output which replaces it with `***` before passing to Azure OpenAI. The dealer code (JMF-ATL-001) is business data, not personal PII — check your policy on whether it needs redaction too.

---

**Q5.** How does CLU differ from full AI Agents, and when would you still choose CLU in 2026?

> **A:** CLU requires predefined intents and entity types — it's a pattern matcher. AI Agents use LLMs to infer intent and extract entities from any natural language without predefined schemas. Choose CLU when: (1) the intent set is fixed and well-defined, (2) cost is critical at high volume (CLU is much cheaper per call than LLM), (3) predictability matters (CLU only fires defined intents, agents can surprise you), or (4) latency must be <100ms.

---

## Memory Hooks

- **"Language Service = prebuilt NLP on text: sentiment, NER, PII, summary, key phrases"**
- **"PII detection before LLM — always strip personal data first"**
- **"Question Answering = simplified RAG from fixed FAQ pairs"**
- **"CLU = intents + entities — cheap, predictable, finite set"**
- **"CLU vs Agent: CLU = fixed intents, Agent = open-ended LLM"**
- **"Translator = separate service from Language"**
- **"Extractive = original sentences, Abstractive = new generated text"**
- **"Opinion Mining = who said what about what"**

---

## 2026 Updates

| Topic | Update |
|---|---|
| **QnA Maker** | Fully retired March 2025 — use Question Answering in AI Language |
| **CLU vs Copilot Studio** | Microsoft Copilot Studio (Power Platform) is the low-code successor to PVA + CLU for bot building. CLU is still the underlying engine |
| **Text Summarization** | Abstractive summarization now GA — was preview in earlier curriculum versions |
| **AI Language + OpenAI** | Many Language Service tasks can now be done with Azure OpenAI (sentiment via prompt, NER via prompt). Language Service still wins on cost and latency for high-volume structured extraction |

---

## Interactive Learning Ideas

### Exercise 1 — Language Studio Exploration (15 min)
Go to language.cognitive.azure.com → try each feature with a JMA dealer ticket you invent:
- Sentiment Analysis with Opinion Mining
- Key Phrase Extraction
- Named Entity Recognition
- PII Detection
Compare the outputs — which gives you the most actionable signal for a dealer support system?

### Exercise 2 — PII Guard in C# (20 min)
Write a C# method `SanitizeBeforeLLM(string text)` that:
1. Calls PII detection
2. Returns the redacted text if PII is found
3. Logs what was redacted (category only, not the actual PII value)
4. Returns original text if no PII found

### Exercise 3 — CLU vs Agent Decision (10 min)
For each JMA scenario, decide: CLU or AI Agent?
- Dealer types: "Check my order status for ATL-001" (one of 5 possible intents)
- Dealer types: "I have a complicated situation with a damaged vehicle and I need help understanding my options and next steps"
- Internal JMA analyst asks: "What's the trend in dealer complaints this quarter?"
- Dealer types: "Cancel order 12345"

### Exercise 4 — Build a Question Answering KB (20 min)
Go to Language Studio → Question Answering → create a project:
- Add 5 JMA-relevant Q&A pairs (e.g., delivery policy, damage reporting, return process)
- Publish to production
- Test with paraphrased versions of the questions
- Note the confidence scores — when does it fail?

### Exercise 5 — Connect to What You Know
Map each Language Service capability to where you've already seen it used in the advanced modules:
- PII detection → used in RAG pipeline (Module 13) — where exactly?
- Sentiment analysis → used in AI Agent decision making (Module 14) — how?
- NER → connects to Custom NER in Module 7 — what's the difference?
- Translation → how would you add this to the JMA EnterpriseSearch pipeline?

---

*Previous: Module 3 — Computer Vision*
*Next: Module 5 — Speech Services*
*Connects deeply to: Module 7 (Custom NER), Module 11 (LLMs — NLP is the foundation), Module 13 (RAG — PII detection), Module 14 (Agents — replaces CLU)*
