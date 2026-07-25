# Note: AI Foundry Portal Navigation vs Our 5 Layers

> Observation: The AI Foundry Portal UI does not label things as "5 layers" —
> it groups features by what you DO, not by architectural layer.
> This note maps what you see in the UI to our 5-layer mental model.

---

## Left Navigation — Mapped to 5 Layers

```
┌─────────────────────────────────────────────────────────────────┐
│  LEFT NAVIGATION                   MAPS TO                      │
│                                                                 │
│  Overview                      ──► Layer 2 (Project home)       │
│  ─────────────────────────────────────────────────────────────  │
│  Model catalog                 ──► Layer 3 (Model Catalog)       │
│  Playgrounds                   ──► Layer 3 (try models live)     │
│  ─────────────────────────────────────────────────────────────  │
│  Build and customize:                                           │
│   ├── Agents                   ──► Layer 4 (Prompt Flow/Agents)  │
│   ├── Templates                ──► Layer 4 (Prompt Flow)         │
│   ├── Fine-tuning              ──► Layer 4 (Fine-tuning)         │
│   └── Content Understanding    ──► Layer 4 (AI Services)         │
│  ─────────────────────────────────────────────────────────────  │
│  Observe and optimize:                                          │
│   ├── Tracing                  ──► Layer 5 (Monitoring)          │
│   └── Monitoring               ──► Layer 5 (Monitoring)          │
│  ─────────────────────────────────────────────────────────────  │
│  Protect and govern:                                            │
│   ├── Evaluation               ──► Layer 4 (Evaluation)          │
│   ├── Guardrails + controls    ──► Layer 4 (Content Safety)      │
│   ├── Risks + alerts           ──► Layer 5 (Monitoring)          │
│   └── Governance               ──► Layer 2 (Hub governance)      │
│  ─────────────────────────────────────────────────────────────  │
│  Azure OpenAI:                                                  │
│   ├── Stored completions       ──► Layer 5 (Deployment)          │
│   └── Batch jobs               ──► Layer 5 (Deployment)          │
│  ─────────────────────────────────────────────────────────────  │
│  My assets:                                                     │
│   ├── Data + Indexes           ──► Layer 4 (RAG & Grounding)     │
│   └── Models + endpoints       ──► Layer 5 (Deployment)          │
└─────────────────────────────────────────────────────────────────┘
```

---

## Why It Looks Different from Our 5 Layers

Microsoft organizes the UI by **what you do**, not by **what layer it is**:

| Microsoft's Grouping | Our Layer |
|---|---|
| **Build and customize** | Layer 4 (AI Services & Tools) |
| **Observe and optimize** | Layer 5 (Monitoring) |
| **Protect and govern** | Layer 4 (Evaluation + Safety) |
| **My assets** | Layer 3 + 5 |
| **Model catalog** | Layer 3 |

---

## You Are Always Inside a Project

When you land on `ai.azure.com` and it shows a project, check the breadcrumb:

```
Microsoft Foundry > ai-learn > Overview
                        │
                        └── ai-learn = your HUB (Layer 2)
                             └── AIML-Learn-Project = your PROJECT (Layer 2)
```

The Portal lands you inside your last used Project — that is why you see the
project overview instead of the portal home. The 5 layers are all present,
just spread across the left nav grouped by purpose.

---

## Why the Portal Skips the Home Page

The URL saved in your browser points directly to the project:

```
https://ai.azure.com/foundryProject/overview
    ?tid=e2ba673a...           ← your tenant
    &wsid=.../aiml-learn-resource   ← specific project

Fix: bookmark https://ai.azure.com (no parameters) for the home page
```

---

## Azure Portal vs AI Foundry Portal

When you go to `portal.azure.com` and open Microsoft Foundry — All Resources,
you see the **underlying Azure infrastructure** (not the AI workloads):

| Azure Portal Shows | AI Foundry Portal Shows |
|---|---|
| `aiml-learn-resource` (Foundry type) | Your Hub & Projects |
| Azure OpenAI resource | Model Catalog connections |
| AI Search resource | RAG & Grounding tool |
| Document Intelligence resource | Content Understanding tool |

> Azure Portal = the plumbing. AI Foundry Portal = the workbench on top of the plumbing.
