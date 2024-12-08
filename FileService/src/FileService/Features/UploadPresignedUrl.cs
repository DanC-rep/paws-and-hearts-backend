using FileService.Endpoints;
using FileService.Infrastructure.Providers.Data;
using FileService.Interfaces;

namespace FileService.Features;

public static class UploadPresignedUrl
{
    private record UploadPresignedUrlRequest(string FileName, string ContentType);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/presigned", Handler);
        }
    }

    private static async Task<IResult> Handler(
        UploadPresignedUrlRequest request,
        IFileProvider provider,
        CancellationToken cancellationToken = default)
    {
        var key = Guid.NewGuid();

        var data = new GetPresignedUrlForUploadData(request.FileName, key, request.ContentType);

        var presignedUrlResult = await provider.GetPresignedUrlForUpload(data, cancellationToken);
        
        if (presignedUrlResult.IsFailure)
            return Results.BadRequest(presignedUrlResult.Error.Message);

        return Results.Ok(new
        {
            key,
            url = presignedUrlResult.Value
        });
        
        
    }
}