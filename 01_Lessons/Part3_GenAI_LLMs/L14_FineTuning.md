# Module 15 — Fine-tuning LLMs
**Part 3: Generative AI & LLMs | AI Solutions Architect Curriculum**

---

## What You Already Know (Recap)

From prior modules:
- **Module 11.3** — Fine-tuning concept: SFT, domain fine-tuning, LoRA/QLoRA theory
- **Module 11.3** — Decision framework: Fine-tuning vs RAG vs Prompt Engineering
- **Module 12** — Azure OpenAI deployments, Chat Completions API, function calling
- **Module 13** — RAG pipelines: when retrieval solves the knowledge problem
- **Module 14** — SK agents: when orchestration solves the multi-step problem

This module makes fine-tuning practical — when to do it, how to do it in Azure OpenAI, and how LoRA/QLoRA work in production.

---

**Running example (used throughout):**
> *JM Family needs an AI assistant that always responds in a specific structured format, uses automotive finance terminology correctly, and maintains a consistent professional tone. Prompt engineering alone cannot guarantee this at scale. This is the fine-tuning problem.*

---

## Topic 15.1 — When to Fine-tune

---

### 1. The Decision Framework — Revisited

You saw this in Module 11.3. Here it is applied to real decisions:

```
PROBLEM                              SOLUTION
────────────────────────────────────────────────────────────────
Need up-to-date or private data      → RAG
  "What are today's overdue invoices?"
  Data changes constantly — fine-tuning cannot help

Need consistent output FORMAT        → Fine-tuning
  "Always return JSON with these exact fields"
  Prompting works but is unreliable at scale

Need specific TONE or STYLE          → Fine-tuning
  "Always respond in JM Family brand voice"
  Hard to enforce with prompts alone

Need DOMAIN VOCABULARY understood    → Fine-tuning
  "Understand terms: floorplan, curtailment, dealer reserve"
  Model keeps confusing automotive finance terms

Need one-off or rare task            → Prompt Engineering
  "Summarize this document"
  GPT-4o already does this well — no fine-tuning needed

Need to SAVE COST at massive scale   → Fine-tuning
  Fine-tune a smaller model (GPT-4o mini)
  It performs like GPT-4o on your specific task
  Cost: 10x cheaper per call
```

---

### 2. The Four Legitimate Reasons to Fine-tune

```
REASON 1 — FORMAT CONSISTENCY
  Problem: LLM sometimes returns JSON, sometimes plain text
  Fine-tuning: train on 100+ examples of correct JSON format
  Result: 99%+ consistent structured output
  JM Family use: invoice extraction always returns same JSON schema

REASON 2 — DOMAIN VOCABULARY
  Problem: model confuses "curtailment" (automotive finance term)
            with "curtailment" (architecture term)
  Fine-tuning: train on JM Family domain text
  Result: model understands your specific terminology
  JM Family use: dealer finance terms used correctly every time

REASON 3 — STYLE AND TONE
  Problem: model responses are too casual / too verbose /
            not in company brand voice
  Fine-tuning: train on examples of ideal response style
  Result: consistent voice without long style instructions in every prompt
  JM Family use: dealer-facing assistant maintains professional tone

REASON 4 — COST OPTIMISATION AT SCALE
  Problem: GPT-4o is expensive — 1 million calls/month = huge bill
  Fine-tuning: train GPT-4o mini on GPT-4o quality examples
  Result: small model does your specific task as well as the big model
  Cost: GPT-4o mini fine-tuned ≈ 10x cheaper than GPT-4o
  JM Family use: high-volume invoice classification at low cost
```

---

### 3. When NOT to Fine-tune

```
DO NOT FINE-TUNE when:

  ✗ You need current or private data
    → Use RAG (Module 13)
    Fine-tuning bakes knowledge into weights at training time
    It cannot access live data at inference time

  ✗ You have fewer than 50-100 training examples
    → Use few-shot prompting (Module 16)
    Fine-tuning with tiny datasets causes overfitting
    Model memorises examples rather than learning the pattern

  ✗ The task changes frequently
    → Use prompt engineering
    Re-fine-tuning every time requirements change is expensive
    Prompt updates are instant and free

  ✗ GPT-4o already does it well enough
    → Do not fine-tune
    Fine-tuning adds cost, complexity, and maintenance burden
    Only fine-tune when base model genuinely falls short
```

---

### 4. Fine-tuning vs RAG vs Prompt Engineering — Decision Table

