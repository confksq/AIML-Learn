"""
04d_rag_with_hf.py — Full RAG using ONLY Hugging Face + FAISS (no paid API)

Every component comes from the open-source stack:
    embeddings  -> sentence-transformers (all-MiniLM-L6-v2)
    vector store-> FAISS (Azure AI Search equivalent)
    generation  -> HF text-generation pipeline (google/flan-t5-base, CPU-friendly)

Azure bridge: this is the exact RAG architecture you run with Azure OpenAI + AI Search,
rebuilt entirely on Hugging Face so it runs locally with no cloud dependency.

Run:
    pip install -r requirements.txt
    python 04d_rag_with_hf.py
"""

import numpy as np
import faiss
from sentence_transformers import SentenceTransformer
from transformers import pipeline

EMBED_MODEL = "all-MiniLM-L6-v2"
# flan-t5 is instruction-tuned and small -> good, cheap local answers for RAG demos.
GEN_MODEL = "google/flan-t5-base"
TOP_K = 2

DOCUMENTS = [
    "Dealer invoices must be submitted within 30 days of the vehicle delivery date. "
    "Late submissions incur a 2% penalty per month on the invoice total.",
    "The dealer reserve is a portion of finance income withheld and paid to the dealer over time, "
    "released once the retail contract meets its performance thresholds.",
    "Curtailment (floorplan financing) requires principal reductions on aged inventory. "
    "Vehicles unsold after 90 days trigger the first curtailment payment.",
    "Warranty claims must be submitted through the dealer portal within 60 days of the repair.",
]


def build_index(docs, embedder):
    vecs = embedder.encode(docs, normalize_embeddings=True).astype("float32")
    index = faiss.IndexFlatIP(vecs.shape[1])   # cosine via normalized inner product
    index.add(vecs)
    return index


def retrieve(query, embedder, index, docs, k=TOP_K):
    qv = embedder.encode([query], normalize_embeddings=True).astype("float32")
    _, idxs = index.search(qv, k)
    return [docs[i] for i in idxs[0]]


def main():
    print("Loading HF embedding + generation models (first run downloads them)...\n")
    embedder = SentenceTransformer(EMBED_MODEL)
    generator = pipeline("text2text-generation", model=GEN_MODEL)  # flan-t5 = seq2seq

    index = build_index(DOCUMENTS, embedder)

    question = "What is the penalty for a late dealer invoice?"
    contexts = retrieve(question, embedder, index, DOCUMENTS)

    # Build a grounded prompt (context + question) — same anti-hallucination pattern as Azure RAG
    context_block = " ".join(contexts)
    prompt = (
        f"Answer the question using only this context.\n"
        f"Context: {context_block}\n"
        f"Question: {question}\n"
        f"Answer:"
    )

    print(f"Question: {question}\n")
    print("Retrieved context:")
    for c in contexts:
        print(f"  - {c[:80]}...")
    print()

    answer = generator(prompt, max_new_tokens=60)[0]["generated_text"]
    print("ANSWER:", answer)
    print("\nEvery component (embeddings, vector store, LLM) is open-source and local.")
    print("Same RAG architecture as Azure OpenAI + AI Search — zero cloud dependency.")


if __name__ == "__main__":
    main()
