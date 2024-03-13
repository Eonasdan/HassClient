namespace HassClient.WS.Messages.Authentication
{
    /// <summary>
    /// Represents an authentication message used by Web Socket API.
    /// </summary>
    internal class AuthenticationRequiredMessage : BaseMessage
    {
        public string? HaVersion { get; set; }

        public AuthenticationRequiredMessage()
            : base("auth_required")
        {
        }
    }
}
