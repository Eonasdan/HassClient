using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.Core.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands.RegistryEntryCollections;
using HassClient.WS.Tests.Mocks.HassServer.CommandProcessors;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class MockHassServerRequestContext
    {
        private const int InconmingBufferSize = 4 * 1024 * 1024; // 4MB

        private readonly List<BaseCommandProcessor> _commandProcessors;

        private readonly ArraySegment<byte> _receivingBuffer;

        public readonly MockHassDb HassDb;

        public readonly EventSubscriptionsProcessor EventSubscriptionsProcessor;

        public bool IsAuthenticating { get; set; }
        public uint LastReceivedId { get; set; }

        public WebSocket WebSocket { get; private set; }

        public MockHassServerRequestContext(MockHassDb hassDb, WebSocket webSocket)
        {
            IsAuthenticating = true;
            LastReceivedId = 0;
            HassDb = hassDb;
            WebSocket = webSocket;
            _receivingBuffer = new ArraySegment<byte>(new byte[InconmingBufferSize]);
            EventSubscriptionsProcessor = new EventSubscriptionsProcessor();
            _commandProcessors = new List<BaseCommandProcessor>
            {
                EventSubscriptionsProcessor,
                new PingCommandProcessor(),
                new GetConfigurationCommandProcessor(),
                new EntitySourceCommandProcessor(),
                new PanelsCommandProcessor(),
                new RenderTemplateCommandProcessor(),
                new SearchCommandProcessor(),
                new CallServiceCommandProcessor(),
                new GetServicesCommandProcessor(),
                new GetStatesCommandProcessor(),
                new RegistryEntryCollectionCommandProcessor<AreaRegistryMessagesFactory, Area>(),
                new DeviceStorageCollectionCommandProcessor(),
                new UserStorageCollectionCommandProcessor(),
                new EntityRegistryStorageCollectionCommandProcessor(),
                new StorageCollectionCommandProcessor<InputBoolean>(),
                new StorageCollectionCommandProcessor<Zone>(),
            };
        }

        public bool TryProccesMessage(BaseIdentifiableMessage receivedCommand, out BaseIdentifiableMessage result)
        {
            var processor = _commandProcessors.FirstOrDefault(x => x.CanProcess(receivedCommand));
            if (processor == null)
            {
                Trace.WriteLine($"[MockHassServer] No Command processor found for received message '{receivedCommand.Type}'");
                result = null;
                return false;
            }

            result = processor.ProcessCommand(this, receivedCommand);
            return true;
        }

        public async Task<TMessage> ReceiveMessageAsync<TMessage>(CancellationToken cancellationToken)
            where TMessage : BaseMessage
        {
            var receivedString = new StringBuilder();
            WebSocketReceiveResult rcvResult;
            do
            {
                rcvResult = await WebSocket.ReceiveAsync(_receivingBuffer, cancellationToken);
                var msgBytes = _receivingBuffer.Skip(_receivingBuffer.Offset).Take(rcvResult.Count).ToArray();
                receivedString.Append(Encoding.UTF8.GetString(msgBytes));
            }
            while (!rcvResult.EndOfMessage);

            var rcvMsg = receivedString.ToString();
            return HassSerializer.DeserializeObject<TMessage>(rcvMsg);
        }

        public async Task SendMessageAsync(BaseMessage message, CancellationToken cancellationToken)
        {
            var sendMsg = HassSerializer.SerializeObject(message);
            var sendBytes = Encoding.UTF8.GetBytes(sendMsg);
            var sendBuffer = new ArraySegment<byte>(sendBytes);
            await WebSocket.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken);
        }
    }
}
