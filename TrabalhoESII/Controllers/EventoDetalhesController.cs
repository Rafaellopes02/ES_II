using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;

namespace TrabalhoESII.Controllers
{
    public class EventoDetalhesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventoDetalhesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[AllowAnonymous]
        [HttpGet("/Eventos/{id}")]
        public async Task<IActionResult> Detalhes(int id)
        {
            var evento = await _context.eventos
                .Include(e => e.categoria)
                .FirstOrDefaultAsync(e => e.idevento == id);

            if (evento == null) return NotFound();

            // Lógica de permissão
            string tipo = "Desconhecido";
            bool podeAdicionarAtividade = true;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var tipoId = User.Claims.FirstOrDefault(c => c.Type == "TipoUtilizadorId")?.Value;
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                if (tipoId == "1" || tipoId == "2") // Admin ou UserManager
                {
                    tipo = tipoId == "1" ? "Admin" : "UserManager";
                    podeAdicionarAtividade = true;
                }
                else if (tipoId == "3" && int.TryParse(userId, out int uid))
                {
                    tipo = "User";
                    var eOrganizador = await _context.organizadoreseventos
                        .AnyAsync(o => o.idutilizador == uid && o.idevento == id && o.eorganizador);
                    podeAdicionarAtividade = eOrganizador;
                }
                
            }

            ViewBag.PodeAdicionarAtividade = podeAdicionarAtividade;

            // Resto das ViewBags
            ViewBag.EventoNome = evento.nome;
            ViewBag.Descricao = evento.descricao;
            ViewBag.Data = evento.data.ToString("dd/MM/yyyy");
            ViewBag.Hora = evento.hora.ToString(@"hh\:mm");
            ViewBag.Local = evento.local;
            ViewBag.Categoria = evento.categoria?.nome;
            ViewBag.Capacidade = evento.capacidade;
            ViewBag.EventoId = evento.idevento;

            return View("~/Views/Eventos/Detalhes/Index.cshtml");
        }
    }
}