using System.Collections.Generic;
using System.Threading.Tasks;
using HassClient.Core.API.Models;
using JetBrains.Annotations;

namespace HassClient.Core.API.Endpoints;

/// <summary>
/// Provides access to the states API for retrieving information about the current state of entities.
/// </summary>
[PublicAPI]
public class StateEndpoint(JsonClient client)
{
    private const string BaseUrl = "states";

    /// <summary>
    /// Retrieves a list of current entities and their states.
    /// </summary>
    /// <returns>A <see cref="List{T}" /> representing the current state.</returns>
    public Task<List<Entity>?> GetStatesAsync() => client.GetAsync<List<Entity>>(BaseUrl);

    /// <summary>
    /// Retrieves the state of an entity by its ID.
    /// </summary>
    /// <returns>A <see cref="Entity" /> representing the current state of the requested <paramref name="entityId" />.</returns>
    public Task<Entity?> GetStateAsync(string entityId) => GetStateAsync<Entity>(entityId);
    
    /// <summary>
    /// Retrieves the state of an entity by its ID.
    /// </summary>
    /// <returns>A <see cref="Entity" /> representing the current state of the requested <paramref name="entityId" />.</returns>
    public Task<T?> GetStateAsync<T>(string entityId) where T : class => client.GetAsync<T>($"/api/states/{entityId}");

    /// <summary>
    /// Sets the state of an entity. If the entity does not exist, it will be created.
    /// </summary>
    /// <param name="entityId">The entity ID of the state to change.</param>
    /// <param name="newState">The new state value.</param>
    /// <param name="setAttributes">Optional. The attributes to set.</param>
    /// <returns>A <see cref="Entity" /> representing the updated state of the updated <paramref name="entityId" />.</returns>
    public Task<Entity?>
        SetStateAsync(string entityId, string newState, Dictionary<string, object>? setAttributes = null) =>
        client.PostAsync<Entity>($"{BaseUrl}/{entityId}",
            client.ToStringContent(new { state = newState, attributes = setAttributes }));
}