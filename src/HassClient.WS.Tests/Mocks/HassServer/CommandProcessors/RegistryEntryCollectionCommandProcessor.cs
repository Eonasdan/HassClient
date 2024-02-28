using System;
using System.Linq;
using System.Reflection;
using Bogus;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands;
using HassClient.WS.Messages.Commands.RegistryEntryCollections;
using HassClient.WS.Messages.Response;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    public class RegistryEntryCollectionCommandProcessor<TFactory, TModel> : BaseCommandProcessor
        where TFactory : RegistryEntryCollectionMessagesFactory<TModel>
        where TModel : RegistryEntryBase
    {
        protected readonly TFactory ModelFactory;

        protected readonly PropertyInfo IdPropertyInfo;

        protected readonly Faker Faker;

        protected readonly string ModelIdPropertyName;

        protected readonly string ApiPrefix;

        protected string ModelName;

        private bool _isContextReady;

        public RegistryEntryCollectionCommandProcessor()
            : this(Activator.CreateInstance<TFactory>())
        {
        }

        protected RegistryEntryCollectionCommandProcessor(TFactory factory)
        {
            ModelFactory = factory;
            Faker = new Faker();

            ModelIdPropertyName = $"{ModelFactory.ModelName}_id";
            ApiPrefix = ModelFactory.ApiPrefix;
            ModelName = ModelFactory.ModelName;
            IdPropertyInfo = GetModelIdPropertyInfo();
        }

        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) =>
            receivedCommand is RawCommandMessage &&
            receivedCommand.Type.StartsWith(ApiPrefix) &&
            IsValidCommandType(receivedCommand.Type);

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            try
            {
                if (!_isContextReady)
                {
                    _isContextReady = true;
                    PrepareHassContext(context);
                }

                var merged = (receivedCommand as RawCommandMessage).MergedObject as JToken;
                var commandType = receivedCommand.Type;
                object result = null;

                if (commandType.EndsWith("list"))
                {
                    result = ProcessListCommand(context, merged);
                }
                else if (commandType.EndsWith("create"))
                {
                    result = ProcessCreateCommand(context, merged);
                }
                else if (commandType.EndsWith("delete"))
                {
                    result = ProcessDeleteCommand(context, merged);
                }
                else if (commandType.EndsWith("update"))
                {
                    result = ProcessUpdateCommand(context, merged) ?? ErrorCodes.NotFound;
                }
                else
                {
                    result = ProcessUnknownCommand(commandType, context, merged);
                }

                if (result is ErrorCodes errorCode)
                {
                    return CreateResultMessageWithError(new ErrorInfo(errorCode));
                }

                var resultObject = new JRaw(HassSerializer.SerializeObject(result));
                return CreateResultMessageWithResult(resultObject);
            }
            catch (Exception ex)
            {
                return CreateResultMessageWithError(new ErrorInfo(ErrorCodes.UnknownError) { Message = ex.Message });
            }
        }

        protected virtual PropertyInfo GetModelIdPropertyInfo()
        {
            var modelType = typeof(TModel);
            var properties = modelType.GetProperties();
            var modelIdProperty = properties.FirstOrDefault(x => HassSerializer.GetSerializedPropertyName(x) == ModelIdPropertyName);
            return modelIdProperty ?? properties.Where(x => x.Name.EndsWith("Id")).MinBy(x => x.Name.Length);
        }

        protected virtual bool IsValidCommandType(string commandType)
        {
            return commandType.EndsWith("create") ||
                   commandType.EndsWith("list") ||
                   commandType.EndsWith("update") ||
                   commandType.EndsWith("delete");
        }

        private string GetModelSerialized(JToken merged)
        {
            var modelSerialized = HassSerializer.SerializeObject(merged);
            var idPropertyName = HassSerializer.GetSerializedPropertyName(IdPropertyInfo);
            if (ModelIdPropertyName != idPropertyName)
            {
                modelSerialized = modelSerialized.Replace(ModelIdPropertyName, idPropertyName);
            }

            return modelSerialized;
        }

        protected virtual TModel DeserializeModel(JToken merged, out string modelSerialized)
        {
            modelSerialized = GetModelSerialized(merged);
            return HassSerializer.DeserializeObject<TModel>(modelSerialized);
        }

        protected virtual void PopulateModel(JToken merged, object target)
        {
            var modelSerialized = GetModelSerialized(merged);
            HassSerializer.PopulateObject(modelSerialized, target);
        }

        protected virtual object ProcessListCommand(MockHassServerRequestContext context, JToken merged)
        {
            return context.HassDb.GetObjects<TModel>();
        }

        protected virtual object ProcessCreateCommand(MockHassServerRequestContext context, JToken merged)
        {
            var model = DeserializeModel(merged, out var _);
            IdPropertyInfo.SetValue(model, Faker.RandomUuid());
            context.HassDb.CreateObject(model);
            return model;
        }

        protected virtual object ProcessUpdateCommand(MockHassServerRequestContext context, JToken merged)
        {
            var model = DeserializeModel(merged, out var modelSerialized);
            return context.HassDb.UpdateObject(model, new JRaw(modelSerialized));
        }

        protected virtual object ProcessDeleteCommand(MockHassServerRequestContext context, JToken merged)
        {
            var model = DeserializeModel(merged, out var _);
            context.HassDb.DeleteObject(model);
            return null;
        }

        protected virtual object ProcessUnknownCommand(string commandType, MockHassServerRequestContext context, JToken merged)
        {
            return ErrorCodes.NotSupported;
        }

        protected virtual void PrepareHassContext(MockHassServerRequestContext context)
        {
        }
    }
}
