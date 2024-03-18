using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using JsonTokenType = System.Text.Json.JsonTokenType;
using Utf8JsonReader = System.Text.Json.Utf8JsonReader;
using Utf8JsonWriter = System.Text.Json.Utf8JsonWriter;

namespace HassClient.Core.Serialization.Converters;

public class SingleOrArrayConverter : JsonConverter<string[]>
{
    public override string[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return [reader.GetString()];
        }

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            return JsonSerializer.Deserialize<string[]>(ref reader, options) ?? Array.Empty<string>();
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
}