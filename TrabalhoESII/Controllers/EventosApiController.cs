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
        
        [HttpGet("detalhes/{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetalhes(int id)
        {
            var evento = await _context.eventos
                .Include(e => e.categoria) // se tiver navegação configurada
                .FirstOrDefaultAsync(e => e.idevento == id);

            if (evento == null)
                return NotFound("Evento não encontrado.");

            return Ok(new
            {
                evento.idevento,
                evento.nome,
                evento.descricao,
                evento.data,
                evento.hora,
                evento.local,
                evento.capacidade,
                evento.idcategoria,
                categoriaNome = evento.categoria?.nome // só se tiver relação com categorias
            });
        }

        [HttpGet("search")]
        [Authorize]
        public IActionResult SearchEventos(
            [FromQuery] string? nome,
            [FromQuery] DateTime? data,
            [FromQuery] string? local,
            [FromQuery] int? idCategoria)
        {
            var query = _context.eventos
                .Include(e => e.categoria)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(e => EF.Functions.ILike(e.nome, $"%{nome}%"));

            if (data.HasValue)
            {
                var dataUtc = DateTime.SpecifyKind(data.Value, DateTimeKind.Utc);
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
                    e.capacidade,
                    categoria = e.categoria.nome // Nome da categoria em vez de ID
                    
                })
                .ToList();

            return Ok(eventos);
        }

        
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditEvento(int id, [FromBody] EventosRegisterModel evento)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var tipoUtilizador = ObterTipoUtilizadorDoToken();

            // Verificar se o utilizador é o criador do evento
            var eCriador = await _context.organizadoreseventos
                .AnyAsync(o => o.idevento == id && o.idutilizador == userId);

            // Apenas o criador OU admin (tipo 1) pode editar
            if (!eCriador && tipoUtilizador != 1)
                return BadRequest("Não tem permissões para editar ou eliminar este evento. Apenas o criador pode fazê-lo.");

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

            await _context.SaveChangesAsync();

            return Ok("Evento atualizado com sucesso.");
        }


        
        [HttpGet("categorias")]
        public IActionResult GetCategorias()
        {
            var categorias = _context.categorias
                .Select(c => new
                {
                    c.idcategoria,
                    c.nome
                })
                .ToList();

            return Ok(categorias);
        }

        
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var tipoUtilizador = ObterTipoUtilizadorDoToken(); // 👈 usar a função que criaste

            // Verificar se o utilizador é o criador do evento
            var eCriador = await _context.organizadoreseventos
                .AnyAsync(o => o.idevento == id && o.idutilizador == userId);

            // Se não for criador e também não for admin, não tem permissões
            if (!eCriador && tipoUtilizador != 1)
                return BadRequest("Não tem permissões para editar ou eliminar este evento. Apenas o criador pode fazê-lo.");

            var evento = await _context.eventos.FindAsync(id);
            if (evento == null)
                return NotFound("Evento não encontrado.");

            // Apagar a relação com o organizador
            var relacoes = _context.organizadoreseventos.Where(oe => oe.idevento == id);
            _context.organizadoreseventos.RemoveRange(relacoes);

            _context.eventos.Remove(evento);
            await _context.SaveChangesAsync();

            return Ok("Evento eliminado com sucesso.");
        }

        
        private int? ObterTipoUtilizadorDoToken()
        {
            var claim = User.FindFirst("TipoUtilizadorId");
            if (claim != null && int.TryParse(claim.Value, out int tipo))
            {
                return tipo;
            }

            return null;
        }

    }
    
}


