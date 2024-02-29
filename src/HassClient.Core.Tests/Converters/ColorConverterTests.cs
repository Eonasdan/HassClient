using System;
using System.Collections.Generic;
using System.IO;
using HassClient.Core.Models.Color;
using HassClient.Core.Serialization.Converters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HassClient.Core.Tests.Converters
{
    [TestFixture(TestOf = typeof(ColorConverter))]
    public class ColorConverterTests
    {
        private readonly ColorConverter _converter = new();

        [Test]
        [TestCase(typeof(Color))]
        [TestCase(typeof(HSColor))]
        [TestCase(typeof(KelvinTemperatureColor))]
        [TestCase(typeof(MiredsTemperatureColor))]
        [TestCase(typeof(NameColor))]
        [TestCase(typeof(RGBColor))]
        [TestCase(typeof(RGBWColor))]
        [TestCase(typeof(XYColor))]
        public void CanConvertColors(Type colorType)
        {
            var canConvert = _converter.CanConvert(colorType);

            Assert.That(canConvert, Is.True);
        }

        public static IEnumerable<TestCaseData> WriteReadJsonTestCases()
        {
            yield return CreateData(new RGBColor(10, 20, 30));
            yield return CreateData(new RGBWColor(10, 20, 30, 255));
            yield return CreateData(new RGBWWColor(10, 20, 30, 128, 255));
            yield return CreateData(new HSColor(10, 20));
            yield return CreateData(new XYColor(0.2f, 0.6f));
            yield return CreateData(new NameColor("test_color"));
            yield return CreateData(new KelvinTemperatureColor(1337));
            yield return CreateData(new MiredsTemperatureColor(256));
            yield break;

            TestCaseData CreateData(Color color) => new TestCaseData(color).SetName($"{{m}}{color.GetType().Name}");
        }

        [Test]
        [TestCaseSource(nameof(WriteReadJsonTestCases))]
        public void WriteJson(Color color)
        {
            var textWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(textWriter);
            var serializer = JsonSerializer.Create();
            _converter.WriteJson(jsonWriter, color, serializer);

            Assert.That(GetJsonRepresentation(color), Is.EqualTo(textWriter.ToString()));
        }

        [Test]
        [TestCaseSource(nameof(WriteReadJsonTestCases))]
        public void ReadJson(Color color)
        {
            var textReader = new StringReader(GetJsonRepresentation(color));
            var jsonReader = new JsonTextReader(textReader);
            var serializer = JsonSerializer.Create();
            var result = _converter.ReadJson(jsonReader, color.GetType(), null, serializer);

            Assert.That(result, Is.Not.Null);
            Assert.That(color, Is.Not.EqualTo(result));
            Assert.That(color.ToString(), Is.EqualTo(result.ToString()));
        }

        public static IEnumerable<TestCaseData> ReadJsonWithExisingValueTestCases()
        {
            yield return CreateData(new RGBColor(10, 20, 30), new RGBColor(40, 50, 60));
            yield return CreateData(new RGBWColor(10, 20, 30, 255), new RGBWColor(40, 50, 60, 128));
            yield return CreateData(new RGBWWColor(10, 20, 30, 128, 255), new RGBWWColor(40, 50, 60, 64, 128));
            yield return CreateData(new HSColor(10, 20), new HSColor(30, 40));
            yield return CreateData(new XYColor(0.2f, 0.6f), new XYColor(0.4f, 0.8f));
            yield return CreateData(new NameColor("test_color"), new NameColor("new_color"));
            yield return CreateData(new KelvinTemperatureColor(1337), new KelvinTemperatureColor(2001));
            yield return CreateData(new MiredsTemperatureColor(256), new MiredsTemperatureColor(106));
            yield break;

            TestCaseData CreateData(Color existing, Color color) => new TestCaseData(existing, color).SetName($"{{m}}{color.GetType().Name}");
        }

        [Test]
        [TestCaseSource(nameof(ReadJsonWithExisingValueTestCases))]
        public void ReadJsonWithExisingValue(Color existing, Color color)
        {
            var textReader = new StringReader(GetJsonRepresentation(color));
            var jsonReader = new JsonTextReader(textReader);
            var serializer = JsonSerializer.Create();
            var result = _converter.ReadJson(jsonReader, color.GetType(), existing, serializer);

            Assert.That(result, Is.Not.Null);
            Assert.That(existing, Is.EqualTo(result));
            Assert.That(color.ToString(), Is.EqualTo(result.ToString()));
        }

        private string GetJsonRepresentation(Color color)
        {
            return color switch
            {
                NameColor => $"\"{color}\"",
                KelvinTemperatureColor kelvinColor => kelvinColor.Kelvins.ToString(),
                MiredsTemperatureColor miredsColor => miredsColor.Mireds.ToString(),
                _ => color.ToString()?.Replace(" ", string.Empty)
            };
        }
    }
}