```
Scenario                                Fine-tune  RAG   Prompt Eng
──────────────────────────────────────────────────────────────────
Answer questions about private docs        ✗        ✓       ✗
Always return specific JSON format         ✓        ✗       ~
Understand company-specific terms          ✓        ~       ~
Summarise a document                       ✗        ✗       ✓
Reduce cost on high-volume task            ✓        ✗       ✗
Answer questions about today's data        ✗        ✓       ✗
Match brand voice consistently             ✓        ✗       ~
One-off custom task                        ✗        ✗       ✓
Multi-step task with tools                 ✗        ✗       ✓ (agent)

✓ = best solution  ~ = works but not ideal  ✗ = wrong tool
```

---

### 5. Cost Reality Check Before Fine-tuning

Fine-tuning is not free. Calculate before committing:

```
Azure OpenAI Fine-tuning costs (approximate 2025/2026):

  Training:
    GPT-4o mini: ~$0.003 per 1K training tokens
    1,000 examples × 500 tokens each = 500K tokens
    Training cost ≈ $1.50 per epoch
    Typical: 3-5 epochs = $5-8 total training cost

  Deployment (hosting the fine-tuned model):
    $1.70 per hour (dedicated deployment)
    ~$40/day just to keep it running
    → Only economical at high call volume

  Inference:
    Fine-tuned GPT-4o mini: ~$0.0003 per 1K tokens input
    Standard GPT-4o: ~$0.005 per 1K tokens input
    → Fine-tuned model is ~16x cheaper per call

  Break-even calculation:
    If you make 10,000+ calls/day → fine-tuning saves money
    If you make 100 calls/day → hosting cost exceeds savings
```

---

## Topic 15.2 — Azure OpenAI Fine-tuning

---

### 1. Supported Models for Fine-tuning in Azure OpenAI

```
Model                   Fine-tuning   Recommended for
──────────────────────────────────────────────────────────────
GPT-4o mini             ✓             Most fine-tuning scenarios
                                      Cost-efficient, good quality
                                      Best choice for JM Family

GPT-4o                  ✓ (limited)   When quality is critical
                                      Significantly more expensive

GPT-3.5-turbo           ✓ (legacy)    Older option, being phased out
                                      Use GPT-4o mini instead

Embedding models        ✗             Cannot fine-tune embedding models
                                      Use as-is or change chunking strategy
```

---

### 2. Training Data Format — JSONL

Azure OpenAI fine-tuning requires training data in JSONL format.
Each line = one training example = one complete conversation.

```json
{"messages": [
  {"role": "system", "content": "You are a JM Family invoice assistant. Always respond in JSON format with fields: status, amount, dealer_code, action_required."},
  {"role": "user", "content": "What is the status of invoice JMF-ATL-001-2026?"},
  {"role": "assistant", "content": "{\"status\": \"overdue\", \"amount\": 47250.00, \"dealer_code\": \"JMF-ATL-001\", \"action_required\": \"Send follow-up within 48 hours\"}"}
]}
{"messages": [
  {"role": "system", "content": "You are a JM Family invoice assistant. Always respond in JSON format with fields: status, amount, dealer_code, action_required."},
  {"role": "user", "content": "Check invoice JMF-DAL-003-2026 status"},
  {"role": "assistant", "content": "{\"status\": \"paid\", \"amount\": 23100.00, \"dealer_code\": \"JMF-DAL-003\", \"action_required\": \"none\"}"}
]}
```

```
Rules for training data:
  Minimum:   10 examples (Azure allows it — quality will be low)
  Practical: 50-100 examples minimum for reliable results
  Ideal:     200-500 examples for production quality
  Format:    JSONL — one JSON object per line, no commas between lines
  Each line: complete conversation with system + user + assistant
  File size: max 512 MB
  Encoding:  UTF-8
```

---

### 3. Preparing Good Training Data

Quality of training data is everything. Bad data → bad fine-tuned model.

```
GOOD training examples:
  ✓ Diverse inputs — many different ways users ask the same question
  ✓ Consistent outputs — assistant always follows the exact same format
  ✓ Representative — covers all cases the model will see in production
  ✓ Clean — no typos in assistant responses, no wrong answers
  ✓ Balanced — not 90% one type of example

BAD training examples:
  ✗ Duplicate inputs — model memorises, does not generalise
  ✗ Inconsistent outputs — model learns conflicting patterns
  ✗ Only easy cases — model fails on edge cases in production
  ✗ Wrong answers — model learns to give wrong answers reliably

JM Family data preparation:
  Source: existing invoice Q&A logs where response was correct
  Clean: remove any responses that were flagged as wrong
  Augment: generate variations of questions (same invoice, different phrasing)
  Split: 80% training, 20% validation
```

