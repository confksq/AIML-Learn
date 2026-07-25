# Module 10 — Bot Development
**Part 2: AI Engineering (AI-102 Level) | AI Solutions Architect Curriculum**
*Created: 2026-06-30*

---

## Why This Module Matters

Bots are the conversation channel layer — the interface through which users interact with your AI services via text or voice. In 2026, "bot" has largely evolved into "AI Agent" (Module 14), but the channel infrastructure (Teams, web chat, phone IVR) is still the Bot Framework.

**Key connection:** Bot Framework handles the CHANNEL (Teams/Web/Slack/phone). AI Agents handle the INTELLIGENCE. You use both together — Bot Framework routes the conversation, Semantic Kernel agents do the reasoning.

**JM Family relevance:** Dealer support chatbot in Teams, internal HR Q&A bot on intranet, voice IVR for dealer phone support — all go through Azure Bot Service for channel connectivity.

---

**Running example:**
> *JM Family wants a dealer support bot in Microsoft Teams — dealers type questions and get answers from the AI knowledge base.*

---

## Topic 10.1 — Bot Framework Fundamentals

---

### 1. What Is the Bot Framework?

Microsoft Bot Framework is the SDK and infrastructure for building conversational applications across multiple channels:

```
User sends message
    │
    ▼
Channel (Teams / Web Chat / Slack / Phone / SMS)
    │
    ▼
Azure Bot Service (channel routing, authentication)
    │
    ▼
Your Bot App (Azure App Service / Azure Functions)
    │ Bot Framework SDK processes Activity
    ▼
Your logic (AI services, databases, APIs)
    │
    ▼
Response sent back through same channel
```

One bot codebase → deployed to multiple channels simultaneously.

---

### 2. Core Concepts: Activities and Turns

**Activity** = any event in the conversation (message, user joined, button click, typing indicator)

```
Activity types:
  MessageActivity    → user or bot sent a text/attachment message
  ConversationUpdate → user joined or left
  Event              → custom application event
  Invoke             → card button clicked
  Typing             → user is typing indicator
```

**Turn** = one complete request-response cycle:

```
Turn 1:
  User sends: "What's the status of order ATL-001?"
  Bot processes → calls OrderAPI → replies: "Order ATL-001 ships July 15."

Turn 2:
  User sends: "Can I change the delivery address?"
  Bot processes → calls ChangeAddressAPI → replies: "Address updated."
```

Each turn is stateless by default — you must manage state explicitly if you need context across turns.

---

### 3. Bot Architecture

```csharp
// The bot class — handles every incoming activity
public class DealerSupportBot : ActivityHandler
{
    private readonly IOrderService _orderService;

    public DealerSupportBot(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // Called on every text message
    protected override async Task OnMessageActivityAsync(
        ITurnContext<IMessageActivity> turnContext,
        CancellationToken cancellationToken)
    {
        var userMessage = turnContext.Activity.Text;

        // Process and reply
        var reply = await ProcessMessageAsync(userMessage);
        await turnContext.SendActivityAsync(reply, cancellationToken: cancellationToken);
    }

    // Called when user joins the conversation
    protected override async Task OnMembersAddedAsync(
        IList<ChannelAccount> membersAdded,
        ITurnContext<IConversationUpdateActivity> turnContext,
        CancellationToken cancellationToken)
    {
        await turnContext.SendActivityAsync(
            "Welcome to JM Family Dealer Support. How can I help you today?",
            cancellationToken: cancellationToken);
    }
}
```

---

### 4. State Management

Bots are stateless by default. For multi-turn conversations you need to store state:

```csharp
// Three state scopes:
UserState         // persists per user (across all conversations)
ConversationState // persists per conversation (all users in one conversation)
PrivateConversationState // persists per user per conversation

// Define what to store
public class ConversationData
{
    public string LastDealerCode { get; set; }
    public string PendingOrderNumber { get; set; }
    public DateTime LastInteraction { get; set; }
}

// Access in your bot
var conversationStateAccessor = _conversationState
    .CreateProperty<ConversationData>("ConversationData");

var data = await conversationStateAccessor.GetAsync(
    turnContext,
    () => new ConversationData(),
    cancellationToken);

// Read/write
data.LastDealerCode = "ATL-001";
await _conversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
```

