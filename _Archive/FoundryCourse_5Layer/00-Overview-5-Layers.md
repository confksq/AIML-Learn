# Azure AI Foundry — End-to-End Overview

## What is Azure AI Foundry?

Azure AI Foundry is Microsoft's **unified platform for building, deploying, and managing AI applications**.
It is the "Azure Portal for AI" — one place where data scientists, AI engineers, and app developers
collaborate to go from idea to production AI.

---

## The 5 Core Layers (End-to-End)

```
┌─────────────────────────────────────────────────┐
│           AI Foundry Portal (UI)                │  ← The window over everything
│  ══════════════════════════════════════════════ │
│  Layer 2 │ Hub & Projects   (org structure)     │
│  ─────────────────────────────────────────────  │
│  Layer 3 │ Model Catalog    (pick your model)   │
│  ─────────────────────────────────────────────  │
│  Layer 4 │ AI Services &    (build with it)     │
│          │ Tools                                │
│  ─────────────────────────────────────────────  │
│  Layer 5 │ Deployment &     (ship & observe)    │
│          │ Monitoring                           │
└─────────────────────────────────────────────────┘
```

> The Portal is **not a standalone layer** — it is the UI surface (glass panel) through which you
> interact with all layers below.

---

## Quick Summary of Each Layer

| Layer | What it is | Key Analogy |
|---|---|---|
| **Portal** | Web UI to manage everything | Like Azure Portal but AI-focused |
| **Hub & Projects** | Organizational containers | Hub = department, Project = team sprint |
| **Model Catalog** | Library of 1,700+ models (OpenAI, Meta, Mistral, etc.) | App Store for AI models |
| **AI Services & Tools** | Prompt Flow, RAG, Fine-tuning, Evaluation, Content Safety | Your AI dev toolkit |
| **Deployment & Monitoring** | Deploy as endpoints, track usage/drift/performance | App Service + App Insights for AI |

---

## AI Services & Tools — What's Inside Layer 4

| Component | Purpose |
|---|---|
| **Prompt Flow** | Visually chain prompts, logic, tools into workflows |
| **RAG / Grounding** | Connect your own data to the model |
| **Evaluation** | Score responses for quality, safety, relevance |
| **Fine-tuning** | Train a base model on your own data |
| **Content Safety** | Filter harmful inputs/outputs |
| **Azure AI Search** | The vector/semantic search engine under RAG |

---

## End-to-End Flow

```
You create a HUB
    └─> Create PROJECTS inside it
            └─> Browse MODEL CATALOG → deploy a model
            └─> Use PROMPT FLOW to chain logic
            └─> Add your data (RAG / Azure Search)
            └─> EVALUATE quality
            └─> DEPLOY as an API endpoint
            └─> MONITOR in production
```

---

## Connections — Cross-Cutting Concept

Connections are **saved credentials to external services**, defined once at the Hub level and
shared across all Projects and all layers.

```
Hub (Layer 2)
 └── Connections defined ONCE here
      ├── Azure OpenAI  ────────────────► used by Prompt Flow (L4)
      ├── Azure AI Search  ────────────► used by RAG (L4)
      ├── Azure Blob Storage  ─────────► used by Fine-tuning (L4)
      └── Custom APIs  ────────────────► used by any tool (L4/L5)
```

**.NET Analogy:** Connections = `appsettings.json` / Azure Key Vault references — defined once, injected everywhere.

---

## Concept Placement Summary

| Concept | Layer | Scope |
|---|---|---|
| **Portal** | Wrapper (UI) | Everything |
| **Hub** | Layer 2 | Org-wide |
| **Connections** | Layer 2 (Hub) | Shared across all projects |
| **Projects** | Layer 2 | Team/sprint level |
| **Model Catalog** | Layer 3 | Per project |
| **Tools (Flow/RAG/Eval)** | Layer 4 | Per project |
| **Deployments** | Layer 5 | Per project |

---

## Course Files

| File | Topic |
|---|---|
| `00-Overview-5-Layers.md` | This file — big picture & 5 layers |
| `01-Layer1-Portal-UI.md` | AI Foundry Portal deep dive |
| `02-Layer2-Hub-and-Projects.md` | Hub, Projects, RBAC |
