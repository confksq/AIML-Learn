# Q&A — L21: Python for AI
**Source chapter:** `01_Lessons/Part4_Architecture/L21_Python_for_AI.md` | **Format:** self-study
**Questions:** 28 | *No overlap with the interview bank or other files — these drill the chapter's Python-for-a-C#-developer content.*

---

## Python Basics (C# Fast-Track)

**Q1. What does the module say you actually need Python for — and NOT for?**
You need to **read** Python from data scientists/AI engineers, run Jupyter notebooks, call Azure OpenAI/AI Services from Python, and understand LangChain code. You do **not** need to build web apps or data pipelines in Python. The framing: your C# knowledge is the cheat code — Python maps almost 1:1.

**Q2. Map the Python tooling to its C# equivalent: venv, pip, requirements.txt, pip install -r.**
`python -m venv .venv` = a project-scoped package folder (like bin/obj + NuGet scope); `pip install` = NuGet install; `pip freeze > requirements.txt` = packages.json/.csproj dependency list; `pip install -r requirements.txt` = `dotnet restore`.

**Q3. How do Python variables and null differ from C#?**
Python needs **no type declaration** (`name = "JM Family"`) though optional type hints are recommended in AI code (`name: str = "..."`). Booleans are capitalized (`True`/`False`), and null is **`None`**.

**Q4. What's the Python equivalent of C# string interpolation and verbatim strings?**
f-strings: `f"Dealer {dealer} owes ${amount}"` = C# `$"..."`. Triple-quoted `"""..."""` multi-line strings = C# verbatim `@"..."` / raw strings (used heavily for system prompts).

**Q5. Give the Python-to-C# mapping for these string methods: strip, lower, `in`, len.**
`.strip()` = `Trim()`, `.lower()` = `ToLower()`, `"x" in text` = `Contains()`, `len(text)` = `text.Length`. Also `.replace()` = `Replace()`, `.split()` = `Split()`.

**Q6. What is a list comprehension, and what LINQ does it replace?**
A one-line transform/filter over a list. `[p * 0.95 for p in prices]` = `prices.Select(p => p * 0.95).ToList()`. `[p for p in prices if p > 40000]` = `prices.Where(p => p > 40000).ToList()`.

**Q7. How do you safely read a dict value with a default, and check key existence?**
`vehicle.get("color", "N/A")` returns the default if the key is missing (like `TryGetValue`). `"color" in vehicle` = `ContainsKey("color")`. `.keys()`/`.values()`/`.items()` give keys/values/key-value pairs.