**State storage backends:**
- `MemoryStorage` — dev only, lost on restart
- `CosmosDbPartitionedStorage` — production, durable, scalable
- `BlobStorage` — production, cheaper than Cosmos for low-access state

---

### 5. Bot Framework Emulator

The Bot Framework Emulator is a desktop app for local development:

```
Install: Download from github.com/microsoft/BotFramework-Emulator
Run your bot locally (dotnet run)
Connect Emulator to: http://localhost:3978/api/messages
Test conversations without deploying to Azure or Teams
Inspect: each activity, payload, state changes
```

Essential for development — don't deploy to Teams until local testing passes.

---

## Topic 10.2 — Building Bots with C#

---

### 1. Creating a Bot Project

```bash
# Install Bot Framework templates
dotnet new install Microsoft.Bot.Framework.CSharp.EchoBot

# Create echo bot project
dotnet new echobot -n JmaDealerSupportBot
cd JmaDealerSupportBot

# Project structure:
# JmaDealerSupportBot/
#   ├── Bots/
#   │   └── EchoBot.cs          ← your bot logic
#   ├── appsettings.json         ← MicrosoftAppId, MicrosoftAppPassword
#   └── Startup.cs               ← DI registration
```

---

### 2. Sending Rich Cards

Plain text gets boring fast. Cards make responses interactive:

```csharp
// Hero Card — image + title + buttons
var heroCard = new HeroCard
{
    Title = "Order ATL-001-F150",
    Subtitle = "Ford F-150 XLT | Expected: July 15, 2026",
    Text = "Your order is confirmed and in transit.",
    Images = new List<CardImage>
    {
        new CardImage("https://jmastorage.blob.core.windows.net/vehicles/f150.jpg")
    },
    Buttons = new List<CardAction>
    {
        new CardAction(ActionTypes.ImBack, "Track Delivery", value: "track ATL-001"),
        new CardAction(ActionTypes.ImBack, "Change Address", value: "change address ATL-001"),
        new CardAction(ActionTypes.OpenUrl, "View Invoice", value: "https://jmfamily.com/invoices/ATL-001")
    }
};

var reply = MessageFactory.Attachment(heroCard.ToAttachment());
await turnContext.SendActivityAsync(reply, cancellationToken: cancellationToken);
```

**Card types available:**
- `HeroCard` — image + title + buttons (most common)
- `ThumbnailCard` — small image + text + buttons
- `AdaptiveCard` — fully custom JSON layout (richest option, works in Teams)
- `ReceiptCard` — itemized list (good for invoice display)
- `SigninCard` — OAuth login flow

---

### 3. Adaptive Cards — The Modern Standard

Adaptive Cards are JSON-defined cards that render natively in Teams, Outlook, and Web Chat:

```json
{
  "type": "AdaptiveCard",
  "version": "1.4",
  "body": [
    {
      "type": "TextBlock",
      "text": "Order Status: ATL-001-F150",
      "weight": "Bolder",
      "size": "Medium"
    },
    {
      "type": "FactSet",
      "facts": [
        {"title": "Vehicle", "value": "Ford F-150 XLT"},
        {"title": "Status", "value": "In Transit"},
        {"title": "ETA", "value": "July 15, 2026"},
        {"title": "Dealer", "value": "ATL-001 Southeast Region"}
      ]
    }
  ],
  "actions": [
    {"type": "Action.Submit", "title": "Track Delivery", "data": {"action": "track", "orderId": "ATL-001"}},
    {"type": "Action.OpenUrl", "title": "View Invoice", "url": "https://jmfamily.com/invoices/ATL-001"}
  ]
}
```

Design Adaptive Cards at: adaptivecards.io/designer

---

### 4. Waterfall Dialogs — Multi-Step Conversations

Waterfall dialogs guide users through a sequence of steps:

