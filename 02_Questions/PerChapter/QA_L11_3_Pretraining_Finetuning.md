# Q&A — L11_3: LLMs — Pre-training & Fine-tuning
**Source chapter:** `01_Lessons/Part3_GenAI_LLMs/L11_3_LLMs_Pretraining_Finetuning.md` | **Format:** self-study
**Questions:** 28 | *No overlap with the interview bank (01_Fundamentals Q7/Q11/Q12/Q15 cover the architect-judgment versions) or the chapter's own mini quiz.*

---

## Pre-training

**Q1. Walk the 6-step pre-training loop using the chapter's "recovery ticket" sentence.**
(1) Take a training sentence → (2) show the model a partial sequence ("...requires submitting a") with the next token as target ("ticket") → (3) model outputs a probability distribution over all ~100k tokens (ticket 34%, form 21%, request 18%…) → (4) compare to the actual token, compute loss → (5) **backpropagation** nudges all weights to make the right token more likely (gradient descent) → (6) slide forward one token and repeat — trillions of times across billions of documents.

**Q2. What four things does the model learn purely from next-token prediction?**
Grammar/syntax (which patterns follow which), facts (repeated patterns like "Paris is the capital of…"), reasoning chains (cause-effect patterns in text), and style/tone/domain knowledge. Key insight: none of it was explicitly programmed — all extracted from the prediction task.

**Q3. Name five sources in GPT-class pre-training data, and GPT-3's total token count.**
Common Crawl web pages, books (Books1/2, Gutenberg), code (GitHub, StackOverflow), Wikipedia (all languages), news, curated sets (WebText, C4, The Pile). GPT-3: ~**300 billion tokens**; GPT-4's is undisclosed and larger.

**Q4. Explain the knowledge cutoff mechanically — where does the date actually come from?**
The pre-training dataset has a **collection snapshot date** — everything after that date simply isn't in the data, so the model can't know it, and will often hallucinate a plausible answer instead of knowing it doesn't know. This is the chapter's strongest argument for RAG: keep knowledge current without retraining.

**Q5. What is a base/foundation model, and what happens if you ask one a direct question?**
The raw output of pre-training (GPT-3 base, LLaMA base, Mistral base) — excellent at language and broad facts, but **not an assistant**. Asked "How do I recover my laptop files?", it might autocomplete your sentence, write a Wikipedia-style article, or ramble — anything but a direct assistant-format answer. Base models are raw material; further training makes them assistants.
*Memory hook: "Base model = knows language, not an assistant."*

---

## Fine-tuning

**Q6. Why is fine-tuning so much cheaper than pre-training? Three reasons.**
(1) You start from a model that already knows language; (2) only a few epochs on a small dataset (thousands of examples, not billions); (3) weights are adjusted slightly, not learned from scratch.

**Q7. Name the three fine-tuning types and what each targets.**
**Instruction fine-tuning (SFT)** — teach the assistant format via instruction-response pairs (how GPT-3 base became InstructGPT/ChatGPT). **Domain fine-tuning** — domain knowledge/vocabulary (internal docs, terminology, processes). **Task-specific fine-tuning** — a specific output format or task (classification labels, always-JSON, fixed tone).

**Q8. What does Azure OpenAI's fine-tuning training file look like?**
JSONL — one JSON object per line, each a full `messages` array: `{"messages": [{"role":"system",...},{"role":"user",...},{"role":"assistant",...}]}`. Upload the file; Azure runs the training (LoRA-style parameter-efficient under the hood) and gives you a fine-tuned deployment endpoint.

**Q9. What are the fine-tuning numbers to know: minimum examples, typical epochs, when-to-bother rule?**
Azure OpenAI minimum: **10 examples** (recommended 50–1,000+). Typical epochs: **3–5**. Rule of thumb: fine-tune only with **100+ labeled examples AND a problem prompting can't solve**. And quality beats quantity: 500 clean examples beat 50,000 noisy ones.

---

## LoRA & QLoRA

**Q10. Explain LoRA's mechanics — what's frozen, what's trained, what's the inference formula?**
The original weight matrices **W stay frozen** (never updated). LoRA adds two small matrices **A and B**; only they train. Inference: `output = W·x + A·B·x`. A and B together are ~**0.1–1%** of total parameters.

**Q11. Why does LoRA work at all — what property of LLM weights does it exploit?**
Weight matrices are highly redundant — the fine-tuning "signal" lives in a **low-dimensional subspace**, so two small low-rank matrices can capture it without touching the bulk of the model.

**Q12. Contrast full fine-tune vs LoRA on GPT-3-scale hardware.**
Full fine-tune of GPT-3: ~5TB GPU memory, weeks of compute. LoRA: a **single A100**, hours to days — with quality typically within **1–3%** of full fine-tuning.

**Q13. What does QLoRA add, and what does it make possible?**
Quantizes the **frozen base weights to 4-bit** (NF4) while training the LoRA adapters at 16-bit → ~4x less GPU memory → fine-tuning a **70B model on a single 24GB consumer GPU**, previously impossible.
*Memory hook: "LoRA = freeze big, train tiny adapters; QLoRA = same + 4-bit frozen weights."*

