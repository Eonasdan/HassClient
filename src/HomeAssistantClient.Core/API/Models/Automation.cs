using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Models
{
    /// <summary>
    /// Represents automation object.
    /// </summary>
    [PublicAPI]
    public class Automation : BaseHttpContent
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the actions.
        /// </summary>
        [JsonPropertyName("action")]
        public List<Dictionary<string, object>> Actions { get; set; } = [];

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        [JsonPropertyName("condition")]
        public List<Dictionary<string, object>> Conditions { get; set; } = [];

        /// <summary>
        /// Gets or sets the triggers.
        /// </summary>
        [JsonPropertyName("trigger")]
        public List<Dictionary<string, object>> Triggers { get; set; } = [];
    }
    
    /// <summary>
    /// Represents the automation result.
    /// </summary>
    public class AutomationResultObject
    {
        /// <summary>
        /// Gets or sets the automation result.
        /// </summary>
        [JsonPropertyName("result")]
        public string? Result { get; set; }
    }
}
