using Newtonsoft.Json;

namespace HassClient.Core.Models
{
    /// <summary>
    /// Represents an entity state's context.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Gets the ID of this context.
        /// </summary>
        [JsonPropertyName("Id")]
        public string? Id { get; init; }

        /// <summary>
        /// Gets the Parent Context ID if this element is a child of another context, otherwise <see langword="null" />.
        /// </summary>
        [JsonPropertyName("ParentId")]
        public string? ParentId { get; init; }

        /// <summary>
        /// Gets the User ID of this element, or <see langword="null" /> for the default user or no user.
        /// </summary>
        [JsonPropertyName("UserId")]
        public string? UserId { get; init; }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Context)}: {Id}{(!string.IsNullOrWhiteSpace(ParentId) ? " / Parent: " + ParentId : string.Empty)}";
    }
}
