using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models;
using HassClient.WS.Messages.Commands;
using JetBrains.Annotations;

namespace HassClient.WS.Extensions;

[PublicAPI]
public static class PanelExtensions
{
    /// <summary>
    /// Gets the <see cref="PanelInfo"/> of the panel located at the specified <paramref name="urlPath"/> in the Home Assistant instance.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="urlPath">The URL path of the panel.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is the <see cref="PanelInfo"/> object.
    /// </returns>
    public static async Task<PanelInfo?> GetPanelAsync(this HassClientWebSocket client, string urlPath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(urlPath))
        {
            throw new ArgumentException($"'{nameof(urlPath)}' cannot be null or empty", nameof(urlPath));
        }

        var commandMessage = new GetPanelsMessage();
        var dict =
            await client.SendCommandWithResultAsync<Dictionary<string, PanelInfo>>(commandMessage,
                cancellationToken);
        if (dict != null &&
            dict.TryGetValue(urlPath, out var result))
        {
            return result;
        }

        return default;
    }

    /// <summary>
    /// Gets a collection with the <see cref="PanelInfo"/> for every registered panel in the Home Assistant instance.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a collection of
    /// <see cref="PanelInfo"/> of every registered panel in the Home Assistant instance.
    /// </returns>
    public static async Task<IEnumerable<PanelInfo>?> GetPanelsAsync(this HassClientWebSocket client,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = new GetPanelsMessage();
        var dict =
            await client.SendCommandWithResultAsync<Dictionary<string, PanelInfo>>(commandMessage,
                cancellationToken);
        return dict?.Values;
    }
}