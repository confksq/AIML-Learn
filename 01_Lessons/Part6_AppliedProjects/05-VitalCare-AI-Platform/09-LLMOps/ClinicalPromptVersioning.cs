// GAP TOPIC + MODULE 11: Clinical Prompt Versioning
// HEALTHCARE EQUIVALENT OF: PromptVersioning.cs (JMA)
// KEY DIFFERENCE: Clinical prompts require PHARMACIST sign-off before production
//                 Not just engineering review — a licensed clinician must approve

namespace VitalCare.LLMOps;

// INTERVIEW: "How does clinical prompt versioning differ from JMA?"
// "The CI pipeline is the same — prompts in Git, YAML files, eval on every PR.
//  But we add one gate that JMA doesn't have: pharmacist sign-off.
//  After the automated eval passes, the PR requires approval from a clinical
//  pharmacist before it can merge. This is because a prompt change could
//  inadvertently alter clinical decision thresholds — something only a licensed
//  clinician is qualified to evaluate. Engineering can check groundedness scores.
//  Only a pharmacist can check whether the decision logic is clinically safe."
public class ClinicalPromptVersionStore
{
    private readonly string _promptsBasePath;
    private readonly ILogger<ClinicalPromptVersionStore> _logger;

    public ClinicalPromptVersionStore(string promptsBasePath, ILogger<ClinicalPromptVersionStore> logger)
    { _promptsBasePath = promptsBasePath; _logger = logger; }

    public async Task<ClinicalPromptVersion> LoadAsync(string promptName, string version = "latest")
    {
        var filePath = version == "latest"
            ? Path.Combine(_promptsBasePath, $"{promptName}.json")
            : Path.Combine(_promptsBasePath, "versions", $"{promptName}.v{version}.json");

        _logger.LogInformation("[CLINICAL PROMPT] Loading {Name} v{Version}", promptName, version);
        var json = await File.ReadAllTextAsync(filePath);
        return System.Text.Json.JsonSerializer.Deserialize<ClinicalPromptVersion>(json)!;
    }

    public async Task<bool> ValidateBeforePromotionAsync(
        ClinicalPromptVersion candidate,
        List<ClinicalGoldenCase> goldenCases,
        double groundednessThreshold = 0.90)
    {
        _logger.LogInformation(
            "[CLINICAL PROMPT VALIDATE] {Name} v{Version} — testing against {Count} pharmacist-reviewed cases",
            candidate.Name, candidate.Version, goldenCases.Count);

        var simulatedGroundedness = 0.92;
        var passes = simulatedGroundedness >= groundednessThreshold
                  && candidate.PharmacistApproved;  // INTERVIEW: MUST have pharmacist approval

        if (!candidate.PharmacistApproved)
            _logger.LogCritical("[CLINICAL PROMPT] BLOCKED — pharmacist sign-off required before clinical prompt can deploy. Prompt: {Name}", candidate.Name);

        _logger.LogInformation("[CLINICAL PROMPT] {Name} v{Version}: {Result}",
            candidate.Name, candidate.Version, passes ? "PASS — ready for deployment" : "BLOCKED");

        return await Task.FromResult(passes);
    }
}

public record ClinicalPromptVersion
{
    public string   Name               { get; init; } = string.Empty;
    public string   Version            { get; init; } = string.Empty;
    public string   Content            { get; init; } = string.Empty;
    public DateTime CreatedAt          { get; init; }
    public string   Author             { get; init; } = string.Empty;
    public string   ChangeLog          { get; init; } = string.Empty;
    public string   ApprovedBy         { get; init; } = string.Empty;       // engineer
    public bool     PharmacistApproved { get; init; }                        // INTERVIEW: clinical gate
    public string   PharmacistName     { get; init; } = string.Empty;       // licensed pharmacist who approved
    public DateTime PharmacistApprovalDate { get; init; }
}
