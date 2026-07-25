# Q&A — L07: Azure AI Services Deep Dive
**Source chapter:** `01_Lessons/Part2_AzureAIServices/L07_AzureAIServices_DeepDive.md` | **Format:** self-study
**Questions:** 34 | *No overlap with the interview bank (which covers L07 topics at architect-judgment level in `02_Azure_AI_Platform.md`) or the chapter's own self-test — these test the chapter's factual content directly.*

---

## Management, Auth & RBAC

**Q1. What are the two provisioning flavors of Azure AI Services, and what does each mean?**
**Multi-service resource** — one resource (one endpoint, one key set) covering Vision, Language, Speech, Document Intelligence, etc. **Individual service resource** — one resource per service type. Multi-service simplifies dev/test and billing; individual gives production isolation, separate quotas, and separate RBAC per service.

**Q2. How does a single multi-service endpoint serve different services?**
Same base URL (`https://<name>.cognitiveservices.azure.com/`), routed by API path: `/vision/v3.2/analyze` → Computer Vision, `/language/analyze-text` → Language, `/formrecognizer/documentModels/prebuilt-invoice:analyze` → Document Intelligence.

**Q3. Why does every Azure AI Services resource come with two API keys?**
Zero-downtime rotation: rotate Key1 while apps use Key2, update apps to the new Key1, then rotate Key2. Both keys are symmetric (either works) and rotate independently.
*Memory hook: "Two keys, one rotation."*

**Q4. What credential class enables Managed Identity in code, and what does its lookup chain try?**
`DefaultAzureCredential()` — it tries Managed Identity first, then falls back through developer credentials (Visual Studio, Azure CLI, etc.). Same code works in the cloud (uses MI) and on a dev laptop (uses your az login) with zero secrets in code or config.

**Q5. What RBAC assignment makes a Managed-Identity call to an AI service actually work?**
The calling resource's identity (e.g., the Function App's system-assigned MI) must hold the **Cognitive Services User** role on the target AI Services resource. Identity without the role assignment = 401/403.

**Q6. Map the three key RBAC roles to their typical assignees in the JMA pipeline.**
| Role | Can do | JMA assignee |
|---|---|---|
| Cognitive Services User | Call APIs (inference only) | The Azure Function |
| Cognitive Services Contributor | Manage resource + call APIs | Azure DevOps service principal (deploys, reads keys) |
| Owner/Contributor | Full control incl. billing | Admins / your personal account |

---

## Networking & Monitoring

**Q7. What is the default network exposure of an Azure AI Services resource?**
Public internet — anyone with the endpoint URL and a valid key can reach it from anywhere. Restriction is opt-in, not default.

**Q8. Describe Option A for restricting access: VNet Service Endpoints + IP firewall.**
Set `networkAcls.defaultAction` to `Deny` and allow-list specific IP ranges (`ipRules`) and/or VNet subnets (`virtualNetworkRules`). Traffic still targets the public endpoint address, but the firewall rejects anything not on the list. Quick to set up, lower cost.

**Q9. Describe Option B: Private Endpoint — what actually changes?**
The service gets a **private IP inside your VNet** (e.g., 10.0.1.5); the public endpoint is disabled entirely. Traffic never transits the public internet. Required by many compliance frameworks — the chapter's call: JMA's Document Intelligence handling contracts/financial documents should use it.

**Q10. What are the three monitoring layers for any AI Services resource, and what does each answer?**
**Metrics** (Azure Monitor) — what's happening right now, real-time graphs. **Diagnostic Logs** (→ Log Analytics workspace) — per-request detail and audit trail. **Alerts** — proactive notification when error rate or latency crosses thresholds.

**Q11. Name the five key metrics for an AI Services resource.**
`SuccessfulCalls`, `TotalErrors`, `ClientErrors` (4xx — bad requests, auth failures, quota exceeded), `ServerErrors` (5xx — service-side), `Latency`.

