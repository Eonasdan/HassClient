﻿namespace HassClient.WS.Messages.Authentication;

/// <summary>
/// Represents an authentication message used by Web Socket API.
/// </summary>
internal class AuthenticationMessage : BaseMessage
{
    public string? AccessToken { get; set; }

    public AuthenticationMessage()
        : base("auth")
    {
    }
}