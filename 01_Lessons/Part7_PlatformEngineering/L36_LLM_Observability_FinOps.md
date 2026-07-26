# Module 36 — LLM Observability, Tracing and FinOps

**Part 7: Platform Engineering & AI-Assisted Delivery**
*Created: 2026-07-26 · FDE-Prep · Clears tracker rows 46, 47, 48, 49, 50, 51, 52*

> **Builds on `L31` §4–5** (three-layer observability, the monitoring dashboard) and
> `L19` §4–6 (monitoring, drift, prompt versioning). Those teach *what* to monitor.
> This module teaches the **tooling** the JD names: OpenTelemetry, LangSmith, Arize, LiteLLM,
> Grafana, Dynatrace — and the cost discipline that pays for all of it.

---

## Why This Module Exists

The JD lists, under **Platform & Infrastructure FDE**:

> *"Observability — OpenTelemetry, Dynatrace, Grafana, **LLM metrics**"*
> *"LLMOps — model routing (**LiteLLM**), semantic caching, prompt versioning, A/B testing"*
> *"**Cost / FinOps for LLM**"*

and under **Agentic Systems FDE**:

> *"Tracing — **LangSmith**, **Arize**, OpenTelemetry"*

Library coverage before this module: OpenTelemetry 🟡 (one Foundry mention in `L17`, one
architecture row in `VitalCare:437`), LiteLLM 🟡 (six scattered lines), FinOps 🟡
(`VitalCare:911`), **LangSmith one line, Arize/Langfuse and Dynatrace nothing.**

---

## Section 1 — Why LLM Observability Is Different

Traditional APM answers *"is it up and fast?"* For an agent that is not enough — an agent can be
100% available, sub-second, and **completely wrong**.

| Traditional service | LLM / agent system |
|---|---|
| Deterministic — same input, same output | **Non-deterministic** — same input, different path |
| Success = HTTP 200 | 200 with a hallucinated answer is a **failure** |
| Latency = one number | latency = TTFT + tokens/sec + tool round-trips |
| Cost ≈ compute hours | **cost = tokens, and it varies per request** |
| One call | an agent loop is **N calls you did not write** |
| Errors throw | failures are *plausible*, and silent |

**The consequence:** you need a fourth signal beyond metrics, logs and traces — **quality**. `L31`
§4 already frames this as three layers; the practical addition here is that the quality layer needs
its own tooling.

```
Layer 1  INFRASTRUCTURE   pods, CPU, memory, restarts        → Prometheus / Dynatrace
Layer 2  AI SERVICE       latency, tokens, 429s, cost        → OTel + Grafana
Layer 3  QUALITY          groundedness, tool accuracy,       → LangSmith / Langfuse /
                          task success, refusal rate            Arize / RAGAS
```

---

## Section 2 — OpenTelemetry for Agents

### 2.1 The vocabulary

| Term | Meaning |
|---|---|
| **Trace** | one end-to-end request |
| **Span** | one operation inside it — nested to form a tree |
| **Attribute** | key/value on a span (`gen_ai.request.model = "gpt-4o"`) |
| **Context propagation** | passing trace IDs across service boundaries |
| **Exporter** | ships telemetry to a backend (Jaeger, Grafana Tempo, App Insights, Dynatrace) |

**Why OTel and not a vendor SDK:** it is vendor-neutral. Instrument once, export anywhere. That is
exactly the argument `VitalCare:437, :1553` already makes — OTel plus Prometheus plus Grafana, with
Azure Monitor / AWS X-Ray as drop-in alternatives.

### 2.2 What an agent trace should look like

```
Trace: cancel-request-88213                              [4.2s total]
├── span: intake.classify                    [0.6s]  120 tok    $0.0004
├── span: tool.lookup_contract               [0.3s]  ← no tokens, a DB call
├── span: llm.reason                         [1.1s]  980 tok    $0.0031
├── span: tool.calc_refund                   [0.1s]
├── span: llm.reason                         [0.9s] 1240 tok    $0.0039
├── span: guardrail.groundedness_check       [0.4s]  310 tok    $0.0010
└── span: tool.submit_cancellation           [0.8s]
                                             ────────────────────────────
                                             4.2s   2650 tok    $0.0084
```

Every question that matters is answerable from this: which step was slow, how many loop iterations,
what did it cost, where did it go wrong.

### 2.3 GenAI semantic conventions

OTel has standard attribute names for LLM calls. Use them — dashboards and backends understand them
without custom mapping.

