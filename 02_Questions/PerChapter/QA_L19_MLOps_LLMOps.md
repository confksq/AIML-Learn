# Q&A — L19: MLOps and LLMOps
**Source chapter:** `01_Lessons/Part4_Architecture/L19_MLOps_LLMOps.md` | **Format:** self-study
**Questions:** 30 | *No overlap with the interview bank (Module 6 covers LLMOps/CI-CD at architect-judgment level) or the chapter's own self-test — these drill the chapter's specifics.*

---

## MLOps vs LLMOps

**Q1. Give the one-line definition of each.**
MLOps = **DevOps for traditional ML models** (version, train, deploy, monitor, retrain). LLMOps = **DevOps for LLM-based applications** (version prompts, evaluate quality, monitor responses, detect drift, A/B test).

**Q2. Contrast MLOps and LLMOps on: what you own, what you version, what drifts, how you test.**
MLOps — you **own the model weights**, version the model binary, drift = input data changes, test accuracy/F1/AUC. LLMOps — you **call a hosted model**, version the prompt + config, drift = provider model update or document staleness, test groundedness/relevance/coherence/fluency.

**Q3. What's the failure story without any Ops discipline?**
A data scientist emails a `.pkl` to a dev → dev copies it to a server manually → nobody knows which version is in production → the model silently degrades unnoticed → a prompt is changed in code with no review or rollback → "it worked on my machine" for AI systems.

---

## Model Versioning & Lifecycle

**Q4. Name the five stages of the traditional ML lifecycle (Azure ML).**
Development (experiments, tracked runs) → Registration (Model Registry, versioned with accuracy/date/dataset) → Deployment (Managed Online Endpoint, blue-green) → Monitoring (data drift + prediction drift, alert on threshold) → Retirement (retire old version once new is proven; keep history to revert).

**Q5. Name the five stages of the LLM lifecycle (Azure AI Foundry).**
Model Selection (catalog → baseline config) → Prompt Development (system prompt in Prompt Flow, Git-versioned, each change a commit) → Evaluation (eval flow vs test dataset, must pass thresholds) → Deployment (promote evaluated prompt, keep previous for one-command rollback) → Monitoring (token usage, latency, Content Safety blocks, feedback, hallucination rate).

**Q6. What's the model-registry rule, and how is "production" designated?**
**Never delete versions — always keep history.** "Production" is a **tag**, not the deletion of other versions (e.g., v3 tagged production, v4 in staging/shadow mode), so you can always revert.

---

## CI/CD for AI

**Q7. How does AI CI/CD differ from standard software CI/CD?**
Standard: commit→build→unit tests→deploy — fast, deterministic. AI adds two new steps and a gate: commit→build→unit tests→**model evaluation→prompt evaluation→quality gate (pass/fail on metrics)**→deploy if passed. Slower (evaluation takes time), non-deterministic (LLM output varies), and the **quality gate blocks deploy if metrics drop**.

**Q8. Walk the five stages of the AI CI/CD pipeline.**
(1) Build & unit tests (dotnet build/test). (2) **Model evaluation** — run eval flow on 100 golden Q&A pairs; fail if groundedness/relevance/coherence/fluency below threshold. (3) Integration tests (call the real RAG endpoint; check format, latency <5s, no errors). (4) Deploy to staging + smoke tests. (5) Deploy to production (manual architect approval, blue-green, 10% traffic first, monitor 30 min, then 100%).

**Q9. What thresholds does the pipeline's quality gate check?**
Groundedness ≥ 0.85, relevance ≥ 0.80, coherence ≥ 0.80, fluency ≥ 0.80 — any metric below threshold fails the pipeline.

**Q10. In the Azure DevOps YAML, what does the CheckQualityGate step do?**
It reads `evaluation-results.json` and **`exit 1` (fails the pipeline)** if `groundedness < 0.85`, otherwise prints "Quality gate passed." This is what blocks a bad prompt/model change from reaching production.

---

## Monitoring & Observability

