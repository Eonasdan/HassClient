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

            Assert.That(entities, Is.Not.Null);
            Assert.That(entities, Is.Not.Empty);
        }

        [Test]
        public async Task GetEntitySourceWithFilterAsync()
        {
            var entityId = "zone.home";
            var result = await HassWsApi.GetEntitySourceAsync(entityId);

            Assert.That(result.EntityId, Is.EqualTo(entityId));
            Assert.That(result.Domain, Is.EqualTo(entityId.Split('.')[0]));
            Assert.That(result.Source, Is.Not.Null);
        }
    }
}
