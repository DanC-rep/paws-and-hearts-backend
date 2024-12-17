using FileService.Endpoints;
using FileService.Infrastructure.MongoDataAccess;
using FileService.Interfaces;
using FileService.Contracts.Responses;

namespace FileService.Features;

public static class DownloadPresignedUrl
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("files/{id:guid}/presigned", Handler);
        }
    }

    private static async Task<IResult> Handler(
        Guid id,
        IFileProvider provider,
        IFilesRepository repository,
        CancellationToken cancellationToken = default)
    {
        var file = await repository.GetById(id, cancellationToken);

        if (file.IsFailure)
            return Results.BadRequest(file.Error.Message);
        
        var presignedUrlResult = await provider.GetPresignedUrlForDownload(file.Value.Key, cancellationToken);
            
        if (presignedUrlResult.IsFailure)
            return Results.BadRequest(presignedUrlResult.Error.Message);
        
        return Results.Ok(new FileResponse(id, presignedUrlResult.Value));
    }
}