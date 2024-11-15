using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System.Net;
using WebAPP.Extensions;
using WebAPP.Infrastructure.GlobalParameters;
using WebAPP.Infrastructure.Models.enums;
using WebAPP.MiddlewareFactory;
using WebAPP.Infrastructure.Utilities;
using System.Text.Json;
using WebApp.Infrastructure.Models.dto;
using WebApp.Infrastructure.Utilities;
using WebAPP.Services;

namespace WebAPP.Controllers
{
    // Senza routing. Ciò vuold dire che se non specificato il controller è HomeController.
    public class HomeController : Controller, IActionFilter
    {
        private readonly string[] tokenArray = ["X-XSRF-TOKEN="];
        private readonly string[] commaArray = ["; "];
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, object> _configLogger;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly Tokens _initialToken;

        public HomeController(ILogger logger, HttpClientFactory httpClientFactory, IStringLocalizer<HomeController> localizer, ILanguage language)
        {
            _logger = logger;
            _httpClient = httpClientFactory.Client;
            _configLogger = new()
            {
                { "OperationId", Guid.NewGuid() }
            };
            _localizer = localizer;
            _initialToken = new()
            {
                Login = string.Empty,
                Password = string.Empty,
                Profile = ProfileItems.User,
                Language = language.UserLanguage
            };
    }

        [HttpGet]
        public async Task<IActionResult> IndexAsync() => await Task.Run(() => View(_initialToken));

        #region Logon/logoff
        [HttpPost("Logon")]
        public async Task<IActionResult> LogonAsync(Tokens token)
        {
            try
            {
                AccountDto? responseAccount = null;
                // Il dato di business "account" viene serializzato e converito in array di byte e incapsulato in HttpContent. Per rendere il tutto esplicito usare PostAsync
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/Logon", token);
                switch (httpResponseMessage.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.InternalServerError: throw new BadHttpRequestException(await httpResponseMessage.Content.ReadAsStringAsync());
                    case HttpStatusCode.Unauthorized:
                        responseAccount = (await httpResponseMessage.Content.ReadFromJsonAsync<AccountDto>())!;
                        if (responseAccount is not null) token.Errors = responseAccount.Errors;
                        break;
                    case HttpStatusCode.OK:
                        try
                        {
                            responseAccount = (await httpResponseMessage.Content.ReadFromJsonAsync<AccountDto>())!;
                            if (responseAccount is not null)
                            {
                                
                                if (responseAccount.Status == StatusItems.ChangePassword) return View("Views/Home/ChangePassword.cshtml", responseAccount);
                                var cookies = httpResponseMessage.Headers.GetValues("Set-Cookie");
                                responseAccount.Cookie = cookies.First(c => c.StartsWith("XSRF-TOKEN")).Split(commaArray, StringSplitOptions.None)[0];
                                responseAccount.RequestVerificationToken = cookies.First(c => c.StartsWith("X-XSRF-TOKEN"))
                                                                                  .Split(tokenArray, StringSplitOptions.None)[1]
                                                                                  .Split(commaArray, StringSplitOptions.None)[0];
                                _httpClient.SetTokens(responseAccount);
                                return View("Views/Dashboard/Dashboard.cshtml", responseAccount);
                            }
                        }
                        catch (JsonException ex)
                        {
                            WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                            token.Errors = [_localizer[ex.Message]];
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                // json per eliminare gli apici doppi nella stringa ritornata
                token.Errors = [ _localizer[ex.Message ?? "UnpredictableError" ] ];
            }
            return View($"Index", token);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(Tokens token)
        {
            try
            {
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}home/ChangePassword", token);
                switch (httpResponseMessage.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.InternalServerError: throw new BadHttpRequestException(await httpResponseMessage.Content.ReadAsStringAsync());
                    case HttpStatusCode.OK:
                        var responseAccount = (await httpResponseMessage.Content.ReadFromJsonAsync<AccountDto>())!;
                        if (responseAccount is not null)
                        {
                            var cookies = httpResponseMessage.Headers.GetValues("Set-Cookie");
                            responseAccount.Cookie = cookies.First(c => c.StartsWith("XSRF-TOKEN")).Split(commaArray, StringSplitOptions.None)[0];
                            responseAccount.RequestVerificationToken = cookies.First(c => c.StartsWith("X-XSRF-TOKEN"))
                                                                              .Split(tokenArray, StringSplitOptions.None)[1]
                                                                              .Split(commaArray, StringSplitOptions.None)[0];
                            _httpClient.SetTokens(responseAccount);
                            if (responseAccount.Status == StatusItems.ChangePassword) return View("Views/Home/ChangePassword.cshtml", responseAccount);
                            return View("Views/Dashboard/Dashboard.cshtml", responseAccount);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                // json per eliminare gli apici doppi nella stringa ritornata
                token.Errors = [_localizer[JsonSerializer.Deserialize<string>(ex.Message) ?? "UnpredictableError"]];
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
