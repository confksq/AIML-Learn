# Multi-Agent — When One Agent Is Not Enough

> Companion to: [07 — AI Agents JMA Real World](07-AI-Agents-JMA-RealWorld.md)

---

## Why Multi-Agent?

```
Single Agent Problem:
  One agent trying to do EVERYTHING
   ├── Gets confused with too many tools
   ├── Context window fills up fast
   ├── Can't specialize — jack of all trades
   └── One failure = entire task fails
```

---

## What is Multi-Agent?

```
Instead of ONE agent doing everything:

                 ORCHESTRATOR AGENT
                 (the manager/coordinator)
                  │
       ┌──────────┼──────────┐
       ▼          ▼          ▼
  Research    Writer      Validator
  Agent       Agent       Agent
  (specialist) (specialist) (specialist)
```

> Think of it as a **team of employees** vs **one person doing every job**.

---

## The Two Roles

```
ORCHESTRATOR AGENT              SPECIALIST AGENT
──────────────────              ────────────────
Receives the goal               Receives a specific sub-task
Breaks it into sub-tasks        Executes ONE thing very well
Assigns to specialists          Returns result to orchestrator
Collects results                Has its own tools & knowledge
Composes final answer           Focused, smaller context window
Like: a Project Manager         Like: a Developer / Designer
```

---

## JMA Multi-Agent Example
### Vehicle Purchase Journey Agent System

```
Customer: "I want to buy a RAV4 Hybrid, trade in my Camry,
           get financing, add accessories, and have it
           delivered to my home in Miami by next Friday."

This is TOO COMPLEX for one agent.
Multi-Agent handles it:
```

```
┌─────────────────────────────────────────────────────────┐
│           ORCHESTRATOR AGENT                            │
│           "JMA Purchase Journey Manager"                │
│                                                         │
│  Receives full customer request                         │
│  Breaks into 5 parallel tasks                           │
│  Coordinates all specialists                            │
│  Composes final response                                │
└──────┬──────────┬──────────┬──────────┬────────────────┘
       │          │          │          │
       ▼          ▼          ▼          ▼
┌──────────┐ ┌─────────┐ ┌────────┐ ┌──────────────────┐
│ Trade-in │ │Inventory│ │Finance │ │  Delivery        │
│  Agent   │ │  Agent  │ │ Agent  │ │  Agent           │
│          │ │         │ │        │ │                  │
│ Checks   │ │ Finds   │ │Calcul- │ │ Checks Miami     │
│ Camry    │ │ RAV4    │ │ates    │ │ delivery routes  │
│ value    │ │ Hybrid  │ │monthly │ │ Confirms Friday  │
│          │ │ stock   │ │payment │ │ slot available   │
└──────────┘ └─────────┘ └────────┘ └──────────────────┘
       │          │          │                │
       └──────────┴──────────┴────────────────┘
                             │
                             ▼
              ┌──────────────────────────┐
              │  Accessories Agent       │
              │                          │
              │  Suggests add-ons based  │
              │  on RAV4 + Miami climate │
              │  (tint, all-weather mats)│
              └──────────────────────────┘
                             │
                             ▼
              ┌──────────────────────────┐
              │  ORCHESTRATOR composes   │
              │  complete purchase plan  │
              │  for customer            │
              └──────────────────────────┘
```

---

## What the Customer Sees

```
┌─────────────────────────────────────────────────────────┐
│  JMA Complete Purchase Plan                             │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  🚗 TRADE-IN: 2021 Camry → $18,500                      │
│                                                         │
│  🚙 YOUR CAR: RAV4 Hybrid XLE — Midnight Black          │
│     In stock at Fort Lauderdale — VIN: JTMRW...         │
│                                                         │
│  💰 FINANCING: $487/mo at 4.9% for 60 months            │
│     Net after trade-in: $24,000                         │
│                                                         │
│  🎨 RECOMMENDED ACCESSORIES (Miami climate):            │
│     • Ceramic window tint          $450                 │
│     • All-weather floor mats       $189                 │
│     • Paint protection film        $899                 │
│                                                         │
│  🚚 DELIVERY: Friday Jun 19 between 2PM–5PM             │
│     To: Miami, FL — Driver assigned                     │
│                                                         │
│  Total out-of-door: $26,038 financed                    │
│  [Confirm Purchase]  [Modify]  [Call Dealer]            │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## How Agents Communicate

```
Orchestrator → Specialist:
{
  "task": "check_trade_in",
  "data": {
    "make": "Toyota",
    "model": "Camry",
    "year": 2021,
    "zip": "33101"
  }
}

