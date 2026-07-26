# 03 — Security Automation: Vulnerability Scanning in an AI Pipeline

*Part 6 applied project · Created 2026-07-26 · FDE-Prep · tracker row 40*

> ⚠️ **Sanitised write-up of real shipped work.** No vendor names, hostnames, subnets, Key Vault
> names, service accounts or ticket references. Pattern and metrics only.

## What this is

Unlike the other Part 6 modules, this is not a tutorial you work through — it is **documentation of
something you already built**, written so it stops being invisible.

The Forward Deployed AI Engineer JD asks for *"security automation and vulnerability management"*
and names *"an automated vulnerability discovery and remediation pipeline."* The discovery half was
already delivered; it just lived in a repository and a chat log rather than anywhere an interviewer
or an ATS could see it.

## Files

| File | Contents |
|---|---|
| `01_concepts.md` | The architecture, the four engineering decisions worth defending, the stream-disposal bug and its lesson, and the AI triage layer that would complete it |
| `05_resume_bullet.md` | Five ready bullets, a 90-second STAR story, and six follow-up questions with answers |

## The one-line version

A malware-scanning gate inserted between external file ingestion and external forwarding in a
containerised pipeline — REST scan service rather than host agents (right shape for ephemeral pods),
certificate-pinned (the verdict is a security decision), fail-closed with dead-letter quarantine
(a scanner that fails open is decoration).

## Why it earns a place in Part 6

| Part 6 criterion | Met |
|---|---|
| Runnable/real, not just readable | ✅ shipped to a dev environment, returning real verdicts |
| Employer-facing evidence | ✅ résumé bullets + interview story |
| Teaches something | ✅ security control design, cert pinning trade-offs, the limits of test coverage |

## Related

`L31` §2–3 · `L33` §9.3 · `L34` · `L35` §5.1 · `L36` · `VitalCare_AI_Assessment_Response.md:1441`
