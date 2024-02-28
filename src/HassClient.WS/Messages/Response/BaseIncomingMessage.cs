namespace HassClient.WS.Messages.Response
{
    /// <summary>
    /// Represents an identifiable incoming message (any but authentication messages).
    /// </summary>
    internal abstract class BaseIncomingMessage : BaseIdentifiableMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseIncomingMessage"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc/></param>
        protected BaseIncomingMessage(string type)
            : base(type)
        {
        }
    }
}
