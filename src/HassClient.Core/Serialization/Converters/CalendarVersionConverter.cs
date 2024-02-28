using System;
using HassClient.Core.Models;
using Newtonsoft.Json;

namespace HassClient.Core.Serialization.Converters
{
    /// <summary>
    /// Converter for <see cref="CalendarVersion"/>.
    /// </summary>
    public class CalendarVersionConverter : JsonConverter<CalendarVersion>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, CalendarVersion value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        /// <inheritdoc />
        public override CalendarVersion ReadJson(JsonReader reader, Type objectType, CalendarVersion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var versionStr = serializer.Deserialize<string>(reader);
            existingValue = existingValue ?? new CalendarVersion();
            existingValue.Parse(versionStr);
            return existingValue;
        }
    }
}
