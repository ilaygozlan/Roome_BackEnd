using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Roome_BackEnd.BL;

public class GoogleCalendarService
{
    public static async Task<string> AddOpenHouseToCalendarAsync(OpenHouse openHouse, string userAccessToken)
    {
        var credential = GoogleCredential
            .FromAccessToken(userAccessToken)
            .CreateScoped(CalendarService.Scope.Calendar);

        var service = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Roome App"
        });

        Event newEvent = new Event()
        {
            Summary = $"סיור בדירה #{openHouse.ApartmentId}",
            Description = "סיור בית פתוח דרך Roome",
            Start = new EventDateTime()
            {
                DateTimeDateTimeOffset = openHouse.Date.Date + TimeSpan.Parse(openHouse.StartTime),
                TimeZone = "Asia/Jerusalem"
            },
            End = new EventDateTime()
            {
                DateTimeDateTimeOffset = openHouse.Date.Date + TimeSpan.Parse(openHouse.EndTime),
                TimeZone = "Asia/Jerusalem"
            }
        };

        var createdEvent = await service.Events.Insert(newEvent, "primary").ExecuteAsync();
        Console.WriteLine("Event created at calender " + createdEvent.HtmlLink);

        return createdEvent.HtmlLink;
    }
}
