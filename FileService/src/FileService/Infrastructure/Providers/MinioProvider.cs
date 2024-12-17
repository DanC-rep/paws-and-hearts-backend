using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Data.Models;
using FileService.Data.Shared;
using FileService.Infrastructure.Providers.Data;
using FileService.Interfaces;
using Minio.DataModel;

namespace FileService.Infrastructure.Providers;

public class MinioProvider : IFileProvider
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<MinioProvider> _logger;

    public const string BUCKET_NAME = "dev-bucket";
    
    private const int MAX_DEGREE_OF_PARALLELIZM = 10;
    private const int EXPIRATION_URL = 24;

    public MinioProvider(IAmazonS3 s3Client, ILogger<MinioProvider> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
    }
    
    public async Task<Result<string, Error>> GetPresignedUrlForUpload(
        GetPresignedUrlForUploadData data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var presignedRequest = new GetPreSignedUrlRequest()
            {
                BucketName = BUCKET_NAME,
                Key = data.Key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddHours(EXPIRATION_URL),
                Protocol = Protocol.HTTP,
                ContentType = data.ContentType,
                Metadata =
                {
                    ["file-name"] = data.FileName
                }
            };

            var presignedUrl = await _s3Client.GetPreSignedURLAsync(presignedRequest);

            return presignedUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Fail to get {fileName} upload url to minio in bucket {bucket}",
                data.FileName,
                BUCKET_NAME);

            return Error.Failure("file.upload", "Fail to upload file in minio");
        }
    }

    public async Task<Result<string, Error>> GetPresignedUrlForDownload(
        string key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var presignedRequest = new GetPreSignedUrlRequest()
            {
                BucketName = BUCKET_NAME,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddHours(EXPIRATION_URL),
                Protocol = Protocol.HTTP,
            };

            var presignedUrl = await _s3Client.GetPreSignedURLAsync(presignedRequest);

            return presignedUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Fail to get {key} download url from minio in bucket {bucket}",
                key,
                BUCKET_NAME);

            return Error.Failure("file.get", "Fail to get file from minio");
        }
    }

    public async Task<Result<string, Error>> StartMultipartUpload(
        StartMultipartUploadData data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var presignedRequest = new InitiateMultipartUploadRequest
            {
                BucketName = BUCKET_NAME,
                Key = data.Key,
                ContentType = data.ContentType,
                Metadata =
                {
                    ["file-name"] = data.FileName
                }
            };

            var response = await _s3Client
                .InitiateMultipartUploadAsync(presignedRequest, cancellationToken);

            return response.UploadId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail to start multipart upload for file {fileName}", data.FileName);

            return Error.Failure("start.multipart", "Fail to start multipart upload");
        }
    }

    public async Task<Result<string, Error>> GetPresignedUrlForUploadPart(
        GetPresignedUrlForUploadPartData data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var presignedRequest = new GetPreSignedUrlRequest()
            {
                BucketName = BUCKET_NAME,
                Key = data.Key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddHours(EXPIRATION_URL),
                Protocol = Protocol.HTTP,
                UploadId = data.UploadId,
                PartNumber = data.PartNumber
            };

            var presignedUrl = await _s3Client.GetPreSignedURLAsync(presignedRequest);

            return presignedUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail to generating presigned url for upload id: {uploadId}", data.UploadId);

            return Error.Failure("upload.part", "Fail to upload part of file in minio");
        }
    }

    public async Task<Result<CompleteMultipartUploadResponse, Error>> CompleteMultipartUpload(
        CompleteMultipartUploadData data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var presignedRequest = new CompleteMultipartUploadRequest
            {
                BucketName = BUCKET_NAME,
                Key = data.Key,
                UploadId = data.UploadId,
                PartETags = data.Parts.Select(p => new PartETag(p.PartNumber, p.ETag)).ToList()
            };

            var response = await _s3Client
                .CompleteMultipartUploadAsync(presignedRequest, cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail to complete multipart upload for upload id: {uploadId}", data.UploadId);
            
            return Error.Failure("complete.multipart", "Fail to complete multipart upload");
        }
    }

    public async Task<GetObjectMetadataResponse> GetObjectMetadata(
        string key, 
        CancellationToken cancellationToken = default)
    {
        var metaDataRequest = new GetObjectMetadataRequest
        {
            BucketName = BUCKET_NAME,
            Key = key
        };
        
        return await _s3Client.GetObjectMetadataAsync(metaDataRequest, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<string>, Error>> DownloadFiles(
        IEnumerable<FileData> filesData, CancellationToken cancellationToken = default)
    {
        var semaphoreSlim = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELIZM);
        var filesList = filesData.ToList();
        
        try
        {
            await IsBucketExists(filesList.Select(f => f.BucketName), cancellationToken);

            var tasks = filesList.Select(async file =>
                await GetPresignedUrlForDownload(file.Key, semaphoreSlim, cancellationToken));

            var pathsResult = await Task.WhenAll(tasks);

            if (pathsResult.Any(p => p.IsFailure))
                return pathsResult.First().Error;

            var results = pathsResult.Select(p => p.Value).ToList();

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail to download files");

            return Error.Failure("file.download", "Fail to download files");
        }
    }

    private async Task<Result<string, Error>> GetPresignedUrlForDownload(
        string key,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken = default)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);

        try
        {
            var presignedRequest = new GetPreSignedUrlRequest()
            {
                BucketName = BUCKET_NAME,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddHours(EXPIRATION_URL),
                Protocol = Protocol.HTTP,
            };

            var presignedUrl = await _s3Client.GetPreSignedURLAsync(presignedRequest);

            return presignedUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Fail to get {key} download url from minio in bucket {bucket}",
                key,
                BUCKET_NAME);

            return Error.Failure("file.get", "Fail to get file from minio");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private async Task IsBucketExists(
        IEnumerable<string> bucketNames, CancellationToken cancellationToken = default)
    {
        HashSet<string> buckets = [..bucketNames];

        var response = await _s3Client.ListBucketsAsync(cancellationToken);

        foreach (var bucketName in buckets)
        {
            var isExists = response.Buckets
                .Exists(b => b.BucketName.Equals(bucketName, StringComparison.OrdinalIgnoreCase));

            if (!isExists)
            {
                var request = new PutBucketRequest
                {
                    BucketName = bucketName
                };

                await _s3Client.PutBucketAsync(request, cancellationToken);
            }
        }
    }

    public async Task<UnitResult<Error>> DeleteObject(
        string bucketName,
        string key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request, cancellationToken);

            return Result.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Can not delete file from minio");
            
            return Error.Failure("minio.file.delete", "Can not delete file from minio");
        }
    }
}