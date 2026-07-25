# Module 04 — Framework Comparison: LangGraph vs AutoGen vs Semantic Kernel


---

## Why This Module Matters

The screener confirmed focus on "entire AI agent workflow end-to-end." Framework choice IS the workflow. You will be asked:
- "Which framework did you use and why?"
- "How would you pick between LangGraph and Semantic Kernel if the team is 60% Python, 40% .NET?"
- "How does each framework manage agent state?"

Your anchor: You use **Semantic Kernel** in production at JM Family (.NET-native, Azure-integrated). You've evaluated the others. You can speak to all three.

---

> **⚙️ Config or Code? — This Entire Module**
> All four frameworks (Semantic Kernel, LangGraph, LangChain, AutoGen) are **100% Custom Code**. There is no portal configuration. You install the SDK, write your agent code, and deploy it like any other application. The only Config elements are the underlying Azure services each framework connects to (covered in their respective modules — Azure AI Foundry for models, Azure AI Search for RAG, etc.).

## Section 1 — What Each Framework IS (the 10-second version)

Before the deep dive, burn this into memory:

| Framework | One-line identity |
|---|---|
| **Semantic Kernel** | Microsoft's **production** AI SDK — plugin-based, .NET-native, Azure-native |
| **LangGraph** | **State machine graph** for Python agents — nodes are functions, edges are transitions, state is typed |
| **AutoGen** | **Multi-agent conversation** framework — agents talk to each other in a group chat |

These aren't interchangeable alternatives. They solve **different shapes of problem.** The interviewer wants to know if you understand that distinction.

---

## Section 2 — Semantic Kernel (Your Home Turf)

You built this. At JM Family, your production agent loop runs on Semantic Kernel in C#.

**The mental model:** Think of SK like an **electrical panel**. The Kernel is the panel box. Plugins are the circuit breakers — each one handles one domain (SharePoint retrieval, Azure AI Search query, OpenAI generation, Content Safety check). The Planner is the electrician who decides which circuits to switch on and in what order.

**Four core pieces you use every day:**
1. **Kernel** — the container. Holds all your plugins, your LLM service config, your memory config.
2. **KernelFunction** — a C# method decorated with `[KernelFunction]`. The LLM sees its name and description and decides when to call it. That's your tool.
3. **ChatHistory** — the state object. Every message (system, user, assistant) lives here. You pass it explicitly between turns.
4. **Planner** — the reasoning engine. Stepwise Planner runs ReAct loops: Reason → call a function → Observe the result → Reason again.

**Healthcare example — Ambient Documentation:**
A physician sees a patient. You have a SK agent running:
- Plugin 1: `SpeechToTextPlugin` — transcribes the visit audio
- Plugin 2: `EhrReadPlugin` — pulls the patient's prior diagnoses from FHIR
- Plugin 3: `SoapNotePlugin` — GPT-4o drafts the SOAP note
- Plugin 4: `ContentSafetyPlugin` — blocks any hallucinated medications before the note is saved
- Plugin 5: `EhrWritePlugin` — commits the approved note back to the EHR

The Planner calls them in sequence. ChatHistory carries the session context. `FunctionInvocationFilter` logs every plugin call with timestamp, latency, input/output — that's your HIPAA audit trail.

**JM Family anchor:**
"At JM Family I built Semantic Kernel agent loops in C# — plugins for SharePoint retrieval, Azure AI Search hybrid query, Azure OpenAI generation, and Content Safety filtering. The Kernel wires them all together. ChatHistory carries the session state between turns."

**Why SK wins for your context:**
- C#/.NET — your team already knows it
- Azure Managed Identity works without a single API key in code
- Azure AI Search, Azure OpenAI, Content Safety all have first-class SK connectors
- `FunctionInvocationFilter` gives you interception at every tool call — perfect for safety guardrails

**Tradeoff / when NOT to use:**
- Python-only team with no .NET — LangGraph will feel more natural
- Rapid prototyping with no enterprise constraints — SK has more boilerplate
- Graph-shaped workflows with many conditional branches — LangGraph's visual graph model is cleaner

