# AI Agents — JMA Real World Example
## Dealer Support Agent — Full End-to-End

---

## The Scenario

```
JMA Southeast Toyota Dealer — Fort Lauderdale, FL

A customer calls the dealership:
"Hi, my name is Sarah Johnson. I have a 2021 Toyota Camry 
I want to trade in. I'm looking for a RAV4 hybrid. 
My budget is around $45,000. Can you help me figure out 
what my monthly payments would be and when I can 
come in for a test drive?"

The dealer rep types this into the JMA Dealer Portal
powered by the AI Agent.
```

---

## What the Agent Does — Step by Step

```
GOAL: Help dealer answer Sarah's request completely

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
THINK: I need 4 things:
       1. Trade-in value for 2021 Camry
       2. RAV4 Hybrid inventory availability
       3. Monthly payment calculation
       4. Test drive availability
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

STEP 1:
THINK:  Get trade-in value first
ACT:    → calls TradeIn Tool
        {
          "make": "Toyota",
          "model": "Camry",
          "year": 2021,
          "zip": "33301"
        }
OBSERVE:{
          "trade_in_value": "$18,500",
          "condition": "Good",
          "market_demand": "High"
        }

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
STEP 2:
THINK:  Check RAV4 Hybrid inventory
ACT:    → calls Inventory Search Tool
        {
          "model": "RAV4 Hybrid",
          "max_price": 45000,
          "dealer": "Fort Lauderdale"
        }
OBSERVE:{
          "results": [
            {
              "vin": "JTMRWRFV4PD123456",
              "model": "RAV4 Hybrid XLE",
              "color": "Midnight Black",
              "msrp": 42500,
              "mpg": "41 city / 38 hwy",
              "status": "In Stock"
            },
            {
              "vin": "JTMRWRFV4PD789012",
              "model": "RAV4 Hybrid XLE Premium",
              "color": "Blueprint",
              "msrp": 44800,
              "mpg": "41 city / 38 hwy",
              "status": "In Stock"
            }
          ]
        }

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
STEP 3:
THINK:  Calculate monthly payments
        Trade-in = $18,500
        RAV4 XLE = $42,500
        Net price = $24,000
ACT:    → calls Finance Calculator Tool
        {
          "vehicle_price": 42500,
          "trade_in": 18500,
          "down_payment": 0,
          "credit_score_range": "good",
          "term_months": 60
        }
OBSERVE:{
          "monthly_payment": "$487",
          "apr": "4.9%",
          "term": "60 months",
          "total_cost": "$29,220"
        }

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
STEP 4:
THINK:  Check test drive availability
ACT:    → calls Scheduling Tool
        {
          "dealer": "Fort Lauderdale",
          "vehicle_vin": "JTMRWRFV4PD123456",
          "available_days": 7
        }
OBSERVE:{
          "slots": [
            "2026-06-15 Mon 10:00 AM",
            "2026-06-15 Mon 2:00 PM",
            "2026-06-16 Tue 11:00 AM"
          ]
        }

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
THINK:  I have everything. Compose final response.
STOP
```

---

## What the Dealer Sees on Screen

```
┌─────────────────────────────────────────────────────────┐
│  JMA Dealer Portal — AI Agent Response                  │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  Hi! Here's everything for Sarah Johnson:               │
│                                                         │
│  🚗 TRADE-IN — 2021 Toyota Camry                        │
│     Estimated Value: $18,500 (Good condition)           │
│     Market demand is HIGH — great time to trade!        │
│                                                         │
│  🚙 MATCHING INVENTORY — RAV4 Hybrid (2 available)      │
│     Option 1: XLE — Midnight Black  — $42,500           │
│     Option 2: XLE Premium — Blueprint — $44,800         │
│     Both get 41 MPG city / 38 MPG highway               │
│                                                         │
│  💰 MONTHLY PAYMENT (Option 1 with trade-in)            │
│     Net after trade: $24,000                            │
│     Monthly: $487/mo at 4.9% APR for 60 months         │
│                                                         │
│  📅 TEST DRIVE AVAILABILITY                             │
│     Mon Jun 15 — 10:00 AM  ← soonest                   │
│     Mon Jun 15 —  2:00 PM                               │
│     Tue Jun 16 — 11:00 AM                               │
│                                                         │
│  [Book Mon 10AM]  [Book Mon 2PM]  [Book Tue 11AM]       │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## The Agent Setup Behind the Scene

```
Agent Name:    JMA Dealer Support Agent
Brain:         GPT-4o
Deployed on:   AI Foundry → Build & customize → Agents

