# Module 35 — AI-Assisted Engineering

**Part 7: Platform Engineering & AI-Assisted Delivery**
*Created: 2026-07-26 · FDE-Prep · Clears tracker rows 12, 53, 54, 55, 56*

> **This is the shortest module and the highest-return one.** Most of it is *doing*, not reading.
> One evening — install Cursor, ship one real change — turns four red tracker rows green.

---

## Why This Module Exists

The JD does not list Cursor and Copilot as nice-to-haves. It says:

> *"**Build AI-assisted solutions using Cursor and GitHub Copilot.**"* — Key Responsibilities
> *"**AI-native mindset** — uses AI as the first approach to solving problems."* — Behavioral Traits
> *"These engineers are expected to act as **transformation catalysts — not traditional developers**."*

And it quantifies the outcome already achieved on the account: **60% reduction in migration effort,
50% reduction in modernization effort.**

The library's coverage before this module: **Cursor — zero hits. Copilot — 14 files, every one of
them about Copilot *Studio* or *M365 Copilot*, the products.** Nothing about AI-assisted coding as a
practice. That is the gap.

---

## Section 1 — The Tool Landscape

| Tool | What it is | Best at | Model |
|---|---|---|---|
| **GitHub Copilot** | IDE completion + chat, in VS Code / Visual Studio / JetBrains | Inline completion while you type; you already have it | GPT / Claude |
| **Cursor** | A **fork of VS Code** built around AI | Multi-file edits, codebase-wide context, agent mode | Claude / GPT, switchable |
| **Claude Code** | Terminal-native agent | Long autonomous tasks, git operations, running commands | Claude |
| **Windsurf** | VS Code fork, agentic | similar to Cursor | mixed |

**Cursor is a VS Code fork**, which is the important practical fact: your extensions, keybindings
and settings import on first launch. Switching costs about five minutes.

### 1.1 The mental model that matters

```
COMPLETION           →  "finish this line"        Copilot tab
CHAT                 →  "explain / write this"    Copilot Chat, Cursor Chat
MULTI-FILE EDIT      →  "change this across 12 files"   Cursor Composer   ← the real unlock
AGENT                →  "do this task; run commands, read files, iterate"  Cursor Agent,
                                                                            Claude Code
```

Most people stop at completion and conclude AI coding is marginal. **The value is in the bottom two
rows** — and that is exactly the difference between a "traditional developer" and the
"transformation catalyst" the JD is describing.

---

## Section 2 — Cursor

### 2.1 Setup (do this tonight)

1. Download from `cursor.com`, install, choose *"import VS Code settings"*.
2. Open a real repository — `jma-claims-automation` or your CallMiner repo. **Not a toy project.**
   Cursor's advantage is codebase context; a hello-world hides it entirely.
3. Sign in. Pick a model (Claude for reasoning-heavy work, GPT for speed).
4. Learn four keys and stop there:

| Key | Feature | Use |
|---|---|---|
| `Tab` | completion | accept inline suggestion |
| `Ctrl/Cmd + K` | inline edit | select code → describe the change → applied in place |
| `Ctrl/Cmd + L` | chat | ask about the selected code |
| `Ctrl/Cmd + I` | **Composer** | **multi-file change — the one that matters** |

### 2.2 Codebase context — `@` symbols

```
@file        one file
@folder      a directory
@codebase    semantic search across the whole repo
@git         diffs and history
@docs        indexed external documentation
@web         live search
```

```
@codebase Where do we handle language detection for the CallMiner feed,
and what happens when Appl doesn't start with I or O?
```

That is a question Copilot's inline completion cannot answer, because it has no repo-wide index.

### 2.3 `.cursorrules` — the highest-leverage file in the repo

A file at the repo root that is prepended to **every** AI request in that project. It is how you
stop re-explaining your conventions.

