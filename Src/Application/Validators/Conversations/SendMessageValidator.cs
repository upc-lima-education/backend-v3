using Backend.Src.Application.Dtos.Requests.Conversations;
using Backend.Src.Domain.Rules.Conversations;
using FluentValidation;

internal sealed class SendMessageValidator : AbstractValidator<SendMessageRequest>
{
    private const int MaxContentLength = MessageRules.MaxContentLength;

    public SendMessageValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message cannot be empty")
            .MaximumLength(MaxContentLength).WithMessage($"Message must be at most {MaxContentLength} characters long");
    }
}