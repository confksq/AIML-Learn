"""
04a_text_generation.py — Text generation with Hugging Face `pipeline`

Demonstrates the HF one-liner API and how temperature / top_p shape output.
Uses distilgpt2 (~350 MB) so it runs on CPU with no paid API.

Azure bridge: pipeline("text-generation") is the open-source equivalent of an
Azure OpenAI chat completion; temperature/top_p behave exactly as in the Azure SDK.

Run:
    pip install -r requirements.txt
    python 04a_text_generation.py
"""

from transformers import pipeline

MODEL = "distilgpt2"          # tiny GPT-2 variant; swap for "gpt2" or a local Mistral/Phi-3
PROMPT = "The future of enterprise AI is"


def main():
    print(f"Loading text-generation pipeline ({MODEL})... first run downloads the model.\n")
    # pipeline() wraps tokenizer + model + decoding into one callable.
    generator = pipeline("text-generation", model=MODEL)

    # --- Low temperature: focused, more deterministic ---
    print("=== temperature=0.3 (focused / factual) ===")
    out = generator(
        PROMPT,
        max_new_tokens=40,
        temperature=0.3,      # low randomness -> sticks to high-probability tokens
        top_p=0.9,            # nucleus sampling: consider tokens up to 90% cumulative prob
        do_sample=True,
        truncation=True,
        pad_token_id=generator.tokenizer.eos_token_id,
    )
    print(out[0]["generated_text"], "\n")

    # --- High temperature: more creative / varied ---
    print("=== temperature=1.0 (creative / varied) ===")
    out = generator(
        PROMPT,
        max_new_tokens=40,
        temperature=1.0,      # higher randomness -> more diverse, higher hallucination risk
        top_p=0.95,
        do_sample=True,
        truncation=True,
        pad_token_id=generator.tokenizer.eos_token_id,
    )
    print(out[0]["generated_text"], "\n")

    print("Takeaway: same model, same prompt — temperature/top_p change the output character.")
    print("For factual/RAG tasks use low temperature; for brainstorming use higher. Same as Azure OpenAI.")


if __name__ == "__main__":
    main()
