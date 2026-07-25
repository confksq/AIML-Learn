# Module 1 — Introduction to AI
**Part 1: AI Fundamentals | AI Solutions Architect Curriculum**

---

## Why This Module Matters

You have already learned how LLMs work — transformers, tokens, embeddings, RAG, weights. That is the advanced end of AI. This module fills in the foundational vocabulary that everything else assumes:

- What AI actually is (and what it is not)
- How Machine Learning works conceptually — supervised, unsupervised, reinforcement
- What different AI workloads exist and which Azure service handles each
- Responsible AI — the principles every Azure AI architect must apply

**Module 6 (Azure ML) uses this vocabulary constantly.** Without it, terms like "classification model," "regression," "training dataset," "overfitting" will be unfamiliar.

---

**Running example (used throughout):**
> *JM Family wants to build AI systems that predict which dealers are likely to submit late invoices, detect anomalies in vehicle pricing, and answer employee questions about policy documents.*

---

## Topic 1.1 — What Is Artificial Intelligence?

---

### 1. The Simple Definition

AI is any system that performs tasks that normally require human intelligence:

- Recognizing a face in a photo
- Understanding a spoken question
- Predicting next month's sales
- Translating text between languages
- Playing chess better than any human

**Key point:** AI does not require consciousness or understanding. It requires the ability to produce intelligent-seeming outputs from inputs.

---

### 2. AI vs Traditional Software

Traditional software follows explicit rules a programmer wrote:

```
Traditional rule-based system:
  IF invoice_days_late > 30 THEN flag = "overdue"
  IF invoice_days_late > 60 THEN flag = "critical"

Problem: what about 45 days? What about partial payments?
         Every edge case requires a new rule.
         Rules become unmanageable at scale.
```

AI learns rules from data instead of being programmed with them:

```
AI/ML system:
  Show it 10,000 historical invoices
  Show it which ones became problems
  It learns the patterns itself
  Handles edge cases it was never explicitly taught
```

---

### 3. Types of AI

| Type | What it does | Example |
|---|---|---|
| **Narrow AI** | One specific task, very well | Chess engine, face recognition, GPT-4o |
| **General AI (AGI)** | Any intellectual task a human can do | Does not exist yet |
| **Super AI** | Surpasses all human intelligence | Theoretical |

**Everything available today is Narrow AI.** GPT-4o feels general but it is a very capable narrow system — it generates text. It cannot drive a car, perform surgery, or genuinely reason the way humans do.

---

### 4. How AI Relates to Machine Learning and Deep Learning

These terms are nested — each is a subset of the previous:

```
┌─────────────────────────────────────────────────────┐
│  ARTIFICIAL INTELLIGENCE                            │
│  (any system mimicking human intelligence)          │
│                                                     │
│  ┌───────────────────────────────────────────────┐  │
│  │  MACHINE LEARNING                             │  │
│  │  (systems that learn from data)               │  │
│  │                                               │  │
│  │  ┌─────────────────────────────────────────┐  │  │
│  │  │  DEEP LEARNING                          │  │  │
│  │  │  (ML using neural networks              │  │  │
│  │  │   with many layers)                     │  │  │
│  │  │                                         │  │  │
│  │  │  ┌───────────────────────────────────┐  │  │  │
│  │  │  │  GENERATIVE AI                    │  │  │  │
│  │  │  │  (deep learning that creates      │  │  │  │
│  │  │  │   new content — text, images)     │  │  │  │
│  │  │  └───────────────────────────────────┘  │  │  │
│  │  └─────────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────┘
```

GPT-4o, Claude, Gemini = Generative AI = Deep Learning = Machine Learning = AI.

---

### 5. Rule-Based vs Machine Learning vs Deep Learning

| | Rule-Based | Machine Learning | Deep Learning |
|---|---|---|---|
| **How** | Programmer writes rules | Algorithm learns rules from data | Neural network learns complex patterns |
| **Data needed** | None | Thousands of examples | Millions of examples |
| **Handles complexity** | Poor — rules explode | Good | Excellent |
| **Explainability** | Perfect — you wrote the rules | Moderate | Poor — black box |
| **Example** | Invoice overdue flag | Predict late invoice | GPT-4o, image recognition |

---

## Topic 1.2 — Understanding Machine Learning

