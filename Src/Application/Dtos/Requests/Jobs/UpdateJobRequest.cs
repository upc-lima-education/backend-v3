using Backend.Src.Domain.ValueObjects.Jobs;
using Backend.Src.Domain.Entities.Skills;
using Backend.Src.Application.Dtos.Data.Jobs;

namespace Backend.Src.Application.Dtos.Requests.Jobs;
public record UpdateJobRequest(
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
    JobLocationData Location,
    //Payment
    JobPaymentData Payment,
    //Traceability
    DateTime OpensAt,
    DateTime ClosesAt,
    //Apply Url
    string? ApplyUrl
);
