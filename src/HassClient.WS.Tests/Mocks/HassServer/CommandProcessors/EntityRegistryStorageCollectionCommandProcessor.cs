using System.Linq;
using System.Runtime.Serialization;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;
using HassClient.WS.Messages.Commands.RegistryEntryCollections;
using HassClient.WS.Messages.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    internal class EntityRegistryStorageCollectionCommandProcessor
        : RegistryEntryCollectionCommandProcessor<EntityRegistryMessagesFactory, EntityRegistryEntry>
    {
        private class MockRegistryEntity : EntityRegistryEntry
        {
            [JsonIgnore]
            public readonly EntityRegistryEntryBase Entry;

            public MockRegistryEntity(string entityId, string originalName, string originalIcon = null, DisabledByEnum disabledBy = DisabledByEnum.None)
            : base(entityId, null, null, disabledBy)
            {
                OriginalName = originalName;
                OriginalIcon = originalIcon;
            }

            public MockRegistryEntity(EntityRegistryEntryBase entry, DisabledByEnum disabledBy = DisabledByEnum.None)
            : this(entry.EntityId, entry.Name, entry.Icon, disabledBy)
            {
                Entry = entry;
                UniqueId = entry.UniqueId;
                Name = entry.Name;
                Icon = entry.Icon;
            }

            [OnDeserialized]
            private void OnDeserializedMock(StreamingContext context)
            {
                Entry.Name = Name;
                Entry.Icon = Icon;
                //this.Entry.UniqueId = this.EntityId.SplitEntityId()[1];
            }
        }

        protected override bool IsValidCommandType(string commandType)
        {
            return commandType.EndsWith("create") ||
                   commandType.EndsWith("list") ||
                   commandType.EndsWith("update") ||
                   commandType.EndsWith("get") ||
                   commandType.EndsWith("remove");
        }

        protected override object ProcessListCommand(MockHassServerRequestContext context, JToken merged)
        {
            return context.HassDb.GetAllEntityEntries().Select(x => x as EntityRegistryEntry ?? EntityRegistryEntry.CreateFromEntry(x));
        }

        protected override object ProcessUpdateCommand(MockHassServerRequestContext context, JToken merged)
        {
            var newEntityIdProperty = merged.FirstOrDefault(x => x is JProperty property && property.Name == "new_entity_id");
            var newEntityId = (string)newEntityIdProperty;
            newEntityIdProperty?.Remove();

            var entityIdProperty = merged.FirstOrDefault(x => x is JProperty property && property.Name == "entity_id");
            var entityId = (string)entityIdProperty;
            var result = FindRegistryEntry(context, entityId, createIfNotFound: true);
            if (result == null)
                return new EntityEntryResponse { EntityEntryRaw = new JRaw(HassSerializer.SerializeObject(result)) };
            
            if (newEntityId != null)
            {
                context.HassDb.DeleteObject(result.Entry);
                ((JProperty)entityIdProperty).Value = newEntityId;
            }

            PopulateModel(merged, result);

            if (newEntityId != null)
            {
                context.HassDb.CreateObject(result.Entry);
            }

            return new EntityEntryResponse { EntityEntryRaw = new JRaw(HassSerializer.SerializeObject(result)) };
        }

        protected override object ProcessUnknownCommand(string commandType, MockHassServerRequestContext context, JToken merged)
        {
            var entityId = merged.Value<string>("entity_id");
            if (string.IsNullOrEmpty(entityId))
            {
                return ErrorCodes.InvalidFormat;
            }

            if (commandType.EndsWith("get"))
            {
                return FindRegistryEntry(context, entityId, createIfNotFound: true);
            }

            if (!commandType.EndsWith("remove")) return base.ProcessUnknownCommand(commandType, context, merged);
            
            var mockEntry = FindRegistryEntry(context, entityId, createIfNotFound: false);
            if (mockEntry == null)
            {
                return ErrorCodes.NotFound;
            }

            context.HassDb.DeleteObject(mockEntry);
            var result = context.HassDb.DeleteObject(mockEntry.Entry);
            return result ? null : ErrorCodes.NotFound;

        }

        private MockRegistryEntity FindRegistryEntry(MockHassServerRequestContext context, string entityId, bool createIfNotFound)
        {
            var hassDb = context.HassDb;
            var result = hassDb.GetObjects<MockRegistryEntity>()?.FirstOrDefault(x => x.EntityId == entityId);
            if (result != null)
            {
                return result;
            }

            var entry = hassDb.FindEntityEntry(entityId);
            if (entry == null)
            {
                return null;
            }

            result = new MockRegistryEntity(entry);

            if (createIfNotFound)
            {
                hassDb.CreateObject(result);
            }

            return result;
        }

        protected override void PrepareHassContext(MockHassServerRequestContext context)
        {
            base.PrepareHassContext(context);
            var hassDb = context.HassDb;
            hassDb.CreateObject(new MockRegistryEntity("light.bed_light", "Bed Light")
            {
                UniqueId = Faker.RandomUuid(),
                ConfigEntryId = Faker.RandomUuid(),
            });

            hassDb.CreateObject(new MockRegistryEntity("switch.fake_switch", "Fake Switch", "mdi: switch", DisabledByEnum.Integration)
            {
                UniqueId = Faker.RandomUuid(),
                ConfigEntryId = Faker.RandomUuid()
            });
        }
    }
}
