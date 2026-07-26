# AIML-Learn — Concept Index

**Generated:** 2026-07-19 · **Updated:** 2026-07-26 (Part 7) · **1,145 concepts · 1,272 locations**

Alphabetical. Look up a term, see how deeply it is covered and exactly where.

| Mark | Depth |
|---|---|
| ● | **Taught in depth** — explanation plus example, code, or worked detail |
| ◐ | **Covered** — explained in a paragraph or table, but briefly |
| ○ | **Mentioned** — a name-drop, table row, or single clause |

`L##:line` = lesson file and line number. `P6/...` = Part 6 applied projects. `IB/` = interview bank · `HLP01` = high-level prep · `PyTrack/` = Python track · `VitalCare` = assessment.
Structural contents (every heading, in order) → `00_CONTENTS.md`

---


## A

**A/B testing prompts** — Split traffic between prompt versions  
● L19:595

**A2A message bus** — Typed envelope routing between agents  
● P6/02-Dealer/WORKFLOW:206

**A2A Protocol** — Open standard for agent-to-agent messaging  
● L29:25 · ○ L17:1069

**A2A protocol** — Standard for agent-to-agent task delegation  
◐ IB/04_Agents:108

**A2A typed envelope** — MessageId, CorrelationId, SchemaVersion, payload  
● P6/02-Dealer/FLOW_WITH_LOOPS:315

**A2A vs MCP vs direct call** — Agent-agent vs agent-tool vs in-process  
● L29:149

**Abstractive summarization** — LLM-generated new summary text  
◐ L03:286

**Accountability principle** — Human oversight of AI systems  
◐ L01:505

**Accuracy** — Correct predictions over total  
◐ L01:325

**Acoustic model** — Maps audio signal to phonemes  
◐ L05:38

**Action group (Bedrock)** — Agent tool via OpenAPI plus Lambda  
◐ P6/06-Bedrock/01_concepts:85

**Active-active vs active-passive** — Multi-region topology cost/RTO tradeoff  
● IB/05_SolArch:146

**Activity** — Any single bot conversation event  
● L10:55

**ActivityHandler** — Base bot class overriding activity handlers  
● L10:86

**ActivitySource** — System.Diagnostics is OpenTelemetry in .NET  
◐ L36:135

**Adaptive Cards** — JSON-defined rich cards for Teams  
● L10:237

**ADC (Application Default Credentials)** — Local GCP auth, no keys  
◐ P6/09-Vertex/01_concepts:97

**ADF vs Azure Functions** — Batch orchestration vs event-driven compute  
◐ L20:247

**ADO pipeline** — Azure DevOps CI/CD environment  
◐ P6/02-Dealer/JMA-Complete-Flow:18

**Advanced RAG** — Pre/post-retrieval improvement generation  
◐ L13:66

**Agent Builder (Google)** — Low-code agent console, Copilot Studio analog  
◐ P6/09-Vertex/03_interview_qa:26

**Agent Bus** — Broker enforcing A2A contract  
● L29:70

**Agent capability discovery** — Agents advertise what they can do  
◐ L29:26

**Agent Development Kit (ADK)** — Google open-source agent framework  
● P6/09-Vertex/01_concepts:66

**Agent Engine (Vertex)** — Managed agent runtime  
◐ P6/09-Vertex/02_architecture:61

**Agent evaluation** — Success rate, steps, tool accuracy  
● PyTrack/1.5-Agents:1632

**Agent lifecycle** — Build-test-evaluate-deploy-monitor-iterate  
● L22:94

**Agent lifecycle (Think-Act-Observe)** — Iterative reason-tool-result loop  
● L17:960

**Agent loop (minimal, framework-free)** — Raw Python while-loop over tool calls  
● PyTrack/1.5-Agents:117

**Agent memory (short vs long-term)** — Context window vs vector store  
◐ L16:477

**Agent memory scoping (shared vs private)** — Per-agent memory visibility rules  
◐ IB/04_Agents:194

**Agent safety guardrails** — Injection, loops, unauthorized tool risks  
● L16:894

**Agent Service** — Foundry hosted agent runtime  
◐ L17:1064

**Agent threads** — Conversation state container for agents  
○ L17:1066

**Agent tools (Foundry)** — Search, Bing, functions, code interpreter  
◐ L17:1067

**AgentBus** — Validate, audit, route, dead-letter  
● P6/02-Dealer/FLOW_WITH_LOOPS:318

**AgentExecutor** — LangChain ReAct agent runner  
◐ L21:663

**AgentGroupChat** — SK multi-agent orchestration class  
○ L16:2033

**Agentic architecture pattern** — RAG plus tools plus orchestration  
◐ L18:70

**Agentic autonomy levels** — Five-level human-involvement ladder  
● VitalCare:1441

**Agentic hallucination** — Wrong action, not wrong statement  
● IB/04_Agents:218 · ● L24:18

**Agentic RAG** — Agent chooses index and timing  
● L16:621 · ◐ L13:1177

**Agentic retrieval** — Search runs RAG internally, returns answer  
○ L09:734

**AgentMessage envelope** — Typed A2A message schema  
● L29:43 · ● P6/02-Dealer/README:125

**AI Agent** — LLM that plans, acts, adapts  
● L16:418

**AI CI/CD vs standard CI/CD** — Adds evaluation and quality gate stages  
◐ L19:147

**AI gateway pattern** — APIM fronting Azure OpenAI  
◐ L18:468

**AI inventory / registry** — Central list of AI systems and owners  
● IB/06_RAI:260

**AI Search replicas** — Scale query throughput and HA  
◐ L18:136

**AI-assisted coding practice** — Completion, chat, multi-file edit, agent  
● L35:28

**AI-assisted migration** — Generate IaC, verify with plan  
● L33:450

**AI-first infrastructure plays** — Config reading, template translation, triage  
● L35:212

**AKS vs EKS vs GKE** — Identity, autoscaling, registry differences  
● L34:282

**Aliases (index)** — Pointer enabling zero-downtime reindex  
◐ L09:626

**Alignment** — AI does what designers intend  
● L11_4:151

**all-MiniLM-L6-v2** — 384-dim sentence-transformer embedder  
● P6/01-Ollama/02_architecture:40

**Alpha (LoRA scaling factor)** — Adapter output scaling, typically 2x rank  
◐ L14:531

**Ambient clinical documentation** — AI drafts SOAP note from encounter audio  
● VitalCare:212

**Anomaly Detector** — Telemetry anomaly detection service  
○ L20:418

**Ansible** — Agentless YAML config management over SSH  
● L33:391

**Answer Relevance** — Does answer address question  
○ L13:1203

**Answer relevance (RAGAS)** — Does answer address the question  
● P6/03-RAGAS/01_concepts:23

**Answers (semantic)** — Direct answer extracted from top result  
◐ L09:437

**Anthropic computer-use** — Agent loop with GUI primitives instead of API tools  
● L35:261

**AP (Average Precision)** — Combined precision/recall metric per tag  
◐ L04:225

**API key authentication** — key1/key2 subscription key auth  
● L07:58

**APIM gateway** — JWT, rate limit, audit before tool routing  
● P6/02-Dealer/WORKFLOW:220

**APIM policies for Azure OpenAI** — XML rate-limit and cache policies  
● L20:199

**App Insights AI dashboard** — Panels for tokens, latency, quality  
● L19:415

**App-of-apps** — Root Application bootstrapping an environment  
◐ L34:264

**Approximate nearest neighbor (ANN)** — Fast approximate vector retrieval  
◐ L09:391

**ArgoCD Application** — CRD binding a Git path to a cluster namespace  
● L34:225

**ArgoCD vs Flux** — UI and AppProjects versus CLI-first composability  
◐ L34:269

**Arize Phoenix** — Embedding drift and RAG retrieval analysis  
● L36:207

**ARO and ROSA** — Managed OpenShift on Azure and AWS  
○ L34:362

**AssistantAgent** — AutoGen LLM reasoning agent  
◐ L25:190

**Async/await (Python)** — Coroutines with asyncio.run  
● L21:229

**Asynchronous analysis flow** — POST, Operation-Location, poll result  
● L08:66

**Attack surface — agent memory** — Cross-session PHI contamination risk  
◐ L24:354

**Attack surface — RAG retrieval** — Poisoned index, unauthorized retrieval  
◐ L24:337

**Attack surface — system prompt** — Injection and extraction attacks  
◐ L24:332

**Attack surface — tool calls** — Injected parameters, tool result injection  
◐ L24:342

**Attention** — Tokens weight relevance of others  
● L11_1:46

**Attention (Q,K,V)** — softmax(QK/sqrt-d)V weighted token mixing  
● PyTrack/Part1:44

**Attention vs RNN/LSTM** — Parallel long-range dependency modeling  
● IB/01_Fund:8

**AUC_weighted** — Metric for imbalanced classification  
● L06:357

**Audio format requirements** — 16kHz 16-bit mono WAV PCM  
◐ L05:81

**Audit logging** — Who queried what and when  
◐ L20:477

**Audit store (inter-agent)** — HIPAA log of every agent message  
◐ L29:78

**Audit trail for AI decisions** — Model, prompt, context, tool calls captured  
● IB/06_RAI:224

**Audit trail record schema** — Immutable per-interaction AI decision record  
● VitalCare:748

**AuditFilter** — IFunctionInvocationFilter logging tool calls  
● P6/02-Dealer/FLOW_WITH_LOOPS:148

**Auto-merging retrieval** — Retrieve small nodes, return merged parent  
◐ P6/05-LlamaIndex/01_concepts:92

**AutoGen** — Multi-agent group-chat framework  
● L25:181 · ◐ P6/02-crewAI/01_concepts:75

**AutoInvokeKernelFunctions** — SK auto-executes LLM tool calls  
● L16:294 · ○ L18:82

**Automated evaluation pipeline** — Lint, golden dataset, gate, shadow  
● L31:266

**Automatic prompt rollback** — Revert prompt version on drift  
● L31:125

**Automatic rollback triggers** — Drift, latency, error-rate conditions  
● L31:352

**AutoML** — Automated algorithm and hyperparameter search  
● L06:205

**AutoML leaderboard** — Ranked model results after run  
◐ L06:294

**AutoML task types** — Classification, regression, forecasting, vision, NLP  
◐ L06:256

**AutoModel** — Raw hidden states for embeddings  
◐ P6/04-HuggingFace/01_concepts:72

**AutoModelForCausalLM** — Generation head model class  
● P6/04-HuggingFace/01_concepts:61

**AutoTokenizer** — Loads model-matched tokenizer  
● P6/04-HuggingFace/01_concepts:63

**AWS CDK** — Infrastructure in real code, synthesizes CloudFormation  
● L33:324

**Azure AI Agent Service** — Hosted serverless agent runtime  
○ L16:2032

**Azure AI Content Safety** — Input/output harmful content filter  
● L11_4:302 · ◐ L02:388

**Azure AI Face Service** — Face detection, verification, identification  
◐ L04:281

**Azure AI Foundry** — Unified AI build-evaluate-deploy platform  
● L17:29 · ● L22:18 · ◐ L02:105 · ◐ L16:739

**Azure AI Foundry evaluation** — Groundedness/relevance/coherence/fluency scores  
● L16:805

**Azure AI Foundry vs Azure OpenAI Studio** — New unified portal replaces old one  
● L17:63

**Azure AI Foundry vs Semantic Kernel** — Visual portal vs code SDK  
● L17:87

**Azure AI Language** — Prebuilt NLP capability service  
◐ L03:146

**Azure AI Search** — Cloud index/enrich/query retrieval service  
● L09:30

**Azure AI Search (Python SDK)** — SearchClient hybrid vector query  
● L21:729

**Azure AI Search as vector DB** — HNSW, hybrid, filters, metrics  
◐ L13:508

**Azure AI Translator** — Separate 100+ language translation service  
◐ L03:561

**Azure AI Vision** — Prebuilt image analysis foundation API  
◐ L04:108

**Azure API Management as AI gateway** — Rate limit, cache, load balance, log  
● L20:179

**Azure Bot Service** — Cloud channel routing and auth for bots  
◐ L10:437

**Azure Container Apps** — Containerized AI services, scale to zero  
○ L20:39

**Azure Data Factory batch enrichment** — Pipeline for bulk AI reprocessing  
● L20:221

**Azure DevOps YAML pipeline** — CI/CD triggering prompt evaluation  
● L31:294

**Azure Document Intelligence** — Managed OCR plus structured extraction  
● L30:90

**Azure Document Intelligence (Python)** — Prebuilt invoice model analysis  
◐ L21:762

**Azure Event Grid** — Push event bus triggering AI processing  
● L20:105

**Azure Event Hubs** — High-volume streaming ingestion  
○ L20:35

**Azure Functions for AI** — Event-triggered serverless AI compute  
● L20:55

**Azure Kubernetes Service (AKS)** — Long-running inference at scale  
○ L20:40

**Azure ML Designer** — Visual drag-drop ML pipeline builder  
● L06:408

**Azure ML Experiments** — Tracked training runs with metrics  
○ L19:76

**Azure ML Managed Online Endpoint** — Hosted model serving with traffic split  
◐ L19:86

**Azure ML Model Monitor** — Watches data and prediction drift  
◐ L19:91

