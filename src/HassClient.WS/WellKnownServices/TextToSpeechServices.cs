using System.Threading.Tasks;
using HassClient.WS.Extensions;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace HassClient.WS.WellKnownServices;

/// <summary>
/// Helper methods for the TTS services.
/// </summary>
[PublicAPI]
public static class TextToSpeechServices
{
    /// <summary>
    /// Speaks something using text-to-speech on a media player.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="speakData"></param>
    public static async Task Speak(HassClientWebSocket client, SpeakData speakData)
    {
        await client.CallServiceAsync("tts", "speak",
            data: speakData
        );
    }

    /// <summary>
    /// Removes all cached text-to-speech files and purges the memory.
    /// </summary>
    /// <param name="client"></param>
    public static async Task ClearCache(HassClientWebSocket client)
    {
        await client.CallServiceAsync("tts", "clear_cache");
    }

    /// <summary>
    /// Say something using text-to-speech on a media player with cloud.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="speakData"></param>
    public static async Task CloudSay(HassClientWebSocket client, CloudSpeakData speakData)
    {
        await client.CallServiceAsync("tts", "cloud_say", data: speakData);
    }

    /// <summary>
    /// Speaks something using text-to-speech on a media player.
    /// </summary>
    [PublicAPI]
    public class SpeakData : SpeakDataBase
    {
        /// <summary>
        /// Entity to use to generate the TTS. Defaults to "tts.google_en_com"
        /// </summary>
        [JsonProperty("entity_id"), JsonRequired]
        public string EntityId { get; set; } = "tts.google_en_com";

        /// <summary>
        /// Media players to play the message.
        /// </summary>
        [JsonProperty("media_player_entity_id"), JsonRequired]
        public string MediaPlayerEntityId { get; set; } = default!;
    }

    /// <summary>
    /// Say something using text-to-speech on a media player with cloud.
    /// </summary>
    [PublicAPI]
    public class CloudSpeakData : SpeakDataBase
    {
        /// <summary>
        /// Media players to play the message.
        /// </summary>
        [JsonProperty("entity_id"), JsonRequired]
        public string MediaPlayerEntityId { get; set; } = default!;
    }

    [PublicAPI]
    public class SpeakDataBase
    {
        /// <summary>
        /// The text you want to convert into speech so that you can listen to it on your device.
        /// </summary>
        [JsonProperty("message"), JsonRequired]
        public string Message { get; set; } = default!;

        /// <summary>
        /// Stores this message locally so that when the text is requested again, the output can be produced more quickly.
        /// </summary>
        [JsonProperty("cache")]
        public bool Cache { get; set; }

        /// <summary>
        /// Language to use for speech generation.
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string? Language { get; set; }

        /// <summary>
        /// A dictionary containing integration-specific options.
        /// </summary>
        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public object? Options { get; set; }
    }
}