---

### 4. The Fine-tuning Workflow in Azure OpenAI

```
Step 1: Prepare training data (JSONL file)
Step 2: Upload file to Azure OpenAI
Step 3: Create fine-tuning job
Step 4: Monitor training progress
Step 5: Deploy fine-tuned model
Step 6: Test and evaluate
Step 7: Use in your application
```

---

### 5. C# — Complete Fine-tuning Workflow

```csharp
using Azure.AI.OpenAI;
using Azure;
using System.ClientModel;

var endpoint = new Uri("https://jmf-openai.openai.azure.com/");
var credential = new DefaultAzureCredential();
var client = new AzureOpenAIClient(endpoint, credential);

// ── STEP 1: Upload training file ──────────────────────────────
var fileClient = client.GetOpenAIFileClient();

using var trainingStream = File.OpenRead("jmf_invoice_training.jsonl");
var uploadResponse = await fileClient.UploadFileAsync(
    file: trainingStream,
    filename: "jmf_invoice_training.jsonl",
    purpose: FileUploadPurpose.FineTune
);

string trainingFileId = uploadResponse.Value.Id;
Console.WriteLine($"Training file uploaded: {trainingFileId}");

// ── STEP 2: Upload validation file (optional but recommended) ──
using var validationStream = File.OpenRead("jmf_invoice_validation.jsonl");
var validationResponse = await fileClient.UploadFileAsync(
    file: validationStream,
    filename: "jmf_invoice_validation.jsonl",
    purpose: FileUploadPurpose.FineTune
);

string validationFileId = validationResponse.Value.Id;

// ── STEP 3: Create fine-tuning job ────────────────────────────
var fineTuningClient = client.GetFineTuningClient();

var jobResponse = await fineTuningClient.CreateJobAsync(
    new FineTuningJobCreationOptions(
        model: "gpt-4o-mini",
        trainingFile: trainingFileId)
    {
        ValidationFile = validationFileId,
        Hyperparameters = new FineTuningJobHyperparameters
        {
            // null = Azure auto-selects (recommended to start)
            NEpochs = null,
            BatchSize = null,
            LearningRateMultiplier = null
        },
        Suffix = "jmf-invoice-v1"   // fine-tuned model name suffix
    }
);

string jobId = jobResponse.Value.Id;
Console.WriteLine($"Fine-tuning job created: {jobId}");
Console.WriteLine($"Status: {jobResponse.Value.Status}");

// ── STEP 4: Monitor training progress ─────────────────────────
while (true)
{
    await Task.Delay(TimeSpan.FromMinutes(1));

    var statusResponse = await fineTuningClient.GetJobAsync(jobId);
    var job = statusResponse.Value;

    Console.WriteLine($"Status: {job.Status} | " +
                      $"Trained tokens: {job.TrainedTokens}");

    if (job.Status == FineTuningJobStatus.Succeeded)
    {
        Console.WriteLine($"Fine-tuned model: {job.FineTunedModel}");
        break;
    }

    if (job.Status == FineTuningJobStatus.Failed ||
        job.Status == FineTuningJobStatus.Cancelled)
    {
        Console.WriteLine($"Job failed: {job.Error?.Message}");
        break;
    }
}

// ── STEP 5: Use fine-tuned model ──────────────────────────────
// Deploy the fine-tuned model in Azure OpenAI Studio (portal)
// Then use it exactly like any other deployment:

var chatClient = client.GetChatClient("jmf-invoice-v1-deployment");

var messages = new List<ChatMessage>
{
    new SystemChatMessage(
        "You are a JM Family invoice assistant. " +
        "Always respond in JSON format."),
    new UserChatMessage(
        "What is the status of invoice JMF-ATL-001-2026?")
};

var response = await chatClient.CompleteChatAsync(messages);
Console.WriteLine(response.Value.Content[0].Text);
// Output is now reliably JSON — the fine-tuned behaviour
```

---

### 6. Monitoring Training Quality — Loss Curves

Azure OpenAI shows training metrics you must understand:

```
Training loss:
  Measures how well the model fits training data
  Should decrease steadily over epochs
  If not decreasing → learning rate too low or data quality issue

Validation loss:
  Measures how well model generalises to unseen examples
  Should also decrease (roughly tracking training loss)

Overfitting signal:
  Training loss keeps decreasing
  Validation loss starts INCREASING
  ← Model is memorising training data, not learning the pattern
  Fix: add more training examples, reduce epochs, add diversity

Good training signal:
  Both curves decrease and level off together
  Small gap between training and validation loss
```

