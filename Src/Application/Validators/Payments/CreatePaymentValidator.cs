using Backend.Src.Application.Dtos.Requests.Payments;
using Backend.Src.Domain.Rules.Payments;
using Backend.Src.Domain.ValueObjects.Payments;
using FluentValidation;

namespace Backend.Src.Application.Validators.Payments;

internal sealed class CreatePaymentValidator : AbstractValidator<CreatePaymentRequest>
{
    private const int MinAmountLength = PaymentRules.MinAmountLength;

    public CreatePaymentValidator()
    {
        RuleFor(x => x.CreditPlan)
            .IsInEnum()
            .NotEqual(CreditPlan.Free)
            .WithMessage("The Free plan is included with the account and cannot be purchased.");

        RuleFor(x => x.Platform)
            .Equal(PaymentPlatform.Paypal)
            .WithMessage("PayPal is the only payment platform currently available.");
    }
}