---

## Section 3 — LangGraph (The One You'd Recommend for Python Teams)

LangGraph is what you'd reach for if a Python team came to you with a complex, branching workflow. The key concept: **it's a state machine, not a conversation.**

**The mental model:** Think of LangGraph like a **hospital patient pathway flowchart** — the kind they hang on the wall in the ER. Boxes are steps. Arrows are transitions. Some arrows have conditions ("if lab result is abnormal, go left; if normal, go right"). The patient (your state object) carries their chart through every box.

**Four core pieces:**
1. **StateGraph** — the graph definition. You add nodes and edges to it.
2. **State** — a `TypedDict` or Pydantic model. Every node reads from it and returns a partial update. The graph merges updates automatically.
3. **Node** — a Python function. Takes state in, returns state delta out.
4. **Checkpointer** — saves state to SQLite or Redis after every node. If the workflow crashes at node 7 of 12, it resumes at node 8 — not from scratch.

**Healthcare example — Prior Authorization:**

```python
class PriorAuthState(TypedDict):
    patient_id: str
    diagnosis_code: str
    policy_result: str       # what the payer API returned
    requires_review: bool
    approval_status: str
    audit_log: list[str]
```

Graph flow:
1. `fetch_ehr` node → reads patient record, populates `diagnosis_code`
2. `call_payer_api` node → checks insurance rules, populates `policy_result`
3. Conditional edge: if `requires_review == True` → interrupt, wait for physician
4. `physician_review` node (human-in-the-loop) → physician approves/denies
5. `generate_approval_letter` node → drafts the letter, saves to EHR

Every step is logged in `audit_log`. If the payer API times out at step 2, the Checkpointer saved after step 1 — you resume from step 2, not from scratch. That crash recovery is a clinical workflow feature, not a nice-to-have.

**The one thing LangGraph has that SK doesn't out-of-the-box:** built-in `interrupt_before` / `interrupt_after` at any node. Human approval as a first-class graph primitive.

**Tradeoff / when NOT to use:**
- .NET shop — LangGraph is Python only
- Simple linear agent with no branching — overkill, adds complexity
- Production enterprise Azure with .NET team — SK is a better fit

---

## Section 4 — LangChain (The Foundation Layer)

LangChain is the **original Python orchestration framework** — the one that started the agent ecosystem. LangGraph is actually built ON TOP of LangChain. Understanding how they relate is important for the interview.

**The mental model:** Think of LangChain like **hospital supply chain management** — it connects all the parts (LLMs, tools, memory, document loaders) into a pipeline. LangGraph is the advanced version that adds a graph structure and state machine on top of that supply chain.

