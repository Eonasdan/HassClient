using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HassClient.Core.Models;

[PublicAPI]
public class ConfigurationModel
{
    [JsonPropertyName("latitude")] public double Latitude { get; init; }

    [JsonPropertyName("longitude")] public double Longitude { get; init; }

    /// <summary>
    /// Gets the altitude above sea level in meters of the current location.
    /// </summary>
    [JsonPropertyName("elevation")]
    public long? Elevation { get; init; }

    [JsonPropertyName("unit_system")] public UnitSystem? UnitSystem { get; init; }

    /// <summary>
    /// Gets the location's friendly name.
    /// </summary>
    [JsonPropertyName("location_name")]
    public string LocationName { get; init; } = default!;

    /// <summary>
    /// Gets the time zone name (column "TZ" from <see href="https://en.wikipedia.org/wiki/List_of_tz_database_time_zones"/>).
    /// </summary>
    [JsonPropertyName("time_zone")]
    public string? TimeZone { get; init; }

    /// <summary>
    /// Gets the list of components loaded, in the [domain] or [domain].[component] format.
    /// </summary>
    [JsonPropertyName("components")]
    public List<string> Components { get; init; } = [];

    /// <summary>
    /// Gets the relative path to the configuration directory (usually "/config").
    /// </summary>
    [JsonPropertyName("config_dir")]
    public string? ConfigDirectory { get; init; }

    [JsonPropertyName("whitelist_external_dirs")]
    public List<string> WhitelistExternalDirectories { get; init; } = [];

    /// <summary>
    /// Gets the list of folders that can be used as sources for sending files. (e.g. /config/www).
    /// </summary>
    // ReSharper disable once StringLiteralTypo
    [JsonPropertyName("allowlist_external_dirs")]
    public List<string> AllowedExternalDirs { get; init; } = [];

    /// <summary>
    /// Gets the list of external URLs that can be fetched.
    /// <para>URLs can match specific resources (e.g., "http://10.10.10.12/images/image1.jpg") or a relative path that allows
    /// access to resources within it (e.g., "http://10.10.10.12/images" would allow access to anything under that path).
    /// </para>
    /// </summary>
    // ReSharper disable once StringLiteralTypo
    [JsonPropertyName("allowlist_external_urls")]
    public List<string> AllowedExternalUrls { get; init; } = [];

    [JsonPropertyName("version")] public string? Version { get; init; }

    /// <summary>
    /// Gets the configuration source, or type of configuration file (usually "storage").
    /// </summary>
    [JsonPropertyName("config_source")]
    public string? ConfigSource { get; init; }

    [JsonPropertyName("recovery_mode")] public bool? RecoveryMode { get; init; }

    /// <summary>
    /// Gets the current state of Home Assistant (usually "RUNNING").
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; init; }

    /// <summary>
    /// Gets the URL that Home Assistant is available on from the Internet (e.g. "https://example.duckdns.org:8123").
    /// </summary>
    [JsonPropertyName("external_url")]
    public Uri? ExternalUrl { get; init; }

    /// <summary>
    /// Gets the URL that Home Assistant is available on from the local network (e.g. "http://homeassistant.local:8123").
    /// </summary>
    [JsonPropertyName("internal_url")]
    public Uri? InternalUrl { get; init; }

    /// <summary>
    /// Gets the currency code according to ISO 4217 (column "Code" from <see href="https://en.wikipedia.org/wiki/ISO_4217#Active_codes"/>).
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("country")] public string? Country { get; init; }

    [JsonPropertyName("language")] public string? Language { get; init; }

    /// <summary>
    /// Gets a value indicating whether Home Assistant is running in safe mode.
    /// </summary>
    [JsonPropertyName("safe_mode")]
    public bool? SafeMode { get; init; }

    public override string ToString() => LocationName;
}

public class UnitSystem
{
    /// <summary>
    /// Gets the length unit (usually "km" or "mi").
    /// </summary>
    [JsonPropertyName("length")]
    public string? Length { get; init; }

    [JsonPropertyName("accumulated_precipitation")]
    public string? AccumulatedPrecipitation { get; init; }

    /// <summary>
    /// Gets the mass unit (usually "g" or "lb").
    /// </summary>
    [JsonPropertyName("mass")]
    public string? Mass { get; init; }

    /// <summary>
    /// Gets the pressure unit (usually "Pa" or "psi").
    /// </summary>
    [JsonPropertyName("pressure")]
    public string? Pressure { get; init; }

    /// <summary>
    /// Gets the temperature unit including degree symbol (usually "°C" or "°F").
    /// </summary>
    [JsonPropertyName("temperature")]
    public string? Temperature { get; init; }

    /// <summary>
    /// Gets the volume unit (usually "L" or "gal").
    /// </summary>
    [JsonPropertyName("volume")]
    public string? Volume { get; init; }

    /// <summary>
    /// Gets the wind speed unit (usually "m/s" or "mi/h").
    /// </summary>
    [JsonPropertyName("wind_speed")]
    public string? WindSpeed { get; init; }
}