using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.API.Models;
using JetBrains.Annotations;

namespace HassClient.Core.API.Endpoints;

/// <summary>
/// Provides a wrapper around the States endpoint for retrieving entity info.
/// </summary>
[PublicAPI]
public class EntityEndpoint(JsonClient client)
{
    private const string BaseUrl = "states";

    private Task<IEnumerable<Entity>?> GetStates() => client.GetAsync<IEnumerable<Entity>>(BaseUrl);

    /// <summary>
    /// Retrieves a list of all current entity names (that have state) in the format "domain.name".
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}" /> of strings of all known entities (with state) at the time.</returns>
    public async Task<IEnumerable<string?>?> GetEntities() => 
        (await GetStates())?.Select(s => s.EntityId);

    /// <summary>
    /// Retrieves a list of entity names for a particular domain (that have state) in the format "domain.name".
    /// </summary>
    /// <param name="domainFilter">A domain name to filter the entity list to (e.g. "light").</param>
    /// <returns>An <see cref="IEnumerable{T}" /> of strings of all known entities (with state) at the time.</returns>
    public async Task<IEnumerable<string?>?> GetEntities(string domainFilter) => (await GetStates())?.Where(s => s.EntityId != null && s.EntityId.StartsWith(
        $"{domainFilter}."))
        .Select(s => s.EntityId);
}