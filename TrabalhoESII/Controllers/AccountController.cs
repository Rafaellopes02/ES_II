using Microsoft.AspNetCore.Mvc;

namespace TrabalhoESII.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }
    }
}