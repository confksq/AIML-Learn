# AI-103 Coverage Estimate — vs. AIML-Learn Library

**Date:** 2026-07-28
**Basis:** Skills-measured outline as of April 16, 2026 (5 domains), spot-checked against
`00_START_HERE.md` / `00_INDEX.md` and targeted greps of `01_Lessons/`. This is a **domain-level
estimate, not a bullet-by-bullet audit** — see the "How to get a real number" note at the bottom.

---

## Table 1 — Coverage by domain

| Domain (weight) | Existing coverage | Gap |
|---|---|---|
| Plan & manage Foundry solutions (25–30%) | Strong — `L17` Foundry, `L18` Architecture, `L19` MLOps/LLMOps (quotas, managed identity, drift monitoring all already taught) | Needs terminology refresh for Foundry Agent Service naming + CI/CD-for-Foundry-projects specifics; no net-new lesson |
| Generative AI & agentic solutions (30–35%) | Very strong — Part 3 (`L12`–`L16`) + Part 5 (`L22`–`L31`), same track FDE-Prep already scored ~85% | Light — mostly vocabulary alignment (Foundry Tools, tool schemas) |
| Computer vision (10–15%) | Partial — `L04` covers analysis/OCR/objects, but **image/video generation, inpainting, mask-based edits, video-editing workflows = zero hits** in the whole library | **Real gap** — genuinely new content, not a refresh |
| Text analysis (10–15%) | Solid — Translator (17 hits), sentiment/entity/NLP via `L03`/`L07` | Small — "speech as agent modality" tie-in needs expansion |
| Information extraction (10–15%) | Decent — Content Understanding already has 9 hits (`L08`, `L30`), RAG ingestion via `L09`/`L13` | Needs pro-mode pipeline / video-analysis specifics added |

**Confirmed zero-hit terms** (grepped `-wi` across `01_Lessons/` + `00_INDEX.md`): Sora, video
generation, inpainting, alt-text, watermark.

---

## Table 2 — Weighted overall estimate

| Domain | Exam weight | Coverage estimate | Weighted |
|---|---|---|---|
| Plan & manage Foundry | ~27.5% | ~85–90% | ~24 |
| Generative AI & agentic | ~32.5% | ~85–90% | ~28 |
| Computer vision | ~12.5% | ~40–50% (generation sub-skills are 0%) | ~6 |
| Text analysis | ~12.5% | ~80–85% | ~10 |
| Information extraction | ~12.5% | ~65–70% | ~8 |
| **Total** | | | **~76–80%** |

**Why not ~90%:** computer vision is weighted 10–15% of the exam, and roughly half of that domain's
bullets (image/video generation, inpainting, mask-based edits, video editing) have zero coverage
anywhere in the library. That single hole drags the overall number down more than a domain count
("4 of 5 mostly fine") would suggest.

---

## How to get a real number instead of an estimate

Score all AI-103 skill bullets individually against `file:line` evidence, the way
`08_Jobs/FDE/FDE-Prep_Tracker.md` did for the 60 FDE requirement rows. Not done yet — this file is
the domain-level estimate that would precede that exercise.

Related: `00_START_HERE.md`, `08_Jobs/FDE/FDE-Prep_Tracker.md` (tracker pattern to replicate).
