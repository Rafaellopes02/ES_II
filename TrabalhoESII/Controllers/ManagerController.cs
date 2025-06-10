// Caminho: Controllers/AdminController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrabalhoESII.Controllers
{
    [Authorize(Policy = "ManagerOnly")] // Apenas administradores podem aceder
    public class ManagerController : Controller
    {
        public IActionResult Painel()
        {
            return View(); // Vai procurar a view em /Views/Admin/Painel.cshtml
        }
    }
}