**Azure ML Model Registry** — Versioned model store with history  
● L19:81

**Azure ML Workspace** — End-to-end ML lifecycle container  
● L06:30

**Azure Monitor metrics** — Calls, latency, error graphs  
◐ L07:151

**Azure OpenAI Embedding skill** — Vectorizes chunks during indexing  
● L09:259

**Azure OpenAI Service** — Managed OpenAI models in Azure  
● L12:31

**Azure Policy** — Governance rules, tagging, locks  
○ L17:1102

**Azure Service Bus** — Durable queue smoothing AI throttling  
● L20:132

**Azure Synapse Analytics + AI** — Spark/SQL batch AI enrichment analytics  
◐ L20:492


## B

**BAA (Business Associate Agreement)** — HIPAA vendor contract for PHI processing  
● VitalCare:68

**Base model / foundation model** — Pre-trained, not yet assistant  
◐ L11_3:140

**Batch API** — Async bulk completions, 50% discount  
○ L12:699

**Batch API discount** — 50% cheaper offline embedding calls  
○ L20:445

**Batch embedding** — Array input, one API call  
◐ L12:311

**Batch endpoint** — Asynchronous bulk scoring endpoint  
● L06:628

**Batch ingestion pipeline pattern** — Blob to DI to chunk to index  
◐ L18:93

**Batch processing pattern** — Fan-out/fan-in document analysis  
◐ L08:465

**Batch transcription** — Async parallel audio file transcription  
● L05:188

**Bedrock Agents** — AWS agent orchestration with action groups  
● P6/06-Bedrock/01_concepts:82

**Bedrock Guardrails** — Content filters, PII, denied topics  
◐ P6/06-Bedrock/01_concepts:91

**Bedrock Knowledge Bases** — Managed RAG over S3 documents  
● P6/06-Bedrock/01_concepts:67

**bedrock-runtime client** — boto3 client for model invocation  
● P6/06-Bedrock/01_concepts:44

**BERT** — Encoder-only understanding model  
○ L11_1:151

**Bi-encoder** — Separate query/doc embedding, fast  
◐ L13:746

**Bias monitoring** — Demographic performance disparity tracking  
◐ VitalCare:706

**Bicep** — Declarative Azure resource IaC  
● L02:216

**Bicep to Terraform mapping** — param, var, output, module, loops, conditionals  
● L33:155

**Big-O complexity table** — list vs dict vs set vs deque operations  
● L32:493

**Binarization** — Convert scan to pure black-and-white  
● L30:64

**Binary quantization** — 32x vector compression, ~5% loss  
○ L09:733

**BitsAndBytesConfig** — Enables 4-bit NF4 base loading  
◐ P6/08-LoRA/01_concepts:74

**Blast radius (state splitting)** — Split state by lifecycle and risk  
● L33:252

**Blob trigger** — Azure Function event on PDF upload  
◐ P6/02-Dealer/JMA-Complete-Flow:140

**Blue-green deployment** — Gradual traffic shift between versions  
● L06:714 · ◐ L19:88

**Blue-green index swap** — Zero-downtime embedding model migration  
◐ VitalCare:532

**Blue-green promotion** — 0-10-50-100 percent traffic shifts  
◐ L31:346

**BM25** — Keyword scoring in hybrid search  
◐ L09:334 · ◐ L11_2:401

**BM25 keyword search** — Exact-term index alongside vectors  
● P6/02-Dealer/WORKFLOW:140

**Bot Framework** — SDK for multi-channel conversational apps  
◐ L10:26

**Bot Framework Emulator** — Local desktop bot testing tool  
◐ L10:161

**Bot security layers** — JWT, SSO OBO, managed identity  
◐ L10:526

**boto3** — AWS Python SDK for Bedrock  
◐ P6/06-Bedrock/01_concepts:40

**Bounding box** — Object location rectangle coordinates  
◐ L04:47

**BPE (Byte Pair Encoding)** — Merge frequent adjacent character pairs  
● L11_2:52 · ◐ PyTrack/Part1:31

**Built-in skills** — Prepackaged enrichment operations  
◐ L09:245

**Business rule validation** — Domain rules on extracted fields  
◐ L30:182


## C

**Cache invalidation** — Event-driven vs TTL staleness control  
● IB/05_SolArch:342

**Caching (RAG)** — Redis in front of search/embeddings  
◐ L18:153

**CAG (Cache-Augmented Generation)** — Precomputed KV cache over static corpus  
● IB/03_RAG:104 · ● L23:27

**Canary / blue-green for GenAI** — Quality signals gate promotion  
● IB/06_RAI:198

**Caption (image)** — One-sentence image description  
◐ L04:123

**Captions (semantic)** — Highlighted relevant snippets per result  
◐ L09:435

**Cascading failure** — Bad specialist output reaches physician  
◐ L31:39

**Catastrophic forgetting** — Narrow fine-tune degrades general ability  
◐ P6/08-LoRA/03_interview_qa:55

**Causal masking** — Hiding future tokens during training  
◐ PyTrack/Part1:63

**CDK in C#** — Type-safe infrastructure with xUnit tests  
● L33:324

**Chain of Thought (CoT)** — Step-by-step reasoning before answer  
● L15:182

**Chain-of-Thought (CoT)** — Intermediate reasoning tokens as scratchpad  
● PyTrack/Part1:202

**Chain-of-thought prompting** — Force step-by-step reasoning before answer  
● L27:602

**Change detection** — Indexer reprocesses only changed docs  
◐ L09:212

**Chargeback / showback** — Per-tenant cost attribution tagging  
◐ IB/05_SolArch:404

**Chat Completions (Python)** — client.chat.completions.create call  
● L21:354

**ChatHistory** — SK rolling conversation thread  
● P6/02-Dealer/JMA-Complete-Flow:230 · ◐ L16:302

**ChatHistorySummarizationReducer** — SK built-in history summarizer  
● L16:1203

**ChatHistoryTruncationReducer** — SK sliding-window history reducer  
◐ L16:1197

**Checkov** — Policy scanner for Terraform, CFN, Bicep, K8s  
● L33:523

**Checkpointer** — LangGraph state persistence, resume after crash  
● L25:82

**checksum/config annotation** — Forces a rollout when a ConfigMap changes  
● L34:147

**Chroma** — Local open-source vector store  
○ L13:534

**Chunk overlap** — Shared tokens across chunk boundaries  
● L23:246 · ◐ L13:265

**Chunk size and overlap** — 200-800 tokens, 10-20% overlap  
● IB/03_RAG:20 · ◐ L09:946

**Chunk size guidelines** — 512 tokens default, overlap 50  
◐ L13:444

**Chunking (clinical document types)** — Per-document-type strategy and metadata  
● VitalCare:504

**Chunking strategies** — Fixed, recursive, semantic, paragraph  
● L23:222 · ● P6/02-Dealer/FLOW_WITH_LOOPS:36 · ● PyTrack/Part1:386

**Circuit breaker** — Open, half-open, closed failure states  
● L12:921 · ● L31:69 · ● P6/02-Dealer/FLOW_WITH_LOOPS:349

**Circuit states (CLOSED/OPEN/HALF-OPEN)** — Breaker lifecycle and probe recovery  
● L31:87

**Citation forcing** — Source IDs per claim, validated post-gen  
● IB/03_RAG:56

**Citation validation** — Verify Source N refs exist  
● L16:1918

**Citation verification** — Confirm cited chunk exists and supports claim  
● VitalCare:1296

**Citations in RAG** — Source file, page, excerpt  
● L13:903

**Claim confidence routing** — Auto / human review / dead letter thresholds  
● P6/02-Dealer/JMA-Complete-Flow:176

**ClaimDecisionPlugin** — Writes approval or escalation to DMS  
◐ P6/02-Dealer/WORKFLOW:166

**ClaimValidatorAgent** — Fast rule-based pre-check sub-agent  
◐ P6/02-Dealer/FLOW_WITH_LOOPS:304

**Classification** — Supervised prediction of a category  
◐ L01:171

**Claude on Bedrock** — Anthropic flagship models on AWS  
◐ P6/06-Bedrock/01_concepts:30

**Clinical Decision Support (CDS)** — Advisory-only guideline surfacing at care point  
● VitalCare:57

**ClinicalAgentBus** — C# validate-verify-audit-route implementation  
● L29:105

**Cluster analysis** — Grouping similar unlabeled records  
◐ L01:211

**Codebase context symbols** — @file, @folder, @codebase, @git, @docs  
◐ L35:73

**cog-jma-dev-frm-recognizer** — JMA dev Document Intelligence resource  
◐ L08:767

**Cognitive Services User role** — RBAC role granting inference calls  
◐ L07:98

**Coherence metric** — Logical structure and reasoning score  
◐ L17:425

**Cold-start retrieval problem** — Sparse index yields ungrounded confident answers  
◐ VitalCare:1281

**collections module** — defaultdict, Counter, deque  
● L32:521

**Community detection** — Clustering related graph entities  
◐ P6/07-GraphRAG/01_concepts:73

**Community summaries** — Cluster summaries enabling global questions  
● P6/07-GraphRAG/03_interview_qa:27

**Composed model** — Wraps custom models, auto-routes by type  
● L08:348

**Compounding errors** — Each step builds on hallucinated output  
◐ L24:100

**Compute cluster** — Autoscaling VM pool for training  
◐ L06:89

**Compute instance** — Single dev VM for notebooks  
◐ L06:88

**Concept drift** — Correct answer changes, data stale  
● L19:460

**Conditional edge** — Graph branch based on state value  
◐ L25:99

**Confidence gate** — Reject answer below score threshold  
● L13:955

**Confidence gating** — Stop when step confidence below threshold  
◐ L24:124

**Confidence routing** — Min-field-confidence branch to queues  
● L08:418 · ● L30:204 · ● P6/02-Dealer/FLOW_WITH_LOOPS:112

**Confidence score thresholds** — 0.90/0.70 reliability bands  
● L08:225

**Config-as-code vs IaC** — Configures the box versus creates the box  
● L33:391

**Connected Agents** — Agent calls another as sub-agent  
◐ L17:803

**Constitutional AI (CAI)** — Model self-critiques against written principles  
● L11_4:186

**Container billing heartbeat** — Containers must reach Azure billing  
● L07:230

**Containers for AI Services** — On-prem Docker images of AI APIs  
● L07:218

**Containment layer** — Actions taken when detection fires  
● L24:153

**Content Safety** — Input/output filters, PII, injection  
● L22:192

**Content Safety categories** — Hate, violence, sexual, self-harm  
● L17:617

**Content Safety categories/severity** — Hate/violence/sexual/self-harm thresholds  
● IB/06_RAI:11

**Content Safety severity levels** — 0/2/4/6 thresholds strict to lenient  
● L17:629

**Content Understanding** — Document Intelligence inside AI Foundry  
◐ L08:803 · ○ L17:804

**Context managers** — with statement, C# using, __enter__/__exit__  
● L32:387

**Context Precision** — Fraction of chunks actually used  
○ L13:1204

**Context precision (RAGAS)** — Are retrieved chunks relevant and ranked  
● P6/03-RAGAS/01_concepts:29

**Context Recall** — Did retrieval find all relevant  
○ L13:1205

**Context recall (RAGAS)** — Did retrieval find needed chunks  
● P6/03-RAGAS/01_concepts:26

**Context rot** — Long conversations degrade via bad summaries  
◐ HLP01:71

**Context window** — Max tokens input plus output  
● L11_2:157

**Context window budgeting** — Allocate tokens across prompt parts  
● L13:871

**Context window determinants** — Training-time positional/attention choice  
◐ IB/01_Fund:74

**Context window management** — Rolling window, summarization, retention  
● L27:738

**Context window vs memory** — Working set vs retention strategy  
● HLP01:27

**contextmanager decorator** — Generator form of a context manager  
● L32:420

**Contextual embeddings** — Vector varies with surrounding context  
◐ L11_2:290

**ContinueConversationAsync** — Sends proactive message via stored reference  
● L10:511

**Continuous evaluation architecture** — Offline gates plus online sampled scoring  
● IB/06_RAI:148

**Conversational Language Understanding (CLU)** — Intent plus entity extraction service  
● L03:420

**ConversationAnalysisClient** — C# client for CLU prediction  
● L03:498

**ConversationState** — Per-conversation bot state scope  
● L10:128

**Converse API (Bedrock)** — Provider-agnostic messages-in/out call  
● P6/06-Bedrock/01_concepts:52

**Copilot Studio** — Low-code Microsoft agent builder  
○ L16:2036

**Corrective RAG (CRAG)** — Re-query when retrieval confidence low  
● L13:1085

**CorrelationId** — Ties all messages in one workflow  
◐ L29:47

**Cosine similarity** — Angle between vectors, -1 to 1  
● L11_2:340 · ● L23:304 · ◐ IB/01_Fund:62 · ◐ L13:549 · ◐ L21:841

**Cosine similarity in FAISS** — Normalize vectors, use IndexFlatIP  
● P6/01-Ollama/03_interview_qa:26

**Cosine similarity metric** — Distance function for text embeddings  
◐ L09:513

**Cosmos DB vector store** — DiskANN vector index in Cosmos  
◐ L09:816

**CosmosDbPartitionedStorage** — Durable production bot state backend  
◐ L10:156

