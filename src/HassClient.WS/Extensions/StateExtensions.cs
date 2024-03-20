using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models;
using HassClient.Core.Serialization;
using HassClient.WS.Messages.Commands;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Extensions;

[PublicAPI]
public static class StateExtensions
{
    /// <summary>
    /// Gets a collection with the state of every registered entity in the Home Assistant instance.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a collection with
    /// the state of every registered entity in the Home Assistant instance.
    /// </returns>
    public static Task<IEnumerable<StateModel>?> GetStatesAsync(this HassClientWebSocket client, CancellationToken cancellationToken = default)
    {
        return client.SendCommandWithResultAsync<IEnumerable<StateModel>>(new GetStatesMessage(),
            cancellationToken);
    }

    /// <summary>
    /// Gets a collection of <see cref="ServiceDomain"/> of every registered service in the Home Assistant instance.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a collection of
    /// <see cref="ServiceDomain"/> of every registered service in the Home Assistant instance.
    /// </returns>
    public static async Task<IEnumerable<ServiceDomain>?> GetServicesAsync(this HassClientWebSocket client, CancellationToken cancellationToken = default)
    {
        var commandMessage = new GetServicesMessage();
        var dict = await client.SendCommandWithResultAsync<Dictionary<string, JRaw>>(commandMessage,
            cancellationToken);
        return dict?.Select(x =>
            new ServiceDomain
            {
                Domain = x.Key,
                Services = HassSerializer.DeserializeObject<HassDomainServices>(x.Value),
            });
    }
}