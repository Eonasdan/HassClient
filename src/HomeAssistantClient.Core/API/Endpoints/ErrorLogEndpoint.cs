using HomeAssistantClient.Core.API.Models;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Endpoints;

/// <summary>
/// Provides access to the error log API for retrieving the current error log messages.
/// </summary>
[PublicAPI]
public class ErrorLogEndpoint(JsonClient client)
{
    /// <summary>
    /// Retrieves a list of error log entries.
    /// </summary>
    /// <returns>An <see cref="ErrorLog" /> containing error log entries.</returns>
    public async Task<ErrorLog> GetErrorLogEntries() => new(
        await client.GetAsync("error_log"));
}