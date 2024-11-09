using CSharpFunctionalExtensions;
using PawsAndHearts.Accounts.Contracts.Dtos;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.Accounts.Contracts;

public interface IAccountsContract
{
    Task<Result<IEnumerable<string>, ErrorList>> GetPermissionsByUserId(
        Guid userId,
        CancellationToken cancellationToken = default);
    
    Task<Result<UserDto, ErrorList>> GetUserById(Guid userId, CancellationToken cancellationToken = default);

    Task<UnitResult<ErrorList>> CreateVolunteerAccount(
        Guid userId, 
        Experience experience, 
        IEnumerable<Requisite> requisites, 
        CancellationToken cancellationToken = default);
}