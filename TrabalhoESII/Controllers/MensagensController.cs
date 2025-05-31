using Microsoft.AspNetCore.Mvc;
using TrabalhoESII.Models;
using System.Collections.Generic;
using System.Linq;

namespace TrabalhoESII.Controllers
{
    public class MensagensController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MensagensController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult EnviarParaSelecionados(int eventoId, string conteudo, List<int> destinatarios)
        {
            if (string.IsNullOrWhiteSpace(conteudo) || destinatarios == null || !destinatarios.Any())
            {
                TempData["Erro"] = "Mensagem ou destinatários inválidos.";
                return RedirectToAction("Detalhes", "EventoDetalhes", new { id = eventoId });
            }

            foreach (var userId in destinatarios)
            {
                _context.Mensagens.Add(new Mensagem
                {
                    EventoId = eventoId,
                    Conteudo = conteudo,
                    DestinatarioId = userId
                });
            }

            _context.SaveChanges();
            TempData["MensagemEnviada"] = "Mensagem enviada com sucesso!";
            return RedirectToAction("Detalhes", "EventoDetalhes", new { id = eventoId });
        }
    }
}
