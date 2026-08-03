# Module 13 — RAG (Retrieval-Augmented Generation) Deep Dive
**Part 3: Generative AI & LLMs | AI Solutions Architect Curriculum**

---

## What You Already Know (Recap)

From prior modules and JM Family work:
- **LLMs (Module 11)** — transformer architecture, tokenization, context windows, embeddings
- **Azure OpenAI (Module 12)** — Chat Completions API, embeddings API, function calling
- **Azure AI Search (Module 9)** — index, hybrid search, semantic ranking, HNSW vectors
- **Document Intelligence (Module 8)** — extracting structured data from PDFs
- **RAG basics (covered informally)** — retrieval → context → LLM → answer flow

This module goes deep on every layer of RAG — how to chunk documents correctly, which retrieval strategy to use, how to construct prompts with context, and advanced patterns that solve real production problems.

---

**Running example (used throughout):**
> *JM Family's enterprise RAG app: employees ask questions about vehicle invoices, dealer agreements, and policy documents. The system retrieves relevant chunks from Azure AI Search and generates accurate, cited answers via Azure OpenAI.*

---

## Topic 13.1 — RAG Fundamentals

---

### 1. Why RAG Exists

LLMs have two fundamental limitations:

| Limitation | Problem | RAG Solution |
|-----------|---------|-------------|
| **Knowledge cutoff** | Model trained on data up to a date — knows nothing after | Retrieve current documents at query time |
| **Context size** | Can't fit your entire document library in the prompt | Retrieve only the relevant chunks |
| **Hallucination** | Makes up facts it doesn't know | Ground answers in retrieved documents |
| **No private data** | Model never saw your company's internal documents | Retrieve from your own indexed data |

RAG = LLM's knowledge + your up-to-date private data, fused at query time.

---

### 2. The Basic RAG Pattern

```
User Question
     ↓
[Retrieve] → Search index for relevant document chunks
     ↓
[Augment]  → Add retrieved chunks to the prompt as context
     ↓
[Generate] → LLM answers using the context, not its training data
     ↓
Answer (grounded in your documents)
```

The three letters in RAG are also the three steps.

---

### 3. Naive RAG vs Advanced RAG vs Modular RAG

| Generation | What it is | Problem it solves |
|-----------|-----------|------------------|
| **Naive RAG** | Basic retrieval → prompt → generate | Works for simple Q&A, fails on complex queries |
| **Advanced RAG** | Pre-retrieval + post-retrieval improvements | Better chunk quality, better retrieval |
| **Modular RAG** | RAG as pipeline with swappable components (agents, routing, tools) | Handles complex multi-step reasoning |

You'll start with Naive RAG to understand the fundamentals, then learn Advanced and Modular patterns in Topics 13.5 and 13.8.

---

### 4. What RAG Is NOT

Common misconceptions:

| Not RAG | What it actually is |
|---------|-------------------|
| Fine-tuning on your documents | Fine-tuning bakes knowledge into weights — RAG retrieves at runtime |
| Semantic search | Semantic search retrieves; RAG adds generation on top |
| Full document in the context window | "Stuff everything in" is not RAG — no retrieval step |
| Azure AI Search alone | Search is the retrieval layer; RAG includes the generation layer |

---

### 5. RAG Architecture Components

```
┌─────────────────────────────────────────────────────────────┐
│                    OFFLINE (Indexing)                       │
│  Documents → Chunking → Embedding → Vector Store           │
├─────────────────────────────────────────────────────────────┤
│                    ONLINE (Query)                           │
│  Question → Embed → Retrieve → Augment Prompt → Generate   │
└─────────────────────────────────────────────────────────────┘
```

Two separate pipelines:
- **Indexing pipeline** — runs once per document (or when docs update)
- **Query pipeline** — runs on every user question, in real time

---

## Topic 13.2 — Document Processing

---

### 1. The Document Processing Problem

Before you can chunk and embed a document, you need clean text. Raw documents are messy:

| Document type | Problem | Solution |
|--------------|---------|---------|
| **PDF (text-based)** | Columns, headers, footers break text flow | Azure AI Document Intelligence layout model |
| **PDF (scanned/image)** | No text layer — just pixels | Document Intelligence OCR + layout |
| **Word (.docx)** | Metadata, revision history, styles embedded | Parse with DocumentFormat.OpenXml or DI |
| **Excel (.xlsx)** | Tabular data — doesn't chunk like prose | Serialize rows to structured text |
| **HTML** | Tags, scripts, navigation noise | Strip tags, keep semantic structure |
| **PowerPoint (.pptx)** | Slides = visual structure, not prose | Extract per-slide text, preserve slide order |

---

### 2. Document Loading in Azure

**Option A — Azure AI Document Intelligence (your JM Family pattern):**
```csharp
var client = new DocumentAnalysisClient(endpoint, new DefaultAzureCredential());
var operation = await client.AnalyzeDocumentFromUriAsync(
    WaitUntil.Completed, "prebuilt-layout", documentUri);

var result = operation.Value;

// Extract clean text preserving structure
var cleanText = new StringBuilder();
foreach (var paragraph in result.Paragraphs)
{
    cleanText.AppendLine(paragraph.Content);
}
// Tables — serialize to text
foreach (var table in result.Tables)
{
    foreach (var cell in table.Cells)
    {
        cleanText.Append($"{cell.Content}\t");
    }
    cleanText.AppendLine();
}
```

**Why use Document Intelligence for loading (not just extraction):**
- Preserves reading order (columns, headers, footers are correctly sequenced)
- Tables serialized as structured text (not jumbled)
- Page boundaries preserved for citation (chunk → page number)

**Option B — Direct text extraction (simpler documents):**
```csharp
// For .docx
using var doc = WordprocessingDocument.Open(stream, false);
var text = doc.MainDocumentPart.Document.Body.InnerText;
```

---

### 3. Text Cleaning Before Chunking

After extraction, clean before chunking:

```csharp
private string CleanText(string raw)
{
    // Remove excessive whitespace
    var text = Regex.Replace(raw, @"\s{3,}", "\n\n");
    // Remove headers/footers (page numbers, document titles repeated)
    text = Regex.Replace(text, @"Page \d+ of \d+", "");
    // Normalize unicode
    text = text.Normalize(NormalizationForm.FormC);
    // Remove null bytes and control characters
    text = Regex.Replace(text, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", "");
    return text.Trim();
}
```

**What to preserve:**
- Section headings — critical context for chunking and retrieval
- Table structure — serialize rows, keep column names
- Lists — preserve bullet structure
- Page numbers — needed for citations

**What to strip:**
- Repeated headers/footers
- Page number patterns
- Watermarks (if extractable)
- Excessive whitespace

---

### 4. Metadata Extraction

Every chunk needs metadata for filtering and citation:

```csharp
public class DocumentChunk
{
    public string Id { get; set; }           // "invoice-001-chunk-03"
    public string Content { get; set; }      // the text
    public float[] ContentVector { get; set; } // embedding
    public string SourceDocumentId { get; set; } // "invoice-001"
    public string SourceFileName { get; set; }   // "FordInvoice_2026_01.pdf"
    public int PageNumber { get; set; }          // 2
    public int ChunkIndex { get; set; }          // 3
    public string DocumentType { get; set; }     // "invoice"
    public string DealerCode { get; set; }       // "JMF-ATL-001"
    public DateTime DocumentDate { get; set; }   // 2026-01-15
    public string Section { get; set; }          // "Line Items"
}
```

