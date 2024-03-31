using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HassClient.Core.API.Models;
//todo this is redundant with HassClient.Core.Models, however one is text.json
/// <summary>
/// Represents an entity state's context.
/// </summary>
[PublicAPI]
public class StateContext
{
    /// <summary>
    /// Gets or sets the ID of this context.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the Parent Context ID if this element is a child of another context, otherwise <see langword="null" />.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public string? ParentId { get; set; }
        
    /// <summary>
    /// Gets or sets the User ID of this element, or <see langword="null" /> for the default user or no user.
    /// </summary>
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    /// <summary>
    /// Gets a string representation of this object.
    /// </summary>
    public override string ToString() => $"Context: {Id}{(!string.IsNullOrWhiteSpace(ParentId) ? " / Parent: " + ParentId : "")}";
}