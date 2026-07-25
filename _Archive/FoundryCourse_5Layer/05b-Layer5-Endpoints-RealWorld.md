# Layer 5: Endpoints — Real World Examples

> Companion to: [05 — Layer 5: Deployment & Monitoring](05-Layer5-Deployment-and-Monitoring.md)

---

## What an Endpoint Actually Looks Like

When you deploy in AI Foundry, you get a **URL + API Key**.
Your app calls it like any REST API — no magic, just HTTP.

---

## Type 1: Model Endpoint

Direct call to a deployed model — no flow, just the model.

### What it looks like in AI Foundry Portal

```
My assets → Models + endpoints → aiml-learn-resource

Name:     gpt-4o-deployment
Type:     Azure OpenAI
Endpoint: https://aiml-learn-resource.openai.azure.com/
Key:      3a8f2c••••••••••••••••••••••••e91b
```

### Real HTTP Request (what your C# app sends)

```http
POST https://aiml-learn-resource.openai.azure.com/openai/deployments/gpt-4o/chat/completions?api-version=2024-02-01
Content-Type: application/json
api-key: 3a8f2c••••••••e91b

{
  "messages": [
    {
      "role": "system",
      "content": "You are a Toyota vehicle advisor for JM Family."
    },
    {
      "role": "user",
      "content": "What SUVs do you have under $40,000?"
    }
  ],
  "max_tokens": 500,
  "temperature": 0.7
}
```

### Real HTTP Response (what comes back)

```json
{
  "id": "chatcmpl-9Xk2mN3pQ7rT",
  "object": "chat.completion",
  "created": 1718321847,
  "model": "gpt-4o-2024-05-13",
  "choices": [
    {
      "index": 0,
      "message": {
        "role": "assistant",
        "content": "We have 3 Toyota SUVs under $40,000: RAV4 ($29,995), Corolla Cross ($26,990), and C-HR ($24,500)."
      },
      "finish_reason": "stop"
    }
  ],
  "usage": {
    "prompt_tokens": 42,
    "completion_tokens": 38,
    "total_tokens": 80
  }
}
```

---

## Type 2: Prompt Flow Endpoint

Your entire AI workflow (RAG + prompt + safety) deployed as one endpoint.

### What it looks like in AI Foundry Portal

```
My assets → Models + endpoints → Prompt flow deployments

Name:        vehicle-chatbot-endpoint
Type:        Prompt Flow
Status:      ✅ Succeeded
Endpoint:    https://aiml-learn-resource.services.ai.azure.com/api/project
             /deployments/vehicle-chatbot-endpoint/chat
Key:         7c4d1a••••••••••••••••••••••••f23e
Scoring URI: https://aiml-learn-resource.services.ai.azure.com
             /api/project/deployments/vehicle-chatbot-endpoint/score
```

### Real HTTP Request

```http
POST https://aiml-learn-resource.services.ai.azure.com/api/project/deployments/vehicle-chatbot-endpoint/score
Content-Type: application/json
Authorization: Bearer 7c4d1a••••••••f23e

{
  "question": "What SUVs do you have under $40,000?",
  "chat_history": []
}
```

### Real HTTP Response

```json
{
  "answer": "Based on our current inventory, we have 3 Toyota SUVs under $40,000: RAV4 ($29,995) available in Midnight Black and Pearl White, Corolla Cross ($26,990) in 4 colors, and C-HR ($24,500). Would you like to schedule a test drive?",
  "context": [
    {
      "content": "RAV4 2024 - Base MSRP $29,995 - Colors: Midnight Black, Pearl White, Blueprint",
      "source": "toyota-inventory-2024.pdf",
      "score": 0.94
    },
    {
      "content": "Corolla Cross 2024 - Base MSRP $26,990",
      "source": "toyota-inventory-2024.pdf",
      "score": 0.89
    }
  ],
  "groundedness_score": 0.96,
  "latency_ms": 1842
}
```

> Notice the difference — Prompt Flow endpoint returns **context + scores + sources** too,
> not just the answer. This is RAG in action.

---

## Type 3: Embedding Endpoint

Converts text to vectors — used internally by RAG.

### Real HTTP Request

