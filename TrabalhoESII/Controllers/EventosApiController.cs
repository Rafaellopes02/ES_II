using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TrabalhoESII.Models;

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
                return BadRequest(ModelState);

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

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int utilizadorId))
            {
                var registoOrganizador = new organizadoreseventos
                {
                    idutilizador = utilizadorId,
                    idevento = novoEvento.idevento,
                    eorganizador = true
                };

                _context.organizadoreseventos.Add(registoOrganizador);               
            }

foreach (var ingresso in evento.ingressos)
    {
        _context.ingressos.Add(new ingressos
        {
            nomeingresso = ingresso.nomeingresso,
            idtipoingresso = ingresso.idtipoingresso,
            quantidadedefinida = ingresso.quantidadedefinida,
            quantidadeatual = ingresso.quantidadedefinida,
            preco = ingresso.preco,
            idevento = novoEvento.idevento
        });
    }
            await _context.SaveChangesAsync();
            return Ok(novoEvento); // <-- Alteração principal: retorna o objeto com o ID
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
            
            var inscritos = await _context.organizadoreseventos
                .CountAsync(oe => oe.idevento == id && !oe.eorganizador);


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
                categoriaNome = evento.categoria?.nome, // só se tiver relação com categorias
                inscritos
            });
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchEventos(
            [FromQuery] string? nome,
            [FromQuery] DateTime? data,
            [FromQuery] string? local,
            [FromQuery] int? idCategoria)
        {
            var userIdClaim = User.FindFirst("UserId");
            int.TryParse(userIdClaim?.Value, out int userId);

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

            var eventos = await query
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
                    inscrito = _context.utilizadoreseventos.Any(o => o.idevento == e.idevento && o.idutilizador == userId ),
                    eorganizador = _context.organizadoreseventos
                        .Any(o => o.idevento == e.idevento && o.idutilizador == userId && o.eorganizador), // ✔ seguro como bool
                    idutilizador = _context.organizadoreseventos
                        .Where(o => o.idevento == e.idevento && o.eorganizador)
                        .Select(o => o.idutilizador)
                        .FirstOrDefault(),
                    inscritos = _context.organizadoreseventos
                        .Count(o => o.idevento == e.idevento && !o.eorganizador),  
                    jaComprouIngresso = _context.organizadoreseventos
                         .Any(u => u.idevento == e.idevento && u.idutilizador == userId && !u.eorganizador),                   
                })
                .ToListAsync();

            return Ok(eventos);
        }

        [HttpGet("{idevento}/verificar-compra")]
        [Authorize]
        public IActionResult VerificarCompra(int idevento)
    {
         var userIdClaim = User.FindFirst("UserId");
         if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
             return Unauthorized();

         bool comprou = _context.utilizadoreseventos.Any(ue =>
                ue.idevento == idevento &&
                ue.idutilizador == userId &&
                ue.estado == "Confirmado");

         return Ok(new { comprou });
}
        [HttpGet("{id}/inscritos")]
        [Authorize]
        public async Task<IActionResult> ObterInscritos(int id)
        {
            var evento = await _context.eventos.FindAsync(id);
            if (evento == null)
                return NotFound("Evento não encontrado.");

            var inscritos = await _context.organizadoreseventos
                .Where(o => o.idevento == id && o.eorganizador == false)
                .Include(o => o.utilizadores) // garantir que os dados do utilizador são carregados
                .Select(o => new
                {
                    nome = o.utilizadores.nome,
                    email = o.utilizadores.email
                })
                .ToListAsync();

            return Ok(inscritos);
        }

        
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditEvento(int id, [FromBody] EventosRegisterModel evento)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var tipoUtilizador = ObterTipoUtilizadorDoToken();

            var eCriador = await _context.organizadoreseventos
                .AnyAsync(o => o.idevento == id && o.idutilizador == userId && o.eorganizador);

            if (!eCriador && tipoUtilizador != 1)
                return BadRequest("Não tem permissões para editar ou eliminar este evento. Apenas o criador ou um administrador pode fazê-lo.");

            var eventoExistente = await _context.eventos.FindAsync(id);
            if (eventoExistente == null)
                return NotFound("Evento não encontrado.");
            
            bool dataAlterada = eventoExistente.data != DateTime.SpecifyKind(evento.data, DateTimeKind.Utc);
            bool localAlterado = eventoExistente.local != evento.local;
            bool nomeAlterado = eventoExistente.nome != evento.nome;

            eventoExistente.nome = evento.nome;
            eventoExistente.descricao = evento.descricao;
            eventoExistente.data = DateTime.SpecifyKind(evento.data, DateTimeKind.Utc);
            eventoExistente.hora = evento.hora;
            eventoExistente.local = evento.local;
            eventoExistente.capacidade = evento.capacidade;
            eventoExistente.idcategoria = evento.idCategoria;

            await _context.SaveChangesAsync();
            if (dataAlterada || localAlterado || nomeAlterado)
            {
                await CriarNotificacoesParaInscritos(id, eventoExistente, dataAlterada, localAlterado, nomeAlterado);
            }
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

            var tipoUtilizador = ObterTipoUtilizadorDoToken(); // 1 = admin

            // Verifica se o utilizador atual é o criador (eorganizador = true)
            var eCriador = await _context.organizadoreseventos
                .AnyAsync(o => o.idevento == id && o.idutilizador == userId && o.eorganizador);

            // Se não for criador nem admin, não pode apagar
            if (!eCriador && tipoUtilizador != 1)
                return BadRequest("Não tem permissões para eliminar este evento. Apenas o criador ou um administrador pode fazê-lo.");

            var evento = await _context.eventos.FindAsync(id);
            if (evento == null)
                return NotFound("Evento não encontrado.");
            
            var participantes = await _context.organizadoreseventos
                .Where(oe => oe.idevento == id && !oe.eorganizador)
                .Select(oe => oe.idutilizador)
                .ToListAsync();

            foreach (var idUtilizador in participantes)
            {
                var notificacao = new notificacoes
                {
                    idutilizador = idUtilizador,
                    mensagem = $"O evento {evento.nome} foi cancelado."
                };
                _context.notificacoes.Add(notificacao);
            }

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
        
        [HttpPost("{id}/inscrever")]
        [Authorize]
        public async Task<IActionResult> Inscrever(int id, [FromBody] InscricaoRequest request)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var evento = await _context.eventos.FindAsync(id);
            if (evento == null)
                return NotFound("Evento não encontrado.");

            if (evento.data.Date < DateTime.UtcNow.Date)
                return BadRequest("Evento já decorreu.");

            bool jaInscrito = await _context.organizadoreseventos
                .AnyAsync(o => o.idevento == id && o.idutilizador == userId);
            if (jaInscrito)
            {
                    
var existente = await _context.organizadoreseventos
        .FirstOrDefaultAsync(o => o.idevento == id && o.idutilizador == userId);

    if (existente?.eorganizador == true)
        return BadRequest("Já é organizador deste evento e não pode inscrever-se como participante.");
else
    return BadRequest("Já está inscrito.");

                }

            // ✅ Usar a propriedade recebida do JSON
            var ingresso = await _context.ingressos.FindAsync(request.idingresso);
            if (ingresso == null || ingresso.idevento != id)
                return BadRequest("Ingresso inválido para este evento.");

            if (ingresso.quantidadeatual <= 0)
                return BadRequest("Não há mais disponibilidade para este ingresso.");

            var novaInscricao = new organizadoreseventos
            {
                idevento = id,
                idutilizador = userId,
                eorganizador = false
            };
            _context.organizadoreseventos.Add(novaInscricao);

            ingresso.quantidadeatual -= 1;

            await _context.SaveChangesAsync();

            return Ok("Inscrição realizada com sucesso!");
        }
        
        [Authorize]
        [HttpPost("/api/eventos/{id}/cancelar")]
        public async Task<IActionResult> CancelarInscricao(int id)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Forbid();

            var inscricao = await _context.organizadoreseventos
                .FirstOrDefaultAsync(oe => oe.idevento == id && oe.idutilizador == userId && !oe.eorganizador);

            if (inscricao == null)
                return NotFound("Inscrição não encontrada.");
            
            var ingresso = await _context.ingressos
                .FirstOrDefaultAsync(i => i.idevento == id && i.quantidadeatual < i.quantidadedefinida);

            if (ingresso != null)
            {
                ingresso.quantidadeatual++;
                _context.ingressos.Update(ingresso);
            }
            
            var atividadesIds = await _context.atividades
                .Where(a => a.idevento == id)
                .Select(a => a.idatividade)
                .ToListAsync();
            
            var inscricoesAtividades = _context.utilizadoresatividades
                .Where(ua => atividadesIds.Contains(ua.idatividade) && ua.idutilizador == userId);
            
            _context.utilizadoresatividades.RemoveRange(inscricoesAtividades);
            
            _context.organizadoreseventos.Remove(inscricao);
            await _context.SaveChangesAsync();

            return Ok("Inscrição cancelada com sucesso.");
        }
        
        private async Task CriarNotificacoesParaInscritos(int idevento, eventos evento, bool dataAlterada, bool localAlterado, bool nomeAlterado)
        {
            var participantes = await _context.organizadoreseventos
                .Where(oe => oe.idevento == idevento && !oe.eorganizador)
                .Select(oe => oe.idutilizador)
                .ToListAsync();

            if (!participantes.Any()) return;

            var mensagem = "O evento " + evento.nome + " foi alterado: ";
            var alteracoes = new List<string>();
    
            if (nomeAlterado) alteracoes.Add("nome");
            if (dataAlterada) alteracoes.Add("data");
            if (localAlterado) alteracoes.Add("localização");
    
            mensagem += string.Join(", ", alteracoes) + ".";

            foreach (var idUtilizador in participantes)
            {
                var notificacao = new notificacoes
                {
                    idutilizador = idUtilizador,
                    mensagem = mensagem
                };

                _context.notificacoes.Add(notificacao);
            }

            await _context.SaveChangesAsync();
        }
        
        [HttpGet("ingressos/por-evento/{idevento}")]
        [Authorize]
        public IActionResult GetIngressosPorEvento(int idevento)
        {
            var ingressos = _context.ingressos
                .Where(i => i.idevento == idevento)
                .Select(i => new {
                    i.idingresso,
                    i.nomeingresso,
                    i.preco
                })
                .ToList();

            return Ok(ingressos);
        }

    }
}


