using System;
using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models
{
    [TestFixture(TestOf = typeof(Person))]
    public class PersonTests
    {
        private readonly User _testUser = User.CreateUnmodified("test", MockHelpers.GetRandomTestName(), false);

        [Test]
        public void HasPublicConstructorWithParameters()
        {
            var constructor = typeof(Person).GetConstructors()
                                            .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
            Assert.NotNull(constructor);
        }

        [Test]
        public void NewPersonHasPendingChanges()
        {
            var testEntry = new Person(MockHelpers.GetRandomTestName(), _testUser);
            Assert.IsTrue(testEntry.HasPendingChanges);
        }

        [Test]
        public void NewPersonIsStorageEntry()
        {
            var testEntry = new Person(MockHelpers.GetRandomTestName(), _testUser);
            Assert.IsTrue(testEntry.IsStorageEntry);
        }

        [Test]
        public void NewPersonIsUntracked()
        {
            var testEntry = new Person(MockHelpers.GetRandomTestName(), _testUser);
            Assert.False(testEntry.IsTracked);
        }

        [Test]
        public void NewPersonHasUserId()
        {
            var testEntry = new Person(MockHelpers.GetRandomTestName(), _testUser);
            Assert.AreEqual(testEntry.UserId, _testUser.Id);
        }

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewPersonWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new Person(value, _testUser));
        }

        [Test]
        public void SetNewIconThrows()
        {
            var testEntry = CreateTestEntry(out _, out _, out var _, out _, out _);

            Assert.Throws<InvalidOperationException>(() => testEntry.Icon = MockHelpers.GetRandomTestName());
        }

        [Test]
        public void SetNewInvalidDeviceTrackerThrows()
        {
            var testEntry = CreateTestEntry(out _, out _, out var _, out _, out _);

            Assert.Throws<InvalidOperationException>(
                () => testEntry.DeviceTrackers.Add(MockHelpers.GetRandomEntityId(KnownDomains.Camera)));
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out var initialName, out _, out _, out _);

            testEntry.Name = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Name = initialName;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewPictureMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out _, out _, out var picture, out _);

            testEntry.Picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Picture = picture;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewUserMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out _, out var user, out _, out _);

            testEntry.ChangeUser(_testUser);
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.ChangeUser(user);
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewDeviceTrackerMakesHasPendingChangesTrue()
        {
            var testDeviceTracker = MockHelpers.GetRandomEntityId(KnownDomains.DeviceTracker);
            var testEntry = CreateTestEntry(out _, out _, out _, out _, out var deviceTrackers);

            testEntry.DeviceTrackers.Add(testDeviceTracker);
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.DeviceTrackers.Remove(testDeviceTracker);
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = CreateTestEntry(out _, out var initialName, out var initialUser, out var initialPicture, out var initialDeviceTrackers);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.ChangeUser(_testUser);
            testEntry.Picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
            testEntry.DeviceTrackers.Add(MockHelpers.GetRandomEntityId(KnownDomains.DeviceTracker));
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.DiscardPendingChanges();
            Assert.False(testEntry.HasPendingChanges);
            Assert.AreEqual(initialName, testEntry.Name);
            Assert.AreEqual(initialUser.Id, testEntry.UserId);
            Assert.AreEqual(initialPicture, testEntry.Picture);
            Assert.AreEqual(initialDeviceTrackers, testEntry.DeviceTrackers);
        }

        private Person CreateTestEntry(out string entityId, out string name, out User user, out string picture, out IEnumerable<string> deviceTrackers)
        {
            entityId = MockHelpers.GetRandomEntityId(KnownDomains.Person);
            name = MockHelpers.GetRandomTestName();
            user = User.CreateUnmodified(MockHelpers.GetRandomTestName(), name, false);
            picture = "/test/Picture.png";
            deviceTrackers = new[] { MockHelpers.GetRandomEntityId(KnownDomains.DeviceTracker), MockHelpers.GetRandomEntityId(KnownDomains.DeviceTracker) };
            return Person.CreateUnmodified(entityId, name, user.Id, picture, deviceTrackers);
        }
    }
}
