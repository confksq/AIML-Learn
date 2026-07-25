// GAP TOPIC: Clinical Guideline Chunking
// HEALTHCARE EQUIVALENT OF: ChunkingStrategy.cs (JMA)
// Chunks clinical coverage policies, formulary PDFs, clinical guidelines
// KEY DIFFERENCES:
// - Paragraph chunking preferred (same as JMA) — clinical rules live in paragraphs
// - Section headers are important metadata — ICD-10 codes, drug names in headers
// - Parent-child for clinical guidelines (precise retrieval + full clinical context)

namespace VitalCare.DocumentPipeline;

public class ClinicalGuidelineChunker
{
    private const int ChildChunkTokens  = 250;
    private const int OverlapTokens     = 40;

    public List<ClinicalChunk> ChunkDocument(string documentId, string text, string documentType)
    {
        return documentType switch
        {
            "formulary"          => ChunkByParagraph(documentId, text),   // drug entries = one paragraph each
            "clinical-guideline" => ChunkParentChild(documentId, text),   // guidelines = need full section context
            "coverage-policy"    => ChunkByParagraph(documentId, text),   // policies = paragraph = one rule
            _                    => ChunkByParagraph(documentId, text)
        };
    }

    // INTERVIEW: "Why paragraph for clinical guidelines?"
    // "Clinical coverage criteria live in paragraphs — each paragraph is one criterion.
    //  'Patient must have failed methotrexate therapy for at least 3 months' is one paragraph.
    //  Fixed-size chunks risk splitting that sentence. Losing 'for at least 3 months'
    //  could make the agent approve when it should pend. In clinical AI, chunking
    //  quality is a patient safety concern, not just a retrieval quality concern."
    private List<ClinicalChunk> ChunkByParagraph(string documentId, string text)
    {
        var paragraphs = text.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        var chunks = new List<ClinicalChunk>();

        for (int i = 0; i < paragraphs.Length; i++)
        {
            var para = paragraphs[i].Trim();
            if (para.Length < 30) continue;

            chunks.Add(new ClinicalChunk
            {
                ChunkId      = $"{documentId}::para::{i}",
                DocumentId   = documentId,
                Text         = para,
                Strategy     = "paragraph",
                // INTERVIEW: Extract clinical metadata from chunk — used as search filters
                ContainsICD10 = para.Contains("ICD-10") || System.Text.RegularExpressions.Regex.IsMatch(para, @"[A-Z]\d{2}"),
                ContainsNDC   = para.Contains("NDC") || para.Contains("drug code"),
                TokenCount    = para.Split(' ').Length
            });
        }
        return chunks;
    }

    // Parent-child: child chunk (200 tokens) used for HNSW retrieval precision
    //               parent chunk (full section) injected into prompt for clinical context
    private List<ClinicalChunk> ChunkParentChild(string documentId, string text)
    {
        var sections = text.Split("##", StringSplitOptions.RemoveEmptyEntries);
        var chunks   = new List<ClinicalChunk>();

        for (int si = 0; si < sections.Length; si++)
        {
            var parentText = sections[si].Trim();
            var parentId   = $"{documentId}::parent::{si}";
            var children   = parentText.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

            for (int ci = 0; ci < children.Length; ci++)
            {
                var child = children[ci].Trim();
                if (child.Length < 30) continue;

                chunks.Add(new ClinicalChunk
                {
                    ChunkId       = $"{documentId}::child::{si}::{ci}",
                    DocumentId    = documentId,
                    ParentId      = parentId,
                    Text          = child,      // embedded + searched
                    ParentText    = parentText, // injected into prompt
                    Strategy      = "parent-child",
                    ContainsICD10 = child.Contains("ICD-10"),
                    ContainsNDC   = child.Contains("NDC"),
                    TokenCount    = child.Split(' ').Length
                });
            }
        }
        return chunks;
    }
}

public record ClinicalChunk
{
    public string ChunkId      { get; init; } = string.Empty;
    public string DocumentId   { get; init; } = string.Empty;
    public string ParentId     { get; init; } = string.Empty;
    public string Text         { get; init; } = string.Empty;
    public string ParentText   { get; init; } = string.Empty;
    public string Strategy     { get; init; } = string.Empty;
    public bool   ContainsICD10 { get; init; }
    public bool   ContainsNDC  { get; init; }
    public int    TokenCount   { get; init; }
}
