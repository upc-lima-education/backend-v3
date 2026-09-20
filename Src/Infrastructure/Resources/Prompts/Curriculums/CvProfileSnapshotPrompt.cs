using System.Text.Json;
using Backend.Src.Domain.Contracts.Curriculums;

namespace Backend.Src.Infrastructure.Resources.Prompts.Curriculums;

public static class CvProfileSnapshotPrompt
{
    public static string ToPrompt(CvProfileSnapshot snapshot)
    {
        var prompt = "==USER DATA (JSON)==\n";
        var jsonSnapshot = JsonSerializer.Serialize(snapshot);
        prompt += jsonSnapshot.ToString();
        return prompt;
    }
}