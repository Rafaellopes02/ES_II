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

    var eventosRaw = await _context.eventos
        .Include(e => e.categoria)
        .ToListAsync();

    var organizadores = await _context.organizadoreseventos
        .Where(o => o.idutilizador == userId)
        .ToListAsync();

    var confirmados = await _context.utilizadoreseventos
        .Where(u => u.idutilizador == userId && u.estado == "Confirmado")
        .ToListAsync();

    var eventos = new List<object>();

            foreach (var e in eventosRaw)
            {
                var eorganizador = organizadores.FirstOrDefault(o => o.idevento == e.idevento)?.eorganizador ?? false;
                var idutilizador = organizadores.FirstOrDefault(o => o.idevento == e.idevento && o.eorganizador)?.idutilizador ?? 0;
                var inscrito = confirmados.Any(c => c.idevento == e.idevento);
                var jaComprou = confirmados.Any(c => c.idevento == e.idevento);

                var inscritos = await _context.organizadoreseventos
                    .CountAsync(o => o.idevento == e.idevento && !o.eorganizador);

                eventos.Add(new
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
                    inscrito,
                    jaComprouIngresso = jaComprou,
                    eorganizador,
                    idutilizador,
                    inscritos
                });
                        
            }
            return Json(new { eventos });
        }
    }
}