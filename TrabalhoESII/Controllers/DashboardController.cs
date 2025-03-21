using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;

namespace TrabalhoESII.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Retorna a View da Dashboard sem exigir login
        [HttpGet]
        public IActionResult Index()
        {
            return View("Dashboard");
        }

        // ✅ Retorna JSON com estatísticas sem exigir JWT
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalEventos = await _context.eventos.CountAsync();
            var totalParticipantes = await _context.utilizadores.CountAsync();
            var totalCategorias = await _context.categorias.CountAsync();

            return Json(new
            {
                TotalEventos = totalEventos,
                TotalParticipantes = totalParticipantes,
                TotalCategorias = totalCategorias
            });
        }
    }
}