using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using HassClient.Core.Models.KnownEnums;
using HassClient.WS.Extensions;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace HassClient.WS.WellKnownServices;

[PublicAPI]
public static class ConversationServices
{
    public static async Task<SpeechResult?> SpeakAsync(IHomeAssistantClientWrapper wrapper, string text)
    {
        return await wrapper.WebSocketClient.CallServiceAsync<SpeechResult>("conversation", "process", data: new { text });
    }

    public class SpeechResult
    {
        [JsonProperty("response")] public Response Response { get; init; } = default!;

        [JsonProperty("conversation_id")]
        public string? ConversationId { get; init; }
    }

    public class Response
    {
        [JsonProperty("speech")]
        public Speech? Speech { get; init; }

        [JsonProperty("card")]
        public Card? Card { get; init; }

        [JsonProperty("language")]
        public string? Language { get; init; }

        [JsonProperty("response_type")]
        public SpeechResponseType ResponseType { get; init; }

        [JsonProperty("data")]
        public Data? Data { get; init; }
    }

    [JsonConverter(typeof(SnakeJsonStringEnumConverter<SpeechResponseType>))]
    public enum SpeechResponseType
    {
        [EnumMember(Value = "action_done")]
        ActionDone,
        [EnumMember(Value = "query_answer")]
        QueryAnswer,
        Error
    }

    public class Card;

    public class Data
    {
        [JsonProperty("targets")]
        public List<ResponseData> Targets { get; init; } = [];

        [JsonProperty("success")]
        public List<ResponseData> Success { get; init; } = [];

        [JsonProperty("failed")]
        public List<ResponseData> Failed { get; init; } = [];

        [JsonProperty("code")]
        public SpeechResponseErrorCode? Code { get; init; }
    }

    [JsonConverter(typeof(SnakeJsonStringEnumConverter<SpeechResponseErrorCode>))]
    public enum SpeechResponseErrorCode
    {
        /// <summary>
        /// The input text did not match any intents
        /// </summary>
        [EnumMember(Value = "no_intent_match")]
        NoIntentMatch,
        /// <summary>
        /// The targeted area, device, or entity does not exist.
        /// </summary>
        [EnumMember(Value = "no_valid_targets")]
        NoValidTargets,
        /// <summary>
        /// An unexpected error occurred while handling the intent
        /// </summary>
        [EnumMember(Value = "failed_to_handle")]
        FailedToHandle,
        /// <summary>
        /// An error occurred outside the scope of intent processing.
        /// </summary>
        Unknown
    }

    public class ResponseData
    {
        [JsonProperty("name")]
        public string? Name { get; init; }

        [JsonProperty("type")]
        public string? Type { get; init; }

        [JsonProperty("id")]
        public string? Id { get; init; }
    }

    public class Speech
    {
        [JsonProperty("plain")]
        public SpeechResponse? Plain { get; init; }

        [JsonProperty("ssml")]
        public SpeechResponse? SSML { get; init; }
    }

    public class SpeechResponse
    {
        [JsonProperty("speech")]
        public string? SpeechText { get; init; }

        [JsonProperty("extra_data")]
        public object? ExtraData { get; init; }
    }
}