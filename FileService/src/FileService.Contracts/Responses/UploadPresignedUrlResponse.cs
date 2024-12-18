namespace FileService.Contracts.Responses;

public record UploadPresignedUrlResponse(Guid Id, string PresignedUrl);