namespace InvestTrackerWebApi.Infrastructure.ImageStorage;

using System.Text.RegularExpressions;
using Imgur.API.Authentication;
using Imgur.API.Endpoints;
using InvestTrackerWebApi.Application.ImageStorage;

public class ImgurStorageService : IImageStorageService
{
    public async Task<string> UploadAsync<T>(
        ImageUploadRequest? request,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (request == null || request.Data == null)
        {
            return string.Empty;
        }

        if (request.Extension is null)
        {
            throw new InvalidOperationException("File Format Not Supported.");
        }

        if (request.Name is null)
        {
            throw new InvalidOperationException("Name is required.");
        }

        string base64Data = Regex.Match(request.Data, "data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

        var streamData = new MemoryStream(Convert.FromBase64String(base64Data));
        if (streamData.Length > 0)
        {
            var apiClient = new ApiClient("702491963783e6f", "eacbb4a9d3a5d26ac6e4149034a1fae3974ee6e9");
            var httpClient = new HttpClient();

            var imageEndpoint = new ImageEndpoint(apiClient, httpClient);
            var imageUpload = await imageEndpoint.UploadImageAsync(streamData);
            return imageUpload.Link;
        }
        else
        {
            return string.Empty;
        }
    }
}
