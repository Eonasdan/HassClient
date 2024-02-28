using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;

namespace HassClient.WS.Messages.Commands.RegistryEntryCollections
{
    internal class EntityRegistryMessagesFactory : RegistryEntryCollectionMessagesFactory<EntityRegistryEntry>
    {
        public static readonly EntityRegistryMessagesFactory Instance = new EntityRegistryMessagesFactory();

        public EntityRegistryMessagesFactory()
            : base("config/entity_registry", "entity")
        {
        }

        public BaseOutgoingMessage CreateGetMessage(string entityId)
        {
            return CreateCustomOperationMessage("get", entityId);
        }

        public BaseOutgoingMessage CreateUpdateMessage(EntityRegistryEntry entity, string newEntityId, bool? disable, bool forceUpdate)
        {
            var model = CreateDefaultUpdateObject(entity, forceUpdate);

            if (newEntityId != null)
            {
                var merged = HassSerializer.CreateJObject(new { NewEntityId = newEntityId });
                model.Merge(merged);
            }

            if (disable.HasValue)
            {
                var merged = HassSerializer.CreateJObject(new { DisabledBy = disable.Value ? DisabledByEnum.User : (DisabledByEnum?)null });
                model.Merge(merged);
            }

            return CreateUpdateMessage(entity.EntityId, model);
        }

        public new BaseOutgoingMessage CreateDeleteMessage(EntityRegistryEntry entity)
        {
            return CreateCustomOperationMessage("remove", entity.EntityId);
        }
    }
}
