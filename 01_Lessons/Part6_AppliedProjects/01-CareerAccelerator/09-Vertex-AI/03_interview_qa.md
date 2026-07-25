# 03 — Interview Q&A: GCP Vertex AI + ADK (15 questions, senior level)

---

**Q1. What is Vertex AI, and what's its Azure equivalent?**
Google Cloud's managed GenAI/ML platform — foundation models (Gemini), managed RAG (Vertex AI Search / Vector Search), agents (Agent Development Kit + Agent Engine), embeddings, and safety filters. It's the GCP mirror of Azure AI Foundry / Azure OpenAI; same architecture, different vendor primitives, called via the google-genai or vertexai SDK.

**Q2. What is Gemini and how does it compare to GPT-4o?**
Google's flagship model family — natively multimodal (text/image/audio/video) with a very long context window (~1–2M tokens vs GPT-4o's 128k). Gemini Flash is the cost-efficient GPT-4o-mini analog; Gemini Pro the GPT-4o analog. I'd benchmark both on the actual task rather than assume a winner.

**Q3. Gemini API vs Vertex AI — when each?**
The Gemini API (AI Studio, API-key) is for prototyping — like calling api.openai.com. Vertex AI runs in your GCP project with IAM auth, data governance, and VPC-SC — the enterprise choice, exactly like preferring Azure OpenAI over OpenAI-direct. The google-genai SDK targets both; set GOOGLE_GENAI_USE_VERTEXAI=True for Vertex.

**Q4. What is Vertex AI Search?**
Managed RAG — point it at GCS, BigQuery, or websites and it chunks, embeds, indexes, and serves grounded, cited answers. It's the Azure OpenAI "On Your Data" + AI Search equivalent. For a custom retrieval loop I'd use Vertex AI Vector Search (the AI Search vector index analog) instead.

**Q5. What is Vertex AI Vector Search?**
A managed vector database (approximate nearest neighbor, formerly Matching Engine) — the Azure AI Search vector index / FAISS equivalent when you want to build the RAG pipeline yourself. Use Vertex AI Search for a fully managed loop, Vector Search when you need control over chunking/retrieval/prompting.

**Q6. What is the Agent Development Kit (ADK)?**
Google's open-source agent framework — the Vertex-native counterpart to Semantic Kernel and crewAI. You define an Agent (Gemini + instructions + tools), where tools are Python functions (like [KernelFunction]) plus built-ins (Google Search, code exec) and other agents-as-tools for multi-agent patterns. Deploy to Agent Engine (managed) or Cloud Run.

**Q7. How does ADK compare to Semantic Kernel and crewAI?**
Same mental model — an LLM that plans (ReAct), calls tools, and can delegate to sub-agents. ADK is Gemini/Vertex-native; SK is .NET/Azure-native; crewAI is a Python role-based crew abstraction. The concepts transfer directly; I pick per the target cloud and language.

**Q8. What is Agent Builder vs ADK?**
Agent Builder is the low-code console layer (the Copilot Studio analog) for building agents and Vertex AI Search apps without code; ADK is the pro-code framework. Prototype in Agent Builder, build production in ADK — the same Prompt-Flow-then-Semantic-Kernel path I use on Azure.

**Q9. How does auth work on GCP vs Azure?**
GCP uses service accounts with IAM roles; locally you authenticate via Application Default Credentials (gcloud auth application-default login), and on GCP compute the attached service account is used automatically. It's the direct analog of Azure Managed Identity + DefaultAzureCredential — no keys in code, least-privilege roles.

**Q10. What is Model Garden, and is Claude available on GCP?**
Model Garden is Vertex's model catalog (150+ models) — the Foundry-catalog / Bedrock-access equivalent. And yes: Claude (Anthropic), Llama, Mistral, and Gemma are all available through Vertex. So Claude runs on both Bedrock and Vertex — knowing that signals real multi-cloud breadth.

**Q11. When would you choose Vertex AI over Azure Foundry or Bedrock?**
It follows the org's cloud and model preference. GCP-native shops with data in BigQuery/GCS wanting Gemini → Vertex; Azure-native (M365, wants GPT-4o/o1) → Foundry; AWS-native wanting Claude → Bedrock. Vertex's specific differentiators are Gemini's very long context and native BigQuery integration.

**Q12. What multi-cloud value do you bring across Azure, AWS, and GCP?**
I deliver the same GenAI architecture — foundation model + managed RAG + agents + safety + IAM-style auth — on all three, and map cleanly between them: Azure OpenAI ↔ Bedrock ↔ Gemini; AI Search ↔ Knowledge Bases ↔ Vertex AI Search; Semantic Kernel ↔ Bedrock Agents ↔ ADK. That lets me serve any client's cloud footprint without a re-architecture.

**Q13. How would you build RAG on Vertex end to end?**
Put documents in GCS or BigQuery, create a Vertex AI Search datastore (managed chunk/embed/index), and query the serving API for grounded, cited answers via Gemini. For control, use Vertex Vector Search with text-embedding-004 and assemble the prompt myself. It's the same flow as Azure On Your Data vs custom RAG.

**Q14. What are Gemini's long context and multimodality worth architecturally?**
The ~1–2M-token context changes the "stuff context vs RAG" calculus — more can fit before you need retrieval — though "lost in the middle" and cost still argue for curated RAG. Native multimodality means one model handles text+image+audio+video, useful for document/vehicle-image pipelines without bolting on separate services.

**Q15. Design a portable GenAI system that runs on Azure, AWS, or GCP.**
Wrap generation and retrieval behind internal interfaces (IGenerationService / IRetrievalService) with three implementations — Azure OpenAI+AI Search, Bedrock+Knowledge Bases, Vertex Gemini+AI Search. Keep prompts, evaluation (RAGAS), and safety policy vendor-neutral. The app then runs on any cloud by swapping the implementation — the same abstraction discipline that survives SDK/model changes.

---
*Frame answers as "the Azure equivalent is X, the GCP primitive is Y, here's how I choose" — with Bedrock (module 06) that's a full three-cloud story few candidates have.*
