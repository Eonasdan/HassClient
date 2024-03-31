using System.Threading.Tasks;
using HassClient.Core.API.Models;
using JetBrains.Annotations;

namespace HassClient.Core.API.Endpoints;

/// <summary>
/// Provides access to the info API for retrieving statistics about Supervisor and Core.
/// </summary>
[PublicAPI]
public class StatsEndpoint(JsonClient client)
{
    private const string BaseUrl = "hassio";

    /// <summary>
    /// Retrieves Supervisor information.
    /// </summary>
    /// <returns>A <see cref="StatsObject"/> representing Supervisor stats.</returns>
    public Task<ResponseObject<StatsObject>?> GetSupervisorStatsAsync() => client.GetAsync<ResponseObject<StatsObject>>($"{BaseUrl}/supervisor/stats");

    /// <summary>
    /// Retrieves Core stats.
    /// </summary>
    /// <returns>A <see cref="StatsObject"/> representing Core stats.</returns>
    public Task<ResponseObject<StatsObject>?> GetCoreStatsAsync() => client.GetAsync<ResponseObject<StatsObject>>($"{BaseUrl}/core/stats");
}