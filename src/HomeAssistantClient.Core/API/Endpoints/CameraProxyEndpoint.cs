using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Endpoints;

/// <summary>
/// Provides access to the camera proxy API which allows fetching of the current image from a camera entity.
/// </summary>
[PublicAPI]
public class CameraProxyEndpoint(JsonClient client)
{
    private const string BaseUrl = "camera_proxy";
    
    /// <summary>
    /// Retrieves the most recently available (still) image data from the specified <paramref name="cameraEntityId" />.
    /// </summary>
    /// <param name="cameraEntityId">The camera entity ID to retrieve the image for.</param>
    /// <returns>A byte array containing the still image, typically in JPEG format.</returns>
    public Task<byte[]?> GetCameraImageAsync(string cameraEntityId) => 
        client.GetAsync<byte[]>($"{BaseUrl}/{cameraEntityId}");

    /// <summary>
    /// Retrieves the most recently available (still) image data from the specified <paramref name="cameraEntityId" />.
    /// </summary>
    /// <param name="cameraEntityId">The camera entity ID to retrieve the image for.</param>
    /// <param name="includeDataPrefix"><c>true</c> to include the prefix "data:image/jpg;base64,", <c>false</c> to omit. Defaults to <c>true</c>.</param>
    /// <returns>A web-friendly Base64-encoded still image, in JPEG format.</returns>
    public async Task<string> GetCameraImageAsBase64(string cameraEntityId, bool includeDataPrefix = true) => (includeDataPrefix ? "data:image/jpg;base64," : "") + Convert.ToBase64String(await GetCameraImageAsync(cameraEntityId) ?? Array.Empty<byte>());
}