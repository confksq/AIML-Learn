# Module 21 — Python for AI
**Part 4: Enterprise AI Solutions | AI Solutions Architect Curriculum**
*Created: 2026-06-30*

---

## Why This Module Exists

Every AI job posting mentions Python. You don't need to build web apps or data pipelines in Python. You need to:
- Read Python code written by data scientists and AI engineers
- Run Jupyter notebooks
- Call Azure OpenAI and Azure AI Services from Python
- Understand LangChain code you'll encounter in codebases

**Your C# knowledge is the cheat code.** Python concepts map almost 1:1 to C#. This module always shows the C# equivalent so you learn Python in terms of what you already know.

---

## Topic 21.1 — Python Basics (C# Developer Fast-Track)

---

### 1. Setup

```bash
# Install Python (if not already installed)
# Download from python.org — get 3.11 or 3.12

# Check version
python --version

# Create a virtual environment (like a project-scoped NuGet folder)
python -m venv .venv

# Activate it
source .venv/bin/activate       # Linux/Mac/WSL
.venv\Scripts\activate          # Windows CMD
.venv\Scripts\Activate.ps1      # Windows PowerShell

# Install packages (like NuGet)
pip install openai azure-ai-openai python-dotenv

# Save dependencies (like packages.json / .csproj)
pip freeze > requirements.txt

# Restore from requirements (like dotnet restore)
pip install -r requirements.txt
```

---

### 2. Variables and Types

```python
# Python — no type declarations needed (but you CAN add type hints)
name = "JM Family"          # string
count = 42                  # int
price = 42500.0             # float
is_active = True            # bool (capital T/F)
nothing = None              # null equivalent

# With type hints (recommended in AI code — clearer to read)
name: str = "JM Family"
count: int = 42
price: float = 42500.0
is_active: bool = True
```

```csharp
// C# equivalent
string name = "JM Family";
int count = 42;
double price = 42500.0;
bool isActive = true;
object nothing = null;
```

---

### 3. Strings

```python
# f-strings (like C# $"" interpolation)
dealer = "ATL-001"
amount = 42500
message = f"Dealer {dealer} owes ${amount}"

# Multi-line strings (like C# verbatim @"" or raw strings)
system_prompt = """
You are a JMA dealer support agent.
Always be professional and cite your sources.
Never make up inventory or pricing.
"""

# Common string methods
text = "  Hello World  "
text.strip()           # Trim()
text.lower()           # ToLower()
text.upper()           # ToUpper()
text.replace("o", "0") # Replace()
text.split(" ")        # Split()
"Hello" in text        # Contains()
len(text)              # text.Length
```

---

### 4. Lists and Dictionaries

```python
# List (like C# List<T>)
models = ["GPT-4o", "GPT-4o-mini", "o1", "Phi-4"]
models.append("Claude")      # Add()
models.remove("o1")          # Remove()
models[0]                    # [0] indexing
len(models)                  # Count
models[-1]                   # last item (no C# equivalent without LINQ)

# List comprehension (C# LINQ Select)
prices = [42500, 44800, 38900]
discounted = [p * 0.95 for p in prices]
# C# equivalent: prices.Select(p => p * 0.95).ToList()

filtered = [p for p in prices if p > 40000]
# C# equivalent: prices.Where(p => p > 40000).ToList()
```

```python
# Dictionary (like C# Dictionary<string, object>)
vehicle = {
    "make": "Toyota",
    "model": "RAV4 Hybrid",
    "year": 2026,
    "price": 42500,
    "in_stock": True
}

vehicle["make"]              # vehicle["make"]
vehicle.get("color", "N/A") # TryGetValue equivalent — returns default if missing
vehicle.keys()               # .Keys
vehicle.values()             # .Values
vehicle.items()              # .Select(kv => kv) — key-value pairs

# Add / update
vehicle["color"] = "Black"

# Check existence
"color" in vehicle           # vehicle.ContainsKey("color")
```

---

### 5. Functions

