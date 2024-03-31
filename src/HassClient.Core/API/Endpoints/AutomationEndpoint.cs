using System.Threading.Tasks;
using HassClient.Core.API.Models;
using JetBrains.Annotations;

namespace HassClient.Core.API.Endpoints;

/// <summary>
/// Provides access to the automations API for working with automations.
/// </summary>
[PublicAPI]
public class AutomationEndpoint(JsonClient client)
{
    private const string BaseUrl = "config/automation/config";

    /// <summary>
    /// Create the <see cref="Automation"/>.
    /// </summary>
    /// <param name="automation">The <see cref="Automation"/>.</param>
    /// <returns>The <see cref="AutomationResultObject"/>.</returns>
    public Task<AutomationResultObject?> CreateAsync(Automation automation) =>
        client.PostAsync<AutomationResultObject>($"{BaseUrl}/{automation.Id}",
            automation.ToJsonStringContent());

    /// <summary>
    /// Read the <see cref="Automation"/>.
    /// </summary>
    /// <param name="id">The automation id.</param>
    /// <returns>The <see cref="Automation"/>.</returns>
    public Task<Automation?> Get(string id) =>
        client.GetAsync<Automation>($"{BaseUrl}/{id}");

    /// <summary>
    /// Update the <see cref="Automation"/>.
    /// </summary>
    /// <param name="automation">The <see cref="Automation"/>.</param>
    /// <returns>The <see cref="AutomationResultObject"/>.</returns>
    public Task<AutomationResultObject?> UpdateAsync(Automation automation) =>
        CreateAsync(automation);

    /// <summary>
    /// Delete the <see cref="Automation"/>.
    /// </summary>
    /// <param name="id">The automation id.</param>
    /// <returns>The <see cref="AutomationResultObject"/>.</returns>
    public Task<bool> DeleteAsync(string id) =>
        client.DeleteAsync($"{BaseUrl}/{id}");
}