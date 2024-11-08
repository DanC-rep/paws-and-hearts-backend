using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Contracts.Dtos;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Application.Queries.GetUserById;

public class GetUserByIdHandler : IQueryHandlerWithResult<UserDto, GetUserByIdQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetUserByIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }
    
    public async Task<Result<UserDto, ErrorList>> Handle(
        GetUserByIdQuery query, 
        CancellationToken cancellationToken = default)
    {
        var user = await GetUserById(query.UserId, cancellationToken);

        if (user is null)
            return Errors.General.NotFound(query.UserId, "user").ToErrorList();

        return user;
    }

    private async Task<UserDto?> GetUserById(Guid userId, CancellationToken cancellationToken = default) =>
        await _readDbContext.Users
            .Include(u => u.AdminAccount)
            .Include(u => u.VolunteerAccount)
            .Include(u => u.ParticipantAccount)
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
}