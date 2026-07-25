# Module 6 — Azure Machine Learning
**Part 1: AI Fundamentals | AI Solutions Architect Curriculum**

---

## What You Already Know (Recap)

From Module 1 and your existing knowledge:
- **Supervised learning** — labeled data, trains a model to predict labels
- **Classification vs Regression** — category output vs number output
- **Training / Test split** — never evaluate on training data
- **Overfitting** — model memorizes instead of generalizing
- **Azure OpenAI (Module 12)** — LLMs for generation
- **Azure AI Search (Module 9)** — retrieval and vector search
- **RAG (Module 13)** — retrieval-augmented generation pipeline

This module covers **Azure Machine Learning** — Microsoft's managed platform for building, training, and deploying traditional ML models (not just LLMs). It sits between "raw Python ML code" and "fully managed Azure AI services."

---

**Running example (used throughout):**
> *JM Family wants to predict which dealers will submit late invoices, forecast vehicle inventory needs, and detect anomalous pricing — using Azure ML to build, train, and deploy these models without managing infrastructure.*

---

## Topic 6.1 — Azure ML Workspace

---

### 1. What Is Azure ML Workspace?

Azure Machine Learning Workspace is Microsoft's **end-to-end ML platform** — a single place to manage everything in the ML lifecycle:

```
┌──────────────────────────────────────────────────────────────┐
│                  Azure ML Workspace                          │
│                                                              │
│  Data         Compute        Experiments      Models         │
│  ───────      ───────        ───────────      ──────         │
│  Datastores   Compute        Training runs    Model registry │
│  Datasets     clusters       Metrics/logs     Versions       │
│  Data assets  Instances      Artifacts        Tags           │
│               Serverless                                     │
│                                                              │
│  Pipelines    Environments   Endpoints        Monitoring     │
│  ─────────    ────────────   ─────────        ──────────     │
│  ML pipeline  Docker images  Real-time        Drift detect   │
│  steps        Conda envs     Batch            Data quality   │
│  schedules    Dependencies   Online           Alerts         │
└──────────────────────────────────────────────────────────────┘
```

**One workspace per project/team** — JM Family would have separate workspaces for dev, staging, prod environments.

---

### 2. Core Workspace Components

#### Datastores and Data Assets

Where your training data lives and how Azure ML references it:

```
Datastore = connection to a storage location
  ├── Azure Blob Storage     ← most common
  ├── Azure Data Lake        ← large scale analytics
  ├── Azure SQL Database     ← structured data
  └── Azure Files            ← shared file access

Data Asset = a versioned, tracked reference to specific data
  └── invoice_dataset_v3
      ├── Source: blob://jmf-ml-data/invoices/2026/
      ├── Format: Parquet
      ├── Version: 3
      └── Schema documented
```

**Why version data?** If your model performance drops in production, you need to know exactly what data it was trained on. Data versioning enables rollback and debugging.

---

#### Compute

Where your training and inference code actually runs:

| Compute Type | What it is | When to use |
|---|---|---|
| **Compute Instance** | Single VM — your personal dev machine | Notebooks, exploration, development |
| **Compute Cluster** | Auto-scaling pool of VMs | Training jobs — scales up then back to 0 |
| **Serverless Compute** | No cluster to manage — Azure provisions on demand | Simple training jobs, cost-efficient |
| **Inference Cluster** | AKS cluster for deployed models | High-volume real-time endpoints |
| **Attached Compute** | Bring your own (Databricks, Synapse) | Existing infrastructure |

**JM Family recommendation:**
- Development: Compute Instance (Standard_DS3_v2)
- Training: Serverless Compute or Compute Cluster (Standard_DS4_v2, min 0 nodes)
- Deployment: Managed Online Endpoint (Azure handles the cluster)

---

#### Environments

Reproducible Python environments for training:

```yaml
# Azure ML Environment definition
name: jmf-ml-invoice-env
version: 2
dependencies:
  - python=3.10
  - pip:
    - scikit-learn==1.4.0
    - pandas==2.1.0
    - azure-ai-ml==1.12.0
    - azureml-mlflow==1.54.0
```

