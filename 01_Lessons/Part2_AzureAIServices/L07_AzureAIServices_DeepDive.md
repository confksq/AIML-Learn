# Module 7 — Azure AI Services Deep Dive
**Part 2: AI Engineering (AI-102 Level) | AI Solutions Architect Curriculum**

---

## What You Already Know (Recap)

From your Part 3 sessions:
- **Azure OpenAI Service** — deployments, endpoints, Managed Identity, Private Endpoints, TPM quotas
- **RAG pattern** — Azure AI Search + embeddings + Azure OpenAI, orchestrated by your app
- **Tokenization, embeddings, attention** — how the model works internally
- **Content Safety** — categories, severity levels, groundedness detection

You've been using Azure AI services in practice at JM Family (Document Intelligence, Azure AI Search, Azure Functions pipeline). This module puts the **engineering backbone** behind all of that — how you manage, secure, deploy, and customize Azure AI Services at scale.

---

**Running example (used throughout):**
> *JM Family's document processing pipeline: PDF invoices land in Blob Storage → Azure Function triggers → Document Intelligence extracts fields → Azure OpenAI summarizes → results stored in Cosmos DB / AI Search.*

This is the real architecture. Every concept in this module maps to a decision you make when building or operating this pipeline.

---

## Topic 7.1 — Advanced AI Services Management

---

### 1. Azure AI Services: Single Resource vs Multi-Service vs Individual

Azure AI Services come in two provisioning flavors:

| Resource Type | What it is | When to use |
|---|---|---|
| **Azure AI Services (multi-service)** | One resource for Vision, Language, Speech, Document Intelligence, etc. | Dev/test, apps that use multiple services, simpler billing |
| **Individual service** (e.g., Azure AI Language) | One resource = one service type | Production isolation, separate billing, separate quotas per service |

**One endpoint, many services (multi-service):**
```
https://<name>.cognitiveservices.azure.com/
```

The same base URL routes to different capabilities depending on the API path:
- `/vision/v3.2/analyze` → Computer Vision
- `/language/analyze-text` → Language Service
- `/formrecognizer/documentModels/prebuilt-invoice:analyze` → Document Intelligence

**Architect decision:**
- **Dev/test or tightly coupled pipeline** → multi-service resource (easier to manage)
- **Production with different owners per service** → separate resources (separate RBAC, quotas, monitoring)

---

### 2. Authentication: API Keys vs Managed Identity

**Two ways to authenticate to Azure AI Services:**

#### Option A: API Keys (Subscription Keys)
```csharp
// Every Azure AI Services resource has two keys (Key1 / Key2)
var client = new DocumentAnalysisClient(
    new Uri("https://myaccount.cognitiveservices.azure.com/"),
    new AzureKeyCredential("abc123...")
);
```
- Two keys support **key rotation without downtime** — rotate Key1, update apps, rotate Key2
- Keys are symmetric: both work, rotate independently
- **Do not hardcode keys** — use Key Vault or environment variables

#### Option B: Managed Identity (Recommended for production)
```csharp
// No key in code at all — identity comes from the compute resource
var client = new DocumentAnalysisClient(
    new Uri("https://myaccount.cognitiveservices.azure.com/"),
    new DefaultAzureCredential()
);
```
- `DefaultAzureCredential` tries: Managed Identity → VS credential → Azure CLI → etc.
- **Zero secrets to rotate or leak**
- Requires assigning the right RBAC role to the compute identity

**Why Managed Identity is preferred:**
```
Azure Function (System-assigned MI)
    ↓ DefaultAzureCredential
Azure AI Services
    ← RBAC: "Cognitive Services User" role assigned to the Function's identity
```

No key in code. No secret in Key Vault. Just identity.

---

### 3. RBAC: The Three Roles You Need to Know

| Role | Can do | Typical assignee |
|---|---|---|
| **Cognitive Services User** | Call APIs (inference) | Apps, Azure Functions, web APIs |
| **Cognitive Services Contributor** | Manage resource + call APIs | Developers, CI/CD pipelines |
| **Owner / Contributor** | Full control including billing | Admins only |

