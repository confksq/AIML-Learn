# 01 — Concepts: crewAI

> **Bridge from what you already know:** crewAI is **Semantic Kernel's multi-agent story, in Python**. You've built orchestrator + specialist agents in C# with SK — crewAI is the same ideas with different object names.

---

## 1. The one-sentence mental model

**A crewAI `Crew` is your SK orchestrator; a crewAI `Agent` is an SK specialist agent; a `Task` is the goal you hand it; a `Process` is the orchestration pattern (sequential/hierarchical).** Same ReAct reasoning underneath, same tool-calling, same memory concepts.

| You know (Semantic Kernel, C#) | crewAI (Python) | Same idea |
|---|---|---|
| Specialist agent (role + system prompt) | **Agent** (role + goal + backstory) | An LLM with a defined job |
| The goal you give an agent | **Task** (description + expected_output) | A unit of work |
| Orchestrator coordinating specialists | **Crew** | The team + how it runs |
| Sequential / hierarchical orchestration | **Process** (`sequential` / `hierarchical`) | Coordination pattern |
| `[KernelFunction]` plugin | **Tool** | What an agent can call |
| `ChatHistory` / vector memory | crewAI **memory** (short/long/entity) | Context across steps |
| `AutoInvokeKernelFunctions` (ReAct) | crewAI's built-in agent loop | Reason → act → observe |

---

## 2. The four core building blocks

**Agent** — defined by three fields that shape its behavior:
- `role` — who it is ("Senior Research Analyst")
- `goal` — what it optimizes for ("Find accurate, current information on the topic")
- `backstory` — persona/context that steers tone and expertise
Plus: `llm` (which model), `tools` (what it can call), `allow_delegation` (can it hand work to teammates).

**Task** — a unit of work assigned to an agent:
- `description` — what to do
- `expected_output` — what "done" looks like (crewAI uses this to keep the agent on target)
- `agent` — who does it
- `context` — outputs of prior tasks this one depends on

**Crew** — the team: a list of agents + a list of tasks + a `process`.

**Process** — how the crew executes:
- `Process.sequential` — tasks run in order, each feeding the next (like SK's sequential pipeline)
- `Process.hierarchical` — a manager agent plans and delegates to workers (like an SK supervisor/orchestrator pattern), requires a `manager_llm`

---

## 3. Sequential vs Hierarchical (maps to SK orchestration)

```
SEQUENTIAL (Process.sequential)          HIERARCHICAL (Process.hierarchical)
Researcher ─▶ Writer ─▶ Reviewer          Manager Agent
(fixed order, output chains)               ├─ delegates ─▶ Researcher
                                           ├─ delegates ─▶ Writer
                                           └─ delegates ─▶ Reviewer
                                          (manager plans + routes dynamically)
```

- **Sequential** = your SK *sequential pipeline* — predictable, cheap, good when the steps are known.
- **Hierarchical** = your SK *supervisor/orchestrator agent* — a manager LLM decides who does what, more adaptive, more expensive (extra manager calls). Same trade-off you already reason about in SK.

---

## 4. Tools and memory (unchanged concepts)

- **Tools** — Python functions or crewAI-provided tools (web search, file read, code execution) an agent can call. Same as SK `[KernelFunction]` plugins: the description is what the LLM reads to decide when to call it.
- **Memory** — crewAI supports short-term (within a run), long-term (across runs, persisted), and entity memory (facts about entities). Maps to SK's ChatHistory (short) + vector memory (long).

---

## 5. When to use crewAI vs Semantic Kernel vs LangGraph

| Framework | Language | Best for | Style |
|---|---|---|---|
| **Semantic Kernel** | C#/.NET (also Python) | Enterprise .NET, Azure-native production | Structured, plugin-based, your default at JMA |
| **crewAI** | Python | Fast role-based multi-agent, Python teams | High-level, opinionated "crew of agents" abstraction |
| **LangGraph** | Python | Complex, cyclic, stateful agent graphs | Low-level graph of nodes/edges — most control, most code |
| **AutoGen** | Python | Conversational multi-agent, research | Agents talk to each other in a conversation loop |
| **MAF (Microsoft Agent Framework)** | Python/.NET | Microsoft's converging agent stack | Newer, unifies SK + AutoGen ideas |

**The senior answer:** "crewAI is the fastest way to stand up a role-based multi-agent workflow in Python — great when the pattern is 'a crew of specialists with clear roles.' I use Semantic Kernel for enterprise .NET/Azure production, and LangGraph when I need fine-grained control over a cyclic, stateful agent graph that crewAI's higher-level abstraction can't express."

---

## 6. crewAI vs AutoGen vs MAF — quick contrast

- **crewAI** — role/goal/backstory agents, sequential or hierarchical process. Opinionated and quick to build.
- **AutoGen** — agents converse with each other (group chat), more emergent, more research-oriented.
- **MAF (Microsoft Agent Framework)** — Microsoft's newer framework converging Semantic Kernel + AutoGen; worth naming as "where Microsoft is heading" since you're Azure-focused.

---

## 7. Why this matters on your resume specifically

Azure AI Foundry JDs now list crewAI next to Semantic Kernel because teams want engineers fluent in **both** the .NET/Azure agent stack **and** the Python-native one. You already have the harder half (SK, production agents at JMA). This module closes the "can you also do it in Python's crewAI?" gap that a keyword screen checks for.

---
*Next: `02_architecture.md` — the 3-agent pipeline.*
