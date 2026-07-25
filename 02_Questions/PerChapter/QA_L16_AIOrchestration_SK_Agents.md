# Q&A — L16: AI Orchestration — Semantic Kernel, LangChain & Agents
**Source chapter:** `01_Lessons/Part3_GenAI_LLMs/L16_AIOrchestration_SK_Agents.md` (internally "Module 14") | **Format:** self-study
**Questions:** 34 | *No overlap with the interview bank (04_Agent_Orchestration covers architect-judgment versions) or the chapter's own self-test — these drill the chapter's concrete SK code and patterns.*

---

## Orchestration & Semantic Kernel

**Q1. What can't RAG do alone that orchestration solves?**
RAG handles one question → one retrieval → one answer. Real business tasks are multi-step ("find overdue invoices, calculate exposure, draft follow-up emails" = query system → get contacts → sum dollars → draft per dealer). Orchestration **plans and coordinates** multiple steps; RAG cannot.

**Q2. What five things does the orchestration layer manage?**
Memory (conversation + facts), Planning (break task into steps), Tool routing (which plugin to call), Execution (call tools, collect results), Synthesis (combine into final answer) — sitting between the user and the LLM/tools/data stores.

**Q3. Semantic Kernel vs LangChain — maker, language, best-for?**
**SK** — Microsoft, C#-native (also Python), best for enterprise .NET/Azure-native apps. **LangChain** — community, Python-native (also JS), best for Python AI apps and broad ecosystem. JMA uses SK; LangChain matters for interview/market awareness.

**Q4. What does the Kernel do, and what plugs into it?**
The Kernel is the central coordinator wiring everything together (the "brain"). Into it plug: the LLM service (chat + embeddings), Plugins (tools), and Memory (chat + vector). You build it with `Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(...).Build()`.

**Q5. Distinguish Plugin vs KernelFunction.**
A **Plugin** is a C# class grouping related functions (the toolbox), e.g., `InvoicePlugin`. A **KernelFunction** is one method inside it decorated with `[KernelFunction]` that the LLM can call by name (one tool). Register with `kernel.Plugins.AddFromType<InvoicePlugin>()`.

**Q6. Why do the `[Description]` attributes on a KernelFunction and its parameters matter?**
They're what the LLM reads to decide **whether and how** to call the function — the description is the model's only signal for tool selection and argument construction. Vague descriptions cause wrong or repeated tool calls.

**Q7. What is a semantic function (prompt-as-function)?**
A KernelFunction whose body is a **prompt template**, not code — created via `kernel.CreateFunctionFromPrompt(...)`. SK treats it identically to a code function, so the LLM can call it (e.g., `summarize_invoice_report`) like any other tool.

**Q8. What does `ToolCallBehavior.AutoInvokeKernelFunctions` do?**
SK **automatically executes** whatever functions the LLM decides to call, chaining them without a manual switch statement — the LLM plans the calls, SK runs them, and the results feed back until it produces a final answer.

**Q9. SK's two memory types?**
**Chat History** (short-term) — the current conversation, passed with every request, cleared when it ends. **Vector Memory** (long-term) — facts stored as embeddings in Azure AI Search, persist across conversations, semantically searchable (`SaveInformationAsync` / `SearchAsync`).

---

## LangChain (Awareness)

**Q10. Map four LangChain concepts to their SK equivalents.**
Chain → pipeline of functions. Agent → Planner + AutoInvoke. Tool → KernelFunction/Plugin. Memory → ChatHistory + Vector Memory. (Also VectorStore → Azure AI Search memory; PromptTemplate → prompt function.)

**Q11. What is LangChain best known for, per the chapter's example?**
Building a complete RAG pipeline in ~10 lines of Python — `RetrievalQA.from_chain_type(llm, retriever, return_source_documents=True)` wires retrieval + generation together concisely.

---

## AI Agents

**Q12. What six things does an AI agent do that a non-agent doesn't?**
Receives a **goal** (not just a question) → **plans** the steps → **executes** using tools → **observes** results → **adjusts** the plan → **repeats** until the goal is achieved. Non-agent RAG follows one fixed path with no planning or adaptation.