**For your JM Family pipeline:**
- Azure Function → **Cognitive Services User** on Document Intelligence resource
- Azure DevOps service principal → **Cognitive Services Contributor** (deploys, reads keys)
- Your personal account → **Contributor** or **Owner** for management

---

### 4. Networking: Virtual Networks and Private Endpoints

**Default state:** Azure AI Services endpoint is public internet.

**You have two ways to restrict access:**

#### Option A: Virtual Network Service Endpoints + IP Firewall rules
```json
{
  "networkAcls": {
    "defaultAction": "Deny",
    "ipRules": [{"value": "203.0.113.0/24"}],
    "virtualNetworkRules": [
      {"id": "/subscriptions/.../virtualNetworks/vnet-prod/subnets/snet-functions"}
    ]
  }
}
```
- Traffic still uses public endpoint address, but firewall blocks anything not from your VNet/IPs
- Quick to set up, lower cost

#### Option B: Private Endpoint (recommended for enterprise)
```
Azure Function (in VNet) 
    → Private Endpoint 
    → Private IP: 10.0.1.5 
    → Azure AI Services
    (no public internet transit)
```
- Endpoint resolves to a private IP inside your VNet
- Public endpoint is **disabled** — no one outside can hit it
- Higher security, required for many compliance frameworks

**JM Family relevance:** If your Document Intelligence resource handles contracts/financial documents, Private Endpoint is the right call.

---

### 5. Monitoring: Azure Monitor + Diagnostic Logs

**Three things to monitor on any Azure AI Services resource:**

| What | Where | Why |
|---|---|---|
| **Metrics** | Azure Monitor Metrics | Calls, latency, errors — real-time graphs |
| **Diagnostic Logs** | Log Analytics Workspace | Per-request details, audit trail |
| **Alerts** | Azure Monitor Alerts | Notify when error rate spikes or latency exceeds threshold |

**Key metrics for Azure AI Services:**
- `SuccessfulCalls` — normal traffic
- `TotalErrors` — any errors (4xx + 5xx)
- `ClientErrors` (4xx) — bad requests, auth failures, quota exceeded
- `ServerErrors` (5xx) — service-side failures
- `Latency` — response time

**Enable diagnostic logs (Bicep/ARM):**
```bicep
resource diagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'ai-diagnostics'
  scope: cognitiveServicesAccount
  properties: {
    workspaceId: logAnalyticsWorkspace.id
    logs: [
      { category: 'Audit', enabled: true }
      { category: 'RequestResponse', enabled: true }
    ]
    metrics: [{ category: 'AllMetrics', enabled: true }]
  }
}
```

---

### 6. Throttling: TPM/RPM Limits and Retry Logic

**Every Azure AI Services resource has quota limits:**
- **Transactions per Second (TPS)** for most services (Vision, Language, Speech)
- **Tokens per Minute (TPM)** for Azure OpenAI

**What happens when you exceed the limit:**
```
HTTP 429 Too Many Requests
Retry-After: 30
```

**How to handle in C#:**
```csharp
// Polly-based retry with exponential backoff
var retryPolicy = Policy
    .Handle<RequestFailedException>(ex => ex.Status == 429)
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
        onRetry: (ex, delay, attempt, ctx) => 
            logger.LogWarning("Throttled. Retry {attempt} after {delay}s", attempt, delay.TotalSeconds)
    );
```

**Architect strategies to avoid throttling:**
1. **Scale out horizontally** — multiple function instances → same quota, still throttled
2. **Request quota increase** — Azure portal → Quotas → submit request
3. **Use multiple resources** — round-robin across two AI resources (doubles effective TPM)
4. **Queue and pace** — ADF/Service Bus throttle ingestion speed upstream

---

## Topic 7.2 — Containers for AI Services

---

### 1. Why Run Azure AI Services in Containers?

**The cloud endpoint is fine for most cases. Containers exist for specific scenarios:**

| Scenario | Why containers |
|---|---|
| **Disconnected / air-gapped** | Factory floor, submarine, plane — no internet |
| **Data sovereignty** | Data cannot leave a specific region or datacenter |
| **Ultra-low latency** | Processing on-device avoids network round-trip |
| **Compliance** | Regulated industry where data must stay on-premises |
| **Cost at scale** | Very high volume → on-prem hardware cheaper than per-call pricing |