```
Epoch guidance:
  1-2 epochs:   Underfitting — model has not learned enough
  3-4 epochs:   Usually optimal for most fine-tuning tasks
  5+ epochs:    Risk of overfitting — monitor validation loss carefully
```

---

### 7. Evaluating the Fine-tuned Model

Before deploying to production, always evaluate:

```csharp
// Simple evaluation — compare fine-tuned vs base model on test set
public async Task<EvaluationResult> EvaluateFineTunedModelAsync(
    List<TestCase> testCases,
    string fineTunedDeployment,
    string baseDeployment)
{
    int fineTunedCorrect = 0;
    int baseCorrect = 0;

    foreach (var testCase in testCases)
    {
        // Test fine-tuned model
        var ftResponse = await GetResponseAsync(fineTunedDeployment, testCase.Input);
        if (IsCorrectFormat(ftResponse) && MatchesExpected(ftResponse, testCase.Expected))
            fineTunedCorrect++;

        // Test base model
        var baseResponse = await GetResponseAsync(baseDeployment, testCase.Input);
        if (IsCorrectFormat(baseResponse) && MatchesExpected(baseResponse, testCase.Expected))
            baseCorrect++;
    }

    return new EvaluationResult
    {
        FineTunedAccuracy = (double)fineTunedCorrect / testCases.Count,
        BaseModelAccuracy = (double)baseCorrect / testCases.Count,
        Improvement = fineTunedCorrect - baseCorrect
    };
}
```

```
Minimum bar for production:
  Fine-tuned model accuracy > base model accuracy
  Format compliance: 99%+ (this is what you fine-tuned for)
  Regression check: fine-tuned model should not be WORSE on general tasks
```

---

## Topic 15.3 — Parameter-Efficient Fine-tuning (LoRA and QLoRA)

---

### 1. Why Full Fine-tuning Is Expensive

Understanding the problem that LoRA solves:

```
Full fine-tuning:
  Take a model with 7 billion parameters
  Update ALL 7 billion parameters during training
  Requires: ~28 GB GPU memory (7B params × 4 bytes each)
  A100 GPU costs: ~$3/hour
  Training time: hours to days
  Result: entire new copy of the model (28 GB storage)

  For GPT-4o (estimated 200B+ parameters):
  Cannot fine-tune fully — not even possible on most hardware
  Azure OpenAI fine-tuning = Microsoft runs this for you
                              you pay per training token
```

---

### 2. LoRA — Low-Rank Adaptation

LoRA solves the memory problem by only training small adapter layers:

```
Core idea:
  A large weight matrix W can be approximated as:
    W = W₀ + ΔW
    where W₀ = original frozen weights (never updated)
          ΔW = the change needed for your task

  ΔW is a large matrix — still expensive to train

  LoRA factorises ΔW into two small matrices:
    ΔW = A × B
    where A is (d × r) and B is (r × d)
          r = rank (a small number like 4, 8, or 16)
          d = original dimension (large, like 4096)

  Instead of training ΔW (d × d = 4096 × 4096 = 16M params)
  LoRA trains A and B (d×r + r×d = 2 × 4096 × 8 = 65K params)

  Parameters trained: ~0.4% of original model
  Memory saved: ~95%
```

```
Visual:

  Original layer:   [W₀]  ← frozen, never changes
                     +
  LoRA adapters:    [A] × [B]  ← only these 0.4% of params train

  At inference:     output = W₀(input) + A(B(input))
                    Adapter result added to frozen result
```

---

### 3. LoRA Key Hyperparameters

```
Rank (r):
  Controls adapter size
  r = 4:   Smallest — fast, less expressive, good for simple tasks
  r = 8:   Common default — good balance
  r = 16:  Larger — more expressive, more memory
  r = 64:  Approaching full fine-tuning territory

  JM Family: r = 8 is the right starting point

Alpha (α):
  Scaling factor for the adapter output
  α = 16 with r = 8 → scaling = α/r = 2
  Common: set α = 2× rank (e.g. r=8, α=16)

Target modules:
  Which layers to apply LoRA to
  Common: query and value projection layers (q_proj, v_proj)
  More layers = more capacity but more memory
```

---

### 4. QLoRA — Quantized LoRA

QLoRA takes LoRA further by also quantizing the frozen base model:

