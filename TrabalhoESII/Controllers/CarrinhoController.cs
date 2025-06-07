using Microsoft.AspNetCore.Mvc;
using TrabalhoESII.Models;
using Microsoft.AspNetCore.Authorization;

namespace TrabalhoESII.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarrinhoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("/carrinho/adicionar")]
        public IActionResult AdicionarAoCarrinho([FromBody] Carrinho item)
        {
            var carrinho = HttpContext.Session.GetObjectFromJson<List<Carrinho>>("Carrinho") ?? new List<Carrinho>();
            carrinho.Add(item);
            HttpContext.Session.SetObjectAsJson("Carrinho", carrinho);

            return Ok("Item adicionado ao carrinho.");
        }

        [HttpGet("/carrinho")]
        public IActionResult VerCarrinho()
        {
            var carrinho = HttpContext.Session.GetObjectFromJson<List<Carrinho>>("Carrinho") ?? new List<Carrinho>();
            return View("VerCarrinho", carrinho);
        }

        [Authorize]
        [HttpPost("/carrinho/finalizar")]
        public async Task<IActionResult> FinalizarCompra()
        {
            var carrinho = HttpContext.Session.GetObjectFromJson<List<Carrinho>>("Carrinho");
            if (carrinho == null || !carrinho.Any())
                return RedirectToAction("Index", "Eventos");

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return RedirectToAction("Login", "Auth");

            foreach (var item in carrinho)
            {
                var pagamento = new pagamentos
{
    idutilizador = userId,
    idingresso = item.IdIngresso,
    idtipopagamento = 1, // assume um tipo padrão (por exemplo: "online")
    idestado = 1, // assume "pendente" ou "pago" conforme tua lógica
    datahora = DateTime.UtcNow,
    descricao = $"Compra do ingresso: {item.NomeIngresso}"
};

_context.pagamentos.Add(pagamento);

            }

            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Carrinho");

            return RedirectToAction("Index", "Eventos");
        }
    }
}
