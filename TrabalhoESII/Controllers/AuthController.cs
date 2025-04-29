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
            if (_context.utilizadores
                .Any(u => u.email.ToLower() == model.Email.ToLower()))
            {
                return BadRequest("Email já está em uso.");
            }

            {
                if (_context.utilizadores
                    .Any(u => u.nomeutilizador.ToLower() == model.NomeUtilizador.ToLower()))
                {
                    return BadRequest("Nome de utilizador já está em uso.");
                }
                
                var user = new utilizadores
                {
                    nome = model.Nome,
                    nomeutilizador = model.NomeUtilizador,
                    senha = BCrypt.Net.BCrypt.HashPassword(model.Senha),
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

            var user = _context.utilizadores
                .FirstOrDefault(u => u.nomeutilizador == model.NomeUtilizador);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Senha, user.senha))
            {
                return Unauthorized("Credenciais inválidas.");
            }
            
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            );
            
            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.nomeutilizador),
                new Claim("UserId", user.idutilizador.ToString()),
                new Claim("TipoUtilizadorId", user.idtipoutilizador.ToString())
            });

            var token = new JwtTokenBuilder()
                .SetSubject(claims)
                .SetExpires(DateTime.UtcNow.AddHours(2))
                .SetSigningCredentials(signingCredentials)
                .Build();
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }
    
    public class JwtTokenBuilder
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly SecurityTokenDescriptor _tokenDescriptor;

        public JwtTokenBuilder()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
            _tokenDescriptor = new SecurityTokenDescriptor();
        }

        public JwtTokenBuilder SetSubject(ClaimsIdentity subject)
        {
            _tokenDescriptor.Subject = subject;
            return this;
        }

        public JwtTokenBuilder SetExpires(DateTime expires)
        {
            _tokenDescriptor.Expires = expires;
            return this;
        }

        public JwtTokenBuilder SetSigningCredentials(SigningCredentials signingCredentials)
        {
            _tokenDescriptor.SigningCredentials = signingCredentials;
            return this;
        }

        public SecurityToken Build()
        {
            return _tokenHandler.CreateToken(_tokenDescriptor);
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
        public int IdTipoUtilizador { get; set; }
    }

    public class LoginModel
    {
        public string NomeUtilizador { get; set; }
        public string Senha { get; set; }
    }
}