**Cost formula (monthly LLM)** — Queries times tokens times price  
● L18:336

**Cost per user interaction** — Sum all sub-calls per logical interaction  
● IB/05_SolArch:380

**Cost routing / model routing** — Route simple queries to cheap model  
● L12:983

**count vs for_each** — for_each gives stable keys — prefer it  
● L33:170

**crewAI Agent** — Role, goal, backstory LLM worker  
● P6/02-crewAI/01_concepts:25

**crewAI Crew** — Agents plus tasks plus process  
● P6/02-crewAI/01_concepts:37

**crewAI memory** — Short-term, long-term, entity memory  
◐ P6/02-crewAI/01_concepts:64

**crewAI Task** — Description, expected_output, agent, context  
● P6/02-crewAI/01_concepts:31

**Cross-attention vs self-attention** — Queries from different vs same sequence  
◐ IB/01_Fund:20

**Cross-encoder** — Scores query-doc pair jointly  
◐ L13:749

**Cross-encoder re-ranker** — Re-scores chunks to fix precision  
◐ P6/03-RAGAS/03_interview_qa:20

**Cross-field consistency validation** — Related fields must logically agree  
◐ L30:174

**Cross-session contamination** — Patient A PHI leaking to patient B  
◐ L24:355

**Cursor** — VS Code fork built around AI-assisted editing  
● L35:56

**Cursor agent mode** — Runs commands and iterates — branch only  
◐ L35:140

**Cursor Composer** — Multi-file AI edit — where the productivity gain is  
● L35:126

**cursorrules file** — Repo-root conventions prepended to every AI request  
● L35:91

**Custom exception hierarchy** — Catch at the altitude you care about  
● L32:444

**Custom Model training (DI)** — Label, train, evaluate, deploy workflow  
● L08:278

**Custom NER** — User-defined entity type extraction  
● L07:446

**Custom Neural model** — Semantic extraction across varied layouts  
● L08:265 · ◐ L30:100

**Custom Speech** — Domain-adapted speech recognition model  
● L05:308

**Custom Template model** — DI model for fixed-layout forms  
◐ L08:264 · ◐ L30:99

**Custom Text Classification** — Single or multi-label document classes  
◐ L07:451

**Custom Translator** — Domain-vocabulary translation model  
◐ L03:595

**Custom Vision** — Train your own image classifier/detector  
● L04:181

**Customer-managed keys** — Own encryption keys for index  
○ L20:486

**Cypher** — Neo4j pattern-matching query language  
● P6/07-GraphRAG/01_concepts:37


## D

**DALL-E 3** — Text-to-image generation model  
○ L12:61

**Data classes** — @dataclass generates init, repr, eq — C# record  
● L32:72

**Data classification** — Public/Internal/Confidential/Restricted tiers  
◐ L20:465

**Data drift** — Input distribution shifts over time  
● L06:753 · ● L19:452

**Data governance for AI** — Classify, PII, access, retention, compliance  
● L20:457

**Data residency** — Processing stays in one Azure region  
○ L20:485

**Data residency (US/EU/APAC)** — National-level PHI isolation requirements  
● VitalCare:939

**Data residency / sovereignty** — Region-pinned storage and compliant routing  
● IB/05_SolArch:244

**Data source (Search)** — Connection config to raw data store  
◐ L09:70

**Datastore** — Workspace connection to storage location  
◐ L06:63

**De-identification** — Replace PHI with synthetic tokens  
◐ L24:299

**De-identification (Safe Harbor)** — Removing 18 HIPAA identifiers for training  
◐ VitalCare:959

**De-noise** — Remove scan speckle and artifacts  
● L30:43

**De-skew** — Rotate tilted scans to horizontal  
● L30:55

**Dead letter queue** — Destination for failed extractions  
◐ L08:411 · ◐ P6/02-Dealer/JMA-Complete-Flow:191

**Dead-letter queue** — Preserve undeliverable messages, alert  
● L29:131 · ◐ L20:172

**Dead-letter replay** — Reprocess failed messages after fix  
◐ L31:150

**DealerEligibilityPlugin** — KernelFunction calling DMS eligibility API  
◐ P6/02-Dealer/WORKFLOW:112

**Declarative Agent (M365 Copilot)** — JSON-defined Copilot extension agent  
● L20:333

**Decoder-only architecture** — Generative GPT-style transformer  
◐ L11_1:152

**Decorator with arguments** — Three nesting levels — retry with backoff  
● L32:325

**Decorators** — Function taking a function, returning a replacement  
● L32:284

**Deep learning** — Multi-layer neural network learning  
◐ L01:96

**DefaultAzureCredential** — Credential chain resolving managed identity  
● L07:78 · ◐ L18:286 · ◐ P6/02-Dealer/FLOW_WITH_LOOPS:141

**Degradation chain (clinical)** — Five levels ending in human clinician  
● VitalCare:841

**Degradation ladder** — Full model to smaller to cache to retrieval-only  
◐ HLP01:259

**Deployment (Azure OpenAI)** — Named model instance with quota  
● L12:74

**deque** — Fixed-size sliding window for agent memory  
● L32:521

**Design patterns in Python** — Which GoF patterns collapse and why  
● L32:551

**Diagnostic logs** — Per-request audit logs to Log Analytics  
◐ L07:163

**Diarization** — Speaker separation in transcripts  
● L05:235

**Dictionaries (Python)** — Key-value mapping with get/keys/items  
● L21:128

**Dimension truncation** — Storing fewer embedding dimensions  
◐ L09:571

**Direct prompt injection** — User types override instructions  
◐ L15:494 · ◐ L24:223

**Distance metrics** — Cosine, dot product, Euclidean  
◐ L13:545

**Distillation** — Train small model on large outputs  
◐ L14:758

**DMS API** — JM Family dealer system of record  
◐ P6/02-Dealer/JMA-Complete-Flow:335

**Document (LlamaIndex)** — A loaded source item  
● P6/05-LlamaIndex/01_concepts:26

**Document Intelligence** — Structured extraction from documents  
● L08:30

**Document Intelligence custom model** — jma-incentive-claim trained extractor  
● P6/02-Dealer/JMA-Complete-Flow:150

**Document Intelligence layout model** — Extract ordered text and tables  
● L13:126

**Document Intelligence pipeline** — Extract fields from uploaded docs  
◐ L20:79

**Document Intelligence Studio** — Portal for labeling and training templates  
◐ L08:290 · ◐ L30:103

**Document Intelligence vs AI Search** — Reader versus finder distinction  
● L08:699

**Document-specific chunking** — Preserve tables and sections  
● L13:403

**Domain segregation (MCP)** — One domain per server rule  
◐ L26:262

**Dot product** — Equivalent to cosine when normalized  
◐ L11_2:361

**DPO (Direct Preference Optimization)** — Trains on chosen/rejected pairs, no reward model  
● PyTrack/1.4-FineTuning:549 · ○ L14:755

**Drift response playbook** — Actions per drift type detected  
● L19:477

**Drift types and detection** — Input, data, model-behavior, quality drift  
● IB/06_RAI:136

**Dynamic few-shot selection** — Pick relevant examples within token budget  
● L16:1487


**Dynatrace** — Commercial APM with automatic root-cause  
◐ L36:409

## E

**Embedding cache** — Hash-keyed reuse of chunk vectors  
◐ HLP01:249 · ◐ L18:384

**Embedding dimension consistency** — Index dim must match embedder  
◐ P6/01-Ollama/02_architecture:47

**Embedding dimensions** — Vector length, 256-3072 typical  
◐ L11_2:323

**Embedding drift** — Query and index in mismatched vector spaces  
● VitalCare:1263

**Embedding model selection** — Dimensionality vs cost/quality tradeoff  
◐ IB/03_RAG:32

**Embedding models (clinical)** — PubMedBERT, ClinicalBERT, BGE-M3 choices  
◐ VitalCare:513

**Embeddings** — Vectors encoding text meaning  
● L11_2:262 · ◐ IB/01_Fund:62

**Embeddings (Python)** — client.embeddings.create with batch input  
● L21:392

**Embeddings API** — Convert text to vectors  
● L12:283

**Encoder-decoder architecture** — T5/BART translate-summarize models  
○ L11_1:153

**End-to-end agent workflow** — Eleven-step receive-to-monitor pipeline  
● L27:65

**EnterpriseSearch.Sync** — JMA push-based SharePoint index WebJob  
● L09:1014

**Entity extraction (LLM)** — Turns text into nodes and edges  
● P6/07-GraphRAG/02_architecture:51

**Entity memory** — Extracted structured facts prepended each turn  
● PyTrack/1.5-Agents:1012

**Entity Recognition skill** — Extracts people, orgs, locations  
◐ L09:253

**Entra ID** — Identity, RBAC, conditional access  
○ L17:1102

**Environment variables / .env** — dotenv loading of secrets  
● L21:296

**Environments (Azure ML)** — Pinned reproducible Python dependencies  
◐ L06:102

**Episodic memory** — Past interactions as events — experience, not knowledge  
● HLP01:100

**Error handling pattern** — Status-code-specific catch branches  
● L08:490

**Escalation queue** — Route exceeded-limit sessions to humans  
◐ L31:144

**EscalationService** — Ticket plus RSM email/Teams notify  
● P6/02-Dealer/FLOW_WITH_LOOPS:355

**EU AI Act** — Risk tiers with deferred high-risk timeline  
● IB/06_RAI:236

**EU AI Act (healthcare high-risk)** — CDS as Annex III high-risk AI  
◐ VitalCare:932

**EU AI Act implications** — Oversight, audit, explainability requirements  
○ L18:469

**Euclidean distance** — Magnitude-sensitive, poor for text  
◐ L11_2:348

**Evaluation dataset preparation** — 20-50 question/answer test pairs  
● L17:446

**Evaluation flow (Foundry)** — Batch scoring pipeline against dataset  
● L17:443

**Evaluation framework (Foundry)** — Golden dataset, judge LLM, dashboard  
● L22:160

**Evaluation pipeline (quality gate)** — 100 golden cases gate deployment  
● P6/02-Dealer/JMA-Complete-Flow:547

**Event Grid trigger** — Near-real-time indexer run trigger  
◐ L09:910

**Event Grid vs Service Bus** — Push fire-and-forget vs pull durable  
● L20:120

**Eviction order** — Middle turns, then tool outputs, never system prompt  
● HLP01:59

**Exception chaining** — raise NewError from e preserves the cause  
◐ L32:472

**expected_output** — Task output contract keeping agent on target  
● P6/02-crewAI/03_interview_qa:17

**Experiments and jobs** — Tracked training runs with metrics  
◐ L06:122

**Explainable AI** — Showing features driving a prediction  
◐ L01:496

**Exponential backoff** — Doubling retry delay on throttling  
● L07:200 · ● L12:868 · ◐ L31:50


## F

**f-strings** — Python string interpolation  
◐ L21:82

**F1 Score** — Harmonic mean of precision and recall  
◐ L01:328

**Face Detection** — Locating faces without identity  
◐ L04:289

**Face Identification** — One-to-many face matching  
◐ L04:291

**Face Verification** — One-to-one face matching  
◐ L04:290

**Faceted filtering** — Structured field-based result narrowing  
◐ L08:555

**Facets** — Grouped counts for filter panels  
◐ L09:457

**Factory pattern (Python)** — A dict of callables  
◐ L32:572

**Factual hallucination** — Single call states falsehood confidently  
● L24:17

**Fail-fast strategy** — Whole task fails on specialist failure  
◐ L28:134

**Failover cutover mechanics** — Front Door health probes trigger routing  
◐ IB/05_SolArch:170

**Failure propagation (meta-agent)** — Bad sub-agent output corrupts levels above  
◐ IB/04_Agents:120

**Failure propagation in hierarchies** — Fail-fast, partial, retry-fallback options  
● L28:126

**Fairness metrics** — Equal performance across groups  
◐ L17:678

**Fairness principle** — No biased treatment across groups  
◐ L01:407

**FAISS** — In-memory vector index, IndexFlatIP  
● P6/01-Ollama/01_concepts:82 · ● PyTrack/Part1:465

**Faithfulness** — Claims supported by retrieved context  
● PyTrack/Part1:1201 · ◐ L13:1202

**Faithfulness (RAGAS)** — Claims supported by retrieved context  
● P6/03-RAGAS/01_concepts:19

**Fan-out / fan-in graph** — LangGraph parallel specialist dispatch  
◐ L28:116

**Feature** — Input variable used for prediction  
◐ L01:347

**Feature engineering** — Deriving new predictive inputs  
◐ L01:363

**Feature importance** — Ranked feature contribution scores  
● L06:375

**FedRAMP for LLM** — Whole boundary authorised — GovCloud or Azure Government  
● L33:509

**Feed-Forward Network (FFN)** — Per-token layers storing knowledge  
◐ L11_1:184

**Few-shot prompting** — 2-5 examples teach output pattern  
● L15:144 · ● L27:578

**FHIR** — Healthcare data interoperability standard  
◐ P6/05-VitalCare/README:112

**FHIR R4** — Healthcare interoperability resource standard  
● VitalCare:409

**Field attributes** — searchable/filterable/sortable/facetable/retrievable/key  
● L09:90

