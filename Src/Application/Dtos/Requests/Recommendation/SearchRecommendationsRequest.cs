using System.Text.Json.Serialization;

namespace Backend.Src.Application.Dtos.Requests.Recommendation;

public sealed record SearchRecommendationsRequest(
    [property: JsonPropertyName("title_search")] string Query,
    [property: JsonPropertyName("ubigeo")] string? Ubigeo = null,
    [property: JsonPropertyName("education_level")] string? EducationLevel = null,
    [property: JsonPropertyName("experience")] string? Experience = null,
    [property: JsonPropertyName("min_salary")] decimal? MinSalary = null,
    [property: JsonPropertyName("work_hours")] string? WorkHours = null,
    [property: JsonPropertyName("job_type")] string? JobType = null,
    [property: JsonPropertyName("page")] int Page = 1,
    [property: JsonPropertyName("page_size")] int PageSize = 10);
