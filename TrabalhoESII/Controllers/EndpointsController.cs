using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TrabalhoESII.ViewModels;

namespace TrabalhoESII.Controllers
{
    [Route("endpoints")]
    public class EndpointsController : Controller
    {
        private readonly EndpointDataSource _endpointDataSource;

        public EndpointsController(EndpointDataSource endpointDataSource)
        {
            _endpointDataSource = endpointDataSource;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var endpoints = _endpointDataSource.Endpoints
                .OfType<RouteEndpoint>()
                .Where(e =>
                        !e.RoutePattern.RawText.Contains("{controller=Home}") && // ignora os padrões default
                        !e.RoutePattern.RawText.Contains("Index")                // ignora Index
                )
                .Select(e => new EndpointViewModel
                {
                    Route = e.RoutePattern.RawText,
                    Method = e.Metadata
                        .OfType<HttpMethodMetadata>()
                        .FirstOrDefault()?.HttpMethods.FirstOrDefault() ?? "ANY"
                })
                .OrderBy(e => e.Route)
                .ToList();

            return View(endpoints);
        }
    }
}