
# Module 05 — MCP Hub: What It Is, How It Works, and Why Healthcare Needs It


---

## Why This Module Matters

The screener confirmed focus on "entire AI agent workflow end-to-end." MCP Hub is the connective tissue of that workflow — it's how your agent reaches every tool cleanly. You will be asked:
- "What is MCP and why does it exist?"
- "What's the difference between MCP and APIM?"
- "How would you use them together in a healthcare system?"

Your anchor: JM Family has `oai-jma-dev-shared-mcp` and `di-jma-dev-shared-mcp` — MCP-connected resources in dev.

---

## Section 1 — What MCP IS (and why it exists)

Before MCP, every agent that needed to call an external tool had to be custom-wired. Your agent calls an EHR? You write a custom HTTP client, handle auth, parse the response, handle errors — all manually. Your agent calls a pharmacy system? Same thing again. Different format, different auth, different error codes.

**MCP (Model Context Protocol)** is an open standard (created by Anthropic, now industry-adopted) that gives every tool a **common plug shape** — so any agent can connect to any tool without custom wiring.

**The mental model:** Think of MCP like a **universal power adapter**. Before it, every country had a different socket (EHR socket, pharmacy socket, lab socket). You needed a custom adapter for each one. MCP is the universal socket standard — one adapter fits all tools. The agent plugs in once, talks to everything.

**What MCP standardizes:**
- How a tool **advertises** what it can do (tool schema / capability declaration)
- How an agent **calls** the tool (request format)
- How the tool **responds** (response format)
- How **auth** is handled (token passing)

---

