using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Helpers;
using HassClient.Core.Models;
using HassClient.Core.Models.Events;
using HassClient.Core.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Authentication;
using HassClient.WS.Messages.Commands;
using HassClient.WS.Messages.Response;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class MockHassServerWebSocket : MockServerWebSocket
    {
        private readonly MockHassDb _hassDb = new();

        private MockHassServerRequestContext _activeRequestContext;

        public CalendarVersion HaVersion => CalendarVersion.Create("2022.1.0");

        public ConnectionParameters ConnectionParameters { get; private set; }

        public bool IgnoreAuthenticationMessages { get; set; } = false;

        public TimeSpan ResponseSimulatedDelay { get; set; } = TimeSpan.Zero;

        public MockHassServerWebSocket()
        {
            ConnectionParameters = ConnectionParameters.CreateFromInstanceBaseUrl(
                $"http://{ServerUri.Host}:{ServerUri.Port}",
                GenerateRandomToken());
        }

        public Task<bool> RaiseStateChangedEventAsync(string entityId)
        {
            var data = MockHassModelFactory.StateChangedEventFaker
                                           .GenerateWithEntityId(entityId);

            var eventResult = new EventResultInfo
            {
                EventType = KnownEventTypes.StateChanged.ToEventTypeString(),
                Origin = "mock_server",
                TimeFired = DateTimeOffset.Now,
                Data = new JRaw(HassSerializer.SerializeObject(data)),
                Context = data.OldState.Context
            };

            var eventResultObject = new JRaw(HassSerializer.SerializeObject(eventResult));
            return RaiseEventAsync(KnownEventTypes.StateChanged, eventResultObject);
        }

        public async Task<bool> RaiseEventAsync(KnownEventTypes eventType, JRaw eventResultObject)
        {
            var context = _activeRequestContext;
            if (context.EventSubscriptionsProcessor.TryGetSubscribers(eventType, out var subscribers))
            {
                foreach (var id in subscribers)
                {
                    await context.SendMessageAsync(new EventResultMessage { Event = eventResultObject, Id = id }, default);
                }

                return true;
            }

            return false;
        }

        private string GenerateRandomToken() => Guid.NewGuid().ToString("N");

        protected override async Task RespondToWebSocketRequestAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var context = new MockHassServerRequestContext(_hassDb, webSocket);

            await context.SendMessageAsync(new AuthenticationRequiredMessage { HaVersion = HaVersion.ToString() }, cancellationToken);

            try
            {
                while (true)
                {
                    if (context.IsAuthenticating)
                    {
                        var incomingMessage = await context.ReceiveMessageAsync<BaseMessage>(cancellationToken);
                        await Task.Delay(ResponseSimulatedDelay);

                        if (!IgnoreAuthenticationMessages &&
                            incomingMessage is AuthenticationMessage authMessage)
                        {
                            if (authMessage.AccessToken == ConnectionParameters.AccessToken)
                            {
                                await context.SendMessageAsync(new AuthenticationOkMessage { HaVersion = HaVersion.ToString() }, cancellationToken);
                                context.IsAuthenticating = false;
                                _activeRequestContext = context;
                            }
                            else
                            {
                                await context.SendMessageAsync(new AuthenticationInvalidMessage(), cancellationToken);
                                break;
                            }
                        }
                    }
                    else
                    {
                        var receivedMessage = await context.ReceiveMessageAsync<BaseOutgoingMessage>(cancellationToken);
                        var receivedMessageId = receivedMessage.Id;

                        await Task.Delay(ResponseSimulatedDelay);

                        BaseIdentifiableMessage response;
                        if (context.LastReceivedId >= receivedMessageId)
                        {
                            response = new ResultMessage { Error = new ErrorInfo(ErrorCodes.IdReuse) };
                        }
                        else
                        {
                            context.LastReceivedId = receivedMessageId;

                            if (receivedMessage is PingMessage)
                            {
                                response = new PongMessage();
                            }
                            else if (!context.TryProccesMessage(receivedMessage, out response))
                            {
                                response = new ResultMessage { Error = new ErrorInfo(ErrorCodes.UnknownCommand) };
                            }
                        }

                        response.Id = receivedMessageId;
                        await context.SendMessageAsync(response, cancellationToken);
                    }
                }
            }
            catch
            {
                Trace.WriteLine("A problem occured while attending client. Closing connection.");
                await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, string.Empty, default);
            }
        }
    }
}
