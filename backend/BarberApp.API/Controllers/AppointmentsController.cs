using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberApp.API.Data;
using BarberApp.API.Models;
using BarberApp.API.DTOs;
using BarberApp.API.Services;

namespace BarberApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly BarberAppDbContext _context;
    private readonly IGoogleCalendarService _calendarService;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(
        BarberAppDbContext context, 
        IGoogleCalendarService calendarService,
        ILogger<AppointmentsController> logger)
    {
        _context = context;
        _calendarService = calendarService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAppointments(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var query = _context.Appointments.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(a => a.StartTime >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(a => a.StartTime <= endDate.Value);

        var appointments = await query
            .OrderBy(a => a.StartTime)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                CustomerName = a.CustomerName,
                CustomerEmail = a.CustomerEmail,
                CustomerPhone = a.CustomerPhone,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                ServiceType = a.ServiceType,
                Notes = a.Notes,
                Status = a.Status.ToString(),
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentResponseDto>> GetAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
            return NotFound();

        return Ok(new AppointmentResponseDto
        {
            Id = appointment.Id,
            CustomerName = appointment.CustomerName,
            CustomerEmail = appointment.CustomerEmail,
            CustomerPhone = appointment.CustomerPhone,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            ServiceType = appointment.ServiceType,
            Notes = appointment.Notes,
            Status = appointment.Status.ToString(),
            CreatedAt = appointment.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentResponseDto>> CreateAppointment(CreateAppointmentDto dto)
    {
        var serviceType = await _context.ServiceTypes.FindAsync(dto.ServiceTypeId);
        if (serviceType == null)
            return BadRequest("Invalid service type");

        var endTime = dto.StartTime.AddMinutes(serviceType.DurationMinutes);

        // Check for conflicts
        var hasConflict = await _context.Appointments
            .AnyAsync(a => a.Status != AppointmentStatus.Cancelled &&
                          ((dto.StartTime >= a.StartTime && dto.StartTime < a.EndTime) ||
                           (endTime > a.StartTime && endTime <= a.EndTime)));

        if (hasConflict)
            return BadRequest("This time slot is already booked");

        var appointment = new Appointment
        {
            CustomerName = dto.CustomerName,
            CustomerEmail = dto.CustomerEmail,
            CustomerPhone = dto.CustomerPhone,
            StartTime = dto.StartTime,
            EndTime = endTime,
            ServiceType = serviceType.Name,
            Notes = dto.Notes,
            Status = AppointmentStatus.Pending
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // Try to create Google Calendar event
        try
        {
            var barberInfo = await _context.BarberInfos.FirstOrDefaultAsync();
            if (barberInfo?.GoogleCalendarId != null)
            {
                var eventId = await _calendarService.CreateEvent(
                    barberInfo.GoogleCalendarId,
                    $"Appointment: {serviceType.Name}",
                    $"Customer: {dto.CustomerName}\nEmail: {dto.CustomerEmail}\nPhone: {dto.CustomerPhone ?? "N/A"}\nNotes: {dto.Notes ?? "N/A"}",
                    dto.StartTime,
                    endTime,
                    dto.CustomerEmail
                );

                if (eventId != null)
                {
                    appointment.GoogleCalendarEventId = eventId;
                    await _context.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Google Calendar event");
        }

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, 
            new AppointmentResponseDto
            {
                Id = appointment.Id,
                CustomerName = appointment.CustomerName,
                CustomerEmail = appointment.CustomerEmail,
                CustomerPhone = appointment.CustomerPhone,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                ServiceType = appointment.ServiceType,
                Notes = appointment.Notes,
                Status = appointment.Status.ToString(),
                CreatedAt = appointment.CreatedAt
            });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(int id, UpdateAppointmentDto dto)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        if (dto.StartTime.HasValue)
        {
            var serviceType = await _context.ServiceTypes
                .FirstOrDefaultAsync(s => s.Name == appointment.ServiceType);
            
            if (serviceType != null)
            {
                var endTime = dto.StartTime.Value.AddMinutes(serviceType.DurationMinutes);
                
                // Check for conflicts
                var hasConflict = await _context.Appointments
                    .AnyAsync(a => a.Id != id && 
                                  a.Status != AppointmentStatus.Cancelled &&
                                  ((dto.StartTime >= a.StartTime && dto.StartTime < a.EndTime) ||
                                   (endTime > a.StartTime && endTime <= a.EndTime)));

                if (hasConflict)
                    return BadRequest("This time slot is already booked");

                appointment.StartTime = dto.StartTime.Value;
                appointment.EndTime = endTime;
            }
        }

        if (dto.Notes != null)
            appointment.Notes = dto.Notes;

        if (dto.Status != null && Enum.TryParse<AppointmentStatus>(dto.Status, out var status))
            appointment.Status = status;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        // Try to delete from Google Calendar
        try
        {
            var barberInfo = await _context.BarberInfos.FirstOrDefaultAsync();
            if (barberInfo?.GoogleCalendarId != null && appointment.GoogleCalendarEventId != null)
            {
                await _calendarService.DeleteEvent(barberInfo.GoogleCalendarId, appointment.GoogleCalendarEventId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete Google Calendar event");
        }

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("available-slots")]
    public async Task<ActionResult<IEnumerable<AvailableSlotDto>>> GetAvailableSlots(
        [FromQuery] DateTime date,
        [FromQuery] int serviceTypeId)
    {
        var serviceType = await _context.ServiceTypes.FindAsync(serviceTypeId);
        if (serviceType == null)
            return BadRequest("Invalid service type");

        var barberInfo = await _context.BarberInfos.FirstOrDefaultAsync();
        var calendarId = barberInfo?.GoogleCalendarId ?? "primary";

        var slots = await _calendarService.GetAvailableSlots(calendarId, date, serviceType.DurationMinutes);
        
        // Filter out slots that are already booked in database
        var bookedSlots = await _context.Appointments
            .Where(a => a.StartTime.Date == date.Date && a.Status != AppointmentStatus.Cancelled)
            .Select(a => new { a.StartTime, a.EndTime })
            .ToListAsync();

        var availableSlots = slots.Where(slot =>
            !bookedSlots.Any(booked =>
                (slot.StartTime >= booked.StartTime && slot.StartTime < booked.EndTime) ||
                (slot.EndTime > booked.StartTime && slot.EndTime <= booked.EndTime)
            )
        ).ToList();

        return Ok(availableSlots);
    }
}
