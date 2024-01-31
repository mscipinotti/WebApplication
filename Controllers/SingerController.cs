using AutoMapper;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using WebAPP.Infrastructure;
using WebAPP.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPP.Controllers
{
    public class SingerController : Controller
    {
        private readonly IAntiforgery _antiforgery;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public SingerController(IAntiforgery antiforgery, IMapper mapper)
        {
            _antiforgery = antiforgery;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(GlobalParameters.Config.GetValue<string>("apiURL")!)

            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Singers(Tokens token)
        {
            // HttpContent per la chiamata sul client verso il BE. HttpContext per la risposta sul server verso il client.
            // I due token dell'antiforgery devono comparire in HttpContext.Form quello __RequestVerificationToken, l'altro in HttpContext.Headers.Cookie.
            // Così, in ValidateRequestAsync di DefaultAntiforgery.cs chiamato a sua volta da AntiforgeryTokenMiddleware sul BE la _tokenStore.GetRequestTokensAsync(httpContext) trova correttamente i due token per poterli validare.
            // I due token devono avere i nomi di __RequestVerificationToken e Cookie rispettivamente come i parametri presenti in tokens.FormFieldName e tokens.HeaderName e devono assumere i valori in
            // tokens.RequestToken e tokens.CookieToken in tokens di DefaultAntiforgery.cssul BE.
            try
            {
                // Antiforgery token
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("__RequestVerificationToken", token.RequestVerificationToken)
                });
                content.Headers.Add("Cookie", token.Cookie);

                // Jwt token
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.JwtToken}");

                HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/Singers", content);
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var singers = _mapper.Map<SingersDto>(token);
                    singers.Singers = await httpResponseMessage.Content.ReadFromJsonAsync<List<SingerDto>>();
                    return View(singers);
                }
            }
            catch (Exception)
            {
                // da sistemare
            }
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddSinger(Tokens token, SingerDto singer)
        {
            try
            {
                using HttpClient httpClient = new()
                {
                    BaseAddress = new Uri(GlobalParameters.Config.GetValue<string>("apiURL")!)
                };
                httpClient.DefaultRequestHeaders.Accept.Clear();

                // Antiforgery token
                httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", $"{token.RequestVerificationToken}");
                httpClient.DefaultRequestHeaders.Add("Cookie", $"{token.Cookie}");

                // Jwt token
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.JwtToken);

                // Il dato di business "account" viene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/AddSinger", singer);
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK) return RedirectToAction("Singers");
                var errors = await httpResponseMessage.Content.ReadFromJsonAsync<Errors>();
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
                // da sistemare
            }
            return View("Views/Home/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSinger(Tokens token, SingerDto singer)
        {
            try
            {
                using HttpClient httpClient = new()
                {
                    BaseAddress = new Uri(GlobalParameters.Config.GetValue<string>("apiURL")!)
                };
                httpClient.DefaultRequestHeaders.Accept.Clear();

                var request = new HttpRequestMessage(HttpMethod.Delete, $"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/DeleteSinger")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(singer), Encoding.UTF8, "application/json")
                };

                // Antiforgery token
                httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", $"{token.RequestVerificationToken}");
                httpClient.DefaultRequestHeaders.Add("Cookie", $"{token.Cookie}");

                // Jwt token
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.JwtToken);

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK) return Json(new { success = true, responseText = "Deleted singer!" });
                var errors = await httpResponseMessage.Content.ReadFromJsonAsync<Errors>();
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
                // da sistemare
            }
            return View("Views/Home/Index.cshtml");
        }
    }
}
