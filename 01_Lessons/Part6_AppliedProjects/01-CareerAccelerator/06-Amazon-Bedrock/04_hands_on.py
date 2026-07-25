"""
04_hands_on.py — Amazon Bedrock via boto3: invoke a model + show the RAG (Knowledge Bases) shape

What this demonstrates (the AWS mirror of your Azure AI Foundry work):
    Azure OpenAI chat call     -> bedrock-runtime.converse(...)
    Azure OpenAI On Your Data  -> bedrock-agent-runtime.retrieve_and_generate(...)

Prereqs:
    pip install -r requirements.txt
    aws configure                      # set AWS_ACCESS_KEY_ID / SECRET / region (e.g. us-east-1)
    # Enable model access for Claude in the Bedrock console (one-time).

Run:
    python 04_hands_on.py

No AWS account? Read the comments — they document the exact API shape. The concepts
and the azure_vs_bedrock_comparison.md stand alone.
"""

import boto3
from botocore.exceptions import ClientError, NoCredentialsError

REGION = "us-east-1"
# Claude 3 Sonnet on Bedrock. Model IDs are provider-qualified (vs Azure deployment names).
MODEL_ID = "anthropic.claude-3-sonnet-20240229-v1:0"


# --------------------------------------------------------------------------------------
# PART 1 — Invoke a foundation model with the modern, provider-agnostic `converse` API.
# This is the closest analog to an Azure OpenAI chat completion.
# --------------------------------------------------------------------------------------
def invoke_model(prompt: str) -> str:
    client = boto3.client("bedrock-runtime", region_name=REGION)

    response = client.converse(
        modelId=MODEL_ID,
        messages=[
            {"role": "user", "content": [{"text": prompt}]},
        ],
        # inferenceConfig maps to Azure OpenAI parameters (max_tokens, temperature, top_p)
        inferenceConfig={"maxTokens": 300, "temperature": 0.2, "topP": 0.9},
    )
    # converse returns a uniform shape regardless of provider
    return response["output"]["message"]["content"][0]["text"]


# --------------------------------------------------------------------------------------
# PART 2 — RAG via Knowledge Bases (the Azure "On Your Data" equivalent).
# Requires a Knowledge Base created in the Bedrock console (S3 source + vector store).
# Shown here fully commented; supply your knowledgeBaseId to run it live.
# --------------------------------------------------------------------------------------
def rag_with_knowledge_base(question: str, knowledge_base_id: str) -> str:
    client = boto3.client("bedrock-agent-runtime", region_name=REGION)

    response = client.retrieve_and_generate(
        input={"text": question},
        retrieveAndGenerateConfiguration={
            "type": "KNOWLEDGE_BASE",
            "knowledgeBaseConfiguration": {
                "knowledgeBaseId": knowledge_base_id,      # created in the Bedrock console
                "modelArn": f"arn:aws:bedrock:{REGION}::foundation-model/{MODEL_ID}",
            },
        },
    )
    # Bedrock retrieves chunks + calls the model + returns a grounded answer with citations
    answer = response["output"]["text"]
    citations = response.get("citations", [])
    return answer, citations


def main():
    print("=== Bedrock: invoke a foundation model (converse API) ===\n")
    try:
        answer = invoke_model("In two sentences, what is retrieval-augmented generation (RAG)?")
        print("ANSWER:", answer)
    except (NoCredentialsError, ClientError) as e:
        print("Could not call Bedrock live (this is expected without AWS creds / model access).")
        print(f"  Reason: {type(e).__name__}: {e}")
        print("\nThe code above is the exact API shape. To run live:")
        print("  1) aws configure   2) enable Claude access in the Bedrock console")

    print("\n=== Bedrock: RAG via Knowledge Bases (retrieve_and_generate) ===")
    print("Uncomment below and supply a knowledgeBaseId created in the console:")
    print('  answer, citations = rag_with_knowledge_base("What is the late invoice penalty?", "KB123ABC")')
    print("\nThis is the AWS mirror of Azure OpenAI 'On Your Data' over Azure AI Search.")


if __name__ == "__main__":
    main()