```python
# Basic function
def greet_dealer(name: str, region: str = "Southeast") -> str:
    return f"Welcome {name} from {region} region"

# Call it
message = greet_dealer("ATL-001")
message = greet_dealer("ATL-001", region="Northeast")

# *args = params[], **kwargs = Dictionary<string, object>
def log_event(event_type: str, **details):
    print(f"Event: {event_type}")
    for key, value in details.items():
        print(f"  {key}: {value}")

log_event("order_created", dealer="ATL-001", amount=42500, model="RAV4")
```

```csharp
// C# equivalent
string GreetDealer(string name, string region = "Southeast")
    => $"Welcome {name} from {region} region";
```

---

### 6. Classes

```python
# Python class
class DealerAgent:
    def __init__(self, dealer_id: str, model: str = "gpt-4o"):
        self.dealer_id = dealer_id    # self = this in C#
        self.model = model
        self._history = []            # _ prefix = private by convention

    def ask(self, question: str) -> str:
        self._history.append({"role": "user", "content": question})
        # ... call OpenAI ...
        return "answer"

    @property
    def message_count(self) -> int:   # property (like C# get-only property)
        return len(self._history)

# Instantiate
agent = DealerAgent("ATL-001", model="gpt-4o-mini")
response = agent.ask("What RAV4s do you have in stock?")
print(agent.message_count)
```

```csharp
// C# equivalent
public class DealerAgent
{
    private readonly string _dealerId;
    private readonly string _model;
    private readonly List<object> _history = new();

    public DealerAgent(string dealerId, string model = "gpt-4o")
    {
        _dealerId = dealerId;
        _model = model;
    }
    public int MessageCount => _history.Count;
}
```

---

### 7. Async / Await

Python's async looks almost identical to C#:

```python
import asyncio

# async function definition
async def call_openai_async(prompt: str) -> str:
    # await works just like C#
    response = await client.chat.completions.create(
        model="gpt-4o",
        messages=[{"role": "user", "content": prompt}]
    )
    return response.choices[0].message.content

# Run async code
async def main():
    result = await call_openai_async("What is RAG?")
    print(result)

asyncio.run(main())  # C# equivalent: main().GetAwaiter().GetResult()
```

---

### 8. Error Handling

```python
try:
    response = await client.chat.completions.create(...)
except openai.RateLimitError as e:          # specific exception (like catch (RateLimitException))
    print(f"Rate limited: {e}")
except openai.APIConnectionError as e:
    print(f"Connection failed: {e}")
except Exception as e:                       # catch-all
    print(f"Unexpected error: {e}")
    raise                                    # re-throw
finally:
    print("Done")                            # always runs
```

---

### 9. Working with JSON

AI APIs constantly return JSON. In Python it's trivial:

```python
import json

# Parse JSON string → Python dict (like JsonSerializer.Deserialize<T>)
json_string = '{"dealer": "ATL-001", "amount": 42500}'
data = json.loads(json_string)
print(data["dealer"])   # ATL-001

# Python dict → JSON string (like JsonSerializer.Serialize)
vehicle = {"make": "Toyota", "model": "RAV4", "price": 42500}
json_output = json.dumps(vehicle, indent=2)

# Read JSON file
with open("config.json", "r") as f:
    config = json.load(f)

# Write JSON file
with open("output.json", "w") as f:
    json.dump(vehicle, f, indent=2)
```

---

### 10. Environment Variables (.env files)

```python
# .env file (never commit this — same as appsettings.json secrets)
# AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/
# AZURE_OPENAI_KEY=your-key-here
# AZURE_OPENAI_DEPLOYMENT=gpt-4o

# Load .env file (pip install python-dotenv)
from dotenv import load_dotenv
import os

load_dotenv()  # reads .env file into environment

endpoint = os.getenv("AZURE_OPENAI_ENDPOINT")
api_key  = os.getenv("AZURE_OPENAI_KEY")
model    = os.getenv("AZURE_OPENAI_DEPLOYMENT", "gpt-4o")  # default value
```

---

## Topic 21.2 — Azure OpenAI in Python

---

### 1. Setup

```bash
pip install openai azure-identity
```

### 2. Client Initialization