```
LoRA:
  Frozen weights: stored in float16 (2 bytes per param)
  Adapters: trained in float16
  Memory for 7B model: ~14 GB

QLoRA:
  Frozen weights: compressed to 4-bit (0.5 bytes per param)
  Adapters: trained in bfloat16 (better precision for training)
  Memory for 7B model: ~5 GB

  Same quality as LoRA
  Uses 3x less GPU memory
  Can fine-tune larger models on smaller GPUs
```

```
When to use which:
  LoRA:   You have reasonable GPU memory (16GB+)
          Slightly faster training
          Open-source models (Llama, Mistral, Phi)

  QLoRA:  Limited GPU memory (8-12 GB)
          Want to fine-tune larger models
          Azure ML with smaller GPU SKUs
          Open-source models on a budget

  Azure OpenAI fine-tuning (Topic 15.2):
          You do not control LoRA vs QLoRA — Microsoft decides
          You just upload data and pay per token
          Best for GPT-4o mini on Azure
```

---

### 5. LoRA vs Azure OpenAI Fine-tuning — Which to Use

```
USE AZURE OPENAI FINE-TUNING when:
  ✓ Your app already uses Azure OpenAI (GPT-4o mini, GPT-4o)
  ✓ You want managed infrastructure — no GPU setup
  ✓ You need enterprise compliance (data stays in your Azure tenant)
  ✓ Small-medium dataset (100-10,000 examples)
  ✓ JM Family production scenario — this is the right choice

USE LORA/QLORA when:
  ✓ You are fine-tuning an open-source model (Llama, Mistral, Phi-3)
  ✓ You want to own the model weights (not pay per call)
  ✓ Very large dataset — more cost-effective at scale
  ✓ Research or experimentation
  ✓ Azure ML with your own GPU compute
  ✗ NOT for GPT-4o — you cannot access GPT-4o weights
```

---

### 6. LoRA Fine-tuning with Azure ML — Python (Awareness Level)

You need to know this exists and how it looks — you will not write it daily but interviewers ask about it:

```python
from transformers import AutoModelForCausalLM, AutoTokenizer
from peft import LoraConfig, get_peft_model, TaskType
from transformers import TrainingArguments, Trainer

# Load base model — Phi-3 mini (Microsoft's small open model)
model_name = "microsoft/Phi-3-mini-4k-instruct"
model = AutoModelForCausalLM.from_pretrained(model_name)
tokenizer = AutoTokenizer.from_pretrained(model_name)

# Configure LoRA
lora_config = LoraConfig(
    task_type=TaskType.CAUSAL_LM,
    r=8,                          # rank
    lora_alpha=16,                # scaling factor
    target_modules=["q_proj", "v_proj"],  # which layers to adapt
    lora_dropout=0.05,
    bias="none"
)

# Wrap base model with LoRA adapters
model = get_peft_model(model, lora_config)
model.print_trainable_parameters()
# Output: trainable params: 2,097,152 || all params: 3,823,000,000
#         trainable%: 0.055%  ← only 0.055% of params train

# Train (standard HuggingFace Trainer)
training_args = TrainingArguments(
    output_dir="./jmf-phi3-lora",
    num_train_epochs=3,
    per_device_train_batch_size=4,
    learning_rate=2e-4,
    fp16=True,                    # use float16 for training speed
)

trainer = Trainer(
    model=model,
    args=training_args,
    train_dataset=train_dataset,
    eval_dataset=eval_dataset,
)

trainer.train()

# Save only the LoRA adapters (small — a few MB, not the full model)
model.save_pretrained("./jmf-phi3-lora-adapters")
```

```
What gets saved:
  Full model (Phi-3 mini):    ~7 GB
  LoRA adapters only:         ~8 MB   ← this is all you store and share
  At inference: load base model + apply adapters
```

---

### 7. Complete Picture — Fine-tuning Decision Flow

```
NEED                              SOLUTION
──────────────────────────────────────────────────────────────────────
Private/live data in answers    → RAG (Module 13) — not fine-tuning

Consistent JSON output format   → Azure OpenAI fine-tuning (GPT-4o mini)
  JM Family: invoice extraction   100-500 JSONL examples
  stays in Azure tenant           $5-10 training + hosting cost

Brand voice / tone              → Azure OpenAI fine-tuning (GPT-4o mini)
  Consistent dealer comms         200+ style examples

Reduce cost on high volume      → Azure OpenAI fine-tuning (GPT-4o mini)
  10,000+ calls/day               Train small model to match big model

Fine-tune open-source model     → LoRA on Azure ML
  Llama, Mistral, Phi-3           Python + HuggingFace PEFT library
  Own the weights                 GPU compute in Azure ML

Limited GPU memory              → QLoRA on Azure ML
  Same as LoRA but 4-bit quant    Works on smaller GPU SKUs

Quick task with few examples    → Few-shot prompt (Module 16)
  < 50 examples                   No fine-tuning needed
```

