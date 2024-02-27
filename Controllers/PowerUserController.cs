using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPP.Infrastructure.Models;
using WebAPP.MiddlewareFactory;

namespace WebAPP.Infrastructure.Controllers
{
    public class PowerUserController : Controller
    {
        private readonly HttpClient _httpClient;
        public PowerUserController(HttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.Client;
        }

        [HttpPost]
        public async Task<IActionResult> Index(Tokens token) => await Task.Run(() => View(token));


    }
}
