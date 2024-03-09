using System;
using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models
{
    [TestFixture(TestOf = typeof(InputBoolean))]
    public class InputBooleanTests
    {
        [Test]
        public void HasPublicConstructorWithParameters()
        {
            var constructor = typeof(InputBoolean).GetConstructors()
                                                  .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
            Assert.That(constructor, Is.Not.Null);
        }

        [Test]
        public void NewInputBooleanHasPendingChanges()
        {
            var testEntry = new InputBoolean(MockHelpers.GetRandomTestName());
            Assert.That(testEntry.HasPendingChanges, Is.True);
        }

        [Test]
        public void NewInputBooleanIsUntracked()
        {
            var testEntry = new InputBoolean(MockHelpers.GetRandomTestName());
            Assert.That(testEntry.IsTracked, Is.False);
        }

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewInputBooleanWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new InputBoolean(value));
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

        [Test]
        public void SetNewInitialMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out _, out _, out var initial);

            testEntry.Initial = !initial;
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.Initial = initial;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = CreateTestEntry(out _, out var initialName, out var initialIcon, out var initial);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.Icon = MockHelpers.GetRandomTestName();
            testEntry.Initial = !initial;
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.DiscardPendingChanges();
            Assert.That(testEntry.HasPendingChanges, Is.False);
            Assert.That(initialName, Is.EqualTo(testEntry.Name));
            Assert.That(initialIcon, Is.EqualTo(testEntry.Icon));
            Assert.That(initial, Is.EqualTo(testEntry.Initial));
        }

        private InputBoolean? CreateTestEntry(out string entityId, out string name, out string icon, out bool initial)
        {
            entityId = MockHelpers.GetRandomEntityId(KnownDomains.InputBoolean);
            name = MockHelpers.GetRandomTestName();
            icon = "mdi:fan";
            initial = true;
            return InputBoolean.CreateUnmodified(entityId, name, icon, initial);
        }
    }
}
