﻿using HassClient.Core.Models.Color;
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

            Assert.AreEqual(red, color.R);
            Assert.AreEqual(green, color.G);
            Assert.AreEqual(blue , color.B);
        }

        [Test]
        public void FromRgbw()
        {
            byte red = 10;
            byte green = 20;
            byte blue = 30;
            byte white = 255;

            var color = Color.FromRgbw(red, green, blue, white);

            Assert.AreEqual(red, color.R);
            Assert.AreEqual(green, color.G);
            Assert.AreEqual(blue, color.B);
            Assert.AreEqual(white, color.W);
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

            Assert.AreEqual(red, color.R);
            Assert.AreEqual(green, color.G);
            Assert.AreEqual(blue, color.B);
            Assert.AreEqual(coldWhite, color.Cw);
            Assert.AreEqual(warmWhite, color.Ww);
        }

        [Test]
        public void FromHs()
        {
            uint hue = 10;
            uint saturation = 20;

            var color = Color.FromHS(hue, saturation);

            Assert.AreEqual(hue, color.Hue);
            Assert.AreEqual(saturation, color.Saturation);
        }

        [Test]
        public void FromXy()
        {
            var x = 0.2f;
            var y = 0.6f;

            var color = Color.FromXY(x, y);

            Assert.AreEqual(x, color.X);
            Assert.AreEqual(y, color.Y);
        }

        [Test]
        public void FromKelvinTemperature()
        {
            uint kelvins = 1337;

            var color = Color.FromKelvinTemperature(kelvins);

            Assert.AreEqual(kelvins, color.Kelvins);
        }

        [Test]
        public void FromMireds()
        {
            uint mireds = 256;

            var color = Color.FromMireds(mireds);

            Assert.AreEqual(mireds, color.Mireds);
        }
    }
}
