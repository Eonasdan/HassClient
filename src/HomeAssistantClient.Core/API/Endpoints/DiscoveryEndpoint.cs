using HomeAssistantClient.Core.API.Models;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Endpoints;

/// <summary>
/// Provides access to the discovery API for retrieving the current Home Assistant instance information.
/// </summary>
[PublicAPI]
public class DiscoveryEndpoint(JsonClient client)
{
    /// <summary>
    /// Retrieves the current Home Assistant discovery object.
    /// </summary>
    /// <returns>A <see cref="Discovery" /> representing the current Home Assistant instance information.</returns>
    public Task<Discovery?> GetDiscoveryInfo() => client.GetAsync<Discovery>("/api/discovery_info");
}