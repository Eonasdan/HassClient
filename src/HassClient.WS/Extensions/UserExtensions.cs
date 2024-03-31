using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;
using HassClient.WS.Messages.Commands.RegistryEntryCollections;
using JetBrains.Annotations;

namespace HassClient.WS.Extensions;

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