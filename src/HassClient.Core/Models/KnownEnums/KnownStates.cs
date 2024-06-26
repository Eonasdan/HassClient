using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HassClient.Core.Models.KnownEnums;

/// <summary>
/// Represents a list of known states. Useful to reduce use of strings.
/// </summary>
[SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1602:Enumeration items should be documented",
    Justification = "Due to the nature of the list, it is not necessary to document each field.")]
[PublicAPI]
[JsonConverter(typeof(SnakeJsonStringEnumConverter<KnownStates>))]
public enum KnownStates
{
    /// <summary>
    /// Used to represent a state not defined within this enum.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The entity state is not available.
    /// </summary>
    Unavailable,

    /// <summary>
    /// The entity state is unknown.
    /// </summary>
    Unknown,

    AboveHorizon,
    Active,
    Armed,
    ArmedAway,
    ArmedCustomBypass,
    ArmedHome,
    ArmedNight,
    Arming,
    Auto,
    BackedUp,
    BelowHorizon,
    Cleaning,
    Closed,
    Closing,
    Configure,
    Configured,
    Cool,
    Dead,
    Disarmed,
    Disarming,
    Discharging,
    Docked,
    Dry,
    Eco,
    Error,
    FanOnly,
    Far,
    Hans,
    Heat,
    HeatCool,
    Home,
    Idle,
    Initializing,
    Locked,
    None,
    NotHome,
    Notifying,
    Off,
    Ok,
    On,
    Open,
    Opening,
    Paused,
    Pending,
    Playing,
    PriorityOnly,
    Problem,
    Ready,
    Recording,
    Returning,
    Sleeping,
    Standby,
    Still,
    Stopped,
    Streaming,
    Triggered,
    Unlocked,
    Vibrate,
    Zoning,
}


[JsonConverter(typeof(KebabJsonStringEnumConverter<KnowWeatherStates>))]
[PublicAPI]
public enum KnowWeatherStates
{
    [EnumMember(Value = "clear-night")] ClearNight,
    Cloudy,
    Exceptional,
    Fog,
    Hail,
    Lightning,
    [EnumMember(Value = "lightning-rainy")]
    LightningRainy,
    // ReSharper disable once StringLiteralTypo
    [EnumMember(Value = "partlycloudy")]
    PartlyCloudy,
    Pouring,
    Rainy,
    Snowy,
    [EnumMember(Value = "snowy-rainy")] SnowyRainy,
    Sunny,
    Windy,
    [EnumMember(Value = "windy-variant")] WindyVariant
}

public class KebabJsonStringEnumConverter<TEnum>() :
    JsonStringEnumConverter<TEnum>(JsonNamingPolicy.KebabCaseLower) where TEnum : struct, Enum;

public class SnakeJsonStringEnumConverter<TEnum>() :
    JsonStringEnumConverter<TEnum>(JsonNamingPolicy.SnakeCaseLower) where TEnum : struct, Enum;