**Q12. Which two diagnostic log categories does the chapter's Bicep example enable, and where do they go?**
`Audit` and `RequestResponse`, sent to a **Log Analytics workspace** (plus `AllMetrics`). Configured via a `Microsoft.Insights/diagnosticSettings` resource scoped to the cognitive services account.

---

## Throttling

**Q13. What quota unit applies to most AI services vs Azure OpenAI specifically?**
Most services: **TPS** (transactions per second). Azure OpenAI: **TPM** (tokens per minute). Both are per-resource quota ceilings.

**Q14. What exactly does the service return when you exceed quota, and what should your code honor?**
HTTP **429 Too Many Requests** with a **`Retry-After`** header (seconds to wait). Retry logic should honor that header — the chapter's pattern is Polly with exponential backoff (2^attempt seconds) on status 429.

**Q15. The chapter lists four strategies to avoid throttling — name them.**
(1) ~~Scale out horizontally~~ — listed to make the point it *doesn't work* (same shared quota); (2) **request a quota increase** via the portal; (3) **multiple resources round-robined** (doubles effective TPM/TPS); (4) **queue and pace upstream** (Service Bus/ADF smoothing ingestion speed).

---

## Containers

**Q16. List the five scenarios where AI Services containers make sense.**
(1) Disconnected/air-gapped environments (factory floor, plane), (2) data sovereignty (data can't leave a datacenter/region), (3) ultra-low latency (on-device, no network round-trip), (4) regulated-industry compliance requiring on-prem, (5) cost at very high volume (on-prem hardware vs per-call pricing).

**Q17. What's the "billing phones home" constraint?**
Containers are NOT fully offline — they must periodically reach Azure's billing endpoint or they stop working. Raw request data stays local; billing metadata goes to Azure. Firewall rule needed: outbound to `*.cognitiveservices.azure.com`.
*Memory hook: "Billing always phones home."*

**Q18. What three environment variables must every AI Services container receive?**
`ApiKey` (key from your Azure resource — still needed, for billing), `Billing` (your Azure resource's endpoint URL), `Eula` (must be `"accept"`). Missing any one = container won't start.
*Memory hook: "ApiKey, Billing, Eula — three or no start."*

**Q19. Where do the container images come from, and what does a running container expose?**
Pulled from **Microsoft Container Registry (MCR)** — e.g., `mcr.microsoft.com/azure-cognitive-services/textanalytics/language`. Once running, it serves the **same API as the cloud endpoint** locally (e.g., `http://localhost:5000`).

**Q20. Which services support containers, and which notable one does not?**
Support: AI Language (sentiment, key phrase, language detection, NER/custom NER), Vision Read OCR, Speech (STT/TTS/custom), Translator, Face (detection only), Document Intelligence (Layout, Read, and a growing set of prebuilt models). **Azure OpenAI cannot be containerized** — for on-prem LLMs you'd use open-source models (e.g., via Ollama) separately.

**Q21. What resource sizing does the chapter recommend per AI container instance in Kubernetes?**
2–4 GB RAM and 1–2 CPU cores per instance — AI inference containers are CPU/memory-hungry. The chapter's K8s example: requests 2Gi/1 CPU, limits 4Gi/2 CPU, with the ApiKey injected from a K8s Secret.

**Q22. Reconstruct the chapter's cloud-vs-container decision tree.**
Data must stay on-prem/in a specific region? → Container. No reliable internet? → Container. Need <20ms latency (IoT/edge)? → Container. Per-call cost at your volume exceeds on-prem infra cost? → Evaluate containers. Otherwise → **Cloud endpoint** (simpler, always up-to-date).

---

## Custom Models & Training

**Q23. What conditions justify a custom model over prebuilt?**
Specialized domain vocabulary (medical/legal/financial), prebuilt accuracy below ~85% on your data, labeled training data available, or business entities/categories that prebuilt schemas don't include.

**Q24. What are the three main custom-model capabilities across the AI services?**
| Service | Custom capability | Example |
|---|---|---|
| AI Vision (Custom Vision) | Image classification, object detection | "Is this part damaged?" |
| AI Speech | Custom acoustic + language models, pronunciation | Call-center audio, brand names |
| AI Language | Custom NER, custom text classification | Extract JMA-specific entities |

**Q25. Custom Vision: classification vs object detection — what's the output difference?**
**Classification** answers "what is this image?" (a label for the whole image — Damaged/Not damaged). **Object detection** answers "where are things?" (bounding boxes around each detected object — boxes around defects).

**Q26. Walk the Custom Vision training workflow.**
Create Custom Vision resource (training + prediction endpoints) → create project (classification or detection) → tag & upload images (min 15/tag, 50+ recommended) → train (Quick = minutes; Advanced = hours, better accuracy) → evaluate (Precision, Recall, AP) → **publish the iteration** (gets a Prediction URL) → call from your app.

**Q27. Define Precision, Recall, and AP in Custom Vision's evaluation.**
**Precision** — of the times you said "damaged," how often right? (guards false positives). **Recall** — of all actually-damaged items, how many caught? (guards false negatives). **AP (Average Precision)** — combines both into one 0–1 number; higher is better.

**Q28. What three layers can Custom Speech customize, and what does each fix?**
**Acoustic model** — audio environment (factory noise, phone-line audio). **Language model** — domain vocabulary/phrases ("JM Family," "iPacket," "AutoNation"). **Pronunciation** — unusual words, acronyms, proper nouns.

**Q29. What metric evaluates a Custom Speech model, and how do you judge success?**
**WER — Word Error Rate** (lower is better), compared **baseline vs custom**: if the custom model's WER beats the base model's on your test audio, the customization is paying off. Then deploy and set `config.EndpointId` to the custom endpoint in `SpeechConfig`.

**Q30. Custom NER vs Custom Text Classification — what does each produce?**
**Custom NER** extracts your own entity types from text (`VehicleMake`, `DealerCode`, `InvoiceAmount` spans within a document). **Custom Text Classification** assigns whole-document labels — single-label (Invoice OR Contract OR PO) or multi-label (Invoice + Amendment).

**Q31. What's the training data floor for Custom NER and custom classification?**
Custom NER: 50 documents minimum, ~200+ for good results. Text classification: ~200 documents total, ~10 per class minimum. Trained in **Language Studio** (language.cognitive.azure.com), evaluated with per-entity/per-class Precision/Recall/F1, deployed to a named slot (production/staging).

**Q32. In the C# call to custom NER, what two names identify what to run?**
`projectName` (the Language Studio project, e.g., "jmfamily-invoices") and `deploymentName` (the slot, e.g., "production") — passed to `RecognizeCustomEntitiesAsync` along with the documents. Results carry entity `Category`, `Text`, and `ConfidenceScore`.

**Q33. Recite the "Prebuilt → Custom → Fine-tuned" rule and its logic.**
Exhaust each level before moving to the next: prebuilt (maintained by Microsoft, improves automatically, zero training cost) → custom (structured outputs prebuilt can't give — entities, classifications) → fine-tuning an LLM (generative/conversational behavior only). Each step up adds cost and maintenance burden — never skip levels without exhausting the cheaper one.

**Q34. From the 2026 updates: what changed about custom NER and the service branding?**
Custom NER gained **zero-shot entity recognition** — common entity patterns identifiable without labeled training data. Branding: "Cognitive Services" is now "Azure AI Services" everywhere (same APIs/endpoints — code unaffected), and resources are also manageable from **AI Foundry** (ai.azure.com) alongside the Azure portal. Also GA'd: Prompt Shields and groundedness detection in Content Safety.

---

*Curriculum Q&A Batch A — file 2 of 3. Next: QA_L08 (Document Intelligence).*
