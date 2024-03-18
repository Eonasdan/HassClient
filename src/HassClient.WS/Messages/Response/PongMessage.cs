namespace HassClient.WS.Messages.Response;

internal class PongMessage : BaseIncomingMessage
{
    public PongMessage()
        : base("pong")
    {
        }
}