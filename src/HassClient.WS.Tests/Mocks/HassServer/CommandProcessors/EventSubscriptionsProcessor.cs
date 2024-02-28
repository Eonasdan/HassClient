using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Helpers;
using HassClient.Core.Models.Events;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands.Subscriptions;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    public class EventSubscriptionsProcessor : BaseCommandProcessor
    {
        private readonly Dictionary<string, List<uint>> _subscribersByEventType = new();

        public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
        {
            return receivedCommand is SubscribeEventsMessage || receivedCommand is UnsubscribeEventsMessage;
        }

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            if (receivedCommand is SubscribeEventsMessage subscribeMessage)
            {
                var eventType = subscribeMessage.EventType ?? KnownEventTypes.Any.ToEventTypeString();
                if (!_subscribersByEventType.TryGetValue(eventType, out var subscribers))
                {
                    subscribers = new List<uint>();
                    _subscribersByEventType.Add(eventType, subscribers);

                }
                subscribers.Add(subscribeMessage.Id);
                return CreateResultMessageWithResult(null);
            }

            if (receivedCommand is not UnsubscribeEventsMessage unsubscribeMessage)
                return CreateResultMessageWithResult(null);
            
            foreach (var item in _subscribersByEventType.Values.Where(item => item.Remove(unsubscribeMessage.SubscriptionId)))
            {
                //success = true;
                break;
            }

            return CreateResultMessageWithResult(null);
        }

        public bool TryGetSubscribers(KnownEventTypes eventType, out List<uint> subscribers)
        {
            subscribers = new List<uint>();
            if (eventType != KnownEventTypes.Any &&
                _subscribersByEventType.TryGetValue(KnownEventTypes.Any.ToEventTypeString(), out var anySubscribers))
            {
                subscribers.AddRange(anySubscribers);
            }

            if (_subscribersByEventType.TryGetValue(eventType.ToEventTypeString(), out var typeSubscribers))
            {
                subscribers.AddRange(typeSubscribers);
            }

            return subscribers.Count > 0;
        }

        public void ClearSubscriptions()
        {
            _subscribersByEventType.Clear();
        }
    }
}
