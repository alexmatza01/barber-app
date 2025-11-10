using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberApp.API.Data;
using BarberApp.API.Models;
using BarberApp.API.DTOs;

namespace BarberApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly BarberAppDbContext _context;

    public AdminController(BarberAppDbContext context)
    {
        _context = context;
    }

    [HttpPut("barber-info")]
    public async Task<IActionResult> UpdateBarberInfo(UpdateBarberInfoDto dto)
    {
        var info = await _context.BarberInfos.FirstOrDefaultAsync();
        
        if (info == null)
            return NotFound();

        if (dto.Name != null) info.Name = dto.Name;
        if (dto.Bio != null) info.Bio = dto.Bio;
        if (dto.ProfileImage != null) info.ProfileImage = dto.ProfileImage;
        if (dto.Phone != null) info.Phone = dto.Phone;
        if (dto.Email != null) info.Email = dto.Email;
        if (dto.Address != null) info.Address = dto.Address;
        if (dto.GoogleCalendarId != null) info.GoogleCalendarId = dto.GoogleCalendarId;
        
        info.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("portfolio")]
    public async Task<ActionResult<PortfolioItem>> CreatePortfolioItem(CreatePortfolioItemDto dto)
    {
        var item = new PortfolioItem
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Order = dto.Order
        };

        _context.PortfolioItems.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetPortfolioItem", "Portfolio", new { id = item.Id }, item);
    }

    [HttpPut("portfolio/{id}")]
    public async Task<IActionResult> UpdatePortfolioItem(int id, UpdatePortfolioItemDto dto)
    {
        var item = await _context.PortfolioItems.FindAsync(id);
        
        if (item == null)
            return NotFound();

        if (dto.Title != null) item.Title = dto.Title;
        if (dto.Description != null) item.Description = dto.Description;
        if (dto.ImageUrl != null) item.ImageUrl = dto.ImageUrl;
        if (dto.Order.HasValue) item.Order = dto.Order.Value;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("portfolio/{id}")]
    public async Task<IActionResult> DeletePortfolioItem(int id)
    {
        var item = await _context.PortfolioItems.FindAsync(id);
        
        if (item == null)
            return NotFound();

        _context.PortfolioItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("service-types")]
    public async Task<ActionResult<ServiceType>> CreateServiceType(CreateServiceTypeDto dto)
    {
        var serviceType = new ServiceType
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            DurationMinutes = dto.DurationMinutes,
            IsActive = true
        };

        _context.ServiceTypes.Add(serviceType);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetServiceType", "ServiceTypes", new { id = serviceType.Id }, serviceType);
    }

    [HttpPut("service-types/{id}")]
    public async Task<IActionResult> UpdateServiceType(int id, UpdateServiceTypeDto dto)
    {
        var serviceType = await _context.ServiceTypes.FindAsync(id);
        
        if (serviceType == null)
            return NotFound();

        if (dto.Name != null) serviceType.Name = dto.Name;
        if (dto.Description != null) serviceType.Description = dto.Description;
        if (dto.Price.HasValue) serviceType.Price = dto.Price.Value;
        if (dto.DurationMinutes.HasValue) serviceType.DurationMinutes = dto.DurationMinutes.Value;
        if (dto.IsActive.HasValue) serviceType.IsActive = dto.IsActive.Value;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("service-types/{id}")]
    public async Task<IActionResult> DeleteServiceType(int id)
    {
        var serviceType = await _context.ServiceTypes.FindAsync(id);
        
        if (serviceType == null)
            return NotFound();

        serviceType.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
