namespace HassClient.WS.Messages.Commands.Search;

internal class SearchRelatedMessage : BaseOutgoingMessage
{
    [JsonProperty(Required = Required.Always)]
    public ItemTypes ItemType { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string? ItemId { get; set; }

    public SearchRelatedMessage()
        : base("search/related")
    {
    }

    public SearchRelatedMessage(ItemTypes itemType, string itemId)
        : this()
    {
        ItemType = itemType;
        ItemId = itemId;
    }
}