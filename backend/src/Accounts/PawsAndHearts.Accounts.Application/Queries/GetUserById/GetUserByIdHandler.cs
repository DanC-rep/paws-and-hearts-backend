using CSharpFunctionalExtensions;
using FileService.Communication;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Contracts.Dtos;
using PawsAndHearts.Accounts.Contracts.Responses;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Application.Queries.GetUserById;

public class GetUserByIdHandler : IQueryHandlerWithResult<GetUserByIdResponse, GetUserByIdQuery>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IFileService _fileService;

    public GetUserByIdHandler(
        IReadDbContext readDbContext,
        IFileService fileService)
    {
        _readDbContext = readDbContext;
        _fileService = fileService;
    }
    
    public async Task<Result<GetUserByIdResponse, ErrorList>> Handle(
        GetUserByIdQuery query, 
        CancellationToken cancellationToken = default)
    {
        var user = await GetUserById(query.UserId, cancellationToken);

        if (user is null)
            return Errors.General.NotFound(query.UserId, "user").ToErrorList();

        if (user.PhotoId is null)
            return GetUserByIdResponse.Create(user);

        var downloadUrlResponse = await _fileService
            .DownloadFilePresignedUrl((Guid)user.PhotoId!, cancellationToken);

        if (downloadUrlResponse.IsFailure)
            return Error.Failure("file.download.url", "Fail to get file download url").ToErrorList();
        
        return GetUserByIdResponse.Create(user, downloadUrlResponse.Value.PresignedUrl);
    }

    private async Task<UserDto?> GetUserById(Guid userId, CancellationToken cancellationToken = default) =>
        await _readDbContext.Users
            .Include(u => u.AdminAccount)
            .Include(u => u.VolunteerAccount)
            .Include(u => u.ParticipantAccount)
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
}