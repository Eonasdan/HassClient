using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Models;

/// <summary>
/// Represents a discovery info object, used to convey basic information about the HA instance.
/// </summary>
[PublicAPI]
public class Discovery
{
    /// <summary>
    /// Gets or sets the Base URL for the instance (e.g. http://192.168.0.2:8123).
    /// </summary>
    [JsonPropertyName("base_url")]
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the location name for this instance (e.g. "Home").
    /// </summary>
    [JsonPropertyName("location_name")]
    public string? LocationName { get; set; }

    /// <summary>
    /// Gets or sets whether or not a password is required to use the API. (should most always be "true").
    /// </summary>
    [JsonPropertyName("requires_api_password")]
    public bool RequiresApiPassword { get; set; }

    /// <summary>
    /// Gets or sets the current version of Home Assistant (e.g. 0.96.1).
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets a string representation of this object.
    /// </summary>
    public override string ToString()
    {
            return $"Discovery: {LocationName} ({BaseUrl})";
        }
}