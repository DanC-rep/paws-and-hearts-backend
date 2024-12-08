using CSharpFunctionalExtensions;
using FileService.Data.Models;
using FileService.Data.Shared;

namespace FileService.Infrastructure.MongoDataAccess;

public interface IFilesRepository
{
    Task AddRange(IEnumerable<FileData> filesData, CancellationToken cancellationToken = default);

    Task DeleteRange(IEnumerable<Guid> filesIds, CancellationToken cancellationToken = default);

    Task<Result<List<FileData>, Error>> Get(
        IEnumerable<Guid> filesIds, 
        CancellationToken cancellationToken = default);

    Task<Result<FileData, Error>> GetById(Guid id, CancellationToken cancellationToken = default);
}