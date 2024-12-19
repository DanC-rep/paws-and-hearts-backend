using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.Accounts.Application.UseCases.CompleteUploadPhoto;

public class CompleteUploadPhotoHandler : ICommandHandler<CompleteMultipartUploadResponse, CompleteUploadPhotoCommand>
{
    private readonly IFileService _fileService;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompleteUploadPhotoHandler> _logger;

    public CompleteUploadPhotoHandler(
        IFileService fileService,
        UserManager<User> userManager,
        ILogger<CompleteUploadPhotoHandler> logger,
        [FromKeyedServices(Modules.Accounts)] IUnitOfWork unitOfWork)
    {
        _fileService = fileService;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<CompleteMultipartUploadResponse, ErrorList>> Handle(
        CompleteUploadPhotoCommand command, 
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        
        if (user is null)
            return Errors.General.NotFound(command.UserId, "user id").ToErrorList();

        var completeUploadRequest = new CompleteMultipartRequest(command.UploadId, command.Parts);

        var result = await _fileService.CompleteMultipartUpload(
            completeUploadRequest, command.Key, cancellationToken);
        
        if (result.IsFailure)
            return Error.Failure("complete.multipart.upload", result.Error).ToErrorList();

        var photo = new Photo(result.Value.FileId);

        user.Photo = photo;

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Photo was uploaded for user with id {petID}", command.UploadId);

        return result.Value;
    }
}