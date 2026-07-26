# Q&A — L32 Advanced Python for AI Engineers

*Created 2026-07-26 · FDE-Prep · Self-test, deliberately non-overlapping with the module's own
Self-Test section. Answer aloud before reading the answer.*

---

**Q1. `@dataclass` — what exactly does it generate, and what is the C# equivalent?**

`__init__`, `__repr__` and `__eq__` (value equality). With `order=True` it also generates comparison
operators. The C# equivalent is a `record` — same purpose: a data-carrying type with a generated
constructor, printing and value equality.

---

**Q2. Why does this accumulate across calls, and what are the two fixes?**

```python
def f(items=[]):
    items.append(1)
    return items
```

Default arguments are evaluated **once, at function-definition time** — not per call. So every call
shares the same list object. Fixes: `items: list | None = None` then `items = items or []` inside;
or in a dataclass, `field(default_factory=list)`.

---

**Q3. `[x for x in y]` vs `(x for x in y)`?**

Brackets build a list immediately — all elements in memory. Parentheses create a generator — nothing
is computed until iterated. For anything file-sized, use the generator.

---

**Q4. A generator is iterated twice and the second loop sees nothing. Why? Fix?**

Generators are single-use. Once exhausted they yield nothing — and raise no error, which is what
makes it dangerous inside retry logic. Fix: materialise to a list if you need multiple passes, or
create a fresh generator per pass.

---

**Q5. Write `@retry(times=3)` from memory. How many nested functions, and what does each return?**

Three levels:
- `retry(times)` → returns `decorator`
- `decorator(fn)` → returns `wrapper`
- `wrapper(*args, **kwargs)` → the thing that actually runs and calls `fn`

The `@functools.wraps(fn)` goes on `wrapper`.

---

**Q6. Why does omitting `functools.wraps` break LangChain's `@tool`?**

Without it the wrapper does not carry the original `__name__`, `__doc__` or signature. LangChain
reads the **docstring** to build the tool description shown to the model, and the signature to build
the argument schema. Lose those and the model gets a nameless tool with no description.

---

**Q7. What does returning `True` from `__exit__` do? Why is it almost always wrong?**

It **swallows the exception** — the error disappears and execution continues as if nothing failed.
Almost always wrong because it converts a loud failure into a silent one. Return `False`.

---

**Q8. `v not in known_vins` where `known_vins` is a list of 100,000 items, inside a loop over
100,000 items. Complexity? One-line fix?**

O(n·m) — about 10 billion comparisons. `known = set(known_vins)` first makes membership O(1),
giving O(n+m) — about 200,000 operations.

---

**Q9. When would you use `deque` over `list`?**

When you insert or remove at the **front**: `list` is O(n) there, `deque` is O(1). And
`deque(maxlen=N)` gives a self-evicting fixed-size window — exactly what an agent's short-term
memory buffer wants.

---

**Q10. Why does `Protocol` beat an abstract base class for injecting a fake tool in tests?**

`Protocol` is **structural** — anything with matching method signatures satisfies it, with no
inheritance. Your test double does not have to import or subclass anything, and third-party classes
can satisfy your interface without modification.

---

**Q11. Complexity of brute-force vector search vs an HNSW index — and why does that justify a vector
database?**

Brute force is O(n·d) — every vector, every dimension. HNSW is roughly O(log n), approximate. At
millions of vectors that is the difference between seconds and milliseconds per query. The vector
database exists to provide the index, not the storage.

---

**Q12. Which Gang-of-Four patterns effectively disappear in Python, and why?**

Strategy (functions are first-class — just pass one), Factory (a dict of callables), and Singleton
(a module is imported once and cached). They disappear because Python has first-class functions and
duck typing, so the ceremony those patterns exist to provide is already in the language. The
structural ones — Repository, Adapter, Decorator — survive.

---

**Q13. What does `@dataclass(frozen=True)` buy you in agent code specifically?**

Immutable configuration and results. Agent state bugs are painful because the mutation happens
several tool calls before the symptom appears; freezing the objects that flow through the loop turns
a silent corruption into an immediate `FrozenInstanceError`.

---

**Q14. Why validate LLM output with Pydantic rather than `json.loads`?**

`json.loads` proves the text is JSON; it does not prove the *shape* is right. Pydantic validates
types, required fields and constraints at the boundary, so a malformed VIN fails at parse time
rather than three layers down inside a tool call.

---

**Q15. `lru_cache` on an embedding function — one benefit and two constraints.**

Benefit: identical inputs skip a paid API call entirely. Constraints: arguments and return values
must be **hashable** (so return a tuple, not a list), and there is **no TTL** — entries never
expire, so it is wrong for anything that can go stale.

---

## Scoring

| Score | Read |
|---|---|
| 13–15 | Row 15 is green. Move to `L33`. |
| 9–12 | Re-read the sections you missed — they map 1:1 to §1–§7. |
| < 9 | Re-read `L32` end to end, then `06_Supplementary/PythonTrack/1.5-AIAgents.md` as practice. |
