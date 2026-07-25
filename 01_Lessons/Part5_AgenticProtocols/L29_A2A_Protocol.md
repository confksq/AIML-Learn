# Module 08 — A2A Protocol: Agent-to-Agent Communication

> **⚙️ Config or Code? — This Module**
> - **Portal Config only:** Azure Service Bus namespace + queue creation (portal), dead-letter queue settings (portal), Managed Identity assignment per agent (portal), Key Vault access policies for HMAC signing key (portal)
> - **Custom Code:** `AgentMessage` envelope class (C#), `ClinicalAgentBus` implementation (schema validation, HMAC verification, audit logging, routing), HMAC signing + verification (`System.Security.Cryptography`), dead-letter handler (replay worker), all A2A communication is code — there is no portal UI for the protocol itself
> - **Both:** HMAC signing key (store in Key Vault = portal; retrieve and use in code = SDK); Audit store (Cosmos DB = portal setup; write audit entries = SDK code)

---

## Why This Module Matters

The job description explicitly calls out "Implement A2A (Agent-to-Agent) Protocol standards for secure, structured inter-agent communication." This is a 2025 standard — most candidates won't know it. If you can speak to it fluently, you immediately stand out. You will be asked:
- "What is A2A and why does it exist?"
- "How does an agent authenticate to another agent?"
- "How do you prevent an agent from accepting a forged message?"

Your anchor: In the VitalCare platform, the Supervisor communicates with all specialists via a `ClinicalAgentBus` — typed message envelopes with MessageId, CorrelationId, and SchemaVersion on every call.

---

## Section 1 — What A2A IS and Why It Exists

Before A2A, if Agent A needed to call Agent B, you'd wire them together with a custom HTTP call, a shared queue message, or a direct method invocation. Every pair of agents had its own custom contract — its own format, its own auth, its own error handling. That works for two agents. It breaks at twenty.

**A2A (Agent-to-Agent Protocol)** is an open standard (initiated by Google in 2025, now multi-vendor) that defines:
- How agents **discover** each other's capabilities
- How agents **send tasks** to each other
- How agents **report progress and results** back
- How authentication works between agents
- How errors and failures propagate

**The mental model:** Think of A2A like **FHIR for agents**. FHIR standardized how hospital systems exchange patient records — before it, every integration was custom. A2A standardizes how AI agents exchange tasks — before it, every agent-to-agent call was custom.

Just as a hospital system doesn't need to know whether the lab system is Epic or Cerner (they both speak FHIR), an agent doesn't need to know whether the specialist it's calling is built on Semantic Kernel or LangGraph (they both speak A2A).

---

## Section 2 — The A2A Message Envelope

Every message in the A2A protocol is a structured envelope. In the VitalCare implementation:

```csharp
public class AgentMessage
{
    public string MessageId { get; set; }       // unique ID for this message
    public string CorrelationId { get; set; }   // ties together all messages in one workflow
    public string SchemaVersion { get; set; }   // "1.0" — sender and receiver must agree
    public string SenderId { get; set; }        // which agent sent this
    public string ReceiverId { get; set; }      // which agent this is addressed to
    public string TaskType { get; set; }        // "ValidateClaim" / "CheckPolicy" / "DetectFraud"
    public string PayloadJson { get; set; }     // the actual task payload — typed per TaskType
    public DateTime Timestamp { get; set; }
    public string HmacSignature { get; set; }   // message integrity — receiver verifies this
}
```

**Why each field matters:**

| Field | Why it exists |
|-------|--------------|
| `MessageId` | Idempotency — if the message is delivered twice, the receiver ignores the duplicate |
| `CorrelationId` | End-to-end tracing — every message in a prior auth workflow shares the same CorrelationId |
| `SchemaVersion` | Versioning — when you update the protocol, old agents don't silently break |
| `HmacSignature` | Integrity — a rogue agent cannot forge a message from the Supervisor |

---

## Section 3 — The Agent Bus: What Validates and Routes

Agents don't call each other directly. They communicate through an **AgentBus** — the message broker that enforces the A2A contract.

```
Supervisor Agent
      ↓  publishes AgentMessage
[ClinicalAgentBus]
  1. Validate schema (SchemaVersion matches, all required fields present)
  2. Verify HMAC signature (message not tampered)
  3. Log to audit store (HIPAA — every inter-agent message is recorded)
  4. Route to the correct specialist based on ReceiverId
  5. On failure → dead-letter queue (message preserved, alert fired)
      ↓
Specialist Agent receives validated message
```

**Why the Bus matters:**
Without a Bus, every agent must implement its own schema validation, signature verification, and audit logging — that's 20 copies of the same code. The Bus centralizes it. All agents are automatically HIPAA-compliant on inter-agent calls.

---

## Section 4 — Authentication Between Agents (the question they probe)

**The trap:** candidates assume inter-agent calls are trusted because they're internal. That's wrong in any regulated environment.

In the VitalCare platform, each agent has its own **Managed Identity** in Azure. When the Supervisor calls the PolicyChecker via the AgentBus:

1. Supervisor's Managed Identity fetches a short-lived token from Azure AD
2. Token is attached to the AgentMessage (not stored, not logged)
3. AgentBus verifies the token before routing
4. PolicyChecker only accepts messages where the `SenderId` matches a whitelisted agent identity

**Why this matters for PHI:**
If the FraudDetector's identity is compromised and begins sending forged messages to the PolicyChecker to suppress fraud flags — the HMAC signature fails, the AgentBus dead-letters the message, and an alert fires. A forged message never reaches a specialist.

```csharp
// ClinicalAgentBus.cs — core validation
public async Task<bool> ValidateAndRouteAsync(AgentMessage message)
{
    // 1. Schema check
    if (message.SchemaVersion != _expectedVersion)
        throw new SchemaMismatchException(message.MessageId);

    // 2. HMAC verification
    var expectedHmac = ComputeHmac(message, _signingKey);
    if (!CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(message.HmacSignature),
            Encoding.UTF8.GetBytes(expectedHmac)))
        throw new MessageIntegrityException(message.MessageId);

    // 3. Audit log (MessageId, CorrelationId, SenderId, ReceiverId, TaskType — no PHI)
    await _auditStore.LogAsync(message.MessageId, message.CorrelationId,
        message.SenderId, message.ReceiverId, message.TaskType);

    // 4. Route
    await _router.RouteAsync(message);
    return true;
}
```

---

## Section 5 — Dead-Letter Queue: What Happens on Failure

**The dead-letter queue is your safety net.** When the AgentBus cannot deliver a message (specialist unavailable, schema mismatch, signature failure, specialist crashes mid-processing):

1. Message goes to the **dead-letter queue** (Azure Service Bus dead-letter)
2. Alert fires to the on-call engineer
3. Message is preserved with full envelope intact — nothing is lost
4. Downstream systems are notified: prior auth is PENDED, not silently dropped

**The interview answer on failure:**
"In a PHI system, we follow the principle of 'preserve and surface.' A failed inter-agent message goes to a dead-letter queue with its full CorrelationId and audit trail. The workflow is pended — the patient's prior auth request is not abandoned, it's routed to human review with the failure reason attached. The on-call engineer can inspect the dead-letter, fix the issue, and replay the message. Nothing disappears."

---

## Section 6 — A2A vs Direct Method Calls vs MCP

Candidates sometimes confuse A2A with direct method calls or MCP. Clarify the distinction:

| Pattern | What it is | When to use |
|---------|-----------|------------|
| **Direct method call** | Agent A calls Agent B's method in-process | Only if agents share the same process — no fault isolation |
| **MCP** | Agent calls an external **tool** (API, database, search index) via a standard protocol | Agent ↔ Tool communication |
| **A2A** | Agent calls another **agent** — the receiver has its own LLM reasoning loop | Agent ↔ Agent communication |

MCP is horizontal (agent to tool). A2A is vertical (agent to agent). They coexist in the same platform:

```
Supervisor Agent
   ↓ A2A (to specialist agent)
PolicyChecker Agent
   ↓ MCP (to external tool)
Payer Eligibility API
```

---

## Section 7 — JM Family Anchor

"At JM Family, my current system uses direct Semantic Kernel method calls between the orchestrator and validators — they run in the same process, which is fine for our scale. If I were building the VitalCare 180-hospital platform, I'd add the AgentBus layer because you now have agents deployed across different services, potentially on different clusters. Direct method calls don't survive service boundaries, don't give you an audit trail, and don't handle partial failures gracefully. The AgentBus pattern is what scales that."

---

## Quick-Reference Interview Answers

**Q: What is A2A Protocol and why does it exist?**
"A2A is a 2025 open standard for agent-to-agent communication — think FHIR but for AI agents. Before it, every agent-to-agent call was a custom HTTP call with custom auth and custom error handling. A2A standardizes the message envelope, discovery, auth, and failure handling so any compliant agent can communicate with any other — regardless of which framework built it."

**Q: How do you prevent a rogue agent from forging messages to another agent?**
"Every message carries an HMAC signature computed with a key the sender holds. The AgentBus verifies the signature before routing. Even if the message payload is intercepted and modified in transit, the HMAC fails and the message is dead-lettered. Each agent also has its own Managed Identity — only whitelisted sender identities are accepted by each specialist. Two layers: message integrity via HMAC, sender identity via Managed Identity tokens."

**Q: What happens to a message that can't be delivered?**
"It goes to a dead-letter queue — Azure Service Bus dead-letter in our implementation. The message is preserved with its full envelope: MessageId, CorrelationId, audit trail. An alert fires. The workflow is pended, not dropped. The on-call engineer can diagnose from the dead-letter, fix the root cause, and replay the message. In a PHI system, silent message loss is never acceptable — every failure must produce an auditable, actionable outcome."
