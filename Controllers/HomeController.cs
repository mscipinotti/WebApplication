using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
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

        public HomeController(IHttpContextAccessor httpContextAccessor, IAntiforgery antiforgery)
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
                var accountDto = JsonConvert.DeserializeObject<AccountDto>(httpResponseMessage.Content.ReadAsStringAsync().Result);
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

            // Il dato di business "account" viene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
            HttpResponseMessage httpResponseMessage = await client.PostAsJsonAsync($"{apiURL}home/Logon", account);
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                loggedOnAccount = await httpResponseMessage.Content.ReadFromJsonAsync<AccountDto>();

            Uri uri = new Uri(apiURL);
            CookieContainer cookies = new CookieContainer();
            foreach (var cookieHeader in httpResponseMessage.Headers.GetValues("Set-Cookie"))       // Lettura del cookie
                cookies.SetCookies(uri, cookieHeader);

            return View("~/Views/Home/Index.cshtml", loggedOnAccount ?? account);
        }

        [HttpPost("AddSinger")]
        public async Task<IActionResult> AddSingerAsync(AccountDto account)
        {
            SingerDto singer = new()
            {
                Id = 10,
                Age = 44,
                Firstname = "Mario",
                StageName = "Capanna",
                Surname = "boh",
                Account = "pippo",
                Email = "prova@gmail.com",
                RequestVerificationToken = account.RequestVerificationToken,
                Cookie = account.Cookie
            };
            using HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(apiURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            //Request.Headers.Cookie.Append()


            // Probabile fomra corretta
            httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", $"{account.RequestVerificationToken}");
            httpClient.DefaultRequestHeaders.Add("XSRF-TOKEN", $"{account.Cookie}");


            httpClient.DefaultRequestHeaders.Add("Cookie", $"X-XSRF-TOKEN={account.Cookie}");                                      // Scrittura del cookie
            httpClient.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={account.Cookie}");
            //httpClient.DefaultRequestHeaders.Add("Cookie", $"RequestVerificationToken={account.RequestVerificationToken}");
            //Request.Headers["Cookie"] = $"XSRF-TOKEN={account.Cookie}";
            //Request.Headers["Cookie"] = $"X-XSRF-TOKEN={account.Cookie}";
            AccountDto? loggedOnAccount = null;
            // Il dato di business "account" viene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync($"{apiURL}home/AddSinger", singer);
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                loggedOnAccount = await httpResponseMessage.Content.ReadFromJsonAsync<AccountDto>();
            return View("~/Views/Home/Index.cshtml", account);
        }
    }
}
