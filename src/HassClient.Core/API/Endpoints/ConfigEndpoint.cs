using System.Net.Http;
using System.Threading.Tasks;
using HassClient.Core.API.Models;
using JetBrains.Annotations;

namespace HassClient.Core.API.Endpoints;

/// <summary>
/// Provides access to the configuration API for retrieving the current Home Assistant configuration.
/// </summary>
[PublicAPI]
public class ConfigEndpoint(JsonClient client)
{
    private const string BaseUrl = "config";

    /// <summary>
    /// Retrieves the current Home Assistant configuration object.
    /// </summary>
    /// <returns>A <see cref="ConfigurationObject" /> representing the current Home Assistant configuration.</returns>
    public Task<ConfigurationObject?> GetConfiguration() => client.GetAsync<ConfigurationObject>(BaseUrl);

    /// <summary>
    /// Performs a configuration check and returns the result.
    /// </summary>
    /// <returns>A <see cref="ConfigurationCheckResult" /> containing the results of the check, and any errors that occurred.</returns>
    public Task<ConfigurationCheckResult?> CheckConfiguration() =>  client.PostAsync<ConfigurationCheckResult>($"{BaseUrl}/core/check_config", new StringContent(""));
}