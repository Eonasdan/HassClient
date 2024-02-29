using System.Threading.Tasks;
using HassClient.WS.Messages.Commands;
using HassClient.WS.Messages.Response;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public class RawCommandApiTests : BaseHassWsApiTest
    {
        [Test]
        public async Task SendRawCommandWithResult()
        {
            var result = await HassWsApi.SendRawCommandWithResultAsync(new RawCommandMessage("get_config"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Result, Is.Not.Null);
        }

        [Test]
        public async Task SendRawCommandWithSuccess()
        {
            var result = await HassWsApi.SendRawCommandWithSuccessAsync(new RawCommandMessage("get_config"));

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task SendInvalidRawCommandWithResultReturnsUnknownCommandError()
        {
            var result = await HassWsApi.SendRawCommandWithResultAsync(new RawCommandMessage("invalid_command"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.Result, Is.Null);
            Assert.That(result.Error, Is.Not.Null);
            Assert.That(ErrorCodes.UnknownCommand, Is.EqualTo(result.Error.Code));
        }

        [Test]
        public async Task PingPongCommandResponseIsEncapsulatedAsResultMessage()
        {
            var result = await HassWsApi.SendRawCommandWithResultAsync(new RawCommandMessage("ping"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
        }
    }
}
