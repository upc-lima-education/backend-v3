using Backend.Src.Application.Dtos.Requests.Auth;
using FluentValidation;

namespace Backend.Src.Application.Validators.Auth;

internal sealed class UpdateContactValidator : AbstractValidator<UpdateContactRequest>
{
    public UpdateContactValidator()
    {
        RuleFor(x => x.PhoneNumber).MaximumLength(30).Matches(@"^\+?[0-9 ()-]*$").When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}
