using System.IO;
using HassClient.Core.Models;
using HassClient.Core.Serialization.Converters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HassClient.Core.Tests.Converters
{
    [TestFixture(TestOf = typeof(CalendarVersionConverter))]
    public class CalendarVersionConverterTests
    {
        private readonly CalendarVersionConverter _converter = new();

        private readonly CalendarVersion _testVersion = CalendarVersion.Create("2022.02.4b3");

        [Test]
        public void CanConvertCalVer()
        {
            var canConvert = _converter.CanConvert(typeof(CalendarVersion));

            Assert.True(canConvert);
        }

        [Test]
        public void WriteJson()
        {
            var textWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(textWriter);
            var serializer = JsonSerializer.Create();

            _converter.WriteJson(jsonWriter, _testVersion, serializer);

            Assert.AreEqual($"\"{_testVersion}\"", textWriter.ToString());
        }

        [Test]
        public void ReadJson()
        {
            var textReader = new StringReader($"\"{_testVersion}\"");
            var jsonReader = new JsonTextReader(textReader);
            var serializer = JsonSerializer.Create();
            var result = _converter.ReadJson(jsonReader, _testVersion.GetType(), null, serializer);

            Assert.NotNull(result);
            Assert.AreNotEqual(_testVersion, result);
            Assert.AreEqual(_testVersion.ToString(), result.ToString());
        }

        public void ReadJsonWithExisingValue()
        {
            var existingVersion = CalendarVersion.Create("2021.05.7b1");

            var textReader = new StringReader(_testVersion.ToString());
            var jsonReader = new JsonTextReader(textReader);
            var serializer = JsonSerializer.Create();
            var result = _converter.ReadJson(jsonReader, _testVersion.GetType(), existingVersion, serializer);

            Assert.NotNull(result);
            Assert.AreEqual(existingVersion, result);
            Assert.AreEqual(_testVersion.ToString(), result.ToString());
        }
    }
}
