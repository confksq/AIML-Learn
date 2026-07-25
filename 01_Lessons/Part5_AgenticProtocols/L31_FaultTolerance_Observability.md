# Module 10 — Fault Tolerance, Self-Healing Agents & Observability


---

> **⚙️ Config or Code? — Quick Reference for This Module**
> | Component | Portal Config | Custom Code |
> |---|---|---|
> | Polly RetryPolicy | None ❌ | 100% C# code (`Policy.Handle<>().WaitAndRetryAsync()`) ✅ |
> | Polly CircuitBreaker | None ❌ | 100% C# code (`CircuitBreakerAsync()`) ✅ |
> | Azure Monitor alerts | Portal (alert rules, thresholds) ✅ | None for basic alerts |
> | Azure Monitor dashboards | Portal (workbooks, charts) ✅ | None |
> | App Insights — enable | Portal (toggle on resource) ✅ | Add SDK NuGet package |
> | App Insights — custom events | None ❌ | `_telemetry.TrackEvent()` in C# ✅ |
> | Container Insights (pod health) | Portal (enable on AKS/Container Apps) ✅ | None |
> | Groundedness drift detection | None ❌ | Rolling average calculation + threshold check in code ✅ |
> | Prompt rollback | None ❌ | CI/CD pipeline script + git revert automation ✅ |
> | Foundry evaluation (quality gate) | Run eval in Foundry portal ✅ | Prepare golden dataset (Python) + CI/CD YAML ✅ |
> | LLMOps CI/CD pipeline | Azure DevOps pipeline config (YAML) ✅ | Scripts inside the pipeline ✅ |
> | Dead-letter queue setup | Azure Service Bus portal ✅ | Replay worker code ✅ |

## Why This Module Matters

The job description asks you to "Engineer fault-tolerant agent systems with end-to-end observability, monitoring, and self-healing capabilities." This is an architect-level question — it separates people who built a demo from people who ran agents in production at 3am. You will be asked:
- "Your agent is calling a payer API that starts timing out. What happens?"
- "How do you know your agent's answer quality is degrading without a human checking?"
- "What does 'self-healing' mean in the context of an AI agent?"

Your anchor: JM Family production uses Polly for retry/circuit breaker on all external calls. Module 19 covered the three monitoring layers. The VitalCare platform uses the same patterns at healthcare scale.

---

## Section 1 — Why Fault Tolerance Is Different for AI Agents

Fault tolerance in traditional APIs is simple: retry the call, return an error code. In AI agents, it's harder because:

1. **Failures are silent.** An agent that gets a poor RAG result doesn't crash — it produces a confident-sounding wrong answer. The failure mode is hallucination, not an exception.
2. **State is mid-flight.** An agent 8 steps into a 12-step prior auth workflow that fails at step 9 has accumulated context that must be preserved — not lost.
3. **Cascading is fast.** One specialist agent returning bad data feeds the Supervisor, which produces a bad synthesis, which reaches the physician. Three hops, seconds elapsed, wrong clinical decision.

**The mental model:** Think of AI agent fault tolerance like a **hospital ICU monitoring system**. It doesn't just alert when a patient flatlines (hard crash). It alerts when trends are heading the wrong way: oxygen drifting down 1% per hour, heart rate variability narrowing. Your monitoring must detect drift before the crash.

---

## Section 2 — Polly: Retry and Circuit Breaker

**Polly** is the standard .NET resilience library. It handles two patterns:

### Retry Policy
When a transient failure occurs (network blip, API rate limit, temporary timeout), retry with exponential backoff and jitter.

```csharp
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .Or<TimeoutRejectedException>()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt =>
            TimeSpan.FromSeconds(Math.Pow(2, attempt))   // 2s, 4s, 8s
            + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 500)),  // jitter
        onRetry: (exception, delay, attempt, context) =>
            _logger.LogWarning("Retry {Attempt} after {Delay}ms: {Error}",
                attempt, delay.TotalMilliseconds, exception.Message)
    );
```

