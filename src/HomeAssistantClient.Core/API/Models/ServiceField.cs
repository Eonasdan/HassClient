using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Models;

/// <summary>
/// Represents a single field in a service call.
/// </summary>
[PublicAPI]
public class ServiceField
{
    /// <summary>
    /// Gets or sets the description of this field.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the example text for this field (may be <see langword="null" />).
    /// </summary>
    [JsonConverter(typeof(ServiceExampleDeserializer))]
    public string? Example { get; set; }

    public class ServiceExampleDeserializer : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert) => true;

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
                using var jsonDoc = JsonDocument.ParseValue(ref reader);
                return 
                    jsonDoc.RootElement.ValueKind is JsonValueKind.Object or JsonValueKind.Array ? 
                        jsonDoc.RootElement.GetRawText() :
                        jsonDoc.RootElement.ToString();
            }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
                throw new InvalidOperationException("Should not get here.");
            }
    }
}