```http
POST https://aiml-learn-resource.openai.azure.com/openai/deployments/text-embedding-3-large/embeddings?api-version=2024-02-01
Content-Type: application/json
api-key: 3a8f2c••••••••e91b

{
  "input": "SUV under 40000 dollars family car"
}
```

### Real HTTP Response

```json
{
  "object": "list",
  "data": [
    {
      "object": "embedding",
      "index": 0,
      "embedding": [
        -0.0023064255,
         0.0061155798,
        -0.0064517975,
         0.0048785757,
        -0.0027444670,
        ... (3072 numbers total)
      ]
    }
  ],
  "model": "text-embedding-3-large",
  "usage": {
    "prompt_tokens": 8,
    "total_tokens": 8
  }
}
```

> That array of 3072 numbers IS the vector — it captures the **meaning** of your text
> mathematically so AI Search can find similar content.

---

## Side-by-Side Comparison

```
┌─────────────────┬──────────────────────────────┬────────────────────────┐
│                 │  Model Endpoint               │  Prompt Flow Endpoint  │
├─────────────────┼──────────────────────────────┼────────────────────────┤
│ What it runs    │  Just the model              │  Full workflow          │
│ Input           │  messages[]                  │  question + history     │
│ Output          │  answer only                 │  answer + context +     │
│                 │                              │  scores + sources       │
│ RAG included    │  No                          │  Yes                    │
│ Safety included │  No                          │  Yes                    │
│ Who calls it    │  Your app directly            │  Your app              │
│ JMA example     │  Quick one-off query         │  Full chatbot           │
└─────────────────┴──────────────────────────────┴────────────────────────┘
```

---

## How Your C# App Calls It (Semantic Kernel)

```csharp
// Model Endpoint — direct call via SK
var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: "gpt-4o",
        endpoint: "https://aiml-learn-resource.openai.azure.com/",
        apiKey: "3a8f2c••••••••e91b"
    )
    .Build();

var response = await kernel.InvokePromptAsync(
    "What SUVs do you have under $40,000?"
);

// Prompt Flow Endpoint — call via HttpClient
var client = new HttpClient();
client.DefaultRequestHeaders.Add("Authorization", "Bearer 7c4d1a••••f23e");

var payload = new { question = "What SUVs under $40k?", chat_history = new[] {} };
var result = await client.PostAsJsonAsync(
    "https://aiml-learn-resource.services.ai.azure.com/api/project/deployments/vehicle-chatbot-endpoint/score",
    payload
);
```

---

## Real-Time vs Batch — What the Request Looks Like

```
REAL-TIME (one question, instant answer):
  POST /score
  { "question": "What SUVs under $40k?" }
  → Response in ~1.5 seconds

BATCH (thousands of questions, process overnight):
  POST /jobs
  {
    "input_data": {
      "path": "azureml://datastores/inventory/paths/questions.csv"
    }
  }
  → Job ID returned immediately
  → Check status: GET /jobs/{job-id}
  → Results file ready in blob storage 2 hours later
```

---

## End-to-End Picture — All 3 Endpoints Together

```
Customer types: "I need a family SUV under $40k"
        │
        ▼
Your Angular App
        │
        ▼ calls Prompt Flow Endpoint
https://aiml-learn-resource.services.ai.azure.com/.../score
        │
        │  internally Prompt Flow calls:
        ├──► Embedding Endpoint   → converts question to vector
        │    https://aiml-learn-resource.openai.azure.com/.../embeddings
        │
        ├──► Azure AI Search      → finds matching inventory chunks
        │
        └──► Model Endpoint       → GPT-4o generates response
             https://aiml-learn-resource.openai.azure.com/.../chat/completions
        │
        ▼
Response back to Angular App with answer + sources + scores
```

---

## One-Line Summary

> A **Model Endpoint** = direct line to the AI brain.
> A **Prompt Flow Endpoint** = the full AI pipeline (RAG + safety + scoring) behind one URL.
> Your app just calls a REST API — it doesn't care what's behind it.

---

## Navigation

| | |
|---|---|
| **Previous** | [05 — Layer 5: Deployment & Monitoring](05-Layer5-Deployment-and-Monitoring.md) |
| **Next** | `06-AI-Agents-Deep-Dive.md` *(coming soon)* |
