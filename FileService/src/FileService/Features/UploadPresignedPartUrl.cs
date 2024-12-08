using Amazon.S3;
using Amazon.S3.Model;
using FileService.Endpoints;
using FileService.Infrastructure.Providers.Data;
using FileService.Interfaces;

namespace FileService.Features;

public static class UploadPresignedPartUrl
{
    private record UploadPresignedPartUrlRequest(string UploadId, int PartNumber);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/{key}/presigned-part", Handler);
        }
    }

    private static async Task<IResult> Handler(
        string key,
        UploadPresignedPartUrlRequest request,
        IFileProvider provider,
        CancellationToken cancellationToken = default)
    {
        var data = new GetPresignedUrlForUploadPartData(key, request.UploadId, request.PartNumber);

        var presignedUrlResult = await provider.GetPresignedUrlForUploadPart(data, cancellationToken);
        
        if (presignedUrlResult.IsFailure)
            return Results.BadRequest(presignedUrlResult.Error.Message);
        
        return Results.Ok(new
        {
            key,
            url = presignedUrlResult.Value  
        });
    }
}