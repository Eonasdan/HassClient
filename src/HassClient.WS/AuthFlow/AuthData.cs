using System.Text.Json.Serialization;

namespace HassClient.WS.AuthFlow;

public class AuthData
{
    [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
    [JsonPropertyName("token_type")] public string? TokenType { get; set; }
    [JsonPropertyName("refresh_token")] public string? RefreshToken { get; set; }
    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
    [JsonPropertyName("ha_auth_provider")] public string? HaAuthProvider { get; set; }
    public long Expires { get; set; }
}