```python
# Option A: API Key (dev/testing only)
from openai import AzureOpenAI

client = AzureOpenAI(
    azure_endpoint=os.getenv("AZURE_OPENAI_ENDPOINT"),
    api_key=os.getenv("AZURE_OPENAI_KEY"),
    api_version="2024-12-01-preview"
)

# Option B: Managed Identity (production — no keys)
from azure.identity import DefaultAzureCredential, get_bearer_token_provider
from openai import AzureOpenAI

credential = DefaultAzureCredential()
token_provider = get_bearer_token_provider(
    credential, "https://cognitiveservices.azure.com/.default")

client = AzureOpenAI(
    azure_endpoint=os.getenv("AZURE_OPENAI_ENDPOINT"),
    azure_ad_token_provider=token_provider,
    api_version="2024-12-01-preview"
)
```

### 3. Chat Completion

```python
# Basic chat
response = client.chat.completions.create(
    model="gpt-4o",       # your deployment name
    messages=[
        {"role": "system",    "content": "You are a JMA dealer support agent."},
        {"role": "user",      "content": "What RAV4 Hybrid models are available?"}
    ],
    temperature=0.7,
    max_tokens=500
)

answer = response.choices[0].message.content
print(answer)

# Usage tracking
print(f"Tokens used: {response.usage.total_tokens}")
print(f"Cost estimate: ${response.usage.total_tokens * 0.0000025:.6f}")
```

### 4. Streaming

```python
# Stream tokens as they arrive (like IAsyncEnumerable in C#)
stream = client.chat.completions.create(
    model="gpt-4o",
    messages=[{"role": "user", "content": "Explain RAG in 3 sentences"}],
    stream=True      # ← enable streaming
)

for chunk in stream:
    if chunk.choices[0].delta.content:
        print(chunk.choices[0].delta.content, end="", flush=True)
print()  # newline at end
```

### 5. Embeddings

```python
# Generate embedding for a text
response = client.embeddings.create(
    model="text-embedding-3-large",   # your embedding deployment name
    input="RAV4 Hybrid XLE specifications and fuel economy"
)

vector = response.data[0].embedding   # list of 3072 floats
print(f"Dimensions: {len(vector)}")   # 3072

# Batch embeddings (multiple texts at once)
texts = ["RAV4 Hybrid", "Camry sedan", "Tacoma pickup"]
response = client.embeddings.create(model="text-embedding-3-large", input=texts)
vectors = [item.embedding for item in response.data]
```

### 6. Function Calling / Tool Use

```python
import json

# Define tools
tools = [
    {
        "type": "function",
        "function": {
            "name": "get_inventory",
            "description": "Search vehicle inventory by model",
            "parameters": {
                "type": "object",
                "properties": {
                    "model": {"type": "string", "description": "Vehicle model name"},
                    "max_price": {"type": "number", "description": "Maximum price"}
                },
                "required": ["model"]
            }
        }
    }
]

messages = [
    {"role": "user", "content": "Find RAV4 Hybrid under $45,000"}
]

response = client.chat.completions.create(
    model="gpt-4o",
    messages=messages,
    tools=tools,
    tool_choice="auto"
)

# Check if model wants to call a tool
if response.choices[0].finish_reason == "tool_calls":
    tool_call = response.choices[0].message.tool_calls[0]
    function_name = tool_call.function.name
    arguments = json.loads(tool_call.function.arguments)

    print(f"Model wants to call: {function_name}")
    print(f"With args: {arguments}")

    # Call your actual function
    result = get_inventory(arguments["model"], arguments.get("max_price"))

    # Send result back
    messages.append(response.choices[0].message)
    messages.append({
        "role": "tool",
        "tool_call_id": tool_call.id,
        "content": json.dumps(result)
    })

    final = client.chat.completions.create(model="gpt-4o", messages=messages)
    print(final.choices[0].message.content)
```

### 7. Structured Output

```python
from pydantic import BaseModel
from typing import List

# Define your output schema with Pydantic (like C# record/class)
class VehicleMatch(BaseModel):
    make: str
    model: str
    trim: str
    price: float
    in_stock: bool

class InventoryResponse(BaseModel):
    matches: List[VehicleMatch]
    total_count: int
    recommendation: str

# Parse response into typed object (guaranteed valid JSON)
response = client.beta.chat.completions.parse(
    model="gpt-4o",
    messages=[
        {"role": "system", "content": "Extract vehicle information from the text."},
        {"role": "user", "content": "We have RAV4 Hybrid XLE at $42,500 in stock..."}
    ],
    response_format=InventoryResponse   # ← Pydantic model as schema
)

result: InventoryResponse = response.choices[0].message.parsed
print(result.matches[0].price)   # 42500.0 — typed, not string
```

