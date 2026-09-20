using System.Globalization;
using System.Net;
using System.Text;
using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.Ports.Curriculums;
using Backend.Src.Domain.ValueObjects.Curriculums;

namespace Backend.Src.Infrastructure.Adapters.Curriculums;

public class CvHtmlRendererAdapter : ICvHtmlRendererPort
{
    public string Render(CvStructuredContent cv)
    {
        var content = new StringBuilder();

        AppendHeader(content, cv.Header);
        AppendTextSection(content, cv.Summary);
        AppendExperienceSection(content, cv.Experience);
        AppendEducationSection(content, cv.Education);
        AppendTextSection(content, cv.Skills);
        AppendTextSection(content, cv.Languages);
        AppendTextSection(content, cv.Certifications);
        AppendTextSection(content, cv.Projects);
        AppendTextSection(content, cv.Awards);
        AppendCustomSections(content, cv.CustomSections);

        var template = ReadTemplate("CvTemplate.html");

        return template.Replace(
            "{{CONTENT}}",
            content.ToString()
        );
    }

    private static void AppendHeader(StringBuilder html, CvHeader header)
    {
        var template = ReadTemplate("CvHeader.html");
        var result = template
            .Replace("{{FULL_NAME}}", Escape(header.FullName))
            .Replace("{{HEADLINE}}", Escape(header.Headline))
            .Replace("{{EMAIL}}", header.Email)
            .Replace("{{PHONE}}", header.Phone)
            .Replace("{{LOCATION}}", header.Location);
        html.Append(result);
    }

    private static void AppendTextSection(StringBuilder html, CvTextSection? section)
    {
        if (section is null) return;
        var template = ReadTemplate("CvTextSection.html");
        var result = template
            .Replace("{{TITLE}}", Escape(section.Title))
            .Replace("{{DESCRIPTION}}", FormatText(section.Description));
        html.Append(result);
    }

    private static void AppendExperienceSection(StringBuilder html, CvExperienceSection? section)
    {
        if (section is null) return;
        var items = new StringBuilder();
        foreach (var item in section.Items)
            items.Append(RenderExperienceItem(item));
        var template = ReadTemplate("CvExperienceSection.html");
        var result = template
            .Replace("{{TITLE}}", "Experiencia Laboral")
            .Replace("{{ITEMS}}", items.ToString());
        html.Append(result);
    }

    private static string RenderExperienceItem(CvExperienceItem item)
    {
        var template = ReadTemplate("CvExperienceItem.html");
        var date = $"{FormatDate(item.StartDate)} - {FormatDate(item.EndDate)}";
        return template
            .Replace("{{POSITION}}", Escape(item.Position))
            .Replace("{{EMPLOYER}}", Escape(item.Company))
            .Replace("{{DATE}}", date)
            .Replace("{{DESCRIPTION}}", FormatText(item.Description));
    }

    private static void AppendEducationSection(StringBuilder html, CvEducationSection? section)
    {
        if (section is null) return;
        var items = new StringBuilder();

        foreach (var item in section.Items)
            items.Append(RenderEducationItem(item));
        var template = ReadTemplate("CvEducationSection.html");
        var result = template
            .Replace("{{TITLE}}", "Educación")
            .Replace("{{ITEMS}}", items.ToString());

        html.Append(result);
    }

    private static string RenderEducationItem(CvEducationItem item)
    {
        var template = ReadTemplate("CvEducationItem.html");
        var date = $"{FormatDate(item.StartDate)} - {FormatDate(item.EndDate)}";
        return template
            .Replace("{{STUDY}}", Escape(item.FieldOfStudy))
            .Replace("{{INSTITUTION}}", Escape(item.Institution))
            .Replace("{{DATE}}", date)
            .Replace("{{ACADEMIC_LEVEL}}", Escape(item.Degree));
    }

    private static void AppendCustomSections(StringBuilder html, IEnumerable<CvCustomSection> sections)
    {
        foreach (var section in sections.OrderBy(x => x.Order))
            html.Append(RenderCustomSection(section));
    }

    private static string RenderCustomSection(CvCustomSection section)
    {
        var template = ReadTemplate("CvCustomSection.html");
        return template
            .Replace("{{TITLE}}", Escape(section.Title))
            .Replace("{{DESCRIPTION}}", FormatText(section.Description));
    }

    private static string ReadTemplate(string template)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Html", "Curriculums", template);
        return File.ReadAllText(path);
    }

    private static string Escape(string? value)
    {
        return WebUtility.HtmlEncode(value ?? string.Empty);
    }

    private static string FormatText(string? value)
    {
        return WebUtility.HtmlEncode(value ?? string.Empty)
            .Replace("\r\n", "\n")
            .Replace("\n", "<br>");
    }

    private static string FormatDate(DateOnly? date)
    {
        return date?.ToString("MMMM yyyy", new CultureInfo("es-ES")) ?? "Actualidad";
    }
}