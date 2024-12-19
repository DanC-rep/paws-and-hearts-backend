using FileService.Contracts.Requests;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.PetManagement.Application.UseCases.StartUploadPhotosToPet;

public record StartUploadPhotosToPetCommand(
    Guid VolunteerId, 
    Guid PetId, 
    IEnumerable<StartMultipartUploadRequest> Files) : ICommand
{
    public static StartUploadPhotosToPetCommand Create(
        Guid volunteerId,
        Guid petId,
        IEnumerable<StartMultipartUploadRequest> files) =>
        new(volunteerId, petId, files);
}