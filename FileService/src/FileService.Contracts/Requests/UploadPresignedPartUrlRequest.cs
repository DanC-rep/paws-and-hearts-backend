namespace FileService.Contracts.Requests;

public record UploadPresignedPartUrlRequest(string UploadId, int PartNumber);