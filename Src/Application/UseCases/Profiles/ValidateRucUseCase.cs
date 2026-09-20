using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Domain.Ports.Profiles;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Profiles;

public class ValidateRucUseCase(
    IRucValidationPort rucValidationPort,
    IValidator<ValidateRucRequest> validator
)
{
    public async Task<bool> ExecuteAsync(ValidateRucRequest request)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var isRucValid = await rucValidationPort.ValidateRucAsync(request.Ruc);
        return isRucValid;
    }
}