**Why environments matter:** Training ran fine last month. Now it fails because scikit-learn auto-updated. Pinned environments ensure reproducibility.

---

#### Experiments and Jobs

Every training run is tracked as a job within an experiment:

```
Experiment: "invoice-late-prediction"
  ├── Job run 1: accuracy=0.72, algorithm=LogisticRegression
  ├── Job run 2: accuracy=0.81, algorithm=RandomForest
  ├── Job run 3: accuracy=0.85, algorithm=GradientBoosting
  └── Job run 4: accuracy=0.87, algorithm=GradientBoosting, tuned ← best
```

Every job automatically logs:
- Metrics (accuracy, precision, recall, F1)
- Parameters (algorithm settings)
- Artifacts (model files, charts)
- Duration and compute cost

**This is what makes ML reproducible** — you can always go back and see exactly what produced a given model.

---

#### Model Registry

Versioned storage for trained models:

```
Model: invoice-late-predictor
  ├── Version 1: accuracy=0.81  (Feb 2026)  archived
  ├── Version 2: accuracy=0.85  (Mar 2026)  archived
  └── Version 3: accuracy=0.87  (May 2026)  ← current production
      ├── Framework: scikit-learn
      ├── Training job: run_20260515_143022
      ├── Training data: invoice_dataset_v3
      └── Tags: approved=true, owner=bala@jmfamily.com
```

**Why registry matters:** When you deploy a new model version and something goes wrong, you roll back to the previous version in one click.

---

### 3. Creating a Workspace — Azure CLI

```bash
# Create resource group
az group create --name rg-jmf-ml-dev --location eastus

# Create Azure ML workspace
az ml workspace create \
  --name ws-jmf-ml-dev \
  --resource-group rg-jmf-ml-dev \
  --location eastus

# Associated resources created automatically:
#   Storage Account    (data, artifacts)
#   Key Vault          (secrets)
#   Application Insights (monitoring)
#   Container Registry (environments/Docker images)
```

---

### 4. Azure ML Studio — The Web UI

Everything in the workspace is accessible through Azure ML Studio (`ml.azure.com`):

```
Left navigation:
  Author          → Notebooks, Designer, AutoML
  Assets          → Data, Models, Environments, Components
  Jobs            → All training runs and their metrics
  Endpoints       → Deployed models
  Monitoring      → Production model health
```

You can do everything via UI (Studio), SDK (Python), or CLI. For JM Family production workloads, CLI and SDK are preferred — repeatable, scriptable, CI/CD friendly.

---

## Topic 6.2 — Automated ML (AutoML)

---

### 1. What Is AutoML?

AutoML automates the most time-consuming parts of the ML workflow:

```
Without AutoML (manual):
  Data scientist tries:
    Algorithm 1: Logistic Regression    → accuracy 0.72
    Algorithm 2: Random Forest          → accuracy 0.81
    Algorithm 3: Gradient Boosting      → accuracy 0.85
    Algorithm 4: XGBoost (tuned)        → accuracy 0.87
    Algorithm 5: Neural Network         → accuracy 0.84
    ...hours or days of experimentation...

With AutoML:
  You specify: data, target column, task type, time limit
  Azure ML tries all algorithms automatically
  Returns the best model
  ...runs in the background while you do other work...
```

**AutoML is not magic** — it runs the same algorithms a data scientist would try, just faster and systematically. The value is speed and breadth of search.

---

### 2. What AutoML Automates

```
Feature engineering    →  creates new features automatically
                          (date → day_of_week, month, quarter)
                          (text → TF-IDF vectors)

Algorithm selection    →  tries 20+ algorithms in parallel
                          LightGBM, XGBoost, Random Forest,
                          Logistic Regression, SGD, etc.

Hyperparameter tuning  →  finds best settings per algorithm
                          (tree depth, learning rate, etc.)

Ensemble building      →  combines multiple models
                          (VotingEnsemble, StackEnsemble)

Cross-validation       →  validates each model properly
                          prevents overfitting

Model explanation      →  shows which features matter most
```

