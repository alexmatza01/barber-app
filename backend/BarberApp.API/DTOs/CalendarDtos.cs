namespace BarberApp.API.DTOs;

public class AvailableSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class CalendarEventDto
{
    public string? Id { get; set; }
    public string? Summary { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