```csharp
// Define the dialog steps
var waterfallSteps = new WaterfallStep[]
{
    AskDealerCodeStepAsync,
    AskOrderNumberStepAsync,
    ConfirmAndLookupStepAsync
};

AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
AddDialog(new TextPrompt(nameof(TextPrompt)));

// Step 1: Ask for dealer code
private async Task<DialogTurnResult> AskDealerCodeStepAsync(
    WaterfallStepContext stepContext, CancellationToken cancellationToken)
{
    return await stepContext.PromptAsync(
        nameof(TextPrompt),
        new PromptOptions { Prompt = MessageFactory.Text("What is your dealer code?") },
        cancellationToken);
}

// Step 2: Got dealer code, ask for order number
private async Task<DialogTurnResult> AskOrderNumberStepAsync(
    WaterfallStepContext stepContext, CancellationToken cancellationToken)
{
    stepContext.Values["dealerCode"] = (string)stepContext.Result;
    return await stepContext.PromptAsync(
        nameof(TextPrompt),
        new PromptOptions { Prompt = MessageFactory.Text("What is your order number?") },
        cancellationToken);
}

// Step 3: Look up and respond
private async Task<DialogTurnResult> ConfirmAndLookupStepAsync(
    WaterfallStepContext stepContext, CancellationToken cancellationToken)
{
    var orderNumber = (string)stepContext.Result;
    var dealerCode = (string)stepContext.Values["dealerCode"];

    var status = await _orderService.GetStatusAsync(dealerCode, orderNumber);
    await stepContext.Context.SendActivityAsync($"Order {orderNumber}: {status}", cancellationToken: cancellationToken);

    return await stepContext.EndDialogAsync(null, cancellationToken);
}
```

---

## Topic 10.3 — Integrating AI Services

---

### 1. Adding CLU for Intent Recognition

Wire CLU into your bot to understand user intent before routing:

```csharp
protected override async Task OnMessageActivityAsync(
    ITurnContext<IMessageActivity> turnContext,
    CancellationToken cancellationToken)
{
    var userMessage = turnContext.Activity.Text;

    // Recognize intent with CLU
    var cluResult = await _cluService.AnalyzeAsync(userMessage);
    var topIntent = cluResult.TopIntent;
    var confidence = cluResult.Confidence;

    if (confidence < 0.60)
    {
        // Low confidence — ask for clarification
        await turnContext.SendActivityAsync(
            "I'm not sure I understood. Could you rephrase that?",
            cancellationToken: cancellationToken);
        return;
    }

    // Route based on intent
    switch (topIntent)
    {
        case "CheckOrderStatus":
            await _dialogSet.CreateContext(turnContext).BeginDialogAsync(
                nameof(OrderStatusDialog), null, cancellationToken);
            break;

        case "ReportDamage":
            await _dialogSet.CreateContext(turnContext).BeginDialogAsync(
                nameof(DamageReportDialog), null, cancellationToken);
            break;

        default:
            // Fall through to AI (OpenAI RAG)
            await HandleWithAIAsync(turnContext, userMessage, cancellationToken);
            break;
    }
}
```

---

### 2. Adding Azure OpenAI for Open-Ended Questions

For questions outside defined intents, fall through to RAG:

```csharp
private async Task HandleWithAIAsync(
    ITurnContext turnContext, string userMessage, CancellationToken ct)
{
    // Show typing indicator while AI processes
    await turnContext.SendActivityAsync(
        new Activity { Type = ActivityTypes.Typing }, cancellationToken: ct);

    // Call your RAG pipeline
    var ragResponse = await _ragService.QueryAsync(
        userMessage,
        conversationHistory: await GetConversationHistoryAsync(turnContext)
    );

    // Send response with citation
    var responseText = ragResponse.Answer;
    if (ragResponse.Sources.Any())
        responseText += $"\n\n*Source: {string.Join(", ", ragResponse.Sources)}*";

    await turnContext.SendActivityAsync(responseText, cancellationToken: ct);
}
```

---

### 3. CLU + QA + OpenAI — Three-Layer Intent Routing

```
User message arrives
    │
    ▼ Layer 1: CLU (structured intents)
    ├─ CheckOrderStatus → OrderStatusDialog
    ├─ ReportDamage → DamageReportDialog
    ├─ CancelOrder → CancellationDialog
    │
    ▼ No CLU match (confidence < 0.60)
    Layer 2: Question Answering (FAQ knowledge base)
    ├─ "What are your office hours?" → QA answer
    ├─ "How do I submit a warranty claim?" → QA answer
    │
    ▼ No QA match (confidence < 0.50)
    Layer 3: Azure OpenAI RAG
    └─ Open-ended questions from full document knowledge base
```

This layered approach: cheapest first (CLU), then moderate (QA), then expensive (LLM) only when needed.

---

