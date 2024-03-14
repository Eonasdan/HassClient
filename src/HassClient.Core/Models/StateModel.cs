using System;
using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Helpers;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.Core.Models
{
    /// <summary>
    /// Represents a single entity's state.
    /// </summary>
    public class StateModel
    {
        /// <summary>
        /// Gets the Entity ID that this state represents.
        /// </summary>
        [JsonPropertyName("EntityId")]
        public string? EntityId { get; init; }

        /// <summary>
        /// Gets the string representation of the state that this entity is currently in.
        /// </summary>
        [JsonPropertyName("State")]
        public string? State { get; init; }

        /// <summary>
        /// Gets the state that this entity is currently in as a <see cref="KnownStates"/>.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public KnownStates KnownState => State.AsKnownState();

        /// <summary>
        /// Gets the entity's current attributes and values.
        /// </summary>
        [JsonPropertyName("Attributes")]
        public List<Tuple<string, JRaw>> Attributes { get; init; }

        /// <summary>
        /// Gets the context for this entity's state.
        /// </summary>
        [JsonProperty]
        public Context Context { get; internal set; }

        /// <summary>
        /// Gets the UTC date and time that this state was last changed.
        /// </summary>
        [JsonPropertyName("LastChanged")]
        public DateTimeOffset LastChanged { get; init; }

        /// <summary>
        /// Gets the UTC date and time that this state was last updated.
        /// </summary>
        [JsonPropertyName("LastUpdated")]
        public DateTimeOffset LastUpdated { get; init; }

        /// <summary>
        /// Attempts to get the value of the specified attribute by <paramref name="name"/>,
        /// and cast the value to type <typeparamref name="T" />.
        /// </summary>
        /// <exception cref="InvalidCastException">Thrown when the specified type <typeparamref name="T"/>
        /// cannot be cast to the attribute's current value.</exception>
        /// <typeparam name="T">The desired type to cast the attribute value to.</typeparam>
        /// <param name="name">The name of the attribute to retrieve the value for.</param>
        /// <returns>The attribute's current value, cast to type <typeparamref name="T" />.</returns>
        public T GetAttributeValue<T>(string name) => Attributes.All(x => x.Item1 != name) ? default : HassSerializer.DeserializeObject<T>(Attributes.First(x => x.Item1 == name).Item2); //todo this was a dictionary

        /// <summary>
        /// Attempts to get the values of the specified attribute by <paramref name="name"/> as an
        /// <see cref="IEnumerable{T}"/>.
        /// <para>
        /// If the attribute is not defined, an empty enumeration will be returned.
        /// </para>
        /// </summary>
        /// <exception cref="InvalidCastException">Thrown when the specified type <typeparamref name="T"/>
        /// cannot be cast to the attribute's current value.</exception>
        /// <typeparam name="T">The desired type to cast the attribute values to.</typeparam>
        /// <param name="name">The name of the attribute to retrieve the values for.</param>
        /// <returns>
        /// An enumeration containing attribute's current values, cast to type <typeparamref name="T" />.
        /// </returns>
        public IEnumerable<T> GetAttributeValues<T>(string name)
        {
            return GetAttributeValue<IEnumerable<T>>(name) ?? Enumerable.Empty<T>();
        }

        internal TEnum GetAttributeValue<TEnum>(string name, KnownEnumCache<TEnum> knownEnumCache)
            where TEnum : struct, Enum
        {
            return knownEnumCache.AsEnum(GetAttributeValue<string>(name));
        }

        /// <inheritdoc />
        public override string ToString() => $"{EntityId}: {State}";
    }
}
