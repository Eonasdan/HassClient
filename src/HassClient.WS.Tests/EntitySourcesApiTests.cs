using System.Threading.Tasks;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public class EntitySourcesApiTests : BaseHassWsApiTest
    {
        [Test]
        public async Task GetEntitySources()
        {
            var entities = await HassWsApi.GetEntitySourcesAsync();

            Assert.IsNotNull(entities);
            Assert.IsNotEmpty(entities);
        }

        [Test]
        public async Task GetEntitySourceWithFilterAsync()
        {
            var entityId = "zone.home";
            var result = await HassWsApi.GetEntitySourceAsync(entityId);

            Assert.AreEqual(result.EntityId, entityId);
            Assert.AreEqual(result.Domain, entityId.Split('.')[0]);
            Assert.NotNull(result.Source);
        }
    }
}
