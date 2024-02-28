using HassClient.Core.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    public class GetConfigurationCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is GetConfigMessage;

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var configuration = MockHassModelFactory.ConfigurationFaker.Generate();
            var resultObject = new JRaw(HassSerializer.SerializeObject(configuration));
            return CreateResultMessageWithResult(resultObject);
        }
    }
}
