using System;
using System.Text.Json.Serialization;
using HassClient.Core.Models.KnownEnums;
using Newtonsoft.Json;

namespace HassClient.Core.Models;

public class Forecast
{
    [JsonProperty("condition")]
    [JsonPropertyName("condition")]
    public KnowWeatherStates Condition { get; set; }

    [JsonProperty("datetime")]
    [JsonPropertyName("datetime")]
    public DateTimeOffset Datetime { get; set; }

    [JsonProperty("wind_bearing")]
    [JsonPropertyName("wind_bearing")]
    public double WindBearing { get; set; }

    [JsonProperty("temperature")]
    [JsonPropertyName("temperature")]
    public long Temperature { get; set; }

    // ReSharper disable once StringLiteralTypo
    [JsonProperty("templow")]
    // ReSharper disable once StringLiteralTypo
    [JsonPropertyName("templow")]
    public long TemperatureLow { get; set; }

    [JsonProperty("wind_speed")]
    [JsonPropertyName("wind_speed")]
    public double WindSpeed { get; set; }

    [JsonProperty("precipitation")]
    [JsonPropertyName("precipitation")]
    public double Precipitation { get; set; }

    [JsonProperty("humidity")]
    [JsonPropertyName("humidity")]
    public long Humidity { get; set; }
}