using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HassClient.Core.API;
using HassClient.Core.API.Endpoints;
using HassClient.Core.API.Models;
using HassClient.WS.AuthFlow;
using JetBrains.Annotations;

namespace HassClient.WS;

[PublicAPI]
public class ApiClient : JsonClient
{
    public ApiClient(IHttpClientFactory factory, HomeAssistantConfiguration homeAssistantConfiguration) : base(factory)
    {
        HttpClient.BaseAddress = new Uri($"{homeAssistantConfiguration!.Uri}/api/");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", homeAssistantConfiguration.Token);
        HttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("identity"));

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