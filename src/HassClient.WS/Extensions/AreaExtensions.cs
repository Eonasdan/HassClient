using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.WS.Messages.Commands.RegistryEntryCollections;
using JetBrains.Annotations;

namespace HassClient.WS.Extensions;

[PublicAPI]
public static class AreaExtensions
{
    /// <summary>
    /// Gets a collection with every registered <see cref="Area"/> in the Home Assistant instance.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a collection with
    /// every registered <see cref="Area"/> in the Home Assistant instance.
    /// </returns>
    public static Task<IEnumerable<Area?>?> GetAreasAsync(this HassClientWebSocket client,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = AreaRegistryMessagesFactory.Instance.CreateListMessage();
        return client.SendCommandWithResultAsync<IEnumerable<Area?>>(commandMessage,
            cancellationToken);
    }

    /// <summary>
    /// Creates a new <see cref="Area"/>.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="area">The <see cref="Area"/> with the new values.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// create operation was successfully done.
    /// </returns>
    public static async Task<bool> CreateAreaAsync(this HassClientWebSocket client, Area? area,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = AreaRegistryMessagesFactory.Instance.CreateCreateMessage(area);
        var result = await client.SendCommandWithResultAsync(commandMessage, cancellationToken);
        if (result.Success)
        {
            result.PopulateResult(area);
        }

        return result.Success;
    }

    /// <summary>
    /// Updates an existing <see cref="Area"/>.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="area">The <see cref="Area"/> with the new values.</param>
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
    public static async Task<bool> UpdateAreaAsync(this HassClientWebSocket client, Area? area,
        bool forceUpdate = false,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = AreaRegistryMessagesFactory.Instance.CreateUpdateMessage(area, forceUpdate);

        var result = await client.SendCommandWithResultAsync(commandMessage, cancellationToken);
        if (result.Success)
        {
            result.PopulateResult(area);
        }

        return result.Success;
    }

    /// <summary>
    /// Deletes an existing <see cref="Area"/>.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="area">The <see cref="Area"/> to delete.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// delete operation was successfully done.
    /// </returns>
    public static async Task<bool> DeleteAreaAsync(this HassClientWebSocket client, Area area,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = AreaRegistryMessagesFactory.Instance.CreateDeleteMessage(area);
        var success = await client.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
        if (success)
        {
            area.Untrack();
        }

        return success;
    }
}