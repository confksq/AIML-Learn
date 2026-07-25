"""
04_hands_on.py — End-to-end LOCAL RAG with Ollama + FAISS (no paid API)

What this demonstrates (the same pipeline you run in Azure, but 100% local):
    Azure OpenAI endpoint   -> Ollama server (localhost:11434)
    text-embedding-3        -> sentence-transformers (all-MiniLM-L6-v2)
    Azure AI Search vector  -> FAISS index (in memory)
    GPT-4o generation       -> LLaMA 3 running locally

Run:
    ollama serve &            # start the local model server
    ollama pull llama3        # one-time model download
    pip install -r requirements.txt
    python 04_hands_on.py

Every section is commented to explain WHAT it does and WHY, mapped to the Azure
equivalent you already know.
"""

import textwrap
import numpy as np
import faiss                                   # local vector store (Azure AI Search equivalent)
import requests                                # to call the Ollama HTTP API
from sentence_transformers import SentenceTransformer  # local embeddings (text-embedding-3 equivalent)

# --------------------------------------------------------------------------------------
# CONFIG
# --------------------------------------------------------------------------------------
OLLAMA_URL = "http://localhost:11434"          # Ollama server. In Azure this is your endpoint URL.
LLM_MODEL = "llama3"                            # the generation model (like a GPT-4o deployment)
EMBED_MODEL_NAME = "all-MiniLM-L6-v2"          # 384-dim embeddings; tiny + fast + fully local
TOP_K = 3                                       # how many chunks to retrieve (your top-K in Azure RAG)

# --------------------------------------------------------------------------------------
# SAMPLE CORPUS
# In a real pipeline these come from PDFs via a loader (Document Intelligence equivalent).
# Here we hard-code a few "documents" so the script is self-contained and runnable.
# --------------------------------------------------------------------------------------
DOCUMENTS = [
    "JM Family dealer invoices must be submitted within 30 days of the vehicle delivery date. "
    "Late submissions incur a 2% penalty per month calculated on the invoice total.",

    "The dealer reserve is a portion of the finance income withheld and paid to the dealer over time. "
    "It is released once the underlying retail contract meets its performance thresholds.",

    "Floorplan financing (curtailment) requires dealers to make principal reductions on aged inventory. "
    "Vehicles unsold after 90 days trigger the first curtailment payment.",

    "Ollama runs open-source LLMs locally and exposes an OpenAI-compatible API on port 11434, "
    "making it suitable for air-gapped and regulated environments where data cannot leave the premises.",

    "FAISS is an in-process vector search library. For small corpora use an exact Flat index; "
    "for large corpora use HNSW, the same approximate-nearest-neighbor algorithm Azure AI Search uses.",
]


# --------------------------------------------------------------------------------------
# STEP 1 — CHUNKING
# Split documents into overlapping chunks. Our sample docs are already chunk-sized,
# so this is a light word-based splitter to show the pattern (recursive/512-token in prod).
# --------------------------------------------------------------------------------------
def chunk_text(text: str, chunk_words: int = 80, overlap: int = 15) -> list[str]:
    words = text.split()
    if len(words) <= chunk_words:
        return [text]
    chunks, start = [], 0
    while start < len(words):
        end = min(start + chunk_words, len(words))
        chunks.append(" ".join(words[start:end]))
        start += chunk_words - overlap          # overlap so answers spanning a boundary aren't lost
    return chunks


def build_chunks(documents: list[str]) -> list[dict]:
    """Return chunks with source metadata (needed for citations, exactly like Azure RAG)."""
    chunks = []
    for doc_id, doc in enumerate(documents):
        for piece in chunk_text(doc):
            chunks.append({"source_id": doc_id, "text": piece})
    return chunks


