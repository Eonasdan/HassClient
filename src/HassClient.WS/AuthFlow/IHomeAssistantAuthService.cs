using System;
using System.Threading.Tasks;

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