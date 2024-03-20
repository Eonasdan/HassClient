using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;
using HassClient.WS.Messages.Commands;
using HassClient.WS.Messages.Commands.RegistryEntryCollections;
using JetBrains.Annotations;

namespace HassClient.WS.Extensions;

[PublicAPI]
public static class EntityExtensions
{
    /// <summary>
    /// Gets the <see cref="EntitySource"/> of a specified entity.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="entityId">The entity id.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is the <see cref="EntitySource"/>.
    /// </returns>
    public static async Task<EntitySource?> GetEntitySourceAsync(this HassClientWebSocket client, string entityId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(entityId))
        {
            throw new ArgumentNullException(nameof(entityId));
        }

        var result = await client.GetEntitySourcesAsync(cancellationToken, entityId);
        return result.FirstOrDefault();
    }

    /// <summary>
    /// Gets a collection with the <see cref="EntitySource"/> of the specified entities.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="entityIds">The entities ids.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a collection of
    /// <see cref="EntitySource"/> of the specified entities.
    /// </returns>
    public static Task<IEnumerable<EntitySource?>> GetEntitySourcesAsync(this HassClientWebSocket client,
        params string[]? entityIds)
    {
        return client.GetEntitySourcesAsync(CancellationToken.None, entityIds);
    }

    /// <summary>
    /// Gets a collection with the <see cref="EntitySource"/> of the specified entities.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <param name="entityIds">The entities ids.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a collection of
    /// <see cref="EntitySource"/> of the specified entities.
    /// </returns>
    public static async Task<IEnumerable<EntitySource?>> GetEntitySourcesAsync(this HassClientWebSocket client,
        CancellationToken cancellationToken,
        params string[]? entityIds)
    {
        var commandMessage = new EntitySourceMessage
            { EntityIds = entityIds is { Length: > 0 } ? entityIds : null };
        var dict =
            await client.SendCommandWithResultAsync<Dictionary<string, EntitySource>>(commandMessage,
                cancellationToken);
        return dict?.Select(x =>
        {
            var entitySource = x.Value;
            entitySource.EntityId = x.Key;
            return entitySource;
        }) ?? Array.Empty<EntitySource>();
    }

    /// <summary>
    /// Gets a collection with the <see cref="EntityRegistryEntry"/> of every registered entity in the Home Assistant instance.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a collection of
    /// <see cref="EntityRegistryEntry"/> of every registered entity in the Home Assistant instance.
    /// </returns>
    public static Task<IEnumerable<EntityRegistryEntry>?> GetEntitiesAsync(this HassClientWebSocket client,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = EntityRegistryMessagesFactory.Instance.CreateListMessage();
        return client.SendCommandWithResultAsync<IEnumerable<EntityRegistryEntry>>(commandMessage,
            cancellationToken);
    }

    /// <summary>
    /// Gets the <see cref="EntityRegistryEntry"/> of a specified entity.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="entityId">The entity id.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is the <see cref="EntityRegistryEntry"/>.
    /// </returns>
    public static Task<EntityRegistryEntry?> GetEntityAsync(this HassClientWebSocket client, string entityId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(entityId))
        {
            throw new ArgumentException($"'{nameof(entityId)}' cannot be null or empty", nameof(entityId));
        }

        var commandMessage = EntityRegistryMessagesFactory.Instance.CreateGetMessage(entityId);
        return client.SendCommandWithResultAsync<EntityRegistryEntry>(commandMessage,
            cancellationToken);
    }

    /// <summary>
    /// Refresh a given <see cref="EntityRegistryEntry"/> with the values from the server.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="entityRegistryEntry">The entity registry entry to refresh.</param>
    /// <param name="newEntityId">If not <see langword="null"/>, it will be used as entity id.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// refresh operation was successfully done.
    /// </returns>
    public static async Task<bool> RefreshEntityAsync(this HassClientWebSocket client,
        EntityRegistryEntry? entityRegistryEntry, string? newEntityId = null,
        CancellationToken cancellationToken = default)
    {
        var entityId = newEntityId ?? entityRegistryEntry?.EntityId;

        if (entityId == null) return true;

        var commandMessage = EntityRegistryMessagesFactory.Instance.CreateGetMessage(entityId);
        var result = await client.SendCommandWithResultAsync(commandMessage, cancellationToken);
        if (!result.Success)
        {
            return false;
        }

        result.PopulateResult(entityRegistryEntry);

        return true;
    }

    /// <summary>
    /// Updates an existing <see cref="EntityRegistryEntry"/> with the specified data.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="entity">The <see cref="EntityRegistryEntry"/> with the new values.</param>
    /// <param name="newEntityId">If not <see langword="null"/>, it will update the current entity id.</param>
    /// <param name="disable">If not <see langword="null"/>, it will enable or disable the entity.</param>
    /// <param name="forceUpdate">
    /// Indicates if the update operation should force the update of every modifiable property.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// update operation was successfully done.
    /// </returns>
    public static async Task<bool> UpdateEntityAsync(this HassClientWebSocket client, EntityRegistryEntry? entity,
        string? newEntityId = null,
        bool? disable = null, bool forceUpdate = false, CancellationToken cancellationToken = default)
    {
        if (newEntityId == entity?.EntityId)
        {
            throw new ArgumentException($"{nameof(newEntityId)} cannot be the same as {nameof(entity.EntityId)}");
        }

        var commandMessage =
            EntityRegistryMessagesFactory.Instance.CreateUpdateMessage(entity, newEntityId, disable, forceUpdate);
        var result =
            await client.SendCommandWithResultAsync<EntityEntryResponse>(commandMessage,
                cancellationToken);
        if (result == null)
        {
            return false;
        }

        HassSerializer.PopulateObject(result.EntityEntryRaw, entity);
        return true;
    }

    /// <summary>
    /// Deletes an existing <see cref="EntityRegistryEntry"/>.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="entity">The <see cref="EntityRegistryEntry"/> to delete.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// delete operation was successfully done.
    /// </returns>
    public static async Task<bool> DeleteEntityAsync(this HassClientWebSocket client, EntityRegistryEntry? entity,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = EntityRegistryMessagesFactory.Instance.CreateDeleteMessage(entity);
        var success = await client.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
        if (success)
        {
            entity?.Untrack();
        }

        return success;
    }
}