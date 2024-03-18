using HomeAssistantClient.Core.Socket;
using HomeAssistantClient.Core.Socket.Models;
using HomeAssistantClient.Core.Socket.Models.Events;

namespace HomeAssistantClient;

public class WebSocketClient
{
    private readonly HassClientWebSocket _hassClientWebSocket = new();

    /// <summary>
    /// Gets the current connection state of the web socket.
    /// </summary>
    public ConnectionStates ConnectionState => _hassClientWebSocket.ConnectionState;

    /// <summary>
    /// Occurs when the <see cref="ConnectionState"/> is changed.
    /// </summary>
    public event EventHandler<ConnectionStates>? ConnectionStateChanged
    {
        add => _hassClientWebSocket.ConnectionStateChanged += value;
        remove => _hassClientWebSocket.ConnectionStateChanged -= value;
    }

    /// <summary>
    /// Gets the <see cref="StateChangedEventListener"/> instance of this client instance.
    /// </summary>
    public StateChangedEventListener? StateChangedEventListener { get; private set; }

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
    /// <returns>A task representing the connection work.</returns>
    public async Task ConnectAsync(ConnectionParameters? connectionParameters, int retries = 0,
        CancellationToken cancellationToken = default)
    {
        await _hassClientWebSocket.ConnectAsync(connectionParameters, retries, () =>
        {
            StateChangedEventListener = new StateChangedEventListener();
            StateChangedEventListener.Initialize(_hassClientWebSocket);
        }, cancellationToken);
    }

    /// <summary>
    /// Close the Home Assistant connection as an asynchronous operation.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public Task CloseAsync(CancellationToken cancellationToken = default)
    {
        return _hassClientWebSocket.CloseAsync(cancellationToken);
    }

    /// <summary>
    /// Subscribes an <see cref="EventHandler{EventResultInfo}"/> to handle events received from the Home Assistance instance.
    /// </summary>
    /// <param name="value">The <see cref="EventHandler{EventResultInfo}"/> to be included.</param>
    /// <param name="eventType">The event type to listen to. By default, no filter will be applied.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// subscription was successfully done.
    /// </returns>
    public Task<bool> AddEventHandlerSubscriptionAsync<T>(T value,
        KnownEventTypes eventType = KnownEventTypes.Any,
        CancellationToken cancellationToken = default) where T : Delegate
    {
        return _hassClientWebSocket.AddEventHandlerSubscriptionAsync(value, eventType, cancellationToken);
    }

    /// <summary>
    /// Removes an <see cref="EventHandler{EventResultInfo}"/> subscription.
    /// </summary>
    /// <param name="value">The <see cref="EventHandler{EventResultInfo}"/> to be removed.</param>
    /// <param name="eventType">The event type filter used in the subscription.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// subscription removal was successfully done.
    /// </returns>
    public Task<bool> RemoveEventHandlerSubscriptionAsync<T>(T value,
        KnownEventTypes eventType = KnownEventTypes.Any, CancellationToken cancellationToken = default)
        where T : Delegate
    {
        return _hassClientWebSocket.RemoveEventHandlerSubscriptionAsync(value, eventType, cancellationToken);
    }

    /// <summary>
    /// Subscribes an <see cref="EventHandler{EventResultInfo}"/> to handle events received from the Home Assistance instance.
    /// </summary>
    /// <param name="value">The <see cref="EventHandler{EventResultInfo}"/> to be included.</param>
    /// <param name="eventType">The event type to listen to. By default, no filter will be applied.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// subscription was successfully done.
    /// </returns>
    public Task<bool> AddEventHandlerSubscriptionAsync<T>(T value, string eventType,
        CancellationToken cancellationToken = default) where T : Delegate
    {
        return _hassClientWebSocket.AddEventHandlerSubscriptionAsync(value, eventType, cancellationToken);
    }

    /// <summary>
    /// Removes an <see cref="EventHandler{EventResultInfo}"/> subscription.
    /// </summary>
    /// <param name="value">The <see cref="EventHandler{EventResultInfo}"/> to be removed.</param>
    /// <param name="eventType">The event type filter used in the subscription.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
    /// subscription removal was successfully done.
    /// </returns>
    public Task<bool> RemoveEventHandlerSubscriptionAsync<T>(T value, string eventType,
        CancellationToken cancellationToken = default) where T : Delegate
    {
        return _hassClientWebSocket.RemoveEventHandlerSubscriptionAsync(value, eventType, cancellationToken);
    }
}