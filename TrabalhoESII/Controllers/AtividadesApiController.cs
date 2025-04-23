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
                .AsQueryable();

            var atividades = await query
                .Select(a => new
                {
                    a.idatividade,
                    a.nome,
                    data = a.data.ToString("yyyy-MM-dd"),
                    hora = a.hora.ToString(), // .ToString("hh\\:mm") se quiseres formatado
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

            var atividade = new atividades // 👈 muda aqui de "atividade" para "atividades"
            {
                nome = novaAtividade.nome,
                quantidademaxima = novaAtividade.quantidademaxima,
                data = DateTime.SpecifyKind(novaAtividade.data, DateTimeKind.Utc),
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

            // Verifica se já está inscrito
            var jaInscrito = await _context.utilizadoresatividades
                .AnyAsync(a => a.idatividade == id && a.idutilizador == userId);

            if (jaInscrito)
                return BadRequest("Já está inscrito nesta atividade.");

            // Inscrever
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

    }
}