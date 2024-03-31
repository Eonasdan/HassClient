using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HassClient.Core.API.Models;
using JetBrains.Annotations;

namespace HassClient.Core.API.Endpoints;

/// <summary>
/// Provides access to the calendar API for retrieving information about calendar entries.
/// </summary>
[PublicAPI]
public class CalendarEndpoint(JsonClient client)
{
    private const string BaseUrl = "calendars";
    
    /// <summary>
    /// Retrieves a list of current and future calendar items, from now until the specified <paramref name="daysFromNow" />. The maximum number of results is driven by the "max_results" configuration option in the calendar config.
    /// </summary>
    /// <param name="calendarEntityName">The full name of the calendar entity. If this parameter does not start with "calendar.", it will be prepended automatically.</param>
    /// <param name="daysFromNow">Optional, defaults to 30. The number of days from the current point in time to retrieve calendar items for.</param>
    /// <returns>A <see cref="List{T}" /> representing the calendar items found.</returns>
    public Task<List<Calendar>?> GetEventsAsync(string calendarEntityName, int daysFromNow = 30) =>
        GetEventsAsync(calendarEntityName, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(daysFromNow));

    /// <summary>
    /// Retrieves a list of current and future calendar items, between the <paramref name="start" /> and <paramref name="end" /> parameters. The maximum number of results is driven by the "max_results" configuration option in the calendar config.
    /// </summary>
    /// <param name="calendarEntityName">The full name of the calendar entity. If this parameter does not start with "calendar.", it will be prepended automatically.</param>
    /// <param name="start">The start date/time to search.</param>
    /// <param name="end">The end date/time to search.</param>
    /// <returns>A <see cref="List{CalendarObject}" /> representing the calendar items found.</returns>
    public Task<List<Calendar>?> GetEventsAsync(string calendarEntityName, DateTimeOffset start, DateTimeOffset end) =>
        client.GetAsync<List<Calendar>>(
            $@"{BaseUrl}/{(calendarEntityName.StartsWith("calendar.") ? calendarEntityName : $"calendar.{calendarEntityName}")}?start={start:yyyy-MM-dd\THH:mm:ss}Z&end={end:yyyy-MM-dd\THH:mm:ss}Z");
    
    //todo missing crud operations?
}