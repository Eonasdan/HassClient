using System.Linq;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models
{
    [TestFixture(TestOf = typeof(EntityRegistryEntry))]
    public class EntityRegistryEntryTests
    {
        [Test]
        public void HasNoPublicConstructors()
        {
            var constructor = typeof(EntityRegistryEntry)
                                    .GetConstructors()
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
        public void SetNewIconMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out _, out var initialIcon, out _);

            testEntry.Icon = MockHelpers.GetRandomTestName();
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.Icon = initialIcon;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        /*[Test]
        public void SetDisableMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out _, out var initialDisabledBy);

            testEntry.DisabledBy = DisabledByEnum.User;
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.DisabledBy = initialDisabledBy;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }*/

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = CreateTestEntry(out _, out var initialName, out var initialIcon, out var initialDisabledBy);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.Icon = MockHelpers.GetRandomTestName();
            //testEntry.DisabledBy = DisabledByEnum.User;
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.DiscardPendingChanges();
            Assert.That(testEntry.HasPendingChanges, Is.False);
            Assert.That(initialName, Is.EqualTo(testEntry.Name));
            Assert.That(initialIcon, Is.EqualTo(testEntry.Icon));
            Assert.That(initialDisabledBy, Is.EqualTo(testEntry.DisabledBy));
        }

        private EntityRegistryEntry? CreateTestEntry(out string entityId, out string name, out string icon, out DisabledByEnum disabledBy)
        {
            entityId = MockHelpers.GetRandomEntityId(KnownDomains.InputBoolean);
            name = MockHelpers.GetRandomTestName();
            icon = "mdi:camera";
            disabledBy = DisabledByEnum.Integration;
            return EntityRegistryEntry.CreateUnmodified(entityId, name, icon, disabledBy);
        }
    }
}
