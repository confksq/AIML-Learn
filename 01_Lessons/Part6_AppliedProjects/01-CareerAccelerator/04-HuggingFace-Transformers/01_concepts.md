# 01 — Concepts: Hugging Face

> **Bridge from what you already know:** Hugging Face is the **open-source Azure AI Foundry** — a model catalog + libraries to run those models. You know GPT-4o and `text-embedding-3`; HF is where the open-source equivalents live and how you run them.

---

## 1. The one-sentence mental model

**The Hugging Face Hub is Azure AI Foundry's Model Catalog, and the `transformers` library is the SDK to run those models locally.** Instead of deploying a GPT-4o endpoint, you download a model from the Hub and run it in-process.

| You know (Azure) | Hugging Face equivalent |
|---|---|
| Azure AI Foundry **Model Catalog** (1,600+ models) | **HF Hub** (500,000+ models) |
| Azure OpenAI **deployment** | `AutoModel.from_pretrained("...")` — load a model by name |
| Azure OpenAI SDK call | `pipeline("task")(input)` — the one-liner API |
| `text-embedding-3` | `sentence-transformers` models |
| Content Understanding / task services | HF task **pipelines** (classification, NER, summarization, ASR…) |
| Azure ML datasets | HF **Datasets** library |
| Fine-tuning in Foundry | HF **PEFT** (LoRA/QLoRA) + `Trainer` |

---

## 2. The five parts of the HF ecosystem

1. **Hub** (huggingface.co) — the registry. Models, datasets, and Spaces (demo apps). Every model has a card, files, and a name like `mistralai/Mistral-7B-Instruct-v0.3` (`org/model`).
2. **`transformers`** — the core Python library to load and run any model (`AutoModel`, `AutoTokenizer`, `pipeline`).
3. **`datasets`** — load/stream datasets in one line (`load_dataset("...")`). The format RAGAS also uses.
4. **`sentence-transformers`** — embedding models for retrieval/similarity (built on transformers).
5. **PEFT** — parameter-efficient fine-tuning (LoRA/QLoRA) — covered in module 08.
   Plus **Inference API / Endpoints** — HF's hosted option if you don't want to run locally.

---

## 3. `pipeline()` — the one-liner you'll use constantly

`pipeline()` wraps tokenizer + model + post-processing for a task into a single callable. It's the fastest way to *do* something with HF:

```python
from transformers import pipeline

# text generation
gen = pipeline("text-generation", model="distilgpt2")
gen("The future of AI is", max_new_tokens=30)

# zero-shot classification (no training!)
clf = pipeline("zero-shot-classification", model="facebook/bart-large-mnli")
clf("The invoice is 45 days overdue", candidate_labels=["billing", "delivery", "warranty"])

# sentiment, NER, summarization, translation, ASR ... all the same shape
```

Common tasks: `text-generation`, `zero-shot-classification`, `sentiment-analysis`, `ner`, `summarization`, `translation`, `automatic-speech-recognition`, `feature-extraction` (embeddings).

---

## 4. Tokenizer + Model (what `pipeline` hides)

Under the one-liner, every model needs a matching tokenizer (they're a pair):

```python
from transformers import AutoTokenizer, AutoModelForCausalLM

tok = AutoTokenizer.from_pretrained("distilgpt2")     # text <-> token IDs
model = AutoModelForCausalLM.from_pretrained("distilgpt2")  # the weights

ids = tok("Hello world", return_tensors="pt")         # encode
out = model.generate(**ids, max_new_tokens=20)        # run
print(tok.decode(out[0]))                             # decode
```

- **Tokenizer must match the model** — each model was trained with a specific tokenizer (BPE/WordPiece/SentencePiece — the curriculum L11_2 material). Loading the wrong one produces garbage.
- **`AutoModelForCausalLM`** = generation (GPT-style). **`AutoModelForSequenceClassification`** = classification. **`AutoModel`** = raw embeddings/features. The class encodes the task head.

---

## 5. Finding a model on the Hub

Filter by **task** (text-generation, embeddings…), **size** (can it run on your hardware?), **license** (Apache/MIT vs gated like Llama), and **popularity/downloads**. Model names are `org/model`. Gated models (Llama, some Mistral) require accepting a license and an HF token.

The architect's move — same as Azure Foundry: don't grab the most famous model; pick by task fit, size that runs on your hardware, and license.

---

## 6. Which models run locally vs need the Inference API

| Runs locally (free) | Needs Inference API / Endpoints (or big GPU) |
|---|---|
| Small models: `distilgpt2`, `all-MiniLM-L6-v2`, `bart-large-mnli`, TinyLlama, Phi-3-mini | Large models: Llama-3-70B, Mixtral, big generative models |
| CPU-friendly embeddings + classification | High-throughput generation at scale |

This module deliberately uses **small local models** so everything runs without a GPU or a paid API — proving you can operate the stack, not that you have a data center.

---

## 7. HF vs Azure OpenAI — decision table

| Factor | Hugging Face | Azure OpenAI |
|---|---|---|
| Model choice | 500k+ open models, any task | Curated frontier models (GPT-4o, o1) |
| Top-end quality | Best open models trail GPT-4o | Frontier quality |
| Control | Full — own weights, run anywhere, fine-tune freely | Managed endpoint, provider controls the model |
| Cost | Free to run locally; you pay for compute | Per-token |
| Enterprise ops | DIY (no Managed Identity/SLA unless you build it) | Native Azure AD, Private Link, SLA, compliance |
| Best for | Open-source/on-prem, research, custom tasks, fine-tuning | Managed production, frontier quality, compliance |

**The senior answer:** "I use Azure OpenAI for managed, compliant, frontier-quality production, and Hugging Face when I need open-source models I can run on-prem or fine-tune freely, or a specialized task model (NER, classification, ASR) that isn't a chat LLM. HF is also where I'd source and run a model for an air-gapped deployment via Ollama/local serving."

---
*Next: `02_architecture.md`*
