using HassClient.Core.Models.Events;

namespace HassClient.WS.Messages.Commands.Subscriptions;

internal class SubscribeEventsMessage : BaseOutgoingMessage
{
    [JsonProperty]
    public string? EventType { get; set; }

    public SubscribeEventsMessage()
        : base("subscribe_events")
    {
    }

    public SubscribeEventsMessage(string eventType)
        : this()
    {
        EventType = eventType;
    }

    private bool ShouldSerializeEventType() => EventType != Event.AnyEventFilter && EventType != null;
}