"""
04_hands_on.py — GCP Vertex AI: Gemini generation + embeddings + a minimal ADK agent

What this demonstrates (the GCP mirror of your Azure AI Foundry work):
    Azure OpenAI chat call    -> Vertex Gemini generate_content(...)
    text-embedding-3          -> Vertex text-embedding-004
    Semantic Kernel agent     -> Agent Development Kit (ADK) Agent

Prereqs:
    pip install -r requirements.txt
    gcloud auth application-default login          # Application Default Credentials (ADC)
    export GOOGLE_CLOUD_PROJECT=your-project
    export GOOGLE_CLOUD_LOCATION=us-central1
    export GOOGLE_GENAI_USE_VERTEXAI=True           # route the google-genai SDK through Vertex

Run:
    python 04_hands_on.py

No GCP account? The code documents the exact API shape; concepts + comparison stand alone.
"""

import os

PROJECT = os.getenv("GOOGLE_CLOUD_PROJECT", "your-project")
LOCATION = os.getenv("GOOGLE_CLOUD_LOCATION", "us-central1")


# --------------------------------------------------------------------------------------
# PART 1 — Call Gemini on Vertex AI (the Azure OpenAI chat-completion equivalent)
# --------------------------------------------------------------------------------------
def gemini_generate(prompt: str) -> str:
    from google import genai

    # vertexai=True routes through your GCP project (enterprise: IAM, data governance)
    client = genai.Client(vertexai=True, project=PROJECT, location=LOCATION)

    resp = client.models.generate_content(
        model="gemini-2.0-flash",            # Flash = fast/cheap (GPT-4o-mini analog)
        contents=prompt,
        # config maps to Azure OpenAI params (temperature, max_output_tokens, etc.)
        config={"temperature": 0.2, "max_output_tokens": 300},
    )
    return resp.text


# --------------------------------------------------------------------------------------
# PART 2 — Embeddings with text-embedding-004 (the text-embedding-3 equivalent)
# --------------------------------------------------------------------------------------
def gemini_embed(texts: list[str]) -> list[list[float]]:
    from google import genai

    client = genai.Client(vertexai=True, project=PROJECT, location=LOCATION)
    resp = client.models.embed_content(
        model="text-embedding-004",
        contents=texts,
    )
    return [e.values for e in resp.embeddings]


# --------------------------------------------------------------------------------------
# PART 3 — A minimal Agent Development Kit (ADK) agent (the Semantic Kernel equivalent)
# The agent plans (ReAct) and calls the Python tool, just like SK AutoInvokeKernelFunctions.
# --------------------------------------------------------------------------------------
def build_adk_agent():
    from google.adk.agents import Agent

    # A tool is just a Python function with a docstring (like an SK [KernelFunction]).
    def get_inventory(model: str) -> str:
        """Look up current vehicle inventory for a given model.

        Args:
            model: the vehicle model name, e.g. 'RAV4 Hybrid'.
        Returns:
            A short availability summary.
        """
        # In production this calls your real inventory API.
        return f"3 {model} vehicles currently in stock at the Southeast region."

    agent = Agent(
        name="jma_dealer_agent",
        model="gemini-2.0-flash",
        instruction=(
            "You are a JMA dealer support agent. Use the available tools to answer "
            "questions about inventory. Be concise and cite the tool result."
        ),
        tools=[get_inventory],
    )
    return agent


def main():
    print("=== PART 1: Gemini generation on Vertex AI ===")
    try:
        print(gemini_generate("In two sentences, what is retrieval-augmented generation (RAG)?"))
    except Exception as e:
        print("Could not call Vertex live (expected without GCP auth/project).")
        print(f"  {type(e).__name__}: {e}")
        print("  Set up: gcloud auth application-default login + GOOGLE_CLOUD_PROJECT")

    print("\n=== PART 2: Embeddings (text-embedding-004) ===")
    try:
        vecs = gemini_embed(["RAV4 Hybrid fuel economy", "Camry sedan pricing"])
        print(f"  Got {len(vecs)} embeddings, dim={len(vecs[0])}")
    except Exception as e:
        print(f"  (skipped live call) {type(e).__name__}: {e}")

    print("\n=== PART 3: ADK agent (Semantic Kernel equivalent) ===")
    print("Agent definition (tools + instruction) — run via ADK Runner or `adk web`:")
    print("  agent = build_adk_agent()  # gemini-2.0-flash + get_inventory tool")
    print("  Ask: 'How many RAV4 Hybrids are in stock?' -> agent calls get_inventory()")
    print("\nSame pattern as Azure Semantic Kernel: define tools, give instructions,")
    print("the agent plans (ReAct) and invokes them. Deploy to Vertex Agent Engine or Cloud Run.")


if __name__ == "__main__":
    main()
