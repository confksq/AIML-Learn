# Résumé Bullets — Security Automation / Vulnerability Management

*Created 2026-07-26 · FDE-Prep · Clears tracker row 40*

> ⚠️ **Sanitised.** No vendor names, hostnames, Key Vault names, subnets, service accounts or
> ticket numbers. Describe the pattern and the metric — never the identifiers.
> Fill the `[…]` placeholders with your own true numbers before using these.

---

## Primary bullet — lead with this

> **Built an automated malware-scanning gateway** into a containerised data pipeline handling
> third-party file ingestion, integrating a REST scanning service with **certificate pinning**,
> **fail-closed verdict handling** and **dead-letter quarantine** — eliminating an unscanned
> external-to-external file path and clearing the security control gate for production release.

**Why this wording:** it names the JD's exact phrase (*security automation*), states an
architectural decision (fail-closed), and ends in an outcome rather than an activity.

## Alternate — Kubernetes emphasis

> Designed a **Kubernetes-native security scanning integration** for an ephemeral, autoscaled
> workload, rejecting agent-based scanning in favour of a REST scan service to keep pod images
> immutable and scanning capacity independently scalable; secrets and pinned certificate material
> resolved at runtime via **workload identity**, never baked into images.

## Alternate — engineering-rigour emphasis

> Delivered a cert-pinned scan client with **86 unit and 30 integration tests**, then root-caused a
> **response-stream disposal defect** that only surfaced against production-sized payloads —
> establishing a fixture-sizing standard so resource-lifetime bugs are caught before live testing.

## Alternate — infrastructure engineering (tracker rows 23, 34)

> Engineered end-to-end connectivity for a cloud data pipeline across **Kubernetes egress, hub
> firewall policy, managed secret storage, private database access and privileged-identity
> activation**, diagnosing and resolving a `CrashLoopBackOff` caused by missing RBAC bindings.

## Forward-looking — the increment worth proposing

> Designed an **AI-assisted vulnerability triage layer** on top of scanner output: reachability and
> fix-availability analysis, automated pull requests for deterministic version pins with CI as the
> proof gate, human approval for anything altering runtime behaviour, and **expiring suppressions**
> so false positives cannot become permanent blindness.

---

## Interview story — 90 seconds

**Situation.** A pipeline ingested recording files from an external vendor and forwarded them to an
external analytics platform. Nothing inspected the payload in between — a clean automated path
between two external parties, running on our credentials.

**Task.** Insert a scanning control without breaking throughput, in a workload that runs as
ephemeral autoscaled pods.

**Action.** Rejected agent-based scanning — wrong shape for pods that live minutes and run immutable
images. Built a REST client against a scanning service instead, with certificate pinning because the
verdict is a security decision and standard TLS trusts every CA in the store. Secrets and the pinned
value came from a managed secret store via workload identity. Fail-closed: an inconclusive scan
retries with backoff, then dead-letters for human review — a scanner that fails open is decoration.

**Result.** Shipped to dev returning real verdicts. 86 unit and 30 integration tests. Live testing
surfaced one genuine bug the tests could not — a response stream disposed before it was fully read,
invisible with small fixtures and fatal at real payload sizes. Fixed it and changed the fixture
standard so resource-lifetime bugs get caught earlier.

**What I'd do next.** The scanning half is discovery. The remediation half is a triage agent doing
reachability and fix-availability analysis, auto-PRs for version pins with CI as the gate, humans
approving anything that changes runtime behaviour, and suppressions that expire.

---

## Follow-up questions to be ready for

| Question | The short answer |
|---|---|
| *Why pin certificates rather than trust standard TLS?* | The scan verdict is a security decision. Standard validation trusts every CA in the store; a compromised or mis-issued cert returns "clean" for anything. Pinning narrows trust to one expected certificate. |
| *What breaks with pinning?* | Certificate rotation. The expected value lives in a secret store, not the image; there's a documented rotation runbook and an alert on pinning-failure rate — otherwise a routine renewal becomes an outage. |
| *Why not scan at the source?* | We don't control the vendor. The control belongs at the trust boundary we own. |
| *What if the scanner is down?* | Fail closed. Retry with exponential backoff and jitter, circuit-break after repeated failures, dead-letter for replay. Throughput degrades; nothing unscanned moves. |
| *How would you know it's working?* | Scan rate, verdict distribution, p95 scan latency, dead-letter depth, and pinning-failure count — alert on the last two. |
| *Isn't this just a proxy?* | It's a control point. The engineering is in the failure modes: what happens on timeout, on an inconclusive verdict, on a rotated certificate, on a payload larger than expected. |

---

## Related

`01_concepts.md` (the architecture) · `L31` §2–3 (retry, circuit breaker, dead-letter) ·
`L33` §9.3 (policy-as-code in the same pipeline) · `L35` §5.1 (the AI triage layer) ·
`L36` (metrics and alerting)
