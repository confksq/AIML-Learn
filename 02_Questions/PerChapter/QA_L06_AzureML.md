# Q&A — L06: Azure Machine Learning
**Source chapter:** `01_Lessons/Part1_Foundations/L06_AzureML.md` | **Format:** self-study (read Q → attempt answer → check)
**Questions:** 30 | *No overlap with the interview bank (`Questions/01–06`) or the chapter's own self-test (R6).*

---

## Workspace & Core Components

**Q1. What is an Azure ML Workspace, and what associated Azure resources are created automatically with it?**
The workspace is Microsoft's end-to-end ML platform — one place managing data, compute, experiments, models, pipelines, environments, endpoints, and monitoring. Creating one auto-provisions four companion resources: a **Storage Account** (data/artifacts), **Key Vault** (secrets), **Application Insights** (monitoring), and **Container Registry** (Docker images for environments).
*Memory hook: "Workspace + 4 companions: Storage, Key Vault, App Insights, ACR."*

**Q2. How many workspaces should a team like JMA create, and how are they organized?**
One workspace per project/team **per environment** — e.g., separate dev, staging, and prod workspaces. This isolates experiments, data, and models across environments (same isolation principle as any dev/prod separation).

**Q3. What's the difference between a Datastore and a Data Asset?**
A **Datastore** is a *connection* to a storage location (Blob, Data Lake, Azure SQL, Azure Files). A **Data Asset** is a *versioned, tracked reference* to specific data within it — e.g., `invoice_dataset_v3` pointing to a specific blob path, with format, version number, and documented schema.

**Q4. Why version training data at all?**
If model performance drops in production, you need to know exactly what data trained it — data versioning enables rollback, debugging, and reproducing any historical training run. Without it, "what did v2 train on?" is unanswerable.

**Q5. Name the five compute types in Azure ML and when each is used.**
| Compute | Use |
|---|---|
| **Compute Instance** | Single VM — personal dev machine, notebooks, exploration |
| **Compute Cluster** | Auto-scaling VM pool for training jobs — scales up, then back to 0 |
| **Serverless Compute** | No cluster to manage; Azure provisions on demand — simple training, cost-efficient (now GA and recommended for most jobs) |
| **Inference Cluster** | AKS cluster behind deployed models — high-volume real-time endpoints |
| **Attached Compute** | Bring-your-own (Databricks, Synapse) — existing infrastructure |

**Q6. Why is "min 0 nodes" significant for a Compute Cluster?**
The cluster scales down to zero when idle — you pay nothing between training jobs. This is the core cost advantage over keeping a fixed VM running.
*Memory hook: "Cluster scales to 0 — pay only when training."*

**Q7. What problem do Azure ML Environments solve? Give the failure scenario they prevent.**
Reproducibility. Scenario: training ran fine last month, now fails because scikit-learn auto-updated to a breaking version. A pinned environment (exact Python version + exact package versions in a YAML definition) guarantees the same dependencies every run.

**Q8. What does every training job automatically log?**
Metrics (accuracy, precision, recall, F1), parameters (algorithm settings), artifacts (model files, charts), and duration/compute cost — all grouped under an experiment. This automatic logging is what makes any past model reproducible and auditable.

**Q9. What is the Model Registry, and what metadata does a registered model carry?**
Versioned storage for trained models. Each version records: framework, the training job that produced it, the training data asset/version used, and tags (approval status, owner). Its operational payoff: one-click rollback to a previous version when a new deployment misbehaves.

**Q10. What are the three ways to work with Azure ML, and which are preferred for production?**
Studio UI (`ml.azure.com`), Python SDK, and CLI. For production: **CLI and SDK** — repeatable, scriptable, CI/CD-friendly. Studio is for exploration and visibility.

---

## AutoML

**Q11. What does AutoML automate — list the six things.**
(1) **Feature engineering** (date → day-of-week/month/quarter; text → TF-IDF), (2) **algorithm selection** (20+ algorithms tried in parallel), (3) **hyperparameter tuning**, (4) **ensemble building** (Voting/Stack ensembles), (5) **cross-validation**, (6) **model explanation** (feature importance).

**Q12. "AutoML is not magic" — what does that statement actually mean?**
It runs the same algorithms a data scientist would try manually — just faster and more systematically. Its value is *speed and breadth of search*, not access to better algorithms. It won't beat a well-designed custom architecture for genuinely novel problems.

**Q13. List the six AutoML task types with a JMA-style example each.**
| Task | Example |
|---|---|
| Classification | Invoice late or on-time? |
| Regression | Days until payment? |
| Time Series Forecasting | Vehicle inventory needed next quarter? |
| NLP Classification | Dealer complaint category? |
| Image Classification | Vehicle condition from photo? |
| Object Detection | Detect the VIN plate in a photo? |

**Q14. What key configuration does an AutoML run require?**
Dataset, **target column** (what to predict), **task type**, **primary metric** (what "best" means), compute, training time limit, and max concurrent trials. Optionally: cross-validation folds and model explainability enabled.

