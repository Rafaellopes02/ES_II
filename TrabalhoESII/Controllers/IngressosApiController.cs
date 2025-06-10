using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrabalhoESII.Models;

namespace TrabalhoESII.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/ingressos")]
    public class IngressosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IngressosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostIngressos([FromBody] List<IngressoDTO> ingressosDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ingressos = ingressosDTO.Select(dto => new ingressos
            {
                nomeingresso = dto.nomeingresso,
                idtipoingresso = dto.idtipoingresso,
                quantidadedefinida = dto.quantidadedefinida,
                quantidadeatual = dto.quantidadeatual,
                preco = dto.preco,
                idevento = dto.idevento
            }).ToList();

            await _context.ingressos.AddRangeAsync(ingressos);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("por-evento/{id}")]
        [Authorize]
        public IActionResult GetIngressosPorEvento(int id)
        {
            var ingressos = _context.ingressos
                .Where(i => i.idevento == id)
                .Select(i => new {
                    i.idingresso,
                    i.nomeingresso,
                    i.quantidadeatual,
                    i.quantidadedefinida,
                    i.preco
                })
                .ToList();

            return Ok(ingressos);
        }
    }
}