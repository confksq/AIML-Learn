# AI-103 — Gap to Certification Plan

**Created:** 2026-07-19
**Target:** Microsoft Certified: Azure AI Apps and Agents Developer Associate
**Exam:** AI-103 — Developing AI Apps and Agents on Azure

| Detail | Value |
|---|---|
| Cost | $165 USD |
| Duration | 120 min · ~40–60 questions |
| Passing score | 700 / 1000 |
| Delivery | Pearson VUE |
| Certification GA | June 2026 |
| Skills measured as of | **April 16, 2026** |
| Study guide last updated | **July 7, 2026** |

> **Source:** official Microsoft Learn study guide only —
> https://learn.microsoft.com/en-us/credentials/certifications/resources/study-guides/ai-103
> Third-party prep sites and braindumps were deliberately excluded: the cert is ~1 month GA, so that
> content is thin and SEO-generated, and dumps violate exam policy.
> **Re-check the study guide before booking** — Microsoft revises these and posts a change log.

---

## Headline

**25 objectives covered · 25 partial · 13 gaps.**

The good news is structural: your library is deepest exactly where the exam is heaviest. Domains 1
and 2 together are **55–65% of the exam** and have **zero hard gaps**.

The risk is not where it looks. The visible hole is Computer Vision (12 of 15 bullets missing) — but
that caps at 15%. The real risk is that **Domain 2, the largest domain, is covered by analogues
rather than by the thing being tested.**

---

## Coverage by domain

| Domain | Weight | Covered | Partial | Gap |
|---|---|---|---|---|
| 1 · Plan and manage an Azure AI solution | 25–30% | 8 | 8 | 0 |
| 2 · Implement generative AI and agentic solutions | **30–35%** | 8 | 8 | 0 |
| 3 · Implement computer vision solutions | 10–15% | 0 | 3 | **12** |
| 4 · Implement text analysis solutions | 10–15% | 4 | 3 | 1 |
| 5 · Implement information extraction solutions | 10–15% | 5 | 3 | 0 |
| **Total** | | **25** | **25** | **13** |

**Strongest bullets on the whole exam:** semantic/hybrid/vector search for grounding (`L09`, `L13`),
retrieval and indexing method selection, RAG implementation, agent memory and tool integration.

---

## The four findings that actually decide this

### 1. Domain 2 is taught through the wrong implementation ⚠️ *biggest weighted risk*

You know agents deeply — but via **Semantic Kernel in C#**, **crewAI in Python**, and a custom
`SupervisorAgent.cs`. The exam tests the **Foundry Agent Service**: agent/thread/run objects, tool
schema registration, connected agents, approval-flow controls, run-level tracing — through the
Python **`azure-ai-projects`** SDK.

`AIProjectClient` appears **exactly once** in the entire lesson corpus. "Foundry Agent Service"
appears in 7 files, **none of them lessons**.

Strong conceptual knowledge that doesn't convert on implementation-shaped items is the specific
failure mode here.

### 2. Azure Content Understanding — a footnote that carries 15–20% of the exam

The objectives invoke it in Domain 2.2 (agent tools), all of Domain 3.2, and **both** bullets of
Domain 5.2. The library contains roughly **10 substantive lines** about it — two table rows in `L08`,
one in `L17`, two Q&A entries.

Nothing on analyzers, schema definition, field extraction config, pro mode vs standard mode, markdown
output from analyzers, or video/audio analyzers.

### 3. Python is a real liability for an exam that assumes app development

The audience profile states: *"you should have experience developing apps by using Python."*

Code-fence census across `01_Lessons/`:

| | Count |
|---|---|
| C# blocks | **123** across 19 files |
| Python blocks | **34** — and 24 are inside `L21` alone |
| Portfolio `.cs` files | 51 |
| Portfolio `.py` files | 13 (1,358 lines total, avg 104 lines) |

Every service lesson — Vision, Speech, Language, Document Intelligence, AI Search, Azure OpenAI, RAG,
SK, MLOps — teaches its API calls in **C#**. `L21_Python_for_AI.md` is explicitly a *"C# Developer
Fast-Track"*: it teaches Python **reading and snippet-writing**, not application development. No
project structure, packaging, pytest, FastAPI, or error/retry architecture.
`06_Supplementary/PythonTrack/` is 4,903 lines of **prose**, not code.

**Verdict: partial, bordering on live risk** — concentrated in the 30–35% domain.

### 4. `L17` has a currency problem

- The exam says **"Microsoft Foundry."** That string appears **3 times library-wide, never in a
  lesson** — only in prep chat notes, where it was observed as a portal breadcrumb and *not
  recognized as a rename*. The evidence was there and got missed.
- **Prompt Flow occupies Topic 17.3** as a primary authoring surface. It's been de-emphasized in the
  current Foundry story — stale weight on a superseded surface.
- `L17`'s "2026 Updates" block retro-patches Agents GA, Connected Agents, Content Understanding,
  Evaluation GA, Tracing GA, and Model Router as **one-line table rows**. That's the tell: the lesson
  body predates the current product, and the most exam-relevant capabilities exist only as appended
  one-liners.

