using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HassClient.WS.Tests.RegistryEntryApiTests
{
    public class DeviceRegistryApiTests : BaseHassWsApiTest
    {
        [Test]
        public async Task GetDevices()
        {
            var devices = await HassWsApi.GetDevicesAsync();

            Assert.NotNull(devices);
            Assert.IsNotEmpty(devices);
        }

        [Test]
        public async Task UpdateNameDevice()
        {
            var devices = await HassWsApi.GetDevicesAsync();
            var testDevice = devices.FirstOrDefault();
            Assert.NotNull(testDevice, "SetUp failed");

            var newName = $"TestDevice_{DateTime.Now.Ticks}";
            testDevice.Name = newName;
            var result = await HassWsApi.UpdateDeviceAsync(testDevice);

            Assert.IsTrue(result);
            Assert.IsFalse(testDevice.HasPendingChanges);
            Assert.AreEqual(newName, testDevice.Name);
        }

        [Test]
        public async Task UpdateAreaIdDevice()
        {
            var devices = await HassWsApi.GetDevicesAsync();
            var testDevice = devices.FirstOrDefault();
            Assert.NotNull(testDevice, "SetUp failed");

            var newAreaId = $"{DateTime.Now.Ticks}";
            testDevice.AreaId = newAreaId;
            var result = await HassWsApi.UpdateDeviceAsync(testDevice);

            Assert.IsTrue(result);
            Assert.IsFalse(testDevice.HasPendingChanges);
            Assert.AreEqual(newAreaId, testDevice.AreaId);
        }
    }
}