**Q15. Which limits can you set on an AutoML job via the SDK, and why do they matter?**
`timeout_minutes` (total budget), `trial_timeout_minutes` (per-trial cap), `max_trials`, `max_concurrent_trials`, and `enable_early_termination` (kill clearly-losing trials early). Together they cap compute cost — without them an AutoML sweep can burn budget on hopeless trials.

**Q16. Why does accuracy mislead on imbalanced classes, and what metric do you use instead?**
If 80% of invoices are on-time, a model that always predicts "on-time" scores 80% accuracy while catching zero late invoices. Use **AUC_weighted** for imbalanced classification — it measures ranking quality across both classes rather than raw hit rate.
*Memory hook: "Accuracy lies when classes are unequal."*

**Q17. When would you pick Precision_score_weighted vs Recall_score_weighted as the primary metric?**
**Precision** when false positives are costly (wrongly flagging a good dealer damages the relationship). **Recall** when false negatives are costly (missing actual fraud/late payment costs money) — fraud and late-payment detection typically optimize recall.

**Q18. RMSE vs MAE vs R² — when does each fit?**
**RMSE** when large errors should be penalized disproportionately (being off by 100 units of inventory is much worse than 10). **MAE** when all error magnitudes count equally. **R²** when you want to communicate how much variance the model explains.

**Q19. What is a VotingEnsemble's counterpart, the StackEnsemble?**
Voting averages/majority-votes the member models' outputs. **Stacking** trains a *meta-model* that learns how to best combine the member models' predictions as its inputs — one more learned layer instead of a fixed combination rule. AutoML builds both after individual trials complete.

**Q20. How does AutoML's model explainability output satisfy a Responsible AI requirement?**
Feature importance shows which inputs drove predictions (e.g., payment_terms_days weight 0.34). When a stakeholder asks "why was this dealer flagged?", you can answer with the actual driving factors — satisfying the **Transparency** principle. Enable it with `enable_model_explainability=True`.

**Q21. When is custom training the right call over AutoML?**
Complex custom architectures (e.g., custom deep learning), when you need full control over the algorithm, or when the problem doesn't fit AutoML's task types. Trade-off: days-to-weeks instead of hours, high data-science skill required, and explainability must be implemented yourself.

---

## Designer

**Q22. What is Azure ML Designer, and what's its core building block?**
A visual drag-and-drop interface for building ML pipelines with no code. The building block is a **component** — a pre-built operation (Import Data, Clean Missing Data, Split Data, Train Model, Score Model, Evaluate Model) connected on a canvas so data flows through them.

**Q23. In a Designer pipeline, why remove columns like `invoice_id` and `dealer_name` before training?**
They're identifiers, not predictive features — the model could memorize them (overfitting to specific dealers/invoices) instead of learning generalizable patterns. Feature selection keeps only columns with genuine predictive relationship to the target.

**Q24. Why normalize numeric columns like `invoice_amount` before training?**
Large scale differences between features (invoice_amount in tens of thousands vs. payment_terms in tens) bias some algorithms toward the larger-scaled feature. Normalizing to a common range (0–1) puts features on equal footing.

**Q25. What is a stratified split, and why use it on the label column?**
A train/test split that preserves the class ratio in both sets — if 20% of all invoices are late, both the 80% training set and the 20% test set keep that 20/80 ratio. Without it, a random split could under-represent the minority class in one set and distort both training and evaluation.

**Q26. Designer vs AutoML vs custom Python — one-line positioning for each.**
**Designer:** no code, you pick the algorithm visually — learning, prototypes, communicating with non-technical stakeholders. **AutoML:** no code, *it* picks the algorithm — fast model finding. **Custom Python:** full control in git — production custom models. JMA production = AutoML or SDK; Designer for prototyping.

**Q27. What can you do with a finished Designer pipeline beyond running it in Studio?**
Publish it as a **Pipeline Endpoint** — a REST URL callable from application code (e.g., C# `HttpClient` POST with parameter assignments), turning the visual pipeline into a reusable, invocable service.

---

## Deployment & Monitoring

**Q28. What two functions must a `score.py` scoring script implement, and what does each do?**
`init()` — runs **once** at container startup, loads the model from `AZUREML_MODEL_DIR` into a global. `run(raw_data)` — runs **per request**: parses the JSON payload, builds the feature array, predicts, returns prediction + probability.
*Memory hook: "init() once, run() per request."*

**Q29. In endpoint deployment, what's the difference between the *endpoint* and the *deployment*?**
The **endpoint** is the stable URL + auth + traffic-splitting config. The **deployment** is an actual model version + environment + compute behind it. One endpoint can host multiple deployments (v3, v4) with traffic split between them — which is exactly what enables blue-green rollout without changing the URL clients call.

**Q30. Why `instance_count=2` on a production online deployment when one instance handles the load?**
High availability — with a single instance, any VM restart/failure means downtime. Two instances let one carry traffic while the other recovers. (Capacity is a separate question; two is the availability *floor*.)

---

*Curriculum Q&A Batch A — file 1 of 3. Next: QA_L07 (Azure AI Services Deep Dive).*
