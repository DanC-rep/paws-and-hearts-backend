using CSharpFunctionalExtensions;
using PawsAndHearts.Accounts.Application.Queries.GetPermissionsByUserId;
using PawsAndHearts.Accounts.Application.Queries.GetUserById;
using PawsAndHearts.Accounts.Application.UseCases.CreateVolunteerAccount;
using PawsAndHearts.Accounts.Contracts;
using PawsAndHearts.Accounts.Contracts.Dtos;
using PawsAndHearts.Accounts.Contracts.Responses;
using PawsAndHearts.Core.Dtos;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.Accounts.Presentation;

public class AccountsContract : IAccountsContract
{
    private readonly GetPermissionsByUserIdHandler _getPermissionsByUserIdHandler;
    private readonly GetUserByIdHandler _getUserByIdHandler;
    private readonly CreateVolunteerAccountHandler _createVolunteerAccountHandler;

    public AccountsContract(
        GetPermissionsByUserIdHandler getPermissionsByUserIdHandler,
        GetUserByIdHandler getUserByIdHandler,
        CreateVolunteerAccountHandler createVolunteerAccountHandler)
    {
        _getPermissionsByUserIdHandler = getPermissionsByUserIdHandler;
        _getUserByIdHandler = getUserByIdHandler;
        _createVolunteerAccountHandler = createVolunteerAccountHandler;
    }


    public async Task<Result<IEnumerable<string>, ErrorList>> GetPermissionsByUserId(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var query = new GetPermissionsByUserIdQuery(userId);
        
        return await _getPermissionsByUserIdHandler.Handle(query, cancellationToken);
    }

    public async Task<Result<GetUserByIdResponse, ErrorList>> GetUserById(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(userId);
        
        return await _getUserByIdHandler.Handle(query, cancellationToken);
    }

    public async Task<UnitResult<ErrorList>> CreateVolunteerAccount(
        Guid userId, 
        Experience experience, 
        IEnumerable<Requisite> requisites,
        CancellationToken cancellationToken = default)
    {
       var requisitesDtos = requisites.Select(r =>
           new RequisiteDto(r.Name, r.Description)).ToList();
       
       var command = new CreateVolunteerAccountCommand(userId, experience.Value, requisitesDtos);

       var result = await _createVolunteerAccountHandler.Handle(command, cancellationToken);

       if (result.IsFailure)
           return result.Error;

       return Result.Success<ErrorList>();
    }
}