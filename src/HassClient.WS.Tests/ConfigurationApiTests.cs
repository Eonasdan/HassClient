using System.Threading.Tasks;
using HassClient.Core.Models;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public class ConfigurationApiTests : BaseHassWsApiTest
    {

        private ConfigurationModel? _configuration;

        [OneTimeSetUp]
        [Test]
        public async Task GetConfiguration()
        {
            if (_configuration != null)
            {
                return;
            }

            _configuration = await HassWsApi.GetConfigurationAsync();

            Assert.That(_configuration, Is.Not.Null);
        }

        [Test]
        public void ConfigurationHasAllowedExternalDirs()
        {
            Assert.That(_configuration.AllowedExternalDirs, Is.Not.Null);
            Assert.That(_configuration.AllowedExternalDirs, Is.Not.Empty);
        }

        [Test]
        public void ConfigurationHasAllowedExternalUrls()
        {
            Assert.That(_configuration.AllowedExternalUrls, Is.Not.Null);
        }

        [Test]
        public void ConfigurationHasComponents()
        {
            Assert.That(_configuration.Components, Is.Not.Null);
            Assert.That(_configuration.Components, Is.Not.Empty);
        }

        [Test]
        public void ConfigurationHasConfigDirectory()
        {
            Assert.That(_configuration.ConfigDirectory, Is.Not.Null);
        }

        [Test]
        public void ConfigurationHasConfigSource()
        {
            Assert.That(_configuration.ConfigSource, Is.Not.Null);
        }

        [Test]
        public void ConfigurationHasLocation()
        {
            Assert.That(_configuration.LocationName, Is.Not.Null);
            Assert.That(_configuration.Latitude, Is.Not.Zero);
            Assert.That(_configuration.Longitude, Is.Not.Zero);
        }

        [Test]
        public void ConfigurationHasState()
        {
            Assert.That(_configuration.State, Is.Not.Null);
        }

        [Test]
        public void ConfigurationHasTimeZone()
        {
            Assert.That(_configuration.TimeZone, Is.Not.Null);
        }

        [Test]
        public void ConfigurationHasUnitSystem()
        {
            Assert.That(_configuration.UnitSystem, Is.Not.Null);
            Assert.That(_configuration.UnitSystem.Length, Is.Not.Null);
            Assert.That(_configuration.UnitSystem.Mass, Is.Not.Null);
            Assert.That(_configuration.UnitSystem.Pressure, Is.Not.Null);
            Assert.That(_configuration.UnitSystem.Temperature, Is.Not.Null);
            Assert.That(_configuration.UnitSystem.Volume, Is.Not.Null);
        }

        [Test]
        public void ConfigurationHasVersion()
        {
            Assert.That(_configuration.Version, Is.Not.Null);
        }
    }
}
