using Backend.Src.Domain.Contracts.Common;
using Backend.Src.Domain.Ports.Common;

namespace Backend.Src.Infrastructure.Adapters.Common;

public class HtmlTemplateAdapter : IHtmlTemplatePort
{
    private readonly Dictionary<HtmlTemplate, string> templates = new()
    {
        [HtmlTemplate.PasswordReset] = Path.Combine("Auth", "PasswordResetEmailTemplate.html"),
        [HtmlTemplate.CandidateSelected] = Path.Combine("Recruitment", "CandidateSelected.html"),
        [HtmlTemplate.CandidateRejected] = Path.Combine("Recruitment", "CandidateRejected.html")
    };

    public string Render(HtmlTemplate template, params (string Key, string Value)[] values)
    {
        if (!templates.TryGetValue(template, out var fileName))
            throw new InvalidOperationException($"Template '{template}' is not configured.");
        var html = ReadTemplate(fileName);
        foreach (var (key, value) in values)
        {
            html = html.Replace($"{{{{{key}}}}}", value);
        }
        return html;
    }

    private static string ReadTemplate(string fileName)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Resources",
            "Html",
            fileName
        );
        return File.ReadAllText(path);
    }
}