**Q11. Name the three monitoring layers for AI systems.**
Layer 1 — **Infrastructure** (CPU, memory, response time, error rate — Azure Monitor/App Insights). Layer 2 — **AI-specific** (token usage/request, LLM latency p50/p95/p99, embedding latency, AI Search latency, Content Safety block rate). Layer 3 — **Quality/LLMOps** (groundedness, relevance, user feedback, hallucination rate — Foundry evaluations + custom logging).

**Q12. In the observability code, what AI-specific metrics get tracked per request?**
`RAG.InputTokens`, `RAG.OutputTokens`, `RAG.ChunksRetrieved`, `RAG.LatencyMs`, `RAG.GroundednessScore` — plus a custom event with question hash, model used, and cache-hit flag; exceptions are tracked and the operation marked failed.

**Q13. Name six App Insights dashboard panels and an alert for each.**
Token spend $/day (alert > $X/day), latency percentiles (alert p95 > 8s), quality scores rolling 24h (alert groundedness < 0.80), Content Safety blocks/hour (spike = attack/misuse), cache hit rate (low = cache not working or queries too varied), error rate (429/500/503; alert > 2%).

---

## Drift Detection

**Q14. Define the three types of drift with a JMA example each.**
**Data drift** (inputs change) — users start asking about new invoice types not in the index; detect via query-embedding clustering shifting from the training distribution. **Concept drift** (world changes, answer changes) — late penalty changes 2%→3% but indexed docs still say 2%; detect via groundedness/feedback drops and golden-dataset re-evaluation. **Model drift** (provider updates the model) — OpenAI silently updates GPT-4o, behavior changes with no code change; detect via scheduled (weekly) evaluation vs baseline, alert if scores drop > 5%.

**Q15. Give the drift response for each: data drift, concept drift, model drift.**
Data drift → re-index new documents, update chunking if structure changed, re-evaluate. Concept drift → update source documents with correct info, re-ingest, re-evaluate. Model drift → run full evaluation immediately; if scores dropped, update the prompt to compensate; if severe, **pin to the previous model version**; document in the decision log.

**Q16. When quality drops but no drift cause is found, what do you check?**
Whether top-K is too low (not enough context retrieved), whether chunk size needs adjustment, recent prompt changes, and red-team to find edge cases.

**Q17. What triggers automated retraining in the traditional-ML code, and what's the threshold?**
`TriggerRetrainingIfNeededAsync` checks the data-drift score; if it exceeds **0.15 (15%)**, it logs a warning and creates an Azure ML pipeline job to retrain the classifier.

---

## LLMOps — Prompts, Evaluation, A/B

**Q18. Why version prompts, and what's the good vs bad approach?**
Prompts change model behavior as much as code — they need history, review, and rollback. **Bad:** hardcoded in a C# string, changed directly in production, no history/rollback. **Good:** stored as files in Git (`/prompts/invoice-assistant/v1.1.0.md`), loaded at runtime from config (`"PromptVersion": "v1.1.0"`); rollback = change config value + redeploy; history = git log.

**Q19. In the PromptLoader code, where does the version come from and what happens if the file is missing?**
The version is read from config (`Prompts:{promptName}:Version`, defaulting to "latest"); the prompt is loaded from `prompts/{promptName}/{version}.md`. If the file doesn't exist, it **throws FileNotFoundException** — the prompt is never silently missing.

**Q20. What is a golden dataset — size, who writes it, and what each pair contains?**
~**100 hand-crafted Q&A pairs** written by **domain experts** (the invoice team), each with a question + ideal answer + source document. The eval flow runs all pairs through the RAG pipeline, scores groundedness/relevance/coherence/fluency, and requires all ≥ 0.80 to pass the quality gate.

**Q21. Walk an A/B prompt test from setup to promotion.**
New prompt v2 scored 0.89 groundedness in evaluation vs v1's 0.83; validate in production before full rollout. Setup: route **10% traffic → v2, 90% → v1 (control)**, run 1 week. Measure: v1 groundedness 0.83 / rating 3.8; v2 groundedness 0.88 / rating 4.1 → **v2 wins → promote to 100%**. In code: a feature flag decides which prompt version loads per user.

