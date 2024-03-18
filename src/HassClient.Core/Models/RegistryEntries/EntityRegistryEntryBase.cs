using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using HassClient.Core.Models.RegistryEntries.Modifiable;

namespace HassClient.Core.Models.RegistryEntries;

/// <summary>
/// Base class that defines a entity registry entry.
/// </summary>
public abstract class EntityRegistryEntryBase
{
    [JsonPropertyName("unique_id")]
    public string UniqueId { get; protected set; }
    
    [JsonPropertyName("name"), ModifiableProperty] private string? NameBacking { get; set; }

    /// <summary>
    /// Gets a value indicating that the name of the entity registry entry can be
    /// <see langword="null"/> or whitespace. It is <see langword="false"/> by default.
    /// </summary>
    protected virtual bool AcceptsNullOrWhiteSpaceName => false;

    /// <summary>
    /// Gets the entity identifier of the entity.
    /// </summary>
    [JsonPropertyName("entity_id")]
    public string? EntityId { get; init; }

    /// <summary>
    /// Gets or sets the friendly name of this entity.
    /// </summary>
    public virtual string? Name
    {
        get => NameBacking;
        set
        {
            if (!AcceptsNullOrWhiteSpaceName &&
                string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"'{nameof(Name)}' cannot be null or whitespace.");
            }

            NameBacking = value;
        }
    }

    /// <summary>
    /// Gets or sets the icon to display in front of the entity in the front-end.
    /// </summary>
    [JsonPropertyName("icon"), ModifiableProperty]
    public virtual string? Icon { get; set; }

    [JsonConstructor]
    private protected EntityRegistryEntryBase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityRegistryEntryBase"/> class.
    /// </summary>
    /// <param name="name">The entity name.</param>
    /// <param name="icon">The entity icon.</param>
    protected EntityRegistryEntryBase(string? name, string? icon)
    {
        if (!AcceptsNullOrWhiteSpaceName &&
            string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));
        }

        Name = name;
        Icon = icon;
    }

    /// <summary>
    /// Method used by the serializer to determine if the <see cref="Icon"/> property should be serialized.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the property should be serialized; otherwise, <see langword="false"/>.
    /// </returns>
    public bool ShouldSerializeIcon() => Icon != null;
    
    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is EntityRegistryEntryBase registryEntryBase &&
               UniqueId == registryEntryBase.UniqueId;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return -401120461 + EqualityComparer<string>.Default.GetHashCode(UniqueId);
    }
}