---

### 3. AutoML Task Types

| Task | What it predicts | JM Family Example |
|---|---|---|
| **Classification** | Category label | Late or on-time? Fraud or legitimate? |
| **Regression** | Numeric value | Days to payment? Invoice amount? |
| **Time Series Forecasting** | Future values over time | Vehicle inventory needed next quarter? |
| **NLP Classification** | Text category | Dealer complaint category? |
| **Image Classification** | Image category | Vehicle condition from photo? |
| **Object Detection** | Objects in image | Detect VIN plate in photo? |

---

### 4. Running AutoML — Azure ML Studio

**Step 1 — Create or upload dataset**
```
Azure ML Studio → Data → Create
  Name: invoice-training-data
  Source: Azure Blob Storage
  Format: CSV or Parquet
  Schema verified automatically
```

**Step 2 — Launch AutoML job**
```
Azure ML Studio → Author → Automated ML → New run
  Dataset:         invoice-training-data
  Target column:   payment_status        ← what to predict
  Task type:       Classification
  Primary metric:  AUC_weighted          ← for imbalanced classes
  Compute:         Serverless
  Training time:   60 minutes max
  Concurrent runs: 4 simultaneous trials
```

**Step 3 — Monitor and select best model**
```
After training:
  AutoML shows leaderboard:
    Rank 1: VotingEnsemble        AUC=0.923  ← best
    Rank 2: LightGBM              AUC=0.918
    Rank 3: XGBoostClassifier     AUC=0.911
    Rank 4: RandomForest          AUC=0.897
    ...

Click best model → Deploy or register
```

---

### 5. AutoML via Python SDK

```python
from azure.ai.ml import MLClient
from azure.ai.ml.automl import classification
from azure.ai.ml.entities import Data
from azure.identity import DefaultAzureCredential

# Connect to workspace
ml_client = MLClient(
    DefaultAzureCredential(),
    subscription_id="a4656eb6-5a57-4548-9e60-0b905e3e16a2",
    resource_group_name="rg-jmf-ml-dev",
    workspace_name="ws-jmf-ml-dev"
)

# Configure AutoML classification job
automl_job = classification(
    compute="serverless",
    experiment_name="invoice-late-prediction",
    training_data=ml_client.data.get("invoice-training-data", version="3"),
    target_column_name="payment_status",
    primary_metric="AUC_weighted",
    n_cross_validations=5,
    enable_model_explainability=True
)

# Set limits
automl_job.set_limits(
    timeout_minutes=60,
    trial_timeout_minutes=10,
    max_trials=20,
    max_concurrent_trials=4,
    enable_early_termination=True
)

# Submit
returned_job = ml_client.jobs.create_or_update(automl_job)
print(f"Job submitted: {returned_job.name}")
```

---

### 6. AutoML Metrics — Choosing the Right One

**For Classification:**

| Metric | Use when |
|---|---|
| **Accuracy** | Classes are balanced (50/50 split) |
| **AUC_weighted** | Classes are imbalanced (most invoices are on-time, few are late) |
| **F1_score_weighted** | Balance precision and recall both matter |
| **Precision_score_weighted** | False positives are costly |
| **Recall_score_weighted** | False negatives are costly (use for fraud/late detection) |

**JM Family — use AUC_weighted:** In practice, 80% of invoices are on-time and 20% are late. This imbalance means accuracy is misleading (a model that always predicts "on-time" gets 80% accuracy but catches zero late invoices).

**For Regression:**

| Metric | Use when |
|---|---|
| **RMSE** | Large errors should be penalized heavily |
| **MAE** | All errors should be treated equally |
| **R2** | Understand how much variance is explained |

---

### 7. AutoML Model Explanations

After training, AutoML shows **feature importance** — which input features drove predictions most:

