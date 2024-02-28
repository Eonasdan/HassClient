using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Models;
using HassClient.Core.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    public class EntitySourceCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is EntitySourceMessage;

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var commandEntitySource = receivedCommand as EntitySourceMessage;
            IEnumerable<EntitySource> objs;
            objs = commandEntitySource?.EntityIds?.Length > 0 ? 
                MockHassModelFactory.EntitySourceFaker.GenerateWithEntityIds(commandEntitySource.EntityIds) :
                MockHassModelFactory.EntitySourceFaker.Generate(10);

            var resultObject = new JRaw(HassSerializer.SerializeObject(objs.ToDistinctDictionary(x => x.EntityId)));
            return CreateResultMessageWithResult(resultObject);
        }
    }
}
