using System.Threading;
using System.Threading.Tasks;
using HassClient.WS.Messages.Commands;
using JetBrains.Annotations;

namespace HassClient.WS.Extensions;

[PublicAPI]
public static class TemplateExtensions
{
    public static async Task<string?> RenderTemplateAsync(this HassClientWebSocket client, string template, CancellationToken cancellationToken = default)
    {
        var commandMessage = new RenderTemplateMessage { Template = template };
        if (!await client.SendCommandWithSuccessAsync(commandMessage, cancellationToken))
        {
            return default;
        }

        return await commandMessage.WaitResponseTask;
    }
}