**Important constraint:** AI Services containers still phone home to Azure for billing. They do NOT work fully offline — billing data goes to Azure, raw request data stays local.

---

### 2. How Container Images Work

**Microsoft hosts container images on Microsoft Container Registry (MCR):**
```bash
# Pull a container image (example: Language Detection)
docker pull mcr.microsoft.com/azure-cognitive-services/textanalytics/language:latest
```

**Required environment variables for every AI Services container:**
```bash
docker run -d \
  -p 5000:5000 \
  -e ApiKey="<your-key>" \
  -e Billing="https://<name>.cognitiveservices.azure.com/" \
  -e Eula="accept" \
  mcr.microsoft.com/azure-cognitive-services/textanalytics/language:latest
```

Three required values:
| Variable | Purpose |
|---|---|
| `ApiKey` | Auth key from your Azure resource (still needed for billing) |
| `Billing` | Endpoint URL of your Azure resource (for billing calls) |
| `Eula` | Must be set to `"accept"` to acknowledge terms |

**The container then runs locally at `http://localhost:5000`** — same API as the cloud endpoint.

---

### 3. Which Services Support Containers?

Not all Azure AI Services support containers. Key ones:

| Service | Container capability |
|---|---|
| **Azure AI Language** | Sentiment, Key Phrase, Language Detection, NER, Custom NER |
| **Azure AI Vision** | Read (OCR) |
| **Azure AI Speech** | Speech-to-Text, Text-to-Speech, Custom Speech |
| **Azure AI Translator** | Translation |
| **Azure AI Face** | Face detection (limited — not identification) |
| **Document Intelligence** | Layout, Read, prebuilt models |
| **Azure OpenAI** | ❌ Not available as container |

**Important:** Azure OpenAI models cannot be containerized. For on-premises LLM needs, you'd use Ollama/open-source models separately.

---

### 4. Kubernetes Deployment Pattern

For production container deployments, Kubernetes is the standard approach:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: ai-language-service
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: language
        image: mcr.microsoft.com/azure-cognitive-services/textanalytics/language:latest
        ports:
        - containerPort: 5000
        env:
        - name: ApiKey
          valueFrom:
            secretKeyRef:
              name: ai-secrets
              key: api-key
        - name: Billing
          value: "https://myaccount.cognitiveservices.azure.com/"
        - name: Eula
          value: "accept"
        resources:
          requests:
            memory: "2Gi"
            cpu: "1"
          limits:
            memory: "4Gi"
            cpu: "2"
```

**Sizing note:** AI inference containers are CPU/memory intensive. Budget 2–4 GB RAM and 1–2 cores per container instance.

---

### 5. When to Choose Cloud vs Container

```
Decision tree:

Does data need to stay on-premises or in a specific region?
    YES → Container
    NO  ↓

Is there reliable internet connectivity?
    NO → Container
    YES ↓

Do you need latency < 20ms (IoT/edge)?
    YES → Container
    NO  ↓

Is per-call cost at your volume higher than on-prem infrastructure?
    YES → Evaluate containers
    NO  → Cloud endpoint (simpler, always up-to-date)
```

---

## Topic 7.3 — Custom Models and Training

---

### 1. Why Custom Models?

**Prebuilt models work well for general cases. You need custom models when:**
- Your domain vocabulary is specialized (medical, legal, financial)
- Prebuilt accuracy on your data is < 85% (typical threshold)
- You have labeled training data specific to your use case
- Prebuilt categories/entities don't match your business entities

**Three main custom model types in Azure AI Services:**

| Service | Custom capability | Use case |
|---|---|---|
| **Azure AI Vision** | Custom image classification, object detection | "Is this a damaged part?" |
| **Azure AI Speech** | Custom acoustic model, custom language model | Call center audio, technical jargon |
| **Azure AI Language** | Custom NER, custom text classification | Extract JM Family-specific entities |

---

### 2. Azure AI Custom Vision

**Two task types:**

| Type | What it does | Example |
|---|---|---|
| **Image Classification** | What is this image? | "Damaged" / "Not damaged" / "Minor damage" |
| **Object Detection** | Where are things in this image? | Bounding boxes around defects |

**Training workflow:**
```
1. Create Custom Vision resource (Training + Prediction endpoints)
2. Create a Project (Classification or Object Detection)
3. Tag and upload images (min 15 images per tag, recommend 50+)
4. Train (Quick Training: minutes | Advanced Training: hours, better accuracy)
5. Evaluate: Precision, Recall, AP (Average Precision)
6. Publish iteration → gets a Prediction URL
7. Call Prediction URL from your app
```

**Key metrics:**
- **Precision:** Of the times you said "damaged", how often were you right? (avoids false positives)
- **Recall:** Of all actually-damaged items, how many did you catch? (avoids false negatives)
- **AP (Average Precision):** Combines both into one number, 0–1 (higher is better)

**C# call to custom prediction:**
```csharp
var predictionClient = new CustomVisionPredictionClient(
    new ApiKeyServiceClientCredentials("<prediction-key>"),
    new Uri("https://<endpoint>/customvision/v3.0/Prediction/")
);

