namespace FileService.Contracts.Responses;

public record CompleteMultipartUploadResponse(Guid FileId, string Location);