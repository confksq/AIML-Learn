# Module 32 — Advanced Python for AI Engineers

**Part 7: Platform Engineering & AI-Assisted Delivery**
*Created: 2026-07-26 · FDE-Prep · Clears tracker rows 15, 16*

> **Supersedes `L21_Python_for_AI.md` for writing-level Python.**
> `L21` stays as the C#→Python translation reference and the "read a data scientist's notebook"
> module. It says plainly: *"You don't need to build web apps or data pipelines in Python. You need
> to read Python code."* That was correct for AI-102 and an architect track. It is **not** enough
> for a JD that says *"Advanced Python programming skills, including OOP, Iterators & Generators,
> Decorators, Type Hints & Data Classes, Data Structures & Algorithms, Design Patterns."*
> This module closes that gap.

---

## Why This Module Exists

You will be screened on Python. Not on Azure architecture, not on RAG — on whether you can write
idiomatic Python under time pressure. The six things below are what interviewers actually probe,
and none of them appear anywhere else in this library.

| JD sub-skill | Covered before this module | Now |
|---|---|---|
| OOP · Functions · Exception handling | `L21` §5, §6, §8 — basic | deepened here |
| Type Hints | `L21` §2 — mentioned | §1 |
| **Data Classes** | ❌ nothing | **§1** |
| **Iterators & Generators** | ❌ nothing | **§2** |
| **Decorators** | ❌ nothing | **§3** |
| **Context managers** | ❌ nothing | **§4** |
| **Data Structures & Algorithms / Big-O** | ❌ nothing | **§6** |
| **Design Patterns** | ❌ nothing | **§7** |

**Your advantage:** every one of these has a direct C# analogue. You are not learning new ideas,
you are learning new spellings. Each section leads with the C# you already know.

**Prerequisite:** `L21` §1–§10. **Companion reading:** `06_Supplementary/PythonTrack/1.5-AIAgents.md`
(1,981 lines) — the best real Python in this library. Read it *as Python practice* after this module.

---

## Section 1 — Type Hints and Data Classes

### 1.1 Type hints are documentation the tooling can check

Python is dynamically typed. Type hints do **not** change runtime behaviour — Python ignores them.
They exist for your IDE, for `mypy`, and for humans.

```python
from typing import Optional

def calc_refund(price: float, months_used: int, fee: float = 50.0) -> float:
    """Pro-rata refund on a 60-month VSC contract."""
    unused = (60 - months_used) / 60
    return round(price * unused - fee, 2)
```

C# equivalent — same information, enforced by the compiler instead of a linter:

```csharp
public decimal CalcRefund(decimal price, int monthsUsed, decimal fee = 50m)
```

**Modern syntax (Python 3.10+)** — use this, not the old `typing` imports:

| Old | Modern | C# |
|---|---|---|
| `List[str]` | `list[str]` | `List<string>` |
| `Dict[str, int]` | `dict[str, int]` | `Dictionary<string,int>` |
| `Optional[str]` | `str \| None` | `string?` |
| `Union[int, str]` | `int \| str` | — |

### 1.2 Data classes replace boilerplate classes

**The problem.** A plain class needs `__init__`, `__repr__`, `__eq__` written by hand:

```python
class Contract:                                    # ❌ the long way
    def __init__(self, contract_id, vin, price, months_used):
        self.contract_id = contract_id
        self.vin = vin
        self.price = price
        self.months_used = months_used
    def __repr__(self):
        return f"Contract({self.contract_id}, {self.vin})"
    def __eq__(self, other):
        return (self.contract_id == other.contract_id and self.vin == other.vin)
```

**The fix.** `@dataclass` generates all of it:

```python
from dataclasses import dataclass, field

@dataclass
class Contract:
    contract_id: str
    vin: str
    price: float
    months_used: int
    fee: float = 50.0                      # default value
    tags: list[str] = field(default_factory=list)   # ⚠️ see the trap below

    @property
    def refund(self) -> float:             # computed, not stored
        return round(self.price * (60 - self.months_used) / 60 - self.fee, 2)
```

```python
c = Contract("VSC-88213", "1HGCM82633A004352", 2400.0, 22)
print(c)          # Contract(contract_id='VSC-88213', vin='1HGCM…', price=2400.0, …)
print(c.refund)   # 1520.0
c == Contract("VSC-88213", "1HGCM82633A004352", 2400.0, 22)   # True — value equality
```

