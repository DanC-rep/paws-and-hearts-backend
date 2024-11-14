using FluentValidation;
using PawsAndHearts.Core.Validation;
using PawsAndHearts.Discussions.Domain.ValueObjects;

namespace PawsAndHearts.Discussions.Application.UseCases.SendMessage;

public class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(c => c.Message)
            .MustBeValueObject(MessageText.Create);
    }
}