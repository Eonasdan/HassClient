﻿using System;
using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models
{
    [TestFixture(TestOf = typeof(Zone))]
    public class ZoneTests
    {
        [Test]
        public void HasPublicConstructorWithParameters()
        {
            var constructor = typeof(Zone).GetConstructors()
                                          .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
            Assert.That(constructor, Is.Not.Null);
        }

        [Test]
        public void NewZoneHasPendingChanges()
        {
            var testEntry = new Zone(MockHelpers.GetRandomTestName(), 20, 30, 5);
            Assert.That(testEntry.HasPendingChanges, Is.True);
        }

        [Test]
        public void NewZoneIsUntracked()
        {
            var testEntry = new Zone(MockHelpers.GetRandomTestName(), 20, 30, 5);
            Assert.That(testEntry.IsTracked, Is.False);
        }

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewZoneWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new Zone(value, 20, 30, 5));
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out var initialName, out _, out _, out _, out _, out _);

            testEntry.Name = MockHelpers.GetRandomTestName();
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.Name = initialName;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        [Test]
        public void SetNewIconMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out var initialIcon, out _, out _, out _, out _);

            testEntry.Icon = "mdi:test";
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.Icon = initialIcon;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        [Test]
        public void SetNewLongitudeMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out _, out var initialLongitude, out _, out _, out _);

            testEntry.Longitude += 10;
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.Longitude = initialLongitude;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        [Test]
        public void SetNewLatitudeMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out _, out _, out var initialLatitude, out _, out _);

            testEntry.Latitude += 10;
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.Latitude = initialLatitude;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        [Test]
        public void SetNewRadiusMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out _, out _, out _, out var initialRadius, out _);

            testEntry.Radius += 10;
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.Radius = initialRadius;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        [Test]
        public void SetNewIsPassiveMakesHasPendingChangesTrue()
        {
            var testEntry = CreateTestEntry(out _, out _, out _, out _, out _, out var initialIsPassive);

            testEntry.IsPassive = !initialIsPassive;
            Assert.That(testEntry.HasPendingChanges, Is.True);

            testEntry.IsPassive = initialIsPassive;
            Assert.That(testEntry.HasPendingChanges, Is.False);
        }

        private Zone? CreateTestEntry(out string name, out string icon, out float longitude, out float latitude, out float radius, out bool isPassive)
        {
            name = MockHelpers.GetRandomTestName();
            icon = "mdi:zone";
            longitude = 20;
            latitude = 30;
            radius = 5;
            isPassive = false;
            return Zone.CreateUnmodified("testId", name, longitude, latitude, radius, icon, isPassive);
        }
    }
}
