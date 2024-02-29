using System;
using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Models.RegistryEntries;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models
{
    [TestFixture(TestOf = typeof(User))]
    public class UserTests
    {
        [Test]
        public void HasPublicConstructorWithParameters()
        {
            var constructor = typeof(User).GetConstructors()
                                          .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
            Assert.That(constructor, Is.Not.Null);
        }

        [Test]
        public void NewUserHasPendingChanges()
        {
            var testEntry = new User(MockHelpers.GetRandomTestName());
            Assert.That(testEntry.HasPendingChanges, Is.True);
        }

        [Test]
        public void NewUserIsUntracked()
        {
            var testEntry = new User(MockHelpers.GetRandomTestName());
            Assert.That(testEntry.IsTracked, Is.False);
        }

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewUserWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new User(value));
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out var initialName);

            testEntry.Name = MockHelpers.GetRandomTestName();
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.Name = initialName;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        [Test]
        public void SetNewIsAdministratorMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _);

            testEntry.IsAdministrator = true;
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.IsAdministrator = false;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        [Test]
        public void AddNewGroupIdMakesHasPendingChangesTrue()
        {
            var testGroupId = "TestGroupId";
            var testEntry = CreateTestEntry(out _);

            testEntry.GroupIds.Add(testGroupId);
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.GroupIds.Remove(testGroupId);
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        private User CreateTestEntry(out string name)
        {
            name = MockHelpers.GetRandomTestName();
            return User.CreateUnmodified("testId", name, false);
        }
    }
}
