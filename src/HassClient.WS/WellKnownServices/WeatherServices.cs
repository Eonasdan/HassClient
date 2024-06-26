using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using HassClient.Core.API.Models.WellKnowStates;
using HassClient.Core.Models;
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
    /// <param name="wrapper"></param>
    /// <param name="forecastRequest"></param>
    public static async Task<List<Forecast>> GetForecastAsync(IHomeAssistantClientWrapper wrapper,
        WeatherForecastRequest forecastRequest)
    {
        var result = await wrapper.WebSocketClient.CallServiceAsync<Dictionary<string, ForecastResponse>>("weather", "get_forecasts",
            data: forecastRequest
        );

        if (result == null || !result.TryGetValue(forecastRequest.EntityId, out var response)) 
            return [];

        return response.Forecasts;
    }
    
    public static async Task<(KnowWeatherStates State, WeatherState.WeatherAttributes Weather)?> GetCurrentAsync(IHomeAssistantClientWrapper wrapper)
    {
        var weatherState = await wrapper.ApiClient.State
            .GetStateAsync<WeatherState>("weather.forecast_home");

        if (weatherState is null) return null;

        return (weatherState.State, weatherState.Attributes);
    }

    public static Task<SunState?> GetSunAsync(IHomeAssistantClientWrapper wrapper)
    {
        return wrapper.ApiClient.State.GetStateAsync<SunState>("sun.sun");
    }

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
}