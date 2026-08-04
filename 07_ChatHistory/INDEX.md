# Chat History Index

**Last updated:** 2026-08-03 · **48 files, ~5.1 MB**

> ⚠️ **Still unindexed:** the 2026-07-29 (×4) and 2026-08-02 (×3) sessions sit in this folder but
> have never been read to write accurate topic summaries — they are **not** in the table below.
> The 2026-08-03 row was written from the live session and is accurate.

All session transcripts. Consolidated 2026-07-18 from four former locations
(`PartsModules/ChatHist/`, `NewLearn/ChatHist/`, `AIFoundry/`, and one misfiled in `PartsModules/` root).

These are raw transcripts — the distilled content lives in `01_Lessons/`.
Search here only when you need the *reasoning* behind a lesson, not the lesson itself.

> ⚠️ This index was stale from 2026-07-19 to 2026-07-26 — it stopped at the library-reorg session
> and was missing eight transcripts.

---

## Claude Code sessions (newest first)

| Date | File | Topic |
|---|---|---|
| **2026-08-03** | `Session_ChatHistory_2026-08-03_Part8-Fabric_Phase2-Phase6.md` | **Built Part 8 — `L37_MicrosoftFabric.md`** (883 lines) + `QA_L37` (17 Qs). Resumed after an unexpected window close: state reconstructed from `git status` + an **empty `Part8_DataPlatform/` folder timestamped 3 min after the last commit**. Phase 2 + Phase 6 of the consolidation plan · OneLake/shortcuts · Direct Lake + **DirectQuery fallback** · Medallion worked through on VitalCare prior-auth · **Fabric↔Foundry: SQL tool vs RAG routing**, and *does the agent respect RLS?* (no — a copied index detaches the security model) · CU smoothing/throttling · **GraphRAG Local vs Global Search** added as Q8a · 4 indexes + 35 concepts · ML gaps #72/#73 and FDE #60 closed · both PRDs marked delivered with path corrections |
| 2026-07-31 | `Session_ChatHistory_2026-07-31_FDE-Prep.md` | **FDE-Prep** — Forward Deployed Engineer role (Juno Beach, FL) resume tailoring · confirmed tool scope (Copilot + Claude Code only; no Devin/Windsurf) · **2 chronology defects found & fixed** (Bedrock/Claude 3 on a 2019–21 role; GraphRAG pre-dating its July 2024 release at KPMG) · version corrections (Ollama 0.6→0.32, crewAI 1.15→1.14) · MCP spec 2026-07-28 · interview probes |
| 2026-07-27 | `Session_ChatHistory_2026-07-27_L28-vs-L29-A2A-MetaAgent.md` | **FDE-Prep continued** — pushed Part 7 + FDE-Prep reading set + `09_ML/MLEngineer_Coverage` to GitHub · **L28 (Meta-Agent) vs L29 (A2A) distinction**: pattern vs protocol, WHO vs HOW, `L28`'s PENDED-on-failure vs `L29`'s dead-letter-on-failure. **Ends on an open check question** — same mechanism or two? 100 messages |
| 2026-07-26 | `Session_ChatHistory_2026-07-26_FDE-Prep_Part7-Build.md` | **FDE-Prep** — AI agent vs agentic AI · two JD coverage analyses · IaC terminology across Azure/AWS/GCP · **built Part 7 (`L32`–`L36`)**, `Part6/03` vuln write-up, `QA_L32`–`QA_L36`, full index regeneration, 3 stale-index fixes. 85 messages |
| 2026-07-25 | `Session_ChatHistory_2026-07-25_TokenEmbedding-AKS-SKhosting-recall.md` | AI Infrastructure Engineer role · GPU cluster on AKS · node/pod/namespace diagram · token/embedding recall · where to host Semantic Kernel *(superset of `_3`)* |
| 2026-07-25 | `Session_ChatHistory_2026-07-25_3.md` | Same session, earlier export — GitHub account setup, chat-history recall |
| 2026-07-25 | `Session_ChatHistory_2026-07-25_2.md` | **CallMiner pipeline** — AKS↔CallMiner connectivity after firewall rule, DSX scan client, language split |
| 2026-07-25 | `Session_ChatHistory_2026-07-25.md` | Short — ClaimsAutomation chat-history location |
| 2026-07-23 | `Session_ChatHistory_2026-07-23_Agentic-architecture-fundamentals-part-2.md` | **Part 5 Q&A part 2** — when C# vs one LLM call vs an agent loop · ReAct mechanics · vector DB internals (chunk vs vector vs metadata) · SK vs LangChain · scaling agents · Skills. **Ends on 3 unanswered check questions** |
| 2026-07-22 | `Session_ChatHistory_2026-07-22.md` | CallMiner — SETF delivery intent, 66-column mapping, Postgres enrichment, vendor reply processing |
| 2026-07-19 | `Session_ChatHistory_2026-07-19_Agentic-architecture-fundamentals.md` | **Part 5 Q&A part 1** — agent triggers, input/output shape, the JM Family cancellation example, agent-loop model |
| 2026-07-19 | `Session_ChatHistory_2026-07-19_LibraryReorg_Index_JD_AI103.md` | Library reorg into 6 Parts · index generation · Synergech/Lorven JD coverage · AI-103 gap-to-certification plan |
| 2026-07-10 | `Session_ChatHistory_2026-07-10.md` | Save-chat tooling *(was misfiled among lesson files)* |
| 2026-07-08 | `Session_ChatHistory_2026-07-08_CareerAccelerator_Portfolio.md` | CareerAccelerator build, portfolio |
| 2026-06-30 | `Session_ChatHistory_2026-06-30_3.md` | Lesson library build |
| 2026-06-30 | `Session_ChatHistory_2026-06-30_2.md` | Lesson library build |
| 2026-06-30 | `ChatHist_AILearning_ProgressReview_AI102_2026-06-30.md` | Progress review, AI-102 completion |
| 2026-06-21 | `Session_ChatHistory_2026-06-21.md` | Platform implementations |
| 2026-06-18 | `Session_ChatHistory_2026-06-18.md` | Short session |
| 2026-06-17 | `Session_ChatHistory_2026-06-17.md` | Deep learning session |
| 2026-06-14 | `claude-AIFoundryQA-imp.md` | AI Foundry hands-on, JMAVehicleIQA agent, CV gap analysis |
| 2026-06-10 | `Session_ChatHistory_2026-06-10.md` | Learning session |
| 2026-06-09 | `Session_ChatHistory_2026-06-09.md` | Learning session |
| 2026-06-08 | `Session_ChatHistory_2026-06-08.md` | Learning session |
| 2026-06-08 | `Session_ChatHistory_2026-06-08-QandA.md` | az login → Foundry topics 1–2 |
| 2026-06-04 | `claude-session-AIAgent-imp.md` | Agents; generated the `06_Supplementary/PythonTrack/` files |
| 2026-06-03 | `Session_ChatHistory_2026-06-03_2.md` | Learning session |
| 2026-06-03 | `Session_ChatHistory_2026-06-03.md` | Learning session |

