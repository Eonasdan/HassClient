﻿namespace HassClient.WS.Messages.Response
{
    internal class TemplateEventInfo
    {
        public string? Result { get; set; }

        public ListenersTemplateInfo Listeners { get; set; }
    }
}