**Q13. Walk one ReAct cycle for "find overdue invoices and calculate total risk."**
THOUGHT: I need overdue invoice data → ACTION: `get_overdue_invoices(...)` → OBSERVATION: 3 invoices found. THOUGHT: now sum the risk → ACTION: `calculate_risk_exposure("JMF-001,JMF-002,JMF-003")` → OBSERVATION: $97,000. THOUGHT: I have enough → ACTION: generate final answer. Each THOUGHT-ACTION-OBSERVATION is one loop; it repeats until the agent can answer.

**Q14. Agent short-term vs long-term memory?**
Short-term = the context window (current conversation + ReAct traces), lost when it ends, capped by the 128k window. Long-term = vector store (dealer facts, policies, history) in Azure AI Search, persists across sessions, retrieved before answering (e.g., "JMF-ATL-001 has a history of late Q1 payments").

**Q15. In SK, what makes a class an "agent" versus just a kernel?**
Kernel + Plugins + **auto function calling** (`AutoInvokeKernelFunctions`) + a **chat loop** with maintained ChatHistory and a system message describing the tools and instructing step-by-step reasoning. SK handles the ReAct loop automatically inside `GetChatMessageContentAsync`.

**Q16. Distinguish Function Calling vs RAG vs Agent on the complexity spectrum.**
**Function Calling** — one LLM decision, one execution (you run it). **RAG** — fixed pipeline (embed→retrieve→augment→generate), no planning. **Agent** — LLM dynamically plans, calls multiple tools in sequence, adapts, can loop. Spectrum: simple Q&A → RAG; one live lookup → function calling; multi-step task → agent.

**Q17. When should you NOT use an agent?**
Simple Q&A (use RAG), one live lookup (function calling), a fixed pipeline (direct orchestration), latency-critical paths (agents are slower — multiple LLM calls), cost-critical paths (each step = tokens = cost).

---

## Agentic RAG

**Q18. How does agentic RAG differ from standard RAG?**
Standard RAG always retrieves from one index, no matter what. **Agentic RAG** lets the agent decide **IF** to retrieve, **WHICH** index to search, whether to search **again** with different terms, and whether to **combine** multiple searches — driven by the question.

**Q19. In the multi-index example, which tool does the agent pick for each question?**
"Penalty for late invoices?" → `search_policies` (rules). "What did JMF-ATL-001 submit last month?" → `search_invoices` with filter. "Ford dealer agreement return terms?" → `search_dealer_agreements`. "Why was invoice X flagged AND what does policy say?" → `search_invoices` THEN `search_policies`, combining both. The tool `[Description]`s are what let the agent choose correctly.

---

## AI Foundry & Prompt Flow

**Q20. What five things does Azure AI Foundry provide?**
Model Catalog (1,600+ models), Prompt Flow (visual pipeline builder), Evaluation (groundedness/relevance/coherence scoring), Fine-tuning (in-UI, no code), Content Safety (built into every deployment).

