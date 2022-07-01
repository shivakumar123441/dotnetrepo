namespace InvestTrackerWebApi.Application.FileStorage;

using InvestTrackerWebApi.Domain.FileStorage;

public interface IFileStorageService
{
    public Task<AttachmentStorageInfo?> UploadAsync<T>(
        Attachment? attachment, string folderToUpload, CancellationToken cancellationToken = default)
    where T : class;
}
