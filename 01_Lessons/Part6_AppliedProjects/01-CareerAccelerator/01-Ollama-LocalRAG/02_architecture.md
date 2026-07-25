# 02 — Architecture: Local RAG with Ollama

## The pipeline

```
                          INGESTION (one time / on document change)
  ┌────────────────┐   ┌───────────┐   ┌─────────────────────────┐   ┌──────────────┐
  │ Source docs    │──▶│ Chunker   │──▶│ Embedding model         │──▶│ FAISS index  │
  │ (.txt/.pdf/.md)│   │ 512 tok,  │   │ sentence-transformers   │   │ (on disk)    │
  └────────────────┘   │ overlap   │   │ all-MiniLM-L6-v2 (384d) │   └──────────────┘
                       └───────────┘   └─────────────────────────┘

                          QUERY (every user question)
  ┌────────────┐   ┌─────────────────────┐   ┌──────────────┐   ┌────────────────────┐
  │ User query │──▶│ Embed query         │──▶│ FAISS search │──▶│ Top-K chunks       │
  └────────────┘   │ (same model)        │   │ (cosine/L2)  │   │ (+ source refs)    │
                   └─────────────────────┘   └──────────────┘   └─────────┬──────────┘
                                                                          │
                   ┌──────────────────────────────────────────────────────▼──────────┐
                   │ Build prompt:  [system] + [retrieved chunks as context] + [query]│
                   └──────────────────────────────────────────────────────┬──────────┘
                                                                          │
                   ┌──────────────────────────────────────────────────────▼──────────┐
                   │ Ollama server  :11434   →   LLaMA 3 / Mistral  (local inference) │
                   └──────────────────────────────────────────────────────┬──────────┘
                                                                          │
                                            ┌─────────────────────────────▼──────────┐
                                            │ Grounded answer + [Source N] citations │
                                            └────────────────────────────────────────┘

  Everything runs on one machine. Zero cloud calls. Zero per-token cost.
```

## Component breakdown

| Component | Role | Azure equivalent |
|---|---|---|
| **Ollama server** (`:11434`) | Hosts the LLM, exposes chat + embeddings HTTP API. Started with `ollama serve`. | Azure OpenAI endpoint |
| **Model file** (GGUF) | The quantized weights on disk, e.g. `llama3` (~4.7 GB). Pulled once with `ollama pull`. | A GPT-4o deployment |
| **Embedding model** | `sentence-transformers/all-MiniLM-L6-v2` (384-dim, tiny, fast) turns text → vectors. Runs in-process. | `text-embedding-3-small` |
| **FAISS index** | In-memory/on-disk vector store; nearest-neighbor search over chunk embeddings. | Azure AI Search vector index |
| **Chunker** | Splits docs into ~512-token overlapping pieces before embedding. | Same (AI Search skillset split) |
| **Orchestrator** (`04_hands_on.py`) | Wires it together: ingest → retrieve → prompt → generate. | Your Azure Function / SK orchestrator |

## Data flow notes

- **Embedding dimension must be consistent.** `all-MiniLM-L6-v2` outputs 384 dims — the FAISS index is created with `dimension=384`. Swap the embedding model → rebuild the index (same rule as Azure: you can't mix embedding models in one index).
- **The embedding model and the LLM are separate models.** The embedding model (MiniLM) is for retrieval; the LLM (LLaMA 3) is for generation — exactly like `text-embedding-3` vs GPT-4o in your Azure pipeline.
- **Grounding is prompt-enforced.** The system prompt instructs the model to answer only from the provided chunks and to cite `[Source N]` — the same anti-hallucination pattern you use in Azure RAG.

## Scaling this beyond a laptop

For a real air-gapped deployment you'd containerize Ollama and run it on GPU nodes:
```
Kubernetes / Docker
  ├── ollama container (GPU node)         ← model serving
  ├── app container (RAG orchestrator)    ← your Python/C# service
  └── FAISS persisted to a volume, OR a real vector DB (Qdrant/Weaviate) for HA
```
This is the on-prem mirror of your Azure AKS + AI Search production topology.

---
*Next: `03_interview_qa.md`*
