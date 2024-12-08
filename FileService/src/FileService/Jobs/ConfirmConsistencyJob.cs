using FileService.Infrastructure.MongoDataAccess;
using FileService.Interfaces;
using Hangfire;

namespace FileService.Jobs;

public class ConfirmConsistencyJob(
    IFilesRepository repository, 
    IFileProvider provider,
    ILogger<ConfirmConsistencyJob> logger)
{
    
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = [5, 10, 15])]
    public async Task Execute(Guid fileId, string bucketName, string key)
    {
        try
        {
            logger.LogInformation("Starting ConfirmConsistencyJob with {fileId} and {key}", fileId, key);

            var fileData = await repository.GetById(fileId);

            var metadata = await provider.GetObjectMetadata(key);

            if (fileData.IsFailure)
            {
                logger.LogWarning(
                    "Record not found in MongoDb for id: {fileId}. Deleting file from s3 storage", fileId);

                await provider.DeleteObject(bucketName, key);
                return;
            }

            if (fileData.Value.Key != key)
            {
                logger.LogError("MongoDb data key does not match metadata key. " +
                                "Deleting file from s3 storage and MongoDb");

                await provider.DeleteObject(bucketName, key);
                await repository.DeleteRange([fileId]);
            }
            
            logger.LogInformation("ConfirmConsistencyJob is done for {fileId} and {key}", fileId, key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can not check consistency");
        }
    }
}