---

### 1. What Machine Learning Actually Does

ML finds patterns in data and uses those patterns to make predictions on new data.

```
Training phase (learning):
  Historical data: 10,000 invoices + outcome (paid on time / late)
  Algorithm finds: which features predict lateness?
  Learns: dealers with >60 day terms + high volume = higher late risk

Inference phase (using what it learned):
  New invoice arrives for dealer JMF-ATL-001
  Model predicts: 78% chance of late submission
  System flags for follow-up
```

---

### 2. The Three Types of Machine Learning

---

#### Supervised Learning — Learning With Answers

You provide both the input data AND the correct answer. The model learns to map inputs to answers.

```
Training data format:
  Input (features)                          Answer (label)
  ─────────────────────────────────────────────────────────
  [dealer_volume=high, terms=60d, region=SE]  → late
  [dealer_volume=low,  terms=30d, region=NE]  → on_time
  [dealer_volume=high, terms=30d, region=SW]  → on_time
  ...10,000 rows...

Model learns the mapping:
  features → label
```

**Two main tasks in supervised learning:**

| Task | What it predicts | Output | JM Family Example |
|---|---|---|---|
| **Classification** | Which category? | A label/class | Late or On-time? Fraud or Legitimate? |
| **Regression** | How much / how many? | A number | Predicted invoice amount? Days until payment? |

**Classification example:**
```
Input:  dealer profile features
Output: "late" or "on_time"   ← one of predefined categories
```

**Regression example:**
```
Input:  dealer profile + vehicle type
Output: 47.3   ← a number (predicted days to payment)
```

---

#### Unsupervised Learning — Learning Without Answers

You provide only input data — no correct answers. The model finds structure on its own.

```
Training data format:
  Input (features only — no labels)
  ─────────────────────────────────────────────────────────
  [dealer_volume=high, terms=60d, region=SE, invoice_count=200]
  [dealer_volume=low,  terms=30d, region=NE, invoice_count=50]
  [dealer_volume=high, terms=45d, region=SW, invoice_count=180]
  ...10,000 rows, no labels...

Model discovers: these naturally group into 3 clusters
  Cluster A: high volume, long terms, Southeast
  Cluster B: low volume, short terms, Northeast
  Cluster C: medium volume, mixed terms
```

**Main tasks:**

| Task | What it does | JM Family Example |
|---|---|---|
| **Clustering** | Groups similar items | Group dealers by behavior patterns |
| **Anomaly Detection** | Finds outliers | Detect unusual invoice amounts |
| **Dimensionality Reduction** | Simplifies data | Compress 100 features to 10 key ones |

---

#### Reinforcement Learning — Learning By Trial and Error

An agent takes actions in an environment, receives rewards or penalties, learns to maximize reward over time.

```
Agent: pricing algorithm
Environment: dealer market
Action: set vehicle price
Reward: sale completed (+1), no sale (0), complaint (-1)

Over millions of iterations:
  Agent learns which prices maximize sales and satisfaction
```

**Used for:** game playing (chess, Go), robotics, autonomous vehicles, dynamic pricing. Less common in standard enterprise AI. Azure AutoML does not use RL.

---

### 3. The ML Workflow — How a Model Gets Built

```
Step 1: COLLECT DATA
  Historical invoices, dealer profiles, payment records
  More data = better model (usually)

Step 2: PREPARE DATA
  Clean: remove nulls, fix errors
  Transform: convert text to numbers, normalize scales
  Split: 80% training / 20% testing

Step 3: CHOOSE ALGORITHM
  Classification? → Logistic Regression, Random Forest, Neural Network
  Regression?     → Linear Regression, Gradient Boosting
  Clustering?     → K-Means, DBSCAN

Step 4: TRAIN
  Feed training data (80%) to algorithm
  Algorithm adjusts internal parameters (weights/coefficients)
  until it predicts training data well

Step 5: EVALUATE
  Test on unseen data (20%)
  Measure accuracy, precision, recall, F1 score
  If poor → go back to step 2 or 3

Step 6: DEPLOY
  Wrap model in an API
  New invoices flow in → predictions flow out
```

---

### 4. Training Data vs Validation Data vs Test Data

Splitting data into three parts prevents the model from "cheating":

