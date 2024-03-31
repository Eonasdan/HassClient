using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace HassClient.Core.Serialization.Converters;

/// <summary>
/// Converter to convert Set[Tuple[str, str]] to <see cref="Dictionary{TKey,TValue}"/>.
/// </summary>
public class TupleSetToDictionaryConverter : JsonConverter<Dictionary<string, string?>>
{
    public override void WriteJson(JsonWriter writer, Dictionary<string, string?>? value, JsonSerializer serializer)
    {
        var array = value?.Select(x => new[] { x.Key, x.Value }).ToArray();
        serializer.Serialize(writer, array);
    }

    public override Dictionary<string, string?> ReadJson(JsonReader reader, Type objectType, Dictionary<string, string?>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var array = serializer.Deserialize<string[][]>(reader);

        if (array == null ||
            array.Length == 0)
        {
            return new Dictionary<string, string?>();
        }

        return array.ToDictionary(x => x[0], x => x.Length > 1 ? x[1] : null);
    }
}
    
    
/// <summary>
/// Converter to convert Set[Tuple[str, str]] to <see cref="Tuple{TKey,TValue}"/>.
/// </summary>
public class TupleSetConverter : JsonConverter<List<Tuple<string, string>>>
{
    public override void WriteJson(JsonWriter writer, List<Tuple<string, string>>? value, JsonSerializer serializer)
    {
        if (value == null) return;
        var array = value.Select(x => new[] { x.Item1, x.Item1 }).ToArray();
        serializer.Serialize(writer, array);
    }

    public override List<Tuple<string, string>> ReadJson(JsonReader reader, Type objectType, List<Tuple<string, string>>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var array = serializer.Deserialize<string[][]>(reader);

        if (array == null ||
            array.Length == 0)
        {
            return [];
        }

        return array
            .Select(x => new Tuple<string, string>(x[0], x[1]))
            .ToList();;
    }
}