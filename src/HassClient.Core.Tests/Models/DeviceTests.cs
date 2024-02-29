using System.Linq;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models
{
    [TestFixture(TestOf = typeof(Device))]
    public class DeviceTests
    {
        [Test]
        public void HasNoPublicConstructors()
        {
            var constructor = typeof(Device).GetConstructors()
                                            .FirstOrDefault(x => x.IsPublic);
            Assert.That(constructor, Is.Null);
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out var initialName, out _, out _);

            testEntry.Name = MockHelpers.GetRandomTestName();
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.Name = initialName;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        [Test]
        public void SetNewAreaIdMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out _, out var initialAreaId, out _);

            testEntry.AreaId = MockHelpers.GetRandomTestName();
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.AreaId = initialAreaId;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = CreateTestEntry(out _, out var initialName, out var initialAreaId, out var initialDisabledBy);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.AreaId = MockHelpers.GetRandomTestName();
            //testEntry.DisabledBy = DisabledByEnum.User;
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.DiscardPendingChanges();
            Assert.That(testEntry.HasPendingChanges, Is.False);
            Assert.That(initialName, Is.EqualTo(testEntry.Name));
            Assert.That(initialAreaId, Is.EqualTo(testEntry.AreaId));
            Assert.That(initialDisabledBy, Is.EqualTo(testEntry.DisabledBy));
        }

        [Test]
        public void NameIsNameByUserIfDefined()
        {
            var testEntry = CreateTestEntry(out _, out _, out _, out _);

            Assert.That(testEntry.OriginalName, Is.EqualTo(testEntry.Name));

            var testName = MockHelpers.GetRandomTestName();
            testEntry.Name = testName;
            Assert.That(testName, Is.EqualTo(testEntry.Name));

            testEntry.Name = null;
            Assert.That(testEntry.OriginalName, Is.EqualTo(testEntry.Name));
        }

        private Device CreateTestEntry(out string entityId, out string name, out string areaId, out DisabledByEnum disabledBy)
        {
            entityId = MockHelpers.GetRandomEntityId(KnownDomains.Esphome);
            name = MockHelpers.GetRandomTestName();
            areaId = MockHelpers.GetRandomTestName();
            disabledBy = DisabledByEnum.Integration;
            return Device.CreateUnmodified(entityId, name, areaId, disabledBy);
        }
    }
}
