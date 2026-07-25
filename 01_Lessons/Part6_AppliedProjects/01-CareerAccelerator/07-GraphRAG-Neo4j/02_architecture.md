# 02 — Architecture: GraphRAG + Neo4j

## Graph construction + graph-RAG flow

```
                    BUILD THE GRAPH (offline)
  ┌────────────┐   ┌─────────────────────────┐   ┌──────────────────────────┐
  │ Documents  │──▶│ LLM entity + relationship│──▶│ Neo4j                    │
  │            │   │ extraction               │   │ (:Dealer)-[:MANAGED_BY]->│
  │            │   │ "who/what + how related" │   │ (:Manager)               │
  └────────────┘   └─────────────────────────┘   │ (:Dealer)-[:HAS_ACCOUNT]>│
                          (optional)             │ (:Account {status})      │
                    community detection + summaries└──────────────────────────┘

                    QUERY (online)
  ┌────────────┐   ┌──────────────────────┐   ┌───────────────────────────┐
  │ user query │──▶│ identify entities    │──▶│ Cypher traversal from those│
  │            │   │ in the query         │   │ entities (multi-hop MATCH) │
  └────────────┘   └──────────────────────┘   └─────────────┬─────────────┘
                                                            │ connected subgraph
                                                            ▼
                                          ┌──────────────────────────────────┐
                                          │ feed subgraph as context to LLM  │
                                          │ ─▶ relationship-grounded answer  │
                                          └──────────────────────────────────┘

  Neo4j runs locally via docker-compose (bolt :7687, browser :7474).
```

## Vector RAG vs GraphRAG — same question, different retrieval (demo 04c)

```
  Question: "Which dealers share a manager who also handles a late account?"

  VECTOR RAG:                              GRAPHRAG:
    embed question                           identify entities: Dealer, Manager, Account
    -> cosine search over chunks             -> Cypher multi-hop MATCH:
    -> returns text passages ABOUT dealers      (d1)-[:MANAGED_BY]->(m)<-[:MANAGED_BY]-(d2),
    -> cannot compute the shared-manager        (m)-[:MANAGES]->(:Account {status:'late'})
       relationship across records           -> returns the exact connected dealers
    RESULT: vague / incomplete               RESULT: precise, relationship-correct
```
This side-by-side is the whole point of the module — it shows *where* a graph wins.

## Component breakdown

| Component | Role | Your Azure / vector equivalent |
|---|---|---|
| **Neo4j** | Graph database (nodes + edges) | Azure AI Search (but for relationships) / Cosmos Gremlin |
| **Cypher** | Query language — pattern-match relationships | Vector search query / OData filter |
| **Entity extraction (LLM)** | Turns text → nodes + edges | Chunking + embedding (but for structure) |
| **Node** | An entity (Dealer, Manager, Account) | A chunk (but a *thing*, not text) |
| **Edge/relationship** | A typed connection (MANAGED_BY) | (no vector-RAG analog — this is the new capability) |
| **Community summaries** | Cluster-level summaries for global questions | (no chunk-RAG analog) |
| **Graph-RAG query** | Identify entities → traverse → answer | Embed → retrieve → answer |

## Data flow notes

- **Graph construction is the expensive part.** The LLM extraction step runs over the whole corpus and must re-run as documents change — the reason you only build a graph when the questions justify it.
- **Cypher is the skill to practice.** The `MATCH (a)-[:REL]->(b)` pattern-matching is what expresses multi-hop questions in one query. Read `-[]->` as an edge, `()` as a node.
- **Hybrid is common in production.** Vector index for "find the passage," graph for "find the connections" — many real systems run both and merge results.

## Neo4j deployment (this module)

`docker-compose.yml` runs Neo4j Community locally: bolt protocol on `:7687` (what the Python driver connects to) and the Browser UI on `:7474` (visualize your graph). In Azure production you'd run Neo4j on AKS/marketplace or use Cosmos DB's Gremlin API as a managed graph store.

---
*Next: `03_interview_qa.md`*
