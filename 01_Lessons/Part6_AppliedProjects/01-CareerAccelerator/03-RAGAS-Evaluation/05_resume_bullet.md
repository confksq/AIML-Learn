# 05 — Resume Bullet

**Primary (concise):**
> Implemented a RAGAS-based RAG evaluation framework measuring faithfulness, answer relevance, context recall, and context precision — enabling production quality gates and data-driven RAG tuning.

**Alternative (impact-oriented):**
> Instrumented RAG pipelines with RAGAS to quantify answer groundedness and retrieval quality, isolating generation vs retrieval failures and wiring evaluation into CI/CD as a quality gate before promotion.

**Skills row additions:**
`RAGAS · RAG evaluation · faithfulness/groundedness · context precision & recall · LLM-as-judge · TruLens / Azure AI Evaluation awareness`

**Talking point for interviews:**
"I already do groundedness checks and A/B testing on RAG at JM Family — RAGAS formalizes that into four measurable metrics I can gate deploys on. When faithfulness drops it's a generation fix; when context recall drops it's a retrieval fix. That per-stage diagnosis is what makes RAG improvement systematic instead of guesswork."
