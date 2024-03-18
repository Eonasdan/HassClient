using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries.Modifiable;
using System.Text.Json.Serialization;

namespace HassClient.Core.Models.RegistryEntries.StorageEntities;

/// <summary>
/// Represents a zone.
/// </summary>
[StorageEntityDomain(KnownDomains.Zone)]
public class Zone : StorageEntityRegistryEntryBase
{
    private readonly ModifiableProperty<float> _latitude = new(nameof(Latitude));

    private readonly ModifiableProperty<float> _longitude = new(nameof(Longitude));

    private readonly ModifiableProperty<bool> _passive = new(nameof(IsPassive));

    private readonly ModifiableProperty<float> _radius = new(nameof(Radius));

    /// <summary>
    /// Gets or sets the latitude of the center point of the zone.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float Latitude
    {
        get => _latitude.Value;
        init => _latitude.Value = value;
    }

    /// <summary>
    /// Gets or sets the longitude of the center point of the zone.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float Longitude
    {
        get => _longitude.Value;
        init => _longitude.Value = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the zone will be used only for automation and hide it
    /// from the frontend and not use the zone for device tracker name.
    /// </summary>
    [JsonPropertyName("passive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool IsPassive
    {
        get => _passive.Value;
        init => _passive.Value = value;
    }

    /// <summary>
    /// Gets or sets the radius of the zone in meters.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float Radius
    {
        get => _radius.Value;
        init => _radius.Value = value;
    }

    [JsonConstructor]
    private Zone()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Zone"/> class.
    /// </summary>
    /// <param name="name">The entity name.</param>
    /// <param name="longitude">The longitude of the center point of the zone.</param>
    /// <param name="latitude">The latitude of the center point of the zone.</param>
    /// <param name="radius">the radius of the zone in meters.</param>
    /// <param name="icon">The entity icon.</param>
    /// <param name="isPassive">
    /// Whether the zone will be used only for automation and hide it
    /// from the frontend and not use the zone for device tracker name.
    /// </param>
    public Zone(string? name, float longitude, float latitude, float radius, string? icon = null, bool isPassive = false)
        : base(name, icon)
    {
        Longitude = longitude;
        Latitude = latitude;
        Radius = radius;
        IsPassive = isPassive;
    }

    // Used for testing purposes.
    internal static Zone CreateUnmodified(string? uniqueId, string? name, float longitude, float latitude, float radius, string? icon = null, bool isPassive = false)
    {
        var result = new Zone(name, longitude, latitude, radius, icon, isPassive) { Id = uniqueId };
        result.SaveChanges();
        return result;
    }

    /// <inheritdoc />
    protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
    {
        return base.GetModifiableProperties()
            .Append(_latitude)
            .Append(_longitude)
            .Append(_passive)
            .Append(_radius);
    }

    /// <inheritdoc />
    public override string ToString() => $"{nameof(Zone)}: {Name}";

    // Used for testing purposes.
    internal Zone? Clone()
    {
        var result = CreateUnmodified(UniqueId, Name, Longitude, Latitude, Radius, Icon, IsPassive);
        return result;
    }
}