var result = await predictionClient.ClassifyImageAsync(
    projectId: Guid.Parse("<project-id>"),
    publishedName: "Iteration1",
    imageData: imageStream
);

foreach (var pred in result.Predictions)
    Console.WriteLine($"{pred.TagName}: {pred.Probability:P1}");
```

---

### 3. Azure AI Custom Speech

**Three layers you can customize:**

| Layer | What it fixes | Example |
|---|---|---|
| **Acoustic model** | Audio environment quality | Factory floor noise, call center phone audio |
| **Language model** | Domain vocabulary/phrases | "JM Family", "iPacket", "AutoNation" — brand names |
| **Pronunciation** | Unusual word pronunciations | Acronyms, proper nouns |

**Training workflow:**
```
1. Create Speech resource with custom training capability
2. Upload training data:
   - Acoustic: .wav files + transcripts (1000+ utterances for good results)
   - Language: plain text sentences with domain vocabulary (thousands of sentences)
3. Train custom model
4. Evaluate: Word Error Rate (WER) — lower is better, baseline vs custom
5. Deploy to endpoint
6. Use custom endpoint URL in your SpeechConfig
```

```csharp
var config = SpeechConfig.FromSubscription("<key>", "<region>");
config.EndpointId = "<custom-endpoint-id>"; // your custom model
var recognizer = new SpeechRecognizer(config);
```

---

### 4. Azure AI Language — Custom NER and Text Classification

This is the most relevant for your JM Family work (document processing, entity extraction).

**Custom Named Entity Recognition (Custom NER):**
- Define your own entity types: `VehicleMake`, `DealerCode`, `InvoiceAmount`, etc.
- Label training documents with these entities
- Model learns to extract them from new documents

**Custom Text Classification:**
- **Single-label:** Each document belongs to one class (`Invoice`, `Contract`, `PO`)
- **Multi-label:** Document can have multiple classes (`Invoice + Amendment`)

**Workflow (Azure AI Language Studio):**
```
1. Create Azure AI Language resource
2. Create project in Language Studio (language.cognitive.azure.com)
3. Import or upload training data (.txt or JSON format)
4. Label entities (NER) or assign labels (classification)
5. Train model (15 min – 2 hrs depending on data size)
6. Evaluate: Precision, Recall, F1 per entity/class
7. Deploy model → deploy to named slot (production / staging)
8. Call via API
```

**Minimum training data:**
- Custom NER: ~200 labeled documents (50 minimum, but 200+ for good results)
- Text Classification: ~200 documents total, ~10 per class minimum

**C# call:**
```csharp
var client = new TextAnalyticsClient(
    new Uri("https://<endpoint>.cognitiveservices.azure.com/"),
    new DefaultAzureCredential()
);

var response = await client.RecognizeCustomEntitiesAsync(
    waitUntil: WaitUntil.Completed,
    documents: new List<string> { "Invoice #12345 from Ford Motor Co..." },
    projectName: "jmfamily-invoices",
    deploymentName: "production"
);

foreach (var doc in response.Value)
    foreach (var entity in doc.Entities)
        Console.WriteLine($"{entity.Category}: {entity.Text} ({entity.ConfidenceScore:P0})");
