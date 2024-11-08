using FluentValidation;
using PawsAndHearts.Core.Validation;
using PawsAndHearts.VolunteerRequests.Domain.ValueObjects;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.SendVolunteerRequestForRevision;

public class SendVolunteerRequestForRevisionValidator : AbstractValidator<SendVolunteerRequestForRevisionCommand>
{
    public SendVolunteerRequestForRevisionValidator()
    {
        RuleFor(c => c.RejectionComment)
            .MustBeValueObject(RejectionComment.Create);
    }
}