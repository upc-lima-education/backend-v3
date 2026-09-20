using System.Text.Json;
using Backend.Src.Domain.Contracts.Curriculums;

namespace Backend.Src.Infrastructure.Resources.Prompts.Curriculums;

public static class CvDocumentPrompt
{
    public static string ToPrompt(CvStructuredContentSnapshot cv)
    {
        var prompt = "==CV DATA (JSON)==\n";
        var jsonCv = JsonSerializer.Serialize(cv);
        prompt += jsonCv.ToString();
        return prompt;
    }
}