using HassClient.Core.Helpers;

namespace HassClient.Core.Models;

/// <summary>
/// Represents a state changed event.
/// </summary>
public class StateChangedEvent
{
    /// <summary>
    /// Gets the entity id of the entity.
    /// </summary>
    [JsonPropertyName("EntityId")]
    public string? EntityId { get; init; }

    /// <summary>
    /// Gets or sets the entity domain of the entity.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public string Domain => EntityId.GetDomain();

    /// <summary>
    /// Gets the old state.
    /// </summary>
    [JsonPropertyName("OldState")]
    public StateModel OldState { get; init; }

    /// <summary>
    /// Gets the new state.
    /// </summary>
    [JsonPropertyName("NewState")]
    public StateModel NewState { get; init; }
}