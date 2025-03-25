using Microsoft.AspNetCore.Mvc;
using TrabalhoESII.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
/*
namespace TrabalhoESII.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login(string nomeUtilizador, string senha)
        {
            var user = _context.utilizadores
                .FirstOrDefault(u => u.nomeutilizador == nomeUtilizador && u.senha == senha);

            if (user != null)
            {
                // 🔐 Gerar token JWT
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.nomeutilizador),
                    new Claim("UserId", user.idutilizador.ToString()),
                    new Claim("TipoUtilizadorId", user.idtipoutilizador.ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // 🔽 Passa o token para a view usando ViewBag
                ViewBag.Token = tokenString;
                ViewBag.UserType = user.tiposutilizadores.nome;

                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.ErrorMessage = "Credenciais inválidas.";
            return View();
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }
    }
}*/
using Microsoft.AspNetCore.Mvc;

namespace TrabalhoESII.Controllers
{
    public class AccountController : Controller
    {
        // Mostra a página de login
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View(); // Aponta para Views/Account/Login.cshtml
        }

        // Mostra a página de registo
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View(); // Aponta para Views/Account/Register.cshtml
        }
    }
}

