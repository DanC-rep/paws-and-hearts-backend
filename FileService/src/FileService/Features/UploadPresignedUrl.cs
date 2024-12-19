using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Data.Models;
using FileService.Endpoints;
using FileService.Infrastructure.MongoDataAccess;
using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Providers.Data;
using FileService.Interfaces;
using FileService.Jobs;
using Hangfire;

namespace FileService.Features;

public static class UploadPresignedUrl
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/presigned", Handler);
        }
    }

    private static async Task<IResult> Handler(
        UploadPresignedUrlRequest request,
        IFilesRepository repository,
        IFileProvider provider,
        CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(request.FileName);
        
        var key = $"{Guid.NewGuid()}{extension}";

        var data = new GetPresignedUrlForUploadData(request.FileName, key, request.ContentType);

        var presignedUrlResult = await provider.GetPresignedUrlForUpload(data, cancellationToken);
        
        if (presignedUrlResult.IsFailure)
            return Results.BadRequest(presignedUrlResult.Error.Message);

        var fileId = Guid.NewGuid();
        
        var fileData = new FileData
        {
            Id = fileId,
            FileSize = request.Size,
            ContentType = request.ContentType,
            UploadDate = DateTime.UtcNow,
            Key = key,
            BucketName = MinioProvider.BUCKET_NAME
        };

        await repository.AddRange([fileData], cancellationToken);
        
        BackgroundJob.Schedule<ConfirmConsistencyJob>(
            j => j.Execute(fileId, MinioProvider.BUCKET_NAME, key),
            TimeSpan.FromMinutes(1));

        var response = new UploadPresignedUrlResponse(fileId, presignedUrlResult.Value);

        return Results.Ok(response);
        
        
    }
}