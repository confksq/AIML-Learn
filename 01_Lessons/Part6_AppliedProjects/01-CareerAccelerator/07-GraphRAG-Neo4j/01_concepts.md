# 01 — Concepts: GraphRAG + Neo4j

> **Bridge from what you already know:** you know vector RAG (Azure AI Search) — retrieve chunks by cosine similarity. GraphRAG adds a **different retrieval paradigm**: traverse explicit *relationships* between entities. This is the one genuinely new skill in the track — graph thinking + Cypher — so it gets the most attention.

---

## 1. The one-sentence mental model

**Vector RAG finds chunks that are *similar* to your query; GraphRAG finds entities that are *connected* to each other.** When the answer depends on relationships and multi-hop reasoning ("A manages B who owns C"), a graph does what vector similarity structurally can't.

| Vector RAG (you know) | GraphRAG (new) |
|---|---|
| Store: embeddings in Azure AI Search / FAISS | Store: nodes + edges in Neo4j |
| Retrieve: nearest vectors (cosine) | Retrieve: traverse relationships (Cypher) |
| Good at: "find text about X" | Good at: "how is X connected to Y across N hops" |
| Unit: a chunk | Unit: an entity (node) + its relationships (edges) |
| Fails at: multi-hop relationship questions | Fails at: fuzzy semantic "find similar text" |

---

## 2. Knowledge graph basics

A graph is **nodes** (entities) connected by **edges** (relationships), each with **properties**:

```
(Dealer {code:"ATL-001"}) -[:MANAGED_BY]-> (Manager {name:"Jane"})
(Dealer {code:"ATL-001"}) -[:HAS_ACCOUNT]-> (Account {status:"late"})
(Manager {name:"Jane"})   -[:MANAGES]->    (Dealer {code:"DAL-002"})
```

- **Node** = a thing (Dealer, Manager, Account). Has a *label* (its type) and properties.
- **Edge/relationship** = a typed, directed connection (`MANAGED_BY`, `HAS_ACCOUNT`). Can also have properties.
- **Property** = a key/value on a node or edge (`code:"ATL-001"`, `status:"late"`).

---

## 3. Cypher — the query language (the new muscle)

Cypher is to Neo4j what SQL is to a relational DB — but it's built around **pattern matching on relationships**. The `()` are nodes, the `-[]->` are edges. Read it like ASCII art of the graph:

```cypher
// Create nodes and a relationship
CREATE (d:Dealer {code:"ATL-001"})-[:MANAGED_BY]->(m:Manager {name:"Jane"})

// Find all dealers managed by Jane
MATCH (d:Dealer)-[:MANAGED_BY]->(m:Manager {name:"Jane"})
RETURN d.code

// MULTI-HOP: dealers who share a manager who also manages a late account
MATCH (d1:Dealer)-[:MANAGED_BY]->(m:Manager)<-[:MANAGED_BY]-(d2:Dealer),
      (m)-[:MANAGES]->(:Account {status:"late"})
RETURN d1.code, d2.code, m.name
```

That last query is the point: **a single Cypher pattern expresses a multi-hop relationship question** that would be awkward-to-impossible with vector similarity. This is the skill worth practicing — writing `MATCH` patterns.

---

## 4. What GraphRAG actually is

GraphRAG = RAG where retrieval traverses a knowledge graph instead of (or alongside) a vector index. The pipeline:

```
BUILD (offline):
  documents ─▶ LLM entity + relationship extraction ─▶ nodes + edges ─▶ Neo4j
              (optionally: community detection + community summaries)

QUERY:
  question ─▶ identify entities ─▶ Cypher traversal from those entities
           ─▶ gather connected subgraph ─▶ feed to LLM as context ─▶ answer
```

**Microsoft GraphRAG** (the open-source project) formalizes this: it uses an LLM to extract entities/relationships from a corpus, builds a graph, detects **communities** (clusters of related entities), and generates **community summaries** — enabling "global" questions over a whole corpus ("what are the main themes across all these documents?") that chunk-retrieval can't answer.

---

## 5. When GraphRAG beats vector RAG (and when it doesn't)

| Question type | Best retrieval |
|---|---|
| "What does the policy say about late invoices?" | **Vector RAG** — semantic lookup of a passage |
| "Find documents similar to this one" | **Vector RAG** — that's literally cosine similarity |
| "Which dealers share a manager who handles late accounts?" | **GraphRAG** — multi-hop relationship traversal |
| "What are the main themes across all 500 contracts?" | **GraphRAG** (community summaries) — global synthesis |
| "How is entity A connected to entity D?" | **GraphRAG** — path finding |
| General enterprise Q&A | **Hybrid** — vector for passages + graph for relationships |

**The honest trade-off:** GraphRAG's graph-construction step (LLM entity/relationship extraction over the whole corpus) is expensive and must be re-run as documents change. It's justified only when the questions genuinely require relationship/multi-hop reasoning — don't build a graph for questions vector RAG already answers.

---

## 6. Vector vs Graph vs Hybrid — decision table

| Factor | Vector RAG | GraphRAG | Hybrid |
|---|---|---|---|
| Retrieval by | Semantic similarity | Relationships / traversal | Both |
| Best for | Passage lookup, fuzzy match | Multi-hop, connections, global themes | Real enterprise Q&A |
| Build cost | Low (chunk + embed) | High (LLM extraction + graph) | Highest |
| Freshness | Re-embed changed docs | Re-extract/re-build graph | Both |
| Azure fit | Azure AI Search | Neo4j / Cosmos DB Gremlin | AI Search + graph |

**The senior answer:** "Vector RAG is my default — it answers most enterprise Q&A cheaply. I add a knowledge graph when the questions are genuinely relational or multi-hop (shared entities, paths, global themes) that similarity search can't reach, and I often run hybrid — vector for the passage, graph for the connections. GraphRAG's construction cost means I only build the graph when the query patterns justify it."

---

## 7. Neo4j in the Azure world

- Neo4j is the leading graph DB; it runs anywhere (this module uses Docker locally). On Azure you'd run Neo4j (marketplace/AKS) or use **Azure Cosmos DB's Gremlin API** as a managed graph store.
- The vector-store analog you know is Azure AI Search; the graph-store analog is Neo4j/Cosmos Gremlin. GraphRAG frequently pairs *both* — vector index for passages, graph for relationships.

---
*Next: `02_architecture.md`*