```

---

### 5. Choosing Between Prebuilt vs Custom vs Fine-tuned

```
Is there a prebuilt model that covers your use case?
    YES → Use it. Prebuilt is maintained, improves automatically.
    NO  ↓

Do you need structured outputs (entities, classifications)?
    YES → Custom AI Language / Custom Vision
    NO  ↓

Do you need generative/conversational behavior?
    YES → Azure OpenAI fine-tuning (Module 15)
    NO  → Stay with prebuilt + prompt engineering
```

**Rule of thumb:** Prebuilt → Custom → Fine-tuned is order of increasing cost and complexity. Exhaust each level before moving to the next.

---

## Module 7 — Architecture Summary

```
┌─────────────────────────────────────────────────────────────────┐
│                    Azure AI Services Resource                    │
│                                                                  │
│  Authentication: API Key OR Managed Identity (preferred)         │
│  RBAC: Cognitive Services User → apps                           │
│        Cognitive Services Contributor → devs/CI                 │
│                                                                  │
│  Networking: Public → VNet Service Endpoints → Private Endpoint  │
│  Monitoring: Metrics + Diagnostic Logs → Log Analytics          │
│  Throttling: 429 + Retry-After → exponential backoff            │
│                                                                  │
│  Deployment options:                                             │
│  ┌──────────────┐    ┌────────────────────────┐                │
│  │ Cloud (PaaS)  │    │ Container (Docker/K8s)  │               │
│  │ Always online │    │ On-prem / disconnected  │               │
│  │ Auto-updates  │    │ Data sovereignty         │               │
│  └──────────────┘    └────────────────────────┘                │
│                                                                  │
│  Models:                                                         │
│  ┌────────────┐  ┌───────────────┐  ┌────────────────────┐     │
│  │  Prebuilt  │  │ Custom Vision │  │ Custom NER/Classify │     │
│  │ (default)  │  │ Custom Speech │  │ (Language Studio)   │     │
│  └────────────┘  └───────────────┘  └────────────────────┘     │
└─────────────────────────────────────────────────────────────────┘
```

---

## Recall — Module 7 Self-Test Questions

Try answering before checking the answers below.

**Q1.** Your Azure Function processes insurance documents using Document Intelligence. The function currently uses API keys stored in environment variables. Your security team flags this. What's the correct fix, and what Azure role do you assign?

**Q2.** A client says "we process medical records and data cannot leave our datacenter." You need sentiment analysis on patient feedback. Can Azure AI Language Service help? How?

**Q3.** You have a Language container running at `localhost:5000`. It's been running fine for 2 hours, then suddenly stops. You look at container logs and see "Billing endpoint unreachable." What happened and how do you fix it?

**Q4.** You want to extract `VehicleMake`, `DealerCode`, and `TransactionDate` from automotive invoices. Prebuilt Document Intelligence invoice model gives you `Vendor`, `Total`, `Date` but not the vehicle-specific fields. What's the right next step?

**Q5.** Your Azure AI Services resource starts returning HTTP 429 errors. Your team lead says "just add more function instances." Why won't that help, and what will?

**Q6.** You have Custom Vision trained with 30 images per tag. Precision is 72%, Recall is 68%. What should you do first?

---

<details>
<summary>Answers (expand after attempting)</summary>

**A1.** Switch to **System-Assigned Managed Identity** on the Function App. Grant that identity the **Cognitive Services User** role on the Document Intelligence resource. Replace `AzureKeyCredential` with `DefaultAzureCredential()` in code. Delete the environment variable keys.

**A2.** Yes — use **Azure AI Language container**. Deploy the Sentiment Analysis container on-premises. Container sends billing metadata to Azure but raw text never leaves the datacenter. Patient data stays local.

**A3.** The container needs internet connectivity to Azure's billing endpoint. The network connection was lost (or a firewall rule changed). The container doesn't work without the billing heartbeat. Fix: restore internet access or whitelist `*.cognitiveservices.azure.com` outbound. Containers are NOT fully offline — billing traffic must reach Azure.

**A4.** Use **Custom NER** (Azure AI Language → Custom Named Entity Recognition). Upload labeled invoices where you tag `VehicleMake`, `DealerCode`, `TransactionDate`. Train a custom model. Deploy it. Call alongside or instead of the Document Intelligence prebuilt model.

**A5.** More function instances share the same quota — 10 instances still hit the same 1000 TPS limit. Solutions: request a quota increase in Azure portal, add a second AI Services resource and load-balance across both, or add a Service Bus queue upstream to smooth ingestion rate.

**A6.** Add more training images (target 100+ per tag). 30 images per tag is near the minimum. Precision and Recall both below 80% with limited training data → more data before tuning anything else.

</details>

---

## Memory Hooks

- **"Two keys, one rotation"** — API key rotation without downtime: rotate K1 while K2 is live, then rotate K2
- **"Billing always phones home"** — containers are NOT offline; billing endpoint must be reachable
- **"Three required env vars: ApiKey, Billing, Eula"** — missing any one = container won't start
- **"Prebuilt → Custom → Fine-tune"** — exhaust each level before adding complexity
- **"429 + Retry-After = throttle"** — more instances don't help; need more quota or second resource
- **"Cognitive Services User = inference only"** — apps get this role, not Contributor

---
---

## 2026 Updates

| Topic | Update |
|---|---|
| **Azure AI Services rebrand** | "Cognitive Services" is now "Azure AI Services" across portal and docs. Same APIs, same endpoints — just a rebrand. Your existing code still works |
| **AI Foundry as unified hub** | Azure AI Services resources are now managed from ai.azure.com (AI Foundry) as well as portal.azure.com. Foundry shows models, deployments, and content safety in one place |
| **Content Safety GA** | Prompt Shields (prompt injection detection) now GA. Groundedness detection (hallucination blocker) also GA. Both should be standard in any production RAG pipeline |
| **Custom NER improvements** | Custom NER now supports zero-shot entity recognition — can identify entity types without labeled training data for common entity patterns |
| **Container updates** | Document Intelligence containers now support more prebuilt models (Layout, Invoice, W-2). Check MCR for latest tags |

---

## Interactive Learning Ideas

### Exercise 1 — Security Audit of JMA AI Resources (20 min)
Go to portal.azure.com and audit your JMA AI resources:
- For each resource: is public network access enabled or disabled?
- Is a Managed Identity assigned?
- Are diagnostic logs sending to a Log Analytics workspace?
- What RBAC roles are assigned, and to whom?
- Document what needs to change to reach production security standard.

### Exercise 2 — Container Run (20 min)
Pull and run an Azure AI Language container locally:
```bash
docker pull mcr.microsoft.com/azure-cognitive-services/textanalytics/language:latest
docker run -p 5000:5000 \
  -e ApiKey="<your-key>" \
  -e Billing="https://<your-resource>.cognitiveservices.azure.com/" \
  -e Eula="accept" \
  mcr.microsoft.com/azure-cognitive-services/textanalytics/language:latest
