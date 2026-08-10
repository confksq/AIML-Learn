# Session — 2026-08-08 → 2026-08-10
## Q&A file audit · Interview Bible conversion · Detailed sections A & B

**Root:** `C:\pers\AIML-Learn\` · **Branch:** `main` · **Remote:** `github.com/confksq/AIML-Learn` (**public**)
**Commits produced:** `fd18dec`, `71385e9` — plus `2a86360` from a parallel window that carried this
session's Section B file.
**Ran alongside a second Claude window** working the same repo. See §7 — that overlap shaped how
commits were handled and is the main operational lesson here.

---

## 1. What was asked, in order

1. *"Check `C:\pers\AIML-Learn` — where do interview questions and answers reside, list all those files."*
2. *"List all of them in a table ordered."*
3. *"Create a md file out of it and place it under aiml folder."*
4. *"Check in this file too."*
5. *"Create a detailed why/when/how/what … for A. RAG Architecture & Design (15)"* (list pasted inline)
6. *"Convert `BALA K - ULTIMATE FDEAI LEAD INTERV` to a readable, formatted .md, place under question answer, give it a reasonable name."*
7. *"Check in to local and push to GitHub."*
8. *"Do the same detailed why/when/how/what for section B."*
9. *"Commit and push."*

---

## 2. The Q&A audit — what actually exists

Swept the whole repo, not just `02_Questions/`. **55 files carrying interview Q&A**, in five tiers:

| Tier | Files | Questions |
|---|---:|---:|
| `02_Questions/InterviewBank/` | 7 | 138 |
| `02_Questions/PerChapter/` | 25 | 655 |
| `01_Lessons/Part6.../03_interview_qa.md` | 9 | 141 |
| `08_Jobs/AscndIntr/PrepPlan/` | 2 | 11 |
| `07_ChatHistory/` (raw dumps) | 4 | ~32 |
| `_Archive/StaleTrackers/` | 1 | 100 |

**Two counting traps hit during the sweep, both worth remembering:**

- **`PerChapter/` files use `**Qn.**`, not `### Qn`.** The first grep returned *0 questions* for all
  33 PerChapter files. They actually hold 655. Any coverage claim built on a single Q-pattern grep
  is wrong — this is the same failure mode already recorded in `project_ailearn_progress.md` about
  grepping only `L##` files.
- **The largest single question count in the repo is the *archived* file.**
  `_Archive/StaleTrackers/Interview_Prep_AI_Engineer_Complete.md` holds 100 questions — more than
  any live file at the time. It is stale by repo structure but it is **not** dead content: it turned
  out to be the source of the section-A and section-B lists (see §5).

**Output:** `00_QA_FILE_INDEX.md` at repo root (alongside `00_INDEX.md` / `00_CONTENTS.md` /
`00_MAP.md`, since the table spans four top-level folders). Summary rollup + 50-row ordered table +
a known-gaps section.

---

## 3. Interview Bible conversion — `.txt` → structured `.md`

**Input:** `BALA K - ULTIMATE FDEAI LEAD INTERV.txt` — 83 KB, 1,799 lines, CRLF, a chat export with
**all markdown stripped**. Headings were bare lines, lists were blank-line-separated fragments, code
blocks were bare language-tag lines followed by unfenced code, tables were tab-separated rows.

**Output:** `02_Questions/Interview_Bible_77Q_FDE_AI_Lead.md` — 1,469 lines, 77 Q, 4 sections, 9 parts.

### Why a converter script rather than hand-editing
1,799 lines of transformation is not reliably hand-editable, and hand-editing gives no way to prove
nothing was lost. A script is deterministic, re-runnable after a rule change, and verifiable.
Script kept in scratch (`convert.py`), not committed — the `.md` is the artifact.

### The structural mapping (the part that had to be exact)
Code-block **ends** could not be found heuristically. The obvious rule — "a code block ends at the
first line that ends with `:`" — breaks immediately:

```
}                              ← end of a json block
Step 4: Query-Time Boosting    ← next line: prose, does NOT end with a colon
```

So all 13 code blocks and 5 tables were **located by line number after visual inspection** and
hard-coded as ranges. Deterministic beats clever when the input is a fixed one-off file.

| | Count | Languages / note |
|---|---:|---|
| Code blocks | 13 | `python` ×5, `sql` ×2, `json` ×3, `yaml`, `text`, `cypher` |
| Tab-separated tables | 5 | model routing · Bedrock pricing · GPT tiering · RAG vs fine-tune · master index |

`cypher` was found late — it wasn't in the initial language-tag grep, so the Neo4j query in Q77 had
been silently flattened into a paragraph. Caught by re-reading the rendered tail.

