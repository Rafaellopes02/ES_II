﻿using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using TrabalhoESII.Models;

namespace TrabalhoESII.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TiposIngressosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TiposIngressosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTiposIngressos()
        {
            var tipos = _context.tiposingressos
                .Select(t => new
                {
                    idtipoingresso = t.idtipoingresso,
                    nome = t.nome
                })
                .ToList();

            return Ok(tipos);
        }
    }
}