using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.Core.Tests;
using HassClient.WS.Extensions;
using NUnit.Framework;

namespace HassClient.WS.Tests.RegistryEntryApiTests.StorageEntityRegistryEntryApiTests
{
    public class ZoneApiTests : BaseHassWsApiTest
    {
        private Zone? _testZone;

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateZone()
        {
            if (_testZone == null)
            {
                _testZone = new Zone(MockHelpers.GetRandomTestName(), 20.1f, 34.6f, 10.5f, "mdi:fan", true);
                var result = await HassWsApi.CreateStorageEntityRegistryEntryAsync(_testZone);

                Assert.That(result, Is.True, "SetUp failed");
                return;
            }

            Assert.That(_testZone, Is.Not.Null);
            Assert.That(_testZone.UniqueId, Is.Not.Null);
            Assert.That(_testZone.Name, Is.Not.Null);
            Assert.That(_testZone.HasPendingChanges, Is.False);
            Assert.That(_testZone.IsTracked, Is.True);
        }

        [Test, Order(2)]
        public async Task GetZones()
        {
            IEnumerable<Zone?> result = await HassWsApi.GetStorageEntityRegistryEntriesAsync<Zone>();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Contains(_testZone), Is.True);
            Assert.That(result.All(x => x.Id != null), Is.True);
            Assert.That(result.All(x => x.UniqueId != null), Is.True);
            Assert.That(result.All(x => x.EntityId.StartsWith("zone.")), Is.True);
            Assert.That(result.Any(x => x.Name != null), Is.True);
            Assert.That(result.Any(x => x.Longitude > 0), Is.True);
            Assert.That(result.Any(x => x.Latitude > 0), Is.True);
            Assert.That(result.Any(x => x.Longitude != x.Latitude), Is.True);
            Assert.That(result.Any(x => x.Radius > 0), Is.True);
            Assert.That(result.Any(x => x.IsPassive), Is.True);
            Assert.That(result.Any(x => x.Icon != null), Is.True);
        }

        [Test, Order(3)]
        public async Task UpdateZoneName()
        {
            _testZone.Name = $"{nameof(ZoneApiTests)}_{DateTime.Now.Ticks}";
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testZone);

            Assert.That(result, Is.True);
            Assert.That(_testZone.HasPendingChanges, Is.False);
        }

        [Test, Order(4)]
        public async Task UpdateZoneInitial()
        {
            _testZone.IsPassive = false;
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testZone);

            Assert.That(result, Is.True);
            Assert.That(_testZone.HasPendingChanges, Is.False);
        }

        [Test, Order(5)]
        public async Task UpdateZoneIcon()
        {
            _testZone.Icon = "mdi:lightbulb";
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testZone);

            Assert.That(result, Is.True);
            Assert.That(_testZone.HasPendingChanges, Is.False);
        }

        [Test, Order(6)]
        public async Task UpdateWithForce()
        {
            var initialName = _testZone.Name;
            var initialIcon = _testZone.Icon;
            var initialLongitude = _testZone.Longitude;
            var initialLatitude = _testZone.Latitude;
            var initialRadius = _testZone.Radius;
            var initialIsPassive = _testZone.IsPassive;
            var clonedEntry = _testZone.Clone();
            clonedEntry.Name = $"{initialName}_cloned";
            clonedEntry.Icon = $"{initialIcon}_cloned";
            clonedEntry.Longitude = initialLongitude + 15f;
            clonedEntry.Latitude = initialLatitude + 15f;
            clonedEntry.Radius = initialRadius + 15f;
            clonedEntry.IsPassive = !initialIsPassive;
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(clonedEntry);
            Assert.That(result, Is.True, "SetUp failed");
            Assert.That(_testZone.HasPendingChanges, Is.False, "SetUp failed");

            result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testZone, forceUpdate: true);
            Assert.That(result, Is.True);
            Assert.That(initialName, Is.EqualTo(_testZone.Name));
            Assert.That(initialIcon, Is.EqualTo(_testZone.Icon));
            Assert.That(initialLongitude, Is.EqualTo(_testZone.Longitude));
            Assert.That(initialLatitude, Is.EqualTo(_testZone.Latitude));
            Assert.That(initialRadius, Is.EqualTo(_testZone.Radius));
            Assert.That(initialIsPassive, Is.EqualTo(_testZone.IsPassive));
            Assert.That(_testZone.HasPendingChanges, Is.False);
        }

        [OneTimeTearDown]
        [Test, Order(7)]
        public async Task DeleteZone()
        {
            if (_testZone == null)
            {
                return;
            }

            var result = await HassWsApi.DeleteStorageEntityRegistryEntryAsync(_testZone);
            var deletedZone = _testZone;
            _testZone = null;

            Assert.That(result, Is.True);
            Assert.That(deletedZone.IsTracked, Is.False);
        }
    }
}
