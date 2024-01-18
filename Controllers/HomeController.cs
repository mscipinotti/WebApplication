using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using WebAPP.Models;
using WebAPP.Infrastructure;
using System.Security.Principal;
using System.Net.Http.Json;

namespace WebAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAntiforgery _antiforgery;
        private readonly HttpClient _httpClient;

        public HomeController(IHttpContextAccessor httpContextAccessor, IAntiforgery antiforgery)
        {
            _httpContextAccessor = httpContextAccessor;
            _antiforgery = antiforgery;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(GlobalParameters.Config.GetValue<string>("apiURL")!)

            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Logon(AccountDto? account)
        {
            try
            {
                // Il dato di business "account" viene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
                HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/Logon", account);
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    account = await httpResponseMessage.Content.ReadFromJsonAsync<AccountDto>();
                    if (account is not null)
                    {
                        var cookies = httpResponseMessage.Headers.GetValues("Set-Cookie");
                        account.Cookie = cookies.First(c => c.StartsWith("XSRF-TOKEN")).Split(new string[] { "; " }, StringSplitOptions.None)[0];
                        account.RequestVerificationToken = cookies.First(c => c.StartsWith("X-XSRF-TOKEN"))
                                                                   .Split(new string[] { "X-XSRF-TOKEN=" }, StringSplitOptions.None)[1]
                                                                   .Split(new string[] { "; " }, StringSplitOptions.None)[0];
                        return View(account);
                    }
                }
            }
            catch (Exception)
            {
                // da sistemare
            }
            return View("~/Views/Home/SignIn.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Singers()
        {
            try {
                //HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/GetSingers", account);
                //if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                //{
                //    var singers = await httpResponseMessage.Content.ReadFromJsonAsync<List<SingerDto>>();
                    return View();
                    //}
                }
            catch (Exception) { 
            // da sistemare
            }
            return View("~/Views/Home/SignIn.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> AddSinger(AccountDto account)
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
            httpClient.BaseAddress = new Uri(GlobalParameters.Config.GetValue<string>("apiURL")!);
            httpClient.DefaultRequestHeaders.Accept.Clear();

            // Antiforgery token
            httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", $"{account.RequestVerificationToken}");
            httpClient.DefaultRequestHeaders.Add("Cookie", $"{account.Cookie}");

            // Jwt token
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", account.JwtToken);

            AccountDto? loggedOnAccount = null;
            // Il dato di business "account" viene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/AddSinger", singer);
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                loggedOnAccount = await httpResponseMessage.Content.ReadFromJsonAsync<AccountDto>();
            return View("~/Views/Home/Index.cshtml", account);
        }
    }
}
