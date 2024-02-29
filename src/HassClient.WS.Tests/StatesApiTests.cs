using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public class StatesApiTests : BaseHassWsApiTest
    {
        private IEnumerable<StateModel> _states;

        [OneTimeSetUp]
        [Test]
        public async Task GetStates()
        {
            if (_states != null)
            {
                return;
            }

            _states = await HassWsApi.GetStatesAsync();

            Assert.That(_states, Is.Not.Null);
            Assert.That(_states, Is.Not.Empty);
        }

        [Test]
        public void GetStatesHasAttributes()
        {
            Assert.That(_states.All(x => x.Attributes.Count > 0), Is.True);
        }

        [Test]
        public void GetStatesHasLastChanged()
        {
            Assert.That(_states.All(x => x.LastChanged != default), Is.True);
        }

        [Test]
        public void GetStatesHasLastUpdated()
        {
            Assert.That(_states.All(x => x.LastUpdated != default), Is.True);
        }

        [Test]
        public void GetStatesHasState()
        {
            Assert.That(_states.All(x => x.State != default), Is.True);
        }

        [Test]
        public void GetStatesHasEntityId()
        {
            Assert.That(_states.All(x => x.EntityId != default), Is.True);
        }

        [Test]
        public void GetStatesHasContext()
        {
            Assert.That(_states.All(x => x.Context != default), Is.True);
        }
    }
}
