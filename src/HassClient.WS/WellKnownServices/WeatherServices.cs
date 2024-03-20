using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using HassClient.Core.Models.KnownEnums;
using HassClient.WS.Extensions;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace HassClient.WS.WellKnownServices;

[PublicAPI]
public static class WeatherServices
{
    /// <summary>
    /// Get forecasts
    /// </summary>
    /// <param name="client"></param>
    /// <param name="forecastRequest"></param>
    public static async Task<List<Forecast>> GetForecast(HassClientWebSocket client,
        WeatherForecastRequest forecastRequest)
    {
        var result = await client.CallServiceAsync<Dictionary<string, ForecastResponse>>("weather", "get_forecasts",
            data: forecastRequest
        );

        if (result == null || !result.TryGetValue(forecastRequest.EntityId, out var response)) 
            return [];

        return response.Forecasts;
    }
    //todo also get from "state" since that has current temps

    public class WeatherForecastRequest(
        ForecastType type = ForecastType.Daily,
        string entityId = "weather.forecast_home")
    {
        [JsonProperty("entity_id"), JsonRequired]
        public string EntityId { get; set; } = entityId;

        [JsonProperty("type"), JsonRequired] public ForecastType Type { get; set; } = type;
    }

    [PublicAPI]
    public enum ForecastType
    {
        Daily,
        Hourly,
        TwiceDaily
    }

    public class ForecastResponse
    {
        [JsonProperty("forecast")] public List<Forecast> Forecasts { get; set; } = [];
    }

    public class Forecast
    {
        [JsonProperty("condition")] public KnownStates Condition { get; set; }

        [JsonProperty("datetime")] public DateTimeOffset Datetime { get; set; }

        [JsonProperty("wind_bearing")] public double WindBearing { get; set; }

        [JsonProperty("temperature")] public long Temperature { get; set; }

        // ReSharper disable once StringLiteralTypo
        [JsonProperty("templow")] public long TemperatureLow { get; set; }

        [JsonProperty("wind_speed")] public double WindSpeed { get; set; }

        [JsonProperty("precipitation")] public double Precipitation { get; set; }

        [JsonProperty("humidity")] public long Humidity { get; set; }
    }
}