```
Full dataset: 10,000 invoices
  │
  ├── Training set    (70%) — 7,000 invoices
  │   Model learns from this
  │
  ├── Validation set  (15%) — 1,500 invoices
  │   Tune model settings (hyperparameters) using this
  │   Used during training — not for final score
  │
  └── Test set        (15%) — 1,500 invoices
      Final evaluation ONLY
      Model never sees this during training
      True measure of real-world performance
```

**Why not just use one dataset?**
If you train and test on the same data, the model "memorizes" the answers — like studying the exact exam questions. It scores perfectly but fails on real data.

---

### 5. Overfitting and Underfitting

```
UNDERFITTING:
  Model too simple — misses the real pattern
  Training accuracy: 60%  Test accuracy: 59%
  Like a student who barely studied
  Fix: more complex model, more features

GOOD FIT:
  Model learned the real pattern
  Training accuracy: 92%  Test accuracy: 90%
  Small gap between training and test
  This is what you want

OVERFITTING:
  Model memorized training data — does not generalize
  Training accuracy: 99%  Test accuracy: 65%
  Large gap between training and test
  Like a student who memorized textbook but can't apply knowledge
  Fix: more training data, simpler model, regularization
```

---

### 6. Key ML Metrics

**For Classification:**

| Metric | What it measures | Formula |
|---|---|---|
| **Accuracy** | % of correct predictions overall | correct / total |
| **Precision** | Of predicted "late", how many actually were late? | true positives / (true + false positives) |
| **Recall** | Of all actual "late" invoices, how many did we catch? | true positives / (true positives + false negatives) |
| **F1 Score** | Balance of precision and recall | 2 × (precision × recall) / (precision + recall) |

**When to prioritize recall over precision (JM Family):**
- Late invoice detection: better to flag some on-time invoices (low precision) than miss actual late ones (low recall) — missing a late invoice costs money
- Fraud detection: same — flag more false positives than miss real fraud

**For Regression:**

| Metric | What it measures |
|---|---|
| **MAE** | Average absolute error (in same units as prediction) |
| **RMSE** | Root mean squared error (penalizes large errors more) |
| **R²** | How much variance the model explains (1.0 = perfect) |

---

### 7. Features and Labels

```
Feature = an input variable the model uses to make predictions

Label = the output the model is trying to predict

Example — predicting late invoice:
  Features (inputs):
    dealer_region          = "Southeast"
    dealer_invoice_volume  = 200
    payment_terms_days     = 60
    vehicle_type           = "Truck"
    invoice_amount         = 47000
    
  Label (output):
    payment_status = "late"   ← what the model predicts
```

**Feature engineering** — creating new features from existing ones:
```
Raw features:      invoice_date, payment_date
Engineered feature: days_to_payment = payment_date - invoice_date
                   (more predictive than raw dates)
```

---

## Topic 1.3 — AI Workloads and Considerations

*(Marked complete — brief recap)*

---

### Azure AI Workload Map

| Workload | What it does | Azure Service |
|---|---|---|
| **Computer Vision** | Understand images/video | Azure AI Vision |
| **Natural Language Processing** | Understand/generate text | Azure OpenAI, Language Service |
| **Speech** | Speech↔text, translation | Azure AI Speech |
| **Document Intelligence** | Extract data from documents | Azure Document Intelligence |
| **Knowledge Mining** | Search and surface insights | Azure AI Search |
| **Generative AI** | Generate text, images, code | Azure OpenAI |
| **Anomaly Detection** | Find outliers in time series | Azure Anomaly Detector |
| **Decision** | Recommendations, content moderation | Azure Content Safety |

---

## Topic 1.4 — Responsible AI Principles

---

### 1. Why Responsible AI Matters for an Architect

As an AI Solutions Architect at JM Family, you design systems that affect real people — dealers, employees, customers. A biased model or opaque decision can cause real harm and legal liability. Microsoft's Responsible AI principles are not just ethics — they are architecture requirements.

---

### 2. Microsoft's Six Responsible AI Principles

---

#### Fairness

AI systems should treat all people fairly — no bias based on gender, race, age, or other protected characteristics.

