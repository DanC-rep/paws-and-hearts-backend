namespace FileService.Contracts.Requests;

public record StartMultipartUploadRequest(string FileName, string ContentType);