**Filtered retrieval** — Metadata pre-filter before vector search  
◐ L13:620

**Fine-tune for behavior, RAG for knowledge** — Decision rule for fine-tuning  
● P6/08-LoRA/01_concepts:17

**Fine-tune vs RAG vs Tool** — Speed of change decides capability  
● L17:855

**Fine-tuning** — Further training on task data  
● L11_3:163 · ● L22:250

**Fine-tuning cost/break-even** — Training, hosting, per-call economics  
● L14:148

**Fine-tuning UI (Foundry)** — No-code training job workflow  
● L17:556

**Fine-tuning workflow (Azure)** — Upload, job, monitor, deploy  
● L14:259

**finish_reason** — stop, length, content_filter values  
◐ L12:203

**FinOps governance** — Unit economics and spend anomaly alerts  
◐ VitalCare:911

**FinOps levers** — Caching, tiering, compression, iteration caps  
● L36:338

**Fixed-size chunking** — Split by token count with overlap  
● L13:244

**Fluency metric** — Natural grammatical language score  
◐ L17:430

**forces replacement** — The phrase to search for before approving a plan  
● L33:209

**Format validation** — Extracted value matches expected pattern  
◐ L30:165

**Foundry Hub** — Azure infrastructure layer, shared resources  
● L22:47

**Foundry Local** — On-device model runtime, NPU/CPU  
○ L17:1100

**Foundry Project** — Team workspace inside a hub  
● L22:49

**Foundry SDK (azure-ai-projects)** — Agents, inference, evaluations, memory  
◐ L17:1096

**Framework selection criteria** — Map problem shape to framework  
● L25:257

**FraudDetectorAgent** — Anomaly scoring specialist sub-agent  
◐ P6/02-Dealer/FLOW_WITH_LOOPS:309

**Freshness scoring function** — Recency-based relevance boost  
◐ L09:616

**Frozen dataclass** — Immutable value object for agent config  
◐ L32:122

**Full fine-tuning** — All weights updated, ~28GB for 7B  
◐ P6/08-LoRA/01_concepts:23

**Full-text search** — Tokenized keyword BM25 matching  
◐ L09:327

**Function calling** — Model emits JSON request, your code executes  
● L12:338 · ● PyTrack/1.5-Agents:561

**Function calling (Python)** — Tool schema and tool_calls handling  
● L21:410

**Function calling vs agent** — Who decides what vs whether to run  
◐ HLP01:274

**FunctionInvocationFilter** — SK hook before/after tool invocation  
● L16:923 · ● L27:365 · ● P6/02-Dealer/README:114


**functools.wraps** — Preserves name, docstring, signature — needed by @tool  
● L32:309

## G

**Game-day failover testing** — Deliberately triggered controlled failover  
◐ IB/05_SolArch:206

**Gated model** — License acceptance plus token required  
◐ P6/04-HuggingFace/03_interview_qa:41

**Gemini** — Google multimodal flagship model family  
● P6/09-Vertex/01_concepts:25

**Gemini long context** — 1-2M token window vs 128k  
◐ P6/09-Vertex/03_interview_qa:44

**GenAI semantic conventions** — Standard gen_ai.* span attributes  
● L36:92

**Generative AI** — Deep learning creating new content  
◐ L01:99

**Generator exhaustion** — Single-use — second iteration silently empty  
● L32:271

**Generator expression** — Parentheses not brackets — nothing materialises  
● L32:220

**get_peft_model** — Wraps base model with LoRA adapters  
● P6/08-LoRA/01_concepts:88

**GGUF** — Quantized model file format for llama.cpp  
● P6/01-Ollama/01_concepts:76

**GitOps** — Git is truth; an in-cluster agent reconciles continuously  
● L34:206

**GKE Autopilot** — No node management, pay per pod  
◐ L34:298

**Global Standard deployment** — Global-routed Azure OpenAI deployment type  
○ L17:1054

**Goal drift** — Agent loses original objective mid-workflow  
◐ L24:99

**Goal misgeneralization** — Correct in training, wrong deployed  
○ L11_4:180

**Golden dataset** — Labeled reference set, versioned like code  
● IB/06_RAI:112 · ● L19:574 · ● P6/03-RAGAS/01_concepts:88

**google-genai SDK** — Client targeting Gemini API and Vertex  
● P6/09-Vertex/01_concepts:44

**GPT vs BERT vs T5** — Decoder, encoder, encoder-decoder tasks  
◐ PyTrack/Part1:66

**GPT-2 / PEFT local demo** — Small model LoRA parameter comparison  
● PyTrack/1.4-FineTuning:277

**GPT-4.1** — 1M context, better instruction following  
○ L12:698

**GPT-4o** — 128k multimodal production model  
◐ L12:54

**GPT-4o mini** — Cheap high-volume model, 17x cheaper  
◐ L12:55

**Grafana and Prometheus** — Custom LLM and agent dashboards  
● L36:395

**Graph retrieval (UMLS/SNOMED)** — Clinical concept graph for CDS reasoning  
◐ VitalCare:543

**GraphRAG** — RAG retrieving via graph traversal  
● P6/07-GraphRAG/01_concepts:59 · ◐ IB/03_RAG:92 · ◐ L13:1320

**Groundedness** — Every claim supported by retrieved context  
● L22:155

**Groundedness as safety control** — Block/disclaim/human-review policy on failure  
◐ IB/06_RAI:35

**Groundedness detection** — Post-gen check of claim support  
● IB/03_RAG:68 · ● L11_4:348 · ● L17:644 · ● L22:197

**Groundedness drift detection** — Rolling-average quality decline alarm  
● L31:107

**Groundedness evaluation** — Judge scores claim support in RAG  
● L24:48

**Groundedness metric** — Answer supported by retrieved chunks  
● L17:407

**Groundedness monitor** — Async GPT-4o judge scoring live decisions  
● P6/02-Dealer/JMA-Complete-Flow:521

**Groundedness validation in code** — Content Safety API check pattern  
● L16:1871

**Grounding with Bing Search** — External web grounding connection  
○ L17:1058

**GroupChat** — AutoGen shared multi-agent conversation  
◐ L25:192

**GroupChatManager** — LLM picks next speaker, non-deterministic  
◐ L25:198

**Guardrails** — Three validation layers before delivery  
● L27:338

**Guardrails (agent)** — Iteration caps, allow-lists, HITL, audit  
◐ HLP01:323

**Guardrails (input/output)** — Presidio, jailbreak, clinical safety classifier  
● VitalCare:676


## H

**Hallucination** — Fluent confident wrong model output  
● L24:9

**Hallucination mitigation stack** — Groundedness, refusal, confidence gate, citation check  
● VitalCare:578

**Hallucination prevention** — Prompt, temp 0, threshold, citations  
● L13:943

**Hallucination rate** — Share of unsupported answers  
◐ L19:365

**Hallucination root cause** — Probabilistic generation without verification  
◐ IB/01_Fund:111

**Handwriting recognition** — Read API handwritten text support  
◐ L04:408

**HAPI FHIR** — Open-source portable FHIR R4 server  
◐ VitalCare:409

**HCL** — HashiCorp Configuration Language  
● L33:112

**Helm** — Templating and release management for Kubernetes  
● L34:32

**helm --atomic** — Auto-rollback on failed or timed-out upgrade  
● L34:167

**Helm chart anatomy** — Chart.yaml, values.yaml, templates, charts  
● L34:56

**Helm release and revision** — Named versioned install — what rollback targets  
● L34:181

**Helm template syntax** — Values, Release, nindent, include, range  
● L34:147

**Hero Card** — Image, title, buttons bot card  
◐ L10:207

**HF Hub** — Registry of models, datasets, Spaces  
● P6/04-HuggingFace/01_concepts:25

**HHH framework** — Helpful, Harmless, Honest targets  
◐ L11_4:133

**Hierarchical chunking** — Section summaries plus paragraph chunks  
◐ IB/03_RAG:188

**Hierarchical process (crewAI)** — Manager LLM plans and delegates  
● P6/02-crewAI/01_concepts:41

**High water mark** — Last-processed timestamp for incremental indexing  
◐ L09:219

**HIPAA audit logging** — Log every inter-agent message  
◐ P6/05-VitalCare/README:106

**HIPAA Security Rule** — Safeguards; LLM logs with PHI are PHI  
◐ VitalCare:928

**HITRUST CSF** — Certifiable control framework mapping HIPAA/NIST  
○ VitalCare:935

**HL7 v2** — Legacy clinical messaging needing FHIR transform  
◐ VitalCare:67

**HMAC signature** — Message integrity, prevents forged agent messages  
● L29:53

**HNSW** — Graph-based approximate vector index  
● L09:507 · ● L13:559

**HNSW complexity** — Why an ANN index beats brute-force vector search  
◐ L32:537

**HNSW index config** — m, efConstruction, efSearch, cosine  
● P6/02-Dealer/FLOW_WITH_LOOPS:67

**HNSW indexing** — Multi-layer graph ANN vector search  
● L23:274

**HNSW parameters** — m, efConstruction, efSearch tuning  
● L09:508

**Hough transform** — Detects dominant line angle for de-skew  
◐ L30:60

**Hub-based project (classic)** — AML-style project auto-provisioning four resources  
◐ L17:1087

**HuggingFace pipeline()** — One-liner tokenizer plus model plus postproc  
● P6/04-HuggingFace/01_concepts:34

**Human review queue** — Mid-confidence extractions routed to reviewer  
◐ L30:211 · ◐ P6/02-Dealer/JMA-Complete-Flow:186

**Human-in-the-loop** — Confirm before high-stakes action  
◐ L01:516 · ◐ L16:943

**Human-in-the-loop escalation** — Confidence-triggered handoff with context  
● VitalCare:719

**Human-in-the-loop gate** — Pause before high-stakes decisions  
◐ L24:118

**Human-in-the-loop interrupt** — LangGraph interrupt_before/after primitive  
◐ L25:105

**Hybrid MCP + APIM pattern** — MCP for agents, APIM for governance  
● L26:93

**Hybrid query** — Keyword plus vector combined search  
● L09:397

**Hybrid RAG + CAG pattern** — Static in context, dynamic retrieved  
◐ L23:123

**Hybrid retrieval** — Vector plus BM25 plus RRF plus rerank  
● P6/02-Dealer/WORKFLOW:134

**Hybrid retrieval (graph + vector)** — Vector for passages, graph for connections  
● P6/07-GraphRAG/03_interview_qa:33

**Hybrid search** — BM25 plus vector fused via RRF  
● L11_2:396 · ● L23:65 · ● PyTrack/Part1:421

**Hybrid search (Python)** — Keyword plus vector query combined  
● L21:745

**Hybrid search + reranking pipeline** — Parallel retrieval, fusion, cross-encoder  
● IB/03_RAG:44

**HyDE** — Embed hypothetical answer, not question  
● L13:670


## I

**IAM roles (AWS)** — Least-privilege Bedrock invoke auth  
◐ P6/06-Bedrock/03_interview_qa:23

**Idempotency (MessageId)** — Duplicate delivery ignored by receiver  
◐ L29:61

**Image Analysis 4.0** — Caption, tags, objects, OCR API  
● L04:119

**Image Classification** — Whole-image label assignment  
◐ L04:46

**Import and vectorize wizard** — No-code chunk-embed-index setup  
● L09:934

**Inclusiveness principle** — Accessible AI for all users  
◐ L01:467

**Index (AI Search)** — Schema plus stored searchable documents  
◐ L09:67

**Index freshness / re-indexing** — Event-driven vs scheduled batch update  
● IB/03_RAG:140

**Index schema (JMA)** — Keyword-only fields, no vectors  
● L09:1082

**Index staleness (clinical)** — Superseded guidelines still retrievable  
● VitalCare:1269

**Indexer** — Scheduled pull ingestion pipeline  
● L09:68

**Indexer schedule** — none/once/5min/hourly/daily polling  
◐ L09:894

**IndexFlatL2 / IndexFlatIP** — Exact brute-force FAISS index types  
● P6/01-Ollama/03_interview_qa:23

**IndexHNSWFlat** — Approximate nearest neighbor FAISS index  
◐ P6/01-Ollama/01_concepts:88

**Indirect prompt injection** — Instructions hidden in retrieved docs  
● L11_4:271 · ◐ L15:501 · ◐ L18:230 · ◐ L24:228

**Inference phase** — Applying trained model to new data  
◐ L01:138

**Infrastructure Manager (GCP)** — Managed Terraform — GCP's native IaC path  
◐ L33:309

**Input validation (LLM)** — Block injection patterns before call  
● L18:253

**Instruction fine-tuning (SFT)** — Instruction-response pair training  
● L11_3:182

**Instruction hierarchy enforcement** — Separate system, user, retrieved content  
◐ L24:251

**Integrated vectorization** — Index auto-embeds text at query time  
◐ L09:304

**Intents** — User goals recognized by CLU  
● L03:448

**Intermediate state checkpointing** — Log input/tools/output per agent step  
◐ L24:115

**IRSA** — IAM Roles for Service Accounts — EKS workload identity  
● L34:298

**Istio service mesh** — mTLS and traffic policy sidecar  
◐ VitalCare:390


**Iterators and generators** — Lazy sequence, C# yield return  
● L32:180