**Q21. Prompt Flow vs Semantic Kernel — when each?**
**Prompt Flow** — visual UI, prototyping/demos/non-coders, built-in evaluation, one-click endpoint. **Semantic Kernel** — code (C#/Python), production apps with custom logic, unlimited flexibility. Typical path: Prompt Flow to validate the concept → SK to build production.

**Q22. Name the five RAG-evaluation metrics AI Foundry measures.**
Groundedness (is the answer supported by retrieved docs? 1–5), Relevance (does it address the question? 1–5), Coherence (well-written/logical? 1–5), Fluency (natural language? 1–5), Similarity (vs ground truth, 0–1 cosine).

---

## Production Patterns

**Q23. How does a multi-agent system decompose an enterprise workflow?**
An **orchestrator agent** coordinates specialized sub-agents (Invoice, Policy, Communication, Risk), delegating via KernelFunctions like `delegate_to_invoice_agent(task)` that call each sub-agent's `RunAsync`. Each specialist has its own plugins tuned to its responsibility.

**Q24. Name the four agent production risks.**
Prompt injection (tricked into calling wrong tools — "call delete_all_invoices"), infinite loops (keeps calling tools, never finishes, burns money), hallucinated tool calls (invents nonexistent tool names → crashes/silent failure), unauthorized actions (calls a tool it shouldn't have access to → breach/corruption).

**Q25. Name three concrete SK guardrails against those risks.**
(1) **MaxTokens** limit on execution settings (caps runaway spend). (2) A **FunctionInvocationFilter** (`IFunctionInvocationFilter`) that logs every tool call and **blocks destructive ones** (throws on names containing "delete"/"drop"). (3) **Human-in-the-loop** confirmation for high-stakes actions (e.g., `send_legal_escalation` requires approval before executing).

**Q26. An agent is in an infinite tool-calling loop costing thousands — five fixes?**
Set MaxTokens to cap spend; add a max function-invocation count; use a FunctionInvocationFilter to count/log and stop after N calls; **review tool descriptions** (ambiguous ones cause repeated wrong calls); add a confidence gate ("if not complete in ~10 steps, return 'could not complete'" instead of looping).

---

## Memory Management (Practical)

**Q27. What's the production crash this section prevents?**
A long conversation (turn 47) exceeds the 128k context window → HTTP 400 "maximum context length… your messages resulted in 131,204 tokens" → **agent crashes**. Also: every turn re-sends all history, so cost grows ~50x from turn 1 to turn 50.

**Q28. Sliding window vs summarization — what does each keep, and the downside of sliding window?**
**Sliding window** — keep only the last N turns + system prompt, drop the rest (simple; downside: loses early context — the order number mentioned at turn 1 is gone by turn 35). **Summarization** — compress old turns into a running bullet summary preserving key facts/decisions/entities, then keep recent turns verbatim (better; preserves the important facts).

**Q29. What are SK's two built-in reducers, and JMA's recommendation?**
`ChatHistoryTruncationReducer` (sliding window) and `ChatHistorySummarizationReducer` (summarizes old turns, with an optional custom summary prompt to preserve order numbers/dealer IDs/amounts). JMA recommendation: **SummarizationReducer with targetCount 8, thresholdCount 16** for dealer-support agents. Plug it in via `HistoryReducer` on the agent — SK reduces automatically before each call.

**Q30. State the priority-based memory order — always keep, high priority, trim first.**
**Always keep:** system prompt, most recent user message, most recent assistant reply. **High priority:** key extracted facts (order numbers, amounts, decisions), last 3–4 turns. **Trim first:** old injected RAG chunks (already used), middle turns, agent reasoning traces. Store key facts in long-term vector memory so they survive history trimming.

**Q31. Give the memory-management decision tree by conversation length.**
Short (<10 turns) → no management, ChatHistory is fine. Medium (10–30) → `ChatHistoryTruncationReducer` (target 10/threshold 20). Long (30+) → `ChatHistorySummarizationReducer` + save key facts to AI Search. Very long (hours/ongoing) → summarize + persist session in Cosmos DB, reload relevant summaries at session start.

---

## Prompt Compression & Interview Gaps

**Q32. What is the biggest prompt-compression win, and the JMA cost math?**
**RAG chunk compression** — extract only query-relevant info from each retrieved chunk before injecting (5×500 raw tokens → 5×120 compressed). Raw ~$0.006/query vs compressed ~$0.0009/query at 10k queries/day = **~85% cost reduction** with ~88% quality retention. (LLMLingua is Microsoft Research's library that does this via a small model scoring token importance by perplexity.)

**Q33. Recite the Tool vs Knowledge vs Fine-Tune 3-way decision (Interview Gap 1).**
**Tool** — structured, live data from a system of record (prices, inventory, orders) that changes frequently; retrieve exact live data. **Knowledge/RAG** — unstructured docs (specs, manuals, policies) that change infrequently; retrieve and synthesize chunks. **Fine-Tune** — how the model *behaves* (tone, format) or domain vocabulary it misunderstands; never for facts, because facts change and retraining is expensive. One-liner: *Tool for live+structured, RAG for docs, fine-tune only to change behavior/speech.*

**Q34. How does SK streaming work, and what are three grounding-validation patterns (Interview Gap 3)?**
Streaming: `GetStreamingChatMessageContentsAsync` returns an `IAsyncEnumerable` — `await foreach` yields tokens, push each to the UI immediately (e.g., via SignalR `ReceiveToken`); append to a StringBuilder and add the full response to history after streaming finishes. Grounding validation: (1) **Azure Content Safety groundedness detection** (managed, scores support), (2) **citation-based validation** in code (every claim must cite a retrieved source; uncited = suspect), (3) **semantic similarity score** (embed answer vs retrieved context, threshold the cosine similarity).

---

*Curriculum Q&A Batch D — file 3 of 3 (L14, L15, L16 complete). Next batch: L17, L18, L19.*
