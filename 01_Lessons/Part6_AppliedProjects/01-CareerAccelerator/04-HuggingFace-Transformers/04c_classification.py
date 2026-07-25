"""
04c_classification.py — Zero-shot classification with Hugging Face

Classify text into labels WITHOUT training a model, using facebook/bart-large-mnli.
You supply the candidate labels at inference time.

Azure bridge: this is the open-source alternative to Azure AI Language custom text
classification — but with zero labeled data and zero training. Great for triage/routing.

Run:
    pip install -r requirements.txt
    python 04c_classification.py
"""

from transformers import pipeline

MODEL = "facebook/bart-large-mnli"   # trained on NLI -> can judge entailment for any labels


def main():
    print(f"Loading zero-shot-classification pipeline ({MODEL})... first run downloads ~1.6 GB.\n")
    classifier = pipeline("zero-shot-classification", model=MODEL)

    # Labels chosen at INFERENCE time — no training needed. Change them freely.
    labels = ["billing dispute", "delivery issue", "warranty claim", "general inquiry"]

    tickets = [
        "My invoice shows a 2% late penalty I don't think I owe.",
        "The vehicle was supposed to arrive last week and it's still not here.",
        "The transmission failed and I need it repaired under coverage.",
        "What are your office hours during the holidays?",
    ]

    for ticket in tickets:
        result = classifier(ticket, candidate_labels=labels)
        top_label = result["labels"][0]
        top_score = result["scores"][0]
        print(f"Ticket: {ticket}")
        print(f"  -> {top_label}  (confidence {top_score:.2f})\n")

    print("Takeaway: zero-shot means you pick labels at runtime with no training data.")
    print("Perfect for a cheap triage/routing tier before an expensive LLM call.")


if __name__ == "__main__":
    main()
