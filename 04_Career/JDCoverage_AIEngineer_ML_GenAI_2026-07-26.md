# JD Coverage — AI Engineer (ML · GenAI · Agentic AI)

**Analysed:** 2026-07-26 · **Method:** grep-verified against the library, not inferred from `00_INDEX.md`
**Companion:** the Forward Deployed AI Engineer JD is tracked separately and in more depth in
`08_Jobs/FDE/FDE-Prep_Tracker.md` — 60 rows, learning order, build status. This file covers the
*other* live JD.

> **Format follows `JDCoverage_Synergech_Lorven_2026-07-19.md`.**
> 🟢 library covers · 🔵 you have it (day job) · 🟡 partial · 🔴 gap

---

## Headline

**This JD is a different role shape than the library was built for.** The library is Azure-first,
C#-first, GenAI-first. This JD is **Python-first, framework-first, classical-ML-first**.

| Half of the JD | Coverage |
|---|---|
| **GenAI · Agentic · RAG · LLMs** | **~85%** |
| **Classical ML · advanced Python · the DS library stack** | **~20%** *(before Part 7; ~55% after `L32`)* |

---

## 🟢 Strong — defensible at interview altitude

| JD requirement | Where | Depth |
|---|---|---|
| LLMs — attention, tokenization, embeddings | `L11_1`–`L11_4` | ● 4 files |
| RAG architectures | `L13` (1,527 ln) · `L23` CAG · `P6/01-CareerAccelerator/07-GraphRAG-Neo4j/` | ● |
| Prompt Engineering | `L15` | ● |
| Vector Databases | `L09` · FAISS in `P6/…/01-Ollama-LocalRAG/` + `PythonTrack/Part1-AI-LLMs.md` | ● |
| **LangGraph** | `L25` — StateGraph, TypedDict state, Checkpointer, `interrupt_before`, worked code | ● |
| **LangChain** | `L25` §4 (LCEL, when to graduate) · `L21` §21.4 | ◐→● |
| **LlamaIndex** | `P6/01-CareerAccelerator/05-LlamaIndex-RAG/` — **only coverage in the library** | ● |
| Agentic AI frameworks | `L16` (2,084 ln) · all of Part 5 (`L22`–`L31`) · `…/02-crewAI-MultiAgent/` | ●● strongest area |
| OpenAI APIs | `L12` — Azure OpenAI flavour | ● |
| Google Gemini | `P6/01-CareerAccelerator/09-Vertex-AI/` — concepts, architecture, `hands_on.py` | ● |
| Production-ready AI applications | `L18` · `L20` · `L31` · both Part 6 platforms | ● |
| Stakeholder communication · roadmaps · technical leadership | `04_Career/` · `05_Assessments/VitalCare` (1,562 ln, solo) · InterviewBank | ● |

---

## 🟡 Partial — survives a question, not a deep probe

| JD requirement | Reality |
|---|---|
| **ML pipelines** (ingest → train → deploy → monitor) | `L06` covers Azure ML end to end (AutoML, Designer, online/batch endpoints); `L19` covers CI/CD, drift, retraining, monitoring. But it is **portal- and Azure-SDK-flavoured**. The JD means code-first sklearn/PyTorch pipelines |
| **Multimodal (text · image · audio · video)** | All four exist separately — `L04` vision, `L05` speech, Whisper in `L12`/`L17`/`L22`, Gemini video in `P6/…/09-Vertex-AI/`. **Nothing builds an integrated multimodal application** |
| **Anthropic API** | Constitutional AI in `L11_4` · MCP in `L26` · Claude-via-Bedrock in `P6/…/06-Amazon-Bedrock/04_hands_on.py`. **The direct Claude API is absent** |
| **Pinecone** | 7 files, all decision tables ("when Pinecone vs AI Search"). No hands-on |
| **FAISS** | ● in PythonTrack and the Ollama module — genuinely covered |

---

## 🔴 Gaps

