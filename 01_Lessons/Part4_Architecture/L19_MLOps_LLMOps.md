# Module 19 — MLOps and LLMOps

**Part:** 3 — Generative AI & LLMs  
**Curriculum:** Updated v2 (23 modules)  
**Prerequisites:** Module 18 (AI Solution Architecture), Module 15 (Fine-tuning), Module 17 (Azure AI Foundry)

---

## What This Module Covers

```
19.1  What Is MLOps and LLMOps
19.2  Model Versioning and Lifecycle Management
19.3  CI/CD for AI Pipelines
19.4  Monitoring and Observability
19.5  Drift Detection and Retraining
19.6  LLMOps — Prompt Versioning, Evaluation, A/B Testing
```

---

## 19.1 What Is MLOps and LLMOps

### The Problem Without Ops

```
WITHOUT MLOps / LLMOps:

  Data Scientist trains a model
  → emails the .pkl file to a developer
  → developer copies it to a server manually
  → nobody knows which version is in production
  → model silently degrades — nobody notices
  → prompt changed in code — no review, no rollback
  → "it worked on my machine" for AI systems
```

### MLOps vs LLMOps — Side by Side

```
MLOps:                          LLMOps:
────────────────────────────────────────────────────────
Traditional ML models           Large Language Models
(classification, regression)    (GPT-4o, fine-tuned models)

You OWN the model weights       You CALL a hosted model
Train from scratch              Fine-tune or use as-is
Deploy a .pkl / ONNX file       Deploy a prompt + API call
Drift = input data changes      Drift = model update by provider
Retrain when performance drops  Update prompt or fine-tune
Version the model binary        Version the prompt + config
Test: accuracy, F1, AUC         Test: groundedness, relevance,
                                      coherence, fluency
Tools: Azure ML, MLflow         Tools: Azure AI Foundry,
                                       LangSmith, PromptFlow
```

### One Line Each

```
MLOps   = DevOps for traditional ML models
          (version, train, deploy, monitor, retrain)

LLMOps  = DevOps for LLM-based applications
          (version prompts, evaluate quality,
           monitor responses, detect drift, A/B test)
```

---

## 19.2 Model Versioning and Lifecycle Management

### Traditional ML — Model Lifecycle (Azure ML)

```
STAGE 1 — Development
  Data Scientist experiments in Azure ML
  Runs tracked in Azure ML Experiments
  Each run logs: metrics, parameters, artifacts

STAGE 2 — Registration
  Best model registered in Azure ML Model Registry
  Version 1 → Version 2 → Version 3
  Each version has: accuracy, F1, training date, dataset

STAGE 3 — Deployment
  Model deployed to Azure ML Managed Online Endpoint
  Blue-green: v1 (90% traffic) + v2 (10% traffic)
  Promote v2 to 100% when validated

STAGE 4 — Monitoring
  Azure ML Model Monitor watches:
    data drift (inputs changing)
    prediction drift (outputs changing)
  Alert when drift exceeds threshold

STAGE 5 — Retirement
  Old version retired when new version proven stable
  Model Registry keeps history — always revert if needed
```

### LLM — Model Lifecycle (Azure AI Foundry)

```
STAGE 1 — Model Selection
  Azure AI Foundry Model Catalog
  Choose: GPT-4o mini / GPT-4o / Llama / Mistral
  Register chosen model + deployment config as baseline

STAGE 2 — Prompt Development
  Write system prompt in Prompt Flow
  Version controlled in Git (like code)
  Each prompt change = a commit

STAGE 3 — Evaluation
  Run evaluation flow against test dataset
  Metrics: groundedness, relevance, coherence, fluency
  Must pass minimum thresholds before promotion

STAGE 4 — Deployment
  Promote evaluated prompt to production
  Previous prompt kept — rollback in one command

STAGE 5 — Monitoring
  Track: token usage, latency, Content Safety blocks,
         user feedback scores, hallucination rate
  Alert when quality drops below threshold
```

### JM Family Model Registry Example

```
Azure ML Model Registry — jmf-invoice-classifier:
  v1 (2025-03-01): Accuracy=0.87  ← retired
  v2 (2025-06-15): Accuracy=0.91  ← retired
  v3 (2025-09-20): Accuracy=0.94  ← production
  v4 (2026-01-10): Accuracy=0.93  ← staging (shadow mode)

Rule: never delete versions — always keep history
      production = tag, not deletion of others
```