**Why metadata matters:**
- **Filtering** — `filter=documentType eq 'invoice' and dealerCode eq 'JMF-ATL-001'`
- **Citation** — "Source: FordInvoice_2026_01.pdf, Page 2"
- **Re-ranking** — boost recent documents, filter by date range
- **Deduplication** — re-index updated documents by `SourceDocumentId`

---

## Topic 13.3 — Chunking Strategies

---

### 1. Why Chunking Matters

Chunking is the most impactful decision in your RAG pipeline. A bad chunking strategy breaks everything downstream:

| Problem | Cause | Effect |
|---------|-------|--------|
| Chunk too large | Embeds whole page as one vector | Vector averages too much meaning — poor retrieval |
| Chunk too small | Single sentence per chunk | Lacks context — LLM can't answer from it |
| Split mid-sentence | Fixed character split | Broken meaning, bad embeddings |
| Split mid-table | Fixed character split | Table cells in different chunks — LLM can't read the table |
| No overlap | Fixed chunks with no overlap | Answer spans two chunks — retrieval misses it |

---

### 2. Fixed-Size Chunking

Split by token count or character count with overlap.

```csharp
public List<string> FixedSizeChunk(string text, int chunkSize = 512, int overlap = 50)
{
    var words = text.Split(' ');
    var chunks = new List<string>();
    int start = 0;

    while (start < words.Length)
    {
        int end = Math.Min(start + chunkSize, words.Length);
        chunks.Add(string.Join(" ", words[start..end]));
        start += chunkSize - overlap;  // overlap: last 50 words repeat in next chunk
    }
    return chunks;
}
```

**Overlap purpose:** If an answer spans the boundary of two chunks, the overlap ensures one chunk contains enough context to be useful.

```
Chunk 1: [...words 1-512...]
Chunk 2: [...words 463-974...]  ← words 463-512 repeated (overlap)
Chunk 3: [...words 925-1436...]
```

**When to use:** Simple documents with uniform prose structure. Fast, predictable.
**Avoid for:** Tables, structured documents, documents with headers/sections.

---

### 3. Sentence / Paragraph Chunking

Split on sentence or paragraph boundaries:

```csharp
// Paragraph chunking
var paragraphs = text.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

// Sentence chunking (simple)
var sentences = text.Split(new[] { ". ", "! ", "? " }, StringSplitOptions.None);

// Group sentences into chunks of ~300 tokens
var chunks = new List<string>();
var current = new StringBuilder();
foreach (var sentence in sentences)
{
    if (current.Length + sentence.Length > 1500)  // ~300 tokens ≈ 1500 chars
    {
        chunks.Add(current.ToString().Trim());
        current.Clear();
    }
    current.Append(sentence + ". ");
}
```

**When to use:** Articles, policies, documentation with clear paragraph structure.
**Advantage:** No mid-sentence breaks — embeddings are on complete thoughts.

---

### 4. Recursive Character Chunking

The most common strategy — tries to split on natural boundaries in priority order:

```
Priority order: \n\n → \n → . → , → " " → character
```

```csharp
public List<string> RecursiveChunk(string text, int maxChunkSize = 512, int overlap = 50)
{
    var separators = new[] { "\n\n", "\n", ". ", ", ", " ", "" };
    return SplitRecursive(text, separators, maxChunkSize, overlap);
}

private List<string> SplitRecursive(string text, string[] separators, int maxSize, int overlap)
{
    if (text.Length <= maxSize) return new List<string> { text };

    // Try each separator in priority order
    foreach (var sep in separators)
    {
        if (!text.Contains(sep)) continue;

        var parts = text.Split(sep);
        var chunks = new List<string>();
        var current = new StringBuilder();

        foreach (var part in parts)
        {
            if (current.Length + part.Length + sep.Length <= maxSize)
            {
                current.Append(part + sep);
            }
            else
            {
                if (current.Length > 0) chunks.Add(current.ToString().Trim());
                current.Clear().Append(part + sep);
            }
        }
        if (current.Length > 0) chunks.Add(current.ToString().Trim());
        return chunks;
    }
    // Fall back to character split
    return Enumerable.Range(0, (text.Length + maxSize - 1) / maxSize)
        .Select(i => text.Substring(i * maxSize, Math.Min(maxSize, text.Length - i * maxSize)))
        .ToList();
}
```

**This is the default strategy in LangChain (`RecursiveCharacterTextSplitter`).** Use it unless you have a specific reason not to.

---

### 5. Semantic Chunking

Split based on meaning shifts, not structure:

```
1. Embed every sentence individually
2. Calculate cosine similarity between adjacent sentences
3. Where similarity drops sharply = topic boundary = chunk boundary
```

```csharp
public async Task<List<string>> SemanticChunkAsync(string text, float breakThreshold = 0.3f)
{
    var sentences = text.Split(new[] { ". ", "! ", "? " }, StringSplitOptions.None);
    var embeddings = await EmbedBatchAsync(sentences);  // embed all sentences

    var chunks = new List<string>();
    var currentChunk = new List<string> { sentences[0] };

    for (int i = 1; i < sentences.Length; i++)
    {
        float similarity = CosineSimilarity(embeddings[i - 1], embeddings[i]);
        if (similarity < breakThreshold)  // topic shift detected
        {
            chunks.Add(string.Join(". ", currentChunk));
            currentChunk = new List<string>();
        }
        currentChunk.Add(sentences[i]);
    }
    if (currentChunk.Any()) chunks.Add(string.Join(". ", currentChunk));
    return chunks;
}
```

**When to use:** Long documents where topics shift mid-page (annual reports, research papers).
**Downside:** Expensive — requires embedding every sentence during chunking.

---

### 6. Document-Specific Chunking

For structured documents like invoices and contracts:

**Table chunking — preserve table structure:**
```csharp
// Serialize table as structured text — don't split mid-table
foreach (var table in docResult.Tables)
{
    var tableText = new StringBuilder();
    tableText.AppendLine($"[TABLE: {table.RowCount} rows × {table.ColumnCount} cols]");
    // Group cells by row
    var rows = table.Cells.GroupBy(c => c.RowIndex).OrderBy(g => g.Key);
    foreach (var row in rows)
    {
        var cells = row.OrderBy(c => c.ColumnIndex).Select(c => c.Content);
        tableText.AppendLine(string.Join(" | ", cells));
    }
    // Treat entire table as one chunk (don't split it)
    chunks.Add(new DocumentChunk { Content = tableText.ToString(), Section = "Table" });
}
```

**Section-aware chunking — use headings as chunk boundaries:**
```csharp
// Use Document Intelligence paragraph roles to detect headings
foreach (var paragraph in docResult.Paragraphs)
{
    if (paragraph.Role == ParagraphRole.SectionHeading)
    {
        // Save current chunk, start new one
        if (currentChunk.Length > 0) SaveChunk(currentChunk.ToString(), currentSection);
        currentSection = paragraph.Content;
        currentChunk.Clear();
    }
    currentChunk.AppendLine(paragraph.Content);
}
```

