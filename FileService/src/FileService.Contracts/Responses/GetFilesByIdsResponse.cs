namespace FileService.Contracts.Responses;

public record GetFilesByIdsResponse(IEnumerable<FileInfo> FilesInfo);

public record FileInfo(Guid Id, string DownloadPath, string Key, DateTime UploadDate);