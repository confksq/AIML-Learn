# FDE-Prep — Coverage Tracker

**Created:** 2026-07-26
**Codename:** `FDE-Prep` — say this in any session and the whole plan loads
**Source JD:** Forward Deployed AI Engineering team — AI-first infrastructure organization
(tracks: *Platform & Infrastructure FDE* · *Agentic Systems FDE* · *Principal FDE* future profile)
**Sprint window:** 2026-07-26 → 2026-07-27 noon (single block, no day-splits)
**Build status:** ✅ **All material built 2026-07-26** — Part 7 (`L32`–`L36`, 2,806 lines), `Part6/03` write-up, `HLP01` episodic section, `QA_L32`–`QA_L36`, `L19` §3 append. **0 rows now lack material.** What remains is *study*, not authoring.

> This file is the authoritative status for FDE-Prep. It supersedes any coverage
> claim made in chat. Coverage was verified by grep against the library on 2026-07-26,
> not assumed from `00_INDEX.md` alone.

---

## Legend

| Mark | Meaning | Your action | Time cost |
|---|---|---|---|
| 🟢 | **Library covers it** — a lesson file teaches it | Read / revise | minutes–hours |
| 🔵 | **You already have it** — day job or prior prep; no lesson exists | **Write the bullet** | ~10 min, zero study |
| 🟡 | **Partial** — concept taught, tool/flavour/depth missing | Depends — see decisions below | varies |
| 🟠 | **Module built, study pending** — material now exists; you have not read it | **Read the module** | see Hrs |
| 🔴 | **Gap** — no material anywhere | Nothing to read | — |

> **🟠 is not 🟢.** Writing `L33` did not make you able to write Terraform — that was the explicit
> warning when this was planned, and it still holds. A row goes 🟠 → 🟢 when you have studied the
> module *and* passed its `QA_L##` self-test. Rows 53 and 57 need something done, not read.

**🟢 vs 🔵 matters.** Green = *"I studied it"* — provable by pointing at a file.
Blue = *"I did it"* — provable by pointing at a cluster, a PR, a ticket. Blue is stronger in an
interview, but only once it's written down. That is why Stage 0 comes first and costs nothing.

**Cloud column values**

| Value | Meaning |
|---|---|
| **Agnostic** | Cloud-independent — learn once, works everywhere. Cheapest to study |
| **Both** | Exists on Azure *and* AWS, each with its own flavour to learn |
| **Both (AWS-pref)** | Works on both, but this JD prefers AWS — study it **AWS-first** |
| **Azure** / **AWS** / **GCP** | Vendor-specific |
| **On-prem** | Datacenter only (VMware) |
| **Hybrid** | Spans on-prem *and* cloud — that's its reason to exist (OpenShift: ARO on Azure, ROSA on AWS) |
| **—** | Not a technology |

---

## File locations — every reference resolved