```
JM Family risk:
  Invoice late-payment prediction model trained on historical data
  If historically certain regions were flagged more (due to economic factors)
  Model learns: Southeast region = higher risk
  Could unfairly disadvantage Southeast dealers

Architect's job:
  Audit training data for demographic bias
  Test model performance across dealer regions, sizes
  Use Azure Fairlearn (open source fairness toolkit)
  Monitor predictions for disparate impact after deployment
```

---

#### Reliability and Safety

AI systems should perform reliably and safely — behave as designed, fail safely.

```
JM Family risk:
  RAG app gives wrong invoice information
  Dealer makes business decision based on hallucinated fact
  Financial or legal consequence

Architect's job:
  Confidence gates (score < 0.65 → don't answer)
  Citation requirements (every claim backed by source)
  Groundedness checks (Azure Content Safety)
  Fallback to human review when confidence is low
  Regular evaluation against ground truth test set
```

---

#### Privacy and Security

AI systems should protect personal and business data.

```
JM Family risks:
  Dealer financial data sent to OpenAI without consent
  Employee PII included in prompts to external models
  Model trained on confidential dealer agreements

Architect's job:
  Azure OpenAI (data stays in your tenant — not used for training)
  Managed Identity instead of API keys
  PII detection before sending to LLM (Azure AI Language)
  Data classification — know what data goes where
  VNet integration — traffic stays off public internet
```

---

#### Inclusiveness

AI systems should empower everyone — including people with disabilities, different languages, varying technical literacy.

```
JM Family application:
  Document Q&A app should work for non-technical dealers
  Interface should support screen readers
  Support Spanish-speaking dealer contacts
  Error messages should be plain language, not technical jargon

Architect's job:
  Design for accessibility from the start (not as afterthought)
  Multi-language support where users need it
  Plain language responses — avoid jargon in AI answers
```

---

#### Transparency

Users should understand how and why AI made a decision.

```
JM Family risk:
  Model flags dealer as "high risk" — dealer asks why
  Black box answer: "The model predicted so" — unacceptable

Architect's job:
  Explainable AI: show which features drove the prediction
  Azure ML model explanations (feature importance scores)
  Citations in RAG — "Answer based on: FordAgreement.pdf, Page 3"
  Clear disclosure: "This response was generated by AI"
  Human review workflow for high-stakes decisions
```

---

#### Accountability

There should be human oversight — people are responsible for AI systems and their impacts.

```
JM Family application:
  AI flags an invoice as fraudulent → human reviews before action
  AI recommends a dealer contract term → lawyer approves
  AI model output logged → audit trail for decisions
  
Architect's job:
  Human-in-the-loop for high-stakes decisions
  Audit logging of all AI predictions and responses
  Clear ownership: who is responsible for this AI system?
  Regular review: is the model still performing as expected?
  Process for handling AI errors and complaints
```

---

### 3. Responsible AI in Azure — Tools Available

| Tool | Purpose |
|---|---|
| **Azure Content Safety** | Detect harmful content, groundedness checking |
| **Azure AI Fairlearn** | Measure and mitigate model bias |
| **Azure ML Model Explanations** | Feature importance — why did model predict this? |
| **Azure Policy** | Enforce governance on AI resources |
| **Prompt Shields** | Detect prompt injection attacks |
| **Azure Monitor** | Track AI system behavior in production |

---

## Topic R1 — Recall: Module 1 Review & Quiz

---

**Q1.** What is the difference between AI, Machine Learning, and Deep Learning?

> **A:** AI is the broad field of systems that mimic human intelligence. Machine Learning is a subset of AI where systems learn from data instead of being explicitly programmed. Deep Learning is a subset of ML that uses multi-layer neural networks to learn complex patterns. Generative AI (GPT-4o, Claude) is a subset of Deep Learning that generates new content.

---

**Q2.** A JM Family system predicts whether an invoice will be paid on time or late. What type of ML is this? What type of task?

> **A:** Supervised learning (you have historical invoices with known outcomes as labels). The task is classification — predicting one of two categories: "on time" or "late."

---

**Q3.** The same system predicts how many days until payment. What changes?

> **A:** Still supervised learning, but the task changes from classification to regression — predicting a continuous number (days) instead of a category.

---

**Q4.** Your model achieves 99% accuracy on training data but only 67% on test data. What is wrong and how do you fix it?

> **A:** Overfitting — the model memorized the training data instead of learning generalizable patterns. Fix: get more training data, simplify the model, apply regularization, or use cross-validation during training.

