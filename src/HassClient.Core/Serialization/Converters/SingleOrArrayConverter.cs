/*using System;
using Newtonsoft.Json;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using JsonTokenType = System.Text.Json.JsonTokenType;
using Utf8JsonReader = System.Text.Json.Utf8JsonReader;
using Utf8JsonWriter = System.Text.Json.Utf8JsonWriter;

namespace HassClient.Core.Serialization.Converters;

public class SingleOrArrayConverter : JsonConverter<string[]>
{
    public override string[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new[] { reader.GetString() };
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            return JsonSerializer.Deserialize<string[]>(ref reader, options);
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
    {
        if (value.Length == 1)
        {
            writer.WriteStringValue(value[0]);
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}*/

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.Core.Serialization.Converters;

public class SingleOrArrayConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(string[]));
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return token.Type switch
        {
            JTokenType.Null => null,
            JTokenType.String => [token.ToObject<string>()],
            JTokenType.Array => token.ToObject<string[]>(),
            _ => throw new JsonException()
        };
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var stringArray = value as string[];
        if (stringArray is { Length: 1 })
        {
            serializer.Serialize(writer, stringArray[0]);
        }
        else
        {
            serializer.Serialize(writer, stringArray);
        }
    }
}