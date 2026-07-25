# Semantic Kernel
## What It Is, Why It Exists, What You Can Build

---

## What Semantic Kernel Is

```
Semantic Kernel = a C# SDK (NuGet package) from Microsoft
                  for building AI-powered applications in code

NuGet: Microsoft.SemanticKernel

It is NOT:
  ❌ Only for AI agents
  ❌ Only for Azure AI Foundry
  ❌ A Microsoft-hosted service

It IS:
  ✅ A library you add to any .NET project
  ✅ Works with Azure OpenAI, OpenAI, HuggingFace, Ollama
  ✅ Deploys wherever .NET runs (App Service, Functions, Console)
```

---

## The Analogy to JMA Code

```
Azure SDK (Azure.Search.Documents NuGet):
  └── SearchClient.UploadDocumentsAsync(batch)
  └── SDK handles: HTTP, auth, retry, serialization
  └── You don't write HTTP POST to AI Search manually

Semantic Kernel (Microsoft.SemanticKernel NuGet):
  └── chatService.GetChatMessageContentAsync(history)
  └── SK handles: OpenAI API, tool routing, history, retry
  └── You don't write HTTP POST to OpenAI manually

Same concept — different layer.
SK is the Azure SDK equivalent, but for AI orchestration.
```

---

## Why It Exists — Real Scenario WITHOUT SK

**Scenario: JMA employee asks "Find contracts for dealer 4512 cancelled last month and summarise reasons"**

```csharp
// WITHOUT SK — you write ALL of this yourself:

// STEP 1: Call OpenAI API manually
var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
var requestBody = new { model = "gpt-4o", messages = [...], tools = [...] };
var response = await httpClient.PostAsJsonAsync(endpoint, requestBody);

// STEP 2: Parse response — check if model wants a tool call
var json = await response.Content.ReadAsStringAsync();
var parsed = JsonDocument.Parse(json);
var choice = parsed.RootElement.GetProperty("choices")[0].GetProperty("message");

if (choice.TryGetProperty("tool_calls", out var toolCalls))
{
    // STEP 3: Parse tool arguments the model returned
    var args = JsonDocument.Parse(toolCalls[0].GetProperty("function")
                                              .GetProperty("arguments").GetString());
    var dealerId = args.RootElement.GetProperty("dealerId").GetString();

    // STEP 4: Call your actual search code
    var contracts = await SearchContractsAsync(dealerId, month);

    // STEP 5: Build SECOND API call with tool result
    var secondRequestBody = new
    {
        model = "gpt-4o",
        messages = new[]
        {
            new { role = "system",    content = "You are a JMA assistant." },
            new { role = "user",      content = userQuestion },
            new { role = "assistant", tool_calls = toolCalls },
            new { role = "tool",      content = JsonSerializer.Serialize(contracts),
                  tool_call_id = toolCalls[0].GetProperty("id").GetString() }
        }
    };

    var secondResponse = await httpClient.PostAsJsonAsync(endpoint, secondRequestBody);

    // STEP 6: Parse final answer
    var finalAnswer = JsonDocument.Parse(await secondResponse.Content.ReadAsStringAsync())
        .RootElement.GetProperty("choices")[0]
        .GetProperty("message").GetProperty("content").GetString();

    // STEP 7: Manage conversation history manually
    conversationHistory.Add(new { role = "user",      content = userQuestion });
    conversationHistory.Add(new { role = "assistant", content = finalAnswer });
}
// STEP 8: Handle 429 retries — you write this
// STEP 9: Handle streaming — you write this
// ~150 lines for ONE question that calls ONE tool
// Rewrite for every project
```

---

## WITH Semantic Kernel — Same Scenario

```csharp
// SK handles steps 1-9 above. You write only business logic:

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion("gpt-4o", endpoint, credential)
    .Build();

kernel.Plugins.AddFromType<ContractPlugin>(); // your C# method

var history = new ChatHistory("You are a JMA contract assistant.");
history.AddUserMessage(userQuestion);

// SK does: API call → detect tool → run your method →
//          second API call → return answer → update history
var answer = await chatService.GetChatMessageContentAsync(
    history,
    new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions },
    kernel);

// ~10 lines. SK wrote the other 140 for you.
```

---

## Core Building Blocks

```
┌─────────────────────────────────────────────────────┐
│  KERNEL                                             │
│  ← central object, like SearchClient               │
│  ← wires up: model, plugins, memory, filters       │
│  var kernel = Kernel.CreateBuilder()...Build()      │
├─────────────────────────────────────────────────────┤
│  PLUGIN                                             │
│  ← C# class with methods the AI can call           │
│  ← equivalent to OpenAPI Action in AI Foundry      │
│                                                     │
│  public class ContractPlugin                        │
│  {                                                  │
│      [KernelFunction]                               │
│      public string SearchContracts(string dealer)   │
│      {                                              │
│          return contractService.Search(dealer);     │
│      }                                              │
│  }                                                  │
├─────────────────────────────────────────────────────┤
│  CHAT HISTORY                                       │
│  ← manages conversation context (context window)   │
│  ← system, user, assistant messages                │
├─────────────────────────────────────────────────────┤
│  MEMORY                                             │
│  ← stores/retrieves facts across sessions           │
│  ← connects to AI Search vector index              │
├─────────────────────────────────────────────────────┤
│  FILTERS (FunctionInvocationFilter)                 │
│  ← intercept before/after tool calls               │
│  ← apply business rules, logging, safety           │
└─────────────────────────────────────────────────────┘
```

