using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HassClient.Core.Serialization.Converters;

/// <summary>
/// Converter to convert Set[Tuple[str, str]] to <see cref="Dictionary{TKey,TValue}"/>.
/// </summary>
public class TupleSetToDictionaryConverter : JsonConverter<Dictionary<string, string?>>
{
    public override Dictionary<string, string?> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var values = JsonSerializer.Deserialize<string[][]>(ref reader, options);

        if (values == null || values.Length == 0)
        {
            return new Dictionary<string, string?>();
        }

        return values.ToDictionary(x => x[0], x => x.Length > 1 ? x[1] : null);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, string?> value, JsonSerializerOptions options)
    {
        var array = value.Select(x => new[] { x.Key, x.Value }).ToArray();
        JsonSerializer.Serialize(writer, array, options);
    }
}

public class TupleSetConverter : JsonConverter<List<Tuple<string, string>>>
{
    public override List<Tuple<string, string>> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var values = JsonSerializer.Deserialize<string[][]>(ref reader, options);

        if (values == null || values.Length == 0)
        {
            return [];
        }

        return values.Select(x => new Tuple<string, string>(x[0], x[1])).ToList();
    }

    public override void Write(Utf8JsonWriter writer, List<Tuple<string, string>> value, JsonSerializerOptions options)
    {
        var array = value.Select(x => new[] { x.Item1, x.Item2 }).ToArray();
        JsonSerializer.Serialize(writer, array, options);
    }
}