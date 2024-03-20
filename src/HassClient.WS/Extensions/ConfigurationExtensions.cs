using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models;
using HassClient.WS.Messages.Commands;
using JetBrains.Annotations;

namespace HassClient.WS.Extensions;

[PublicAPI]
public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets a dump of the configuration in use by the Home Assistant instance.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is the <see cref="ConfigurationModel"/> object.
    /// </returns>
    public static Task<ConfigurationModel?> GetConfigurationAsync(this HassClientWebSocket client, CancellationToken cancellationToken = default)
    {
        var commandMessage = new GetConfigMessage();
        return client.SendCommandWithResultAsync<ConfigurationModel>(commandMessage,
            cancellationToken);
    }

    /// <summary>
    /// Refresh the configuration in use by the Home Assistant instance.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="configuration">The configuration model to be refreshed.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// refresh operation was successfully done.
    /// </returns>
    public static async Task<bool> RefreshConfigurationAsync(this HassClientWebSocket client, ConfigurationModel? configuration,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = new GetConfigMessage();
        var result = await client.SendCommandWithResultAsync(commandMessage, cancellationToken);
        if (!result.Success)
        {
            return false;
        }

        result.PopulateResult(configuration);
        return true;
    }
}