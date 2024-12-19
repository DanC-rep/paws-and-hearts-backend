using FileService.Contracts;
using FileService.Contracts.Requests;

namespace PawsAndHearts.PetManagement.Contracts.Requests.Volunteer;

public record CompleteUploadPhotosToPetRequest(IEnumerable<CompleteMultipartUploadRequest> Files);

public record CompleteMultipartUploadRequest(string UploadId, List<PartETagInfo> Parts, string Key);