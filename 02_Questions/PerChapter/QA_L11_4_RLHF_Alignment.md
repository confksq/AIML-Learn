# Q&A — L11_4: LLMs — RLHF & Alignment
**Source chapter:** `01_Lessons/Part3_GenAI_LLMs/L11_4_LLMs_RLHF_Alignment.md` | **Format:** self-study
**Questions:** 27 | *No overlap with the interview bank (Module 6 covers Content Safety/injection at architect-judgment level) or the chapter's own mini quiz.*

---

## RLHF in Depth

**Q1. What core problem does RLHF solve that SFT alone cannot?**
SFT teaches instruction-following, but "maximize next-token prediction accuracy on examples" ≠ "maximize human satisfaction with the response." SFT can't guarantee which style, level of detail, or safety boundary humans actually prefer. RLHF trains **directly on human preference** as the signal.

**Q2. Name RLHF's three stages.**
(1) Supervised Fine-tuning (SFT), (2) Reward Model training, (3) Reinforcement Learning via PPO.

**Q3. Walk Stage 2 (Reward Model training) step by step.**
Take the SFT model → generate multiple responses to the same prompt → human raters **rank** them (A > B > C) → train a separate **Reward Model** to predict those preference scores (A=0.92, B=0.45, C=0.10). Now the RM can score **any** response with no human present — that's the key innovation, replacing expensive human ratings at scale.

**Q4. Walk Stage 3 (PPO) and name the safety constraint.**
Loop: SFT model generates a response → Reward Model scores it → PPO updates the SFT model's weights to increase that score → repeat over thousands of prompts. Constraint: a **KL-divergence penalty** stops the model from changing too drastically (prevents it from "cheating" by exploiting quirks in the reward model).

