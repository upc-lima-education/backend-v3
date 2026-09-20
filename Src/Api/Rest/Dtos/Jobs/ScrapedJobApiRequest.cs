using System.Text.Json.Serialization;
using Backend.Src.Domain.ValueObjects.Jobs;

namespace Backend.Src.Api.Rest.Dtos.Jobs;

public record ScrapedJobApiRequest(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("job_type")] JobType JobType,
    [property: JsonPropertyName("work_hours")] WorkHours WorkHours,
    [property: JsonPropertyName("skills")] List<string> Skills,
    [property: JsonPropertyName("experience")] Experience Experience,
    [property: JsonPropertyName("education_level")] EducationLevel EducationLevel,
    [property: JsonPropertyName("ubigeo")] string? Ubigeo,
    [property: JsonPropertyName("address")] string? Address,
    [property: JsonPropertyName("min_salary")] decimal? MinSalary,
    [property: JsonPropertyName("max_salary")] decimal? MaxSalary,
    [property: JsonPropertyName("currency")] Currency? Currency,
    [property: JsonPropertyName("salary_period")] SalaryPeriod? SalaryPeriod,
    [property: JsonPropertyName("compensation_type")] CompensationType? CompensationType,
    [property: JsonPropertyName("opens_at")] DateTime? OpensAt,
    [property: JsonPropertyName("closes_at")] DateTime? ClosesAt,
    [property: JsonPropertyName("origin_page")] OriginPage OriginPage,
    [property: JsonPropertyName("source_url")] string SourceUrl,
    [property: JsonPropertyName("external_company_name")] string? ExternalCompanyName = null,
    [property: JsonPropertyName("external_company_image")] string? ExternalCompanyImage = null
);
