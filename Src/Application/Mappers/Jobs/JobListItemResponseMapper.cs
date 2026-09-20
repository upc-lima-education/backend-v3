using Backend.Src.Application.Dtos.Responses.Jobs;
using Backend.Src.Domain.Entities.Jobs;

namespace Backend.Src.Application.Mappers.Jobs;

public class JobListItemResponseMapper
{
    public static JobListItemResponse ToResponse(Job job)
    {
        var companyImageUrl = !string.IsNullOrWhiteSpace(job.Company?.Profile?.ProfilePicture)
            ? job.Company.Profile.ProfilePicture
            : (!string.IsNullOrWhiteSpace(job.ExternalCompanyImage) ? job.ExternalCompanyImage : null);

        return new JobListItemResponse(
            job.Id,
            job.Company?.CompanyName ?? job.ExternalCompanyName ?? "",
            companyImageUrl,
            job.Title,
            job.Ubigeo,
            job.JobType.ToString(),
            job.JobStatus.ToString(),
            job.OriginPage.ToString(),
            job.ClosesAt
        );
    }
}
