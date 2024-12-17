namespace FileService.Contracts.Responses;

public record FileResponse(Guid FileId, string PresignedUrl);