```
Feature Importance for invoice-late-predictor:

  payment_terms_days      ████████████████  0.34  (most important)
  dealer_invoice_volume   ████████████      0.26
  dealer_region           ████████          0.18
  vehicle_type            ██████            0.13
  invoice_amount          ███               0.09
```

**This answers the Transparency Responsible AI requirement** — when a dealer asks "why was I flagged?" you can say: "Your 60-day payment terms and high invoice volume are the primary factors."

---

### 8. When to Use AutoML vs Custom Training

| | AutoML | Custom Training |
|---|---|---|
| **Best for** | Standard tabular ML tasks | Complex custom architectures |
| **Data science skill needed** | Low | High |
| **Control over algorithm** | Low — AutoML chooses | Full control |
| **Time to first model** | Hours | Days to weeks |
| **Explainability** | Built in | Must implement yourself |
| **JM Family use** | Invoice prediction, demand forecast | Custom deep learning models |

---

## Topic 6.3 — Azure ML Designer

---

### 1. What Is Azure ML Designer?

Designer is a **visual drag-and-drop interface** for building ML pipelines — no code required.

```
Canvas interface:
  ┌──────────┐     ┌──────────┐     ┌──────────┐
  │  Invoice │ ──► │  Clean   │ ──► │  Split   │
  │   Data   │     │   Data   │     │   Data   │
  └──────────┘     └──────────┘     └──────────┘
                                          │
                              ┌───────────┴──────────┐
                              │                      │
                         ┌────▼─────┐          ┌────▼─────┐
                         │  Train   │          │   Test   │
                         │  Model   │          │   Data   │
                         └────┬─────┘          └────┬─────┘
                              │                      │
                         ┌────▼─────────────────────▼─────┐
                         │        Score & Evaluate         │
                         └─────────────────────────────────┘
```

Each box is a **component** — a pre-built operation. You connect them with lines (data flows from left to right, top to bottom).

---

### 2. Key Designer Components

| Component Category | Examples |
|---|---|
| **Data Input** | Import Data, Enter Data Manually |
| **Data Transformation** | Clean Missing Data, Normalize Data, Select Columns, Split Data |
| **Feature Selection** | Filter Based Feature Selection, Permutation Feature Importance |
| **Classification** | Two-Class Logistic Regression, Random Forest, Boosted Decision Tree |
| **Regression** | Linear Regression, Boosted Decision Tree Regression |
| **Model Training** | Train Model, Cross Validate Model |
| **Evaluation** | Evaluate Model, Score Model |

---

### 3. Designer Pipeline — JM Family Example

Building a late invoice classifier in Designer:

```
Step 1: [Import Data]
  Source: Azure Blob — invoice_training.csv

Step 2: [Select Columns in Dataset]
  Keep: payment_terms_days, dealer_volume, region, vehicle_type, 
        invoice_amount, payment_status
  Remove: invoice_id, dealer_name (identifiers, not predictive)

Step 3: [Clean Missing Data]
  For numeric columns: replace with median
  For categorical: replace with mode

Step 4: [Normalize Data]
  Normalize: invoice_amount (range 0-1)
             payment_terms_days (range 0-1)
  Reason: large scale differences bias some algorithms

Step 5: [Split Data]
  Fraction: 0.8 (80% training, 20% test)
  Stratified split on: payment_status (preserve class ratio)

Step 6: [Two-Class Boosted Decision Tree]
  (connects to training port of Train Model)

Step 7: [Train Model]
  Label column: payment_status
  Left input: algorithm (Step 6)
  Right input: training data (80% split from Step 5)

Step 8: [Score Model]
  Left input: trained model (Step 7)
  Right input: test data (20% split from Step 5)

Step 9: [Evaluate Model]
  Input: scored test data (Step 8)
  Output: accuracy, AUC, precision, recall, confusion matrix
```

---

### 4. Designer vs AutoML vs Custom Code

