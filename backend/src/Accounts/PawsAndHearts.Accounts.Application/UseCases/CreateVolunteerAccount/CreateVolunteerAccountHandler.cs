using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.Accounts.Application.UseCases.CreateVolunteerAccount;

public class CreateVolunteerAccountHandler : ICommandHandler<CreateVolunteerAccountCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountManager _accountManager;
    private readonly IValidator<CreateVolunteerAccountCommand> _validator;
    private readonly ILogger<CreateVolunteerAccountHandler> _logger;

    public CreateVolunteerAccountHandler(
        UserManager<User> userManager,
        [FromKeyedServices(Modules.Accounts)] IUnitOfWork unitOfWork,
        IAccountManager accountManager,
        IValidator<CreateVolunteerAccountCommand> validator,
        ILogger<CreateVolunteerAccountHandler> logger)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _accountManager = accountManager;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        CreateVolunteerAccountCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
            return Errors.General.NotFound(command.UserId).ToErrorList();

        var experience = Experience.Create(command.Experience).Value;
        
        var requisites = command.Requisites.Select(r =>
            Requisite.Create(r.Name, r.Description).Value).ToList();

        var volunteerAccount = new VolunteerAccount(user, experience, requisites);

        await _accountManager.AddVolunteerAccount(volunteerAccount, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Volunteer account was created with id {volunteerAccountId}", volunteerAccount.Id);

        return Result.Success<ErrorList>();
    }
}