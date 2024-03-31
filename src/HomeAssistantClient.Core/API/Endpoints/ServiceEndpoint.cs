using HomeAssistantClient.Core.API.Models;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Endpoints;

/// <summary>
/// Provides access to the service API for retrieving information about services and calling services.
/// </summary>
[PublicAPI]
public class ServiceEndpoint(JsonClient client)
{
    private const string BaseUrl = "services";

    /// <summary>
        /// Retrieves a list of current services, separated into service domains.
        /// </summary>
        /// <returns>A <see cref="List{ServiceDomainObject}" /> representing available services grouped by domain.</returns>
        public Task<List<ServiceDomainObject>?> GetServicesAsync() => client.GetAsync<List<ServiceDomainObject>>(BaseUrl);

        /// <summary>
        /// Calls a service using the given <paramref name="domain" />, <paramref name="service" />, and optionally, <paramref name="fields" />.
        /// </summary>
        /// <param name="domain">The domain of the service (e.g. "light").</param>
        /// <param name="service">The name of the service (e.g. "turn_on").</param>
        /// <param name="fields">Optional. An object representing the fields/parameters to pass to the service. Can be an anonymous type, or a <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, object&gt;</see>.</param>
        /// <returns></returns>
        public Task<List<Entity>?> CallServiceAsync(string domain, string service, object? fields) => client.PostAsync<List<Entity>>($"{BaseUrl}/{domain}/{service}", client.ToStringContent(fields));

        /// <summary>
        /// Calls a service using the given fully-qualified <paramref name="serviceName" />, and optionally, <paramref name="fields" />.
        /// </summary>
        /// <param name="serviceName">The fully-qualified service name (e.g. "light.turn_on").</param>
        /// <param name="fields">Optional. An object representing the fields/parameters to pass to the service. Can be an anonymous type, or a <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, object&gt;</see>.</param>
        /// <returns></returns>
        public Task<List<Entity>?> CallServiceAsync(string serviceName, object? fields = null) => client.PostAsync<List<Entity>>($"{BaseUrl}/{serviceName.Split('.')[0]}/{serviceName.Split('.')[1]}", client.ToStringContent(fields));

        /// <summary>
        /// Calls a service using the given <paramref name="domain" />, <paramref name="service" />, and optionally, <paramref name="fields" />.
        /// </summary>
        /// <param name="domain">The domain of the service (e.g. "light").</param>
        /// <param name="service">The name of the service (e.g. "turn_on").</param>
        /// <param name="fields">Optional. A JSON string representing the fields/parameters to pass to the service. Ensure the JSON is a well-formatted object.</param>
        /// <returns></returns>
        public Task<List<Entity>?> CallServiceAsync(string domain, string service, string? fields) => client.PostAsync<List<Entity>>($"{BaseUrl}/{domain}/{service}", client.ToStringContent(fields));

        /// <summary>
        /// Calls a service using the given fully-qualified <paramref name="serviceName" />, and optionally, <paramref name="fields" />.
        /// </summary>
        /// <param name="serviceName">The fully-qualified service name (e.g. "light.turn_on").</param>
        /// <param name="fields">Optional. A JSON string representing the fields/parameters to pass to the service. Ensure the JSON is a well-formatted object.</param>
        /// <returns></returns>
        public Task<List<Entity>?> CallServiceAsync(string serviceName, string? fields = null) => client.PostAsync<List<Entity>>($"{BaseUrl}/{serviceName.Split('.')[0]}/{serviceName.Split('.')[1]}", client.ToStringContent(fields));

        /// <summary>
        /// Calls a service using the given fully-qualified <paramref name="serviceName" /> and one or more <paramref name="entityIds" />.
        /// </summary>
        /// <param name="serviceName">The fully-qualified service name (e.g. "light.turn_on").</param>
        /// <param name="entityIds">The entity IDs to pass to the service (using the <c>entity_ids</c> parameter).</param>
        /// <returns></returns>
        public Task<List<Entity>?> CallServiceForEntitiesAsync(string serviceName, params string[] entityIds) => client.PostAsync<List<Entity>>($"{BaseUrl}/{serviceName.Split('.')[0]}/{serviceName.Split('.')[1]}", client.ToStringContent(new { entity_id = entityIds }));
}