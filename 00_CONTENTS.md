# AIML-Learn — Detailed Contents

**Generated:** 2026-07-19 · **2,319 topics across 125 files**

> Every heading in every teaching file, to sub-sub-module depth. Numbers are **line numbers** —
> open the file and jump straight there (`Ctrl+G` in VS Code).
> For "where is topic X?" use the alphabetical `00_INDEX.md` instead.

**Jump:** [Part 1](#part-1-foundations) · [Part 2](#part-2-azure-ai-services) · [Part 3](#part-3-genai-llms) · [Part 4](#part-4-architecture-operations) · [Part 5](#part-5-agentic-protocols-patterns) · [Part 6](#part-6-applied-projects) · [Part 7](#part-7-platform-engineering--ai-assisted-delivery) · [Questions & Prep](#questions-prep) · [Supplementary](#supplementary) · [Assessments](#assessments)

---


## Part 1 — Foundations


### Module 1 — Introduction to AI
`01_Lessons/Part1_Foundations/L01_Introduction_to_AI.md` · 637 lines · 40 topics

```
    6  ▸ Why This Module Matters
   24  ▸ Topic 1.1 — What Is Artificial Intelligence?
   28    · 1. The Simple Definition
   42    · 2. AI vs Traditional Software
   68    · 3. Types of AI
   80    · 4. How AI Relates to Machine Learning and Deep Learning
  112    · 5. Rule-Based vs Machine Learning vs Deep Learning
  124  ▸ Topic 1.2 — Understanding Machine Learning
  128    · 1. What Machine Learning Actually Does
  146    · 2. The Three Types of Machine Learning
  150      - Supervised Learning — Learning With Answers
  188      - Unsupervised Learning — Learning Without Answers
  217      - Reinforcement Learning — Learning By Trial and Error
  235    · 3. The ML Workflow — How a Model Gets Built
  269    · 4. Training Data vs Validation Data vs Test Data
  294    · 5. Overfitting and Underfitting
  319    · 6. Key ML Metrics
  344    · 7. Features and Labels
  372  ▸ Topic 1.3 — AI Workloads and Considerations
  378    · Azure AI Workload Map
  393  ▸ Topic 1.4 — Responsible AI Principles
  397    · 1. Why Responsible AI Matters for an Architect
  403    · 2. Microsoft's Six Responsible AI Principles
  407      - Fairness
  427      - Reliability and Safety
  447      - Privacy and Security
  467      - Inclusiveness
  486      - Transparency
  505      - Accountability
  525    · 3. Responsible AI in Azure — Tools Available
  538  ▸ Topic R1 — Recall: Module 1 Review & Quiz
  578  ▸ Memory Hooks
  591  ▸ 2026 Updates — What's Changed Since This Module Was Written
  603  ▸ Interactive Learning Ideas
  605    · Exercise 1 — Draw the Hierarchy (5 min)
  608    · Exercise 2 — Classify JMA Scenarios (10 min)
  621    · Exercise 3 — Metric Decision (5 min)
  627    · Exercise 4 — Responsible AI Audit (10 min)
  630    · Exercise 5 — Azure Portal Check (5 min)
```

### Module 2 — Azure AI Services Overview
`01_Lessons/Part1_Foundations/L02_AzureAIServices_Overview.md` · 501 lines · 28 topics

```
    7  ▸ Why This Module Matters
   24  ▸ Topic 2.1 — Azure AI Platform Introduction
   28    · 1. The Azure AI Services Landscape
   61    · 2. Which Service for Which Job — Decision Table
   80    · 3. Multi-Service vs Single-Service Resources
  105    · 4. Azure AI Foundry — The Unified Portal (2026)
  129    · 5. Choosing the Right Service — Architect Decision Flow
  154  ▸ Topic 2.2 — Provisioning Azure AI Resources
  158    · 1. Creating Azure AI Services Resources
  176    · 2. Keys and Endpoints
  199    · 3. Pricing Tiers
  211    · 4. ARM Templates and Bicep for AI Resources
  242    · 5. Managing with Azure CLI
  271  ▸ Topic 2.3 — Security and Compliance
  275    · 1. Authentication Options
  321    · 2. Network Security — VNets and Private Endpoints
  351    · 3. Managed Identity for AI Services — The Full Pattern
  370    · 4. Data Privacy and Compliance
  388    · 5. Azure AI Content Safety — Overview
  416  ▸ Topic R2 — Recall: Module 2 Review & Quiz
  450  ▸ Memory Hooks
  462  ▸ Interactive Learning Ideas
  464    · Exercise 1 — Service Mapping (10 min)
  472    · Exercise 2 — Portal Walkthrough (15 min)
  479    · Exercise 3 — Bicep Deploy (20 min)
  488    · Exercise 4 — CLI Practice (10 min)
  491    · Exercise 5 — Security Audit Question
```

### Module 4 — Natural Language Processing
`01_Lessons/Part1_Foundations/L03_NLP_Fundamentals.md` · 734 lines · 47 topics

```
    7  ▸ Why This Module Matters
   25  ▸ Topic 4.1 — NLP Concepts
   29    · 1. What Is NLP?
   51    · 2. Classical NLP Pipeline (Pre-LLM)
   80    · 3. Key NLP Concepts
   82      - Tokenization (Classical vs LLM)
   93      - Stop Words
   96      - Stemming vs Lemmatization
  105      - Part-of-Speech (POS) Tagging
  109      - Named Entity Recognition (NER)
  123      - Sentiment Analysis
  142  ▸ Topic 4.2 — Azure AI Language Service
  146    · 1. What Azure AI Language Covers
  166    · 2. Calling the Language Service in C#
  181    · 3. Sentiment Analysis
  215    · 4. Key Phrase Extraction
  233    · 5. Named Entity Recognition
  249    · 6. PII Detection — Critical for JMA
  276    · 7. Text Summarization
  300    · 8. Batch Processing — Analyze Multiple Documents
  321  ▸ Topic 4.3 — Question Answering
  325    · 1. What Question Answering Is
  347    · 2. QnA Maker vs Question Answering
  360    · 3. Creating a Knowledge Base
  380    · 4. Calling Question Answering from C#
  407    · 5. When to Use QA vs Full RAG
  420  ▸ Topic 4.4 — Conversational Language Understanding (CLU)
  424    · 1. What CLU Does
  446    · 2. Intents and Entities
  466    · 3. CLU Workflow
  495    · 4. Calling CLU from C#
  540    · 5. CLU vs AI Agents (Modern Perspective)
  557  ▸ Topic 4.5 — Text Translation
  561    · 1. Azure AI Translator Service
  582    · 2. Translator Capabilities
  595    · 3. Custom Translator — When to Use
  613    · 4. C# Translation Call
  634  ▸ Topic R4 — Recall: Module 4 Review & Quiz
  668  ▸ Memory Hooks
  681  ▸ 2026 Updates
  692  ▸ Interactive Learning Ideas
  694    · Exercise 1 — Language Studio Exploration (15 min)
  702    · Exercise 2 — PII Guard in C# (20 min)
  709    · Exercise 3 — CLU vs Agent Decision (10 min)
  716    · Exercise 4 — Build a Question Answering KB (20 min)
  723    · Exercise 5 — Connect to What You Know
```

### Module 3 — Computer Vision Fundamentals
`01_Lessons/Part1_Foundations/L04_ComputerVision.md` · 543 lines · 36 topics

```
    7  ▸ Why This Module Matters
   25  ▸ Topic 3.1 — Computer Vision Concepts
   29    · 1. What Is Computer Vision?
   42    · 2. Core CV Tasks
   54    · 3. Classification vs Detection vs Segmentation
   85    · 4. How CV Models Learn
  102  ▸ Topic 3.2 — Azure AI Vision Service
  106    · 1. What Azure AI Vision Does
  119    · 2. Key Capabilities — Image Analysis 4.0
  134    · 3. Calling Vision API in C#
  170    · 4. What Image Analysis 4.0 Cannot Do
  181  ▸ Topic 3.3 — Azure AI Custom Vision
  185    · 1. When to Use Custom Vision
  198    · 2. Two Project Types
  207    · 3. The Custom Vision Workflow
  240    · 4. Custom Vision API Call (after publishing)
  268    · 5. Exporting to ONNX for Edge Deployment
  281  ▸ Topic 3.4 — Azure AI Face Service
  285    · 1. What Face Service Does
  298    · 2. Limited Access Policy — Important
  318    · 3. Responsible AI for Face Service
  335  ▸ Topic 3.5 — Reading Text with OCR
  339    · 1. OCR vs Read API — Key Distinction
  355    · 2. OCR vs Document Intelligence — Critical Distinction for JMA
  378    · 3. Read API in C# (Part of Image Analysis 4.0)
  404    · 4. Multi-Language and Handwriting
  415    · 5. OCR Integration Pattern — Connecting to the JMA Pipeline
  444  ▸ Topic R3 — Recall: Module 3 Review & Quiz
  484  ▸ Memory Hooks
  496  ▸ Interactive Learning Ideas
  498    · Exercise 1 — Vision Studio Hands-On (15 min)
  505    · Exercise 2 — OCR vs Document Intelligence Comparison (15 min)
  512    · Exercise 3 — Custom Vision Project (30 min)
  520    · Exercise 4 — Architect Decision Quiz
  532    · Exercise 5 — Connect to JMA Pipeline (10 min)
```

### Module 5 — Speech Services
`01_Lessons/Part1_Foundations/L05_SpeechServices.md` · 601 lines · 32 topics

```
    7  ▸ Why This Module Matters
   25  ▸ Topic 5.1 — Speech Concepts
   29    · 1. The Speech Pipeline
   65    · 2. Key Speech Tasks
   76    · 3. Audio Format Requirements
   94  ▸ Topic 5.2 — Azure AI Speech Service
   98    · 1. Service Capabilities Overview
  126    · 2. Speech-to-Text — Real-Time Recognition
  158    · 3. Speech-to-Text — From Audio File (Call Recording)
  188    · 4. Batch Transcription — Scale for Call Centers
  235    · 5. Diarization — Speaker Separation
  254    · 6. Text-to-Speech — Neural Voices
  280    · 7. SSML — Fine Control Over Speech
  308    · 8. Custom Speech — Domain Vocabulary
  344  ▸ Topic 5.3 — Speech Translation & Speaker Recognition
  348    · 1. Real-Time Speech Translation
  382    · 2. Multi-Language Translation in One Call
  395    · 3. Speaker Recognition
  397      - Speaker Verification (1:1)
  424      - Speaker Identification (1:many)
  440    · 4. Voice Profiles and Enrollment
  457    · 5. Full JMA Call Center Pipeline
  507  ▸ Topic R5 — Recall: Module 5 Review & Quiz
  541  ▸ Memory Hooks
  554  ▸ 2026 Updates
  566  ▸ Interactive Learning Ideas
  568    · Exercise 1 — Speech Studio Exploration (15 min)
  575    · Exercise 2 — Transcribe a JMA Call Scenario (20 min)
  581    · Exercise 3 — SSML Practice (15 min)
  587    · Exercise 4 — JMA Call Center Architecture (15 min)
  590    · Exercise 5 — Custom Speech Planning
```

### Module 6 — Azure Machine Learning
`01_Lessons/Part1_Foundations/L06_AzureML.md` · 940 lines · 45 topics

```
    6  ▸ What You Already Know (Recap)
   26  ▸ Topic 6.1 — Azure ML Workspace
   30    · 1. What Is Azure ML Workspace?
   57    · 2. Core Workspace Components
   59      - Datastores and Data Assets
   82      - Compute
  101      - Environments
  122      - Experiments and Jobs
  144      - Model Registry
  163    · 3. Creating a Workspace — Azure CLI
  184    · 4. Azure ML Studio — The Web UI
  201  ▸ Topic 6.2 — Automated ML (AutoML)
  205    · 1. What Is AutoML?
  230    · 2. What AutoML Automates
  255    · 3. AutoML Task Types
  268    · 4. Running AutoML — Azure ML Studio
  306    · 5. AutoML via Python SDK
  349    · 6. AutoML Metrics — Choosing the Right One
  373    · 7. AutoML Model Explanations
  391    · 8. When to Use AutoML vs Custom Training
  404  ▸ Topic 6.3 — Azure ML Designer
  408    · 1. What Is Azure ML Designer?
  435    · 2. Key Designer Components
  449    · 3. Designer Pipeline — JM Family Example
  494    · 4. Designer vs AutoML vs Custom Code
  508    · 5. Publishing Designer Pipelines as Endpoints
  528  ▸ Topic 6.4 — Model Deployment
  532    · 1. The Deployment Problem
  552    · 2. Two Types of Endpoints
  554      - Managed Online Endpoints — Real-Time Inference
  628      - Batch Endpoints — Batch Inference
  664    · 3. Calling a Deployed Endpoint from C#
  714    · 4. Blue-Green Deployment — Safe Model Updates
  743    · 5. Model Monitoring After Deployment
  786  ▸ Topic R6 — Recall: Module 6 & Part 1 Comprehensive
  826  ▸ Production Architecture — JM Family Azure ML Full Picture
  869  ▸ Memory Hooks
  886  ▸ 2026 Updates
  898  ▸ Interactive Learning Ideas
  900    · Exercise 1 — Azure ML Studio Walkthrough (15 min)
  906    · Exercise 2 — AutoML Run (30 min)
  915    · Exercise 3 — Score.py Pattern
  921    · Exercise 4 — Blue-Green Deployment Drill
  929    · Exercise 5 — Drift Detection Design
```

## Part 2 — Azure AI Services


### Module 7 — Azure AI Services Deep Dive
`01_Lessons/Part2_AzureAIServices/L07_AzureAIServices_DeepDive.md` · 648 lines · 35 topics

```
    6  ▸ What You Already Know (Recap)
   25  ▸ Topic 7.1 — Advanced AI Services Management
   29    · 1. Azure AI Services: Single Resource vs Multi-Service vs Individual
   54    · 2. Authentication: API Keys vs Managed Identity
   58      - Option A: API Keys (Subscription Keys)
   70      - Option B: Managed Identity (Recommended for production)
   94    · 3. RBAC: The Three Roles You Need to Know
  109    · 4. Networking: Virtual Networks and Private Endpoints
  115      - Option A: Virtual Network Service Endpoints + IP Firewall rules
  130      - Option B: Private Endpoint (recommended for enterprise)
  146    · 5. Monitoring: Azure Monitor + Diagnostic Logs
  181    · 6. Throttling: TPM/RPM Limits and Retry Logic
  214  ▸ Topic 7.2 — Containers for AI Services
  218    · 1. Why Run Azure AI Services in Containers?
  234    · 2. How Container Images Work
  263    · 3. Which Services Support Containers?
  281    · 4. Kubernetes Deployment Pattern
  322    · 5. When to Choose Cloud vs Container
  346  ▸ Topic 7.3 — Custom Models and Training
  350    · 1. Why Custom Models?
  368    · 2. Azure AI Custom Vision
  412    · 3. Azure AI Custom Speech
  442    · 4. Azure AI Language — Custom NER and Text Classification
  492    · 5. Choosing Between Prebuilt vs Custom vs Fine-tuned
  512  ▸ Module 7 — Architecture Summary
  543  ▸ Recall — Module 7 Self-Test Questions
  580  ▸ Memory Hooks
  592  ▸ 2026 Updates
  604  ▸ Interactive Learning Ideas
  606    · Exercise 1 — Security Audit of JMA AI Resources (20 min)
  614    · Exercise 2 — Container Run (20 min)
  626    · Exercise 3 — Throttle Simulation
  629    · Exercise 4 — Custom NER Project
  637    · Exercise 5 — Prebuilt → Custom Decision
```

### Module 8 — Document Intelligence
`01_Lessons/Part2_AzureAIServices/L08_DocumentIntelligence.md` · 813 lines · 43 topics

```
    6  ▸ What You Already Know (Recap)
   26  ▸ Topic 8.1 — Document Intelligence Overview
   30    · 1. What Is Azure AI Document Intelligence?
   45    · 2. How It Works Internally
   80    · 3. Resource Tiers
   91    · 4. Supported Input Formats
  105  ▸ Topic 8.2 — Prebuilt Models
  109    · 1. What Are Prebuilt Models?
  117    · 2. Full Prebuilt Model List
  143    · 3. Calling a Prebuilt Model — C# Example
  181    · 4. Understanding the Response JSON
  232    · 5. JM Family Gap: What Prebuilt Invoice Doesn't Give You
  244  ▸ Topic 8.3 — Custom Models
  248    · 1. When to Use Custom Models
  260    · 2. Custom Model Types
  278    · 3. Training a Custom Model — Step by Step
  315    · 4. Labeling Tips
  325    · 5. Custom Model — C# Call
  348    · 6. Composed Models
  378    · 7. Model Lifecycle — Versioning
  394  ▸ Topic 8.4 — Integration Patterns
  398    · 1. The Standard Pipeline Pattern (Your JM Family Architecture)
  418    · 2. Handling Low-Confidence Results
  446    · 3. Analyzing from Blob Storage (Managed Identity Pattern)
  465    · 4. Batch Processing Pattern
  490    · 5. Error Handling Pattern
  527    · 6. Document Intelligence + Azure AI Search (Your RAG Pipeline)
  559  ▸ Module 8 — Architecture Summary
  591  ▸ Recall — Module 8 Self-Test Questions
  628  ▸ Memory Hooks
  641  ▸ 2026 Updates
  654  ▸ Interactive Learning Ideas
  656    · Exercise 1 — Prebuilt Model Comparison (20 min)
  663    · Exercise 2 — Markdown Output Test (15 min)
  666    · Exercise 3 — Confidence Routing Implementation (20 min)
  674    · Exercise 4 — JMA Pipeline Trace (15 min)
  681    · Exercise 5 — Custom Template vs Neural Decision
  695  ▸ Appendix — Merged from Legacy Notes
  699    · 1. Document Intelligence vs Azure AI Search — Reader vs Finder
  739    · 2. When NOT to Use Document Intelligence
  764    · 3. JMA Production — Current State and the RAG Gap
  803    · 4. Content Understanding — Document Intelligence Inside AI Foundry
```

### Module 9 — Azure AI Search
`01_Lessons/Part2_AzureAIServices/L09_AzureAISearch.md` · 1117 lines · 57 topics

```
    6  ▸ What You Already Know (Recap)
   26  ▸ Topic 9.1 — Azure AI Search Fundamentals
   30    · 1. What Is Azure AI Search?
   47    · 2. Core Components
   75    · 3. Index Schema — The Fields
  114    · 4. Service Tiers
  131  ▸ Topic 9.2 — Data Ingestion
  135    · 1. Three Ways to Get Data In
  147    · 2. Push API — C# Example
  178    · 3. Pull (Indexer) — Blob Storage Example
  212    · 4. Change Detection
  223  ▸ Topic 9.3 — AI Enrichment (Skillsets)
  227    · 1. What Is a Skillset?
  245    · 2. Built-in Skills (No Training Required)
  263    · 3. The Azure OpenAI Embedding Skill — Most Important for RAG
  293    · 4. Knowledge Store
  304    · 5. Integrated Vectorization (Newer Pattern)
  319  ▸ Topic 9.4 — Querying and Search Experience
  323    · 1. Three Query Types
  334    · 2. Full-Text Query (BM25)
  365    · 3. Vector Query
  397    · 4. Hybrid Query (Recommended for RAG)
  435    · 5. Captions and Answers
  457    · 6. Facets and Aggregations
  474  ▸ Topic 9.5 — Vector Search & Semantic Search
  478    · 1. Vector Search Configuration
  519    · 2. Semantic Ranker
  561    · 3. Embedding Models — Which to Use
  575    · 4. Full RAG Query Flow with Azure AI Search
  606    · 5. Index Management — Operations You'll Do
  635  ▸ Module 9 — Integration Pattern: JM Family Full Pipeline
  674  ▸ Module 9 — Self-Test Questions
  712  ▸ Memory Hooks
  727  ▸ 2026 Updates
  740  ▸ Interactive Learning Ideas
  742    · Exercise 1 — Portal Wizard vs Push API Comparison (20 min)
  749    · Exercise 2 — Query Type Comparison (15 min)
  756    · Exercise 3 — Vector Compression Test
  764    · Exercise 4 — JMA Index Audit (20 min)
  771    · Exercise 5 — Hybrid Search Implementation
  786  ▸ Interview Gap: Vector Database Comparison
  788    · The Interview Question
  796    · The Options
  812    · Side-by-Side Comparison
  833    · When to Use Each — Decision Guide
  869    · JMA Recommendation
  884    · The One-Line Interview Answer
  890  ▸ Appendix — Merged from Legacy Notes
  894    · 1. Indexer Schedule Options and the Polling Limit
  910    · 2. Near Real-Time Indexing — The Event Grid Pattern
  934    · 3. Import and Vectorize Data Wizard — What It Asks and What It Creates
  974    · 4. RAG vs File vs Multi RAG — The Blob Processing Choice
  988    · 5. Push vs Pull — Full Capability Comparison
 1014    · 6. JMA Production — `EnterpriseSearch.Sync` and the Five Reasons for Push
 1082    · 7. JMA Index Schema — Keyword-Only, No Vectors
 1103    · 8. Staging Environment — No Indexers (Confirmed)
```

### Module 10 — Bot Development
`01_Lessons/Part2_AzureAIServices/L10_BotDevelopment.md` · 637 lines · 30 topics

```
    7  ▸ Why This Module Matters
   22  ▸ Topic 10.1 — Bot Framework Fundamentals
   26    · 1. What Is the Bot Framework?
   53    · 2. Core Concepts: Activities and Turns
   82    · 3. Bot Architecture
  122    · 4. State Management
  161    · 5. Bot Framework Emulator
  177  ▸ Topic 10.2 — Building Bots with C#
  181    · 1. Creating a Bot Project
  201    · 2. Sending Rich Cards
  237    · 3. Adaptive Cards — The Modern Standard
  273    · 4. Waterfall Dialogs — Multi-Step Conversations
  326  ▸ Topic 10.3 — Integrating AI Services
  330    · 1. Adding CLU for Intent Recognition
  378    · 2. Adding Azure OpenAI for Open-Ended Questions
  407    · 3. CLU + QA + OpenAI — Three-Layer Intent Routing
  431  ▸ Topic 10.4 — Deploying Bots
  435    · 1. Azure Bot Service
  458    · 2. Deployment to Azure App Service
  487    · 3. Microsoft Teams Bot — Key Differences
  524    · 4. Bot Security
  544  ▸ Topic R10 — Recall: Module 10 Review & Quiz
  578  ▸ Memory Hooks
  591  ▸ 2026 Updates
  602  ▸ Interactive Learning Ideas
  604    · Exercise 1 — Echo Bot in 15 Minutes (15 min)
  611    · Exercise 2 — Add Adaptive Card Response (20 min)
  618    · Exercise 3 — Three-Layer Intent Router (30 min)
  625    · Exercise 4 — Deploy to Azure and Connect to Teams (30 min)
```

## Part 3 — GenAI & LLMs


### Module 11.1 — How LLMs Work: Attention & Transformer Architecture
`01_Lessons/Part3_GenAI_LLMs/L11_1_LLMs_Attention_Transformer.md` · 359 lines · 26 topics

```
    6  ▸ What You Already Know (Recap)
   20  ▸ 1. The Problem Attention Solves
   44  ▸ 2. What is Attention?
   63  ▸ 3. How Attention Works (Simple Version)
   85  ▸ 4. Self-Attention
   98  ▸ 5. Multi-Head Attention
  123  ▸ 6. Positional Encoding
  143  ▸ 7. The Transformer Architecture
  147    · Two variants:
  157    · Decoder block (what GPT uses) — one layer:
  182  ▸ 8. Feed-Forward Network (FFN)
  192  ▸ 9. Stacking Layers — How Depth Helps
  209  ▸ 10. GPT Architecture Walkthrough — End to End
  238  ▸ 11. Key Numbers to Know (for Azure Architect context)
  250  ▸ 12. Why This Matters for You as an Architect
  264  ▸ 13. Common Misconceptions (for interviews)
  276  ▸ 14. Mini Quiz (Test Yourself)
  289  ▸ Memory Hooks
  300  ▸ What Comes Next (Module 11.2+)
  312  ▸ 2026 Updates
  324  ▸ Interactive Learning Ideas
  326    · Exercise 1 — Attention on Paper (15 min)
  333    · Exercise 2 — Context Window Budget Planning (10 min)
  342    · Exercise 3 — Model Selection Decision
  349    · Exercise 4 — MoE vs Dense Model Comparison
```

### Module 11.2 — Tokenization & Embeddings (Deep Dive)
`01_Lessons/Part3_GenAI_LLMs/L11_2_LLMs_Tokenization_Embeddings.md` · 578 lines · 39 topics

```
    6  ▸ What You Already Know (Recap)
   24  ▸ Part A — Tokenization (Deep Dive)
   28  ▸ 1. What is Tokenization?
   50  ▸ 2. BPE — Byte Pair Encoding (GPT's approach)
   93  ▸ 3. SentencePiece — Used by T5, LLaMA, Gemini
  123  ▸ 4. WordPiece — Used by BERT, Azure AI Language
  151  ▸ 5. Token Limits and Context Windows ⭐ (Most Important for Architects)
  155    · What is the Context Window?
  176    · Context Windows by Model
  189    · Why Context Window Matters for RAG
  226    · The "Lost in the Middle" Problem
  240    · Token Limits Affect Cost
  256  ▸ Part B — Embeddings (Deep Dive)
  260  ▸ 6. What Are Embeddings (Revisited)?
  273    · Why similar meaning = similar vector
  283  ▸ 7. Word vs Sentence Embeddings
  285    · Word Embeddings (older — Word2Vec, GloVe)
  290    · Contextual Embeddings (modern — BERT, OpenAI)
  295    · Sentence / Chunk Embeddings (what RAG uses)
  319  ▸ 8. Embedding Dimensions and Similarity
  321    · Dimensions
  336    · Similarity Measures
  365  ▸ 9. Using Embeddings for Semantic Search
  369    · The Full RAG Embedding Flow
  392    · Hybrid Search (Keyword + Semantic)
  418  ▸ 10. Embedding Happens in Two Places (Clarification)
  453  ▸ 11. Why This Matters for You as an Architect
  468  ▸ 12. Numbers to Know
  483  ▸ 13. Common Misconceptions
  496  ▸ 14. Mini Quiz (Test Yourself)
  509  ▸ Memory Hooks
  522  ▸ What Comes Next (Module 11.3)
  534  ▸ 2026 Updates
  546  ▸ Interactive Learning Ideas
  548    · Exercise 1 — Tokenizer Hands-On (10 min)
  555    · Exercise 2 — Embedding Dimension Trade-off
  562    · Exercise 3 — Cosine Similarity Calculator (15 min)
  568    · Exercise 4 — Token Budget Audit
```

### Module 11.3 — Pre-training & Fine-tuning
`01_Lessons/Part3_GenAI_LLMs/L11_3_LLMs_Pretraining_Finetuning.md` · 543 lines · 38 topics

```
    6  ▸ What You Already Know (Recap)
   27  ▸ Part A — Pre-training
   31  ▸ 1. What is Pre-training?
   49  ▸ 2. How Pre-training Works — Next-Token Prediction at Scale
   91  ▸ 3. The Training Data
  111  ▸ 4. Why Models Have a Knowledge Cutoff Date
  138  ▸ 5. What Pre-training Produces — The Base Model
  157  ▸ Part B — Fine-tuning
  161  ▸ 6. What is Fine-tuning?
  180  ▸ 7. Types of Fine-tuning
  182    · Type 1 — Instruction Fine-tuning (Supervised Fine-tuning / SFT)
  201    · Type 2 — Domain Fine-tuning
  212    · Type 3 — Task-specific Fine-tuning
  223  ▸ 8. RLHF — Reinforcement Learning from Human Feedback
  266  ▸ 9. LoRA and QLoRA — Efficient Fine-tuning
  272    · How LoRA Works
  294    · QLoRA — Quantized LoRA
  306    · LoRA in Azure OpenAI
  325  ▸ 10. Fine-tuning vs RAG vs Prompt Engineering — The Decision Framework ⭐
  329    · The Three Levers
  337    · Decision Tree
  356    · When Fine-tuning is the Wrong Answer
  365    · When Fine-tuning IS the Right Answer
  375    · Applied to JM Family IT Helpdesk
  393  ▸ 11. Transfer Learning — Why You Don't Train From Scratch
  419  ▸ 12. Why This Matters for You as an Architect
  433  ▸ 13. Numbers to Know
  447  ▸ 14. Common Misconceptions
  460  ▸ 15. Mini Quiz (Test Yourself)
  473  ▸ Memory Hooks
  485  ▸ What Comes Next (Module 11.4)
  498  ▸ 2026 Updates
  510  ▸ Interactive Learning Ideas
  512    · Exercise 1 — Fine-tune Decision Drill (10 min)
  520    · Exercise 2 — JSONL Fine-tune Dataset (20 min)
  527    · Exercise 3 — Training Cost Estimate
  534    · Exercise 4 — Loss Curve Analysis
```

### Module 11.4 — RLHF & Alignment
`01_Lessons/Part3_GenAI_LLMs/L11_4_LLMs_RLHF_Alignment.md` · 511 lines · 38 topics

```
    6  ▸ What You Already Know (Recap)
   24  ▸ Part A — RLHF in Depth
   28  ▸ 1. The Problem RLHF Solves
   53  ▸ 2. RLHF — The Full Process
   63    · Stage 1 — Supervised Fine-tuning (SFT)
   67    · Stage 2 — Reward Model Training
   91    · Stage 3 — PPO (Proximal Policy Optimization)
  123  ▸ 3. What RLHF Actually Optimizes For
  145  ▸ Part B — Alignment
  149  ▸ 4. What is Alignment?
  155    · Why Alignment is Hard
  173    · Types of Misalignment
  184  ▸ 5. Constitutional AI — Anthropic's Approach (How Claude Works)
  190    · The CAI Process
  207    · The Constitution
  221  ▸ 6. Prompt Injection and Jailbreaking
  225    · Jailbreaking
  247    · Prompt Injection
  275    · Defending Against Prompt Injection in RAG
  300  ▸ 7. Azure Content Safety — Where It Fits
  306    · What It Detects
  318    · Severity Levels
  328    · Where It Sits in Your Architecture
  346    · Groundedness Detection (RAG-specific)
  361  ▸ 8. Responsible AI Principles (Microsoft / Azure)
  385  ▸ 9. Why This Matters for You as an Architect
  399  ▸ 10. Numbers to Know
  411  ▸ 11. Common Misconceptions
  424  ▸ 12. Mini Quiz (Test Yourself)
  437  ▸ Memory Hooks
  450  ▸ What Comes Next (Module 11.5)
  464  ▸ 2026 Updates
  476  ▸ Interactive Learning Ideas
  478    · Exercise 1 — Prompt Injection Test (15 min)
  485    · Exercise 2 — Groundedness Check Implementation (20 min)
  492    · Exercise 3 — Content Safety Architecture Design (15 min)
  499    · Exercise 4 — Six Principles Compliance Check
```

### Module 12 — Azure OpenAI Service
`01_Lessons/Part3_GenAI_LLMs/L12_AzureOpenAI_Services.md` · 1016 lines · 59 topics

```
    6  ▸ What You Already Know (Recap)
   29  ▸ 1. What is Azure OpenAI Service?
   50  ▸ 2. Models Available in Azure OpenAI
   70  ▸ 3. Key Concepts: Deployments and Endpoints
   72    · Deployments
   87    · Endpoints
  101    · Tokens Per Minute (TPM)
  116  ▸ 4. The Chat Completions API
  120    · Request Structure
  153    · The Three Roles
  161    · Key Parameters
  178    · Response Structure
  210  ▸ 5. Streaming Responses
  237  ▸ 6. System Prompt Engineering for Azure OpenAI
  241    · Structure for an IT Helpdesk System Prompt
  251    · Full Example
  281  ▸ 7. The Embeddings API
  285    · How to Call It
  311    · Batch Embedding (for indexing documents)
  325    · Embedding Cost
  336  ▸ 8. Function Calling (Tool Use)
  342    · How It Works
  353    · IT Helpdesk Example
  439    · When to Use Function Calling
  453  ▸ 9. RAG with Azure OpenAI + Azure AI Search
  457    · Architecture
  492    · Azure OpenAI On Your Data Feature
  522  ▸ 10. Authentication and Security
  524    · Managed Identity (Recommended)
  543    · Network Security
  560  ▸ 11. Monitoring and Cost Management
  562    · Azure Monitor + Application Insights
  574    · Cost Formula
  596    · Cost Controls
  608  ▸ 12. Why This Matters for You as an Architect
  623  ▸ 13. Numbers to Know
  638  ▸ 14. Common Misconceptions
  651  ▸ 15. Mini Quiz (Test Yourself)
  664  ▸ Memory Hooks
  677  ▸ What Comes Next (Module 13)
  691  ▸ 2026 Updates
  704  ▸ Interactive Learning Ideas
  706    · Exercise 1 — Structured Outputs in C# (20 min)
  719    · Exercise 2 — o1 vs GPT-4o Comparison (15 min)
  724    · Exercise 3 — Batch API for Nightly Jobs (15 min)
  732    · Exercise 4 — Function Calling Chain
  747  ▸ Interview Gap 1: Parallel Function Calling
  749    · What It Is
  775    · C# Implementation
  846  ▸ Interview Gap 2: Resilience Patterns for AI Endpoints
  848    · The Problem
  868    · Pattern 1 — Exponential Backoff with Polly (C#)
  898    · Pattern 2 — Fallback to Secondary Deployment
  921    · Pattern 3 — Circuit Breaker
  936    · TPM/RPM Quota Strategy
  962  ▸ Interview Gap 3: Model Selection & Cost Routing
  964    · The Decision Table
  981    · Cost Routing in Code (SK)
```

### Module 13 — RAG (Retrieval-Augmented Generation) Deep Dive
`01_Lessons/Part3_GenAI_LLMs/L13_RAG_DeepDive.md` · 1527 lines · 69 topics

```
    6  ▸ What You Already Know (Recap)
   24  ▸ Topic 13.1 — RAG Fundamentals
   28    · 1. Why RAG Exists
   43    · 2. The Basic RAG Pattern
   61    · 3. Naive RAG vs Advanced RAG vs Modular RAG
   73    · 4. What RAG Is NOT
   86    · 5. RAG Architecture Components
  104  ▸ Topic 13.2 — Document Processing
  108    · 1. The Document Processing Problem
  123    · 2. Document Loading in Azure
  164    · 3. Text Cleaning Before Chunking
  197    · 4. Metadata Extraction
  226  ▸ Topic 13.3 — Chunking Strategies
  230    · 1. Why Chunking Matters
  244    · 2. Fixed-Size Chunking
  278    · 3. Sentence / Paragraph Chunking
  308    · 4. Recursive Character Chunking
  362    · 5. Semantic Chunking
  401    · 6. Document-Specific Chunking
  442    · 7. Chunk Size Guidelines
  458    · 8. Parent-Child Chunking (Small-to-Big Retrieval)
  494  ▸ Topic 13.4 — Vector Databases
  498    · 1. What Is a Vector Database?
  507    · 2. Azure AI Search as Your Vector Database
  526    · 3. Other Vector Database Options
  543    · 4. Distance Metrics
  557    · 5. HNSW — How Vector Search Works Internally
  579  ▸ Topic 13.5 — Retrieval Strategies
  583    · 1. The Retrieval Problem
  594    · 2. Basic Retrieval — Top-K
  618    · 3. Filtered Retrieval
  637    · 4. Multi-Query Retrieval
  670    · 5. HyDE — Hypothetical Document Embeddings
  699    · 6. Maximal Marginal Relevance (MMR)
  741    · 7. Re-Ranking with a Cross-Encoder
  767    · 8. Self-Querying Retrieval
  802  ▸ Topic 13.6 — Generation with Retrieved Context
  806    · 1. Prompt Construction
  869    · 2. Context Window Management
  901    · 3. Citation and Grounding
  941    · 4. Hallucination Prevention
  969    · 5. Conversation History in RAG (Multi-turn)
 1000  ▸ Topic 13.7 — Azure "On Your Data" Feature
 1004    · 1. What Is "On Your Data"?
 1020    · 2. On Your Data — API Configuration
 1052    · 3. On Your Data — When to Use vs Custom RAG
 1070    · 4. On Your Data Limitations
 1080  ▸ Topic 13.8 — Advanced RAG Patterns
 1084    · 1. Corrective RAG (CRAG)
 1126    · 2. Query Decomposition
 1157    · 3. Step-Back Prompting
 1175    · 4. Agentic RAG
 1196    · 5. RAG Evaluation — How to Measure Quality
 1216    · 6. Production RAG Architecture — JM Family Full Picture
 1261  ▸ Module 13 — Self-Test Questions
 1299  ▸ Memory Hooks
 1316  ▸ 2026 Updates
 1328  ▸ Interactive Learning Ideas
 1330    · Exercise 1 — Chunk Size Experiment (20 min)
 1333    · Exercise 2 — HyDE Implementation (20 min)
 1342    · Exercise 3 — Citation Chain Implementation
 1350    · Exercise 4 — GraphRAG vs Standard RAG Comparison
 1356    · Exercise 5 — RAG Pipeline Health Check
 1370  ▸ Interview Gap: Advanced Chunking Strategies
 1372    · Why Basic Chunking Is Not Enough
 1390    · Strategy 5 — Parent-Child Chunking
 1470    · Strategy 6 — Late Chunking
 1518    · Chunking Strategy Decision Table
```

### Module 15 — Fine-tuning LLMs
`01_Lessons/Part3_GenAI_LLMs/L14_FineTuning.md` · 795 lines · 32 topics

```
    6  ▸ What You Already Know (Recap)
   24  ▸ Topic 15.1 — When to Fine-tune
   28    · 1. The Decision Framework — Revisited
   63    · 2. The Four Legitimate Reasons to Fine-tune
   96    · 3. When NOT to Fine-tune
  124    · 4. Fine-tuning vs RAG vs Prompt Engineering — Decision Table
  144    · 5. Cost Reality Check Before Fine-tuning
  174  ▸ Topic 15.2 — Azure OpenAI Fine-tuning
  178    · 1. Supported Models for Fine-tuning in Azure OpenAI
  199    · 2. Training Data Format — JSONL
  230    · 3. Preparing Good Training Data
  257    · 4. The Fine-tuning Workflow in Azure OpenAI
  271    · 5. C# — Complete Fine-tuning Workflow
  376    · 6. Monitoring Training Quality — Loss Curves
  410    · 7. Evaluating the Fine-tuned Model
  455  ▸ Topic 15.3 — Parameter-Efficient Fine-tuning (LoRA and QLoRA)
  459    · 1. Why Full Fine-tuning Is Expensive
  480    · 2. LoRA — Low-Rank Adaptation
  519    · 3. LoRA Key Hyperparameters
  544    · 4. QLoRA — Quantized LoRA
  583    · 5. LoRA vs Azure OpenAI Fine-tuning — Which to Use
  604    · 6. LoRA Fine-tuning with Azure ML — Python (Awareness Level)
  665    · 7. Complete Picture — Fine-tuning Decision Flow
  695  ▸ Module 15 — Self-Test Questions
  733  ▸ Memory Hooks
  750  ▸ 2026 Updates
  762  ▸ Interactive Learning Ideas
  764    · Exercise 1 — Decision Framework Quiz (10 min)
  772    · Exercise 2 — JSONL Dataset Creation (20 min)
  777    · Exercise 3 — LoRA Math (10 min)
  784    · Exercise 4 — Distillation Pipeline Design
```

### Module 16 — Prompt Engineering
`01_Lessons/Part3_GenAI_LLMs/L15_PromptEngineering.md` · 782 lines · 37 topics

```
    7  ▸ What You Already Know (Recap)
   23  ▸ Topic 16.1 — Why Prompt Engineering Matters
   27    · 1. The Same Model, Completely Different Results
   52    · 2. Why It Matters More Than You Think
   76    · 3. Prompt Structure — The Three Roles
  109  ▸ Topic 16.2 — Core Prompting Patterns
  113    · 1. Zero-Shot Prompting
  143    · 2. Few-Shot Prompting
  182    · 3. Chain of Thought (CoT) Prompting
  227    · 4. ReAct Prompting (Reason + Act)
  257    · 5. Zero-Shot vs Few-Shot vs Chain of Thought — When to Use
  279  ▸ Topic 16.3 — System Prompt Design
  283    · 1. The System Prompt Is the Most Important Prompt
  308    · 2. System Prompt Template — JM Family Invoice Assistant
  352    · 3. Common System Prompt Mistakes
  381  ▸ Topic 16.4 — Advanced Patterns
  385    · 1. Self-Consistency
  408    · 2. Prompt Chaining
  440    · 3. Meta-Prompting
  467  ▸ Topic 16.5 — Prompt Injection Defense
  471    · 1. What Is Prompt Injection
  490    · 2. Two Types of Injection
  513    · 3. Defense Patterns
  553    · 4. C# — Input Validation Before LLM Call
  592  ▸ Topic 16.6 — Prompt Optimization for Production
  596    · 1. Token Cost Optimization
  624    · 2. Temperature Setting by Use Case
  652    · 3. Structured Output — JSON Mode
  681  ▸ Module 16 — Self-Test Questions
  719  ▸ Memory Hooks
  736  ▸ 2026 Updates
  748  ▸ Interactive Learning Ideas
  750    · Exercise 1 — System Prompt Hardening (20 min)
  757    · Exercise 2 — Temperature Calibration (15 min)
  762    · Exercise 3 — Prompt Caching Cost Calculator
  769    · Exercise 4 — Chain-of-Thought vs Direct Answer
```

### Module 14 — AI Orchestration: Semantic Kernel, LangChain & AI Agents
`01_Lessons/Part3_GenAI_LLMs/L16_AIOrchestration_SK_Agents.md` · 2084 lines · 81 topics

```
    6  ▸ What You Already Know (Recap)
   24  ▸ Topic 14.1 — What Is AI Orchestration and Why It Exists
   28    · 1. The Problem RAG Alone Cannot Solve
   54    · 2. What Orchestration Actually Does
   76    · 3. The Two Main Frameworks
   88  ▸ Topic 14.2 — Semantic Kernel
   92    · 1. What Is Semantic Kernel?
  115    · 2. Core Concepts
  140    · 3. Setting Up the Kernel — C#
  163    · 4. Plugins — The Tools the LLM Can Use
  211    · 5. Semantic Functions — Prompts as Functions
  237    · 6. Invoking Functions Directly
  256    · 7. Chat with Auto Function Calling — The Magic
  298    · 8. Memory in Semantic Kernel
  328  ▸ Topic 14.3 — LangChain (Awareness Level)
  332    · 1. What Is LangChain?
  358    · 2. Key LangChain Concepts Mapped to Semantic Kernel
  372    · 3. LangChain RAG in Python — For Awareness
  412  ▸ Topic 14.4 — AI Agents
  416    · 1. What Is an AI Agent?
  448    · 2. The ReAct Pattern — How Agents Think
  474    · 3. Agent Memory — Two Types
  498    · 4. Building an Agent with Semantic Kernel — C#
  562    · 5. Agent vs RAG vs Function Calling — The Distinction
  595    · 6. When to Use an Agent — Decision Guide
  615  ▸ Topic 14.5 — Agentic RAG
  619    · 1. What Is Agentic RAG?
  638    · 2. Agentic RAG Pattern — JM Family
  704    · 3. Multi-Index Agentic RAG Architecture
  733  ▸ Topic 14.6 — Azure AI Foundry and Prompt Flow
  737    · 1. What Is Azure AI Foundry?
  761    · 2. Prompt Flow — Visual Orchestration
  787    · 3. Prompt Flow vs Semantic Kernel — When to Use Which
  800    · 4. RAG Evaluation in Azure AI Foundry
  844  ▸ Topic 14.7 — Production Patterns
  848    · 1. Multi-Agent Systems
  891    · 2. Agent Safety — Critical for Production
  960    · 3. Complete JM Family Agent Architecture
  994  ▸ Module 14 — Self-Test Questions
 1032  ▸ Topic 14.X — Memory Management (Practical Strategies)
 1036    · The Problem Our Modules Left Unanswered
 1063    · 1. The Context Window Reality
 1086    · 2. Strategy 1 — Sliding Window (Simplest)
 1123    · 3. Strategy 2 — Conversation Summarization (Better)
 1191    · 4. Strategy 3 — SK Built-in ChatHistoryReducer (Production Standard)
 1232    · 5. Strategy 4 — Token Counting Before Every Request
 1270    · 6. Priority-Based Memory — What to Always Keep
 1307    · 7. Memory Management Decision Tree
 1330  ▸ Topic 14.Y — Prompt Compression
 1334    · The Problem
 1355    · 1. What Is Prompt Compression?
 1377    · 2. RAG Chunk Compression — Most Impactful
 1431    · 3. LLMLingua — Microsoft's Prompt Compression Library
 1485    · 4. Dynamic Few-Shot Selection
 1545    · 5. System Prompt Compression — Quick Wins
 1572    · 6. Prompt Caching — Azure's Built-In Compression Alternative
 1608    · 7. Compression Strategy by Scenario
 1636  ▸ Interview Gap 1: Tool vs Knowledge vs Fine-Tune — The 3-Way Decision
 1642    · The Three Options
 1670    · Decision Framework
 1699    · Side-by-Side Comparison
 1714    · The JMA Agent — Applied Decision
 1742    · Interview One-Liner
 1748  ▸ Interview Gap 2: Streaming in Semantic Kernel
 1750    · Why It Matters
 1764    · SK Streaming — IAsyncEnumerable Pattern
 1798    · Pushing Tokens to UI with SignalR
 1822    · Streaming with Tool Calls
 1845  ▸ Interview Gap 3: Grounding Validation in Code
 1847    · The Problem
 1869    · Pattern 1 — Azure Content Safety Groundedness Detection
 1916    · Pattern 2 — Citation-Based Validation in Code
 1970    · Pattern 3 — Semantic Similarity Score
 1998    · Which Pattern to Use
 2028  ▸ 2026 Updates
 2041  ▸ Interactive Learning Ideas
 2043    · Exercise 1 — SK Plugin in C# (30 min)
 2056    · Exercise 2 — Multi-Agent Design (20 min)
 2064    · Exercise 3 — ReAct Loop Trace (15 min)
 2073    · Exercise 4 — SK vs Copilot Studio Decision
```

## Part 4 — Architecture & Operations


### Module 17 — Azure AI Foundry
`01_Lessons/Part4_Architecture/L17_AzureAIFoundry.md` · 1102 lines · 48 topics

```
    7  ▸ What You Already Know (Recap)
   25  ▸ Topic 17.1 — What Is Azure AI Foundry
   29    · 1. The One-Line Definition
   38    · 2. Why It Exists — The Problem Before Foundry
   63    · 3. Azure AI Foundry vs Azure OpenAI Studio
   87    · 4. Azure AI Foundry vs Semantic Kernel — Clarified
  123    · 5. Azure AI Foundry Key Components
  153  ▸ Topic 17.2 — Model Catalog
  157    · 1. What Is the Model Catalog
  175    · 2. Two Deployment Options for Catalog Models
  202    · 3. How to Choose the Right Model
  229    · 4. Model Comparison in Azure AI Foundry
  246  ▸ Topic 17.3 — Prompt Flow
  250    · 1. What Is Prompt Flow
  264    · 2. The RAG Pipeline in Prompt Flow
  300    · 3. Node Types in Prompt Flow
  326    · 4. Prompt Flow vs Semantic Kernel — Decision Guide
  354    · 5. Deploying a Prompt Flow as REST Endpoint
  380  ▸ Topic 17.4 — Evaluation Flows
  384    · 1. Why Evaluation Matters
  404    · 2. The Five Evaluation Metrics
  443    · 3. Running an Evaluation in Azure AI Foundry
  481    · 4. Minimum Quality Bar for Production
  502    · 5. C# — Calling the Evaluation API
  552  ▸ Topic 17.5 — Fine-tuning in Azure AI Foundry
  556    · 1. Fine-tuning UI — No Code Required
  578    · 2. Monitoring Training in the UI
  595    · 3. Fine-tuning → Evaluate → Deploy Flow
  613  ▸ Topic 17.6 — Content Safety and Responsible AI
  617    · 1. Content Safety Built Into Every Deployment
  644    · 2. Groundedness Detection — New in Foundry
  673    · 3. Responsible AI Dashboard
  702    · 4. Complete JM Family AI Foundry Workflow
  743  ▸ Module 17 — Self-Test Questions
  781  ▸ Memory Hooks
  798  ▸ 2026 Updates
  811  ▸ Interactive Learning Ideas
  813    · Exercise 1 — Build a JMA Agent in Foundry (30 min)
  821    · Exercise 2 — Evaluation Pipeline (20 min)
  829    · Exercise 3 — Tracing a Failed Agent Response (15 min)
  836    · Exercise 4 — Foundry vs SK Decision
  851  ▸ Appendix — Merged from Legacy Notes
  855    · 1. Tool vs Knowledge (RAG) vs Fine-Tune — The Deciding Factor
  884    · 2. Why RAG Cannot Replace a Tool for Live Data
  921    · 3. Decision Tree — Which Capability to Attach
  952    · 4. JMA Worked Example — Dealer Support Agent End-to-End
 1045    · 5. Azure AI Foundry — Complete Capability Taxonomy
```

### Module 18 — AI Solution Architecture
`01_Lessons/Part4_Architecture/L18_AISolutionArchitecture.md` · 509 lines · 29 topics

```
    9  ▸ What This Module Covers
   20  ▸ 18.1 Architecture Patterns for AI Solutions
   22    · The Three Core Patterns
   43    · Pattern 1 — Simple RAG Architecture
   70    · Pattern 2 — Agentic Architecture
   93    · Pattern 3 — Batch Ingestion Pipeline
  115    · Decision Table — Which Pattern to Use
  131  ▸ 18.2 Scalability and Performance
  133    · The Four Scalability Levers
  162    · Latency Optimisation
  195    · JM Family Scalability Example
  215  ▸ 18.3 Security for AI
  217    · The AI Security Threat Model
  250    · Security Architecture in C#
  292    · Security Checklist for AI Architect
  318  ▸ 18.4 Cost Management and Optimisation
  320    · Where AI Costs Come From
  336    · The Cost Formula
  360    · Cost Optimisation Strategies
  398    · Cost Monitoring Setup
  425    · Architecture Decision at JM Family
  445  ▸ Self-Test Questions
  461  ▸ 2026 Updates
  473  ▸ Interactive Learning Ideas
  475    · Exercise 1 — JMA Architecture Review (20 min)
  478    · Exercise 2 — Cost Model (15 min)
  487    · Exercise 3 — Latency Budget Design
  497    · Exercise 4 — Security Architecture Checklist
```

### Module 19 — MLOps and LLMOps
`01_Lessons/Part4_Architecture/L19_MLOps_LLMOps.md` · 757 lines · 37 topics

```
     9  ▸ What This Module Covers
    22  ▸ 19.1 What Is MLOps and LLMOps
    24    · The Problem Without Ops
    38    · MLOps vs LLMOps — Side by Side
    58    · One Line Each
    71  ▸ 19.2 Model Versioning and Lifecycle Management
    73    · Traditional ML — Model Lifecycle (Azure ML)
   102    · LLM — Model Lifecycle (Azure AI Foundry)
   130    · JM Family Model Registry Example
   145  ▸ 19.3 CI/CD for AI Pipelines
   147    · Standard Software CI/CD vs AI CI/CD
   166    · AI CI/CD Pipeline — Azure DevOps
   203    · Azure DevOps Pipeline YAML (key stages)
   260    · The Same Pipeline in GitHub Actions
   344  ▸ 19.4 Monitoring and Observability
   346    · What to Monitor in AI Systems
   369    · Observability Code in C#
   415    · App Insights Dashboard — What to Build
   447  ▸ 19.5 Drift Detection and Retraining
   449    · Two Types of Drift
   477    · Drift Response Playbook
   503    · Automated Retraining (Traditional ML)
   528  ▸ 19.6 LLMOps — Prompt Versioning, Evaluation, A/B Testing
   530    · Prompt Versioning
   553    · Prompt Version in C#
   574    · Evaluation Pipeline — Golden Dataset
   595    · A/B Testing Prompts
   620    · LLMOps Maturity Levels
   651  ▸ MLOps vs LLMOps — Final Summary
   672  ▸ Self-Test Questions
   688  ▸ 2026 Updates
   700  ▸ Interactive Learning Ideas
   702    · Exercise 1 — Golden Dataset Creation (20 min)
   710    · Exercise 2 — CI/CD Pipeline Design (15 min)
   723    · Exercise 3 — Drift Detection for LLMs (15 min)
   731    · Exercise 4 — .prompty File Creation
```

### Module 20 — Integration Patterns
`01_Lessons/Part4_Architecture/L20_IntegrationPatterns.md` · 602 lines · 27 topics

```
    7  ▸ Why This Module Matters
   22  ▸ Topic 20.1 — Azure Integration Services for AI
   26    · 1. The AI Integration Stack
   55    · 2. Azure Functions as the AI Processing Engine
  105    · 3. Event Grid — Event-Driven AI Triggers
  132    · 4. Service Bus — Rate-Limiting AI Calls
  179    · 5. Azure API Management as AI Gateway
  221    · 6. Azure Data Factory — Batch AI Enrichment
  259  ▸ Topic 20.2 — Microsoft 365 Integration
  263    · 1. The Microsoft Copilot Ecosystem
  289    · 2. Microsoft Graph — Accessing M365 Data for AI
  333    · 3. Building a Declarative Agent for M365 Copilot
  380    · 4. Power Platform AI Builder
  398  ▸ Topic 20.3 — Enterprise Data Integration
  402    · 1. Connecting Enterprise Data Sources to AI
  429    · 2. Real-Time vs Batch Data Pipelines
  457    · 3. Data Governance for AI
  492    · 4. Azure Synapse + AI — Analytics at Scale
  512  ▸ Topic R20 — Recall: Module 20 Review & Quiz
  546  ▸ Memory Hooks
  558  ▸ Interactive Learning Ideas
  560    · Exercise 1 — Full Pipeline Design (30 min)
  567    · Exercise 2 — APIM Rate Limiting Setup (20 min)
  575    · Exercise 3 — Graph Integration (20 min)
  582    · Exercise 4 — Declarative Agent Build (20 min)
  590    · Exercise 5 — Data Governance Audit
```

### Module 21 — Python for AI
`01_Lessons/Part4_Architecture/L21_Python_for_AI.md` · 889 lines · 44 topics

```
    7  ▸ Why This Module Exists
   19  ▸ Topic 21.1 — Python Basics (C# Developer Fast-Track)
   23    · 1. Setup
   52    · 2. Variables and Types
   80    · 3. Strings
  108    · 4. Lists and Dictionaries
  153    · 5. Functions
  181    · 6. Classes
  225    · 7. Async / Await
  251    · 8. Error Handling
  269    · 9. Working with JSON
  296    · 10. Environment Variables (.env files)
  317  ▸ Topic 21.2 — Azure OpenAI in Python
  321    · 1. Setup
  327    · 2. Client Initialization
  354    · 3. Chat Completion
  376    · 4. Streaming
  392    · 5. Embeddings
  410    · 6. Function Calling / Tool Use
  469    · 7. Structured Output
  504  ▸ Topic 21.3 — Jupyter Notebooks
  508    · What They Are
  521    · Running Notebooks
  535    · Notebook Shortcuts
  547    · Reading a Data Science Notebook
  582  ▸ Topic 21.4 — LangChain (Awareness Level)
  586    · What LangChain Is
  594    · Core Concepts Mapped to Semantic Kernel
  606    · Simple Chat
  627    · RAG Pipeline in LangChain
  660    · LangChain Agent
  699  ▸ Topic 21.5 — Azure AI Services in Python
  703    · Azure AI Language (NLP)
  727    · Azure AI Search (Vector Search)
  762    · Azure Document Intelligence
  789  ▸ Topic R21 — Quick Recall: Python vs C# Cheat Sheet
  821  ▸ Interactive Learning Ideas
  823    · Exercise 1 — Hello Azure OpenAI in Python (20 min)
  834    · Exercise 2 — Embeddings + Cosine Similarity (20 min)
  847    · Exercise 3 — LangChain RAG (30 min)
  855    · Exercise 4 — Read a Data Science Notebook (20 min)
  865    · Exercise 5 — Azure AI Search in Python (20 min)
  872  ▸ Memory Hooks
```

## Part 5 — Agentic Protocols & Patterns


### Module 01 — Azure AI Foundry: Platform, Agent Lifecycle, and Healthcare Architecture
`01_Lessons/Part5_AgenticProtocols/L22_Foundry_AgentLifecycle.md` · 323 lines · 16 topics

```
    5  ▸ Why This Module Matters
   16  ▸ Section 1 — What Azure AI Foundry IS
   33  ▸ Section 2 — The Hierarchy: Hub → Project → Resources
   68  ▸ Section 3 — The 8 Building Blocks of AI Foundry
   90  ▸ Section 4 — Agent Lifecycle in AI Foundry
  126  ▸ Section 5 — Foundry vs Semantic Kernel: When to Use Which
  150  ▸ Section 6 — Evaluation Deep Dive (Clinical = Higher Bar)
  188  ▸ Section 7 — Content Safety and Guardrails
  214  ▸ Section 8 — CV SKILL: Fine-Tuning vs RAG vs Prompt Engineering
  218    · The Three Adaptation Options
  230    · Decision Framework — when to use each
  263    · The Decision Matrix
  275    · Supervised Fine-Tuning in Azure AI Foundry
  304    · Interview Answer
  311  ▸ Quick-Reference Interview Answers
```

### Module 02 — CAG vs RAG
`01_Lessons/Part5_AgenticProtocols/L23_CAG_vs_RAG.md` · 465 lines · 29 topics

```
    8  ▸ Section 1: What They Are and Why the Distinction Matters
   44  ▸ Section 2: How RAG Works — The Architecture
   48    · Phase 1 — Indexing (offline, done once or on update)
   54    · Phase 2 — Retrieval (at query time)
   60    · Phase 3 — Generation
   65    · Hybrid Search (important for healthcare)
   75  ▸ Section 3: How CAG Works — The Architecture
   93  ▸ Section 4: Head-to-Head Comparison
  108  ▸ Section 5: When to Use Which — The Decision Framework
  110    · Use RAG when:
  117    · Use CAG when:
  123    · The hybrid case:
  132  ▸ Section 6: Healthcare Context
  134    · Why RAG dominates healthcare:
  140    · Healthcare RAG examples:
  151    · Where CAG makes sense in healthcare:
  158  ▸ Section 7: Your JM Family Anchors
  168  ▸ Section 8: CTO Summary — Your 60-Second Verbal Answer
  180  ▸ Section 9: Q&A Drill
  218  ▸ Section 10 — CV SKILL: Chunking Strategies + HNSW Indexing
  222    · Chunking Strategies — the three types
  270    · HNSW Indexing — what it is and why it matters
  323  ▸ Section 11 — CV SKILL: Transformer Fundamentals
  327    · Self-Attention — simple explanation
  373    · Tokenization — BPE and WordPiece
  399    · LoRA / PEFT — efficient fine-tuning
  421    · RLHF — why models follow instructions
  448  ▸ Key Terms to Use in Interview
```

### Module 03 — Hallucination: Factual + Agentic
`01_Lessons/Part5_AgenticProtocols/L24_Hallucination_Mitigation.md` · 392 lines · 23 topics

```
    7  ▸ Section 1: What Hallucination Is and Why It Happens
   24  ▸ Section 2: Factual Hallucination
   26    · 2.1 Why It Happens
   36    · 2.2 Healthcare Consequences
   46    · 2.3 Detection: Groundedness Evaluation
   60    · 2.4 Mitigation Strategies for Factual Hallucination
   84  ▸ Section 3: Agentic Hallucination
   86    · 3.1 Why It Is Different and More Dangerous
  102    · 3.2 Why Agentic Hallucination Is Harder to Detect
  110    · 3.3 Detection Strategies for Agentic Hallucination
  137  ▸ Section 4: Architect's Framework — How to Discuss This in the Interview
  163  ▸ Section 5: JM Family Anchors
  173  ▸ Section 6: CTO Summary — Your 60-Second Verbal Answer
  185  ▸ Section 7: Q&A Drill
  213  ▸ Section 8 — CV SKILL: AI Security — Prompt Injection, Jailbreak, PII, Threat Modeling
  217    · Prompt Injection — the most dangerous AI attack
  262    · Jailbreak — bypassing safety guardrails
  283    · PII Detection and Redaction
  308    · Grounding Validation — preventing data leakage from retrieval
  327    · AI Threat Modeling — the attack surfaces
  365    · Interview Answers
  375  ▸ Key Terms to Use in Interview
```

### Module 04 — Framework Comparison: LangGraph vs AutoGen vs Semantic Kernel
`01_Lessons/Part5_AgenticProtocols/L25_AgentFramework_Comparison.md` · 258 lines · 10 topics

```
    6  ▸ Why This Module Matters
   20  ▸ Section 1 — What Each Framework IS (the 10-second version)
   34  ▸ Section 2 — Semantic Kernel (Your Home Turf)
   72  ▸ Section 3 — LangGraph (The One You'd Recommend for Python Teams)
  114  ▸ Section 4 — LangChain (The Foundation Layer)
  181  ▸ Section 5 — AutoGen (The One You Use for Research Only)
  209  ▸ Section 6 — State Management (The Trap Question)
  227  ▸ Section 7 — The Terror Question: 60% Python / 40% .NET Team
  243  ▸ Quick-Reference Interview Answers
```

### Module 05 — MCP Hub: What It Is, How It Works, and Why Healthcare Needs It
`01_Lessons/Part5_AgenticProtocols/L26_MCP_ModelContextProtocol.md` · 390 lines · 16 topics

```
    7  ▸ Why This Module Matters
   18  ▸ Section 1 — What MCP IS (and why it exists)
   38  ▸ Section 2 — What an MCP Hub IS
   69  ▸ Section 3 — MCP vs APIM (the question they WILL ask)
   93  ▸ Section 4 — The Hybrid MCP + APIM Pattern (what you'd actually build)
  130  ▸ Section 5 — MCP Hub Governance: Policies and Standards
  134    · Who Owns the MCP Hub
  150    · The 6 Governance Policies You Define
  231    · The Server Registry / Catalog
  256  ▸ Section 6 — MCP Server Boundaries, Responsibilities, and Segregation
  260    · How to Define Server Boundaries — 3 Rules
  319    · Segregation Strategies Summary
  329    · Read-Only vs Write Server Segregation
  348    · Healthcare Example — VitalCare MCP Server Map
  377  ▸ Section 7 — The Interview Answer
```

### Module 06 — Agent Workflow CENTERPIECE
`01_Lessons/Part5_AgenticProtocols/L27_Agent_Workflow_EndToEnd.md` · 762 lines · 27 topics

```
   55  ▸ Why This Is the CENTERPIECE
   63  ▸ The Full End-to-End Picture
   93  ▸ Step-by-Step Deep Dive
   95    · Step 1 — RECEIVE: How the Agent Gets a Task
  132    · Step 2 — REASON: How the Agent Thinks
  159    · Step 3 — PLAN: Selecting Tools
  193    · Step 4 — RETRIEVE: RAG in the Workflow
  220    · Step 5 — TOOL CALL: Calling External Systems
  255    · Step 6 — OBSERVE: Reading Tool Responses
  280    · Step 7 — LOOP: The ReAct Pattern
  309    · Step 8 — GENERATE: Producing the Final Response
  338    · Step 9 — VALIDATE: Guardrails Before Delivery
  381    · Step 10 — RESPOND: Delivery to User
  395    · Step 11 — MONITOR: Observability in Production
  431  ▸ The 4-5 Minute Interview Answer (Memorize This)
  455  ▸ Failure Scenarios — What If Things Go Wrong
  457    · Scenario 1: Tool call times out
  470    · Scenario 2: RAG retrieves wrong chunks
  482    · Scenario 3: Agent loops without converging
  494    · Scenario 4: Hallucinated claim in output
  507  ▸ JM Family Production Architecture (Exact Stack)
  533  ▸ Quick-Reference Interview Answers
  555  ▸ CV SKILL: Prompt Engineering Techniques + Token Optimization
  559    · The 5 Prompt Engineering Techniques
  664    · Token Optimization Strategies
  759    · Interview Answer
```

### Module 07 — Meta-Agent Hierarchies: Agents of Agents
`01_Lessons/Part5_AgenticProtocols/L28_MetaAgent_Hierarchies.md` · 164 lines · 9 topics

```
   10  ▸ Why This Module Matters
   21  ▸ Section 1 — What a Meta-Agent Hierarchy IS
   31  ▸ Section 2 — The Three-Layer Architecture
   61  ▸ Section 3 — Healthcare Example: Prior Auth Meta-Agent
   92  ▸ Section 4 — Parallel vs Sequential Execution
  126  ▸ Section 5 — Failure Propagation in Hierarchies
  149  ▸ Section 6 — JM Family Anchor
  155  ▸ Quick-Reference Interview Answers
```

### Module 08 — A2A Protocol: Agent-to-Agent Communication
`01_Lessons/Part5_AgenticProtocols/L29_A2A_Protocol.md` · 182 lines · 10 topics

```
   10  ▸ Why This Module Matters
   21  ▸ Section 1 — What A2A IS and Why It Exists
   38  ▸ Section 2 — The A2A Message Envelope
   68  ▸ Section 3 — The Agent Bus: What Validates and Routes
   90  ▸ Section 4 — Authentication Between Agents (the question they probe)
  131  ▸ Section 5 — Dead-Letter Queue: What Happens on Failure
  145  ▸ Section 6 — A2A vs Direct Method Calls vs MCP
  167  ▸ Section 7 — JM Family Anchor
  173  ▸ Quick-Reference Interview Answers
```

### Module 09 — OCR Pipelines: Azure Document Intelligence vs John Snow Labs
`01_Lessons/Part5_AgenticProtocols/L30_OCR_Pipelines.md` · 231 lines · 12 topics

```
   18  ▸ Why This Module Matters
   29  ▸ Section 1 — What OCR Is Solving (the real problem)
   39  ▸ Section 2 — Pre-Processing Pipeline (before OCR runs)
   43    · Step 1 — De-Noise
   55    · Step 2 — De-Skew
   64    · Step 3 — Binarization
   88  ▸ Section 3 — Azure Document Intelligence (What You Use)
  128  ▸ Section 4 — John Snow Labs (The Healthcare Alternative)
  159  ▸ Section 5 — Post-Processing Validation
  204  ▸ Section 6 — Confidence Routing (Your Production Pattern)
  222  ▸ Quick-Reference Interview Answers
```

### Module 10 — Fault Tolerance, Self-Healing Agents & Observability
`01_Lessons/Part5_AgenticProtocols/L31_FaultTolerance_Observability.md` · 422 lines · 26 topics

```
   22  ▸ Why This Module Matters
   33  ▸ Section 1 — Why Fault Tolerance Is Different for AI Agents
   45  ▸ Section 2 — Polly: Retry and Circuit Breaker
   49    · Retry Policy
   69    · Circuit Breaker
  103  ▸ Section 3 — Agent Self-Healing Patterns
  107    · Pattern 1 — Groundedness Drift Detection
  125    · Pattern 2 — Automatic Prompt Rollback
  135    · Pattern 3 — Agent Restart on Tool Failure Loop
  150    · Pattern 4 — Dead-Letter Replay
  167  ▸ Section 4 — End-to-End Observability: Three Layers
  171    · Layer 1 — Infrastructure (is the compute healthy?)
  177    · Layer 2 — AI Service (is the LLM responding?)
  184    · Layer 3 — Quality (is the agent answering correctly?)
  195  ▸ Section 5 — The Three-Layer Monitoring Dashboard
  215  ▸ Section 6 — JM Family Anchor
  223  ▸ Section 7 — CV SKILL: LLMOps — Complete Practice
  227    · What LLMOps Is
  241    · Component 1 — Prompt Versioning in Git
  266    · Component 2 — Automated Evaluation Pipeline
  331    · Component 3 — Model Deployment Management
  352    · Component 4 — Automatic Rollback
  378    · Component 5 — Production Monitoring Dashboard
  406    · Interview Answer
  413  ▸ Quick-Reference Interview Answers
```

## Part 6 — Applied Projects


### 01 — Concepts: Ollama & Local LLMs
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/01-Ollama-LocalRAG/01_concepts.md` · 119 lines · 9 topics

```
    7  ▸ 1. The one-sentence mental model
   22  ▸ 2. Why companies run local LLMs
   34  ▸ 3. The Ollama REST API (mirrors OpenAI)
   54  ▸ 4. Supported models (the ones that matter)
   67  ▸ 5. Quantization (the one genuinely new term)
   82  ▸ 6. FAISS — your local vector store
   99  ▸ 7. Chunking (unchanged from your Azure RAG)
  105  ▸ 8. Decision table — Ollama vs Azure OpenAI
```

### 02 — Architecture: Local RAG with Ollama
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/01-Ollama-LocalRAG/02_architecture.md` · 63 lines · 5 topics

```
    3  ▸ The pipeline
   34  ▸ Component breakdown
   45  ▸ Data flow notes
   51  ▸ Scaling this beyond a laptop
```

### 03 — Interview Q&A: Ollama & Local LLMs (15 questions, senior level)
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/01-Ollama-LocalRAG/03_interview_qa.md` · 51 lines · 1 topics

```
```

### 05 — Resume Bullet
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/01-Ollama-LocalRAG/05_resume_bullet.md` · 13 lines · 1 topics

```
```

### 01 — Ollama + Local LLMs (Local RAG)
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/01-Ollama-LocalRAG/README.md` · 72 lines · 6 topics

```
    8  ▸ Why this module matters for the job search
   19  ▸ What you'll have after this module
   26  ▸ Prerequisites
   50  ▸ Quick start (3 commands)
   61  ▸ Files
```

### 01 — Concepts: crewAI
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/02-crewAI-MultiAgent/01_concepts.md` · 95 lines · 8 topics

```
    7  ▸ 1. The one-sentence mental model
   23  ▸ 2. The four core building blocks
   45  ▸ 3. Sequential vs Hierarchical (maps to SK orchestration)
   61  ▸ 4. Tools and memory (unchanged concepts)
   68  ▸ 5. When to use crewAI vs Semantic Kernel vs LangGraph
   82  ▸ 6. crewAI vs AutoGen vs MAF — quick contrast
   90  ▸ 7. Why this matters on your resume specifically
```

### 02 — Architecture: crewAI 3-Agent Research Pipeline
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/02-crewAI-MultiAgent/02_architecture.md` · 58 lines · 5 topics

```
    3  ▸ The pipeline (sequential process)
   35  ▸ Component breakdown
   47  ▸ Data flow notes
   53  ▸ Scaling to hierarchical
```

### 03 — Interview Q&A: crewAI Multi-Agent (15 questions, senior level)
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/02-crewAI-MultiAgent/03_interview_qa.md` · 51 lines · 1 topics

```
```

### 05 — Resume Bullet
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/02-crewAI-MultiAgent/05_resume_bullet.md` · 13 lines · 1 topics

```
```

### 02 — crewAI Multi-Agent
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/02-crewAI-MultiAgent/README.md` · 51 lines · 6 topics

```
    8  ▸ Why this module matters for the job search
   14  ▸ What you'll have after this module
   21  ▸ Prerequisites
   31  ▸ Quick start
   40  ▸ Files
```

### 01 — Concepts: RAGAS Evaluation
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/03-RAGAS-Evaluation/01_concepts.md` · 91 lines · 12 topics

```
    7  ▸ 1. Why evaluate RAG at all?
   15  ▸ 2. The 4 core metrics, in plain English
   19    · Faithfulness — *"Did the answer stick to the retrieved context?"*
   23    · Answer Relevance — *"Does the answer actually address the question?"*
   26    · Context Recall — *"Did retrieval find the chunks needed to answer?"*
   29    · Context Precision — *"Are the retrieved chunks actually useful?"*
   34  ▸ 3. Which metric points at which fix
   49  ▸ 4. Score interpretation (rules of thumb)
   62  ▸ 5. How RAGAS actually computes these — "LLM-as-judge"
   71  ▸ 6. RAGAS vs TruLens vs Azure AI Evaluation
   83  ▸ 7. Ground truth — what you need to provide
```

### 02 — Architecture: RAGAS Evaluation Flow
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/03-RAGAS-Evaluation/02_architecture.md` · 67 lines · 5 topics

```
    3  ▸ The evaluation flow
   46  ▸ Component breakdown
   56  ▸ Data flow notes
   62  ▸ Where this plugs into CI/CD
```

### 03 — Interview Q&A: RAGAS Evaluation (15 questions, senior level)
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/03-RAGAS-Evaluation/03_interview_qa.md` · 51 lines · 1 topics

```
```

### 05 — Resume Bullet
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/03-RAGAS-Evaluation/05_resume_bullet.md` · 13 lines · 1 topics

```
```

### 03 — RAGAS Evaluation
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/03-RAGAS-Evaluation/README.md` · 52 lines · 6 topics

```
    8  ▸ Why this module matters for the job search
   14  ▸ What you'll have after this module
   22  ▸ Prerequisites
   30  ▸ Quick start
   40  ▸ Files
```

### 01 — Concepts: Hugging Face
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/04-HuggingFace-Transformers/01_concepts.md` · 109 lines · 8 topics

```
    7  ▸ 1. The one-sentence mental model
   23  ▸ 2. The five parts of the HF ecosystem
   34  ▸ 3. `pipeline()` — the one-liner you'll use constantly
   56  ▸ 4. Tokenizer + Model (what `pipeline` hides)
   76  ▸ 5. Finding a model on the Hub
   84  ▸ 6. Which models run locally vs need the Inference API
   95  ▸ 7. HF vs Azure OpenAI — decision table
```

### 02 — Architecture: How Hugging Face Fits Together
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/04-HuggingFace-Transformers/02_architecture.md` · 53 lines · 5 topics

```
    3  ▸ The ecosystem map
   26  ▸ The four things this module builds (mapped to demos)
   35  ▸ The RAG demo (04d) data flow
   45  ▸ Component notes
```

### 03 — Interview Q&A: Hugging Face Transformers (15 questions, senior level)
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/04-HuggingFace-Transformers/03_interview_qa.md` · 51 lines · 1 topics

```
```

### 05 — Resume Bullet
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/04-HuggingFace-Transformers/05_resume_bullet.md` · 13 lines · 1 topics

```
```

### 04 — Hugging Face Transformers
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/04-HuggingFace-Transformers/README.md` · 55 lines · 6 topics

```
    8  ▸ Why this module matters for the job search
   14  ▸ What you'll have after this module
   21  ▸ Prerequisites
   30  ▸ Quick start
   41  ▸ Files
```

### 01 — Concepts: LlamaIndex
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/05-LlamaIndex-RAG/01_concepts.md` · 100 lines · 7 topics

```
    7  ▸ 1. The one-sentence mental model
   24  ▸ 2. The core objects
   47  ▸ 3. LangChain vs LlamaIndex — the core difference
   61  ▸ 4. When to choose LlamaIndex vs LangChain (decision table)
   73  ▸ 5. LlamaIndex with local models (Ollama)
   90  ▸ 6. What LlamaIndex adds beyond naive RAG
```

### 02 — Architecture: LlamaIndex RAG
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/05-LlamaIndex-RAG/02_architecture.md` · 58 lines · 5 topics

```
    3  ▸ The pipeline
   32  ▸ Component breakdown
   47  ▸ Data flow notes
   53  ▸ Scaling beyond in-memory
```

### 03 — Interview Q&A: LlamaIndex (15 questions, senior level)
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/05-LlamaIndex-RAG/03_interview_qa.md` · 51 lines · 1 topics

```
```

### 05 — Resume Bullet
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/05-LlamaIndex-RAG/05_resume_bullet.md` · 13 lines · 1 topics

```
```

### 05 — LlamaIndex RAG
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/05-LlamaIndex-RAG/README.md` · 50 lines · 6 topics

```
    8  ▸ Why this module matters for the job search
   14  ▸ What you'll have after this module
   21  ▸ Prerequisites
   30  ▸ Quick start
   39  ▸ Files
```

### 01 — Concepts: Amazon Bedrock
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/06-Amazon-Bedrock/01_concepts.md` · 110 lines · 8 topics

```
    7  ▸ 1. The one-sentence mental model
   24  ▸ 2. Models available on Bedrock
   40  ▸ 3. Calling a model — boto3
   67  ▸ 4. Bedrock Knowledge Bases (managed RAG)
   82  ▸ 5. Bedrock Agents
   91  ▸ 6. Bedrock Guardrails
   97  ▸ 7. When to use Bedrock vs Azure AI Foundry
```

### 02 — Architecture: Amazon Bedrock
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/06-Amazon-Bedrock/02_architecture.md` · 65 lines · 6 topics

```
    3  ▸ The Bedrock platform (mapped to Azure AI Foundry)
   28  ▸ RAG via Knowledge Bases — data flow
   40  ▸ Component breakdown
   55  ▸ Two invocation APIs (know both)
   60  ▸ Multi-cloud placement note
```

### 03 — Interview Q&A: Amazon Bedrock & Multi-Cloud AI (15 questions, senior level)
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/06-Amazon-Bedrock/03_interview_qa.md` · 51 lines · 1 topics

```
```

### 05 — Resume Bullet
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/06-Amazon-Bedrock/05_resume_bullet.md` · 13 lines · 1 topics

```
```

### 06 — Amazon Bedrock
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/06-Amazon-Bedrock/README.md` · 53 lines · 6 topics

```
    8  ▸ Why this module matters for the job search
   14  ▸ What you'll have after this module
   21  ▸ Prerequisites
   32  ▸ Quick start
   41  ▸ Files
```

### Azure AI Foundry ↔ Amazon Bedrock — 15-Dimension Comparison
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/06-Amazon-Bedrock/azure_vs_bedrock_comparison.md` · 34 lines · 3 topics

```
   23  ▸ Bonus dimensions (nice to name)
   32  ▸ The one-liner for interviews
```

### 01 — Concepts: GraphRAG + Neo4j
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/07-GraphRAG-Neo4j/01_concepts.md` · 112 lines · 8 topics

```
    7  ▸ 1. The one-sentence mental model
   21  ▸ 2. Knowledge graph basics
   37  ▸ 3. Cypher — the query language (the new muscle)
   59  ▸ 4. What GraphRAG actually is
   77  ▸ 5. When GraphRAG beats vector RAG (and when it doesn't)
   92  ▸ 6. Vector vs Graph vs Hybrid — decision table
  106  ▸ 7. Neo4j in the Azure world
```

### 02 — Architecture: GraphRAG + Neo4j
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/07-GraphRAG-Neo4j/02_architecture.md` · 68 lines · 6 topics

```
    3  ▸ Graph construction + graph-RAG flow
   30  ▸ Vector RAG vs GraphRAG — same question, different retrieval (demo 04c)
   45  ▸ Component breakdown
   57  ▸ Data flow notes
   63  ▸ Neo4j deployment (this module)
```

### 03 — Interview Q&A: GraphRAG + Neo4j (15 questions, senior level)
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/07-GraphRAG-Neo4j/03_interview_qa.md` · 52 lines · 1 topics

```
```

### 05 — Resume Bullet
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/07-GraphRAG-Neo4j/05_resume_bullet.md` · 13 lines · 1 topics

```
```

### 07 — GraphRAG + Neo4j
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/07-GraphRAG-Neo4j/README.md` · 56 lines · 6 topics

```
    8  ▸ Why this module matters for the job search
   14  ▸ What you'll have after this module
   21  ▸ Prerequisites
   31  ▸ Quick start
   42  ▸ Files
```

### 01 — Concepts: LoRA / QLoRA Fine-Tuning
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/08-LoRA-FineTuning/01_concepts.md` · 128 lines · 10 topics

```
    7  ▸ 1. When fine-tuning (not prompting, not RAG)
   21  ▸ 2. Full fine-tuning vs LoRA vs QLoRA (memory + compute)
   36  ▸ 3. LoRA math, in plain English (no PhD required)
   59  ▸ 4. LoRA key hyperparameters
   70  ▸ 5. QLoRA = LoRA + 4-bit quantized frozen base
   79  ▸ 6. PEFT — the Hugging Face toolkit
   96  ▸ 7. Quantization formats (name these in interviews)
  109  ▸ 8. LoRA/QLoRA vs Azure OpenAI fine-tuning
  123  ▸ 9. Overfitting (the thing to watch)
```

### 02 — Architecture: LoRA / QLoRA Fine-Tuning
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/08-LoRA-FineTuning/02_architecture.md` · 64 lines · 6 topics

```
    3  ▸ Where LoRA sits inside the model
   19  ▸ The training flow (what the notebook does)
   38  ▸ Component breakdown
   51  ▸ Inference after fine-tuning
   59  ▸ Why this runs on free Colab
```

### 03 — Interview Q&A: LoRA / QLoRA Fine-Tuning (20 questions, senior level)
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/08-LoRA-FineTuning/03_interview_qa.md` · 68 lines · 1 topics

```
```

### 05 — Resume Bullet
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/08-LoRA-FineTuning/05_resume_bullet.md` · 13 lines · 1 topics

```
```

### 08 — LoRA / QLoRA Fine-Tuning
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/08-LoRA-FineTuning/README.md` · 48 lines · 6 topics

```
    8  ▸ Why this module matters for the job search
   14  ▸ What you'll have after this module
   21  ▸ Prerequisites
   30  ▸ Quick start
   37  ▸ Files
```

### 01 — Concepts: GCP Vertex AI + Agent Development Kit
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/09-Vertex-AI/01_concepts.md` · 120 lines · 9 topics

```
    7  ▸ 1. The one-sentence mental model
   25  ▸ 2. Gemini — the model family
   39  ▸ 3. Two ways to call Gemini (know both)
   57  ▸ 4. RAG on Vertex — two options
   66  ▸ 5. Agent Development Kit (ADK)
   95  ▸ 6. Auth — service accounts + ADC (vs Managed Identity)
  101  ▸ 7. When Vertex AI vs Azure AI Foundry vs Bedrock
  115  ▸ 8. Model Garden
```

### 02 — Architecture: GCP Vertex AI
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/09-Vertex-AI/02_architecture.md` · 72 lines · 6 topics

```
    3  ▸ The Vertex AI platform (mapped to Azure AI Foundry)
   27  ▸ RAG flow (Vertex AI Search) — mapped to Azure
   37  ▸ Agent flow (ADK) — mapped to Semantic Kernel
   50  ▸ Component breakdown
   67  ▸ Multi-cloud placement note
```

### 03 — Interview Q&A: GCP Vertex AI + ADK (15 questions, senior level)
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/09-Vertex-AI/03_interview_qa.md` · 51 lines · 1 topics

```
```

### 05 — Resume Bullet
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/09-Vertex-AI/05_resume_bullet.md` · 13 lines · 1 topics

```
```

### 09 — GCP Vertex AI + Agent Development Kit
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/09-Vertex-AI/README.md` · 54 lines · 6 topics

```
    8  ▸ Why this module matters for the job search
   14  ▸ What you'll have after this module
   21  ▸ Prerequisites
   33  ▸ Quick start
   43  ▸ Files
```

### Career Accelerator — Gap-Skill Portfolio
`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/README.md` · 40 lines · 4 topics

```
    9  ▸ Modules
   25  ▸ Each module contains
   34  ▸ The bridge philosophy
```

### JMA Dealer Intelligence Platform — Flow with Loops and Conditions
`01_Lessons/Part6_AppliedProjects/02-DealerIntelligence-Platform/FLOW_WITH_LOOPS.md` · 491 lines · 6 topics

```
    5  ▸ LEGEND
   29  ▸ PART 1 — ONE-TIME SETUP  (Run once when you deploy)
   92  ▸ PART 2 — RUNTIME FLOW  (Every time a dealer submits a claim)
  381  ▸ PART 3 — CONTINUOUS QUALITY  (Runs in background, always on)
  444  ▸ SUMMARY: CODE vs CONFIG for Every Module
```

### JMA Dealer Intelligence Platform — Complete Flow Diagram
`01_Lessons/Part6_AppliedProjects/02-DealerIntelligence-Platform/JMA-DealerIntelligence-Complete-Flow.md` · 625 lines · 8 topics

```
    2  Includes: Call Types | Environments | Zones | Loops | Conditions | Frequency
    6  ▸ LEGEND
   51  ▸ PART 1 — ONE-TIME SETUP (Build the Knowledge Base)
  131  ▸ PART 2 — RUNTIME FLOW (Every Dealer Claim Submission)
  435  ▸ PART 3 — COMPLEX CLAIMS (Meta-Agent Orchestration)
  513  ▸ PART 4 — CONTINUOUS QUALITY (LLMOps)
  579  ▸ COMPLETE CALL TYPE SUMMARY
```

### JMA Dealer Intelligence Platform
`01_Lessons/Part6_AppliedProjects/02-DealerIntelligence-Platform/README.md` · 141 lines · 8 topics

```
    8  ▸ What This Project Does
   36  ▸ Domain Mapping — JMA to Healthcare (Ascendion Interview)
   52  ▸ Module Coverage Map
   68  ▸ Tech Stack
   84  ▸ How to Read This Project
   95  ▸ The Interview Bridge (Say This)
  101  ▸ Project Structure
```

### JMA Dealer Intelligence Platform — End-to-End Workflow
`01_Lessons/Part6_AppliedProjects/02-DealerIntelligence-Platform/WORKFLOW.md` · 330 lines · 5 topics

```
    5  ▸ PATH A: ONE-TIME SETUP (Index Policy Documents)
   56  ▸ PATH B: RUNTIME (Process Dealer Claim)
  237  ▸ PATH C: CONTINUOUS QUALITY (LLMOps — runs in background)
  280  ▸ COMPLETE MODULE MAP
```

### VitalCare AI Platform
`01_Lessons/Part6_AppliedProjects/05-VitalCare-AI-Platform/README.md` · 216 lines · 14 topics

```
    9  ▸ Why This Project Exists
   20  ▸ What This System Does
   54  ▸ Domain Mapping: JMA → Healthcare
   71  ▸ Module Coverage Map
   89  ▸ Key Healthcare-Specific Differences from JMA
   91    · 1. PHI (Protected Health Information)
  106    · 2. HIPAA Compliance
  112    · 3. FHIR (Fast Healthcare Interoperability Resources)
  123    · 4. Clinical Decision Support (CDS) Rules
  129    · 5. Groundedness = Patient Safety
  137  ▸ Tech Stack
  155  ▸ Interview Bridge Quote
  172  ▸ Project Structure
```


## Part 7 — Platform Engineering & AI-Assisted Delivery

> Built 2026-07-26 for **FDE-Prep**. Closes the engineering-hands gaps the
> AI-102 / architect curriculum deliberately scoped out.

### Module 32 — Advanced Python for AI Engineers
`01_Lessons/Part7_PlatformEngineering/L32_AdvancedPython_for_AI.md` · 762 lines · 42 topics

```
    16  ▸ Why This Module Exists
    41  ▸ Section 1 — Type Hints and Data Classes
    43    · 1.1 Type hints are documentation the tooling can check
    72    · 1.2 Data classes replace boilerplate classes
   122    · 1.3 Immutability
   137    · ⚠️ The mutable-default trap — a classic interview question
   158    · 1.4 Pydantic — where you will actually meet this in AI code
   180  ▸ Section 2 — Iterators and Generators
   182    · 2.1 The idea
   206    · 2.2 Why it matters — memory
   220    · 2.3 Generator expressions
   239    · 2.4 Streaming LLM output — the AI-specific use
   256    · 2.5 `yield from`, and the `itertools` you should know
   271    · ⚠️ Generators are single-use
   284  ▸ Section 3 — Decorators
   286    · 3.1 What they are
   309    · 3.2 Always use `functools.wraps`
   325    · 3.3 A decorator with arguments — three levels deep
   362    · 3.4 Decorators you will meet in AI code
   387  ▸ Section 4 — Context Managers
   389    · 4.1 `with` is C# `using`
   401    · 4.2 Writing your own — the class form
   420    · 4.3 The generator form — shorter, and what you will usually write
   442  ▸ Section 5 — Exceptions, Properly
   444    · 5.1 Custom exception hierarchies
   472    · 5.2 Rules worth internalising
   491  ▸ Section 6 — Data Structures and Big-O
   493    · 6.1 The complexity table you must be able to recite
   506    · 6.2 The single most common interview mistake
   521    · 6.3 `collections` you should know
   537    · 6.4 Complexity of things you already do
   551  ▸ Section 7 — Design Patterns in Python
   556    · 7.1 Strategy → just pass a function
   572    · 7.2 Factory → a dict of callables
   584    · 7.3 Singleton → a module
   595    · 7.4 Dependency injection → default arguments and Protocols
   611    · 7.5 Repository, Adapter, Decorator
   619  ▸ Section 8 — Putting It Together
   691  ▸ JM Family Anchor
   704  ▸ Self-Test Questions
   723  ▸ Quick-Reference Interview Answers
   758  ▸ Related
```

### Module 33 — Infrastructure as Code: Terraform for a Bicep Developer
`01_Lessons/Part7_PlatformEngineering/L33_IaC_Terraform_for_Bicep_Devs.md` · 647 lines · 34 topics

```
    11  ▸ Why This Module Exists
    32  ▸ Section 1 — The Delta: Who Owns State
    34    · 1.1 The picture
    53    · 1.2 What state ownership forces you to handle
    63    · 1.3 The remote backend — always configure one
    92    · 1.4 ⚠️ State contains secrets in plaintext
   112  ▸ Section 2 — HCL for a Bicep Developer
   155    · 2.1 Syntax translation table
   170    · 2.2 `count` vs `for_each` — get this right
   180    · 2.3 Data sources — read something you did not create
   198  ▸ Section 3 — The Workflow
   209    · 3.1 Reading a plan
   235    · 3.2 In a pipeline
   252  ▸ Section 4 — Modules and Reuse
   286  ▸ Section 5 — Multi-Cloud in One File
   309    · 5.1 GCP — Terraform *is* the native approach
   324  ▸ Section 6 — AWS CDK: Your Unfair Advantage
   375  ▸ Section 7 — The Rest of the Landscape
   377    · 7.1 Pulumi
   391    · 7.2 Ansible and Puppet — a different layer
   425    · 7.3 VMware — awareness only
   434  ▸ Section 8 — Cloud Migration
   439    · 8.1 The 6 Rs
   450    · 8.2 Where AI reduces migration effort — the JD's actual claim
   463    · 8.3 `terraform import` — adopting what already exists
   482  ▸ Section 9 — Cloud Security and Compliance in IaC
   484    · 9.1 VPC and PrivateLink (JD row 39)
   509    · 9.2 Compliance for LLM workloads (JD row 38)
   523    · 9.3 Policy-as-code — the JD's "guardrails"
   550  ▸ Section 10 — Testing IaC
   575  ▸ JM Family Anchor
   590  ▸ Self-Test Questions
   610  ▸ Quick-Reference Interview Answers
   643  ▸ Related
```

### Module 34 — Kubernetes, Helm and GitOps for AI Platforms
`01_Lessons/Part7_PlatformEngineering/L34_Kubernetes_Helm_GitOps.md` · 501 lines · 25 topics

```
    13  ▸ Why This Module Exists
    32  ▸ Section 1 — Where Helm Fits
    41    · 1.1 The problem Helm solves
    56  ▸ Section 2 — Chart Anatomy
   147    · 2.1 Template syntax you must recognise
   167  ▸ Section 3 — Release Lifecycle
   181    · 3.1 A **release** is Helm's stateful concept
   206  ▸ Section 4 — GitOps and ArgoCD
   208    · 4.1 Push vs pull — the model change
   225    · 4.2 An ArgoCD Application
   251    · 4.3 Why this matters for AI workloads specifically
   264    · 4.4 App-of-apps
   269    · 4.5 ArgoCD vs Flux
   282  ▸ Section 5 — AKS vs EKS vs GKE
   298    · 5.1 The three you should be able to speak to
   327  ▸ Section 6 — Service Mesh
   329    · 6.1 What it is
   342    · 6.2 The options
   350    · 6.3 When NOT to use one — say this part
   362  ▸ Section 7 — OpenShift (Awareness)
   386  ▸ Section 8 — GitHub Actions (tracker row 26)
   431  ▸ JM Family Anchor
   451  ▸ Self-Test Questions
   470  ▸ Quick-Reference Interview Answers
   497  ▸ Related
```

### Module 35 — AI-Assisted Engineering
`01_Lessons/Part7_PlatformEngineering/L35_AI_Assisted_Engineering.md` · 368 lines · 26 topics

```
    11  ▸ Why This Module Exists
    28  ▸ Section 1 — The Tool Landscape
    40    · 1.1 The mental model that matters
    56  ▸ Section 2 — Cursor
    58    · 2.1 Setup (do this tonight)
    73    · 2.2 Codebase context — `@` symbols
    91    · 2.3 `.cursorrules` — the highest-leverage file in the repo
   126    · 2.4 Composer — the multi-file unlock
   140    · 2.5 Agent mode
   149  ▸ Section 3 — Prompting for Code
   160    · 3.1 The four ingredients
   167    · 3.2 What to never delegate unreviewed
   182  ▸ Section 4 — Measuring the Gain
   188    · 4.1 What to measure
   198    · 4.2 How to measure honestly
   212  ▸ Section 5 — AI-First Plays for an Infrastructure Engineer
   226    · 5.1 The vulnerability-remediation pipeline
   245    · 5.2 N8N — awareness
   261  ▸ Section 6 — Anthropic Computer-Use (tracker row 12)
   263    · 6.1 What it is
   276    · 6.2 Why an infrastructure org cares
   282    · 6.3 Why you would usually *not* use it
   299  ▸ Section 7 — Making It Real Tonight
   320  ▸ Self-Test Questions
   335  ▸ Quick-Reference Interview Answers
   364  ▸ Related
```

### Module 36 — LLM Observability, Tracing and FinOps
`01_Lessons/Part7_PlatformEngineering/L36_LLM_Observability_FinOps.md` · 528 lines · 34 topics

```
    13  ▸ Why This Module Exists
    31  ▸ Section 1 — Why LLM Observability Is Different
    58  ▸ Section 2 — OpenTelemetry for Agents
    60    · 2.1 The vocabulary
    74    · 2.2 What an agent trace should look like
    92    · 2.3 GenAI semantic conventions
   106    · 2.4 Instrumenting — Python
   135    · 2.5 Instrumenting — C# / Semantic Kernel
   149    · 2.6 Context propagation across agents
   170  ▸ Section 3 — LLM-Native Tracing Platforms
   183    · 3.1 LangSmith
   199    · 3.2 Langfuse — the one to pick under compliance
   207    · 3.3 Arize Phoenix — RAG and drift
   213    · 3.4 What to log — and the trap
   229  ▸ Section 4 — LiteLLM: Model Routing
   231    · 4.1 What it is
   243    · 4.2 Config
   273    · 4.3 Why an architect wants it
   287    · 4.4 The honest trade-off
   295  ▸ Section 5 — Semantic Caching
   324  ▸ Section 6 — FinOps for LLM
   326    · 6.1 The cost model
   338    · 6.2 The levers, in order of return
   358    · 6.3 Showback and chargeback
   371    · 6.4 Guardrails that prevent the 3 a.m. incident
   395  ▸ Section 7 — Dashboards: Prometheus, Grafana, Dynatrace
   397    · 7.1 The stack
   409    · 7.2 Grafana vs Dynatrace
   423    · 7.3 The dashboard to build
   439    · 7.4 What to alert on
   456  ▸ JM Family Anchor
   468  ▸ Self-Test Questions
   488  ▸ Quick-Reference Interview Answers
   522  ▸ Related
```

## Part 8 — Data Platform


### Module 37 — Microsoft Fabric: OneLake, Lakehouse, Medallion and the AI Data Platform
`01_Lessons/Part8_DataPlatform/L37_MicrosoftFabric.md` · 883 lines · 40 topics

```
    14  ▸ Why This Module Exists
    46  ▸ Section 1 — Fabric Architecture and the SaaS Model
    48    · 1.1 The one-line definition
    56    · 1.2 Why it is not just Synapse rebranded
    87    · 1.3 The object hierarchy
   108    · 1.4 The workloads (experiences)
   124  ▸ Section 2 — OneLake
   126    · 2.1 What it is
   152    · 2.2 The one-copy principle
   167    · 2.3 Shortcuts — the feature that makes adoption realistic
   196    · 2.4 Delta-Parquet and V-Order
   215  ▸ Section 3 — Lakehouse vs Warehouse, and Direct Lake
   217    · 3.1 The two items
   234    · 3.2 Decision criteria
   259    · 3.3 Direct Lake — the headline feature
   296  ▸ Section 4 — The Medallion Architecture, Worked Through
   304    · 4.0 The worked example (VitalCare prior auth)
   314    · 4.1 Bronze — land it exactly as it arrived
   362    · 4.2 Silver — make it correct, one row per real-world thing
   399    · 4.3 Gold — shape it for how it will be consumed
   429    · 4.4 How to lay this out in Fabric
   444    · 4.5 What each hop runs on
   457  ▸ Section 5 — Dataflows Gen2 vs Pipelines vs Notebooks
   462    · 5.1 What each one is
   476    · 5.2 The decision table
   491    · 5.3 The production pattern
   519    · 5.4 Two adjacent options worth naming (Mirroring, Copy job)
   528    · 5.5 Incremental processing — the four mechanisms
   549    · 5.6 Real-Time Intelligence, briefly
   566  ▸ Section 6 — Fabric ↔ Azure AI Foundry: Grounding Agents on OneLake
   571    · 6.1 The framing
   581    · 6.2 Four integration patterns
   663    · 6.3 The governance question interviewers use to separate candidates
   696  ▸ Section 7 — Governance, Capacity and Cost
   698    · 7.1 Capacity Units and F-SKUs
   721    · 7.2 Pause and resume — the biggest cost lever
   734    · 7.3 Smoothing, bursting and throttling
   766    · 7.4 Security and governance model (RLS · OLS · CLS)
   811    · 7.5 Cost discipline checklist
   828  ▸ Section 8 — How This Connects to Everything Else
   848  ▸ Section 9 — The 60-Second Interview Answer
   878  ▸ Related
```

## Questions & Prep


### High-Level Prep — Memory · Tokenization Efficiency · Scaling · Agents
`02_Questions/HighLevelPrep/HLP01_Memory_Tokens_Scaling_Agents.md` · 361 lines · 35 topics

```
    13  1. Memory
    17  ▸ The 30-second answer
    25  ▸ The single most important distinction
    33  ▸ Four layers
    46  ▸ Five strategies — with the trade-off that matters
    59  ▸ What to evict when the window fills
    67  ▸ Failure modes to name
    80  ▸ Multi-agent memory
    91  ▸ Your JMA hook
   100  ▸ Episodic memory — the fourth type they'll ask about
   170  2. Tokenization Efficiency
   172  ▸ The 30-second answer
   178  ▸ The budget formula
   186  ▸ The levers, ranked by actual impact
   201  ▸ Tokenizer facts worth knowing
   208  ▸ The trap question
   217  3. Scaling AI
   219  ▸ The 30-second answer
   227  ▸ Four dimensions
   236  ▸ The quota point
   243  ▸ Caching — three distinct kinds
   253  ▸ Streaming
   259  ▸ Degradation ladder
   266  4. AI Agents
   268  ▸ The 30-second answer
   274  ▸ The distinction they're testing
   285  ▸ ReAct
   289  ▸ When NOT to use an agent
   299  ▸ Framework landscape
   314  ▸ Multi-agent patterns
   323  ▸ Guardrails — name these
   331  ▸ Production concerns
   338  Rapid self-check
   355  ▸ Cross-references
```

### Module 1 — GenAI/LLM Fundamentals
`02_Questions/InterviewBank/01_Fundamentals.md` · 192 lines · 16 topics

```
    8    · Q1. What is attention, and why did it replace RNN/LSTM architectures for sequence modeling?
   20    · Q2. Explain self-attention vs cross-attention — where is each used?
   33    · Q3. Walk through the Transformer architecture end to end.
   49    · Q4. What is tokenization, and why does it matter architecturally — not just linguistically?
   62    · Q5. What are embeddings, and how is similarity between them measured?
   74    · Q6. What determines a model's context window, and what are the trade-offs of a larger one?
   89    · Q7. Differentiate pretraining, fine-tuning, and RLHF.
  101    · Q8. What problem does RLHF solve that pretraining alone doesn't?
  111    · Q9. Why do LLMs hallucinate — what's the fundamental cause?
  121    · Q10. What do temperature, top-p, and top-k control, and how would you tune them for different use cases?
  138    · Q11. What are scaling laws, and what actually determines how capable a model is?
  148    · Q12. When would you use zero-shot, few-shot prompting, versus fine-tuning?
  160    · Q13. What is Mixture of Experts (MoE), and why does it matter for cost/latency?
  170    · Q14. At the architecture level, how do multimodal models combine image and text?
  180    · Q15. What is model distillation, and when would you deploy a distilled model instead of the full-size one?
```

### Module 2 — Azure AI Platform
`02_Questions/InterviewBank/02_Azure_AI_Platform.md` · 224 lines · 19 topics

```
    8    · Q1. Multi-service Azure AI Services resource vs individual per-service resources — how do you decide?
   20    · Q2. API keys vs Managed Identity — which do you standardize on, and why?
   32    · Q3. Cognitive Services User vs Contributor — who gets which role?
   44    · Q4. VNet Service Endpoints + firewall rules vs Private Endpoint — when do you need the stronger option?
   56    · Q5. What do you monitor on every Azure AI Services resource, and why those specific things?
   68    · Q6. How do you handle 429 throttling, and why doesn't "just add more instances" work?
   80    · Q7. Azure OpenAI: PTU (Provisioned Throughput) vs pay-as-you-go — how do you choose?
   92    · Q8. How do you handle Azure OpenAI model version deprecation in a production architecture?
  104    · Q9. Azure AI Search: keyword vs vector vs hybrid — when is pure keyword actually the right call?
  116    · Q10. Push API vs indexer (pull) model for Azure AI Search — how do you decide?
  128    · Q11. Document Intelligence: prebuilt models vs custom — what's the actual decision threshold?
  140    · Q12. What does Azure AI Foundry unify, and when do you actually need it vs raw resource management?
  152    · Q13. Content Safety: what are you actually configuring, and where does groundedness detection fit?
  164    · Q14. When do Azure AI Services containers make sense over the cloud endpoint?
  176    · Q15. In a shared multi-service resource, how do you maintain a security boundary between different consuming teams/services?
  188    · Q16. Semantic ranker vs a custom reranking model in Azure AI Search — when do you build your own?
  200    · Q17. What's a sound disaster-recovery strategy for an Azure AI Search index?
  212    · Q18. How would you architect for a future Azure AI service deprecation or breaking API change beyond model versions (e.g., an SDK major version bump)?
```

### Module 3 — RAG Architecture
`02_Questions/InterviewBank/03_RAG_Architecture.md` · 224 lines · 19 topics

```
    8    · Q1. Fixed-size vs semantic vs recursive chunking — how do you choose?
   20    · Q2. How do you choose chunk size and overlap?
   32    · Q3. How do you pick an embedding model, and what's the actual trade-off in dimensionality?
   44    · Q4. Design a hybrid search + reranking pipeline end to end.
   56    · Q5. How do you force a generative model to cite its sources and stay grounded?
   68    · Q6. What is groundedness detection, and how does it differ from Content Safety?
   80    · Q7. RAG answer is wrong despite grounded documents existing in the index — diagnose the two most likely causes.
   92    · Q8. When does GraphRAG outperform standard vector RAG?
  104    · Q9. CAG (Cache-Augmented Generation) vs RAG — what's the actual trade-off?
  116    · Q10. What is multi-hop / agentic RAG, and when is single-pass retrieval insufficient?
  128    · Q11. How do you handle structured data (tables, forms) in a RAG pipeline that's otherwise built for prose?
  140    · Q12. How do you keep a RAG index fresh as source documents change, without over-engineering it?
  152    · Q13. How do you evaluate RAG quality — what actually gets measured?
  164    · Q14. What is query rewriting/expansion, and why would you add it before retrieval?
  176    · Q15. How do you handle multi-tenant access control in retrieval — preventing Tenant A's query from surfacing Tenant B's documents?
  188    · Q16. Do you chunk a 3-page document and a 300-page document the same way?
  200    · Q17. Where does RAG cost actually accumulate, and what's the highest-leverage optimization?
  212    · Q18. Walk through your end-to-end RAG architecture as if this were the "centerpiece" interview question.
```

### Module 4 — Agent Orchestration
`02_Questions/InterviewBank/04_Agent_Orchestration.md` · 230 lines · 22 topics

```
    8  ▸ Tool-Calling & Planning
   10    · Q1. How does function/tool calling actually work under the hood?
   22    · Q2. What is the ReAct pattern, and why does it outperform a single-shot tool call for complex tasks?
   34    · Q3. What problem do Semantic Kernel Planners solve that direct tool-calling doesn't?
   46    · Q4. How do you validate/sanitize tool-call arguments the model generates?
   58    · Q5. How do you design tool-call failure handling and retries?
   70    · Q6. Single large multi-purpose tool vs many granular single-purpose tools — how do you decide tool granularity?
   82  ▸ Multi-Agent Coordination
   84    · Q7. When do you actually need multiple agents instead of one agent with more tools?
   96    · Q8. Design a supervisor/orchestrator pattern for a multi-agent system.
  108    · Q9. What does the A2A (Agent-to-Agent) protocol standardize, and why does it matter architecturally?
  120    · Q10. What is failure propagation in meta-agent hierarchies, and how do you contain it?
  132    · Q11. What is an MCP Hub, and how does MCP relate to (or differ from) APIM as a gateway pattern?
  144    · Q12. Design the agent-to-agent communication bus pattern (message passing) for a multi-agent system.
  156  ▸ Agent Memory
  158    · Q13. Differentiate short-term/session memory from long-term memory in an agent system.
  170    · Q14. How is vector-backed long-term agent memory actually implemented?
  182    · Q15. When and how do you summarize/compact conversation history instead of letting it grow unbounded?
  194    · Q16. How do you scope memory in a multi-agent system — shared vs private memory per agent?
  206    · Q17. How do you handle memory staleness/invalidation in a long-lived agent system?
  218    · Q18. What is agentic hallucination, and how is it distinct from the factual/RAG hallucination covered in Modules 1 and 3?
```

### Module 5 — Solution & Deployment Architecture
`02_Questions/InterviewBank/05_Solution_Architecture.md` · 442 lines · 43 topics

```
    8  ▸ 5a. Local / Dev Deployment (5)
   10    · Q1. What does "local deployment" actually mean for a GenAI pipeline, given Azure OpenAI can't be containerized?
   22    · Q2. How do you isolate dev/test from production to avoid cross-contamination?
   34    · Q3. What's a cost-zero (or near-zero) iteration strategy for GenAI development?
   46    · Q4. How do you handle local/lightweight search-index development without standing up full production-scale AI Search?
   58    · Q5. How do you test a GenAI pipeline in CI without making live, costly Azure AI calls on every commit?
   70  ▸ 5b. Single-Region Production (6)
   72    · Q6. How do you design for high availability within a single region for a GenAI pipeline?
   84    · Q7. How do you load-balance across multiple Azure OpenAI deployments within a region?
   96    · Q8. How do you design quota budget across multiple internal services calling one shared Azure OpenAI deployment?
  108    · Q9. How do you roll out a new model version or prompt change safely in production?
  120    · Q10. What does disaster recovery look like within a single region (before considering multi-region)?
  132    · Q11. How do you size PTU/quota capacity for expected single-region production load?
  144  ▸ 5c. Multi-Region — Active-Passive / Active-Active (7)
  146    · Q12. Active-passive vs active-active multi-region — how do you decide?
  158    · Q13. How do you keep a vector search index in sync across regions?
  170    · Q14. How does failover traffic cutover actually work mechanically?
  182    · Q15. How do RPO and RTO requirements drive the active-passive vs active-active decision?
  194    · Q16. What is split-brain risk in an active-active design, and how do you avoid it?
  206    · Q17. How do you actually test multi-region failover before you need it for real?
  218    · Q18. How do you justify the cost of multi-region to leadership against the risk of not having it?
  230  ▸ 5d. Global Scale-Out (6)
  232    · Q19. How do you route users to the nearest/best-performing region globally?
  244    · Q20. How do you architect around data residency and sovereignty constraints in a global deployment?
  256    · Q21. Why can't GenAI inference be "edge cached" the way static assets can via a CDN?
  268    · Q22. What does the true cost model look like for a global, multi-region deployment?
  280    · Q23. How do you handle a new model version being available in some regions before others?
  292    · Q24. How do you design global load balancing that accounts for both latency and AI-specific quality/SLA differences across regions?
  304  ▸ 5e. Caching Strategy (5)
  306    · Q25. What does prompt caching actually cache, and when does it help?
  318    · Q26. How does semantic caching differ from exact-match response caching?
  330    · Q27. When would you deploy CAG (Module 3 Q9) specifically as a deployment-tier caching strategy, not just a RAG alternative?
  342    · Q28. What makes cache invalidation the hardest part of caching in a GenAI pipeline specifically?
  354    · Q29. How do you maintain cache consistency across regions in a multi-region deployment?
  366  ▸ 5f. Pricing & Cost-Optimization Best Practices (4)
  368    · Q30. PTU vs PAYG as a solution-architecture decision — what's the commitment-risk trade-off?
  380    · Q31. How do you measure the true cost per user interaction, not just per API call?
  392    · Q32. Design a model-tiering/routing architecture pattern — cheap model for triage, expensive model for complex cases.
  404    · Q33. How do you design cost attribution/chargeback for a shared platform serving multiple internal teams or tenants?
  416  ▸ 5g. Multi-Tenant & Cost/Security Trade-offs (2)
  418    · Q34. Shared capacity vs dedicated capacity per tenant — how do you decide the isolation model?
  430    · Q35. Synthesizing the whole module — where does the security boundary sit at each deployment tier, and how does it change as you climb the ladder?
```

### Module 6 — Responsible AI, LLMOps & Governance
`02_Questions/InterviewBank/06_Responsible_AI_LLMOps.md` · 284 lines · 28 topics

```
    9  ▸ 6a. Content Safety (4)
   11    · Q1. Walk through Azure AI Content Safety's category/severity model and how you'd configure it for a production endpoint.
   23    · Q2. What are Prompt Shields, and what attack classes do they cover?
   35    · Q3. Groundedness detection as a safety control — where does it sit in the production request path?
   47    · Q4. How do you make the safety layer itself observable — proving it's working, not just present?
   59  ▸ 6b. Prompt Injection & Security (4)
   61    · Q5. Design a layered defense against prompt injection — no single control is sufficient. What are the layers?
   73    · Q6. How does prompt injection change when the system is an *agent with tools* rather than a chat endpoint?
   85    · Q7. How do you red-team a GenAI system before launch, and what does "passing" look like?
   98    · Q8. What PII controls does a GenAI pipeline need — at ingestion, inference, and logging?
  110  ▸ 6c. Evaluation & Drift (4)
  112    · Q9. What is a golden dataset, how do you build one, and how do you keep it from going stale?
  124    · Q10. LLM-as-judge evaluation — when do you trust it, and how do you validate the judge?
  136    · Q11. What kinds of drift affect a production GenAI system, and how do you detect each?
  148    · Q12. Design the continuous evaluation architecture for a production GenAI platform — offline and online together.
  160  ▸ 6d. CI/CD for LLMOps (4)
  162    · Q13. Prompts as deployable artifacts — what does prompt versioning actually require?
  174    · Q14. What gates belong in a GenAI CI/CD pipeline that a traditional pipeline doesn't have?
  186    · Q15. Design the model rollback story — what has to be true for rollback to actually work?
  198    · Q16. How do canary/blue-green patterns change for GenAI versus traditional services?
  210  ▸ 6e. AI Governance (6)
  212    · Q17. Design a model approval workflow for an enterprise — what gets reviewed before a model/use case ships?
  224    · Q18. What must an audit trail capture for a consequential AI-assisted decision, and how does GenAI make this harder?
  236    · Q19. Map the EU AI Act's current state (mid-2026) to what a GenAI architect actually has to do.
  248    · Q20. Who is accountable when an AI system causes harm — how do you structure ownership so the answer isn't "nobody"?
  260    · Q21. What is an AI inventory/registry, and why is it the foundation the rest of governance stands on?
  272    · Q22. Synthesis — a healthcare client asks you to stand up AI governance from zero for their GenAI platform. Sequence the first 90 days.
```

### Q&A — L06: Azure Machine Learning
`02_Questions/PerChapter/QA_L06_AzureML.md` · 130 lines · 5 topics

```
    7  ▸ Workspace & Core Components
   49  ▸ AutoML
   94  ▸ Designer
  116  ▸ Deployment & Monitoring
```

### Q&A — L07: Azure AI Services Deep Dive
`02_Questions/PerChapter/QA_L07_AzureAIServices_DeepDive.md` · 140 lines · 6 topics

```
    7  ▸ Management, Auth & RBAC
   34  ▸ Networking & Monitoring
   56  ▸ Throttling
   69  ▸ Containers
   96  ▸ Custom Models & Training
```

### Q&A — L08: Document Intelligence
`02_Questions/PerChapter/QA_L08_DocumentIntelligence.md` · 119 lines · 6 topics

```
    7  ▸ Overview & Internals
   27  ▸ Prebuilt Models
   54  ▸ Custom Models
   81  ▸ Integration Patterns
  103  ▸ 2026 Updates & Edge Facts
```

### Q&A — L09: Azure AI Search
`02_Questions/PerChapter/QA_L09_AzureAISearch.md` · 137 lines · 7 topics

```
    7  ▸ Fundamentals & Index Schema
   36  ▸ Ingestion
   55  ▸ Skillsets & Enrichment
   74  ▸ Querying
   97  ▸ Vector Config & Semantic Ranker
  117  ▸ Index Ops & 2026 Updates
```

### Q&A — L10: Bot Development
`02_Questions/PerChapter/QA_L10_BotDevelopment.md` · 92 lines · 5 topics

```
    7  ▸ Fundamentals
   39  ▸ Cards & Dialogs
   55  ▸ AI Integration
   69  ▸ Deployment, Teams & Security
```

### Q&A — L11_1: LLMs — Attention & Transformer Architecture
`02_Questions/PerChapter/QA_L11_1_Attention_Transformer.md` · 114 lines · 6 topics

```
    7  ▸ Attention Mechanics
   35  ▸ Transformer Architecture
   66  ▸ Key Numbers & Architect Relevance
   82  ▸ 2026 Updates
  101  ▸ Applied (from the chapter's exercises, answered)
```

### Q&A — L11_2: LLMs — Tokenization & Embeddings
`02_Questions/PerChapter/QA_L11_2_Tokenization_Embeddings.md` · 115 lines · 4 topics

```
    7  ▸ Tokenization
   49  ▸ Context Windows
   78  ▸ Embeddings
```

### Q&A — L11_3: LLMs — Pre-training & Fine-tuning
`02_Questions/PerChapter/QA_L11_3_Pretraining_Finetuning.md` · 114 lines · 6 topics

```
    7  ▸ Pre-training
   27  ▸ Fine-tuning
   43  ▸ LoRA & QLoRA
   63  ▸ The Decision Framework
   86  ▸ Misconceptions & Updates
```

### Q&A — L11_4: LLMs — RLHF & Alignment
`02_Questions/PerChapter/QA_L11_4_RLHF_Alignment.md` · 112 lines · 7 topics

```
    7  ▸ RLHF in Depth
   29  ▸ Alignment
   48  ▸ Jailbreaking & Prompt Injection
   61  ▸ Azure Content Safety
   83  ▸ Responsible AI
   96  ▸ Misconceptions
```

### Q&A — L12: Azure OpenAI Service
`02_Questions/PerChapter/QA_L12_AzureOpenAI_Services.md` · 152 lines · 11 topics

```
    7  ▸ What It Is & Models
   20  ▸ Deployments, Endpoints, TPM
   33  ▸ Chat Completions API
   52  ▸ Streaming & System Prompts
   65  ▸ Embeddings API
   75  ▸ Function Calling
   94  ▸ RAG & "On Your Data"
  104  ▸ Auth, Security, Monitoring
  126  ▸ Resilience & Model Selection (Interview Gaps)
  139  ▸ 2026 Updates
```

### Q&A — L13: RAG Deep Dive
`02_Questions/PerChapter/QA_L13_RAG_DeepDive.md` · 139 lines · 8 topics

```
    7  ▸ Fundamentals
   26  ▸ Document Processing
   39  ▸ Chunking
   68  ▸ Vector Databases
   84  ▸ Retrieval Strategies
  110  ▸ Generation
  129  ▸ On Your Data, Advanced Patterns & 2026
```

### Q&A — L14: Fine-Tuning LLMs
`02_Questions/PerChapter/QA_L14_FineTuning.md` · 105 lines · 6 topics

```
    7  ▸ When to Fine-Tune
   23  ▸ Azure OpenAI Fine-Tuning
   51  ▸ Loss Curves & Overfitting
   64  ▸ LoRA / QLoRA (Practical)
   83  ▸ Misconceptions & 2026
```

### Q&A — L15: Prompt Engineering
`02_Questions/PerChapter/QA_L15_PromptEngineering.md` · 129 lines · 9 topics

```
    7  ▸ Why It Matters
   17  ▸ Core Patterns
   42  ▸ System Prompt Design
   55  ▸ Advanced Patterns
   68  ▸ Injection Defense
   84  ▸ Production Optimization
  100  ▸ 2026 Updates
  119  ▸ Applied
```

### Q&A — L16: AI Orchestration — Semantic Kernel, LangChain & Agents
`02_Questions/PerChapter/QA_L16_AIOrchestration_SK_Agents.md` · 141 lines · 9 topics

```
    7  ▸ Orchestration & Semantic Kernel
   38  ▸ LangChain (Awareness)
   48  ▸ AI Agents
   70  ▸ Agentic RAG
   80  ▸ AI Foundry & Prompt Flow
   93  ▸ Production Patterns
  109  ▸ Memory Management (Practical)
  128  ▸ Prompt Compression & Interview Gaps
```

### Q&A — L17: Azure AI Foundry
`02_Questions/PerChapter/QA_L17_AzureAIFoundry.md` · 121 lines · 7 topics

```
    7  ▸ What It Is
   23  ▸ Model Catalog
   39  ▸ Prompt Flow
   58  ▸ Evaluation
   77  ▸ Fine-Tuning & Content Safety in Foundry
   96  ▸ 2026 Updates & Applied
```

### Q&A — L18: AI Solution Architecture
`02_Questions/PerChapter/QA_L18_AISolutionArchitecture.md` · 121 lines · 7 topics

```
    7  ▸ Architecture Patterns
   26  ▸ Scalability & Performance
   42  ▸ Security
   61  ▸ Cost Management
   83  ▸ 2026 Updates
  102  ▸ Applied (Self-Test & Exercises)
```

### Q&A — L19: MLOps and LLMOps
`02_Questions/PerChapter/QA_L19_MLOps_LLMOps.md` · 129 lines · 9 topics

```
    7  ▸ MLOps vs LLMOps
   20  ▸ Model Versioning & Lifecycle
   33  ▸ CI/CD for AI
   49  ▸ Monitoring & Observability
   62  ▸ Drift Detection
   78  ▸ LLMOps — Prompts, Evaluation, A/B
   97  ▸ Final Summary & 2026
  116  ▸ Applied (Self-Test & Exercises)
```

### Q&A — L20: Integration Patterns
`02_Questions/PerChapter/QA_L20_IntegrationPatterns.md` · 113 lines · 5 topics

```
    7  ▸ Azure Integration Services for AI
   44  ▸ Microsoft 365 Integration
   69  ▸ Enterprise Data Integration
   88  ▸ Applied (Recall Quiz & Exercises)
```

### Q&A — L21: Python for AI
`02_Questions/PerChapter/QA_L21_Python_for_AI.md` · 107 lines · 5 topics

```
    7  ▸ Python Basics (C# Fast-Track)
   50  ▸ Azure OpenAI in Python
   72  ▸ Jupyter & LangChain
   94  ▸ Azure AI Services in Python & Cheat Sheet
```

## Supplementary


### 1.4 Fine-Tuning & Parameter-Efficient Methods
`06_Supplementary/PythonTrack/1.4-FineTuning.md` · 1013 lines · 48 topics

```
    8  ▸ Concept 1 — What Is Fine-Tuning and Why Do We Need It?
   10    · 1. Real-World Problem (Hook)
   26    · 2. Simple Concept Explanation
   40    · 3. Diagram
   69    · 4. Step-by-Step Breakdown
   99    · 5. Code Implementation
  155    · 6. Output Explanation
  169    · 7. Common Mistakes
  180    · 8. Mini Exercise
  192  ▸ Concept 2 — LoRA (Low-Rank Adaptation)
  194    · 1. Real-World Problem (Hook)
  208    · 2. Simple Concept Explanation
  225    · 3. Diagram
  250    · 4. Step-by-Step Breakdown
  275    · 5. Code Implementation
  317    · 6. Output Explanation
  340    · 7. Common Mistakes
  351    · 8. Mini Exercise
  363  ▸ Concept 3 — QLoRA (Quantized LoRA)
  365    · 1. Real-World Problem (Hook)
  375    · 2. Simple Concept Explanation
  396    · 3. Diagram
  418    · 4. Step-by-Step Breakdown
  445    · 5. Code Implementation
  508    · 6. Output Explanation
  526    · 7. Common Mistakes
  537    · 8. Mini Exercise
  549  ▸ Concept 4 — RLHF vs DPO (Alignment Training)
  551    · 1. Real-World Problem (Hook)
  563    · 2. Simple Concept Explanation
  581    · 3. Diagram
  612    · 4. Step-by-Step Breakdown
  646    · 5. Code Implementation
  712    · 6. Output Explanation
  727    · 7. Common Mistakes
  738    · 8. Mini Exercise
  752  ▸ Concept 5 — Decision Framework: What to Use When
  754    · 1. Real-World Problem (Hook)
  771    · 2. Simple Concept Explanation
  785    · 3. Diagram — Decision Tree
  820    · 4. Step-by-Step Breakdown
  851    · 5. Code — Decision Helper
  910    · 6. Output Explanation
  921    · 7. Common Mistakes
  932    · 8. Mini Exercise
  945  ▸ 9. Mini Project — End of Chapter
 1000  ▸ Chapter Summary
```

### 1.5 AI Agents & Function Calling
`06_Supplementary/PythonTrack/1.5-AIAgents.md` · 1981 lines · 66 topics

```
    8  ▸ Concept 1 — What Is an AI Agent and Why Do We Need It?
   10    · 1. Real-World Problem (Hook)
   28    · 2. Simple Concept Explanation
   48    · 3. Diagram
   88    · 4. Step-by-Step Breakdown
  117    · 5. Code — Minimal Agent Loop (No framework)
  230    · 6. Output Explanation
  263    · 7. Common Mistakes
  274    · 8. Mini Exercise
  284  ▸ Concept 2 — ReAct Pattern (Reason + Act)
  286    · 1. Real-World Problem (Hook)
  310    · 2. Simple Concept Explanation
  332    · 3. Diagram
  385    · 4. Step-by-Step Breakdown
  414    · 5. Code Implementation
  515    · 6. Output Explanation
  540    · 7. Common Mistakes
  551    · 8. Mini Exercise
  561  ▸ Concept 3 — Function Calling (How Tools Actually Work)
  563    · 1. Real-World Problem (Hook)
  578    · 2. Simple Concept Explanation
  593    · 3. Diagram
  639    · 4. Step-by-Step Breakdown
  669    · 5. Code Implementation
  817    · 6. Output Explanation
  846    · 7. Common Mistakes
  857    · 8. Mini Exercise
  867  ▸ Concept 4 — Agent Memory Types
  869    · 1. Real-World Problem (Hook)
  883    · 2. Simple Concept Explanation
  902    · 3. Diagram
  935    · 4. Step-by-Step Breakdown
  967    · 5. Code Implementation
 1078    · 6. Output Explanation
 1100    · 7. Common Mistakes
 1111    · 8. Mini Exercise
 1121  ▸ Concept 5 — Multi-Agent Systems
 1123    · 1. Real-World Problem (Hook)
 1139    · 2. Simple Concept Explanation
 1156    · 3. Diagram
 1191    · 4. Step-by-Step Breakdown
 1218    · 5. Code Implementation
 1321    · 6. Output Explanation
 1354    · 7. Common Mistakes
 1365    · 8. Mini Exercise
 1375  ▸ Concept 6 — LangChain & LangGraph Basics
 1377    · 1. Real-World Problem (Hook)
 1391    · 2. Simple Concept Explanation
 1406    · 3. Diagram
 1452    · 4. Step-by-Step Breakdown
 1472    · 5. Code Implementation
 1578    · 6. Output Explanation
 1611    · 7. Common Mistakes
 1622    · 8. Mini Exercise
 1632  ▸ Concept 7 — Agent Evaluation
 1634    · 1. Real-World Problem (Hook)
 1648    · 2. Simple Concept Explanation
 1665    · 3. Diagram
 1694    · 4. Step-by-Step Breakdown
 1725    · 5. Code Implementation
 1867    · 6. Output Explanation
 1892    · 7. Common Mistakes
 1903    · 8. Mini Exercise
 1913  ▸ 9. Mini Project — End of Chapter
 1967  ▸ Chapter Summary
```

### AI/ML Curriculum – Missing Topics (Gap Fill)
`06_Supplementary/PythonTrack/AIMLcurriculum-gaps.md` · 110 lines · 11 topics

```
    7  ▸ Gap 1: SQL & Data Querying
   17  ▸ Gap 2: HuggingFace Ecosystem
   29  ▸ Gap 3: Experiment Tracking & Model Registry
   38  ▸ Gap 4: Model Serving & Inference Optimization
   51  ▸ Gap 5: NLP Fundamentals (Pre-Transformer)
   60  ▸ Gap 6: Generative AI Beyond LLMs
   70  ▸ Gap 7: Async Python & API Patterns
   79  ▸ Gap 8: Reinforcement Learning Basics
   89  ▸ Gap 9: Non-Azure Cloud (AWS & GCP)
   99  ▸ Gap 10: DSA for Coding Interviews
```

### Complete AI/ML Engineer Curriculum – Zero Gap
`06_Supplementary/PythonTrack/AIMLcurriculum.md` · 558 lines · 61 topics

```
    7  ▸ Table of Contents
   24  ▸ Part 0: Programming & Software Engineering for ML
   28    · 0.1 Python
   38    · 0.2 NumPy
   47    · 0.3 Pandas
   58    · 0.4 Matplotlib & Seaborn
   65    · 0.5 Scikit-learn API
   73    · 0.6 PyTorch
   86    · 0.7 Version Control (Git)
   93    · 0.8 CI/CD for ML
  100    · 0.9 Testing & Debugging
  109  ▸ Part 1: Artificial Intelligence (Applied AI / LLMs)
  113    · 1.1 Large Language Models – Architectures
  121    · 1.2 Prompt Engineering
  129    · 1.3 Retrieval-Augmented Generation (RAG)
  154    · 1.4 Fine-Tuning & Parameter-Efficient Methods
  163    · 1.5 AI Agents & Function Calling
  181    · 1.6 LLM Evaluation & Benchmarks
  195  ▸ Part 2: Machine Learning (Traditional & Deep Learning)
  199    · 2.1 Mathematics for Machine Learning
  220    · 2.2 Traditional Machine Learning
  239    · 2.3 Deep Learning (PyTorch Focus)
  271  ▸ Part 3: Azure AI Stack
  275    · 3.1 Azure OpenAI Service
  283    · 3.2 Azure AI Search
  292    · 3.3 Azure AI Document Intelligence
  298    · 3.4 Azure AI Language & Speech
  306    · 3.5 Azure AI Content Safety
  312    · 3.6 Semantic Kernel (.NET / Python)
  319    · 3.7 Azure Machine Learning
  327    · 3.8 LLMOps on Azure (Prompt Flow & Monitoring)
  337  ▸ Part 4: MLOps & Infrastructure
  341    · 4.1 Containerization
  347    · 4.2 Orchestration (Kubernetes & AKS)
  356    · 4.3 Infrastructure as Code (IaC)
  362    · 4.4 CI/CD for ML/AI
  368    · 4.5 Monitoring & Observability
  377  ▸ Part 5: Data Engineering for AI
  381    · 5.1 Data Storage
  388    · 5.2 Data Movement
  395    · 5.3 Data Transformation
  401    · 5.4 Feature Store (Concept)
  408  ▸ Part 6: Security & Responsible AI
  412    · 6.1 Security for AI Systems
  425    · 6.2 Responsible AI
  435  ▸ Part 7: System Design for AI
  439    · 7.1 RAG System Design (End-to-End)
  450    · 7.2 Multi-Agent System Design
  455    · 7.3 Batch vs Real-Time Inference
  464    · 7.4 High Availability & Disaster Recovery
  473  ▸ Part 8: Interview Preparation
  477    · 8.1 Common ML Theory Questions
  490    · 8.2 Coding Questions (Python + ML)
  499    · 8.3 Azure-Specific Scenarios
  509    · 8.4 System Design Whiteboarding
  517  ▸ Part 9: Study & Practice Guide
  519    · 9.1 Active Learning Techniques
  528    · 9.2 Mock Interview Plan
  537    · 9.3 Weekly Schedule (15 hrs/week)
  549    · 9.4 Success Metrics
```

### Part 1: Artificial Intelligence (Applied AI / LLMs)
`06_Supplementary/PythonTrack/Part1-AI-LLMs.md` · 1241 lines · 94 topics

```
    9  ▸ 1.1 Large Language Models – Architectures
   11    · Analogy
   22    · How It Works Internally
   24      - Step 1 — Tokenization
   37      - Step 2 — Embeddings
   44      - Step 3 — Self-Attention (The Core Idea)
   57      - Step 4 — Multi-Head Attention
   60      - Step 5 — Feed-Forward + Add & Norm
   63      - Step 6 — Causal Masking (GPT-style only)
   66      - GPT vs BERT vs T5 at a glance
   75      - Context Window
   80      - Mixture of Experts (MoE)
   87    · Code — PRIMM Exercise
   89      - PREDICT first: What do you think this code outputs?
  112      - RUN it → then look at the token count. Was your prediction close?
  114      - Now see tokenization directly:
  130      - MODIFY — Try these one at a time and observe:
  135      - MAKE — Your turn:
  146    · Interview Q&A
  170  ▸ 1.2 Prompt Engineering
  172    · Analogy
  180    · How It Works Internally
  182      - System vs User vs Assistant Roles
  192      - Zero-shot vs Few-shot vs Many-shot
  202      - Chain-of-Thought (CoT)
  212      - Tree-of-Thoughts (ToT)
  215      - Self-Consistency
  218      - Output Formatting
  225    · Code — PRIMM Exercise
  227      - PREDICT: Which prompt will give a better structured response?
  271      - Chain-of-Thought example:
  292      - JSON mode (structured output):
  309      - MODIFY — Try these:
  314      - MAKE — Build a reusable prompt builder:
  328    · Interview Q&A
  352  ▸ 1.3 Retrieval-Augmented Generation (RAG)
  354    · Analogy
  366    · How It Works Internally
  368      - Full Pipeline
  386      - Stage 1 — Chunking
  398      - Stage 2 — Embedding
  408      - Stage 3 — Vector Database
  421      - Stage 4 — Hybrid Search
  428      - Stage 5 — Reranking
  436      - Stage 6 — Generation with Grounding
  450      - Advanced RAG Patterns
  461    · Code — PRIMM Exercise
  463      - PREDICT: What will the retrieved chunks be for the query "What is the refund policy?"
  534      - MODIFY — Try these:
  539      - MAKE — Add a chunking function:
  565    · Interview Q&A
  589  ▸ 1.4 Fine-Tuning & Parameter-Efficient Methods
  591    · Analogy
  601    · How It Works Internally
  603      - Supervised Fine-Tuning (SFT)
  612      - LoRA (Low-Rank Adaptation)
  630      - QLoRA
  635      - Decision Framework: When to use what?
  645      - RLHF vs DPO
  663    · Code — PRIMM Exercise
  665      - PREDICT: What do `r`, `lora_alpha`, and `target_modules` control?
  699      - See which layers have LoRA adapters:
  707      - For production (QLoRA on a real LLM):
  738      - MODIFY — Try these:
  743      - MAKE — Write a function to compare trainable parameters:
  759    · Interview Q&A
  783  ▸ 1.5 AI Agents & Function Calling
  785    · Analogy
  798    · How It Works Internally
  800      - ReAct (Reason + Act) Pattern
  820      - Function Calling (OpenAI)
  833      - Agent Memory Types
  842      - Multi-Agent Patterns
  858    · Code — PRIMM Exercise
  860      - PREDICT: How many tool calls will the agent make for "What is 15% of 847, and what is the square root of that result?"
  967      - MODIFY — Try these:
  972      - MAKE — Build a simple memory-enabled agent:
  995    · Interview Q&A
 1019  ▸ 1.6 LLM Evaluation & Benchmarks
 1021    · Analogy
 1033    · How It Works Internally
 1035      - Offline Evaluation (before deployment)
 1051      - Benchmarks (standardized test suites)
 1064      - RAG-Specific Evaluation (RAGAS)
 1075      - Production Metrics (post-deployment)
 1088    · Code — PRIMM Exercise
 1090      - PREDICT: Which response will score higher on faithfulness?
 1142      - Simple BLEU score (no API needed):
 1161      - MODIFY — Try these:
 1166      - MAKE — Build a mini evaluation pipeline:
 1195    · Interview Q&A
 1219  ▸ Summary — Part 1 at a Glance
 1232  ▸ What's Next
```

## Assessments


### Enterprise AI Lead Assessment — Healthcare Edition (Cloud-Agnostic)
`05_Assessments/Assessment_Breakdown.md` · 276 lines · 28 topics

```
    2  ▸ Full Breakdown by Topic
    6  ▸ Assessment at a Glance
   19  ▸ The Scenario in Plain English
   29  ▸ Evaluation Scorecard
   44  ▸ Part 1 — Executive Summary (10% weight)
   64  ▸ Part 2 — Enterprise Architecture Design (20% weight)
   66    · A. Business Architecture
   73    · B. Logical Architecture Diagram (cloud-agnostic)
   80    · C. Cloud-Agnostic Deployment Architecture
   96  ▸ Part 3 — AI/GenAI Platform Strategy (20% weight)
   98    · A. RAG Design (Retrieval Augmented Generation)
  106    · B. Model Strategy
  114    · C. Responsible AI & Clinical Safety
  125    · D. Human-in-the-Loop Design
  134  ▸ Part 4 — Scalability & Operational Excellence (10% weight)
  136    · Scalability
  142    · Reliability
  150    · Cost Optimization
  160  ▸ Part 5 — Security & Compliance (15% weight)
  177  ▸ Part 6 — Delivery & Implementation Strategy (5% weight)
  179    · Phased Roadmap
  188    · Risk Register — Categories & Examples
  201  ▸ Critical Thinking Questions — 10 Mandatory (10% weight)
  220  ▸ Platform Capability Stack — Must Cover All Areas
  250  ▸ Deliverables
  263  ▸ Win Conditions
  271  ▸ Lose Conditions
```

### Enterprise AI Lead Assessment — Healthcare Edition (Cloud-Agnostic)
`05_Assessments/VitalCare_AI_Assessment_Response.md` · 1562 lines · 111 topics

```
    2  ▸ Assessment Response: VitalCare Health Global — Enterprise AI Clinical & Member Engagement Platform
   10  ▸ Table of Contents
   22  Part 1 — Executive Summary
   28  ▸ 1. Business Context
   36  ▸ 2. Core Pain Points
   50  ▸ 3. AI Opportunity Areas — With Clinical Safety Framing
   65  ▸ 4. Unstated Assumptions — Identified
   75  ▸ 5. Governance Gaps — Relative to HIPAA/HITECH and Emerging AI Regulation
   87  ▸ 6. Business Risks
   96  ▸ 7. Strategic Tradeoffs
  109  ▸ 8. Expected KPIs & Business Outcomes
  125  ▸ 9. Transformation Roadmap
  136  ▸ 10. Cloud-Portability Justification
  144  Part 2 — Enterprise Architecture Design
  148  ▸ A. Business Architecture
  150    · Business Domains
  167    · User Personas
  181    · AI Touchpoints Across the Patient & Member Journey
  210    · Business Workflows
  259    · Governance Layers
  290    · Operational Boundaries
  314  ▸ B. Logical Architecture Diagram
  373  ▸ C. Cloud-Agnostic Deployment Architecture
  377    · Identity & Access
  386    · Networking
  395    · AI Services
  405    · Data Services
  416    · Streaming
  424    · Compute
  433    · Monitoring & Observability
  442    · CI/CD & GitOps
  452    · Disaster Recovery
  471    · Multi-Region Deployment
  498  Part 3 — AI/GenAI Platform Strategy
  502  ▸ A. RAG (Retrieval Augmented Generation) Design
  504    · Chunking Strategy
  513    · Embedding Strategy
  534    · Retrieval Design
  558    · Grounding Methodology
  578    · Hallucination Mitigation
  592  ▸ B. Model Strategy
  594    · Model Selection Criteria (applied in order)
  604    · Model Portfolio
  631    · Model Routing Strategy
  654    · Fine-tuning vs RAG vs Prompt Engineering
  665    · Latency Optimizations
  674  ▸ C. Responsible AI & Clinical Safety
  676    · Input Guardrails
  685    · Output Guardrails
  695    · Clinical-Specific Safety Rules
  706    · Bias Monitoring
  719  ▸ D. Human-in-the-Loop Design
  721    · Escalation Flows
  743    · Audit Trail Design
  773    · Clinical Review Process
  781  Part 4 — Scalability & Operational Excellence
  785  ▸ Scalability Strategy
  787    · Peak Traffic Patterns
  796    · Handling Strategies
  821  ▸ Reliability Strategy
  823    · Uptime Targets
  831    · High Availability
  839    · Graceful Degradation Chains
  865    · RPO / RTO per Data Class
  878  ▸ Cost Optimization Strategy
  880    · Top 5 Cost Drivers
  888    · Token Optimization
  895    · Caching Strategy
  904    · GPU Utilization
  911    · FinOps Governance
  920  Part 5 — Security & Compliance
  924  ▸ Regulatory Alignment
  939  ▸ Data Residency
  951  ▸ PHI Protection
  975  ▸ Encryption
  987  ▸ Secrets Management
  998  ▸ Network Security
 1009  ▸ Access Control
 1032  ▸ AI Governance
 1042  ▸ Supply Chain Security
 1052  ▸ Prompt & Output Security
 1062  Part 6 — Delivery & Implementation Strategy
 1066  ▸ Phased Roadmap
 1068    · Phase 1 — MVP (Months 1–6)
 1091    · Phase 2 — Pilot (Months 7–12)
 1111    · Phase 3 — Enterprise Rollout (Months 13–24)
 1127    · Phase 4 — Stabilization (Months 25–30)
 1141  ▸ Risk Register
 1143    · Technical Risks
 1153    · Operational Risks
 1162    · Compliance Risks
 1171    · AI Governance Risks
 1180    · Adoption Risks
 1188    · Vendor Lock-in Risk
 1200  Critical Thinking Questions
 1204  ▸ Q1: When should a managed frontier-model API NOT be used?
 1232  ▸ Q2: Kubernetes vs Serverless for clinical workloads
 1255  ▸ Q3: Vector search failure modes and clinical risk
 1289  ▸ Q4: Detecting and monitoring hallucinations in production
 1312  ▸ Q5: Four data architecture tradeoffs
 1354  ▸ Q6: Five failure scenario impacts
 1398  ▸ Q7: Redesign if centralized PHI storage becomes legally restricted
 1420  ▸ Q8: Top 5 hidden operational costs in healthcare GenAI
 1439  ▸ Q9: When is agentic AI appropriate?
 1481  ▸ Q10: Rollback strategy for a failed GenAI deployment
 1485    · Technical Rollback Components
 1502    · Downstream Side Effects — The Hard Part
 1516    · Rollback Decision Authority
 1525    · Post-Rollback Process
 1534  ▸ Appendix — Platform Capability Stack
```

---

**Total: 2313 topics indexed.**