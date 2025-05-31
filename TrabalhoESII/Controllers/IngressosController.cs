using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrabalhoESII.Models;

namespace TrabalhoESII.Controllers
{
    [Authorize]
    public class IngressosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngressosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Comprar(int id)
        {
            var evento = _context.eventos.FirstOrDefault(e => e.idevento == id);

            if (evento == null)
            {
                return NotFound("Evento não encontrado.");
            }

            var ingressos = _context.ingressos
                .Where(i => i.idevento == id && i.quantidadeatual > 0)
                .Join(_context.tiposingressos,
                    i => i.idtipoingresso,
                    t => t.idtipoingresso,
                    (i, t) => new IngressoDTO
                    {
                        idtipoingresso = i.idingresso,
                        nomeingresso = t.nome,
                        preco = i.preco
                    })
                .ToList();

            ViewBag.Evento = evento;
            ViewBag.Ingressos = ingressos;

            Console.WriteLine("Ingressos carregados: " + ingressos.Count);
            return View();
        }

[HttpPost]
public IActionResult FinalizarCompra(int idIngresso, string metodoPagamento)
{
    
    var userIdClaim = User.FindFirst("UserId");
    if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
    {
        return RedirectToAction("Login", "Auth");
    }

    var ingresso = _context.ingressos.FirstOrDefault(i => i.idingresso == idIngresso);

    if (ingresso == null || ingresso.quantidadeatual <= 0)
    {
        TempData["ErroCompra"] = "Ingresso inválido ou indisponível.";
        return RedirectToAction("Index", "Eventos");
    }
    var jaExiste = _context.utilizadoreseventos.Any(ue =>
        ue.idutilizador == userId && ue.idevento == ingresso.idevento
    );

    if (jaExiste)
    {
        TempData["ErroCompra"] = "Já está inscrito neste evento.";
        return RedirectToAction("Index", "Eventos");
    }


    // Registar a inscrição (podes ajustar a tua tabela de inscrições aqui se necessário)
            var novaInscricao = new utilizadoreseventos
            {
                idutilizador = userId,
                idevento = ingresso.idevento,
                idingresso = idIngresso,
                estado = "Confirmado",
                datainscricao = DateTime.UtcNow,
                
    };

    // Reduzir quantidade disponível
    ingresso.quantidadeatual--;

    _context.ingressos.Update(ingresso);
    _context.utilizadoreseventos.Add(novaInscricao);
    _context.SaveChanges();

    // Redirecionar para a lista de eventos
    return RedirectToAction("Index", "Eventos");
}

    }
}