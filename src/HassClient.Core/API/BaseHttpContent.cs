using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace HassClient.Core.API;

public abstract class BaseHttpContent
{
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public StringContent ToJsonStringContent()
    {
        return new StringContent(ToJson(), Encoding.UTF8, "application/json");
    }

    public static T? FromJson<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }

}
