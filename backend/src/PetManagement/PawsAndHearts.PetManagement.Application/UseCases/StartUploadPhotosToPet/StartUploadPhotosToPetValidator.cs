using FluentValidation;
using PawsAndHearts.Core.Validation;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.PetManagement.Application.UseCases.StartUploadPhotosToPet;

public class StartUploadPhotosToPetValidator : AbstractValidator<StartUploadPhotosToPetCommand>
{
    public StartUploadPhotosToPetValidator()
    {
        RuleFor(a => a.VolunteerId).NotEmpty()
            .WithError(Errors.General.ValueIsRequired("volunteer id"));

        RuleFor(a => a.PetId).NotEmpty()
            .WithError(Errors.General.ValueIsRequired("pet id"));

        RuleFor(a => a.Files).NotEmpty()
            .WithError(Errors.General.ValueIsRequired("files"));
    }
}