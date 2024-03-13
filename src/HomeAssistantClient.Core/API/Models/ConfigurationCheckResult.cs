using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Models
{
    /// <summary>
    /// Represents the Home Assistant configuration object.
    /// </summary>
    [PublicAPI]
    public class ConfigurationCheckResult
    {
        /// <summary>
        /// Gets the errors that occurred. This value is <see langword="null" /> if <see cref="Result" /> is <c>valid</c>.
        /// </summary>
        [JsonPropertyName("errors")]
        public string? Errors { get; set; }

        /// <summary>
        /// Gets the result of the configuration check. Valid values are <c>valid</c> and <c>invalid</c>.
        /// </summary>
        [JsonPropertyName("result")]
        public string? Result { get; set; }
        
        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        public override string ToString() => $"Configuration check result (status: {Result})";
    }
}