```
# .cursorrules

## Project
Azure-hosted .NET 8 services plus Python data pipelines for JM Family.

## C#
- Target .NET 8. File-scoped namespaces. Nullable enabled.
- Constructor injection only — no service locator.
- No secrets in code. Azure Key Vault via DefaultAzureCredential, always.
- xUnit + FluentAssertions. Arrange/Act/Assert, no comments marking the sections.

## Python
- 3.12. Type hints on every public function.
- Pydantic for anything crossing a boundary (LLM output, HTTP, config).
- Generators over list comprehensions for anything file-sized.
- pytest. No unittest.

## Infrastructure
- Bicep for Azure, Terraform when multi-cloud.
- Never commit .tfstate. Never inline a connection string.

## Style
- Match surrounding code. Do not add explanatory comments for obvious lines.
- If you are unsure about a business rule, ask — do not invent one.
```

That last line is worth its weight. Without it, models invent plausible business rules and you find
out in production.

### 2.4 Composer — the multi-file unlock

`Ctrl+I`, then describe an outcome rather than an edit:

```
Add structured logging to every tool method in the cancellation agent.
Use ILogger<T> via constructor injection, log method entry with arguments
and exit with duration in ms. Follow the pattern already in
Services/ContractService.cs. Don't log the VIN — it's PII.
```

Cursor proposes a diff **across every affected file**. You review file by file and accept or reject
each. This is where the JD's 50–60% numbers come from — not from faster typing.

### 2.5 Agent mode

Cursor runs commands, reads output, and iterates. Good for: "run the tests, fix what fails."
Dangerous for: anything touching infrastructure without review.

**Rule: agent mode on a branch, never on `main`, and read the diff before committing.**

---

## Section 3 — Prompting for Code

Prompting a codebase is not prompting a chat window. `L15` covers general prompt engineering; this
is the coding-specific delta.

| ❌ Weak | ✅ Strong |
|---|---|
| "Add error handling" | "Wrap the HTTP call in the retry policy from `Resilience/PolicyFactory.cs`, 3 attempts, exponential backoff. Let `TaskCanceledException` propagate — the caller handles cancellation." |
| "Write tests" | "Write xUnit tests for `CalcRefund` covering: zero months used, full term elapsed, negative refund clamped to zero, and null contract. Use FluentAssertions, match the style in `RefundServiceTests.cs`." |
| "Make this faster" | "This runs in O(n·m) because `known_vins` is a list. Convert to a set and explain the complexity change in the PR description." |

### 3.1 The four ingredients

1. **Point at an existing pattern** — *"follow `ContractService.cs`"* beats any style description.
2. **State the constraint** — *"don't log the VIN"*, *"no new NuGet packages"*.
3. **Say what "done" means** — *"tests pass and `dotnet format` is clean"*.
4. **Give the failure mode** — *"if the business rule is unclear, ask; don't invent one"*.

### 3.2 What to never delegate unreviewed

| Delegate freely | Review every line |
|---|---|
| Boilerplate, DTOs, mappers | Anything touching auth or secrets |
| Test scaffolding | IAM / RBAC policy |
| Doc comments, READMEs | Money or refund calculations |
| Format/lint fixes | Anything that deletes data |
| Language translation (Bicep→Terraform) | Regex on production data |

The middle column is the one that separates people who use these tools safely from people who
eventually cause an incident.

---

## Section 4 — Measuring the Gain

The JD is explicit about **measurable productivity improvements**, and the account already quotes
60% and 50%. If you cannot quantify, you cannot be a transformation catalyst — you are just someone
with a plugin.

### 4.1 What to measure

| Metric | How | Example claim |
|---|---|---|
| **Cycle time** | ticket open → merged | "PR cycle time down from 3 days to 1" |
| **Effort per unit** | hours per migrated service | "4 hrs → 1.5 hrs per service" |
| **Coverage delta** | before/after test coverage | "62% → 84% in two sprints" |
| **Toil eliminated** | runs/month × minutes saved | "40 manual runs × 25 min = 16 hrs/month" |
| **Lead time to POC** | idea → working demo | "2 weeks → 2 days" |

