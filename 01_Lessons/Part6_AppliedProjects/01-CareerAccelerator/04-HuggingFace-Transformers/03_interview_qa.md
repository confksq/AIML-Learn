# 03 — Interview Q&A: Hugging Face Transformers (15 questions, senior level)

---

**Q1. What is Hugging Face, and how does it relate to Azure AI Foundry?**
Hugging Face is the open-source AI ecosystem: the Hub (a registry of 500k+ models, datasets, and demo Spaces), the `transformers` library to run them, `datasets`, `sentence-transformers`, and PEFT for fine-tuning. It's the open-source counterpart to Azure AI Foundry's Model Catalog + SDK — instead of deploying a managed GPT-4o endpoint, you download a model by name and run it locally.

**Q2. What does the `pipeline()` API do?**
It wraps tokenizer + model + task-specific post-processing into a single callable for a task — `pipeline("text-generation")`, `pipeline("zero-shot-classification")`, etc. It's the fastest way to run a model without managing the tokenizer and model heads yourself, and it's the same shape across every task.

**Q3. Why must the tokenizer match the model?**
Each model was trained with a specific tokenizer (BPE, WordPiece, or SentencePiece) and a specific vocabulary. The tokenizer maps text to the exact token IDs the model's embedding layer expects. Load a mismatched tokenizer and you feed the model wrong IDs, producing garbage. `AutoTokenizer.from_pretrained(name)` guarantees the matching pair.

**Q4. What's the difference between AutoModelForCausalLM, AutoModelForSequenceClassification, and AutoModel?**
The class encodes the task head. `AutoModelForCausalLM` adds a next-token head for generation (GPT-style). `AutoModelForSequenceClassification` adds a classification head. `AutoModel` gives the raw hidden states/features (used for embeddings). Same base transformer, different head for the task.

**Q5. What is zero-shot classification and how does it work without training?**
You give the model candidate labels at inference time and it scores which label the text entails — no task-specific training. Models like `facebook/bart-large-mnli` are trained on natural language inference, so they can judge "does this text entail the label 'billing'?" for any labels you pass. It's the fast path when you don't have labeled data to train a custom classifier.

**Q6. How do you choose a model on the Hub?**
Filter by task (text-generation, embeddings, classification), size (does it fit your hardware — CPU vs GPU, memory), license (Apache/MIT vs gated like Llama that needs license acceptance + a token), and popularity/downloads as a quality signal. Same discipline as Azure Foundry: pick by task fit and constraints, not fame.

**Q7. Which models can you run locally vs which need the Inference API?**
Small models — distilgpt2, all-MiniLM-L6-v2, bart-large-mnli, TinyLlama, Phi-3-mini — run on CPU locally, free. Large models — Llama-3-70B, Mixtral — need a big GPU or HF's hosted Inference API/Endpoints. For a portfolio you use small local models to prove you can operate the stack without a data center.

**Q8. How is sentence-transformers different from a generative model?**
sentence-transformers produces embeddings — dense vectors for retrieval/similarity — it's the `text-embedding-3` equivalent. A generative model (CausalLM) produces text. They're separate models for separate jobs; a RAG pipeline uses both (embeddings for retrieval, a CausalLM for generation).

**Q9. When would you use Hugging Face over Azure OpenAI?**
When I need open-source models I can run on-prem or fine-tune freely, a specialized task model that isn't a chat LLM (NER, ASR, zero-shot classification), or to source a model for an air-gapped deployment. I stay on Azure OpenAI for managed, compliant, frontier-quality production. Often it's hybrid — HF for embeddings/specialized tasks, Azure OpenAI for frontier generation.

**Q10. What are the trade-offs of running open-source models locally?**
You get control (own the weights, run anywhere, fine-tune, no per-token cost) at the price of quality (best open models trail GPT-4o), ops burden (you run/scale/patch/secure it), and no managed compliance/SLA/Managed-Identity unless you build it. The architect decides when that trade is worth it — usually data residency, cost at scale, or custom-task needs.

**Q11. How does model caching work, and why does it matter behind a corporate proxy?**
`from_pretrained` downloads model files once into `~/.cache/huggingface` (or `HF_HOME`) and reuses them. Behind a proxy that blocks the Hub (e.g., Zscaler), you pre-seed the cache from an allowed network or set `HF_HOME` to a shared cache, then run offline with `HF_HUB_OFFLINE=1`. This is a real enterprise consideration.

**Q12. Build a full RAG pipeline using only Hugging Face — what components?**
A sentence-transformers model for embeddings, FAISS (or Chroma) for the vector store, and a `transformers` text-generation model (or a local Ollama model) for generation. Ingest: chunk → embed → index. Query: embed → search → build prompt → generate. Every component comes from the open-source stack, no paid API — the same RAG architecture as Azure OpenAI + AI Search.

**Q13. What is a gated model and how do you access one?**
Some models (Llama family, some Mistral) require accepting a license on the Hub and authenticating with an HF token (`huggingface-cli login`). Until you accept and authenticate, `from_pretrained` returns a 403. Enterprises track which licenses they've accepted as part of model governance.

**Q14. How does Hugging Face fit into fine-tuning?**
HF is the standard fine-tuning stack: load a base model + tokenizer from the Hub, apply LoRA/QLoRA adapters via the PEFT library, train with the `Trainer` or `SFTTrainer`, and push the adapter back to the Hub. This is exactly the L8 module — HF is where open-source fine-tuning happens, the counterpart to Azure OpenAI fine-tuning.

**Q15. A JD lists 'Hugging Face, PyTorch, and transformers' — how do you frame your fit given an Azure background?**
I understand what these models do from running Azure OpenAI in production, and I work the open-source stack directly: `transformers`/`pipeline` for generation and task models, sentence-transformers for embeddings, FAISS for retrieval, and PEFT for LoRA fine-tuning. I can build the same RAG and agent architectures on either stack and choose between them on data-residency, cost, and quality criteria.

---
*Frame every answer as "the Azure equivalent is X, and here's the HF way" — you're mapping known concepts, not learning them fresh.*
