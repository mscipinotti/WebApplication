using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using WebAPP.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPP.Controllers
{
    public class HomeController : Controller
    {
        string apiURL = "https://localhost:44396/";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAntiforgery _antiforgery;

        public HomeController (IHttpContextAccessor httpContextAccessor, IAntiforgery antiforgery)
        {
            _httpContextAccessor = httpContextAccessor;
            _antiforgery = antiforgery;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiURL);
                client.DefaultRequestHeaders.Accept.Clear();

                HttpResponseMessage httpResponseMessage = await client.GetAsync($"{apiURL}home/GetAntiforgeryToken");
                var accountDto = JsonConvert.DeserializeObject<AccountDto> (httpResponseMessage.Content.ReadAsStringAsync().Result);
                return View("Index", accountDto);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Index(AccountDto account)
        {

            AccountDto? loggedOnAccount = null;

            using HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(apiURL);
            client.DefaultRequestHeaders.Accept.Clear();

            _antiforgery.SetCookieTokenAndHeader(HttpContext);
                
            // Il dato di business "account" vinene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
            HttpResponseMessage httpResponseMessage = await client.PostAsJsonAsync($"{apiURL}home/Logon", account);
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                loggedOnAccount = await httpResponseMessage.Content.ReadFromJsonAsync<AccountDto>();

            return View("~/Views/Home/Index.cshtml", loggedOnAccount ?? account);
        }
    }
}
