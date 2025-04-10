using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;
using Microsoft.AspNetCore.Authorization;

namespace TrabalhoESII.Controllers
{
    [ApiController]
    [Route("api/eventos")]
    public class EventosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterEvento([FromBody] EventosRegisterModel evento)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var novoEvento = new eventos
            {
                nome = evento.nome,
                descricao = evento.descricao,
                data = DateTime.SpecifyKind(evento.data, DateTimeKind.Utc),
                hora = evento.hora,
                local = evento.local,
                capacidade = evento.capacidade,
                idcategoria = evento.idCategoria
            };

            _context.eventos.Add(novoEvento);
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpGet("search")]
        [Authorize]
        public IActionResult SearchEventos([FromQuery] string? nome, [FromQuery] DateTime? data, [FromQuery] string? local, [FromQuery] int? idCategoria)
        {
            var query = _context.eventos.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(e => EF.Functions.ILike(e.nome, $"%{nome}%"));

            if (data.HasValue)
            {
                var dataUtc = DateTime.SpecifyKind(data.Value.Date, DateTimeKind.Utc);
                query = query.Where(e => e.data.Date == dataUtc.Date);
            }

            if (!string.IsNullOrEmpty(local))
                query = query.Where(e => EF.Functions.ILike(e.local, $"%{local}%"));

            if (idCategoria.HasValue)
                query = query.Where(e => e.idcategoria == idCategoria.Value);

            var eventos = query
                .Select(e => new
                {
                    e.idevento,
                    e.nome,
                    e.descricao,
                    e.data,
                    e.hora,
                    e.local,
                    e.capacidade
                })
                .ToList();

            return Ok(eventos);
        }
        
        [HttpGet("categorias")]
        public IActionResult GetCategorias()
        {
            var categorias = _context.categorias
                .Select(c => new { c.idcategoria, c.nome })
                .ToList();

            return Ok(categorias);
        }
        
        [HttpGet("futuros")]
        [Authorize]
        public IActionResult GetEventosFuturos()
        {
            var hoje = DateTime.UtcNow.Date;
            var eventos = _context.eventos
                .Where(e => e.data >= hoje)
                .Select(e => new {
                    e.idevento,
                    e.nome,
                    e.descricao,
                    e.data,
                    e.hora,
                    e.local,
                    e.capacidade
                })
                .ToList();

            return Ok(eventos);
        }

        [HttpGet("passados")]
        [Authorize]
        public IActionResult GetEventosPassados()
        {
            var hoje = DateTime.UtcNow.Date;
            var eventos = _context.eventos
                .Where(e => e.data < hoje)
                .Select(e => new {
                    e.idevento,
                    e.nome,
                    e.descricao,
                    e.data,
                    e.hora,
                    e.local,
                    e.capacidade
                })
                .ToList();

            return Ok(eventos);
        }
        
    }
}