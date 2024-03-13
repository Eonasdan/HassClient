﻿using System;
using System.Collections.Generic;
using HassClient.Core.Models.RegistryEntries.Modifiable;
using System.Text.Json.Serialization;

namespace HassClient.Core.Models.RegistryEntries
{
    /// <summary>
    /// Represents an area.
    /// </summary>
    public class Area : RegistryEntryBase
    {
        private readonly ModifiableProperty<string?> _name = new(nameof(Name));

        private readonly ModifiableProperty<string?> _picture = new(nameof(Picture));

        /// <inheritdoc />
        protected internal override string? UniqueId
        {
            get => Id;
            protected set => Id = value;
        }

        /// <summary>
        /// Gets the ID of this area.
        /// </summary>
        [JsonPropertyName("area_id")]
        public string? Id { get; private set; }

        /// <summary>
        /// Gets or sets the name of this area.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name
        {
            get => _name.Value;
            init
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidOperationException($"'{nameof(Name)}' cannot be null or whitespace.");
                }

                _name.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets a URL (relative or absolute) to a picture for this area.
        /// </summary>
        [JsonPropertyName("picture")]
        public string? Picture
        {
            get => _picture.Value;
            init => _picture.Value = value;
        }

        [JsonConstructor]
        private Area()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Area"/> class.
        /// </summary>
        /// <param name="name">The name of the area.</param>
        /// <param name="picture">a URL (relative or absolute) to a picture for this area.</param>
        public Area(string name, string? picture = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));
            }

            Name = name;
            Picture = picture;
        }

        // Used for testing purposes.
        internal static Area CreateUnmodified(string name, string picture)
        {
            var result = new Area(name, picture);
            result.SaveChanges();
            return result;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            yield return _name;
            yield return _picture;
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Area)}: {Name}";

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Area area &&
                   Id == area.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
        }

        // Used for testing purposes.
        internal Area Clone()
        {
            var result = CreateUnmodified(Name!, Picture!);
            result.UniqueId = UniqueId;
            return result;
        }
    }
}