| | Designer | AutoML | Custom Python |
|---|---|---|---|
| **Coding required** | None | None | Yes |
| **Algorithm choice** | You drag the algorithm | AutoML chooses | You write it |
| **Flexibility** | Medium — limited to built-in components | Low — AutoML decides | Full |
| **Best for** | Learning, quick prototypes, non-coders | Fast model finding | Production custom models |
| **Reproducibility** | Pipeline is visual and saved | Job config saved | Code in git |

**For JM Family production:** AutoML or custom Python SDK. Designer for prototyping and communication with non-technical stakeholders.

---

### 5. Publishing Designer Pipelines as Endpoints

After building a Designer pipeline, you can publish it as a REST endpoint:

```
Designer pipeline → Publish → Pipeline Endpoint
  URL: https://eastus.api.azureml.ms/pipelines/v1.0/.../run

Call from C#:
var client = new HttpClient();
var request = new
{
    ExperimentName = "invoice-prediction",
    ParameterAssignments = new { invoice_amount = 47000, terms = 60 }
};
var response = await client.PostAsJsonAsync(pipelineEndpointUrl, request);
```

---

## Topic 6.4 — Model Deployment

---

### 1. The Deployment Problem

A trained model sitting in the Model Registry does nothing for anyone. Deployment makes it callable from your applications.

```
Model in Registry:
  invoice-late-predictor v3 (scikit-learn, .pkl file)
  → sitting in Azure Blob Storage
  → useless until deployed

After deployment:
  REST API endpoint:
  POST https://jmf-invoice-pred.eastus.inference.ml.azure.com/score
  Body: {"payment_terms_days": 60, "dealer_volume": 200, "region": "SE"}
  Response: {"prediction": "late", "probability": 0.78}
  → your C# app calls this, gets prediction in real time
```

---

### 2. Two Types of Endpoints

#### Managed Online Endpoints — Real-Time Inference

Request → immediate response (milliseconds):

```
Use when:
  Application needs instant prediction
  One invoice at a time
  User is waiting for the answer

JM Family use:
  New invoice arrives → predict late/on-time → route to follow-up queue
  Response needed in < 500ms
```

```python
from azure.ai.ml.entities import (
    ManagedOnlineEndpoint,
    ManagedOnlineDeployment,
    Model,
    Environment,
    CodeConfiguration
)

# Create endpoint (the URL, auth, scaling config)
endpoint = ManagedOnlineEndpoint(
    name="jmf-invoice-predictor",
    description="Late invoice prediction endpoint",
    auth_mode="key"
)
ml_client.online_endpoints.begin_create_or_update(endpoint).result()

# Create deployment (the actual model + compute behind the endpoint)
deployment = ManagedOnlineDeployment(
    name="v3",
    endpoint_name="jmf-invoice-predictor",
    model=ml_client.models.get("invoice-late-predictor", version="3"),
    environment="jmf-ml-invoice-env:2",
    code_configuration=CodeConfiguration(
        code="./scoring",          # folder with score.py
        scoring_script="score.py"
    ),
    instance_type="Standard_DS3_v2",
    instance_count=2               # 2 for high availability
)
ml_client.online_deployments.begin_create_or_update(deployment).result()
```

**The scoring script (score.py) — required:**
```python
import json
import joblib
import numpy as np

def init():
    global model
    model_path = os.path.join(os.environ["AZUREML_MODEL_DIR"], "model.pkl")
    model = joblib.load(model_path)

def run(raw_data):
    data = json.loads(raw_data)
    features = np.array([[
        data["payment_terms_days"],
        data["dealer_volume"],
        data["region_encoded"],
        data["invoice_amount"]
    ]])
    prediction = model.predict(features)[0]
    probability = model.predict_proba(features)[0].max()
    return {"prediction": prediction, "probability": float(probability)}
```

---

#### Batch Endpoints — Batch Inference

Process large files of records overnight:

```
Use when:
  Scoring thousands of records at once
  Results not needed immediately
  Nightly batch job

JM Family use:
  Every night: score all invoices submitted today
  Flag high-risk ones for follow-up tomorrow morning
  Output CSV → Azure Blob → Power BI dashboard
```

