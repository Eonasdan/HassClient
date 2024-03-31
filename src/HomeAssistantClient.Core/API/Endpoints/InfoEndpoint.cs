using System.Net;
using HomeAssistantClient.Core.API.Models;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Endpoints;

/// <summary>
/// Provides access to the info API for retrieving information about Supervisor, Core and Host.
/// </summary>
[PublicAPI]
public class InfoEndpoint(JsonClient client)
{
    private const string BaseUrl = "hassio";

    /// <summary>
        /// Retrieves Supervisor information.
        /// </summary>
        /// <returns>A <see cref="SupervisorInfoObject"/> representing Supervisor information.</returns>
        public async Task<ResponseObject<SupervisorInfoObject>?> GetSupervisorInfo()
        {
            try
            {
                return await client.GetAsync<ResponseObject<SupervisorInfoObject>>($"{BaseUrl}/supervisor/info");
            }
            catch (SimpleHttpResponseException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                throw new SupervisorNotFoundException("This does not appear to be a Home Assistant Supervisor instance. See inner exception for more details.", e);
            }
        }

        /// <summary>
        /// Retrieves Host information.
        /// </summary>
        /// <returns>A <see cref="HostInfo"/> representing Host information.</returns>
        public async Task<ResponseObject<HostInfo>?> GetHostInfo()
        {
            try
            {
                return await client.GetAsync<ResponseObject<HostInfo>>($"{BaseUrl}/host/info");
            }
            catch (SimpleHttpResponseException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                throw new SupervisorNotFoundException("This does not appear to be a Home Assistant Supervisor instance. See inner exception for more details.", e);
            }
        }

        /// <summary>
        /// Retrieves Core information.
        /// </summary>
        /// <returns>A <see cref="CoreInfo"/> representing Host information.</returns>
        public async Task<ResponseObject<CoreInfo>?> GetCoreInfo()
        {
            try
            {
                return await client.GetAsync<ResponseObject<CoreInfo>>($"{BaseUrl}/core/info");
            }
            catch (SimpleHttpResponseException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                throw new SupervisorNotFoundException("This does not appear to be a Home Assistant Supervisor instance. See inner exception for more details.", e);
            }
        }
        
        /// <summary>
        /// The exception that occurs when a Supervisor-only API call is made to a non-Supervisor environment.
        /// </summary>
        public class SupervisorNotFoundException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the SupervisorNotFoundException.
            /// </summary>
            public SupervisorNotFoundException()
            {
            }

            /// <summary>
            /// Initializes a new instance of the SupervisorNotFoundException.
            /// </summary>
            public SupervisorNotFoundException(string message) : base(message)
            {
            }

            /// <summary>
            /// Initializes a new instance of the SupervisorNotFoundException.
            /// </summary>
            public SupervisorNotFoundException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
}