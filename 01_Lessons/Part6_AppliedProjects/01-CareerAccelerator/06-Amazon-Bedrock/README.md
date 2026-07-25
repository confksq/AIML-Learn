# 06 — Amazon Bedrock

**Part of:** Career Accelerator portfolio · **PRD Feature L6** · **Phase 2 (Week 3)**
**Skill:** Amazon Bedrock — AWS's managed GenAI platform — and multi-cloud AI fluency (Azure ↔ AWS).

---

## Why this module matters for the job search

~20% of Senior AI / AI Agents JDs list **Amazon Bedrock**, and increasingly ask for **multi-cloud** engineers who can work "Bedrock + Azure AI Foundry." You already run the Azure equivalent of everything Bedrock does. This module maps your Azure AI Foundry expertise onto AWS 1:1, so you can credibly say "I can deliver the same GenAI architecture on either cloud" — a differentiator most Azure-only candidates can't claim.

---

## What you'll have after this module
- A working `boto3` script (`04_hands_on.py`) that invokes a foundation model on Bedrock and shows the RAG-via-Knowledge-Bases shape
- A **15-dimension Azure ↔ Bedrock comparison** (`azure_vs_bedrock_comparison.md`)
- 15 senior-level interview Q&A on multi-cloud AI trade-offs

---

## Prerequisites
```bash
pip install -r requirements.txt
```
AWS access (free-tier compatible for small tests):
- An AWS account with **Bedrock model access enabled** (request access to Claude/Titan in the Bedrock console — required once)
- Credentials configured: `aws configure` (or env vars `AWS_ACCESS_KEY_ID` / `AWS_SECRET_ACCESS_KEY` / `AWS_REGION`)
> No AWS account? The script is fully commented so you can read the exact API shape; the concepts and comparison doc stand alone.

---

## Quick start
```bash
pip install -r requirements.txt
aws configure                       # set keys + region (e.g. us-east-1)
python 04_hands_on.py               # invoke a Bedrock model via boto3
```

---

## Files
| File | What it is |
|---|---|
| `01_concepts.md` | Bedrock concepts, mapped 1:1 from Azure AI Foundry |
| `02_architecture.md` | Bedrock component architecture + RAG flow |
| `03_interview_qa.md` | 15 senior-level interview Q&A (multi-cloud focus) |
| `04_hands_on.py` | boto3: invoke a model + Knowledge Bases RAG shape |
| `azure_vs_bedrock_comparison.md` | 15-dimension side-by-side comparison |
| `05_resume_bullet.md` | Ready-to-paste resume bullet |
| `requirements.txt` | Python dependencies |

---
*Bridge: Azure OpenAI Service → Bedrock model API · Azure AI Search → Bedrock Knowledge Bases · Semantic Kernel/agents → Bedrock Agents · Azure AI Foundry → Bedrock (platform) · GPT-4o → Claude 3 / Titan / Llama*
