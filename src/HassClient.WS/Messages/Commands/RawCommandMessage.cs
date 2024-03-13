﻿using Newtonsoft.Json;

namespace HassClient.WS.Messages.Commands
{
    /// <summary>
    /// Outgoing message used for raw commands.
    /// </summary>
    [JsonObject(MissingMemberHandling = MissingMemberHandling.Ignore)]
    public class RawCommandMessage : BaseOutgoingMessage
    {
        /// <summary>
        /// Object containing additional fields that will be merged to the base message.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public object MergedObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawCommandMessage"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc/></param>
        /// <param name="mergedObject">Object containing additional fields that will be merged to the base message.</param>
        public RawCommandMessage(string type, object mergedObject = null)
            : base(type)
        {
            MergedObject = mergedObject;
        }
    }
}
