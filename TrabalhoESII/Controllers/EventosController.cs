using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;

namespace TrabalhoESII.Controllers
{
    public class EventosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("/eventos")]
        public IActionResult Index()
        {
            string tipo = "Desconhecido";

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var tipoId = User.Claims.FirstOrDefault(c => c.Type == "TipoUtilizadorId")?.Value;

                if (tipoId == "1") tipo = "Admin";
                else if (tipoId == "2") tipo = "UserManager";
                else if (tipoId == "3") tipo = "User";
            }

            ViewBag.UserType = tipo;
            return View();
        }

        [Authorize]
        [HttpGet("/eventos/stats")]
        public async Task<IActionResult> GetEventosStats()
{
    var userIdClaim = User.FindFirst("UserId");
    int.TryParse(userIdClaim?.Value, out int userId);

    var eventos = await _context.eventos
        .Include(e => e.categoria)
        .Select(e => new

   
                {
                    e.idevento,
                    e.nome,
                    data = e.data.ToString("yyyy-MM-dd"),
                    e.hora,
                    e.local,
                    e.descricao,
                    e.capacidade,
                    e.idcategoria,
                    categoriaNome = e.categoria.nome,

                     // Verifica se o utilizador está inscrito neste evento
                    inscrito =
    _context.organizadoreseventos
        .Any(o => o.idevento == e.idevento && o.idutilizador == userId)
    ||
    _context.pagamentos
        .Include(p => p.ingressos)
        .Any(p => p.idutilizador == userId && p.ingressos.idevento == e.idevento),

                    // Verifica se é o organizador (criador)
                    eorganizador = _context.organizadoreseventos
                        .Where(o => o.idevento == e.idevento && o.idutilizador == userId)
                        .Select(o => o.eorganizador)
                        .FirstOrDefault(),

                    // ID do organizador (criador)
                    idutilizador = _context.organizadoreseventos
                        .Where(o => o.idevento == e.idevento && o.eorganizador)
                        .Select(o => o.idutilizador)
                        .FirstOrDefault(),

                    // Contagem de inscritos (excluindo organizador)
                    inscritos = _context.organizadoreseventos
                        .Count(o => o.idevento == e.idevento && !o.eorganizador)
                })
                .ToListAsync();
                    
            return Json(new { eventos });
        }
    }
}