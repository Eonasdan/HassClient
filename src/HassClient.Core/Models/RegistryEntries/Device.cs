using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HassClient.Core.Models.RegistryEntries;

/// <summary>
/// Represents a device.
/// <para>
/// More information at <see href="https://developers.home-assistant.io/docs/device_registry_index/"/>.
/// </para>
/// </summary>
[PublicAPI]
public class Device
{
    [JsonPropertyName("area_id"), ModifiableProperty]
    public string? AreaId { get; init; }

    [JsonPropertyName("configuration_url")]
    public string? ConfigurationUrl { get; init; }

    [JsonPropertyName("config_entries")] public List<string> ConfigEntries { get; init; } = [];

    [JsonPropertyName("connections")] public List<Tuple<string, string>> Connections { get; init; } = [];

    /// <summary>
    /// Gets a value indicating the disabling source, if any.
    /// </summary>
    [JsonPropertyName("disabled_by"), ModifiableProperty]
    public DisabledByEnum DisabledBy { get; init; } = DisabledByEnum.None;

    /// <summary>
    /// Gets a value indicating whether the device is disabled.
    /// </summary>
    [JsonIgnore]
    public bool IsDisabled => DisabledBy != DisabledByEnum.None;

    [JsonPropertyName("entry_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DeviceEntryTypes EntryType { get; init; } = DeviceEntryTypes.None;

    [JsonPropertyName("hw_version")] public string? HwVersion { get; init; }

    [JsonPropertyName("id")] public string Id { get; init; } = default!;

    [JsonPropertyName("identifiers")] public List<Tuple<string, string>> Identifiers { get; init; } = [];

    [JsonPropertyName("manufacturer")] public string? Manufacturer { get; init; }

    [JsonPropertyName("model")] public string? Model { get; init; }

    [JsonPropertyName("name_by_user"), ModifiableProperty]
    public string? NameByUser { get; set; }

    [JsonPropertyName("name")] private string OriginalName { get; init; } = default!;
    
    /// <summary>
    /// Gets the current name of this device.
    /// It will the one given by the user after creation; otherwise, <see cref="OriginalName"/>.
    /// <para>
    /// If set to <see langword="null"/>, the <see cref="OriginalName"/> will be used.
    /// </para>
    /// </summary>
    [JsonIgnore]
    public string Name
    {
        get => NameByUser ?? OriginalName;
        set => NameByUser = value == OriginalName ? null : value;
    }

    [JsonPropertyName("serial_number")] public string? SerialNumber { get; init; }

    [JsonPropertyName("sw_version")] 
    public string[] SwVersion { get; init; } = [];

    [JsonPropertyName("via_device_id")] public string? ViaDeviceId { get; init; }
    
    public override string ToString() => $"{nameof(Device)}: {Name}";

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Device device &&
               Id == device.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return -401120461 + EqualityComparer<string>.Default.GetHashCode(Id);
    }
}

/// <summary>
/// Defines the device entry type possible values.
/// </summary>
[PublicAPI]
public enum DeviceEntryTypes
{
    /// <summary>
    /// Device has not defined type.
    /// </summary>
    None,

    /// <summary>
    /// Device is a service entry type.
    /// </summary>
    Service,
}

public class ModifiablePropertyAttribute : Attribute;