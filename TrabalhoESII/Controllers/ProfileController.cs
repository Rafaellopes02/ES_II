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

            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(model.Nome) || model.Nome.Length < 2 || model.Nome.Length > 70)
                erros.Add("O nome deve ter entre 2 e 100 caracteres.");

            if (string.IsNullOrWhiteSpace(model.Email))
                erros.Add("O email é obrigatório.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                erros.Add("O email informado não é válido.");

            if (model.Idade < 18)
                erros.Add("A idade introduzida não é válida.");

            if (string.IsNullOrWhiteSpace(model.Telefone) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Telefone, @"^\d{9}$"))
            {
                erros.Add("O telefone deve conter apenas números e ter 9 dígitos.");
            }

            if (string.IsNullOrWhiteSpace(model.Nacionalidade))
                erros.Add("A nacionalidade é obrigatória.");

            if (!string.IsNullOrEmpty(NovaSenha) || !string.IsNullOrEmpty(ConfirmarSenha))
            {
                if (NovaSenha != ConfirmarSenha)
                    erros.Add("As senhas não coincidem.");
                else if (!string.IsNullOrEmpty(NovaSenha) && NovaSenha.Length < 8)
                    erros.Add("A nova senha deve ter pelo menos 8 caracteres.");
            }

            if (erros.Any())
            {
                ViewBag.StatusMessage = string.Join("<br>", erros);
                return View("EditUserProfile", model);
            }

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

            TempData["StatusMessage"] = "Perfil atualizado com sucesso!";
            return RedirectToAction("UserProfile", "Profile");
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
    }
}