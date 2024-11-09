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
using Constants = PawsAndHearts.Accounts.Domain.Constants;

namespace PawsAndHearts.Accounts.Application.UseCases.Register;

public class RegisterUserHandler : ICommandHandler<RegisterUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IAccountManager _accountManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterUserHandler> _logger;
    private readonly IValidator<RegisterUserCommand> _validator;

    public RegisterUserHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IAccountManager accountManager,
        [FromKeyedServices(Modules.Accounts)] IUnitOfWork unitOfWork,
        ILogger<RegisterUserHandler> logger,
        IValidator<RegisterUserCommand> validator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _accountManager = accountManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _validator = validator;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        RegisterUserCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == Constants.PARTICIPANT, cancellationToken);

            if (role is null)
                return Errors.General.NotFound(null, "role").ToErrorList();
            
            var socialNetworks = command.SocialNetworks.Select(s =>
                SocialNetwork.Create(s.Link, s.Name).Value).ToList();
            
            var fullName = FullName.Create(
                command.FullName.Name,
                command.FullName.Surname,
                command.FullName.Patronymic).Value;

            var userResult = User.CreateParticipant(
                command.UserName, 
                command.Email,
                fullName,
                socialNetworks,
                role);

            if (userResult.IsFailure)
                return userResult.Error.ToErrorList();

            var result = await _userManager.CreateAsync(userResult.Value, command.Password);

            if (!result.Succeeded)
                return Error.Failure("register.user", "can not register user").ToErrorList();

            var participantAccount = new ParticipantAccount(userResult.Value);
            
            await _accountManager.AddParticipantAccount(participantAccount, cancellationToken);

            await _unitOfWork.SaveChanges(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            
            _logger.LogInformation("User was created with name {userName}", command.UserName);

            return Result.Success<ErrorList>();
        }
        catch (Exception ex)
        {
            _logger.LogError("User registration was failed");
            
            await transaction.RollbackAsync(cancellationToken);

            return Error.Failure("register.user", "Can not register user").ToErrorList();
        }
    }
}