## Topic 10.4 — Deploying Bots

---

### 1. Azure Bot Service

Azure Bot Service is the cloud infrastructure that:
- Hosts your bot endpoint
- Routes messages from channels (Teams, Web, Slack) to your app
- Handles authentication between channels and your bot
- Provides channel management UI

```
Channels available:
  Microsoft Teams      ← most common for JM Family
  Web Chat             ← embed on website
  Direct Line          ← custom apps (mobile, desktop)
  Slack
  Telegram
  Facebook Messenger
  SMS (via Twilio)
  Email
  Phone (via Azure Communication Services)
```

---

### 2. Deployment to Azure App Service

```bicep
// Azure Bot Service resource
resource botService 'Microsoft.BotService/botServices@2022-09-15' = {
  name: 'bot-jma-dealer-support'
  location: 'global'  // Bot Service is always global
  sku: { name: 'S1' }
  kind: 'azurebot'
  properties: {
    displayName: 'JMA Dealer Support'
    msaAppId: managedIdentity.properties.clientId
    endpoint: 'https://app-jma-bot.azurewebsites.net/api/messages'
  }
}

// Teams channel
resource teamsChannel 'Microsoft.BotService/botServices/channels@2022-09-15' = {
  parent: botService
  name: 'MsTeamsChannel'
  properties: {
    channelName: 'MsTeamsChannel'
    properties: { isEnabled: true }
  }
}
```

---

### 3. Microsoft Teams Bot — Key Differences

Teams bots have extra capabilities compared to web chat:

```
Teams-specific features:
  @mention support         → user types "@JMABot what's my order status?"
  Proactive messages       → bot sends message without user initiating
  Task modules             → pop-up forms inside Teams
  Message extensions       → bot accessible from compose box
  Meeting bots             → join Teams meetings, take notes
  Adaptive Cards           → rich interactive cards (best in Teams)
```

**Proactive messages** — bot initiates contact (e.g., "Your order ATL-001 just shipped"):

```csharp
// Store conversation reference when user first messages bot
_conversationReferences[userId] = turnContext.Activity.GetConversationReference();

// Later, proactively notify user (called from elsewhere in your system)
public async Task SendProactiveNotificationAsync(string userId, string message)
{
    var conversationReference = _conversationReferences[userId];
    await _adapter.ContinueConversationAsync(
        _appId,
        conversationReference,
        async (context, token) =>
            await context.SendActivityAsync(message, cancellationToken: token),
        CancellationToken.None);
}
```

JM Family use: automatically notify dealer when their order ships, without waiting for them to ask.

---

### 4. Bot Security

```
Authentication layers:
  1. Bot Framework authentication
     Azure Bot Service validates JWTs between channels and your bot
     Your bot verifies the JWT on every incoming request

  2. User authentication (SSO with Teams)
     Users sign in once to Teams → bot gets their identity
     Bot can call APIs on behalf of the user (OBO flow)
     No separate login needed

  3. Managed Identity for your bot → AI services
     Bot App Service has Managed Identity
     Calls Azure OpenAI, AI Search with identity (no keys)
```

---

## Topic R10 — Recall: Module 10 Review & Quiz

---

**Q1.** What is the difference between a Turn and an Activity in Bot Framework?

> **A:** An Activity is any single event in the bot conversation — a message sent, a user joining, a button clicked, a typing indicator. A Turn is one complete request-response cycle: one incoming Activity triggers your bot code, which processes it and sends back one or more responses. A turn can involve multiple outgoing Activities (e.g., a typing indicator followed by the actual response card).

---

**Q2.** JM Family wants the bot to remember which dealer code a user mentioned earlier in the conversation so they don't have to repeat it. How do you implement this?

> **A:** Use `ConversationState` with a state accessor. Create a `ConversationData` class with a `LastDealerCode` property. In each turn, read the state, check if a dealer code was mentioned, store it if so, and use it in subsequent turns. Back the state with `CosmosDbPartitionedStorage` in production for durability across restarts.

---

**Q3.** Why use Waterfall Dialogs instead of just handling everything in `OnMessageActivityAsync`?

> **A:** Waterfall Dialogs manage multi-step conversations with built-in state tracking. Without them, you'd have to manually track "which step are we on?" in your own state. Waterfall handles the step sequencing, prompts, input validation, and step results automatically. For a simple one-turn question-answer, `OnMessageActivityAsync` is fine. For multi-step flows (ask dealer code → ask order number → confirm and look up), use Waterfall.

