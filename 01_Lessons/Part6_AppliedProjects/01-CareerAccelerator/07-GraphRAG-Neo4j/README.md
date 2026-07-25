# 07 — GraphRAG + Neo4j

**Part of:** Career Accelerator portfolio · **PRD Feature L7** · **Phase 2 (Week 4)**
**Skill:** Knowledge graphs (Neo4j + Cypher) and GraphRAG — retrieval that reasons over *relationships*, not just vector similarity.

---

## Why this module matters for the job search

~15% of JDs and growing — including a "Lead AI Engineer – Knowledge Graphs & GenAI" role in the inbox. GraphRAG is where RAG is heading for **multi-hop relationship questions** that vector search structurally can't answer ("which dealers share a fleet manager who also handles late-paying accounts?"). You already know vector RAG cold; this module adds the one genuinely new skill in the whole track — **graph thinking + Cypher** — and shows exactly when a graph beats a vector index.

---

## What you'll have after this module
- Neo4j running in **one command** (`docker-compose up`)
- Three runnable scripts: Neo4j/Cypher basics, a graph-RAG pipeline, and a **side-by-side vector-vs-graph comparison** on the same question
- 15 senior-level interview Q&A on vector vs graph vs hybrid retrieval

---

## Prerequisites
```bash
pip install -r requirements.txt
docker compose up -d              # starts Neo4j Community on localhost:7687 (bolt) + :7474 (browser)
```
Neo4j Browser UI: http://localhost:7474 (user `neo4j`, password `testpassword` — set in docker-compose.yml).
For `04b_graph_rag.py`, an LLM is used to extract entities — set `OPENAI_API_KEY` or point it at local Ollama (see the script).

---

## Quick start
```bash
pip install -r requirements.txt
docker compose up -d
python 04a_neo4j_basics.py                 # create nodes/edges, run Cypher queries
python 04b_graph_rag.py                    # LLM entity extraction -> graph -> graph-enhanced retrieval
python 04c_vector_vs_graph_comparison.py   # same question, vector RAG answer vs graph RAG answer
```

---

## Files
| File | What it is |
|---|---|
| `01_concepts.md` | Graphs, Cypher, GraphRAG — bridged from your vector-RAG knowledge |
| `02_architecture.md` | Graph construction + graph-RAG flow |
| `03_interview_qa.md` | 15 senior-level interview Q&A |
| `04a_neo4j_basics.py` | Connect to Neo4j, create nodes/edges, run Cypher |
| `04b_graph_rag.py` | LLM entity extraction → Neo4j → graph-enhanced retrieval |
| `04c_vector_vs_graph_comparison.py` | Same question: vector RAG vs graph RAG, side by side |
| `docker-compose.yml` | One-command Neo4j Community setup |
| `05_resume_bullet.md` | Ready-to-paste resume bullet |
| `requirements.txt` | Python dependencies |

---
*Bridge: Azure AI Search (vector) → Neo4j (graph) · cosine similarity retrieval → Cypher graph traversal · single-hop RAG → multi-hop relationship reasoning · Microsoft GraphRAG (entity extraction → communities)*
