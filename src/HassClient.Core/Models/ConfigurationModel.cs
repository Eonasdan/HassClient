using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HassClient.Core.Models
{
    /// <summary>
    /// Represents the Home Assistant configuration object.
    /// </summary>
    public class ConfigurationModel
    {
        
        [JsonPropertyName("whitelist_external_dirs")]
        private string[] WhitelistExternalDirs
        {
            get => AllowedExternalDirs?.ToArray();
            set => AllowedExternalDirs = AllowedExternalDirs ?? value.ToList();
        }

        [JsonPropertyName("allowlist_external_dirs")]
        private string[] AllowListExternalDirs
        {
            get => AllowedExternalDirs?.ToArray();
            set => AllowedExternalDirs = AllowedExternalDirs ?? value.ToList();
        }

        /// <summary>
        /// Gets the latitude of the current location.
        /// </summary>
        [JsonPropertyName("Latitude")]
        public float Latitude { get; init; }

        /// <summary>
        /// Gets the longitude of the current location.
        /// </summary>
        [JsonPropertyName("Longitude")]
        public float Longitude { get; init; }

        /// <summary>
        /// Gets the altitude above sea level in meters of the current location.
        /// </summary>
        [JsonPropertyName("Elevation")]
        public int Elevation { get; init; }

        /// <summary>
        /// Gets a container for units of measure.
        /// </summary>
        [JsonPropertyName("UnitSystem")]
        public UnitSystemModel UnitSystem { get; init; }

        /// <summary>
        /// Gets the location's friendly name.
        /// </summary>
        [JsonPropertyName("LocationName")]
        public string? LocationName { get; init; }

        /// <summary>
        /// Gets the time zone name (column "TZ" from <see href="https://en.wikipedia.org/wiki/List_of_tz_database_time_zones"/>).
        /// </summary>
        [JsonPropertyName("TimeZone")]
        public string? TimeZone { get; init; }

        /// <summary>
        /// Gets the list of components loaded, in the [domain] or [domain].[component] format.
        /// </summary>
        [JsonPropertyName("Components")]
        public List<string> Components { get; init; }

        /// <summary>
        /// Gets the relative path to the configuration directory (usually "/config").
        /// </summary>
        [JsonPropertyName("config_dir")]
        public string? ConfigDirectory { get; private set; }

        /// <summary>
        /// Gets the list of folders that can be used as sources for sending files. (e.g. /config/www).
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public List<string> AllowedExternalDirs { get; private set; }

        /// <summary>
        /// Gets the list of external URLs that can be fetched.
        /// <para>URLs can match specific resources (e.g., "http://10.10.10.12/images/image1.jpg") or a relative path that allows
        /// access to resources within it (e.g., "http://10.10.10.12/images" would allow access to anything under that path).
        /// </para>
        /// </summary>
        [JsonPropertyName("allowlist_external_urls")]
        public List<string> AllowedExternalUrls { get; private set; }

        /// <summary>
        /// Gets the version of Home Assistant that is currently running.
        /// </summary>
        [JsonPropertyName("Version")]
        public CalendarVersion Version { get; init; }

        /// <summary>
        /// Gets the configuration source, or type of configuration file (usually "storage").
        /// </summary>
        [JsonPropertyName("ConfigSource")]
        public string? ConfigSource { get; init; }

        /// <summary>
        /// Gets a value indicating whether Home Assistant is running in safe mode.
        /// </summary>
        [JsonPropertyName("SafeMode")]
        public bool SafeMode { get; init; }

        /// <summary>
        /// Gets the current state of Home Assistant (usually "RUNNING").
        /// </summary>
        [JsonPropertyName("State")]
        public string? State { get; init; }

        /// <summary>
        /// Gets the URL that Home Assistant is available on from the Internet (e.g. "https://example.duckdns.org:8123").
        /// </summary>
        [JsonPropertyName("ExternalUrl")]
        public string? ExternalUrl { get; init; }

        /// <summary>
        /// Gets the URL that Home Assistant is available on from the local network (e.g. "http://homeassistant.local:8123").
        /// </summary>
        [JsonPropertyName("InternalUrl")]
        public string? InternalUrl { get; init; }

        /// <summary>
        /// Gets the currency code according to ISO 4217 (column "Code" from <see href="https://en.wikipedia.org/wiki/ISO_4217#Active_codes"/>).
        /// </summary>
        public string? Currency { get; private set; }

        /// <inheritdoc />
        public override string ToString() => LocationName;
    }
}