Instructions (System Prompt):
  "You are a JMA Southeast Toyota dealer support agent.
   When a dealer submits a customer request:
   1. Always check trade-in value if customer has a vehicle
   2. Always search current inventory matching customer needs
   3. Always calculate financing with trade-in applied
   4. Always show next 3 available test drive slots
   Never make up inventory or pricing — always use tools."

Tools Connected:
 ├── TradeIn Tool        → JMA pricing API
 ├── Inventory Search    → Azure AI Search (toyota-inventory-index)
 ├── Finance Calculator  → JMA finance API
 └── Scheduling Tool     → JMA appointment API

Knowledge:
 └── RAG Index           → Toyota specs, features, comparison docs
```

---

## Without Agent vs With Agent

```
WITHOUT AGENT (today):               WITH AGENT:
─────────────────────                ───────────
Dealer opens 4 tabs:                 Dealer types one sentence
 Tab 1: Trade-in tool                Agent does everything
 Tab 2: Inventory system             Response in 8 seconds
 Tab 3: Finance calculator           One screen, complete answer
 Tab 4: Scheduling system            Dealer just clicks Book

Takes: 10-15 minutes                 Takes: 8 seconds
Risk: manual errors                  Risk: near zero
```

---

## Where This Lives in Our 5 Layers

```
Layer 2 │ Hub: hub-jma-dev-ai
        │ Project: proj-dealer-support-agent
        │ Connections: JMA APIs, Azure AI Search
─────────────────────────────────────────────
Layer 3 │ Model: GPT-4o (serverless)
        │ Embeddings: text-embedding-3-large
─────────────────────────────────────────────
Layer 4 │ Agent: JMA Dealer Support Agent
        │ Tools: TradeIn, Inventory, Finance, Schedule
        │ RAG: toyota-inventory-index
        │ Content Safety: ON
─────────────────────────────────────────────
Layer 5 │ Endpoint: dealer-agent-endpoint
        │ Called by: JMA Dealer Portal (Angular app)
        │ Monitored: Tracing ON, Quality alerts ON
```

---

## The 4 Core Agent Components — Applied to JMA

```
┌─────────────────────────────────────────────────┐
│          JMA DEALER SUPPORT AGENT               │
│                                                 │
│  ┌─────────┐   ┌─────────────────────────────┐  │
│  │  Brain  │   │  Tools                      │  │
│  │ GPT-4o  │   │  ├── TradeIn API            │  │
│  │         │   │  ├── Inventory Search       │  │
│  │ decides │   │  ├── Finance Calculator     │  │
│  │ what    │   │  └── Scheduling API         │  │
│  │ to do   │   └─────────────────────────────┘  │
│  │ next    │                                     │
│  └─────────┘   ┌─────────────────────────────┐  │
│                │  Memory                     │  │
│                │  Short: Sarah's session     │  │
│                │  Long:  Past interactions   │  │
│                └─────────────────────────────┘  │
│                                                 │
│  Planning Loop: Think → Act → Observe → Think   │
└─────────────────────────────────────────────────┘
```

---

## One-Line Summary

> One dealer message → Agent calls 4 JMA APIs in the right order
> → delivers a complete, accurate answer in seconds
> — no tabs, no manual lookups, no errors.

---

## Navigation

| | |
|---|---|
| **Previous** | [06 — Course Progress Recap](06-Course-Progress-Recap.md) |
| **Next** | `08-AI-Agents-MultiAgent.md` *(coming soon)* |
