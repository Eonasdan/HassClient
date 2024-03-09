using Newtonsoft.Json;

namespace HassClient.WS.Messages.Commands
{
    internal class CallServiceMessage() : BaseOutgoingMessage("call_service")
    {
        [JsonProperty(Required = Required.Always)]
        public string Domain { get; set; } = null!;

        [JsonProperty(Required = Required.Always)]
        public string Service { get; set; } = null!;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object? ServiceData { get; set; }

        public CallServiceMessage(string domain, string service, object? serviceData)
            : this()
        {
            Domain = domain;
            Service = service;
            ServiceData = serviceData;
        }
    }
}