**itertools** — islice, chain, groupby, batched  
◐ L32:256

## J

**Jailbreak** — Adversarial bypass of safety training  
● L24:262

**Jailbreak detection** — Prompt Shields manipulation classifier  
◐ IB/06_RAI:23

**Jailbreaking** — Inputs bypassing safety training  
◐ L11_4:227

**Jitter** — Randomized retry delay spread  
◐ L31:66

**jma-func-aisync** — JMA sync Function App identity chain  
◐ L02:356

**John Snow Labs** — Clinical NLP and de-identification platform  
● L30:130

**JSON handling (Python)** — loads/dumps/load/dump functions  
● L21:269

**JSON mode** — Force valid JSON responses  
◐ L15:654

**JSONL training format** — One conversation per line  
● L14:201

**Judge LLM** — Model scoring another model's output  
◐ L24:53

**Jupyter notebooks** — Interactive code and markdown cells  
◐ L21:504

**JWT validation (APIM)** — Token check before tool routing  
◐ P6/02-Dealer/JMA-Complete-Flow:506


## K

**Karpenter** — Per-pod instance provisioning — matters for GPU  
● L34:298

**KEDA autoscaling** — Queue-depth-driven GPU pool scaling  
◐ VitalCare:816

**Kernel** — SK container for plugins and services  
◐ L25:41

**Kernel (Semantic Kernel)** — Central coordinator wiring LLM/plugins  
● L16:147

**KernelFunction** — One callable tool method  
● L16:173 · ● L27:165

**KernelFunction plugin** — C# method exposed as LLM tool  
● P6/02-Dealer/JMA-Complete-Flow:220

**Key Phrase Extraction** — Pulls most important phrases  
◐ L03:215

**Key rotation** — Two keys enable zero-downtime rotation  
◐ L07:66

**Key-value extraction** — Detects labeled field-value pairs  
◐ L08:37

**KL divergence penalty** — Constrains PPO weight drift  
◐ L11_4:102

**KNN (exact)** — Exhaustive accurate vector search  
◐ L09:391

**Knowledge cutoff** — Training data snapshot date  
● L11_3:113

**Knowledge graph** — Nodes, edges, properties model  
● P6/07-GraphRAG/01_concepts:21

**Knowledge Store** — Enriched output projected to storage  
◐ L09:294

**Kubernetes deployment (AI containers)** — Replicated container deployment manifest  
◐ L07:281

**KV cache** — Cached attention state enabling CAG  
◐ L23:77


## L

**Labeling tips (DI)** — More docs, all occurrences, variation  
◐ L08:315

**Labels** — Target value model predicts  
◐ L01:349

**LangChain** — Python AI orchestration framework  
● L21:582 · ● L25:114 · ◐ L16:334 · ◐ PyTrack/1.5-Agents:1375

**LangChain-to-SK concept mapping** — Table equating LangChain and SK  
◐ L21:594

**Langfuse** — Self-hostable LLM tracing — the compliance choice  
● L36:199

**LangGraph** — StateGraph nodes, edges, conditional routing  
● L25:72 · ● PyTrack/1.5-Agents:1524 · ◐ P6/02-crewAI/01_concepts:74 · ○ L16:2035

**LangGraph vs Semantic Kernel** — Explicit state machine vs implicit loop  
◐ HLP01:309

**LangSmith** — LangChain-native tracing, datasets and evaluators  
● L36:183

**Language Detection** — Identifies text language with score  
◐ L03:156

**Language model (speech)** — Assembles words from phonemes  
◐ L05:41

**Late chunking** — Embed document first, chunk embeddings  
● L13:1470

**Latency breakdown (RAG)** — Embed, search, generate, network timings  
● L18:164

**Latency budget** — Per-step allocation to meet P95  
◐ L18:488

**Latency percentiles (p50/p95/p99)** — Distribution-based latency monitoring  
◐ L31:179

**Layout analysis** — Pages, lines, tables, selection marks  
◐ L08:36

**LCEL (LangChain Expression Language)** — Pipe syntax for RAG chains  
◐ L25:174

**Lemmatization** — Reduce word to dictionary form  
◐ L03:96

**Limited Access policy** — Application required for face recognition  
● L04:298

**List comprehension** — Inline map/filter over sequence  
● L21:119

**List vs set membership** — O(n) versus O(1) — the common quadratic bug  
● L32:506

**LiteLLM** — Portable OpenAI-compatible model router  
● L36:229 · ◐ VitalCare:399

**LLaMA 3** — Default general-purpose local model  
◐ P6/01-Ollama/01_concepts:58

**LlamaIndex Node** — Chunk plus metadata plus relationships  
● P6/05-LlamaIndex/01_concepts:27

**LlamaIndex QueryEngine** — Retrieve, prompt, LLM, cited answer  
● P6/05-LlamaIndex/01_concepts:30

**LlamaIndex Retriever** — Pulls top-K relevant nodes  
◐ P6/05-LlamaIndex/01_concepts:29

**LlamaIndex Settings** — Global llm and embed_model config  
● P6/05-LlamaIndex/02_architecture:49

**LlamaIndex VectorStoreIndex** — Standard embedding-based searchable index  
● P6/05-LlamaIndex/01_concepts:28

**LLM alerting thresholds** — Cost/hour, cache collapse, groundedness drop  
● L36:439

**LLM cost guardrails** — Budgets, rate limits, iteration caps, alerts  
● L36:371

**LLM cost model** — Output tokens cost 3-5x input; loops compound  
● L36:326

**LLM observability** — Why HTTP 200 is not success for an agent  
● L36:31

**LLM-as-judge** — Rubric scoring calibrated against humans  
● IB/06_RAI:124 · ● P6/03-RAGAS/01_concepts:62 · ◐ L13:1202

**LLM-as-judge bias** — Self-preference, verbosity, position bias  
◐ P6/03-RAGAS/03_interview_qa:38

**LLMLingua** — Perplexity-based prompt compression library  
● L16:1431

**LLMOps** — DevOps for LLM applications  
● L19:61 · ● L31:227

**LLMOps dashboard** — Prompt version, quality, cost, latency  
◐ L31:378

**LLMOps maturity levels** — Level 0 manual to 3 advanced  
● L19:620

**Logit bias** — Boost/ban specific token IDs  
◐ PyTrack/Part1:346

**Loop controls / max iterations** — Hard stop on runaway ReAct loops  
◐ L27:292

**LoRA** — Low-rank delta-W = A x B factorization  
● L11_3:270 · ● L14:480

**LoRA (Low-Rank Adaptation)** — Frozen weights plus trained AxB matrices  
● L23:405 · ● PyTrack/1.4-FineTuning:192

**LoRA / QLoRA / DPO** — Fine-tuning job techniques in Foundry  
○ L17:1082

**LoRA adapter artifact** — Few-MB trained matrices, not base model  
● P6/08-LoRA/03_interview_qa:34

**LoRA low-rank decomposition** — Delta-W approximated by A times B  
● P6/08-LoRA/01_concepts:36

**LoRA rank r** — Adapter capacity vs trainable parameters  
● PyTrack/1.4-FineTuning:256

**lora_alpha** — Adapter output scaling, typically 2r  
● P6/08-LoRA/01_concepts:64

**LoraConfig** — PEFT adapter definition object  
● P6/08-LoRA/01_concepts:86

**Loss curves / overfitting** — Validation loss rising signals overfit  
● L14:380

**Lost in the middle** — Middle context gets less attention  
● L11_2:226 · ◐ HLP01:72


**lru_cache** — Free memoisation — needs hashable args  
◐ L32:362

## M

**MAE** — Mean absolute prediction error  
◐ L01:338

**Managed compute deployment** — Dedicated GPU cluster, fixed cost  
● L17:189

**Managed Identity** — Keyless Azure AD resource credential  
● L02:188 · ● L12:524 · ◐ L18:236

**Managed Identity per agent** — Each agent has own Azure AD identity  
◐ L29:94

**Managed Identity vs API keys** — DefaultAzureCredential eliminates secrets  
◐ IB/02_Azure:20

**Managed Online Endpoint** — Real-time REST inference endpoint  
● L06:554

**Many-shot prompting** — 50-100 examples in large context  
○ L15:744

**Masked self-attention** — Hide future tokens during generation  
◐ L11_1:177

**Matryoshka embeddings** — Variable-dimension truncatable embeddings  
◐ L09:571

**Matryoshka embeddings (MRL)** — Truncatable variable-dimension embeddings  
○ L11_2:538

**Max tool calls limit** — Cap before escalation (10 / 8 clinical)  
◐ L31:141

**max_tokens** — Cap on generated response length  
◐ L12:165

**MaxAutoInvokeAttempts** — ReAct loop iteration cap of 10  
◐ P6/02-Dealer/JMA-Complete-Flow:258

**MCP (Model Context Protocol)** — Open standard for agent-tool connection  
● L26:22

**MCP governance** — Enterprise policies over server pool  
● L26:130

**MCP Hub** — Central gateway routing agents to tools  
● L26:40 · ◐ IB/04_Agents:132

**MCP Hub / tool registry** — Central tool discovery and routing  
● P6/02-Dealer/WORKFLOW:213

**MCP server** — Code implementing tool endpoints  
◐ L26:35

**Measuring AI productivity** — Baseline first, cite sample size  
● L35:182

**Medical NER** — Extracts diagnoses, drugs, dosages  
◐ L30:136

**Memory layers (four)** — Working, session, long-term, scratchpad  
● HLP01:33

**Memory management (agents)** — Prevent context overflow in conversations  
● L16:1063

**Memory poisoning** — Injected content persisted and re-served  
◐ HLP01:75

**Memory staleness / invalidation** — TTL or re-validate before consequential use  
◐ IB/04_Agents:206

**Memory Store (Foundry agents)** — Long-term cross-session agent memory  
○ L17:1070

**Memory strategies (five)** — Buffer, sliding, summarize, vector, hybrid  
● HLP01:46

**Memory types (agent)** — Short-term, summary, entity, long-term  
● PyTrack/1.5-Agents:867

**Meta-agent** — Agent that decomposes and delegates  
● L28:23

**Meta-prompting** — LLM improves its own prompt  
◐ L15:440

**Metadata extraction** — Chunk fields for filter and citation  
● L13:199

**Metadata filtering (clinical)** — effective_date, plan_id, jurisdiction filters  
● VitalCare:548

**Metadata-filtered retrieval** — Tenant scope enforced inside search query  
● IB/03_RAG:176

**Microsoft 365 Copilot** — Copilot embedded in Office apps  
◐ L20:269

**Microsoft Container Registry (MCR)** — Hosts AI service container images  
◐ L07:236

**Microsoft Graph** — API for all M365 data  
● L20:289

**Microsoft Graph API** — SharePoint document read source  
◐ L09:1044

**Microsoft GraphRAG** — Open-source graph construction project  
◐ P6/07-GraphRAG/01_concepts:73

**Mixture of Experts (MoE)** — Router activates subset of experts per token  
● IB/01_Fund:160 · ◐ L11_1:316

**MLOps** — DevOps for traditional ML models  
● L19:61

**MLOps vs LLMOps** — Model weights versus prompts comparison  
● L19:38

**MMR (Maximal Marginal Relevance)** — Balance relevance against redundancy  
● L13:699

**Model access opt-in (Bedrock)** — One-time console enablement per model  
◐ P6/06-Bedrock/01_concepts:36

**Model approval workflow** — Staged intake, risk tier, sign-off, registry  
● IB/06_RAI:212

**Model catalog** — 1,600+ browsable deployable models  
● L17:157

**Model comparison (side by side)** — Same prompt across multiple models  
◐ L17:229

**Model deprecation / retirement** — 12-month advance retirement notice  
○ L19:696

**Model drift** — Provider silently updates hosted model  
● L06:753 · ● L19:469

**Model Garden** — Vertex catalog with Claude, Llama, Gemma  
◐ P6/09-Vertex/01_concepts:115

**Model lifecycle (LLM)** — Select, prompt, evaluate, deploy, monitor  
● L19:102

**Model Registry** — Versioned trained model storage  
● L06:144

**Model registry (MLflow)** — Versioned models with clinical sign-off  
◐ VitalCare:1034

**Model rollback** — Model+prompt+config as one validated unit  
● IB/06_RAI:186

**Model router** — Route by query complexity to cheaper model  
◐ L17:807

**Model routing (clinical tiers)** — Four tiers gated by PHI detection  
● VitalCare:631

**Model routing / tiering** — Cheap triage model, expensive escalation  
● IB/05_SolArch:392

**Model routing and fallback** — Automatic failover on 429 or outage  
● L36:243

**Model selection criteria** — Compliance, cost/quality, task type  
● L17:202

**Model tier selection** — Route task complexity to cheapest model  
● L27:721

**Model tiering** — Local triage escalating to cloud model  
● L36:338 · ● P6/01-Ollama/03_interview_qa:47

**Model version pinning** — Never use latest for clinical AI  
● L36:243 · ◐ VitalCare:1388

**Model versioning (DI)** — Manual model ID swap strategy  
◐ L08:378

**Multi RAG (multi-modal)** — Text plus image vector indexing  
◐ L09:982

**Multi-agent cost modeling** — Token cost per workflow not query  
◐ L18:465

