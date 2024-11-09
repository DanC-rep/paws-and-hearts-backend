using FluentValidation;
using PawsAndHearts.Core.Validation;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.Accounts.Application.UseCases.CreateVolunteerAccount;

public class CreateVolunteerAccountValidator : AbstractValidator<CreateVolunteerAccountCommand>
{
    public CreateVolunteerAccountValidator()
    {
        RuleFor(c => c.Experience)
            .MustBeValueObject(Experience.Create);
        
        RuleForEach(c => c.Requisites)
            .MustBeValueObject(r => 
                Requisite.Create(r.Name, r.Description));
    }
}