| Attribute | Example |
|---|---|
| `gen_ai.system` | `azure.ai.openai` |
| `gen_ai.request.model` | `gpt-4o` |
| `gen_ai.request.temperature` | `0.0` |
| `gen_ai.usage.input_tokens` | `980` |
| `gen_ai.usage.output_tokens` | `260` |
| `gen_ai.response.finish_reasons` | `["stop"]` |

### 2.4 Instrumenting — Python

```python
from opentelemetry import trace
from opentelemetry.sdk.trace import TracerProvider
from opentelemetry.sdk.trace.export import BatchSpanProcessor
from opentelemetry.exporter.otlp.proto.grpc.trace_exporter import OTLPSpanExporter

trace.set_tracer_provider(TracerProvider())
trace.get_tracer_provider().add_span_processor(
    BatchSpanProcessor(OTLPSpanExporter(endpoint="http://otel-collector:4317")))

tracer = trace.get_tracer(__name__)

def call_llm(prompt: str) -> str:
    with tracer.start_as_current_span("llm.reason") as span:
        span.set_attribute("gen_ai.system", "azure.ai.openai")
        span.set_attribute("gen_ai.request.model", "gpt-4o")
        resp = client.chat.completions.create(
            model="gpt-4o", messages=[{"role": "user", "content": prompt}])
        u = resp.usage
        span.set_attribute("gen_ai.usage.input_tokens",  u.prompt_tokens)
        span.set_attribute("gen_ai.usage.output_tokens", u.completion_tokens)
        span.set_attribute("gen_ai.cost_usd", cost_of(u, "gpt-4o"))
        return resp.choices[0].message.content
```

That `with` block is `L32` §4 — a context manager. Same pattern, applied.

### 2.5 Instrumenting — C# / Semantic Kernel

```csharp
private static readonly ActivitySource Source = new("JMA.Agents");

using var activity = Source.StartActivity("llm.reason");
activity?.SetTag("gen_ai.request.model", "gpt-4o");
var result = await kernel.InvokePromptAsync(prompt);
activity?.SetTag("gen_ai.usage.input_tokens", usage.PromptTokens);
```

`System.Diagnostics.ActivitySource` **is** OpenTelemetry in .NET. If you already emit Activities,
you already emit OTel spans — you only need an exporter.

### 2.6 Context propagation across agents

For multi-agent systems (`L28`, `L29`), the trace ID must travel with the message or you get
disconnected traces and cannot answer "which agent caused this."

```python
from opentelemetry.propagate import inject, extract

headers = {}
inject(headers)                      # sender: put trace context in the envelope
bus.publish(AgentMessage(payload=..., headers=headers))

ctx = extract(message.headers)       # receiver: continue the same trace
with tracer.start_as_current_span("pricing.calc", context=ctx):
    ...
```

Add a `headers` field to the `AgentMessage` envelope in `L29` and this works end to end.

---

## Section 3 — LLM-Native Tracing Platforms

OTel gives you spans. These give you **prompts, completions, evaluations and datasets** — the
quality layer.

| | **LangSmith** | **Langfuse** | **Arize Phoenix** |
|---|---|---|---|
| By | LangChain | open-source | Arize AI |
| Hosting | SaaS (self-host on enterprise) | **self-hostable, OSS** | **self-hostable, OSS** |
| Strength | deepest LangChain/LangGraph integration | cost tracking, prompt management | **embedding drift, RAG analysis** |
| OTel | partial | **OTel-native** | **OTel-native** |
| Best for | LangChain shops | regulated / self-host requirement | RAG quality and drift |

### 3.1 LangSmith

```python
import os
os.environ["LANGCHAIN_TRACING_V2"] = "true"
os.environ["LANGCHAIN_API_KEY"]    = "..."
os.environ["LANGCHAIN_PROJECT"]    = "cancellation-agent-prod"
# every LangChain / LangGraph call is now traced. No code change.
```

What you get: full prompt and completion text per step, the LangGraph node path taken, token and
cost per step, latency waterfall, plus **datasets and evaluators** — capture production traces,
curate them into a golden dataset, and regression-test prompt changes against it.

That is `L19` §6's golden-dataset evaluation pipeline, productised.

### 3.2 Langfuse — the one to pick under compliance

Self-hosted, so prompts and completions never leave your boundary. For PHI or FedRAMP work
(`L33` §9.2) that is often decisive — a SaaS tracing tool means your prompts are leaving the
compliance boundary, and prompts contain the sensitive data.

