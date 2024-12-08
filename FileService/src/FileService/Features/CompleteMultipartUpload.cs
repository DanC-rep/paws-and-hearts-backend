using FileService.Data.Models;
using FileService.Endpoints;
using FileService.Infrastructure.MongoDataAccess;
using FileService.Infrastructure.Providers.Data;
using FileService.Interfaces;
using FileService.Jobs;
using Hangfire;

namespace FileService.Features;

public static class CompleteMultipartUpload
{
    private record CompleteMultipartRequest(string UploadId, List<PartETagInfo> Parts);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/{key}/complete-multipart", Handler);
        }
    }

    private static async Task<IResult> Handler(
        string key,
        CompleteMultipartRequest request,
        IFilesRepository repository,
        IFileProvider provider,
        CancellationToken cancellationToken = default)
    {
        var data = new CompleteMultipartUploadData(request.UploadId, key, request.Parts);

        var result = await provider.CompleteMultipartUpload(data, cancellationToken);
        
        if (result.IsFailure)
            return Results.BadRequest(result.Error.Message);

        var metaData = await provider.GetObjectMetadata(key, cancellationToken);

        var fileId = Guid.NewGuid();

        var fileData = new FileData
        {
            Id = fileId,
            StoragePath = key,
            FileSize = metaData.Headers.ContentLength,
            ContentType = metaData.Headers.ContentType,
            UploadDate = DateTime.UtcNow,
            Key = key,
            BucketName = result.Value.BucketName
        };

        await repository.AddRange([fileData], cancellationToken);

        BackgroundJob.Schedule<ConfirmConsistencyJob>(
            j => j.Execute(fileId, result.Value.BucketName, key),
            TimeSpan.FromMinutes(1));
        
        return Results.Ok(new
        {
            key,
            location = result.Value.Location
        });
    }
}