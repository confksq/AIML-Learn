"""
04b_graph_rag.py — GraphRAG: LLM entity extraction -> Neo4j -> graph-enhanced retrieval

Demonstrates the GraphRAG build + query loop:
    1. Use an LLM to extract entities + relationships from text
    2. Write them into Neo4j as nodes + edges
    3. Answer a relationship question by traversing the graph, then have the LLM phrase it

Prereqs:
    pip install -r requirements.txt
    docker compose up -d
    export OPENAI_API_KEY=...        # or point the client at local Ollama (see build_llm)

Run:
    python 04b_graph_rag.py

Note: entity extraction quality depends on the LLM. This is a compact, readable demo of
the pattern (the Microsoft GraphRAG project does this at scale with community detection).
"""

import json
from neo4j import GraphDatabase
from openai import OpenAI

URI = "bolt://localhost:7687"
AUTH = ("neo4j", "testpassword")

# Source text to turn into a graph
SOURCE_TEXT = """
Dealer ATL-001 is managed by Jane. Dealer DAL-002 is also managed by Jane.
Dealer MIA-003 is managed by Bob. Jane manages a late account A1.
Bob manages a current account A2.
"""

EXTRACTION_PROMPT = """Extract entities and relationships from the text as JSON.
Return: {"nodes":[{"id":"...","label":"Dealer|Manager|Account","props":{}}],
         "edges":[{"from":"...","to":"...","type":"MANAGED_BY|MANAGES"}]}
Use the entity name/code as the id. Text:
"""


def build_llm() -> OpenAI:
    # For local/free: return OpenAI(base_url="http://localhost:11434/v1", api_key="ollama")
    return OpenAI()


def extract_graph(client: OpenAI, text: str) -> dict:
    """Use the LLM to turn text into nodes + edges (the GraphRAG construction step)."""
    resp = client.chat.completions.create(
        model="gpt-4o-mini",
        messages=[
            {"role": "system", "content": "You extract knowledge graphs as strict JSON."},
            {"role": "user", "content": EXTRACTION_PROMPT + text},
        ],
        response_format={"type": "json_object"},
        temperature=0,
    )
    return json.loads(resp.choices[0].message.content)


def write_graph(driver, graph: dict):
    """Upsert extracted nodes + edges into Neo4j."""
    with driver.session() as session:
        session.run("MATCH (n) DETACH DELETE n")     # reset for idempotent demo
        for node in graph.get("nodes", []):
            label = node.get("label", "Entity")
            session.run(
                f"MERGE (n:{label} {{id:$id}}) SET n += $props",
                id=node["id"], props=node.get("props", {}),
            )
        for edge in graph.get("edges", []):
            rel = edge.get("type", "RELATED_TO")
            session.run(
                f"MATCH (a {{id:$from}}), (b {{id:$to}}) MERGE (a)-[:{rel}]->(b)",
                {"from": edge["from"], "to": edge["to"]},
            )


def graph_answer(driver, client: OpenAI, question: str) -> str:
    """Traverse the graph for relevant facts, then let the LLM phrase the answer."""
    # A broad traversal that gathers connected facts (in a real system, scope by query entities)
    with driver.session() as session:
        rows = session.run("""
            MATCH (a)-[r]->(b)
            RETURN coalesce(a.id, a.name) AS a, type(r) AS rel,
                   coalesce(b.id, b.name) AS b, b.status AS status
        """).data()

    facts = "\n".join(
        f"- {row['a']} {row['rel']} {row['b']}" + (f" (status={row['status']})" if row.get("status") else "")
        for row in rows
    )
    resp = client.chat.completions.create(
        model="gpt-4o-mini",
        messages=[
            {"role": "system", "content": "Answer the question using ONLY these graph facts. Be precise about relationships."},
            {"role": "user", "content": f"Graph facts:\n{facts}\n\nQuestion: {question}"},
        ],
        temperature=0,
    )
    return resp.choices[0].message.content


def main():
    driver = GraphDatabase.driver(URI, auth=AUTH)
    try:
        driver.verify_connectivity()
    except Exception as e:
        print("Could not connect to Neo4j. Start it with:  docker compose up -d")
        print(f"  {type(e).__name__}: {e}")
        return

    client = build_llm()

    print("1) Extracting entities + relationships from text (LLM)...")
    graph = extract_graph(client, SOURCE_TEXT)
    print(f"   nodes={len(graph.get('nodes', []))} edges={len(graph.get('edges', []))}")

    print("2) Writing graph into Neo4j...")
    write_graph(driver, graph)

    question = "Which dealers share a manager who also handles a late account?"
    print(f"\n3) Graph-RAG answer to: {question}\n")
    print(graph_answer(driver, client, question))

    driver.close()


if __name__ == "__main__":
    main()