### Heuristics used for the prose (and the thresholds that mattered)
- **Bullet runs** — ≥2 consecutive single-line, blank-separated paragraphs under 800 chars become a
  list. The 800 threshold was raised from 400 because at 400 the five "Ingestion / Indexing /
  Retrieval / Generation / Evaluation Layer" siblings in Q1 rendered *inconsistently* — the long one
  a paragraph, the shorter ones bullets.
- **Sub-heads** — a line ≤120 chars ending in `:`, or matching `Failure N:` / `Step N:` / `Phase N:`,
  becomes a bold heading and nests the items after it.
- **De-nesting** — `Optimization:` / `My Decision:` / `Trade-off:` style conclusion lines reset to
  level 0, otherwise they stayed wrongly indented under the preceding sub-list.
- **Label bolding** — a leading `Label:` of ≤4 words gets bolded, with a stopword list (`We`, `This`,
  `It`, `If`, `When`, …) so ordinary sentences containing a colon aren't mangled.

### Verification — the bit worth reusing
A **token-level diff** between source and output: normalise away markdown punctuation, tokenise,
`Counter(source) - Counter(output)`.

**Result: 12,747 of 12,760 tokens preserved.** All 13 differences were the title lines (rebuilt in
the new header) plus casing (`SECTION`→`Section`, `MASTER INDEX`→`Master Index`). That is a
cheap, near-total proof of no content loss — reuse it for any future document conversion.

Also checked: 26 fences (balanced), 77 `#### Q` headings, 9 Part headings, 0 stray language tags
outside fences, 0 residual tab characters.

---

## 4. Detailed sections A & B — the rehearsal layer

Two files under a new `02_Questions/Detailed/` folder:

| File | Lines | Covers |
|---|---:|---|
| `QA_Detail_A_RAG_Architecture_15Q.md` | 1,332 | RAG architecture & design (Q1–Q15) |
| `QA_Detail_B_Azure_Foundry_OpenAI_15Q.md` | 1,476 | Azure AI Foundry & Azure OpenAI (Q16–Q30) |

**Format** — per `00_PLAN_InterviewQA_2026-08-08.md` §5, extended to the four-verb structure the user
asked for:

```
what they're testing · 60-second spoken answer (literal words)
WHAT it is · WHY it works that way · WHEN to use it / when not
HOW to implement or diagnose · your example · the trade-off
follow-up probes with short answers · red flag (what a weak answer sounds like)
+ drill sheet (one must-say sentence per Q) + cross-reference table
```

The plan's standing rule is **four points — what it is, why it works that way, a concrete example,
the trade-off** — and it's marked non-negotiable. The user's "why/when/how/what" request maps onto
it with WHEN and HOW added, so both were satisfied rather than one replacing the other.

---

## 5. Where section A and section B came from

The section-A list was pasted inline in the prompt. When section B was requested, the source had to
be located — it is:

**`_Archive/StaleTrackers/Interview_Prep_AI_Engineer_Complete.md`**, which is the delivered output of
**PRD Feature 12** (`04_Career/PRD_Bala_AI_Career_Acceleration.md`, ~line 715): *"100 Senior AI
Engineer interview questions"* with exactly this distribution:

| | Section | Q | Status |
|---|---|---:|---|
| A | RAG Architecture & Design | 15 | ✅ expanded |
| B | Azure AI Foundry & Azure OpenAI | 15 | ✅ expanded |
| C | AI Agents & Agentic AI | 15 | ⬜ |
| D | LLMOps, Evaluation & Monitoring | 10 | ⬜ |
| E | Vector Databases & Embeddings | 10 | ⬜ |
| F | Prompt Engineering | 10 | ⬜ |
| G | Open-Source LLMs & Hugging Face | 8 | ⬜ |
| H | Fine-tuning (LoRA/QLoRA) | 7 | ⬜ |
| I | AI Safety & Responsible AI | 5 | ⬜ |
| J | System Design — AI Systems | 5 | ⬜ |

**So the `Detailed/` folder is the depth pass over PRD Feature 12, section by section. C–J remain.**
Section C (Agents, 15 Q) is the obvious next one — it's the largest remaining and the material the
last five interviews probed hardest.

---

## 6. Content flags raised — the defensibility problems

These are the substantive findings, not formatting. Each is written into the relevant file rather
than only mentioned in conversation.

1. **Hybrid vs keyword-only contradiction (§A-Q4).** The source anchor says *"JMA's prod index is
   keyword-only; staging adds vectors."* The resume claims **95% retrieval accuracy via hybrid
   vector/keyword** in production. Both cannot be said to the same interviewer. File A carries a ⚠️
   block at the top with both tellings; the answer is written the resume-consistent way
   (keyword-only = the baseline migrated *from*). **Still needs the user's decision on which is true.**

2. **"Eliminating hallucinations."** Restated throughout as *measured groundedness with validated
   citations*. Nobody eliminates hallucination; the claim invites a follow-up that can't be won.

