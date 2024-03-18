using System.Text.Json.Serialization;

namespace HassClient.Core.Models;

/// <summary>
/// Represents the manifest that specify basic information about an integration.
/// </summary>
public class IntegrationManifest
{
    /// <summary>
    /// Gets a value indicating whether the integration is maintained by Home Assistant Core.
    /// </summary>
    [JsonPropertyName("IsBuiltIn")]
    public bool IsBuiltIn { get; init; }

    /// <summary>
    /// Gets an unique short name consisting of characters and underscores (e.g. "mobile_app").
    /// </summary>
    [JsonPropertyName("Domain")]
    public string? Domain { get; init; }

    /// <summary>
    /// Gets a friendly name for the integration.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the website containing documentation on how to use the integration.
    /// </summary>
    [JsonPropertyName("Documentation")]
    public string? Documentation { get; init; }

    /// <summary>
    /// Gets the issue tracker where users reports issues if they run into one. This is only defined for not
    /// <see cref="IsBuiltIn"/> integrations.
    /// </summary>
    [JsonPropertyName("IssueTracker")]
    public string? IssueTracker { get; init; }

    /// <summary>
    /// Gets a list of other integrations that need to set up successfully prior to the integration being loaded.
    /// </summary>
    [JsonPropertyName("Dependencies")]
    public string[] Dependencies { get; init; }

    /// <summary>
    /// Gets a list of optional dependencies that might be used by this integration.
    /// </summary>
    [JsonPropertyName("AfterDependencies")]
    public string[] AfterDependencies { get; init; }

    /// <summary>
    /// Gets a list of GitHub usernames or team names of people that are responsible for this integration.
    /// </summary>
    [JsonPropertyName("CodeOwners")]
    public string[] CodeOwners { get; init; }

    /// <summary>
    /// Gets a value indicating whether the integration has a configuration flow to create a config entry.
    /// </summary>
    [JsonPropertyName("ConfigFlow")]
    public bool ConfigFlow { get; init; }

    /// <summary>
    /// Gets a list of Python libraries or modules needed by this integration.
    /// </summary>
    [JsonPropertyName("Requirements")]
    public string[] Requirements { get; init; }

    /// <summary>
    /// Gets an value that scores an integration on the code quality and user experience.
    /// <para>
    /// More info at <see href="https://developers.home-assistant.io/docs/integration_quality_scale_index"/>.
    /// </para>
    /// </summary>
    [JsonPropertyName("QualityScale")]
    public string? QualityScale { get; init; }

    /// <summary>
    /// Gets the version number from which this integration is available or compatible.
    /// </summary>
    [JsonPropertyName("homeassistant")]
    public CalendarVersion SinceHassVersion { get; private set; }

    /// <inheritdoc />
    public override string ToString() => Name;
}