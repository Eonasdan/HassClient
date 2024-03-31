﻿using Microsoft.Extensions.Configuration;

namespace HassClient.WS.AuthFlow;

public class HomeAssistantConfiguration
{
    private const string SectionName = "HomeAssistant";

    public string Uri { get; set; } = default!;
    public string? ClientId { get; set; }

    public bool LimitHassInstance { get; set; } = true;
    public string? Token { get; set; }

    public static HomeAssistantConfiguration FromConfig(IConfiguration configuration)
    {
        var homeAssistantConfiguration = new HomeAssistantConfiguration();
        configuration.GetSection(SectionName).Bind(homeAssistantConfiguration);
        return homeAssistantConfiguration;
    }
}