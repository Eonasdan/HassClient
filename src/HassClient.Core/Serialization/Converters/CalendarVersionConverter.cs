using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using HassClient.Core.Models;

namespace HassClient.Core.Serialization.Converters;

/// <summary>
/// Converter for <see cref="CalendarVersion"/>.
/// </summary>
public class CalendarVersionConverter : JsonConverter<CalendarVersion>
{
    public override CalendarVersion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var versionStr = reader.GetString();
        var existingValue = new CalendarVersion();
        if (versionStr != null) existingValue.Parse(versionStr);

        return existingValue;
    }

    public override void Write(Utf8JsonWriter writer, CalendarVersion value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}