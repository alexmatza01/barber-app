using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberApp.API.Data;
using BarberApp.API.Models;

namespace BarberApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceTypesController : ControllerBase
{
    private readonly BarberAppDbContext _context;

    public ServiceTypesController(BarberAppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceType>>> GetServiceTypes()
    {
        return await _context.ServiceTypes
            .Where(s => s.IsActive)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceType>> GetServiceType(int id)
    {
        var serviceType = await _context.ServiceTypes.FindAsync(id);

        if (serviceType == null)
            return NotFound();

        return serviceType;
    }
}
