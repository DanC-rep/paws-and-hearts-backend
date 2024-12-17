namespace FileService.Contracts.Requests;

public record CompleteMultipartRequest(string UploadId, List<PartETagInfo> Parts);