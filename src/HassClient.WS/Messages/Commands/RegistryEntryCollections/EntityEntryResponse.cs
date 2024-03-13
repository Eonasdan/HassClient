using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Messages.Commands.RegistryEntryCollections
{
    internal class EntityEntryResponse
    {
        [JsonPropertyName("entity_entry")]
        public JRaw EntityEntryRaw { get; set; }

        public int ReloadDelay { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"{EntityEntryRaw}";
    }
}