```
Then call `http://localhost:5000` with the same JSON you'd send to the cloud endpoint. Confirm same response.

### Exercise 3 — Throttle Simulation
Write a C# console app that sends 50 requests per second to an AI Language resource on F0 tier (which throttles at ~5/sec). Add Polly retry with exponential backoff. Observe the retry behavior in console output. This is the fastest way to internalize the 429 retry pattern.

### Exercise 4 — Custom NER Project
Go to Language Studio → Custom NER → New project:
- Define 3 entity types relevant to JMA: `VehicleModel`, `DealerCode`, `InvoiceAmount`
- Upload 10 sample dealer support ticket texts
- Label the entities manually
- Train (even with minimal data — you'll see poor metrics, which is instructive)
- Note the minimum data required for acceptable F1 score

### Exercise 5 — Prebuilt → Custom Decision
For each JMA scenario, decide: use prebuilt model OR build custom?
- Extract vendor name and total from a standard invoice PDF
- Extract "JMA Vehicle Grade" (A/B/C) and "Dealer Territory Code" from a dealer agreement
- Detect sentiment in English dealer feedback
- Classify dealer tickets as: complaint, inquiry, escalation, or compliment
- Identify person names and phone numbers in dealer emails

---

*Chapter file for: AI Solutions Architect Curriculum | Part 2 Module 7*
*Written: 2026-05-27 | Updated: 2026-06-30*
