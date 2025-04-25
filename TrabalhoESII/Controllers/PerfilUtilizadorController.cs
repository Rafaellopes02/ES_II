using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrabalhoESII.Controllers
{
    public class PerfilUtilizadorController : Controller
    {
        // Rota principal para "Meu Perfil"
        [AllowAnonymous]
        [HttpGet("/perfil")]
        public IActionResult PerfilUtilizador()
        {
            return View("perfilutilizador");
        }
    }
}