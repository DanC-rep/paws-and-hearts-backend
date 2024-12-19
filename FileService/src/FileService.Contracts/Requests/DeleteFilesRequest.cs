namespace FileService.Contracts.Requests;

public record DeleteFilesRequest(IEnumerable<Guid> FilesIds);