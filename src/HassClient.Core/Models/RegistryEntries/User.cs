using System;
using System.Collections.Generic;
using HassClient.Core.Models.RegistryEntries.Modifiable;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.Core.Models.RegistryEntries
{
    /// <summary>
    /// Represents a Home Assistant user.
    /// </summary>
    public class User : RegistryEntryBase
    {
        private readonly ModifiableProperty<string> _name = new(nameof(Name));

        private readonly ModifiableProperty<bool> _isActive = new(nameof(IsActive));

        private readonly ModifiableProperty<bool> _isLocalOnly = new(nameof(IsLocalOnly));

        private readonly ModifiablePropertyCollection<string> _groupIds = new(nameof(GroupIds));

        /// <summary>
        /// The System Administrator group id constant.
        /// </summary>
        public const string SysadminGroupId = "system-admin";

        /// <inheritdoc />
        protected internal override string? UniqueId
        {
            get => Id;
            protected set => Id = value;
        }

        /// <summary>
        /// Gets the ID of this user.
        /// </summary>
        [JsonPropertyName("Id")]
        public string? Id { get; init; }

        /// <summary>
        /// Gets or sets the name of this user.
        /// </summary>
        [JsonProperty]
        public string? Name
        {
            get => _name.Value;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidOperationException($"'{nameof(Name)}' cannot be null or whitespace.");
                }

                _name.Value = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is owner of the system. In this case, the user will have full access to everything.
        /// </summary>
        [JsonPropertyName("IsOwner")]
        public bool IsOwner { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is only allowed to log in from the local network and not from the internet or cloud.
        /// </summary>
        [JsonPropertyName("local_only")]
        public bool IsLocalOnly
        {
            get => _isLocalOnly.Value;
            set => _isLocalOnly.Value = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        [JsonProperty]
        public bool IsActive
        {
            get => _isActive.Value;
            set => _isActive.Value = value;
        }

        /// <summary>
        /// Gets a value indicating whether the user is administrator.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsAdministrator
        {
            get => GroupIds?.Contains(SysadminGroupId) == true;
            set
            {
                if (value)
                {
                    GroupIds.Add(SysadminGroupId);
                }
                else
                {
                    GroupIds.Remove(SysadminGroupId);
                }
            }
        }

        /// <summary>
        /// Gets the user name of the user.
        /// </summary>
        [JsonPropertyName("Username")]
        public string? Username { get; init; }

        /// <summary>
        /// Gets a value indicating whether the user has been generated automatically by the system.
        /// </summary>
        [JsonPropertyName("SystemGenerated")]
        public bool SystemGenerated { get; init; }

        /// <summary>
        /// Gets a set of group ids where the user is included.
        /// </summary>
        [JsonProperty]
        public ICollection<string> GroupIds => _groupIds.Value;

        /// <summary>
        /// Gets the credentials of this user.
        /// </summary>
        [JsonPropertyName("Credentials")]
        public JRaw Credentials { get; init; }

        [System.Text.Json.Serialization.JsonConstructor]
        private User()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <param name="groupIds">The group ids where the user is included.</param>
        public User(string name, IEnumerable<string> groupIds = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));
            }

            Name = name;
            if (groupIds == null) return;
            
            foreach (var item in groupIds)
            {
                _groupIds.Value.Add(item);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <param name="isAdministrator">A value indicating is the user will be included in the <see cref="SysadminGroupId"/>.</param>
        public User(string name, bool isAdministrator)
            : this(name)
        {
            IsAdministrator = isAdministrator;
        }

        /// <summary>
        /// Method used by the serializer to determine if the <see cref="IsActive"/> property should be serialized.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the property should be serialized; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ShouldSerializeIsActive() => IsTracked;

        // Used for testing purposes.
        internal static User CreateUnmodified(string? uniqueId, string name, bool isOwner)
        {
            var result = new User(name, isOwner)
            {
                UniqueId = uniqueId,
                IsOwner = isOwner,
            };
            result.SaveChanges();

            return result;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            yield return _name;
            yield return _isActive;
            yield return _isLocalOnly;
            yield return _groupIds;
        }

        internal void SetIsActive(bool value)
        {
            IsActive = value;
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(User)}: {Name}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is User user &&
                   Id == user.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
        }

        // Used for testing purposes.
        internal User? Clone()
        {
            var result = CreateUnmodified(UniqueId, Name, IsOwner);
            return result;
        }
    }
}
