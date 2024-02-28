using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands;
using HassClient.WS.Messages.Response;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    public class PingCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is PingMessage;

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            return new PongMessage();
        }
    }
}
