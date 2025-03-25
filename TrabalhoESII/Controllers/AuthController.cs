using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TrabalhoESII.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TrabalhoESII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Endpoint para registar um novo utilizador.
        /// </summary>
        [HttpPost("Register")]
        public IActionResult Register([FromBody] LoginRegisterModel model)
        {
            if (_context.utilizadores.Any(u => u.nomeutilizador == model.NomeUtilizador))
            {
                return BadRequest("Nome de utilizador já existe.");
            }

            var user = new utilizadores
            {
                nome = model.Nome,
                nomeutilizador = model.NomeUtilizador,
                senha = BCrypt.Net.BCrypt.HashPassword(model.Senha), // Encriptar senha
                email = model.Email,
                nacionalidade = model.Nacionalidade,
                idade = model.Idade,
                telefone = model.Telefone,
                idtipoutilizador = model.IdTipoUtilizador
            };

            _context.utilizadores.Add(user);
            _context.SaveChanges();

            return Ok("Utilizador registado com sucesso!");
        }

        /// <summary>
        /// Endpoint para login e geração de token JWT.
        /// </summary>
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null)
            {
                return BadRequest("Requisição inválida.");
            }

            // Buscar o utilizador na base de dados
            var user = _context.utilizadores.FirstOrDefault(u => u.nomeutilizador == model.NomeUtilizador);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Senha, user.senha))
            {
                return Unauthorized("Credenciais inválidas.");
            }

            // Criar token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.nomeutilizador),
                    new Claim("UserId", user.idutilizador.ToString()),
                    new Claim("TipoUtilizadorId", user.idtipoutilizador.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }

    public class LoginRegisterModel
    {
        public string Nome { get; set; }
        public string NomeUtilizador { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public string Nacionalidade { get; set; }
        public int Idade { get; set; }
        public string Telefone { get; set; }
        public int IdTipoUtilizador { get; set; } // Alterado para corresponder à chave estrangeira
    }

    public class LoginModel
    {
        public string NomeUtilizador { get; set; }
        public string Senha { get; set; }
    }
}
