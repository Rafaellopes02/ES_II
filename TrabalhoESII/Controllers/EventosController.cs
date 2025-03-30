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
            return View(); // renderiza Views/Eventos/Index.cshtml
        }

        [Authorize]
        [HttpGet("/eventos/stats")]
        public async Task<IActionResult> GetEventosStats()
        {
            var eventos = await _context.eventos
                .Include(e => e.categoria)
                .Select(e => new {
                    e.idevento,
                    e.nome,
                    data = e.data.ToString("yyyy-MM-dd"),
                    e.hora,
                    e.local,
                    e.descricao,
                    e.capacidade,
                    e.idcategoria
                })
                .ToListAsync();

            return Json(new { eventos });
        }
    }
}