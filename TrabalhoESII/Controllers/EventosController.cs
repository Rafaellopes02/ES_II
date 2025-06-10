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

            var eventosRaw = await _context.eventos
                .Include(e => e.categoria)
                .ToListAsync();

            var organizadores = userId.HasValue
                ? await _context.organizadoreseventos
                    .Where(o => o.idutilizador == userId)
                    .ToListAsync()
                : new List<organizadoreseventos>();

            var confirmados = userId.HasValue
                ? await _context.utilizadoreseventos
                    .Where(u => u.idutilizador == userId && u.estado == "Confirmado")
                    .ToListAsync()
                : new List<utilizadoreseventos>();

            var eventos = new List<object>();

            foreach (var e in eventosRaw)
            {
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
                    categoriaNome = e.categoria?.nome ?? "",

                    inscritos,
                    eorganizador = organizadores.FirstOrDefault(o => o.idevento == e.idevento)?.eorganizador ?? false,
                    idutilizador = organizadores.FirstOrDefault(o => o.idevento == e.idevento && o.eorganizador)?.idutilizador ?? 0,
                    inscrito = confirmados.Any(c => c.idevento == e.idevento),
                    jaComprou = confirmados.Any(c => c.idevento == e.idevento)
                });
            }

            return Ok(new { eventos });
        }


        [Authorize]
        [HttpGet("/eventos/detalhes/{id}")]
        public async Task<IActionResult> Detalhes(int id)
        {
            var evento = await _context.eventos
                .Include(e => e.categoria)
                .FirstOrDefaultAsync(e => e.idevento == id);

            if (evento == null)
                return NotFound();

            ViewBag.EventoId = evento.idevento;
            ViewBag.EventoNome = evento.nome;
            ViewBag.Descricao = evento.descricao;
            ViewBag.Data = evento.data.ToString("yyyy-MM-dd");
            ViewBag.Hora = evento.hora.ToString(@"hh\:mm");
            ViewBag.Local = evento.local;
            ViewBag.Categoria = evento.categoria?.nome ?? "Desconhecida";
            ViewBag.Capacidade = evento.capacidade;

            // Carregar participantes
            var participantes = await _context.utilizadoreseventos
                .Include(ue => ue.utilizador)
                .Where(ue => ue.idevento == id)
                .Select(ue => ue.utilizador)
                .ToListAsync();

            ViewBag.Participantes = participantes;

            // Permitir mostrar botões especiais ao organizador
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var organizador = await _context.organizadoreseventos
                .AnyAsync(o => o.idevento == id && o.idutilizador == userId && o.eorganizador);
            ViewBag.PodeAdicionarAtividade = organizador;

            return View("Detalhes/Index");
        }


    }
}