---

## Module 15 — Self-Test Questions

**Q1.** A JM Family manager says "The AI keeps giving different JSON formats for invoice extraction — sometimes it adds extra fields, sometimes it skips fields. How do you fix this?" What is the right solution and why?

> **A:** Fine-tune GPT-4o mini on 100-500 JSONL training examples where every assistant response is the exact correct JSON schema. Fine-tuning teaches the model the output format through repeated examples — prompt engineering alone cannot guarantee 99%+ format consistency at scale. RAG is not the solution because this is a format/behaviour problem, not a data retrieval problem.

---

**Q2.** What is the minimum you need to start a fine-tuning job in Azure OpenAI and what is the realistic minimum for production quality?

> **A:** Azure OpenAI allows fine-tuning with as few as 10 examples (technical minimum). But 10 examples leads to overfitting — the model memorises rather than learns. Practical minimum for reliable results: 50-100 examples. Production quality: 200-500 diverse, clean, representative examples with a separate validation set of 20% held out.

---

**Q3.** What does LoRA actually do differently from full fine-tuning? Use the matrix math idea — no need for exact formulas.

> **A:** Full fine-tuning updates all model weights — for a 7B parameter model that means updating 7 billion numbers, requiring ~28 GB GPU memory. LoRA freezes all original weights and instead learns two small matrices A and B whose product approximates the weight change needed. These two small matrices together contain only ~0.4% of the original parameter count. At inference, the original output and the adapter output are added together. Same task performance, ~95% less GPU memory.

---

**Q4.** A JM Family team wants to fine-tune a model to answer questions about their internal invoice policies. Is this the right tool? What would you recommend instead and why?

> **A:** No — this is the wrong tool. Fine-tuning bakes knowledge into model weights at training time. Invoice policies change (new rules, updated penalties, new dealer agreements). Every policy update would require re-fine-tuning the model (expensive, slow). The right solution is RAG — store policies in Azure AI Search, retrieve relevant chunks at query time, answer based on current documents. Fine-tuning is for behaviour and format, not for knowledge about changing data.

---

**Q5.** What is the difference between LoRA and QLoRA? When would you choose QLoRA?

> **A:** LoRA trains small adapter matrices while keeping the frozen base model in float16 (2 bytes per parameter). QLoRA adds 4-bit quantization of the frozen base model (0.5 bytes per parameter), reducing memory to roughly one-third of LoRA. Choose QLoRA when GPU memory is the constraint — fine-tuning a 7B model requires ~14 GB with LoRA but only ~5 GB with QLoRA. Both produce similar quality. For Azure OpenAI (GPT-4o mini), you do not control this — Microsoft handles the fine-tuning infrastructure.

---

**Q6.** Training loss is decreasing but validation loss starts increasing after epoch 3. What is happening and how do you fix it?

> **A:** This is overfitting — the model is memorising the training examples instead of learning the general pattern. It performs well on training data but fails to generalise to new inputs. Fix: (1) Stop training at epoch 3 (before validation loss diverges). (2) Add more training examples — more data reduces overfitting. (3) Add more variety to training examples — if they are too similar, the model memorises rather than generalises. (4) Reduce the number of epochs for future runs.

---

## Memory Hooks

- **"Fine-tune for BEHAVIOUR — RAG for KNOWLEDGE"**
- **"Format consistency, domain vocabulary, brand voice, cost at scale = the four reasons to fine-tune"**
- **"JSONL = one complete conversation per line — system + user + assistant"**
- **"LoRA = train 0.4% of parameters, freeze the rest — same result, 95% less memory"**
- **"QLoRA = LoRA + 4-bit quantize the frozen weights — fine-tune 7B model on 8 GB GPU"**
- **"Validation loss rising while training loss falls = overfitting — stop and add data"**
- **"Azure OpenAI fine-tuning = managed, stays in your tenant, GPT-4o mini = best choice"**
- **"LoRA adapters are tiny (8 MB) — the full model (7 GB) never changes"**
- **"Break-even: fine-tuning saves money only at 10,000+ calls/day"**
- **"< 50 examples = use few-shot prompting instead, not fine-tuning"**

