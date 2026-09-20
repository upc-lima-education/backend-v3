using Backend.Src.Application.Dtos.Data.Jobs;
using Backend.Src.Domain.ValueObjects.Jobs;

namespace Backend.Src.Application.Dtos.Requests.Jobs;
public record CreateInternalJobRequest(
    //Details
    string Title,
    string Description,
    JobType JobType,
    WorkHours WorkHours,
    //Requirements
    List<string> Skills,
    Experience Experience,
    EducationLevel EducationLevel,
    //Location
    JobLocationData? Location,
    //Payment
    JobPaymentData? Payment,
    //Traceability
    DateTime OpensAt,
    DateTime ClosesAt,
    //External
    string? ApplyUrl
);