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

