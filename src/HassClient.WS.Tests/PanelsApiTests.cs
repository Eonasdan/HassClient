using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public class PanelsApiTests : BaseHassWsApiTest
    {
        private IEnumerable<PanelInfo>? _panels;

        [OneTimeSetUp]
        [Test]
        public async Task GetPanels()
        {
            if (_panels != null)
            {
                return;
            }

            _panels = await HassWsApi.GetPanelsAsync();
            Assert.That(_panels, Is.Not.Null);
            Assert.That(_panels, Is.Not.Empty);
            Assert.That(_panels, Is.Not.Empty);
        }

        [Test]
        public async Task GetPanel()
        {
            if (_panels != null)
            {
                return;
            }

            var firstPanel = _panels?.FirstOrDefault();
            Assert.That(firstPanel, Is.Not.Null, "SetUp failed");

            var result = await HassWsApi.GetPanelAsync(firstPanel.UrlPath);

            Assert.That(result, Is.Not.Null);
            Assert.That(firstPanel, Is.EqualTo(result));
            
            Assert.That(firstPanel, Is.EqualTo(result));
        }

        [Test]
        public void GetPanelWithNullUrlPathThrows()
        {
            Assert.ThrowsAsync<ArgumentException>(() => HassWsApi.GetPanelAsync(null));
        }

        [Test]
        public void GetPanelsHasComponentName()
        {
            Assert.That(_panels.All(x => x.ComponentName != default), Is.True);
        }

        [Test]
        public void GetPanelsHasConfiguration()
        {
            Assert.That(_panels.All(x => x.Configuration != default), Is.True);
        }

        [Test]
        public void GetPanelsHasIcon()
        {
            Assert.That(_panels.Any(x => x.Icon != default), Is.True);
        }

        [Test]
        public void GetPanelsHasRequireAdmin()
        {
            Assert.That(_panels.Any(x => x.RequireAdmin), Is.True);
        }

        [Test]
        public void GetPanelsHasTitle()
        {
            Assert.That(_panels.Any(x => x.Title != default), Is.True);
        }

        [Test]
        public void GetPanelsHasUrlPath()
        {
            Assert.That(_panels.All(x => x.UrlPath != default), Is.True);
        }
    }
}