### 4.2 How to measure honestly

Baseline **before** you adopt. Pick a repeating task, time three instances, then time three with AI
assistance. Cite the sample size. *"Roughly 3× on Bicep-to-Terraform translation, measured over five
modules"* is credible; *"AI made us 60% faster"* is not.

**Interview line:**
> "I baseline first. On a repeating task — say translating Bicep modules to Terraform — I timed
> three by hand, then three with Cursor Composer plus `terraform plan` as the verification gate.
> That gave a defensible per-unit number rather than a vibe. The verification gate matters as much
> as the generation: the model writes it, the plan proves it, so speed doesn't cost correctness."

---

## Section 5 — AI-First Plays for an Infrastructure Engineer

Concrete, from the JD's own target domains. Each is a POC you could demo.

| Domain | The manual toil | AI-first play |
|---|---|---|
| **Cloud** | Reading 400 VM configs to write Terraform | Feed discovery output to a model; generate modules; `plan` verifies |
| **Cloud** | Bicep → Terraform by hand | Composer translates a module at a time; `plan` + Checkov gate |
| **Network** | Reading firewall rules to find why egress fails | Model parses rule dumps and traces the path |
| **Compute** | Right-sizing from utilisation reports | Model reads metrics, proposes requests/limits |
| **Database** | Schema archaeology on a vendor feed | Model maps a 47-column `.unl` to a documented schema *(you did this manually)* |
| **Security** | Triaging a CVE report across repos | Agent reads the report, greps, opens PRs bumping versions |
| **Automation** | Writing runbooks | Generate from the IaC and pipeline definitions |

### 5.1 The vulnerability-remediation pipeline

The JD calls out *"automated vulnerability discovery and remediation."* The shape:

```
Scanner (Trivy / Snyk / Dependabot / DSX)
    │  findings
    ▼
Triage agent  ── is it reachable? is there a fix version? blast radius?
    │
    ├─ auto-fixable → open a PR bumping the dependency, tests run in CI
    ├─ needs judgment → enrich the ticket with context, assign to a human
    └─ false positive → suppress with a recorded reason
```

**You have already built the first box** — the cert-pinned DSX REST scan client, live in dev,
returning real scan verdicts. The triage layer on top is a weekend POC and it is *literally* what
this JD asks for.

### 5.2 N8N — awareness

A low-code, self-hostable workflow automation tool (open-source Zapier) with LLM and agent nodes.
Where it fits: **glue between systems for non-developers** — ServiceNow ticket → enrich with an LLM
→ post to Teams → open a Jira issue, all drawn on a canvas.

| Use N8N when | Use LangGraph/SK when |
|---|---|
| Business-user-visible glue | Production agent logic |
| Many SaaS connectors, little logic | Complex state, retries, checkpointing |
| Prototype in an afternoon | Testable, version-controlled, in CI |

Know what it is; don't put production agent orchestration in it.

---

## Section 6 — Anthropic Computer-Use (tracker row 12)

### 6.1 What it is

A model capability where the LLM is given screenshots and can emit mouse/keyboard actions — click,
type, scroll — driving a GUI like a person. The loop:

```
screenshot → model reasons → emits action (click x,y / type "…") →
executed in a VM → new screenshot → repeat
```

It is the same **agent loop** you already know (`L27`), with the tool set replaced by GUI primitives
instead of API calls.

### 6.2 Why an infrastructure org cares

The honest answer: **for legacy systems with no API.** Mainframe green screens, vendor consoles,
appliance web UIs that were never meant to be automated. That is common in the exact enterprise
estates this JD targets.

### 6.3 Why you would usually *not* use it

| Concern | Detail |
|---|---|
| **Brittle** | A UI redesign breaks it; an API contract does not |
| **Slow and costly** | Every step is a screenshot — many tokens per click |
| **Hard to audit** | "It clicked something" is a poor audit trail |
| **Security** | An agent with mouse control has enormous blast radius. Sandbox it |

