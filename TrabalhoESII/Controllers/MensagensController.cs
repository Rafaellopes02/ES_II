using Microsoft.AspNetCore.Mvc;
using TrabalhoESII.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


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

            var remetenteId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);

            foreach (var userId in destinatarios)
            {
                _context.Mensagens.Add(new Mensagem
                {
                    EventoId = eventoId,
                    Conteudo = conteudo,
                    DestinatarioId = userId,
                    RemetenteId = remetenteId
                });
            }

            _context.SaveChanges();
            TempData["MensagemEnviada"] = "Mensagem enviada com sucesso!";
            return RedirectToAction("Detalhes", "EventoDetalhes", new { id = eventoId });
        }

        [HttpGet]
        public IActionResult MinhasMensagens()
        {
            // Garantir que o utilizador está autenticado e tem a claim UserId
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            //Procurar mensagens
            var mensagens = _context.Mensagens
                .Include(m => m.Remetente) // incluir info do remetente
                .Where(m => m.DestinatarioId == userId)
                .OrderByDescending(m => m.DataEnvio)
                .ToList();

            return View(mensagens);
        }

    }
}
