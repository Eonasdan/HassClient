namespace HassClient.WS.Messages.Commands
{
    internal class PingMessage : BaseOutgoingMessage
    {
        public PingMessage()
            : base("ping")
        {
        }
    }
}
