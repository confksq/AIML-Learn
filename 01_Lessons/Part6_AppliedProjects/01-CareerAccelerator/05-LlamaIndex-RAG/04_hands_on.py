"""
04_hands_on.py — RAG with LlamaIndex (local Ollama backend, source citations)

Demonstrates the RAG-specialized framework: Documents -> Nodes -> VectorStoreIndex
-> QueryEngine -> cited answer, in a handful of lines.

Azure bridge: this is the same retrieve->augment->generate pipeline you run with
Azure OpenAI + AI Search, expressed in LlamaIndex's data-centric abstraction, running
fully local via Ollama + a HuggingFace embedding model (no paid API).

Run:
    pip install -r requirements.txt
    ollama serve && ollama pull llama3
    python 04_hands_on.py
"""

import os

from llama_index.core import (
    VectorStoreIndex,
    SimpleDirectoryReader,
    Settings,
    Document,
)
from llama_index.llms.ollama import Ollama
from llama_index.embeddings.huggingface import HuggingFaceEmbedding

DATA_DIR = "data"


# --------------------------------------------------------------------------------------
# Create a sample document on first run so the demo is self-contained.
# In production, SimpleDirectoryReader loads real PDFs/txt/md from this folder.
# --------------------------------------------------------------------------------------
def ensure_sample_data():
    os.makedirs(DATA_DIR, exist_ok=True)
    sample_path = os.path.join(DATA_DIR, "dealer_policy.txt")
    if not os.path.exists(sample_path):
        with open(sample_path, "w", encoding="utf-8") as f:
            f.write(
                "Dealer invoices must be submitted within 30 days of the vehicle delivery date. "
                "Late submissions incur a 2 percent penalty per month calculated on the invoice total.\n\n"
                "The dealer reserve is a portion of finance income withheld and paid to the dealer over time. "
                "It is released once the underlying retail contract meets its performance thresholds.\n\n"
                "Curtailment, also called floorplan financing, requires principal reductions on aged inventory. "
                "Vehicles unsold after 90 days trigger the first curtailment payment.\n\n"
                "Warranty claims must be submitted through the dealer portal within 60 days of the repair.\n"
            )


def main():
    ensure_sample_data()

    # --- Configure LlamaIndex to run fully local (no paid API) ---
    # Settings is the global config: set the LLM + embedding model once, everything uses them.
    Settings.llm = Ollama(model="llama3", request_timeout=120.0)          # local generation
    Settings.embed_model = HuggingFaceEmbedding(model_name="all-MiniLM-L6-v2")  # local embeddings

    # --- INGEST: load documents from the folder into Documents ---
    print(f"Loading documents from ./{DATA_DIR} ...")
    documents = SimpleDirectoryReader(DATA_DIR).load_data()
    print(f"  Loaded {len(documents)} document(s).")

    # --- INDEX: chunk -> Nodes -> embed -> VectorStoreIndex (in-memory here) ---
    print("Building VectorStoreIndex (chunk -> embed -> index)...")
    index = VectorStoreIndex.from_documents(documents)

    # --- QUERY: QueryEngine retrieves, prompts the LLM, synthesizes, and cites sources ---
    query_engine = index.as_query_engine(similarity_top_k=2)

    question = "What is the penalty for submitting a dealer invoice late?"
    print(f"\nQuestion: {question}\n")

    response = query_engine.query(question)

    print("ANSWER:")
    print(f"  {response}\n")

    # Citations for free: the source nodes the answer was built from
    print("SOURCE NODES (citations):")
    for i, node in enumerate(response.source_nodes, 1):
        score = getattr(node, "score", None)
        score_str = f"{score:.3f}" if score is not None else "n/a"
        print(f"  [{i}] score={score_str}  {node.node.get_content()[:80]}...")

    print("\nSame RAG architecture as Azure OpenAI + AI Search — expressed in LlamaIndex,")
    print("running fully local via Ollama + HuggingFace embeddings, with citations built in.")


if __name__ == "__main__":
    main()
