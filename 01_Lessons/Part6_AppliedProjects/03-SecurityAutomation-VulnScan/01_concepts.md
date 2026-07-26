# Security Automation — Malware Scanning in an AI Data Pipeline

**Part 6: Applied Projects** · *Created 2026-07-26 · FDE-Prep · Clears tracker row 40*

> ⚠️ **Sanitised.** This documents a real system shipped to a development environment. All
> internal identifiers — hostnames, subnet ranges, Key Vault names, service-account names, ticket
> content, vendor endpoints and campaign codes — have been removed. What remains is the
> **architecture pattern, the engineering decisions and the measurable outcome**. Nothing here is
> classified above Public/General.
>
> **Rule for reuse:** when you talk about this in an interview, describe the *pattern and the
> metric*. Never the identifiers.

---

## Why this exists

The Forward Deployed AI Engineer JD lists, under Preferred Skills:

> *"Security automation and vulnerability management"*

and in the account summary:

> *"an automated vulnerability discovery and remediation pipeline"*

This is the one requirement where the work was **already done** — it simply lived in a repository
and a chat log instead of a portfolio file. That is the definition of an invisible skill.

---

## The problem

An audio/data pipeline ingests third-party recording files from an external vendor over SFTP, then
forwards them to a downstream analytics platform. Two facts make this a security control point:

1. **The files originate outside the organisation's trust boundary.**
2. **They are forwarded onward**, so anything malicious propagates rather than terminating.

Without a scanning gate, the pipeline is a clean, automated malware delivery path between two
external parties, running with the organisation's credentials.

```
external vendor ──SFTP──► ingest service ──► transform ──► external analytics platform
                                    ▲
                                    │  ← nothing inspected the payload
```

## The control

Insert a scan gate between ingest and forward. Nothing moves downstream until it returns a verdict.

```
vendor ──SFTP──► ingest ──► ┌─────────────────┐ ──clean──► transform ──► downstream
                             │  SCAN GATEWAY   │
                             │  (REST client → │ ──infected──► quarantine + alert
                             │   scan service) │
                             └─────────────────┘ ──error────► retry, then dead-letter
```

---

## Engineering decisions worth defending

### 1. REST client over agent-based scanning

An on-host scanning agent assumes a long-lived host. The workload runs as **pods in a managed
Kubernetes cluster** — ephemeral, autoscaled, immutable images. Installing and licensing an agent
per pod is the wrong shape.

A REST call to a scanning service keeps the pod stateless and the scanning capacity independently
scalable.

| | Agent per host | **REST scan service** |
|---|---|---|
| Fits ephemeral pods | ✗ | **✓** |
| Image stays immutable | ✗ | **✓** |
| Scales independently | ✗ | **✓** |
| Adds a network dependency | — | ✓ (mitigated by retry + circuit breaker) |

### 2. Certificate pinning

The scan verdict is a security decision. If an attacker can man-in-the-middle the scan call, they
can return `clean` for anything. Standard TLS validation trusts every CA in the store; **pinning
narrows trust to one expected certificate**.

```csharp
// Pattern only — no real thumbprints or endpoints.
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
    {
        if (cert is null) return false;
        // Compare against the expected certificate, sourced from a secret store —
        // never hardcoded, never committed.
        return CryptographicOperations.FixedTimeEquals(
            cert.GetCertHash(), _expectedThumbprint);
    }
};
```

**The trade-off to state honestly:** pinning breaks on certificate rotation. You need the expected
value in a secret store (not the image), a documented rotation runbook, and an alert on
pinning-failure rate — otherwise a routine renewal becomes an outage.

### 3. Secrets from a managed secret store, never configuration

Credentials and the pinned certificate value are resolved at runtime from a managed secret store
using workload identity — no secrets in the image, in environment variables, or in source.

### 4. Fail closed

On an inconclusive scan the file **does not** move downstream. It retries with exponential backoff,
then dead-letters for human review.

> "A scanner that fails open is decoration. If I cannot prove a file is clean, it does not move."

This is `L31` §3's dead-letter-replay pattern applied to a security control.

---

## The bug that only live testing found

Unit and integration tests passed. The first real scan against the live service crashed.

**Cause:** the response stream was disposed before it had been fully consumed. In tests the payloads
were small enough to buffer completely in one read, so disposal-order never mattered. Real files
were large enough that the read was still in progress when disposal ran.

**The lesson, which is the actually valuable part:**

| | |
|---|---|
| What the tests proved | The logic was correct |
| What they could not prove | The **resource lifetime** was correct under real payload sizes |
| Why | Test fixtures were small; small streams hide disposal-order bugs |
| Fix | Consume the stream fully before disposal; assert on a payload large enough to force multiple reads |

**Interview value:** this is a concrete, specific engineering story with a root cause and a
generalisable lesson. It is far stronger than "I have good test coverage," because it shows you know
what tests *cannot* tell you.

---

## Where AI extends this — the JD's actual ask

What shipped is the **discovery** half. The JD asks for *"discovery **and remediation**."* The
remediation layer is the natural increment and a strong POC:

```
scanner / SBOM / dependency findings
            │
            ▼
      TRIAGE AGENT
      ├─ Is the vulnerable path actually reachable in our code?
      ├─ Does a fixed version exist? Is it a breaking change?
      └─ What is the blast radius if we patch now?
            │
   ┌────────┼─────────────────────┐
   ▼        ▼                     ▼
auto-fix   enrich + assign     suppress
open a PR  to a human with     with a recorded
bumping    context already     reason and an
the version gathered           expiry date
   │
   ▼
CI proves nothing broke ── human approves the merge
```

**Design points to defend:**

| Decision | Reasoning |
|---|---|
| Agent **triages**, humans **approve** | Autonomy Level 3 — `VitalCare:1441`. It gathers and recommends; it does not merge |
| Auto-PR limited to **version pins** | Deterministic, reversible, and CI proves it |
| Suppressions **expire** | Otherwise "false positive" becomes permanent blindness |
| Every action logged to the trace | `L36` — an auditor needs to know why a finding was closed |
| Reachability before severity | A CVSS 9.8 in a code path you never call outranks nothing |

**The metric that matters** is not "vulnerabilities found" — a scanner does that. It is **mean time
to remediate**, and the proportion closed without human effort.

---

## What this demonstrates on a CV

| Claim | Evidence |
|---|---|
| Security automation in a CI/CD and container context | Scan gateway integrated into an automated pipeline |
| Secure-by-design engineering | Certificate pinning, fail-closed, secrets from a managed store |
| Kubernetes-native thinking | Rejected agent-based scanning as wrong for ephemeral workloads |
| Production debugging | Stream-lifetime bug found in live testing, root-caused and fixed |
| Test discipline **and its limits** | 86 unit + 30 integration tests passing — and a clear account of what they could not catch |

---

## Related

`L31` §2–3 (retry, circuit breaker, dead-letter replay) · `L33` §9.3 (Checkov/OPA — policy-as-code
in the same pipeline) · `L34` (deploying this into Kubernetes) · `L35` §5.1 (the AI triage layer) ·
`L36` (tracing and alerting on it) · `VitalCare_AI_Assessment_Response.md:1441` (autonomy ladder)