**Interview line:**
> "Computer-use is the agent loop with GUI primitives instead of API tools — screenshot in, click or
> keystroke out. I'd use it as a last resort, for legacy systems that genuinely have no API, and
> always in a sandboxed VM with a human gate on anything irreversible. If an API exists, that's a
> better tool call: faster, cheaper, auditable, and it doesn't break when someone moves a button."

---

## Section 7 — Making It Real Tonight

The whole module in one evening:

| # | Do | Time |
|---|---|---|
| 1 | Install Cursor, import VS Code settings | 5 min |
| 2 | Open a **real** repo | 2 min |
| 3 | Write a `.cursorrules` for it — copy §2.3 and edit | 15 min |
| 4 | Ask `@codebase` three questions you already know the answer to. Judge the quality | 10 min |
| 5 | `Ctrl+K` one small refactor | 10 min |
| 6 | **`Ctrl+I` Composer — one real multi-file change** | 30 min |
| 7 | Run the tests. Fix what broke | 15 min |
| 8 | **Commit and push it** | 5 min |
| 9 | Write down: what took how long, and what you would have estimated by hand | 10 min |

Step 9 is the one people skip, and it is the one that becomes the résumé bullet and the interview
answer. Step 8 is what makes rows 53–56 green rather than aspirational.

---

## Self-Test Questions

1. Cursor vs Copilot — what can Cursor do that inline completion fundamentally cannot, and why?
2. What is `.cursorrules` and why does it beat repeating instructions in each prompt?
3. Name three things you would never accept from an AI without reading every line.
4. Why is "AI made us 60% faster" a weak claim? What would you say instead?
5. What is the verification gate in AI-generated Terraform, and why does it matter more than the
   generation?
6. Describe the computer-use loop. How is it the same as the agent loop in `L27`?
7. Give one case where computer-use is right and one where an API call is clearly better.
8. When is N8N the right tool, and when is it the wrong one?
9. You are asked to cut vulnerability-remediation toil. Sketch the pipeline and name the human gate.

---

## Quick-Reference Interview Answers

**"How do you use AI in your day-to-day engineering?"**
> "Three levels. Completion for typing, chat for understanding unfamiliar code, and multi-file
> composer for real changes — that last one is where the gain actually is. I keep a `.cursorrules`
> in each repo with our conventions so I'm not re-explaining them, and I point the model at an
> existing file as the pattern rather than describing style. What I don't delegate unreviewed is
> anything touching auth, IAM, money or data deletion. And I baseline before I claim a number —
> on Bicep-to-Terraform translation I measured roughly 3× over five modules, with `terraform plan`
> and Checkov as the gate, so speed didn't cost correctness."

**"What does 'AI-first' mean to you?"**
> "Asking 'can this be automated or generated?' before asking 'how long will this take me?'. In an
> infrastructure org the toil is legible — reading configs, translating templates, triaging scan
> findings, writing runbooks — and all of it is generation plus a deterministic verification step.
> The catalyst part isn't using the tools myself; it's making the verification gate good enough that
> the team trusts the generation. That's what makes it spread."

**"How would you automate vulnerability remediation?"**
> "Scanner produces findings, a triage agent enriches each one — is the path reachable, is there a
> fix version, what's the blast radius — then it splits three ways: auto-fixable gets a PR bumping
> the dependency with CI proving nothing broke, judgment calls get an enriched ticket routed to a
> human, false positives get suppressed with a recorded reason so they don't come back. The human
> gate sits on anything that changes runtime behaviour rather than a version pin. I've already built
> the scanning half of that — a cert-pinned REST scan client running in dev — so the triage layer is
> the increment."

---

## Related

`L15` (prompt engineering fundamentals) · `L27` (the agent loop that computer-use reuses) ·
`L33` (`terraform plan` as the verification gate) · `L26` (MCP — how these tools reach your systems) ·
`Part6_AppliedProjects/03-SecurityAutomation-VulnScan/` (the shipped scan client)
