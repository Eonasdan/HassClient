using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HassClient.Core.API;

public class JsonClient : IDisposable
{
    protected readonly HttpClient HttpClient;

    // ReSharper disable once MemberCanBeProtected.Global
    public JsonClient(IHttpClientFactory factory, string? clientName = null)
    {
        HttpClient = clientName is null ? factory.CreateClient() : factory.CreateClient(clientName);
        HttpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> GetAsync(string url)
    {
        var responseMessage = await HttpClient.GetAsync(url);
        return await VerifySuccessAsync(responseMessage);
    }

    public async Task<T?> GetAsync<T>(string url) where T : class
    {
        if (!typeof(T).IsArray || !typeof(T).GetElementType()!.IsAssignableFrom(typeof(byte)))
            return JsonSerializer.Deserialize<T>(await GetAsync(url));

        var responseMessage = await HttpClient.GetAsync(url);
        return await VerifySuccessByteArrayAsync(responseMessage) as T;
    }

    public async Task<string> PutAsync(string url, HttpContent content)
    {
        var responseMessage = await HttpClient.PutAsync(url, content);
        return await VerifySuccessAsync(responseMessage);
    }

    public async Task<string> PostAsync(string url, HttpContent? content)
    {
        var responseMessage = await HttpClient.PostAsync(url, content);
        return await VerifySuccessAsync(responseMessage);
    }

    public async Task<T?> PostAsync<T>(string url, HttpContent? content)
    {
        return JsonSerializer.Deserialize<T>(await PostAsync(url, content));
    }

    public async Task<T?> PutAsync<T>(string url, HttpContent content)
    {
        return JsonSerializer.Deserialize<T>(await PutAsync(url, content));
    }

    public async Task<bool> DeleteAsync(string url)
    {
        var responseMessage = await HttpClient.DeleteAsync(url);
        return responseMessage.IsSuccessStatusCode;
    }

    public static string ToJson(object? value)
    {
        return value is null ? "" : JsonSerializer.Serialize(value);
    }

    public StringContent ToStringContent(object? value)
    {
        return value is null ?
            new StringContent("") :
            new StringContent(ToJson(value), Encoding.UTF8, "application/json");
    }

    public StringContent ToStringContent(string? value)
    {
        return new StringContent(value, Encoding.UTF8, "application/json");
    }

    private static async Task<string> VerifySuccessAsync(HttpResponseMessage responseMessage)
    {
        if (responseMessage.Content == null) throw new SimpleHttpResponseException(responseMessage.StatusCode, "No content");

        var content = await responseMessage.Content.ReadAsStringAsync();
        if (responseMessage.IsSuccessStatusCode) return content;

        responseMessage.Content.Dispose();

        throw new SimpleHttpResponseException(responseMessage.StatusCode, content);
    }

    private static async Task<byte[]> VerifySuccessByteArrayAsync(HttpResponseMessage responseMessage)
    {
        if (responseMessage.Content == null) throw new SimpleHttpResponseException(responseMessage.StatusCode, "No content");

        var content = await responseMessage.Content.ReadAsByteArrayAsync();
        if (responseMessage.IsSuccessStatusCode) return content;

        responseMessage.Content.Dispose();

        throw new SimpleHttpResponseException(responseMessage.StatusCode, "Failed to read byte array");
    }


    public void Dispose()
    {
        HttpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}

public class SimpleHttpResponseException(HttpStatusCode statusCode, string content) : Exception(content)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
