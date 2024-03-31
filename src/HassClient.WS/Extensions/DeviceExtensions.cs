using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
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