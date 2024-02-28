using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;
using HassClient.WS.Messages.Commands.RegistryEntryCollections;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    internal class UserStorageCollectionCommandProcessor
        : RegistryEntryCollectionCommandProcessor<UserMessagesFactory, User>
    {
        protected override object ProcessCreateCommand(MockHassServerRequestContext context, JToken merged)
        {
            var user = (User)base.ProcessCreateCommand(context, merged);
            user.SetIsActive(true);
            return new UserResponse { UserRaw = new JRaw(HassSerializer.SerializeObject(user)) };
        }

        protected override object ProcessUpdateCommand(MockHassServerRequestContext context, JToken merged)
        {
            var user = base.ProcessUpdateCommand(context, merged);
            return new UserResponse { UserRaw = new JRaw(HassSerializer.SerializeObject(user)) };
        }

        protected override object ProcessListCommand(MockHassServerRequestContext context, JToken merged)
        {
            return base.ProcessListCommand(context, merged);
        }

        protected override void PrepareHassContext(MockHassServerRequestContext context)
        {
            base.PrepareHassContext(context);
            var ownerUser = User.CreateUnmodified(Faker.RandomUuid(), "owner", true);
            context.HassDb.CreateObject(ownerUser);
        }
    }
}
