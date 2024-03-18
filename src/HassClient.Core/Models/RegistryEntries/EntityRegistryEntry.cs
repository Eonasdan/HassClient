using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HassClient.Core.Models.RegistryEntries;

/// <summary>
/// The Entity Registry keeps a registry of entities. Entities are uniquely identified by their domain, platform and
/// an unique id provided by that platform.
/// </summary>
[PublicAPI]
public class EntityRegistryEntry : EntityRegistryEntryBase
{
    [JsonPropertyName("area_id")] public string? AreaId { get; set; }

    [JsonPropertyName("config_entry_id")] public string? ConfigEntryId { get; set; }

    [JsonPropertyName("device_id")] public string? DeviceId { get; set; }

    [JsonPropertyName("disabled_by"), ModifiableProperty]
    public DisabledByEnum? DisabledBy { get; set; } = DisabledByEnum.None;

    [JsonPropertyName("entity_category")]
    public EntityCategory? EntityCategory { get; set; } = RegistryEntries.EntityCategory.None;

    [JsonPropertyName("has_entity_name")] public bool HasEntityName { get; set; }

    [JsonPropertyName("hidden_by")] public HiddenBy HiddenBy { get; set; }

    [JsonPropertyName("id")] public string? Id { get; set; }

    [JsonPropertyName("options")] public Options? Options { get; set; }

    [JsonPropertyName("original_name")] public string? OriginalName { get; set; }

    [JsonPropertyName("platform")] public string? Platform { get; set; }

    [JsonPropertyName("translation_key")] public string? TranslationKey { get; set; }
    
    public override string ToString() => $"{nameof(EntityRegistryEntry)}: {EntityId}";
}

[PublicAPI]
public class Options
{
    [JsonPropertyName("conversation")] public Conversation? Conversation { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("sensor")]
    public Sensor? Sensor { get; set; }
}

[PublicAPI]
public class Conversation
{
    [JsonPropertyName("should_expose")] public bool ShouldExpose { get; set; }
}

[PublicAPI]
public class Sensor
{
    [JsonPropertyName("suggested_display_precision")]
    public long SuggestedDisplayPrecision { get; set; }
}