**Q14. Where does LoRA show up in the Azure ecosystem, per the 2026 updates?**
Azure OpenAI fine-tuning uses LoRA-style methods under the hood (GPT-4o and 4o-mini fine-tuning now GA, JSONL workflow; vision fine-tuning supports image inputs too). **AI Foundry's model catalog** offers LoRA fine-tuning on open-source models (Llama, Phi, Mistral) directly in the portal — no GPU cluster management.

---

## The Decision Framework

**Q15. Recite the three levers and their cost/when-to-use one-liners.**
**Prompt engineering** — zero cost, always try first. **RAG** — low-medium cost (retrieval+embedding infra), for up-to-date/company-specific/large-volume knowledge. **Fine-tuning** — high cost (compute + data prep), for behavior/format changes prompting can't achieve.
*Memory hook: "Default order: Prompt → RAG → Fine-tune."*

**Q16. Recite the decision tree.**
Can a well-written system prompt solve it? YES → done. NO → does the model need facts/docs it doesn't know? YES → **RAG** (knowledge problem). NO → is the issue behavior/tone/format/domain vocabulary? YES → **fine-tune**. NO → needs a fundamentally different capability → specialized model/different approach.

**Q17. Four scenarios where fine-tuning is the WRONG answer — give the right one for each.**
Model doesn't know current IT policy → **RAG** (policies change; retraining is slow). Answers too long → **system prompt** ("Be concise, 2-3 sentences"). Doesn't know your ticket system's name → **system prompt** ("Our ticket system is ServiceNow"). Needs live ticket status → **function calling/tool use**.

**Q18. Five scenarios where fine-tuning IS right.**
(1) Consistent output format that must never break (always-JSON — prompts can be overridden, weights can't); (2) deep domain vocabulary that can't be injected via RAG; (3) removing a long system prompt to cut latency/cost by baking behavior in; (4) making a small model behave like a big one (distillation on GPT-4 outputs); (5) strict tone/persona enforcement more robust than prompting.

**Q19. In the JMA IT-helpdesk assessment, which single use case did the chapter greenlight for fine-tuning?**
**Consistent JSON output for ticket creation** — policy answers stay RAG (frequent updates), tone/terminology start with system prompt. Overall recommendation: RAG + prompt engineering first; fine-tune only with 1,000+ labeled examples AND a prompting-resistant behavior problem.

**Q20. Define transfer learning with the chapter's doctor analogy.**
Take a model trained on a large general task and adapt it to a specific one — like hiring a doctor and training them on your hospital's EMR system: you don't re-teach medicine, just the hospital-specific procedures on top. Pre-training gave language + general patterns; fine-tuning adds your company's specifics; you never pay for pre-training again.

---

## Misconceptions & Updates

**Q21. Correct: "fine-tuning makes the model smarter."**
It changes **behavior**, not intelligence — the model learns your format, vocabulary, and style; it gains no new reasoning capability.

**Q22. Correct: "the model remembers fine-tuning data exactly."**
It learns **patterns**, not verbatim text — fine-tuning doesn't create a lookup table (which is also why fine-tuning is the wrong tool for injecting retrievable facts).

**Q23. Correct: "prompt engineering is just a workaround until you fine-tune."**
It's a legitimate, maintainable production approach — many production systems never need fine-tuning at all.

**Q24. Correct: "RAG and fine-tuning are interchangeable."**
RAG solves **knowledge** problems; fine-tuning solves **behavior** problems. They're complementary — production systems often use both.

**Q25. What's the Phi-4 lesson from the 2026 updates?**
Phi-4 (3.8B parameters) reaches near-GPT-4 performance on many benchmarks at ~10x lower cost — **model size ≠ intelligence; training-data quality matters more than parameter count.**

**Q26. Why can't you fine-tune your own o1/o3-style reasoning model behavior?**
Reasoning models use a **separate training regime** (internally generated chain-of-thought reasoning tokens) that isn't standard fine-tuning and isn't publicly exposable — it's a provider-side capability, not a customer-tunable one.

**Q27. What does overfitting look like in a fine-tuning loss curve (chapter Exercise 4)?**
Training loss keeps falling while **validation loss stops falling and starts rising** — the model is memorizing training examples instead of generalizing. Stop at the epoch where validation loss bottoms out. Underfitting = both losses still falling (train longer); good fit = both plateau together.

**Q28. Fine-tune decision drill (chapter Exercise 1) — answer all five JMA scenarios.**
Dealer questions from monthly-changing policy docs → **RAG**. Always formal business English → **prompt engineering** (fine-tune only if prompting proves insufficient). Classify tickets into 5 consistent categories → **fine-tune** if high volume + label consistency matters (or few-shot prompting first). "iPacket = our digital retailing tool" → **system prompt** (single term doesn't justify training). Live inventory levels → **function calling** — neither RAG-static nor fine-tuning handles live data.

---

*Curriculum Q&A Batch C — file 1 of 4. Next: QA_L11_4 (RLHF & Alignment).*
