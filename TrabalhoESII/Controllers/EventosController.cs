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
            int? userId = null;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (int.TryParse(userIdClaim, out var uid))
                    userId = uid;
            }

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

                    inscrito = userId != null &&
                        (
                            _context.organizadoreseventos.Any(o => o.idevento == e.idevento && o.idutilizador == userId)
                            ||
                            _context.pagamentos
                                .Include(p => p.ingressos)
                                .Any(p => p.idutilizador == userId && p.ingressos.idevento == e.idevento)
                        ),

                    eorganizador = userId != null &&
                        _context.organizadoreseventos
                            .Any(o => o.idevento == e.idevento && o.idutilizador == userId && o.eorganizador),

                    idutilizador = _context.organizadoreseventos
                        .Where(o => o.idevento == e.idevento && o.eorganizador)
                        .Select(o => o.idutilizador)
                        .FirstOrDefault(),

                    inscritos = _context.organizadoreseventos
                        .Count(o => o.idevento == e.idevento && !o.eorganizador)
                })
                .ToListAsync();

            return Json(new { eventos });
        }
    }
}
