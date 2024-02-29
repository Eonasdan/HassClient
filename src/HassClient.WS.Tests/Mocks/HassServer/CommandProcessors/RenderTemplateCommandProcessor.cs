using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using HassClient.Core.Helpers;
using HassClient.Core.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands;
using HassClient.WS.Messages.Response;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    public class RenderTemplateCommandProcessor : BaseCommandProcessor
    {
        private HashSet<string> _entityIds;

        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is RenderTemplateMessage;

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var commandRenderTemplate = receivedCommand as RenderTemplateMessage;

            _entityIds = new HashSet<string>();
            var result = Regex.Replace(commandRenderTemplate.Template, @"{{ (.*) }}", RenderTemplateValue);
            var listeners = new ListenersTemplateInfo
            {
                All = false,
                Time = false,
            };
            listeners.Entities = _entityIds.ToArray();
            listeners.Domains = listeners.Entities.Select(x => x.GetDomain()).ToArray();
            _entityIds = null;

            var renderTemplateEvent = new TemplateEventInfo
            {
                Result = result,
                Listeners = listeners,
            };

            var eventMsg = new EventResultMessage
            {
                Id = commandRenderTemplate.Id,
                Event = new JRaw(HassSerializer.SerializeObject(renderTemplateEvent))
            };
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(40);
                await context.SendMessageAsync(eventMsg, CancellationToken.None);
            });

            return CreateResultMessageWithResult(null);
        }

        private string RenderTemplateValue(Match match)
        {
            var statesPattern = @"states\('(.*)'\)";
            return Regex.Replace(match.Groups[1].Value, statesPattern, RenderStateValue);
        }

        private string RenderStateValue(Match match)
        {
            var entityId = match.Groups[1].Value;
            _entityIds.Add(entityId);

            if (entityId.GetDomain() == "sun")
            {
                return "below_horizon";
            }

            return new Faker().RandomEntityState();
        }
    }
}