# --------------------------------------------------------------------------------------
# STEP 2 — EMBED + INDEX (the ingestion half of RAG)
# Embed each chunk and load the vectors into FAISS. Cosine similarity via normalized
# vectors + inner-product index (same reason Azure recommends cosine for text).
# --------------------------------------------------------------------------------------
def build_index(chunks: list[dict], embedder: SentenceTransformer):
    texts = [c["text"] for c in chunks]
    vectors = embedder.encode(texts, normalize_embeddings=True)   # normalize -> cosine via inner product
    vectors = np.asarray(vectors, dtype="float32")
    dim = vectors.shape[1]                                        # 384 for all-MiniLM-L6-v2
    index = faiss.IndexFlatIP(dim)                               # inner product = cosine on unit vectors
    index.add(vectors)                                           # Azure equivalent: push docs to the index
    return index


# --------------------------------------------------------------------------------------
# STEP 3 — RETRIEVE (the query half of RAG)
# Embed the question with the SAME model, search FAISS, return top-K chunks + scores.
# --------------------------------------------------------------------------------------
def retrieve(query: str, embedder, index, chunks, top_k: int = TOP_K):
    q_vec = embedder.encode([query], normalize_embeddings=True).astype("float32")
    scores, idxs = index.search(q_vec, top_k)                    # nearest-neighbor search
    results = []
    for score, idx in zip(scores[0], idxs[0]):
        results.append({**chunks[idx], "score": float(score)})
    return results


# --------------------------------------------------------------------------------------
# STEP 4 — GENERATE with the LOCAL LLM
# Build a grounded prompt (context + question) and call Ollama. The system prompt
# enforces grounding + citations, the same anti-hallucination pattern as Azure RAG.
# --------------------------------------------------------------------------------------
def generate_answer(query: str, retrieved: list[dict]) -> str:
    context_block = "\n\n".join(
        f"[Source {r['source_id']}] {r['text']}" for r in retrieved
    )
    system_prompt = (
        "You are a JM Family assistant. Answer the question using ONLY the sources below. "
        "Cite the source you used as [Source N]. If the answer is not in the sources, "
        "say 'I don't have that information in the provided documents.'"
    )
    user_prompt = f"Sources:\n{context_block}\n\nQuestion: {query}"

    # Call Ollama's OpenAI-compatible chat endpoint. Change only the URL vs Azure OpenAI.
    resp = requests.post(
        f"{OLLAMA_URL}/api/chat",
        json={
            "model": LLM_MODEL,
            "messages": [
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_prompt},
            ],
            "stream": False,
            "options": {"temperature": 0.1},                    # low temp for factual RAG
        },
        timeout=120,
    )
    resp.raise_for_status()
    return resp.json()["message"]["content"]


# --------------------------------------------------------------------------------------
# MAIN — wire it together and print a grounded, cited answer
# --------------------------------------------------------------------------------------
def main():
    print("Loading local embedding model (first run downloads ~90 MB)...")
    embedder = SentenceTransformer(EMBED_MODEL_NAME)

    print("Building chunks + FAISS index...")
    chunks = build_chunks(DOCUMENTS)
    index = build_index(chunks, embedder)
    print(f"  Indexed {len(chunks)} chunks at dim {index.d}.\n")

    question = "What is the penalty for submitting a dealer invoice late?"
    print(f"Question: {question}\n")

    retrieved = retrieve(question, embedder, index, chunks)
    print("Retrieved chunks (source_id, score):")
    for r in retrieved:
        print(f"  [Source {r['source_id']}] score={r['score']:.3f}  {r['text'][:70]}...")
    print()

    print("Generating answer with local LLaMA 3 (via Ollama)...\n")
    try:
        answer = generate_answer(question, retrieved)
    except requests.exceptions.ConnectionError:
        print("ERROR: Could not reach Ollama at", OLLAMA_URL)
        print("Start it with:  ollama serve   and pull the model:  ollama pull llama3")
        return

    print("ANSWER:")
    print(textwrap.fill(answer, width=90))
    print("\nSources used:", sorted({r["source_id"] for r in retrieved}))


if __name__ == "__main__":
    main()
