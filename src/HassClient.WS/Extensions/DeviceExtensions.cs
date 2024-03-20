using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.Core.Serialization;
using HassClient.WS.Messages.Commands.RegistryEntryCollections;
using JetBrains.Annotations;

namespace HassClient.WS.Extensions;

[PublicAPI]
public static class DeviceExtensions
{
    /// <summary>
    /// Gets a collection with every registered <see cref="Device"/> in the Home Assistant instance.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a collection with
    /// every registered <see cref="Device"/> in the Home Assistant instance.
    /// </returns>
    public static Task<IEnumerable<Device>?> GetDevicesAsync(this HassClientWebSocket client,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = DeviceRegistryMessagesFactory.Instance.CreateListMessage();
        return client.SendCommandWithResultAsync<IEnumerable<Device>>(commandMessage,
            cancellationToken);
    }

    /// <summary>
    /// Updates an existing <see cref="Device"/>.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="device">The <see cref="Device"/> with the new values.</param>
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
    public static async Task<bool> UpdateDeviceAsync(this HassClientWebSocket client, Device? device,
        bool? disable = null, bool forceUpdate = false,
        CancellationToken cancellationToken = default)
    {
        var commandMessage =
            DeviceRegistryMessagesFactory.Instance.CreateUpdateMessage(device, disable, forceUpdate);
        var result = await client.SendCommandWithResultAsync(commandMessage, cancellationToken);
        if (result.Success)
        {
            result.PopulateResult(device);
        }

        return result.Success;
    }
}

[PublicAPI]
public static class UserExtensions
{
    /// <summary>
    /// Gets a collection with every registered <see cref="User"/> in the Home Assistant instance.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a collection with
    /// every registered <see cref="User"/> in the Home Assistant instance.
    /// </returns>
    public static Task<IEnumerable<User>?> GetUsersAsync(this HassClientWebSocket client,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = UserMessagesFactory.Instance.CreateListMessage();
        return client.SendCommandWithResultAsync<IEnumerable<User>>(commandMessage,
            cancellationToken);
    }

    /// <summary>
    /// Creates a new <see cref="User"/>.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="user">The new <see cref="User"/>.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// create operation was successfully done.
    /// </returns>
    public static async Task<bool> CreateUserAsync(this HassClientWebSocket client, User user,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = UserMessagesFactory.Instance.CreateCreateMessage(user);
        var result =
            await client.SendCommandWithResultAsync<UserResponse>(commandMessage, cancellationToken);

        if (result == null) return false;
        HassSerializer.PopulateObject(result.UserRaw, user);
        return true;
    }

    /// <summary>
    /// Updates an existing <see cref="User"/>.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="user">The <see cref="User"/> with the new values.</param>
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
    public static async Task<bool> UpdateUserAsync(this HassClientWebSocket client, User user, bool forceUpdate = false,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = UserMessagesFactory.Instance.CreateUpdateMessage(user, forceUpdate);
        var result =
            await client.SendCommandWithResultAsync<UserResponse>(commandMessage, cancellationToken);
        if (result == null)
        {
            return false;
        }

        HassSerializer.PopulateObject(result.UserRaw, user);
        return true;
    }

    /// <summary>
    /// Deletes an existing <see cref="User"/>.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="user">The <see cref="User"/> to delete.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// delete operation was successfully done.
    /// </returns>
    public static async Task<bool> DeleteUserAsync(this HassClientWebSocket client, User user,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = UserMessagesFactory.Instance.CreateDeleteMessage(user);
        var success = await client.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
        if (success)
        {
            user.Untrack();
        }

        return success;
    }
}

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