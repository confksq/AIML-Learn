# Module 7 — Behavioral & Leadership

**Source plan:** `AIML-Learn/04_Career/Roadmap_Coverage_Check_2026-08-03.md` (gap #9), `Consolidation_and_Update_Plan_2026-08-03.md` Phase 1
**Format:** STAR (Situation / Task / Action / Result) + follow-up probe
**Question count:** 12 stories mapped to ~30 question phrasings
**Target roles:** Lead AI Engineer, AI Architect, Forward Deployed Engineer

---

## ⚠️ How to use this file

The **structure, framing, and technical anchors** below are drawn from documented work in this repo (`L18_AISolutionArchitecture.md`, `04_Career/currentresconent.txt`, `08_Jobs/FDE/FDE-Prep_Tracker.md`).

Anything marked **`[FILL: …]`** is a detail only you know — names, team sizes, dates, exact outcomes. **Fill these in before using this file.** Do not walk into an interview with a placeholder unfilled; a story you can't specify is a story that reads as invented.

**Honesty guardrails — enforced throughout:**
- **JM Family work = production.** Real system, real users, real cost decisions. Speak in past tense, first person, freely.
- **VitalCare = assessment/design exercise** (`_Archive/PDFRenders/VitalCare_AI_Assessment_Response.pdf`). Frame as *"a prior-auth platform I designed for an assessment"* — **never** as production experience. If asked to elaborate, saying "that was a design exercise, not something I shipped" is a strength, not a weakness.
- **Ollama/crewAI/GraphRAG/LoRA portfolio projects = self-directed learning.** Frame as "hands-on POC work," which is exactly what `currentresconent.txt` already claims. Consistent.

---

## The STAR framework

| Letter | What it is | Time to spend | Common mistake |
|---|---|---|---|
| **S — Situation** | Context the listener needs, and nothing more | ~15% | Over-explaining background; burning 2 minutes before anything happens |
| **T — Task** | What *you specifically* were accountable for | ~15% | Saying "we" — the interviewer is assessing *you* |
| **A — Action** | What you did, the options you weighed, why you chose | **~50%** | Describing the outcome instead of the decision-making |
| **R — Result** | Quantified where possible, plus what you learned | ~20% | No number, or a number you can't defend |

**Target length: 2–3 minutes.** For a Lead role the **Action** section is where seniority shows — an interviewer wants to hear you *choosing between options under constraint*, not narrating a happy path.

**Two rules that matter more than the acronym:**
1. **Own the "I."** Use "we" for context, "I" for decisions. A story where every action is "we" reads as a story you observed.
2. **Volunteer the trade-off.** Seniors say what the decision *cost*. "I chose X, which meant giving up Y, and here's why that was acceptable" is the single strongest behavioral signal.

---

## Story-to-question mapping

One story answers many phrasings. Know these 12 cold rather than memorizing 30 answers.

| # | Story | Also answers |
|---|---|---|
| 1 | Mentoring a junior through an AI project | "Develop someone", "raise the bar", "someone struggling" |
| 2 | RAG vs fine-tuning disagreement | "Disagreed with a stakeholder", "pushed back", "unpopular recommendation" |
| 3 | The 429 / TPM quota incident | "Production incident", "under pressure", "something broke" |
| 4 | Driving AI adoption against resistance | "Influence without authority", "resistance to change", "sold an idea" |
| 5 | Model tiering cost decision | "Business impact", "saved money", "measurable result" |
| 6 | A failure and what changed | "Biggest failure", "what would you do differently", "a mistake" |
| 7 | Product vs engineering conflict | "Cross-functional conflict", "competing priorities", "difficult colleague" |
| 8 | Latency vs accuracy under a deadline | "Trade-off under pressure", "imperfect decision", "incomplete information" |
| 9 | Setting team AI best practices | "Set standards", "improved a process", "technical leadership" |
| 10 | Explaining hallucination to an executive | "Explain technical to non-technical", "managed expectations" |
| 11 | Ambiguous requirements | "Unclear direction", "figured it out yourself", "ambiguity" |
| 12 | Self-directed upskilling | "Learned something new", "stayed current", "growth" |

---

## 7a. Leadership & Mentoring (3)

### Q1. Tell me about a time you mentored a junior engineer.

- **S:** `[FILL: junior engineer's level/background]` joined the team as we were extending the Azure RAG pipeline. They were strong in .NET but had no exposure to retrieval systems, and were treating the LLM as a black box — when answers were wrong, their instinct was to rewrite the prompt.
- **T:** I owned the delivery, but the more valuable outcome was them being able to debug retrieval independently rather than escalating every quality issue to me.
- **A:** Rather than fixing their bugs, I taught the **failure-isolation ladder** we use: *is the chunk even in the index? → did retrieval return it? → did it survive re-ranking into top-K? → did the model ignore it?* Most "prompt problems" turn out to be retrieval problems at step 1 or 3. I had them instrument each stage and log what came back. I deliberately let them sit with one bug for `[FILL: ~a day?]` before stepping in, then paired on it. I also had them present the root cause to the team — teaching it back is what made it stick.
- **R:** `[FILL: outcome — e.g. they independently diagnosed the next N retrieval issues; became the go-to for chunking; shipped X]`. What I'd generalize: the leverage was in giving them a **diagnostic sequence**, not an answer. One transferable framework beat ten fixed bugs.

**Follow-up probe:** *"What if they'd pushed back and just wanted the answer?"* — I'd give the answer *and* the reasoning, then have them apply it to the next one. Mentoring under a deadline sometimes means unblocking first and teaching second; refusing to unblock someone on principle is ego, not mentorship.

---

### Q2. How do you set technical standards for a team?

- **S:** As AI work spread beyond the initial pipeline, prompts were living inline in C# files, changing without review, and there was no way to tell which prompt version produced a given bad answer.
- **T:** I needed guardrails that wouldn't slow delivery — a heavyweight process would have been ignored, which is worse than no process.
- **A:** I introduced three things, deliberately kept minimal: **(1) prompt versioning** — prompts moved out of code into versioned artifacts so a regression could be traced to a specific change; **(2) an evaluation gate** — groundedness/relevance/coherence scored via Azure AI Foundry evaluation pipelines before promotion, wired into the Azure DevOps release; **(3) a model-tier default** — GPT-4o-mini unless a documented reason justified GPT-4o. I wrote each as roughly a page, not a policy document. I also seeded it by converting the existing prompts myself instead of asking the team to do the migration.
- **R:** `[FILL: adoption outcome — e.g. all N prompts migrated; regressions caught pre-prod; time-to-diagnose dropped]`. The lesson: standards get adopted when the person proposing them absorbs the migration cost.

**Follow-up probe:** *"How do you handle someone who ignores the standard?"* — First assume the standard is wrong or too costly and ask why they routed around it; that's genuinely the common case. If the standard is right, the fix is making compliance the path of least resistance — a template, a pipeline gate — rather than relying on discipline.

---

### Q3. Tell me about a time you influenced without authority.

- **S:** `[FILL: which team/stakeholder]` was skeptical of putting a generative system in front of `[FILL: dealers / internal users]` — the concern was that a wrong answer with a confident tone was worse than no answer at all. That is a legitimate objection, not resistance to change.
- **T:** I had no authority over that team's roadmap. I needed them to *want* it.
- **A:** I didn't argue that hallucination was overblown — I agreed with the risk and attacked it directly. I built a narrow POC scoped to one high-repetition question set, with grounding enforced against Azure AI Search and citations surfaced so every answer showed its source. Then I ran their own hardest questions through it in front of them, **including the ones it got wrong**. Showing failure modes honestly did more for credibility than a polished demo would have.
- **R:** `[FILL: outcome — did they adopt? what scope? timeline?]`. The transferable point: with a skeptical stakeholder, demonstrating the *guardrail* beats demonstrating the *capability*.

**Follow-up probe:** *"What if the POC had failed their questions badly?"* — Then the answer is genuinely "not yet for this use case," and I'd rather find that in a POC than in production. The credibility earned by reporting that honestly is what buys you the next attempt.

---

## 7b. Conflict & Decision-Making (4)

### Q4. Tell me about a time you disagreed with a stakeholder.

- **S:** `[FILL: who]` wanted to fine-tune a model on enterprise documents, on the reasoning that a model "trained on our data" would be more accurate than retrieval.
- **T:** As the architect I owned the recommendation, and I thought fine-tuning was the wrong tool for this problem.
- **A:** I reframed it from preference to **decision criteria** — content update frequency, data volume, cost, and latency. The deciding factor was update frequency: the documents changed `[FILL: how often]`, and fine-tuning bakes knowledge into weights, so every content change means retraining. RAG updates by re-indexing. I also named what fine-tuning *is* good for — behavior, format, and tone, not fresh facts — so it read as an engineering judgment rather than a rejection. I offered a hybrid path: RAG for knowledge, fine-tuning later if we needed consistent output *style*.
- **R:** `[FILL: decision reached and outcome]`. What made it work was arguing from a criteria table both of us could evaluate, not from authority.

**Follow-up probe:** *"What if they'd overruled you?"* — I'd implement it well and instrument it to make the failure mode visible early — retraining cost per content update, staleness of answers. Being right later is worthless if nobody measured it. But I'd say my concern once, clearly, and in writing.

---

### Q5. Describe a production incident you owned.

- **S:** `[FILL: confirm this was a real incident vs. a risk you designed against — if the latter, reframe as Q8]` Concurrent load on the invoice assistant pushed us into the Azure OpenAI TPM quota ceiling. Users got 429s, and from their side the system simply looked broken — a throttle doesn't announce itself as a throttle.
- **T:** Restore service, then remove the class of failure rather than the instance.
- **A:** Short term, I `[FILL: immediate mitigation]`. The real diagnosis was that we'd sized quota against average load, not concurrent peak. The fix was layered rather than a single lever: requested a quota increase (100K → 500K TPM); added a **Redis cache** for high-repetition queries — *"what is the late penalty?"* alone was asked ~50×/day and had been hitting the model every time; reduced **top-K from 10 to 3** with re-ranking to preserve quality while cutting tokens per call; enabled **streaming** so time-to-first-token stopped reading as a hang; and added an **App Insights alert at 80% TPM** so we'd see the next ceiling before users did.
- **R:** `[FILL: quantified — error rate, latency, headroom]`. The durable lesson: quota is a capacity-planning problem, and the alert threshold mattered as much as the quota bump — the fix that prevented recurrence was the one that made the limit *visible*.

**Follow-up probe:** *"Why not just raise the quota and move on?"* — That treats the symptom and buys a more expensive version of the same failure. Caching removed load permanently, top-K tuning cut cost per call, and the alert converted a user-discovered outage into an engineer-discovered warning. The quota increase alone would have hidden the problem until the next growth step.

---

### Q6. Tell me about a trade-off you made under pressure.

- **S:** `[FILL: deadline/context]` — dealer-facing responses had a hard P95 latency requirement of under 2 seconds, and the higher-quality configuration didn't fit the budget.
- **T:** Decide what to give up, and be able to defend it.
- **A:** I built an explicit **latency budget** and measured each stage: embedding, retrieval, re-ranking, generation. Generation dominated. The options were a smaller model, fewer retrieved chunks, or dropping re-ranking. I chose **GPT-4o-mini with top-K=3 plus re-ranking**, keeping re-ranking specifically because dropping it degraded answer quality more than the smaller model did — re-ranking was cheap in latency and expensive to lose. I routed the genuinely hard cases — contract analysis, where reasoning depth matters and volume is low (~50/day) — to full GPT-4o on a separate path.
- **R:** `[FILL: measured P95 achieved]`. Cost landed around **$21/month for invoice Q&A vs ~$345 if we'd used GPT-4o for everything**. The framing I'd keep: not "which model is best" but "which model per workload," with the trade-off documented so it could be revisited.

**Follow-up probe:** *"What did that cost you?"* — GPT-4o-mini is measurably weaker on multi-step reasoning. I accepted that for structured, repetitive invoice questions where it wasn't needed, and I put a quality evaluation in place to catch it if that assumption stopped holding. The risk was real; it was bounded and monitored, not waved away.

---

### Q7. Tell me about a cross-functional conflict.

- **S:** `[FILL: the specific tension — e.g. product wanted broad open-ended Q&A; engineering/compliance wanted narrow scoped answers]`
- **T:** `[FILL: your accountability]`
- **A:** `[FILL — structure to use:]` I separated the *stated* positions from the *underlying* concerns. Product's real goal was `[FILL]`; the objection was really about `[FILL]`. Once those were named, the disagreement was narrower than it looked. I proposed `[FILL: the scoped compromise]` and made the decision reversible by `[FILL: e.g. phased rollout / feature flag]`.
- **R:** `[FILL]`. The generalizable move: most cross-functional conflict is two teams optimizing different metrics honestly. Surfacing both metrics converts an argument into a trade-off discussion.

**Follow-up probe:** *"What if you couldn't reach agreement?"* — Escalate with a written summary of both options and a recommendation, not with a complaint. Escalation is a legitimate tool when it hands the decision-maker a real choice; it's a failure when it hands them a conflict to referee.

---

## 7c. Failure & Growth (2)

### Q8. Tell me about a significant failure.

> **Pick a real one.** The strongest version is a genuine misjudgment with a concrete cost. Candidates from your documented work:
> - Sizing quota against average rather than peak load (see Q5)
> - `[FILL: a chunking/retrieval strategy that underperformed and had to be reworked]`
> - `[FILL: a scope or timeline commitment that slipped]`

- **S:** `[FILL]`
- **T:** `[FILL]`
- **A:** `[FILL — must include: what I actually got wrong, when I realized it, and what I did about it]`
- **R:** `[FILL: cost of the mistake — be specific]`. What changed afterward: `[FILL: the concrete practice you adopted]`.

**Follow-up probe:** *"What would you do differently?"* — Answer with a *process* change, not a resolution to try harder. "I now size capacity against peak concurrency and alert at 80%" is an answer; "I'd be more careful" is not.

**⚠️ Anti-patterns:** don't pick a humblebrag ("I care too much"), don't blame another team, and don't pick a failure so small it signals you've never owned anything consequential.

---

### Q9. How do you stay current in a field moving this fast?

- **S:** My production depth is Azure-native — Semantic Kernel, Azure OpenAI, Azure AI Search, AI Foundry. The market was moving toward Python/LangChain, agentic protocols, and graph-based retrieval, and depth in one stack can quietly become a ceiling.
- **T:** Close that gap deliberately rather than waiting for a project to force it.
- **A:** I built a structured curriculum and worked through it hands-on rather than by reading — `[FILL: adjust to what you've actually completed]`: local RAG with Ollama, multi-agent orchestration with crewAI, RAGAS evaluation, GraphRAG with Neo4j, LoRA/QLoRA fine-tuning, and LlamaIndex. Each one is a runnable project, not notes. I deliberately chose the ecosystems adjacent to what I already knew, so the transfer was conceptual rather than starting cold — Semantic Kernel plugins map onto LangChain tools; Azure AI Search hybrid retrieval maps onto Qdrant/FAISS.
- **R:** `[FILL: what this enabled — a conversation, a POC, a role]`. The approach I'd defend: depth in one stack plus deliberate breadth into two or three others beats shallow familiarity with everything.

**Follow-up probe:** *"What's still a gap for you?"* — Name a real one. `[FILL — honest options from your own coverage tracking: production-scale Microsoft Fabric / data-platform work, or large-scale distributed training]`. Naming a real gap and your plan for it is far stronger than claiming full coverage; interviewers discount the latter automatically.

---

## 7d. Communication & Business Impact (3)

### Q10. How do you explain a technical limitation to a non-technical executive?

- **S:** `[FILL: who and when]` asked, in effect, whether we could guarantee the assistant would never give a wrong answer.
- **T:** Give an honest answer without either overpromising or making the system sound untrustworthy.
- **A:** I avoided the word "hallucination" — it sounds like a defect to be patched. I framed it as: the model generates *plausible* text, and plausible is not the same as *correct*; it has no internal notion of truth. So we don't rely on the model knowing things — we **ground** it: it answers only from retrieved enterprise content, every answer carries a citation, and we score groundedness automatically. Then I gave the honest bound: this reduces wrong answers substantially and makes them **auditable**, but it does not reduce them to zero, so the design assumes a human check where the cost of being wrong is high.
- **R:** `[FILL: the decision that followed]`. What worked was pairing the limitation with the mitigation *and* the residual risk in the same breath. Executives distrust "it's fine"; they act on "here's the bound and here's what we do about it."

**Follow-up probe:** *"They ask for a percentage guarantee — what do you say?"* — I'd give measured groundedness/relevance scores from our evaluation pipeline and describe what they do and don't mean. I wouldn't invent an accuracy SLA for generative output; committing to a number I can't hold is a worse outcome than the uncomfortable conversation.

---

### Q11. Tell me about work with measurable business impact.

- **S:** The AI cost model was being driven by a default assumption that the strongest model should serve every workload.
- **T:** Bring cost under control without a quality regression — a cheaper system that answers worse is not a saving.
- **A:** I profiled cost by *workload* rather than in aggregate. Invoice Q&A: ~500 queries/day, structured and repetitive. Contract analysis: ~50/day, genuinely needs reasoning depth. I applied **model tiering** (GPT-4o-mini for invoice, GPT-4o for contracts), **Redis caching** on high-repetition queries, **top-K reduction from 10 to 3** with re-ranking to hold quality, and **embedding caching** to avoid re-embedding unchanged chunks on re-index. Critically, I gated each change behind groundedness/relevance evaluation so I could prove quality held.
- **R:** Invoice Q&A dropped from **~$345/month to ~$21/month** — roughly a **94% reduction on that workload** — with total AI spend around **$56/month** across both. `[FILL: confirm these against actuals before quoting; these are the modelled figures in L18.]` Quality held per the evaluation gate. `[FILL: any downstream outcome — budget approved, scope expanded]`
- **Trade-off I'd volunteer:** GPT-4o-mini is weaker on multi-step reasoning; that was acceptable *for that workload specifically*, and the evaluation gate existed to catch it if it stopped being true.

**Follow-up probe:** *"How do you know quality didn't degrade?"* — Automated evaluation on groundedness, relevance, and coherence before promotion, plus production monitoring for cost and quality drift via Azure Monitor. Without the eval gate this is a cost-cutting story with an unmeasured quality claim attached — which is exactly the version an interviewer should be skeptical of.

---

### Q12. Tell me about working with ambiguous requirements.

- **S:** `[FILL: the ambiguous ask — e.g. "we want an AI assistant for X" with no defined scope, success criteria, or data source]`
- **T:** `[FILL]`
- **A:** `[FILL — structure to use:]` I converted the ambiguity into three answerable questions: **what does a good answer look like** (which gave us the evaluation criteria), **what content is it allowed to answer from** (which defined the index), and **what happens when it doesn't know** (which defined the fallback). Rather than waiting for a specification, I `[FILL: built a narrow slice / ran a workshop / drafted a criteria doc]` and used it to force the decisions into the open.
- **R:** `[FILL]`. The approach that generalizes: with an ambiguous ask, build the smallest thing that makes the disagreement visible. People specify far better against something concrete than against a blank page.

**Follow-up probe:** *"What if the stakeholder still can't define success?"* — That's a signal the use case isn't ready, and it's worth saying so early. I'd propose a scoped POC with a defined evaluation set instead of an open-ended build — it either produces clarity or produces a cheap, fast "no."

---

## Preparation checklist

- [ ] Replace **every** `[FILL: …]` — no placeholders survive into an interview
- [ ] Verify the L18 cost figures ($345 → $21, ~$56 total) against what you can actually defend; label as *modelled* if that's what they are
- [ ] Confirm whether Q5's incident is a real incident or a risk you designed against — reframe rather than overstate
- [ ] Never describe **VitalCare** as production; it is an assessment design exercise
- [ ] Rehearse Q1, Q4, Q5, Q11 out loud — these four cover the highest-frequency prompts
- [ ] Time each: 2–3 minutes; **Action ≈ 50%**
- [ ] For each story, prepare the one number you'd defend under scrutiny
- [ ] Audit your pronouns: "we" for context, **"I" for every decision**

---

## Related

- Technical depth behind these stories: `L18_AISolutionArchitecture.md` (§18.2 scalability, §18.4 cost), `L36_LLM_Observability_FinOps.md`
- Leadership-framed *technical* probes already in the bank: `05_Solution_Architecture.md` Q18 (ROC/cost justification), Q-probes at :154, :214, :388; `06_Responsible_AI_LLMOps.md` :94
- Role targeting: `08_Jobs/FDE/FDE-Prep_Tracker.md` (closes item #60 — Mentoring / transformation catalyst)
