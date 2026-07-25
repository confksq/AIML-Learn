# Complete AI/ML Engineer Curriculum – Zero Gap

> A structured, end-to-end roadmap for becoming a production-ready AI/ML engineer with Azure expertise.

---

## Table of Contents

| Phase | Focus Area | Parts |
|-------|-----------|-------|
| **Foundation** | Programming, Tools & Engineering | [Part 0](#part-0-programming--software-engineering-for-ml) |
| **Core AI** | LLMs, RAG, Agents, Evaluation | [Part 1](#part-1-artificial-intelligence-applied-ai--llms) |
| **Core ML** | Math, Classical ML, Deep Learning | [Part 2](#part-2-machine-learning-traditional--deep-learning) |
| **Azure Stack** | Azure AI Services & LLMOps | [Part 3](#part-3-azure-ai-stack) |
| **Infrastructure** | MLOps, Containers, IaC | [Part 4](#part-4-mlops--infrastructure) |
| **Data** | Storage, Pipelines, Feature Stores | [Part 5](#part-5-data-engineering-for-ai) |
| **Safety** | Security & Responsible AI | [Part 6](#part-6-security--responsible-ai) |
| **Architecture** | System Design Patterns | [Part 7](#part-7-system-design-for-ai) |
| **Interview** | Theory, Coding, Scenarios | [Part 8](#part-8-interview-preparation) |
| **Practice** | Study Plan & Success Metrics | [Part 9](#part-9-study--practice-guide) |

---

## Part 0: Programming & Software Engineering for ML

> **Goal:** Build the engineering foundation needed to implement, test, and ship ML systems.

### 0.1 Python

- **Core syntax** – data types, loops, conditionals, functions
- List/dict comprehensions, generators
- Decorators, context managers
- **Object-oriented programming** – classes, inheritance, polymorphism, magic methods (`__init__`, `__call__`, `__repr__`)
- **Data structures & algorithms** – lists, tuples, sets, dicts, queues, stacks; Big-O notation
- **Error handling** – try/except/finally, custom exceptions
- **File I/O** – text, JSON, CSV

### 0.2 NumPy

- Array creation (`zeros`, `ones`, `arange`, `linspace`, random)
- Indexing, slicing, boolean masking
- Broadcasting rules
- Vectorized operations (universal functions)
- Linear algebra (`dot`, `matmul`, `linalg.eig`, `linalg.svd`)
- Random module (`randn`, `randint`, `seed`)

### 0.3 Pandas

- Series and DataFrame – creation from dict, list, NumPy
- Indexing: `loc`, `iloc`, boolean indexing
- Data cleaning: `dropna`, `fillna`, `replace`, `drop_duplicates`
- Merging/joining: `merge`, `concat`, `join`
- Grouping & aggregation: `groupby`, `agg`, `pivot_table`
- Apply functions: `apply`, `map`, `applymap`
- Time series: `to_datetime`, `resample`, `shift`
- I/O: `read_csv`, `to_csv`, `read_parquet`, `to_parquet`

### 0.4 Matplotlib & Seaborn

- Basic plots: line, scatter, bar, histogram, boxplot
- Subplots, figure/axes customization
- Heatmaps, pairplots (Seaborn)
- Saving figures (`savefig`)

### 0.5 Scikit-learn API

- Estimator interface: `fit`, `predict`, `transform`, `fit_transform`
- Model selection: `train_test_split`, `cross_val_score`, `GridSearchCV`
- Metrics: accuracy, precision, recall, F1, ROC-AUC, MSE
- Pipelines: `Pipeline`, `ColumnTransformer`
- Preprocessing: `StandardScaler`, `MinMaxScaler`, `OneHotEncoder`, `LabelEncoder`

### 0.6 PyTorch

- **Tensors** – creation, operations, device management (CPU/GPU), `requires_grad`
- **Autograd** – `backward()`, gradient accumulation, `detach()`, `no_grad()`
- **`torch.nn`** – `Module`, `Linear`, `Conv2d`, `LSTM`, `Embedding`
- **Loss functions** – `MSELoss`, `CrossEntropyLoss`, `BCEWithLogitsLoss`
- **Optimizers** – `SGD`, `Adam`, `AdamW`, `lr_scheduler`
- **Dataset & DataLoader** – custom `Dataset`, transforms, collation
- **Training loop** – forward, backward, step, zero_grad, checkpointing
- **Transfer learning** – loading pretrained models (ResNet, BERT), freezing layers
- **Mixed precision** – `torch.cuda.amp`, `GradScaler`
- **Distributed training** – `DistributedDataParallel` (concept)

### 0.7 Version Control (Git)

- `init`, `add`, `commit`, `push`, `pull`, `clone`
- Branching: `branch`, `checkout`, `merge`, `rebase`
- Resolving conflicts, pull requests, code review
- `.gitignore` best practices

### 0.8 CI/CD for ML

- **GitHub Actions** – YAML workflows, triggers, jobs, steps
- **Azure DevOps** – YAML pipelines, classic release pipelines
- Running tests (`pytest`), linting (`flake8`, `black`), security scans (`bandit`, `trivy`)
- Automated model deployment (Prompt Flow, Azure CLI)

### 0.9 Testing & Debugging

- **Unit testing** – `pytest` (fixtures, parametrize), `unittest`
- **Logging** – `logging` module, Application Insights
- **Debugging** – `pdb`, `breakpoint()`, VS Code debugger, `ipdb`
- **Profiling** – `cProfile`, `line_profiler`, PyTorch profiler

---

## Part 1: Artificial Intelligence (Applied AI / LLMs)

> **Goal:** Understand and apply modern LLM capabilities — RAG, agents, fine-tuning, and evaluation.

### 1.1 Large Language Models – Architectures

- GPT family (GPT-3, GPT-4, GPT-4o, GPT-4-Turbo) – causal language modeling
- Open-source models: Llama 2/3, Mistral, Phi, Gemma
- Mixture of Experts (MoE) – concept and trade-offs
- Context window extension: RoPE, ALiBi, sliding window attention
- Tokenization: BPE, WordPiece, SentencePiece; token budget calculation

### 1.2 Prompt Engineering

- System prompts, user prompts, few-shot learning
- Chain-of-Thought (CoT), Tree-of-Thoughts (ToT), self-consistency
- Prompt chaining, output formatting (JSON mode, regex, logit bias)
- Zero-shot vs few-shot vs many-shot
- Meta-prompting, automatic prompt optimization

### 1.3 Retrieval-Augmented Generation (RAG)

**Indexing Pipeline**
- Chunking: fixed-size (overlap), semantic, paragraph, recursive
- Embedding models: text-embedding-3 (small/large), ada-002, Cohere, Voyage
- Vector DB internals: HNSW, IVF, PQ, scalar quantization
- Metadata filtering, hybrid scoring (BM25 + vector)

**Retrieval**
- Hybrid search (keyword + vector)
- Reranking with cross-encoders (Cohere rerank, BGE-reranker)
- Multi-stage retrieval (retrieve → rerank → filter)

**Generation**
- Context stuffing, prompt compression (LLMLingua)
- Groundedness checking, citation generation

**Advanced RAG**
- Self-RAG, Corrective RAG (CRAG), Adaptive RAG
- Iterative retrieval, multi-hop retrieval

**RAG Evaluation**
- Retrieval: hit rate, MRR, NDCG
- Generation: faithfulness, answer relevance, context relevance (RAGAS)

### 1.4 Fine-Tuning & Parameter-Efficient Methods

- Supervised Fine-Tuning (SFT) – data prep, instruction formatting
- LoRA / QLoRA – rank decomposition theory, `peft` library practice
- Prefix tuning, adapter layers, IA3
- RLHF (overview): reward model, PPO
- Direct Preference Optimization (DPO)
- Decision framework: **RAG vs fine-tuning vs prompt engineering**

### 1.5 AI Agents & Function Calling

**Agent Architectures**
- ReAct (Reason + Act) – thought, action, observation loop
- Plan-and-execute, LLM-compiler
- Multi-agent: supervisor, handoff, group chat

**Tool Use & Function Calling**
- OpenAI function calling (tools schema, parallel calls)
- Semantic Kernel plugins & planners (.NET / Python)
- LangChain / LangGraph basics

**Memory**
- Short-term buffer, long-term vector memory, entity memory, summary memory

**Agent Evaluation**
- Success rate, steps to completion, tool selection accuracy

### 1.6 LLM Evaluation & Benchmarks

**Offline Evaluation**
- Perplexity, BLEU, ROUGE, METEOR
- LLM-as-a-judge (GPT-4 eval), G-Eval, Prometheus

**Benchmarks**
- MMLU, HumanEval, MBPP, HELM, TruthfulQA, GSM8K

**Production Metrics**
- Latency (TTFT, TPOT), token cost, groundedness, coherence, safety violation rate

---

## Part 2: Machine Learning (Traditional & Deep Learning)

> **Goal:** Build rigorous understanding of the math and algorithms that power modern AI.

### 2.1 Mathematics for Machine Learning

**Linear Algebra**
- Vectors: dot product, cosine similarity, norms (L1, L2, L∞), linear independence, basis
- Matrices: multiplication, transpose, inverse, determinant, rank, trace
- Eigenvalues & eigenvectors (PCA, attention)
- SVD (Singular Value Decomposition)

**Calculus**
- Partial derivatives, gradient, Jacobian
- Chain rule
- Optimization: convex vs non-convex, gradient descent (SGD, Adam, RMSprop), LR schedules
- Backpropagation: computational graphs, vanishing/exploding gradients

**Probability & Statistics**
- Conditional probability, Bayes' theorem, random variables
- Distributions: Gaussian, Bernoulli, Binomial, Poisson, Exponential
- Descriptive stats: mean, median, variance, covariance, correlation
- Inferential: hypothesis testing, p-value, confidence interval, MLE, bias-variance tradeoff
- Bayesian basics: prior, posterior, MAP

### 2.2 Traditional Machine Learning

**Supervised Learning**
- Regression: Linear, Ridge, Lasso, ElasticNet; evaluation (MSE, MAE, R²)
- Classification: Logistic Regression, Decision Trees (entropy, Gini), Random Forest, XGBoost/LightGBM/CatBoost, SVM (kernels), KNN, Naive Bayes

**Unsupervised Learning**
- Clustering: K-means (Elbow, silhouette), hierarchical, DBSCAN
- Dimensionality reduction: PCA, t-SNE, UMAP

**Evaluation & Validation**
- Train/val/test splits, cross-validation (k-fold, stratified)
- Confusion matrix, precision, recall, F1, ROC-AUC, log loss
- Learning curves, bias-variance decomposition

**Feature Engineering**
- Scaling (Standard, MinMax, Robust), encoding (one-hot, label, target)
- Missing value imputation, feature selection (filter, wrapper, embedded)

### 2.3 Deep Learning (PyTorch Focus)

**Neural Network Basics**
- Perceptron, activation functions (ReLU, Sigmoid, Tanh, Swish, GELU)
- MLP, loss functions (MSE, cross-entropy), backpropagation (detailed)
- Regularization: Dropout, BatchNorm, LayerNorm, weight decay

**Training Optimization**
- Optimizers: SGD with momentum, Adam, AdamW, RMSprop
- LR warmup, cosine annealing, gradient clipping
- Initialization (Xavier, He)

**Convolutional Neural Networks (CNNs)**
- Convolution, pooling, stride, padding
- Architectures: LeNet, AlexNet, VGG, ResNet, Inception, EfficientNet
- Transfer learning, fine-tuning
- Applications: classification, detection (YOLO), segmentation (U-Net)

**Recurrent Neural Networks (RNNs)**
- Vanilla RNN (vanishing gradient problem)
- LSTM (gates, cell state), GRU
- Bidirectional, stacked RNN
- Applications: time series, sentiment, sequence generation

**Attention & Transformers**
- Self-attention (Q, K, V), multi-head attention
- Positional encoding (sinusoidal, learned)
- Transformer block: attention → add&norm → feed-forward → add&norm
- Encoder-only (BERT), decoder-only (GPT), encoder-decoder (T5)

---

## Part 3: Azure AI Stack

> **Goal:** Master the Azure services needed to build, deploy, and operate AI solutions at scale.

### 3.1 Azure OpenAI Service

- Model deployment: global vs regional, provisioned vs pay-as-you-go
- Quotas, rate limits, best practices
- Content filtering: severity levels, custom blocklists
- Fine-tuning on Azure OpenAI
- "Azure OpenAI on your data" (built-in RAG with AI Search)

### 3.2 Azure AI Search

- Index design: fields, analyzers, suggesters
- Vector search: HNSW, exhaustive KNN
- Hybrid search + semantic ranking (L30+ tier)
- Indexers (Blob, SQL, Cosmos DB, SharePoint)
- Skillsets: built-in cognitive skills, custom web API skills
- Synonyms, scoring profiles, filters

### 3.3 Azure AI Document Intelligence

- Pre-built models: Layout, General Document, Invoice, Receipt, ID, Tax (W-2, 1099)
- Custom extraction models (labeling, training)
- Model composition

### 3.4 Azure AI Language & Speech

- Text Analytics: NER, PII, sentiment, key phrases, language detection
- Conversational Language Understanding (CLU) – intents, entities
- Speech-to-text (real-time, batch), custom models
- Text-to-speech (neural voices, SSML)
- Translator (text, document)

### 3.5 Azure AI Content Safety

- Prompt shields (jailbreak, indirect injection)
- Groundedness detection
- Protected material, custom blocklists

### 3.6 Semantic Kernel (.NET / Python)

- Kernel, plugins (native, prompt functions)
- Planners (sequential, stepwise, function calling stepwise)
- Memory (volatile, vector store)
- Agent loops (function calling, multi-agent)

### 3.7 Azure Machine Learning

- Workspaces, compute instances/clusters, serverless Spark
- AutoML (classification, regression, time series)
- Model registry, environments
- Pipelines (Python SDK v2)
- Managed online endpoints, batch endpoints

### 3.8 LLMOps on Azure (Prompt Flow & Monitoring)

- Azure AI Foundry (AI Studio)
- Prompt Flow: flows (LLM, Python), variants, evaluation, deployment
- Model deployment (managed online, batch)
- Monitoring: Application Insights, cost dashboards, drift detection
- CI/CD for AI: version prompts, models, and code

---

## Part 4: MLOps & Infrastructure

> **Goal:** Operationalize AI systems with containers, orchestration, IaC, and observability.

### 4.1 Containerization

- Docker: Dockerfile (multi-stage), building, tagging
- Azure Container Registry (ACR)
- Docker Compose

### 4.2 Orchestration (Kubernetes & AKS)

- Kubernetes primitives: Pods, Deployments, Services, Ingress
- ConfigMaps, Secrets (with Key Vault CSI)
- Persistent Volumes (Azure Disk, Files)
- Helm charts
- KEDA (event-driven autoscaling)
- AKS specifics: Managed Identities, VNet, Azure Policy

### 4.3 Infrastructure as Code (IaC)

- ARM templates (JSON, nested)
- Bicep (Azure native)
- Terraform (azurerm provider, state management)

### 4.4 CI/CD for ML/AI

- Azure DevOps YAML pipelines (stages, environments)
- GitHub Actions (workflows, runners)
- ML steps: linting, testing, security scan, container build/push, model deployment

### 4.5 Monitoring & Observability

- Azure Monitor: metrics, alerts, action groups
- Log Analytics: KQL queries
- Application Insights: traces, requests, dependencies, exceptions
- Prometheus + Grafana (for AKS)

---

## Part 5: Data Engineering for AI

> **Goal:** Understand how to move, store, and transform data that feeds AI pipelines.

### 5.1 Data Storage

- Azure Blob Storage (tiers, lifecycle)
- Azure Data Lake Gen2 (hierarchical namespace, ACLs)
- Azure SQL, Cosmos DB (SQL, MongoDB, Table APIs)
- Delta Lake (ACID, time travel, vacuum)

### 5.2 Data Movement

- Azure Data Factory (pipelines, activities, triggers)
- Copy activity, data flows, integration runtime
- Event Hubs, Service Bus (streaming)
- Microsoft Graph API (SharePoint, mail)

### 5.3 Data Transformation

- Azure Synapse Analytics (dedicated/serverless SQL, Spark pools)
- Delta Lake operations: merge (upsert), optimize (Z-order)
- ELT vs ETL, incremental load (watermark, CDC)

### 5.4 Feature Store (Concept)

- Offline vs online feature stores
- Feature definition, materialization, serving (Azure ML Feature Store, Feast)

---

## Part 6: Security & Responsible AI

> **Goal:** Build trustworthy, compliant, and secure AI systems.

### 6.1 Security for AI Systems

**Identity & Access**
- Managed Identities, RBAC, Entra ID (app registrations, OAuth2)
- Key Vault: secrets, keys, certificates (access policies, soft-delete)

**Network**
- Private Endpoints, VNet integration, firewalls

**AI-Specific Threats & Defenses**
- Threats: prompt injection, indirect injection, data extraction, model inversion
- Defenses: Azure AI Content Safety, input sanitization, adversarial training

### 6.2 Responsible AI

- **Fairness** – disparate impact (80% rule), equality of odds; Fairlearn
- **Explainability** – SHAP, LIME, AzureML Interpretability
- **Transparency** – model cards, datasheets for datasets
- **Privacy** – differential privacy basics (ε, δ), DP-SGD, PII redaction
- **Regulations** – EU AI Act, NIST AI RMF

---

## Part 7: System Design for AI

> **Goal:** Design scalable, cost-effective, and resilient AI architectures.

### 7.1 RAG System Design (End-to-End)

```
Ingestion:  document → chunk → embed → index
Query:      retrieve → rerank → augment → generate
```

- Caching: semantic caching (embedding similarity)
- Cost optimization: token compression, model tier selection
- A/B testing: shadow deployments

### 7.2 Multi-Agent System Design

- Supervisor pattern, handoff, tool retrieval
- Memory management (shared vs isolated)

### 7.3 Batch vs Real-Time Inference

| Mode | Use Case | Tradeoff |
|------|----------|----------|
| Batch | High volume, async | Cost-efficient, higher latency |
| Real-time | Low latency required | Higher cost, always-on compute |

- Decision factors: latency, cost, freshness

### 7.4 High Availability & Disaster Recovery

- Multi-region deployment (active-active / active-passive)
- Geo-redundant storage (GRS, GZRS), failover groups
- Azure Front Door / Traffic Manager
- RPO and RTO definitions

---

## Part 8: Interview Preparation

> **Goal:** Demonstrate depth and clarity across theory, coding, and scenario-based questions.

### 8.1 Common ML Theory Questions

| Question | Key Concepts to Cover |
|----------|-----------------------|
| Bias-variance tradeoff | Underfitting vs overfitting, decomposition |
| Backpropagation step-by-step | Chain rule, computational graph |
| Why multi-head attention? | Parallel subspaces, diverse feature capture |
| RAG vs fine-tuning: when to use which | Freshness vs behavior, cost, data availability |
| Vanishing gradient solution | LSTM gates, ResNet skip connections |
| Batch norm vs layer norm | Training vs inference, sequence models |
| Cosine similarity vs Euclidean | Magnitude invariance, high-dim space behavior |
| LLM generation: temperature, top-k, top-p | Sampling strategies, creativity vs accuracy |

### 8.2 Coding Questions (Python + ML)

- Custom training loop in PyTorch
- Cosine similarity matrix (NumPy)
- Simple RAG retrieval with FAISS/NumPy
- Debugging: NaN loss, overfitting
- k-fold cross-validation from scratch
- Prompt chain example (extract entities → generate)

### 8.3 Azure-Specific Scenarios

| Scenario | Key Levers |
|----------|-----------|
| Irrelevant RAG retrieval | Chunk size, embedding model, hybrid weights, reranking |
| Reduce GPT-4 cost | Caching, cheaper model tier, prompt compression, batching |
| Secure healthcare Q&A | Private endpoints, PII redaction, RBAC, no data retention |
| Semantic Kernel tool failure | Retry, fallback, validation |
| Monitor LLM drift | Groundedness score, topic shift, cost anomalies |

### 8.4 System Design Whiteboarding

- Real-time chatbot for a bank (RAG + agent + safety)
- Batch document classification (Document Intelligence → GPT → SQL)
- Recommendation system with embeddings

---

## Part 9: Study & Practice Guide

### 9.1 Active Learning Techniques

| Technique | How to Apply |
|-----------|-------------|
| Explain aloud | Record yourself explaining a topic (e.g., "How does LoRA work?") |
| Write summaries | One paragraph per subtopic, in your own words |
| Code every example | Don't just read — run and modify the code |
| Teach someone | Explain to a peer or an imaginary audience |

### 9.2 Mock Interview Plan

| Weeks | Focus |
|-------|-------|
| 1–2 | Theory flashcards (100 cards) |
| 3–4 | Coding drills (30 min/day – PyTorch, NumPy, scikit-learn) |
| 5–6 | Azure scenario practice (use real Azure free tier) |
| 7–8 | Full mock interviews (peer or Pramp) |

### 9.3 Weekly Schedule (15 hrs/week)

| Day | Focus | Hours |
|-----|-------|-------|
| Monday | Math / ML theory | 2 |
| Tuesday | Coding (PyTorch or RAG) | 2 |
| Wednesday | Azure services hands-on | 2 |
| Thursday | System design whiteboarding | 2 |
| Friday | Interview practice (theory + coding) | 2 |
| Saturday | Full project (e.g., build a RAG pipeline) | 3 |
| Sunday | Review weak areas, update flashcards | 2 |

### 9.4 Success Metrics

- [ ] You can explain every numbered item in this curriculum without looking.
- [ ] You can build a minimal RAG + agent + deployment on Azure in under 4 hours.
- [ ] You can solve 2 out of 3 medium ML coding problems in 30 minutes.
- [ ] Mock interview feedback: *"clear, structured, deep knowledge."*

---

*End of Curriculum*
