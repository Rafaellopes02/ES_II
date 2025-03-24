using Microsoft.AspNetCore.Mvc;
using TrabalhoESII.Models;

namespace TrabalhoESII.Controllers
{
    public class AccountController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login(string nomeUtilizador, string senha)
        {
            var user = _context.utilizadores.FirstOrDefault(u => u.nomeutilizador == nomeUtilizador && u.senha == senha);
            if (user != null)
            {
                return RedirectToAction("Index", "Dashboard", new { userType = user.tiposutilizadores.nome });
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
}