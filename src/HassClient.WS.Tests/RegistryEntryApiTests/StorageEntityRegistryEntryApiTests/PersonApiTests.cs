using System;
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
        private Person _testPerson;

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreatePerson()
        {
            if (_testPerson == null)
            {
                var testUser = new User(MockHelpers.GetRandomTestName(), false);
                var result = await HassWsApi.CreateUserAsync(testUser);
                Assert.IsTrue(result, "SetUp failed");

                _testPerson = new Person(testUser.Name, testUser);
                result = await HassWsApi.CreateStorageEntityRegistryEntryAsync(_testPerson);
                Assert.IsTrue(result, "SetUp failed");
                return;
            }

            Assert.NotNull(_testPerson);
            Assert.NotNull(_testPerson.UniqueId);
            Assert.NotNull(_testPerson.Name);
            Assert.IsFalse(_testPerson.HasPendingChanges);
            Assert.IsTrue(_testPerson.IsTracked);
        }

        [Test, Order(2)]
        public async Task GetPersons()
        {
            var result = await HassWsApi.GetStorageEntityRegistryEntriesAsync<Person>();

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsTrue(result.Contains(_testPerson));
            Assert.IsTrue(result.All(x => x.Id != null));
            Assert.IsTrue(result.All(x => x.UniqueId != null));
            Assert.IsTrue(result.All(x => x.EntityId.StartsWith("person.")));
            Assert.IsTrue(result.Any(x => x.Name != null));
            Assert.IsFalse(_testPerson.HasPendingChanges);
        }

        [Test, Order(3)]
        public async Task UpdatePersonName()
        {
            _testPerson.Name = $"{nameof(PersonApiTests)}_{DateTime.Now.Ticks}";
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson);

            Assert.IsTrue(result);
            Assert.IsFalse(_testPerson.HasPendingChanges);
        }

        [Test, Order(3)]
        public async Task UpdatePersonPicture()
        {
            _testPerson.Picture = "test/Picture.png";
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson);

            Assert.IsTrue(result);
            Assert.IsFalse(_testPerson.HasPendingChanges);
        }

        [Test, Order(3)]
        public async Task UpdatePersonDeviceTrackers()
        {
            _testPerson.DeviceTrackers.Add($"device_tracker.{MockHelpers.GetRandomTestName()}");
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson);

            Assert.IsTrue(result);
            Assert.IsFalse(_testPerson.HasPendingChanges);
        }

        [Test, Order(3)]
        public async Task UpdatePersonUserId()
        {
            var testUser = new User(MockHelpers.GetRandomTestName(), false);
            var result = await HassWsApi.CreateUserAsync(testUser);
            Assert.IsTrue(result, "SetUp failed");

            _testPerson.ChangeUser(testUser);
            result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson);

            Assert.IsTrue(result);
            Assert.IsFalse(_testPerson.HasPendingChanges);
        }

        [Test, Order(4)]
        public async Task UpdateWithForce()
        {
            var initialName = _testPerson.Name;
            var clonedEntry = _testPerson.Clone();
            clonedEntry.Name = $"{initialName}_cloned";
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(clonedEntry);
            Assert.IsTrue(result, "SetUp failed");
            Assert.False(_testPerson.HasPendingChanges, "SetUp failed");

            result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson, forceUpdate: true);
            Assert.IsTrue(result);
            Assert.AreEqual(initialName, _testPerson.Name);
            Assert.IsFalse(_testPerson.HasPendingChanges);
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

            Assert.IsTrue(result);
            Assert.IsFalse(deletedPerson.IsTracked);
        }
    }
}
