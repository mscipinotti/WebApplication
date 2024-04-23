using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System.Net;
using WebAPP.Extensions;
using WebAPP.Infrastructure.GlobalParameters;
using WebAPP.Infrastructure.Models;
using WebAPP.MiddlewareFactory;
using WebAPP.Utilities;

namespace WebAPP.Controllers
{
    public class HomeController : Controller, IActionFilter
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, object> _configLogger;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly Tokens _initialToken = new ()
        {
            Login = string.Empty,
            Password = string.Empty
        };

        public HomeController(ILogger logger, HttpClientFactory httpClientFactory, IMapper mapper, IStringLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _httpClient = httpClientFactory.Client;
            _mapper = mapper;
            _configLogger = new()
            {
                { "OperationId", Guid.NewGuid() }
            };
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync() => await Task.Run(() => View(_initialToken));

        #region Logon/logoff
        [HttpPost("Logon")]
        public async Task<IActionResult> LogonAsync(Tokens token)
        {
            try
            {
                // Il dato di business "account" viene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/Logon", token);
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var responseAccount = (await httpResponseMessage.Content.ReadFromJsonAsync<AccountDto>())!;
                    if (responseAccount is not null)
                    {
                        var cookies = httpResponseMessage.Headers.GetValues("Set-Cookie");
                        responseAccount.Cookie = cookies.First(c => c.StartsWith("XSRF-TOKEN")).Split(new string[] { "; " }, StringSplitOptions.None)[0];
                        responseAccount.RequestVerificationToken = cookies.First(c => c.StartsWith("X-XSRF-TOKEN"))
                                                                          .Split(new string[] { "X-XSRF-TOKEN=" }, StringSplitOptions.None)[1]
                                                                          .Split(new string[] { "; " }, StringSplitOptions.None)[0];
                        _httpClient.SetTokens(responseAccount);
                        return View("Views/Dashboard/Dashboard.cshtml", responseAccount);
                    }
                }
                token.Errors = [httpResponseMessage.StatusCode.ToString()];
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                token.Errors = [ex.Message];
            }
            return View($"Index", token);
        }

        [HttpPost("Logoff")]
        public async Task<IActionResult> LogoffAsync(Tokens token)
        {
            try
            {
                _httpClient.SetTokens(token);
                HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/Logoff", token);
                // Il servizio Logoff è targato per ritornare codici 200 e 400 esclusivamente
                if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest) throw new BadHttpRequestException(httpResponseMessage.Content.ReadAsStream().ToString() ?? string.Empty);
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                token.Errors = [ex.Message];
            }
            return View("Index", token);
        }
        #endregion

        #region Singer
        [HttpPost]
        public async Task<IActionResult> AddSinger(Tokens? token, SingerDto singer)
        {
            try
            {
                _httpClient.SetTokens(token);
                // Il dato di business "account" viene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
                HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/AddSinger", token);
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    token = await httpResponseMessage.Content.ReadFromJsonAsync<AccountDto>();
                    if (token is not null)
                    {
                        var cookies = httpResponseMessage.Headers.GetValues("Set-Cookie");
                        token.Cookie = cookies.First(c => c.StartsWith("XSRF-TOKEN")).Split(new string[] { "; " }, StringSplitOptions.None)[0];
                        token.RequestVerificationToken = cookies.First(c => c.StartsWith("X-XSRF-TOKEN"))
                                                                      .Split(new string[] { "X-XSRF-TOKEN=" }, StringSplitOptions.None)[1]
                                                                      .Split(new string[] { "; " }, StringSplitOptions.None)[0];
                        return View("Views/Dashboard/Dashboard.cshtml", token);
                    }
                }
            }
            catch (Exception)
            {
                // da sistemare
            }
            return View("Index");
        }
        #endregion

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Questi due metodi permettono di scrivere nel log quando comincia una chiamata e quando termina senza intasare il codice della action stessa e di validare il contesto.Il log è centralizzato
            try
            {
                if (!context.ModelState.IsValid)
                {
                    WriteLog.WriteErrorLog(_logger, _configLogger, $"Invalid context for {context.ActionDescriptor.DisplayName} action");
                }

                _configLogger["Action"] = context.ActionDescriptor.DisplayName ?? string.Empty;

                using (_logger.BeginScope(_configLogger)) WriteLog.WriteInfoLog(_logger, _configLogger, "Calling {Action} ...");
            }
            catch (ArgumentException ex)
            {
                WriteLog.WriteInfoLog(_logger, _configLogger, $"Something went wrong, probably token validation: {ex.Message}");
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            using (_logger.BeginScope(_configLogger)) WriteLog.WriteInfoLog(_logger, _configLogger, "{Action} call ended");
        }
    }
}
