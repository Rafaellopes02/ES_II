using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Controllers;
using TrabalhoESII.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TrabalhoESII.Tests
{
    [TestFixture]
    public class AtividadesApiControllerTests
    {
        private ApplicationDbContext _context;
        private AtividadesApiController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Categoria mínima para FK
            _context.categorias.Add(new categorias { idcategoria = 1, nome = "Genérica" });

            _context.eventos.Add(new eventos
            {
                idevento    = 10,
                nome        = "Evento Teste",
                descricao   = "Evento de seed para testes",
                local       = "Lisboa",
                data        = DateTime.Today,
                hora        = new TimeSpan(10, 0, 0),
                capacidade  = 100,
                idcategoria = 1
            });

            _context.atividades.Add(new atividades
            {
                idatividade      = 1,
                nome             = "Atividade 1",
                idevento         = 10,
                data             = DateTime.Today,
                hora             = new TimeSpan(11, 0, 0),
                quantidademaxima = 20
            });

            _context.SaveChanges();

            _controller = new AtividadesApiController(_context);

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim("UserId", "1") }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose(); // Liberta o contexto após cada teste
        }

        [Test]
        public async Task SearchAtividades_DeveRetornarAtividadesDoEvento()
        {
            var result = await _controller.SearchAtividades(10);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var atividades = okResult.Value as IEnumerable<object>;
            Assert.IsNotNull(atividades);
            Assert.AreEqual(1, atividades.Count());
        }

        [Test]
        public async Task InscreverNaAtividade_ComDadosValidos_DeveRetornarSucesso()
        {
            var result = await _controller.InscreverNaAtividade(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Inscrição realizada com sucesso.", okResult.Value);
        }

        [Test]
        public async Task InscreverNaAtividade_SendoOrganizador_DeveRetornarErro()
        {
            _context.organizadoreseventos.Add(new organizadoreseventos
            {
                idevento = 10,
                idutilizador = 1,
                eorganizador = true
            });
            _context.SaveChanges();

            var result = await _controller.InscreverNaAtividade(1);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("Organizadores não podem inscrever-se nas atividades do seu próprio evento.", badRequest.Value);
        }
    }
}