Specialist → Orchestrator:
{
  "result": "success",
  "trade_in_value": 18500,
  "condition": "Good",
  "market_demand": "High"
}
```

---

## Parallel vs Sequential Execution

```
PARALLEL (faster — run at same time):
  Trade-in Agent    ──────────────────► done in 1.2s
  Inventory Agent   ──────────────────► done in 0.9s
  Finance Agent     ──────────────────► done in 0.5s
  Delivery Agent    ──────────────────► done in 1.1s
                                        ──────────────
  Total wait:                           1.2s (longest)

SEQUENTIAL (slower — one after another):
  Trade-in Agent    ► 1.2s
  Inventory Agent             ► 0.9s
  Finance Agent                         ► 0.5s
  Delivery Agent                                  ► 1.1s
                                        ──────────────
  Total wait:                           3.7s

Multi-Agent runs in PARALLEL → 3x faster ✅
```

---

## Agent vs Multi-Agent — Full Comparison

```
┌──────────────────────┬────────────────────┬──────────────────────────────┐
│                      │  Single Agent      │  Multi-Agent                 │
├──────────────────────┼────────────────────┼──────────────────────────────┤
│ Structure            │ One agent          │ Orchestrator + Specialists   │
│ Task complexity      │ Simple / focused   │ Complex / multi-step         │
│ Specialization       │ Generalist         │ Each agent is an expert      │
│ Speed                │ Sequential         │ Parallel — much faster       │
│ Context window       │ Fills up fast      │ Each agent has own window    │
│ Failure impact       │ All or nothing     │ One fails, others continue   │
│ Cost                 │ Lower              │ Higher (multiple LLM calls)  │
│ Maintenance          │ Simple             │ More complex to manage       │
│ Best for             │ Focused tasks      │ End-to-end journeys          │
│ JMA example          │ Trade-in check     │ Full purchase journey        │
└──────────────────────┴────────────────────┴──────────────────────────────┘
```

---

## What If Multi-Agent Didn't Exist?

```
WITHOUT Multi-Agent — Single Agent tries everything:

Customer: "Buy RAV4, trade Camry, finance, accessories, deliver Friday"
        │
        ▼
Single Agent:
  ├── Context window: fills up with all tool results
  ├── Too many tools: gets confused which to call
  ├── Sequential only: takes 3x longer
  ├── One API fails: entire conversation breaks
  └── No specialization: mediocre at everything

Result:
  ❌ Slower responses
  ❌ Higher error rate
  ❌ Context overflow on complex tasks
  ❌ No fault tolerance
  ❌ Poor quality answers
  ❌ Dealer has bad experience
  ❌ Customer walks away
```

---

## When to Use Which

```
Use SINGLE AGENT when:            Use MULTI-AGENT when:
──────────────────────            ─────────────────────
One clear focused task            Multiple parallel tasks
Few tools needed (2-4)            Many tools needed (5+)
Short conversation                Long complex journey
Quick lookup                      End-to-end workflow
Trade-in check only               Full purchase journey
Inventory search only             Research + Write + Validate
```

---

## Where Multi-Agent Lives in AI Foundry

```
AI Foundry Portal
 └── Build and customize
      └── Agents
           └── Connected agents   ◄── Multi-Agent lives HERE
                ├── Create Orchestrator Agent
                ├── Create Specialist Agents
                └── Connect them via Agent-to-Agent calls
```

---

## One-Line Summary

> **Single Agent** = one smart employee handling a focused task.
> **Multi-Agent** = a coordinated team where a manager delegates to specialists
> running in parallel — faster, more accurate, fault-tolerant,
> built for complex end-to-end workflows.

---

## Navigation

| | |
|---|---|
| **Previous** | [07b — Tool vs Knowledge vs Fine-tune](07b-Agents-Tool-vs-Knowledge-vs-FineTune.md) |
| **Next** | `09-RAG-Deep-Dive.md` *(coming soon)* |
