using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Models
{
    /// <summary>
    /// Represents a logbook entry object. The only consistently available properties are <see cref="Timestamp" />, <see cref="Name" />, and <see cref="EntityId" />.
    /// </summary>
    [PublicAPI]
    public class LogbookObject
    {
        /// <summary>
        /// Gets or sets the entity ID for this entry.
        /// </summary>
        [JsonPropertyName("entity_id")]
        public string? EntityId { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        [JsonPropertyName("domain")]
        public string? Domain { get; set; }

        /// <summary>
        /// Gets or sets the context user ID associated with this entry.
        /// </summary>
        [JsonPropertyName("context_user_id")]
        public string? ContextUserId { get; set; }

        /// <summary>
        /// Gets or sets the context entity ID associated with this entry. (For example, which automation triggered this change?)
        /// </summary>
        [JsonPropertyName("context_entity_id")]
        public string? ContextEntityId { get; set; }

        /// <summary>
        /// Gets or sets the friendly name for the <see cref="ContextEntityId" />. This is sometimes the same as <see cref="ContextName" />.
        /// </summary>
        [JsonPropertyName("context_entity_id_name")]
        public string? ContextEntityIdName { get; set; }

        /// <summary>
        /// Gets or sets the type of associated change, for example, if an automation triggered this event, the value is 'automation_triggered'.
        /// </summary>
        [JsonPropertyName("context_event_type")]
        public string? ContextEventType { get; set; }

        /// <summary>
        /// Gets or sets the domain associated with the context entity. For example, if an automation triggered this change, the value is 'automation'.
        /// </summary>
        [JsonPropertyName("context_domain")]
        public string? ContextDomain { get; set; }

        /// <summary>
        /// Gets or sets the name associated with the context entity (e.g. the name of the automation). This is sometimes the same as <see cref="ContextEntityIdName" />.
        /// </summary>
        [JsonPropertyName("context_name")]
        public string? ContextName { get; set; }

        /// <summary>
        /// Gets or sets the new state that the entity transitioned to.
        /// </summary>
        [JsonPropertyName("state")]
        public string? State { get; set; }

        /// <summary>
        /// Gets or sets the source for this logbook entry (i.e. what triggered this change).
        /// </summary>
        [JsonPropertyName("source")]
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets the message, a brief description of what happened.
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the category name for this entry.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when this entry occurred..
        /// </summary>
        [JsonPropertyName("when")]
        public DateTimeOffset Timestamp { get; set; }
    }
}
