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


        // A dashboard pode ser acedida sem login, mas vai exigir token via JS para os dados
        [AllowAnonymous]
        [HttpGet("/dashboard")]
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
            return View("Dashboard");
        }


        // Este endpoint exige JWT e retorna JSON
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
    
    public async Task<IActionResult> EventosComGraficos()
{
    var eventos = await _context.eventos.ToListAsync();
    return View(eventos);
}
    
    }
}