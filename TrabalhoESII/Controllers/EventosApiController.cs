using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        // [Authorize] ← removido para permitir acesso sem autenticação
        public async Task<IActionResult> RegisterEvento([FromBody] EventosRegisterModel evento)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            // Criar o novo evento
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

            // Tentar obter o ID do utilizador autenticado (opcional)
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int utilizadorId))
            {
                var registoOrganizador = new organizadoreseventos
                {
                    idutilizador = utilizadorId,
                    idevento = novoEvento.idevento
                };

                _context.organizadoreseventos.Add(registoOrganizador);
                await _context.SaveChangesAsync();
            }

            return Ok(new { EventoId = novoEvento.idevento });
        }

        [HttpGet("search")]
        [Authorize]
        public IActionResult SearchEventos([FromQuery] string? nome, [FromQuery] DateTime? data, [FromQuery] string? local, [FromQuery] int? idCategoria)
        {
            var query = _context.eventos.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(e => EF.Functions.ILike(e.nome, $"%{nome}%"));

            if (data.HasValue)
                query = query.Where(e => e.data.Date == data.Value.Date);

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
        [HttpPut("{id}")] 
        //[Authorize]
        public async Task<IActionResult> EditEvento(int id, [FromBody] EventosRegisterModel evento)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var eventoExistente = await _context.eventos.FindAsync(id);

            if (eventoExistente == null)
                return NotFound("Evento não encontrado.");

            // Atualizar campos
            eventoExistente.nome = evento.nome;
            eventoExistente.descricao = evento.descricao;
            eventoExistente.data = DateTime.SpecifyKind(evento.data, DateTimeKind.Utc);
            eventoExistente.hora = evento.hora;
            eventoExistente.local = evento.local;
            eventoExistente.capacidade = evento.capacidade;
            eventoExistente.idcategoria = evento.idCategoria;

            _context.eventos.Update(eventoExistente);
            await _context.SaveChangesAsync();

            return Ok("Evento atualizado com sucesso.");
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            var evento = await _context.eventos.FindAsync(id);

            if (evento == null)
                return NotFound("Evento não encontrado.");

            // Remover possíveis relações em tabelas dependentes (como organizadores)
            var relacoes = _context.organizadoreseventos.Where(oe => oe.idevento == id);
            _context.organizadoreseventos.RemoveRange(relacoes);

            _context.eventos.Remove(evento);
            await _context.SaveChangesAsync();

            return Ok("Evento eliminado com sucesso.");
        }
    }
}
