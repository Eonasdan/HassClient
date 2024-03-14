using HassClient.Core.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.Core.Models
{
    /// <summary>
    /// Defines information related with a Home Assistant UI panel.
    /// </summary>
    public class PanelInfo
    {
        /// <summary>
        /// Gets the panel component name. Typical values are <c>config</c>, <c>lovelace</c>, <c>custom</c>, etc.
        /// </summary>
        [JsonPropertyName("ComponentName")]
        public string? ComponentName { get; init; }

        /// <summary>
        /// Gets the icon to display in the front-end.
        /// </summary>
        [JsonPropertyName("Icon")]
        public string? Icon { get; init; }

        /// <summary>
        /// Gets the title to display in the front-end.
        /// </summary>
        [JsonPropertyName("Title")]
        public string? Title { get; init; }

        /// <summary>
        /// Gets an object that contains specific configuration parameters of the panel.
        /// </summary>
        [JsonPropertyName("config")]
        public JRaw Configuration { get; private set; }

        /// <summary>
        /// Gets the URL path of the panel from which it can be accessed.
        /// </summary>
        [JsonPropertyName("UrlPath")]
        public string? UrlPath { get; init; }

        /// <summary>
        /// Gets a value indicating whether a user needs administrator rights to access the panel.
        /// </summary>
        [JsonPropertyName("RequireAdmin")]
        public bool RequireAdmin { get; init; }

        /// <summary>
        /// Deserializes the <see cref="Configuration"/> object with the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <returns>The deserialized configuration object.</returns>
        public T DeserializeConfig<T>()
        {
            return HassSerializer.DeserializeObject<T>(Configuration);
        }

        /// <inheritdoc />
        public override string ToString() => Title ?? UrlPath;
    }
}
