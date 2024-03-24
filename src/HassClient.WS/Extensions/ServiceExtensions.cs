using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Helpers;
using HassClient.Core.Models;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Serialization;
using HassClient.WS.Messages.Commands;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Extensions;

[PublicAPI]
public static class ServiceExtensions
{

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
    
    /// <summary>
    /// Calls a service within a specific domain.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="domain">The service domain.</param>
    /// <param name="service">The service to call.</param>
    /// <param name="data">The optional data to use in the service invocation.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a <see cref="Context"/>
    /// associated with the result of the service invocation.
    /// </returns>
    public static async Task<T?> CallServiceAsync<T>(this HassClientWebSocket client,
        string domain, string service,
        object? data = null,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = new CallServiceMessageWithResponse(domain, service, data);
        var result = await client.SendCommandWithResultAsync<BaseResultResponse<T>>(commandMessage,
            cancellationToken);
        return result != null ? result.Response : default;
    }
    
    
    /// <summary>
    /// Calls a service within a specific domain.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="domain">The service domain.</param>
    /// <param name="service">The service to call.</param>
    /// <param name="data">The optional data to use in the service invocation.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a <see cref="Context"/>
    /// associated with the result of the service invocation.
    /// </returns>
    public static async Task<Context?> CallServiceAsync(this HassClientWebSocket client,
        string domain, string service,
        object? data = null,
        CancellationToken cancellationToken = default)
    {
        var commandMessage = new CallServiceMessage(domain, service, data);
        var state = await client.SendCommandWithResultAsync<StateModel>(commandMessage,
            cancellationToken);
        return state?.Context;
    }

    /// <summary>
    /// Calls a service within a specific domain.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="domain">The service domain.</param>
    /// <param name="service">The service to call.</param>
    /// <param name="data">The optional data to use in the service invocation.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// service invocation was successfully done.
    /// </returns>
    public static async Task<bool> CallServiceAsync(
        this HassClientWebSocket client,
        KnownDomains domain, KnownServices service, object? data = null,
        CancellationToken cancellationToken = default)
    {
        var context = await client.CallServiceAsync(domain.ToDomainString(), service.ToServiceString(), data,
            cancellationToken);
        return context != null;
    }

    /// <summary>
    /// Calls a service within a specific domain and entities.
    /// <para>
    /// This overload is useful when only entity_id is needed in service invocation.
    /// </para>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="domain">The service domain.</param>
    /// <param name="service">The service to call.</param>
    /// <param name="entityIds">The ids of the target entities affected by the service call.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// service invocation was successfully done.
    /// </returns>
    public static Task<bool> CallServiceForEntitiesAsync(
        this HassClientWebSocket client,
        string domain,
        string service,
        params string[] entityIds)
    {
        return client.CallServiceForEntitiesAsync(domain, service, CancellationToken.None, entityIds);
    }

    /// <summary>
    /// Calls a service within a specific domain and entities.
    /// <para>
    /// This overload is useful when only entity_id is needed in service invocation.
    /// </para>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="domain">The service domain.</param>
    /// <param name="service">The service to call.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <param name="entityIds">The ids of the target entities affected by the service call.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// service invocation was successfully done.
    /// </returns>
    public static async Task<bool> CallServiceForEntitiesAsync(
        this HassClientWebSocket client,
        string domain,
        string service,
        CancellationToken cancellationToken = default,
        params string[] entityIds)
    {
        var context = await client.CallServiceAsync(domain, service, new { entity_id = entityIds }, cancellationToken);
        return context != null;
    }

    /// <summary>
    /// Calls a service within a specific domain and entities.
    /// <para>
    /// This overload is useful when only entity_id is needed in service invocation.
    /// </para>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="domain">The service domain.</param>
    /// <param name="service">The service to call.</param>
    /// <param name="entityIds">The ids of the target entities affected by the service call.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// service invocation was successfully done.
    /// </returns>
    public static Task<bool> CallServiceForEntitiesAsync(
        this HassClientWebSocket client,
        KnownDomains domain, KnownServices service,
        params string[] entityIds)
    {
        return client.CallServiceForEntitiesAsync(domain, service, CancellationToken.None, entityIds);
    }

    /// <summary>
    /// Calls a service within a specific domain and entities.
    /// <para>
    /// This overload is useful when only entity_id is needed in service invocation.
    /// </para>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="domain">The service domain.</param>
    /// <param name="service">The service to call.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <param name="entityIds">The ids of the target entities affected by the service call.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// service invocation was successfully done.
    /// </returns>
    public static Task<bool> CallServiceForEntitiesAsync(
        this HassClientWebSocket client,
        KnownDomains domain, KnownServices service,
        CancellationToken cancellationToken = default,
        params string[] entityIds)
    {
        return client.CallServiceAsync(domain, service, new { entity_id = entityIds }, cancellationToken);
    }
}