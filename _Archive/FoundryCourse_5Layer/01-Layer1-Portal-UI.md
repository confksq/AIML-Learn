# Layer 1: AI Foundry Portal (The UI)

**URL:** `ai.azure.com` (formerly Azure AI Studio)

The Portal is the **command center** — everything you do in AI Foundry starts here.
It is not a standalone layer but the **glass panel over all 5 layers** — every action
in Layers 2–5 happens through the Portal.

---

## Portal Position in the Architecture

```
┌─────────────────────────────────────────────────┐
│           AI Foundry Portal (UI)                │  ← YOU ARE HERE
│                                                 │
│  The WINDOW through which you interact          │
│  with ALL the layers below                      │
│                                                 │
│  Home | Explore | Build                         │
└────────────────┬────────────────────────────────┘
                 │  (you see & control everything below through the portal)
                 ▼
┌─────────────────────────────────────────────────┐
│  Layer 2 │ Hub & Projects                       │
│  Layer 3 │ Model Catalog                        │
│  Layer 4 │ AI Services & Tools                  │
│  Layer 5 │ Deployment & Monitoring              │
└─────────────────────────────────────────────────┘
```

---

## The Three Main Sections

```
┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│  Home       │  │  Explore    │  │  Build      │
│  (overview) │  │  (catalog)  │  │  (projects) │
└─────────────┘  └─────────────┘  └─────────────┘
```

| Section | What's there |
|---|---|
| **Home** | Your recent projects, hubs, quick-start guides |
| **Explore** | Model catalog, benchmarks, Azure AI Services gallery |
| **Build** | Your actual workspace — projects, deployments, prompt flows |

---

## What the Portal Manages

```
Portal
 ├── Hubs & Projects      ← create/manage workspaces
 ├── Model Deployments    ← see what's running, endpoints, costs
 ├── Prompt Flows         ← visual flow builder lives here
 ├── Data & Indexes       ← upload files, connect storage
 ├── Evaluations          ← run and view eval results
 ├── Content Filters      ← configure safety policies
 └── Settings & Access    ← RBAC, connections, keys
```

---

## Important Concept: Connections

The Portal manages **Connections** — saved credentials to external services — configured under
the Hub (Layer 2) and reused across all projects.

| Connection Type | Used For |
|---|---|
| Azure OpenAI | Calling GPT models |
| Azure AI Search | Powering RAG / semantic search |
| Azure Blob Storage | Storing training data, files |
| GitHub / Custom APIs | Integrating external tools |

**.NET Analogy:** Connections = Connection Strings in `appsettings.json` or Key Vault references.
Defined once, consumed everywhere — no hardcoding per service.

---

## Portal vs Azure Portal

| | Azure Portal | AI Foundry Portal |
|---|---|---|
| **Focus** | All Azure resources | AI workloads only |
| **Audience** | Infra / DevOps / Developers | AI Engineers / Data Scientists |
| **Creates** | VMs, DBs, networks... | Models, flows, evals... |
| **Relationship** | AI Foundry Hub is just another resource here | Specialized view of AI resources |

> You still provision underlying Azure resources (Storage, Search, OpenAI) via Azure Portal or
> Bicep/ARM — AI Foundry Portal gives you the **AI-focused experience** on top of those resources.

---

## One-Line Summary

> The AI Foundry Portal is your **AI workbench UI** — where you organize projects, explore models,
> build prompt flows, evaluate quality, and monitor deployments, all without leaving one browser tab.

---

## Navigation

| | |
|---|---|
| **Previous** | [00 — Overview & 5 Layers](00-Overview-5-Layers.md) |
| **Next** | [02 — Layer 2: Hub & Projects](02-Layer2-Hub-and-Projects.md) |
