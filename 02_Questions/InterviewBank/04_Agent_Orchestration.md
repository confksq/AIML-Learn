# Module 4 — Agent Orchestration
**Source plan:** `AIML-Learn/04_Career/00_PRD.md` §4–5, `01_EXECUTION_PLAN.md`
**Format:** WHY / HOW / WHEN / SCALE / DEPLOY
**Question count:** 18 (Tool-calling & Planning: 6, Multi-agent Coordination: 6, Agent Memory: 6)

---

## Tool-Calling & Planning

### Q1. How does function/tool calling actually work under the hood?

- **WHY:** LLMs can't natively execute code or fetch live data — tool calling gives the model a structured way to request an external action and receive its result back into the conversation.
- **HOW:** The model is given tool schemas (name, description, parameter JSON schema) alongside the prompt; when it decides a tool is needed, it emits a structured tool-call (name + arguments) instead of natural-language text; your orchestration code executes the actual function and feeds the result back into the context as a new message; the model then continues generating using that result.
- **WHEN:** Any task requiring live data, computation, or an action outside the model's parametric knowledge (order lookup, calculation, sending an email) — not for tasks the model can answer from context/knowledge alone.
- **SCALE:** More available tools means a larger tool-schema payload in every request (consumes context/tokens) and a harder tool-selection problem for the model — tool count doesn't scale for free.
- **DEPLOY:** Tool execution itself may call regional services (a database, another API) — the orchestration layer needs to route the actual tool execution to the correct region's backend, not just the model call.

**Follow-up probe:** "The model calls a tool with a malformed argument — whose responsibility is catching that, and where?" (The orchestration layer, via schema validation before execution — never trust model-generated arguments as pre-validated, since hallucinated/malformed arguments are a known failure mode, not an edge case.)

---

### Q2. What is the ReAct pattern, and why does it outperform a single-shot tool call for complex tasks?

