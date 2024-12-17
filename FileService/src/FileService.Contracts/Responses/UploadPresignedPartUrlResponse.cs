namespace FileService.Contracts.Responses;

public record UploadPresignedPartUrlResponse(string Key, string PresignedUrl);