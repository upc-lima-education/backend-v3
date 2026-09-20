using Backend.Src.Application.Dtos.Requests.Profiles;
using FluentValidation;

namespace Backend.Src.Application.Validators.Profiles;

internal sealed class ValidateRucValidator : AbstractValidator<ValidateRucRequest>
{
    public ValidateRucValidator()
    {
        RuleFor(x => x.Ruc)
            .NotEmpty().WithMessage("RUC cannot be empty")
            .Must(BeAValidRuc).WithMessage("The RUC is mathematically invalid or has an incorrect format.");
    }

    private static bool BeAValidRuc(string ruc)
    {
        if (string.IsNullOrWhiteSpace(ruc) || ruc.Length != 11 || !ruc.All(char.IsDigit))
            return false;

        if (!ruc.StartsWith("10") && !ruc.StartsWith("15") &&
            !ruc.StartsWith("17") && !ruc.StartsWith("20"))
            return false;

        ReadOnlySpan<int> factors = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2];
        var sum = 0;

        for (var i = 0; i < 10; i++)
        {
            sum += (ruc[i] - '0') * factors[i];
        }

        var remainder = sum % 11;
        var check = 11 - remainder;

        check = check switch
        {
            10 => 0,
            11 => 1,
            _ => check
        };

        var lastDigit = ruc[10] - '0';

        return check == lastDigit;
    }
}
