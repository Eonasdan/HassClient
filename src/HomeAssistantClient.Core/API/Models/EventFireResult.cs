using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Models;

/// <summary>
/// Represents the result of an event firing.
/// </summary>
[PublicAPI]
public class EventFireResult
{
    /// <summary>
    /// Gets the resulting message from the event fire command.
    /// </summary>
    public string? Message { get; set; }
}