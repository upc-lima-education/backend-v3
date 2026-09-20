using Backend.Src.Api.Rest.Dtos.Jobs;
using Backend.Src.Application.Dtos.Requests.Jobs;

namespace Backend.Src.Api.Rest.Mappers.Jobs;

public static class SyncJobMapper
{
    public static ScrapedJobRequest ToApplicationRequest(ScrapedJobApiRequest request)
    {
        return new ScrapedJobRequest(
            request.Title,
            request.Description,
            request.JobType,
            request.WorkHours,
            request.Skills,
            request.Experience,
            request.EducationLevel,
            request.Ubigeo,
            request.Address,
            request.MinSalary,
            request.MaxSalary,
            request.Currency,
            request.SalaryPeriod,
            request.CompensationType,
            request.OpensAt,
            request.ClosesAt,
            request.OriginPage,
            request.SourceUrl,
            request.ExternalCompanyName,
            request.ExternalCompanyImage
        );
    }
}
