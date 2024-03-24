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
    public class InputBooleanApiTests : BaseHassWsApiTest
    {
        private InputBoolean? _testInputBoolean;

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateInputBoolean()
        {
            if (_testInputBoolean == null)
            {
                _testInputBoolean = new InputBoolean(MockHelpers.GetRandomTestName(), "mdi:fan", true);
                var result = await HassWsApi.CreateStorageEntityRegistryEntryAsync(_testInputBoolean);

                Assert.That(result, Is.True, "SetUp failed");
                return;
            }

            Assert.That(_testInputBoolean, Is.Not.Null);
            Assert.That(_testInputBoolean.UniqueId, Is.Not.Null);
            Assert.That(_testInputBoolean.Name, Is.Not.Null);
            Assert.That(_testInputBoolean.HasPendingChanges, Is.False);
            Assert.That(_testInputBoolean.IsTracked, Is.True);
        }

        [Test, Order(2)]
        public async Task GetInputBooleans()
        {
            IEnumerable<InputBoolean?> result = await HassWsApi.GetStorageEntityRegistryEntriesAsync<InputBoolean>();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Contains(_testInputBoolean), Is.True);
            Assert.That(result.All(x => x.Id != null), Is.True);
            Assert.That(result.All(x => x.UniqueId != null), Is.True);
            Assert.That(result.All(x => x.EntityId.StartsWith("input_boolean.")), Is.True);
            Assert.That(result.Any(x => x.Name != null), Is.True);
            Assert.That(result.Any(x => x.Initial), Is.True);
            Assert.That(result.Any(x => x.Icon != null), Is.True);
            Assert.That(_testInputBoolean.HasPendingChanges, Is.False);
        }

        [Test, Order(3)]
        public async Task UpdateInputBooleanName()
        {
            _testInputBoolean.Name = $"{nameof(InputBooleanApiTests)}_{DateTime.Now.Ticks}";
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testInputBoolean);

            Assert.That(result, Is.True);
            Assert.That(_testInputBoolean.HasPendingChanges, Is.False);
        }

        [Test, Order(4)]
        public async Task UpdateInputBooleanInitial()
        {
            _testInputBoolean.Initial = false;
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testInputBoolean);

            Assert.That(result, Is.True);
            Assert.That(_testInputBoolean.HasPendingChanges, Is.False);
        }

        [Test, Order(5)]
        public async Task UpdateInputBooleanIcon()
        {
            _testInputBoolean.Icon = "mdi:lightbulb";
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testInputBoolean);

            Assert.That(result, Is.True);
            Assert.That(_testInputBoolean.HasPendingChanges, Is.False);
        }

        [Test, Order(6)]
        public async Task UpdateWithForce()
        {
            var initialName = _testInputBoolean.Name;
            var initialIcon = _testInputBoolean.Icon;
            var initialInitial = _testInputBoolean.Initial;
            var clonedEntry = _testInputBoolean.Clone();
            clonedEntry.Name = $"{initialName}_cloned";
            clonedEntry.Icon = $"{initialIcon}_cloned";
            clonedEntry.Initial = !initialInitial;
            var result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(clonedEntry);
            Assert.That(result, Is.True, "SetUp failed");
            Assert.That(_testInputBoolean.HasPendingChanges, Is.False, "SetUp failed");

            result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testInputBoolean, forceUpdate: true);
            Assert.That(result, Is.True);
            Assert.That(initialName, Is.EqualTo(_testInputBoolean.Name));
            Assert.That(initialIcon, Is.EqualTo(_testInputBoolean.Icon));
            Assert.That(initialInitial, Is.EqualTo(_testInputBoolean.Initial));
            Assert.That(_testInputBoolean.HasPendingChanges, Is.False);
        }

        [OneTimeTearDown]
        [Test, Order(7)]
        public async Task DeleteInputBoolean()
        {
            if (_testInputBoolean == null)
            {
                return;
            }

            var result = await HassWsApi.DeleteStorageEntityRegistryEntryAsync(_testInputBoolean);
            var deletedInputBoolean = _testInputBoolean;
            _testInputBoolean = null;

            Assert.That(result, Is.True);
            Assert.That(deletedInputBoolean.IsTracked, Is.False);
        }
    }
}
