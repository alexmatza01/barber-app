using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberApp.API.Data;
using BarberApp.API.Models;

namespace BarberApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarberInfoController : ControllerBase
{
    private readonly BarberAppDbContext _context;

    public BarberInfoController(BarberAppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<BarberInfo>> GetBarberInfo()
    {
        var info = await _context.BarberInfos.FirstOrDefaultAsync();
        
        if (info == null)
            return NotFound();

        return info;
    }
}