---

### 7. Chunk Size Guidelines

| Document type | Recommended chunk size | Overlap |
|--------------|----------------------|---------|
| Invoices / forms (structured) | 256–512 tokens | 20–30 tokens |
| Policies / contracts (prose) | 512–1024 tokens | 50–100 tokens |
| Technical docs / manuals | 512–768 tokens | 50 tokens |
| FAQs | One Q&A pair per chunk | None |
| Tables | Entire table as one chunk | None |

**Rule of thumb:** Chunk size should be large enough to answer a question on its own, small enough that the vector is specific. 512 tokens is a good default starting point.

**Token ≈ 4 characters in English.** 512 tokens ≈ 2048 characters ≈ ~350 words.

---

### 8. Parent-Child Chunking (Small-to-Big Retrieval)

Store two levels of chunks — retrieve small, return large:

```
Parent chunk: 1024 tokens (full section)
    ├── Child chunk A: 256 tokens (first part)
    ├── Child chunk B: 256 tokens (second part)
    └── Child chunk C: 256 tokens (third part)
```

- **Index child chunks** (small = specific vectors, better retrieval)
- **When a child chunk matches**, return its **parent chunk** to the LLM (more context)

```csharp
public class ParentChildChunk
{
    public string ChildId { get; set; }     // "doc-001-child-03"
    public string ParentId { get; set; }    // "doc-001-parent-01"
    public string ChildContent { get; set; }   // indexed + embedded
    public string ParentContent { get; set; }  // retrieved but not indexed
}

// At query time:
var childResults = await SearchAsync(queryVector, topK: 5);
var parentContents = childResults
    .Select(r => GetParentContent(r.ParentId))  // fetch parent from Cosmos/Blob
    .Distinct()
    .ToList();
// Send parentContents to LLM (not child contents)
```

**Why this works:** Small chunks have precise, specific embeddings (better recall). Large parents have full context (better answer quality).

---

## Topic 13.4 — Vector Databases

---

### 1. What Is a Vector Database?

A vector database stores embeddings (float arrays) and answers the question: *"Which stored vectors are closest to this query vector?"*

Standard databases (SQL, Cosmos DB) store and filter structured data.
Vector databases store and search by mathematical distance in high-dimensional space.

---

### 2. Azure AI Search as Your Vector Database

You already know Azure AI Search from Module 9. For JM Family, it is both your **keyword search engine** and your **vector database** in one service.

**What makes it a vector DB:**
- Stores `Collection(Edm.Single)` fields (float arrays)
- HNSW approximate nearest neighbor index
- Cosine / Euclidean / dot product distance metrics
- Native hybrid search (keyword + vector in one query)
- Filters on metadata fields alongside vector search

**When Azure AI Search is the right choice:**
- Already using it for keyword/full-text search
- Need hybrid search (keyword + semantic)
- Azure-native deployment (Managed Identity, VNet, compliance)
- Documents already indexed there

---

### 3. Other Vector Database Options

| Database | Type | Best for | Notes |
|----------|------|---------|-------|
| **Azure AI Search** | Managed cloud | Azure-native, hybrid search, enterprise | Your primary choice |
| **Qdrant** | Open source / cloud | High performance, filtering | Popular in open-source RAG stacks |
| **Pinecone** | Managed cloud | Pure vector search, serverless | Popular but no keyword search |
| **Weaviate** | Open source / cloud | Multi-modal, graph features | Richer but more complex |
| **Chroma** | Open source, local | Local dev, LangChain default | Not production-scale |
| **pgvector** | PostgreSQL extension | Already using PostgreSQL | Good for small-to-medium scale |
| **Cosmos DB (NoSQL)** | Managed cloud | Already using Cosmos + need vectors | Added vector search support |
| **Redis** | In-memory | Ultra-low latency | Cache + vector DB |

**For JM Family:** Azure AI Search covers everything you need. You don't need a separate vector database.

---

### 4. Distance Metrics

How similarity between vectors is measured:

| Metric | Formula | Use for |
|--------|---------|---------|
| **Cosine similarity** | cos(θ) between vectors | Text embeddings (direction matters, not magnitude) |
| **Dot product** | a · b | Normalized embeddings — equivalent to cosine, faster |
| **Euclidean (L2)** | √Σ(aᵢ-bᵢ)² | Image embeddings, when magnitude matters |

**For text RAG:** Always use **cosine similarity**. Text-embedding-3-* models are trained for cosine distance.

---

### 5. HNSW — How Vector Search Works Internally

HNSW (Hierarchical Navigable Small World) is the algorithm behind fast vector search.

```
Layer 2 (sparse): A ─────── E
Layer 1 (medium): A ─── C ─── E ─── G
Layer 0 (dense):  A─B─C─D─E─F─G─H  (all nodes)
```

**Search process:**
1. Start at entry point in top layer
2. Greedily navigate to closest node to query
3. Drop to next layer, repeat
4. At bottom layer, collect K nearest neighbors

**Why it's fast:** Instead of comparing query to all N vectors (O(N)), HNSW navigates a graph structure (O(log N)).

**Trade-off:** Approximate, not exact. A small number of true nearest neighbors may be missed. In practice, recall is ~99%+ with good parameters.

---

## Topic 13.5 — Retrieval Strategies

---

### 1. The Retrieval Problem