**This is a C# `record`.** Same purpose: a data-carrying type with generated constructor, printing
and value equality.

```csharp
public record Contract(string ContractId, string Vin, decimal Price, int MonthsUsed);
```

### 1.3 Immutability

```python
@dataclass(frozen=True)          # like a C# readonly record struct
class AgentConfig:
    model: str
    temperature: float = 0.0

cfg = AgentConfig("gpt-4o")
cfg.temperature = 0.7            # ❌ FrozenInstanceError
```

Freeze anything that flows into an agent's config or a tool's arguments. Mutation bugs in agent
state are miserable to debug because the mutation happens three tool calls before the symptom.

### ⚠️ The mutable-default trap — a classic interview question

```python
@dataclass
class Bad:
    tags: list[str] = []            # ❌ ValueError at class definition time

def bad_fn(items: list = []):       # ❌ silently shares ONE list across ALL calls
    items.append(1)
    return items

bad_fn()   # [1]
bad_fn()   # [1, 1]   ← the same list, forever
```

**Why:** default arguments are evaluated **once**, when the function is defined — not per call.
**Fix:** `field(default_factory=list)` in a dataclass, or `items: list | None = None` then
`items = items or []` in a function.

If you remember one thing from this section, remember this one. It gets asked constantly.

### 1.4 Pydantic — where you will actually meet this in AI code

`dataclass` validates nothing. **Pydantic** does, and it is the backbone of FastAPI, LangChain
tool schemas, and structured LLM output.

```python
from pydantic import BaseModel, Field

class CancellationRequest(BaseModel):
    vin: str = Field(min_length=17, max_length=17)
    reason: str
    refund_override: float | None = None

# Parses AND validates — raises ValidationError on bad input
req = CancellationRequest.model_validate_json(llm_output)
```

This is how you stop an LLM's JSON from crashing your pipeline three layers down. **Every
production agent should validate tool arguments with Pydantic before executing them.**

---

## Section 2 — Iterators and Generators

### 2.1 The idea

You already know this in C#: `yield return` and `IEnumerable<T>`. Same thing, same reason.

```csharp
// C# — lazy, one at a time
IEnumerable<string> ReadLines(string path) {
    using var r = new StreamReader(path);
    string line;
    while ((line = r.ReadLine()) != null) yield return line;
}
```

```python
# Python — identical semantics
def read_lines(path: str):
    with open(path) as f:
        for line in f:
            yield line.rstrip()
```

**`yield` turns a function into a generator.** Calling it runs *no code* — it returns a generator
object. Code runs only as you iterate, one item at a time, and execution *pauses* at each `yield`.

### 2.2 Why it matters — memory

```python
def all_records(path):                 # ❌ loads 2 GB into RAM
    return [parse(line) for line in open(path)]

def stream_records(path):              # ✅ constant memory
    for line in open(path):
        yield parse(line)
```

Your CallMiner `.unl` feed is exactly this shape: a large delimited file you transform row by row.
A generator processes a 2 GB file in a few MB of RAM. A list comprehension does not.

### 2.3 Generator expressions

```python
squares = [x * x for x in range(1_000_000)]    # list — builds all 1M now
squares = (x * x for x in range(1_000_000))    # generator — builds none yet
                                                # ↑ parentheses, not brackets
```

Chain them and nothing materialises until the final consumer pulls:

```python
lines   = (l.rstrip() for l in open("nsc_recordings.unl"))
records = (parse(l) for l in lines)
spanish = (r for r in records if r.language == "es")
total   = sum(1 for _ in spanish)      # only NOW does anything execute
```

That is a full ETL pipeline in constant memory.

### 2.4 Streaming LLM output — the AI-specific use

Every token-streaming API you will write is a generator:

```python
def stream_answer(client, prompt: str):
    for chunk in client.chat.completions.create(
            model="gpt-4o", messages=[{"role": "user", "content": prompt}], stream=True):
        if chunk.choices[0].delta.content:
            yield chunk.choices[0].delta.content

for token in stream_answer(client, "Explain VSC cancellation"):
    print(token, end="", flush=True)
```

The JD's *"React streaming agent UI"* is this generator on the server side.

### 2.5 `yield from`, and the `itertools` you should know

