using FileService.Endpoints;
using FileService.Infrastructure.MongoDataAccess;
using FileService.Infrastructure.Providers.Data;
using FileService.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FileService.Features;

public static class GetFilesByIds
{
    private record GetFilesByIdsRequest(IEnumerable<Guid> FilesIds);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files", Handler);
        }
    }

    private static async Task<IResult> Handler(
        GetFilesByIdsRequest request,
        IFilesRepository repository,
        IFileProvider provider,
        CancellationToken cancellationToken = default)
    {
        var files = await repository.Get(request.FilesIds, cancellationToken);

        if (files.IsFailure)
            return Results.BadRequest(files.Error.Message);

        var paths = await provider.DownloadFiles(files.Value, cancellationToken);

        if (paths.IsFailure)
            return Results.BadRequest(paths.Error.Message);
        
        var response = files.Value.Zip(paths.Value,(file,url) => 
        { 
            file.DownloadPath = url;
            return file;
        }).ToList();
        
        return Results.Ok(response);
    }
}