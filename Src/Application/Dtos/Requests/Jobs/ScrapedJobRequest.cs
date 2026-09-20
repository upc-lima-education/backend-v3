using Backend.Src.Domain.ValueObjects.Jobs;

namespace Backend.Src.Application.Dtos.Requests.Jobs;

public record ScrapedJobRequest(
    string Title,
    string Description,
    JobType JobType,
    WorkHours WorkHours,
    List<string> Skills,
    Experience Experience,
    EducationLevel EducationLevel,
    string? Ubigeo,
    string? Address,
    decimal? MinSalary,
    decimal? MaxSalary,
    Currency? Currency,
    SalaryPeriod? SalaryPeriod,
    CompensationType? CompensationType,
    DateTime? OpensAt,
    DateTime? ClosesAt,
    OriginPage OriginPage,
    string SourceUrl,
    string? ExternalCompanyName,
    string? ExternalCompanyImage
);
