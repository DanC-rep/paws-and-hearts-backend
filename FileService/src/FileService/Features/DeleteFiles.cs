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

public static class DeleteFiles
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/delete", Handler);
        }
    }

    private static async Task<IResult> Handler(
        DeleteFilesRequest request,
        IFilesRepository repository,
        IFileProvider provider,
        CancellationToken cancellationToken = default)
    {

        var files = await repository.Get(request.FilesIds, cancellationToken);
        
        if (files.IsFailure)
            return Results.BadRequest(files.Error.Message);

        var result = await provider.DeleteObjects(files.Value, cancellationToken);
        
        if (result.IsFailure)
            return Results.BadRequest(result.Error.Message);

        await repository.DeleteRange(request.FilesIds, cancellationToken);
        
        return Results.Ok();
    }
}