---

## Study plan — sequenced by weighted impact

Ordered so the heaviest exam weight is addressed first. Hours are study + hands-on.

| # | Focus | Closes | Hrs | Why this order |
|---|---|---|---|---|
| **1** | **Foundry Agent Service + `azure-ai-projects` in Python — hands-on** | D2 (30–35%) | 12–16 | Largest domain, currently taught via analogues. Build one real agent: threads, runs, tool schemas, connected agents, approval flow, run tracing |
| **2** | **Azure Content Understanding end-to-end** | D2 + D3 + D5 (~15–20%) | 8–10 | One service, three domains. Analyzers, schema definition, field extraction, pro vs standard mode, markdown output, video/audio analyzers |
| **3** | **Python application development uplift** | cross-cutting | 10–12 | Not syntax — project structure, packaging, async, error/retry, SDK idioms. Rewrite one existing C# portfolio piece in Python |
| **4** | **Image + video generation and editing** | D3.1 (~7%) | 8–10 | Shallow, well-documented surface. DALL-E/GPT-image-1 APIs, size/quality/style params, inpainting, masks, reference media, video generation and editing |
| **5** | **Multimodal understanding + vision RAI** | D3.2, D3.3 (~7%) | 6–8 | Captioning via LLMs, visual QA grounding, alt-text/accessibility, object/region ID, visual content filters, **image-embedded prompt injection** |
| **6** | **`L17` refresh to current Foundry** | D1 + D2 | 4–6 | Rewrite around Microsoft Foundry naming and the Agent Service; demote Prompt Flow; promote the retro-patched one-liners into taught material |
| **7** | **Partial-objective sweep** | D1, D2, D4 | 6–8 | Provenance metadata + approval workflows · agent governance/tool-access models · index health + relevance monitoring · self-critique and reflection loops · hybrid LLM + rules engines · audio-native multimodal reasoning · Foundry CI/CD integration |

**Total: 54–70 hours.** At your documented pace (22 hrs/week) that's **3–4 weeks**.

### Minimum viable path

If time-boxed, items **1–3 (30–38 hrs)** address roughly **55–65%** of exam weight and the two
cross-cutting liabilities. Domain 3 alone can't fail you at 10–15%; Domain 2 plus a Python handicap
can.

---

## What you already have — don't re-study these

Revise from existing files rather than starting fresh:

| Objective area | Your file |
|---|---|
| Retrieval, indexing, hybrid/semantic/vector search | `01_Lessons/Part2_AzureAIServices/L09_AzureAISearch.md` |
| RAG implementation, chunking | `01_Lessons/Part3_GenAI_LLMs/L13_RAG_DeepDive.md` |
| Agent memory, tool vs knowledge vs fine-tune | `01_Lessons/Part3_GenAI_LLMs/L16_...md` · `02_Questions/HighLevelPrep/HLP01_...md` |
| Quotas, TPM, scaling, cost | `01_Lessons/Part3_GenAI_LLMs/L12_AzureOpenAI_Services.md` |
| Security: managed identity, private endpoints, RBAC | `01_Lessons/Part2_AzureAIServices/L07_...md` |
| Evaluation, groundedness, RAI instrumentation | `L17` Topic 17.4 · `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/03-RAGAS-Evaluation/` |
| Prompt engineering, model parameters | `01_Lessons/Part3_GenAI_LLMs/L15_PromptEngineering.md` |
| Document Intelligence, OCR, layout, field extraction | `01_Lessons/Part2_AzureAIServices/L08_DocumentIntelligence.md` |
| Sentiment, PII, NER, translation | `01_Lessons/Part1_Foundations/L03_NLP_Fundamentals.md` |
| Speech-to-text, TTS, custom speech | `01_Lessons/Part1_Foundations/L05_SpeechServices.md` |
| Monitoring, drift, CI/CD | `01_Lessons/Part4_Architecture/L19_MLOps_LLMOps.md` |

---

## Note on the resume

`C:\Users\confksq\Project\jbs\syner.txt` and `finan.txt` currently list **AI-103 as held**. Records in
this library show **AI-102 only** (confirmed 2026-06-30), and AI-103 only reached GA in June 2026.

Certification claims are among the easiest things for a client to verify — Microsoft credentials are
publicly checkable via a transcript link. Worth resolving before the Synergech and Lorven
conversations. It is also the most fixable item on that resume: this plan is 3–4 weeks.

---

## Cross-references

- JD coverage analysis → `04_Career/JDCoverage_Synergech_Lorven_2026-07-19.md`
- High-level prep (memory, tokens, scaling, agents) → `02_Questions/HighLevelPrep/HLP01_Memory_Tokens_Scaling_Agents.md`
- Foundry agent prep notes → `08_Jobs/AscndIntr/PrepPlan/` + `ChatHist/Module01_AzureAIFoundry_HubProject_AgentTools_2026-06-27.md`
- Library index → `00_START_HERE.md`
