using AutoMapper;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using WebAPP.Infrastructure.Infrastructure;
using WebAPP.Infrastructure.Models;
using WebAPP.Utilities;

namespace WebAPP.Infrastructure.Controllers
{
    public class HomeController : Controller, IActionFilter
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAntiforgery _antiforgery;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, object> _configLogger;

        public HomeController(IConfiguration config, ILogger logger, IHttpContextAccessor httpContextAccessor, IAntiforgery antiforgery, IMapper mapper)
        {
            _config = config;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _antiforgery = antiforgery;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(GlobalParameters.Config.GetValue<string>("apiURL")!)

            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _mapper = mapper;
            _configLogger = new()
            {
                { "OperationId", Guid.NewGuid() }
            };
        }

        [HttpGet]
        public IActionResult Index() => View(new Tokens()); 

        #region Logon/logoff
        [HttpPost]
        public async Task<IActionResult> Logon(AccountDto account)
        {
            HttpResponseMessage httpResponseMessage;
            try
            {
                // Il dato di business "account" viene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
                httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/Logon", account);
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    account = (await httpResponseMessage.Content.ReadFromJsonAsync<AccountDto>())!;
                    if (account is not null)
                    {
                        var cookies = httpResponseMessage.Headers.GetValues("Set-Cookie");
                        account.Cookie = cookies.First(c => c.StartsWith("XSRF-TOKEN")).Split(new string[] { "; " }, StringSplitOptions.None)[0];
                        account.RequestVerificationToken = cookies.First(c => c.StartsWith("X-XSRF-TOKEN"))
                                                                      .Split(new string[] { "X-XSRF-TOKEN=" }, StringSplitOptions.None)[1]
                                                                      .Split(new string[] { "; " }, StringSplitOptions.None)[0];
                        return View("Views/Dashboard/Dashboard.cshtml", account);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                account.Errors = [ ex.Message ];
                return View($"Index", account);
            }
            account.Errors = [ httpResponseMessage.StatusCode.ToString() ];
            return View($"Index", account);
        }

        [HttpPost]
        public async Task<IActionResult> Logoff(Tokens token)
        {
            InitHttpClient(token);
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/Logoff", token);
                if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                {
                    token = (await httpResponseMessage.Content.ReadFromJsonAsync<Tokens?>())!;
                    return BadRequest(token.Errors![0]);
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
            }
            return Ok();
        }
        #endregion

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
                        return View("Views/Dashboard/Dashboard.cshtml", account);
                    }
                }
            }
            catch (Exception)
            {
                // da sistemare
            }
            return View("Index");
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            // Questi due metodi permettono di scrivere nel log quando comincia una chiamata e quando termina senza intasare il codice della action stessa e di validare il contesto.Il log è centralizzato
            try
            {
                if (!context.ModelState.IsValid)
                {
                    //context.Result = context.ModelState.GetInvalidModelStateObjectResult();
                    WriteLog.WriteErrorLog(_logger, _configLogger, $"Invalid context for {context.ActionDescriptor.DisplayName} action");
                }

                _configLogger["Action"] = context.ActionDescriptor.DisplayName ?? string.Empty;

                using (_logger.BeginScope(_configLogger)) _logger.LogInformation("Calling {Action} ...", _configLogger["Action"]);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Something went wrong, probably token validation: {ex.Message}");
            }
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            using (_logger.BeginScope(_configLogger)) _logger.LogInformation("{Action} call ended", _configLogger["Action"]);
        }
        private void InitHttpClient(Tokens token)
        {
            // Antiforgery token
            _httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", $"{token.RequestVerificationToken}");
            _httpClient.DefaultRequestHeaders.Add("Cookie", $"{token.Cookie}");

            // Jwt token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.JwtToken);
        }
    }
}
