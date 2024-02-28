using System;
using HassClient.Core.Models.RegistryEntries.Modifiable;
using Newtonsoft.Json;

namespace HassClient.Core.Serialization.Converters
{
    /// <summary>
    /// Converter for <see cref="ModifiableProperty{T}"/>.
    /// </summary>
    public class ModifiablePropertyConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                var type = value.GetType();
                value = type.GetProperty(nameof(ModifiableProperty<object>.Value))
                    ?.GetValue(value, null);
            }

            serializer.Serialize(writer, value);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var argType = objectType.GetGenericArguments()[0];
            var value = serializer.Deserialize(reader, argType);

            if (existingValue != null)
            {
                objectType.GetProperty(nameof(ModifiableProperty<object>.Value))
                    ?.SetValue(existingValue, value);
            }
            else
            {
                existingValue = Activator.CreateInstance(objectType, value);
            }

            return existingValue;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(ModifiableProperty<>);
        }
    }
}
