using FluentValidation;
using PawsAndHearts.Core.Validation;
using PawsAndHearts.Discussions.Domain.ValueObjects;

namespace PawsAndHearts.Discussions.Application.UseCases.UpdateMessage;

public class UpdateMessageValidator : AbstractValidator<UpdateMessageCommand>
{
    public UpdateMessageValidator()
    {
        RuleFor(c => c.Message)
            .MustBeValueObject(MessageText.Create);
    }
}