```python
def all_dealer_notes(files):
    for f in files:
        yield from read_lines(f)        # delegate to another generator
```

| Function | Does |
|---|---|
| `itertools.islice(gen, 10)` | first 10 without consuming the rest |
| `itertools.chain(a, b)` | concatenate iterables lazily |
| `itertools.groupby(sorted_it, key)` | group runs — **must be pre-sorted** |
| `itertools.batched(it, n)` *(3.12+)* | fixed-size chunks — perfect for embedding batches |

### ⚠️ Generators are single-use

```python
g = (x for x in range(3))
list(g)   # [0, 1, 2]
list(g)   # []   ← exhausted, silently
```

No error. Just empty. This bites people in retry logic — the retry re-consumes an already-drained
generator and sees nothing. If you need to iterate twice, materialise to a list first.

---

## Section 3 — Decorators

### 3.1 What they are

A decorator is **a function that takes a function and returns a replacement**. You have used the
idea in C# as attributes plus middleware — cross-cutting behaviour wrapped around a method without
editing the method.

```python
def logged(fn):
    def wrapper(*args, **kwargs):
        print(f"→ {fn.__name__}")
        result = fn(*args, **kwargs)
        print(f"← {fn.__name__}")
        return result
    return wrapper

@logged                      # exactly equivalent to:  submit = logged(submit)
def submit(contract_id):
    ...
```

**`@logged` is pure syntax sugar for `submit = logged(submit)`.** Once that clicks, decorators stop
being magic.

### 3.2 Always use `functools.wraps`

Without it, the wrapper impersonates the original badly — `__name__`, `__doc__` and the signature
are lost, which breaks logging, docs, and **LLM tool-schema generation** (frameworks read the
docstring to describe the tool to the model).

```python
import functools

def logged(fn):
    @functools.wraps(fn)          # ✅ copies __name__, __doc__, signature
    def wrapper(*args, **kwargs):
        return fn(*args, **kwargs)
    return wrapper
```

### 3.3 A decorator with arguments — three levels deep

```python
import time, functools

def retry(times: int = 3, delay: float = 1.0, backoff: float = 2.0):
    def decorator(fn):
        @functools.wraps(fn)
        def wrapper(*args, **kwargs):
            wait = delay
            for attempt in range(1, times + 1):
                try:
                    return fn(*args, **kwargs)
                except Exception as e:
                    if attempt == times:
                        raise
                    print(f"attempt {attempt} failed: {e} — retrying in {wait}s")
                    time.sleep(wait)
                    wait *= backoff
        return wrapper
    return decorator

@retry(times=5, delay=0.5)
def call_contract_api(vin: str) -> dict:
    ...
```

**This is Polly in Python.** `L31` §2 teaches the retry + circuit-breaker pattern in C# with Polly;
this is the same pattern, hand-rolled. Understand the three nesting levels and you understand
decorators completely:

```
retry(times=5)        → returns decorator
  decorator(fn)       → returns wrapper
    wrapper(*args)    → the thing that actually runs
```

### 3.4 Decorators you will meet in AI code

| Decorator | From | Does |
|---|---|---|
| `@tool` | LangChain | Registers a function as an agent tool; **reads the docstring as the description** |
| `@kernel_function` | Semantic Kernel (Python) | Same idea — SK's `[KernelFunction]` attribute |
| `@app.get("/x")` | FastAPI | Route registration |
| `@functools.lru_cache` | stdlib | Memoisation — free caching |
| `@property` | stdlib | Computed attribute, C# get-only property |
| `@staticmethod` / `@classmethod` | stdlib | C# `static` / factory method |
| `@dataclass` | stdlib | §1.2 |

```python
@functools.lru_cache(maxsize=1024)
def embed(text: str) -> tuple[float, ...]:
    """Cache identical embedding calls — real money saved."""
    return tuple(client.embeddings.create(input=text, model="text-embedding-3-small")
                       .data[0].embedding)
```

> ⚠️ `lru_cache` requires **hashable** arguments and returns — hence the `tuple`, not `list`.
> It is also unbounded in time: no TTL. For an LLM cache with expiry, see `L36`.

---

## Section 4 — Context Managers

### 4.1 `with` is C# `using`

```csharp
using var conn = new SqlConnection(cs);   // C#: IDisposable
```

