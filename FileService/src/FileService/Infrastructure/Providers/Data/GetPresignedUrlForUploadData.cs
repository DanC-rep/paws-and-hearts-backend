namespace FileService.Infrastructure.Providers.Data;

public record GetPresignedUrlForUploadData(string FileName, Guid Key, string ContentType);