---

## Full List — Everything SK Can Build

```
1.  SIMPLE CHAT
    GPT-4o conversation in your C# app with history
    Example: JMA internal Q&A bot inside existing .NET app

2.  RAG PIPELINE (in code)
    Chunk → embed → store → retrieve → generate, full control
    Example: Contract natural language search

3.  AI AGENT (single)
    Agent decides which tools to call automatically
    Example: "Get dealer 4512 contracts, summarise, email it"

4.  MULTI-AGENT SYSTEM
    Orchestrator agent routes to specialist agents
    Example: Research agent + Writer agent + Validator agent

5.  DOCUMENT PROCESSING PIPELINE
    Ingest PDFs → extract → classify → store, no conversation
    Example: Enhance JMA WebJob to classify cancellation reasons

6.  FUNCTION CALLING / TOOL ORCHESTRATION
    Model chains multiple C# function calls automatically
    Example: Find contract → get dealer info → check policy → respond

7.  PROMPT CHAINING
    Output of one prompt becomes input of next prompt
    Example: Extract → translate → summarise → format as JSON

8.  STRUCTURED OUTPUT EXTRACTION
    Pass raw text → get typed C# object back
    Example: Extract ContractNumber, DealerId, Amount from scanned doc

9.  SEMANTIC MEMORY / VECTOR STORE
    Save facts to AI Search → retrieve by meaning across sessions
    Example: Remember dealer preferences across conversations

10. STREAMING RESPONSES
    Token-by-token output like ChatGPT typing effect
    IAsyncEnumerable<string> in C#
    Example: JMA chatbot that streams answers in real time

11. CONTENT SAFETY / FILTERING
    FunctionInvocationFilter plugs into SK pipeline
    Example: Block prompt injection in contract search

12. EVALUATION / TESTING
    Run golden dataset through SK app in CI/CD
    Example: Test 100 sample questions before every deployment

13. COST OPTIMISATION ROUTING
    Route simple queries to GPT-4o mini, complex to GPT-4o
    Example: Save cost — 80% of queries go to cheaper model
```

---

## SK vs AI Foundry Portal — The Real Difference

```
                    AI FOUNDRY PORTAL         SEMANTIC KERNEL (C#)
────────────────────────────────────────────────────────────────────
What it is          Microsoft web UI          NuGet package in your app
Agent lives         Microsoft's cloud         Your Azure App Service
Code required       None                      Yes — C#
Business logic      ❌ no IF/ELSE in portal   ✅ full C# logic
Internal DB access  ❌ needs API wrapper       ✅ direct SQL/EF calls
Custom logging      ❌ not controllable        ✅ your App Insights
Cost routing        ❌ one model per agent     ✅ per-query model choice
Streaming           ❌ not controllable        ✅ IAsyncEnumerable
CI/CD testing       ❌ not possible            ✅ automated eval
Best for            Prototype / demo           Production application
```

---

## AI Foundry Portal vs SK — They Are Independent

```
You can use SK WITHOUT AI Foundry.
You can use AI Foundry WITHOUT SK.
They CAN work together (SK calls AI Foundry agent via REST) but optional.

RELATIONSHIP:
  AI Foundry = where you design and test
  SK         = where you build production
```

---

## JMA Scenario — Why SK Is Needed

```
JMA contract assistant requirements:
  1. Search contracts by natural language        ← AI Search plugin
  2. Check dealer credit status (SQL database)  ← [KernelFunction] GetDealerCredit()
  3. Apply business rules (frozen accounts)     ← FunctionInvocationFilter
  4. Stream answers token by token              ← GetStreamingChatMessageContentsAsync()
  5. Log queries to JMA's own App Insights      ← TelemetryClient.TrackEvent()
  6. Route simple queries to GPT-4o mini        ← IF/ELSE model selection in code

AI Foundry portal can do requirement 1 only.
SK can do all 6.
```

---

## Where SK Fits — JMA Roadmap

```
TODAY (prototype):
  AI Foundry portal → JMAVehicleIQA agent
  ← quick to build, good for testing RAG behaviour

PRODUCTION:
  SK C# agent → deployed as Azure App Service
  ├── reads from documents-dev AI Search index (existing)
  ├── uses EnterpriseSearch.Api for contract lookup (existing)
  ├── uses EnterpriseSearch.Sync WebJob for indexing (existing)
  └── adds GPT-4o conversation + business logic on top
  ← your existing JMA infrastructure + SK on top
```

---

## One-Line Summary

> Semantic Kernel is a C# SDK that handles all AI infrastructure plumbing
> (API calls, tool routing, history, memory, retry, streaming) so you write
> only business logic — the same way Azure SDK handles HTTP/auth plumbing
> for AI Search in the JMA WebJob.

---

## Navigation

| | |
|---|---|
| **Previous** | [09 — RAG Deep Dive](09-RAG-Deep-Dive.md) |
| **Next** | `11-LLMOps-Evaluation.md` *(coming soon)* |