```python
# Batch endpoint processes files, not individual requests
batch_endpoint = BatchEndpoint(
    name="jmf-invoice-batch-predictor",
    description="Nightly invoice risk scoring"
)

batch_deployment = ModelBatchDeployment(
    name="v3-batch",
    endpoint_name="jmf-invoice-batch-predictor",
    model=ml_client.models.get("invoice-late-predictor", version="3"),
    compute="jmf-compute-cluster",
    output_action=BatchDeploymentOutputAction.APPEND_ROW,
    mini_batch_size=100,             # process 100 records per mini-batch
    max_concurrency_per_instance=4
)
```

---

### 3. Calling a Deployed Endpoint from C#

```csharp
public class InvoiceRiskClient
{
    private readonly HttpClient _httpClient;
    private readonly string _endpointUrl;
    private readonly string _apiKey;

    public InvoiceRiskClient(string endpointUrl, string apiKey)
    {
        _endpointUrl = endpointUrl;
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public async Task<InvoiceRiskResult> PredictAsync(InvoiceFeatures features)
    {
        var payload = JsonSerializer.Serialize(features);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_endpointUrl, content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<InvoiceRiskResult>(result);
    }
}

// Usage
var client = new InvoiceRiskClient(
    "https://jmf-invoice-predictor.eastus.inference.ml.azure.com/score",
    Environment.GetEnvironmentVariable("ML_API_KEY")
);

var risk = await client.PredictAsync(new InvoiceFeatures
{
    PaymentTermsDays = 60,
    DealerVolume = 200,
    Region = "Southeast",
    InvoiceAmount = 47000
});

Console.WriteLine($"Risk: {risk.Prediction}, Confidence: {risk.Probability:P0}");
// Output: Risk: late, Confidence: 78%
```

---

### 4. Blue-Green Deployment — Safe Model Updates

Never switch 100% of traffic to a new model immediately. Use traffic splitting:

```python
# Deploy new model version as "green" deployment
# Keep old "blue" deployment running

# Start: 100% traffic to v3 (blue)
endpoint.traffic = {"v3": 100, "v4": 0}

# Test: send 10% to new v4 (green)
endpoint.traffic = {"v3": 90, "v4": 10}
# Monitor metrics — if v4 performs well...

# Graduate: 50/50
endpoint.traffic = {"v3": 50, "v4": 50}

# Full cutover: 100% to v4
endpoint.traffic = {"v3": 0, "v4": 100}

# Remove old deployment
ml_client.online_deployments.begin_delete("v3", endpoint_name="jmf-invoice-predictor")
```

**Why this matters:** If the new model has a bug or unexpected behavior, you can instantly roll back by setting traffic back to the old deployment.

---

### 5. Model Monitoring After Deployment

Models degrade over time — the real world changes but the model doesn't:

```
Trained on: 2025 invoice data
Deployed: January 2026
Problem: Economic conditions change → dealer payment patterns shift
Result: Model accuracy drifts from 87% → 74% by July 2026

This is called DATA DRIFT or MODEL DRIFT
```

**Azure ML Monitoring:**
```python
# Set up monitoring for the deployed endpoint
monitoring_job = MonitorSchedule(
    name="invoice-predictor-monitor",
    trigger=RecurrenceTrigger(frequency="week", interval=1),
    create_monitor=MonitorDefinition(
        monitoring_target=MonitoringTarget(
            endpoint_deployment_id="jmf-invoice-predictor:v4"
        ),
        signals={
            "data_drift": DataDriftSignal(
                reference_data=training_data_reference,
                features=["payment_terms_days", "dealer_volume", "invoice_amount"],
                metric_thresholds=[
                    NumericalDriftMetricThreshold(
                        metric=NumericalDriftMetrics.NORMALIZED_WASSERSTEIN_DISTANCE,
                        threshold=0.15  # alert if drift > 15%
                    )
                ]
            )
        }
    )
)
```

