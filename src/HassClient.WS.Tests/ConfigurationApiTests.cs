using System.Threading.Tasks;
using HassClient.Core.Models;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public class ConfigurationApiTests : BaseHassWsApiTest
    {

        private ConfigurationModel _configuration;

        [OneTimeSetUp]
        [Test]
        public async Task GetConfiguration()
        {
            if (_configuration != null)
            {
                return;
            }

            _configuration = await HassWsApi.GetConfigurationAsync();

            Assert.IsNotNull(_configuration);
        }

        [Test]
        public void ConfigurationHasAllowedExternalDirs()
        {
            Assert.NotNull(_configuration.AllowedExternalDirs);
            Assert.IsNotEmpty(_configuration.AllowedExternalDirs);
        }

        [Test]
        public void ConfigurationHasAllowedExternalUrls()
        {
            Assert.NotNull(_configuration.AllowedExternalUrls);
        }

        [Test]
        public void ConfigurationHasComponents()
        {
            Assert.NotNull(_configuration.Components);
            Assert.IsNotEmpty(_configuration.Components);
        }

        [Test]
        public void ConfigurationHasConfigDirectory()
        {
            Assert.NotNull(_configuration.ConfigDirectory);
        }

        [Test]
        public void ConfigurationHasConfigSource()
        {
            Assert.NotNull(_configuration.ConfigSource);
        }

        [Test]
        public void ConfigurationHasLocation()
        {
            Assert.NotNull(_configuration.LocationName);
            Assert.NotZero(_configuration.Latitude);
            Assert.NotZero(_configuration.Longitude);
        }

        [Test]
        public void ConfigurationHasState()
        {
            Assert.NotNull(_configuration.State);
        }

        [Test]
        public void ConfigurationHasTimeZone()
        {
            Assert.NotNull(_configuration.TimeZone);
        }

        [Test]
        public void ConfigurationHasUnitSystem()
        {
            Assert.NotNull(_configuration.UnitSystem);
            Assert.NotNull(_configuration.UnitSystem.Length);
            Assert.NotNull(_configuration.UnitSystem.Mass);
            Assert.NotNull(_configuration.UnitSystem.Pressure);
            Assert.NotNull(_configuration.UnitSystem.Temperature);
            Assert.NotNull(_configuration.UnitSystem.Volume);
        }

        [Test]
        public void ConfigurationHasVersion()
        {
            Assert.NotNull(_configuration.Version);
        }
    }
}