```python
with open("data.unl") as f:               # Python: __enter__ / __exit__
    process(f)
# file is closed here even if process() throws
```

### 4.2 Writing your own — the class form

```python
class Timed:
    def __init__(self, label: str):
        self.label = label
    def __enter__(self):
        self.t0 = time.perf_counter()
        return self                       # value bound by `as`
    def __exit__(self, exc_type, exc, tb):
        print(f"{self.label}: {time.perf_counter() - self.t0:.2f}s")
        return False                      # False = do not swallow exceptions

with Timed("embedding batch"):
    embed_all(chunks)
```

`__exit__` returning `True` **swallows the exception**. Almost always you want `False`.

### 4.3 The generator form — shorter, and what you will usually write

```python
from contextlib import contextmanager

@contextmanager
def timed(label: str):
    t0 = time.perf_counter()
    try:
        yield                              # everything before = __enter__
    finally:                               # everything after  = __exit__
        print(f"{label}: {time.perf_counter() - t0:.2f}s")
```

Note this combines §2 and §3 — a generator, wrapped by a decorator, to make a context manager.
That is idiomatic Python in one object.

**Real uses:** DB transactions, temporary Azure credentials, agent tracing spans (`L36`),
suppressing noise with `contextlib.suppress(FileNotFoundError)`.

---

## Section 5 — Exceptions, Properly

### 5.1 Custom exception hierarchies

```python
class AgentError(Exception):
    """Base for everything this agent raises."""

class ToolError(AgentError):
    def __init__(self, tool: str, message: str):
        self.tool = tool
        super().__init__(f"[{tool}] {message}")

class ToolTimeout(ToolError): ...
class ToolUnauthorized(ToolError): ...
```

Callers can then catch at exactly the altitude they care about:

```python
try:
    run_agent(request)
except ToolTimeout:
    retry_later(request)              # transient — retry
except AgentError as e:
    dead_letter(request, str(e))      # anything else ours — park it
```

This is the Python spelling of `L31` §3's dead-letter-replay pattern.

### 5.2 Rules worth internalising

| Rule | Why |
|---|---|
| Never `except:` bare | Catches `KeyboardInterrupt` and `SystemExit` — you cannot Ctrl-C your own program |
| Never `except Exception: pass` | Silent failure. The #1 cause of "the agent just stopped doing anything" |
| `raise NewError(...) from e` | Preserves the original cause in the traceback |
| `finally` always runs | Cleanup that must happen |
| `else` on `try` runs only if no exception | Rare but expressive |

```python
try:
    result = call_tool(name, args)
except KeyError as e:
    raise ToolError(name, f"missing argument {e}") from e      # ✅ chained
```

---

## Section 6 — Data Structures and Big-O

### 6.1 The complexity table you must be able to recite

| Operation | `list` | `dict` | `set` | `deque` |
|---|---|---|---|---|
| Index `x[i]` | **O(1)** | — | — | O(n) |
| Key/member lookup | **O(n)** | **O(1)** | **O(1)** | O(n) |
| Append to end | O(1)* | — | — | **O(1)** |
| Insert/pop at **front** | **O(n)** ❌ | — | — | **O(1)** ✅ |
| Delete by key/value | O(n) | **O(1)** | **O(1)** | O(n) |
| Ordered? | yes | yes (3.7+) | **no** | yes |

\* amortised.

### 6.2 The single most common interview mistake

```python
# ❌ O(n × m) — quadratic. Dies at scale.
missing = [v for v in dealer_vins if v not in known_vins]        # known_vins is a list

# ✅ O(n + m) — linear.
known = set(known_vins)
missing = [v for v in dealer_vins if v not in known]
```

At 100k × 100k that is roughly 10,000,000,000 comparisons versus 200,000. **`in` on a list is
O(n); `in` on a set or dict is O(1).** Converting to a set before a membership loop is the single
highest-value Big-O habit in day-to-day Python.

### 6.3 `collections` you should know

```python
from collections import defaultdict, Counter, deque

by_dealer = defaultdict(list)                    # no KeyError, auto-creates []
for r in records:
    by_dealer[r.dealer_id].append(r)

Counter(r.language for r in records).most_common(3)   # [('en', 8412), ('es', 1190)]

window = deque(maxlen=10)                        # fixed-size sliding window —
window.append(msg)                               # auto-evicts oldest. Perfect for
                                                 # agent short-term memory (L16, HLP01)
```

