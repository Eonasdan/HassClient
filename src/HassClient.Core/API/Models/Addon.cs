using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HassClient.Core.API.Models;

/// <summary>
/// Represents Add-on object.
/// </summary>
[PublicAPI]
public class Addon
{
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// <c>True</c> if icon is available, otherwise <c>False</c>.
    /// </summary>
    [JsonPropertyName("icon")]
    public bool Icon { get; set; }

    /// <summary>
    /// <c>True</c> if logo is available, otherwise <c>False</c>.
    /// </summary>
    [JsonPropertyName("logo")]
    public bool Logo { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the repository.
    /// </summary>
    [JsonPropertyName("repository")]
    public string? Repository { get; set; }

    /// <summary>
    /// Gets or sets the slug.
    /// </summary>
    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// <c>True</c> if update is available, otherwise <c>False</c>.
    /// </summary>
    [JsonPropertyName("update_available")]
    public bool UpdateAvailable { get; set; }

    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets the latest version.
    /// </summary>
    [JsonPropertyName("version_latest")]
    public string? VersionLatest { get; set; }
}