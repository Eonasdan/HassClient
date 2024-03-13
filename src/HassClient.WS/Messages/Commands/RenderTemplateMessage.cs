using System.Threading.Tasks;
using HassClient.WS.Messages.Response;
using Newtonsoft.Json;

namespace HassClient.WS.Messages.Commands
{
    internal class RenderTemplateMessage : BaseOutgoingMessage
    {
        private readonly TaskCompletionSource<string?> _templateEventReceivedTcs;

        [System.Text.Json.Serialization.JsonIgnore]
        public Task<string?> WaitResponseTask => _templateEventReceivedTcs.Task;

        [JsonProperty(Required = Required.Always)]
        public string? Template { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] EntitiesIds { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Variables { get; set; }

        public RenderTemplateMessage()
            : base("render_template")
        {
            _templateEventReceivedTcs = new TaskCompletionSource<string?>();
        }

        public void ProcessEventReceivedMessage(EventResultMessage eventResultMessage)
        {
            var templateEventInfo = eventResultMessage.DeserializeEvent<TemplateEventInfo>();
            _templateEventReceivedTcs.SetResult(templateEventInfo.Result);
        }
    }
}
