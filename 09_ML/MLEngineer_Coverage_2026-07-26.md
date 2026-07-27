# ML Engineer Role — Library Coverage Matrix

**Analysed:** 2026-07-26 · **Method:** grep-verified against `01_Lessons/`, `06_Supplementary/`
and `02_Questions/` — not inferred from `00_INDEX.md` or `00_CONTENTS.md`.

**Question answered:** *what does an ML Engineer role need to know, and how much of it does this
library actually teach?*

> **Legend**
> 🟢 **Covered** — taught at usable depth
> 🟡 **Partial** — mentions, definitions, or platform-level only; survives a question, not a probe
> 🔴 **Not covered** — absent, **or listed in a syllabus but never taught**

---

## Headline

**The library was built for an Azure AI Solutions Architect, not an ML Engineer.** Those roles
overlap on GenAI and operations and diverge almost completely on modelling, the data-science
library stack, and data engineering.

| Half of the ML Engineer role | Coverage |
|---|---|
| GenAI · agentic · RAG · LLMOps · Azure platform · architecture | **~85%** |
| Math foundations · classical ML · DL frameworks · data engineering · experiment tracking | **~20–25%** |

---

## The full matrix

| # | Area | Topic | Status | Where it lives / What's missing |
|---:|---|---|:--:|---|
| 1 | Math & Stats | Linear algebra (vectors, matrices) | 🔴 | Only `PythonTrack/AIMLcurriculum.md` — a syllabus, not a lesson |
| 2 | Math & Stats | Calculus, chain rule, derivatives | 🔴 | Same syllabus only |
| 3 | Math & Stats | Probability & distributions | 🔴 | Same syllabus only |
| 4 | Math & Stats | Bayes theorem | 🔴 | Same syllabus only |
| 5 | Math & Stats | Hypothesis testing, p-values, significance | 🔴 | Same syllabus only |
| 6 | Math & Stats | Gradient descent / backprop (conceptual) | 🟡 | `L11_3` — narrative, no math worked through |
| 7 | Programming | Python basics (C#→Python) | 🟢 | `L21` §21.1 |
| 8 | Programming | Type hints, dataclasses, Pydantic | 🟢 | `L32` §1 |
| 9 | Programming | Generators, decorators, context managers | 🟢 | `L32` §2–4 |
| 10 | Programming | Exceptions, design patterns | 🟢 | `L32` §5, §7 |
| 11 | Programming | Big-O / complexity | 🟢 | `L32` §6 |
| 12 | Programming | DSA interview patterns (two-pointer, DP, graphs) | 🟡 | `L32` §6 has complexity; the pattern drills are Gap 10, unbuilt |
| 13 | Programming | Async Python (`asyncio`, `httpx`, SSE) | 🟡 | `L21` §7 basic `async/await`; Gap 7 unbuilt |
| 14 | Programming | **NumPy** | 🔴 | Import lines only |
| 15 | Programming | **pandas** | 🔴 | `df.head()` / `df.describe()` only |
| 16 | Programming | Matplotlib / plotting | 🔴 | Absent |
| 17 | Programming | **SQL** (joins, windows, CTEs, plans) | 🔴 | **Gap 1 in `AIMLcurriculum-gaps.md`, never built** |
| 18 | Programming | Git, CI/CD pipelines | 🟢 | `L19` — Azure DevOps + GitHub Actions YAML |
| 19 | Programming | pytest / testing discipline | 🟡 | 3 mentions |
| 20 | Programming | Docker | 🟢 | `L34` |
| 21 | Programming | Kubernetes, Helm, GitOps | 🟢 | `L34` |
| 22 | Programming | Terraform / IaC | 🟢 | `L33` + `08_Jobs/FDE/IaC_Glossary_Azure_AWS_GCP.md` |
| 23 | Classical ML | ML taxonomy, workflow, features/labels | 🟢 | `L01` §1.2 |
| 24 | Classical ML | Train/val/test splits | 🟢 | `L01` §1.2.4 |
| 25 | Classical ML | Overfitting / underfitting | 🟢 | `L01` §1.2.5 |
| 26 | Classical ML | Core metrics (accuracy, precision, recall, F1) | 🟢 | `L01` §1.2.6 |
| 27 | Classical ML | Confusion matrix | 🟡 | 3 files, definitional |
| 28 | Classical ML | ROC-AUC / PR-AUC | 🟡 | 3 files, mostly AutoML metric selection |
| 29 | Classical ML | Bias-variance tradeoff | 🟡 | 1 file |
| 30 | Classical ML | Cross-validation | 🟡 | 3 files, definitional |
| 31 | Classical ML | Regularization | 🟡 | 3 files |
| 32 | Classical ML | Linear / logistic regression | 🟡 | 2–3 files, named not taught |
| 33 | Classical ML | Decision trees / random forest | 🟡 | Named in AutoML context |
| 34 | Classical ML | **XGBoost / LightGBM** | 🔴 | 5 lines, **all AutoML leaderboard output**. Never coded |
| 35 | Classical ML | **scikit-learn** | 🔴 | 4 env-pinning mentions + one ~15-line notebook-reading snippet in `L21` |
| 36 | Classical ML | Clustering / k-means | 🟡 | 2–3 files |
| 37 | Classical ML | PCA / dimensionality reduction | 🟡 | 1 file |
| 38 | Classical ML | Feature engineering | 🔴 | 3 passing mentions |
| 39 | Classical ML | Class imbalance / SMOTE | 🔴 | Zero hits |
| 40 | Classical ML | Data leakage | 🟡 | 6 files, mostly non-ML context |
| 41 | Classical ML | Hyperparameter tuning (Optuna) | 🔴 | Optuna zero hits; concept only via AutoML |
| 42 | Classical ML | Model calibration | 🔴 | Absent |
| 43 | Classical ML | AutoML (Azure), Designer pipelines | 🟢 | `L06` §6.2–6.3 |
| 44 | Deep Learning | **PyTorch** | 🔴 | Named in one interview-Q&A file + a syllabus. **No teaching** |
| 45 | Deep Learning | **TensorFlow** | 🔴 | One mention in `L04` |
| 46 | Deep Learning | Training loops, optimizers, mixed precision | 🔴 | Absent |
| 47 | Deep Learning | CNNs / convolution | 🔴 | 1 mention |
| 48 | Deep Learning | RNNs / LSTMs | 🔴 | 1–3 mentions |
| 49 | Deep Learning | **Transformers & attention** | 🟢 | `L11_1` |
| 50 | Deep Learning | Tokenization & embeddings | 🟢 | `L11_2` (578 ln) |
| 51 | Deep Learning | Transfer learning / fine-tuning | 🟢 | `L14` · `L11_3` · `P6/01-CareerAccelerator/08-LoRA-FineTuning/` |
| 52 | Deep Learning | Distributed training (DDP, DeepSpeed) | 🔴 | 1 passing mention; DeepSpeed zero |
| 53 | GenAI / LLM | LLM internals, pre-training | 🟢 | `L11_1`–`L11_3` |
| 54 | GenAI / LLM | RLHF & alignment | 🟢 | `L11_4` |
| 55 | GenAI / LLM | Prompt engineering | 🟢 | `L15` |
| 56 | GenAI / LLM | RAG (chunking, hybrid, rerank) | 🟢 | `L13` (1,527 ln) · `L23` CAG · GraphRAG module |
| 57 | GenAI / LLM | Vector DBs / FAISS | 🟢 | `L09` · Ollama module · `PythonTrack/Part1-AI-LLMs.md` |
| 58 | GenAI / LLM | Pinecone hands-on | 🔴 | 7 files, all decision tables |
| 59 | GenAI / LLM | Agents & orchestration (SK, LangChain, LangGraph, AutoGen, crewAI) | 🟢 | `L16` (2,084 ln) · `L25` · all of Part 5 — **strongest area** |
| 60 | GenAI / LLM | MCP · A2A · meta-agents | 🟢 | `L26` · `L29` · `L28` |
| 61 | GenAI / LLM | LLM eval (RAGAS, golden sets, LLM-judge) | 🟢 | `L19` §19.6 · RAGAS module |
| 62 | GenAI / LLM | Hallucination mitigation / guardrails | 🟢 | `L24` |
| 63 | GenAI / LLM | Azure OpenAI | 🟢 | `L12` |
| 64 | GenAI / LLM | **Anthropic Claude API (direct)** | 🔴 | Only via Bedrock; direct API absent |
| 65 | GenAI / LLM | Integrated multimodal app | 🔴 | Vision, speech, video exist separately; nothing combines them |
| 66 | GenAI / LLM | GANs, VAEs, diffusion | 🔴 | Gap 6, unbuilt |
| 67 | GenAI / LLM | Classic NLP (TF-IDF, Word2Vec, spaCy, NER) | 🟡 | Scattered in `L09` / `L11_2`; no standalone treatment |
| 68 | Data Eng | **Airflow / Dagster** | 🔴 | Zero hits |
| 69 | Data Eng | **Kafka / event streaming ingestion** | 🔴 | Zero hits |
| 70 | Data Eng | Spark / Databricks | 🟡 | Name-drops in architecture diagrams |
| 71 | Data Eng | ADF / Synapse | 🟡 | Referenced, no pipeline built |
| 72 | Data Eng | Delta / Parquet | 🟡 | Mentions |
| 73 | Data Eng | **Medallion / lakehouse** | 🔴 | Zero hits |
| 74 | Data Eng | Feature store | 🟡 | 4 mentions in `L06`/`L19`, never taught |
| 75 | Data Eng | Data quality / contracts / Great Expectations | 🔴 | Contracts + GE zero hits |
| 76 | Data Eng | Data versioning (DVC) | 🔴 | Syllabus only |
| 77 | MLOps | MLOps vs LLMOps, maturity model | 🟢 | `L19` |
| 78 | MLOps | Model registry & lifecycle | 🟢 | `L19` §19.2 (Azure ML / Foundry) |
| 79 | MLOps | CI/CD for ML | 🟢 | `L19` §19.3 |
| 80 | MLOps | Drift detection & retraining | 🟢 | `L19` §19.5 |
| 81 | MLOps | Monitoring, tracing, observability | 🟢 | `L19` §19.4 · `L36` |
| 82 | MLOps | Token/GPU cost & FinOps | 🟢 | `L36` |
| 83 | MLOps | Blue-green, canary, shadow, A/B | 🟢 | `L06` §6.4 · `L19` §19.6 |
| 84 | MLOps | Azure ML endpoints (online/batch) | 🟢 | `L06` §6.4 |
| 85 | MLOps | **MLflow** (tracking, registry, serving) | 🔴 | 4 one-line mentions |
| 86 | MLOps | Weights & Biases | 🔴 | Syllabus only |
| 87 | MLOps | **Model serving via FastAPI / Flask** | 🔴 | FastAPI appears in `L32` as a Pydantic example, not a serving lesson |
| 88 | MLOps | ONNX export | 🟡 | 12 mentions, no export-and-benchmark exercise |
| 89 | MLOps | Quantization | 🟡 | 45 mentions, LLM-inference framing |
| 90 | MLOps | Distillation / pruning | 🟡 | 6 mentions / pruning absent |
| 91 | MLOps | vLLM, TGI, Triton, TensorRT | 🔴 | 2 mentions / zero |
| 92 | MLOps | Batching strategies (dynamic, continuous) | 🔴 | Absent |
| 93 | Systems | AI solution architecture | 🟢 | `L18` |
| 94 | Systems | Integration patterns | 🟢 | `L20` |
| 95 | Systems | Fault tolerance, self-healing | 🟢 | `L31` |
| 96 | Systems | Azure platform depth | 🟢 | `L02` · `L06` · `L07` · `L17` · `L22` |
| 97 | Systems | AWS / GCP | 🟡 | Bedrock + Vertex AI modules — module-level, not platform depth |
| 98 | Systems | Security, identity, network isolation | 🟡 | Architecture-level across `L18`/Part 6; no standalone module |
| 99 | Responsible AI | RAI principles, fairness | 🟢 | `L01` §1.4 |
| 100 | Responsible AI | EU AI Act, governance | 🟢 | 5 files |
| 101 | Responsible AI | **Explainability (SHAP / LIME)** | 🔴 | Syllabus only — a naive `SHAP` grep returns 35 files because it matches the word *shape* |
| 102 | Responsible AI | Differential privacy | 🔴 | 1 mention |
| 103 | Responsible AI | Model cards | 🟡 | 2 mentions |
| 104 | Reinforcement Learning | MDP, Q-learning, DQN, policy gradients | 🔴 | Gap 8 unbuilt; PPO covered only inside RLHF (`L11_4`) |
| 105 | Domain / Soft | Business framing, stakeholder comms, roadmaps | 🟢 | `04_Career/` · `05_Assessments/VitalCare` (1,562 ln) |
| 106 | Domain / Soft | Interview prep & self-assessment | 🟢 | `02_Questions/InterviewBank/` · `HighLevelPrep/` · `08_Jobs/` |

---

## Tally

| Status | Count | Share |
|---|---:|---:|
| 🟢 Covered | 43 | 41% |
| 🟡 Partial | 25 | 24% |
| 🔴 Not covered | 38 | 36% |

**Read the matrix by column, not by row.** Every 🟢 clusters in GenAI/agentic, Azure platform,
MLOps concepts, and the Part 7 engineering modules. Every dense 🔴 block sits in four places:

- **Math foundations** — rows 1–5
- **The DS library stack** — rows 14–17, 34–35, 44–46
- **Data engineering** — rows 68–76
- **Experiment tracking + model serving** — rows 85–92

---

## Gap-only view — the 63 rows that are not green

Same data as the full matrix, filtered to 🔴 and 🟡 and re-sorted 🔴 first. `#` cross-references
the full matrix above. This is the study/build worklist.

| # | Status | Area | Topic | What's actually there |
|---:|:--:|---|---|---|
| 1 | 🔴 | Math & Stats | Linear algebra (vectors, matrices) | Syllabus only (`AIMLcurriculum.md`) |
| 2 | 🔴 | Math & Stats | Calculus, chain rule, derivatives | Syllabus only |
| 3 | 🔴 | Math & Stats | Probability & distributions | Syllabus only |
| 4 | 🔴 | Math & Stats | Bayes theorem | Syllabus only |
| 5 | 🔴 | Math & Stats | Hypothesis testing, p-values | Syllabus only |
| 14 | 🔴 | Programming | **NumPy** | Import lines only |
| 15 | 🔴 | Programming | **pandas** | `df.head()` / `df.describe()` only |
| 16 | 🔴 | Programming | Matplotlib / plotting | Absent |
| 17 | 🔴 | Programming | **SQL** (joins, windows, CTEs, plans) | **Gap 1, never built** |
| 34 | 🔴 | Classical ML | **XGBoost / LightGBM** | 5 lines, all AutoML leaderboard output |
| 35 | 🔴 | Classical ML | **scikit-learn** | 4 env-pinning mentions + a 15-line snippet |
| 38 | 🔴 | Classical ML | Feature engineering | 3 passing mentions |
| 39 | 🔴 | Classical ML | Class imbalance / SMOTE | Zero hits |
| 41 | 🔴 | Classical ML | Hyperparameter tuning (Optuna) | Optuna zero hits |
| 42 | 🔴 | Classical ML | Model calibration | Absent |
| 44 | 🔴 | Deep Learning | **PyTorch** | Named in one Q&A file + a syllabus. No teaching |
| 45 | 🔴 | Deep Learning | **TensorFlow** | One mention in `L04` |
| 46 | 🔴 | Deep Learning | Training loops, optimizers, mixed precision | Absent |
| 47 | 🔴 | Deep Learning | CNNs / convolution | 1 mention |
| 48 | 🔴 | Deep Learning | RNNs / LSTMs | 1–3 mentions |
| 52 | 🔴 | Deep Learning | Distributed training (DDP, DeepSpeed) | 1 mention / DeepSpeed zero |
| 58 | 🔴 | GenAI / LLM | Pinecone hands-on | 7 files, all decision tables |
| 64 | 🔴 | GenAI / LLM | **Anthropic Claude API (direct)** | Only via Bedrock |
| 65 | 🔴 | GenAI / LLM | Integrated multimodal app | Modalities exist separately, never combined |
| 66 | 🔴 | GenAI / LLM | GANs, VAEs, diffusion | Gap 6, unbuilt |
| 68 | 🔴 | Data Eng | **Airflow / Dagster** | Zero hits |
| 69 | 🔴 | Data Eng | **Kafka / event streaming ingestion** | Zero hits |
| 73 | 🔴 | Data Eng | **Medallion / lakehouse** | Zero hits |
| 75 | 🔴 | Data Eng | Data quality / contracts / Great Expectations | Zero hits |
| 76 | 🔴 | Data Eng | Data versioning (DVC) | Syllabus only |
| 85 | 🔴 | MLOps | **MLflow** (tracking, registry, serving) | 4 one-line mentions |
| 86 | 🔴 | MLOps | Weights & Biases | Syllabus only |
| 87 | 🔴 | MLOps | **Model serving via FastAPI / Flask** | FastAPI is a Pydantic example in `L32`, not serving |
| 91 | 🔴 | MLOps | vLLM, TGI, Triton, TensorRT | 2 mentions / zero |
| 92 | 🔴 | MLOps | Batching strategies (dynamic, continuous) | Absent |
| 101 | 🔴 | Responsible AI | **Explainability (SHAP / LIME)** | Syllabus only — grep false-positives on *shape* |
| 102 | 🔴 | Responsible AI | Differential privacy | 1 mention |
| 104 | 🔴 | Reinforcement Learning | MDP, Q-learning, DQN, policy gradients | Gap 8 unbuilt; PPO only inside RLHF |
| 6 | 🟡 | Math & Stats | Gradient descent / backprop | `L11_3` narrative, no math worked |
| 12 | 🟡 | Programming | DSA interview patterns | `L32` §6 has Big-O; drills are Gap 10 |
| 13 | 🟡 | Programming | Async Python (`asyncio`, `httpx`, SSE) | `L21` §7 basics; Gap 7 unbuilt |
| 19 | 🟡 | Programming | pytest / testing discipline | 3 mentions |
| 27 | 🟡 | Classical ML | Confusion matrix | 3 files, definitional |
| 28 | 🟡 | Classical ML | ROC-AUC / PR-AUC | 3 files, AutoML metric selection |
| 29 | 🟡 | Classical ML | Bias-variance tradeoff | 1 file |
| 30 | 🟡 | Classical ML | Cross-validation | 3 files, definitional |
| 31 | 🟡 | Classical ML | Regularization | 3 files |
| 32 | 🟡 | Classical ML | Linear / logistic regression | Named, not taught |
| 33 | 🟡 | Classical ML | Decision trees / random forest | Named in AutoML context |
| 36 | 🟡 | Classical ML | Clustering / k-means | 2–3 files |
| 37 | 🟡 | Classical ML | PCA / dimensionality reduction | 1 file |
| 40 | 🟡 | Classical ML | Data leakage | 6 files, mostly non-ML context |
| 67 | 🟡 | GenAI / LLM | Classic NLP (TF-IDF, Word2Vec, spaCy) | Scattered, no standalone treatment |
| 70 | 🟡 | Data Eng | Spark / Databricks | Name-drops in diagrams |
| 71 | 🟡 | Data Eng | ADF / Synapse | Referenced, no pipeline built |
| 72 | 🟡 | Data Eng | Delta / Parquet | Mentions |
| 74 | 🟡 | Data Eng | Feature store | 4 mentions, never taught |
| 88 | 🟡 | MLOps | ONNX export | 12 mentions, no exercise |
| 89 | 🟡 | MLOps | Quantization | 45 mentions, LLM-inference framing |
| 90 | 🟡 | MLOps | Distillation / pruning | 6 mentions / pruning absent |
| 97 | 🟡 | Systems | AWS / GCP | Bedrock + Vertex modules, not platform depth |
| 98 | 🟡 | Systems | Security, identity, network isolation | Architecture-level, no standalone module |
| 103 | 🟡 | Responsible AI | Model cards | 2 mentions |

### Where the gaps concentrate

| Area | 🔴 | 🟡 | Total gaps | Area size |
|---|--:|--:|--:|--:|
| Classical ML | 5 | 9 | **14** | 21 |
| Data Engineering | 5 | 4 | **9** | 9 ← *100% gap* |
| MLOps | 5 | 3 | **8** | 16 |
| Programming | 4 | 3 | **7** | 16 |
| Deep Learning | 6 | 0 | **6** | 9 |
| Math & Stats | 5 | 1 | **6** | 6 ← *100% gap* |
| GenAI / LLM | 4 | 1 | **5** | 15 |
| Responsible AI | 2 | 1 | **3** | 5 |
| Systems | 0 | 2 | **2** | 6 |
| Reinforcement Learning | 1 | 0 | **1** | 1 |

**Math & Stats and Data Engineering have zero green rows** — those two areas are entirely gap.
Classical ML is the largest absolute gap at 14 rows, but it is mostly 🟡: the concepts are in
`L01`, only the tooling is missing. That is why one notebook (build priority 1 below) moves so much
of it at once.

---

## Build plan — four modules convert 21 of the 38 red rows (plus 4 yellow → green)

| Priority | Build | Converts rows | Est. |
|:--:|---|---|---|
| 1 | **sklearn + XGBoost + MLflow pipeline**, end to end in a notebook | 34, 35, 38, 39, 41, 42, 85, 86 | ~8 hrs |
| 2 | **SQL module** (Gap 1) — joins, window functions, CTEs, execution plans | 17 | ~4 hrs |
| 3 | **PyTorch training-loop module** + NumPy/pandas hands-on | 14, 15, 16, 44, 46, 47, 48 | ~10 hrs |
| 4 | **Data-engineering module** — ADF/Databricks → Delta medallion → feature store | 68–76 | ~10 hrs |

Then, in descending value: ML math primer (1–5), SHAP/explainability (101), FastAPI serving (87),
RL basics (104).

**Priority 1 is the same item flagged independently in
`04_Career/JDCoverage_AIEngineer_ML_GenAI_2026-07-26.md`** as the highest-return unbuilt thing in
the library. Two separate role analyses landing on the same module is the strongest signal here.

---

## ⚠️ The syllabus trap

`06_Supplementary/PythonTrack/AIMLcurriculum.md` (558 ln) and `AIMLcurriculum-gaps.md` (110 ln)
*list* PyTorch, MLflow, SQL, ONNX, SHAP, RL, GANs and DSA. They are **bullet outlines of topics to
learn — not lessons.** Reading `00_INDEX.md` or those two files makes the library look like it
covers this material.

Every 🔴 above marked "syllabus only" is a row that a naive index read would score green.
`00_START_HERE.md` previously pointed at PythonTrack as coverage for *"PyTorch / ML math /
classical ML"*; that pointer was corrected on **2026-07-26**, not 2026-07-19 — the reorg of
2026-07-19 left the wrong pointer in place for a week. PythonTrack's real teaching files
(`1.4-FineTuning`, `1.5-AIAgents`, `Part1-AI-LLMs`) are all GenAI.

**Second grep trap, alongside the `SHAP`/*shape* one in row 101:** `EKS` matches "wEEKS" and
`Arize` matches "summARIZE". Use `grep -w` for any short or substring-prone term before scoring a
row green.

---

## Related

`04_Career/JDCoverage_AIEngineer_ML_GenAI_2026-07-26.md` — adjacent JD, same method, ~85% / ~20% split ·
`08_Jobs/FDE/FDE-Prep_Tracker.md` — Forward Deployed AI Engineer, 60 rows ·
`04_Career/JDCoverage_Synergech_Lorven_2026-07-19.md` — prior JDs ·
`01_Lessons/Part7_PlatformEngineering/` — `L32`–`L36`, the modules that closed the platform-engineering gaps