**When drift is detected:** retrain model on recent data → evaluate → blue-green deploy new version.

---

## Topic R6 — Recall: Module 6 & Part 1 Comprehensive

---

**Q1.** What are the four main components of Azure ML Workspace that you interact with in an ML project?

> **A:** (1) **Data** — datastores and versioned data assets where training data lives. (2) **Compute** — instances for dev, clusters for training, endpoints for inference. (3) **Experiments/Jobs** — every training run logged with metrics, parameters, artifacts. (4) **Model Registry** — versioned model storage with metadata, enabling rollback and audit trail.

---

**Q2.** JM Family wants to predict vehicle inventory needs for next quarter across all dealer regions. Which AutoML task type and which metric would you choose?

> **A:** **Time Series Forecasting** — predicting future numeric values over time (inventory counts per quarter). Primary metric: **RMSE** or **MAE** (regression metrics — how far off is the predicted inventory count from actual). Avoid accuracy — that is a classification metric.

---

**Q3.** Your AutoML job ran for 60 minutes and the leaderboard shows VotingEnsemble at AUC=0.923. What is a VotingEnsemble and why does AutoML often return it as the best model?

> **A:** VotingEnsemble combines the predictions of multiple models (e.g., LightGBM + XGBoost + RandomForest) by averaging their probability outputs or taking a majority vote. It typically outperforms any single model because errors from different algorithms cancel each other out. AutoML specifically builds it after trying individual algorithms, using the best-performing ones as its components.

---

**Q4.** What is the difference between a Managed Online Endpoint and a Batch Endpoint? Give a JM Family use case for each.

> **A:** **Managed Online Endpoint** — synchronous, real-time, one request at a time, response in milliseconds. JM Family use: predict late/on-time as each invoice arrives from a dealer system. **Batch Endpoint** — asynchronous, processes large files overnight, output written to storage. JM Family use: nightly batch scoring of all invoices submitted that day, results available next morning in Power BI.

---

**Q5.** Your invoice risk model was 87% accurate when deployed in January. By June it is 74% accurate. What is this called and how do you address it in Azure ML?

> **A:** **Data drift / model drift** — the real-world data distribution has shifted (dealer payment patterns changed) but the model was trained on old patterns. Address it: (1) Set up Azure ML Monitoring to detect drift automatically. (2) When drift threshold is crossed, retrain the model on recent data. (3) Evaluate new model. (4) Deploy using blue-green traffic splitting — send 10% traffic to new model, monitor, gradually increase to 100% if performance is better.

---

**Q6.** A stakeholder asks why the model flagged dealer JMF-ATL-001 as high risk. How does Azure ML help you answer this?

> **A:** Azure ML AutoML with `enable_model_explainability=True` generates **feature importance scores** — showing which input features contributed most to this specific prediction. For JMF-ATL-001 you can say: "Payment terms of 60 days (weight: 0.34) and high invoice volume of 200 (weight: 0.26) are the primary factors driving the high-risk prediction." This directly satisfies the **Transparency** Responsible AI requirement.

---

## Production Architecture — JM Family Azure ML Full Picture

