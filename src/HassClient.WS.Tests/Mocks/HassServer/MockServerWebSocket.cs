using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Ninja.WebSockets;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public abstract class MockServerWebSocket : IDisposable
    {
        private readonly IWebSocketServerFactory _webSocketServerFactory = new WebSocketServerFactory();

        private readonly List<TcpClient> _activeClients = new();

        private TcpListener _listener;

        private bool _isDisposed;

        private TaskCompletionSource<bool> _startTcs;

        private Task _socketListenerTask;

        public Uri ServerUri { get; private set; }

        public bool IsStarted => _startTcs?.Task.IsCompleted == true ? _startTcs.Task.Result : false;

        public MockServerWebSocket()
        {
            var port = GetAvailablePortNumber();
            ServerUri = new Uri($"ws://127.0.0.1:{port}");
        }

        private int GetAvailablePortNumber()
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            var range = Enumerable.Range(8000, ushort.MaxValue);
            var portsInUse = tcpConnInfoArray.Select(x => x.Port);

            return range.Except(portsInUse).FirstOrDefault();
        }

        public Task StartAsync()
        {
            lock (ServerUri)
            {
                if (_startTcs != null &&
                    (_startTcs.Task.Status == TaskStatus.Running || IsStarted))
                {
                    return _startTcs.Task;
                }
            }

            _startTcs = new TaskCompletionSource<bool>();
            _socketListenerTask = Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        _listener = new TcpListener(IPAddress.Any, ServerUri.Port);
                        _listener.Start();

                        _startTcs.SetResult(true);

                        Trace.TraceInformation($"Server started listening on port {ServerUri.Port}");
                        while (true)
                        {
                            var tcpClient = await _listener.AcceptTcpClientAsync();
                            ProcessTcpClient(tcpClient);
                        }
                    }
                    catch (SocketException ex)
                    {
                        _startTcs.SetResult(false);

                        var message = string.Format("Error listening on port {0}. Make sure IIS or another application is not running and consuming your port.", ServerUri.Port);
                        throw new Exception(message, ex);
                    }
                });

            return _startTcs.Task;
        }

        private void ProcessTcpClient(TcpClient tcpClient)
        {
            Task.Run(() => ProcessTcpClientAsync(tcpClient));
        }

        private async Task ProcessTcpClientAsync(TcpClient tcpClient)
        {
            var source = new CancellationTokenSource();
            _activeClients.Add(tcpClient);

            try
            {
                if (_isDisposed)
                {
                    return;
                }

                // this worker thread stays alive until either of the following happens:
                // Client sends a close connection request OR
                // An unhandled exception is thrown OR
                // The server is disposed
                Trace.TraceInformation("Server: Connection opened. Reading HTTP header from stream");

                // get a secure or insecure stream
                var stream = tcpClient.GetStream();
                var context = await _webSocketServerFactory.ReadHttpHeaderFromStreamAsync(stream);
                if (context.IsWebSocketRequest)
                {
                    var options = new WebSocketServerOptions { KeepAliveInterval = TimeSpan.MaxValue };
                    Trace.TraceInformation("HTTP header has requested an upgrade to Web Socket protocol. Negotiating Web Socket handshake");

                    var webSocket = await _webSocketServerFactory.AcceptWebSocketAsync(context, options);

                    Trace.TraceInformation("Web Socket handshake response sent. Stream ready.");
                    await RespondToWebSocketRequestAsync(webSocket, source.Token);
                }
                else
                {
                    Trace.TraceInformation("HTTP header contains no web socket upgrade request. Ignoring");
                }

                Trace.TraceInformation("Server: Connection closed");
            }
            catch (ObjectDisposedException)
            {
                // Do nothing. This will be thrown if the Listener has been stopped
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
            finally
            {
                _activeClients.Remove(tcpClient);

                try
                {
                    tcpClient.Client.Close();
                    tcpClient.Close();
                    source.Cancel();
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Failed to close TCP connection: {ex}");
                }
            }
        }

        protected abstract Task RespondToWebSocketRequestAsync(WebSocket webSocket, CancellationToken token);

        public Task CloseActiveClientsAsync()
        {
            foreach (var client in _activeClients.ToArray())
            {
                client.Close();
                _activeClients.Remove(client);
            }

            // Wait to clients notices disconnection
            return Task.Delay(1000);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                // safely attempt to shut down the listener
                try
                {
                    if (_listener != null)
                    {
                        if (_listener.Server != null)
                        {
                            _listener.Server.Close();
                        }

                        _listener.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }

                Trace.TraceInformation("Web Server disposed");
            }
        }
    }
}
