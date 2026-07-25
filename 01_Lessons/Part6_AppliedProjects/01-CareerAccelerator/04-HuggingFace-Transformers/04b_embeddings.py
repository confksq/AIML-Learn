"""
04b_embeddings.py — Embeddings + cosine similarity semantic search (Hugging Face)

Demonstrates sentence-transformers (the text-embedding-3 equivalent) and semantic
search by cosine similarity — the retrieval half of RAG, done fully locally.

Azure bridge: all-MiniLM-L6-v2 (384-dim) <-> text-embedding-3-small; cosine similarity
is the same metric Azure AI Search uses for text vectors.

Run:
    pip install -r requirements.txt
    python 04b_embeddings.py
"""

import numpy as np
from sentence_transformers import SentenceTransformer

MODEL = "all-MiniLM-L6-v2"   # 384-dim, tiny, CPU-friendly


def cosine_similarity(a: np.ndarray, b: np.ndarray) -> float:
    return float(np.dot(a, b) / (np.linalg.norm(a) * np.linalg.norm(b)))


def main():
    print(f"Loading embedding model ({MODEL})...\n")
    embedder = SentenceTransformer(MODEL)

    # A small "corpus" to search over
    corpus = [
        "Dealer invoices must be submitted within 30 days of delivery.",
        "The dealer reserve is released when the retail contract performs.",
        "Curtailment requires principal reductions on aged inventory.",
        "The office cafeteria serves lunch until 2pm on weekdays.",
    ]
    corpus_vecs = embedder.encode(corpus)     # (n, 384)

    query = "When do I have to send in my invoice?"
    query_vec = embedder.encode(query)        # (384,)

    # Rank corpus by cosine similarity to the query (semantic, not keyword)
    scored = sorted(
        ((cosine_similarity(query_vec, cv), text) for cv, text in zip(corpus_vecs, corpus)),
        reverse=True,
    )

    print(f"Query: {query}\n")
    print("Ranked results (cosine similarity):")
    for score, text in scored:
        print(f"  {score:.3f}  {text}")

    print("\nNote: the top hit ('invoices within 30 days') has NO keyword overlap with the")
    print("query ('send in my invoice') — semantic search matches meaning, not words.")
    print("This is the retrieval half of RAG. In Azure it's text-embedding-3 + AI Search vectors.")


if __name__ == "__main__":
    main()
