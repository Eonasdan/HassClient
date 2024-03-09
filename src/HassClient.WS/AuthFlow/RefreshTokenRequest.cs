using System.Text.Json.Serialization;

namespace HassClient.WS.AuthFlow;

public class RefreshTokenRequest
{
    [JsonPropertyName("grant_type")] public string GrantType { get; set; } = "refresh_token";
    [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }
}