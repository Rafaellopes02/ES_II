using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace TrabalhoESII.Controllers
{
    [ApiController]
    [Route("api/atividades")]
    public class AtividadesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AtividadesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("search-atividades")]
        [Authorize]
        public async Task<IActionResult> SearchAtividades(
            [FromQuery] int idevento)
        {
            var userIdClaim = User.FindFirst("UserId");
            int.TryParse(userIdClaim?.Value, out int userId);

            var query = _context.atividades
                .Where(a => a.idevento == idevento)
                .OrderBy(a => a.data)
                .ThenBy(a => a.hora) // Ordena também por hora, caso haja mais de uma atividade na mesma data
                .AsQueryable();

            var atividades = await query
                .Select(a => new
                {
                    a.idatividade,
                    a.nome,
                    data = a.data.ToString("yyyy-MM-dd"),
                    hora = a.hora.ToString(@"hh\:mm"),
                    a.quantidademaxima,
                    inscrito = _context.utilizadoresatividades
                        .Any(u => u.idatividade == a.idatividade && u.idutilizador == userId)
                })
                .ToListAsync();

            return Ok(atividades);
        }

        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> RegisterAtividade([FromBody] AtividadeRegisterModel novaAtividade)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var evento = await _context.eventos.FindAsync(novaAtividade.idevento);
            if (evento == null)
                return BadRequest("Evento não encontrado.");

            var dataAtividade = DateTime.SpecifyKind(novaAtividade.data.Date, DateTimeKind.Utc);
            var dataEvento = DateTime.SpecifyKind(evento.data.Date, DateTimeKind.Utc);

            if (dataAtividade < dataEvento)
                return BadRequest("A data da atividade não pode ser anterior à data do evento.");

            if (dataAtividade == dataEvento && novaAtividade.hora < evento.hora)
                return BadRequest("A hora da atividade não pode ser anterior à hora do evento.");
            
            if (novaAtividade.quantidademaxima > evento.capacidade)
                return BadRequest("A capacidade da atividade não pode ser superior à capacidade do evento.");

            var atividade = new atividades
            {
                nome = novaAtividade.nome,
                quantidademaxima = novaAtividade.quantidademaxima,
                data = dataAtividade,
                hora = novaAtividade.hora,
                idevento = novaAtividade.idevento
            };

            _context.atividades.Add(atividade);
            await _context.SaveChangesAsync();

            return Ok(new { id = atividade.idatividade });
        }
        [HttpPost("{id}/inscrever")]
        [Authorize]
        public async Task<IActionResult> InscreverNaAtividade(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var atividade = await _context.atividades.FindAsync(id);
            if (atividade == null)
                return NotFound("Atividade não encontrada.");

            var jaInscrito = await _context.utilizadoresatividades
                .AnyAsync(a => a.idatividade == id && a.idutilizador == userId);

            if (jaInscrito)
                return BadRequest("Já está inscrito nesta atividade.");

            var novaInscricao = new utilizadoresatividades
            {
                idutilizador = userId,
                idatividade = id,
                idevento = atividade.idevento
            };

            _context.utilizadoresatividades.Add(novaInscricao);
            await _context.SaveChangesAsync();

            return Ok("Inscrição realizada com sucesso.");
        }
        
        [HttpPost("{id}/cancelar-inscricao")]
        [Authorize]
        public async Task<IActionResult> CancelarInscricaoNaAtividade(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var inscricao = await _context.utilizadoresatividades
                .FirstOrDefaultAsync(a => a.idatividade == id && a.idutilizador == userId);

            if (inscricao == null)
                return NotFound("Não está inscrito nesta atividade.");

            _context.utilizadoresatividades.Remove(inscricao);
            await _context.SaveChangesAsync();

            return Ok("Inscrição cancelada com sucesso.");
        }
    }
}