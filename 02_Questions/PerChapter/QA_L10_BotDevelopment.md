# Q&A — L10: Bot Development
**Source chapter:** `01_Lessons/Part2_AzureAIServices/L10_BotDevelopment.md` | **Format:** self-study
**Questions:** 22 | *No overlap with the interview bank or the chapter's own recall quiz — these test the chapter's factual content directly.*

---

## Fundamentals

**Q1. In 2026 terms, what's the division of labor between Bot Framework and AI Agents?**
Bot Framework handles the **CHANNEL** (Teams, web chat, Slack, phone IVR — routing the conversation); AI Agents (Semantic Kernel etc.) handle the **INTELLIGENCE** (the reasoning). You use both: Bot Framework routes, agents think.
*Memory hook: "Bot Framework = channel layer, AI = intelligence layer."*

**Q2. Trace a message's path from user to your logic and back.**
User sends message → **Channel** (Teams/Web Chat/Slack/Phone/SMS) → **Azure Bot Service** (channel routing + authentication) → **your bot app** (App Service/Functions, Bot Framework SDK processes the Activity) → your logic (AI services, DBs, APIs) → response returns through the same channel. One codebase serves all channels simultaneously.

**Q3. Name five Activity types.**
`MessageActivity` (text/attachment message), `ConversationUpdate` (user joined/left), `Event` (custom app event), `Invoke` (card button clicked), `Typing` (typing indicator).

**Q4. Are turns stateful by default? What follows from that?**
No — each turn (one request-response cycle) is stateless. Any context needed across turns (the dealer code mentioned two turns ago) must be stored explicitly via state management.

**Q5. In the ActivityHandler class, which two override methods did the chapter use, and when does each fire?**
`OnMessageActivityAsync` — on every text message (read `turnContext.Activity.Text`, reply with `SendActivityAsync`). `OnMembersAddedAsync` — when a user joins the conversation (send the welcome message).

**Q6. Differentiate the three state scopes.**
`UserState` — per user, across **all** conversations; `ConversationState` — per conversation (all users in it); `PrivateConversationState` — per user **per** conversation.

**Q7. What are the three state storage backends and their placement?**
`MemoryStorage` — dev only, lost on restart; `CosmosDbPartitionedStorage` — production, durable, scalable; `BlobStorage` — production, cheaper for low-access state.

**Q8. How do you read state in a turn, and what must you do after modifying it?**
Create a property accessor (`_conversationState.CreateProperty<ConversationData>("ConversationData")`), `GetAsync` with a factory default, mutate the object — then **`SaveChangesAsync(turnContext)`** or the change is lost.

**Q9. What is the Bot Framework Emulator and what does it connect to?**
A desktop app for local bot testing — run the bot locally (`dotnet run`), point the Emulator at `http://localhost:3978/api/messages`, converse without deploying, and inspect each Activity's JSON payload and state changes. Don't deploy to Teams until local testing passes.

---

## Cards & Dialogs

**Q10. Name the five card types and when you'd reach for each.**
`HeroCard` — image + title + buttons (most common); `ThumbnailCard` — small image variant; `AdaptiveCard` — fully custom JSON layout, richest, best in Teams; `ReceiptCard` — itemized lists (invoice display); `SigninCard` — OAuth login flow.

**Q11. On a HeroCard, what's the difference between ActionTypes.ImBack and ActionTypes.OpenUrl?**
`ImBack` sends the button's value back into the conversation as if the user typed it ("track ATL-001"); `OpenUrl` opens a link in the browser. Adaptive Cards use `Action.Submit` (posts structured `data`) and `Action.OpenUrl` equivalents.

**Q12. What makes Adaptive Cards "the modern standard," and where do you design them?**
JSON-defined cards that render **natively** in Teams, Outlook, and Web Chat — layout blocks like `TextBlock` and `FactSet` plus actions. Designer: **adaptivecards.io/designer**.

**Q13. In a Waterfall Dialog, how does data pass from one step to the next?**
Each step's prompt result arrives as `stepContext.Result` in the **next** step; values that must survive multiple steps are stashed in `stepContext.Values["key"]`. The dialog ends with `EndDialogAsync`. Prompts (e.g., `TextPrompt`) are registered alongside the WaterfallDialog itself.

---

## AI Integration

**Q14. In the CLU-integrated bot, what happens below the 0.60 confidence line?**
The bot doesn't guess — it asks the user to rephrase ("I'm not sure I understood…"). Above the line, it switches on `TopIntent` to route into the matching dialog; unmatched intents fall through to the AI/RAG handler.

**Q15. Reconstruct the three-layer routing pattern and its cost logic.**
**Layer 1: CLU** — high-confidence structured intents → specific dialogs/APIs (cheapest). **Layer 2: Question Answering** — FAQ-style questions from a knowledge base (moderate; threshold ~0.50–0.60). **Layer 3: Azure OpenAI RAG** — open-ended questions over the full document base (most expensive, only when the cheaper layers miss).
*Memory hook: "CLU → QA → OpenAI — cheapest first."*

**Q16. What two UX touches did the RAG fall-through handler add?**
A **typing indicator** (`ActivityTypes.Typing`) while the AI processes, and **source citations** appended to the answer (`*Source: …*`) — plus passing conversation history into the RAG query for context.

---

## Deployment, Teams & Security

**Q17. What does Azure Bot Service itself do, and what's notable about its location?**
Hosts the bot registration/endpoint, routes messages between channels and your app, handles channel↔bot authentication, and provides channel management. The Bot Service resource is **always `location: global`** — your bot *app* (App Service) is the regional piece.
*Memory hook: "Bot Service = global, bot app = regional."*

**Q18. Name six channels available through Bot Service.**
Microsoft Teams (JMA's main), Web Chat (website embed), Direct Line (custom mobile/desktop apps), Slack, Telegram, Facebook Messenger, SMS (Twilio), Email, Phone (Azure Communication Services).

**Q19. What Teams-specific capabilities go beyond plain web chat?**
@mention support, **proactive messages**, task modules (pop-up forms), message extensions (bot in the compose box), meeting bots (join/take notes), and best-in-class Adaptive Card rendering.

**Q20. How do proactive messages work mechanically?**
When the user first messages the bot, store their `ConversationReference`. Later, when an external event fires (order shipped — via Event Grid/Service Bus), call `adapter.ContinueConversationAsync(appId, storedReference, callback)` to send a message **without** the user initiating. JMA use: auto-notify a dealer their order shipped.

**Q21. Describe the three authentication layers in bot security.**
(1) **Bot Framework auth** — Bot Service and your bot exchange/validate JWTs on every request; (2) **user auth / Teams SSO** — the user's Teams identity flows to the bot, enabling on-behalf-of (OBO) API calls with no separate login; (3) **Managed Identity** from your bot's App Service to AI services (OpenAI, AI Search) — no keys.

**Q22. From the 2026 updates: what replaced Power Virtual Agents, and what's the Bot Framework's future direction?**
**Microsoft Copilot Studio** replaced PVA — low-code bot building with built-in GPT-4o/CLU/AI Search (good for non-developer teams; Bot Framework SDK remains the production custom-bot path). Direction: Bot Framework channels converge with **Azure AI Agent Service** — agent logic defined in SK/AI Foundry, channels connected via Bot Service. Also new: the Teams AI Library, and ACS integration for voice bots answering phone calls.

---

*Curriculum Q&A Batch B — file 2 of 4. Next: QA_L11_1 (Attention & Transformer).*
