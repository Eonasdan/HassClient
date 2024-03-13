using System.Text.Json.Serialization;

namespace HassClient.WS.Messages.Commands
{
    internal class EntitySourceMessage : BaseOutgoingMessage
    {
        [JsonPropertyName("entity_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? EntityIds { get; set; }

        public EntitySourceMessage()
            : base("entity/source")
        {
        }
    }
}