**⚙️ Config or Code? — MCP Protocol Itself**
- **Custom Code only:** Writing an MCP Server (implement the tool endpoint in Python/C#/Node), writing the tool schemas (`[Description]` attributes or JSON schema), connecting agent to MCP Hub (SDK code in SK or LangChain)
- **No portal config** for MCP protocol itself — it is a code-level standard

## Section 2 — What an MCP Hub IS

An **MCP Hub** is a central gateway that sits between your agents and all your tools.

```
Agent
  ↓  one connection
MCP Hub  ← the gateway
  ↓  routes to the right tool
┌──────────────────────────────────────────┐
│ EHR Tool  │ Lab Tool  │ Pharmacy Tool  │ FHIR Tool │
└──────────────────────────────────────────┘
```

Without a hub, every agent connects directly to every tool — that's N×M connections. With a hub, every agent connects to one place — the hub routes it. That's N+M connections.

**The hub does four jobs:**
1. **Discovery** — agents ask "what tools are available?" Hub returns the list
2. **Routing** — agent calls `get_patient_medications` → hub knows which backend tool handles that
3. **Auth** — hub holds credentials centrally; agents never touch raw API keys
4. **Logging** — every tool call goes through the hub → one place to audit

**Healthcare example — MCP Hub at a hospital network:**
- 180 hospitals, each with an EHR system
- 12 agents (prior auth, ambient docs, discharge planning, member self-service...)
- Without hub: 180 × 12 = 2,160 custom connections to maintain
- With MCP Hub: each agent connects to the hub, hub connects to 180 EHR endpoints
- When Hospital X upgrades their EHR API — you update ONE connector in the hub, not 12 agents

---

## Section 3 — MCP vs APIM (the question they WILL ask)

Both sit between your agents and your tools. The interviewer wants to know if you understand the difference.

| | MCP Hub | Azure API Management (APIM) |
|---|---|---|
| **Purpose** | AI agent ↔ tool integration | HTTP API gateway for any client |
| **Who uses it** | AI agents (LLMs) | Any app, browser, mobile, agent |
| **Tool discovery** | Built-in — agents ask "what can you do?" | Not built-in — you document APIs externally |
| **Schema format** | AI-native (tool name, description, parameters the LLM understands) | OpenAPI / Swagger |
| **Auth** | Token passing for agent context | OAuth, API keys, subscriptions |
| **Throttling/quota** | Basic | Enterprise-grade (per subscription, per product) |
| **Monitoring** | Agent call tracing | Full API analytics, developer portal |

**The key distinction in one sentence:**

APIM is built for **HTTP traffic from any client**. MCP is built for **LLM agents that need to discover and call tools intelligently.**

**Healthcare analogy:**
- APIM = the hospital's **main reception desk** — any visitor (app, browser, agent) checks in here, gets directed
- MCP Hub = the **clinical coordinator** — specifically wired for how AI agents communicate, understands tool capabilities, speaks the agent's language

---

## Section 4 — The Hybrid MCP + APIM Pattern (what you'd actually build)

In production healthcare, you don't choose one or the other — **you use both together.**

```
Agent (Semantic Kernel)
      ↓
MCP Hub  ← handles tool discovery, agent-native protocol, context passing
      ↓
APIM  ← handles enterprise concerns: throttling, auth, logging, versioning
      ↓
Backend Tools (EHR, FHIR, Lab System, Pharmacy API)
```

**Why this pattern:**
- MCP Hub handles the **AI side** — tool schemas, agent context, LLM-native communication
- APIM handles the **enterprise side** — rate limits, API keys, compliance logging, versioning
- Neither does the other's job well alone

**Healthcare example — Prior Auth agent:**
1. Prior Auth Agent asks MCP Hub: "what tools do I have?"
2. MCP Hub returns: `get_patient_eligibility`, `check_policy_rules`, `submit_auth_request`
3. Agent calls `get_patient_eligibility` via MCP Hub
4. MCP Hub forwards the call to APIM
5. APIM enforces: rate limit (max 100 calls/min), logs the call for HIPAA audit, routes to the payer's eligibility API
6. Response flows back through APIM → MCP Hub → Agent

**JM Family anchor:**
"At JM Family we have `oai-jma-dev-shared-mcp` and `di-jma-dev-shared-mcp` — those are the MCP-connected resources. The pattern is the same: MCP handles the agent-tool protocol, APIM sits in front of our backend APIs for enterprise governance."

**⚙️ Config or Code? — MCP Hub + APIM Pattern**
- **Portal Config only:** Create APIM instance (portal), configure APIM rate limit policies (portal XML policy editor), set up APIM OAuth (portal), create Container Apps environment for hosting MCP servers (portal), set RBAC roles on Key Vault (portal)
- **Custom Code:** MCP server implementation (write the tool endpoints), agent-side MCP client connection (SK code to discover and call tools), tool schema definitions (code attributes/JSON)
- **Both:** APIM policies (create instance = portal; write custom policy expressions = XML code in portal editor); MCP server hosting (Container Apps = portal; server code = custom code)

---

## Section 5 — MCP Hub Governance: Policies and Standards

This is what the job description means by "Govern MCP Hub architecture, defining policies and standards across a centralized pool of MCP Servers." Most candidates know what MCP is — few can speak to governing it at enterprise scale.

### Who Owns the MCP Hub

```
Platform Engineering Team owns the Hub:
├── Sets the standards all MCP servers must follow
├── Approves new MCP servers before they join the pool
├── Manages the server registry / catalog
├── Monitors health and usage across all servers
└── Enforces deprecation and versioning policies

Individual Teams own their MCP Servers:
├── Clinical team owns: fhir-mcp-server, lab-mcp-server
├── Billing team owns: claims-mcp-server, eligibility-mcp-server
└── Each team responsible for their server's uptime + compliance
```

### The 6 Governance Policies You Define

**Policy 1 — Tool Naming Standard**
```
All tools must follow: {domain}_{action}_{resource}
Examples:
├── clinical_get_patient_record      ✅
├── billing_submit_claim             ✅
├── getPatientData                   ❌ rejected — no domain prefix
└── check_stuff                      ❌ rejected — not descriptive
```
Why: Agents use tool descriptions + names to route. Inconsistent naming = wrong tool selection.

**Policy 2 — Tool Description Standard**
```
Every tool description must include:
├── What it does (one sentence)
├── When the LLM should call it (trigger condition)
├── What it returns (output format)
└── What it does NOT do (scope boundary)

Example:
"Retrieves a patient's current medication list from FHIR.
 Call when the user asks about current medications or prescriptions.
 Returns JSON array of active medications with dosage and frequency.
 Does NOT return historical or discontinued medications."
```

**Policy 3 — Authentication Policy**
```
All MCP servers MUST:
├── Use Managed Identity for Azure-to-Azure connections
├── Store external API credentials in Key Vault (never hardcoded)
├── Rotate secrets on a schedule (90-day max for API keys)
└── Log authentication failures to central audit store

No exceptions — platform team rejects servers that violate this.
```

**Policy 4 — Versioning Policy**
```
Semantic versioning: v{major}.{minor}
├── Breaking change (rename tool, change parameters) → v2.0
├── New tool added → v1.1
└── Bug fix → v1.0.1

Deprecation process:
├── Announce deprecation 60 days in advance
├── Run old and new version in parallel during transition
├── Remove old version only after all agents migrate
└── Never delete a version with active callers
```

**Policy 5 — Rate Limiting and Quota Policy**
```
Each MCP server must define:
├── Max calls per minute per agent identity
├── Max calls per minute total
└── Behavior when limit hit: queue vs reject

Enforced by APIM in the hybrid pattern:
└── APIM applies per-subscription quotas
└── MCP Hub reports quota violations to platform team
```

**Policy 6 — PHI Handling Policy**
```
MCP servers that handle PHI MUST:
├── Be deployed in isolated Container Apps environment
├── Have dedicated Key Vault (not shared with non-PHI servers)
├── Log all PHI access to HIPAA audit store
├── Never log PHI in tool call payloads (log IDs only)
└── Require additional RBAC role: "PHI-Tool-Caller"
    └── Not all agents get this — only clinical agents
```

**⚙️ Config or Code? — Governance Policies**
- **Portal Config only:** PHI server isolation in dedicated Container Apps environment (portal), dedicated Key Vault per PHI zone (portal), RBAC role assignment "PHI-Tool-Caller" (portal), APIM rate limiting quota per subscription (portal), Container Apps environment networking (portal)
- **Custom Code:** Tool naming + description standards (enforced in code review + PR templates), versioning policy enforcement (CI/CD pipeline check), PHI payload masking in logs (code in MCP server), server registry/catalog implementation (a config file or API your platform team maintains)
- **Both:** Authentication policy (Managed Identity = portal; Key Vault secret retrieval at startup = Code)

### The Server Registry / Catalog

The MCP Hub maintains a catalog of all approved servers:

```
MCP Server Registry:
┌─────────────────────────────────────────────────────────┐
│ Server Name        │ Domain   │ PHI │ Version │ Owner   │
├─────────────────────────────────────────────────────────┤
│ fhir-mcp-server    │ Clinical │ YES │ v2.1    │ Clin.   │
│ lab-mcp-server     │ Clinical │ YES │ v1.3    │ Clin.   │
│ claims-mcp-server  │ Billing  │ NO  │ v1.0    │ Billing │
│ elig-mcp-server    │ Billing  │ NO  │ v1.2    │ Billing │
│ search-mcp-server  │ Platform │ NO  │ v3.0    │ Platform│
└─────────────────────────────────────────────────────────┘

New server onboarding:
├── Team submits server for platform team review
├── Platform team checks: naming, auth, PHI classification, description quality
├── Approved → added to registry → agents can discover it
└── Rejected → feedback given → resubmit
```

---

## Section 6 — MCP Server Boundaries, Responsibilities, and Segregation

This directly maps to: "Define MCP Server boundaries, responsibilities, and segregation strategies within the enterprise hub."

### How to Define Server Boundaries — 3 Rules

**Rule 1: One Domain per Server (Single Responsibility)**
```
WRONG — one giant server:
└── hospital-mcp-server
    ├── get_patient_record    ← clinical
    ├── submit_claim          ← billing
    ├── search_documents      ← platform
    └── send_notification     ← communication
    Problem: one server down = everything down
    Problem: PHI and non-PHI tools mixed together
    Problem: different teams own different tools — conflicts

RIGHT — one server per domain:
├── fhir-mcp-server      → clinical patient data only
├── claims-mcp-server    → billing and claims only
├── search-mcp-server    → document search only
└── notify-mcp-server    → notifications only
```

**Rule 2: Segregate by PHI Sensitivity**
```
PHI boundary is the most critical in healthcare:

PHI Servers (strict controls):
├── fhir-mcp-server      → patient records
├── lab-mcp-server       → lab results
└── imaging-mcp-server   → radiology reports

Non-PHI Servers (standard controls):
├── claims-mcp-server    → claim status (no patient details)
├── formulary-mcp-server → drug policy (no patient data)
└── search-mcp-server    → document search (de-identified)

Enforcement:
├── PHI servers: dedicated Container Apps environment
├── PHI servers: dedicated Key Vault
├── PHI servers: RBAC role "PHI-Tool-Caller" required
└── Non-PHI servers: standard RBAC role "Tool-Caller"
```

**Rule 3: Segregate by Environment**
```
Same server, three instances:
├── fhir-mcp-server-DEV    → dev Foundry Hub
├── fhir-mcp-server-STG    → staging Foundry Hub
└── fhir-mcp-server-PROD   → prod Foundry Hub

Each instance:
├── Different credentials
├── Different Key Vault
├── Different FHIR endpoint (dev/staging/prod EHR)
└── Different rate limits (prod has higher quota)

Agents in dev NEVER call prod MCP servers.
Enforced by environment-specific Hub configurations.
```

### Segregation Strategies Summary

| Strategy | What you separate | Why |
|---|---|---|
| **Domain** | Clinical / Billing / Platform / Communication | Single responsibility, team ownership |
| **PHI sensitivity** | PHI tools vs non-PHI tools | HIPAA isolation, access control |
| **Environment** | Dev / Staging / Prod | No dev agent calls prod data |
| **Team ownership** | Each team owns their servers | Clear accountability |
| **Risk level** | Read-only vs write/destructive | Write tools get extra auth + audit |

### Read-Only vs Write Server Segregation

```
READ servers (lower risk):
├── fhir-read-mcp-server   → GET patient data
└── search-mcp-server      → search documents
    Controls: standard Managed Identity

WRITE/ACTION servers (higher risk):
├── fhir-write-mcp-server  → UPDATE EHR records
├── submit-mcp-server      → SUBMIT prior auth decisions
└── notify-mcp-server      → SEND physician alerts
    Controls:
    ├── Additional RBAC role required
    ├── HMAC signature on every call (A2A pattern)
    ├── Human-in-the-loop gate before execution
    └── Every write logged with agent identity + timestamp
```

### Healthcare Example — VitalCare MCP Server Map

```
VitalCare MCP Hub
│
├── PHI ZONE (isolated environment)
│   ├── fhir-read-mcp-server    → read patient records
│   ├── fhir-write-mcp-server   → update EHR (write-only, extra controls)
│   ├── lab-mcp-server          → lab results
│   └── imaging-mcp-server      → radiology reports
│
├── CLINICAL NON-PHI ZONE
│   ├── formulary-mcp-server    → drug policy lookup
│   ├── guidelines-mcp-server   → clinical guidelines RAG
│   └── icd-mcp-server          → ICD-10/CPT code lookup
│
├── OPERATIONAL ZONE
│   ├── claims-mcp-server       → claim status
│   ├── eligibility-mcp-server  → coverage check
│   └── payer-mcp-server        → payer API gateway
│
└── PLATFORM ZONE
    ├── search-mcp-server       → enterprise AI Search
    ├── notify-mcp-server       → Teams/email notifications
    └── audit-mcp-server        → write to audit log
```

---

## Section 7 — The Interview Answer

**The answer they want to hear:**

> "In production I use a hybrid MCP + APIM pattern. MCP Hub sits closest to the agent — it handles tool discovery, capability schemas, and context passing in a format the LLM natively understands. APIM sits in front of the backend APIs — it handles throttling, OAuth, versioning, and compliance logging. In a healthcare context, every tool call that touches PHI goes through APIM so we have a single auditable gateway for HIPAA. The agent never knows or cares about the backend complexity — it just asks the MCP Hub what tools exist, picks the right one, and calls it."

**Q: How do you govern a centralized pool of MCP Servers at enterprise scale?**
> "Governance has two layers: Hub-level policies that all servers must follow, and server-level boundaries that define what each server owns. At the Hub level I define six policies: tool naming standards so agents route correctly, tool description standards so LLMs pick the right tool, authentication policy enforcing Managed Identity for Azure services and Key Vault for external credentials, versioning policy with 60-day deprecation windows, rate limiting enforced by APIM, and PHI handling policy requiring isolated environments and dedicated Key Vaults for any server that touches patient data. Every new MCP server is reviewed by the platform team against these policies before it joins the registry. No unapproved server can be discovered by agents."

**Q: How do you define MCP Server boundaries and segregation strategies?**
> "Three rules. First, one domain per server — clinical, billing, platform, communication each get their own server. No mixed-domain servers because a single failure cascades and PHI controls become impossible to enforce. Second, segregate by PHI sensitivity — PHI tools run in an isolated Container Apps environment with a dedicated Key Vault and require an additional RBAC role to call. Non-PHI tools use standard controls. This enforces HIPAA minimum necessary access at the infrastructure level. Third, segregate read from write — read-only tools use standard Managed Identity, write tools require additional RBAC, HMAC signatures, and a human-in-the-loop gate before execution. In VitalCare I'd have four zones: PHI zone, clinical non-PHI zone, operational zone, and platform zone — each with its own security perimeter."

**Q: What is the decision criteria for MCP vs Azure APIM?**
> "They solve different problems so the decision is not either/or — it's where in the stack to use each. MCP is built for AI agents: it handles tool discovery, capability schemas in LLM-native format, and agent context. APIM is built for HTTP governance: throttling, OAuth, versioning, developer portal, analytics. When the caller is an AI agent that needs to discover and select tools intelligently — MCP. When the concern is enterprise API governance, rate limiting, and compliance logging — APIM. In production I layer them: MCP Hub handles the agent protocol, APIM sits in front of backends for enterprise governance. The integration pattern that drives the decision is whether the caller needs LLM-native tool discovery — if yes, MCP is required in the stack."
