using System;
using System.Collections.Generic;
using HassClient.Core.Helpers;
using HassClient.Core.Models.KnownEnums;
using Newtonsoft.Json;

namespace HassClient.Core.Models.RegistryEntries.StorageEntities
{
    /// <summary>
    /// Represents an input boolean.
    /// </summary>
    public abstract class StorageEntityRegistryEntryBase : EntityRegistryEntryBase
    {
        private readonly KnownDomains _domain;

        /// <inheritdoc />
        protected internal override string UniqueId
        {
            get => Id;
            set => Id = value;
        }

        /// <summary>
        /// Gets the entity identifier of the entity entry.
        /// </summary>
        [JsonProperty]
        public string Id { get; protected set; }

        /// <inheritdoc />
        public override string EntityId => $"{_domain.ToDomainString()}.{UniqueId}";

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageEntityRegistryEntryBase"/> class.
        /// </summary>
        protected StorageEntityRegistryEntryBase()
        {
            _domain = GetDomain(GetType());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageEntityRegistryEntryBase"/> class.
        /// </summary>
        /// <param name="name">The entity name.</param>
        /// <param name="icon">The entity icon.</param>
        protected StorageEntityRegistryEntryBase(string name, string icon)
            : base(name, icon)
        {
            _domain = GetDomain(GetType());
        }

        /// <inheritdoc />
        public override string ToString() => $"{_domain}: {Name}";

        private static readonly Dictionary<Type, KnownDomains> DomainsByType = new();

        internal static KnownDomains GetDomain<T>()
            where T : StorageEntityRegistryEntryBase
        {
            return GetDomain(typeof(T));
        }

        internal static KnownDomains GetDomain(Type type)
        {
            if (DomainsByType.TryGetValue(type, out var domain)) return domain;
            
            var attribute = (StorageEntityDomainAttribute)Attribute.GetCustomAttribute(type, typeof(StorageEntityDomainAttribute));
            domain = attribute.Domain;
            DomainsByType.Add(type, domain);

            return domain;
        }
    }
}
