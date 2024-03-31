using System.Text.Json.Serialization;

namespace HomeAssistantClient.Core.API.Models;

/// <summary>
/// Represents Supervisor info object.
/// </summary>
public class SupervisorInfoObject
{
    /// <summary>
    /// Gets or sets the addons.
    /// </summary>
    [JsonPropertyName("addons")]
    public IList<Addon> Addons { get; set; }

    /// <summary>
    /// Gets or sets the addons repositories.
    /// </summary>
    [JsonPropertyName("addons_repositories")]
    public IList<string> AddonsRepositories { get; set; }

    /// <summary>
    /// Gets or sets the processor architecture.
    /// </summary>
    [JsonPropertyName("arch")]
    public string? Arch { get; set; }

    /// <summary>
    /// Gets or sets the processor channel.
    /// </summary>
    [JsonPropertyName("channel")]
    public string? Channel { get; set; }

    /// <summary>
    /// <code>True</code> if debug, otherwise <code>False</code>.
    /// </summary>
    [JsonPropertyName("debug")]
    public bool Debug { get; set; }

    /// <summary>
    /// <code>True</code> if debug block, otherwise <code>False</code>.
    /// </summary>
    [JsonPropertyName("debug_block")]
    public bool DebugBlock { get; set; }

    /// <summary>
    /// <code>True</code> if diagnostics is available, otherwise <code>False</code>.
    /// </summary>
    [JsonPropertyName("diagnostics")]
    public bool Diagnostics { get; set; }

    /// <summary>
    /// <code>True</code> if healthy, otherwise <code>False</code>.
    /// </summary>
    [JsonPropertyName("healthy")]
    public bool Healthy { get; set; }

    /// <summary>
    /// Gets or sets the IP address.
    /// </summary>
    [JsonPropertyName("ip_address")]
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the logging.
    /// </summary>
    [JsonPropertyName("logging")]
    public string? Logging { get; set; }

    /// <summary>
    /// <code>True</code> if supported, otherwise <code>False</code>.
    /// </summary>
    [JsonPropertyName("supported")]
    public bool Supported { get; set; }

    /// <summary>
    /// Gets or sets the timezone.
    /// </summary>
    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

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
}