using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.Core.Tests;
using HassClient.WS.Extensions;
using NUnit.Framework;

namespace HassClient.WS.Tests.RegistryEntryApiTests
{
    public class EntityRegistryApiTests : BaseHassWsApiTest
    {
        private InputBoolean? _testInputBoolean;

        private string _testEntityId;

        protected override async Task OneTimeSetUp()
        {
            await base.OneTimeSetUp();
            _testInputBoolean = new InputBoolean(MockHelpers.GetRandomTestName(), "mdi:switch");
            var result = await HassWsApi.CreateStorageEntityRegistryEntryAsync(_testInputBoolean);
            _testEntityId = _testInputBoolean.EntityId;

            Assert.That(result, Is.True, "SetUp failed");
        }

        protected override async Task OneTimeTearDown()
        {
            await base.OneTimeTearDown();
            await HassWsApi.DeleteStorageEntityRegistryEntryAsync(_testInputBoolean);
        }

        [Test]
        public async Task GetEntities()
        {
            var entities = await HassWsApi.GetEntitiesAsync();

            Assert.That(entities, Is.Not.Null);
            Assert.That(entities, Is.Not.Empty);
            Assert.That(entities.All(e => e.EntityId != null), Is.True);
            Assert.That(entities.All(e => e.Platform != null), Is.True, entities.FirstOrDefault(e => e.Platform == null)?.EntityId);
            Assert.That(entities.Any(e => e.ConfigEntryId != null), Is.True);
        }

        [Test]
        public void GetEntityWithNullEntityIdThrows()
        {
            Assert.ThrowsAsync<ArgumentException>(() => HassWsApi.GetEntityAsync(null));
        }

        [Test]
        public void UpdateEntityWithSameEntityIdThrows()
        {
            var testEntity = new EntityRegistryEntry("switch.TestEntity", null, null);

            Assert.ThrowsAsync<ArgumentException>(() => HassWsApi.UpdateEntityAsync(testEntity, testEntity.EntityId));
        }

        [Test]
        public async Task GetEntity()
        {
            var entityId = "light.bed_light";
            var entity = await HassWsApi.GetEntityAsync(entityId);

            Assert.That(entity, Is.Not.Null);;
            Assert.That(entity.ConfigEntryId, Is.Not.Null);;
            Assert.That(entity.OriginalName, Is.Not.Null);
            Assert.That(entity.Name, Is.Not.Null);
            Assert.That(entityId, Is.EqualTo(entity.EntityId));
        }

        [Test, Order(1), NonParallelizable]
        public async Task GetCreatedEntity()
        {
            var entity = await HassWsApi.GetEntityAsync(_testEntityId);

            Assert.That(entity, Is.Not.Null);
            Assert.That(entity.OriginalName, Is.Not.Null);
            Assert.That(entity.OriginalIcon, Is.Not.Null);
            Assert.That(entity.Name, Is.Not.Null);
            Assert.That(entity.Icon, Is.Not.Null);
            Assert.That(_testEntityId, Is.EqualTo(entity.EntityId));
        }

        [Order(1), NonParallelizable]
        [TestCase(true)]
        [TestCase(false)]
        public async Task UpdateEntityDisable(bool disable)
        {
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);

            var result = await HassWsApi.UpdateEntityAsync(testEntity, disable: disable);

            Assert.That(result, Is.True);
            Assert.That(_testEntityId, Is.EqualTo(testEntity.EntityId));
            Assert.That(disable, Is.EqualTo(testEntity.IsDisabled));
        }

        [Test, Order(1), NonParallelizable]
        public async Task UpdateEntityName()
        {
            var newName = MockHelpers.GetRandomTestName();
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);

            testEntity.Name = newName;
            var result = await HassWsApi.UpdateEntityAsync(testEntity);

            Assert.That(result, Is.True);
            Assert.That(_testEntityId, Is.EqualTo(testEntity.EntityId));
            Assert.That(newName, Is.EqualTo(testEntity.Name));
            Assert.That(newName, Is.Not.EqualTo(testEntity.Name));
            Assert.That(newName, Is.Not.EqualTo(testEntity.OriginalName));
        }

        [Test, Order(1), NonParallelizable]
        public async Task UpdateEntityIcon()
        {
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);

            var newIcon = "mdi:fan";
            testEntity.Icon = newIcon;
            var result = await HassWsApi.UpdateEntityAsync(testEntity);

            Assert.That(result, Is.True);
            Assert.That(_testEntityId, Is.EqualTo(testEntity.EntityId));
            Assert.That(newIcon, Is.EqualTo(testEntity.Icon));
            Assert.That(newIcon, Is.Not.EqualTo(testEntity.OriginalIcon));
        }

        [Test, Order(1)]
        public async Task RefreshEntity()
        {
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);
            var clonedEntity = testEntity.Clone();
            clonedEntity.Name = MockHelpers.GetRandomTestName();
            var result = await HassWsApi.UpdateEntityAsync(clonedEntity);
            Assert.That(result, Is.True, "SetUp failed");
            Assert.That(testEntity.HasPendingChanges, Is.False, "SetUp failed");

            result = await HassWsApi.RefreshEntityAsync(testEntity);
            Assert.That(result, Is.True);
            Assert.That(clonedEntity.Name, Is.EqualTo(testEntity.Name));
        }

        [Test, Order(2), NonParallelizable]
        public async Task UpdateEntityId()
        {
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);
            var newEntityId = _testEntityId + 1;

            var result = await HassWsApi.UpdateEntityAsync(testEntity, newEntityId);

            Assert.That(result, Is.True);
            Assert.That(newEntityId, Is.EqualTo(testEntity.EntityId));
            Assert.That(_testEntityId, Is.Not.EqualTo(newEntityId));

            _testEntityId = newEntityId; // This is needed for DeleteEntityTest
        }

        [Test, Order(3)]
        public async Task UpdateWithForce()
        {
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);
            var initialName = testEntity.Name;
            var initialIcon = testEntity.Icon;
            var initialDisabledBy = testEntity.DisabledBy;
            var clonedEntry = testEntity.Clone();
            clonedEntry.Name = $"{initialName}_cloned";
            clonedEntry.Icon = $"{initialIcon}_cloned";
            var result = await HassWsApi.UpdateEntityAsync(clonedEntry, disable: true);
            Assert.That(result, Is.True, "SetUp failed");
            Assert.That(testEntity.HasPendingChanges, Is.False, "SetUp failed");

            result = await HassWsApi.UpdateEntityAsync(testEntity, disable: false, forceUpdate: true);
            Assert.That(result, Is.True);
            Assert.That(initialName, Is.EqualTo(testEntity.Name));
            Assert.That(initialIcon, Is.EqualTo(testEntity.Icon));
            Assert.That(initialDisabledBy, Is.EqualTo(testEntity.DisabledBy));
        }

        [Test, Order(4), NonParallelizable]
        public async Task DeleteEntity()
        {
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);
            var result = await HassWsApi.DeleteEntityAsync(testEntity);
            var testEntity1 = await HassWsApi.GetEntityAsync(_testEntityId);

            Assert.That(result, Is.True);
            Assert.That(testEntity1, Is.Null);
            Assert.That(testEntity.IsTracked, Is.False);
        }
    }
}
