using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Skills;

public class InvalidSkillException(string message)
    : DomainException(message)
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Title => "Incorrect format for skill";
}