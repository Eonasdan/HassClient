namespace HassClient.WS.Messages.Commands
{
    internal class GetConfigMessage : BaseOutgoingMessage
    {
        public GetConfigMessage()
            : base("get_config")
        {
        }
    }
}
