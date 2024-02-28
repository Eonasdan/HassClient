using System;
using HassClient.Core.Models.Color;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.Core.Serialization.Converters
{
    /// <summary>
    /// Converter for <see cref="Color"/>.
    /// </summary>
    public class ColorConverter : JsonConverter<Color>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            switch (value)
            {
                case NameColor _:
                    serializer.Serialize(writer, value.ToString());
                    break;
                case KelvinTemperatureColor kelvinColor:
                    serializer.Serialize(writer, kelvinColor.Kelvins);
                    break;
                case MiredsTemperatureColor miredsColor:
                    serializer.Serialize(writer, miredsColor.Mireds);
                    break;
                default:
                    serializer.Serialize(writer, JArray.Parse(value.ToString()));
                    break;
            }
        }

        /// <inheritdoc />
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(RGBWColor))
            {
                var values = serializer.Deserialize<JArray>(reader);
                if (!hasExistingValue)
                    return Color.FromRgbw((byte)values[0], (byte)values[1], (byte)values[2], (byte)values[3]);
                
                var rgbwColor = existingValue as RGBWColor;
                rgbwColor.R = (byte)values[0];
                rgbwColor.G = (byte)values[1];
                rgbwColor.B = (byte)values[2];
                rgbwColor.W = (byte)values[3];
                return rgbwColor;

            }

            if (objectType == typeof(RGBWWColor))
            {
                var values = serializer.Deserialize<JArray>(reader);
                if (!hasExistingValue)
                    return Color.FromRGBWW((byte)values[0], (byte)values[1], (byte)values[2], (byte)values[3],
                        (byte)values[4]);
                
                var rgbwwColor = existingValue as RGBWWColor;
                rgbwwColor.R = (byte)values[0];
                rgbwwColor.G = (byte)values[1];
                rgbwwColor.B = (byte)values[2];
                rgbwwColor.Cw = (byte)values[3];
                rgbwwColor.Ww = (byte)values[4];
                return rgbwwColor;

            }
            if (objectType == typeof(RGBColor))
            {
                var values = serializer.Deserialize<JArray>(reader);
                if (!hasExistingValue) return Color.FromRGB((byte)values[0], (byte)values[1], (byte)values[2]);
                
                var rgbColor = existingValue as RGBColor;
                rgbColor.R = (byte)values[0];
                rgbColor.G = (byte)values[1];
                rgbColor.B = (byte)values[2];
                return rgbColor;

            }
            if (objectType == typeof(HSColor))
            {
                var values = serializer.Deserialize<JArray>(reader);
                if (!hasExistingValue) return Color.FromHS((uint)values[0], (uint)values[1]);
                
                var hsColor = existingValue as HSColor;
                hsColor.Hue = (uint)values[0];
                hsColor.Saturation = (uint)values[1];
                return hsColor;

            }
            if (objectType == typeof(XYColor))
            {
                var values = serializer.Deserialize<JArray>(reader);
                if (!hasExistingValue) return Color.FromXY((float)values[0], (float)values[1]);
                
                var xyColor = existingValue as XYColor;
                xyColor.X = (float)values[0];
                xyColor.Y = (float)values[1];
                return xyColor;

            }
            if (objectType == typeof(NameColor))
            {
                var colorName = serializer.Deserialize<string>(reader);
                if (!hasExistingValue) return new NameColor(colorName);
                
                var nameColor = existingValue as NameColor;
                nameColor.Name = colorName;
                return nameColor;

            }
            if (objectType == typeof(MiredsTemperatureColor))
            {
                var mireds = serializer.Deserialize<uint>(reader);
                if (!hasExistingValue) return Color.FromMireds(mireds);
                
                var color = existingValue as MiredsTemperatureColor;
                color.Mireds = mireds;
                return color;

            }
            
            // ReSharper disable once InvertIf
            if (objectType == typeof(KelvinTemperatureColor))
            {
                var kelvins = serializer.Deserialize<uint>(reader);
                if (!hasExistingValue) return Color.FromKelvinTemperature(kelvins);
                
                var color = existingValue as KelvinTemperatureColor;
                color.Kelvins = kelvins;
                return color;

            }

            return null;
        }
    }
}
