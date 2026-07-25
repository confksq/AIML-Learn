# 01 — Concepts: LlamaIndex

> **Bridge from what you already know:** you know LangChain and you know RAG cold (curriculum L13). LlamaIndex is a **RAG-specialized** framework — the same retrieve → augment → generate pipeline, wrapped in a cleaner, data-centric abstraction.

---

## 1. The one-sentence mental model

**LlamaIndex is LangChain's RAG-focused cousin: where LangChain is a general orchestration toolkit, LlamaIndex is purpose-built for "ingest data → index it → query it with citations."** If your task *is* RAG, LlamaIndex gets you there with less code.

| RAG concept you know | LlamaIndex object |
|---|---|
| A source file | **Document** |
| A chunk | **Node** (a chunk + metadata + relationships) |
| The vector index | **Index** (`VectorStoreIndex`, etc.) |
| Embedding model | **embed_model** (Settings) |
| The LLM | **llm** (Settings) |
| Your RAG orchestrator | **QueryEngine** |
| Retrieval step | **Retriever** |
| Azure AI Search | a **VectorStore** integration (or the built-in in-memory store) |

---

## 2. The core objects

- **Document** — a loaded source (from a file, a directory, a database). `SimpleDirectoryReader("data").load_data()` loads a folder.
- **Node** — a chunk of a Document plus metadata and relationships to sibling/parent nodes. LlamaIndex's node-with-relationships model is why parent-child / hierarchical retrieval is natural here.
- **Index** — the searchable structure built from nodes. `VectorStoreIndex` is the standard (embeddings + vector search). Others exist (SummaryIndex, KeywordTableIndex, KnowledgeGraphIndex).
- **Retriever** — pulls the top-K relevant nodes for a query.
- **QueryEngine** — the end-to-end "ask a question, get a cited answer" object: `index.as_query_engine()`. It retrieves, builds the prompt, calls the LLM, and returns a response with **source_nodes**.

```python
from llama_index.core import VectorStoreIndex, SimpleDirectoryReader

docs = SimpleDirectoryReader("data").load_data()   # Documents
index = VectorStoreIndex.from_documents(docs)      # chunk -> Nodes -> embed -> index
qe = index.as_query_engine()                       # QueryEngine
resp = qe.query("What is the late invoice penalty?")
print(resp)                # the answer
print(resp.source_nodes)   # the chunks it used — citations for free
```

That's a full RAG pipeline in four lines — the "RAG-specialized" value.

---

## 3. LangChain vs LlamaIndex — the core difference

| | **LangChain** | **LlamaIndex** |
|---|---|---|
| Primary purpose | General LLM orchestration (chains, agents, tools) | Data-centric RAG (ingest, index, query) |
| Best at | Complex multi-step workflows, agents, tool use | Retrieval quality, indexing strategies, cited Q&A |
| Abstraction | Broad and flexible (more building blocks) | Focused and opinionated for RAG |
| When it wins | You need agents, tools, and orchestration beyond RAG | Your core problem *is* retrieval over your data |
| Citations | You wire them up | `source_nodes` out of the box |

**The senior answer:** "LangChain is a general orchestration framework; LlamaIndex is RAG-specialized. If the core problem is high-quality retrieval and cited Q&A over a document corpus, I reach for LlamaIndex — it gives better indexing abstractions and citations with less code. If I need agents, tools, and multi-step orchestration around the RAG, LangChain (or Semantic Kernel on the .NET side) fits better. They also compose — LlamaIndex as the retrieval layer inside a LangChain/agent workflow."

---

## 4. When to choose LlamaIndex vs LangChain (decision table)

| Situation | Choose |
|---|---|
| Q&A over a document corpus, want citations, minimal code | **LlamaIndex** |
| Advanced indexing (hierarchical, auto-merging, knowledge-graph) | **LlamaIndex** |
| Multi-step agent with tools, memory, branching logic | **LangChain** / LangGraph |
| Enterprise .NET/Azure production | **Semantic Kernel** |
| RAG *inside* a larger agent workflow | **LlamaIndex retriever + LangChain agent** |

---

## 5. LlamaIndex with local models (Ollama)

Nothing forces a paid API. Configure the global `Settings` to use an Ollama LLM and a local embedding model, and the whole pipeline runs offline:

```python
from llama_index.core import Settings
from llama_index.llms.ollama import Ollama
from llama_index.embeddings.huggingface import HuggingFaceEmbedding

Settings.llm = Ollama(model="llama3")                                  # local generation
Settings.embed_model = HuggingFaceEmbedding("all-MiniLM-L6-v2")        # local embeddings
```

This is the same "swap the backend, keep the architecture" move as modules 01 and 04 — LlamaIndex just makes the RAG layer terse.

---

## 6. What LlamaIndex adds beyond naive RAG

- **Node relationships** → parent-child / auto-merging retrieval (retrieve small, return the merged parent — the L13 parent-child pattern, built in).
- **Multiple index types** — vector, summary, keyword, knowledge-graph — and **router/composable** indexes that pick the right one per query.
- **Response synthesizers** — strategies for combining retrieved nodes into an answer (refine, tree-summarize) for long contexts.
- **Built-in evaluators** — faithfulness/relevancy evaluation (overlaps with RAGAS from module 03).

You don't need all of it day one — but naming these in an interview shows you know LlamaIndex is more than "LangChain but for RAG."

---
*Next: `02_architecture.md`*
