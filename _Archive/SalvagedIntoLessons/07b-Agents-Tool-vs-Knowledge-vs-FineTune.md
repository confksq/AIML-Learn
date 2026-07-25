# Agents: Tool vs Knowledge (RAG) vs Fine-Tune — When to Use What

> Companion to: [07 — AI Agents JMA Real World](07-AI-Agents-JMA-RealWorld.md)

---

## The Confusion — Why All Three Exist

```
Data changes frequently?

Fine-tuning said:  "Don't fine-tune, use RAG"
RAG said:          "Use RAG for your data"
Tools said:        "Use Tools for live data"

So which one??
```

---

## The Key — HOW FAST Does It Change?

```
SPEED OF CHANGE decides everything:

Changes every few MONTHS/YEARS    →  Fine-tune
 └── Model behavior, tone, style
     "Always respond like a Toyota advisor"

Changes every few DAYS/WEEKS      →  RAG (Knowledge)
 └── Documents, specs, policies
     "RAV4 2024 specs PDF"
     "Updated warranty terms doc"

Changes every MINUTE/HOUR/SECOND  →  Tool (API)
 └── Live transactional data
     "Current inventory count"
     "Today's interest rate"
     "Available slots right now"
```

---

## Visual Timeline

```
←──────────────────────────────────────────────────►
NEVER          MONTHS        DAYS/WEEKS    SECONDS
changes        changes        changes      changes

Fine-tune      Fine-tune      RAG          Tool
   │               │           │            │
Model tone     Domain         Specs        Live
Personality    knowledge      Policies     inventory
Style          Terminology    Manuals      Pricing
                                           Slots
```

---

## JMA Applied — All Three Together

```
FINE-TUNE (once, behavior):
 └── "Always be professional"
 └── "Always upsell extended warranty"
 └── "Always follow JMA brand voice"
      ↑ trained ONCE, baked into model

RAG / KNOWLEDGE (updated occasionally):
 └── RAV4 2024 specs PDF        → updated when new model year
 └── Warranty terms doc         → updated when policy changes
 └── Trim comparison guide      → updated seasonally
      ↑ re-index when doc changes

TOOL / API (live, every call):
 └── Current inventory count    → changes every hour
 └── Today's APR rate           → changes weekly
 └── Available test drive slots → changes every minute
      ↑ called fresh every single time
```

---

## Super Simple Rule

| Question | Answer |
|---|---|
| How should it **behave**? | Fine-tune |
| What should it **know**? | RAG |
| What is happening **right now**? | Tool |

---

## Can RAG Be Updated Every Second?

Technically yes — but it is not designed for that.

### What Happens When You Update RAG

```
New document / data arrives
        │
        ▼
Step 1: Chunk the document        ← split into pieces
        │                            takes time
        ▼
Step 2: Embed each chunk          ← convert to vectors
        │                            costs money per token
        ▼
Step 3: Index into AI Search      ← store in vector DB
        │                            takes time
        ▼
Step 4: Available to Agent        ← now searchable

Total time: seconds to minutes per update
```

### RAG vs Tool — Speed Comparison

```
┌─────────────────────────────────────────────────┐
│  Tool (API call)                                │
│  └── Fresh data every call                      │
│  └── Response: milliseconds                     │
│  └── Cost: one API call                         │
│  └── No indexing needed                         │
├─────────────────────────────────────────────────┤
│  RAG (document index)                           │
│  └── Fresh data after re-indexing               │
│  └── Re-index time: seconds to minutes          │
│  └── Cost: embedding tokens per chunk           │
│  └── Indexing pipeline needed                   │
└─────────────────────────────────────────────────┘
```

### The Problem With RAG Every Second

```
Inventory updates every second:
  Car #1 sells at 10:00:01 AM
        │
        ▼
  RAG re-index triggered
  Chunking... embedding... indexing...
        │
        ▼
  Takes 30 seconds to complete
        │
        ▼
  Car #2 sells at 10:00:05 AM
        │
        ▼
  RAG re-index triggered AGAIN
        │
        ▼
  Backlog builds up ❌
  Data always stale ❌
  Costs skyrocket   ❌
  Agent gets confused ❌
```

### When RAG Update Frequency Makes Sense

```
Update RAG:               ✅ Makes sense?
────────────────────────────────────────
Every few months          ✅ Perfect
Every few weeks           ✅ Great
Every few days            ✅ Fine
Every few hours           ⚠️  Possible but watch costs
Every few minutes         ⚠️  Possible but heavy pipeline
Every few seconds         ❌ Use a Tool instead
Real-time / live          ❌ Definitely use a Tool
```

### The Right Approach for Live Inventory

```
❌ Wrong:
   Inventory DB → re-index RAG every second → Agent searches RAG

✅ Right:
   Inventory DB → REST API → Agent calls Tool → gets live count
```

---

## JMA Full Example — All Three in Action

```
Agent: JMA Dealer Support Agent
│
├── FINE-TUNE (behavior — trained once)
│    └── JMA brand voice, professional tone
│    └── Always upsell warranty
│    └── Toyota domain terminology
│
├── KNOWLEDGE / RAG (reference — updated occasionally)
│    ├── RAV4-2024-specs.pdf         → new model year
│    ├── toyota-warranty-guide.pdf   → policy changes
│    ├── trim-comparison.pdf         → seasonal updates
│    └── jma-dealer-policies.pdf     → when rules change
│
└── TOOLS (live — called every request)
     ├── get_trade_in_value()        → live pricing API
     ├── search_inventory()          → live inventory DB
     ├── calculate_payment()         → live rate calculator
     └── book_test_drive()           → live scheduling API
```

---

## Decision Tree

```
Is the data LIVE / real-time?
 ├── YES → TOOL (API call)
 └── NO  → Is it a calculation or action?
            ├── YES → TOOL (function call)
            └── NO  → Is it static reference text?
                       ├── YES → KNOWLEDGE (RAG)
                       └── NO  → Is it about HOW to behave?
                                  ├── YES → FINE-TUNE
                                  └── NO  → System prompt
```

---

## Quick Reference Table

| Data Type | Example | Solution | Why |
|---|---|---|---|
| Live inventory | Cars in stock right now | Tool | Changes every hour |
| Live pricing | Today's APR rate | Tool | Changes weekly |
| Live slots | Test drive availability | Tool | Changes every minute |
| Vehicle specs | RAV4 engine, mpg, features | RAG | Changes yearly |
| Warranty terms | Coverage details | RAG | Changes occasionally |
| Dealer policies | Trade-in rules | RAG | Changes rarely |
| Brand tone | Professional, friendly | Fine-tune | Trained once |
| Domain language | Toyota terminology | Fine-tune | Trained once |
| Response style | Always upsell warranty | Fine-tune | Trained once |

---

## One-Line Summary

> **Fine-tune** shapes HOW the model talks. **RAG** gives it WHAT to read.
> **Tool** gives it WHAT IS HAPPENING RIGHT NOW.
> Speed of change is the deciding factor — seconds → Tool, days → RAG, permanent → Fine-tune.

---

## Navigation

| | |
|---|---|
| **Previous** | [07 — AI Agents JMA Real World](07-AI-Agents-JMA-RealWorld.md) |
| **Next** | `08-AI-Agents-MultiAgent.md` *(coming soon)* |
