"""
04c_vector_vs_graph_comparison.py — Same question, vector RAG vs graph RAG, side by side

The whole point of the module: show WHERE a graph beats vector similarity.

Question: "Which dealers share a manager who also handles a late account?"
  - VECTOR RAG: embeds the question, retrieves the most similar text chunks. It returns
    passages ABOUT dealers/managers but cannot COMPUTE the shared-manager relationship.
  - GRAPH RAG:  runs a Cypher multi-hop traversal that returns the exact connected dealers.

Prereqs:
    pip install -r requirements.txt
    docker compose up -d
Run:
    python 04c_vector_vs_graph_comparison.py

This demo is self-contained (no LLM/API needed): it uses FAISS for the vector side and
a pre-built Neo4j graph for the graph side, so the contrast is visible without keys.
"""

import numpy as np
import faiss
from sentence_transformers import SentenceTransformer
from neo4j import GraphDatabase

URI = "bolt://localhost:7687"
AUTH = ("neo4j", "testpassword")

QUESTION = "Which dealers share a manager who also handles a late account?"

# The corpus a vector index would hold — factual sentences, but the ANSWER requires
# combining relationships ACROSS sentences, which similarity search can't do.
CHUNKS = [
    "Dealer ATL-001 is managed by Jane.",
    "Dealer DAL-002 is managed by Jane.",
    "Dealer MIA-003 is managed by Bob.",
    "Jane manages a late account A1.",
    "Bob manages a current account A2.",
]


# ---------------------------- VECTOR RAG side ----------------------------
def vector_retrieve(question, k=3):
    embedder = SentenceTransformer("all-MiniLM-L6-v2")
    vecs = embedder.encode(CHUNKS, normalize_embeddings=True).astype("float32")
    index = faiss.IndexFlatIP(vecs.shape[1])
    index.add(vecs)
    qv = embedder.encode([question], normalize_embeddings=True).astype("float32")
    _, idxs = index.search(qv, k)
    return [CHUNKS[i] for i in idxs[0]]


# ---------------------------- GRAPH RAG side ----------------------------
def build_graph(driver):
    with driver.session() as s:
        s.run("MATCH (n) DETACH DELETE n")
        s.run("""
            CREATE (jane:Manager {name:'Jane'})
            CREATE (bob:Manager  {name:'Bob'})
            CREATE (:Dealer {code:'ATL-001'})-[:MANAGED_BY]->(jane)
            CREATE (:Dealer {code:'DAL-002'})-[:MANAGED_BY]->(jane)
            CREATE (:Dealer {code:'MIA-003'})-[:MANAGED_BY]->(bob)
            CREATE (jane)-[:MANAGES]->(:Account {status:'late'})
            CREATE (bob)-[:MANAGES]->(:Account {status:'current'})
        """)


def graph_answer(driver):
    with driver.session() as s:
        rows = s.run("""
            MATCH (d1:Dealer)-[:MANAGED_BY]->(m:Manager)<-[:MANAGED_BY]-(d2:Dealer),
                  (m)-[:MANAGES]->(:Account {status:'late'})
            WHERE d1.code < d2.code
            RETURN d1.code AS d1, d2.code AS d2, m.name AS mgr
        """).data()
    return rows


def main():
    print(f"QUESTION: {QUESTION}\n")

    # ---- Vector RAG ----
    print("=" * 64)
    print("VECTOR RAG (cosine similarity retrieval)")
    print("=" * 64)
    hits = vector_retrieve(QUESTION)
    print("Retrieved the most similar chunks:")
    for h in hits:
        print(f"  - {h}")
    print("=> Returns relevant sentences, but CANNOT compute the shared-manager")
    print("   relationship across them. The answer is left to the LLM to infer, unreliably.\n")

    # ---- Graph RAG ----
    print("=" * 64)
    print("GRAPH RAG (Cypher multi-hop traversal)")
    print("=" * 64)
    driver = GraphDatabase.driver(URI, auth=AUTH)
    try:
        driver.verify_connectivity()
    except Exception as e:
        print("Neo4j not reachable — start it with: docker compose up -d")
        print(f"  {type(e).__name__}: {e}")
        return
    build_graph(driver)
    rows = graph_answer(driver)
    print("Cypher directly computes the relationship:")
    for r in rows:
        print(f"  {r['d1']} and {r['d2']} share manager {r['mgr']} (who handles a late account)")
    print("=> Precise, relationship-correct answer — the graph's structural advantage.\n")
    driver.close()

    print("TAKEAWAY: vector RAG for semantic passage lookup; graph RAG for multi-hop")
    print("relationship questions. In production, run hybrid and route per query type.")


if __name__ == "__main__":
    main()