**Why jitter?** Without jitter, all retrying agents hit the recovering API at the exact same second — a thundering herd that re-overwhelms it. Jitter spreads the load randomly across a time window.

### Circuit Breaker
When a downstream service is consistently failing (not a transient blip — it's actually down), stop calling it immediately and fail fast.

```csharp
var circuitBreaker = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(
        exceptionsAllowedBeforeBreaking: 5,     // open after 5 failures
        durationOfBreak: TimeSpan.FromSeconds(60),  // stay open 60s
        onBreak: (ex, duration) =>
            _logger.LogError("Circuit OPEN — PayerAPI unavailable for {Duration}s", duration.TotalSeconds),
        onReset: () =>
            _logger.LogInformation("Circuit CLOSED — PayerAPI recovering"),
        onHalfOpen: () =>
            _logger.LogInformation("Circuit HALF-OPEN — sending probe request")
    );
```

**Circuit states:**
```
CLOSED (normal) → 5 failures in 30s → OPEN (fail-fast, no calls for 60s)
                                           ↓ after 60s
                                       HALF-OPEN (send one probe)
                                           ↓ probe succeeds
                                       CLOSED (back to normal)
```

**Healthcare example — Prior Auth agent calling BlueCross payer API:**
- BlueCross API goes down at 2pm
- Without circuit breaker: every prior auth request waits 30s for timeout × 3 retries = 90s wasted per request. 500 requests queued = 12+ hours of wasted compute
- With circuit breaker: after 5 failures, all calls fail immediately with `CircuitOpenException`. Prior auths are pended and queued for when the circuit closes. Total wasted time: 5 × timeout, then instant failures.

---

## Section 3 — Agent Self-Healing Patterns

"Self-healing" in AI agents means the system **detects degraded quality and responds automatically** — without waiting for a human to notice.

### Pattern 1 — Groundedness Drift Detection

Your agent's answers are checked for groundedness (factual accuracy against the retrieved context). You track groundedness scores over a rolling window.

```csharp
// In EvaluationPipeline.cs
if (rollingAvgGroundedness < GROUNDEDNESS_THRESHOLD)
{
    // Trigger automatic rollback to previous prompt version
    await _promptVersionManager.RollbackAsync();
    await _alertManager.NotifyAsync("Groundedness drift detected — rolled back prompt");
}
```

**Threshold:** JM Family uses 0.85. VitalCare healthcare: 0.90 (higher stakes — clinical decisions).

**Why it matters:** A prompt update might have introduced subtle instruction drift that makes the model answer with less grounding. The evaluation pipeline catches this within minutes, not days.

### Pattern 2 — Automatic Prompt Rollback

When groundedness drops below threshold, the system automatically:
1. Rolls back to the previous prompt version in Git
2. Redeploys the old prompt to the agent
3. Fires an alert to the AI engineering team
4. Logs the incident with before/after groundedness scores

This is LLMOps in action — the same concept as a blue-green deployment rollback, applied to AI prompts.

### Pattern 3 — Agent Restart on Tool Failure Loop

An agent can enter a failure loop: it calls a tool, the tool fails, it retries, it fails again, it retries infinitely. Guards:

```csharp
// In Semantic Kernel FunctionInvocationFilter
if (context.ToolCallCount > MAX_TOOL_CALLS)
{
    context.Cancel();
    await _escalationQueue.SendAsync(sessionId, "Max tool calls exceeded");
}
```

**Max tool calls:** JM Family uses 10. In healthcare: 8 (tighter — clinical urgency means faster escalation).

### Pattern 4 — Dead-Letter Replay

When a workflow fails (agent crashes, message is dead-lettered), the system can replay the message automatically once the root cause is resolved — without human re-submission.

```
Dead-letter queue
      ↓ root cause fixed (circuit closed, service recovered)
Replay worker reads dead-letter
      ↓
Re-publishes message to the main queue
      ↓
Workflow resumes from the point of failure (LangGraph Checkpointer)
or restarts from beginning (SK — no auto-checkpointer)
```

---

## Section 4 — End-to-End Observability: Three Layers

Observability for AI agents is not just "is the service up?" It's three layers, each measuring something different:

### Layer 1 — Infrastructure (is the compute healthy?)
- CPU / memory / pod restarts
- Azure Function scale-out events
- Queue depth (Service Bus backlog growing = agents can't keep up)
- **Tool:** Azure Monitor + Container Insights

### Layer 2 — AI Service (is the LLM responding?)
- Token usage per request (trending up = prompts growing, costs rising)
- Latency per call (p50, p95, p99)
- Rate limit hits (429 errors from Azure OpenAI)
- TPM quota utilization
- **Tool:** Azure OpenAI metrics in Azure Monitor + App Insights custom events

### Layer 3 — Quality (is the agent answering correctly?)
- Groundedness score (is the answer supported by retrieved context?)
- Relevance score (did the retrieval return relevant chunks?)
- Coherence score (is the output logically structured?)
- Latency from user question to final answer (full pipeline, not just LLM)
- **Tool:** Azure AI Foundry Evaluation + App Insights custom metrics

**The rule:** An alert that only fires when the service crashes is Infrastructure Layer 1. An architect designs all three layers. Quality Layer 3 is what distinguishes AI system monitoring from regular system monitoring.

---

## Section 5 — The Three-Layer Monitoring Dashboard

```
┌─────────────────────────────────────────────────────────┐
│  LAYER 1: INFRASTRUCTURE                                │
│  CPU: 42%   Memory: 61%   Pod restarts: 0   Queue: 12  │
├─────────────────────────────────────────────────────────┤
│  LAYER 2: AI SERVICE                                    │
│  Tokens/req: 1,847   Latency p95: 2.3s   429s: 0      │
│  TPM utilization: 68%   Cost/hr: $4.20                 │
├─────────────────────────────────────────────────────────┤
│  LAYER 3: QUALITY                                       │
│  Groundedness: 0.91 ✅   Relevance: 0.88 ✅            │
│  Coherence: 0.87 ✅      Hallucination rate: 2.1% ✅   │
│  Rolling 1hr avg groundedness: 0.89 (threshold: 0.85)  │
└─────────────────────────────────────────────────────────┘
```

---

## Section 6 — JM Family Anchor

"At JM Family, every external call in the agent pipeline — Document Intelligence, Azure AI Search, Azure OpenAI — goes through a Polly RetryPolicy (3 retries, exponential backoff with jitter) and a CircuitBreaker (opens after 5 consecutive failures, stays open 60 seconds). App Insights captures token usage, latency, and cost per request. The groundedness evaluation runs on a 20-sample rolling window every 30 minutes — if it drops below 0.85, we get a Teams alert and the engineering team reviews before the next deployment cycle. We don't yet have automatic prompt rollback in production — that's the next LLMOps maturity step."

---

---

## Section 7 — CV SKILL: LLMOps — Complete Practice

> **CV anchor:** "LLMOps — prompt versioning, model deployment management and rollback via Azure AI Foundry, automated evaluation pipelines, production monitoring (token/cost/quality), Azure DevOps CI/CD integration for AI systems"

### What LLMOps Is

```
LLMOps = DevOps applied to AI systems

Traditional DevOps manages: code → tests → build → deploy → monitor
LLMOps manages:             prompts → evaluation → deploy → monitor → rollback

The key difference:
├── Code changes are deterministic — same input, same output
└── Prompt changes are probabilistic — same input, slightly different output
    └── You CANNOT just unit test a prompt — you need statistical evaluation
```

### Component 1 — Prompt Versioning in Git

```
Every system prompt is a text artifact → treat it like code

File structure:
prompts/
├── clinical_pa_agent/
│   ├── v1.0.0.txt    ← original prompt
│   ├── v1.1.0.txt    ← added few-shot examples
│   └── v2.0.0.txt    ← breaking change: new output schema
└── ambient_doc_agent/
    └── v1.0.0.txt

Git branching for prompts:
├── main = production prompts
├── feature/add-cot-reasoning = in-development change
└── PR process: prompt change requires clinical + legal review before merge

Why it matters:
├── Full history of what changed and when
├── Blame for regressions: "groundedness dropped 6/23 → git log shows prompt v2.0 merged 6/22"
└── Rollback = git revert, not manual editing in the portal
```

### Component 2 — Automated Evaluation Pipeline

```
Every prompt change → automated evaluation BEFORE merge to main

Pipeline stages:

Stage 1: Lint
└── System prompt validator checks: max tokens, required sections present,
    security instructions present, output format schema valid

Stage 2: Golden Dataset Evaluation
└── Run the new prompt against 100 curated test cases in Azure AI Foundry
└── Score: groundedness, relevance, coherence, clinical accuracy
└── Generate comparison report: new prompt vs current production

Stage 3: Quality Gate
├── Groundedness ≥ 0.90 ✅
├── Relevance ≥ 0.85 ✅
├── No regression on 15 critical test cases ✅
└── If any gate fails → PR blocked, team notified, no merge

Stage 4: A/B Shadow Deployment
└── New prompt receives 10% of traffic in production
└── Monitor metrics for 24 hours
└── Promote to 100% if metrics hold; rollback if degraded
```

**Azure DevOps YAML pipeline:**
```yaml
trigger:
  paths:
    include:
      - prompts/**           # Only fires when prompt files change

stages:
  - stage: Evaluate
    jobs:
      - job: GoldenDataset
        steps:
          - task: AzureCLI@2
            inputs:
              scriptType: bash
              scriptLocation: inlineScript
              inlineScript: |
                # Run evaluation in Azure AI Foundry
                az ml job create --file evaluation_job.yaml \
                  --workspace-name vitalcare-foundry \
                  --resource-group vitalcare-rg

  - stage: QualityGate
    dependsOn: Evaluate
    jobs:
      - job: CheckThresholds
        steps:
          - task: PythonScript@0
            inputs:
              scriptSource: inline
              script: |
                import json
                results = json.load(open('eval_results.json'))
                assert results['groundedness'] >= 0.90, "Groundedness gate failed"
                assert results['relevance'] >= 0.85, "Relevance gate failed"
```

### Component 3 — Model Deployment Management

```
Azure AI Foundry deployment pattern:

dev environment:
└── Foundry Project (dev) → deploy new model version → evaluate → promote

staging environment:
└── Foundry Project (staging) → shadow traffic 10% → monitor 24h → promote

production environment:
├── Active deployment: GPT-4o version 2025-04-01 (90% traffic)
└── New deployment: GPT-4o version 2025-06-01 (10% traffic)

Blue-Green promotion:
├── Shift: 0% → 10% → 50% → 100% traffic
└── Each shift: watch quality metrics for 1-2 hours
└── If metrics degrade at any shift: rollback to previous 100%
```

### Component 4 — Automatic Rollback

```
Rollback triggers (automatic):

Trigger 1: Groundedness drift
└── Rolling 1-hour average drops below 0.85
└── Action: revert to previous prompt version (git revert)
           redeploy previous model version in Foundry
           alert team

Trigger 2: Latency spike
└── p95 latency exceeds 5s for 10+ minutes
└── Action: revert to smaller model (GPT-4o-mini) or previous version
           alert team

Trigger 3: Error rate spike
└── Tool call failure rate exceeds 5%
└── Action: circuit breaker opens + alert (see Section 2)

Manual rollback:
└── Team can trigger rollback from Azure DevOps pipeline
└── Single-button: "Deploy v1.1.0 to production"
└── Git tag on every production deployment → easy to find and redeploy
```

### Component 5 — Production Monitoring Dashboard

```
LLMOps monitoring adds AI-specific metrics to standard infra monitoring:

┌─────────────────────────────────────────────────────────────────┐
│  LLMOPS DASHBOARD — VitalCare Prior Auth Agent                  │
├─────────────────────────────────────────────────────────────────┤
│  PROMPT & MODEL                                                 │
│  Active prompt version: v1.2.0 (deployed 2026-06-20)           │
│  Active model: GPT-4o 2025-04-01                               │
│  Canary: GPT-4o 2025-06-01 (10% traffic)                       │
├─────────────────────────────────────────────────────────────────┤
│  QUALITY (rolling 1-hour)                                       │
│  Groundedness: 0.91 ✅    Relevance: 0.88 ✅                   │
│  Drift from baseline: +0.01 ✅                                  │
├─────────────────────────────────────────────────────────────────┤
│  COST & TOKENS                                                  │
│  Tokens/request: 1,847   Cost/request: $0.0046                 │
│  Daily cost: $230   Monthly forecast: $6,900                   │
│  Token budget utilization: 72% of 128K context window          │
├─────────────────────────────────────────────────────────────────┤
│  PERFORMANCE                                                    │
│  Latency p50: 1.2s   p95: 2.8s   p99: 4.1s                    │
│  Throughput: 347 requests/hr   Error rate: 0.3% ✅             │
└─────────────────────────────────────────────────────────────────┘
```

### Interview Answer

**Q: How do you manage prompt versions and deployments in production?**
> "Prompts live in Git alongside the code — versioned, reviewed, and deployed through the same Azure DevOps pipeline. Every prompt change triggers our automated evaluation pipeline: lint the prompt for structure, run it against a golden dataset of 100 curated test cases in Azure AI Foundry, and gate on groundedness threshold. If it passes, it goes to 10% shadow traffic in production. We monitor quality metrics for 24 hours before promoting to 100%. If groundedness drops below 0.85 in the rolling 1-hour window, an automated rollback reverts to the previous prompt version and redeploys — no manual intervention. Every production deployment is tagged in Git, so rollback is one pipeline trigger. We track token cost, groundedness drift, and latency percentiles in a single LLMOps dashboard, alerting on drift not just crashes."

---

## Quick-Reference Interview Answers

**Q: How do you make an AI agent fault-tolerant against external tool failures?**
"Two patterns work together. Polly RetryPolicy handles transient failures — network blips, rate limit spikes — with exponential backoff and jitter to avoid thundering herd. The CircuitBreaker handles sustained failures — when a downstream API is actually down, the circuit opens and all calls fail immediately instead of waiting for timeout. That converts a 90-second timeout pile-up into instant failures with a human-readable circuit state. Agents route failed requests to a pend queue, never silently drop them."

**Q: What does 'self-healing' mean for an AI agent in production?**
"Three things. First: groundedness drift detection — if the rolling average quality score drops below threshold, the system automatically rolls back to the previous prompt version and alerts the team. Second: circuit breaker auto-recovery — when the downstream service recovers, the circuit transitions half-open, sends a probe, and closes automatically. Third: dead-letter replay — once the root cause is fixed, the replay worker re-processes failed messages without human resubmission. The goal is that the system returns to healthy state automatically for recoverable failures, and routes to human escalation for unrecoverable ones."

**Q: What three things do you monitor for an AI agent in production?**
"Infrastructure layer: CPU, memory, queue depth — standard SRE metrics. AI service layer: token usage, latency percentiles, rate limit hits, cost per request — this is where AI cost surprises come from. Quality layer: groundedness, relevance, coherence scores on a rolling sample — this is what pure infrastructure monitoring misses. An agent can have perfect uptime and still be producing hallucinated clinical recommendations if nobody is watching quality metrics."
