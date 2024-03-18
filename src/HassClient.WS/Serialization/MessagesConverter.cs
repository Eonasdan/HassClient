using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands;

namespace HassClient.WS.Serialization;

internal class MessagesConverter : JsonConverter
{
    private readonly Type _baseMessageType = typeof(BaseMessage);

    private readonly Dictionary<string, Func<BaseMessage>> _factoriesByType;

    public override bool CanRead => true;

    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType)
    {
        return _baseMessageType.IsAssignableFrom(objectType);
    }

    public MessagesConverter()
    {
        _factoriesByType = Assembly.GetAssembly(_baseMessageType)
            .GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(_baseMessageType) && x.GetConstructor(Type.EmptyTypes) != null)
            .Select(x => Expression.Lambda<Func<BaseMessage>>(Expression.New(x)).Compile())
            .ToDictionary(x => x().Type);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        var messageType = (string)obj["type"];

        BaseMessage message;
        if (_factoriesByType.TryGetValue(messageType, out var factory))
        {
            message = factory();
            serializer.Populate(obj.CreateReader(), message);
        }
        else
        {
            var id = obj.GetValue("id").Value<uint>();
            obj.Remove("id");
            obj.Remove("type");
            message = new RawCommandMessage(messageType, obj) { Id = id };
        }

        return message;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}