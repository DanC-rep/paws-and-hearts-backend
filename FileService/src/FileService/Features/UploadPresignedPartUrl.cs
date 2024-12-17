using Amazon.S3;
using Amazon.S3.Model;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Endpoints;
using FileService.Infrastructure.Providers.Data;
using FileService.Interfaces;

namespace FileService.Features;

public static class UploadPresignedPartUrl
{
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

        var response = new UploadPresignedPartUrlResponse(key, presignedUrlResult.Value);
        
        return Results.Ok(response);
    }
}