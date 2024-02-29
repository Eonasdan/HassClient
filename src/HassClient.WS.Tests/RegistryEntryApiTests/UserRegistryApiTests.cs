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

                Assert.That(result, Is.True, "SetUp failed");
                return;
            }

            Assert.That(_testUser.Id, Is.Not.Null);
            Assert.That(_testUser.Name, Is.Not.Null);
            Assert.That(_testUser.IsActive, Is.True);
            Assert.That(_testUser.IsLocalOnly, Is.False);
            Assert.That(_testUser.IsOwner, Is.False);
            Assert.That(_testUser.IsAdministrator, Is.False);
            Assert.That(_testUser.HasPendingChanges, Is.False);
            Assert.That(_testUser.IsTracked, Is.True);
        }

        [Test, Order(2)]
        public async Task GetUsers()
        {
            var users = await HassWsApi.GetUsersAsync();

            Assert.That(users, Is.Not.Null);
            Assert.That(users, Is.Not.Empty);
            Assert.That(users.Contains(_testUser), Is.True);
            Assert.That(users.Any(u => u.IsOwner), Is.True);
            Assert.That(users.Any(u => u.IsAdministrator), Is.True);
        }

        [Test, Order(3)]
        public async Task UpdateUserName()
        {
            var updatedName = MockHelpers.GetRandomTestName();
            _testUser.Name = updatedName;
            var result = await HassWsApi.UpdateUserAsync(_testUser);

            Assert.That(result, Is.True);
            Assert.That(updatedName, Is.EqualTo(_testUser.Name));
        }

        [Test, Order(3)]
        public async Task UpdateUserIsActive()
        {
            _testUser.IsActive = false;
            var result = await HassWsApi.UpdateUserAsync(_testUser);

            Assert.That(result, Is.True);
            Assert.That(_testUser.IsActive, Is.False);
        }

        [Test, Order(3)]
        public async Task UpdateUserIsLocalOnly()
        {
            _testUser.IsLocalOnly = true;
            var result = await HassWsApi.UpdateUserAsync(_testUser);

            Assert.That(result, Is.True);
            Assert.That(_testUser.IsLocalOnly, Is.True);
        }

        [Test, Order(3)]
        public async Task UpdateUserIsAdministrator()
        {
            _testUser.IsAdministrator = true;
            var result = await HassWsApi.UpdateUserAsync(_testUser);

            Assert.That(result, Is.True);
            Assert.That(_testUser.IsAdministrator, Is.True);
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
            Assert.That(result, Is.True, "SetUp failed");
            Assert.That(_testUser.HasPendingChanges, Is.False, "SetUp failed");

            result = await HassWsApi.UpdateUserAsync(_testUser, forceUpdate: true);
            Assert.That(result, Is.True);
            Assert.That(initialName, Is.EqualTo(_testUser.Name));
            Assert.That(initialGroupIds, Is.EqualTo(_testUser.GroupIds));
            Assert.That(initialIsActive, Is.EqualTo(_testUser.IsActive));
            Assert.That(initialIsLocalOnly, Is.EqualTo(_testUser.IsLocalOnly));
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

            Assert.That(result, Is.True);
            Assert.That(deletedUser.IsTracked, Is.False);
        }
    }
}
