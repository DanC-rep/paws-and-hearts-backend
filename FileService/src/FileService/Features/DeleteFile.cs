using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Data.Models;
using FileService.Endpoints;
using FileService.Infrastructure.MongoDataAccess;
using FileService.Infrastructure.Providers.Data;
using FileService.Interfaces;
using FileService.Jobs;
using Hangfire;

namespace FileService.Features;

public static class DeleteFile
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("files/{id:guid}/delete", Handler);
        }
    }

    private static async Task<IResult> Handler(
        Guid id,
        IFilesRepository repository,
        IFileProvider provider,
        CancellationToken cancellationToken = default)
    {

        var file = await repository.GetById(id, cancellationToken);
        
        if (file.IsFailure)
            return Results.BadRequest(file.Error.Message);

        var result = await provider.DeleteObject(file.Value.Key, file.Value.BucketName, cancellationToken);
        
        if (result.IsFailure)
            return Results.BadRequest(result.Error.Message);

        await repository.DeleteRange([file.Value.Id], cancellationToken);
        
        return Results.Ok();
    }
}