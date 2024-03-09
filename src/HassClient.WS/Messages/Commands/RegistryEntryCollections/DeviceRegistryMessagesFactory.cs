using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;

namespace HassClient.WS.Messages.Commands.RegistryEntryCollections
{
    internal class DeviceRegistryMessagesFactory : RegistryEntryCollectionMessagesFactory<Device>
    {
        public static readonly DeviceRegistryMessagesFactory Instance = new();

        public DeviceRegistryMessagesFactory()
            : base("config/device_registry", "device")
        {
        }

        public BaseOutgoingMessage CreateUpdateMessage(Device? device, bool? disable, bool forceUpdate)
        {
            var model = CreateDefaultUpdateObject(device, forceUpdate);

            if (!disable.HasValue) return CreateUpdateMessage(device.Id, model);
            
            var merged = HassSerializer.CreateJObject(new { DisabledBy = disable.Value ? DisabledByEnum.User : (DisabledByEnum?)null });
            model.Merge(merged);

            return CreateUpdateMessage(device.Id, model);
        }
    }
}
