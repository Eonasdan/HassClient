using Newtonsoft.Json;

namespace HassClient.Core.Models
{
    /// <summary>
    /// Represents a container for units of measure.
    /// </summary>
    public class UnitSystemModel
    {
        /// <summary>
        /// Gets the length unit (usually "km" or "mi").
        /// </summary>
        [JsonPropertyName("Length")]
        public string? Length { get; init; }

        /// <summary>
        /// Gets the mass unit (usually "g" or "lb").
        /// </summary>
        [JsonPropertyName("Mass")]
        public string? Mass { get; init; }

        /// <summary>
        /// Gets the pressure unit (usually "Pa" or "psi").
        /// </summary>
        [JsonPropertyName("Pressure")]
        public string? Pressure { get; init; }

        /// <summary>
        /// Gets the temperature unit including degree symbol (usually "°C" or "°F").
        /// </summary>
        [JsonPropertyName("Temperature")]
        public string? Temperature { get; init; }

        /// <summary>
        /// Gets the volume unit (usually "L" or "gal").
        /// </summary>
        [JsonPropertyName("Volume")]
        public string? Volume { get; init; }

        /// <summary>
        /// Gets the accumulated precipitation unit (usually "mm" or "in").
        /// </summary>
        [JsonPropertyName("AccumulatedPrecipitation")]
        public string? AccumulatedPrecipitation { get; init; }

        /// <summary>
        /// Gets the wind speed unit (usually "m/s" or "mi/h").
        /// </summary>
        [JsonPropertyName("WindSpeed")]
        public string? WindSpeed { get; init; }
    }
}