---

## Topic 21.3 — Jupyter Notebooks

---

### What They Are

Jupyter notebooks are interactive Python documents — mix of code cells and markdown cells. Data scientists use them constantly. You'll need to read and run them.

```
NOTEBOOK = series of CELLS
  Code cell:     Python code that runs
  Markdown cell: Documentation / explanation
  Output cell:   Result shown below code (text, charts, tables)

File extension: .ipynb
```

### Running Notebooks

```bash
# Install Jupyter
pip install jupyter

# Start notebook server
jupyter notebook     # opens browser at localhost:8888

# OR use VS Code (recommended for you — familiar IDE)
# Install: Python extension + Jupyter extension
# Open any .ipynb file → runs inline in VS Code
```

### Notebook Shortcuts

```
Shift+Enter    Run cell and move to next
Ctrl+Enter     Run cell and stay
A              Insert cell Above
B              Insert cell Below
D D            Delete cell
M              Change to Markdown cell
Y              Change to Code cell
```

### Reading a Data Science Notebook

When a data scientist shares a notebook, this is what each section typically does:

```python
# Cell 1: Imports (like using statements)
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split

# Cell 2: Load data
df = pd.read_csv("dealer_data.csv")
df.head()                # show first 5 rows

# Cell 3: Explore data
df.shape                 # (rows, columns)
df.describe()            # statistics: mean, std, min, max
df.isnull().sum()        # count missing values

# Cell 4: Train a model
X = df[["feature1", "feature2"]]
y = df["target"]
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2)

from sklearn.ensemble import RandomForestClassifier
model = RandomForestClassifier()
model.fit(X_train, y_train)
print(f"Accuracy: {model.score(X_test, y_test):.2%}")
```

You don't need to write this — you need to READ it and understand what's happening at each step.

---

## Topic 21.4 — LangChain (Awareness Level)

---

### What LangChain Is

LangChain is the Python equivalent of Semantic Kernel — AI orchestration for Python apps. You'll encounter it in job postings and Python codebases.

```bash
pip install langchain langchain-openai langchain-community
```

### Core Concepts Mapped to Semantic Kernel

