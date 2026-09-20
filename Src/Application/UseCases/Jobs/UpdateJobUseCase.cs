using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Application.Dtos.Requests.Jobs;
using Backend.Src.Application.Dtos.Responses.Jobs;
using FluentValidation;
using Backend.Src.Application.Mappers.Jobs;
using Backend.Src.Domain.Exceptions.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Application.Resolvers.Skills;

namespace Backend.Src.Application.UseCases.Jobs;

public class UpdateJobUseCase(
    IJobRepository jobRepository,
    IProfileRepository profileRepository,
    SkillResolver skillResolver,
    IValidator<UpdateJobRequest> validator
)
{
    public async Task<JobResponse> ExecuteAsync(UpdateJobRequest request, Guid jobId, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CompanyProfile is null) throw new CompanyProfileRequiredException();
        
        var job = await jobRepository.GetByIdForUpdateAsync(jobId)
            ?? throw new JobNotFoundException(jobId);
        
        if(job.CompanyId != profile.Id) throw new JobAccessDeniedException(jobId);

        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var skills = await skillResolver.ResolveAsync(request.Skills);

        job.Update(
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
            request.Location.Ubigeo,
            request.Location.Address,
            //Payment
            request.Payment.MinSalary,
            request.Payment.MaxSalary,
            request.Payment.Currency,
            request.Payment.SalaryPeriod,
            request.Payment.CompensationType,
            //Traceability
            request.OpensAt,
            request.ClosesAt,
            request.ApplyUrl
        );

        await jobRepository.UpdateAsync(job);
        var response = JobMapper.ToResponse(job);
        return response;
    }
}