**What LangChain provides:**
1. **Chains** — sequences of LLM calls connected together. Input → LLM → output → next LLM → final output.
2. **Agents** — LLM + tools + memory in a simple loop (similar to SK's Planner but Python-native)
3. **Document Loaders + Splitters** — built-in RAG pipeline components (load PDFs, chunk them, embed them)
4. **Memory** — ConversationBufferMemory, ConversationSummaryMemory for managing chat history
5. **Integrations** — 200+ connectors (Azure OpenAI, Pinecone, Chroma, Weaviate, HuggingFace)

**LangChain vs LangGraph — the key distinction:**

| | LangChain | LangGraph |
|---|---|---|
| **Structure** | Linear chains / simple loops | Graph with nodes + edges + typed state |
| **State** | Implicit in chain context | Explicit TypedDict flowing through graph |
| **Branching** | Basic conditional chains | Full graph-based conditional routing |
| **Crash recovery** | None built-in | Built-in Checkpointer |
| **Human-in-loop** | Manual interruption | First-class `interrupt_before/after` |
| **Best for** | Simple RAG pipelines, quick agents | Complex branching workflows |

**When LangChain is the right choice:**
- Building a RAG pipeline quickly in Python — LangChain's document loaders, splitters, and retrievers are the fastest path
- Simple single-agent with a few tools — no graph needed
- Team already uses LangChain and the workflow is linear
- Proof of concept before formalizing into LangGraph

**When to upgrade from LangChain to LangGraph:**
- Workflow has multiple branches ("if diagnosis is X go left, if Y go right")
- You need crash recovery — checkpoint and resume mid-workflow
- You need human-in-the-loop as a first-class primitive
- State must be explicitly typed and auditable (PHI environments)

**Healthcare example — RAG pipeline with LangChain:**
```python
from langchain_openai import AzureChatOpenAI, AzureOpenAIEmbeddings
from langchain_community.retrievers import AzureAISearchRetriever
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.runnables import RunnablePassthrough

# Simple RAG chain — LangChain is excellent here
retriever = AzureAISearchRetriever(service_name="vitalcare-search")

prompt = ChatPromptTemplate.from_template(
    "Answer using only this context: {context}\nQuestion: {question}"
)

rag_chain = (
    {"context": retriever, "question": RunnablePassthrough()}
    | prompt
    | AzureChatOpenAI(model="gpt-4o")
)

result = rag_chain.invoke("What is the prior auth criteria for semaglutide?")
```

**Why LangChain for RAG but LangGraph for workflows:**
> "LangChain's LCEL (LangChain Expression Language) pipe syntax is the cleanest way to build RAG pipelines — retriever, prompt, LLM in three lines. But for a prior auth workflow with 5 steps, conditional routing, and human review gates, I'd move to LangGraph. LangChain is the foundation; LangGraph is the workflow engine built on it."

**JM Family anchor:**
"At JM Family, if I were building a Python RAG pipeline I'd use LangChain's document loaders and retriever chain — it's the fastest path to a working pipeline. But our production orchestration is Semantic Kernel in C# because we're .NET-native and need Azure-native auth and compliance controls that LangChain doesn't provide out of the box."

---

## Section 5 — AutoGen (The One You Use for Research Only)

AutoGen is from Microsoft Research — not the product team. That distinction matters in an interview.

**The mental model:** Think of AutoGen like a **hospital committee meeting**. You put a Radiologist, Oncologist, and Pharmacist in a room with a coordinator. They take turns speaking, reading each other's comments, and building toward a conclusion. Nobody has an explicit role script — they improvise based on what was said before.

That's powerful for exploration. It's dangerous for production.

**Three core pieces:**
1. **AssistantAgent** — an LLM agent that reasons and responds
2. **UserProxyAgent** — represents the human; can also auto-execute code the AssistantAgent writes
3. **GroupChat** — the shared conversation; agents take turns contributing

**The problem with AutoGen in clinical settings — three reasons:**

**1. Implicit state.** State is the message list. There's no `prior_auth_status: str` field you can check. You read the thread and infer. In a PHI environment with an audit requirement, "infer from thread" is not an acceptable answer.

**2. Non-deterministic routing.** The GroupChatManager uses an LLM to decide which agent speaks next. Same input, different execution, different path. You cannot guarantee the Pharmacist always checks drug interactions before the Oncologist commits to a treatment plan.

**3. No crash recovery.** AutoGen has no Checkpointer equivalent. If your multi-agent conversation crashes after 20 minutes, you restart from zero.

**When AutoGen IS the right call:**
- Code generation and review (AssistantAgent writes code, UserProxyAgent runs it, they iterate)
- Internal research — exploring what a multi-agent system *might* do before you formalize it in LangGraph or SK
- Hackathons, proofs-of-concept with no PHI

---

## Section 6 — State Management (The Trap Question)

Interviewers who've actually built agents will ask this. It separates people who read the docs from people who ran it in production.

**The question they're really asking:** "If your agent takes 12 steps to process a prior auth and crashes on step 9, what happens?"

| Framework | State location | State type | Crash recovery |
|---|---|---|---|
| **SK** | ChatHistory + VectorStore | Explicit, typed, conversation-shaped | Manual — you persist ChatHistory to Redis/Cosmos yourself |
| **LangGraph** | TypedDict flowing through graph | Explicit, typed, graph-shaped | Built-in Checkpointer — automatic resume from last node |
| **LangChain** | Chain context / ConversationMemory | Implicit, conversation-shaped | None built-in — manual persistence needed |
| **AutoGen** | Message list | Implicit, untyped | None — restart from scratch |

**Your answer at JM Family:**
"We persist ChatHistory to Cosmos DB between turns. Each session has a session ID. On reconnect, we reload the history from Cosmos and hand it back to the Kernel. It's not automatic like LangGraph's Checkpointer, but it's production-reliable and we control exactly what goes in."

---

## Section 7 — The Terror Question: 60% Python / 40% .NET Team

*"You have to pick one. Team is 60% Python, 40% .NET. LangGraph or Semantic Kernel?"*

**The answer — four points, memorize this:**

> "I'd pick Semantic Kernel — and here's why I'm not just defaulting to the language majority.
>
> We're on Azure, the client is healthcare, PHI is in scope. Semantic Kernel is the only framework where Azure Managed Identity, Content Safety, and Azure AI Search are first-class connectors — not things you bolt on. That's not a preference, that's a compliance requirement.
>
> For the 60% Python developers — SK has a Python SDK with the same plugin model. Python engineers write KernelFunctions in Python, .NET engineers write the orchestration layer in C#. The plugin contract is the same across languages. They interop cleanly.
>
> The one place I'd bring LangGraph in is for isolated, complex branching workflows — like prior auth decision trees where human-in-the-loop interrupts and crash recovery matter. LangGraph wins there. But the agent orchestration layer, the guardrails, the monitoring? Semantic Kernel on .NET, Azure-native, every time."

---

## Quick-Reference Interview Answers

**Q: Which agent framework do you use and why?**
"Semantic Kernel in production. We're a .NET shop on Azure. SK's plugin model maps directly to our C# service layer, Managed Identity auth works out of the box, and Azure AI Search integration is native. For Python-side data pipelines I've evaluated LangGraph — its typed state graph is excellent for prior auth workflows. AutoGen I keep in research/prototyping only — not suitable for PHI."

**Q: How does LangGraph manage state differently from Semantic Kernel?**
"LangGraph uses an explicit typed state object — a TypedDict or Pydantic model — that flows through every node in the graph. Every node reads from it and writes back a delta. You can checkpoint that state to Redis so a crashed workflow resumes exactly where it stopped. SK manages state through ChatHistory for short-term and pluggable VectorStore for long-term — also explicit and typed, but shaped around conversation turns rather than graph traversal. LangGraph is workflow-native, SK is conversation-native."

**Q: What's wrong with AutoGen for production healthcare?**
"Three problems. State is implicit — it's the message thread, no typed schema, so auditing what the agent knew at step N is hard. It's non-deterministic — the GroupChatManager uses an LLM to decide which agent speaks next, so the same input can produce different execution paths. And there's no crash recovery — if the workflow fails mid-way, you restart from scratch. None of those are acceptable in a PHI environment."

**Q: What is LangChain and how does it differ from LangGraph?**
"LangChain is the foundational Python orchestration framework — it provides chains, agents, document loaders, and 200+ integrations. LangGraph is built on top of LangChain and adds a graph-based state machine with typed state, conditional routing, and a built-in Checkpointer for crash recovery. I use LangChain for RAG pipelines — its LCEL pipe syntax is the fastest path to a working retriever-prompt-LLM chain. I use LangGraph when the workflow has branching, human-in-the-loop gates, or crash recovery requirements. LangChain is the foundation; LangGraph is the workflow engine you graduate to when a linear chain isn't enough."

**Q: How do you choose between all four frameworks?**
"I map the problem shape to the framework. Semantic Kernel: .NET shop, Azure-native, production PHI — compliance and auth are first-class. LangGraph: Python team, complex branching workflow, crash recovery matters, human-in-the-loop required. LangChain: Python team, simple RAG pipeline or linear agent, fastest to prototype. AutoGen: research and exploration only, never PHI, never production — non-deterministic routing and no crash recovery rule it out."
