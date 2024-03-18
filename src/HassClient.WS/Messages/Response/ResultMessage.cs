using HassClient.Core.Serialization;

namespace HassClient.WS.Messages.Response;

internal class ResultMessage : BaseIncomingMessage
{
    [JsonProperty(Required = Required.Always)]
    public bool Success { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public JRaw? Result { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ErrorInfo? Error { get; set; }

    public ResultMessage()
        : base("result")
    {
        }

    public T? DeserializeResult<T>()
    {
            return Result != null ? HassSerializer.DeserializeObject<T>(Result) : default;
        }

    public void PopulateResult(object? target)
    {
            if (Result != null) HassSerializer.PopulateObject(Result, target);
        }
}