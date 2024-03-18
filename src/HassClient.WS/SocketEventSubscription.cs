using System;
using HassClient.WS.Messages.Response;

namespace HassClient.WS;

internal class SocketEventSubscription
{
    private Delegate? _internalEventHandler;

    public uint SubscriptionId { get; set; }

    public uint SubscriptionCount { get; private set; }

    public SocketEventSubscription(uint subscriptionId)
    {
        SubscriptionId = subscriptionId;
    }

    public void AddSubscription<T>(T eventHandler) where T : Delegate
    {
        var tempDelegate = Delegate.Combine(_internalEventHandler, eventHandler);
        _internalEventHandler = tempDelegate as T;
        SubscriptionCount++;
    }

    public void RemoveSubscription<T>(T eventHandler) where T : Delegate
    {
        var tempDelegate = Delegate.Remove(_internalEventHandler, eventHandler);
        _internalEventHandler = tempDelegate as T;
        SubscriptionCount--;
    }

    public void Invoke(EventResultInfo eventResultInfo)
    {
        _internalEventHandler?.DynamicInvoke(this, eventResultInfo);
    }

    public void ClearAllSubscriptions()
    {
        _internalEventHandler = null;
        SubscriptionCount = 0;
    }
}