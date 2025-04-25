using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;
using Microsoft.AspNetCore.Authorization;

namespace TrabalhoESII.Controllers
{
    [ApiController]
    [Route("api/perfil")]
    public class PerfilUtilizadorApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PerfilUtilizadorApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPerfil()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized("Utilizador não autenticado ou token inválido.");
            
            var utilizador = await _context.utilizadores
                .FirstOrDefaultAsync(u => u.idutilizador == userId);

            if (utilizador == null)
                return NotFound("Utilizador não encontrado.");
            
            return Ok(new
            {
                Id = utilizador.idutilizador,
                Nome = utilizador.nome,
                Email = utilizador.email,
                Idade = utilizador.idade,
                Telefone = utilizador.telefone,
                Nacionalidade = utilizador.nacionalidade,
                NomeUtilizador = utilizador.nomeutilizador,
                IdTipoUtilizador = utilizador.idtipoutilizador
            });
        }


        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdatePerfil([FromBody] PerfilUtilizadorModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");
            
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized("Utilizador não autenticado ou token inválido.");
            
            var utilizador = await _context.utilizadores.FindAsync(userId);
            if (utilizador == null)
                return NotFound("Utilizador não encontrado.");
            
            utilizador.nome = model.Nome;
            utilizador.email = model.Email;
            utilizador.idade = model.Idade;
            utilizador.telefone = model.Telefone;
            utilizador.nacionalidade = model.Nacionalidade;
            utilizador.nomeutilizador = model.NomeUtilizador;

            if (!string.IsNullOrEmpty(model.Senha))
            {
                utilizador.senha = BCrypt.Net.BCrypt.HashPassword(model.Senha);
            }
            
            var tipoUtilizador = ObterTipoUtilizadorDoToken();
            if (tipoUtilizador == 1)
            {
                utilizador.idtipoutilizador = model.IdTipoUtilizador;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Perfil atualizado com sucesso.");
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Erro ao atualizar o perfil: {ex.Message}");
            }
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