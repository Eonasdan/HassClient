using System.IO;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    public class GetServicesCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is GetServicesMessage;

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            using var stream = GetResourceStream("GetServicesResponse.json");
            using var sr = new StreamReader(stream);
            using var reader = new JsonTextReader(sr);
            var resultObject = JRaw.Create(reader);
            return CreateResultMessageWithResult(resultObject);
        }
    }
}
