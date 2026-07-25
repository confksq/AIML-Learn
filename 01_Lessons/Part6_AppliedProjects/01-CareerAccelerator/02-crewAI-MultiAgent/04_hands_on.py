"""
04_hands_on.py — crewAI 3-agent research pipeline (Researcher -> Writer -> Reviewer)

What this demonstrates (the Semantic Kernel multi-agent pattern, in Python):
    SK specialist agent      -> crewAI Agent (role + goal + backstory)
    SK task/goal             -> crewAI Task (description + expected_output + context)
    SK orchestrator          -> crewAI Crew (Process.sequential)
    Azure OpenAI deployment  -> OpenAI or local Ollama backend (toggle below)

Run:
    pip install -r requirements.txt
    # Free/local backend:
    ollama serve & ollama pull llama3
    python 04_hands_on.py --topic "Azure AI Foundry"
    # OR OpenAI backend:  export OPENAI_API_KEY=...  and set BACKEND = "openai"

Every section is commented and mapped to the SK equivalent you already know.
"""

import argparse

from crewai import Agent, Task, Crew, Process
from crewai import LLM  # crewAI's model wrapper; works with OpenAI, Azure, Ollama, etc.

# --------------------------------------------------------------------------------------
# CONFIG — choose your LLM backend. No paid API needed if you use Ollama.
# --------------------------------------------------------------------------------------
BACKEND = "ollama"        # "ollama" (free, local) or "openai"


def build_llm() -> LLM:
    """Return a crewAI LLM. This is where you'd point at Azure OpenAI in production."""
    if BACKEND == "ollama":
        # Local model via Ollama's OpenAI-compatible endpoint. No key, no cost.
        return LLM(model="ollama/llama3", base_url="http://localhost:11434")
    # OpenAI (reads OPENAI_API_KEY from env). For Azure OpenAI, use model="azure/<deployment>"
    # plus AZURE_API_KEY / AZURE_API_BASE / AZURE_API_VERSION env vars.
    return LLM(model="gpt-4o-mini")


# --------------------------------------------------------------------------------------
# AGENTS — each is defined by role + goal + backstory (like an SK specialist agent's
# system prompt). allow_delegation=False keeps this a clean sequential pipeline.
# --------------------------------------------------------------------------------------
def build_agents(llm: LLM):
    researcher = Agent(
        role="Senior Research Analyst",
        goal="Gather accurate, current, well-structured findings on the given topic.",
        backstory=(
            "You are a meticulous analyst who values accuracy over speed. You produce "
            "concise bullet-point findings and never invent facts you are unsure about."
        ),
        llm=llm,
        allow_delegation=False,
        verbose=True,
    )

    writer = Agent(
        role="Technical Writer",
        goal="Turn research findings into a clear, well-structured report for a technical audience.",
        backstory=(
            "You are an experienced technical writer who structures information logically "
            "with headings and bullet points, and writes for busy senior engineers."
        ),
        llm=llm,
        allow_delegation=False,
        verbose=True,
    )

    reviewer = Agent(
        role="Editor / Reviewer",
        goal="Validate accuracy, remove fluff, and finalize the report.",
        backstory=(
            "You are a sharp editor. You check that claims are supported by the research, "
            "cut anything unsupported or verbose, and produce the final polished version."
        ),
        llm=llm,
        allow_delegation=False,
        verbose=True,
    )
    return researcher, writer, reviewer


# --------------------------------------------------------------------------------------
# TASKS — each task has a description + expected_output + agent. The `context` field
# creates the dependency chain (Writer sees Researcher's output; Reviewer sees Writer's).
# This is how crewAI passes state between agents, like threading SK agent outputs.
# --------------------------------------------------------------------------------------
def build_tasks(topic: str, researcher, writer, reviewer):
    research_task = Task(
        description=f"Research the topic: '{topic}'. Produce 5-8 accurate, specific findings as bullet points.",
        expected_output="A bullet-point list of 5-8 concise, factual findings about the topic.",
        agent=researcher,
    )

    write_task = Task(
        description=(
            f"Using the research findings, write a structured report on '{topic}' "
            "with a short intro, 3-4 sections with headings, and a conclusion."
        ),
        expected_output="A well-structured markdown report with headings and bullet points.",
        agent=writer,
        context=[research_task],          # depends on the researcher's output
    )

    review_task = Task(
        description=(
            "Review the draft report for accuracy against the research findings. "
            "Remove any unsupported claims or filler. Produce the final polished report."
        ),
        expected_output="The final, polished, accurate markdown report.",
        agent=reviewer,
        context=[write_task],             # depends on the writer's output
    )
    return [research_task, write_task, review_task]


# --------------------------------------------------------------------------------------
# CREW — the orchestrator. Process.sequential runs the tasks in order, chaining outputs.
# Swap to Process.hierarchical + manager_llm for dynamic delegation (SK supervisor pattern).
# --------------------------------------------------------------------------------------
def run_crew(topic: str) -> str:
    llm = build_llm()
    researcher, writer, reviewer = build_agents(llm)
    tasks = build_tasks(topic, researcher, writer, reviewer)

    crew = Crew(
        agents=[researcher, writer, reviewer],
        tasks=tasks,
        process=Process.sequential,       # fixed order; each task feeds the next
        verbose=True,
    )
    result = crew.kickoff()               # run the whole pipeline
    return str(result)


def main():
    parser = argparse.ArgumentParser(description="crewAI 3-agent research pipeline")
    parser.add_argument("--topic", default="Azure AI Foundry", help="Topic to research and report on")
    args = parser.parse_args()

    print(f"\n=== crewAI pipeline (backend={BACKEND}) — topic: {args.topic} ===\n")
    final_report = run_crew(args.topic)

    print("\n=== FINAL REPORT ===\n")
    print(final_report)

    with open("report.md", "w", encoding="utf-8") as f:
        f.write(final_report)
    print("\n(Saved to report.md)")


if __name__ == "__main__":
    main()
