using Backend.Src.Application.Dtos.Data.Jobs;
using Backend.Src.Domain.ValueObjects.Jobs;

namespace Backend.Src.Application.Dtos.Responses.Jobs;

public record JobResponse(
    //Id
    Guid Id,
    //Company
    JobCompanyData? Company,
    //Details
    string Title,
    string Description,
    string JobType,
    string WorkHours,
    //Requirements
    List<string> Skills,
    string Experience,
    string EducationLevel,
    //Location
    JobLocationData? Location,
    //Payment
    JobPaymentData? Payment,
    //Traceability
    DateTime OpensAt,
    DateTime? ClosesAt,
    JobStatus JobStatus,
    string OriginPage,
    int Views,
    string? SourceUrl,
    string? ApplyUrl
);