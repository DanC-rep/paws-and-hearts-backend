namespace FileService.Contracts.Requests;

public record GetFilesByIdsRequest(IEnumerable<Guid> FilesIds);