---

## 19.3 CI/CD for AI Pipelines

### Standard Software CI/CD vs AI CI/CD

```
Standard CI/CD:
  Code commit → build → unit tests → deploy
  Fast: minutes
  Deterministic: same input = same output always

AI CI/CD:
  Code commit → build → unit tests
              → model evaluation (new step)
              → prompt evaluation (new step)
              → quality gate (pass/fail on metrics)
              → deploy if passed
  Slower: evaluation takes time
  Non-deterministic: LLM output varies
  Quality gate: blocks deploy if metrics drop
```

### AI CI/CD Pipeline — Azure DevOps

```
Trigger: PR merged to main branch
         OR new model registered in Model Registry

Stage 1 — Build and Unit Tests
  dotnet build
  dotnet test
  → standard, fast

Stage 2 — Model Evaluation (NEW for AI)
  Run evaluation flow in Azure AI Foundry
  Test dataset: 100 golden Q&A pairs
  Metrics checked:
    groundedness  ≥ 0.85  (answer supported by retrieved chunks)
    relevance     ≥ 0.80  (answer addresses the question)
    coherence     ≥ 0.80  (answer is well-formed)
    fluency       ≥ 0.80  (answer reads naturally)
  → FAIL pipeline if any metric below threshold
  → PASS if all metrics met

Stage 3 — Integration Tests
  Call the actual RAG endpoint with test questions
  Validate response format, latency < 5s, no errors

Stage 4 — Deploy to Staging
  Deploy to staging environment
  Run smoke tests

Stage 5 — Deploy to Production (manual approval)
  Architect approves promotion
  Blue-green deployment — keep old version running
  Shift 10% traffic to new version first
  Monitor for 30 minutes → shift 100% if stable
```

### Azure DevOps Pipeline YAML (key stages)

```yaml
# azure-pipelines.yml — AI Pipeline CI/CD
trigger:
  branches:
    include: [main]

stages:
  - stage: Build
    jobs:
      - job: BuildAndTest
        steps:
          - script: dotnet build
          - script: dotnet test

  - stage: Evaluate
    dependsOn: Build
    jobs:
      - job: ModelEvaluation
        steps:
          - task: AzureCLI@2
            inputs:
              scriptType: bash
              scriptLocation: inlineScript
              inlineScript: |
                # Run evaluation flow in Azure AI Foundry
                az ml flow run create \
                  --file evaluation-flow.yaml \
                  --resource-group jmf-ai-rg \
                  --workspace-name jmf-ai-foundry

          - task: PowerShell@2
            name: CheckQualityGate
            inputs:
              targetType: inline
              script: |
                $results = Get-Content evaluation-results.json | ConvertFrom-Json
                if ($results.groundedness -lt 0.85) {
                  Write-Error "QUALITY GATE FAILED: groundedness=$($results.groundedness)"
                  exit 1
                }
                Write-Host "Quality gate passed."

  - stage: Deploy
    dependsOn: Evaluate
    condition: succeeded()
    jobs:
      - deployment: DeployToProduction
        environment: production
        strategy:
          runOnce:
            deploy:
              steps:
                - script: dotnet publish && az functionapp deployment...
```

---

## 19.4 Monitoring and Observability

### What to Monitor in AI Systems

```
LAYER 1 — Infrastructure (same as any app)
  CPU, memory, response time, error rate
  Tool: Azure Monitor, App Insights

LAYER 2 — AI-specific metrics (NEW)
  Token usage per request
  LLM latency (p50, p95, p99)
  Embedding call latency
  AI Search query latency
  Content Safety block rate
  Tool: App Insights custom metrics

LAYER 3 — Quality metrics (LLMOps-specific)
  Groundedness score (is the answer in the retrieved chunks?)
  Relevance score (does the answer address the question?)
  User feedback (thumbs up/down, star rating)
  Hallucination rate (answer not supported by sources)
  Tool: Azure AI Foundry evaluations, custom logging
```

### Observability Code in C#

