﻿namespace HassClient.WS.Messages.Commands
{
    internal class GetStatesMessage : BaseOutgoingMessage
    {
        public GetStatesMessage()
            : base("get_states")
        {
        }
    }
}