### 6.4 Complexity of things you already do

| Task | Complexity | Note |
|---|---|---|
| Sort | O(n log n) | Timsort — fast on partly-sorted data |
| Binary search (`bisect`) | O(log n) | list must be sorted |
| Brute-force vector search | **O(n·d)** | n vectors × d dimensions — why HNSW exists |
| HNSW / ANN index | ~O(log n) | approximate. This is `L09`/`L13`'s index |

That last pair is worth having ready — it explains *why* a vector database exists, in complexity
terms, which is a stronger answer than "it's faster."

---

## Section 7 — Design Patterns in Python

Python has first-class functions and duck typing, so several Gang-of-Four patterns collapse into
much less code than their C# forms. Knowing *which ones collapse* is the senior signal.

### 7.1 Strategy → just pass a function

```csharp
public interface IRefundStrategy { decimal Calc(Contract c); }   // C#: interface + classes
```

```python
def pro_rata(c: Contract) -> float:  return c.price * (60 - c.months_used) / 60
def flat_50(c: Contract) -> float:   return c.price * 0.5

STRATEGIES = {"pro_rata": pro_rata, "flat": flat_50}

def refund(c: Contract, strategy: str = "pro_rata") -> float:
    return STRATEGIES[strategy](c)          # no interface, no classes
```

### 7.2 Factory → a dict of callables

```python
AGENTS = {"cancellation": CancellationAgent, "claims": ClaimsAgent}

def make_agent(kind: str, **kw):
    try:
        return AGENTS[kind](**kw)
    except KeyError:
        raise ValueError(f"unknown agent: {kind}") from None
```

### 7.3 Singleton → a module

Python modules are imported once and cached. A module-level object *is* a singleton — no
double-checked locking required.

```python
# client.py
_client = AzureOpenAI(...)
def get_client(): return _client
```

### 7.4 Dependency injection → default arguments and Protocols

```python
from typing import Protocol

class ContractStore(Protocol):                      # structural typing — C# interface
    def lookup(self, vin: str) -> Contract: ...     # without needing to inherit

class Agent:
    def __init__(self, store: ContractStore, clock=time.time):
        self.store, self.clock = store, clock       # inject a fake clock in tests
```

`Protocol` is the one to remember: it gives you interface-style type checking **without** forcing
implementers to inherit anything. Perfect for testing agents with fake tools.

### 7.5 Repository, Adapter, Decorator

- **Repository** — unchanged from C#; a class wrapping data access.
- **Adapter** — this is what an **MCP server** is (`L26`): a uniform adapter over a bespoke API.
- **Decorator pattern** — §3. Python's `@` is the language-level form.

---

## Section 8 — Putting It Together

A tool layer for the JM Family cancellation agent using every section above:

```python
from __future__ import annotations
import functools, time
from dataclasses import dataclass
from typing import Protocol, Iterator
from contextlib import contextmanager
from pydantic import BaseModel, Field

# §1 — validated tool arguments
class CancelArgs(BaseModel):
    vin: str = Field(min_length=17, max_length=17)
    reason: str

@dataclass(frozen=True)                                    # §1 — immutable result
class Refund:
    contract_id: str
    amount: float
    method: str

class ContractStore(Protocol):                             # §7 — DI
    def lookup(self, vin: str) -> dict: ...

class ToolError(Exception): ...                            # §5 — typed failure

@contextmanager                                            # §4 + §2 — tracing span
def span(name: str) -> Iterator[None]:
    t0 = time.perf_counter()
    try:
        yield
    finally:
        print(f"[span] {name} {time.perf_counter()-t0:.3f}s")

def retry(times: int = 3):                                 # §3 — cross-cutting
    def deco(fn):
        @functools.wraps(fn)
        def wrapper(*a, **kw):
            for i in range(times):
                try:
                    return fn(*a, **kw)
                except ToolError:
                    if i == times - 1:
                        raise
                    time.sleep(2 ** i)
        return wrapper
    return deco

@retry(times=3)
def calc_refund(store: ContractStore, raw: str) -> Refund:
    """Calculate the pro-rata refund for a VSC contract. Args: vin, reason."""
    args = CancelArgs.model_validate_json(raw)             # §1 — validate LLM output
    with span("lookup"):                                   # §4
        c = store.lookup(args.vin)
    if not c:
        raise ToolError(f"no contract for {args.vin}")     # §5
    amount = round(c["price"] * (60 - c["months_used"]) / 60 - 50, 2)
    return Refund(c["id"], amount, "pro-rata")

def stream_contracts(path: str) -> Iterator[dict]:         # §2 — constant memory
    with open(path) as f:
        for line in f:
            yield parse(line)
```

