using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;

namespace TrabalhoESII.Controllers
{
    [Route("api/categorias")]
    [ApiController]
    public class CategoriasApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriasApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _context.categorias
                .Select(c => new { c.idcategoria, c.nome })
                .ToListAsync();

            return Ok(categorias);
        }
    }
}