using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;
using Microsoft.AspNetCore.Authorization;

namespace TrabalhoESII.Controllers
{
    [Authorize]
    public class MensagensController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MensagensController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Enviar(int eventoId, int destinatarioId, string conteudo)
        {
            int remetenteId = int.Parse(User.FindFirst("UserId").Value);

            var mensagem = new Mensagem
            {
                EventoId = eventoId,
                DestinatarioId = destinatarioId,
                RemetenteId = remetenteId,
                Conteudo = conteudo
            };

            _context.Mensagens.Add(mensagem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Eventos");
        }

        [Authorize]
        public async Task<IActionResult> MinhasMensagens()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return RedirectToAction("Login", "Account");

            var mensagens = await _context.Mensagens
                .Include(m => m.Remetente)
                .Include(m => m.Evento)
                .Where(m => m.DestinatarioId == userId)
                .OrderByDescending(m => m.DataEnvio)
                .ToListAsync();

            return View(mensagens);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EnviarParaSelecionados(int eventoId, List<int> destinatarios, string conteudo)
        {
            var remetenteIdClaim = User.FindFirst("UserId");
            if (remetenteIdClaim == null)
            {
                TempData["Erro"] = "Utilizador não autenticado.";
                return RedirectToAction("Index", "Eventos");
            }

            int remetenteId = int.Parse(remetenteIdClaim.Value);

            if (destinatarios == null || !destinatarios.Any())
            {
                TempData["Erro"] = "Nenhum destinatário selecionado.";
                return RedirectToAction("Detalhes", "Eventos", new { id = eventoId });
            }

            if (string.IsNullOrWhiteSpace(conteudo))
            {
                TempData["Erro"] = "A mensagem não pode estar vazia.";
                return RedirectToAction("Detalhes", "Eventos", new { id = eventoId });
            }


            foreach (var destId in destinatarios)
            {
                var mensagem = new Mensagem
                {
                    EventoId = eventoId,
                    DestinatarioId = destId,
                    RemetenteId = remetenteId,
                    Conteudo = conteudo
                };
                _context.Mensagens.Add(mensagem);
            }

            await _context.SaveChangesAsync();

            TempData["MensagemEnviada"] = "Mensagem enviada com sucesso!";
            return RedirectToAction("Detalhes", "Eventos", new { id = eventoId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EnviarMensagem([FromBody] Mensagem mensagem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int remetenteId))
                return Unauthorized();

            mensagem.RemetenteId = remetenteId;
            mensagem.DataEnvio = DateTime.UtcNow;

            _context.Mensagens.Add(mensagem);
            await _context.SaveChangesAsync();

            return Ok(new { sucesso = true });
        }



    }
}
