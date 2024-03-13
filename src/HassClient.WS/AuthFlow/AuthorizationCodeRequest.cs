using System.Text.Json.Serialization;

namespace HassClient.WS.AuthFlow;

public class AuthorizationCodeRequest
{
    [JsonPropertyName("grant_type")] public string? GrantType { get; set; } = "authorization_code";
    [JsonPropertyName("code")] public string? Code { get; set; }
}