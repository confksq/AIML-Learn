# Q&A — L35 AI-Assisted Engineering

*Created 2026-07-26 · FDE-Prep*

---

**Q1. What can Cursor do that inline completion fundamentally cannot, and why?**

Multi-file edits driven by a semantic index of the whole repository. Inline completion sees only the
current file and a small window of context, so it can finish a line but cannot answer "change this
pattern across twelve files" or "where do we handle X?"

---

**Q2. What is `.cursorrules` and why does it beat repeating instructions per prompt?**

A file at the repo root, prepended to every AI request in that project. It encodes conventions once —
target framework, DI style, secret handling, test framework, "ask rather than invent a business
rule" — so every request inherits them instead of you re-typing them and forgetting some.

---

**Q3. Three things you would never accept from AI without reading every line.**

Anything touching authentication or secrets; IAM/RBAC policy; and money or refund calculations.
(Also acceptable: anything that deletes data, and regex applied to production data.)

---

**Q4. Why is "AI made us 60% faster" a weak claim? What would you say instead?**

It has no baseline, no unit and no sample size, so it cannot be interrogated. Better: *"Roughly 3× on
Bicep-to-Terraform module translation, measured across five modules, with `terraform plan` and
Checkov as the verification gate."* That names the task, the multiple, the sample, and how
correctness was preserved.

---

**Q5. What is the verification gate for AI-generated Terraform, and why does it matter more than the
generation?**

`terraform plan` plus a policy scanner. It matters more because it is **deterministic** — it tells
you exactly what will be created, changed or destroyed regardless of how the code was written. The
gate is what makes non-deterministic generation safe to adopt at speed.

---

**Q6. Describe the computer-use loop. How is it the same as the agent loop in `L27`?**

Screenshot → model reasons → emits an action (click coordinates, keystrokes) → executed in a VM →
new screenshot → repeat. It is the identical reason-act-observe loop; only the tool set has changed
from API calls to GUI primitives.

---

**Q7. One case where computer-use is right, one where an API call is clearly better.**

Right: a legacy vendor console or mainframe terminal with no API, where the alternative is a human
clicking. Better as an API call: anything with a documented endpoint — faster, cheaper, auditable,
and it does not break when someone moves a button.

---

**Q8. When is N8N the right tool, and when the wrong one?**

Right for business-visible glue across many SaaS connectors with little logic — ticket in, enrich,
notify, create issue — prototyped in an afternoon. Wrong for production agent orchestration, which
needs version control, testability, complex state and checkpointing; that belongs in LangGraph or
Semantic Kernel.

---

**Q9. Sketch an AI-assisted vulnerability-remediation pipeline and name the human gate.**

Scanner produces findings → triage agent assesses reachability, fix availability and blast radius →
three routes: auto-PR for deterministic version pins (CI proves nothing broke), enriched ticket
assigned to a human for judgment calls, or a suppression with a recorded reason **and an expiry**.
The human gate sits on anything that changes runtime behaviour rather than a version pin — and on
merging, always.

---

**Q10. Why does the JD distinguish "transformation catalyst" from "traditional developer"?**

Because the value is not the individual's throughput — it is raising the team's. That means finding
automation opportunities unprompted, building the verification gates that make generated work
trustworthy, and demonstrating it so others adopt it. One fast engineer does not produce a 60%
account-level reduction; a changed default way of working does.

---

**Q11. What is the difference between the four levels of AI coding assistance, and where does most
of the value sit?**

Completion (finish this line), chat (explain/write this), multi-file edit (change this across the
codebase), agent (do this task, run commands, iterate). Most people stop at completion and conclude
the gain is marginal; the value is in the bottom two.

---

**Q12. Give the four ingredients of a good code-generation prompt.**

Point at an existing file as the pattern; state the hard constraints; define what "done" means
(tests pass, formatter clean); and specify the failure mode — *"if a business rule is unclear, ask,
do not invent one."*

---

## Scoring

| Score | Read |
|---|---|
| 10–12 | Rows 12, 53–56 are green — **provided you actually shipped the change in §7.** |
| 7–9 | Re-read §3 (prompting) and §4 (measurement). |
| < 7 | Re-read `L35` — but more importantly, do §7. This module is mostly doing. |

> ⚠️ Unlike the other modules, scoring well here does **not** make these rows green on its own.
> Row 53 requires Cursor installed and a change pushed. Do §7 first, then take this quiz.
