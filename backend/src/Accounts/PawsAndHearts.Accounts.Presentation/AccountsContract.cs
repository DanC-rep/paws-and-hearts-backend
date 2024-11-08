using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Application.Queries.GetPermissionsByUserId;
using PawsAndHearts.Accounts.Application.Queries.GetUserById;
using PawsAndHearts.Accounts.Contracts;
using PawsAndHearts.Accounts.Contracts.Dtos;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.Accounts.Presentation;

public class AccountsContract : IAccountsContract
{
    private readonly GetPermissionsByUserIdHandler _getPermissionsByUserIdHandler;
    private readonly GetUserByIdHandler _getUserByIdHandler;
    private readonly UserManager<User> _userManager;
    private readonly IAccountManager _accountManager;
    private readonly IUnitOfWork _unitOfWork;

    public AccountsContract(
        GetPermissionsByUserIdHandler getPermissionsByUserIdHandler,
        GetUserByIdHandler getUserByIdHandler,
        UserManager<User> userManager,
        IAccountManager accountManager,
        [FromKeyedServices(Modules.Accounts)] IUnitOfWork unitOfWork)
    {
        _getPermissionsByUserIdHandler = getPermissionsByUserIdHandler;
        _getUserByIdHandler = getUserByIdHandler;
        _userManager = userManager;
        _accountManager = accountManager;
        _unitOfWork = unitOfWork;
    }


    public async Task<Result<IEnumerable<string>, ErrorList>> GetPermissionsByUserId(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var query = new GetPermissionsByUserIdQuery(userId);
        
        return await _getPermissionsByUserIdHandler.Handle(query, cancellationToken);
    }

    public async Task<Result<UserDto, ErrorList>> GetUserById(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(userId);
        
        return await _getUserByIdHandler.Handle(query, cancellationToken);
    }

    public async Task<UnitResult<Error>> CreateVolunteerAccount(
        Guid userId, 
        Experience experience, 
        IEnumerable<Requisite> requisites,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return Errors.General.NotFound(userId);

        var volunteerAccount = new VolunteerAccount(user, experience, requisites);

        await _accountManager.AddVolunteerAccount(volunteerAccount, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);

        return Result.Success<Error>();
    }
}