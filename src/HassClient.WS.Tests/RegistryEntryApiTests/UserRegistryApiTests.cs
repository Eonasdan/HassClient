using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Tests;
using NUnit.Framework;

namespace HassClient.WS.Tests.RegistryEntryApiTests
{
    public class UserRegistryApiTests : BaseHassWsApiTest
    {
        private User _testUser;

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateUser()
        {
            if (_testUser == null)
            {
                _testUser = new User(MockHelpers.GetRandomTestName());
                var result = await HassWsApi.CreateUserAsync(_testUser);

                Assert.IsTrue(result, "SetUp failed");
                return;
            }

            Assert.NotNull(_testUser.Id);
            Assert.NotNull(_testUser.Name);
            Assert.IsTrue(_testUser.IsActive);
            Assert.IsFalse(_testUser.IsLocalOnly);
            Assert.IsFalse(_testUser.IsOwner);
            Assert.IsFalse(_testUser.IsAdministrator);
            Assert.IsFalse(_testUser.HasPendingChanges);
            Assert.IsTrue(_testUser.IsTracked);
        }

        [Test, Order(2)]
        public async Task GetUsers()
        {
            var users = await HassWsApi.GetUsersAsync();

            Assert.NotNull(users);
            Assert.IsNotEmpty(users);
            Assert.IsTrue(users.Contains(_testUser));
            Assert.IsTrue(users.Any(u => u.IsOwner));
            Assert.IsTrue(users.Any(u => u.IsAdministrator));
        }

        [Test, Order(3)]
        public async Task UpdateUserName()
        {
            var updatedName = MockHelpers.GetRandomTestName();
            _testUser.Name = updatedName;
            var result = await HassWsApi.UpdateUserAsync(_testUser);

            Assert.IsTrue(result);
            Assert.AreEqual(updatedName, _testUser.Name);
        }

        [Test, Order(3)]
        public async Task UpdateUserIsActive()
        {
            _testUser.IsActive = false;
            var result = await HassWsApi.UpdateUserAsync(_testUser);

            Assert.IsTrue(result);
            Assert.IsFalse(_testUser.IsActive);
        }

        [Test, Order(3)]
        public async Task UpdateUserIsLocalOnly()
        {
            _testUser.IsLocalOnly = true;
            var result = await HassWsApi.UpdateUserAsync(_testUser);

            Assert.IsTrue(result);
            Assert.IsTrue(_testUser.IsLocalOnly);
        }

        [Test, Order(3)]
        public async Task UpdateUserIsAdministrator()
        {
            _testUser.IsAdministrator = true;
            var result = await HassWsApi.UpdateUserAsync(_testUser);

            Assert.IsTrue(result);
            Assert.IsTrue(_testUser.IsAdministrator);
        }

        [Test, Order(4)]
        public async Task UpdateWithForce()
        {
            var initialName = _testUser.Name;
            var initialGroupIds = _testUser.GroupIds;
            var initialIsActive = _testUser.IsActive;
            var initialIsLocalOnly = _testUser.IsLocalOnly;
            var clonedEntry = _testUser.Clone();
            clonedEntry.Name = $"{initialName}_cloned";
            clonedEntry.IsAdministrator = !_testUser.IsAdministrator;
            clonedEntry.IsActive = !initialIsActive;
            clonedEntry.IsLocalOnly = !initialIsLocalOnly;
            var result = await HassWsApi.UpdateUserAsync(clonedEntry);
            Assert.IsTrue(result, "SetUp failed");
            Assert.False(_testUser.HasPendingChanges, "SetUp failed");

            result = await HassWsApi.UpdateUserAsync(_testUser, forceUpdate: true);
            Assert.IsTrue(result);
            Assert.AreEqual(initialName, _testUser.Name);
            Assert.AreEqual(initialGroupIds, _testUser.GroupIds);
            Assert.AreEqual(initialIsActive, _testUser.IsActive);
            Assert.AreEqual(initialIsLocalOnly, _testUser.IsLocalOnly);
        }

        [OneTimeTearDown]
        [Test, Order(5)]
        public async Task DeleteUser()
        {
            if (_testUser == null)
            {
                return;
            }

            var result = await HassWsApi.DeleteUserAsync(_testUser);
            var deletedUser = _testUser;
            _testUser = null;

            Assert.IsTrue(result);
            Assert.IsFalse(deletedUser.IsTracked);
        }
    }
}