---

**Q4.** Your JM Family Teams bot should send an automatic notification when a dealer's order ships, even when the dealer hasn't sent a message. What Bot Framework feature handles this?

> **A:** Proactive messages using `ContinueConversationAsync`. When the dealer first messages the bot, store their ConversationReference. When the shipping event fires (from your order system via Event Grid or Service Bus), call `adapter.ContinueConversationAsync` with the stored reference to send an outbound message to their Teams chat without them initiating.

---

**Q5.** How does a modern JM Family bot architecture combine Bot Framework, CLU, Question Answering, and Azure OpenAI?

> **A:** Three-layer routing: (1) CLU handles high-confidence structured intents (CheckOrder, ReportDamage) → routes to Waterfall dialogs that call specific APIs. (2) Question Answering handles FAQ-style questions (office hours, policies) → returns fixed KB answers cheaply. (3) Azure OpenAI RAG handles everything else — open-ended questions answered from the full document knowledge base. This minimizes expensive LLM calls while maximizing coverage.

---

## Memory Hooks

- **"Bot Framework = channel layer, AI = intelligence layer"**
- **"Activity = one event, Turn = one request-response cycle"**
- **"ConversationState for multi-turn memory, CosmosDB backend in production"**
- **"Waterfall = guided multi-step dialog with built-in step tracking"**
- **"Adaptive Cards = richest UI, works best in Teams"**
- **"Proactive messages = bot initiates, use ContinueConversationAsync"**
- **"Three-layer routing: CLU → QA → OpenAI (cheapest to most expensive)"**
- **"Bot Service = global resource, your bot app = regional (App Service)"**

---

## 2026 Updates

| Topic | Update |
|---|---|
| **Copilot Studio replaces PVA** | Power Virtual Agents is now Microsoft Copilot Studio — low-code bot builder with built-in GPT-4o, CLU, and AI Search integration. Good for non-developer teams at JMA. Use Bot Framework SDK for production custom bots |
| **Teams AI Library** | New TypeScript/C# library specifically for Teams bots — simplified Teams-specific features, built-in Adaptive Card handling, message extension support. Replaces older Teams-specific patterns |
| **Azure Communication Services integration** | Bot Framework now integrates with Azure Communication Services for voice bots — your bot can answer phone calls using the same SDK |
| **Bot Framework → AI Agent convergence** | Microsoft is converging Bot Framework channels with Azure AI Agent Service. Future: define your agent logic in SK/AI Foundry, connect channels via Bot Service. Separate concerns cleanly |

---

## Interactive Learning Ideas

### Exercise 1 — Echo Bot in 15 Minutes (15 min)
```bash
dotnet new echobot -n JmaTestBot
cd JmaTestBot && dotnet run
```
Open Bot Framework Emulator → connect to localhost:3978 → send messages and observe the echo. Inspect the Activity JSON for each message.

### Exercise 2 — Add Adaptive Card Response (20 min)
Modify the Echo Bot to respond with an Adaptive Card instead of plain text:
- Design the card at adaptivecards.io/designer
- Convert the JSON to a C# attachment
- Send it as the bot response
Test in Emulator — does the card render?

### Exercise 3 — Three-Layer Intent Router (30 min)
Implement the CLU → QA → OpenAI routing pattern in C#:
1. If CLU confidence > 0.75 for a known intent → handle with dialog
2. If QA confidence > 0.60 → return QA answer
3. Otherwise → call your RAG pipeline
Test with: a known intent phrase, a FAQ phrase, and an open-ended question.

### Exercise 4 — Deploy to Azure and Connect to Teams (30 min)
Deploy your test bot to Azure App Service:
- Create Azure Bot Service resource (free F0 tier)
- Point to your App Service URL
- Add Teams channel in Azure portal
- Install the bot in Teams via App Studio or side-loading
- Test a real Teams conversation with your bot

---

*Previous: Module 9 — Azure AI Search*
*Next: Module 11 — LLMs Deep Dive*
*Connects to: Module 4 (CLU/QA — bot intelligence), Module 12 (Azure OpenAI — bot brain), Module 14 (AI Agents — evolved bot pattern)*
