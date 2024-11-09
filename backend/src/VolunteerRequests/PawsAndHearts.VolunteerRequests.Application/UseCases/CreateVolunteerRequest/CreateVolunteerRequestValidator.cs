using FluentValidation;
using PawsAndHearts.Core.Validation;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.CreateVolunteerRequest;

public class CreateVolunteerRequestValidator : AbstractValidator<CreateVolunteerRequestCommand>
{
    public CreateVolunteerRequestValidator()
    {
        RuleForEach(c => c.Requisites)
            .MustBeValueObject(s => 
                Requisite.Create(s.Name, s.Description));
        
        RuleFor(c => c.Experience)
            .MustBeValueObject(Experience.Create);
    }
}