using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPP.Extensions;
using WebAPP.Infrastructure.Models;
using WebAPP.MiddlewareFactory;
using WebAPP.Utilities;

namespace WebAPP.Infrastructure.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, object> _configLogger;

        public AdministratorController(ILogger logger, HttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.Client;
            _configLogger = new()
            {
                { "OperationId", Guid.NewGuid() }
            };
        }

        [HttpPost("Index")]
        public async Task<IActionResult> IndexAsync(Tokens token)
        {
            try
            {
                _httpClient.SetTokens(token);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.GlobalParameters.Config.GetValue<string>("apiURL")!}Administrator/Accounts", token);
                if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest) throw new BadHttpRequestException(httpResponseMessage.Content.ReadAsStream().ToString() ?? string.Empty);
                return await Task.Run(async () => View(await httpResponseMessage.Content.ReadFromJsonAsync<AccountsDto>()));
            }
            catch(Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                token.Errors = [ex.Message];
            }
            return View(token);
        }

        [HttpPost("AddAccount")]
        public async Task<IActionResult> AddAccountAsync([FromBody] AccountsDto accountsDto)
        {
            try
            {
                _httpClient.SetTokens(accountsDto);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.GlobalParameters.Config.GetValue<string>("apiURL")!}Administrator/AddAccount", accountsDto);
                if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest) throw new BadHttpRequestException(httpResponseMessage.Content.ReadAsStream().ToString() ?? string.Empty);
                return await Task.Run(async () => View(await httpResponseMessage.Content.ReadFromJsonAsync<AccountsDto>()));
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                accountsDto.Errors = [ex.Message];
            }
            return View(accountsDto);
        }

        [HttpPost("DeleteAccount")]
        public async Task<IActionResult> DeleteUserAsync(AccountDto account)
        {
            try
            {
                _httpClient.SetTokens(account);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.GlobalParameters.Config.GetValue<string>("apiURL")!}Administrator/DeleteAccount", account);
                if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest) throw new BadHttpRequestException(httpResponseMessage.Content.ReadAsStream().ToString() ?? string.Empty);
                return await Task.Run(async () => View(await httpResponseMessage.Content.ReadFromJsonAsync<AccountsDto>()));
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                account.Errors = [ex.Message];
            }
            return View(account);
        }
    }
}
