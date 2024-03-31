using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HassClient.Core.API.Endpoints;

/// <summary>
/// Provides access to the template API for rendering Home Assistant templates.
/// </summary>
[PublicAPI]
public class TemplateEndpoint(JsonClient client)
{
    /// <summary>
    /// Renders a template and returns the resulting output as a string.
    /// </summary>
    /// <returns>A string of the rendered template output.</returns>
    public Task<string?> RenderTemplateAsync(string template) =>
        client.PostAsync<string>("/api/template", client.ToStringContent(new { template }));
}