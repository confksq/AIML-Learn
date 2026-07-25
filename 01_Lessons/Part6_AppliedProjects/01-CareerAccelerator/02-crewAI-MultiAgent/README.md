# 02 — crewAI Multi-Agent

**Part of:** Career Accelerator portfolio · **PRD Feature L2** · **Phase 1 (Week 1)**
**Skill:** Python-native multi-agent orchestration with crewAI — the open-source counterpart to Semantic Kernel agents.

---

## Why this module matters for the job search

~20% of Senior AI / GenAI Engineer JDs mention **crewAI** — and Azure AI Foundry job descriptions increasingly list it **alongside** Semantic Kernel. You already build multi-agent systems in C# with SK (orchestrator + specialists, ReAct loop, plugins). This module proves you can do the same in the **Python-native** ecosystem recruiters are screening for.

---

## What you'll have after this module
- A working **3-agent research pipeline** (Researcher → Writer → Reviewer) in `04_hands_on.py`
- Runs against **OpenAI or a local Ollama backend** (toggle in config — no paid API required)
- 15 senior-level interview Q&A on agent roles, task dependencies, process types, and crewAI vs SK vs LangGraph

---

## Prerequisites
```bash
pip install -r requirements.txt
```
Backend options (pick one):
- **Ollama (free, local):** `ollama serve && ollama pull llama3` — set `BACKEND = "ollama"` in the script
- **OpenAI/Azure OpenAI:** set `BACKEND = "openai"` and export `OPENAI_API_KEY` (or Azure vars)

---

## Quick start
```bash
pip install -r requirements.txt
python 04_hands_on.py --topic "Azure AI Foundry"
```
The crew runs three agents in sequence and writes a finalized report to stdout (and `report.md`): the Researcher gathers structured findings, the Writer turns them into a report, the Reviewer validates and finalizes.

---

## Files
| File | What it is |
|---|---|
| `01_concepts.md` | crewAI concepts, bridged from your Semantic Kernel knowledge |
| `02_architecture.md` | 3-agent pipeline diagram + component breakdown |
| `03_interview_qa.md` | 15 senior-level interview Q&A |
| `04_hands_on.py` | 3-agent research pipeline, OpenAI or Ollama backend |
| `05_resume_bullet.md` | Ready-to-paste resume bullet |
| `requirements.txt` | Python dependencies |

---
*Bridge: Semantic Kernel agent → crewAI Agent · SK orchestration pattern → crewAI Process · `[KernelFunction]` → crewAI Tool · ChatHistory → crewAI memory*
