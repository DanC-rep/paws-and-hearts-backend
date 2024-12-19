using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using PawsAndHearts.Core.Abstractions;
using FileService.Contracts.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.Accounts.Application.UseCases.StartUploadPhoto;

public class StartUploadPhotoHandler : ICommandHandler<StartMultipartUploadResponse, StartUploadPhotoCommand>
{
    private readonly IFileService _fileService;
    private readonly UserManager<User> _userManager;

    public StartUploadPhotoHandler(
        IFileService fileService,
        UserManager<User> userManager)
    {
        _fileService = fileService;
        _userManager = userManager;
    }
    
    public async Task<Result<StartMultipartUploadResponse, ErrorList>> Handle(
        StartUploadPhotoCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = Photo.Validate(command.FileName, command.ContentType);

        if (validationResult.IsFailure)
            return validationResult.Error.ToErrorList();

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        
        if (user is null)
            return Errors.General.NotFound(command.UserId, "user id").ToErrorList();

        var startMultipartRequest = new StartMultipartUploadRequest(command.FileName, command.ContentType);

        var result = await _fileService.StartMultipartUpload(startMultipartRequest, cancellationToken);

        if (result.IsFailure)
            return Error.Failure("start.multipart.upload", result.Error).ToErrorList();

        return result.Value;
    }
}