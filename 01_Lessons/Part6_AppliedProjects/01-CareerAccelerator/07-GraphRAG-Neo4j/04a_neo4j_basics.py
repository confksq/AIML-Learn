"""
04a_neo4j_basics.py — Neo4j + Cypher basics

Demonstrates the graph fundamentals: connect, create nodes/edges, run Cypher queries
(including a multi-hop relationship query that vector search can't express).

Prereqs:
    pip install -r requirements.txt
    docker compose up -d          # Neo4j on bolt://localhost:7687

Run:
    python 04a_neo4j_basics.py
"""

from neo4j import GraphDatabase

URI = "bolt://localhost:7687"
AUTH = ("neo4j", "testpassword")     # matches docker-compose.yml


def reset(tx):
    """Clear the graph so the demo is idempotent."""
    tx.run("MATCH (n) DETACH DELETE n")


def build_sample_graph(tx):
    """Create a small dealer/manager/account graph.
    Nodes = entities (Dealer, Manager, Account); edges = typed relationships."""
    tx.run("""
        // Managers
        CREATE (jane:Manager {name:'Jane'})
        CREATE (bob:Manager  {name:'Bob'})

        // Dealers, each managed by a manager
        CREATE (atl:Dealer {code:'ATL-001', region:'Southeast'})-[:MANAGED_BY]->(jane)
        CREATE (dal:Dealer {code:'DAL-002', region:'Southwest'})-[:MANAGED_BY]->(jane)
        CREATE (mia:Dealer {code:'MIA-003', region:'Southeast'})-[:MANAGED_BY]->(bob)

        // Accounts, some late, linked to the managing manager
        CREATE (jane)-[:MANAGES]->(:Account {id:'A1', status:'late'})
        CREATE (bob)-[:MANAGES]->(:Account {id:'A2', status:'current'})
    """)


def query_managed_by(tx, manager_name):
    result = tx.run(
        "MATCH (d:Dealer)-[:MANAGED_BY]->(m:Manager {name:$name}) RETURN d.code AS code",
        name=manager_name,
    )
    return [r["code"] for r in result]


def query_multihop(tx):
    """MULTI-HOP: dealers who share a manager who also manages a late account.
    A single Cypher pattern expresses what vector similarity cannot."""
    result = tx.run("""
        MATCH (d1:Dealer)-[:MANAGED_BY]->(m:Manager)<-[:MANAGED_BY]-(d2:Dealer),
              (m)-[:MANAGES]->(:Account {status:'late'})
        WHERE d1.code < d2.code           // avoid duplicate pairs
        RETURN d1.code AS dealer1, d2.code AS dealer2, m.name AS manager
    """)
    return [(r["dealer1"], r["dealer2"], r["manager"]) for r in result]


def main():
    driver = GraphDatabase.driver(URI, auth=AUTH)
    try:
        driver.verify_connectivity()
    except Exception as e:
        print("Could not connect to Neo4j. Start it with:  docker compose up -d")
        print(f"  {type(e).__name__}: {e}")
        return

    with driver.session() as session:
        session.execute_write(reset)
        session.execute_write(build_sample_graph)
        print("Sample graph created.\n")

        managed = session.execute_read(query_managed_by, "Jane")
        print(f"Dealers managed by Jane: {managed}\n")

        pairs = session.execute_read(query_multihop)
        print("MULTI-HOP — dealers sharing a manager who also handles a LATE account:")
        for d1, d2, mgr in pairs:
            print(f"  {d1} and {d2}  (shared manager: {mgr})")
        print("\nThis relationship query is trivial in Cypher and near-impossible with vector search.")

    driver.close()


if __name__ == "__main__":
    main()
