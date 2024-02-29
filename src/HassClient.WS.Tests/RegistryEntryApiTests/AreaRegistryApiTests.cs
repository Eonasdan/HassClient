using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Tests;
using NUnit.Framework;

namespace HassClient.WS.Tests.RegistryEntryApiTests
{
    public class AreaRegistryApiTests : BaseHassWsApiTest
    {
        private Area _testArea;

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateArea()
        {
            if (_testArea == null)
            {
                _testArea = new Area(MockHelpers.GetRandomTestName());
                var result = await HassWsApi.CreateAreaAsync(_testArea);
                Assert.That(result, Is.True, "SetUp failed");
                return;
            }

            Assert.That(_testArea.Id, Is.Not.Null);
            Assert.That(_testArea.Name, Is.Not.Null);
            Assert.That(_testArea.HasPendingChanges, Is.False);
            Assert.That(_testArea.IsTracked, Is.True);
        }

        [Test, Order(2)]
        public async Task GetAreas()
        {
            var areas = await HassWsApi.GetAreasAsync();

            Assert.That(areas, Is.Not.Null);
            Assert.That(areas, Is.Not.Empty);
            Assert.That(areas.Contains(_testArea), Is.True);
        }

        [Test, Order(3)]
        public async Task UpdateArea()
        {
            _testArea.Name = MockHelpers.GetRandomTestName();
            var result = await HassWsApi.UpdateAreaAsync(_testArea);

            Assert.That(result, Is.True);
            Assert.That(_testArea.HasPendingChanges, Is.False);
        }

        [Test, Order(4)]
        public async Task UpdateWithForce()
        {
            var initialName = _testArea.Name;
            var clonedArea = _testArea.Clone();
            clonedArea.Name = $"{initialName}_cloned";
            var result = await HassWsApi.UpdateAreaAsync(clonedArea);
            Assert.That(result, Is.True, "SetUp failed");
            Assert.That(_testArea.HasPendingChanges, Is.False, "SetUp failed");

            result = await HassWsApi.UpdateAreaAsync(_testArea, forceUpdate: true);
            Assert.That(result, Is.True);
            Assert.That(initialName, Is.EqualTo(_testArea.Name));
        }

        [OneTimeTearDown]
        [Test, Order(5)]
        public async Task DeleteArea()
        {
            if (_testArea == null)
            {
                return;
            }

            var result = await HassWsApi.DeleteAreaAsync(_testArea);
            var deletedArea = _testArea;
            _testArea = null;

            Assert.That(result, Is.True);
            Assert.That(deletedArea.IsTracked, Is.False);
        }
    }
}
