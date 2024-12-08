using FileService.Endpoints;
using FileService.Interfaces;

namespace FileService.Features;

public static class DownloadPresignedUrl
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("files/{key}/presigned", Handler);
        }
    }

    private static async Task<IResult> Handler(
        string key,
        IFileProvider provider,
        CancellationToken cancellationToken = default)
    {
        var presignedUrlResult = await provider.GetPresignedUrlForDownload(key, cancellationToken);
            
        if (presignedUrlResult.IsFailure)
            return Results.BadRequest(presignedUrlResult.Error.Message);
        
        return Results.Ok(new
        {
            key,
            url = presignedUrlResult.Value
        });
    }
}