using System;
using System.Text.Json.Serialization;
using HassClient.Core.Models.KnownEnums;

namespace HassClient.Core.API.Models;

public class BaseState
{
    /// <summary>
    /// Gets or sets the Entity ID that this state represents.
    /// </summary>
    [JsonPropertyName("entity_id")]
    public string? EntityId { get; set; }

    /// <summary>
    /// Gets or sets the string representation of the state that this entity is currently in.
    /// </summary>
    [JsonPropertyName("state")]
    public KnownStates State { get; set; }
        
    /// <summary>
    /// Gets or sets the context for this entity's state.
    /// </summary>
    [JsonPropertyName("context")]
    public StateContext? Context { get; set; }

    /// <summary>
    /// Gets or sets the UTC date and time that this state was last changed.
    /// </summary>
    [JsonPropertyName("last_changed")]
    public DateTimeOffset LastChanged { get; set; }

    /// <summary>
    /// Gets or sets the UTC date and time that this state was last updated.
    /// </summary>
    [JsonPropertyName("last_updated")]
    public DateTimeOffset LastUpdated { get; set; }
}