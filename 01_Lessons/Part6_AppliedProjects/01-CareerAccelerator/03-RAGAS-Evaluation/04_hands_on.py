"""
04_hands_on.py — RAG pipeline instrumented with RAGAS evaluation

What this demonstrates (formalizing the groundedness/A-B-testing you already do at JMA):
    1. Build a small RAG pipeline (FAISS + sentence-transformers + an LLM)
    2. Run the golden dataset (sample_questions.json) through it
    3. Collect { question, answer, contexts, ground_truth } per item
    4. Score with RAGAS: faithfulness, answer relevance, context recall, context precision
    5. Print a score table and flag the weakest metric (which stage to fix)

Run:
    pip install -r requirements.txt
    export OPENAI_API_KEY=...          # RAGAS uses an LLM-as-judge (GPT-4o by default)
    python 04_hands_on.py

Notes:
- The judge model is pinned via RAGAS config; changing it shifts all scores.
- Context Recall requires ground_truth (provided in sample_questions.json).
- To run fully local, configure RAGAS with an Ollama LLM + local embeddings (see comments).
"""

import json
import numpy as np
import faiss
from sentence_transformers import SentenceTransformer

# --- RAG generation backend (OpenAI here; swap base_url for Ollama to run local) ---
from openai import OpenAI

# --- RAGAS ---
from datasets import Dataset
from ragas import evaluate
from ragas.metrics import faithfulness, answer_relevancy, context_recall, context_precision

EMBED_MODEL = "all-MiniLM-L6-v2"      # local embeddings (text-embedding-3 equivalent)
GEN_MODEL = "gpt-4o-mini"             # RAG generation model. For local: point client at Ollama.
TOP_K = 3


# --------------------------------------------------------------------------------------
# Load the golden dataset (documents + question/ground_truth pairs)
# --------------------------------------------------------------------------------------
def load_dataset(path: str = "sample_questions.json") -> dict:
    with open(path, encoding="utf-8") as f:
        return json.load(f)


# --------------------------------------------------------------------------------------
# Build a tiny FAISS index over the documents (each doc is one chunk here for clarity)
# --------------------------------------------------------------------------------------
def build_index(documents: list[str], embedder: SentenceTransformer):
    vecs = embedder.encode(documents, normalize_embeddings=True).astype("float32")
    index = faiss.IndexFlatIP(vecs.shape[1])   # cosine via normalized inner product
    index.add(vecs)
    return index


def retrieve(query: str, embedder, index, documents, top_k=TOP_K) -> list[str]:
    qv = embedder.encode([query], normalize_embeddings=True).astype("float32")
    _, idxs = index.search(qv, top_k)
    return [documents[i] for i in idxs[0]]


# --------------------------------------------------------------------------------------
# Generate a grounded answer from retrieved contexts
# --------------------------------------------------------------------------------------
def generate(client: OpenAI, query: str, contexts: list[str]) -> str:
    context_block = "\n".join(f"- {c}" for c in contexts)
    messages = [
        {"role": "system", "content": (
            "Answer using ONLY the provided context. If the answer is not in the context, "
            "say the information is not available in the provided documents.")},
        {"role": "user", "content": f"Context:\n{context_block}\n\nQuestion: {query}"},
    ]
    resp = client.chat.completions.create(model=GEN_MODEL, messages=messages, temperature=0)
    return resp.choices[0].message.content.strip()


# --------------------------------------------------------------------------------------
# MAIN — run the pipeline over the golden dataset, then evaluate with RAGAS
# --------------------------------------------------------------------------------------
def main():
    data = load_dataset()
    documents = data["documents"]
    qa_pairs = data["qa_pairs"]

    print("Loading local embedding model...")
    embedder = SentenceTransformer(EMBED_MODEL)
    index = build_index(documents, embedder)

    # For local generation, use: OpenAI(base_url="http://localhost:11434/v1", api_key="ollama")
    client = OpenAI()

    print(f"Running {len(qa_pairs)} questions through the RAG pipeline...\n")
    records = {"question": [], "answer": [], "contexts": [], "ground_truth": []}
    for qa in qa_pairs:
        q = qa["question"]
        ctxs = retrieve(q, embedder, index, documents)
        ans = generate(client, q, ctxs)
        records["question"].append(q)
        records["answer"].append(ans)
        records["contexts"].append(ctxs)          # RAGAS needs the retrieved chunks
        records["ground_truth"].append(qa["ground_truth"])
        print(f"  Q: {q}\n  A: {ans}\n")

    # Build a HuggingFace Dataset (the format RAGAS expects)
    ds = Dataset.from_dict(records)

    print("Running RAGAS evaluation (LLM-as-judge)...\n")
    result = evaluate(
        ds,
        metrics=[faithfulness, answer_relevancy, context_recall, context_precision],
    )

    # --------------------------------------------------------------------------------------
    # Print score table + interpret the weakest metric (which stage to fix)
    # --------------------------------------------------------------------------------------
    scores = result.to_pandas()[["faithfulness", "answer_relevancy",
                                 "context_recall", "context_precision"]].mean()
    print("\n" + "=" * 52)
    print("RAGAS SCORES (0-1, higher is better)")
    print("=" * 52)
    labels = {
        "faithfulness": "Faithfulness   (grounding / hallucination)",
        "answer_relevancy": "Answer Relevance (addresses question)",
        "context_recall": "Context Recall  (retrieval found it)",
        "context_precision": "Context Precision (retrieval is clean)",
    }
    for key, label in labels.items():
        print(f"  {label:<42} {scores[key]:.3f}")

    weakest = scores.idxmin()
    fixes = {
        "faithfulness": "generation problem -> tighten grounding prompt, lower temperature",
        "answer_relevancy": "generation problem -> improve prompt template / question clarity",
        "context_recall": "retrieval problem -> better chunking/embeddings, hybrid search, raise top-K",
        "context_precision": "retrieval problem -> add re-ranking, lower top-K, metadata filter",
    }
    print("=" * 52)
    print(f"Weakest metric: {weakest} ({scores[weakest]:.3f})")
    print(f"Likely fix: {fixes[weakest]}")


if __name__ == "__main__":
    main()
