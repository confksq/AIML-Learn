# Ascendion AI Architect — Master Prep Plan (FINAL)
**Candidate:** 
**Interview:** 
**Client:** 
**Screener confirmed focus:** Hallucination + End-to-end AI Agent Workflow

---

## Final Module List

| # | File | Topic |
|---|---|---|
| 01 | 01_Azure_AI_Foundry.md | Azure AI Foundry — platform + agent lifecycle + fine-tuning decision |
| 02 | 02_CAG_vs_RAG.md | RAG vs CAG — chunking strategies, HNSW, hybrid retrieval, transformer fundamentals |
| 03 | 03_Hallucination.md | Hallucination (factual + agentic) + AI Security (prompt injection, PII, threat modeling) |
| 04 | 04_Framework_Comparison.md | LangGraph vs LangChain vs AutoGen vs SK + state management |
| 05 | 05_MCP_Hub.md | MCP Hub + MCP vs APIM + Governance + Server Boundaries + Segregation |
| 06 | 06_Agent_Workflow.md | CENTERPIECE — end-to-end story + prompt engineering techniques + token optimization |
| 07 | 07_Meta_Agents.md | Meta-Agent hierarchies + failure propagation |
| 08 | 08_A2A_Protocol.md | A2A Protocol + schema validation + audit logging |
| 09 | 09_OCR_Pipelines.md | OCR — Azure Doc Intelligence vs John Snow Labs |
| 10 | 10_Fault_Tolerance.md | Fault tolerance + self-healing + observability + complete LLMOps |
| 11 | 11_Defend_Assessment.md | Defend VitalCare assessment (interviewer has read it) |
| 12 | 12_Mock_Interview.md | Full mock + Terror Questions |

---

## REQUIREMENTS vs CV SKILLS — Coverage Map

> **REQUIREMENTS** = Skills explicitly listed in the Ascendion job description (what they will ask)
> **CV SKILLS** = Skills from your resume/CV (what you have done — they will probe these too)

---

### REQUIREMENTS Coverage

| Requirement | Module | Status |
|---|---|---|
| Design & implement Agentic AI on Azure AI Foundry (enterprise-scale) | 01 | ✅ |
| Architect Multi-Agent Decision Frameworks (collaboration + goal resolution) | 06, 07 | ✅ |
| Fault-tolerant agents — observability, monitoring, self-healing | 10 | ✅ |
| Evaluate & deploy platforms for agent creation + lifecycle | 01 | ✅ |
| Workflow management — agent/node interactions, sequencing, state | 04, 06 | ✅ |
| Meta-Agent (Agents of Agents) hierarchies | 07 | ✅ |
| A2A Protocol — secure inter-agent communication | 08 | ✅ |
| Assess & select frameworks (LangGraph, **LangChain**, AutoGen, SK) | 04 | ✅ |
| Govern MCP Hub — policies and standards across centralized pool | 05 | ✅ |
| MCP Server boundaries, responsibilities, segregation strategies | 05 | ✅ |
| MCP vs Azure APIM decision criteria | 05 | ✅ |
| OCR — Azure Doc Intelligence vs John Snow Labs | 09 | ✅ |
| OCR pre-processing pipelines (de-noise, de-skew, binarization) + post-processing | 09 | ✅ |
| RAG vs CAG — latency, cost, freshness trade-offs | 02 | ✅ |

---

### CV SKILLS Coverage

| CV Skill | Module | Status |
|---|---|---|
| RAG pipeline — document ingestion, chunking, embedding, hybrid retrieval, grounded response | 02 | ✅ |
| Chunking strategies (fixed-size, semantic, paragraph-level) + token budget | 02 | ✅ |
| HNSW indexing + cosine similarity scoring + Azure AI Search vector fields | 02 | ✅ |
| Azure AI Document Intelligence — structured extraction for AI indexes | 09 | ✅ |
| SK agent loop — plugins, planners, memory, .NET-native, function calling | 04, 06 | ✅ |
| Prompt engineering — few-shot, CoT, chaining, output constraints, context-window mgmt | 06 | ✅ |
| Token optimization — compression, streaming, model tier selection | 06 | ✅ |
| AI Security — prompt injection defenses, jailbreak, Content Safety, PII redaction, threat modeling | 03 | ✅ |
| Fine-tuning — supervised fine-tuning, eval dataset, fine-tuning vs RAG vs prompt engineering | 01 | ✅ |
| LLMOps — prompt versioning, CI/CD, model deployment/rollback, automated eval, monitoring | 10 | ✅ |
| Transformer fundamentals — self-attention, tokenization (BPE/WordPiece), LoRA/PEFT, RLHF | 02 | ✅ |
| Embedding space geometry — cosine similarity, semantic distance, dimensionality trade-offs | 02 | ✅ |

---

## 5-Day Schedule
- Wed 06/17: Module 01 + 02
- Thu 06/18: Module 03 + 04
- Fri 06/19: Module 05 + 06
- Sat 06/20: Module 07 + 08
- Sun 06/21: Module 09 + 10 + 11 + 12 (Mock day)

---

## The Terror Questions (must answer on Sunday mock)
1. "Your agent hallucinated a drug interaction. It's already in the draft notes. Walk me through your immediate response."
2. "EU bans cross-border PHI tomorrow. Your multi-region cluster is broken. Restore service in 48 hours."
3. "GPU costs tripled last month. CTO is questioning ROI. Defend your architecture's cost model."
4. "You have to pick between LangGraph and Semantic Kernel. Team is 60% Python, 40% .NET. What do you pick and why?"
5. "Your FHIR interface breaks — EHR vendor pushed an update. How does the AI survive?"

---

## The Centerpiece Question (Module 06 — must nail this)
"Walk me through an end-to-end AI agent workflow you have built — from how the agent receives a task, how it reasons, how it calls tools, how you handle failures, how you prevent hallucination, and how you monitor it in production."
Answer must be 4-5 minutes, structured, from memory, anchored to JM Family production, framed for healthcare.

---

## ⚠️ Module relocation — 2026-07-19

Topic modules 01–10 were promoted into the main curriculum as **Part 5 — Agentic Protocols &
Patterns**, because they contained the library's only MCP, A2A, CAG and LangGraph teaching material
and were invisible while filed under `08_Jobs/`.

| Was | Now |
|---|---|
| `01_Azure_AI_Foundry.md` | `01_Lessons/Part5_AgenticProtocols/L22_Foundry_AgentLifecycle.md` |
| `02_CAG_vs_RAG.md` | `.../L23_CAG_vs_RAG.md` |
| `03_Hallucination.md` | `.../L24_Hallucination_Mitigation.md` |
| `04_Framework_Comparison.md` | `.../L25_AgentFramework_Comparison.md` |
| `05_MCP_Hub.md` | `.../L26_MCP_ModelContextProtocol.md` |
| `06_Agent_Workflow.md` | `.../L27_Agent_Workflow_EndToEnd.md` |
| `07_Meta_Agents.md` | `.../L28_MetaAgent_Hierarchies.md` |
| `08_A2A_Protocol.md` | `.../L29_A2A_Protocol.md` |
| `09_OCR_Pipelines.md` | `.../L30_OCR_Pipelines.md` |
| `10_Fault_Tolerance.md` | `.../L31_FaultTolerance_Observability.md` |

Modules 11 (Defend Assessment) and 12 (Mock Interview) stayed here — they are Ascendion-specific,
not general lesson content.
