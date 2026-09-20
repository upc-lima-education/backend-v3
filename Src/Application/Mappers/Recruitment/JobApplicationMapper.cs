using Backend.Src.Application.Dtos.Responses.Recruitment;
using Backend.Src.Domain.Entities.Recruitment;

namespace Backend.Src.Application.Mappers.Recruitment;

public static class JobApplicationMapper
{
    public static JobApplicationResponse ToResponse(JobApplication jobApplication, CandidateApplicationSummaryData? candidate = null)
    {
        return new JobApplicationResponse(
            jobApplication.Id,
            jobApplication.CandidateId,
            jobApplication.Status.ToString(),
            jobApplication.CreatedAt,
            candidate
        );
    }
}