| LangChain | Semantic Kernel (C#) | What it does |
|---|---|---|
| `ChatOpenAI` | `AzureOpenAIChatCompletion` | LLM connection |
| `ChatPromptTemplate` | Prompt function | Parameterized prompt |
| `Chain` (LCEL) | Pipeline of functions | Sequential steps |
| `Tool` | `[KernelFunction]` | What LLM can call |
| `AgentExecutor` | `AutoInvokeKernelFunctions` | ReAct agent loop |
| `ConversationBufferMemory` | `ChatHistory` | Short-term memory |
| `VectorStore` | `AzureAISearchMemory` | Long-term memory |

### Simple Chat

```python
from langchain_openai import AzureChatOpenAI
from langchain_core.messages import HumanMessage, SystemMessage

# Initialize (reads AZURE_OPENAI_* from environment)
llm = AzureChatOpenAI(
    azure_deployment="gpt-4o",
    api_version="2024-12-01-preview"
)

# Simple call
messages = [
    SystemMessage(content="You are a JMA dealer support agent."),
    HumanMessage(content="What is RAG?")
]
response = llm.invoke(messages)
print(response.content)
```

### RAG Pipeline in LangChain

```python
from langchain_openai import AzureOpenAIEmbeddings
from langchain_community.vectorstores import AzureSearch
from langchain.chains import RetrievalQA

# 1. Connect to Azure AI Search (your existing index)
embeddings = AzureOpenAIEmbeddings(azure_deployment="text-embedding-3-large")

vector_store = AzureSearch(
    azure_search_endpoint=os.getenv("AZURE_SEARCH_ENDPOINT"),
    azure_search_key=os.getenv("AZURE_SEARCH_KEY"),
    index_name="toyota-inventory-index",
    embedding_function=embeddings.embed_query
)

# 2. Create retriever
retriever = vector_store.as_retriever(search_kwargs={"k": 5})

# 3. Build RAG chain
qa_chain = RetrievalQA.from_chain_type(
    llm=llm,
    retriever=retriever,
    return_source_documents=True
)

# 4. Ask a question
result = qa_chain.invoke({"query": "What RAV4 Hybrid models are under $45,000?"})
print(result["result"])
print(f"Sources: {[doc.page_content[:100] for doc in result['source_documents']]}")
```

### LangChain Agent

```python
from langchain.agents import create_react_agent, AgentExecutor
from langchain.tools import Tool

# Define tools (like SK KernelFunctions)
def search_inventory(query: str) -> str:
    """Search vehicle inventory. Input: vehicle model name."""
    # call your inventory API
    return f"Found 3 RAV4 Hybrid XLE vehicles matching '{query}'"

def get_pricing(model: str) -> str:
    """Get current pricing for a vehicle model."""
    return f"{model} MSRP: $42,500"

tools = [
    Tool(name="SearchInventory", func=search_inventory,
         description="Search vehicle inventory. Input: vehicle model name."),
    Tool(name="GetPricing", func=get_pricing,
         description="Get current pricing for a vehicle model.")
]

# Create agent (ReAct pattern — same as SK AutoInvoke)
from langchain import hub
prompt = hub.pull("hwchase17/react")

agent = create_react_agent(llm, tools, prompt)
agent_executor = AgentExecutor(agent=agent, tools=tools, verbose=True)

# Run agent
result = agent_executor.invoke({
    "input": "Find RAV4 Hybrid options and tell me the price"
})
print(result["output"])
```

---

## Topic 21.5 — Azure AI Services in Python

---

### Azure AI Language (NLP)

```python
from azure.ai.textanalytics import TextAnalyticsClient
from azure.core.credentials import AzureKeyCredential

client = TextAnalyticsClient(
    endpoint=os.getenv("AZURE_LANGUAGE_ENDPOINT"),
    credential=AzureKeyCredential(os.getenv("AZURE_LANGUAGE_KEY"))
)

# Sentiment analysis
documents = ["This dealer was fantastic!", "Terrible service, very slow."]
result = client.analyze_sentiment(documents)
for doc in result:
    print(f"Sentiment: {doc.sentiment} | Confidence: {doc.confidence_scores}")

# PII detection
result = client.recognize_pii_entities(["My name is John Smith, SSN 123-45-6789"])
for doc in result:
    for entity in doc.entities:
        print(f"PII: {entity.text} → {entity.category}")
```

### Azure AI Search (Vector Search)

```python
from azure.search.documents import SearchClient
from azure.search.documents.models import VectorizedQuery

search_client = SearchClient(
    endpoint=os.getenv("AZURE_SEARCH_ENDPOINT"),
    index_name="toyota-inventory-index",
    credential=AzureKeyCredential(os.getenv("AZURE_SEARCH_KEY"))
)

# Generate query embedding
query = "affordable family SUV hybrid"
query_vector = client.embeddings.create(
    model="text-embedding-3-large", input=query
).data[0].embedding

# Hybrid search (vector + keyword)
results = search_client.search(
    search_text=query,                # keyword component
    vector_queries=[VectorizedQuery(  # vector component
        vector=query_vector,
        k_nearest_neighbors=5,
        fields="contentVector"
    )],
    query_type="semantic",
    semantic_configuration_name="default",
    top=5
)

for result in results:
    print(f"Score: {result['@search.score']:.3f} | {result['content'][:100]}")
```

### Azure Document Intelligence

```python
from azure.ai.documentintelligence import DocumentIntelligenceClient
from azure.core.credentials import AzureKeyCredential

di_client = DocumentIntelligenceClient(
    endpoint=os.getenv("AZURE_DI_ENDPOINT"),
    credential=AzureKeyCredential(os.getenv("AZURE_DI_KEY"))
)

# Analyze a document (prebuilt invoice model)
with open("invoice.pdf", "rb") as f:
    poller = di_client.begin_analyze_document(
        model_id="prebuilt-invoice",
        body=f
    )

result = poller.result()
for doc in result.documents:
    print(f"Vendor: {doc.fields.get('VendorName').content}")
    print(f"Total: {doc.fields.get('InvoiceTotal').content}")
    print(f"Date: {doc.fields.get('InvoiceDate').content}")
```

---

## Topic R21 — Quick Recall: Python vs C# Cheat Sheet

```
CONCEPT            PYTHON                    C#
─────────────────────────────────────────────────────────────────
No type needed     x = 5                     var x = 5;
String format      f"Hello {name}"           $"Hello {name}"
None / null        None                      null
Print              print("hello")            Console.WriteLine("hello");
List               [1, 2, 3]                 new List<int> { 1, 2, 3 }
Dictionary         {"key": "value"}          new Dictionary<string,string>
For loop           for item in items:        foreach (var item in items)
If/else            if x > 0:                 if (x > 0)
Function           def my_func(x):           ReturnType MyFunc(Type x)
Class              class MyClass:            public class MyClass
Constructor        def __init__(self):       public MyClass()
Self               self.property             this.property
Inherit            class Child(Parent):      class Child : Parent
Async function     async def my_func():      async Task MyFunc()
Await              await something()         await something();
Lambda             lambda x: x * 2          x => x * 2
LINQ Select        [x*2 for x in items]      items.Select(x => x*2)
LINQ Where         [x for x in items if x>0] items.Where(x => x>0)
Try/catch          try: ... except Ex as e:  try { } catch (Ex e) { }
Import             import os                 using System;
Null check         if x is None:             if (x == null)
String join        ", ".join(items)          string.Join(", ", items)
Type check         isinstance(x, str)        x is string
```

---

## Interactive Learning Ideas

### Exercise 1 — Hello Azure OpenAI in Python (20 min)
```bash
mkdir python-ai-lab && cd python-ai-lab
python -m venv .venv && source .venv/bin/activate
pip install openai python-dotenv
```
Create `chat.py` that:
1. Reads Azure OpenAI credentials from a `.env` file
2. Sends "What is RAG?" to GPT-4o
3. Prints the response and token count

### Exercise 2 — Embeddings + Cosine Similarity (20 min)
Write Python code that:
1. Embeds 5 vehicle descriptions using text-embedding-3-large
2. Embeds a user query: "family SUV with good fuel economy"
3. Computes cosine similarity between query and each vehicle
4. Returns the top 2 matches

```python
import numpy as np
def cosine_similarity(a, b):
    return np.dot(a, b) / (np.linalg.norm(a) * np.linalg.norm(b))
```

### Exercise 3 — LangChain RAG (30 min)
Build a LangChain RAG pipeline that:
1. Loads a text file (dealer FAQ)
2. Splits into chunks and embeds using Azure OpenAI
3. Stores in an in-memory vector store
4. Answers questions from the FAQ content
Compare to how you'd do this with Semantic Kernel in C#.

### Exercise 4 — Read a Data Science Notebook (20 min)
Download a Hugging Face or Kaggle notebook on sentiment analysis.
Open in VS Code with Jupyter extension. Run it cell by cell.
Goal: understand WHAT each cell does (not HOW to write it).
Questions to answer after:
- What data did it use?
- What model did it train?
- What was the accuracy?
- Which library handled the ML (sklearn? transformers?)

### Exercise 5 — Azure AI Search in Python (20 min)
Rewrite the C# hybrid search code from L09 in Python.
Use `azure-search-documents` SDK.
Compare: what's easier in Python vs C#? What's harder?

---

## Memory Hooks

- **"Python variables need no type — just assign"**
- **"f-string = C# $"" — same idea, different syntax"**
- **"List comprehension = LINQ Select/Where in one line"**
- **"self = this, __init__ = constructor, None = null"**
- **"async/await works exactly like C# — same mental model"**
- **"pip = NuGet, requirements.txt = .csproj packages, .venv = bin/obj folder"**
- **"LangChain = Python's Semantic Kernel — same concepts, different syntax"**
- **"Jupyter notebook = interactive Python script with output shown inline"**
- **"You don't need to WRITE Python like a data scientist — you need to READ it"**

---

*Previous: Module 20 — Integration Patterns*
*This is the final module — you now have the complete AI Solutions Architect curriculum*
*Part 4: Enterprise AI Solutions*
*Created: 2026-06-30*
