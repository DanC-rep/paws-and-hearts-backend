using System.Net;
using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;

namespace FileService.Communication;

public class FileHttpClient(HttpClient httpClient) : IFileService
{
    public async Task<Result<FileResponse, string>> DownloadFilePresignedUrl(
        Guid id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"files/{id}/presigned", cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return "Fail to get file presigned url";

        var fileResponse = await response.Content.ReadFromJsonAsync<FileResponse>(cancellationToken);

        return fileResponse!;
    }

    public async Task<Result<StartMultipartUploadResponse, string>> StartMultipartUpload(
        StartMultipartUploadRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("files/multipart", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return "Fail to start multipart upload";

        var startMultipartUploadResponse = await response.Content
            .ReadFromJsonAsync<StartMultipartUploadResponse>(cancellationToken);

        return startMultipartUploadResponse!;
    }

    public async Task<Result<UploadPresignedPartUrlResponse, string>> UploadPresignedPartUrl(
        UploadPresignedPartUrlRequest request, string key, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"files/{key}/presigned-part", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return "Fail to get presigned url for part";

        var uploadPresignedPartUrlResponse = await response.Content
            .ReadFromJsonAsync<UploadPresignedPartUrlResponse>(cancellationToken);

        return uploadPresignedPartUrlResponse!;
    }

    public async Task<Result<CompleteMultipartUploadResponse, string>> CompleteMultipartUpload(
        CompleteMultipartRequest request, string key, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"files/{key}/complete-multipart", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return "Fail to complete multipart upload";

        var completeMultipartUploadResponse = await response.Content
            .ReadFromJsonAsync<CompleteMultipartUploadResponse>(cancellationToken);

        return completeMultipartUploadResponse!;
    }

    public async Task<Result<UploadPresignedUrlResponse, string>> UploadPresignedUrl(
        UploadPresignedUrlRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("files/presigned", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return "Fail to get presigned url for upload file";

        var uploadPresignedUrlResponse = await response.Content
            .ReadFromJsonAsync<UploadPresignedUrlResponse>(cancellationToken);

        return uploadPresignedUrlResponse!;
    }

    public async Task<Result<GetFilesByIdsResponse, string>> GetFilesByIds(
        GetFilesByIdsRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("files", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return "Fail to get files";

        var getFilesByIdsResponse = await response.Content
            .ReadFromJsonAsync<GetFilesByIdsResponse>(cancellationToken);

        return getFilesByIdsResponse!;
    }

    public async Task<UnitResult<string>> DeleteFiles(
        DeleteFilesRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("files/delete", request, cancellationToken);
            
        if (response.StatusCode != HttpStatusCode.OK)
            return "Fail to delete files";

        return Result.Success<string>();
    }
    
    public async Task<UnitResult<string>> DeleteFile(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"files/{id}/delete", cancellationToken);
            
        if (response.StatusCode != HttpStatusCode.OK)
            return "Fail to delete file";

        return Result.Success<string>();
    }
}