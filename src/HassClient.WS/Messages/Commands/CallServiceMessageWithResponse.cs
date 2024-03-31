using Newtonsoft.Json;

namespace HassClient.WS.Messages.Commands;

internal class CallServiceMessageWithResponse(string domain, string service, object? serviceData) :
    CallServiceMessage(domain, service, serviceData)
{
    [JsonProperty("return_response"), JsonRequired]
    public bool ReturnResponse { get; private set; } = true;
}