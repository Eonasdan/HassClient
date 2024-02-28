namespace HassClient.WS.Messages.Authentication
{
    /// <summary>
    /// Represents an authentication message used by Web Socket API.
    /// </summary>
    internal class AuthenticationOkMessage : BaseMessage
    {
        public string HaVersion { get; set; }

        public AuthenticationOkMessage()
            : base("auth_ok")
        {
        }
    }
}