3. **`text-embedding-ada-002` is a live liability (§B-Q30).** Named in the Bible as the JM Family
   embedding model. 2022 model, on the retirement track, beaten by `text-embedding-3-*` on quality
   *and* price. Turned into a strength: a sized re-embedding project — vectors from different models
   aren't comparable, so migration invalidates the whole index → parallel index, Batch API for the
   re-embed, retrieval golden-set re-run, atomic alias flip.

4. **Two anchors describe candidates, not shipped systems (§B-Q24, §B-Q28).** "JMA invoice extraction
   candidate" and "nightly ticket classification fit." Written as designs to defend, with the exact
   phrasing supplied. Claiming them as production invites throughput/reconciliation follow-ups.

5. **Azure product naming has churned twice** — Azure OpenAI Studio → Azure AI Studio → Azure AI
   Foundry, and further since. Verify the current portal name before any interview.

6. **The Bible's numbers are more specific than the resume's** — 78%→95% retrieval, $152,300,
   92% agent completion, 65% cache hit rate, 6% recall@10 lift, zero injection attacks in 12 months.
   Flagged in a callout at the top of the Bible: precise numbers invite precise follow-ups.

**Independent corroboration:** the parallel window's `Interview_QA_Resume_Based.md` arrived at the
same two resume edits (Appendix B — "eliminating hallucinations" and MCP-as-compliance-standard)
without coordination.

---

## 7. Two windows on one repo — what happened and what to do about it

A second Claude session was working `02_Questions/` at the same time, executing Phases 1–4 of the
plan file. Observed effects:

- Files appeared mid-session that this window had not created (`Interview_QA_Resume_Based.md`,
  `Interview_QA_RealWorld_Asked.md`, `00_DRILL_INDEX.md`, `_tooling/`).
- On the final request, `git status` was **clean** — the other window had already staged and
  committed this window's Section B file and index rows inside its own commit `2a86360`.

**Handling that worked:** commit only files this window authored; leave the other window's
in-progress files alone so a commit here can't capture a half-written state; before committing,
re-check `git status` and `git log -- <path>` to see whether the file is *already* in history.
Pushing then carried both windows' work correctly, with no duplicate commit and no conflict.

**Rule for next time:** with two windows on one repo, always `git log --oneline -- <file>` before
committing. Assuming your own uncommitted work is still uncommitted is wrong often enough to matter.

---

## 8. Public-repo decision

`github.com/confksq/AIML-Learn` is **PUBLIC** (confirmed via `gh repo view`). Flagged before the
first push, because the Bible carries more employer-specific detail than what was already there:
named JM Family pipeline internals, `$152,300`, "zero cross-department data leakage", KPMG's Neo4j
contract schema, ADP's cross-cloud IAM setup. The existing `.gitignore` already excludes
`02-DealerIntelligence-Platform/` and `05_Assessments/` as *"Employer-owned / sensitive —
deliberately NOT pushed to personal GitHub"*, so a line already exists.

Options offered: push as-is · gitignore the Bible · keep local · make the repo private first.
**User chose: push everything as committed.** Done.

Noted for the record: removing this later needs a history rewrite plus a GitHub cache-purge request
— a plain delete-and-commit does not remove it.

---

## 9. Files produced this session

| File | Lines | Commit |
|---|---:|---|
| `00_QA_FILE_INDEX.md` | ~90 | `fd18dec`, updated in `71385e9` / `2a86360` |
| `02_Questions/Interview_Bible_77Q_FDE_AI_Lead.md` | 1,469 | `71385e9` |
| `02_Questions/Detailed/QA_Detail_A_RAG_Architecture_15Q.md` | 1,332 | `71385e9` |
| `02_Questions/Detailed/QA_Detail_B_Azure_Foundry_OpenAI_15Q.md` | 1,476 | `2a86360` (other window) |

**Left uncommitted deliberately:** `BALA K - ULTIMATE FDEAI LEAD INTERV.txt` — the source export.
The `.md` supersedes it, and committing it would put the same employer-specific content in a public
repo twice. Still sitting at repo root.

---

## 10. Open items

1. **Decide the hybrid-vs-keyword story** (§6.1) and edit `Detailed/A` Q4 + Q15 to match. This is the
   only flag that blocks drilling section A honestly.
2. **Sections C–J of PRD Feature 12** remain un-expanded. C (Agents, 15 Q) next.
3. **`ada-002` migration** — size it or drop it from the resume narrative.
4. **`07_ChatHistory/INDEX.md` still carries its stale-rows warning** — the 2026-07-29 (×4) and
   2026-08-02 (×3) transcripts remain unread and unindexed. Untouched this session.
5. **Source `.txt`** — commit as provenance, or delete.
