using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.Core.Models
{
    public class HassDomainServices : Dictionary<string, Service>;
    
    /// <summary>
    /// Represents a single service definition.
    /// </summary>
    public class Service
    {
        [JsonProperty("name")] public string? Name { get; set; }

        [JsonProperty("description")] public string? Description { get; set; }

        [JsonProperty("target")] public JRaw? Target { get; set; }

        [JsonProperty("fields")] public Dictionary<string, ServiceField> Fields { get; set; } = [];

        [JsonProperty("response")] public Response? Response { get; set; }
    }
    
    /// <summary>
    /// Represents a single field in a service call.
    /// </summary>
    public class ServiceField
    {
        [JsonProperty("example")] public JRaw? Example { get; set; }

        [JsonProperty("default")] public JRaw? Default { get; set; }

        [JsonProperty("required")] public bool? Required { get; set; }

        [JsonProperty("advanced")] public bool? Advanced { get; set; }

        [JsonProperty("selector")] public JRaw? Selector { get; set; }

        [JsonProperty("filter")] public Filter? Filter { get; set; }

        [JsonProperty("name")] public string? Name { get; set; }

        [JsonProperty("description")] public string? Description { get; set; }
    }
    
    
    public class Filter
    {
        [JsonProperty("attribute")] public Dictionary<string, JRaw> Attributes { get; set; } = [];

        [JsonProperty("supported_features")] public int[] SupportedFeatures { get; set; } = [];
    }

    public class Response
    {
        [JsonProperty("optional")] public bool? Optional { get; set; }
    }
}
