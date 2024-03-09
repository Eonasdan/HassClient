using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace HassClient.WS.AuthFlow;

public interface IHomeAssistantAuthService
{
    string? AccessToken { get; }
    bool Expired { get; }

    /// <summary>
    /// Generates a URI to redirect the user to so they can sign in to HA
    /// </summary>
    /// <param name="redirectUri"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    string GenerateAuthUri(string redirectUri);

    /// <summary>
    /// Takes a querystring and gets an auth token.
    /// This should be done after the user is successfully returned from HA
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    Task<AuthData> GetTokenFromQueryCodeAsync(string query);

    Task<AuthData> RefreshAccessTokenAsync();
    Task Revoke();
    Task Revoke(string hassUrl, string refreshToken);
    void LoadData(AuthData getItemAsync);
}

public class HomeAssistantAuthService(HttpClient httpClient, IConfiguration configuration) : IHomeAssistantAuthService
{
    private readonly HomeAssistantConfiguration? _configuration = HomeAssistantConfiguration.FromConfig(configuration);

    private static long GenExpires(int expiresIn) => DateTimeOffset.Now.AddSeconds(expiresIn).ToUnixTimeMilliseconds();

    private AuthData? Data { get; set; }

    public string? AccessToken => Data?.AccessToken;

    public bool Expired => Data != null && DateTime.Now > DateTimeOffset.FromUnixTimeMilliseconds(Data.Expires);
    
    /// <summary>
    /// Generates a URI to redirect the user to so they can sign in to HA
    /// </summary>
    /// <param name="redirectUri"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public string GenerateAuthUri(string redirectUri)
    {
        if (_configuration == null) throw new Exception("Invalid configuration");

        var state = EncodeOAuthState(new OAuthState
        {
            HassUrl = _configuration.Uri!,
            ClientId = _configuration.ClientId!
        });
        redirectUri += (redirectUri.Contains('?') ? "&" : "?") + "auth_callback=1";

        var authorizeUrl =
            $"{_configuration.Uri}/auth/authorize?response_type=code&redirect_uri={Uri.EscapeDataString(redirectUri)}";

        if (!string.IsNullOrEmpty(_configuration.ClientId))
            authorizeUrl += $"&client_id={Uri.EscapeDataString(_configuration.ClientId)}";

        if (!string.IsNullOrEmpty(state))
            authorizeUrl += $"&state={Uri.EscapeDataString(state)}";

        return authorizeUrl;
    }

    /// <summary>
    /// Takes a querystring and gets an auth token.
    /// This should be done after the user is successfully returned from HA
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<AuthData> GetTokenFromQueryCodeAsync(string query)
    {
        if (_configuration == null) throw new Exception("Invalid configuration");
        if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query), "QueryString can't be empty");
        
        var queryString = HttpUtility.ParseQueryString(query);
        var stateString = queryString["state"];
        var code = queryString["code"];
        
        if (string.IsNullOrEmpty(stateString) || string.IsNullOrEmpty(code)) 
            throw new Exception("No state or code is missing");
        
        var state = DecodeOAuthState(stateString);

        if (state is null) throw new Exception("Failed to decode state");
        
        if (_configuration.LimitHassInstance && (state.HassUrl != _configuration.Uri || state.ClientId != _configuration.ClientId))
            throw new Exception("Invalid Auth Callback");

        Data = await TokenRequest(state.HassUrl, state.ClientId, new AuthorizationCodeRequest
        {
            Code = code,
            GrantType = "authorization_code"
        });
        return Data;
    }
    
//todo maybe the load/save funcs would be better?
    public async Task<AuthData> RefreshAccessTokenAsync()
    {
        if (Data == null || string.IsNullOrEmpty(Data.RefreshToken))
            throw new Exception("No refresh token");

        var newData = await TokenRequest(_configuration!.Uri!, _configuration.ClientId!, new RefreshTokenRequest
        {
            GrantType = "refresh_token",
            RefreshToken = Data.RefreshToken
        });

        newData.RefreshToken = Data.RefreshToken;
        Data = newData;
        return Data;
    }

    public async Task Revoke()
    {
        if (string.IsNullOrEmpty(Data?.RefreshToken))
            throw new Exception("No refresh_token to revoke");

        var formData = new Dictionary<string, string>
        {
            { "token", Data.RefreshToken }
        };

        await httpClient.PostAsync($"{_configuration.Uri}/auth/revoke", new FormUrlEncodedContent(formData));
    }
    
    public async Task Revoke(string hassUrl, string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            throw new Exception("No refresh_token to revoke");

        var formData = new Dictionary<string, string>
        {
            { "token", refreshToken }
        };

        var response = await httpClient.PostAsync($"{hassUrl}/auth/revoke", new FormUrlEncodedContent(formData));
    }

    public void LoadData(AuthData authData)
    {
        Data = authData;
    }

    private async Task<AuthData> TokenRequest(string hassUrl, string clientId, object data)
    {
        var formData = new Dictionary<string, string?>();
        if (!string.IsNullOrEmpty(clientId))
            formData["client_id"] = clientId;

        var properties = data.GetType().GetProperties();
        foreach (var prop in properties)
        {
            var jsonPropertyNameAttribute = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
            formData[jsonPropertyNameAttribute?.Name ?? prop.Name] = prop.GetValue(data)?.ToString();
        }

        var response = await httpClient.PostAsync($"{hassUrl}/auth/token", new FormUrlEncodedContent(formData));

        if (!response.IsSuccessStatusCode)
        {
            throw response.StatusCode is System.Net.HttpStatusCode.BadRequest or System.Net.HttpStatusCode.Forbidden
                ? new Exception("Invalid Auth")
                : new Exception("Unable to fetch tokens");
        }

        var tokens = await response.Content.ReadFromJsonAsync<AuthData>();
        if (tokens == null) throw new Exception("Reading token failed");
        tokens.Expires = GenExpires(tokens.ExpiresIn);
        return tokens;
    }

    private static string EncodeOAuthState(OAuthState state) =>
        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(state)));

    private static OAuthState? DecodeOAuthState(string encoded) =>
        JsonSerializer.Deserialize<OAuthState>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded)));
    
}