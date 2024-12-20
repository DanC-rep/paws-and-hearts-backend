using FileService.Contracts.Requests;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.PetManagement.Contracts.Requests.Volunteer;

namespace PawsAndHearts.PetManagement.Application.UseCases.CompleteUploadPhotosToPet;

public record CompleteUploadPhotosToPetCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<CompleteMultipartUploadRequest> Files) : ICommand
{
    public static CompleteUploadPhotosToPetCommand Create(
        Guid volunteerId,
        Guid petId,
        IEnumerable<CompleteMultipartUploadRequest> files) =>
        new(volunteerId, petId, files);
}