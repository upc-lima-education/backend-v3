using System.Text.Json;
using Backend.Src.Domain.Contracts.Curriculums;

namespace Backend.Src.Infrastructure.Resources.Prompts.Curriculums;

public static class CvJobPostingPrompt
{
    public static string ToPrompt(JobSnapshot jobSnapshot)
    {
        var prompt = "==DATOS DEL TRABAJO==\n";
        var jsonSnapshot = JsonSerializer.Serialize(jobSnapshot);
        prompt += jsonSnapshot.ToString();
        return prompt;
    }
}