**Q8. In a Python function, how do default parameters and *args/**kwargs map to C#?**
`def greet(name, region="Southeast")` — default parameter like C#. `*args` = `params[]` (variable positional args); `**kwargs` = a `Dictionary<string, object>` of named args (`log_event("x", dealer="ATL-001", amount=42500)`).

**Q9. In a Python class, what do `self`, `__init__`, and the `_` prefix mean?**
`self` = C# `this`; `__init__` = the constructor; a leading `_` (e.g. `self._history`) marks "private by convention" (not enforced). `@property` makes a method act like a get-only property (`agent.message_count`).

**Q10. How does Python inheritance and async/await compare to C#?**
Inheritance: `class Child(Parent):` = `class Child : Parent`. Async is nearly identical: `async def f():` / `await something()` — run with `asyncio.run(main())` (≈ `main().GetAwaiter().GetResult()`).

**Q11. How is exception handling structured in Python?**
`try:` / `except SpecificError as e:` (like `catch (SpecificException e)`) / a bare `except Exception as e:` catch-all / `raise` to re-throw / `finally:` always runs. You can catch specific exceptions like `openai.RateLimitError` before the catch-all.

**Q12. How do you parse and serialize JSON in Python?**
`json.loads(str)` → dict (like `Deserialize`), `json.dumps(dict, indent=2)` → string (like `Serialize`). File I/O: `json.load(f)` / `json.dump(obj, f)` inside a `with open(...) as f:` block.

**Q13. How do you load secrets from a .env file, and what's it analogous to?**
`from dotenv import load_dotenv; load_dotenv()` reads `.env` into environment variables, then `os.getenv("AZURE_OPENAI_ENDPOINT")` (with an optional default). Analogous to appsettings.json secrets — never commit the `.env`.

---

## Azure OpenAI in Python

**Q14. Show the two client-init options and which is production.**
**API key (dev only):** `AzureOpenAI(azure_endpoint=..., api_key=..., api_version=...)`. **Managed Identity (production):** `DefaultAzureCredential()` + `get_bearer_token_provider(credential, "https://cognitiveservices.azure.com/.default")` passed as `azure_ad_token_provider` — no keys.

**Q15. Write the basic chat completion call and how you read the answer and token count.**
`client.chat.completions.create(model="gpt-4o", messages=[{system},{user}], temperature=0.7, max_tokens=500)`. Answer: `response.choices[0].message.content`. Tokens: `response.usage.total_tokens`.

**Q16. How do you enable streaming and consume it?**
Pass `stream=True`, then `for chunk in stream:` and print `chunk.choices[0].delta.content` (guarding for None) with `end="", flush=True`. It's the Python analog of C#'s IAsyncEnumerable streaming.

**Q17. How do you generate a single embedding vs a batch, and what dimension does 3-large return?**
Single: `client.embeddings.create(model="text-embedding-3-large", input="...")` → `response.data[0].embedding` (a list of **3072** floats). Batch: pass a list to `input`, then `[item.embedding for item in response.data]`.

**Q18. Walk the function-calling flow in the Python SDK.**
Define `tools` (JSON schema), call with `tools=tools, tool_choice="auto"`. If `finish_reason == "tool_calls"`, read `message.tool_calls[0]`, `json.loads(tool_call.function.arguments)`, execute your real function, then append the assistant message + a `{"role": "tool", "tool_call_id": ..., "content": json.dumps(result)}` message and call again for the final answer.

**Q19. How does Python get guaranteed structured output, and what library defines the schema?**
**Pydantic** models (`class VehicleMatch(BaseModel): ...`) as the schema, passed to `client.beta.chat.completions.parse(..., response_format=InventoryResponse)`. The result is `response.choices[0].message.parsed` — a **typed object** (`.matches[0].price` is a float, not a string). Pydantic BaseModel ≈ a C# record/class.

---

## Jupyter & LangChain

**Q20. What is a Jupyter notebook, its file extension, and its three cell types?**
An interactive Python document mixing code and prose. Extension **`.ipynb`**. Cell types: **code** (runs Python), **markdown** (docs), **output** (results shown inline — text, charts, tables). Run in `jupyter notebook` (browser) or VS Code with the Python + Jupyter extensions.

**Q21. Name four essential Jupyter shortcuts.**
`Shift+Enter` (run cell, move to next), `Ctrl+Enter` (run, stay), `A`/`B` (insert cell above/below), `D D` (delete cell), `M`/`Y` (to markdown / to code).

**Q22. When reading a data-science notebook, what do these typically do: df.head(), df.describe(), df.isnull().sum(), train_test_split?**
`df.head()` — first 5 rows. `df.describe()` — statistics (mean/std/min/max). `df.isnull().sum()` — count missing values per column. `train_test_split(X, y, test_size=0.2)` — split into train/test sets. The module's point: **read and understand** these, you don't need to write them.

**Q23. What is LangChain, and map four of its concepts to Semantic Kernel.**
Python's equivalent of Semantic Kernel (AI orchestration for Python apps). `ChatOpenAI` → `AzureOpenAIChatCompletion`; `Chain`(LCEL) → pipeline of functions; `Tool` → `[KernelFunction]`; `AgentExecutor` → `AutoInvokeKernelFunctions`; `ConversationBufferMemory` → `ChatHistory`; `VectorStore` → `AzureAISearchMemory`.

**Q24. Walk the four steps of a LangChain RAG pipeline over Azure AI Search.**
(1) Create `AzureOpenAIEmbeddings`; (2) connect an `AzureSearch` vector store to your existing index; (3) `vector_store.as_retriever(search_kwargs={"k": 5})`; (4) `RetrievalQA.from_chain_type(llm, retriever, return_source_documents=True)` then `qa_chain.invoke({"query": "..."})` — returns `result["result"]` plus `source_documents`.

**Q25. How does a LangChain agent map to the SK ReAct pattern?**
Define Python functions as `Tool`s (name + func + description — the description is what the LLM reads, like a `[KernelFunction]` `[Description]`), pull a ReAct prompt (`hub.pull("hwchase17/react")`), create with `create_react_agent(llm, tools, prompt)`, wrap in `AgentExecutor`, and `invoke({"input": ...})`. Same ReAct loop as SK's `AutoInvokeKernelFunctions`.

---

## Azure AI Services in Python & Cheat Sheet

**Q26. Show the Python calls for sentiment and PII detection via Azure AI Language.**
`TextAnalyticsClient(endpoint, AzureKeyCredential(key))`, then `client.analyze_sentiment(documents)` → per-doc `.sentiment` + `.confidence_scores`; and `client.recognize_pii_entities([...])` → per-doc `.entities` with `.text` and `.category`.

**Q27. How does a Python hybrid search on Azure AI Search combine keyword and vector?**
`search_client.search(search_text=query, vector_queries=[VectorizedQuery(vector=query_vector, k_nearest_neighbors=5, fields="contentVector")], query_type="semantic", semantic_configuration_name="default", top=5)` — `search_text` is the keyword component, `vector_queries` the vector component, fused by the service; read `result['@search.score']`.

**Q28. From the cheat sheet, give the Python↔C# mapping for: print, lambda, LINQ Select, null check, string join.**
`print("hi")` = `Console.WriteLine`. `lambda x: x*2` = `x => x*2`. `[x*2 for x in items]` = `items.Select(x => x*2)`. `if x is None:` = `if (x == null)`. `", ".join(items)` = `string.Join(", ", items)`. Core takeaway: *"You don't need to WRITE Python like a data scientist — you need to READ it,"* and async/await/classes/exceptions all map directly from C#.

---

*Curriculum Q&A Batch F — file 2 of 2 (L20, L21 complete). **Curriculum Q&A set COMPLETE: L06–L21, all chapters.***
