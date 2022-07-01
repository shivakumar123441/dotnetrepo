namespace InvestTrackerWebApi.Application.ImageStorage;

public interface IImageStorageService
{
    public Task<string> UploadAsync<T>(
        ImageUploadRequest? request,
        CancellationToken cancellationToken = default)
    where T : class;
}
