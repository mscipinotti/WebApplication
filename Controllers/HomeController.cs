﻿using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using WebAPP.Models;

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

        [HttpGet]
        public IActionResult Index() => View("Views/Home/SignIn.cshtml", new AccountDto());

        [HttpPost("Logon")]
        public async Task<IActionResult> LogonAsync(AccountDto? account)
        {
            try
            {
                using HttpClient client = new()
                {
                    BaseAddress = new Uri(apiURL)
                };
                client.DefaultRequestHeaders.Accept.Clear();

                // Il dato di business "account" viene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
                HttpResponseMessage httpResponseMessage = await client.PostAsJsonAsync($"{apiURL}home/Logon", account);
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
                        return View("~/Views/Home/Index.cshtml", account);
                    }
                }
            }
            catch (Exception)
            {
                // da sistemare
            }
            return View("~/Views/Home/SignIn.cshtml");
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

            // Antiforgery token
            httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", $"{account.RequestVerificationToken}");
            httpClient.DefaultRequestHeaders.Add("Cookie", $"{account.Cookie}");

            // Jwt token
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", account.JwtToken);

            AccountDto? loggedOnAccount = null;
            // Il dato di business "account" viene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync($"{apiURL}home/AddSinger", singer);
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                loggedOnAccount = await httpResponseMessage.Content.ReadFromJsonAsync<AccountDto>();
            return View("~/Views/Home/Index.cshtml", account);
        }
    }
}
