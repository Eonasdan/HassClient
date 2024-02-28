using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HassClient.WS.Messages.Commands;
using HassClient.WS.Messages.Response;
using HassClient.WS.Tests.Mocks;
using HassClient.WS.Tests.Mocks.HassServer;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public class HassClientWebSocketTests
    {
        private MockHassServerWebSocket _mockServer;
        private HassClientWebSocket _wsClient;
        private CancellationTokenSource _connectionCts;
        private MockEventSubscriber _connectionChangedSubscriber;

        [SetUp]
        public void SetUp()
        {
            _mockServer = new MockHassServerWebSocket();
            _connectionChangedSubscriber = new MockEventSubscriber();
            _connectionCts = new CancellationTokenSource();
            _wsClient = new HassClientWebSocket();
            _wsClient.ConnectionStateChanged += _connectionChangedSubscriber.Handle;
        }

        private async Task StartMockServerAsync()
        {
            await _mockServer.StartAsync();

            Assert.IsTrue(_mockServer.IsStarted, "SetUp Failed: Mock server not started.");
        }

        private Task ConnectClientAsync(int retries = 0)
        {
            return _wsClient.ConnectAsync(_mockServer.ConnectionParameters, retries, _connectionCts.Token);
        }

        private async Task StartMockServerAndConnectClientAsync()
        {
            await StartMockServerAsync();
            await ConnectClientAsync();
        }

        [Test]
        public async Task ConnectionStatusChangedRaisedWhenConnecting()
        {
            await StartMockServerAndConnectClientAsync();

            Assert.AreEqual(3, _connectionChangedSubscriber.HitCount);
            Assert.AreEqual(new[] { ConnectionStates.Connecting, ConnectionStates.Authenticating, ConnectionStates.Connected }, _connectionChangedSubscriber.ReceivedEventArgs);
        }

        [Test]
        public async Task ConnectionStatusChangedRaisedWhenClosing()
        {
            await StartMockServerAndConnectClientAsync();
            _connectionChangedSubscriber.Reset();
            await _wsClient.CloseAsync();

            Assert.AreEqual(1, _connectionChangedSubscriber.HitCount);
            Assert.AreEqual(new[] { ConnectionStates.Disconnected }, _connectionChangedSubscriber.ReceivedEventArgs);
        }

        [Test]
        public async Task ConnectionStatusChangedWithDisconnectedRaisedWhenServerCloses()
        {
            await StartMockServerAndConnectClientAsync();
            _connectionChangedSubscriber.Reset();
            await _mockServer.CloseActiveClientsAsync();

            Assert.GreaterOrEqual(_connectionChangedSubscriber.HitCount, 1);
            Assert.AreEqual(ConnectionStates.Disconnected, _connectionChangedSubscriber.ReceivedEventArgs.FirstOrDefault());
        }

        [Test]
        public void SendCommandWhenNotConnectedThrows()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => _wsClient.SendCommandWithResultAsync(default, default));
        }

        [Test]
        public async Task CancelConnectOnceConnectedHasNoEffect()
        {
            await StartMockServerAndConnectClientAsync();

            _connectionCts.Cancel();

            Assert.AreEqual(ConnectionStates.Connected, _wsClient.ConnectionState);
        }

        [Test]
        public async Task CancelConnectWhileAuthenticating()
        {
            _mockServer.IgnoreAuthenticationMessages = true;
            await StartMockServerAsync();

            var connectTask = ConnectClientAsync();
            await _connectionChangedSubscriber.WaitEventArgWithTimeoutAsync(ConnectionStates.Authenticating, 1000);

            Assert.AreEqual(ConnectionStates.Authenticating, _wsClient.ConnectionState, "SetUp Failed");

            _connectionCts.Cancel();

            Assert.CatchAsync<OperationCanceledException>(() => connectTask);
            Assert.AreEqual(ConnectionStates.Disconnected, _wsClient.ConnectionState);
            Assert.AreEqual(TaskStatus.Canceled, connectTask.Status);
        }

        [Test]
        public async Task CloseWhileConnecting()
        {
            _mockServer.IgnoreAuthenticationMessages = true;
            await StartMockServerAsync();
            var connectTask = ConnectClientAsync();

            await _wsClient.CloseAsync();

            Assert.CatchAsync<OperationCanceledException>(async () => await connectTask);
            Assert.AreEqual(ConnectionStates.Disconnected, _wsClient.ConnectionState);
        }

        [Test]
        public void ConnectWithInfiniteRetriesAndNoCancellationTokenThrows()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _wsClient.ConnectAsync(new ConnectionParameters(), -1));
        }

        [Test]
        public async Task ConnectWithInvalidAuthenticationThrows()
        {
            await StartMockServerAsync().ConfigureAwait(false);

            var invalidParameters = new ConnectionParameters
            {
                Endpoint = _mockServer.ConnectionParameters.Endpoint,
                AccessToken = "Invalid_Access_Token"
            };

            await AssertExtensions.ThrowsAsync<AuthenticationException>(_wsClient.ConnectAsync(invalidParameters));
        }

        [Test, NonParallelizable]
        public async Task ConnectWithRetriesAndInvalidAuthenticationThrows()
        {
            await StartMockServerAsync().ConfigureAwait(false);

            var invalidParameters = new ConnectionParameters
            {
                Endpoint = _mockServer.ConnectionParameters.Endpoint,
                AccessToken = "Invalid_Access_Token"
            };

            await AssertExtensions.ThrowsAsync<AuthenticationException>(_wsClient.ConnectAsync(invalidParameters, -1, _connectionCts.Token));
        }

        [Test]
        public async Task ConnectOnceWhileConnectingThrows()
        {
            await StartMockServerAndConnectClientAsync();

            Assert.AreNotEqual(ConnectionStates.Disconnected, _wsClient.ConnectionState);
            Assert.ThrowsAsync<InvalidOperationException>(() => ConnectClientAsync());
        }

        [Test]
        public void ConnectOnceDisposedThrows()
        {
            _wsClient.Dispose();

            Assert.IsTrue(_wsClient.IsDisposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => StartMockServerAndConnectClientAsync());
        }

        [Test]
        public void CloseOnceDisposedThrows()
        {
            _wsClient.Dispose();

            Assert.IsTrue(_wsClient.IsDisposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => _wsClient.CloseAsync());
        }

        [Test]
        public void SendCommandOnceDisposedThrows()
        {
            _wsClient.Dispose();

            Assert.IsTrue(_wsClient.IsDisposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => _wsClient.SendCommandWithSuccessAsync(default, default));
        }

        [Test]
        public void AddEventHandlerSubscriptionOnceDisposedThrows()
        {
            _wsClient.Dispose();

            Assert.IsTrue(_wsClient.IsDisposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => _wsClient.AddEventHandlerSubscriptionAsync(default, default));
        }

        [Test]
        public void RemoveEventHandlerSubscriptionOnceDisposedThrows()
        {
            _wsClient.Dispose();

            Assert.IsTrue(_wsClient.IsDisposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => _wsClient.RemoveEventHandlerSubscriptionAsync(default, default));
        }

        [Test]
        public async Task CancelBeforeAddingEventHandlerSubscriptionThrows()
        {
            await StartMockServerAndConnectClientAsync();
            _mockServer.IgnoreAuthenticationMessages = true;
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var eventSubscriber = new MockEventSubscriber();
            var subscriptionTask = _wsClient.AddEventHandlerSubscriptionAsync(eventSubscriber.Handle, cancellationTokenSource.Token);

            Assert.Zero(_wsClient.SubscriptionsCount);
            Assert.Zero(_wsClient.PendingRequestsCount);
            Assert.CatchAsync<OperationCanceledException>(() => subscriptionTask);
        }

        [Test]
        public async Task CancelAfterAddingEventHandlerSubscriptionThrows()
        {
            await StartMockServerAndConnectClientAsync();
            _mockServer.ResponseSimulatedDelay = TimeSpan.MaxValue;
            var cancellationTokenSource = new CancellationTokenSource();
            var eventSubscriber = new MockEventSubscriber();
            var subscriptionTask = _wsClient.AddEventHandlerSubscriptionAsync(eventSubscriber.Handle, cancellationTokenSource.Token);
            Assert.NotZero(_wsClient.PendingRequestsCount);

            cancellationTokenSource.Cancel();

            Assert.Zero(_wsClient.SubscriptionsCount);
            Assert.Zero(_wsClient.PendingRequestsCount);
            Assert.CatchAsync<OperationCanceledException>(() => subscriptionTask);
        }

        [Test]
        public async Task CancelBeforeSendingCommandThrows()
        {
            await StartMockServerAndConnectClientAsync();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var sendTask = _wsClient.SendCommandWithSuccessAsync(new PingMessage(), cancellationTokenSource.Token);

            Assert.Zero(_wsClient.PendingRequestsCount);
            Assert.CatchAsync<OperationCanceledException>(() => sendTask);
        }

        [Test, Repeat(200)]
        public async Task CancelAfterSendingCommandThrows()
        {
            await StartMockServerAndConnectClientAsync();
            _mockServer.ResponseSimulatedDelay = TimeSpan.MaxValue;

            var cancellationTokenSource = new CancellationTokenSource();
            var sendTask = _wsClient.SendCommandWithSuccessAsync(new PingMessage(), cancellationTokenSource.Token);
            Assert.NotZero(_wsClient.PendingRequestsCount);

            cancellationTokenSource.Cancel();

            Assert.Zero(_wsClient.PendingRequestsCount);

            Assert.CatchAsync<OperationCanceledException>(() => sendTask);
        }

        [Test]
        public async Task Reconnection()
        {
            await StartMockServerAndConnectClientAsync();

            await _mockServer.CloseActiveClientsAsync();
            await _wsClient.WaitForConnectionAsync(TimeSpan.FromMilliseconds(200));

            Assert.AreEqual(ConnectionStates.Connected, _wsClient.ConnectionState);
        }

        [Test]
        public async Task AddedEventHandlerSubscriptionsAreRestoredAfterReconnection()
        {
            var eventSubscriber = new MockEventSubscriber();
            await StartMockServerAndConnectClientAsync();
            var result = await _wsClient.AddEventHandlerSubscriptionAsync(eventSubscriber.Handle, default);
            Assert.IsTrue(result, "SetUp failed");

            await _mockServer.CloseActiveClientsAsync();
            await _wsClient.WaitForConnectionAsync(TimeSpan.FromMilliseconds(200));

            var entityId = "test.mock";
            await _mockServer.RaiseStateChangedEventAsync(entityId);
            var eventResult = await eventSubscriber.WaitFirstEventArgWithTimeoutAsync<EventResultInfo>(500);

            Assert.AreEqual(1, eventSubscriber.HitCount);
            Assert.AreEqual(1, eventSubscriber.ReceivedEventArgs.Count());
            Assert.NotNull(eventResult);
        }
    }
}
