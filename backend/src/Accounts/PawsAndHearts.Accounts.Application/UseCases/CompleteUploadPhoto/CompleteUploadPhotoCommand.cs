using FileService.Contracts;
using FileService.Contracts.Requests;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Accounts.Application.UseCases.CompleteUploadPhoto;

public record CompleteUploadPhotoCommand(Guid UserId, string Key, string UploadId, List<PartETagInfo> Parts) : ICommand
{
    public static CompleteUploadPhotoCommand Create(Guid userId, string key, CompleteMultipartRequest request) =>
        new(userId, key, request.UploadId, request.Parts);
}