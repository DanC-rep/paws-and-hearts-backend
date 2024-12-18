namespace FileService.Contracts.Requests;

public record UploadPresignedUrlRequest(string FileName, string ContentType, long Size);