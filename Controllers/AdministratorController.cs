using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
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
        private readonly CancellationTokenSource _ct;

    public AdministratorController(ILogger logger, HttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.Client;
            _configLogger = new()
            {
                { "OperationId", Guid.NewGuid() }
            };
            _ct = new CancellationTokenSource();
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
                return View(token);
            }
        }

        [HttpPost("AddAccounts")]
        public async Task<IActionResult> AddAccountsAsync([FromBody] AccountsDto accountsDto)
        {
            try
            {
                _httpClient.SetTokens(accountsDto);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.GlobalParameters.Config.GetValue<string>("apiURL")!}Administrator/AddAccounts", accountsDto, _ct.Token);
                if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    accountsDto.Errors = (await httpResponseMessage!.Content.ReadFromJsonAsync<AccountsDto>())!.Errors;
                    return BadRequest(accountsDto);
                }
                return await Task.Run(async () => View("Index", await httpResponseMessage.Content.ReadFromJsonAsync<AccountsDto>()));
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                return View("Index", accountsDto);
            }
        }

        [HttpPost("ModifyAccount")]
        public async Task<IActionResult> ModifyAccountAsync([FromBody] AccountsDto accountsDto)
        {
            try
            {
                _httpClient.SetTokens(accountsDto);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.GlobalParameters.Config.GetValue<string>("apiURL")!}Administrator/ModifyAccount", accountsDto, _ct.Token);
                if (httpResponseMessage.StatusCode != HttpStatusCode.OK) throw new BadHttpRequestException(httpResponseMessage.Content.ReadAsStringAsync().ToString() ?? string.Empty);
                return await Task.Run(async () => View(await httpResponseMessage.Content.ReadFromJsonAsync<AccountsDto>()));
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                accountsDto.Errors = [ex.Message];
            }
            return View(accountsDto);
        }

        [HttpPost("DeleteAccounts")]
        public async Task<IActionResult> DeleteAccountsAsync([FromBody] AccountsDto accountsDto)
        {
            try
            {
                _httpClient.SetTokens(accountsDto);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.GlobalParameters.Config.GetValue<string>("apiURL")!}Administrator/DeleteAccounts", accountsDto, _ct.Token);
                if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                {
                    accountsDto = await httpResponseMessage.Content.ReadFromJsonAsync<AccountsDto>();
                    return BadRequest(accountsDto);
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                accountsDto.Errors = [ex.Message];
                return BadRequest(accountsDto);
            }
            return await Task.Run(() => View(accountsDto));
        }
    }
}
