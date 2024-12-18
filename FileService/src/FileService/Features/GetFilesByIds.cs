using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Endpoints;
using FileService.Infrastructure.MongoDataAccess;
using FileService.Interfaces;
using FileInfo = FileService.Contracts.Responses.FileInfo;

namespace FileService.Features;

public static class GetFilesByIds
{
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
        
        var fileInfos = files.Value.Zip(paths.Value, (file, url) => 
                new FileInfo(file.Id, url, file.Key, file.UploadDate))
            .ToList();

        var response = new GetFilesByIdsResponse(fileInfos);
        
        return Results.Ok(response);
    }
}