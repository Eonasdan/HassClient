using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using HassClient.WS.Tests.Extensions;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    [SetUpFixture]
    public class EnvironmentSetup
    {
        private IContainer _hassContainer;

        [OneTimeSetUp]
        public async Task GlobalSetupAsync()
        {
            var instanceBaseUrl = Environment.GetEnvironmentVariable(BaseHassWsApiTest.TestsInstanceBaseUrlVar);

            if (instanceBaseUrl == null)
            {
                // Create temporary directory with tests resources
                var tmpDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tmpDirectory);
                DirectoryExtensions.CopyFilesRecursively("./resources", tmpDirectory);

                const int hassPort = 8123;
                const string hassVersion = "latest";
                const string tokenFilename = "TOKEN";
                var testcontainersBuilder = new ContainerBuilder()
                      .WithImage($"homeassistant/home-assistant:{hassVersion}")
                      .WithPortBinding(hassPort, assignRandomHostPort: true)
                      .WithExposedPort(hassPort)
                      .WithBindMount(Path.Combine(tmpDirectory, "config"), "/config")
                      .WithBindMount(Path.Combine(tmpDirectory, "scripts"), "/app")
                      .WithWaitStrategy(Wait.ForUnixContainer()
                                            .UntilPortIsAvailable(hassPort))
                      .WithEntrypoint("/bin/bash", "-c")
                      .WithCommand($"python3 /app/create_token.py >/app/{tokenFilename} && /init");

                _hassContainer = testcontainersBuilder.Build();
                await _hassContainer.StartAsync();

                var mappedPort = _hassContainer.GetMappedPublicPort(hassPort);
                var hostTokenPath = Path.Combine(tmpDirectory, "scripts", tokenFilename);
                var accessToken = File.ReadLines(hostTokenPath).First();

                Environment.SetEnvironmentVariable(BaseHassWsApiTest.TestsInstanceBaseUrlVar, $"http://localhost:{mappedPort}");
                Environment.SetEnvironmentVariable(BaseHassWsApiTest.TestsAccessTokenVar, accessToken);
            }
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            if (_hassContainer != null)
            {
                await _hassContainer.DisposeAsync();
            }
        }
    }
}