```
┌─────────────────────────────────────────────────────────────────────┐
│                        TRAINING PIPELINE                            │
│                                                                     │
│  Azure Blob Storage (invoice history)                               │
│       ↓ Azure ML Data Asset (versioned)                             │
│  AutoML Job (classification, AUC_weighted, 60 min)                 │
│       ↓ VotingEnsemble selected as best                             │
│  Model registered → Model Registry v3                               │
│       ↓ feature importance logged                                   │
│  Evaluation report → Azure ML Studio                                │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                        INFERENCE PIPELINE                           │
│                                                                     │
│  New invoice arrives (dealer portal / ERP system)                   │
│       ↓                                                             │
│  C# app calls Managed Online Endpoint (REST API)                    │
│       ↓                                                             │
│  score.py loads model → predicts late/on-time + probability         │
│       ↓                                                             │
│  If probability > 0.70 → flag for follow-up queue                   │
│  If probability < 0.70 → process normally                           │
│       ↓                                                             │
│  Result logged → App Insights (latency, prediction distribution)    │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                        MONITORING PIPELINE                          │
│                                                                     │
│  Azure ML Monitoring (weekly)                                       │
│       ↓ compares current data vs training data                      │
│  Data drift detected → alert fired                                  │
│       ↓                                                             │
│  Trigger: retrain AutoML on last 90 days of data                    │
│       ↓                                                             │
│  New model v4 → blue-green deploy (10% → 50% → 100%)               │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Memory Hooks

- **"Workspace = one place for data, compute, jobs, models, endpoints"**
- **"Compute Cluster scales to 0 — you only pay when training"**
- **"AutoML = systematic algorithm search — not magic, just thorough"**
- **"AUC_weighted for imbalanced classes — accuracy lies when classes are unequal"**
- **"Online Endpoint = real-time one at a time, Batch Endpoint = overnight thousands"**
- **"score.py: init() loads model once, run() called per request"**
- **"Blue-green = 10% → 50% → 100% — never flip 100% instantly"**
- **"Models drift — real world changes, model doesn't — monitor and retrain"**
- **"Feature importance = Transparency Responsible AI — tells you WHY the model decided"**
- **"Data versioning + model versioning = reproducibility + rollback"**

---

---

## 2026 Updates

| Topic | Update |
|---|---|
| **Serverless Compute** | Now fully GA — recommended over Compute Clusters for most training jobs. No cluster management, auto-scales, costs nothing when idle |
| **Azure AI Foundry integration** | Azure ML model registry now visible in Azure AI Foundry (ai.azure.com). Models trained in Azure ML can be deployed to AI Foundry endpoints |
| **MLflow as default tracking** | Azure ML now uses MLflow natively. `mlflow.log_metric()` works directly in Azure ML jobs without custom logging code |
| **Responsible AI dashboard** | GA and enhanced — Fairness, Explainability, Error Analysis, Causal Analysis all in one dashboard per model. Directly addresses Responsible AI requirements from Module 1 |
| **Prompt Flow in Azure ML** | Prompt Flow (for LLM pipelines) is now also accessible from Azure ML workspace, not just AI Foundry |

---

## Interactive Learning Ideas

### Exercise 1 — Azure ML Studio Walkthrough (15 min)
Go to ml.azure.com → explore your workspace (or create a free-tier one):
- Navigate: Data → Jobs → Models → Endpoints
- Find the Responsible AI dashboard under Models
- Note what compute types are available

### Exercise 2 — AutoML Run (30 min)
In Azure ML Studio → Automated ML → New run:
- Use any tabular CSV dataset (you can use a public one from UCI ML Repository)
- Task: Classification
- Primary metric: AUC_weighted
- Training time limit: 15 minutes
- Watch the leaderboard populate in real time
- After it finishes: check feature importance on the best model

### Exercise 3 — Score.py Pattern
Write a `score.py` for a hypothetical invoice risk model:
- `init()`: load model from `AZUREML_MODEL_DIR`
- `run(raw_data)`: parse JSON, run prediction, return result with confidence
- Test it locally with mock data before deploying

### Exercise 4 — Blue-Green Deployment Drill
On paper: a new model v4 is ready. Walk through the exact traffic split sequence you'd use:
- What % goes to v4 first?
- What metric do you monitor?
- At what threshold do you increase traffic?
- What triggers a rollback?
- When do you delete the old deployment?

### Exercise 5 — Drift Detection Design
Design a data drift monitoring strategy for JMA's invoice risk model:
- What features would you monitor? (which ones are most likely to drift?)
- What threshold triggers a retrain alert?
- How often does the monitor run?
- What's the retrain trigger — automatic or manual approval?

---

*Previous: Module 1 — Introduction to AI (foundational vocabulary)*
*Next: Module 7 — Azure AI Services Deep Dive*
*Updated: 2026-06-30*
