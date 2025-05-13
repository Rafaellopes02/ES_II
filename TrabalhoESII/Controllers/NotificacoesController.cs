using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;

namespace TrabalhoESII.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacoesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NotificacoesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotificacoes()
    {
        var userId = int.Parse(User.FindFirst("UserId").Value);
        
        var notificacoes = await _context.notificacoes
            .Where(n => n.idutilizador == userId)
            .OrderByDescending(n => n.idnotificacao)
            .Select(n => new {
                n.idnotificacao,
                n.mensagem,
            })
            .ToListAsync();

        return Ok(notificacoes);
    }
}