`VitalCare:1553` already selects it: *"OpenTelemetry + Prometheus + Grafana + Langfuse (LLM)."*

### 3.3 Arize Phoenix — RAG and drift

Strongest at the embedding layer: visualise embedding clusters, detect drift between your indexed
corpus and live queries, and surface retrieval failures — *"these queries retrieve nothing
relevant."* Directly useful for `L13` RAG debugging.

### 3.4 What to log — and the trap

| Log | Careful with |
|---|---|
| Model, temperature, token counts, cost | **Prompt and completion text** |
| Latency, TTFT, tool names | Anything containing PII/PHI |
| Which prompt version, which node path | Customer identifiers, VINs |
| Retrieved chunk IDs, scores | Retrieved chunk **contents** |

> ⚠️ **Prompt and completion logging is a data-classification decision, not a debugging
> convenience.** Full-text logging is what makes LLM debugging possible and is simultaneously the
> fastest way to leak regulated data into an observability platform. Decide per environment: full
> text in dev, hashed or redacted in prod, with a break-glass path.

---

## Section 4 — LiteLLM: Model Routing

### 4.1 What it is

A proxy and SDK exposing **one OpenAI-compatible API** in front of 100+ providers. Your application
calls one endpoint; LiteLLM decides which model actually serves it.

```
your agents ──► LiteLLM proxy ──┬──► Azure OpenAI  gpt-4o
                                 ├──► AWS Bedrock   claude
                                 ├──► Vertex AI     gemini
                                 └──► self-hosted   vLLM / Llama
```

### 4.2 Config

```yaml
model_list:
  - model_name: primary
    litellm_params:
      model: azure/gpt-4o
      api_base: https://jma-openai.openai.azure.com/
      api_version: "2024-10-01"          # ← PIN the version. Never "latest".
  - model_name: fallback
    litellm_params:
      model: bedrock/anthropic.claude-3-5-sonnet-20241022-v2:0
  - model_name: cheap
    litellm_params:
      model: azure/gpt-4o-mini

router_settings:
  routing_strategy: usage-based-routing-v2
  fallbacks: [{ primary: ["fallback", "cheap"] }]
  num_retries: 3
  allowed_fails: 3
  cooldown_time: 60                      # circuit breaker — L31 §2, at the routing layer

litellm_settings:
  cache: true
  cache_params: { type: redis, ttl: 3600 }
  max_budget: 5000                       # USD/month, hard stop
  budget_duration: 30d
```

### 4.3 Why an architect wants it

| Capability | Value |
|---|---|
| **Provider abstraction** | swap Azure→Bedrock without touching application code |
| **Automatic fallback** | 429 or outage → next model, transparently |
| **Version pinning** | `VitalCare:1394` — never `latest` for clinical AI; rollback is a config change |
| **Virtual keys + budgets** | per-team keys with hard spend caps |
| **Unified cost accounting** | one place that knows every token spent |
| **Caching** | §5 |

`VitalCare:1148` names the risk it mitigates: *"LLM vendor API deprecation — LiteLLM abstraction; 2
model alternatives per use case; 90-day migration window."*

### 4.4 The honest trade-off

Another network hop, another thing to run HA, and it lags provider-specific features. For a
single-model single-cloud app it is overkill. It earns its place when you have multiple teams,
multiple models, or a genuine multi-cloud requirement — which is exactly this JD.

---

## Section 5 — Semantic Caching

`L13`, `L18`, `L20` and `HLP01` already cover the concept. The cost angle:

```
Exact cache      key = hash(prompt)                → only identical strings hit
Semantic cache   key = embedding(prompt)           → "how do I cancel my VSC?" and
                 hit if cosine similarity > 0.95      "what's the VSC cancellation process?"
                                                      both hit the same entry
```

| | Cost | Latency |
|---|---|---|
| Cache miss | full prompt + completion tokens | 2–5 s |
| Semantic hit | **one embedding call** (~1/1000th) | **<100 ms** |

**Two failure modes to state in an interview:**

1. **Threshold too low → wrong answers.** This is `L23`/`L13`'s negation problem again: *"customer
   wants to cancel"* and *"customer does **not** want to cancel"* embed almost identically. A 0.90
   threshold serves the wrong cached answer. Start at 0.95+ and evaluate.
2. **Stale entries.** Policy changed; the cache did not. TTL everything, and invalidate on
   knowledge-base updates.

Never cache personalised responses across users without keying on identity — that is a data-leak
path.

---

## Section 6 — FinOps for LLM

### 6.1 The cost model

