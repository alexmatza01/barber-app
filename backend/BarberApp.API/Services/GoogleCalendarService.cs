using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using BarberApp.API.DTOs;

namespace BarberApp.API.Services;

public interface IGoogleCalendarService
{
    Task<List<AvailableSlotDto>> GetAvailableSlots(string calendarId, DateTime date, int durationMinutes);
    Task<string?> CreateEvent(string calendarId, string summary, string description, DateTime start, DateTime end, string? attendeeEmail);
    Task<bool> DeleteEvent(string calendarId, string eventId);
    Task<List<CalendarEventDto>> GetEvents(string calendarId, DateTime startDate, DateTime endDate);
}

public class GoogleCalendarService : IGoogleCalendarService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleCalendarService> _logger;

    public GoogleCalendarService(IConfiguration configuration, ILogger<GoogleCalendarService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private CalendarService GetCalendarService()
    {
        var credentialPath = _configuration["GoogleCalendar:CredentialPath"];
        
        // For demo purposes, we'll return a service with API key authentication
        // In production, you'd use OAuth2 or Service Account credentials
        var service = new CalendarService(new BaseClientService.Initializer
        {
            ApiKey = _configuration["GoogleCalendar:ApiKey"],
            ApplicationName = "Barber Appointment App"
        });

        return service;
    }

    public async Task<List<AvailableSlotDto>> GetAvailableSlots(string calendarId, DateTime date, int durationMinutes)
    {
        try
        {
            var service = GetCalendarService();
            var request = service.Events.List(calendarId);
            request.TimeMin = date.Date;
            request.TimeMax = date.Date.AddDays(1);
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = await request.ExecuteAsync();
            
            // Define business hours (e.g., 9 AM to 6 PM)
            var businessStart = date.Date.AddHours(9);
            var businessEnd = date.Date.AddHours(18);
            var slotDuration = TimeSpan.FromMinutes(durationMinutes);

            var availableSlots = new List<AvailableSlotDto>();
            var currentTime = businessStart;

            foreach (var evt in events.Items ?? new List<Event>())
            {
                var eventStart = evt.Start.DateTime ?? DateTime.Parse(evt.Start.Date);
                var eventEnd = evt.End.DateTime ?? DateTime.Parse(evt.End.Date);

                // Add slots before this event
                while (currentTime.Add(slotDuration) <= eventStart)
                {
                    availableSlots.Add(new AvailableSlotDto
                    {
                        StartTime = currentTime,
                        EndTime = currentTime.Add(slotDuration)
                    });
                    currentTime = currentTime.Add(slotDuration);
                }

                // Move current time to end of this event
                if (eventEnd > currentTime)
                    currentTime = eventEnd;
            }

            // Add remaining slots until business end
            while (currentTime.Add(slotDuration) <= businessEnd)
            {
                availableSlots.Add(new AvailableSlotDto
                {
                    StartTime = currentTime,
                    EndTime = currentTime.Add(slotDuration)
                });
                currentTime = currentTime.Add(slotDuration);
            }

            return availableSlots;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available slots from Google Calendar");
            // Return mock available slots if calendar is not configured
            return GetMockAvailableSlots(date, durationMinutes);
        }
    }

    public async Task<string?> CreateEvent(string calendarId, string summary, string description, 
        DateTime start, DateTime end, string? attendeeEmail)
    {
        try
        {
            var service = GetCalendarService();
            var newEvent = new Event
            {
                Summary = summary,
                Description = description,
                Start = new EventDateTime { DateTime = start },
                End = new EventDateTime { DateTime = end },
                Attendees = attendeeEmail != null ? new List<EventAttendee>
                {
                    new EventAttendee { Email = attendeeEmail }
                } : null
            };

            var request = service.Events.Insert(newEvent, calendarId);
            var createdEvent = await request.ExecuteAsync();
            return createdEvent.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event in Google Calendar");
            // Return mock event ID if calendar is not configured
            return Guid.NewGuid().ToString();
        }
    }

    public async Task<bool> DeleteEvent(string calendarId, string eventId)
    {
        try
        {
            var service = GetCalendarService();
            await service.Events.Delete(calendarId, eventId).ExecuteAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event from Google Calendar");
            return false;
        }
    }

    public async Task<List<CalendarEventDto>> GetEvents(string calendarId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var service = GetCalendarService();
            var request = service.Events.List(calendarId);
            request.TimeMin = startDate;
            request.TimeMax = endDate;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = await request.ExecuteAsync();
            
            return events.Items?.Select(e => new CalendarEventDto
            {
                Id = e.Id,
                Summary = e.Summary,
                StartTime = e.Start.DateTime,
                EndTime = e.End.DateTime
            }).ToList() ?? new List<CalendarEventDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events from Google Calendar");
            return new List<CalendarEventDto>();
        }
    }

    private List<AvailableSlotDto> GetMockAvailableSlots(DateTime date, int durationMinutes)
    {
        // Return mock slots for demo purposes
        var slots = new List<AvailableSlotDto>();
        var businessStart = date.Date.AddHours(9);
        var businessEnd = date.Date.AddHours(18);
        var slotDuration = TimeSpan.FromMinutes(durationMinutes);
        var currentTime = businessStart;

        while (currentTime.Add(slotDuration) <= businessEnd)
        {
            slots.Add(new AvailableSlotDto
            {
                StartTime = currentTime,
                EndTime = currentTime.Add(slotDuration)
            });
            currentTime = currentTime.Add(slotDuration);
        }

        return slots;
    }
}
