using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.WS.Messages.Commands.RegistryEntryCollections;
using JetBrains.Annotations;

namespace HassClient.WS.Extensions;

[PublicAPI]
public static class StorageEntityExtensions
{
    /// <summary>
    /// Gets a collection with every registered storage entity registry entry of the given type
    /// in the Home Assistant instance.
    /// </summary>
    /// <typeparam name="TStorageEntity">The storage entity registry entry type.</typeparam>
    /// <param name="client"></param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a collection with
    /// every registered <typeparamref name="TStorageEntity"/> entity in the Home Assistant instance.
    /// </returns>
    public static async Task<IEnumerable<TStorageEntity>?> GetStorageEntityRegistryEntriesAsync<TStorageEntity>(
        this HassClientWebSocket client,
        CancellationToken cancellationToken = default)
        where TStorageEntity : StorageEntityRegistryEntryBase
    {
        var commandMessage = StorageCollectionMessagesFactory<TStorageEntity>.Create().CreateListMessage();
        var result = await client.SendCommandWithResultAsync(commandMessage, cancellationToken);
        if (!result.Success) return null;

        if (typeof(TStorageEntity) != typeof(Person)) return result.DeserializeResult<IEnumerable<TStorageEntity>>();

        var response = result.DeserializeResult<PersonResponse>();
        return response?.Storage
            .Select(person =>
            {
                person.IsStorageEntry = true;
                return person;
            })
            .Concat(response.Config)
            .Cast<TStorageEntity>();
    }

    /// <summary>
    /// Creates a new storage entity registry entry of the given type.
    /// </summary>
    /// <typeparam name="TStorageEntity">The storage entity registry entry type.</typeparam>
    /// <param name="client"></param>
    /// <param name="storageEntity">The new storage entity registry entry.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// create operation was successfully done.
    /// </returns>
    public static async Task<bool> CreateStorageEntityRegistryEntryAsync<TStorageEntity>(
        this HassClientWebSocket client,
        TStorageEntity storageEntity,
        CancellationToken cancellationToken = default)
        where TStorageEntity : StorageEntityRegistryEntryBase
    {
        var commandMessage = StorageCollectionMessagesFactory<TStorageEntity>.Create()
            .CreateCreateMessage(storageEntity);
        var result = await client.SendCommandWithResultAsync(commandMessage, cancellationToken);
        if (result.Success)
        {
            result.PopulateResult(storageEntity);
        }

        return result.Success;
    }

    /// <summary>
    /// Updates an existing storage entity registry entry of the given type.
    /// </summary>
    /// <typeparam name="TStorageEntity">The storage entity registry entry type.</typeparam>
    /// <param name="client"></param>
    /// <param name="storageEntity">The storage entity registry entry with the updated values.</param>
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
    public static async Task<bool> UpdateStorageEntityRegistryEntryAsync<TStorageEntity>(
        this HassClientWebSocket client,
        TStorageEntity storageEntity,
        bool forceUpdate = false, CancellationToken cancellationToken = default)
        where TStorageEntity : StorageEntityRegistryEntryBase
    {
        var commandMessage = StorageCollectionMessagesFactory<TStorageEntity>.Create()
            .CreateUpdateMessage(storageEntity, forceUpdate);
        var result = await client.SendCommandWithResultAsync(commandMessage, cancellationToken);
        if (result.Success)
        {
            result.PopulateResult(storageEntity);
        }

        return result.Success;
    }

    /// <summary>
    /// Deletes an existing storage entity registry entry of the given type.
    /// </summary>
    /// <typeparam name="TStorageEntity">The storage entity registry entry type.</typeparam>
    /// <param name="client"></param>
    /// <param name="storageEntity">The storage entity registry entry to delete.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// delete operation was successfully done.
    /// </returns>
    public static async Task<bool> DeleteStorageEntityRegistryEntryAsync<TStorageEntity>(
        this HassClientWebSocket client,
        TStorageEntity storageEntity,
        CancellationToken cancellationToken = default)
        where TStorageEntity : StorageEntityRegistryEntryBase
    {
        var commandMessage = StorageCollectionMessagesFactory<TStorageEntity>.Create()
            .CreateDeleteMessage(storageEntity);
        var success = await client.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
        if (success)
        {
            storageEntity.Untrack();
        }

        return success;
    }
}