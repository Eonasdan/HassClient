using System.Net.Http.Headers;
using HomeAssistantClient.Core.API;
using HomeAssistantClient.Core.API.Endpoints;
using HomeAssistantClient.Core.API.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace HomeAssistantClient.API;

[PublicAPI]
public class ApiClient : JsonClient
{
    public ApiClient(HttpClient client, IConfiguration configuration) : base(client)
    {
        var homeAssistantConfiguration = HomeAssistantConfiguration.FromConfig(configuration);
        client.BaseAddress = new Uri($"{homeAssistantConfiguration!.Uri!}/api/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", homeAssistantConfiguration.ApiKey);
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("identity"));

        Automation = new AutomationEndpoint(this);
        Calendar = new CalendarEndpoint(this);
        CameraProxy = new CameraProxyEndpoint(this);
        Config = new ConfigEndpoint(this);
        Discovery = new DiscoveryEndpoint(this);
        Entity = new EntityEndpoint(this);
        ErrorLog = new ErrorLogEndpoint(this);
        Event = new EventEndpoint(this);
        History = new HistoryEndpoint(this);
        Info = new InfoEndpoint(this);
        Logbook = new LogbookEndpoint(this);
        Service = new ServiceEndpoint(this);
        State = new StateEndpoint(this);
        Stats = new StatsEndpoint(this);
        Template = new TemplateEndpoint(this);
    }
    
    public async Task<MessageObject?> GetStatusMessage() => await GetAsync<MessageObject>("");

    public AutomationEndpoint Automation { get; }
    public CalendarEndpoint Calendar { get; }
    public CameraProxyEndpoint CameraProxy { get; }
    public ConfigEndpoint Config { get; }
    public DiscoveryEndpoint Discovery { get; }
    public EntityEndpoint Entity { get; }
    public ErrorLogEndpoint ErrorLog { get; }
    public EventEndpoint Event { get; }
    public HistoryEndpoint History { get; }
    public InfoEndpoint Info { get; }
    public LogbookEndpoint Logbook { get; }
    public ServiceEndpoint Service { get; }
    public StateEndpoint State { get; }
    public StatsEndpoint Stats { get; }
    public TemplateEndpoint Template { get; }
}