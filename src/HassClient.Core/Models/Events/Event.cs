﻿using System.Text.Json.Serialization;

namespace HassClient.Core.Models.Events
{
    /// <summary>
    /// Represents an event definition in Home Assistant.
    /// </summary>
    public class Event
    {
        internal const string AnyEventFilter = "*";

        /// <summary>
        /// Gets the event's name.
        /// </summary>
        [JsonPropertyName("event")]
        public string? Name { get; internal set; }

        /// <summary>
        /// Gets the listener count for this event.
        /// </summary>
        public int ListenerCount { get; internal set; }

        /// <inheritdoc />
        public override string ToString() => $"Event: {Name} ({ListenerCount} listener(s))";
    }
}
