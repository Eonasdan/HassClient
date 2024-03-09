using System;
using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Models.RegistryEntries;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models
{
    [TestFixture(TestOf = typeof(Area))]
    public class AreaTests
    {
        [Test]
        public void HasPublicConstructorWithParameters()
        {
            var constructor = typeof(Area).GetConstructors()
                                          .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
            Assert.That(constructor, Is.Not.Null);
        }

        [Test]
        public void NewAreaHasPendingChanges()
        {
            var testEntry = new Area(MockHelpers.GetRandomTestName());
            Assert.That(testEntry.HasPendingChanges, Is.True);
        }

        [Test]
        public void NewAreaIsUntracked()
        {
            var testEntry = new Area(MockHelpers.GetRandomTestName());
            Assert.That(testEntry.IsTracked, Is.False);
        }

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewAreaWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new Area(value));
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out var initialName, out _);

            testEntry.Name = MockHelpers.GetRandomTestName();
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.Name = initialName;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        [Test]
        public void SetNewPictureMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out var picture);

            testEntry.Picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.Picture = picture;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = CreateTestEntry(out var initialName, out var initialPicture);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.Picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.DiscardPendingChanges();
            Assert.That(testEntry.HasPendingChanges, Is.False);
            Assert.That(initialName, Is.EqualTo(testEntry.Name));
            Assert.That(initialPicture, Is.EqualTo(testEntry.Picture));
        }

        private Area? CreateTestEntry(out string name, out string picture)
        {
            name = MockHelpers.GetRandomTestName();
            picture = "/test/Picture.png";
            return Area.CreateUnmodified(name, picture);
        }
    }
}
