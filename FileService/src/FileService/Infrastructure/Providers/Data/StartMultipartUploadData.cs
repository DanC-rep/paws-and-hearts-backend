namespace FileService.Infrastructure.Providers.Data;

public record StartMultipartUploadData(string FileName, string Key, string ContentType);