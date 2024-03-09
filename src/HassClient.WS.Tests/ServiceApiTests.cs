using System.Collections.Generic;
using System.Threading.Tasks;
using HassClient.Core.Models;
using HassClient.Core.Models.KnownEnums;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public class ServiceApiTests : BaseHassWsApiTest
    {
        [Test]
        public async Task GetServices()
        {
            var services = await HassWsApi.GetServicesAsync();

            Assert.That(services, Is.Not.Null);
            Assert.That(services, Is.Not.Empty);
        }

        [Test]
        public async Task CallService()
        {
            var result = await HassWsApi.CallServiceAsync("homeassistant", "check_config");

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task CallServiceForEntities()
        {
            var result = await HassWsApi.CallServiceForEntitiesAsync("homeassistant", "update_entity", "sun.sun");

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task CallServiceWithKnwonDomain()
        {
            var result = await HassWsApi.CallServiceAsync(KnownDomains.Homeassistant, KnownServices.CheckConfig);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CallServiceForEntitiesWithKnwonDomain()
        {
            var result = await HassWsApi.CallServiceForEntitiesAsync(KnownDomains.Homeassistant, KnownServices.UpdateEntity, "sun.sun");

            Assert.That(result, Is.True);
        }
    }
}
