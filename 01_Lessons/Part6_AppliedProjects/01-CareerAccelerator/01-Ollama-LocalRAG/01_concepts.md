# 01 — Concepts: Ollama & Local LLMs

> **Bridge from what you already know:** everything here maps 1:1 to the Azure stack you run at JM Family. You're not learning new *concepts* — you're learning the open-source *equivalents*.

---

## 1. The one-sentence mental model

**Ollama is Azure OpenAI Service running on your own machine.** Same request shape, same chat/embeddings endpoints — just `localhost:11434` instead of `https://your-resource.openai.azure.com`, and open-source models (LLaMA 3, Mistral) instead of GPT-4o.

| You know (Azure) | Local equivalent | Same idea |
|---|---|---|
| Azure OpenAI **endpoint** | **Ollama server** (`:11434`) | Hosts the model, exposes an HTTP API |
| GPT-4o **deployment** | `ollama pull llama3` | The specific model you call |
| Azure OpenAI Chat Completions API | Ollama `/api/chat` (OpenAI-compatible) | Send messages, get a completion |
| `text-embedding-3-large` | `sentence-transformers` / Ollama embeddings | Text → vector |
| **Azure AI Search** (vector index) | **FAISS** / Chroma | Store vectors, nearest-neighbor search |
| Managed Identity + TPM quota | Nothing — it's local | No auth, no rate limit, no bill |

---

## 2. Why companies run local LLMs

Three drivers, all of which show up in JDs:

1. **Compliance / air-gapped** — regulated data (PHI, financial, classified) legally can't leave the environment. A local model means prompts and documents never touch the public internet. (This is the healthcare/VitalCare angle you already understand.)
2. **Cost at scale** — Azure OpenAI bills per token. At millions of calls/month, on-prem hardware can be cheaper than the per-call bill.
3. **Data privacy / IP** — startups and enterprises that don't want their proprietary prompts/data used by, or even transiting, a third-party API.

**The honest trade-off:** local models (LLaMA 3 8B, Mistral 7B) are smaller and less capable than GPT-4o. You trade top-end quality for privacy, cost, and control. The architect's job is knowing *when that trade is worth it*.

---

## 3. The Ollama REST API (mirrors OpenAI)

Ollama exposes two APIs. The important one for you: it has an **OpenAI-compatible endpoint**, so your existing OpenAI SDK code works by changing only the base URL.

```python
# This is the OpenAI SDK — pointed at Ollama instead of Azure
from openai import OpenAI
client = OpenAI(base_url="http://localhost:11434/v1", api_key="ollama")  # key is ignored locally

resp = client.chat.completions.create(
    model="llama3",
    messages=[{"role": "user", "content": "What is RAG?"}]
)
print(resp.choices[0].message.content)
```

Ollama also has a **native API** (`/api/generate`, `/api/chat`, `/api/embeddings`) with streaming and model-management endpoints. Both work; the OpenAI-compatible one is the fastest path when porting existing code.

---

## 4. Supported models (the ones that matter)

| Model | Size | Best for |
|---|---|---|
| **LLaMA 3** (8B / 70B) | 4.7 GB / 40 GB | General-purpose; the default choice |
| **Mistral** (7B) | 4.1 GB | Fast, strong reasoning per parameter |
| **Phi-3 mini** (3.8B) | 2.3 GB | Tiny, cheap, near-GPT-4 on many tasks — Microsoft model |
| **Gemma** (2B / 7B) | 1.7 GB / 5 GB | Google's open models |

`ollama pull <model>` downloads a **quantized** version by default (see below), which is why an "8B" model fits in ~4.7 GB instead of ~16 GB.

---

## 5. Quantization (the one genuinely new term)

Full-precision model weights are 16-bit floats (2 bytes each). **Quantization** stores them at lower precision (8-bit, 4-bit) to shrink the model and let it run on consumer hardware — with a small quality loss.

```
LLaMA 3 8B, full precision (FP16):  ~16 GB  → needs a big GPU
LLaMA 3 8B, 4-bit quantized (Q4):   ~4.7 GB → runs on a laptop/small GPU
```

- **GGUF** is the file format Ollama (and llama.cpp) use for quantized models.
- `Q4_K_M`, `Q5`, `Q8` are quantization levels — lower number = smaller/faster, higher = better quality.
- You already met this idea in the fine-tuning modules (QLoRA quantizes the frozen base to 4-bit). Same concept, applied to *serving* instead of *training*.

---

## 6. FAISS — your local vector store

FAISS (Facebook AI Similarity Search) does what Azure AI Search's vector index does: store embeddings and answer "which stored vectors are nearest to this query vector?"

| Azure AI Search | FAISS |
|---|---|
| HNSW ANN index | `IndexHNSWFlat` (same algorithm) |
| Exhaustive KNN | `IndexFlatL2` / `IndexFlatIP` (exact, brute force) |
| Cosine similarity | Normalize vectors + inner product (`IndexFlatIP`) |
| Managed service | In-process library (a file on disk) |

For small local corpora, `IndexFlatL2` (exact) is fine. For large ones, `IndexHNSWFlat` gives the same approximate-nearest-neighbor speed-up you get from Azure AI Search's HNSW.

**What FAISS does NOT give you** (and Azure AI Search does): hybrid keyword+vector search, a semantic re-ranker, metadata filtering as a managed feature. In local RAG you'd add those yourself (e.g., BM25 via `rank_bm25`, a cross-encoder re-ranker from sentence-transformers).

---

## 7. Chunking (unchanged from your Azure RAG)

Chunking works exactly as in Azure — 512-token chunks with overlap, recursive splitting, preserve structure. The embedding model changes (sentence-transformers instead of `text-embedding-3`), but the strategy is identical. See the L13 RAG Deep Dive curriculum for the full treatment; nothing about chunking is local-specific.

---

## 8. Decision table — Ollama vs Azure OpenAI

| Factor | Choose **Ollama (local)** | Choose **Azure OpenAI** |
|---|---|---|
| Data residency | Air-gapped / can't leave premises | Cloud is compliant enough |
| Top-end quality | Acceptable with 8B–70B models | Need GPT-4o / o1 reasoning |
| Cost profile | Very high volume, own the hardware | Variable/moderate volume |
| Latency | Ultra-low, on-device, no network | Network round-trip acceptable |
| Ops burden | You run/scale/patch the server | Microsoft manages it |
| Enterprise integration | DIY (no Managed Identity, no APIM) | Native Azure AD, Private Link, Monitor |

**The senior answer:** "I'd default to Azure OpenAI for managed compliance and top-end quality, and reach for Ollama specifically when the data is air-gapped, the volume makes per-token pricing painful, or ultra-low on-device latency is required — often as a **hybrid** where a local model handles a triage/classification tier and the cloud model handles the hard cases."

---
*Next: `02_architecture.md` — the pipeline diagram and component breakdown.*
