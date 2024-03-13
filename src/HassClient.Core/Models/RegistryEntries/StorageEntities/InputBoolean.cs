using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries.Modifiable;
using System.Text.Json.Serialization;

namespace HassClient.Core.Models.RegistryEntries.StorageEntities
{
    /// <summary>
    /// Represents an input boolean.
    /// </summary>
    [StorageEntityDomain(KnownDomains.InputBoolean)]
    public class InputBoolean : StorageEntityRegistryEntryBase
    {
        private readonly ModifiableProperty<bool> _initial = new(nameof(Initial));

        /// <summary>
        /// Gets or sets the initial value when Home Assistant starts.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool Initial
        {
            get => _initial.Value;
            init => _initial.Value = value;
        }

        [JsonConstructor]
        private InputBoolean()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputBoolean"/> class.
        /// </summary>
        /// <param name="name">The entity name.</param>
        /// <param name="icon">The entity icon.</param>
        /// <param name="initial">The  initial value when Home Assistant starts.</param>
        public InputBoolean(string? name, string? icon = null, bool initial = false)
            : base(name, icon)
        {
            Initial = initial;
        }

        // Used for testing purposes.
        internal static InputBoolean CreateUnmodified(string? uniqueId, string? name, string? icon = null, bool initial = false)
        {
            var result = new InputBoolean(name, icon, initial) { Id = uniqueId };
            result.SaveChanges();
            return result;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            return base.GetModifiableProperties().Append(_initial);
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(InputBoolean)}: {Name}";

        // Used for testing purposes.
        internal InputBoolean Clone()
        {
            var result = CreateUnmodified(UniqueId, Name, Icon, Initial);
            return result;
        }
    }
}
