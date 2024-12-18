using FileService.Contracts.Requests;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Accounts.Application.UseCases.StartUploadPhoto;

public record StartUploadPhotoCommand(Guid UserId, string FileName, string ContentType) : ICommand
{
    public static StartUploadPhotoCommand Create(Guid userId, StartMultipartUploadRequest request) =>
        new(userId, request.FileName, request.ContentType);
}