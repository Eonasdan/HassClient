using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Models;

/// <summary>
/// Represents Host info object.
/// </summary>
[PublicAPI]
public class HostInfo
{
    /// <summary>
    /// Gets or sets the chassis type.
    /// </summary>
    [JsonPropertyName("chassis")]
    public string? Chassis { get; set; }

    /// <summary>
    /// Gets or sets the CPE string.
    /// </summary>
    [JsonPropertyName("cpe")]
    public string? Cpe { get; set; }

    /// <summary>
    /// Gets or sets the deployment type (e.g. production).
    /// </summary>
    [JsonPropertyName("deployment")]
    public string? Deployment { get; set; }

    /// <summary>
    /// Gets or sets the disk free, expressed in GB.
    /// </summary>
    [JsonPropertyName("disk_free")]
    public double DiskFree { get; set; }

    /// <summary>
    /// Gets or sets the disk total, expressed in GB.
    /// </summary>
    [JsonPropertyName("disk_total")]
    public double DiskTotal { get; set; }

    /// <summary>
    /// Gets or sets the disk used, expressed in GB.
    /// </summary>
    [JsonPropertyName("disk_used")]
    public double DiskUsed { get; set; }

    /// <summary>
    /// Gets or sets the feature list.
    /// </summary>
    [JsonPropertyName("features")]
    public IEnumerable<string> Features { get; set; } = [];

    /// <summary>
    /// Gets or sets the hostname.
    /// </summary>
    [JsonPropertyName("hostname")]
    public string? Hostname { get; set; }

    /// <summary>
    /// Gets or sets the kernel version.
    /// </summary>
    [JsonPropertyName("kernel")]
    public string? Kernel { get; set; }

    /// <summary>
    /// Gets or sets the operating system.
    /// </summary>
    [JsonPropertyName("operating_system")]
    public string? OperatingSystem { get; set; }
}