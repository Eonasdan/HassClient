using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.Core.Tests;
using NUnit.Framework;

namespace HassClient.WS.Tests.RegistryEntryApiTests.StorageEntityRegistryEntryApiTests
{
    public class PersonApiTests : BaseHassWsApiTest
    {
        private Person? _testPerson;

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreatePerson()
        {
            if (_testPerson == null)
            {
                var testUser = new User(MockHelpers.GetRandomTestName(), false);
                var result = await HassWsApi.CreateUserAsync(testUser);
                Assert.That(result, Is.True, "SetUp failed");

                _testPerson = new Person(testUser.Name, testUser);
                result = await HassWsApi.CreateStorageEntityRegistryEntryAsync(_testPerson);
                Assert.That(result, Is.True, "SetUp failed");
                return;
            }

            Assert.That(_testPerson, Is.Not.Null);
            Assert.That(_testPerson.UniqueId, Is.Not.Null);
            Assert.That(_testPerson.Name, Is.Not.Null);
            Assert.That(_testPerson.HasPendingChanges, Is.False);
            Assert.That(_testPerson.IsTracked, Is.True);
        }

        [Test, Order(2)]
        public async Task GetPersons()
        {
            IEnumerable<Person?> result = await HassWsApi.GetStorageEntityRegistryEntriesAsync<Person>();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Contains(_testPerson), Is.True);
            Assert.That(result.All(x => x.Id != null), Is.True);
            Assert.That(result.All(x => x.UniqueId != null), Is.True);
            Assert.That(result.All(x => x.EntityId.StartsWith("person.")), Is.True);
            Assert.That(result.Any(x => x.Name != null), Is.True);
            Assert.That(_testPerson.HasPendingChanges, Is.False);
        }

        [Test, Order(3)]
        public async Task UpdatePersonName()
        {
            _testPerson.Name = $"{nameof(PersonApiTests)}_{DateTime.Now.Ticks}";
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson);

            Assert.That(result, Is.True);
            Assert.That(_testPerson.HasPendingChanges, Is.False);
        }

        [Test, Order(3)]
        public async Task UpdatePersonPicture()
        {
            _testPerson.Picture = "test/Picture.png";
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson);

            Assert.That(result, Is.True);
            Assert.That(_testPerson.HasPendingChanges, Is.False);
        }

        [Test, Order(3)]
        public async Task UpdatePersonDeviceTrackers()
        {
            _testPerson.DeviceTrackers.Add($"device_tracker.{MockHelpers.GetRandomTestName()}");
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson);

            Assert.That(result, Is.True);
            Assert.That(_testPerson.HasPendingChanges, Is.False);
        }

        [Test, Order(3)]
        public async Task UpdatePersonUserId()
        {
            var testUser = new User(MockHelpers.GetRandomTestName(), false);
            var result = await HassWsApi.CreateUserAsync(testUser);
            Assert.That(result, Is.True, "SetUp failed");

            _testPerson.ChangeUser(testUser);
            result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson);

            Assert.That(result, Is.True);
            Assert.That(_testPerson.HasPendingChanges, Is.False);
        }

        [Test, Order(4)]
        public async Task UpdateWithForce()
        {
            var initialName = _testPerson.Name;
            var clonedEntry = _testPerson.Clone();
            clonedEntry.Name = $"{initialName}_cloned";
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(clonedEntry);
            Assert.That(result, Is.True, "SetUp failed");
            Assert.That(_testPerson.HasPendingChanges, Is.False, "SetUp failed");

            result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson, forceUpdate: true);
            Assert.That(result, Is.True);
            Assert.That(initialName, Is.EqualTo(_testPerson.Name));
            Assert.That(_testPerson.HasPendingChanges, Is.False);
        }

        [OneTimeTearDown]
        [Test, Order(5)]
        public async Task DeletePerson()
        {
            if (_testPerson == null)
            {
                return;
            }

            var result = await HassWsApi.DeleteStorageEntityRegistryEntryAsync(_testPerson);
            var deletedPerson = _testPerson;
            _testPerson = null;

            Assert.That(result, Is.True);
            Assert.That(deletedPerson.IsTracked, Is.False);
        }
    }
}
