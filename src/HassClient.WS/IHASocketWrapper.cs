using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HassClient.WS;

[PublicAPI]
public interface IHASocketWrapper : IDisposable
{
    HassClientWebSocket Client { get; }
    bool IsConnected { get; }
    Task OpenAsync();
    Task CloseAsync();
}