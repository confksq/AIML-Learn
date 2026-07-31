# Resume Tailoring Session — FDE / AI Developer, Juno Beach FL

**Date:** 2026-07-31
**Resume updated:** `/mnt/c/pers/Resume-May2026/Bala K - Lead AI Engineer_nextra.docx`
**Pre-edit backup:** `/mnt/c/pers/Resume-May2026/Bala K - Lead AI Engineer_nextra_BACKUP.docx`
**Interview format:** Face-to-face (everything on the page must be defensible live)

---

## 1. The Role

**Job Title:** AI Developer — Forward Deployed Engineer (FDE)
**Type:** Long-term contract | **Location:** Juno Beach, FL — onsite, no remote, no relocation assistance
**Stated experience ask:** 1–2 years (vs. Bala's 17+)

**JD core stack:**
- AI coding assistants: Devin, Windsurf, Claude Code, GitHub Copilot
- MCP (Model Context Protocol) — building/consuming servers
- Secure tunnelling: ngrok, Cloudflare Tunnel, SSH port-forwarding
- Python scripting / automation / API integration
- LLM fine-tuning: LoRA, PEFT, prompt-tuning
- Git/GitHub, REST APIs, webhooks, cloud (AWS/Azure/GCP)
- Nice-to-have: Docker, CI/CD, vector DBs, RAG, prompt engineering, agentic workflows, client-facing experience

---

## 2. Confirmed Hands-On Experience (do not claim beyond this)

Answered by Bala, 2026-07-31:

| Area | Confirmed |
|---|---|
| AI coding assistants | **GitHub Copilot + Claude Code only.** NOT Devin. NOT Windsurf. |
| MCP | **Built and deployed in production** |
| Tunnelling | **All three** — ngrok, Cloudflare Tunnel, SSH port-forwarding |
| Seniority framing | Leave as-is — add keywords only, no repositioning |
| Trim level | **Keep all additions as-is** (max keyword coverage chosen over lower density) |

**Devin and Windsurf are deliberately absent from the resume.**
If asked: *"Copilot and Claude Code in production; Devin and Windsurf I know by architecture, not by use — same agentic loop, and I'd be productive fast."*

---

## 3. Errors Found and Fixed

### 3.1 MCP described as a compliance standard — FIXED
**Was:** "...to enforce PII redaction, compliance, and Model Context Protocol (MCP) standards across all GenAI workflows."
**Problem:** MCP is a tool-integration protocol, not a governance/compliance standard. For a role centred on MCP, this signals acronym-level familiarity only.
**Now:** "...to enforce PII redaction and regulatory compliance across all GenAI workflows." MCP moved to bullets where it is built, not enforced.

### 3.2 Chronology break — ADP/Assurant (Jun 2019 – Aug 2021) — FIXED
**Was:** "Designed multi-cloud AI architecture integrating Amazon Bedrock (Claude 3, Titan) alongside Azure AI Foundry..."
**Problem:** Amazon Bedrock GA'd late 2023; Claude 3 shipped March 2024; Azure AI Foundry was named 2024/25 — all **2–4 years after this role ended**.
**Now:** "Designed multi-cloud automation and machine-learning architecture across Azure and AWS for 100K+ annual tax filings, applying document classification and rules-based extraction pipelines; reducing manual classification effort by 70%."
Second bullet also de-anachronised (removed "evaluating Azure AI Foundry and Amazon Bedrock").

### 3.3 Chronology break — GraphRAG at KPMG (Sep 2021 – Jun 2024) — FIXED
**Was:** "Designed GraphRAG + Neo4j 5.x solutions with Azure AI Search vector embeddings..."
**Problem:** Microsoft's GraphRAG repo went public **July 2024 — one month after this engagement ended.** (Concept described Feb 13 2024; paper arXiv:2404.16130 Apr 2024; pre-release code Jul 2024; v1.0 Dec 2024.)
**Now:** "Designed a **GraphRAG-style** knowledge-graph + vector hybrid retrieval solution on Neo4j 5.x with Azure AI Search embeddings and semantic ranking; enabling intelligent document search for 200+ concurrent users."
Keeps the ATS keyword, chronologically honest. The true story is the stronger one: built graph+vector retrieval before the pattern had a brand name.

---

## 4. Version Corrections (verified live 2026-07-31)

| Item | Was | Now | Evidence |
|---|---|---|---|
| Ollama | 0.6 | **0.32** | v0.32.1 released Jul 16 2026 |
| crewAI | 1.15 | **1.14** | 1.14.6 is latest stable; 1.15.x is a dev build |
| Bedrock models | "Claude 3, Titan" | **Claude Sonnet / Opus** | Claude 3 retired Jan 2026 |
| RAGAS | 0.4 | version pin dropped | Couldn't confirm current major — unpinned is safer |
| Llama | LLaMA 3 | Llama 3.x | |
| LlamaIndex | 0.14 | unchanged ✅ | 0.14.22 (May 2026) — still accurate |
| .NET | 10 | unchanged ✅ | .NET 10 current |
| MCP spec | — | **2026-07-28** added | Latest spec, released 3 days before this session |

---

## 5. Full Chronology Audit — Every Role vs. Every Dated Technology

### ✅ JM Family — Jun 2024 → Present — CLEAN
Runs to today, so nothing can be anachronistic.
MCP (Nov 2024), Claude Code (preview Feb 2025 / GA May 2025), Copilot agent mode + coding agent (2025), Azure AI Foundry (Ignite Nov 2024), Ollama 0.32 (Jul 2026), crewAI 1.14 (May 2026), Llama 3.x (Apr 2024+), GPT-4o (May 2024), LangGraph (Jan 2024), KEDA — all inside window.

### ⚠️ KPMG — Sep 2021 → Jun 2024 — 1 DEFECT (fixed)

| Claim | Existed from | Verdict |
|---|---|---|
| GraphRAG | Code public **Jul 2024** | ❌ Fixed → "GraphRAG-style" |
| Azure AI Search | Renamed from Cognitive Search **Oct/Nov 2023** | ✅ Fine |
| Azure AI Document Intelligence | Renamed from Form Recognizer **Jul 2023** | ✅ Fine |
| Azure OpenAI GPT-4 | Mar 2023 | ✅ Fine |
| QLoRA / PEFT | PEFT Feb 2023, QLoRA May 2023 | ✅ Fine |
| Neo4j 5.x | Oct 2022 | ✅ Fine |

> Azure AI Search was suspected as an anachronism but is **not** — the rename (Nov 2023) falls inside the window. Verified rather than "fixed".

### ✅ ADP & Assurant — Jun 2019 → Aug 2021 — fixed (see 3.2)
Soft note: **Bicep** was v0.3 (Mar 2021) / v0.4 (Jun 2021) — pre-1.0 preview during this window. Plausible for an early adopter. If pressed: *"Bicep was preview then; the bulk was Terraform, with Bicep coming in for ARM-native pieces."* Not changed.

### ✅ RSI (Feb 2018 – Jun 2019), Wisconsin DOR (Oct 2015 – Feb 2018), Merrill (Jun–Oct 2015) — CLEAN
Cosmos DB (GA May 2017), ASP.NET Core, Angular, GENTax/VB.NET — all consistent.

**Score: 2 chronology defects across all roles. Both fixed.**

> **Pattern worth remembering:** the trap isn't old tech, it's **current product names on old work**. Vendors rename constantly — Form Recognizer→Document Intelligence, Cognitive Search→AI Search, Azure AD→Entra ID, AI Studio→AI Foundry. Two of those renames happened to land *inside* the KPMG window, so they're fine — but that's luck, not design. Always check the rename date, not just the product.
>
> Technical Competencies is **not** date-bound, so `GraphRAG with Neo4j 5`, `.NET 10`, `Entra ID` are all fine there.

---

## 6. What Was Added to the Resume

1. **Headline** — reframed to forward-deployed / client-embedded AI delivery, surfacing Claude Code · GitHub Copilot · MCP · Python.
2. **Contact line** — location added (currently placeholder "Florida, USA"; JD requires local).
3. **Professional Profile** — rewritten around embedding with client teams, AI coding assistants, production MCP, secure tunnelling, Python automation, LoRA/QLoRA; all original metrics retained.
4. **New Core Expertise bullet** — "AI Developer Tooling & Forward-Deployed Integration".
5. **New Competencies line** — "AI Developer Tooling, MCP & Connectivity": Claude Code, Copilot (agent mode + coding agent), MCP servers/clients (stdio + Streamable HTTP, spec 2026-07-28), ngrok/cloudflared/SSH forwarding, Azure Private Link, REST, webhooks, Git/GitHub, Docker, CI/CD.
6. **4 new JM Family bullets** — production MCP servers; deploying/troubleshooting Claude Code + Copilot; secure tunnelling into locked-down infra; Python glue code + client-facing discovery/POC iteration.
7. **Full-Stack line** — Python depth made explicit (FastAPI, asyncio, httpx, CLI scripting & automation).

**Density note (accepted deliberately):** MCP now appears 7×, Claude Code/Copilot 5×, tunnelling 4×. JM Family went from 8 → 12 bullets. Trimming was offered and declined in favour of maximum ATS/recruiter-screen coverage. The tradeoff moves from the page to the room — see §8.

---

## 7. Open Items — Bala to Complete

- [ ] **Contact line says "Florida, USA"** — replace with real city. JD demands local to Juno Beach (Palm Beach County). Note: Miami ≈ 1 hr south; **Tampa ≈ 3.5 hrs across the state**. If Tampa-based, write it explicitly, e.g. `Tampa, FL — relocating to Palm Beach County`. Ambiguity gets screened out.
- [ ] **"GPT-5 family, o-series reasoning models"** added to Competencies — these are current in Azure AI Foundry, but **cut it if not actually used**.
- [ ] Confirm the 4 new JM Family bullets accurately reflect work done **at JM Family specifically** (placed there as the current role since Jun 2024).

---

## 8. Interview Prep — Most Likely Probes

**1. "Walk me through an MCP server you built."**
It's on the page 7×, so it will be tested. Have one server end-to-end: what it exposed, transport (stdio vs Streamable HTTP), auth handling, and **one thing that broke**. Specifics kill "keyword" suspicion instantly.

**2. "Why a tunnel — couldn't you use Private Link?"**
Private Link for persistent Azure-internal paths; ngrok/cloudflared for exposing a local endpoint to an external agent or webhook during dev/demo without a firewall change.

**3. "GraphRAG at KPMG — the tool wasn't out until July 2024."**
Line now reads *GraphRAG-style*. True answer is the better one: built graph+vector retrieval on Neo4j before the pattern had a brand name.

**4. Devin / Windsurf** — see §2 for the scripted answer.

**5. MCP spec currency** — spec revved to **2026-07-28** three days before this session: stateless core, protocol-level sessions and `Mcp-Session-Id` removed, Tasks moved to an extension, MCP Apps for server-rendered UI, OAuth/OIDC-aligned authorization. Skim the changelog before going. Naming it unprompted is a strong signal — **but only if you can say what changed.**

---

## 9. Sources

- [MCP Specification 2026-07-28](https://modelcontextprotocol.io/specification/2026-07-28)
- [The 2026-07-28 Specification — MCP Blog](https://blog.modelcontextprotocol.io/posts/2026-07-28/)
- [crewai — PyPI](https://pypi.org/project/crewai/)
- [llama-index-core — PyPI](https://pypi.org/project/llama-index-core/)
- [Ollama releases](https://releases.sh/ollama/releases)
- [GraphRAG: New tool for complex data discovery now on GitHub — Microsoft Research](https://www.microsoft.com/en-us/research/blog/graphrag-new-tool-for-complex-data-discovery-now-on-github/)
- [Moving to GraphRAG 1.0 — Microsoft Research](https://www.microsoft.com/en-us/research/blog/moving-to-graphrag-1-0-streamlining-ergonomics-for-developers-and-users/)
- [Azure Form Recognizer is now Azure AI Document Intelligence — Microsoft](https://techcommunity.microsoft.com/blog/azure-ai-foundry-blog/azure-form-recognizer-is-now-azure-ai-document-intelligence-with-new-and-updated/3875765)
- [What's New in Azure AI Search — Microsoft Learn](https://learn.microsoft.com/en-us/azure/search/whats-new)
- [Azure/bicep releases](https://github.com/Azure/bicep/releases)
- [GitHub Copilot 2026 guide — agent mode & coding agent](https://www.nxcode.io/resources/news/github-copilot-complete-guide-2026-features-pricing-agents)
- [Cloudflare Tunnel in 2026](https://recca0120.github.io/en/2026/04/14/cloudflare-tunnel-2026/)