```
cost = (input_tokens × input_rate) + (output_tokens × output_rate)
```

Two properties that break normal cost intuition:

- **Output tokens cost ~3–5× input tokens.** Verbose answers are disproportionately expensive.
- **Agents multiply everything.** A 6-iteration loop resends growing context each time. Cost grows
  roughly quadratically with loop length, not linearly.

### 6.2 The levers, in order of return

| # | Lever | Typical saving | Cost to implement |
|---|---|---|---|
| 1 | **Semantic caching** | 30–60% on repetitive traffic | low |
| 2 | **Model tiering** — small model first, escalate | 40–70% | medium |
| 3 | **Prompt compression** — trim system prompts, few-shot | 10–30% | low |
| 4 | **Cap agent iterations** | prevents tail blowouts | trivial |
| 5 | **Trim RAG context** — top-3 not top-10, rerank | 20–40% | medium |
| 6 | **Batch / provisioned throughput** | 20–50% at scale | high commitment |
| 7 | **Self-host open weights** | large at volume | high ops cost |

**Model tiering is the highest-leverage architectural one:**

```
gpt-4o-mini classifies the request        →  ~1/20th the cost
   ├─ simple / high confidence  → answer with mini
   └─ complex / low confidence  → escalate to gpt-4o
```

### 6.3 Showback and chargeback

`VitalCare:911, :1130` sets the maturity bar: *"showback and chargeback operational for all business
domains."*

| Model | Meaning |
|---|---|
| **Showback** | you report what each team spent. Visibility, no billing |
| **Chargeback** | the cost lands on the team's budget |

Mechanically: a LiteLLM virtual key per team/use case, tags on every request, cost per span in OTel,
aggregated in Grafana.

### 6.4 Guardrails that prevent the 3 a.m. incident

```yaml
max_budget: 5000                    # hard stop per key
rpm_limit: 500
tpm_limit: 200000
```

Plus, in the application:

| Guardrail | Prevents |
|---|---|
| Max agent iterations (e.g. 10) | infinite reasoning loop burning tokens |
| Max tokens per response | one runaway generation |
| Per-user rate limit | a single abusive caller |
| Alert at 50 / 80 / 100% of budget | *discovering* an overrun on the invoice |

`VitalCare:1158` — *"Per-unit cost alerts; hard spending caps; weekly FinOps review."*

**Cost per business unit is the metric that matters**, not cost per token. "$0.04 per prior-auth
processed, versus $12 of analyst time" is a board-level sentence. "We spent $18k on tokens" is not.

---

## Section 7 — Dashboards: Prometheus, Grafana, Dynatrace

### 7.1 The stack

```
app  ──OTel SDK──►  OTel Collector  ──┬──► Prometheus  (metrics)   ─┐
                                       ├──► Tempo/Jaeger (traces)   ├──► Grafana
                                       └──► Loki        (logs)      ─┘
                                       └──► Dynatrace / App Insights / X-Ray
```

The **Collector** is the piece worth knowing: applications export to it, and it fans out to
backends. Change your observability vendor by editing collector config, not application code.

### 7.2 Grafana vs Dynatrace

| | Grafana (+Prometheus) | Dynatrace |
|---|---|---|
| Model | open-source, you assemble | commercial, all-in-one |
| Dashboards | you build them | auto-discovered topology |
| AI ops | — | **Davis AI** — automatic root-cause |
| Cost | infra only | per-host licensing |
| Custom LLM metrics | trivial | supported, more setup |

Enterprises frequently run both: Dynatrace as the mandated APM for infrastructure and application
health, Grafana for the custom LLM/agent dashboards Dynatrace was not designed for. Say that — it
reads as someone who has been in a real enterprise rather than picking a favourite.

### 7.3 The dashboard to build

Extending `L31` §5 into three rows:

```
┌─ LAYER 1 · INFRASTRUCTURE ──────────────────────────────────┐
│  Pod restarts  CPU%  Memory%  Node pressure  Queue depth     │
├─ LAYER 2 · AI SERVICE ──────────────────────────────────────┤
│  p50/p95 latency   TTFT   Tokens/min   429 rate              │
│  $/hour   $/request   Cache hit %   Fallback invocations     │
├─ LAYER 3 · QUALITY ─────────────────────────────────────────┤
│  Groundedness   Tool-call accuracy   Task success %          │
│  Avg loop iterations   Human-escalation rate   Refusal rate  │
└──────────────────────────────────────────────────────────────┘
```

### 7.4 What to alert on

