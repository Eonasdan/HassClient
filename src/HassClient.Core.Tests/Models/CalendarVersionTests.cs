using System;
using HassClient.Core.Models;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models
{
    [TestFixture(TestOf = typeof(CalendarVersion))]
    public class CalendarVersionTests
    {
        [Test]
        public void CreateWithNullThrows()
        {
            var version = new CalendarVersion();
            Assert.Throws<ArgumentNullException>(() => CalendarVersion.Create(null));
        }

        [Test]
        public void CreateWithInvalidYearThrows()
        {
            var version = new CalendarVersion();
            Assert.Throws<ArgumentException>(() => CalendarVersion.Create("invalid"));
        }

        [Test]
        public void CreateWithInvalidMonthThrows()
        {
            var version = new CalendarVersion();
            Assert.Throws<ArgumentException>(() => CalendarVersion.Create("2022.invalid"));
        }

        [Test]
        public void CreateWithInvalidMicroAndModifierThrows()
        {
            var version = new CalendarVersion();
            Assert.Throws<ArgumentException>(() => CalendarVersion.Create("2022.02.''"));
        }

        [Test]
        public void CreateWithYearAndMonth()
        {
            var version = CalendarVersion.Create("2022.02");

            Assert.That(2022, Is.EqualTo(version.Year));
            Assert.That(2, Is.EqualTo(version.Month));
            Assert.That(0, Is.EqualTo(version.Micro));
            Assert.That(string.Empty, Is.EqualTo(version.Modifier));
        }

        [Test]
        public void CreateWithYearAndMonthAndMicro()
        {
            var version = CalendarVersion.Create("2022.02.13");

            Assert.That(2022, Is.EqualTo(version.Year));
            Assert.That(2, Is.EqualTo(version.Month));
            Assert.That(13, Is.EqualTo(version.Micro));
            Assert.That(string.Empty, Is.EqualTo(version.Modifier));
        }

        [Test]
        public void CreateWithYearAndMonthAndModifier()
        {
            var version = CalendarVersion.Create("2022.02.b3");

            Assert.That(2022, Is.EqualTo(version.Year));
            Assert.That(2, Is.EqualTo(version.Month));
            Assert.That(0, Is.EqualTo(version.Micro));
            Assert.That("b3", Is.EqualTo(version.Modifier));
        }

        [Test]
        public void CreateWithYearAndMonthMicroAndModifier()
        {
            var version = CalendarVersion.Create("2022.02.4b3");

            Assert.That(2022, Is.EqualTo(version.Year));
            Assert.That(2, Is.EqualTo(version.Month));
            Assert.That(4, Is.EqualTo(version.Micro));
            Assert.That("b3", Is.EqualTo(version.Modifier));
        }

        [Test]
        public void CreateDateIsCorrect()
        {
            var version = CalendarVersion.Create("2022.02.4b3");

            Assert.That(2022, Is.EqualTo(version.ReleaseDate.Year));
            Assert.That(2, Is.EqualTo(version.ReleaseDate.Month));
        }

        [Test]
        public void ToStringIsCorrect()
        {
            var expectedVersionString = "2022.2.4b3";
            var version = CalendarVersion.Create(expectedVersionString);

            Assert.That(expectedVersionString, Is.EqualTo(version.ToString()));
        }
    }
}