---

---

## 2026 Updates

| Topic | Update |
|---|---|
| **GPT-4o fine-tuning GA** | Full GPT-4o (not just mini) now fine-tuneable in Azure OpenAI. Higher capability ceiling — worth it for complex tasks. Training cost ~$25 per 1M tokens. Vision fine-tuning also supported |
| **Preference fine-tuning (DPO)** | Direct Preference Optimization — alternative to RLHF for alignment fine-tuning. Provide pairs of (preferred response, rejected response) per prompt. Simpler than full RLHF, often comparable results |
| **LoRA in AI Foundry** | Fine-tune Llama 3, Phi-4, Mistral via LoRA directly in AI Foundry portal — no GPU cluster setup. Managed compute, output is a LoRA adapter (small file, not full model weights) |
| **Phi-4 fine-tuning** | Microsoft Phi-4 (3.8B) now fine-tuneable. Smaller model, cheaper inference, competitive quality for domain-specific tasks. Good JMA candidate: fine-tune for automotive/financial vocabulary |
| **Distillation** | New capability in Azure OpenAI — use GPT-4o outputs as training data for fine-tuning GPT-4o mini. Distill large model behavior into small model at fraction of inference cost |

---

## Interactive Learning Ideas

### Exercise 1 — Decision Framework Quiz (10 min)
Apply the fine-tune vs RAG vs prompt engineering decision tree to these JMA scenarios:
- "Respond only in formal business language" → ?
- "Know current dealer inventory levels" → ?
- "Always structure output as JSON with these specific field names" → ?
- "Be an expert in JM Family's specific contract terminology" → ?
- "Know that 'ATL' refers to our Southeast region" → ?

### Exercise 2 — JSONL Dataset Creation (20 min)
Create 15 JSONL fine-tuning examples that teach GPT-4o mini to classify dealer tickets into exactly these 5 categories with consistent label format:
- `DELIVERY_ISSUE`, `VEHICLE_DAMAGE`, `BILLING_DISPUTE`, `GENERAL_INQUIRY`, `ESCALATION_REQUIRED`
Make sure examples cover edge cases and ambiguous cases.

### Exercise 3 — LoRA Math (10 min)
A base model has a weight matrix W of size 4096 × 4096 = 16.7M parameters.
LoRA with rank r=16 uses: A (4096×16) + B (16×4096) = 131,072 parameters.
- What % of the original matrix's parameters does LoRA use?
- If the full model has 7B parameters and LoRA applies to all attention matrices (30% of params), how many parameters does LoRA train?
- Why does this dramatically reduce GPU memory requirements?

### Exercise 4 — Distillation Pipeline Design
Design a JMA distillation pipeline:
1. Generate 1,000 dealer support responses using GPT-4o (teacher model)
2. Use those as fine-tuning data for GPT-4o mini (student model)
3. Evaluate: does mini match 4o quality on your test set after fine-tuning?
4. Calculate cost savings: 1,000 calls/day, 4o vs mini pricing, over 12 months

---

*Previous: Module 14 — AI Orchestration*
*Next: Module 16 — Prompt Engineering*

---
---

## Where a Fine-Tuned Model Deploys — Separate, Not Merged (added 2026-08-02)

Common confusion worth heading off explicitly: if you already have a live deployment (say,
`customerservicejma`, bound to base `gpt-4o`), and you fine-tune a model, **the fine-tuned model does
NOT get added into that existing deployment.** It needs its own, separate deployment.

```
"customerservicejma" deployment
  └── bound to: base gpt-4o model
      (fixed, 1:1 binding — deployment name ↔ specific model)

"jmf-invoice-v1-deployment" (NEW, separate)
  └── bound to: your FINE-TUNED model
      (a genuinely different model artifact — not gpt-4o anymore,
       it's gpt-4o + your trained adapter/weights, with its own
       model ID)
```

**Why it can't merge:** a deployment is a **1:1 binding** to one specific model version (see the
Deployment concept — a named, capacity-controlled wrapper around a pinned model version). A
fine-tuned model gets its **own distinct model ID** once training succeeds — it isn't "gpt-4o with a
flag set." Since `customerservicejma` is already bound to base `gpt-4o`, there's no slot to insert the
fine-tuned model into — it needs a fresh deployment.

**This is exactly what `Suffix = "jmf-invoice-v1"` does in the C# fine-tuning workflow above (§5,
Step 3)** — it's naming the *new, separate* deployment you'll create once the job succeeds. It was
never going into an existing deployment.