**Multi-agent memory transfer** — Full history, summary, or shared state  
● HLP01:80

**Multi-agent patterns** — Supervisor, sequential, peer-to-peer  
◐ HLP01:316

**Multi-agent systems** — Specialist agents with focused prompts  
● L16:850 · ● PyTrack/1.5-Agents:1121

**Multi-cloud single apply** — AWS and Azure resources in one configuration  
● L33:286

**Multi-head attention** — Parallel heads learn different relations  
● L11_1:107

**Multi-hop Cypher MATCH** — Single pattern spanning several relationships  
● P6/07-GraphRAG/01_concepts:49

**Multi-hop RAG** — Follow-up queries discovered at query time  
◐ IB/03_RAG:116

**Multi-index agentic RAG** — Agent routes across several indexes  
◐ L16:706

**Multi-LoRA serving** — One base, hot-swappable task adapters  
◐ P6/08-LoRA/03_interview_qa:58

**Multi-query retrieval** — Generate query variants, deduplicate  
● L13:638

**Multi-service resource** — One key covering many AI services  
◐ L02:84

**Multi-tenant isolation model** — Shared with filters vs dedicated capacity  
◐ IB/05_SolArch:418

**Multi-turn conversations** — Follow-up prompts in knowledge base  
◐ L03:370

**Multimodal architecture** — ViT patch embeddings into shared token space  
◐ IB/01_Fund:170

**Multimodal RAG** — Retrieve over images plus text  
○ L13:1323

**Multimodal transformers** — One model for text/image/audio  
○ L11_1:319


**Mutable default argument** — Defaults evaluated once — shared across calls  
● L32:137

## N

**N+M connections (MCP)** — Hub avoids NxM agent-tool wiring  
◐ P6/02-Dealer/WORKFLOW:224

**N8N** — Low-code workflow automation with LLM nodes  
◐ L35:245

**Naive RAG** — Basic retrieve-prompt-generate pipeline  
◐ L13:65

**Named Entity Recognition** — Classify real-world entities in text  
◐ L03:109

**Neo4j** — Leading graph database, bolt 7687  
● P6/07-GraphRAG/01_concepts:106

**Neural voices** — Natural-sounding synthesized voices  
◐ L05:110

**Next-token prediction** — Pre-training objective at scale  
● L11_3:50

**NF4** — 4-bit normal-float training quantization  
● P6/08-LoRA/01_concepts:103

**NF4 quantization** — 4-bit NormalFloat base model storage  
● PyTrack/1.4-FineTuning:363

**Node (LangGraph)** — Python function returning state delta  
◐ L25:81

**Node relationships** — Parent/sibling links enabling merged retrieval  
● P6/05-LlamaIndex/02_architecture:51

**Node types (Prompt Flow)** — LLM, embedding, search, template, condition  
● L17:300

**Noisy neighbours (retrieval)** — Semantically close but wrong-context chunks  
◐ VitalCare:1275

**Non-deterministic routing** — Same input yields different execution path  
◐ L25:198

**N×M connection problem** — Hub reduces integrations to N+M  
◐ L26:52


## O

**o1 / o3 reasoning models** — Internal chain-of-thought before answering  
◐ L12:695

**Object Detection** — Locate objects with bounding boxes  
◐ L04:47

**Observability layers** — Infra, AI metrics, quality metrics  
● L19:346

**Observability three layers** — Infrastructure, AI service, quality  
● L31:167

**Observe step** — Agent reads tool result, updates reasoning  
● L27:255

**OCR pipeline** — Structured extraction from messy documents  
● L30:31

**OCR skill** — Extracts text from images during indexing  
◐ L09:249

**OCR vs Document Intelligence** — Raw text versus structured fields  
● L04:355

**OCR vs Read API** — Legacy sync versus current async OCR  
◐ L04:339

**OData filter syntax** — Expression language for $filter  
◐ L09:356

**OIDC federated credential** — Short-lived pipeline token, no stored secret  
● L34:386

**Ollama** — Local model server on port 11434  
● P6/01-Ollama/01_concepts:9

**Ollama OpenAI-compatible endpoint** — Swap base URL, keep OpenAI SDK  
● P6/01-Ollama/01_concepts:36

**OMOP CDM** — Open research common data model on Iceberg  
◐ VitalCare:1326

**On Your Data** — Azure managed RAG data_sources  
● L13:1006 · ◐ L12:494

**OnMessageActivityAsync** — Handler for incoming text messages  
● L10:96

**ONNX export** — Offline edge model deployment format  
● L04:268

**OPA and Rego** — Open policy engine for any JSON or YAML  
◐ L33:523

**OpenShift** — Red Hat Kubernetes — hybrid on-prem and cloud  
● L34:362

**OpenTelemetry** — Vendor-neutral traces, metrics and logs  
● L36:58

**OpenTelemetry tracing** — OTel exporter for Foundry traces  
○ L17:1078

**Operation-Location header** — Async operation polling URL  
◐ L08:69

**Opinion Mining** — Extracts sentiment target and assessment  
◐ L03:133

**OTel Collector** — Fan-out to backends without touching app code  
● L36:397

**Output format constraints** — JSON schema enforced in system prompt  
● L27:642

**Output validation** — Post-loop status check before returning  
◐ P6/02-Dealer/JMA-Complete-Flow:414

**Over-refusal** — Refusing legitimate requests too cautiously  
◐ L11_4:179

**Overconfidence** — No reliable internal confidence signal  
◐ L24:34

**Overfitting** — Memorized training data, poor generalization  
● L01:309

**Overfitting in fine-tuning** — Val loss rises while train loss falls  
● P6/08-LoRA/01_concepts:123


## P

**P95 latency** — 95th percentile response time  
◐ L19:424

**Pandas basics** — read_csv, head, describe, isnull  
◐ L21:551

**Parallel function calling** — Multiple tool calls one round trip  
● L12:749

**Parallel vs sequential execution** — Supervisor dispatch strategy choice  
● L28:92

**Parameters (model)** — Learned weights across layers  
○ L11_1:243

**Parametric knowledge gap** — Model never trained on your data  
◐ L24:30

**Parent-child chunking** — Child embedded, parent injected  
● L13:1390 · ● L13:458 · ● P6/02-Dealer/FLOW_WITH_LOOPS:44

**Partial result strategy** — Synthesize available, flag the gap  
◐ L28:135

**Partitions** — Index shards for storage and throughput  
◐ L09:125

**PEFT** — HuggingFace parameter-efficient fine-tuning library  
● P6/08-LoRA/01_concepts:79

**PEFT / LoraConfig** — HuggingFace LoRA training config  
● L14:608

**pgvector** — PostgreSQL vector extension  
◐ L09:803 · ○ L13:535

**PHI containment rule** — PHI never leaves its zone or region  
● VitalCare:369

**PHI sensitivity segregation** — PHI vs non-PHI server zones  
◐ L26:281

**Phi-4** — 3.8B model near GPT-4 quality  
◐ L11_3:506

**PHI-safe logging** — Log member IDs, never names or DOB  
● P6/05-VitalCare/README:98

**Phonemes** — Sound units in speech pipeline  
◐ L05:39

**PII controls across pipeline** — Ingestion, inference, and logging exposure  
● IB/06_RAI:98

**PII Detection** — Find and redact personal information  
● L03:249

**PII detection (Python)** — recognize_pii_entities on text  
◐ L21:720

**PII detection and redaction** — Detect-and-block or tokenize identifiers  
● L24:283 · ◐ L20:469

**Pinecone** — Managed pure vector database  
◐ L09:861 · ○ L13:532

**Planner (SK)** — Breaks goals into steps  
○ L16:135

**Plugin (Semantic Kernel)** — Class grouping callable functions  
● L16:168

**PolicyCheckerAgent** — RAG-based policy evaluation sub-agent  
◐ P6/02-Dealer/FLOW_WITH_LOOPS:309

**PolicyLookupPlugin** — KernelFunction performing hybrid retrieval  
● P6/02-Dealer/WORKFLOW:127

**Polly** — .NET retry and circuit breaker library  
◐ P6/02-Dealer/FLOW_WITH_LOOPS:340

**Polly retry policy** — .NET resilience retry library  
● L07:195

**POS tagging** — Label words by grammatical role  
◐ L03:105

**Positional encoding** — Adds token order to embeddings  
● L11_1:129 · ◐ IB/01_Fund:33

**Post-processing validation** — Format, cross-field, business rule layers  
● L30:159

**Power Platform AI Builder** — No-code AI for citizen developers  
◐ L20:380

**PPO** — RL step maximizing reward model  
● L11_4:91

**Pre-training** — Learn language from massive text  
● L11_3:33

**Prebuilt models** — Microsoft-trained document type models  
● L08:109

**Prebuilt models (DI)** — Invoice, receipt, ID domain extractors  
◐ L30:98

**Prebuilt vs Custom vs Fine-tuned** — Escalating customization decision ladder  
◐ L07:492

**Precision** — Share of flagged positives that are correct  
◐ L01:326

**Presidio** — Open-source PII/PHI detection and redaction  
◐ VitalCare:680

**Pricing tiers** — F0, S0, commitment tier options  
◐ L02:199

**PRIMM** — Predict, Run, Investigate, Modify, Make  
◐ PyTrack/Part1:3

**Prior authorization** — X12 278 request with AI criteria matching  
● VitalCare:224

**Prior Authorization agent** — End-to-end medication approval workflow  
● L27:119

**Priority-based memory** — What to keep when trimming history  
◐ L16:1272

**Private DNS Zone** — Resolves service name to private IP  
◐ L02:346

**Private Endpoint** — Private VNet IP for a PaaS service  
● L02:345

**Private Endpoint vs Service Endpoint** — Private IP vs locked-down public endpoint  
◐ IB/02_Azure:44

**Private Endpoints** — Private network access to AI services  
◐ L18:467

**PrivateLink** — Private service access without public egress  
● L33:484

**Proactive messages** — Bot-initiated outbound notification  
● L10:501

**Process.sequential** — Tasks run in fixed chained order  
● P6/02-crewAI/01_concepts:40

**Prompt caching** — Cached static prefix at 50% cost  
● L16:1577 · ◐ IB/05_SolArch:306 · ○ L15:742

**Prompt chaining** — Output of one prompt feeds next  
● L15:408 · ● L27:619

**Prompt compression** — Remove unneeded prompt tokens  
● L16:1356 · ● L27:680

**Prompt Flow** — Visual drag-drop RAG pipeline builder  
● L17:250 · ◐ L16:763

**Prompt Flow REST endpoint deployment** — One-click scored endpoint from flow  
● L17:354

**Prompt Flow vs Semantic Kernel** — Prototype visual vs production code  
● L17:326

**Prompt injection** — Malicious instructions hijack model  
● L11_4:249 · ● L24:217 · ◐ L18:220

**Prompt injection defense** — Framing, validation, separation, Content Safety  
● L15:515

**Prompt injection in agents** — Injection-to-action tool abuse chains  
● IB/06_RAI:73

**Prompt injection layered defense** — Input, prompt, privilege, output, monitoring  
● IB/06_RAI:61

**Prompt logging risk** — Full-text tracing is a data-classification decision  
● L36:213

**Prompt Shields** — Direct/indirect injection detector  
◐ L11_4:470 · ○ L18:505

**Prompt versioning** — Prompts as reviewed, pinned artifacts  
● IB/06_RAI:162 · ● L19:530 · ● P6/02-Dealer/FLOW_WITH_LOOPS:435

**Prompt versioning in Git** — Prompts as reviewed versioned artifacts  
● L31:241

**.prompty file format** — Prompt plus model config in one file  
◐ L19:731

**Prompting for code** — Point at a pattern, state constraints, define done  
● L35:149

**Protocol (structural typing)** — Interface without inheritance — test doubles  
● L32:595

**Provisioned Throughput (PTU)** — Reserved capacity deployment type  
○ L17:1054

**PTU (Provisioned Throughput Units)** — Reserved capacity, no throttling  
◐ L12:946

**PTU sizing** — Baseline PTU plus PAYG overflow  
◐ IB/05_SolArch:132

**PTU vs pay-as-you-go** — Reserved throughput vs shared dynamic pool  
● IB/02_Azure:80

**Pull indexer** — Scheduled Azure-managed ingestion  
◐ L09:140

**Pulumi** — Multi-cloud IaC in general-purpose languages  
◐ L33:377

**Puppet** — Agent-based pull-model config management  
◐ L33:391

**Push API** — Code-driven document upload to index  
● L09:139

**Push API vs indexer** — App-controlled push vs scheduled pull  
◐ IB/02_Azure:116

**Push vs Pull comparison** — Capability tradeoffs of ingestion modes  
● L09:988

**Push vs pull deployment** — CI holds credentials versus cluster pulls from Git  
● L34:208

**Pydantic** — Validating LLM output and tool arguments  
● L32:158

**Pydantic structured output** — Typed schema-constrained model response  
● L21:469


## Q

**Qdrant** — Open-source high-performance vector DB  
◐ L09:856 · ○ L13:531

**QLoRA** — 4-bit base plus fp16 adapters  
● L11_3:296 · ● L14:544 · ● P6/08-LoRA/01_concepts:70 · ● PyTrack/1.4-FineTuning:363

