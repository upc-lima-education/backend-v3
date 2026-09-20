using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Recruitment;

public sealed class JobApplicationNotFoundException(Guid jobApplicationId)
    : DomainException($"Job Application '{jobApplicationId}' was not found.")
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "Job Application not found";
}