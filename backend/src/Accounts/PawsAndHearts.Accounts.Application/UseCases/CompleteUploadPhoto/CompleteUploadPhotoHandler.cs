using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

    public CompleteUploadPhotoHandler(
        IFileService fileService,
        UserManager<User> userManager,
        [FromKeyedServices(Modules.Accounts)] IUnitOfWork unitOfWork)
    {
        _fileService = fileService;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
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

        return result.Value;
    }
}