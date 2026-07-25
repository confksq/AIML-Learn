# 02 — Architecture: How Hugging Face Fits Together

## The ecosystem map

```
  ┌───────────────────────────────────────────────────────────────────┐
  │ HUGGING FACE HUB  (huggingface.co)   ≈ Azure AI Foundry catalog    │
  │   models (org/model)   ·   datasets   ·   Spaces (demo apps)       │
  └───────────────┬───────────────────────────────────────────────────┘
                  │ download by name (cached in ~/.cache/huggingface)
                  ▼
  ┌───────────────────────────────────────────────────────────────────┐
  │ transformers  (the SDK)                                            │
  │   AutoTokenizer  +  AutoModelForCausalLM / SequenceClassification  │
  │                          │                                         │
  │                          ▼                                         │
  │   pipeline("task")  ── one-liner wrapping tokenizer+model+postproc │
  └───────────────────────────────────────────────────────────────────┘
        │                 │                 │                 │
        ▼                 ▼                 ▼                 ▼
  text-generation   embeddings        classification     RAG (compose)
  (04a)             (sentence-        (zero-shot, 04c)   HF embed + FAISS
                    transformers,04b)                    + local LLM (04d)
```

## The four things this module builds (mapped to demos)

| Demo | Task | HF component | Azure equivalent |
|---|---|---|---|
| `04a_text_generation.py` | Generate text | `pipeline("text-generation")` | GPT-4o chat completion |
| `04b_embeddings.py` | Text → vectors + similarity | `sentence-transformers` | `text-embedding-3` + cosine |
| `04c_classification.py` | Label text without training | `pipeline("zero-shot-classification")` | Azure AI Language / custom classifier |
| `04d_rag_with_hf.py` | Full RAG | HF embeddings + FAISS + local generation | Azure OpenAI + AI Search RAG |

## The RAG demo (04d) data flow

```
  docs ─▶ chunk ─▶ HF sentence-transformer embed ─▶ FAISS index      (ingest)
  query ─▶ same embedder ─▶ FAISS search ─▶ top-K chunks             (retrieve)
       ─▶ build prompt (chunks + query) ─▶ HF text-generation model  (generate)
       ─▶ grounded answer                                            (answer)
```
This is the same RAG shape as module 01 (Ollama), but generation runs through a HF `pipeline` model instead of the Ollama server — showing you can source *every* component (embeddings + LLM) from the HF ecosystem.

## Component notes

- **Tokenizer + model are a matched pair** — `pipeline` loads both together; if you load manually, load both from the same model name.
- **First run downloads; later runs use cache** — models live in `~/.cache/huggingface`. Set `HF_HOME` to relocate (useful behind a proxy with a pre-seeded cache).
- **Model class encodes the task** — `AutoModelForCausalLM` (generation) vs `AutoModelForSequenceClassification` (classification) vs `AutoModel` (features/embeddings).
- **`device_map`/`torch_dtype`** control where/how the model runs (CPU vs GPU, fp16). The demos default to CPU-friendly small models.

---
*Next: `03_interview_qa.md`*
