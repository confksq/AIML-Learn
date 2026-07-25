# 02 — Architecture: crewAI 3-Agent Research Pipeline

## The pipeline (sequential process)

```
  ┌──────────────┐
  │ User Input   │   e.g. --topic "Azure AI Foundry"
  │ (topic)      │
  └──────┬───────┘
         │
  ┌──────▼─────────────────────────────────────────────────────────────┐
  │ CREW (Process.sequential)  — the orchestrator                       │
  │                                                                     │
  │  ┌────────────────────┐   Task 1: research the topic                │
  │  │ Researcher Agent   │──▶ produces structured findings ───────┐    │
  │  │ role/goal/backstory│                                        │    │
  │  └────────────────────┘                                        │    │
  │                                                                ▼    │
  │  ┌────────────────────┐   Task 2: write a report (context = findings)│
  │  │ Writer Agent       │──▶ produces a formatted draft report ──┐    │
  │  └────────────────────┘                                        │    │
  │                                                                ▼    │
  │  ┌────────────────────┐   Task 3: review & finalize (context = draft)│
  │  │ Reviewer Agent     │──▶ validates accuracy, polishes ───────┐    │
  │  └────────────────────┘                                        │    │
  └────────────────────────────────────────────────────────────────┼───┘
                                                                    ▼
                                                          ┌──────────────────┐
                                                          │ Output: report.md│
                                                          └──────────────────┘

  Backend (LLM for all agents): OpenAI/Azure OpenAI  OR  local Ollama (toggle in config)
```

## Component breakdown

| Component | Role | Semantic Kernel equivalent |
|---|---|---|
| **Crew** | Owns the agents + tasks, runs the `Process`. | Your SK orchestrator |
| **Researcher Agent** | role=analyst, goal=gather accurate findings. Task 1. | An SK specialist agent |
| **Writer Agent** | role=writer, goal=turn findings into a report. Task 2 (context = Task 1 output). | Specialist agent |
| **Reviewer Agent** | role=editor, goal=validate + finalize. Task 3 (context = Task 2 output). | Specialist agent |
| **Task** | description + expected_output + agent + context (dependency). | The goal you hand an SK agent |
| **Process.sequential** | Runs tasks in order, chaining outputs. | SK sequential pipeline |
| **LLM backend** | The model every agent uses; swappable OpenAI↔Ollama. | Azure OpenAI deployment / local |

## Data flow notes

- **`context` creates the dependency chain.** Task 2 lists Task 1 in its `context`, so the Writer receives the Researcher's findings automatically. Task 3 depends on Task 2. This is how crewAI passes state between agents — the equivalent of threading an SK agent's output into the next.
- **`expected_output` keeps agents on target.** Each task declares what "done" looks like; crewAI uses it to steer the agent toward a usable, well-shaped result instead of a rambling answer.
- **One backend, three agents.** All three agents share the same LLM here for simplicity. In production you'd tier them — a cheap model for the Researcher, a stronger one for the Reviewer (the model-routing pattern from your Azure work).

## Scaling to hierarchical

Swap `Process.sequential` → `Process.hierarchical` and add a `manager_llm`. A manager agent then plans and delegates dynamically instead of running a fixed order — the crewAI version of an SK supervisor/orchestrator that decides which specialist handles what. More adaptive, more LLM calls, higher cost.

---
*Next: `03_interview_qa.md`*
