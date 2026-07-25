# 03 — Interview Q&A: crewAI Multi-Agent (15 questions, senior level)

---

**Q1. What is crewAI in one sentence, and how does it relate to Semantic Kernel?**
crewAI is a Python framework for role-based multi-agent orchestration — a crew of agents, each with a role/goal/backstory, executing tasks via a sequential or hierarchical process. It's the Python-native counterpart to Semantic Kernel's multi-agent story: same ReAct reasoning, tool-calling, and memory concepts, different object names (Agent/Task/Crew/Process vs SK agents/plugins/orchestrator).

**Q2. What three fields define a crewAI Agent, and why do they matter?**
`role` (who it is), `goal` (what it optimizes for), and `backstory` (persona/context). They matter because they're injected into the system prompt and materially steer the agent's behavior, tone, and expertise — the crewAI equivalent of carefully authoring an SK specialist agent's system prompt.

**Q3. What is a Task, and how does one task depend on another?**
A Task is a unit of work: `description`, `expected_output`, and the `agent` that performs it. Dependencies are expressed via `context` — Task 2 lists Task 1 in its context, so it automatically receives Task 1's output. This is how state chains through a crew, like threading one SK agent's output into the next.

**Q4. Sequential vs hierarchical process — when do you use each?**
Sequential runs tasks in a fixed order, each feeding the next — predictable, cheap, good when the steps are known (my 3-agent Researcher→Writer→Reviewer pipeline). Hierarchical adds a manager LLM that plans and delegates dynamically — more adaptive, more expensive (extra manager calls). It maps directly to SK's sequential pipeline vs supervisor/orchestrator patterns.

**Q5. What role does `expected_output` play?**
It declares what a finished task looks like, which crewAI uses to keep the agent on target and produce a usable, well-shaped result rather than a rambling one. It's a lightweight form of output contract — related to how you'd specify a structured output or acceptance criteria in a production SK task.

**Q6. How do Tools work in crewAI?**
Tools are Python functions (or crewAI-provided tools like web search, file read, code execution) that an agent can call. The tool's description is what the LLM reads to decide when to use it — identical to SK `[KernelFunction]` `[Description]` attributes driving tool selection.

**Q7. How does memory work in crewAI?**
crewAI supports short-term memory (within a run), long-term memory (persisted across runs), and entity memory (facts about specific entities). This maps to SK's ChatHistory (short-term) plus vector memory in Azure AI Search (long-term). Enable it with `memory=True` on the Crew.

**Q8. crewAI vs LangGraph — when would you pick LangGraph instead?**
crewAI is a high-level, opinionated "crew of role-based agents" abstraction — fast to build when the pattern fits. LangGraph is a low-level graph of nodes and edges giving fine-grained control over cyclic, stateful agent flows. I pick LangGraph when I need explicit control over loops, branching, and shared state that crewAI's higher-level abstraction can't express cleanly.

**Q9. crewAI vs AutoGen — what's the difference in style?**
crewAI is task/role-driven with a defined process (sequential/hierarchical). AutoGen is conversation-driven — agents talk to each other in a group chat, producing more emergent behavior, which suits research and exploratory multi-agent scenarios. crewAI is more structured and production-predictable.

**Q10. Can crewAI run against local models? Why does that matter?**
Yes — point the agents' `llm` at an Ollama/OpenAI-compatible endpoint. It matters for air-gapped/regulated environments and cost control: you get multi-agent orchestration with no cloud dependency and no per-token bill. My hands-on runs against either OpenAI or local Ollama via a config toggle.

**Q11. What are the risks of a multi-agent system, and how do you control them?**
Runaway cost (each agent + each tool call = tokens), infinite loops, hallucinated tool calls, and error propagation between agents. Controls: cap iterations/steps, validate tool arguments before execution, use clear tool descriptions to avoid mis-routing, and add human-in-the-loop gates for high-stakes actions — the same guardrails as production SK agents.

**Q12. When is multi-agent overkill vs a single agent with tools?**
Multi-agent is justified when sub-tasks genuinely benefit from different expertise/framing or can run in parallel, or when isolated context per role improves quality. If a single agent with a few tools handles the task well, adding agents just multiplies cost and failure surface. The burden of proof is on justifying the extra agents, not the reverse.

**Q13. How would you cost-optimize a crewAI pipeline?**
Tier the models: a cheap model (GPT-4o-mini or a local Ollama model) for simple agents like the Researcher, a stronger model for the Reviewer where quality matters. Cap iterations, cache repeated sub-results, and prefer sequential over hierarchical unless the dynamic delegation is genuinely needed (hierarchical adds manager-LLM calls).

**Q14. An Azure AI Foundry JD lists both Semantic Kernel and crewAI. How do you frame your fit?**
I build production multi-agent systems in Semantic Kernel (C#, Azure-native) at JM Family — orchestrator + specialists, plugins, memory, guardrails — and I've implemented the same role-based patterns in crewAI in Python. I can pick the right tool per context: SK for enterprise .NET/Azure production, crewAI for fast Python-native crews, LangGraph when I need low-level graph control.

**Q15. Walk through your 3-agent research pipeline design.**
A sequential crew: a Researcher agent (role=analyst, goal=gather accurate findings) produces structured findings; a Writer agent takes those findings (via task context) and produces a formatted report; a Reviewer agent takes the draft and validates/finalizes it. Each task declares its expected output and its context dependency, so state chains cleanly. It runs against OpenAI or local Ollama, and the same structure scales to hierarchical by adding a manager LLM.

---
*Answer these in terms of "the Semantic Kernel equivalent is X" to show you're not learning agents from scratch — you're mapping known concepts to a new framework.*
