using FileService.Contracts.Requests;

namespace PawsAndHearts.PetManagement.Contracts.Requests.Volunteer;

public record StartUploadPhotosToPetRequest(IEnumerable<StartMultipartUploadRequest> Files);