using HassClient.Core.Models.RegistryEntries;
using HassClient.WS.Messages.Commands.RegistryEntryCollections;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    internal class DeviceStorageCollectionCommandProcessor
        : RegistryEntryCollectionCommandProcessor<DeviceRegistryMessagesFactory, Device>
    {
        protected override void PrepareHassContext(MockHassServerRequestContext context)
        {
            base.PrepareHassContext(context);
            context.HassDb.CreateObject(MockHassModelFactory.DeviceFaker.Generate());
        }
    }
}
