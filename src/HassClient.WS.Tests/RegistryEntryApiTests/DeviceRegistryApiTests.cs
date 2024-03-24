using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.WS.Extensions;
using NUnit.Framework;

namespace HassClient.WS.Tests.RegistryEntryApiTests
{
    public class DeviceRegistryApiTests : BaseHassWsApiTest
    {
        [Test]
        public async Task GetDevices()
        {
            var devices = await HassWsApi.GetDevicesAsync();

            Assert.That(devices, Is.Not.Null);
            Assert.That(devices, Is.Not.Empty);
        }

        [Test]
        public async Task UpdateNameDevice()
        {
            var devices = await HassWsApi.GetDevicesAsync();
            var testDevice = devices.FirstOrDefault();
            Assert.That(testDevice, Is.Not.Null, "SetUp failed");

            var newName = $"TestDevice_{DateTime.Now.Ticks}";
            testDevice.Name = newName;
            var result = await HassWsApi.UpdateDeviceAsync(testDevice);

            Assert.That(result, Is.True);
            Assert.That(testDevice.HasPendingChanges, Is.False);
            Assert.That(newName, Is.EqualTo(testDevice.Name));
        }

        [Test]
        public async Task UpdateAreaIdDevice()
        {
            var devices = await HassWsApi.GetDevicesAsync();
            var testDevice = devices.FirstOrDefault();
            Assert.That(testDevice, Is.Not.Null, "SetUp failed");

            var newAreaId = $"{DateTime.Now.Ticks}";
            testDevice.AreaId = newAreaId;
            var result = await HassWsApi.UpdateDeviceAsync(testDevice);

            Assert.That(result, Is.True);
            Assert.That(testDevice.HasPendingChanges, Is.False);
            Assert.That(newAreaId, Is.EqualTo(testDevice.AreaId));
        }
    }
}
