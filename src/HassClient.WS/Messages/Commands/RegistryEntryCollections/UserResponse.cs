using System.Text.Json.Serialization;

namespace HassClient.WS.Messages.Commands.RegistryEntryCollections;

internal class UserResponse
{
    [JsonPropertyName("user")]
    public JRaw UserRaw { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{UserRaw}";
}