namespace FileService.Infrastructure.Providers.Data;

public record GetPresignedUrlForUploadPartData(string Key, string UploadId, int PartNumber);