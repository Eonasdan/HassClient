using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HassClient.WS;

[PublicAPI]
public interface IHomeAssistantClientWrapper : IDisposable
{
    HassClientWebSocket WebSocketClient { get; }
    ApiClient ApiClient { get; }
    bool IsConnected { get; }
    Task OpenAsync();
    Task CloseAsync();
}