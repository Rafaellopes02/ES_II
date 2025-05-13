using Microsoft.AspNetCore.Authorization;
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

        // POST: api/auth/Register
        [HttpPost("Register")]
        public IActionResult Register([FromBody] LoginRegisterModel model)
        {
            if (_context.utilizadores.Any(u => u.email.ToLower() == model.Email.ToLower()))
                return BadRequest("Email já está em uso.");

            if (_context.utilizadores.Any(u => u.nomeutilizador.ToLower() == model.NomeUtilizador.ToLower()))
                return BadRequest("Nome de utilizador já está em uso.");

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

        // POST: api/auth/Login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null)
                return BadRequest("Requisição inválida.");

            var user = _context.utilizadores
                .FirstOrDefault(u => u.nomeutilizador == model.NomeUtilizador);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Senha, user.senha))
                return Unauthorized("Credenciais inválidas.");

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

            Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(2)
            });

            return Ok(new { message = "Login efetuado com sucesso" });
        }
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null) return Unauthorized();

            var userId = identity.FindFirst("UserId")?.Value;
            var tipoId = identity.FindFirst("TipoUtilizadorId")?.Value;

            return Ok(new
            {
                userId = userId,
                tipoUtilizador = tipoId
            });
        }
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwtToken");
            return Ok(new { message = "Logout efetuado com sucesso." });
        }

        
    }

    // === Token Builder para gerar JWT ===
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

    // === Modelos auxiliares ===
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