Retrieval quality determines answer quality. If the wrong chunks are retrieved, the LLM either:
- Answers incorrectly using wrong context
- Says "I don't know" (if context doesn't match)
- Hallucinates (if context is absent and it falls back to training data)

The retrieval strategies below address specific failure modes.

---

### 2. Basic Retrieval — Top-K

Retrieve the K most similar chunks to the query:

```csharp
var results = await searchClient.SearchAsync<DocumentChunk>(
    searchText: userQuery,
    new SearchOptions
    {
        VectorSearch = new VectorSearchOptions
        {
            Queries = { new VectorizedQuery(queryEmbedding) { KNearestNeighborsCount = 5, Fields = { "contentVector" } } }
        },
        QueryType = SearchQueryType.Semantic,
        SemanticSearch = new SemanticSearchOptions { SemanticConfigurationName = "my-config" },
        Size = 5
    }
);
```

**K=5 is a common default.** More chunks = more context but also more noise + token cost.

---

### 3. Filtered Retrieval

Pre-filter before vector search to restrict the search space:

```csharp
new SearchOptions
{
    Filter = "dealerCode eq 'JMF-ATL-001' and documentDate ge 2026-01-01T00:00:00Z",
    VectorSearch = new VectorSearchOptions { ... }
}
```

**Always filter when you can.** Filtering before vector search:
- Reduces candidates (faster)
- Reduces noise (wrong dealer's invoices won't appear)
- Allows user-specific or tenant-specific retrieval (multi-tenancy)

---

### 4. Multi-Query Retrieval

A single user question may not perfectly match the document language. Generate multiple search queries from one question:

```
User: "Did we get the right Ford vehicles from the Atlanta dealer last month?"

Generated queries:
  1. "Ford vehicle invoice Atlanta dealer January 2026"
  2. "JMF-ATL Ford order fulfillment January"
  3. "Atlanta Ford delivery confirmation 2026"
```

```csharp
// Use LLM to generate query variants
var systemPrompt = "Generate 3 different search queries for the user's question. Return as JSON array.";
var queries = await GenerateQueriesAsync(systemPrompt, userQuestion);

// Retrieve for each query, deduplicate results
var allResults = new Dictionary<string, DocumentChunk>();
foreach (var query in queries)
{
    var results = await RetrieveAsync(query, topK: 3);
    foreach (var r in results)
        allResults.TryAdd(r.Id, r);  // deduplicate by chunk ID
}
// Return unique top results
```

**When to use:** Complex questions, business domain language that differs from document language.

---

### 5. HyDE — Hypothetical Document Embeddings

Instead of embedding the question, ask the LLM to write a hypothetical answer, then embed that:

```
User question: "What is the process for dealer invoice reconciliation?"
        ↓
LLM writes: "The dealer invoice reconciliation process involves matching..."
(fake answer based on LLM's general knowledge)
        ↓
Embed the fake answer (not the question)
        ↓
Search: fake answer vector vs real document vectors
```

**Why this works:** A hypothetical answer written in document-style language matches real documents better than a question written in conversational language.

```csharp
var hydePrompt = $"Write a detailed paragraph that would answer: '{userQuestion}'. " +
                  "Use formal document language. Don't say you don't know — write what a good answer would look like.";
var hypotheticalAnswer = await chatClient.GetAnswerAsync(hydePrompt);
var hydeEmbedding = await embeddingsClient.EmbedAsync(hypotheticalAnswer);
// Use hydeEmbedding for vector search instead of question embedding
```

**When to use:** When retrieval recall is poor — questions and documents use very different language.

---

### 6. Maximal Marginal Relevance (MMR)

Standard top-K returns the K most similar chunks — but they might all say the same thing. MMR balances relevance with diversity:

```
Iteration 1: Pick most similar chunk to query
Iteration 2: Pick chunk that is similar to query AND dissimilar to chunk 1
Iteration 3: Pick chunk similar to query AND dissimilar to chunks 1+2
...
```

```csharp
public List<DocumentChunk> MMRSelect(
    float[] queryVector,
    List<(DocumentChunk Chunk, float[] Vector)> candidates,
    int k = 5,
    float lambda = 0.5f)  // 0=max diversity, 1=max relevance
{
    var selected = new List<(DocumentChunk Chunk, float[] Vector)>();
    var remaining = candidates.ToList();

    while (selected.Count < k && remaining.Any())
    {
        var best = remaining.MaxBy(c =>
        {
            float relevance = CosineSimilarity(queryVector, c.Vector);
            float redundancy = selected.Any()
                ? selected.Max(s => CosineSimilarity(s.Vector, c.Vector))
                : 0f;
            return lambda * relevance - (1 - lambda) * redundancy;
        });
        selected.Add(best);
        remaining.Remove(best);
    }
    return selected.Select(s => s.Chunk).ToList();
}
```

**When to use:** Long documents with repeated content (policy manuals, FAQs with similar phrasing).

---

### 7. Re-Ranking with a Cross-Encoder

Two-stage retrieval: retrieve broadly first, then re-rank precisely:

```
Stage 1 — Bi-encoder (fast, approximate):
  Embed query → retrieve top 50 candidates from vector DB

Stage 2 — Cross-encoder (slow, precise):
  Feed each (query, candidate) pair through a small model
  Get a precise relevance score
  Re-rank and keep top 5
```

**Bi-encoder vs Cross-encoder:**
| | Bi-encoder | Cross-encoder |
|--|-----------|--------------|
| How | Embeds query and doc separately | Reads query + doc together |
| Speed | Fast (precomputed doc embeddings) | Slow (runs model per pair) |
| Accuracy | Good | Better (sees full interaction) |
| Use | First-stage retrieval | Re-ranking shortlist |

**Azure option:** Azure AI Search semantic ranker is a managed cross-encoder re-ranker. You're already using it in Module 9 with `QueryType = SearchQueryType.Semantic`.

---

### 8. Self-Querying Retrieval

Let the LLM write the structured filter query from natural language:

```
User: "Show me invoices from Ford dealers in Atlanta over $40,000 in January"
        ↓
LLM extracts structured filters:
{
  "searchQuery": "Ford dealer invoices",
  "filters": {
    "vehicleMake": "Ford",
    "dealerCity": "Atlanta",
    "minAmount": 40000,
    "dateRange": "2026-01"
  }
}
        ↓
Azure AI Search: filter=vehicleMake eq 'Ford' and dealerCity eq 'Atlanta' and totalAmount gt 40000
```

```csharp
var extractPrompt = $"""
    Extract search filters from this query as JSON.
    Available fields: vehicleMake (string), dealerCity (string), totalAmount (number), documentDate (date)
    Query: "{userQuestion}"
    Return: {{"searchQuery": "...", "filter": "OData filter string"}}
    """;
var filterJson = await ExtractFiltersAsync(extractPrompt);
```

**When to use:** Users ask in natural language about structured data. Eliminates manual filter UI.

---

## Topic 13.6 — Generation with Retrieved Context

---

### 1. Prompt Construction

How you assemble the prompt determines answer quality. The standard RAG prompt structure:

```
┌─────────────────────────────────────────────────────────────┐
│ SYSTEM MESSAGE                                              │
│ Role definition + behavior rules + citation instructions   │
├─────────────────────────────────────────────────────────────┤
│ USER MESSAGE                                                │
│ [Context section]                                           │
│   Source 1: {chunk1.content} (from {chunk1.fileName}, p.{n})│
│   Source 2: {chunk2.content} (from {chunk2.fileName}, p.{n})│
│   ...                                                       │
│ [Question]                                                  │
│   {userQuestion}                                            │
└─────────────────────────────────────────────────────────────┘
```

**C# implementation:**
```csharp
private string BuildRAGPrompt(string userQuestion, List<DocumentChunk> chunks)
{
    var contextBuilder = new StringBuilder();
    for (int i = 0; i < chunks.Count; i++)
    {
        contextBuilder.AppendLine($"[Source {i + 1}]: {chunks[i].SourceFileName}, Page {chunks[i].PageNumber}");
        contextBuilder.AppendLine(chunks[i].Content);
        contextBuilder.AppendLine();
    }

    return $"""
        The following sources contain relevant information:

        {contextBuilder}

        Using ONLY the sources above, answer this question:
        {userQuestion}

        If the answer is not in the sources, say "I cannot find this information in the available documents."
        Cite sources using [Source N] notation.
        """;
}

var messages = new List<ChatMessage>
{
    new SystemChatMessage(
        "You are a JM Family document assistant. Answer questions about invoices and dealer agreements. " +
        "Base your answers only on the provided context. Never make up information not in the sources."),
    new UserChatMessage(BuildRAGPrompt(userQuestion, retrievedChunks))
};

var response = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
{
    Temperature = 0,   // 0 = deterministic, factual — always use 0 for RAG
    MaxOutputTokenCount = 1000
});
```

**Temperature = 0 for RAG** — you want factual, deterministic answers from documents, not creative generation.

---

### 2. Context Window Management

Every LLM has a finite context window. Budget it carefully:

```
GPT-4o context window: 128,000 tokens

Budget breakdown:
  System prompt:          ~200 tokens
  Retrieved chunks (5x):  ~2,500 tokens (5 × 512 tokens)
  User question:          ~50 tokens
  Conversation history:   ~1,000 tokens (if multi-turn)
  Answer reserve:         ~1,000 tokens
  ─────────────────────
  Total used:             ~4,750 tokens  (well within 128K)
```

**For GPT-4o this is rarely a problem.** Context window problems arise when:
- You retrieve too many large chunks (K=20, chunk size=2048)
- You include full conversation history without trimming
- Your system prompt is extremely long

**Context window strategies:**
| Strategy | When to use |
|---------|------------|
| **Truncate chunks** | If chunk exceeds limit, trim to first N tokens |
| **Summarize history** | For long conversations, summarize earlier turns |
| **Reduce K** | Reduce retrieved chunks from 10 to 5 |
| **Smaller chunks** | Reduce chunk size at indexing time |

---

### 3. Citation and Grounding

Always return citations with the answer:

```csharp
public class RAGResponse
{
    public string Answer { get; set; }
    public List<Citation> Citations { get; set; }
}

public class Citation
{
    public string SourceFile { get; set; }
    public int PageNumber { get; set; }
    public string RelevantExcerpt { get; set; }
    public float Score { get; set; }
}

// Build response with citations
var response = new RAGResponse
{
    Answer = llmAnswer,
    Citations = retrievedChunks.Select(c => new Citation
    {
        SourceFile = c.SourceFileName,
        PageNumber = c.PageNumber,
        RelevantExcerpt = c.Content[..Math.Min(200, c.Content.Length)],
        Score = c.SearchScore
    }).ToList()
};
```

**Why citations matter:**
- Users can verify the answer
- Regulators/auditors can trace AI decisions to source documents
- Debugging: if the answer is wrong, you can see which chunk mislead the LLM

---

### 4. Hallucination Prevention

RAG reduces hallucination but doesn't eliminate it. Defense strategies:

| Strategy | How |
|---------|-----|
| **"Answer from sources only"** | Explicit system prompt instruction |
| **"Say I don't know"** | Explicitly tell the LLM to admit when context is insufficient |
| **Groundedness check** | Azure Content Safety groundedness detection (Module 11.4) |
| **Temperature = 0** | Reduces creative generation |
| **Confidence threshold** | If search score < 0.7, don't answer — return "no relevant documents found" |
| **Citation requirement** | Force the LLM to cite a source for every claim — ungrounded claims have no citation |

```csharp
// Confidence gate — don't proceed if retrieval confidence is low
var topResult = results.First();
if (topResult.Score < 0.70)
{
    return new RAGResponse
    {
        Answer = "I could not find relevant information in the available documents for your question.",
        Citations = new List<Citation>()
    };
}
```

---

### 5. Conversation History in RAG (Multi-turn)

When users ask follow-up questions, they reference prior context:

```
Turn 1: "Show me Ford invoices from Atlanta"
Turn 2: "Which ones are over $40,000?"    ← "ones" refers to previous results
Turn 3: "Who is the contact for that dealer?"  ← "that dealer" = Atlanta
```

**Problem:** "Which ones are over $40,000" without history = meaningless retrieval query.

**Solution — Query rewriting with history:**
```csharp
var rewritePrompt = $"""
    Conversation history:
    User: Show me Ford invoices from Atlanta
    Assistant: Found 12 Ford invoices from Atlanta dealer JMF-ATL-001...

    Current question: {userQuestion}

    Rewrite the current question as a standalone search query with full context.
    """;
var standaloneQuery = await RewriteQueryAsync(rewritePrompt);
// Now search with standaloneQuery instead of userQuestion
```

**Rewritten query:** "Ford invoices from Atlanta dealer JMF-ATL-001 over $40,000"

---

## Topic 13.7 — Azure "On Your Data" Feature

---

### 1. What Is "On Your Data"?

Azure OpenAI's **On Your Data** feature is a **managed RAG** service — Microsoft handles the retrieval pipeline for you:

```
Without On Your Data (custom RAG):
  Your code: embed → search → build prompt → call chat → return answer

With On Your Data:
  Your code: call chat API with data_sources config → Azure does the rest
```

**Under the hood:** Azure OpenAI calls Azure AI Search on your behalf, retrieves relevant chunks, builds the prompt, and returns a grounded answer — all in one API call.

---

### 2. On Your Data — API Configuration

```csharp
var chatOptions = new ChatCompletionOptions();
chatOptions.AddDataSource(new AzureSearchChatDataSource
{
    Endpoint = new Uri("https://my-search.search.windows.net"),
    IndexName = "invoices-index",
    Authentication = new OnYourDataSystemAssignedManagedIdentityAuthenticationOptions(),
    QueryType = DataSourceQueryType.VectorSemanticHybrid,
    VectorizationSource = new OnYourDataDeploymentNameVectorizationSource
    {
        DeploymentName = "text-embedding-3-small"
    },
    SemanticConfiguration = "my-semantic-config",
    TopNDocuments = 5,
    InScope = true,         // only answer from indexed data
    Strictness = 3,         // 1-5: how strictly to ground (5 = very strict)
    RoleInformation = "You are a JM Family document assistant..."
});

var response = await chatClient.CompleteChatAsync(messages, chatOptions);

// Citations come back in the response
foreach (var context in response.Value.Choices[0].Message.AzureExtensionsContext?.Citations ?? [])
{
    Console.WriteLine($"Citation: {context.Title} — {context.Filepath}");
}
```

---

### 3. On Your Data — When to Use vs Custom RAG

| | On Your Data | Custom RAG |
|--|-------------|-----------|
| **Setup time** | Minutes | Days to weeks |
| **Control** | Low — Microsoft manages retrieval | Full control over every step |
| **Chunking** | Fixed (controlled by index) | Any strategy |
| **Retrieval tuning** | Limited | Full (HyDE, MMR, re-ranking, etc.) |
| **Multi-index** | Supported (multiple data sources) | Custom |
| **Conversation history** | Managed automatically | You manage |
| **Cost** | Included in Azure OpenAI | Pay for Search queries separately |
| **Advanced patterns** | Not supported | Supported |
| **Best for** | PoC, quick demos, simple use cases | Production apps requiring quality tuning |

**JM Family recommendation:** Start with On Your Data to validate the concept quickly, then migrate to custom RAG when you need quality control over chunking and retrieval.

---

### 4. On Your Data Limitations

- Cannot control chunking — uses whatever is in your index
- Cannot implement HyDE, MMR, or multi-query retrieval
- Limited prompt customization (role information only)
- No access to intermediate retrieval results for debugging
- Strictness setting is coarse — can't fine-tune per query

---

## Topic 13.8 — Advanced RAG Patterns

---

### 1. Corrective RAG (CRAG)

Add a self-correction step — if retrieval confidence is low, fall back to a broader search or web search:

```
Retrieve top chunks
    ↓
Evaluate relevance: are these chunks actually relevant to the question?
    ↓
If HIGH confidence: proceed to generation
If LOW confidence: trigger corrective action
    - Re-query with expanded terms
    - Search a different index
    - Return "insufficient information" response
    - Flag for human review
```

```csharp
public async Task<RAGResponse> CorrectiveRAGAsync(string question, string dealerCode)
{
    // First retrieval attempt
    var results = await RetrieveAsync(question, filter: $"dealerCode eq '{dealerCode}'");
    float avgScore = results.Average(r => r.Score);

    if (avgScore < 0.65f)
    {
        // Corrective: try without filter (broader search)
        results = await RetrieveAsync(question, filter: null);
        avgScore = results.Average(r => r.Score);
    }

    if (avgScore < 0.55f)
    {
        return new RAGResponse { Answer = "Insufficient document evidence to answer this question reliably." };
    }

    return await GenerateAsync(question, results);
}
```

---

### 2. Query Decomposition

Break complex multi-part questions into sub-questions, answer each, combine:

```
Complex: "Compare Ford and Honda invoice totals for Atlanta and Dallas dealers in Q1 2026"
        ↓
Decompose:
  Q1: "Ford invoice totals for Atlanta dealer JMF-ATL-001 Q1 2026"
  Q2: "Honda invoice totals for Atlanta dealer JMF-ATL-001 Q1 2026"
  Q3: "Ford invoice totals for Dallas dealer JMF-DAL-002 Q1 2026"
  Q4: "Honda invoice totals for Dallas dealer JMF-DAL-002 Q1 2026"
        ↓
Answer each sub-question individually
        ↓
Synthesize: combine all answers into final comparison
```

```csharp
var decomposePrompt = $"""
    Break this complex question into 2-5 simpler search queries that can each be answered independently.
    Question: {userQuestion}
    Return as JSON array of strings.
    """;
var subQueries = await DecomposeAsync(decomposePrompt);
var subAnswers = await Task.WhenAll(subQueries.Select(q => AnswerSubQueryAsync(q)));
var finalAnswer = await SynthesizeAsync(userQuestion, subAnswers);
```

---

### 3. Step-Back Prompting

Ask a more general version of the question first to retrieve foundational context:

```
Specific: "Why was invoice JMF-ATL-2026-001 rejected?"
Step-back: "What are the general reasons for invoice rejection at JM Family?"
        ↓
Retrieve general policy context (step-back query)
Retrieve specific invoice context (original query)
        ↓
Combine both in prompt — LLM reasons about specific case with general policy context
```

**When to use:** When the answer requires understanding general rules/policies to interpret specific cases.

---

### 4. Agentic RAG

RAG orchestrated by an AI agent that decides which tools to call:

```
User: "Pull the top 5 dealers by revenue this month and check if any have pending invoices"

Agent plan:
  Step 1: [search_tool] "dealer revenue January 2026" → top 5 dealers
  Step 2: [search_tool] "pending invoices {dealer1}" → check dealer 1
  Step 3: [search_tool] "pending invoices {dealer2}" → check dealer 2
  ...
  Step 6: [synthesize] combine results → final answer
```

The agent (Semantic Kernel or LangChain) decides the steps, calls tools in sequence, handles the results — you don't pre-define the flow.

This is covered in depth in Module 14 (AI Orchestration — Semantic Kernel, LangChain, Agents).

---

### 5. RAG Evaluation — How to Measure Quality

You can't improve what you can't measure. Core RAG metrics:

| Metric | What it measures | How to measure |
|--------|----------------|---------------|
| **Faithfulness** | Does the answer stay within the retrieved context? | LLM-as-judge: "Is this answer grounded in these sources?" |
| **Answer Relevance** | Does the answer actually address the question? | LLM-as-judge: "Does this answer the question?" |
| **Context Precision** | Are the retrieved chunks relevant? | % of chunks that were actually used in the answer |
| **Context Recall** | Did retrieval find all relevant chunks? | Hard to measure — needs ground truth dataset |
| **Latency** | End-to-end response time | Application monitoring (App Insights) |
| **Cost per query** | Token usage × price | Azure OpenAI metrics |

**Azure tooling:**
- **Azure AI Foundry** (formerly Azure AI Studio) has built-in RAG evaluation flows
- **Azure Monitor / App Insights** for latency and error tracking
- Custom evaluation: build a test set of Q&A pairs, run your pipeline, score with LLM-as-judge

---

### 6. Production RAG Architecture — JM Family Full Picture

```
┌──────────────────────────────────────────────────────────────────────┐
│                         INGESTION PIPELINE                           │
│                                                                      │
│  SharePoint / Blob Storage                                           │
│       ↓ event trigger (Event Grid / Blob trigger)                   │
│  Azure Function                                                      │
│       ↓                                                              │
│  Document Intelligence (prebuilt-layout or custom-invoice)          │
│       ↓ structured JSON + clean text                                 │
│  Chunking Service (recursive, section-aware, table-preserving)      │
│       ↓ chunks with metadata                                         │
│  Azure OpenAI Embeddings (text-embedding-3-small)                   │
│       ↓ float[1536] per chunk                                        │
│  Azure AI Search (Push API) ←→ Cosmos DB (full doc metadata)        │
│                                                                      │
├──────────────────────────────────────────────────────────────────────┤
│                         QUERY PIPELINE                               │
│                                                                      │
│  User question (Teams / Web App)                                     │
│       ↓                                                              │
│  Query Rewriting (if multi-turn — resolve pronouns/references)      │
│       ↓                                                              │
│  Self-Querying (LLM extracts OData filter from natural language)    │
│       ↓                                                              │
│  Azure OpenAI Embeddings → query vector                              │
│       ↓                                                              │
│  Azure AI Search — Hybrid + Semantic (filter + keyword + vector)    │
│       ↓ top 5 chunks with scores                                     │
│  Confidence Gate (score < 0.65 → corrective action)                 │
│       ↓                                                              │
│  Prompt Construction (system + context chunks + question)           │
│       ↓                                                              │
│  Azure OpenAI Chat (GPT-4o, temp=0, citations required)             │
│       ↓                                                              │
│  Response + Citations → User                                         │
│       ↓                                                              │
│  Logging → App Insights (latency, tokens, scores, feedback)         │
└──────────────────────────────────────────────────────────────────────┘
```

---

## Module 13 — Self-Test Questions

**Q1.** What is the difference between the indexing pipeline and the query pipeline in RAG? Why are they separate?

> **A:** The indexing pipeline runs offline — it processes documents, chunks them, generates embeddings, and stores everything in the vector index. It runs once per document. The query pipeline runs online (real-time) for every user question — it embeds the question, retrieves relevant chunks, builds a prompt, and calls the LLM. They're separate because indexing is a one-time cost per document while querying happens in real time and must be fast.

---

**Q2.** A user asks "Why was this invoice rejected?" and retrieval returns chunks about general invoice policies, not the specific invoice. Which retrieval strategy would help and why?

> **A:** **Step-Back Prompting** — first retrieve general invoice rejection policies (step-back query), then retrieve chunks about the specific invoice (original query), then combine both in the prompt. The LLM can then apply the general policy rules to the specific case. Also consider **Multi-Query Retrieval** — generate "invoice rejection policy" and "invoice [ID] details" as separate queries to retrieve both types of content.

---

**Q3.** You have a 50-page contract. Fixed-size chunking at 512 tokens keeps splitting tables across chunks. What strategy fixes this?

> **A:** **Document-specific chunking** — detect tables using Document Intelligence's `Tables` collection and serialize each table as a single chunk, regardless of its token count. Use section-aware chunking (split on `SectionHeading` paragraph roles) for the prose sections. Never apply fixed-size splitting to structured regions of the document.

---

**Q4.** What is Parent-Child chunking and what problem does it solve?

> **A:** Store two levels — large parent chunks (1024 tokens) and small child chunks (256 tokens). Index only the child chunks (small = specific embeddings = better retrieval). When a child chunk matches a query, return its parent chunk to the LLM (large = full context = better answers). It solves the precision-recall trade-off: small chunks are precise to retrieve, large chunks have enough context to answer well.

---

**Q5.** Your RAG app returns answers, but users complain they're sometimes wrong. How do you add grounding verification?

> **A:** Several layers: (1) **Confidence gate** — if average search score < threshold, don't generate, return "insufficient evidence." (2) **Citation requirement** — prompt the LLM to cite [Source N] for every claim; claims without citations are likely hallucinated. (3) **Azure Content Safety groundedness detection** — runs the answer against the retrieved chunks and flags ungrounded claims. (4) **Temperature = 0** — reduce creative generation. (5) **Explicit prompt instruction** — "If the answer is not in the provided sources, say so."

---

**Q6.** What is the difference between On Your Data and custom RAG? When would you use On Your Data?

> **A:** On Your Data is a managed RAG service built into Azure OpenAI — you configure data sources in the API call and Azure handles retrieval and prompt construction internally. Custom RAG gives you full control over every step (chunking, retrieval strategy, prompt format, re-ranking). Use On Your Data for quick PoCs, demos, or simple use cases where you don't need to tune retrieval quality. Move to custom RAG for production apps where you need quality control, advanced retrieval strategies (HyDE, MMR, query decomposition), or observability into the retrieval pipeline.

---

## Memory Hooks

- **"RAG = embed + retrieve + augment + generate — in that order"** — two pipelines: offline indexing, online querying
- **"Chunking is the most impactful decision"** — wrong chunking breaks retrieval regardless of LLM quality
- **"512 tokens, 50 overlap, recursive split"** — the safe default chunking strategy
- **"Parent-child: retrieve small, return large"** — precision on retrieval, context on generation
- **"Temperature = 0 for RAG"** — factual answers from documents, not creative generation
- **"Cite or flag"** — every claim needs a source; unverifiable claims should be flagged
- **"On Your Data = fast start, custom RAG = production quality"**
- **"HyDE: embed the answer, not the question"** — when question/document language don't match
- **"MMR: relevant AND diverse"** — avoid retrieving 5 chunks that all say the same thing
- **"Your JM Family pipeline IS Module 13"** — chunking → embedding → hybrid search → GPT-4o → citations

---

---

## 2026 Updates

| Topic | Update |
|---|---|
| **GraphRAG GA** | Microsoft GraphRAG now GA (github.com/microsoft/graphrag). Builds a knowledge graph from documents — extracts entities and relationships, enables multi-hop reasoning ("Which dealers share the same fleet manager who also manages accounts with late payments?"). More expensive to index but dramatically better for relationship queries |
| **Agentic RAG** | RAG where an AI Agent decides what to search, when to search, and which index to use. AI Foundry Agents support this natively — add AI Search as a "Knowledge" source to an Agent, and it handles retrieval automatically |
| **Azure AI Foundry RAG wizard** | "Import and vectorize" + Agent builder in AI Foundry lets you build a RAG app with zero code. Good for prototyping; production still needs custom code for filtering, citation, routing |
| **Multimodal RAG** | Retrieve and reason over images + text in the same index. GPT-4o vision + AI Search with image embeddings (Azure AI Vision embedding model). JMA use: search vehicle inspection photos by damage type |
| **Semantic caching** | Azure API Management (APIM) now has built-in semantic cache for Azure OpenAI — caches responses for semantically similar queries (not just exact match). Can reduce LLM calls by 20-40% for FAQ-style workloads |

---

## Interactive Learning Ideas

### Exercise 1 — Chunk Size Experiment (20 min)
Take a 10-page JMA policy document. Chunk it at 256, 512, and 1024 tokens. Embed each set. Ask the same question against all three chunk sizes. Which chunk size gives the best answer? Which gives too little context? Which gives too much noise?

### Exercise 2 — HyDE Implementation (20 min)
Implement HyDE (Hypothetical Document Embeddings) in C#:
1. Take user query: "What is JM Family's vehicle return policy?"
2. Ask GPT-4o to generate a hypothetical answer (without RAG context)
3. Embed the hypothetical answer
4. Use that embedding to search AI Search (instead of embedding the original query)
5. Compare retrieval results vs standard query embedding
Does HyDE improve recall on your test queries?

### Exercise 3 — Citation Chain Implementation
Build a RAG response function that:
- Returns the answer text
- Lists each source document used
- Includes the exact excerpt that supports each claim
- Adds a confidence indicator based on retrieval score
- Returns "I don't have enough information" if top-1 score < 0.75

### Exercise 4 — GraphRAG vs Standard RAG Comparison
Given this query: "Which dealers in the Southeast region have had more than 3 late deliveries AND are managed by the same regional manager?"
- How would standard RAG handle this? (chunk retrieval — would it work?)
- How would GraphRAG handle this? (entity graph traversal)
- What would the knowledge graph look like for this domain?

### Exercise 5 — RAG Pipeline Health Check
For JMA's production RAG pipeline, design a weekly health check:
- What metric tells you retrieval is working? (top-K recall on test queries)
- What metric tells you generation is grounded? (groundedness score distribution)
- What metric tells you users are satisfied? (thumbs up/down, follow-up questions)
- What triggers a reindex? (new documents, schema change, embedding model update)

---

*Next: Module 14 — AI Orchestration Frameworks*
*Updated: 2026-06-30*

---

## Interview Gap: Advanced Chunking Strategies

### Why Basic Chunking Is Not Enough

Our earlier chunking section covers fixed-size, paragraph, and overlapping chunking. These work for simple cases. But interviews — and production — require two more strategies that solve real retrieval failures.

```
THE PROBLEM WITH BASIC CHUNKING:

Fixed chunk (500 tokens):
  Chunk 1: "The RAV4 Hybrid XLE has a 2.5L engine..."
  Chunk 2: "...combined fuel economy of 41 MPG city."  ← answer split across chunks

User asks: "What engine does the RAV4 Hybrid XLE have and what MPG does it get?"
Search returns Chunk 1 OR Chunk 2, not both
Answer is incomplete — retrieval failure, not model failure
```

---

### Strategy 5 — Parent-Child Chunking

Index small chunks for precision retrieval, but return the large parent chunk to the LLM for full context.

```
PARENT-CHILD STRUCTURE:

Parent chunk (1500 tokens) — stored but NOT indexed for search:
  "Section 3: RAV4 Hybrid XLE Specifications
   Engine: 2.5L Dynamic Force 4-cylinder hybrid
   System Output: 219 horsepower
   Fuel Economy: 41 MPG city / 38 MPG highway
   Battery: Nickel-metal hydride
   Drive: Electronic On-Demand AWD
   Cargo: 37.6 cubic feet behind rear seats..."

Child chunks (150 tokens each) — indexed for search:
  Child A: "RAV4 Hybrid XLE engine: 2.5L Dynamic Force, 219hp"
  Child B: "RAV4 Hybrid XLE fuel economy: 41 MPG city, 38 MPG highway"
  Child C: "RAV4 Hybrid XLE cargo: 37.6 cubic feet, AWD standard"

RETRIEVAL FLOW:
  User: "What is the RAV4 Hybrid's fuel economy?"
  → Vector search finds Child B (small, precise match)
  → System retrieves Child B's PARENT (full spec section)
  → LLM gets full context: engine + MPG + battery + cargo
  → Complete, accurate answer
```

**C# implementation with Azure AI Search:**

```csharp
public class ParentChildChunker
{
    public List<ChunkPair> CreateParentChildChunks(
        string documentText,
        int parentSize = 1500,
        int childSize = 150,
        int childOverlap = 20)
    {
        var pairs = new List<ChunkPair>();
        var parentChunks = SplitIntoChunks(documentText, parentSize);

        foreach (var (parent, parentIndex) in parentChunks.Select((p, i) => (p, i)))
        {
            var children = SplitIntoChunks(parent, childSize, overlap: childOverlap);
            foreach (var (child, childIndex) in children.Select((c, i) => (c, i)))
            {
                pairs.Add(new ChunkPair
                {
                    ParentId = $"parent-{parentIndex}",
                    ParentContent = parent,       // sent to LLM
                    ChildId = $"child-{parentIndex}-{childIndex}",
                    ChildContent = child,         // indexed for search
                });
            }
        }
        return pairs;
    }
}

// Index: only child chunks get vectors
// Store: both child AND parent content in the index document
var indexDoc = new SearchDocument
{
    ["id"] = chunk.ChildId,
    ["childContent"] = chunk.ChildContent,
    ["parentContent"] = chunk.ParentContent,   // stored, not vectorized
    ["contentVector"] = await EmbedAsync(chunk.ChildContent)  // child embedded
};

// At retrieval time: return parentContent to LLM, not childContent
var results = await searchClient.SearchAsync<SearchDocument>(queryVector);
var contextForLLM = results.Select(r => r.Document["parentContent"]).ToList();
```

**When to use:** Document sets with hierarchical structure — specs, legal contracts, policy documents, technical manuals. Any time a precise answer requires surrounding context.

---

### Strategy 6 — Late Chunking

Embed the full document first (preserving cross-sentence context), then chunk the embeddings.

```
WHY LATE CHUNKING EXISTS:

TRADITIONAL (early chunking):
  Split document → embed each chunk independently
  Problem: each chunk loses context from surrounding text

  Chunk: "It has a towing capacity of 3,500 lbs"
  Embedding computed on THIS TEXT ALONE
  "It" has no referent — model doesn't know "it" = RAV4 Hybrid
  Retrieval suffers when user asks about RAV4 towing

LATE CHUNKING:
  Embed full document (or large section) using long-context embedding model
  → every token's embedding is influenced by ALL surrounding text
  → "It" in the embedding now encodes "RAV4 Hybrid" from earlier context
  Then: split the token embeddings into chunk-sized groups
  → each chunk retains cross-document context
```

```python
# Late chunking requires a long-context embedding model (e.g., jina-embeddings-v3)
# Not yet supported natively in Azure OpenAI — use when working with Jina or Voyage AI

from transformers import AutoModel
import torch

model = AutoModel.from_pretrained("jinaai/jina-embeddings-v3", trust_remote_code=True)

# Step 1: Embed full document (late chunking flag)
full_doc = "RAV4 Hybrid XLE Specifications. Engine: 2.5L... It has 219hp... It gets 41 MPG..."
embeddings = model.encode([full_doc], late_chunking=True)

# Step 2: Embeddings are already chunk-aligned with cross-doc context
# Each position in embeddings[] corresponds to a sentence/paragraph
# with full document context baked in
```

**Azure context:** Azure OpenAI text-embedding-3-large does NOT support late chunking (uses early chunking). Late chunking is available via Jina AI or Voyage AI models accessible through Azure AI Foundry Model Catalog under third-party models.

**Interview answer:** "We use parent-child chunking in production with Azure AI Search — small children for precise retrieval, large parents for LLM context. Late chunking is on our roadmap once Azure OpenAI embedding models support it natively."

---

### Chunking Strategy Decision Table

| Strategy | Best For | Retrieval Quality | Complexity |
|---|---|---|---|
| Fixed size (500 tokens) | Quick prototypes | ⭐⭐ | Low |
| Paragraph / sentence | General documents | ⭐⭐⭐ | Low |
| Overlapping | Boundary-sensitive content | ⭐⭐⭐ | Low |
| **Parent-Child** | **Hierarchical docs, specs, legal** | **⭐⭐⭐⭐⭐** | **Medium** |
| **Late Chunking** | **Long narrative docs, cross-referencing** | **⭐⭐⭐⭐⭐** | **High** |
| Semantic (topic-based) | Mixed-topic documents | ⭐⭐⭐⭐ | High |

---
---

## Re-ranking and Top-K — Why Fewer Chunks Needs a Second Pass (added 2026-08-02)

**Top-K = simply "how many results you keep."** Top-3 = keep the best 3 chunks. Top-10 = keep the
best 10. K is just a number you choose — nothing more complex than that.

**Re-ranking = a second, more precise scoring pass that re-orders retrieved chunks, so a smaller
top-K still contains the truly best matches** — instead of trusting the first search's rough order.

### Worked example

**Question:** "What is the penalty for late invoices?"

**Step 1 — first-pass search returns 10 chunks, roughly ranked (fast, approximate):**
```
1. Chunk about dealer territory codes         ← wrong, ranked high by mistake
2. Chunk about parts payment terms
3. Chunk about the ACTUAL late invoice penalty  ← this is the one we need!
4. Chunk about warranty claims
... (6 more, irrelevant)
```
Fast vector/keyword search isn't perfect — the real answer landed at position 3, not 1.

**Step 2 — without re-ranking, top-K = 3:** you'd get chunks #1, #2, #3 — the real answer barely made
it in by luck. If it had ranked #4 instead, top-3 would have **missed it entirely**.

**Step 3 — re-ranking fixes the order first**, re-scoring all 10 candidates more carefully (looking at
the query and each chunk *together*, not just comparing separate embeddings):
```
1. Chunk about the ACTUAL late invoice penalty   ← now correctly #1
2. Chunk about warranty claims
3. Chunk about parts payment terms
```

**Step 4 — now take top-K = 3:** the real answer is guaranteed to be included — not luck, because the
order was fixed before cutting down to K.

### Why two passes, not one

- **First pass** (vector/keyword search) — fast, scans the *entire* index, but only approximately
  ranked
- **Second pass** (re-ranker) — slower per item, but only runs on the small first-pass candidate set
  (e.g. 20), producing a far more accurate order

This is the same **"Semantic reranking — ✅ Built-in semantic ranker"** row from the Azure AI Search
vector-DB comparison table (§ "Why would you choose Azure AI Search...") — Azure AI Search has this
re-ranking step natively built in, no separate cross-encoder to stand up yourself.

**One-sentence summary:** reducing top-K alone risks accidentally leaving out the real answer if the
first-pass search ranked it too low; re-ranking first means a smaller top-K is safe, because the order
is trustworthy before you cut it down.
