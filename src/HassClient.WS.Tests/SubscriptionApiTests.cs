using System.Threading.Tasks;
using HassClient.Core.Helpers;
using HassClient.Core.Models;
using HassClient.Core.Models.Events;
using HassClient.Core.Serialization;
using HassClient.WS.Extensions;
using HassClient.WS.Messages.Response;
using HassClient.WS.Tests.Mocks;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public class SubscriptionApiTests : BaseHassWsApiTest
    {
        private const string TestEntitytId = "light.ceiling_lights";

        private async Task<StateChangedEvent> ForceStateChangedAndGetEventData(MockEventSubscriber subscriber)
        {
            var domain = TestEntitytId.GetDomain();
            var update = await HassWsApi.CallServiceForEntitiesAsync(domain, "toggle", TestEntitytId);
            Assert.That(update, Is.Not.Null, "SetUp failed");

            var eventResultInfo = await subscriber.WaitFirstEventArgWithTimeoutAsync<EventResultInfo>(
                                            x => HassSerializer.TryGetEnumFromSnakeCase<KnownEventTypes>(x.EventType, out var knownEventType) &&
                                                   knownEventType == KnownEventTypes.StateChanged,
                                            500);

            Assert.That(eventResultInfo, Is.Not.Null, "SetUp failed");

            return eventResultInfo.DeserializeData<StateChangedEvent>();
        }

        [Test]
        public async Task AddMultipleEventHandlerSubscriptionForAnyEvent()
        {
            var testEventHandler1 = new MockEventHandler<EventResultInfo>();
            var testEventHandler2 = new MockEventHandler<EventResultInfo>();
            var subscriber1 = new MockEventSubscriber();
            var subscriber2 = new MockEventSubscriber();
            testEventHandler1.Event += subscriber1.Handle;
            testEventHandler2.Event += subscriber2.Handle;
            var result1 = await HassWsApi.AddEventHandlerSubscriptionAsync(testEventHandler1.EventHandler);
            var result2 = await HassWsApi.AddEventHandlerSubscriptionAsync(testEventHandler2.EventHandler);

            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);

            var eventData = await ForceStateChangedAndGetEventData(subscriber1);

            Assert.That(subscriber1.HitCount, Is.Not.Zero);
            Assert.That(subscriber1.HitCount, Is.EqualTo(subscriber2.HitCount));
            Assert.That(eventData.EntityId == TestEntitytId, Is.True);
        }

        [Test]
        public async Task AddEventHandlerSubscriptionForAnyEvent()
        {
            var testEventHandler = new MockEventHandler<EventResultInfo>();
            var subscriber = new MockEventSubscriber();
            testEventHandler.Event += subscriber.Handle;
            var result = await HassWsApi.AddEventHandlerSubscriptionAsync(testEventHandler.EventHandler);

            Assert.That(result, Is.True);

            await ForceStateChangedAndGetEventData(subscriber);

            Assert.That(subscriber.HitCount, Is.Not.Zero);
        }

        [Test]
        public async Task AddEventHandlerSubscriptionForStateChangedEvents()
        {
            var testEventHandler = new MockEventHandler<EventResultInfo>();
            var subscriber = new MockEventSubscriber();
            testEventHandler.Event += subscriber.Handle;
            var result = await HassWsApi.AddEventHandlerSubscriptionAsync(testEventHandler.EventHandler, KnownEventTypes.StateChanged);

            Assert.That(result, Is.True);

            var eventData = await ForceStateChangedAndGetEventData(subscriber);

            Assert.That(subscriber.HitCount, Is.Not.Zero);
            Assert.That(eventData.EntityId == TestEntitytId, Is.True);
            Assert.That(eventData.NewState.State, Is.Not.Null);
        }
    }
}
