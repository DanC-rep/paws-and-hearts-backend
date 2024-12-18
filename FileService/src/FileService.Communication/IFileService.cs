using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;

namespace FileService.Communication;

public interface IFileService
{
    Task<Result<FileResponse, string>> DownloadFilePresignedUrl(
        Guid id, CancellationToken cancellationToken = default);

    Task<Result<StartMultipartUploadResponse, string>> StartMultipartUpload(
        StartMultipartUploadRequest request, CancellationToken cancellationToken = default);

    Task<Result<UploadPresignedPartUrlResponse, string>> UploadPresignedPartUrl(
        UploadPresignedPartUrlRequest request, string key, CancellationToken cancellationToken = default);

    Task<Result<CompleteMultipartUploadResponse, string>> CompleteMultipartUpload(
        CompleteMultipartRequest request, string key, CancellationToken cancellationToken = default);

    Task<Result<UploadPresignedUrlResponse, string>> UploadPresignedUrl(
        UploadPresignedUrlRequest request, CancellationToken cancellationToken = default);

    Task<Result<GetFilesByIdsResponse, string>> GetFilesByIds(
        GetFilesByIdsRequest request, CancellationToken cancellationToken = default);
}