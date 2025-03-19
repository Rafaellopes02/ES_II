using Microsoft.AspNetCore.Mvc;
using TrabalhoESII.Models;

[Route("api/[controller]")]
[ApiController]
public class DatabaseTestController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DatabaseTestController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult TestConnection()
    {
        if (_context.Database.CanConnect())
        {
            return Ok("Ligação à base de dados bem-sucedida!");
        }
        return StatusCode(500, "Falha ao conectar à base de dados.");
    }
}