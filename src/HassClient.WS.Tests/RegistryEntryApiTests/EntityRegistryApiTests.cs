using System;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.Core.Tests;
using NUnit.Framework;

namespace HassClient.WS.Tests.RegistryEntryApiTests
{
    public class EntityRegistryApiTests : BaseHassWsApiTest
    {
        private InputBoolean _testInputBoolean;

        private string _testEntityId;

        protected override async Task OneTimeSetUp()
        {
            await base.OneTimeSetUp();
            _testInputBoolean = new InputBoolean(MockHelpers.GetRandomTestName(), "mdi:switch");
            var result = await HassWsApi.CreateStorageEntityRegistryEntryAsync(_testInputBoolean);
            _testEntityId = _testInputBoolean.EntityId;

            Assert.IsTrue(result, "SetUp failed");
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

            Assert.IsNotNull(entities);
            Assert.IsNotEmpty(entities);
            Assert.IsTrue(entities.All(e => e.EntityId != null));
            Assert.IsTrue(entities.All(e => e.Platform != null), entities.FirstOrDefault(e => e.Platform == null)?.EntityId);
            Assert.IsTrue(entities.Any(e => e.ConfigEntryId != null));
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

            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.ConfigEntryId);
            Assert.IsNotNull(entity.OriginalName);
            Assert.IsNotNull(entity.Name);
            Assert.AreEqual(entityId, entity.EntityId);
        }

        [Test, Order(1), NonParallelizable]
        public async Task GetCreatedEntity()
        {
            var entity = await HassWsApi.GetEntityAsync(_testEntityId);

            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.OriginalName);
            Assert.IsNotNull(entity.OriginalIcon);
            Assert.IsNotNull(entity.Name);
            Assert.IsNotNull(entity.Icon);
            Assert.AreEqual(_testEntityId, entity.EntityId);
        }

        [Order(1), NonParallelizable]
        [TestCase(true)]
        [TestCase(false)]
        public async Task UpdateEntityDisable(bool disable)
        {
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);

            var result = await HassWsApi.UpdateEntityAsync(testEntity, disable: disable);

            Assert.IsTrue(result);
            Assert.AreEqual(_testEntityId, testEntity.EntityId);
            Assert.AreEqual(disable, testEntity.IsDisabled);
        }

        [Test, Order(1), NonParallelizable]
        public async Task UpdateEntityName()
        {
            var newName = MockHelpers.GetRandomTestName();
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);

            testEntity.Name = newName;
            var result = await HassWsApi.UpdateEntityAsync(testEntity);

            Assert.IsTrue(result);
            Assert.AreEqual(_testEntityId, testEntity.EntityId);
            Assert.AreEqual(newName, testEntity.Name);
            Assert.AreNotEqual(newName, testEntity.OriginalName);
        }

        [Test, Order(1), NonParallelizable]
        public async Task UpdateEntityIcon()
        {
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);

            var newIcon = "mdi:fan";
            testEntity.Icon = newIcon;
            var result = await HassWsApi.UpdateEntityAsync(testEntity);

            Assert.IsTrue(result);
            Assert.AreEqual(_testEntityId, testEntity.EntityId);
            Assert.AreEqual(newIcon, testEntity.Icon);
            Assert.AreNotEqual(newIcon, testEntity.OriginalIcon);
        }

        [Test, Order(1)]
        public async Task RefreshEntity()
        {
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);
            var clonedEntity = testEntity.Clone();
            clonedEntity.Name = MockHelpers.GetRandomTestName();
            var result = await HassWsApi.UpdateEntityAsync(clonedEntity);
            Assert.IsTrue(result, "SetUp failed");
            Assert.False(testEntity.HasPendingChanges, "SetUp failed");

            result = await HassWsApi.RefreshEntityAsync(testEntity);
            Assert.IsTrue(result);
            Assert.AreEqual(clonedEntity.Name, testEntity.Name);
        }

        [Test, Order(2), NonParallelizable]
        public async Task UpdateEntityId()
        {
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);
            var newEntityId = _testEntityId + 1;

            var result = await HassWsApi.UpdateEntityAsync(testEntity, newEntityId);

            Assert.IsTrue(result);
            Assert.AreEqual(newEntityId, testEntity.EntityId);
            Assert.AreNotEqual(_testEntityId, newEntityId);

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
            Assert.IsTrue(result, "SetUp failed");
            Assert.False(testEntity.HasPendingChanges, "SetUp failed");

            result = await HassWsApi.UpdateEntityAsync(testEntity, disable: false, forceUpdate: true);
            Assert.IsTrue(result);
            Assert.AreEqual(initialName, testEntity.Name);
            Assert.AreEqual(initialIcon, testEntity.Icon);
            Assert.AreEqual(initialDisabledBy, testEntity.DisabledBy);
        }

        [Test, Order(4), NonParallelizable]
        public async Task DeleteEntity()
        {
            var testEntity = await HassWsApi.GetEntityAsync(_testEntityId);
            var result = await HassWsApi.DeleteEntityAsync(testEntity);
            var testEntity1 = await HassWsApi.GetEntityAsync(_testEntityId);

            Assert.IsTrue(result);
            Assert.IsNull(testEntity1);
            Assert.IsFalse(testEntity.IsTracked);
        }
    }
}
