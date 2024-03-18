using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using HassClient.Core.Models.Color;

namespace HassClient.Core.Serialization.Converters;

/// <summary>
/// Converter for <see cref="Color"/>.
/// </summary>
public class ColorConverter : JsonConverter<Color>
{
    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case NameColor _:
                writer.WriteStringValue(value.ToString());
                break;
            case KelvinTemperatureColor kelvinColor:
                writer.WriteNumberValue(kelvinColor.Kelvins);
                break;
            case MiredsTemperatureColor miredsColor:
                writer.WriteNumberValue(miredsColor.Mireds);
                break;
            default:
                JsonSerializer.Serialize(writer,
                    JsonSerializer.Deserialize<byte[]>(value.ToString() ?? string.Empty), options);
                break;
        }
    }

    /// <inheritdoc />
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(NameColor))
        {
            var colorName = reader.GetString();
            return new NameColor(colorName!);
        }

        if (typeToConvert == typeof(KelvinTemperatureColor))
        {
            var kelvins = reader.GetUInt32();
            return Color.FromKelvinTemperature(kelvins);
        }
        
        if (typeToConvert == typeof(RGBWColor))
        {
            var values = JsonSerializer.Deserialize<byte[]>(ref reader, options);
            return Color.FromRgbw(values[0], values[1], values[2], values[3]);
        }

        if (typeToConvert == typeof(RGBWWColor))
        {
            var values = JsonSerializer.Deserialize<byte[]>(ref reader, options);
            return Color.FromRGBWW(values[0], values[1], values[2], values[3], values[4]);
        }

        if (typeToConvert == typeof(RGBColor))
        {
            var values = JsonSerializer.Deserialize<byte[]>(ref reader, options);
            return Color.FromRGB(values[0], values[1], values[2]);
        }

        if (typeToConvert == typeof(HSColor))
        {
            var values = JsonSerializer.Deserialize<uint[]>(ref reader, options);
            return Color.FromHS(values[0], values[1]);
        }

        if (typeToConvert == typeof(XYColor))
        {
            var values = JsonSerializer.Deserialize<float[]>(ref reader, options);
            return Color.FromXY(values[0], values[1]);
        }

        if (typeToConvert == typeof(MiredsTemperatureColor))
        {
            var mireds = JsonSerializer.Deserialize<uint>(ref reader, options);
            return Color.FromMireds(mireds);
        }

        throw new JsonException();
    }
}