using HassClient.Core.Models;
using HassClient.Core.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    public class CallServiceCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is CallServiceMessage;

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var callServiceMsg = receivedCommand as CallServiceMessage;
            var state = new StateModel
            {
                Context = MockHassModelFactory.ContextFaker.Generate()
            };
            var resultObject = new JRaw(HassSerializer.SerializeObject(state));
            return CreateResultMessageWithResult(resultObject);
        }
    }
}
