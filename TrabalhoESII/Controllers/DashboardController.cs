using Microsoft.AspNetCore.Authorization;
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

        // ✅ Página com gráfico de participação (apenas renderiza a view, pode ser pública)
        public async Task<IActionResult> AnaliseParticipacao(int? idevento)
        {
            if (idevento == null)
                return BadRequest("Evento não especificado.");

            var dados = await _context.atividades
                .Where(a => a.idevento == idevento)
                .Select(a => new GraficoParticipacaoViewModel
                {
                    NomeAtividade = a.nome,
                    NumeroParticipantes = _context.utilizadoresatividades.Count(u => u.idatividade == a.idatividade)
                })
                .ToListAsync();

            ViewBag.DadosGrafico = System.Text.Json.JsonSerializer.Serialize(dados);
            return View();
        }

        // ✅ A View é pública, mas o JS vai buscar os dados protegidos
        [AllowAnonymous]
        [HttpGet("/dashboard")]
        public IActionResult Index()
        {
            return View("Dashboard");
        }

        // ✅ Protegido com JWT — só acessível se tiver token válido
        [Authorize]
        [HttpGet("/dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalEventos = await _context.eventos.CountAsync();
            var totalParticipantes = await _context.utilizadores.CountAsync();
            var totalCategorias = await _context.categorias.CountAsync();

            return Json(new
            {
                totalEventos,
                totalParticipantes,
                totalCategorias
            });
        }

        // ✅ Página que mostra gráficos com eventos
        public async Task<IActionResult> EventosComGraficos()
        {
            var eventos = await _context.eventos.ToListAsync();
            return View(eventos);
        }
    }
}