Every JD sub-skill appears here. If you can write this file from memory, §15 and §16 of the tracker
are green.

---

## JM Family Anchor

| Where you would use it | Section |
|---|---|
| Streaming the CallMiner `.unl` feed row by row in constant memory | §2 generators |
| `@retry` on the DSX scan client's HTTP calls | §3 decorators |
| Validating LLM-extracted VIN/contract JSON before hitting the API | §1.4 Pydantic |
| `set()` before checking 100k dealer VINs against a known list | §6.2 Big-O |
| `deque(maxlen=10)` as an agent's short-term memory window | §6.3 |
| Tracing spans around each tool call, feeding OTel | §4 + `L36` |

---

## Self-Test Questions

1. What does `@dataclass` generate for you, and what is the C# equivalent?
2. Why does `def f(items=[])` accumulate values across calls? What are the two fixes?
3. What is the difference between `[x for x in y]` and `(x for x in y)`?
4. A generator is iterated twice and the second loop sees nothing. Why, and how would you fix it?
5. Rewrite `@retry(times=3)` from memory. How many nested functions, and what does each return?
6. Why does omitting `functools.wraps` break LangChain's `@tool`?
7. What does returning `True` from `__exit__` do, and why is it almost always wrong?
8. `v not in known_vins` where `known_vins` is a list of 100k items — what is the complexity, and
   what one-line change fixes it?
9. When would you use `deque` over `list`?
10. Why does `Protocol` beat an abstract base class for injecting a fake tool in tests?
11. Give the complexity of brute-force vector search and of an HNSW index. Why does that difference
    justify a vector database?
12. Which Gang-of-Four patterns effectively disappear in Python, and why?

---

## Quick-Reference Interview Answers

**"Explain decorators."**
> "A decorator is a function that takes a function and returns a replacement — `@retry` is just
> `fn = retry(fn)`. I use them for cross-cutting concerns: retry with backoff, timing, auth,
> caching. The one thing people miss is `functools.wraps` — without it the wrapper loses the
> original name, docstring and signature, which breaks anything that introspects the function. That
> matters in AI code specifically, because LangChain's `@tool` reads the docstring to build the
> schema it shows the model."

**"When would you use a generator?"**
> "Whenever the data is larger than memory or the consumer might stop early. I stream a multi-GB
> delimited feed with a generator pipeline — read, parse, filter — and nothing materialises until
> the final consumer pulls, so it runs in constant memory instead of gigabytes. The other place is
> token streaming from an LLM: `yield` each chunk as it arrives so the UI can render it live. The
> gotcha is that generators are single-use — iterate one twice and the second pass silently sees
> nothing, which is a nasty bug inside retry logic."

**"How do you think about performance in Python?"**
> "Start with the data structure, not the micro-optimisation. The most common real bug I see is a
> membership test against a list inside a loop — that's O(n·m). Converting the lookup side to a set
> makes it O(n+m); at 100k by 100k that's ten billion comparisons down to two hundred thousand. Same
> reasoning scales up: brute-force vector search is O(n·d), which is exactly why an approximate index
> like HNSW exists and why we pay for a vector database."

**"Are design patterns different in Python?"**
> "Several collapse. Strategy is just passing a function, because functions are first-class —
> no interface, no concrete classes. Factory is a dict of callables. Singleton is a module, since
> imports are cached. The ones that survive are the structural ones: Repository, Adapter, and
> Decorator — and Decorator is built into the language. For interface-style contracts I use
> `Protocol`, which gives structural typing, so a test double doesn't have to inherit anything to
> satisfy the type checker."

---

## Related

`L21` (read-level Python, prerequisite) · `L31` (Polly retry/circuit breaker — the C# form of §3.3) ·
`L26` (MCP as the Adapter pattern) · `06_Supplementary/PythonTrack/1.5-AIAgents.md` (practice) ·
`L36` (tracing spans built on §4)
