# FDE-Prep — Module List

**Created:** 2026-07-26
**Source:** the `Status → Module` column of the 60-row table in `FDE-Prep_Tracker.md`
**Purpose:** the flat answer to *"which files do I actually have to read?"*

> The tracker is authoritative on **status and order**. This file is a derived view —
> it deduplicates the Module column and sorts it. If the two disagree, the tracker wins.

**Legend:** 🟢 covered — revise · 🟠 written but not yet studied · 🔵 you already have it, no file

---

## All modules, ascending

| Module | Title | Status | Stage(s) |
|---|---|---|---|
| `L12` | Azure OpenAI Services | 🟢 revise | S4, S7 |
| `L13` | RAG Deep Dive | 🟢 revise | S5 |
| `L15` | Prompt Engineering | 🟢 revise | S4 |
| `L16` | AI Orchestration — SK & Agents | 🟢 revise | S4 |
| `L17` | Azure AI Foundry | 🟢 revise | S0 |
| `L18` | AI Solution Architecture | 🟢 revise | S5 |
| `L19` | MLOps & LLMOps | 🟢 revise | S4, S5, S6 |
| `L20` | Integration Patterns | 🟢 revise | S0, S5 |
| `L22` | Foundry Agent Lifecycle | 🟢 revise | S0 |
| `L25` | Agent Framework Comparison | 🟢 revise | S4 |
| `L26` | MCP — Model Context Protocol | 🟢 revise | S4 |
| `L27` | Agent Workflow End-to-End | 🟢 revise | S4 |
| `L28` | Meta-Agent Hierarchies | 🟢 revise | S4 |
| `L29` | A2A Protocol | 🟢 revise | S4 |
| `L31` | Fault Tolerance & Observability | 🟢 revise | S4, S5 |
| **`L32`** | **Advanced Python for AI** | 🟠 **new** | **S2** |
| **`L33`** | **IaC / Terraform for Bicep Devs** | 🟠 **new** | **S3** |
| **`L34`** | **Kubernetes, Helm & GitOps** | 🟠 **new** | **S6** |
| **`L35`** | **AI-Assisted Engineering** | 🟠 **new** | **S1** |
| **`L36`** | **LLM Observability & FinOps** | 🟠 **new** | **S5** |

## Non-`L##` material

Easy to miss — a filename search for `L##` will not find any of these.

| Item | Path | Status |
|---|---|---|
| `HLP01` | `02_Questions/HighLevelPrep/HLP01_Memory_Tokens_Scaling_Agents.md` | 🟢 revise |
| `VitalCare` | `05_Assessments/VitalCare_AI_Assessment_Response.md` | 🟢 revise |
| crewAI | `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/02-crewAI-MultiAgent/` | 🟢 revise |
| RAGAS | `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/03-RAGAS-Evaluation/` | 🟢 revise |
| VulnScan | `01_Lessons/Part6_AppliedProjects/03-SecurityAutomation-VulnScan/` | 🟠 new |

**Totals:** 20 `L##` files + 5 others = **25 items** · 15 revision · 5 genuinely new · 5 supporting

---

## ⚠️ Ascending is not study order

Read in numeric order and the 5-hour Python block lands before the 1-hour Cursor one.
The tracker's sequence exists for a reason:

```
S1  L35   AI-Assisted Engineering      1.7 hrs   ← Cursor first: doing, not reading. Clears 5 rows fast
S2  L32   Advanced Python for AI       6.0 hrs   ← biggest block; gates every live coding screen
S3  L33   IaC / Terraform              3.0 hrs   ← required-tier on the JD
          ─────────────────────────── 10.7 hrs · fits in one block
S4        agentic revision             2.7 hrs   ← 🟢 only — validating ground you already hold
S5  L36   LLM Observability & FinOps   2.8 hrs
S6  L34   Kubernetes, Helm & GitOps    3.2 hrs
S7        security & compliance revise 1.1 hrs
S8        AWS hands-on                 ongoing   ← no lesson closes this one
```

**The five bold modules are the whole job.** Everything else is re-reading.

---

## Not on this list

`L01`–`L11` · `L14` · `L21` · `L23` · `L24` · `L30`

Not gaps — this JD simply doesn't ask for them. `L21` is superseded by `L32` for
writing-level Python.

---

## Status log

| Date | Event |
|---|---|
| 2026-07-26 | Created from the tracker's Module column. Part 7 (`L32`–`L36`) confirmed present on disk; those rows are 🟠 (written, unstudied), not 🔴. |
