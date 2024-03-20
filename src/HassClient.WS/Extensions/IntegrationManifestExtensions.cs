using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models;
using HassClient.WS.Messages.Commands;
using JetBrains.Annotations;

namespace HassClient.WS.Extensions;

[PublicAPI]
public static class IntegrationManifestExtensions
{
    /// <summary>
    /// Gets the <see cref="IntegrationManifest"/> that contains basic information about the specified integration.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="integrationName">The integration name.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a <see cref="IntegrationManifest"/>
    /// containing basic information about the specified integration.
    /// </returns>
    public static Task<IntegrationManifest?> GetIntegrationManifestAsync(this HassClientWebSocket client,
        string integrationName,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = new GetManifestMessage { Integration = integrationName };
        return client.SendCommandWithResultAsync<IntegrationManifest>(commandMessage,
            cancellationToken);
    }

    /// <summary>
    /// Gets a collection with the <see cref="IntegrationManifest"/> that contains basic information of every
    /// registered integration in the Home Assistant instance.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a collection of
    /// <see cref="IntegrationManifest"/> of every registered integration in the Home Assistant instance.
    /// </returns>
    public static Task<IEnumerable<IntegrationManifest>?> GetIntegrationManifestsAsync(this HassClientWebSocket client,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = new ListManifestsMessage();
        return client.SendCommandWithResultAsync<IEnumerable<IntegrationManifest>>(commandMessage,
            cancellationToken);
    }
}