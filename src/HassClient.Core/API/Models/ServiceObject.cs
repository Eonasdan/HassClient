using System.Collections.Generic;
using JetBrains.Annotations;

namespace HassClient.Core.API.Models;

/// <summary>
/// Represents a single service definition.
/// </summary>
[PublicAPI]
public class ServiceObject
{
    /// <summary>
    /// The description of the service object.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The fields/parameters that the service supports.
    /// </summary>
    public Dictionary<string, ServiceField> Fields { get; set; } = [];
}