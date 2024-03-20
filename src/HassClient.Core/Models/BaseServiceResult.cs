using Newtonsoft.Json;

namespace HassClient.Core.Models;

public class BaseResult
{
    [JsonProperty("context")]
    public Context? Context { get; set; }
}

public class BaseResultResponse<T> : BaseResult
{
    [JsonProperty("response")]
    public T? Response { get; set; }
}