using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Messages.Commands.RegistryEntryCollections
{
    internal class EntityEntryResponse
    {
        [JsonProperty("entity_entry")]
        public JRaw EntityEntryRaw { get; set; }

        public int ReloadDelay { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"{EntityEntryRaw}";
    }
}