| Alert | Threshold | Why |
|---|---|---|
| Cost/hour above baseline | 2× rolling 7-day | runaway loop or attack |
| Cache hit rate collapse | <50% of baseline | cache broken → bill about to spike |
| p95 latency | >2× baseline | provider degradation |
| 429 rate | >5% | quota exhaustion; fallback should be firing |
| **Groundedness score drop** | >10% week-on-week | **quality regression — the one nobody alerts on** |
| Avg loop iterations rising | >1.5× baseline | agent is thrashing |
| Human-escalation rate rising | >1.5× baseline | model or data drift |

The bottom three are the LLM-specific ones. A traditional APM setup will never fire them, and they
are the failures that actually damage a business.

---

## JM Family Anchor

| Your world | Applies |
|---|---|
| AKS pods running agent services | Layer 1 — Container Insights / Prometheus |
| The three-layer dashboard in `L31` §5 | §7.3 extends it with cost and quality |
| CallMiner pipeline stages | OTel spans per stage; trace a recording end to end |
| Multiple model endpoints across environments | LiteLLM proxy, pinned versions, fallback |
| Internal/Confidential JMFE data in prompts | §3.4 — self-hosted Langfuse, redact in prod |

---

## Self-Test Questions

1. Why is HTTP 200 an insufficient definition of success for an agent?
2. Trace vs span vs attribute. What does a single agent request produce?
3. Why use OTel rather than a vendor SDK? What does the Collector let you change without touching
   code?
4. Two agents, disconnected traces. What is missing?
5. LangSmith vs Langfuse — when does compliance force the choice, and why?
6. What is Arize Phoenix uniquely good at?
7. Name four things LiteLLM gives an architect. What is the honest downside?
8. Why is `api_version: "2024-10-01"` better than an alias?
9. Semantic caching at 0.90 similarity — describe the failure, using the cancellation example.
10. Why do output tokens deserve more attention than input tokens? Why do agent loops break linear
    cost intuition?
11. Showback vs chargeback.
12. Which metric would you put in front of a CFO, and why not cost per token?
13. Name three alerts a traditional APM would never fire that an agent platform needs.

---

## Quick-Reference Interview Answers

**"How do you monitor an AI agent in production?"**
> "Three layers. Infrastructure — pods, CPU, restarts, from Prometheus or whatever APM the org
> mandates. AI service — latency, time-to-first-token, tokens per minute, 429 rate and cost per
> request, from OpenTelemetry spans using the GenAI semantic conventions. And a third layer most
> teams skip: quality — groundedness, tool-call accuracy, average loop iterations, human-escalation
> rate. That third layer is the one that catches the failures that matter, because an agent can be
> 100% available, sub-second, and confidently wrong. I trace with OTel so I'm not locked to a
> backend, and add an LLM-native tool — Langfuse if we need self-hosting for compliance, since
> prompts contain the sensitive data."

**"How do you control LLM cost?"**
> "Measure per business unit first — cost per prior-auth processed, not cost per token, because
> that's the number that can be compared to the manual alternative. Then the levers in order:
> semantic caching for repetitive traffic, model tiering so a mini model handles the easy majority
> and escalates only when confidence is low, trimming RAG context and system prompts, and hard caps
> on agent iterations because loop cost grows faster than linearly — you resend growing context each
> turn. Structurally I'd put LiteLLM in front with per-team virtual keys and budgets, so spend is
> attributable and capped rather than discovered on an invoice. And I alert on cost per hour against
> a rolling baseline, because a runaway loop looks exactly like normal traffic until the bill
> arrives."

**"Why LiteLLM?"**
> "Provider abstraction plus operational control in one place. Application code targets one
> OpenAI-compatible endpoint; routing, automatic fallback on 429 or outage, pinned model versions,
> per-team budgets and unified cost accounting all live in config. The pinning matters most for
> regulated work — you never point production at a `latest` alias, and rolling back a model version
> becomes a config change deployed through GitOps rather than a code release. The downside is honest:
> it's another hop to run highly available, and it lags provider-specific features. For one model on
> one cloud it's overkill."

---

## Related

`L31` §4–5 (three-layer observability, the dashboard this extends) · `L19` §4–6 (monitoring, drift,
prompt versioning, A/B testing) · `L13`/`L23` (semantic caching and the negation problem) ·
`L29` (A2A envelope — add trace headers) · `L32` §4 (context managers behind spans) ·
`L34` (GitOps deploys the routing config) ·
`VitalCare_AI_Assessment_Response.md:399, :437, :911, :1148, :1394, :1553`
