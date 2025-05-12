using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrabalhoESII.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TrabalhoESII.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("/profile")]
        public IActionResult UserProfile()
        {
            return View("UserProfile");
        }

        [Authorize]
        [HttpGet("/profile/getuserprofile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized("Utilizador não identificado.");

            var user = await _context.utilizadores
                .FirstOrDefaultAsync(u => u.idutilizador == userId);

            if (user == null)
                return NotFound("Perfil não encontrado.");

            var userProfile = new
            {
                nome = user.nome,
                email = user.email,
                idade = user.idade,
                telefone = user.telefone,
                nacionalidade = user.nacionalidade
            };

            return Json(userProfile);
        }
        
        [Authorize]
        [HttpGet("/profile/edit")]
        public async Task<IActionResult> EditUserProfile()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var user = await _context.utilizadores.FirstOrDefaultAsync(u => u.idutilizador == userId);
            if (user == null)
                return NotFound();

            var model = new ProfileModel
            {
                Nome = user.nome,
                Email = user.email,
                Idade = user.idade,
                Telefone = user.telefone,
                Nacionalidade = user.nacionalidade,
                NomeUtilizador = user.nomeutilizador
            };

            return View("EditUserProfile", model);
        }

        [Authorize]
        [HttpPost("/profile/edit")]
        public async Task<IActionResult> EditUserProfile(ProfileModel model, string NovaSenha, string ConfirmarSenha)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var user = await _context.utilizadores.FirstOrDefaultAsync(u => u.idutilizador == userId);
            if (user == null)
                return NotFound();

            user.nome = model.Nome;
            user.email = model.Email;
            user.idade = model.Idade;
            user.telefone = model.Telefone;
            user.nacionalidade = model.Nacionalidade;

            if (!string.IsNullOrEmpty(NovaSenha) && NovaSenha == ConfirmarSenha)
            {
                user.senha = BCrypt.Net.BCrypt.HashPassword(NovaSenha);
            }

            await _context.SaveChangesAsync();

            ViewBag.StatusMessage = "Perfil atualizado com sucesso!";
            return View("EditUserProfile", model);
        }
    }
}