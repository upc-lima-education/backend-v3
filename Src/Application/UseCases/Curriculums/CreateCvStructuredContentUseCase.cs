using Backend.Src.Application.Dtos.Requests.Curriculums;
using Backend.Src.Application.Mappers.Curriculums;
using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.ValueObjects.Curriculums;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Curriculums;

public class CreateCvStructuredContentUseCase(
    ICvRepository cvRepository,
    IProfileRepository profileRepository,
    ICvStructuredContentRepository structuredRepository,
    IValidator<CreateCvStructuredContentRequest> validator
)
{
    public async Task<Guid> ExecuteAsync(CreateCvStructuredContentRequest request, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();
        
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
        //TODO: Handle IsCurrent in a safer way 
        var cv = new Cv(
                profile.Id, //Candidate Id
                request.Title,
                request.IsCurrent
        );
        var content = CvStructuredContentMapper.ToEntity(request, cv.Id);
        await structuredRepository.CreateAsync(content);
        try
        {
            await cvRepository.CreateAsync(cv);
            return cv.Id;
        }
        catch
        {
            await structuredRepository.DeleteAsync(content.Id);
            throw;
        }
    }
}