using HassClient.Core.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Messages.Response
{
    internal class ResultMessage : BaseIncomingMessage
    {
        [JsonProperty(Required = Required.Always)]
        public bool Success { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JRaw Result { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ErrorInfo Error { get; set; }

        public ResultMessage()
            : base("result")
        {
        }

        public T DeserializeResult<T>()
        {
            return HassSerializer.DeserializeObject<T>(Result);
        }

        public void PopulateResult(object target)
        {
            HassSerializer.PopulateObject(Result, target);
        }
    }
}
