using FileService.Endpoints;
using FileService.Infrastructure.Providers.Data;
using FileService.Interfaces;

namespace FileService.Features;

public static class StartMultipartUpload
{
    private record StartMultipartUploadRequest(string FileName, string ContentType);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/multipart", Handler);
        }
    }

    private static async Task<IResult> Handler(
        StartMultipartUploadRequest request,
        IFileProvider provider,
        CancellationToken cancellationToken = default)
    {
        var fileExtension = Path.GetExtension(request.FileName);
        
        var key = $"{Guid.NewGuid()}.{fileExtension}";

        var data = new StartMultipartUploadData(request.FileName, key, request.ContentType);

        var result = await provider.StartMultipartUpload(data, cancellationToken);
        
        if (result.IsFailure)
            return Results.BadRequest(result.Error.Message);
        
        return Results.Ok(new
        {
            key,
            uploadId = result.Value
        });
        
        
    }
}