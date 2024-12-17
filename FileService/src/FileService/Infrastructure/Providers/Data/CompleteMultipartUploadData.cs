using FileService.Contracts;

namespace FileService.Infrastructure.Providers.Data;

public record CompleteMultipartUploadData(string UploadId, string Key, IEnumerable<PartETagInfo> Parts);