### The practical rollout pattern

```
1. customerservicejma           → still running base gpt-4o, serving real users
2. jmf-invoice-v1-deployment    → NEW deployment, fine-tuned model, deployed
                                   alongside, not yet used by production traffic
3. Test jmf-invoice-v1-deployment thoroughly (same idea as RAGAS/evaluation —
   validate before promoting)
4. Update YOUR APPLICATION CODE to call "jmf-invoice-v1-deployment" instead of
   "customerservicejma" for that specific use case
5. Decommission customerservicejma once nothing calls it anymore — or keep both
   running side by side if only some traffic should use the fine-tuned model
   (a blue-green / gradual cutover)
```

**One-line summary:** two independent deployments in the same Azure OpenAI resource, each pointing at
a different model — your app decides which one to call by name.

---
---

## End-to-End Flow — Where Each Technology Runs (added 2026-08-02)

There are **two different paths** through fine-tuning, and mixing them up is the usual source of
confusion about "where does training actually happen." Both are covered separately below.

### Path A — Azure OpenAI's own managed fine-tuning (gpt-4o-mini, etc.)

```
1. YOU: prepare JSONL training file (your own laptop/machine)
                    ↓
2. UPLOAD to Azure OpenAI (via SDK/API call)
                    ↓
3. TRAINING happens on MICROSOFT'S OWN INFRASTRUCTURE
   — you never see or manage this compute at all
   — you don't configure LoRA yourself here — Microsoft handles
     the efficient-training internals, fully abstracted away
                    ↓
4. Fine-tuned model artifact is produced, stored by Microsoft
                    ↓
5. DEPLOY — you create a NEW Azure OpenAI deployment
   (e.g. "jmf-invoice-v1-deployment") pointing at this model
                    ↓
6. Your app calls that deployment name, same as any other
```
**Technology used:** just the Azure OpenAI SDK + a JSONL file. No Azure ML, no HuggingFace, no LoRA
code — it's fully managed.

### Path B — DIY fine-tuning with HuggingFace + PEFT (LoRA/QLoRA) — open-weight models

```
1. YOU: write Python training script (uses HuggingFace
   transformers + peft libraries)
                    ↓
2. WHERE IT RUNS: Azure ML (a GPU compute cluster you provision
   in Azure) — the "different ML machine." You spin up a GPU
   compute instance in Azure ML, and your Python script runs there.
                    ↓
3. Inside that Azure ML compute, THIS is where LoRA/QLoRA
   actually take effect:
     - Load base model (e.g. Phi-3 mini) from HuggingFace Hub
     - Freeze all its weights
     - (QLoRA only) quantize the frozen weights to 4-bit first
     - Attach LoRA adapters (small extra matrices)
     - Train ONLY the adapters using HuggingFace's Trainer
                    ↓
4. VALIDATE — check the fine-tuned model's accuracy on held-out
   data (still running in the same Azure ML environment)
                    ↓
5. SAVE the adapter (or merge adapter + base into one model)
   — still inside Azure ML, saved to storage (e.g. Azure Blob)
                    ↓
6. DEPLOY — a SEPARATE step: publish this model to an
   Azure ML Endpoint (or another hosting service) so it's
   callable over the network
                    ↓
7. Your app calls that endpoint, same idea as calling a deployment
```
**Technology used:** Python, HuggingFace `transformers` + `peft` libraries, running on **Azure ML
compute** (GPU cluster), deployed via an **Azure ML Endpoint**.

### Side by side

| | Path A (Azure OpenAI managed) | Path B (DIY with PEFT) |
|---|---|---|
| **Where trained** | Microsoft's own infrastructure — invisible to you | **Azure ML** — a GPU compute you provision yourself |
| **Where LoRA/QLoRA happen** | Nowhere visible — abstracted away by Microsoft | Inside your Azure ML compute, in your Python training script |
| **What model types** | Azure OpenAI's own models (gpt-4o-mini, etc.) | Open-weight models (Phi-3, Llama, Mistral) |
| **Where deployed** | Azure OpenAI deployment (the "instance" concept) | Azure ML Endpoint (a different, separate hosting service) |

**One-line summary:** if you're fine-tuning an **Azure OpenAI model**, Microsoft trains it for you —
you never touch LoRA/PEFT directly. If you're fine-tuning an **open-weight model yourself**, that's
when **Azure ML becomes the training machine**, and that's exactly where LoRA/QLoRA/PEFT code
actually runs — before eventually landing on a separate deployment/endpoint.

*Updated: 2026-06-30*
