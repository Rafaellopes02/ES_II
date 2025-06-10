using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;

[ApiController]
[Route("api/manager")]
[Authorize(Policy = "ManagerOnly")] // Apenas UserManagers autenticados
public class ManagerApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ManagerApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var utilizadores = await _context.utilizadores
            .Where(u => u.idtipoutilizador != 1) // 👈 Oculta administradores da lista
            .Select(u => new {
                u.idutilizador,
                u.nome,
                u.nomeutilizador,
                u.email,
                u.telefone,
                u.idtipoutilizador
            }).ToListAsync();

        return Ok(utilizadores);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> EditarParcial(int id, [FromBody] JsonElement dados)
    {
        var user = await _context.utilizadores.FindAsync(id);
        if (user == null) return NotFound("Utilizador não encontrado.");

        if (user.idtipoutilizador == 1)
            return Forbid("Não é permitido editar utilizadores do tipo Administrador.");

        if (dados.TryGetProperty("nome", out var nome))
            user.nome = nome.GetString();

        if (dados.TryGetProperty("email", out var email))
            user.email = email.GetString();

        if (dados.TryGetProperty("telefone", out var telefone))
            user.telefone = telefone.GetString();

        if (dados.TryGetProperty("nomeutilizador", out var username))
            user.nomeutilizador = username.GetString();

        if (dados.TryGetProperty("idtipoutilizador", out var tipo))
            user.idtipoutilizador = tipo.GetInt32();

        await _context.SaveChangesAsync();
        return Ok("Utilizador atualizado parcialmente.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.utilizadores.FindAsync(id);
        if (user == null)
            return NotFound("Utilizador não encontrado.");

        if (user.idtipoutilizador == 1)
            return Forbid("Não é permitido apagar utilizadores do tipo Administrador.");

        var idAtual = int.TryParse(User.FindFirst("UserId")?.Value, out var uid) ? uid : 0;
        if (id == idAtual)
            return BadRequest("Não pode eliminar a sua própria conta.");

        var relacoes = _context.organizadoreseventos.Where(o => o.idutilizador == id);
        _context.organizadoreseventos.RemoveRange(relacoes);

        _context.utilizadores.Remove(user);
        await _context.SaveChangesAsync();

        return Ok("Utilizador eliminado com sucesso.");
    }

    [HttpPatch("{id}/redefinir-senha")]
    public async Task<IActionResult> RedefinirSenha(int id, [FromBody] RedefinirSenhaRequest request)
    {
        var user = await _context.utilizadores.FindAsync(id);
        if (user == null) return NotFound();

        if (user.idtipoutilizador == 1)
            return Forbid("Não é permitido redefinir senha de administradores.");

        user.senha = BCrypt.Net.BCrypt.HashPassword(request.novaSenha);
        await _context.SaveChangesAsync();
        return Ok("Senha atualizada");
    }

    public class RedefinirSenhaRequest
    {
        public string novaSenha { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] Utilizador novo)
    {
        try
        {
            if (novo.IdTipoUtilizador == 1)
                return Forbid("Não é permitido criar novos utilizadores do tipo Administrador.");

            var utilizador = new utilizadores
            {
                nome = novo.Nome,
                email = novo.Email,
                idade = novo.Idade,
                telefone = novo.Telefone,
                nacionalidade = novo.Nacionalidade,
                nomeutilizador = novo.NomeUtilizador,
                senha = BCrypt.Net.BCrypt.HashPassword(novo.Senha),
                idtipoutilizador = novo.IdTipoUtilizador
            };

            _context.utilizadores.Add(utilizador);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Utilizador criado com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest("Erro ao criar utilizador: " + ex.Message);
        }
    }
}
