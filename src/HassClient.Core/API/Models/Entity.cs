using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HassClient.Core.API.Models;

/// <summary>
/// Represents a single entity's state.
/// </summary>
[PublicAPI]
public class Entity : BaseState
{
    /// <summary>
    /// Gets or sets the entity's current attributes and values.
    /// </summary>
    public Dictionary<string, object> Attributes { get; set; } = [];

    /// <summary>
    /// Attempts to get the value of the specified attribute by <paramref name="name" />, and cast the value to type <typeparamref name="T" />.
    /// </summary>
    /// <exception cref="InvalidCastException">Thrown when the specified type <typeparamref name="T" /> cannot be cast to the attribute's current value.</exception>
    /// <typeparam name="T">The desired type to cast the attribute value to.</typeparam>
    /// <param name="name">The name of the attribute to retrieve the value for.</param>
    /// <returns>The attribute's current value, cast to type <typeparamref name="T" />.</returns>
    public T? GetAttributeValue<T>(string name)
    {
            if (!Attributes.TryGetValue(name, out var value))
            {
                return default;
            }

            return value switch
            {
                JsonElement jsonElement => JsonSerializer.Deserialize<T>(jsonElement.GetRawText()),
                string json => JsonSerializer.Deserialize<T>(json),
                _ => (T)value
            };
        }

    /// <summary>
    /// Gets a string representation of this entity's state.
    /// </summary>
    public override string ToString() => $"{EntityId}: {State}";
}