**Q22. Name the four LLMOps maturity levels, and JMA's target.**
Level 0 — Manual (hardcoded prompt, no eval/monitoring — avoid). Level 1 — Basic (prompt in Git config, manual eval, basic monitoring). Level 2 — Automated (versioned + eval in CI/CD, quality gate, cost/latency dashboards). Level 3 — Advanced (A/B testing, automated drift detection, retraining pipeline, per-response hallucination detection, feedback loop into eval dataset). JMA target: **Level 2 → Level 3.**

---

## Final Summary & 2026

**Q23. Summary table — for MLOps vs LLMOps: what you version, what you evaluate, how you retrain.**
MLOps: version the **model binary**, evaluate **accuracy/F1/AUC**, retrain via a **full training run**. LLMOps: version the **prompt + config**, evaluate **groundedness/relevance/fluency**, "retrain" by **updating the prompt or a small fine-tune**.

**Q24. Explaining LLMOps to a traditional ML engineer who asks "where is the model file we deploy?"**
There often isn't one — you **call a hosted model** (GPT-4o) rather than deploying weights. What you version and deploy is the **prompt + config** (temperature, model, template), your RAG configuration, and your evaluation dataset. "Retraining" is usually a prompt update or a small fine-tune, and drift comes from the provider updating the model or your documents going stale — not from your training data shifting.

**Q25. What is a `.prompty` file, and what does it contain?**
The new Foundry format for versioned prompts stored in Git — a single file bundling the **model, parameters, and template** (name/description, model config + temperature/max_tokens, then the system + user template with `{{context}}`/`{{query}}` placeholders). Version-controlled, testable, and deployable the same as code.

**Q26. What CI/CD integration did GitHub Actions get for Azure OpenAI (2026)?**
An official Azure OpenAI action for **evaluation in CI/CD** — trigger evaluation on every PR that changes a system prompt or RAG configuration.

**Q27. What's the 2026 model-lifecycle rule for retirements, and its LLMOps implication?**
Azure OpenAI model retirements are announced **12 months in advance** — LLMOps must **track deprecation dates and include upgrade tasks** in the roadmap so a retirement never causes a surprise outage.

---

## Applied (Self-Test & Exercises)

**Q28. A dev deploys a new system prompt directly to production with no evaluation — which principle did they violate, and what should they have done?**
They violated the **quality-gate / evaluate-before-promote** principle (and prompt versioning). Correct path: store the prompt as a versioned file, run it against the golden dataset, pass the quality-gate thresholds in CI/CD, deploy blue-green with the previous version kept for one-command rollback.

**Q29. Groundedness dropped 0.87→0.71 overnight with no code change — three likely causes and how to investigate each?**
(1) **Model drift** — provider updated the model; investigate by running the golden-dataset evaluation and comparing to baseline. (2) **Concept drift** — source documents went stale vs reality (a policy changed); investigate via user feedback and checking whether answers are still supported by current chunks. (3) **Data/retrieval drift** — new query types or an index/retrieval regression; investigate by inspecting retrieved chunks and query-embedding distribution. Run the eval flow first — it isolates model vs content causes.

**Q30. Design a weekly LLMOps health check (Exercise 3) — four drift signals and their tools.**
**Quality drift** — run the golden dataset weekly; alert if groundedness drops > 5% (Foundry Evaluation). **Cost drift** — alert if avg tokens/response rises > 20% (App Insights token metrics — flags a model or prompt change). **Latency drift** — alert if P95 rises > 500ms (App Insights latency percentiles — check deployment health). **Usage drift** — alert if certain intents spike unexpectedly (App Insights custom events — new behavior or a bug).

---

*Curriculum Q&A Batch E — file 3 of 3 (L17, L18, L19 complete). Next batch: L20, L21 (final).*
