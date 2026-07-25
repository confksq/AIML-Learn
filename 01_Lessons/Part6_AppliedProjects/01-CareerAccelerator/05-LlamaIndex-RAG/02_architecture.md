# 02 — Architecture: LlamaIndex RAG

## The pipeline

```
                        INGESTION (index build)
  ┌────────────────┐   ┌──────────────┐   ┌───────────────┐   ┌─────────────────────┐
  │ SimpleDirectory│──▶│ Documents    │──▶│ Node parser   │──▶│ VectorStoreIndex     │
  │ Reader("data") │   │ (loaded src) │   │ chunk -> Nodes│   │ embed + store vectors│
  └────────────────┘   └──────────────┘   │ (+metadata,   │   └─────────────────────┘
                                          │  relationships)│
                                          └───────────────┘
                                                  ▲
                                          embed_model (Settings)
                                          = HuggingFaceEmbedding / local

                        QUERY
  ┌────────────┐   ┌──────────────────────────────────────────────────────────────┐
  │ user query │──▶│ QueryEngine  =  Retriever + prompt + LLM + response synthesizer │
  └────────────┘   └───────────────────────────┬──────────────────────────────────┘
                                               │  llm (Settings) = Ollama / local
                                               ▼
                              ┌────────────────────────────────────┐
                              │ Response                           │
                              │   .response      -> the answer     │
                              │   .source_nodes  -> citations      │
                              └────────────────────────────────────┘

  Backend fully local: Ollama LLM + HuggingFace embeddings. No paid API.
```

## Component breakdown

| Component | Role | Your RAG / Azure equivalent |
|---|---|---|
| **SimpleDirectoryReader** | Loads files from a folder into Documents | Document Intelligence / a loader |
| **Document** | A loaded source item | A source file |
| **Node parser** | Splits Documents into Nodes (chunks + metadata + relationships) | Your chunker (512-token, overlap) |
| **Node** | A chunk with metadata and parent/sibling links | A chunk with metadata |
| **VectorStoreIndex** | Embeds nodes and stores vectors; searchable | Azure AI Search vector index |
| **embed_model** | The embedding model (set globally via Settings) | text-embedding-3 |
| **Retriever** | Returns top-K relevant nodes | Your retrieval step |
| **QueryEngine** | Retrieve → prompt → LLM → synthesize → cited answer | Your RAG orchestrator |
| **llm** | The generation model (set globally via Settings) | Azure OpenAI GPT-4o / local Ollama |
| **Response.source_nodes** | The chunks used, for citations | Your citation list |

## Data flow notes

- **`Settings` is the global config.** Set `Settings.llm` and `Settings.embed_model` once and every index/query engine uses them. This is where you swap cloud↔local without touching pipeline code.
- **Citations are free.** `response.source_nodes` gives you the chunks (with scores and metadata) the answer was built from — the grounding/citation discipline is built in rather than hand-wired.
- **Node relationships enable advanced retrieval.** Because Nodes track parent/sibling links, LlamaIndex can do auto-merging / parent-child retrieval (retrieve precise small nodes, return the merged larger context) — the L13 parent-child pattern as a first-class feature.

## Scaling beyond in-memory

`VectorStoreIndex.from_documents` uses an in-memory store by default. For production, plug in a real vector store (Qdrant, Weaviate, pgvector, or Azure AI Search) via the corresponding `VectorStore` integration — the pipeline code stays the same, only the storage backend changes. Same portability principle as swapping FAISS for Azure AI Search.

---
*Next: `03_interview_qa.md`*