```csharp
public sealed class ObservableRAGService(
    IRAGService inner,
    TelemetryClient telemetry,
    ILogger<ObservableRAGService> logger) : IRAGService
{
    public async Task<ChatResponse> AskAsync(string question, CancellationToken ct = default)
    {
        using var operation = telemetry.StartOperation<RequestTelemetry>("RAG.Ask");
        var sw = Stopwatch.StartNew();

        try
        {
            var response = await inner.AskAsync(question, ct);
            sw.Stop();

            // Track AI-specific metrics
            telemetry.TrackMetric("RAG.InputTokens",    response.InputTokens);
            telemetry.TrackMetric("RAG.OutputTokens",   response.OutputTokens);
            telemetry.TrackMetric("RAG.ChunksRetrieved",response.ChunksUsed);
            telemetry.TrackMetric("RAG.LatencyMs",      sw.ElapsedMilliseconds);
            telemetry.TrackMetric("RAG.GroundednessScore", response.GroundednessScore);

            // Track as custom event for detailed analysis
            telemetry.TrackEvent("RAG.RequestCompleted", new Dictionary<string, string>
            {
                ["questionHash"] = question.GetHashCode().ToString(),
                ["model"]        = response.ModelUsed,
                ["cached"]       = response.FromCache.ToString()
            });

            operation.Telemetry.Success = true;
            return response;
        }
        catch (Exception ex)
        {
            telemetry.TrackException(ex);
            operation.Telemetry.Success = false;
            throw;
        }
    }
}
```

### App Insights Dashboard — What to Build

```
Dashboard panels:

Panel 1: Token spend ($/day)
  Sum of (InputTokens + OutputTokens) × price
  Alert: > $X/day threshold

Panel 2: Latency percentiles
  p50, p95, p99 response times
  Alert: p95 > 8 seconds

Panel 3: Quality scores (rolling 24h average)
  Groundedness, relevance, coherence
  Alert: groundedness < 0.80

Panel 4: Content Safety blocks
  Count of blocked requests per hour
  Spike = possible attack or misuse

Panel 5: Cache hit rate
  % of requests served from Redis cache
  Low rate = cache not working or queries too varied

Panel 6: Error rate
  429 (quota exceeded), 500 (LLM error), 503 (timeout)
  Alert: error rate > 2%
```

---

## 19.5 Drift Detection and Retraining

### Two Types of Drift

```
DATA DRIFT (inputs changing):
  The questions users ask change over time
  Example: users start asking about new invoice types
           that did not exist when you indexed documents
  Detection: monitor query embeddings over time
             if they cluster far from training distribution
             → drift detected

CONCEPT DRIFT (world changes, model wrong):
  The correct answer changes even though data looks same
  Example: late penalty changed from 2% to 3%
           old indexed documents still say 2%
           RAG returns wrong answer
  Detection: groundedness drops (answer not in chunks)
             user feedback scores drop
             periodic re-evaluation against golden dataset

MODEL DRIFT (provider updates model):
  OpenAI silently updates GPT-4o
  Behaviour changes without your code changing
  Detection: run evaluation flow on schedule (weekly)
             compare current scores vs baseline
             alert if scores drop > 5%
```

### Drift Response Playbook

```
DATA DRIFT detected:
  → Re-index new documents
  → Update chunking if document structure changed
  → Re-run evaluation to confirm recovery

CONCEPT DRIFT detected:
  → Update source documents with correct information
  → Re-ingest affected documents
  → Re-run evaluation

MODEL DRIFT detected (provider updated):
  → Run full evaluation immediately
  → If scores dropped: update prompt to compensate
  → If severe: pin to previous model version
  → Document the change in decision log

QUALITY DROP (no drift cause found):
  → Check if top-K too low (not retrieving enough context)
  → Check if chunk size needs adjustment
  → Review recent prompt changes
  → Red-team to find edge cases
```

### Automated Retraining (Traditional ML)

```csharp
// Azure ML pipeline triggered when drift exceeds threshold
public async Task TriggerRetrainingIfNeededAsync()
{
    var driftScore = await _monitor.GetDataDriftScoreAsync();

    if (driftScore > 0.15)  // 15% drift threshold
    {
        logger.LogWarning("Data drift detected: {Score}. Triggering retraining.", driftScore);

        await _mlClient.Jobs.CreateOrUpdateAsync(
            new PipelineJob
            {
                DisplayName = $"retrain-invoice-classifier-{DateTime.UtcNow:yyyyMMdd}",
                // points to training pipeline definition
                Component = "/subscriptions/.../retraining-pipeline"
            });
    }
}
```

