using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using HassClient.Core.Models;
using JetBrains.Annotations;

namespace HassClient.Core.API.Models.WellKnowStates;

[PublicAPI]
public class SunState : BaseState
{
    [JsonPropertyName("attributes")] public SunAttributes Attributes { get; init; } = default!;

    [PublicAPI]
    public class SunAttributes
    {
        [JsonPropertyName("next_dawn")]
        public DateTimeOffset NextDawn { get; init; }

        [JsonPropertyName("next_dusk")]
        public DateTimeOffset NextDusk { get; init; }

        [JsonPropertyName("next_midnight")]
        public DateTimeOffset NextMidnight { get; init; }

        [JsonPropertyName("next_noon")]
        public DateTimeOffset NextNoon { get; init; }

        [JsonPropertyName("next_rising")]
        public DateTimeOffset NextRising { get; init; }

        [JsonPropertyName("next_setting")]
        public DateTimeOffset NextSetting { get; init; }

        [JsonPropertyName("elevation")]
        public double Elevation { get; init; }

        [JsonPropertyName("azimuth")]
        public double Azimuth { get; init; }

        [JsonPropertyName("rising")]
        public bool Rising { get; init; }

        [JsonPropertyName("friendly_name")]
        public string? FriendlyName { get; init; }
    }
}