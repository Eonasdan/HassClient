using System.Threading.Tasks;
using HassClient.Core.Helpers;
using HassClient.Core.Models;
using HassClient.Core.Models.Events;
using HassClient.Core.Serialization;
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
            Assert.NotNull(update, "SetUp failed");

            var eventResultInfo = await subscriber.WaitFirstEventArgWithTimeoutAsync<EventResultInfo>(
                                            x => HassSerializer.TryGetEnumFromSnakeCase<KnownEventTypes>(x.EventType, out var knownEventType) &&
                                                   knownEventType == KnownEventTypes.StateChanged,
                                            500);

            Assert.NotNull(eventResultInfo, "SetUp failed");

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

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);

            var eventData = await ForceStateChangedAndGetEventData(subscriber1);

            Assert.NotZero(subscriber1.HitCount);
            Assert.AreEqual(subscriber1.HitCount, subscriber2.HitCount);
            Assert.IsTrue(eventData.EntityId == TestEntitytId);
        }

        [Test]
        public async Task AddEventHandlerSubscriptionForAnyEvent()
        {
            var testEventHandler = new MockEventHandler<EventResultInfo>();
            var subscriber = new MockEventSubscriber();
            testEventHandler.Event += subscriber.Handle;
            var result = await HassWsApi.AddEventHandlerSubscriptionAsync(testEventHandler.EventHandler);

            Assert.IsTrue(result);

            await ForceStateChangedAndGetEventData(subscriber);

            Assert.NotZero(subscriber.HitCount);
        }

        [Test]
        public async Task AddEventHandlerSubscriptionForStateChangedEvents()
        {
            var testEventHandler = new MockEventHandler<EventResultInfo>();
            var subscriber = new MockEventSubscriber();
            testEventHandler.Event += subscriber.Handle;
            var result = await HassWsApi.AddEventHandlerSubscriptionAsync(testEventHandler.EventHandler, KnownEventTypes.StateChanged);

            Assert.IsTrue(result);

            var eventData = await ForceStateChangedAndGetEventData(subscriber);

            Assert.NotZero(subscriber.HitCount);
            Assert.IsTrue(eventData.EntityId == TestEntitytId);
            Assert.NotNull(eventData.NewState.State);
        }
    }
}
