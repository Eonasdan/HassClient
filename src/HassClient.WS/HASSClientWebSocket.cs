using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using HassClient.Core.Helpers;
using HassClient.Core.Models;
using HassClient.Core.Models.Events;
using HassClient.Core.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Authentication;
using HassClient.WS.Messages.Commands;
using HassClient.WS.Messages.Commands.Subscriptions;
using HassClient.WS.Messages.Response;
using HassClient.WS.Serialization;
using Newtonsoft.Json.Linq;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an abstraction layer over <see cref="ClientWebSocket"/> used by
    /// <see cref="HassWsApi"/> to send commands and subscribe for events.
    /// </summary>
    public class HassClientWebSocket : IDisposable
    {
        private const string Tag = "[" + nameof(HassClientWebSocket) + "]";

        private const int IncomingBufferSize = 4 * 1024 * 1024; // 4MB

        private readonly TimeSpan _retryingInterval = TimeSpan.FromSeconds(5);

        private readonly SemaphoreSlim _connectionSemaphore = new SemaphoreSlim(1, 1);

        private readonly SemaphoreSlim _sendingSemaphore = new SemaphoreSlim(1, 1);

        private readonly Dictionary<string, SocketEventSubscription> _socketEventSubscriptionIdByEventType = new Dictionary<string, SocketEventSubscription>();
        private readonly Dictionary<uint, Action<EventResultMessage>> _socketEventCallbacksBySubscriptionId = new Dictionary<uint, Action<EventResultMessage>>();
        private readonly ConcurrentDictionary<uint, TaskCompletionSource<BaseIncomingMessage>> _incomingMessageAwaitersById = new ConcurrentDictionary<uint, TaskCompletionSource<BaseIncomingMessage>>();

        private ConnectionParameters _connectionParameters;
        private ArraySegment<byte> _receivingBuffer;

        private Channel<EventResultMessage> _receivedEventsChannel;

        private ClientWebSocket _socket;
        private TaskCompletionSource<bool> _connectionTcs;
        private CancellationTokenSource _closeConnectionCts;
        private uint _lastSentId;
        private Task _socketListenerTask;
        private Task _eventListenerTask;
        private ConnectionStates _connectionState;

        /// <summary>
        /// Gets or sets a value indicating whether the client will try to reconnect when connection is lost.
        /// Default: <see langword="true"/>.
        /// </summary>
        public bool AutomaticReconnection { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the current connection state of the web socket.
        /// </summary>
        public ConnectionStates ConnectionState
        {
            get => _connectionState;
            private set
            {
                if (_connectionState == value) return;
                _connectionState = value;
                if (value == ConnectionStates.Connected)
                {
                    _connectionTcs.TrySetResult(true);
                }

                ConnectionStateChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the connection with the server has been
        /// lost and the client is trying to reconnect.
        /// </summary>
        public bool IsReconnecting { get; private set; }

        /// <summary>
        /// Gets the connected Home Assistant instance version.
        /// </summary>
        public CalendarVersion HaVersion { get; private set; }

        /// <summary>
        /// Gets the number of requests that are pending to be attended by the server.
        /// </summary>
        public int PendingRequestsCount => _incomingMessageAwaitersById.Count;

        /// <summary>
        /// Gets the number of event handler subscriptions.
        /// </summary>
        public int SubscriptionsCount => (int)_socketEventSubscriptionIdByEventType.Values.Sum(x => x.SubscriptionCount);

        /// <summary>
        /// Occurs when the <see cref="ConnectionState"/> is changed.
        /// </summary>
        public event EventHandler<ConnectionStates> ConnectionStateChanged;

        static HassClientWebSocket()
        {
            var converters = HassSerializer.DefaultSettings.Converters;
            if (!converters.Any(x => x is MessagesConverter))
            {
                converters.Add(new MessagesConverter());
            }
        }

        /// <summary>
        /// Connects to a Home Assistant instance using the specified connection parameters.
        /// </summary>
        /// <param name="connectionParameters">The connection parameters.</param>
        /// <param name="retries">
        /// Number of retries if connection failed. Default: 0.
        /// <para>
        /// Retries will only be performed if Home Assistant instance cannot be reached and not if:
        /// authentication fails OR
        /// invalid response from server OR
        /// connection refused by server.
        /// </para>
        /// <para>
        /// If set to <c>-1</c>, this method will try indefinitely until connection succeed or
        /// cancellation is requested. Therefore, <paramref name="cancellationToken"/> must be set
        /// to a value different to <see cref="CancellationToken.None"/> in that case.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task ConnectAsync(ConnectionParameters connectionParameters, int retries = 0, CancellationToken cancellationToken = default)
        {
            CheckIsDisposed();

            if (retries < 0 &&
                cancellationToken == CancellationToken.None)
            {
                throw new ArgumentException(
                    nameof(cancellationToken),
                    $"{nameof(cancellationToken)} must be set to a value different to {nameof(CancellationToken.None)} when retrying indefinitely");
            }

            if (ConnectionState != ConnectionStates.Disconnected)
            {
                throw new InvalidOperationException($"{nameof(HassClientWebSocket)} is not disconnected.");
            }

            _closeConnectionCts = new CancellationTokenSource();

            _receivingBuffer = new ArraySegment<byte>(new byte[IncomingBufferSize]);
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_closeConnectionCts.Token, cancellationToken);
            await InternalConnect(connectionParameters, retries, linkedCts.Token).ConfigureAwait(false);
            _connectionParameters = connectionParameters;
            _receivedEventsChannel = Channel.CreateUnbounded<EventResultMessage>();
            _eventListenerTask = Task.Factory.StartNew(CreateEventListenerTask, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Close the Home Assistant connection as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task CloseAsync(CancellationToken cancellationToken = default)
        {
            CheckIsDisposed();

            if (ConnectionState == ConnectionStates.Disconnected)
            {
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            _closeConnectionCts?.Cancel();
            await _connectionSemaphore.WaitAsync(cancellationToken);

            ClearSocketResources();
            _connectionSemaphore.Release();
        }

        /// <summary>
        /// Waits until the client state changed to connected.
        /// </summary>
        /// <param name="timeout">The maximum time to wait for connection.</param>
        /// <returns>
        /// The task object representing the asynchronous operation. The result of the task is <see langword="true"/>
        /// if the client has been connected or <see langword="false"/> if the connection has been closed.
        /// </returns>
        public Task<bool> WaitForConnectionAsync(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentException($"{nameof(timeout)} must be set greater than zero.");
            }

            return WaitForConnectionAsync(timeout, CancellationToken.None);
        }

        /// <summary>
        /// Waits until the client state changed to connected.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous operation. The result of the task is <see langword="true"/>
        /// if the client has been connected or <see langword="false"/> if the connection has been closed.
        /// </returns>
        public Task<bool> WaitForConnectionAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken == CancellationToken.None)
            {
                throw new ArgumentException($"{nameof(cancellationToken)} must be set to avoid never ending wait..");
            }

            return WaitForConnectionAsync(TimeSpan.Zero, cancellationToken);
        }

        /// <summary>
        /// Waits until the client state changed to connected.
        /// <para>
        /// Either <paramref name="timeout"/> or <paramref name="cancellationToken"/> must be set to avoid never ending wait.
        /// </para>
        /// </summary>
        /// <param name="timeout">The maximum time to wait for connection.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous operation. The result of the task is <see langword="true"/>
        /// if the client has been connected or <see langword="false"/> if the connection has been closed.
        /// </returns>
        public Task<bool> WaitForConnectionAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (timeout <= TimeSpan.Zero && cancellationToken == CancellationToken.None)
            {
                throw new ArgumentException($"Either {nameof(timeout)} or {nameof(cancellationToken)} must be set to avoid never ending wait.");
            }

            if (_connectionState == ConnectionStates.Connected)
            {
                return Task.FromResult(true);
            }

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (_connectionTcs == null)
            {
                return Task.FromResult(false);
            }

            return Task.Run(() => _connectionTcs.Task, cancellationToken);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                ClearSocketResources();

                foreach (var item in _socketEventSubscriptionIdByEventType.Values)
                {
                    item.ClearAllSubscriptions();
                }

                _socketEventSubscriptionIdByEventType.Clear();
            }
        }

        private async Task InternalConnect(ConnectionParameters connectionParameters, int retries, CancellationToken cancellationToken)
        {
            ConnectionState = ConnectionStates.Connecting;

            var retry = false;
            do
            {
                await _connectionSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                try
                {
                    retry = false;
                    cancellationToken.ThrowIfCancellationRequested();

                    _lastSentId = 0;
                    _connectionTcs = new TaskCompletionSource<bool>();
                    _socket = new ClientWebSocket();
                    await _socket.ConnectAsync(connectionParameters.Endpoint, cancellationToken).ConfigureAwait(false);

                    ConnectionState = ConnectionStates.Authenticating;

                    var incomingMsg = await ReceiveMessage<BaseMessage>(_receivingBuffer, cancellationToken);
                    if (incomingMsg is AuthenticationRequiredMessage)
                    {
                        var authMsg = new AuthenticationMessage
                        {
                            AccessToken = connectionParameters.AccessToken
                        };
                        await SendMessage(authMsg, cancellationToken);

                        incomingMsg = await ReceiveMessage<BaseMessage>(_receivingBuffer, cancellationToken);
                        switch (incomingMsg)
                        {
                            case AuthenticationInvalidMessage authenticationInvalid:
                                throw new AuthenticationException($"{Tag} Invalid authentication: {authenticationInvalid.Message}");
                            case AuthenticationOkMessage authenticationOk:
                            {
                                HaVersion = CalendarVersion.Create(authenticationOk.HaVersion);

                                if (IsReconnecting)
                                {
                                    await RestoreEventsSubscriptionsAsync(cancellationToken);
                                }

                                IsReconnecting = false;
                                ConnectionState = ConnectionStates.Connected;

                                Trace.WriteLine($"{Tag} Authentication succeed. Client connected {nameof(HaVersion)}: {HaVersion}");
                                break;
                            }
                        }
                    }
                    else
                    {
                        throw new AuthenticationException("Unexpected message received during authentication.");
                    }

                    _socketListenerTask = Task.Factory.StartNew(CreateSocketListenerTask, TaskCreationOptions.LongRunning);
                }
                catch (Exception ex)
                {
                    retry = (retries < 0 || retries-- > 0) && ex is WebSocketException;

                    if (retry)
                    {
                        Trace.WriteLine($"{Tag} Connecting attempt failed. Retrying in {_retryingInterval.TotalSeconds} seconds...");
                        await Task.Delay(_retryingInterval, cancellationToken);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (!retry && ConnectionState != ConnectionStates.Connected)
                    {
                        ClearSocketResources();
                    }

                    if (_connectionSemaphore.CurrentCount == 0)
                    {
                        _connectionSemaphore.Release();
                    }
                }
            }
            while (retry);
        }

        private async Task RestoreEventsSubscriptionsAsync(CancellationToken closeCancellationToken)
        {
            _socketEventCallbacksBySubscriptionId.Clear();

            foreach (var item in _socketEventSubscriptionIdByEventType)
            {
                ConnectionState = ConnectionStates.Restoring;

                var eventType = item.Key;
                while (true)
                {
                    var subscribeMessage = new SubscribeEventsMessage(eventType);
                    await SendMessage(subscribeMessage, closeCancellationToken);
                    var result = await ReceiveMessage<ResultMessage>(_receivingBuffer, closeCancellationToken);
                    if (result.Success)
                    {
                        var socketSubscription = item.Value;
                        socketSubscription.SubscriptionId = result.Id;
                        break;
                    }
                }
            }
        }

        private async Task CreateSocketListenerTask()
        {
            var closeCancellationToken = _closeConnectionCts.Token;

            try
            {
                while (_socket.State.HasFlag(WebSocketState.Open))
                {
                    var incomingMessage = await ReceiveMessage<BaseIncomingMessage>(_receivingBuffer, closeCancellationToken);
                    closeCancellationToken.ThrowIfCancellationRequested();

                    switch (incomingMessage)
                    {
                        case EventResultMessage eventResultMessage:
                        {
                            if (!_receivedEventsChannel.Writer.TryWrite(eventResultMessage))
                            {
                                Trace.TraceWarning($"{Tag} {nameof(_receivedEventsChannel)} is full. One event message will discarded.");
                            }

                            break;
                        }
                        case PongMessage _:
                        case ResultMessage _:
                        {
                            Debug.WriteLine($"{Tag} Command message received {incomingMessage}");
                            if (_incomingMessageAwaitersById.TryRemove(incomingMessage.Id, out var responseTcs))
                            {
                                responseTcs.SetResult(incomingMessage);
                            }
                            else
                            {
                                Trace.TraceError($"{Tag} No awaiter found for incoming message {incomingMessage}. Message will be discarded.");
                            }

                            break;
                        }
                        default:
                        {
                            if (_socket.State.HasFlag(WebSocketState.Open))
                            {
                                Trace.TraceError($"{Tag} Unexpected message type received: {incomingMessage}");
                            }

                            break;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Trace.WriteLine($"{Tag} Connection stopped for cancellation.");
                return;
            }
            catch (WebSocketException)
            {
            }

            ConnectionState = ConnectionStates.Disconnected;
            Trace.WriteLine($"{Tag} Connection ended {_socket.CloseStatus?.ToString() ?? _socket.State.ToString()}");

            if (closeCancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (AutomaticReconnection &&
                _connectionParameters != null)
            {
                IsReconnecting = true;
                _socketListenerTask = Task.Run(() => InternalConnect(_connectionParameters, -1, closeCancellationToken), closeCancellationToken);
            }
            else
            {
                ClearSocketResources();
            }
        }

        private async Task CreateEventListenerTask()
        {
            var channelReader = _receivedEventsChannel.Reader;
            while (await channelReader.WaitToReadAsync(_closeConnectionCts.Token))
            {
                while (channelReader.TryRead(out var incomingMessage))
                {
                    if (_socketEventCallbacksBySubscriptionId.TryGetValue(incomingMessage.Id, out var callback))
                    {
                        callback(incomingMessage);
                    }
                }
            }
        }

        private void CheckIsDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(HassClientWebSocket));
            }
        }

        private void ClearSocketResources()
        {
            if (ConnectionState != ConnectionStates.Disconnected)
            {
                ConnectionState = ConnectionStates.Disconnected;
                IsReconnecting = false;

                _connectionParameters = null;

                if (_connectionTcs?.TrySetResult(false) == false)
                {
                    _connectionTcs = null;
                }

                _socket.Abort();
                _socket.Dispose();

                _socketEventCallbacksBySubscriptionId.Clear();
                _incomingMessageAwaitersById.Clear();
                _receivedEventsChannel?.Writer.Complete();
            }
        }

        private async Task<TMessage> ReceiveMessage<TMessage>(ArraySegment<byte> buffer, CancellationToken cancellationToken)
                where TMessage : BaseMessage
        {
            var receivedString = new StringBuilder();
            WebSocketReceiveResult rcvResult;
            do
            {
                rcvResult = await _socket.ReceiveAsync(buffer, cancellationToken);
                var msgBytes = buffer.Skip(buffer.Offset).Take(rcvResult.Count).ToArray();
                receivedString.Append(Encoding.UTF8.GetString(msgBytes));
            }
            while (!rcvResult.EndOfMessage);

            var rcvMsg = receivedString.ToString();
            return HassSerializer.DeserializeObject<TMessage>(rcvMsg);
        }

        private Task SendMessage(BaseMessage message, CancellationToken cancellationToken)
        {
            return SendMessage(message, null, cancellationToken);
        }

        private async Task SendMessage(BaseMessage message, TaskCompletionSource<BaseIncomingMessage> responseTcs, CancellationToken cancellationToken)
        {
            try
            {
                object toSerialize = message;
                await _sendingSemaphore.WaitAsync(cancellationToken);
                if (message is BaseIdentifiableMessage identifiableMessage)
                {
                    identifiableMessage.Id = ++_lastSentId;

                    if (message is SubscribeEventsMessage)
                    {
                        _socketEventCallbacksBySubscriptionId.Add(identifiableMessage.Id, ProcessReceivedEventSubscriptionMessage);
                    }
                    else if (message is RenderTemplateMessage renderTemplateMessage)
                    {
                        _socketEventCallbacksBySubscriptionId.Add(identifiableMessage.Id, renderTemplateMessage.ProcessEventReceivedMessage);
                    }
                    else if (message is RawCommandMessage rawCommand &&
                             rawCommand.MergedObject != null)
                    {
                        var mergedMessage = HassSerializer.CreateJObject(message);
                        var mergedObject = rawCommand.MergedObject as JObject ?? HassSerializer.CreateJObject(rawCommand.MergedObject);
                        mergedMessage.Merge(mergedObject);
                        toSerialize = mergedMessage;
                    }

                    if (responseTcs != null)
                    {
                        _incomingMessageAwaitersById.TryAdd(identifiableMessage.Id, responseTcs);
                    }
                }

                var sendMsg = HassSerializer.SerializeObject(toSerialize);
                var sendBytes = Encoding.UTF8.GetBytes(sendMsg);
                var sendBuffer = new ArraySegment<byte>(sendBytes);
                await _socket.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken);
                Trace.WriteLine($"{Tag} Message sent: {toSerialize}");
            }
            finally
            {
                if (_sendingSemaphore.CurrentCount == 0)
                {
                    _sendingSemaphore.Release();
                }
            }
        }

        private void ProcessReceivedEventSubscriptionMessage(EventResultMessage eventResultMessage)
        {
            var eventResultInfo = eventResultMessage.DeserializeEvent<EventResultInfo>();
            if (_socketEventSubscriptionIdByEventType.TryGetValue(eventResultInfo.EventType, out var socketEventSubscription) &&
                socketEventSubscription.SubscriptionId == eventResultMessage.Id)
            {
                socketEventSubscription.Invoke(eventResultInfo);
            }

            if (_socketEventSubscriptionIdByEventType.TryGetValue(Event.AnyEventFilter, out socketEventSubscription) &&
                socketEventSubscription.SubscriptionId == eventResultMessage.Id)
            {
                socketEventSubscription.Invoke(eventResultInfo);
            }
        }

        private async Task<ResultMessage> SendCommandAsync(BaseOutgoingMessage commandMessage, CancellationToken cancellationToken)
        {
            if (ConnectionState != ConnectionStates.Connected)
            {
                throw new InvalidOperationException("Client not connected.");
            }

            try
            {
                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_closeConnectionCts.Token, cancellationToken);
                var responseTcs = new TaskCompletionSource<BaseIncomingMessage>(linkedCts.Token);
                await SendMessage(commandMessage, responseTcs, linkedCts.Token);

                BaseIncomingMessage incomingMessage;
                using (cancellationToken.Register(() => responseTcs.TrySetCanceled()))
                {
                    incomingMessage = await responseTcs.Task;
                }

                if (incomingMessage is ResultMessage resultMessage)
                {
                    return resultMessage;
                }

                if (incomingMessage is PongMessage)
                {
                    return new ResultMessage { Success = true };
                }
                throw new InvalidOperationException($"Unexpected incoming message type '{incomingMessage.Type}' for command type '{commandMessage.Type}'.");
            }
            catch (OperationCanceledException)
            {
                if (commandMessage.Id > 0)
                {
                    _incomingMessageAwaitersById.TryRemove(commandMessage.Id, out var _);
                    _socketEventCallbacksBySubscriptionId.Remove(commandMessage.Id);
                }

                throw;
            }
        }

        private void CheckResultMessageError(BaseOutgoingMessage commandMessage, ResultMessage resultMessage)
        {
            var errorInfo = resultMessage.Error;
            if (errorInfo == null)
            {
                return;
            }

            switch (errorInfo.Code)
            {
                case ErrorCodes.Undefined:
                case ErrorCodes.InvalidFormat:
                case ErrorCodes.IdReuse:
                case ErrorCodes.HomeAssistantError:
                case ErrorCodes.NotSupported:
                    throw new InvalidOperationException($"{errorInfo.Code}: {errorInfo.Message}");
                case ErrorCodes.Unauthorized: throw new UnauthorizedAccessException(errorInfo.Message);
                case ErrorCodes.Timeout: throw new TimeoutException(errorInfo.Message);
            }

            Trace.TraceWarning($"Error response received for command [{commandMessage}] => {resultMessage.Error}");
            Debugger.Break();
        }

        /// <summary>
        /// Sends a command message and returns the <see cref="ResultMessage"/> response from the server.
        /// </summary>
        /// <param name="commandMessage">The command message to be sent.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is the message returned by the server.
        /// </returns>
        internal async Task<ResultMessage> SendCommandWithResultAsync(BaseOutgoingMessage commandMessage, CancellationToken cancellationToken)
        {
            CheckIsDisposed();

            var resultMessage = await SendCommandAsync(commandMessage, cancellationToken);
            CheckResultMessageError(commandMessage, resultMessage);

            return resultMessage;
        }

        /// <summary>
        /// Sends a command message and if succeed returns the result response from the server deserialized
        /// as <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TResult">The type used to deserialize the message result.</typeparam>
        /// <param name="commandMessage">The command message to be sent.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is the message returned by the server.
        /// </returns>
        internal async Task<TResult> SendCommandWithResultAsync<TResult>(BaseOutgoingMessage commandMessage, CancellationToken cancellationToken)
        {
            var resultMessage = await SendCommandWithResultAsync(commandMessage, cancellationToken);
            return !resultMessage.Success ? default : resultMessage.DeserializeResult<TResult>();
        }

        /// <summary>
        /// Sends a command message and returns the value indicating whether response from the server is success.
        /// </summary>
        /// <param name="commandMessage">The command message to be sent.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The result of the task is a value indicating whether response from the server is success.
        /// </returns>
        internal async Task<bool> SendCommandWithSuccessAsync(BaseOutgoingMessage commandMessage, CancellationToken cancellationToken)
        {
            var resultMessage = await SendCommandWithResultAsync(commandMessage, cancellationToken);
            return resultMessage.Success;
        }

        /// <summary>
        /// Adds an <see cref="EventHandler{TEventArgs}"/> to an event subscription.
        /// </summary>
        /// <param name="value">The event handler to subscribe.</param>
        /// <param name="eventType">The event type to subscribe to.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The result of the task is a value indicating whether the subscription was successful.
        /// </returns>
        internal async Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(eventType))
            {
                throw new ArgumentException($"'{nameof(eventType)}' cannot be null or whitespace", nameof(eventType));
            }

            CheckIsDisposed();

            // TODO: Make AddEventHandlerSubscriptionAsync and RemoveEventHandlerSubscriptionAsync thread-safe
            if (!_socketEventSubscriptionIdByEventType.ContainsKey(eventType))
            {
                var subscribeMessage = new SubscribeEventsMessage(eventType);
                if (!await SendCommandWithSuccessAsync(subscribeMessage, cancellationToken))
                {
                    return false;
                }

                _socketEventSubscriptionIdByEventType.Add(eventType, new SocketEventSubscription(subscribeMessage.Id));
            }

            _socketEventSubscriptionIdByEventType[eventType].AddSubscription(value);
            return true;
        }

        internal async Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(eventType))
            {
                throw new ArgumentException($"'{nameof(eventType)}' cannot be null or whitespace", nameof(eventType));
            }

            CheckIsDisposed();

            if (!_socketEventSubscriptionIdByEventType.TryGetValue(eventType, out var socketEventSubscription))
            {
                return false;
            }

            socketEventSubscription.RemoveSubscription(value);

            if (socketEventSubscription.SubscriptionCount == 0)
            {
                var subscribeMessage = new UnsubscribeEventsMessage { SubscriptionId = socketEventSubscription.SubscriptionId };
                if (!await SendCommandWithSuccessAsync(subscribeMessage, cancellationToken))
                {
                    return false;
                }

                _socketEventSubscriptionIdByEventType.Remove(eventType);
            }

            return true;
        }

        internal Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, KnownEventTypes eventType, CancellationToken cancellationToken)
        {
            return AddEventHandlerSubscriptionAsync(value, eventType.ToEventTypeString(), cancellationToken);
        }

        internal Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, KnownEventTypes eventType, CancellationToken cancellationToken)
        {
            return RemoveEventHandlerSubscriptionAsync(value, eventType.ToEventTypeString(), cancellationToken);
        }

        internal Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, CancellationToken cancellationToken)
        {
            return AddEventHandlerSubscriptionAsync(value, Event.AnyEventFilter, cancellationToken);
        }

        internal Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, CancellationToken cancellationToken)
        {
            return RemoveEventHandlerSubscriptionAsync(value, Event.AnyEventFilter, cancellationToken);
        }
    }
}
