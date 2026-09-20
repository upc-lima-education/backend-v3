using Backend.Src.Application.Dtos.Data.Jobs;
using Backend.Src.Application.Dtos.Requests.Jobs;
using Backend.Src.Application.Dtos.Responses.Jobs;
using Backend.Src.Domain.Entities.Jobs;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Entities.Skills;
using Backend.Src.Domain.ValueObjects.Jobs;

namespace Backend.Src.Application.Mappers.Jobs;

public class JobMapper
{
    public static JobResponse ToResponse(Job job)
    {
        JobCompanyData? company = null;
        var companyImageUrl = !string.IsNullOrWhiteSpace(job.Company?.Profile?.ProfilePicture)
            ? job.Company.Profile.ProfilePicture
            : (!string.IsNullOrWhiteSpace(job.ExternalCompanyImage) ? job.ExternalCompanyImage : null);

        if (job.CompanyId is not null && job.Company is not null)
            company = new JobCompanyData(
                job.Company.ProfileId,
                job.Company.CompanyName,
                companyImageUrl,
                job.Company.IsVerified
            );
        else if (!string.IsNullOrWhiteSpace(job.ExternalCompanyName))
            company = new JobCompanyData(
                job.CompanyId ?? Guid.Empty,
                job.ExternalCompanyName,
                companyImageUrl,
                false
            );

        JobLocationData? location = null;
        if (job.Ubigeo is not null || job.Address is not null)
            location = new JobLocationData(
                job.Ubigeo,
                job.Address
            );

        JobPaymentData? payment = null;
        if (job.MinSalary is not null || job.MaxSalary is not null)
            payment = new JobPaymentData(
                job.MinSalary,
                job.MaxSalary,
                job.Currency!.Value,
                job.SalaryPeriod!.Value,
                job.CompensationType!.Value
            );

        return new JobResponse(
            job.Id,
            company,
            job.Title,
            job.Description,
            job.JobType.ToString(),
            job.WorkHours.ToString(),
            job.Skills.Select(s => s.Name).ToList(),
            job.Experience.ToString(),
            job.EducationLevel.ToString(),
            location,
            payment,
            job.OpensAt,
            job.ClosesAt,
            job.JobStatus,
            job.OriginPage.ToString(),
            job.Views,
            job.SourceUrl,
            job.ApplyUrl
        );
    }

    public static Job ToEntity(
        CreateInternalJobRequest request,
        Guid companyId,
        List<Skill> skills,
        string? companyName = null,
        string? companyImage = null
    )
    {
        return new Job(
            //Company
            companyId,
            //Details
            request.Title,
            request.Description,
            request.JobType,
            request.WorkHours,
            //Requirements
            skills,
            request.Experience,
            request.EducationLevel,
            //Location
            request.Location?.Ubigeo,
            request.Location?.Address,
            //Payment
            request.Payment?.MinSalary,
            request.Payment?.MaxSalary,
            request.Payment?.Currency,
            request.Payment?.SalaryPeriod,
            request.Payment?.CompensationType,
            //Traceability
            request.OpensAt,
            request.ClosesAt,
            OriginPage.Internal,
            companyName, //External Company Name 
            null, //Source Url
            request.ApplyUrl,
            companyImage //External Company Image
        );
    }
}
