namespace FileService.Infrastructure.Providers.Data;

public record CompleteMultipartUploadData(string UploadId, string Key, IEnumerable<PartETagInfo> Parts);

public record PartETagInfo(int PartNumber, string ETag);