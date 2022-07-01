namespace InvestTrackerWebApi.Infrastructure.FileStorage;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using InvestTrackerWebApi.Application.FileStorage;
using InvestTrackerWebApi.Domain.FileStorage;

public class AzureBlobStorageService : IFileStorageService
{
    private BlobContainerClient? blobContainerClient = null;

    private BlobContainerClient GetBlobContainerClient()
    {
        if (this.blobContainerClient is null)
        {
            string connection = @"DefaultEndpointsProtocol=https;AccountName=investtrackersa;AccountKey=IWTUVlVi3zcks6+O6bAD6nVBfDf24RlWiWekFqWfnUD4js5FRnieCIgFVF1ni/5G99a/LxbpFDc3xqS+2EXfcw==;EndpointSuffix=core.windows.net";
            string containerName = "useruploads";

            this.blobContainerClient = new BlobContainerClient(connection, containerName);
        }

        return this.blobContainerClient;
    }

    public async Task<AttachmentStorageInfo?> UploadAsync<T>(Attachment? attachment, string folderPath, CancellationToken cancellationToken = default)
        where T : class
    {
        if (attachment == null || attachment.Contents == null)
        {
            throw new InvalidOperationException("File Attachment Not Provided .");
        }

        if (attachment.ContentType is null)
        {
            throw new InvalidOperationException("ContentType Not Supported.");
        }

        if (attachment.Name is null)
        {
            throw new InvalidOperationException("Name is required.");
        }

        var blobContainerClient = this.GetBlobContainerClient();
        var blobClient = blobContainerClient.GetBlobClient(Path.Combine(folderPath, attachment.Name));
        attachment.Contents.Position = 0;
        var httpHeaders = new BlobHttpHeaders
        {
            ContentType = attachment.ContentType,
        };
        var result = await blobClient.UploadAsync(attachment.Contents, httpHeaders, cancellationToken: cancellationToken);

        if (result.GetRawResponse().Status is >= 200 and <= 299)
        {
            return new AttachmentStorageInfo()
            {
                Id = Guid.NewGuid(),
                FileName = attachment.Name,
                UploadedFileUrl = blobClient.Uri.AbsoluteUri,
            };
        }

        return null;
    }
}
