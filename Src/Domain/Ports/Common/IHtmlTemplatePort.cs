using Backend.Src.Domain.Contracts.Common;

namespace Backend.Src.Domain.Ports.Common;

public interface IHtmlTemplatePort
{
    string Render(HtmlTemplate template, params (string Key, string Value)[] values);
}