using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HassClient.Core.API.Models;

/// <summary>
/// Represents an event definition in Home Assistant.
/// </summary>
[PublicAPI]
public class EventObject
{
    /// <summary>
    /// Gets the global event string (*).
    /// </summary>
    public static readonly string GlobalEvent = "*";

    /// <summary>
    /// Gets the event's name.
    /// </summary>
    public string? Event { get; set; }

    /// <summary>
    /// Gets the listener count for this event.
    /// </summary>
    [JsonPropertyName("listener_count")]
    public int ListenerCount { get; set; }

    /// <summary>
    /// Gets a string representation of this object.
    /// </summary>
    public override string ToString() => $"Event: {Event} ({ListenerCount} listener(s))";
}