using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HassClient.Core.API.Models;

/// <summary>
/// Represents Core info object.
/// </summary>
[PublicAPI]
public class CoreInfo
{
    /// <summary>
    /// Gets or sets the processor architecture.
    /// </summary>
    [JsonPropertyName("arch")]
    public string? Arch { get; set; }

    /// <summary>
    /// Gets or sets the audio input.
    /// </summary>
    [JsonPropertyName("audio_input")]
    public string? AudioInput { get; set; }

    /// <summary>
    /// Gets or sets the audio output.
    /// </summary>
    [JsonPropertyName("audio_output")]
    public string? AudioOutput { get; set; }

    /// <summary>
    /// <code>True</code> if booted, otherwise <code>False</code>.
    /// </summary>
    [JsonPropertyName("boot")]
    public bool Boot { get; set; }

    /// <summary>
    /// Gets or sets the image.
    /// </summary>
    [JsonPropertyName("image")]
    public string? Image { get; set; }

    /// <summary>
    /// Gets or sets the IP address.
    /// </summary>
    [JsonPropertyName("ip_address")]
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the last_version.
    /// </summary>
    [JsonPropertyName("last_version")]
    public string? LastVersion { get; set; }

    /// <summary>
    /// Gets or sets the machine.
    /// </summary>
    [JsonPropertyName("machine")]
    public string? Machine { get; set; }

    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    [JsonPropertyName("port")]
    public int Port { get; set; }

    /// <summary>
    /// <code>True</code> if SSL is enabled, otherwise <code>False</code>.
    /// </summary>
    [JsonPropertyName("ssl")]
    public bool Ssl { get; set; }

    /// <summary>
    /// <code>True</code> if update is available, otherwise <code>False</code>.
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

    /// <summary>
    /// Gets or sets the wait boot timeout.
    /// </summary>
    [JsonPropertyName("wait_boot")]
    public int WaitBoot { get; set; }

    /// <summary>
    /// <code>True</code> if watchdog is enabled, otherwise <code>False</code>.
    /// </summary>
    [JsonPropertyName("watchdog")]
    public bool Watchdog { get; set; }
}