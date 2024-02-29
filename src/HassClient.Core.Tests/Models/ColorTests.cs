using HassClient.Core.Models.Color;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models
{
    [TestFixture(TestOf = typeof(Color))]
    public class ColorTests
    {
        [Test]
        public void FromRGB()
        {
            byte red = 10;
            byte green = 20;
            byte blue = 30;

            var color = Color.FromRGB(red, green, blue);

            Assert.That(red, Is.EqualTo(color.R));
            Assert.That(green, Is.EqualTo(color.G));
            Assert.That(blue , Is.EqualTo(color.B));
        }

        [Test]
        public void FromRgbw()
        {
            byte red = 10;
            byte green = 20;
            byte blue = 30;
            byte white = 255;

            var color = Color.FromRgbw(red, green, blue, white);

            Assert.That(red, Is.EqualTo(color.R));
            Assert.That(green, Is.EqualTo(color.G));
            Assert.That(blue, Is.EqualTo(color.B));
            Assert.That(white, Is.EqualTo(color.W));
        }

        [Test]
        public void FromRgbww()
        {
            byte red = 10;
            byte green = 20;
            byte blue = 30;
            byte coldWhite = 128;
            byte warmWhite = 255;

            var color = Color.FromRGBWW(red, green, blue, coldWhite, warmWhite);

            Assert.That(red, Is.EqualTo(color.R));
            Assert.That(green, Is.EqualTo(color.G));
            Assert.That(blue, Is.EqualTo(color.B));
            Assert.That(coldWhite, Is.EqualTo(color.Cw));
            Assert.That(warmWhite, Is.EqualTo(color.Ww));
        }

        [Test]
        public void FromHs()
        {
            uint hue = 10;
            uint saturation = 20;

            var color = Color.FromHS(hue, saturation);

            Assert.That(hue, Is.EqualTo(color.Hue));
            Assert.That(saturation, Is.EqualTo(color.Saturation));
        }

        [Test]
        public void FromXy()
        {
            var x = 0.2f;
            var y = 0.6f;

            var color = Color.FromXY(x, y);

            Assert.That(x, Is.EqualTo(color.X));
            Assert.That(y, Is.EqualTo(color.Y));
        }

        [Test]
        public void FromKelvinTemperature()
        {
            uint kelvins = 1337;

            var color = Color.FromKelvinTemperature(kelvins);

            Assert.That(kelvins, Is.EqualTo(color.Kelvins));
        }

        [Test]
        public void FromMireds()
        {
            uint mireds = 256;

            var color = Color.FromMireds(mireds);

            Assert.That(mireds, Is.EqualTo(color.Mireds));
        }
    }
}
