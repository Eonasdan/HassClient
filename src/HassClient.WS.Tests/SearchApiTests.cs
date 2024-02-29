using System;
using System.Threading.Tasks;
using HassClient.WS.Messages.Commands.Search;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public class SearchApiTests : BaseHassWsApiTest
    {
        public static Array GetItemTypes()
        {
            return Enum.GetValues(typeof(ItemTypes));
        }

        [TestCaseSource(nameof(GetItemTypes))]
        public async Task SearchRelatedUnknown(ItemTypes itemTypes)
        {
            var result = await HassWsApi.SearchRelatedAsync(itemTypes, $"Unknown_{DateTime.Now.Ticks}");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AreaIds, Is.Null);
            Assert.That(result.AutomationIds, Is.Null);
            Assert.That(result.ConfigEntryIds, Is.Null);
            Assert.That(result.DeviceIds, Is.Null);
            Assert.That(result.EntityIds, Is.Null);
        }

        [Test]
        public async Task SearchRelatedKnownEntity()
        {
            var result = await HassWsApi.SearchRelatedAsync(ItemTypes.Entity, "light.bed_light");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ConfigEntryIds, Is.Not.Null);
            Assert.That(result.DeviceIds, Is.Not.Null);
            Assert.That(result.ConfigEntryIds.Length, Is.Not.Zero);
            Assert.That(result.DeviceIds.Length, Is.Not.Zero);
        }
    }
}
