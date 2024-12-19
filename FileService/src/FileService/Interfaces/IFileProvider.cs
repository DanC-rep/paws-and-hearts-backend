using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Data.Models;
using FileService.Data.Shared;
using FileService.Infrastructure.Providers.Data;

namespace FileService.Interfaces;

public interface IFileProvider
{
    Task<Result<string, Error>> GetPresignedUrlForUpload(
        GetPresignedUrlForUploadData data,
        CancellationToken cancellationToken = default);

    Task<Result<string, Error>> GetPresignedUrlForDownload(
        string key,
        CancellationToken cancellationToken = default);

    Task<Result<string, Error>> StartMultipartUpload(
        StartMultipartUploadData data,
        CancellationToken cancellationToken = default);

    Task<Result<string, Error>> GetPresignedUrlForUploadPart(
        GetPresignedUrlForUploadPartData data,
        CancellationToken cancellationToken = default);

    Task<Result<CompleteMultipartUploadResponse, Error>> CompleteMultipartUpload(
        CompleteMultipartUploadData data,
        CancellationToken cancellationToken = default);

    Task<GetObjectMetadataResponse> GetObjectMetadata(
        string key,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<string>, Error>> DownloadFiles(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> DeleteObject(
        string bucketName,
        string key,
        CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> DeleteObjects(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default);
}