---

## 19.6 LLMOps — Prompt Versioning, Evaluation, A/B Testing

### Prompt Versioning

```
Prompts are code — version them like code

BAD (no versioning):
  System prompt hardcoded in C# string
  Changed directly in production
  No history, no rollback

GOOD (versioned):
  Prompts stored as files in Git
    /prompts/invoice-assistant/v1.0.0.md
    /prompts/invoice-assistant/v1.1.0.md
    /prompts/invoice-assistant/v2.0.0.md

  Loaded at runtime from config:
    "PromptVersion": "v1.1.0"

  Rollback = change config value, redeploy
  History = git log on the prompt file
```

### Prompt Version in C#

```csharp
public sealed class PromptLoader(IConfiguration config, IWebHostEnvironment env)
{
    public async Task<string> LoadSystemPromptAsync(string promptName)
    {
        var version = config[$"Prompts:{promptName}:Version"] ?? "latest";
        var path    = Path.Combine(env.ContentRootPath, "prompts", promptName, $"{version}.md");

        if (!File.Exists(path))
            throw new FileNotFoundException($"Prompt '{promptName}' version '{version}' not found.");

        return await File.ReadAllTextAsync(path);
    }
}

// Usage — prompt loaded from file, not hardcoded
var systemPrompt = await _promptLoader.LoadSystemPromptAsync("invoice-assistant");
```

### Evaluation Pipeline — Golden Dataset

```
Golden dataset = 100 hand-crafted Q&A pairs
  Written by domain experts (invoice team)
  Each pair: question + ideal answer + source document

  Example:
    Q: "What is the penalty for a dealer submitting
        an invoice 45 days after the due date?"
    A: "The penalty is 2% per month. At 45 days
        (15 days late), the dealer owes 1% of the
        invoice amount."
    Source: policy-doc-section-4.7.pdf

Evaluation flow runs all 100 pairs through RAG
  Compares actual response to ideal answer
  Scores: groundedness, relevance, coherence, fluency
  Threshold: all scores ≥ 0.80 to pass quality gate
```

### A/B Testing Prompts

```
SCENARIO:
  Current prompt (v1): groundedness = 0.83
  New prompt (v2):     groundedness = 0.89 in evaluation
  Want to validate in production before full rollout

A/B TEST SETUP:
  10% of traffic → v2 prompt
  90% of traffic → v1 prompt (control)
  Run for 1 week

MEASURE:
  v1: groundedness = 0.83, user rating = 3.8/5
  v2: groundedness = 0.88, user rating = 4.1/5
  → v2 wins → promote to 100%

IN CODE:
  var promptVersion = _featureFlags.IsEnabled("prompt-v2", userId)
      ? "v2.0.0"
      : "v1.1.0";
  var prompt = await _promptLoader.LoadSystemPromptAsync("invoice-assistant", promptVersion);
```

### LLMOps Maturity Levels

```
LEVEL 0 — Manual (avoid this):
  Prompt hardcoded in source code
  No evaluation
  No monitoring
  Changes go straight to production

LEVEL 1 — Basic:
  Prompt in config file (versioned in Git)
  Manual evaluation before deploy
  Basic App Insights monitoring

LEVEL 2 — Automated:
  Prompt versioned + evaluation in CI/CD pipeline
  Quality gate blocks bad deploys
  Token cost + latency dashboards in App Insights

LEVEL 3 — Advanced (production-grade):
  A/B testing for prompt changes
  Automated drift detection (weekly evaluation runs)
  Retraining pipeline triggered by drift
  Hallucination detection on every response
  User feedback loop feeding into evaluation dataset

JM Family target: Level 2 → Level 3
```

---

## MLOps vs LLMOps — Final Summary

