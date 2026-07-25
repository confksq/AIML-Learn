# Layer 2: Hub & Projects

The **organizational backbone** of AI Foundry — everything lives inside a Hub or Project.

---

## Position in Architecture

```
┌─────────────────────────────────────────────────┐
│           AI Foundry Portal (UI)                │
│  ══════════════════════════════════════════════ │
│  Layer 2 │ HUB                                  │  ← YOU ARE HERE
│          │  ├── Connections (OpenAI, Search..)  │
│          │  ├── Shared Compute & Storage        │
│          │  └── Projects                        │
│          │       ├── Project A                  │
│          │       └── Project B                  │
│  ─────────────────────────────────────────────  │
│  Layer 3 │ Model Catalog   (inside a Project)   │
│  ─────────────────────────────────────────────  │
│  Layer 4 │ AI Services & Tools (inside Project) │
│  ─────────────────────────────────────────────  │
│  Layer 5 │ Deployment & Monitoring              │
└─────────────────────────────────────────────────┘
```

---

## The Two-Level Hierarchy

```
Azure Subscription
 └── Resource Group
      └── AI Foundry HUB          ← IT/Platform team owns this
           ├── Shared Resources
           ├── Connections
           ├── Security & RBAC
           │
           ├── Project A           ← Dev Team / App team owns this
           ├── Project B
           └── Project C
```

---

## The HUB

The Hub is the **shared infrastructure layer** — created once by your platform/ops team.

```
HUB contains:
 ├── Connections        ← Azure OpenAI, Search, Storage keys
 ├── Compute            ← shared compute for training/fine-tuning
 ├── Storage Account    ← central blob storage
 ├── Key Vault          ← secrets management
 ├── Container Registry ← for custom model containers
 └── RBAC & Policies    ← who can do what
```

**JMA Context:** Your platform team would create one Hub per environment — matching your
`sb-jma-dev-apps`, `sb-jma-stg-apps`, `sb-jma-prod-apps` subscriptions.

---

## The PROJECT

A Project is a **working workspace** for a specific app or team initiative.

```
PROJECT contains:
 ├── Inherits Hub connections
 ├── Model Deployments    ← your deployed models
 ├── Prompt Flows         ← your AI logic/workflows
 ├── Data & Indexes       ← your uploaded/indexed data
 ├── Evaluations          ← your quality test runs
 └── Fine-tuned Models    ← your customized models
```

---

## Hub vs Project

| | Hub | Project |
|---|---|---|
| **Created by** | Platform/IT team | Dev team |
| **Frequency** | Once per environment | Many per Hub |
| **Purpose** | Shared infra & governance | Individual AI workload |
| **Connections** | Defined here | Inherited from Hub |
| **Cost center** | Org-level | Team-level |
| **Analogy** | Azure Resource Group | Individual App Service |

---

## Connections Flow (Defined at Hub, Used Everywhere)

```
Hub (Layer 2)
 └── Connections defined ONCE here
      ├── Azure OpenAI  ────────────────► used by Prompt Flow (L4)
      ├── Azure AI Search  ────────────► used by RAG (L4)
      ├── Azure Blob Storage  ─────────► used by Fine-tuning (L4)
      └── Custom APIs  ────────────────► used by any tool (L4/L5)

Project A  ──┐
Project B  ──┼──► all inherit Hub's connections automatically
Project C  ──┘
```

---

## RBAC — Who Gets What

```
Hub Level Roles:
 ├── Hub Owner       ← full control, manages connections/compute
 ├── Hub Contributor ← can create projects, use shared resources
 └── Hub Reader      ← view only

Project Level Roles:
 ├── Project Owner       ← full control within project
 ├── Project Contributor ← build flows, deploy models, run evals
 └── Project Reader      ← view results only
```

> You can be a **Project Owner** without touching Hub settings — teams stay isolated from each other.

---

## One-Line Summary

> **Hub** = shared infrastructure + governance. **Project** = your team's AI workspace.
> You build in Projects, you govern from the Hub.

---

## Knowledge Check

**Q: Your team at JMA is building a vehicle recommendation chatbot for `sb-jma-dev-apps`.
Would you create a new Hub or a new Project?**

<details>
<summary>Answer</summary>

**A new Project** — inside an existing Hub for the dev environment.
The Hub already exists (created by the platform team for `sb-jma-dev-apps`).
Your team creates a Project within it for the vehicle recommendation chatbot.
Creating a new Hub would mean duplicating shared infrastructure unnecessarily.

</details>

---

## Navigation

| | |
|---|---|
| **Previous** | [01 — Layer 1: Portal UI](01-Layer1-Portal-UI.md) |
| **Next** | `03-Layer3-Model-Catalog.md` *(coming soon)* |
