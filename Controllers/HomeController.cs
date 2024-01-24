using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using WebAPP.Infrastructure;
using WebAPP.Models;

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
        public IActionResult Index()
        {
            ViewData["Layout"] = "_SignInLayout";
            return View(); 
        }

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
                        ViewData["Layout"] = "_Layout";
                        return View("Views/Dashboard/Dashboard.cshtml", account);
                    }
                }
            }
            catch (Exception)
            {
                // da sistemare
            }
            ViewData["Layout"] = "_SignInLayout";
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddSinger(AccountDto? account, SingerDto singer)
        {
            try
            {
                // Il dato di business "account" viene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
                HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/AddSinger", account);
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
                        ViewData["Layout"] = "_Layout";
                        return View("Views/Dashboard/Dashboard.cshtml", account);
                    }
                }
            }
            catch (Exception)
            {
                // da sistemare
            }
            ViewData["Layout"] = "_SignInLayout";
            return View("Index");
        }
    }
}
