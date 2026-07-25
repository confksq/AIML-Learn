# 03 — Interview Q&A: Ollama & Local LLMs (15 questions, senior level)

---

**Q1. When would you recommend a local LLM (Ollama) over Azure OpenAI?**
When data is air-gapped or regulated and legally can't leave the environment; when per-token cost at high volume exceeds owning the hardware; when ultra-low on-device latency is required; or when data-privacy/IP concerns rule out any third-party API. I default to Azure OpenAI for managed compliance and top-end quality, and reach for local only when one of those hard constraints applies — often as a hybrid where a small local model triages and the cloud model handles hard cases.

**Q2. What is Ollama, technically?**
A local model server that wraps llama.cpp, manages model downloads (GGUF files), and exposes an HTTP API on port 11434 — including an OpenAI-compatible endpoint (`/v1/chat/completions`). It lets you run open-source models (LLaMA 3, Mistral, Phi-3) locally with the same request shape as the OpenAI/Azure OpenAI SDKs.

**Q3. How do you point existing OpenAI SDK code at Ollama?**
Change only the base URL and pass a dummy key: `OpenAI(base_url="http://localhost:11434/v1", api_key="ollama")`. Because Ollama implements the OpenAI-compatible API, the rest of the code — messages, model name, streaming — is unchanged. This is the same portability argument as Azure OpenAI using the OpenAI SDK.

**Q4. What is quantization and why does it matter for local serving?**
Storing model weights at lower precision (8-bit, 4-bit) instead of 16-bit floats, shrinking the model so it runs on consumer hardware with a small quality loss. LLaMA 3 8B is ~16 GB at FP16 but ~4.7 GB at 4-bit (Q4) — the difference between "needs a data-center GPU" and "runs on a laptop." Ollama pulls quantized GGUF files by default.

**Q5. What is GGUF, and how does it relate to other quantization formats?**
GGUF is the file format used by llama.cpp/Ollama for quantized models optimized for CPU/GPU inference. Alternatives: AWQ (activation-aware quantization, GPU-focused) and EXL2 (ExLlama format). GGUF is the most portable for local/CPU serving; AWQ/EXL2 target GPU throughput. Quant levels like Q4_K_M / Q5 / Q8 trade size for quality.

**Q6. How is FAISS different from Azure AI Search?**
FAISS is an in-process vector-search library (a file on disk), not a managed service. It gives you the same ANN algorithms (HNSW) and exact search (Flat) as Azure AI Search's vector layer, but not the managed extras: hybrid keyword+vector search, a built-in semantic re-ranker, metadata filtering, RBAC, or HA. In local RAG you add those yourself (BM25 for keywords, a cross-encoder for re-ranking).

**Q7. Which FAISS index type would you choose and why?**
`IndexFlatL2`/`IndexFlatIP` (exact, brute-force) for small corpora — accurate, simple, fine up to tens of thousands of vectors. `IndexHNSWFlat` for large corpora — approximate nearest neighbor, the same HNSW graph Azure AI Search uses, O(log n) search at ~99% recall. Choose Flat for correctness on small data, HNSW when scale makes exact search too slow.

**Q8. Cosine similarity in FAISS — how do you actually get it?**
FAISS doesn't have a "cosine" index directly. You L2-normalize the vectors (unit length) and use `IndexFlatIP` (inner product) — inner product of normalized vectors equals cosine similarity. This is the same reason Azure recommends cosine for text embeddings: direction matters, magnitude doesn't.

**Q9. The embedding model and the LLM — same model or different?**
Different, and it's a common confusion. The embedding model (e.g., sentence-transformers all-MiniLM-L6-v2, 384-dim) exists only to turn text into retrieval vectors. The LLM (LLaMA 3) generates the answer. Exactly like `text-embedding-3` vs GPT-4o in Azure — two separate models with two separate jobs.

**Q10. How do you prevent hallucination in a local RAG pipeline?**
Same layered defense as cloud RAG: a system prompt instructing "answer only from the provided context; if it's not there, say you don't know"; force `[Source N]` citations so ungrounded claims are visible; a retrieval-score/confidence gate; and low temperature. Local models actually hallucinate *more* than GPT-4o, so grounding discipline matters more, not less.

**Q11. What's the honest quality trade-off with local models?**
An 8B–7B local model is meaningfully less capable than GPT-4o on complex reasoning, long-context, and nuanced instruction-following. You trade top-end quality for privacy, cost, and control. Mitigations: use the largest model your hardware allows (LLaMA 3 70B if you have the GPU), keep tasks narrow, and use strong retrieval so the model has less to "reason" about.

**Q12. How would you serve Ollama in production, not just on a laptop?**
Containerize Ollama on GPU nodes in Kubernetes/AKS-equivalent, put your RAG orchestrator in a separate container, and persist FAISS to a volume — or graduate to a real vector DB (Qdrant/Weaviate) for HA and filtering. Add a gateway for rate control and observability. It's the on-prem mirror of an Azure AKS + AI Search topology.

**Q13. A regulated healthcare client can't send PHI to any cloud API but wants a document Q&A assistant. Walk the architecture.**
Fully local: Ollama (LLaMA 3) for generation, sentence-transformers for embeddings, FAISS for retrieval, all on-prem GPU hardware behind the firewall. Documents ingested locally, chunked, embedded, indexed — PHI never leaves the environment. Add PII handling, audit logging, and access control at the app layer. This is exactly the air-gapped scenario local LLMs exist for.

**Q14. What does Ollama NOT give you that Azure OpenAI does?**
Managed compliance certifications, Azure AD/Managed Identity auth, Private Link networking, TPM quota management, Azure Monitor integration, an SLA, automatic model updates, and frontier-model quality (GPT-4o/o1). You take on the ops burden of running, scaling, patching, and securing the server yourself — the classic self-hosted vs managed trade.

**Q15. How would you combine local and cloud models cost-effectively?**
Model tiering: a cheap local model (Phi-3 mini via Ollama) handles high-volume simple tasks — classification, routing, triage — and only escalates complex/ambiguous cases to Azure OpenAI GPT-4o. You get local's cost/latency on the bulk of traffic and the cloud model's quality where it actually matters. Same tiering logic as GPT-4o-mini → GPT-4o routing, extended across the local/cloud boundary.

---
*These bridge your existing Azure RAG knowledge to the open-source stack — answer them in terms of "the Azure equivalent is X" to show senior-level range.*