All paths relative to the repo root (personal `C:\pers\AIML-Learn\` or office `C:\Users\confksq\Project\AIML-Learn\` — both are synced clones of `confksq/AIML-Learn`).
Line counts verified 2026-07-26. `file:line` anchors point at the exact section.

### Existing lessons (🟢 / 🟡 rows)

| Code | Full path | Lines | Read it for |
|---|---|---:|---|
| `L12` | `01_Lessons/Part3_GenAI_LLMs/L12_AzureOpenAI_Services.md` | 1,016 | SOC 2, FedRAMP, LLM-based dev |
| `L13` | `01_Lessons/Part3_GenAI_LLMs/L13_RAG_DeepDive.md` | 1,527 | Semantic caching, RAG |
| `L15` | `01_Lessons/Part3_GenAI_LLMs/L15_PromptEngineering.md` | 782 | Prompt engineering |
| `L16` | `01_Lessons/Part3_GenAI_LLMs/L16_AIOrchestration_SK_Agents.md` | **2,084** | Tool/function calling, SK agents, agentic RAG |
| `L17` | `01_Lessons/Part4_Architecture/L17_AzureAIFoundry.md` | 1,102 | Foundry, OpenTelemetry mention |
| `L18` | `01_Lessons/Part4_Architecture/L18_AISolutionArchitecture.md` | 509 | Agentic architecture pattern, semantic caching |
| `L19` | `01_Lessons/Part4_Architecture/L19_MLOps_LLMOps.md` | **757** | §3 CI/CD YAML · **:260 GitHub Actions (added 2026-07-26)** · **:530** prompt versioning · **:595** A/B testing |
| `L20` | `01_Lessons/Part4_Architecture/L20_IntegrationPatterns.md` | 602 | **:39–40** AKS vs Container Apps · semantic caching |
| `L21` | `01_Lessons/Part4_Architecture/L21_Python_for_AI.md` | 889 | §2 type hints · §5 functions · §6 classes · §7 async · §8 exceptions. **Read-level only** |
| `L22` | `01_Lessons/Part5_AgenticProtocols/L22_Foundry_AgentLifecycle.md` | 323 | Foundry agent lifecycle |
| `L25` | `01_Lessons/Part5_AgenticProtocols/L25_AgentFramework_Comparison.md` | 258 | **LangGraph, AutoGen, LangChain, SK** — the real framework lesson |
| `L26` | `01_Lessons/Part5_AgenticProtocols/L26_MCP_ModelContextProtocol.md` | 390 | MCP — only coverage in library |
| `L27` | `01_Lessons/Part5_AgenticProtocols/L27_Agent_Workflow_EndToEnd.md` | **762** | The centrepiece — end-to-end agent workflow |
| `L28` | `01_Lessons/Part5_AgenticProtocols/L28_MetaAgent_Hierarchies.md` | 164 | Supervisor / agents-of-agents |
| `L29` | `01_Lessons/Part5_AgenticProtocols/L29_A2A_Protocol.md` | 182 | A2A — only coverage in library |
| `L31` | `01_Lessons/Part5_AgenticProtocols/L31_FaultTolerance_Observability.md` | 422 | §2 Polly retry + circuit breaker · §3 self-healing · §4–5 three-layer observability · §7 LLMOps |
| `HLP01` | `02_Questions/HighLevelPrep/HLP01_Memory_Tokens_Scaling_Agents.md` | **361** | **Canonical memory architecture** — context vs session vs long-term vs state · **:100 episodic (added 2026-07-26)** |
| `VitalCare` | `05_Assessments/VitalCare_AI_Assessment_Response.md` | **1,562** | **:447** IaC row · **:911** FinOps governance · **:1441** 5-level autonomy ladder · **:1488–1500** ArgoCD rollback · **:1540–1560** cloud-agnostic tech stack |
| `IB/` | `02_Questions/InterviewBank/` | 6 files | `01_Fundamentals` · `02_Azure_AI_Platform` · `03_RAG_Architecture` · `04_Agent_Orchestration` · `05_Solution_Architecture` · `06_Responsible_AI_LLMOps` |
| `QA_L##` | `02_Questions/PerChapter/` | **24 files** | `QA_L06`–`QA_L21` and **`QA_L32`–`QA_L36`**. ⚠️ **None for L01–L05 or L22–L31** |

### Applied projects (Part 6)

> ⚠️ **Path correction.** The numbered tool modules live **inside** `01-CareerAccelerator/`, not
> directly under `Part6_AppliedProjects/`. Earlier shorthand in this file said `Part6/02-crewAI` —
> the real path is below.

| Code | Full path | Read it for |
|---|---|---|
| crewAI | `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/02-crewAI-MultiAgent/` | **Only CrewAI coverage you own.** `01_concepts.md` · `02_architecture.md` · `03_interview_qa.md` · `04_hands_on.py` · `05_resume_bullet.md` |
| RAGAS | `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/03-RAGAS-Evaluation/` | Non-deterministic evaluation. Same 6-file shape + `sample_questions.json` |
| Bedrock | `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/06-Amazon-Bedrock/` | **Your only real AWS material** — includes `azure_vs_bedrock_comparison.md` |
| Dealer platform | `01_Lessons/Part6_AppliedProjects/02-DealerIntelligence-Platform/` | C# 9-layer agentic platform + `JMA-DealerIntelligence-Complete-Flow.md` |
| VitalCare platform | `01_Lessons/Part6_AppliedProjects/05-VitalCare-AI-Platform/` | Same architecture, healthcare prior-auth domain |

### Supplementary — framework-free Python

| Code | Full path | Lines | Read it for |
|---|---|---:|---|
| PyTrack agents | `06_Supplementary/PythonTrack/1.5-AIAgents.md` | **1,981** | **Best real Python in the library.** Agent loop, ReAct, function-calling dispatcher, memory classes, multi-agent supervisor — all framework-free |
| PyTrack RAG | `06_Supplementary/PythonTrack/Part1-AI-LLMs.md` | 1,241 | FAISS + raw-Python RAG |
| PyTrack curriculum | `06_Supplementary/PythonTrack/AIMLcurriculum.md` | 558 | **§0.1 = a self-audit checklist, not a lesson.** Use it to mark what you cannot explain |
| PyTrack gaps | `06_Supplementary/PythonTrack/AIMLcurriculum-gaps.md` | 110 | Syllabus only — MLflow, ONNX, quantization, vLLM listed but **not taught** |

### Index files — start here in any session

| File | Lines | Answers |
|---|---:|---|
| `00_START_HERE.md` | 302 | What's current, what's where, what's missing. **Authoritative on progress** |
| `00_MAP.md` | 118 | What modules exist — one line each |
| `00_INDEX.md` | **3,532** | **Is topic X covered, how deeply, exactly where** — **1,145 concepts**, depth-marked ● ◐ ○ |
| `00_CONTENTS.md` | 2,986 | Every heading in every lesson, reading order |
| `01_Lessons/00_LearningIndex.md` | **238** | Learning order, all 36 modules / 7 Parts. ✅ **Refreshed 2026-07-26** (was stale at 20 modules) |

### Part 7 — built 2026-07-26 (🟠 rows)

| Module | Path | Lines |
|---|---|---:|
| `L32` | `01_Lessons/Part7_PlatformEngineering/L32_AdvancedPython_for_AI.md` | 762 |
| `L33` | `01_Lessons/Part7_PlatformEngineering/L33_IaC_Terraform_for_Bicep_Devs.md` | 647 |
| `L34` | `01_Lessons/Part7_PlatformEngineering/L34_Kubernetes_Helm_GitOps.md` | 501 |
| `L35` | `01_Lessons/Part7_PlatformEngineering/L35_AI_Assisted_Engineering.md` | 368 |
| `L36` | `01_Lessons/Part7_PlatformEngineering/L36_LLM_Observability_FinOps.md` | 528 |
| Vuln write-up | `01_Lessons/Part6_AppliedProjects/03-SecurityAutomation-VulnScan/` | 334 |
| Episodic memory | `02_Questions/HighLevelPrep/HLP01_Memory_Tokens_Scaling_Agents.md`:100 | +70 |
| Self-tests | `02_Questions/PerChapter/QA_L32.md` … `QA_L36.md` | 691 |
| GitHub Actions append | `01_Lessons/Part4_Architecture/L19_MLOps_LLMOps.md`:260 | +82 |
| This tracker | `08_Jobs/FDE/FDE-Prep_Tracker.md` | — |

### Companion reference — already written

| File | Path | Covers |
|---|---|---|
| **IaC Glossary** | `08_Jobs/FDE/IaC_Glossary_Azure_AWS_GCP.md` | DSL · cross-cloud · CFN · `tfstate` · Stack · full Azure/AWS/GCP/Terraform translation table · every IaC language · declarative vs imperative · IaC vs config-as-code · interview answers. **Read before rows 16–23 (S3).** |

---

## The 60 rows — in learning order

Stages group work that should be done together. Order inside a stage respects dependencies.

| Ord | Stage | # | Requirement | Status → Module | Cloud | Cat | Hrs |
|---:|---|---:|---|---|---|---|---:|
| 1 | **S0 · Claim** | 17 | Infrastructure as Code (concept) | 🔵 Bicep — write the bullet | Azure | Infra | 0 |
| 2 | S0 | 25 | DevOps & CI/CD | 🔵 Azure DevOps + YAML | Azure | Platform | 0 |
| 3 | S0 | 23 | Infrastructure engineering | 🔵 AKS · Key Vault · firewall egress · Postgres | Azure | Infra | 0 |
| 4 | S0 | 27 | Kubernetes — AKS | 🟢🔵 `L20` + live cluster, kubectl, PIM | Azure | Platform | 0 |
| 5 | S0 | 34 | Infra modernization / platform engineering | 🔵 day job | Azure | Platform | 0 |
| 6 | S0 | 58 | Azure AI Foundry / agent platform | 🔵🟢 `L17`, `L22` — already prepared | Azure | Resource | 0 |
| 7 | S0 | 60 | Mentoring / transformation catalyst | 🟢 `02_Questions/InterviewBank/07_Behavioral_Leadership.md` Q1–Q3 (mentoring, standards, influence without authority) — written 2026-08-03 | — | Soft | 0 |
| 8 | S0 | 59 | Problem solving · self-starter · communication | 🟢 `02_Questions/InterviewBank/` + `VitalCare` (101 KB solo) | — | Soft | 0 |
| 9 | **S1 · Tonight** | 53 | **Cursor AI** | 🟠 `L35` §2, **§7 = do it** — install, `.cursorrules`, ship 1 change | Agnostic | Tooling | 1.0 |
| 10 | S1 | 54 | GitHub Copilot **as coding practice** | 🟠 `L35` §1, §3 — library's 14 hits are all Copilot Studio/M365 | Both | Tooling | 0.25 |
| 11 | S1 | 56 | AI-first mindset / AI-assisted engineering | 🟠🔵 `L35` §4, §5 | Agnostic | Tooling | 0.1 |
| 12 | S1 | 55 | N8N | 🟠 `L35` §5.2 | Agnostic | Tooling | 0.1 |
| 13 | S1 | 12 | Anthropic computer-use | 🟠 `L35` §6 | Agnostic | Agent | 0.25 |
| 14 | **S2 · Python** | 15 | **Strong Python development** | 🟠 `L32` §1–§8 — decorators, generators, dataclasses, type hints, exceptions, DS&A + Big-O, design patterns | Agnostic | Code | 5.0 |
| 15 | S2 | 16 | Scripting & APIs | 🟠 `L32` §1, §4, §8 | Agnostic | Code | 1.0 |
| 16 | **S3 · IaC** | 18 | **Terraform / HCL** | 🟠 `L33` §1–§4 — *for a Bicep dev*: **state ownership**, HCL, plan/apply, modules | **Both (AWS-pref)** | Infra | 1.5 |
| 17 | S3 | 20 | AWS CDK | 🟠 `L33` §6 (CDK in **C#**) | AWS | Infra | 0.25 |
| 18 | S3 | 19 | Pulumi | 🟠 `L33` §7.1 | Both | Infra | 0.25 |
| 19 | S3 | 21 | Ansible / Puppet (config-as-code) | 🟠 `L33` §7.2 | Agnostic | Infra | 0.5 |
| 20 | S3 | 39 | VPC / PrivateLink | 🟠 `L33` §9.1 | AWS | Security | 0.25 |
| 21 | S3 | 38 | FedRAMP for LLM | 🟠 `L33` §9.2 | Both | Security | 0.1 |
| 22 | S3 | 24 | Cloud migration | 🟠 `L33` §8 (6 Rs, AI-assisted, `import`) | Both | Infra | 0.1 |
| 23 | S3 | 22 | VMware | 🟠 `L33` §7.3 awareness | On-prem | Infra | 0.05 |
| — | — | — | ⎯⎯⎯ **10.7 hrs · CUT LINE — this much fits today → noon** ⎯⎯⎯ | | | | |
| 24 | **S4 · Agentic** | 4 | Tool / function calling | 🟢 `L16`, `L26`, `L27` — self-test only | Both | Agent | 0.25 |
| 25 | S4 | 1 | LangGraph | 🟢 `L25` — StateGraph, Checkpointer, `interrupt_before`, code | Agnostic | Agent | 0.25 |
| 26 | S4 | 3 | AutoGen | 🟢 `L25` §5 | Agnostic | Agent | 0.1 |
| 27 | S4 | 2 | CrewAI | 🟢 `Part6_AppliedProjects/01-CareerAccelerator/02-crewAI-MultiAgent/` — only coverage you own | Agnostic | Agent | 0.25 |
| 28 | S4 | 6 | Multi-agent patterns | 🟢 `L27`, `L28`, `L29` + Part 6 platforms | Agnostic | Agent | 0.5 |
| 29 | S4 | 7 | Memory — short-term / long-term vector | 🟢 `HLP01` (canonical treatment) | Agnostic | Agent | 0.25 |
| 30 | S4 | 11 | **Memory — episodic** | 🟠 `HLP01`:100 — episodic section added | Agnostic | Agent | 0.25 |
| 31 | S4 | 5 | Orchestration & state — retry, checkpointing, failure modes | 🟢 `L31` §2–3 — Polly, circuit breaker, jitter, dead-letter replay | Agnostic | Agent | 0.25 |
| 32 | S4 | 8 | Human-in-the-loop approval | 🟢 `L25` + `VitalCare:1441` autonomy ladder | Agnostic | Agent | 0.1 |
| 33 | S4 | 9 | Non-deterministic output evaluation | 🟢 `Part6_AppliedProjects/01-CareerAccelerator/03-RAGAS-Evaluation/` + `L19` golden dataset | Agnostic | Agent | 0.25 |
| 34 | S4 | 10 | Agentic AI workflows & orchestration | 🟢 all of Part 5 (`L22`–`L31`) | Both | Agent | 0.1 |
| 35 | S4 | 13 | Prompt engineering | 🟢 `L15` | Both | AI | 0.1 |
| 36 | S4 | 14 | LLM-based software development | 🟢 `L12`, `L16`, Part 5 | Both | AI | 0.1 |
| 37 | **S5 · LLMOps** | 43 | Prompt versioning | 🟢 `L19:448`, `L31:241` | Both | LLMOps | 0.1 |
| 38 | S5 | 44 | A/B testing prompts | 🟢 `L19:513` | Both | LLMOps | 0.1 |
| 39 | S5 | 42 | Semantic caching | 🟢 `L13`, `L18`, `L20`, `HLP01` — 11 files | Both | LLMOps | 0.1 |
| 40 | S5 | 41 | LLM metrics | 🟢 `L31` §4–5 three-layer dashboard | Agnostic | LLMOps | 0.1 |
| 41 | S5 | 45 | Observability (general) | 🟢 `L31` §4–6 | Both | LLMOps | 0.1 |
| 42 | S5 | 46 | **OpenTelemetry** | 🟠 `L36` §2 — instrumenting agents, GenAI conventions, propagation | Both | LLMOps | 0.5 |
| 43 | S5 | 49 | **Tracing — LangSmith** | 🟠 `L36` §3.1 | Agnostic | LLMOps | 0.4 |
| 44 | S5 | 50 | **Tracing — Arize / Langfuse** | 🟠 `L36` §3.2–3.3 | Agnostic | LLMOps | 0.25 |
| 45 | S5 | 47 | **LiteLLM model routing** | 🟠 `L36` §4 | Agnostic | LLMOps | 0.25 |
| 46 | S5 | 48 | **FinOps / cost for LLM** | 🟠 `L36` §6 | Both | LLMOps | 0.5 |
| 47 | S5 | 52 | Grafana / Prometheus | 🟠 `L36` §7.1, §7.3 | Both | LLMOps | 0.25 |
| 48 | S5 | 51 | Dynatrace | 🟠 `L36` §7.2 | Both | LLMOps | 0.1 |
| 49 | **S6 · Platform** | 28 | **Helm** | 🟠 `L34` §2–§3 — chart anatomy, templating, release/revision, `--atomic` | Both | Platform | 1.0 |
| 50 | S6 | 29 | **ArgoCD / GitOps** | 🟠 `L34` §4 | Both | Platform | 0.75 |
| 51 | S6 | 26 | GitHub Actions | 🟢 `L19` §3 — **GitHub Actions section appended 2026-07-26**; your DevOps knowledge transfers | Both | Platform | 0.5 |
| 52 | S6 | 30 | Kubernetes — EKS | 🟠 `L34` §5 (IRSA, Karpenter) | AWS | Platform | 0.5 |
| 53 | S6 | 32 | Service mesh | 🟠 `L34` §6 | Both | Platform | 0.25 |
| 54 | S6 | 31 | Kubernetes — GKE | 🟠 `L34` §5 awareness | GCP | Platform | 0.1 |
| 55 | S6 | 33 | OpenShift | 🟠 `L34` §7 awareness | Hybrid | Platform | 0.1 |
| 56 | **S7 · Security** | 35 | HIPAA | 🟢🟢 24 files incl. `HIPAAGateway.cs`, `ClinicalAuditFilter.cs` | Both | Security | 0.25 |
| 57 | S7 | 36 | Encryption / data residency | 🟢 `VitalCare` | Both | Security | 0.1 |
| 58 | S7 | 37 | SOC 2 | 🟡 `L12` + `VitalCare` — **no module planned** | Both | Security | 0.25 |
| 59 | S7 | 40 | Security automation / vulnerability management | 🔵→🟠 `Part6_AppliedProjects/03-SecurityAutomation-VulnScan/` — **written, sanitised** | Azure | Security | 0.5 |
| 60 | **S8 · Open** | 57 | **AWS platform (JD says preferred)** | 🟡 **experience gap — no lesson fixes this** | AWS | Resource | ∞ |

---

## Complete FDE-Prep reading set — **extract from here, not from the 60 rows**

> ⚠️ **Why this section exists.** The 60-row table names a source only when a *row's status* points
> at one. Five items FDE-Prep genuinely needs never appear in a status cell — a prerequisite, a
> self-test set, an AWS module for a row no lesson can close, a practice file, and one citation that
> was not in backticks. Anyone extracting a module list from the rows alone silently loses them.
> **This table is the authoritative set.** 25 items.

### Lesson modules — 20

| # | Module | Status | Stage |
|---|---|---|---|
| L12 | Azure OpenAI Services | 🟢 revise | S4, S7 |
| L13 | RAG Deep Dive | 🟢 revise | S5 |
| L15 | Prompt Engineering | 🟢 revise | S4 |
| L16 | AI Orchestration — SK & Agents | 🟢 revise | S4 |
| L17 | Azure AI Foundry | 🟢 revise | S0 |
| L18 | AI Solution Architecture | 🟢 revise | S5 |
| L19 | MLOps & LLMOps | 🟢 revise | S4, S5, S6 |
| L20 | Integration Patterns | 🟢 revise | S0, S5 |
| L22 | Foundry Agent Lifecycle | 🟢 revise | S0 |
| L25 | Agent Framework Comparison | 🟢 revise | S4 |
| L26 | MCP | 🟢 revise | S4 |
| L27 | Agent Workflow End-to-End | 🟢 revise | S4 |
| L28 | Meta-Agent Hierarchies | 🟢 revise | S4 |
| L29 | A2A Protocol | 🟢 revise | S4 |
| L31 | Fault Tolerance & Observability | 🟢 revise | S4, S5 |
| **L32** | Advanced Python for AI | 🟠 new | **S2** |
| **L33** | IaC / Terraform for Bicep Devs | 🟠 new | **S3** |
| **L34** | Kubernetes, Helm & GitOps | 🟠 new | **S6** |
| **L35** | AI-Assisted Engineering | 🟠 new | **S1** |
| **L36** | LLM Observability & FinOps | 🟠 new | **S5** |

> `L21_Python_for_AI.md` is **deliberately absent** — `L32` supersedes it for writing-level Python.
> Keep it only as the C#→Python translation reference.

### Non-`L##` sources — 5

| Item | Path | Status | Stage |
|---|---|---|---|
| HLP01 — Memory, Tokens, Scaling, Agents | `02_Questions/HighLevelPrep/HLP01_Memory_Tokens_Scaling_Agents.md` | 🟢 revise · 🟠 `:100` episodic | S4 |
| VitalCare — assessment response | `05_Assessments/VitalCare_AI_Assessment_Response.md` | 🟢 revise | S0, S5, S6, S7 |
| crewAI multi-agent | `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/02-crewAI-MultiAgent/` | 🟢 revise | S4 |
| RAGAS evaluation | `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/03-RAGAS-Evaluation/` | 🟢 revise | S4 |
| Security automation / vuln scan | `01_Lessons/Part6_AppliedProjects/03-SecurityAutomation-VulnScan/` | 🟠 new | S7 |

> ⚠️ **Path-depth collision.** Two items above are both numbered `03-` but sit at **different
> levels** — RAGAS is inside `01-CareerAccelerator/`, the vuln write-up is directly under
> `Part6_AppliedProjects/`. Same for `02-crewAI-MultiAgent/` (inside) versus
> `02-DealerIntelligence-Platform/` (top level). Always carry the full path.

### The five the 60-row table misses — **added 2026-07-26**

| Item | Path | Why it is needed | Stage |
|---|---|---|---|
| **Interview bank** | `02_Questions/InterviewBank/` — 6 files | Cited in **row 59**, but the citation was not in backticks so extractions dropped it. Architect-judgment questions in WHY/HOW/WHEN/SCALE/DEPLOY format | **S0** |
| **Amazon Bedrock module** | `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/06-Amazon-Bedrock/` (incl. `azure_vs_bedrock_comparison.md`) | **Your only real AWS material**, and the JD says *"AWS preferred."* Row 57 cites no source because no lesson closes an experience gap — so it vanished from the row extraction | **S8** |
| **IaC glossary** | `08_Jobs/FDE/IaC_Glossary_Azure_AWS_GCP.md` | **Prerequisite to `L33`** — DSL, cross-cloud, CFN, `tfstate`, Stack, and the full Azure/AWS/GCP/Terraform translation. Read it *before* rows 16–23 | **S3** |
| **Self-tests** | `02_Questions/PerChapter/QA_L32`…`QA_L36.md` | **The only mechanism that moves 🟠 → 🟢.** Without them the five new modules stay amber regardless of how much you read | S1–S6 |
| **Framework-free Python practice** | `06_Supplementary/PythonTrack/1.5-AIAgents.md` — 1,981 lines | Best real Python in the library — agent loop, ReAct, function-calling dispatcher, memory classes, multi-agent supervisor. The practice half of `L32` | **S2** |

**Total: 20 modules + 5 non-`L##` sources + 5 supporting items = 25 things to work through.**

---

## Stage summary

| Stage | What | Hrs | Cumulative |
|---|---|---:|---:|
| **S0** | Claim what you already have — bullets, zero study | **0** | 0 |
| **S1** | Tonight — Cursor + AI tooling (doing, not reading) | **1.7** | 1.7 |
| **S2** | Advanced Python — biggest block, do with a fresh brain | **6.0** | 7.7 |
| **S3** | IaC / Terraform | **3.0** | **10.7 ← fits by noon** |
| **S4** | Agentic — self-test only, already prepared | **2.7** | 13.4 |
| **S5** | LLMOps / tracing / FinOps | **2.8** | 16.2 |
| **S6** | Platform — Helm, GitOps, EKS | **3.2** | 19.4 |
| **S7** | Security & compliance revise | **1.1** | 20.5 |
| **S8** | AWS hands-on | ongoing | — |

### Why this order

1. **S0 first and it costs nothing.** Eight rows are already yours — they are "gaps" only because
   they live in a repo and a chat history instead of a bullet. Cheapest points on the board.
2. **S1 tonight because it is *doing*, not reading.** Cursor installed and one change shipped turns
   four red rows green before you open a single lesson.
3. **S2 next, with a fresh brain.** Python is the single biggest block and the one that gates a live
   coding screen on *both* live JDs.
4. **S3 before S4** even though S4 is more interesting — Terraform is required-tier, agentic
   revision is validation of ground you already hold.
5. **S6 last of the study stages** — preferred-tier, and your YAML + Azure DevOps makes it the
   easiest to pick up late.
6. **S8 has no hours** because no lesson closes it. Free-tier AWS account, deploy something real.

---

## Status tally

| Mark | Rows | Meaning |
|---|---:|---|
| 🟢 | 25 | library covers — read/revise *(+1: row 26 GitHub Actions, now appended to `L19`)* |
| 🔵 | 9 | you have it — write the bullet |
| 🟡 | 6 | partial |
| 🟠 | **28** | **module built 2026-07-26 — study pending** |
| 🔴 | **0** | nothing left without material |

Counts exceed 60 because some rows carry two marks (#27 is 🟢🔵, #40 is 🔵→🟠, #47/#48 were 🟡→🟠,
#56 is 🟠🔵).

### What "done" means from here

| To move | You must |
|---|---|
| 🟠 → 🟢 | Read the module **and** pass its `QA_L##` self-test |
| 🔵 → banked | Write the bullet. ~10 min each, zero study — **Stage 0, still not done** |
| Row 53 → 🟢 | **Install Cursor and push one change.** `L35` §7 is a checklist, not a chapter |
| Row 57 → 🟢 | Free-tier AWS account and a real deploy. **No lesson closes this** |

**Authoring is finished. Everything remaining is yours to do.** The honest read: the library can now
answer every question this JD asks; you cannot yet, and ~20 hours of study is what separates those.

### Track scores

| Track | Score | Verdict |
|---|---|---|
| **Agentic Systems FDE** | **~85%** | **Apply — strong fit.** Part 5 + Part 6 + VitalCare cover nearly the whole spec |
| **Platform & Infrastructure FDE** | **~30% library / higher in practice** | Apply, but **lead with the day job**, not the library |
| **Required Skills** | **~50%** | 3 real blockers: Python depth · AWS hands-on · Cursor |
| **Preferred Skills** | **~50%** | — |

---

## Modules to be built

| Module | Path | Clears rows | Build est. |
|---|---|---|---:|
| `L32_AdvancedPython_for_AI.md` | 762 | `01_Lessons/Part7_PlatformEngineering/` | 15, 16 | 25 min |
| `L33_IaC_Terraform_for_Bicep_Devs.md` | 647 | `01_Lessons/Part7_PlatformEngineering/` | 18–22, 24, 38, 39 | 25 min |
| `L34_Kubernetes_Helm_GitOps.md` | 501 | `01_Lessons/Part7_PlatformEngineering/` | 28–33 | 20 min |
| `L35_AI_Assisted_Engineering.md` | 368 | `01_Lessons/Part7_PlatformEngineering/` | 12, 53–56 | 15 min |
| `L36_LLM_Observability_FinOps.md` | 528 | `01_Lessons/Part7_PlatformEngineering/` | 46–52 | 20 min |
| `03-SecurityAutomation-VulnScan/` | `01_Lessons/Part6_AppliedProjects/` | 40 | 15 min |
| `HLP01` episodic-memory addendum | `02_Questions/HighLevelPrep/` | 11 | 5 min |
| `QA_L32`–`QA_L36.md` | `02_Questions/PerChapter/` | — | 20 min |
| Index regeneration + 2 stale-index fixes | root + `01_Lessons/` + `07_ChatHistory/` | — | 30 min |
| | | **Total build** | **~2.6 hrs** |

### Partial-row decisions — new module vs update in place

| # | Partial | Decision | Reason |
|---|---|---|---|
| 16 | Scripting & APIs | **New → `L32`** | `L21`'s stated identity is *"C# dev fast-track, read-level."* Rewriting it to writing-level destroys its purpose. `L32` carries a banner: *"supersedes L21 for writing-level Python."* |
| 46 | OpenTelemetry | **New → `L36`** | `L17`'s OTel is one Foundry-tracing mention; `VitalCare:437` is an architecture table row |
| 47 | LiteLLM routing | **New → `L36`** | `VitalCare` uses it well but it is 6 scattered lines in a 1,562-line document |
| 48 | FinOps for LLM | **New → `L36`** | `VitalCare:911` stays as the governance example; `L36` teaches the mechanics |
| 26 | GitHub Actions | **Append → `L19` §3** | ~20 lines beside the existing Azure DevOps YAML. Not worth a module |
| 37 | SOC 2 | **No change** | A framework you cite in an architecture answer, not something you build |
| 57 | AWS platform | **No lesson possible** | Experience gap — needs an account and a real deploy |

> **Why new files rather than edits:** `00_INDEX.md` holds **1,145 citations anchored to line
> numbers** (`L19:513`, `L31:241`, `VitalCare:1441`). Inserting content mid-file shifts every line
> below it and silently breaks those citations — nothing errors, the index just starts pointing at
> the wrong lines. Rule: new file, or append-at-end-of-section followed by index regeneration.

---

## Known library defects found during this analysis

| File | Defect | Fix |
|---|---|---|
| `00_START_HERE.md` | Gap table points at `06_Supplementary/PythonTrack/` as coverage for *"PyTorch / ML math / classical ML."* Those files (`AIMLcurriculum.md`, `-gaps.md`) are **syllabi, not lessons** — bullet outlines of topics to learn. Actual teaching files there are all GenAI | Correct the pointer; add Part 7 |
| `01_Lessons/00_LearningIndex.md` | Titled *"AI Solutions Architect — Learning Index"* but **stale since 2026-06-30** — says 20 modules, stops at `L21`, does not know Part 5 or Part 6 exist | Refresh to 31→36 modules, 7 Parts |
| `07_ChatHistory/INDEX.md` | Stale since 2026-07-19 — missing 8 transcripts | Refresh |

## Grep hygiene note

Naive greps over-report coverage. Confirmed false positives on this library:

- `EKS` matches **wEEKS / sEEKS** — real hits: `VitalCare` only
- `Arize` matches **summARIZE / categoRIZE** — real hits: assessment files only

Use `grep -w` for short or substring-prone terms before concluding anything is covered.

---

## Open check questions (unanswered, from 2026-07-23)

1. **Vector search weakness** — *"customer wants to cancel"* vs *"customer does NOT want to cancel"*
   produce near-identical vectors. Why is that a problem, and what does it reveal?
2. **Scaling** — you add 20 replicas in prod and it gets *worse*. What happened, and what should
   have been scaled instead?
3. **Skills** — is the cancellation agent's refund calculation a good candidate to package as a Skill?
4. **Supervisor pattern** — an agent that checks another agent's work: which lesson, and what is the
   pattern called?

---

## Status log

| Date | Event |
|---|---|
| 2026-07-26 | Tracker created. Coverage verified by grep. 60 rows sorted into learning order. Build not yet started — awaiting green signal. |
| 2026-07-26 | Added **File locations** section — every code resolved to a full path with verified line counts. Fixed a wrong shorthand: the numbered tool modules sit inside `01-CareerAccelerator/`, not directly under `Part6_AppliedProjects/`. |
| 2026-07-26 | Created `08_Jobs/FDE/IaC_Glossary_Azure_AWS_GCP.md` — DSL, cross-cloud, CFN, tfstate, Stack + full Azure/AWS/GCP/Terraform translation, all IaC languages. |
| 2026-07-26 | ✅ **Part 7 built — all 5 modules, 2,806 lines.** `L32` (762) · `L33` (647) · `L34` (501) · `L35` (368) · `L36` (528). |
| 2026-07-26 | ✅ **Build complete.** `Part6/03-SecurityAutomation-VulnScan/` (334 lines, sanitised) · `HLP01` episodic-memory section (+70) · `QA_L32`–`QA_L36` (691 lines) · `L19` §3 GitHub Actions append (+82). Indexes regenerated: `00_INDEX` +124 concepts (1,145 total) with all shifted `L19:`/`HLP01:` citations corrected · `00_CONTENTS` +161 topics · `00_MAP` Part 7 · `00_START_HERE` Part 7 + gap-table fix. **All 3 stale indexes fixed** (`00_LearningIndex` 20→36 modules, `07_ChatHistory/INDEX` +8 transcripts, PythonTrack syllabus warning). |
| 2026-07-26 | **28 rows moved 🔴 → 🟠** (module built, study pending). Row 26 moved 🟡 → 🟢 (GitHub Actions appended to `L19` §3). Remaining: row 53 needs Cursor **installed and a change pushed**; row 57 (AWS hands-on) needs an account. Neither is closable by authoring. |
| 2026-07-26 | Tracker refreshed to match reality: **🟠 state introduced** (a built module is not a studied one), all 60 rows re-marked with section anchors, stale line counts corrected (`L19` 675→757, `HLP01` 291→361, `00_INDEX` 3,157→3,532, `00_LearningIndex` STALE→refreshed), "to be built" → "built". |
| 2026-07-26 | Added **Complete FDE-Prep reading set** (25 items) after a module list extracted from the 60 rows came back missing five: `InterviewBank/` (citation was not in backticks), `06-Amazon-Bedrock/` (row 57 cites no source — no lesson closes an experience gap), the **IaC glossary** (prerequisite to `L33`), `QA_L32`–`QA_L36` (the only 🟠→🟢 mechanism), and `PythonTrack/1.5-AIAgents.md`. Row 59 re-cited with a backticked path. **Extract module lists from that section, not from the 60 rows.** |
| 2026-07-26 | Created `04_Career/JDCoverage_AIEngineer_ML_GenAI_2026-07-26.md` for the second live JD (ML + GenAI + Agentic). No separate FDE coverage doc — **this tracker is it**; a duplicate would drift. Flagged there: the biggest library gap *not* closed by Part 7 is a real sklearn + XGBoost + MLflow pipeline (~8 hrs). |
| 2026-07-28 | Added **Quick Reference** table at the bottom — module, status, full path, one row each, for anyone who forgot where things live and doesn't want to parse the 60-row table. |
| 2026-07-30 | Added **JD Requirement → Tracker Row Map** at the very end — every bullet from the pasted JD text checked against the 60-row table by section (Responsibilities, Required Skills, Preferred Skills, Platform & Infrastructure FDE, Agentic Systems FDE). Confirms the pasted JD is the same one the 60 rows were built from — zero uncovered requirements; two role-behavior/outcome bullets flagged as proof-by-doing, not gaps. |

---

## Quick Reference — Module, Status, Full Path

**Base:** repo root — personal `C:\pers\AIML-Learn\` or office `C:\Users\confksq\Project\AIML-Learn\` (synced clones of `confksq/AIML-Learn`)
**Legend:** 🟢 revise · 🟠 written, study pending (🟠→🟢 needs the module read **and** its `QA_L##` passed)

| # | Module | Status | Full path |
|---|---|:--:|---|
| L12 | Azure OpenAI Services | 🟢 | `01_Lessons\Part3_GenAI_LLMs\L12_AzureOpenAI_Services.md` |
| L13 | RAG Deep Dive | 🟢 | `01_Lessons\Part3_GenAI_LLMs\L13_RAG_DeepDive.md` |
| L15 | Prompt Engineering | 🟢 | `01_Lessons\Part3_GenAI_LLMs\L15_PromptEngineering.md` |
| L16 | AI Orchestration — SK & Agents | 🟢 | `01_Lessons\Part3_GenAI_LLMs\L16_AIOrchestration_SK_Agents.md` |
| L17 | Azure AI Foundry | 🟢 | `01_Lessons\Part4_Architecture\L17_AzureAIFoundry.md` |
| L18 | AI Solution Architecture | 🟢 | `01_Lessons\Part4_Architecture\L18_AISolutionArchitecture.md` |
| L19 | MLOps & LLMOps | 🟢 | `01_Lessons\Part4_Architecture\L19_MLOps_LLMOps.md` |
| L20 | Integration Patterns | 🟢 | `01_Lessons\Part4_Architecture\L20_IntegrationPatterns.md` |
| L22 | Foundry Agent Lifecycle | 🟢 | `01_Lessons\Part5_AgenticProtocols\L22_Foundry_AgentLifecycle.md` |
| L25 | Agent Framework Comparison | 🟢 | `01_Lessons\Part5_AgenticProtocols\L25_AgentFramework_Comparison.md` |
| L26 | MCP | 🟢 | `01_Lessons\Part5_AgenticProtocols\L26_MCP_ModelContextProtocol.md` |
| L27 | Agent Workflow End-to-End | 🟢 | `01_Lessons\Part5_AgenticProtocols\L27_Agent_Workflow_EndToEnd.md` |
| L28 | Meta-Agent Hierarchies | 🟢 | `01_Lessons\Part5_AgenticProtocols\L28_MetaAgent_Hierarchies.md` |
| L29 | A2A Protocol | 🟢 | `01_Lessons\Part5_AgenticProtocols\L29_A2A_Protocol.md` |
| L31 | Fault Tolerance & Observability | 🟢 | `01_Lessons\Part5_AgenticProtocols\L31_FaultTolerance_Observability.md` |
| **L32** | Advanced Python for AI | 🟠 | `01_Lessons\Part7_PlatformEngineering\L32_AdvancedPython_for_AI.md` |
| **L33** | IaC / Terraform for Bicep Devs | 🟠 | `01_Lessons\Part7_PlatformEngineering\L33_IaC_Terraform_for_Bicep_Devs.md` |
| **L34** | Kubernetes, Helm & GitOps | 🟠 | `01_Lessons\Part7_PlatformEngineering\L34_Kubernetes_Helm_GitOps.md` |
| **L35** | AI-Assisted Engineering | 🟠 | `01_Lessons\Part7_PlatformEngineering\L35_AI_Assisted_Engineering.md` |
| **L36** | LLM Observability & FinOps | 🟠 | `01_Lessons\Part7_PlatformEngineering\L36_LLM_Observability_FinOps.md` |
| — | HLP01 — Memory/Tokens/Scaling/Agents | 🟢 | `02_Questions\HighLevelPrep\HLP01_Memory_Tokens_Scaling_Agents.md` |
| — | VitalCare assessment | 🟢 | `05_Assessments\VitalCare_AI_Assessment_Response.md` |
| — | crewAI multi-agent | 🟢 | `01_Lessons\Part6_AppliedProjects\01-CareerAccelerator\02-crewAI-MultiAgent\` |
| — | RAGAS evaluation | 🟢 | `01_Lessons\Part6_AppliedProjects\01-CareerAccelerator\03-RAGAS-Evaluation\` |
| — | Security automation / vuln scan | 🟠 | `01_Lessons\Part6_AppliedProjects\03-SecurityAutomation-VulnScan\` |
| — | Interview bank | 🟢 | `02_Questions\InterviewBank\` |
| — | Amazon Bedrock (only real AWS) | 🟢 | `01_Lessons\Part6_AppliedProjects\01-CareerAccelerator\06-Amazon-Bedrock\` |
| — | IaC glossary *(read before L33)* | 🟢 | `08_Jobs\FDE\IaC_Glossary_Azure_AWS_GCP.md` |
| — | Self-tests | 🟠 | `02_Questions\PerChapter\QA_L32.md` … `QA_L36.md` |
| — | Python practice | 🟢 | `06_Supplementary\PythonTrack\1.5-AIAgents.md` |

> `L21_Python_for_AI.md` is deliberately absent — `L32` supersedes it for writing-level Python.
> Study order is not numeric: `L35 → L32 → L33` first (fits one block, ~10.7 hrs), rest after.

---

## JD Requirement → Tracker Row Map — verify full coverage (added 2026-07-30)

> **Purpose.** The JD text pasted in on 2026-07-30 was checked bullet-by-bullet against the 60-row
> table. Every line maps onto a `#` this tracker already scores — this is the same JD (or one
> functionally identical to it) the 60 rows were built from. Use this section to confirm nothing
> was missed; cross-check `#` against the 60-row table or the Quick Reference above for exact
> status/module. Two rows have **no** `#` — flagged explicitly so they aren't mistaken for gaps.

### Responsibilities

| JD requirement | Tracker # | Status | Module / note |
|---|---|:--:|---|
| Embed within infrastructure teams; work with business/infra leaders | — | — | **Role behavior, not a study row.** No lesson closes this — demonstrated on the job, not read |
| Identify automation opportunities independently, without waiting for instructions | #59 | 🟢 | Problem solving / self-starter / communication — `InterviewBank/` + `VitalCare` |
| Build AI-assisted solutions using Cursor and GitHub Copilot | #53, #54 | 🟠 | `L35` §2, §7 (Cursor — install + ship 1 change) · `L35` §1, §3 (Copilot as coding practice) |
| Develop infrastructure automation and build AI workflows/agents | #17, #25, #23, #10 | 🔵🔵🔵🟢 | IaC concept · DevOps & CI/CD · Infra engineering (all Bicep/day-job) · `L22`–`L31` agentic workflows |
| Modernize infrastructure platforms, improve cloud migration efficiency | #34, #24 | 🔵🟠 | day job (platform eng) · `L33` §8 — 6 Rs, AI-assisted migration, `terraform import` |
| Build vulnerability remediation automation | #40 | 🔵→🟠 | `Part6_AppliedProjects/03-SecurityAutomation-VulnScan/` — written, sanitised |
| Demonstrate PoCs rapidly; deliver measurable productivity improvements | #53, #56 | 🟠 | `L35` §7 (ship 1 change), §4–5 — **outcome bullet, no dedicated lesson**; provable only by doing S1 |
| Mentor engineering teams; promote AI adoption | #60 | 🔵 | day job — write the bullet (Stage 0, zero study) |

### Required Skills & Experience

| JD requirement | Tracker # | Status | Module / note |
|---|---|:--:|---|
| Strong Python development, scripting, and APIs | #15, #16 | 🟠 | `L32` §1–§8 (decorators, generators, DS&A, design patterns) · `L32` §1,§4,§8 (scripting/APIs) |
| Infra & cloud: infra engineering, cloud (AWS preferred), IaC, DevOps, automation | #23, #57, #17, #18, #25 | 🔵🟡🔵🟠🔵 | Infra engineering (day job) · **AWS platform — experience gap, no lesson fixes it** · IaC concept · `L33` Terraform · DevOps & CI/CD |
| AI-assisted engineering: Copilot, Cursor, prompt engineering, LLM-based dev, AI-assisted coding | #54, #53, #13, #14, #56 | 🟠🟠🟢🟢🟠 | `L35` (Copilot, Cursor) · `L15` prompt engineering · `L12`/`L16`/Part 5 (LLM-based dev) · `L35` §4-5 |
| Strong problem solving, self-starter mindset, excellent communication | #59 | 🟢 | `InterviewBank/` + `VitalCare` (101 KB solo) |

### Preferred Skills

| JD requirement | Tracker # | Status | Module / note |
|---|---|:--:|---|
| Agentic AI workflows and AI orchestration | #10 | 🟢 | All of Part 5 (`L22`–`L31`) |
| Infrastructure modernization and platform engineering | #34 | 🔵 | day job |
| Security automation and vulnerability management | #40 | 🔵→🟠 | Vuln-scan write-up |
| FinOps and observability | #48, #45 | 🟠🟢 | `L36` §6 (FinOps) · `L31` §4–6 (observability) |
| Cloud migration | #24 | 🟠 | `L33` §8 |

### Platform & Infrastructure FDE

| JD requirement | Tracker # | Status | Module / note |
|---|---|:--:|---|
| IaC — Terraform, Pulumi, AWS CDK, VMware, OpenShift | #17, #18, #19, #20, #22, #33 | 🔵🟠🟠🟠🟠🟠 | `L33` §1–§4 (Terraform) · §7.1 (Pulumi) · §6 (CDK in C#) · §7.3 (VMware awareness) · §7 (OpenShift awareness) |
| Configuration-as-Code — Ansible, Puppet | #21 | 🟠 | `L33` §7.2 |
| CI/CD — GitHub Actions, ArgoCD | #26, #29 | 🟢🟠 | `L19` §3 (appended 2026-07-26) · `L34` §4 |
| Kubernetes — EKS/GKE/AKS, Helm, service mesh | #27, #30, #31, #28, #32 | 🟢🔵🟠🟠🟠 | `L20` + live AKS cluster · `L34` §5 (EKS: IRSA, Karpenter) · §5 (GKE awareness) · §2–3 (Helm) · §6 (service mesh) |
| Observability — OpenTelemetry, Dynatrace, Grafana, LLM metrics | #46, #51, #52, #41 | 🟠🟠🟠🟢 | `L36` §2 · §7.2 · §7.1/7.3 · `L31` §4–5 |
| Cloud security — VPC, PrivateLink, encryption, data residency | #39, #36 | 🟠🟢 | `L33` §9.1 · `VitalCare` |
| Compliance — SOC 2, HIPAA, FedRAMP for LLM | #37, #35, #38 | 🟡🟢🟠 | `L12`+`VitalCare` (no module planned) · 24 files incl. `HIPAAGateway.cs` · `L33` §9.2 |
| Agentic frameworks — LangGraph, CrewAI, N8N | #1, #2, #55 | 🟢🟢🟠 | `L25` StateGraph/Checkpointer · `02-crewAI-MultiAgent/` · `L35` §5.2 |
| LLMOps — LiteLLM routing, semantic caching, prompt versioning, A/B testing | #47, #42, #43, #44 | 🟠🟢🟢🟢 | `L36` §4 · `L13`/`L18`/`L20`/`HLP01` · `L19:448`,`L31:241` · `L19:513` |
| Cost / FinOps for LLM | #48 | 🟠 | `L36` §6 |

### Agentic Systems FDE

| JD requirement | Tracker # | Status | Module / note |
|---|---|:--:|---|
| Agentic frameworks — LangGraph, CrewAI, AutoGen, Anthropic computer-use | #1, #2, #3, #12 | 🟢🟢🟢🟠 | `L25` · `02-crewAI-MultiAgent/` · `L25` §5 · `L35` §6 |
| Tool / function calling | #4 | 🟢 | `L16`, `L26`, `L27` — self-test only |
| Orchestration & state — retry, checkpointing, failure modes | #5 | 🟢 | `L31` §2–3 — Polly, circuit breaker, jitter, dead-letter replay |
| Multi-agent patterns | #6 | 🟢 | `L27`, `L28`, `L29` + Part 6 platforms |
| Memory — short-term, long-term vector, episodic | #7, #11 | 🟢🟠 | `HLP01` canonical treatment · `HLP01`:100 episodic section |
| Human-in-the-loop approval workflows | #8 | 🟢 | `L25` + `VitalCare`:1441 autonomy ladder |
| Non-deterministic output evaluation | #9 | 🟢 | `03-RAGAS-Evaluation/` + `L19` golden dataset |
| Tracing — LangSmith, Arize, OpenTelemetry | #49, #50, #46 | 🟠🟠🟠 | `L36` §3.1 · §3.2–3.3 · §2 |

### Net read on this pass

- **Zero rows uncovered.** Every bullet either has an existing 🟢/🔵/🟠 row, or is one of the two
  flagged non-rows above (role behavior; outcome bullet) which were never meant to be lessons.
- The two flagged items (embedding with teams; measurable productivity gains) are **proof-by-doing**,
  not proof-by-reading — the same S0/S1 logic already in this tracker (write the bullet, ship the
  change), not a new gap.
- This re-confirms the tracker's own verdict: **Required Skills ~50%**, blocked by the same three
  items as before — **Python depth, AWS hands-on, Cursor** — nothing in this pass surfaces a fourth.
