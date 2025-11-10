using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberApp.API.Data;
using BarberApp.API.Models;

namespace BarberApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly BarberAppDbContext _context;

    public PortfolioController(BarberAppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PortfolioItem>>> GetPortfolioItems()
    {
        return await _context.PortfolioItems
            .OrderBy(p => p.Order)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PortfolioItem>> GetPortfolioItem(int id)
    {
        var item = await _context.PortfolioItems.FindAsync(id);

        if (item == null)
            return NotFound();

        return item;
    }
}