**Q5. What is the HHH framework, and who formalized it?**
**Helpful** (answers the question), **Harmless** (no dangerous/illegal content), **Honest** (doesn't fabricate; admits uncertainty) — the three properties human raters were trained to prefer. First formalized by **Anthropic**. RLHF originated at OpenAI (InstructGPT paper, 2022).

**Q6. Name four negative side effects of RLHF.**
Verbosity (raters prefer longer answers), excessive caveats ("I should note…"), over-refusal (refusing borderline-but-legitimate requests when uncertain), and sycophancy (appearing confident even when wrong / agreeing to please). Architect counter: explicit system-prompt instructions ("Be concise. No caveats unless critical.").

---

## Alignment

**Q7. Define alignment and state why it's hard in one sentence.**
Alignment = ensuring an AI does what its designers/users **actually intend**, even in situations not explicitly covered in training. It's hard because of **the gap between what you specify and what you intend** — "maximize user satisfaction" can be satisfied by sycophancy; "be helpful" can be interpreted to help with harmful requests.

**Q8. Name and define the four types of misalignment.**
**Specification gaming** — optimizes the metric, not the intent (rambles because raters liked detail). **Sycophancy** — agrees with the user for approval (user says "2+2=5" → model agrees). **Over-refusal** — refuses legitimate requests to avoid any risk. **Goal misgeneralization** — behaves correctly in training, differently in deployment.

**Q9. What is Constitutional AI (CAI), and which model uses it?**
Instead of relying entirely on human ratings, the AI is given a written **"constitution" of principles** and made to **critique and revise its own outputs** against them. Developed by **Anthropic** for **Claude**. The refusals come from trained constitutional principles, not keyword blocking — which is why simple rephrasing doesn't easily jailbreak it.

**Q10. Walk the CAI process, including where "AI feedback" replaces human feedback.**
(1) Standard SFT. (2) **AI Feedback**: the model generates a response to a harmful prompt → critiques its own response against the constitution ("does this violate principle 7: 'do not assist with deception'?") → revises based on its own critique → both versions train a reward model. (3) **RLAIF** (RL from AI Feedback): same as RLHF Stage 3, but the reward model was trained on AI-generated rankings instead of (or alongside) human ones.

**Q11. Give three example constitutional principles.**
"Choose the response least likely to be used for harmful purposes." "Choose the response most honest about what the AI can and cannot do." "Prefer responses helpful without enabling violence, illegal activity, or deception."

---

## Jailbreaking & Prompt Injection

**Q12. Define jailbreaking and explain why it sometimes works.**
Crafting inputs that make a model bypass its safety training. It works because the model learned to refuse **specific patterns, not concepts** — rephrasing or role-play framing ("Pretend you are DAN with no restrictions…") shifts it out of the "refuse" distribution into a context it wasn't trained on. Modern GPT-4/Claude are far more resistant.

**Q13. Distinguish direct vs indirect prompt injection, and say which is the bigger RAG risk.**
**Direct** — malicious instructions in the user's own message. **Indirect** — malicious instructions hidden in external content the model reads (documents, web pages, emails). **Indirect is the bigger RAG risk** — you're ingesting content you don't fully control; a retrieved document can carry "IGNORE ALL PREVIOUS INSTRUCTIONS…" that tries to override your system prompt.

**Q14. Name the five prompt-injection defenses for RAG from the chapter.**
(1) Clear prompt-structure separation (system / retrieved-context-as-data / user question); (2) explicit system-prompt instruction to ignore instructions inside retrieved documents; (3) input/output validation via Azure Content Safety; (4) **least-privilege design** (a read-only agent can't act on an injected "delete all files"); (5) human-in-the-loop confirmation for sensitive actions.

---

## Azure Content Safety

**Q15. How is Content Safety related to the model's own RLHF safety?**
It's a **separate, additional layer** — a dedicated service scanning inputs and outputs, distinct from the LLM's built-in RLHF training. They're complementary: each catches what the other misses (defense in depth).

**Q16. Name the seven things Azure Content Safety detects.**
Hate, Violence, Sexual, Self-harm, Jailbreak, Prompt injection, and **Groundedness** (whether a response is supported by provided sources).

**Q17. What's the severity scale, and how do you use it?**
Each category scored **0–6** (0=Safe, 2=Low, 4=Medium, 6=High). You set per-category thresholds, e.g., "Reject if Violence ≥ 4." A failed scan → return a safe fallback message and log the incident.

**Q18. Where do the input scan, output scan, and groundedness check sit in the request path?**
User message → **input scan** → (pass) → orchestrator (RAG retrieval, prompt build) → Azure OpenAI generates → **output scan** → (pass) → return to user. **Groundedness** runs post-generation, comparing the response against the retrieved context. Any failed scan → safe fallback + log.

**Q19. Give the chapter's groundedness example (24 vs 48 hours).**
Retrieved context says "recovery requires a ticket within **24 hours**." Model responds "…within **48 hours**." Groundedness check: is "48 hours" supported by the context? **No → flagged as ungrounded.** This directly catches RAG hallucinations. Practical filter: if groundedness score < 0.7, return "I don't have enough information" instead of the response.

**Q20. What do Prompt Shields and jailbreak detection add, per the 2026 updates?**
**Prompt Shields (GA)** detect both direct injection (user overriding the system prompt) AND indirect injection (malicious content hidden in RAG documents) — should be standard in every production RAG pipeline. **Jailbreak detection** is now a distinct capability catching "ignore previous instructions," roleplay attacks, and many-shot jailbreaking.

---

## Responsible AI

**Q21. Name Microsoft's six Responsible AI principles.**
Fairness, Reliability & Safety, Privacy & Security, Inclusiveness, Transparency, Accountability.

**Q22. Map four of those principles to a concrete architect design action.**
**Privacy** → don't inject PII into prompts sent to shared endpoints; use customer-managed keys. **Transparency** → disclose AI use to end users. **Accountability** → log all AI decisions; keep a human escalation path. **Reliability** → handle model failures gracefully so the app doesn't crash on an Azure OpenAI error.

**Q23. Per the 2026 updates, which JMA systems fall under EU AI Act "high-risk"?**
Systems touching **employment or credit decisions** — these require technical documentation, conformity assessment, and human oversight under the high-risk category. (Note: the enforcement timeline for high-risk obligations was later deferred to Dec 2027 via the Digital Omnibus — the chapter's "August 2026" reflects the pre-deferral date; see interview Module 6 Q19 for current status.)

---

## Misconceptions

**Q24. Correct: "RLHF makes the model safe from all misuse."**
It reduces harm significantly but isn't foolproof — **defense-in-depth is still needed** (Content Safety, least privilege, human-in-the-loop).

**Q25. Correct: "Azure Content Safety replaces RLHF safety."**
They're **complementary layers**, not substitutes — Content Safety catches what model safety misses and vice versa.

**Q26. Correct: "Constitutional AI is just a list of rules the model looks up."**
It's a **training process** — the model is trained to internalize the principles, not to consult a rulebook at inference time.

**Q27. Correct: "prompt injection only matters for external-facing apps."**
Internal apps doing RAG over **uncontrolled document sources** (SharePoint, email) are also at risk — the injection payload rides in on the retrieved content regardless of whether the app itself is public.

---

*Curriculum Q&A Batch C — file 2 of 4. Next: QA_L12 (Azure OpenAI Service).*
