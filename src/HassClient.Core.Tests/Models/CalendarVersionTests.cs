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

            Assert.AreEqual(2022, version.Year);
            Assert.AreEqual(2, version.Month);
            Assert.AreEqual(0, version.Micro);
            Assert.AreEqual(string.Empty, version.Modifier);
        }

        [Test]
        public void CreateWithYearAndMonthAndMicro()
        {
            var version = CalendarVersion.Create("2022.02.13");

            Assert.AreEqual(2022, version.Year);
            Assert.AreEqual(2, version.Month);
            Assert.AreEqual(13, version.Micro);
            Assert.AreEqual(string.Empty, version.Modifier);
        }

        [Test]
        public void CreateWithYearAndMonthAndModifier()
        {
            var version = CalendarVersion.Create("2022.02.b3");

            Assert.AreEqual(2022, version.Year);
            Assert.AreEqual(2, version.Month);
            Assert.AreEqual(0, version.Micro);
            Assert.AreEqual("b3", version.Modifier);
        }

        [Test]
        public void CreateWithYearAndMonthMicroAndModifier()
        {
            var version = CalendarVersion.Create("2022.02.4b3");

            Assert.AreEqual(2022, version.Year);
            Assert.AreEqual(2, version.Month);
            Assert.AreEqual(4, version.Micro);
            Assert.AreEqual("b3", version.Modifier);
        }

        [Test]
        public void CreateDateIsCorrect()
        {
            var version = CalendarVersion.Create("2022.02.4b3");

            Assert.AreEqual(2022, version.ReleaseDate.Year);
            Assert.AreEqual(2, version.ReleaseDate.Month);
        }

        [Test]
        public void ToStringIsCorrect()
        {
            var expectedVersionString = "2022.2.4b3";
            var version = CalendarVersion.Create(expectedVersionString);

            Assert.AreEqual(expectedVersionString, version.ToString());
        }
    }
}