**Quality gate** — CI/CD threshold blocking bad deploys  
● L19:235 · ● L22:172

**Quality gate thresholds** — Groundedness .85, relevance .80, accuracy .90  
● P6/02-Dealer/WORKFLOW:263

**Quantization** — Lower-precision weights shrink the model  
● P6/01-Ollama/01_concepts:67

**Quantization levels (Q4_K_M)** — Size versus quality trade tiers  
◐ P6/01-Ollama/01_concepts:77

**Quantization memory math** — fp32/fp16/int8/nf4 GB estimates  
● PyTrack/1.4-FineTuning:494

**Query decomposition** — Split complex question into sub-questions  
● L13:1126

**Query rewriting (multi-turn)** — Make follow-up query standalone  
● L13:981

**Query rewriting / expansion** — Reformulate query before retrieval  
◐ IB/03_RAG:164

**Query/Key/Value (Q,K,V)** — Search, match, and deliver vectors  
● L11_1:67

**Question Answering** — FAQ knowledge base retrieval service  
● L03:321

**Quick vs Advanced training** — Fast versus longer accurate training  
◐ L04:221

**Quota increase request** — Raising TPS/TPM service limits  
◐ L07:208

**Quota partitioning (noisy neighbor)** — Gateway rate caps per internal consumer  
◐ IB/05_SolArch:96


## R

**r (LoRA rank)** — Adapter capacity, default 8  
● P6/08-LoRA/01_concepts:63

**RAG** — Retrieve, augment, generate pattern  
● L13:45

**RAG (Retrieval-Augmented Generation)** — Retrieve documents at query time  
● L23:12

**RAG chain (LangChain)** — RetrievalQA over AzureSearch store  
● L21:627

**RAG Engine (Vertex)** — Managed RAG orchestration service  
○ P6/09-Vertex/01_concepts:62

**RAG evaluation** — Retrieval and generation metrics separated  
● IB/03_RAG:152

**RAG evaluation metrics** — Faithfulness, relevance, precision, recall  
● L13:1198

**RAG pipeline (end-to-end)** — Chunk, embed, index, retrieve, generate  
● PyTrack/Part1:368

**RAG vs File vs Multi RAG** — Wizard blob processing mode choice  
● L09:974

**RAG vs fine-tuning** — Knowledge vs behavior decision tree  
◐ PyTrack/1.4-FineTuning:752

**RAGAS** — Faithfulness, relevance, precision, recall  
◐ PyTrack/Part1:1064

**RAGAS in CI/CD** — Golden-dataset quality gate on change  
● P6/03-RAGAS/02_architecture:62

**RAGAS score interpretation** — 0.90 strong, under 0.70 not shippable  
◐ P6/03-RAGAS/01_concepts:49

**Rank (r) in LoRA** — Adapter dimension, typically 8  
◐ L14:522

**RBAC roles (Foundry)** — Account Owner, Owner, User, Project Manager  
○ L17:1055

**RBAC roles for AI services** — User, Contributor, Reader roles  
◐ L02:296

**Re-ranking** — Two-stage retrieve then rescore  
● L13:741

**ReAct loop** — LLM alternates tool calls and reasoning  
● L27:280 · ● P6/02-Dealer/FLOW_WITH_LOOPS:181

**ReAct pattern** — Thought, Action, Observation interleaved  
● L16:450 · ● PyTrack/1.5-Agents:284 · ◐ L21:683

**ReAct prompting** — Reason before acting in prompts  
◐ L15:227

**Read API** — Async multi-page OCR text extraction  
● L04:378

**Real-time vs batch pipelines** — Event latency versus scheduled throughput  
● L20:429

**Realtime API** — Voice-to-voice streaming API  
○ L12:697

**Recall** — Share of actual positives captured  
◐ L01:327

**RecognizeCustomEntitiesAsync** — C# call to custom NER deployment  
◐ L07:478

**RecognizePiiEntitiesAsync** — C# PII detection and redaction call  
● L03:254

**Recursive character chunking** — Split on separator priority order  
● L13:308

**Red-teaming** — Adversarial pre-launch testing with gates  
● IB/06_RAI:85

**RedactedText** — PII-masked version of input text  
◐ L03:260

**Redis cache** — TTL-based answer cache layer  
◐ L18:431

**Refusal patterns** — Decline when retrieval below threshold  
◐ VitalCare:582

**Regression** — Supervised prediction of a number  
◐ L01:172

**Reinforcement learning** — Reward-driven trial-and-error learning  
◐ L01:217

**Relevance metric** — Does answer address the question  
◐ L17:417

**Reliability and Safety principle** — Predictable behavior and safe failure  
◐ L01:428

**Replicas** — Index copies for throughput and SLA  
◐ L09:124

**Response synthesizer** — Refine or tree-summarize node combination  
◐ P6/05-LlamaIndex/03_interview_qa:32

**Responsible AI dashboard** — Fairness, error, causal analysis views  
● L17:673

**Responsible AI principles** — Six Microsoft AI governance principles  
● L01:403 · ◐ L11_4:363

**Retention filtering** — Date-cutoff exclusion in sync code  
◐ L09:1028

**Retention policy** — Purge conversations and deleted docs  
◐ L20:479

**Retrieval confidence gate** — Minimum docs/similarity before generating  
◐ VitalCare:569

**Retrieval vs generation failure** — Diagnosing which RAG stage broke  
● IB/03_RAG:80

**retrieve_and_generate** — One-call grounded cited Bedrock answer  
● P6/06-Bedrock/01_concepts:78

**Retry policy** — Backoff plus jitter on transient failures  
● L31:49

**Retry-After header** — Server-specified retry delay  
◐ L07:190

**RetryPolicy (exponential backoff)** — 1s, 2s, 4s retries with jitter  
● P6/02-Dealer/FLOW_WITH_LOOPS:343

**Reward model** — Predicts human preference score  
● L11_4:67

**RLAIF** — RL from AI-generated feedback  
◐ L11_4:202

**RLHF** — Three-stage human-preference training  
● L11_3:223 · ● L11_4:53 · ● L23:424 · ◐ IB/01_Fund:89

**RLHF vs DPO** — Two-stage RL vs direct preference loss  
● PyTrack/1.4-FineTuning:549

**RMSE** — Root mean squared error metric  
◐ L01:339

**Row-level security in AI Search** — Filter results by user identity  
● L18:272

**RPO / RTO** — Data-loss and restore-time requirements  
● IB/05_SolArch:182

**RPO / RTO per data class** — Clinical PHI 1min/15min, audit zero RPO  
● VitalCare:865

**RRF (Reciprocal Rank Fusion)** — Rank-position merge of result lists  
● L09:399 · ◐ L11_2:414

**RRF fusion** — Reciprocal rank score merge of two lists  
● P6/02-Dealer/WORKFLOW:143

**RSM escalation** — Regional Sales Manager human review  
◐ P6/02-Dealer/WORKFLOW:176

**Rule-based systems** — Explicit programmer-written logic  
◐ L01:44

**RxNorm normalization** — Map brand drugs to standard codes  
◐ L30:139


## S

**S3 bucket source** — Bedrock KB document source  
◐ P6/06-Bedrock/02_architecture:47

**SAS URL** — Time-limited blob access token URL  
◐ L08:449

**Saved plan file** — Apply what a human approved, not a fresh plan  
● L33:235

**Scalar quantization** — 4x vector compression, ~1% loss  
◐ L09:733

**Scaling laws / Chinchilla** — Params, data, compute must scale together  
◐ IB/01_Fund:138

**Schema validation (A2A)** — Bus checks version and required fields  
◐ L29:76

**score.py** — init/run inference entry script  
● L06:602

**Scoring profiles** — Field and recency relevance boosts  
◐ L09:610

**Section-aware chunking** — Use headings as chunk boundaries  
◐ L13:424

**Security Context Constraints** — Why many public images fail on OpenShift  
◐ L34:362

**Self-attention** — Tokens attend within same sequence  
● L11_1:87 · ● L23:327

**Self-consistency** — Run multiple times, take majority  
◐ L15:385 · ◐ PyTrack/Part1:215

**Self-healing agent** — Detects degradation and responds automatically  
● L31:105

**Self-querying retrieval** — LLM generates structured filter query  
● L13:768

**selfHeal and prune** — Revert manual edits, delete what Git removed  
● L34:225

**Semantic cache threshold** — 0.90 serves the wrong answer on negation  
● L36:295

**Semantic caching** — Embedding-similarity keyed response cache  
● IB/05_SolArch:318 · ● L18:376 · ○ L13:1324

**Semantic caching risk** — Wrong-but-similar answer served  
◐ HLP01:246

**Semantic chunking** — Split where sentence similarity drops  
● L13:362

**Semantic configuration** — Prioritized title/content/keyword fields  
◐ L09:539

**Semantic functions (SK)** — Prompt templates as callable functions  
◐ L16:216

**Semantic Kernel** — Microsoft C# orchestration SDK  
● L16:94 · ● L25:34

**Semantic Kernel planners** — Upfront plan over plugin functions  
◐ IB/04_Agents:34

**Semantic ranker** — LLM re-ranking of top BM25 results  
● L09:519

**Semantic ranker (Azure)** — Managed cross-encoder re-ranker  
◐ L13:763

**Semantic ranker vs custom reranker** — Managed default vs fine-tuned cross-encoder  
◐ IB/02_Azure:188

**Semantic re-ranker** — Cross-encoder re-scores retrieved chunks  
◐ P6/02-Dealer/WORKFLOW:147

**Semantic Segmentation** — Per-pixel category labeling  
◐ L04:48

**Semantic vs episodic memory** — Knowledge versus experience  
● HLP01:100

**Sender whitelist** — Specialist accepts known SenderIds only  
◐ L29:99

**sentence-transformers** — Embedding models for retrieval  
● P6/04-HuggingFace/01_concepts:28

**SentencePiece** — Raw-stream tokenizer marking word starts  
● L11_2:95

**Sentiment Analysis** — Polarity scoring of text  
● L03:181

**Serverless API deployment** — Pay-per-token hosted model endpoint  
● L17:178

**Service accounts (GCP)** — IAM identity, Managed Identity analog  
◐ P6/09-Vertex/01_concepts:95

**Service Endpoint vs Private Endpoint** — Public IP routing versus private IP  
◐ L02:347

**Service mesh** — Sidecar proxy: mTLS, retries, traffic splitting  
● L34:327

**Severity levels (Content Safety)** — 0-6 harm scoring thresholds  
◐ L11_4:320

**Shared state object** — Typed fields with write ownership  
◐ HLP01:85

**Shift-left policy** — Block before deploy versus detect at runtime  
● L33:523

**Short-term vs long-term memory** — Session truncation vs persisted retrieval  
● IB/04_Agents:158

**Showback vs chargeback** — Reporting spend versus billing it  
● L36:358

**Silent failure prohibition** — Every outcome must be actionable  
◐ L28:145

**Similarity metric** — Cosine closeness to reference answer  
◐ L17:435

**SimpleDirectoryReader** — Loads a folder into Documents  
● P6/05-LlamaIndex/01_concepts:26

**Singleton as module** — Imports are cached — no locking needed  
◐ L32:584

**Six Rs of migration** — Rehost, replatform, refactor, repurchase, retire, retain  
● L33:439

**Skillset** — AI enrichment pipeline during indexing  
● L09:227

**Sliding window memory** — Keep last N conversation turns  
● L16:1086 · ◐ HLP01:51

**SMART-on-FHIR** — Clinician SSO launch spec for EHR apps  
◐ VitalCare:382

**source_nodes** — Built-in citations on response  
● P6/05-LlamaIndex/02_architecture:50

**Spark NLP** — JSL distributed NLP library  
◐ L30:130

**Speaker Identification** — One-to-many voice matching  
◐ L05:424

**Speaker Verification** — One-to-one voice identity check  
◐ L05:397

**Specialist agent** — Single-domain agent returning typed result  
◐ L28:50

**Specification gaming** — Optimizes metric not intent  
◐ L11_4:177

**Speech Translation** — Spoken audio to foreign-language text  
● L05:348

**Speech-to-Text (STT)** — Audio converted to transcript  
● L05:126

**Split skill** — Chunks long text for embedding  
◐ L09:251

**Split-brain risk** — Divergent writes during network partition  
● IB/05_SolArch:194

**srch-jma-dev-indexer** — JMA dev search index, no vectors  
◐ L08:774

**srch-jma-prod-indexer** — JMA production Azure AI Search resource  
○ L09:873

**srch-jma-stg-indexer** — JMA staging search, zero indexers  
◐ L09:1106

**SSML** — XML markup controlling speech synthesis  
● L05:280

**Stale document cleanup** — Deletes index docs missing from source  
◐ L09:1031

**State (TypedDict)** — Typed object flowing through graph  
◐ L25:80

**State locking** — DynamoDB table or blob lease prevents corruption  
● L33:63

**State management (bots)** — User, conversation, private state scopes  
● L10:124

**State ownership (IaC)** — The one real delta between Bicep and Terraform  
● L33:32

**StateGraph** — LangGraph node-and-edge definition object  
◐ L25:79

**Stemming** — Crude suffix removal to word root  
◐ L03:96

**Step-back prompting** — Retrieve general context first  
◐ L13:1157