```
                    MLOps              LLMOps
────────────────────────────────────────────────────────
What you version    Model binary       Prompt + config
What you evaluate   Accuracy, F1, AUC  Groundedness,
                                       relevance, fluency
What drifts         Training data      Provider model,
                                       document freshness
How you retrain     Full training run  Update prompt or
                                       fine-tune (small)
CI/CD gate          Accuracy threshold Quality score threshold
Tools               Azure ML, MLflow   Azure AI Foundry,
                                       Prompt Flow
Monitoring          Data drift scorer  Evaluation pipeline
                    Azure ML Monitor   App Insights + custom
```

---

## Self-Test Questions

1. A JM Family developer deploys a new system prompt directly in production with no evaluation. What MLOps/LLMOps principle did they violate and what should they have done instead?

2. Your invoice assistant's groundedness score dropped from 0.87 to 0.71 overnight. Nobody changed the code. What are the three most likely causes and how do you investigate each?

3. What is the difference between data drift and model drift in an LLM context? Give one JM Family example of each.

4. You want to test a new prompt that you believe will improve coherence scores. Walk through the A/B testing process from start to promotion.

5. A traditional ML engineer joins JM Family's AI team. They ask "where is the model file we deploy?" How do you explain LLMOps to them?

6. What is a golden dataset, why do you need at least 100 examples, and who writes the answers in the JM Family context?

---

## 2026 Updates

| Topic | Update |
|---|---|
| **AI Foundry Evaluation GA** | Foundry Evaluation pipelines now GA — run groundedness, relevance, coherence, fluency at scale against golden datasets. Integrate into CI/CD as a quality gate before promoting prompt versions |
| **AI Foundry Tracing GA** | Full distributed tracing for agent workflows — see every LLM call, retrieval step, tool call, latency, and cost in one trace view. Integrated with Azure Monitor and Application Insights |
| **GitHub Actions + Azure OpenAI** | GitHub Actions now has official Azure OpenAI action for evaluation in CI/CD pipelines. Trigger evaluation on every PR that changes a system prompt or RAG configuration |
| **Prompt versioning best practice** | Store prompts in Git as `.prompty` files (new Foundry format) — includes model, parameters, template in one file. Version-controlled, testable, deployable same as code |
| **Model lifecycle in Foundry** | Azure OpenAI model retirements are now announced 12 months in advance. LLMOps must track model deprecation dates and include upgrade tasks in the MLOps roadmap |

---

## Interactive Learning Ideas

### Exercise 1 — Golden Dataset Creation (20 min)
Create a 20-question golden dataset for JMA's DealerSupport RAG system:
- 5 questions with clear answers in your knowledge base (should succeed)
- 5 questions where the answer is in the knowledge base but requires inference
- 5 questions where the answer is NOT in the knowledge base (should return "I don't know")
- 5 adversarial questions (prompt injection attempts)
Write ideal answers for each. This is your evaluation benchmark.

### Exercise 2 — CI/CD Pipeline Design (15 min)
Design a GitHub Actions workflow for JMA's RAG system:
```yaml
on: [pull_request]  # triggers when system_prompt.prompty changes
jobs:
  evaluate:
    steps:
      - run: python evaluate.py --golden-dataset tests/golden.json
      - run: |
          if groundedness_score < 0.85: exit(1)  # fail the PR
```
What files trigger the pipeline? What score thresholds block a merge? Who approves the merge after scores pass?

### Exercise 3 — Drift Detection for LLMs (15 min)
LLM drift is different from ML drift. Design a weekly LLMOps health check for JMA:
- **Quality drift**: run golden dataset weekly — if groundedness drops > 5%, alert
- **Cost drift**: if average tokens per response increases > 20%, investigate (model change? prompt change?)
- **Latency drift**: if P95 response time increases > 500ms, check Azure OpenAI deployment health
- **Usage drift**: if certain intents spike unexpectedly, new user behavior or a bug?
What tool generates each of these metrics?

### Exercise 4 — .prompty File Creation
Create a `.prompty` file for JMA's dealer support system prompt:
```prompty
---
name: JMA Dealer Support
description: Answers dealer questions using JMA knowledge base
model:
  api: chat
  configuration:
    type: azure_openai
    azure_deployment: gpt-4o
  parameters:
    temperature: 0.3
    max_tokens: 500
---
system:
You are a JMA Family dealer support assistant...
{{context}}

user:
{{query}}
```
Store it in Git. Write a test that loads it and runs it against your golden dataset.

---

*Updated: 2026-06-30*
