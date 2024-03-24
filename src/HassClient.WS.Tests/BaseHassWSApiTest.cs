using System;
using System.Threading.Tasks;
using HassClient.Core.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public abstract class BaseHassWsApiTest
    {
        public const string TestsInstanceBaseUrlVar = "TestsInstanceBaseUrl";
        public const string TestsAccessTokenVar = "TestsAccessToken";

        private readonly ConnectionParameters _connectionParameters;

        protected HassClientWebSocket HassWsApi;

        public BaseHassWsApiTest()
        {
            var instanceBaseUrl = Environment.GetEnvironmentVariable(TestsInstanceBaseUrlVar);
            var accessToken = Environment.GetEnvironmentVariable(TestsAccessTokenVar);

            if (instanceBaseUrl == null)
            {
                Assert.Ignore($"Hass instance base URL for tests not provided. It should be set in the environment variable '{TestsInstanceBaseUrlVar}'");
            }

            if (accessToken == null)
            {
                Assert.Ignore($"Hass access token for tests not provided. It should be set in the environment variable '{TestsAccessTokenVar}'");
            }

            _connectionParameters = ConnectionParameters.CreateFromInstanceBaseUrl(instanceBaseUrl, accessToken);
        }

        [OneTimeSetUp]
        protected virtual async Task OneTimeSetUp()
        {
            HassWsApi = new HassClientWebSocket();
            await HassWsApi.ConnectAsync(_connectionParameters);

            HassSerializer.DefaultSettings.MissingMemberHandling = MissingMemberHandling.Error;
            HassSerializer.DefaultSettings.Error += HassSerializerError;

            Assert.That(HassWsApi.ConnectionState, Is.EqualTo(ConnectionStates.Connected), "SetUp failed");
        }

        private void HassSerializerError(object sender, ErrorEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.ErrorContext.Path))
            {
                args.ErrorContext.Handled = true;
                Assert.Fail(args.ErrorContext.Error.Message);
            }
        }

        [OneTimeTearDown]
        protected virtual Task OneTimeTearDown()
        {
            return Task.CompletedTask;
        }
    }
}