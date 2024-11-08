using FluentValidation;
using PawsAndHearts.Core.Validation;
using PawsAndHearts.VolunteerRequests.Domain.ValueObjects;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.RejectVolunteerRequest;

public class RejectVolunteerRequestValidator : AbstractValidator<RejectVolunteerRequestCommand>
{
    public RejectVolunteerRequestValidator()
    {
        RuleFor(c => c.RejectionComment)
            .MustBeValueObject(RejectionComment.Create);
    }
}