### 1. The DS library stack — near-zero

| Library | What is actually there |
|---|---|
| **XGBoost / LightGBM** | 5 lines in `L06`, **all AutoML leaderboard output**. Never taught, never coded |
| **Scikit-learn** | 4 environment-pinning mentions in `L06` + one ~15-line "how to read a notebook" snippet in `L21` |
| **PyTorch** | Named in `P6/…/04-HuggingFace-Transformers/03_interview_qa.md` and a curriculum outline. **No teaching** |
| **TensorFlow** | One mention, `L04` |
| **MLflow** | 4 one-line mentions. Tracking, registry and serving never taught |
| **NumPy / Pandas / Matplotlib** | Import lines and `df.head()` / `df.describe()` |

### 2. Advanced Python — was 3 of 8, now 8 of 8

| JD sub-skill | Before 2026-07-26 | Now |
|---|---|---|
| OOP · Functions & Modules · Exception Handling | ✅ `L21` §5, §6, §8 — *basics* | ✅ deepened in `L32` §5 |
| Type Hints | ◐ `L21` §2 | ✅ **`L32` §1** |
| **Data Classes** | ❌ | ✅ **`L32` §1.2** |
| **Iterators & Generators** | ❌ | ✅ **`L32` §2** |
| **Decorators** | ❌ | ✅ **`L32` §3** |
| **Data Structures & Algorithms / Big-O** | ❌ | ✅ **`L32` §6** |
| **Design Patterns** | ❌ | ✅ **`L32` §7** |

**`L32_AdvancedPython_for_AI.md` (762 lines) closes this row.** It did not exist when this JD was
first analysed; it is the single highest-return study block for this posting.

### 3. The trap that produced the wrong answer before

`06_Supplementary/PythonTrack/AIMLcurriculum.md` (558 ln) and `AIMLcurriculum-gaps.md` (110 ln)
*list* decorators, generators, NumPy, PyTorch, MLflow, ONNX, quantization and vLLM. They are
**syllabi, not lessons** — bullet outlines of topics to learn.

`00_START_HERE.md` previously pointed at PythonTrack as coverage for *"PyTorch / ML math / classical
ML."* **That pointer was wrong and was corrected on 2026-07-26.** PythonTrack's actual teaching files
(`1.4-FineTuning`, `1.5-AIAgents`, `Part1-AI-LLMs`) are all GenAI.

---

## Verdict

| | |
|---|---|
| **Apply if** | the team's real weight is on the Generative/Agentic side and "8+ years ML" is HR boilerplate. Part 5 + Part 6 are genuinely strong; LangGraph, LlamaIndex, crewAI and RAGAS are all defensible |
| **Expect to lose on** | a live Python screen *(mitigated — `L32` now exists)* or *"walk me through your sklearn/XGBoost training pipeline"* |

### Fix list

| # | Gap | Effort | Status |
|---|---|---|---|
| 1 | Advanced Python — decorators, generators, dataclasses, Big-O | ~6 hrs | ✅ **module built** (`L32`), study pending |
| 2 | One real sklearn + XGBoost + MLflow pipeline, end to end in a notebook | ~8 hrs | 🔴 **not built** — converts three ❌ rows at once |
| 3 | Anthropic Claude API | ~2 hrs | 🔴 not built |
| 4 | An integrated multimodal demo (text + image + audio) | ~6 hrs | 🔴 not built |

**Item 2 is the highest-value thing still missing from the library for this JD.** It is the classical-ML
equivalent of what Part 7 did for platform engineering, and it is not covered by `L32`–`L36`.

---

## Related

`08_Jobs/FDE/FDE-Prep_Tracker.md` (the other live JD, 60 rows) ·
`JDCoverage_Synergech_Lorven_2026-07-19.md` (prior JDs, same format) ·
`AI103_GapToCertification_2026-07-19.md` · `01_Lessons/Part7_PlatformEngineering/L32_AdvancedPython_for_AI.md`
