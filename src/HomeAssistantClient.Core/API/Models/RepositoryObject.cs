using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Models;

[PublicAPI]
public class RepositoryObject
{
    /// <summary>
    /// The name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The slug.
    /// </summary>
    [JsonPropertyName("slug")]
    public string? Slug { get; set; }
}