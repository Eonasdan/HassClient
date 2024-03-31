using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HassClient.Core.API.Models;

/// <summary>
/// Represents Home Assistant response.
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public class ResponseObject<T>
{
    /// <summary>
    /// Gets or sets the result.
    /// </summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}