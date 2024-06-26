using System.Collections.Generic;
using System.Text.Json.Serialization;
using HassClient.Core.Models;
using HassClient.Core.Models.KnownEnums;
using JetBrains.Annotations;

namespace HassClient.Core.API.Models.WellKnowStates;

[PublicAPI]
public class WeatherState : BaseState
{
    [JsonPropertyName("attributes")] public WeatherAttributes Attributes { get; init; } = default!;
    
    [JsonPropertyName("state")]
    public new KnowWeatherStates State { get; set; }

    [PublicAPI]
    public class WeatherAttributes
    {
        [JsonPropertyName("temperature")]
        public long Temperature { get; init; }

        [JsonPropertyName("dew_point")]
        public long DewPoint { get; init; }

        [JsonPropertyName("temperature_unit")]
        public string TemperatureUnit { get; init; } = default!;

        [JsonPropertyName("humidity")]
        public long Humidity { get; init; }

        [JsonPropertyName("cloud_coverage")]
        public double CloudCoverage { get; init; }

        [JsonPropertyName("pressure")]
        public double Pressure { get; init; }

        [JsonPropertyName("pressure_unit")]
        public string PressureUnit { get; init; } = default!;

        [JsonPropertyName("wind_bearing")]
        public double WindBearing { get; init; }

        [JsonPropertyName("wind_speed")]
        public double WindSpeed { get; init; }

        [JsonPropertyName("wind_speed_unit")]
        public string WindSpeedUnit { get; init; } = default!;

        [JsonPropertyName("visibility_unit")]
        public string VisibilityUnit { get; init; } = default!;

        [JsonPropertyName("precipitation_unit")]
        public string PrecipitationUnit { get; init; } = default!;

        [JsonPropertyName("forecast")] public List<Forecast> Forecast { get; init; } = [];

        [JsonPropertyName("attribution")]
        public string Attribution { get; init; } = default!;

        [JsonPropertyName("friendly_name")]
        public string FriendlyName { get; init; } = default!;

        [JsonPropertyName("supported_features")]
        public long SupportedFeatures { get; init; }
    }
}