using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HassClient.Core.Models.RegistryEntries;

/// <summary>
/// Represents a Home Assistant user.
/// </summary>
[PublicAPI]
public class User
{
    /// <summary>
    /// The System Administrator group id constant.
    /// </summary>
    public const string SysadminGroupId = "system-admin";

    [JsonPropertyName("id")] public string Id { get; init; } = default!;

    [JsonPropertyName("username")] public string Username { get; init; } = default!;

    [JsonPropertyName("name"), ModifiableProperty, JsonRequired]
    public string Name { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user is owner of the system. In this case, the user will have full access to everything.
    /// </summary>
    [JsonPropertyName("is_owner")]
    public bool IsOwner { get; init; }

    [JsonPropertyName("is_active"), ModifiableProperty]
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is only allowed to log in from the local network and not from the internet or cloud.
    /// </summary>
    [JsonPropertyName("local_only"), ModifiableProperty]
    public bool LocalOnly { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user has been generated automatically by the system.
    /// </summary>
    [JsonPropertyName("system_generated")]
    public bool SystemGenerated { get; init; }

    [JsonPropertyName("group_ids"), ModifiableProperty]
    public List<string> GroupIds { get; init; } = [];

    [JsonPropertyName("credentials")] public List<Credential> Credentials { get; init; } = [];

    [JsonIgnore]
    public bool IsAdministrator
    {
        get => GroupIds.Contains(SysadminGroupId);
        set
        {
            if (value && !IsAdministrator)
            {
                GroupIds.Add(SysadminGroupId);
            }
            else if (IsAdministrator)
            {
                GroupIds.Remove(SysadminGroupId);
            }
        }
    }

    public User(string name, IEnumerable<string>? groupIds = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));
        }

        Name = name;
        if (groupIds == null) return;

        foreach (var item in groupIds)
        {
            GroupIds.Add(item);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="name">The name of the user.</param>
    /// <param name="isAdministrator">A value indicating is the user will be included in the <see cref="SysadminGroupId"/>.</param>
    public User(string name, bool isAdministrator)
        : this(name)
    {
        IsAdministrator = isAdministrator;
    }

    public override string ToString() => $"{nameof(User)}: {Name}";

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is User user &&
               Id == user.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
    }

    [PublicAPI]
    public class Credential
    {
        [JsonPropertyName("type")] public string? Type { get; init; }
    }
}