- **WHY:** A single-shot "decide all tools upfront" approach can't adapt when an early tool result changes what's needed next — ReAct (Reason + Act, interleaved) lets the model observe each result before deciding the next step.
- **HOW:** Loop: model reasons about what to do → takes one action (tool call) → observes the result → reasons again incorporating that observation → repeats until it decides it has enough to answer.
- **WHEN:** Multi-step tasks where later steps depend on earlier results (can't be planned entirely upfront) — overkill for a single deterministic tool call.
- **SCALE:** Each reasoning-action-observation cycle is a full model round-trip — cost and latency scale with step count, same concern as multi-hop RAG (Module 3 Q10); needs a step-count/cost ceiling.
- **DEPLOY:** Same orchestration-layer concern as Q1 — each action step may hit a different regional backend, and the loop's overall latency is the sum of every hop, which compounds badly across regions if tools aren't colocated with the loop's execution.

**Follow-up probe:** "A ReAct loop is stuck reasoning-acting for 12 steps on a task that should take 3 — how do you debug and prevent it?" (Log every reasoning/action/observation step for inspection; likely causes are ambiguous tool descriptions causing repeated wrong tool selection, or a tool returning results the model can't parse — fix the tool contract, and add a hard step ceiling as a backstop regardless.)

---

### Q3. What problem do Semantic Kernel Planners solve that direct tool-calling doesn't?

- **WHY:** Direct tool-calling is one decision at a time (call a tool, get a result, decide again); a planner generates a multi-step plan upfront (or dynamically) sequencing multiple tools/functions to achieve a higher-level goal, useful when the task decomposition itself is non-trivial.
- **HOW:** The planner takes a goal + available plugin functions, and produces an executable plan (a sequence or DAG of function calls) — either generated once upfront (simpler, less adaptive) or dynamically (closer to ReAct, more adaptive but more expensive).
- **WHEN:** Tasks that decompose naturally into a known set of sub-steps using existing plugins (JMA's `IncentiveClaimAgent.cs` + `Plugins/` pattern — `ClaimDecisionPlugin`, `DealerEligibilityPlugin`, `PolicyLookupPlugin` composed together) — not needed for a single tool call.
- **SCALE:** Upfront/static planning is cheaper (one planning call) but less resilient to a step failing mid-execution than dynamic/adaptive planning, which re-plans but costs more per adjustment.
- **DEPLOY:** Plan execution — like any multi-step orchestration — needs its steps' backend calls colocated with the region serving the request to avoid cross-region latency compounding across the plan's length.

**Follow-up probe:** "A static plan's third step fails because upstream data changed since planning — what happens next in a well-designed system?" (The orchestration layer needs failure handling per step — either abort with a clear error, retry that step, or trigger a re-plan from the failure point — a plan that has no failure-handling strategy beyond 'crash' isn't production-ready.)

---

### Q4. How do you validate/sanitize tool-call arguments the model generates?

- **WHY:** The model can hallucinate arguments — a plausible-looking but invalid ID, an out-of-range value, or a subtly wrong type — and executing a tool blindly on hallucinated input is a direct path to a real-world error (this is "agentic hallucination," distinct from factual hallucination, covered further in Q18).
- **HOW:** JSON schema validation on every tool-call argument before execution (type, required fields, enum/range constraints), plus semantic validation where schema alone isn't enough (e.g., does this order ID actually exist) before the tool executes any side-effecting action.
- **WHEN:** Every tool call, without exception — especially any tool with a side effect (write, send, charge, approve) rather than a pure read.
- **SCALE:** Validation logic is cheap relative to the tool execution it's guarding — no meaningful scale trade-off; skipping it to save time is a false economy that scales badly the moment volume includes a hallucinated edge case.
- **DEPLOY:** Validation logic itself is region-agnostic, but the data it validates against (e.g., "does this dealer code exist") needs to query the correct region's source of truth.

**Follow-up probe:** "Your agent hallucinated a plausible-but-nonexistent dealer code, and it passed a basic 'is this a string' schema check — what's missing?" (Schema validation alone only checks *shape*, not *existence* — need a semantic/existence check against the actual system of record before executing, not just type validation; this is the exact class of failure the AuditFilter.cs pattern in JMA's DealerIntelligence platform is designed to catch.)

---

### Q5. How do you design tool-call failure handling and retries?

- **WHY:** External tool calls fail for the same reasons any distributed call fails (timeout, transient error, downstream service down) — an agent without retry/failure logic either crashes or silently proceeds with a missing result.
- **HOW:** Circuit breaker + retry policy per tool (matching JMA's `CircuitBreaker.cs`/`RetryPolicy.cs` pattern), with the agent's reasoning loop explicitly informed when a tool call failed (not silently treated as an empty success) so it can adapt — retry, try an alternate tool, or escalate/ask for human input.
- **WHEN:** Every tool integration, proportional to that tool's real-world reliability — a well-understood internal API needs less defensive handling than a third-party integration with a spotty uptime history.
- **SCALE:** Retry storms are a real risk at scale — if a downstream dependency degrades, many concurrent agent instances retrying simultaneously can worsen the outage; exponential backoff with jitter, and a circuit breaker to stop calling a clearly-down dependency, are what prevent that.
- **DEPLOY:** Circuit breaker state and retry budgets should be scoped per-region if tool backends are regional — a circuit breaker tripped by a regional outage shouldn't block calls to a healthy region's instance of the same tool.

**Follow-up probe:** "A tool call fails 3 times, retries exhaust, and the circuit breaker trips — what should the agent do next, architecturally?" (Escalate — either fall back to a degraded-but-functional path (JMA's `EscalationService.cs`), inform the user of the limitation honestly, or hand off to a human — silently proceeding as if the tool succeeded, or returning a fabricated result, are both worse failure modes than an honest 'I can't complete this right now.')

---

### Q6. Single large multi-purpose tool vs many granular single-purpose tools — how do you decide tool granularity?

- **WHY:** Too few, overly broad tools force the model to guess complex combined arguments and make correct tool-selection ambiguous; too many, overly narrow tools bloat the schema payload and make selection ambiguous the opposite way (many similar-looking tools).
- **HOW:** Granular tools with a single clear responsibility, named and described distinctly enough the model can reliably disambiguate (JMA's `Plugins/` pattern — `ClaimDecisionPlugin`, `DealerEligibilityPlugin`, `PolicyLookupPlugin` as separate, clearly-scoped tools rather than one mega "HandleClaim" tool).
- **WHEN:** Default to granular/single-responsibility tools — split further only if the model is demonstrably confusing when to use which tool (a real evaluation signal, not a hypothetical one).
- **SCALE:** As the tool library grows across a large agent platform, granularity discipline is what keeps tool selection tractable — an unmanaged sprawl of overlapping tools degrades selection accuracy as tool count grows, independent of any single tool's quality.
- **DEPLOY:** Tool granularity is a design-time decision, not deployment-topology-dependent — but a large tool library shared across regions needs consistent naming/versioning discipline so regional agent instances don't drift into incompatible tool sets.

**Follow-up probe:** "You have 40 tools and the agent increasingly picks the wrong one — is the fix more tools, fewer tools, or something else?" (Likely something else first — better tool descriptions/naming to reduce ambiguity, or grouping into a hierarchical selection (pick a category, then a tool within it) — a flat 40-tool list is itself often the actual problem, independent of each tool's individual quality.)

---

## Multi-Agent Coordination

### Q7. When do you actually need multiple agents instead of one agent with more tools?

- **WHY:** A single agent with many tools still has one reasoning context and one "role" — multi-agent makes sense when different sub-tasks genuinely benefit from different expertise/framing, isolated context, or independent failure domains, not just "more tools."
- **HOW:** Decompose by role/responsibility (JMA's pattern: `SupervisorAgent` coordinating `ClaimValidatorAgent`, `FraudDetectorAgent`, `PolicyCheckerAgent` — each with a narrow, distinct responsibility and its own context/prompt tuned for that specific job).
- **WHEN:** When a single agent's context would need to hold too many conflicting concerns simultaneously (fraud detection framing vs policy compliance framing genuinely benefit from separate, focused contexts), or when sub-tasks can run in parallel and a single sequential agent can't.
- **SCALE:** Multi-agent adds coordination overhead (inter-agent messaging, a supervisor's aggregation logic) — only worth that overhead once single-agent-with-more-tools genuinely degrades in quality or can't parallelize.
- **DEPLOY:** Multi-agent systems can be distributed across compute (different agents on different services/regions) in a way a monolithic single agent can't — this becomes a real architectural lever at scale, not just an organizational one.

**Follow-up probe:** "A team wants 5 agents because it 'sounds more sophisticated' for a task a single agent with 3 tools already handles well — how do you push back?" (Multi-agent adds real coordination cost and failure surface (Q10) — the burden of proof is on showing single-agent-with-tools is insufficient, not the reverse; complexity should be earned by a demonstrated limitation, not assumed as inherently better.)

---

### Q8. Design a supervisor/orchestrator pattern for a multi-agent system.

- **WHY:** Without a coordinating layer, agents have no shared sense of overall task state, ordering, or how to reconcile potentially conflicting sub-agent outputs.
- **HOW:** A supervisor agent receives the overall task, delegates sub-tasks to specialist agents (in sequence or parallel depending on dependencies), collects their results, and either synthesizes a final answer or makes the final decision incorporating each specialist's input (JMA's `SupervisorAgent.cs` orchestrating `ClaimValidatorAgent`/`FraudDetectorAgent`/`PolicyCheckerAgent`).
- **WHEN:** Any multi-agent system needs this coordinating layer — pure peer-to-peer agent communication without any coordinator tends to produce unpredictable, hard-to-debug emergent behavior for anything beyond the simplest two-agent handoff.
- **SCALE:** The supervisor becomes a coordination bottleneck as specialist-agent count grows — parallelizing independent specialist calls (not serializing them unnecessarily) is what keeps latency reasonable as the system grows.
- **DEPLOY:** The supervisor and its specialists can be deployed as independently scalable services — specialist agents with different load profiles (fraud detection called on every claim vs a rare escalation path) can scale independently if decoupled behind the supervisor's orchestration.

**Follow-up probe:** "Two specialist agents return conflicting recommendations to the supervisor — what's the resolution logic, and who decides?" (This needs to be an explicit, designed policy — not implicit model judgment call-by-call — e.g., a defined precedence order (policy compliance overrides efficiency), a confidence-score comparison, or escalation to a human when specialists disagree above a certain severity; leaving this undefined is where multi-agent systems produce inconsistent real-world outcomes.)

---

### Q9. What does the A2A (Agent-to-Agent) protocol standardize, and why does it matter architecturally?

- **WHY:** Without a standard, every agent-to-agent integration is bespoke — A2A defines a common way for independently-built agents (potentially from different vendors/frameworks) to discover each other's capabilities and exchange structured task requests/results.
- **HOW:** Agents expose a capability/skill card (what they can do), and communicate via a standardized message format for task delegation and results — analogous to what MCP does for agent-to-tool, but for agent-to-agent.
- **WHEN:** Valuable once agents need to interoperate across team/framework/vendor boundaries — inside a single team's tightly-coupled multi-agent system (like JMA's SK-based `AgentBus.cs` pattern), a custom internal message format is simpler and sufficient; A2A's value is cross-boundary interoperability.
- **SCALE:** Standardization is what lets an agent ecosystem grow without every new agent needing custom integration code with every existing agent — this is precisely the scaling problem A2A targets.
- **DEPLOY:** A2A messages crossing regions or organizational network boundaries need the same security/schema-validation rigor as any external API call — treat inter-agent messages as untrusted input at a trust boundary, not as internal trusted state.

**Follow-up probe:** "Your team's internal agents already communicate fine via a custom message bus — what would justify adopting A2A?" (Only if you need to interoperate with agents outside your team's direct control/framework choice — e.g., a partner's agent, or a different internal team on a different stack; A2A solves an interoperability problem that a closed, single-team system doesn't have yet.)

---

### Q10. What is failure propagation in meta-agent hierarchies, and how do you contain it?

- **WHY:** In a supervisor-of-supervisors (meta-agent) hierarchy, a single specialist agent's failure or bad output can silently propagate upward and corrupt a decision several levels removed from where the actual error occurred, unless each level explicitly validates what it receives.
- **HOW:** Each level in the hierarchy validates/bounds what it accepts from the level below (confidence thresholds, schema validation, sanity checks) rather than blindly trusting and forwarding sub-agent output — plus circuit-breaking a hierarchy branch that's failing repeatedly rather than letting failures cascade upward indefinitely.
- **WHEN:** Any hierarchy deeper than a single supervisor-specialist layer — the deeper the hierarchy, the more critical explicit validation at each boundary becomes, since there are more opportunities for silent corruption.
- **SCALE:** Failure-propagation risk compounds with hierarchy depth and breadth — a wide hierarchy (many specialists) has more failure sources; a deep hierarchy (many levels) has more opportunities for a failure to travel far from its origin before being caught.
- **DEPLOY:** Distributed meta-agent hierarchies (levels running on different services/regions) add network-partition failure modes on top of logical failure propagation — a level needs to handle "I got no response" distinctly from "I got a bad response," since the correct recovery action differs.

**Follow-up probe:** "A low-confidence fraud-detection signal from a leaf-level agent ends up silently driving a top-level auto-denial decision — how did the hierarchy fail, and what's the fix?" (A level above blindly forwarded/weighted a low-confidence signal as if it were high-confidence — the fix is explicit confidence-threshold gating at each level, where low-confidence signals get flagged for escalation/human review rather than silently influencing an automated decision with the same weight as a high-confidence one.)

---

### Q11. What is an MCP Hub, and how does MCP relate to (or differ from) APIM as a gateway pattern?

- **WHY:** MCP (Model Context Protocol) standardizes how an agent discovers and calls tools/data sources; APIM (API Management) is a general-purpose API gateway (auth, rate limiting, routing) — they solve adjacent but distinct problems, and conflating them leads to either duplicated capability or a gap.
- **HOW:** An MCP Hub centralizes tool/resource registration so multiple agents can discover and call a shared set of tools through one standardized interface (JMA's `MCPToolRegistry.cs`), while APIM (JMA's `APIMGateway.cs`) provides the traditional gateway concerns — authentication, rate limiting, traffic shaping — in front of the actual backend APIs those tools call.
- **WHEN:** A hybrid pattern is common and often correct: MCP Hub for agent-facing tool discovery/standardization, fronted by or layered with APIM for the underlying API management concerns (auth, quota, monitoring) that MCP itself doesn't specify — not an either/or choice.
- **SCALE:** As the number of agents and tools grows, a central MCP Hub is what prevents N agents × M tools worth of bespoke point-to-point integration — the same scaling argument as A2A (Q9), applied to agent-to-tool instead of agent-to-agent.
- **DEPLOY:** An MCP Hub/APIM combination can be deployed regionally (each region's agents talk to a regional hub/gateway) or centrally, depending on latency tolerance and whether tool backends themselves are regional or global.

**Follow-up probe:** "Design the hybrid MCP + APIM pattern for JMA's MCPHub folder — where does each piece sit?" (MCP Hub/Tool Registry as the agent-facing discovery and standardized-call layer; APIM sitting behind or alongside it enforcing auth, rate limits, and routing to the actual backend services — the agent talks MCP, the gateway enforces the traditional API-management concerns transparently underneath.)

---

### Q12. Design the agent-to-agent communication bus pattern (message passing) for a multi-agent system.

- **WHY:** Direct method calls between agents tightly couple them and make it hard to add, remove, or scale agents independently — a message bus decouples "who sends" from "who's currently listening," which is what enables the supervisor/specialist pattern to scale and evolve.
- **HOW:** Agents publish structured messages (task requests, results, status) to a bus (JMA's `AgentBus.cs`/`AgentMessage.cs` pattern) rather than calling each other directly; the bus handles routing, and agents subscribe to the message types relevant to their role.
- **WHEN:** Any multi-agent system beyond a simple two-agent direct handoff — the decoupling pays off as soon as you need to add/modify agents without redeploying every other agent that talks to them.
- **SCALE:** A message bus naturally supports horizontal scaling (multiple instances of the same specialist agent consuming from the same queue/topic) in a way direct method calls don't — this is a real throughput lever as specialist-agent load grows.
- **DEPLOY:** The message bus itself needs a deployment-topology decision — regional bus instances with cross-region replication for global multi-agent systems, or a single global bus accepting the cross-region latency, depending on how latency-sensitive the agent coordination is.

**Follow-up probe:** "A specialist agent is overwhelmed with requests during a traffic spike — how does the message-bus pattern help here that direct calls wouldn't?" (Multiple instances of that specialist agent can consume from the same message queue/topic and scale horizontally behind it — with direct method calls, the caller would need to know about and load-balance across specialist instances itself; the bus abstracts that away.)

---

## Agent Memory

### Q13. Differentiate short-term/session memory from long-term memory in an agent system.

- **WHY:** Session memory holds the current conversation/task's working context (what's needed to complete *this* interaction); long-term memory persists information across sessions (what should be remembered *next time*) — conflating them either bloats every session's context with irrelevant history, or loses genuinely useful context between sessions.
- **HOW:** Session memory is typically just the accumulating message history within the current context window (or a summarized/compacted version of it, Q15). Long-term memory is a separate persisted store (often vector-backed, Q14) queried and selectively injected into a new session's context when relevant.
- **WHEN:** Session memory always — it's the basic mechanism of a multi-turn conversation. Long-term memory when the use case genuinely benefits from continuity across sessions (a returning user's preferences, a claim's history across multiple interactions) — not every agent needs it.
- **SCALE:** Session memory's cost scales with conversation length within one session (context window pressure, Module 1 Q6). Long-term memory's cost scales with the number of users/entities being remembered and how much is stored per one — a different scaling axis entirely.
- **DEPLOY:** Session memory is typically ephemeral/in-process or in a fast regional cache; long-term memory needs durable, possibly cross-region-replicated storage if a user's history needs to be available regardless of which region serves their next session.

**Follow-up probe:** "A user's second session doesn't reflect anything from their first session even though long-term memory is implemented — what's the likely gap?" (Long-term memory was written but never actually queried/retrieved and injected at the start of the new session — implementing storage without implementing retrieval-and-injection is a common incomplete pattern; also check if sessions are landing in different regions with non-replicated memory stores, Q17.)

---

### Q14. How is vector-backed long-term agent memory actually implemented?

- **WHY:** Long-term memory needs to be *searchable by relevance*, not just stored — a growing history of past interactions can't all be stuffed into every new session's context, so relevant pieces need to be retrieved on demand, the same underlying problem RAG solves for documents.
- **HOW:** Past interactions/facts are embedded and stored in a vector index (conceptually the same mechanism as RAG's document index, Module 3); at the start of a new session, a query against that memory (based on the current context) retrieves the most relevant past memories to inject.
- **WHEN:** Once session count or history length per user grows large enough that "just include everything" would blow the context budget — for a handful of short past sessions, simpler storage/retrieval may suffice.
- **SCALE:** Same scaling characteristics as RAG's vector index (Module 3) — storage and query cost grow with total memory volume across all users, and per-user memory volume grows over the relationship's lifetime, which is a slower but steadily accumulating cost.
- **DEPLOY:** Same regional/residency considerations as any vector index (Module 2 Q17) — arguably more sensitive here since long-term memory often contains personal/identifying interaction history, raising data-residency and retention-policy questions beyond a general document index.

**Follow-up probe:** "How is vector-backed agent memory different from RAG over documents, architecturally?" (Conceptually the same retrieval mechanism, but the *source* of the embedded content is the agent's own interaction history rather than static documents — which means it's continuously being written to during normal operation, not just periodically re-indexed, and needs a retention/deletion policy since it accumulates personal data indefinitely otherwise.)

---

### Q15. When and how do you summarize/compact conversation history instead of letting it grow unbounded?

- **WHY:** Context window is finite and costly (Module 1 Q6) — an unbounded, ever-growing conversation history eventually exceeds the window or becomes prohibitively expensive per turn, and 'lost in the middle' degrades quality well before the hard limit is even hit.
- **HOW:** Periodically (e.g., every N turns, or when approaching a token threshold) summarize the older portion of the conversation into a condensed form, replacing the raw message history with the summary plus the most recent turns verbatim.
- **WHEN:** Any conversation/session with enough turns to approach a meaningful fraction of the context budget — not needed for short, bounded interactions.
- **SCALE:** Summarization itself costs a model call — at high session volume with long conversations, this recurring cost needs to be weighed against simply truncating older history (cheaper, but loses information summarization preserves).
- **DEPLOY:** Not deployment-topology-specific — but summarization quality/consistency should be validated the same way generation quality is (Module 3 Q13), since a bad summary silently degrades every subsequent turn's context quality.

**Follow-up probe:** "After several rounds of summarization, the agent seems to have 'forgotten' a detail from early in the conversation that the user references — what happened?" (Progressive summarization is lossy by design — each summarization pass can drop or compress details that seemed unimportant at the time but turn out to matter later; mitigations include preserving explicitly-flagged key facts verbatim across summarization passes, or falling back to long-term/vector memory retrieval for anything the summary dropped.)

---

### Q16. How do you scope memory in a multi-agent system — shared vs private memory per agent?

- **WHY:** If every specialist agent shares one undifferentiated memory pool, agents can act on context outside their responsibility (a fraud-detection agent reasoning off policy-compliance-specific memory it shouldn't weight), or leak information across trust boundaries in a multi-tenant system; if memory is entirely private per agent, useful shared context has to be re-derived redundantly by every agent.
- **HOW:** Explicit memory scoping — a shared "task/session" memory layer visible to all agents collaborating on one task (the supervisor's aggregated context), plus private per-agent memory for that agent's own specialized reasoning history, with clear rules about what's promoted from private to shared.
- **WHEN:** Design this scoping explicitly from the start of any multi-agent system with more than a trivial number of specialists — an undesigned default (usually "everything shared" for simplicity) tends to cause the cross-contamination problems above once the system grows.
- **SCALE:** Shared memory becomes a coordination bottleneck and a growing context-injection cost as specialist-agent count grows if not curated — every agent potentially pulling from the same large shared pool doesn't scale the way well-scoped private memory + selective shared context does.
- **DEPLOY:** In multi-tenant deployments (Module 3 Q15's access-control problem applies here too) shared memory scoping must also respect tenant boundaries — a shared memory pool must never let one tenant's agent session see another tenant's memory, regardless of both being "shared" within the platform.

**Follow-up probe:** "A fraud-detection specialist agent's recommendation appears to have been influenced by unrelated policy-compliance memory it shouldn't have had visibility into — what's the architectural fix?" (Memory scoping was too broadly shared — the fix is restricting that specialist's memory access to only what's relevant to its responsibility, with the supervisor deciding what gets promoted to shared context rather than every agent having blanket access to everything.)

---

### Q17. How do you handle memory staleness/invalidation in a long-lived agent system?

- **WHY:** Long-term memory reflects facts as they were *at the time they were stored* — if the underlying reality changes (a policy updates, a user's status changes) and memory isn't invalidated/refreshed, the agent confidently acts on outdated information, the same class of problem as a stale RAG index (Module 3 Q12).
- **HOW:** Tie memory entries to a source-of-truth freshness signal where possible (re-validate against the live system before acting on a memory-derived fact for anything consequential), or define explicit TTL/expiration on memory entries tied to how fast that category of fact typically changes.
- **WHEN:** Critical for any fact memory that could be used to make a consequential decision (eligibility, policy compliance) — less critical for memory used only for conversational continuity/personalization where staleness has low real-world cost.
- **SCALE:** As memory volume grows, a blanket "re-validate everything against source-of-truth before use" policy gets expensive — tiering by consequence (re-validate high-stakes facts, trust cached memory for low-stakes ones) is the practical scaling answer.
- **DEPLOY:** Freshness signals from a source-of-truth system need to be available with acceptable latency from wherever the memory-consuming agent is deployed — a memory-invalidation check that requires a slow cross-region call defeats much of the purpose of having fast local memory.

**Follow-up probe:** "An agent approves a claim based on remembered eligibility status that changed yesterday — how did the architecture fail, and what tier of fix is needed?" (This is a high-stakes fact that should have been re-validated against the live eligibility system before the approval action, not trusted from memory alone — the fix is explicit re-validation-before-action for consequential decisions, not a general memory-freshness improvement across the board.)

---

### Q18. What is agentic hallucination, and how is it distinct from the factual/RAG hallucination covered in Modules 1 and 3?

- **WHY:** Factual hallucination (Module 1 Q9) is the model generating an incorrect *statement*; agentic hallucination is the model generating an incorrect *action* — a fabricated tool call, a hallucinated argument, or a confidently wrong multi-step plan — which has real-world consequences a mere false statement doesn't, since agents actually execute actions.
- **HOW:** Agentic hallucination manifests as: calling a tool that doesn't exist, hallucinating plausible-but-wrong arguments to a real tool (Q4), or reasoning its way to an incorrect plan/sequence of actions with high apparent confidence. Mitigation is layered: schema + semantic argument validation (Q4), bounded step/retry limits (Q2, Q5), and human-in-the-loop gating for consequential/irreversible actions.
- **WHEN:** This risk exists in every agentic system with tool access — the mitigation intensity should scale with the consequence/reversibility of the actions the agent can take, not be uniform.
- **SCALE:** More tools and more autonomous multi-step reasoning both increase the surface area for agentic hallucination — a simple single-tool agent has a much smaller hallucination surface than a complex multi-agent, multi-tool, multi-step system.
- **DEPLOY:** Same validation/gating infrastructure needs to exist consistently at every deployment tier — this isn't a risk that's acceptable to skip in dev and add "later" for production; agentic hallucination in a dev environment against dev data is exactly where you want to catch and fix it before production exposure.

**Follow-up probe:** "Your agent hallucinated a drug interaction warning that's already in a draft clinical note — walk through your immediate response." (This is a terror-question-style scenario from prior interview prep: immediate containment — flag/quarantine the draft before it's finalized or acted upon, trigger the human-in-the-loop review gate that should already exist for clinical-consequence outputs, root-cause whether this was a factual hallucination in generation or an agentic hallucination in a tool call/lookup step, and only then address the systemic fix — the immediate priority is preventing the erroneous content from propagating further, not the retrospective analysis.)

---

*Module 4 of 6 — GenAI Architect Interview Prep. Next: Module 5 — Solution & Deployment Architecture.*
