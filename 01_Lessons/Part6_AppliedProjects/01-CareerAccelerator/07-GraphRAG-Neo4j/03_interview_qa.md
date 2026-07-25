# 03 — Interview Q&A: GraphRAG + Neo4j (15 questions, senior level)

---

**Q1. What is GraphRAG and how does it differ from vector RAG?**
GraphRAG is RAG where retrieval traverses a knowledge graph (entities + relationships) instead of, or alongside, a vector index. Vector RAG finds chunks *similar* to the query by cosine similarity; GraphRAG finds entities *connected* to each other via relationships. GraphRAG answers multi-hop relationship questions that vector similarity structurally can't.

**Q2. When does GraphRAG beat vector RAG, concretely?**
When the answer depends on relationships or multiple hops — "which dealers share a manager who also handles late accounts," "how is entity A connected to entity D," or global synthesis "what are the main themes across all 500 contracts." Vector search returns passages *about* dealers but can't compute the shared-manager relationship across records; a graph traversal returns the exact connected entities.

**Q3. When is vector RAG still the right choice?**
For semantic passage lookup ("what does the policy say about late invoices"), fuzzy "find similar documents," and general enterprise Q&A. Most questions are answered well and cheaply by vector RAG — it's my default. I add a graph only when query patterns are genuinely relational/multi-hop.

**Q4. What is a knowledge graph — nodes, edges, properties?**
Nodes are entities (Dealer, Manager, Account) with a label (type) and properties (key/values). Edges are typed, directed relationships between nodes (MANAGED_BY, HAS_ACCOUNT), which can also carry properties. Together they model *things and how they connect*, which is exactly what relationship questions need.

**Q5. What is Cypher and how do you read it?**
Neo4j's query language, built around pattern-matching on relationships. `()` is a node, `-[:REL]->` is a directed typed edge — you read a query like ASCII art of the graph. `MATCH (d:Dealer)-[:MANAGED_BY]->(m:Manager) RETURN d` finds dealers and their managers. It's to Neo4j what SQL is to a relational DB, but pattern-first.

**Q6. Write a Cypher query for a multi-hop relationship question.**
"Dealers who share a manager who also manages a late account":
`MATCH (d1:Dealer)-[:MANAGED_BY]->(m:Manager)<-[:MANAGED_BY]-(d2:Dealer), (m)-[:MANAGES]->(:Account {status:'late'}) RETURN d1.code, d2.code, m.name`. A single pattern expresses the multi-hop question — that conciseness is why graphs win on relational queries.

**Q7. Walk through building a GraphRAG pipeline.**
Offline: run an LLM over the corpus to extract entities and relationships, write them as nodes/edges into Neo4j, optionally detect communities and generate community summaries. Online: identify entities in the query, run a Cypher traversal from those entities to gather the connected subgraph, then feed that subgraph to the LLM as context to answer. The construction step is the expensive part.

**Q8. What is Microsoft GraphRAG?**
The open-source project that formalizes graph construction: it uses an LLM to extract entities/relationships from a corpus, builds a graph, detects communities (clusters of related entities), and generates community summaries — enabling "global" questions over a whole corpus ("main themes across all documents") that chunk-retrieval can't answer. It's the reference implementation of the pattern.

**Q9. What's the honest cost trade-off of GraphRAG?**
Graph construction — LLM entity/relationship extraction over the entire corpus — is expensive and must be re-run as documents change (freshness cost). It's justified only when the questions genuinely require relationship or multi-hop reasoning. Building a graph for questions vector RAG already answers is wasted cost.

**Q10. What is hybrid retrieval and why is it common in production?**
Running both: a vector index to "find the relevant passage" and a graph to "find the relevant connections," then merging. Real enterprise Q&A mixes passage-lookup and relationship questions, so many systems use vector RAG as the default and consult the graph for the relational subset — best coverage at controlled cost.

**Q11. How would you run Neo4j in an Azure environment?**
Neo4j on AKS or from the Azure Marketplace as a managed VM/container, or use Azure Cosmos DB's Gremlin API as a managed graph store. The vector-store analog is Azure AI Search; the graph-store analog is Neo4j/Cosmos Gremlin — and GraphRAG often pairs both (AI Search for passages, graph for relationships).

**Q12. A client asks for GraphRAG because it's the latest thing. How do you push back?**
I ask what questions they actually need answered. If they're mostly single-fact lookup or semantic Q&A, vector RAG is cheaper and sufficient — GraphRAG's construction cost isn't justified. I'd only recommend a graph if they have genuinely multi-hop/relationship or global-synthesis queries, demonstrated with real example questions, not assumed from hype.

**Q13. How do you keep a knowledge graph fresh?**
Tie graph updates to source-document changes: on change, re-extract entities/relationships for the affected documents and upsert nodes/edges (not a full rebuild if you can scope it). A new document can also invalidate existing relationship inferences, so the update logic is more involved than re-embedding a chunk — a real operational cost of GraphRAG.

**Q14. How does graph retrieval get grounded/cited answers?**
The traversal returns specific nodes and relationships (with their source document properties), which you feed to the LLM as structured context and cite by entity/source. Because the retrieval is explicit relationships rather than fuzzy similarity, the provenance is often clearer than a vector chunk — you can point to the exact path that produced the answer.

**Q15. Design a system that answers both 'what does the policy say' and 'which dealers are connected through X'.**
Hybrid: a vector index (Azure AI Search / FAISS) over document chunks for semantic passage questions, and a Neo4j knowledge graph (built via LLM entity extraction) for relationship/multi-hop questions. A router (or the agent) classifies the query and picks vector, graph, or both, then merges results before generation. Vector is the default; the graph is consulted when the question is relational — best coverage without paying graph-construction cost for questions that don't need it.

---
*This is the module with the most genuinely new material — practice writing Cypher MATCH patterns, and always frame GraphRAG as "the complement to vector RAG for relationship questions," not a replacement.*