---

**Q5.** A dealer complains that your late-payment prediction model consistently flags Southeast region dealers at higher rates than other regions. Which Responsible AI principle is at risk and what do you do?

> **A:** Fairness. Audit the training data for regional bias — the model may have learned historical patterns that unfairly disadvantage Southeast dealers. Use Azure Fairlearn to measure disparate impact across regions. If bias is confirmed, rebalance the training data or apply fairness constraints to the model. Add monitoring to track prediction rates by region in production.

---

**Q6.** What is the difference between precision and recall? When would you prioritize recall for JM Family?

> **A:** Precision = of all invoices the model flagged as late, how many actually were late (low false positives). Recall = of all invoices that actually were late, how many did the model catch (low false negatives). For JM Family, prioritize recall — missing a genuinely late invoice (false negative) costs money. It is better to investigate some false alarms (low precision) than miss real late payments.

---

## Memory Hooks

- **"AI > ML > Deep Learning > GenAI"** — each is a subset of the previous
- **"Supervised = data with answers, Unsupervised = data without answers"**
- **"Classification predicts a category, Regression predicts a number"**
- **"Train/Validate/Test — never test on training data"**
- **"Overfit = memorized, Underfit = didn't learn"**
- **"Precision = quality of positives flagged, Recall = coverage of actual positives"**
- **"Six Responsible AI: Fairness, Reliability, Privacy, Inclusiveness, Transparency, Accountability"**
- **"Responsible AI = architecture requirement, not an afterthought"**

---

## 2026 Updates — What's Changed Since This Module Was Written

| Topic | Update |
|---|---|
| **GenAI models** | GPT-4o, Claude Sonnet 4.6, Gemini 2.0 — all Narrow AI, more capable but same architecture principles apply |
| **EU AI Act (2026)** | Legally binding now — high-risk AI systems (hiring, credit, healthcare) require explainability and human oversight. Responsible AI is now a compliance requirement, not just a best practice |
| **Agentic AI** | New category: AI agents that chain actions autonomously (ReAct loop). Still Narrow AI — but decisions chain, so Accountability principle is more critical |
| **Azure AI Foundry** | New unified portal (ai.azure.com) — replaces Azure OpenAI Studio. The "Azure AI Services landscape" (Module 2) is now managed from one place |
| **AI-102 cert** | You completed this ✅ — all Module 1 concepts are exam-covered and verified |

---

## Interactive Learning Ideas

### Exercise 1 — Draw the Hierarchy (5 min)
Close this file. Draw the AI > ML > Deep Learning > GenAI nesting diagram from memory. Add one real product at each level. Check against Topic 1.1.

### Exercise 2 — Classify JMA Scenarios (10 min)
For each JMA use case below, identify: ML type (supervised/unsupervised/RL) + task (classification/regression/clustering/anomaly):

| JMA Scenario | ML Type | Task |
|---|---|---|
| Predict if a dealer invoice will be paid late | ? | ? |
| Predict how many days until an invoice is paid | ? | ? |
| Group dealers by behavior without labels | ? | ? |
| Detect unusual spikes in vehicle pricing data | ? | ? |
| Train a chatbot on feedback (reward = helpfulness) | ? | ? |

*(Answers: Supervised/Classification, Supervised/Regression, Unsupervised/Clustering, Unsupervised/Anomaly Detection, Reinforcement Learning)*

### Exercise 3 — Metric Decision (5 min)
For each JMA system, decide: optimize **precision** or **recall**? Why?
- Late invoice detector
- Fraud detection on dealer transactions
- Dealer satisfaction classifier (flag = "unhappy")

### Exercise 4 — Responsible AI Audit (10 min)
Pick one JMA AI system you know (EnterpriseSearch, DealerIntelligence, VitalCare). For each of the 6 principles, write one sentence: what's the risk and what's your mitigation?

### Exercise 5 — Azure Portal Check (5 min)
Go to portal.azure.com → search "Responsible AI dashboard" → explore what metrics are available for your existing ML models. Note one metric that maps to Fairness.

---

*Next: Module 2 — Azure AI Services Overview*
*Also feeds into: Module 6 — Azure Machine Learning (AutoML, training, deployment)*
*Updated: 2026-06-30*
