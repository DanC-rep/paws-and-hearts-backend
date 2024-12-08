using CSharpFunctionalExtensions;
using FileService.Data.Models;
using FileService.Data.Shared;
using MongoDB.Driver;

namespace FileService.Infrastructure.MongoDataAccess;

public class FilesRepository : IFilesRepository
{
    private readonly FileMongoDbContext _dbContext;
    
    public FilesRepository(FileMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRange(IEnumerable<FileData> filesData, CancellationToken cancellationToken = default)
    {
        await _dbContext.Files.InsertManyAsync(filesData, cancellationToken: cancellationToken);
    }

    public async Task DeleteRange(IEnumerable<Guid> filesIds, CancellationToken cancellationToken = default)
    {
        await _dbContext.Files.DeleteManyAsync(f => filesIds.Contains(f.Id), cancellationToken);
    }

    public async Task<Result<List<FileData>, Error>> Get(
        IEnumerable<Guid> filesIds, 
        CancellationToken cancellationToken = default)
    {
        var files = await _dbContext.Files.Find(f => filesIds.Contains(f.Id)).ToListAsync(cancellationToken);

        if (files.Count == 0)
            return Error.Null("files.not.found", "Files not found");

        return files;

    }

    public async Task<Result<FileData, Error>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var file = await _dbContext.Files.Find(f => f.Id == id).FirstOrDefaultAsync(cancellationToken);

        if (file is null)
            return Error.NotFound("record.not.found", "File not found");

        return file;
    }
}