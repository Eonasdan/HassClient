using Bogus;
using HassClient.Core.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands.Search;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    public class SearchCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is SearchRelatedMessage;

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var searchMessage = receivedCommand as SearchRelatedMessage;
            var resultResponse = new SearchRelatedResponse();

            if (searchMessage.ItemType == ItemTypes.Entity &&
                searchMessage.ItemId == "light.bed_light")
            {
                var faker = new Faker();
                resultResponse.AreaIds = new[] { faker.RandomUuid() };
                resultResponse.AutomationIds = new[] { faker.RandomUuid() };
                resultResponse.ConfigEntryIds = new[] { faker.RandomUuid() };
                resultResponse.DeviceIds = new[] { faker.RandomUuid() };
                resultResponse.EntityIds = new[] { faker.RandomEntityId() };
            }

            var resultObject = new JRaw(HassSerializer.SerializeObject(resultResponse));
            return CreateResultMessageWithResult(resultObject);
        }
    }
}
