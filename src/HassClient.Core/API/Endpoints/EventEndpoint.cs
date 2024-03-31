using System.Collections.Generic;
using System.Threading.Tasks;
using HassClient.Core.API.Models;
using JetBrains.Annotations;

namespace HassClient.Core.API.Endpoints;

/// <summary>
/// Provides access to the event API for retrieving information about events and firing events.
/// </summary>
[PublicAPI]
public class EventEndpoint(JsonClient client)
{
    private const string BaseUrl = "events";

    /// <summary>
    /// Retrieves a list of event types from the current Home Assistant instance.
    /// </summary>
    /// <returns>A list of <see cref="EventObject" /> representing the available event types.</returns>
    public Task<List<EventObject>?> GetEvents() => client.GetAsync<List<EventObject>>(BaseUrl);

    /// <summary>
    /// Fires an event of type <paramref name="eventType" /> on the event bus.
    /// </summary>
    /// <returns>An <see cref="EventFireResult" /> with a message on if the event fired successfully or not.</returns>
    public Task<EventFireResult?> FireEvent(string eventType, object payload) => client.PostAsync<EventFireResult>(
        $"{BaseUrl}/{eventType}", client.ToStringContent(payload));
}