## Earlier Claude sessions — `chathistClaude/`

| File | Topic |
|---|---|
| `AIML-LearnQandA-langchain-aiagentskilss-imp.md` | LangChain, agent skills *(267 KB — largest)* |
| `ChunkingEmbedQandA.md` | Chunking + embeddings deep Q&A |
| `Part3-and4-chathistory.txt` | Parts 3 & 4 combined |
| `AIML-Learn-AIfoundary-githubpersonalaccountproject.md` | Foundry + GitHub portfolio setup |
| `copilot-session-AILearnAISearchservice1.md` | Azure AI Search |
| `Token-embedding-imp.md` | Tokens and embeddings |
| `AIML-LearnMLOPS.md` | MLOps |
| `Part3_Module11_Session_ChatHistory_2026-05-18.md` | Attention & Transformers |
| `Session_ChatHistory_2026-05-19.md` | Tokenization, embeddings |

## GitHub Copilot sessions — `chathistCopilot/`

| File | Topic |
|---|---|
| `copilot-session-dac57320-…-AgenticAi.md` | Agentic AI |
| `copilot-session-e087a435-…-AILearn2.md` | AI learning |
| `copilot-session-e087a435-…-AILearn.md` | AI learning |
| `copilot-session-1d7a986a-…-AILearn.md` | AI learning |

---

## Two tracks run through these files

Not all of these are AI learning. Know which you are opening:

| Track | Sessions | Note |
|---|---|---|
| **AI learning** | 06-03 → 07-19 agentic, 07-25 token/embedding recall, 07-27 L28/L29 | The curriculum thread |
| **CallMiner audio pipeline** | 07-22, 07-23, 07-25_2 | ⚠️ Contains **JMFE Internal/Confidential** infrastructure detail — AKS subnets, Key Vault names, service accounts, internal hostnames. Fine to work with locally; never paste anywhere external-facing |

---

## Open threads

**Three unanswered check questions** from `2026-07-23_Agentic-architecture-fundamentals-part-2.md`:

1. Vector search weakness — *"customer wants to cancel"* vs *"customer does NOT want to cancel"*
   produce near-identical vectors. Why is that a problem?
2. Scaling — you add 20 replicas in prod and it gets *worse*. What happened?
3. Skills — is the cancellation agent's refund calculation a good candidate to package as a Skill?

✅ The supervisor-agent pattern question (2026-07-26) was answered 2026-07-27 — see `L28`.

**New, from 2026-07-27:** `L28`'s Supervisor returns `PENDED` when a specialist times out; `L29`'s
AgentBus dead-letters an undeliverable message. Same mechanism wearing two names, or two genuinely
different failure paths — and if different, which lesson owns which?

---

## Saving new sessions

New AIML chat histories go **in this folder**. The `/share` skill's script hardcodes its save
directory to the old `PartsModules/` path — override `save_dir` to `07_ChatHistory/` before running
it, or you get another loose transcript sitting among the lesson files.

**Naming:** `Session_ChatHistory_YYYY-MM-DD_Topic.md`
FDE-Prep sessions: `Session_ChatHistory_YYYY-MM-DD_FDE-Prep.md`
