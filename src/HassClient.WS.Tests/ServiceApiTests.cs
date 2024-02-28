using System.Threading.Tasks;
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

            Assert.NotNull(services);
            Assert.IsNotEmpty(services);
        }

        [Test]
        public async Task CallService()
        {
            var result = await HassWsApi.CallServiceAsync("homeassistant", "check_config");

            Assert.NotNull(result);
        }

        [Test]
        public async Task CallServiceForEntities()
        {
            var result = await HassWsApi.CallServiceForEntitiesAsync("homeassistant", "update_entity", "sun.sun");

            Assert.NotNull(result);
        }

        [Test]
        public async Task CallServiceWithKnwonDomain()
        {
            var result = await HassWsApi.CallServiceAsync(KnownDomains.Homeassistant, KnownServices.CheckConfig);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task CallServiceForEntitiesWithKnwonDomain()
        {
            var result = await HassWsApi.CallServiceForEntitiesAsync(KnownDomains.Homeassistant, KnownServices.UpdateEntity, "sun.sun");

            Assert.IsTrue(result);
        }
    }
}
