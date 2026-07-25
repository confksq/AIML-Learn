# 03 — Interview Q&A: LlamaIndex (15 questions, senior level)

---

**Q1. What is LlamaIndex in one sentence?**
A data-centric, RAG-specialized Python framework for ingesting data, indexing it, and querying it with cited answers — the retrieve→augment→generate pipeline wrapped in a focused, opinionated abstraction purpose-built for retrieval over your own data.

**Q2. LlamaIndex vs LangChain — what's the core difference?**
LangChain is a general LLM orchestration toolkit (chains, agents, tools); LlamaIndex is RAG-specialized (ingest, index, query, cite). If the core problem is high-quality retrieval and cited Q&A over a corpus, LlamaIndex gets there with less code and better indexing abstractions. If I need multi-step agents and tool orchestration around the RAG, LangChain (or Semantic Kernel) fits better. They also compose — LlamaIndex as the retrieval layer inside a LangChain agent.

**Q3. Walk through the core LlamaIndex objects.**
Document (a loaded source), Node (a chunk plus metadata and relationships to other nodes), Index (the searchable structure — VectorStoreIndex is standard), Retriever (pulls top-K nodes), and QueryEngine (the end-to-end retrieve→prompt→LLM→cited-answer object via `index.as_query_engine()`).

**Q4. How do you build a RAG pipeline in LlamaIndex?**
Four lines: load documents (`SimpleDirectoryReader("data").load_data()`), build the index (`VectorStoreIndex.from_documents(docs)`), create a query engine (`index.as_query_engine()`), and query it. The response contains both the answer and `source_nodes` for citations. That terseness is the RAG-specialized value.

**Q5. What is a Node and why do node relationships matter?**
A Node is a chunk of a Document plus metadata and relationships (parent/sibling links). The relationships enable advanced retrieval like auto-merging / parent-child — retrieve precise small nodes for accuracy, then return the merged larger parent for context. It's the L13 parent-child pattern as a first-class feature, not something you hand-build.

**Q6. How do you get citations in LlamaIndex?**
They're built in — `response.source_nodes` returns the chunks (with scores and metadata) the answer was synthesized from. You don't have to wire citation-tracking yourself, which is one reason LlamaIndex is convenient for cited Q&A / compliance-sensitive RAG.

**Q7. How do you run LlamaIndex fully locally?**
Set the global `Settings.llm` to an Ollama model and `Settings.embed_model` to a local HuggingFace embedding model. Every index and query engine then uses them — no paid API. Same "swap the backend, keep the architecture" move as running RAG on Ollama or Hugging Face.

**Q8. What's the role of the global Settings object?**
It holds the default LLM, embedding model, chunk size, etc. Set it once and all downstream indexes/query engines inherit it. It's the single place you switch cloud↔local or change the embedding model without touching pipeline code.

**Q9. What index types beyond VectorStoreIndex does LlamaIndex offer?**
SummaryIndex (summarize over all nodes), KeywordTableIndex (keyword lookup), KnowledgeGraphIndex (graph over extracted entities), and composable/router indexes that pick the right sub-index per query. This is a reason to choose LlamaIndex when retrieval strategy matters — it offers more than flat vector search out of the box.

**Q10. What is a response synthesizer?**
The strategy for combining retrieved nodes into a final answer — e.g., "refine" (iteratively improve the answer across nodes) or "tree_summarize" (hierarchically summarize). It matters for long contexts where you can't just stuff all nodes into one prompt — LlamaIndex handles the synthesis pattern for you.

**Q11. How would you use a production vector store instead of the in-memory default?**
Plug in a VectorStore integration — Qdrant, Weaviate, pgvector, or Azure AI Search — via a StorageContext. The pipeline code stays the same; only the storage backend changes. Same portability as swapping FAISS for Azure AI Search in a custom pipeline.

**Q12. How does LlamaIndex handle evaluation?**
It has built-in evaluators for faithfulness and relevancy (does the answer stay grounded in retrieved nodes; does it address the query) — overlapping with RAGAS. So you can evaluate a LlamaIndex pipeline natively, or export the (question, answer, contexts) records and score them with RAGAS for the standardized four metrics.

**Q13. When is LlamaIndex the wrong choice?**
When the problem isn't primarily retrieval — a multi-step agent with many tools, complex branching, and orchestration is better served by LangChain/LangGraph or Semantic Kernel. LlamaIndex's RAG focus is a strength for retrieval and a limitation for general agentic workflows (though you can embed it as the retrieval component).

**Q14. Given you know LangChain, how do you frame LlamaIndex fluency in an interview?**
I know RAG deeply and I've used both frameworks. LlamaIndex is my choice when the core problem is retrieval quality and cited Q&A over a corpus — it gives better indexing abstractions (node relationships, auto-merging, multiple index types) and citations with less code. I use LangChain/Semantic Kernel when I need agents and orchestration around the RAG, and I compose them when both are needed.

**Q15. Design a cited document-Q&A system for a compliance-sensitive use case with LlamaIndex.**
Load documents with metadata (source, page), build a VectorStoreIndex backed by a production vector store, use auto-merging retrieval for precise-yet-contextual chunks, a query engine with a grounding prompt and a response synthesizer, and return `source_nodes` as citations for every answer. Add a faithfulness evaluator (or RAGAS) as a quality gate, and a confidence threshold to return "not found" instead of hallucinating — the same grounding discipline as my Azure RAG, expressed in LlamaIndex.

---
*Anchor answers in "I already know RAG and LangChain — LlamaIndex is the RAG-specialized version, and here's when I pick it."*
