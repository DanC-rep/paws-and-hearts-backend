namespace FileService.Infrastructure.Providers.Data;

public record GetPresignedUrlForUploadData(string FileName, string Key, string ContentType);