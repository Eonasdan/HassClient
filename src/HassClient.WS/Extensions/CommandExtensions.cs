using System.Threading;
using System.Threading.Tasks;
using HassClient.WS.Messages.Commands;
using HassClient.WS.Messages.Commands.Search;
using HassClient.WS.Messages.Response;
using JetBrains.Annotations;

namespace HassClient.WS.Extensions;

[PublicAPI]
public static class CommandExtensions
{
    /// <summary>
    /// Performs a search related operation for the specified item id.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="itemType">The item type.</param>
    /// <param name="itemId">The item unique id.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a <see cref="SearchRelatedResponse"/>
    /// with all found relations.
    /// </returns>
    public static Task<SearchRelatedResponse?> SearchRelatedAsync(this HassClientWebSocket client, ItemTypes itemType,
        string itemId,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = new SearchRelatedMessage(itemType, itemId);
        return client.SendCommandWithResultAsync<SearchRelatedResponse>(commandMessage,
            cancellationToken);
    }

    /// <summary>
    /// Sends a customized command to the Home Assistant instance. This is useful when a command is not defined by the command extensions.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="rawCommandMessage">The raw command message to send.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a <see cref="RawCommandResult"/>
    /// with the response from the server.
    /// </returns>
    public static async Task<RawCommandResult> SendRawCommandWithResultAsync(this HassClientWebSocket client,
        BaseOutgoingMessage rawCommandMessage,
        CancellationToken cancellationToken = default)
    {
        var resultMessage =
            await client.SendCommandWithResultAsync(rawCommandMessage, cancellationToken);
        return RawCommandResult.FromResultMessage(resultMessage);
    }

    /// <summary>
    /// Sends a customized command to the Home Assistant instance. This is useful when a command is not defined by the command extensions.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="rawCommandMessage">The raw command message to send.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a <see cref="bool"/> indicating if
    /// the operation was successfully done.
    /// </returns>
    public static Task<bool> SendRawCommandWithSuccessAsync(this HassClientWebSocket client,
        BaseOutgoingMessage rawCommandMessage,
        CancellationToken cancellationToken = default)
    {
        return client.SendCommandWithSuccessAsync(rawCommandMessage, cancellationToken);
    }
}