**Stepwise Planner** — ReAct planner capped at 10 iterations  
◐ L27:516

**Stop words** — Low-meaning words removed before features  
◐ L03:93

**Strategy pattern (Python)** — Just pass a function — no interface needed  
● L32:556

**Streaming (perceived latency)** — Reduces time-to-first-token, not total  
◐ HLP01:253

**Streaming (Python)** — stream=True chunk delta iteration  
● L21:376

**Streaming in Semantic Kernel** — IAsyncEnumerable token streaming  
● L16:1766

**Streaming LLM output** — Yield each token chunk as it arrives  
● L32:239

**Streaming responses** — Tokens sent as generated  
◐ L12:210 · ◐ L18:174

**Streaming via IAsyncEnumerable** — Emit tokens as generated for perceived speed  
● L27:699

**Structured Outputs** — JSON schema guaranteed responses  
◐ L12:696

**Structured/tabular data in RAG** — Preserve tables as atomic units  
◐ IB/03_RAG:128

**Summarization / compaction** — Compress old turns, keep recent verbatim  
● IB/04_Agents:182

**Summarization compaction trap** — Errors compound summarizing summaries  
◐ HLP01:52

**Supervised fine-tuning** — JSONL upload, train, evaluate, deploy  
● L22:275

**Supervised Fine-Tuning (SFT)** — Continued training on instruction-response pairs  
● PyTrack/1.4-FineTuning:8

**Supervised learning** — Learning from labeled examples  
● L01:150

**Supervisor / orchestrator pattern** — Delegates to specialists, synthesizes result  
● IB/04_Agents:96

**Supervisor agent** — Decomposes, delegates, synthesizes results  
● L28:43

**SupervisorAgent** — Delegates to specialists, synthesizes verdict  
● P6/02-Dealer/WORKFLOW:185

**Supply chain security** — cosign signing, SBOM, model provenance  
◐ VitalCare:1042

**Sycophancy** — Model agrees to please user  
◐ L11_4:178

**System prompt 5-component design** — Identity, scope, rules, format, fallback  
● P6/02-Dealer/FLOW_WITH_LOOPS:163

**System prompt compression** — Shorten prompt used every call  
◐ L16:1547

**System prompt design** — Persona, scope, constraints, format, fallback  
● L15:310 · ● L27:561

**System prompt hardening** — Instructions resist override attempts  
◐ L24:244

**System prompt shortening** — Fewer tokens on every call  
◐ L18:389


## T

**Tags (image)** — Labels with confidence scores  
◐ L04:125

**target_modules** — Layers receiving adapters, q_proj/v_proj  
● P6/08-LoRA/01_concepts:65

**Task.WhenAll parallel dispatch** — C# concurrent specialist invocation  
◐ L28:104

**Teams channel** — Bot Service Teams connectivity resource  
◐ L10:475

**Telemetry custom metrics** — TrackMetric for tokens and latency  
● L19:369

**Temperature** — Randomness control 0 to 2  
● L12:166

**Temperature / top-p / top-k** — Sampling controls over token distribution  
● IB/01_Fund:121

**Temperature by use case** — 0 extraction, 0.7 drafting  
● L15:626

**Terraform backend** — Remote state in S3, Blob or GCS  
● L33:63

**Terraform data source** — Read a resource you do not manage  
● L33:180

**terraform import** — Adopt an existing resource into state  
● L33:463

**Terraform module** — Reusable parameterised bundle  
● L33:252

**Terraform plan symbols** — Plus, minus, tilde, and destroy-then-recreate  
● L33:209

**Terraform provider** — Plug-in adapting Terraform to one cloud API  
● L33:286

**Terraform Registry** — Largest public IaC module library  
◐ L33:252

**Terratest** — Go integration tests that really deploy  
◐ L33:550

**Test set** — Held-out final evaluation data  
◐ L01:283

**Text normalization** — Expands dates and numbers for TTS  
◐ L05:50

**Text Summarization** — Extractive or abstractive condensation  
◐ L03:276

**text-embedding-3-large** — 3072-dim high accuracy embeddings  
◐ L12:58

**text-embedding-3-small** — Cost-efficient embedding model  
◐ L12:59

**Text-to-Speech (TTS)** — Text converted to synthesized audio  
● L05:254

**tfstate plaintext secrets** — Resource attributes stored unencrypted in state  
● L33:92

**Threat model for AI** — Five AI-specific attack classes  
● L18:217

**Three-layer AI dashboard** — Infrastructure, AI service, quality  
● L36:423

**Three-layer intent routing** — CLU then QA then LLM fallback  
● L10:407

**Three-layer meta-agent architecture** — Supervisor, specialists, capabilities  
● L28:33

**Throttling (429) handling** — Backoff; instances don't add quota  
● IB/02_Azure:68

**Thundering herd** — Synchronized retries re-crash service  
◐ L31:67

**Time series forecasting** — Predicting future values over time  
◐ L06:261

**Time to first token (TTFT)** — Latency until first response token  
◐ PyTrack/Part1:1209

**Titan embeddings** — Amazon's own text and embedding models  
◐ P6/06-Bedrock/01_concepts:31

**Token budget formula** — system + memory + context + query + output  
◐ HLP01:178

**Token cost levers (ranked)** — Model choice ~17x beats prompt trimming  
● HLP01:186

**Token counting / tiktoken** — Estimate tokens before request  
◐ L16:1241

**Token limits** — Practical context budget constraint  
● L11_2:151

**Token optimization** — Cost control via prompt and model choices  
● L27:664

**Token usage tracking** — Log input/output tokens per request  
● L18:400

**Tokenization** — Subword units driving cost and context  
● IB/01_Fund:49 · ● L11_2:30 · ◐ L03:82

**Tokenization with tiktoken** — Encode text, inspect token IDs and count  
● PyTrack/Part1:116

**Tokenizer facts** — ~4 chars/token; IDs are model-specific  
◐ HLP01:201

**Tokenizer-model pairing** — Mismatch produces garbage output  
● P6/04-HuggingFace/01_concepts:71

**Tool argument validation** — Schema plus existence checks pre-execution  
● IB/04_Agents:46

**Tool call fabrication** — Agent claims uncalled tool result  
◐ L24:96

**Tool call verification** — Compare claimed vs actual Run tool_calls  
◐ L24:112

**Tool description standard** — What, when, returns, scope boundary  
● L26:163

**Tool discovery** — Agents query hub for available tools  
◐ L26:56

**Tool granularity** — Single-responsibility tools beat mega-tools  
◐ IB/04_Agents:70

**Tool naming standard** — domain_action_resource convention  
◐ L26:152

**Tool schema** — Machine-readable capability declaration  
◐ L26:27

**Tool vs Knowledge vs Fine-tune** — Decision tree by data volatility  
● L16:1644 · ● L17:921

**Tool-call failure handling** — Retry, circuit breaker, honest escalation  
◐ IB/04_Agents:58

**Top-K retrieval** — Return K most similar chunks  
◐ L13:594

**top-K tuning** — Fewer chunks with re-ranking  
● L18:370

**top_p (nucleus sampling)** — Alternative sampling to temperature  
○ L12:167

**TPM (tokens per minute)** — Azure OpenAI throughput quota  
◐ L07:185

**TPM quota** — Tokens-per-minute deployment rate limit  
● L12:101

**TPM quota (Tokens Per Minute)** — Per-deployment throughput limit, 429s  
● L18:142

**TPM quota as bottleneck** — Provider quota, not compute, limits scale  
● HLP01:236

**TPS (transactions per second)** — AI service call rate limit  
◐ L07:184

**Trace context propagation** — Carry trace IDs across agent messages  
● L36:149

**Trace, span, attribute** — OTel vocabulary for one agent request  
● L36:60

**Tracing (Foundry)** — Visual trace of calls and retrievals  
◐ L17:806

**Traffic splitting** — Percentage routing across deployments  
● L06:723

**Training data split** — Train, validation, test partitions  
◐ L01:269

**Transfer learning** — Adapt general model to task  
◐ L11_3:395

**Transformer architecture** — Attention-based layered model  
● L11_1:145

**Transformer layer stacking** — Depth yields higher-level reasoning  
◐ L11_1:192

**Transparency principle** — Explaining how AI decided  
◐ L01:486

**Try/except/finally** — Python exception handling blocks  
◐ L21:251

**Turn** — One bot request-response cycle  
◐ L10:66

**Type hints** — Optional annotations on Python variables  
◐ L21:62

**Type hints (Python)** — Annotations for tooling, ignored at runtime  
● L32:43

**Typed message contracts** — Structured request/response between agents  
◐ L28:163


## U

**Underfitting** — Model too simple to capture pattern  
◐ L01:297

**Unsupervised learning** — Structure discovery without labels  
● L01:188

**UserProxyAgent** — AutoGen human proxy, executes code  
◐ L25:191

**Utterances** — Example phrases training an intent  
◐ L03:473


## V

**values.yaml** — Parameter defaults, overridden per environment  
● L34:56

**Vector database** — Stores and searches embeddings  
◐ L13:498

**Vector DB comparison** — FAISS, Chroma, Qdrant, Pinecone, AI Search  
◐ PyTrack/Part1:408

**Vector field** — Collection(Edm.Single) embedding column  
◐ L09:101

**Vector memory (SK)** — Long-term facts in AI Search  
◐ L16:311

**Vector query** — Nearest-neighbor embedding search  
● L09:365

**Vector search failure modes** — Recall collapse, drift, staleness, noise, cold-start  
● VitalCare:1255

**Vector store choice (pgvector/Qdrant)** — PHI-safe self-hosted vs managed tradeoff  
● VitalCare:1334

**Vector store integrations** — Qdrant, Weaviate, pgvector, AI Search  
◐ P6/05-LlamaIndex/02_architecture:55

**Vector-backed long-term memory** — Embedded past interactions retrieved per session  
● IB/04_Agents:170

**Vendor lock-in exit strategy** — Per-component portability abstraction  
◐ VitalCare:1188

**Versioning policy (MCP)** — Semantic versions with parallel running  
◐ L26:189

**Vertex AI Search** — Fully managed GCP RAG service  
● P6/09-Vertex/01_concepts:59

**Vertex AI Vector Search** — Managed ANN vector DB, formerly Matching Engine  
◐ P6/09-Vertex/01_concepts:60

**Virtual environment (venv)** — Project-scoped Python dependency folder  
◐ L21:32

**Virtual keys and budgets** — Per-team spend caps and attribution  
◐ L36:273

**vLLM** — Self-hosted GPU inference server  
◐ VitalCare:400

**VMware vSphere provider** — On-prem virtualisation as code  
○ L33:425

**VNet integration** — Service endpoints plus IP firewall rules  
◐ L07:115

**Voice profile enrollment** — Recording samples to create voiceprint  
◐ L05:440

**Voice styles** — cheerful, customerservice, newscast expressions  
◐ L05:271

**VotingEnsemble** — Combined multi-model prediction average  
◐ L06:245

**VPC** — AWS private network — Azure VNet equivalent  
● L33:484

**VS Code Foundry Toolkit** — Extension to browse and deploy models  
○ L17:1098


**Vulnerability remediation pipeline** — Scan, triage agent, auto-PR or human gate  
● L35:226

## W

**Waterfall Dialogs** — Sequenced multi-step conversation flow  
● L10:273

**Weaviate** — Multi-modal open-source vector DB  
○ L13:533

**What not to delegate to AI** — Auth, IAM, money, deletion, regex on prod data  
● L35:167

**When not to use a service mesh** — Under ~10 services the operational cost wins  
● L34:350

**Whisper** — Speech-to-text transcription model  
○ L12:62

**Word embeddings (Word2Vec/GloVe)** — Fixed context-independent word vectors  
◐ L11_2:285

**Word Error Rate (WER)** — Percent of words mis-transcribed  
● L05:338

**WordPiece** — Likelihood-based merges, ## continuation  
● L11_2:125


## X

**X12 EDI (837/835/278)** — Claims and prior-auth transaction formats  
◐ VitalCare:332


## Y

**yield** — Turns a function into a generator, pausing execution  
● L32:182

**yield from** — Delegate iteration to another generator  
◐ L32:256


## Z

**Zero-shot classification** — Label at inference via NLI entailment  
● P6/04-HuggingFace/01_concepts:46

**Zero-shot entity recognition** — Custom NER without labeled data  
○ L07:599

**Zero-shot prompting** — Ask directly without examples  
● L15:113

**Zero-shot vs few-shot vs fine-tuning** — When examples beat weight updates  
● IB/01_Fund:148

**Zero-trust networking** — mTLS, NetworkPolicy, egress allowlists  
◐ VitalCare:1000

**Zone 1 / Zone 2 / Zone 3** — Code vs Semantic Kernel vs LLM layers  
● P6/02-Dealer/FLOW_WITH_LOOPS:9

**Zone architecture (MCP)** — PHI, clinical, operational, platform zones  
◐ L26:350


## #

**429 throttling** — Too Many Requests rate-limit error  
● L20:132

**429 throttling handling** — Rate limit errors and retries  
● L12:855

**429 Too Many Requests** — Throttling HTTP status code  
● L07:189

**8kHz telephony audio** — Phone audio lower-accuracy acoustic model  
◐ L05:87
