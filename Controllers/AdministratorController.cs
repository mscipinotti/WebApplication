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

        [HttpPost("UpdateAccounts")]
        public async Task<IActionResult> UpdateAccountsAsync([FromBody] AccountsDto accountsDto)
        {
            try
            {
                _httpClient.SetTokens(accountsDto);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.GlobalParameters.Config.GetValue<string>("apiURL")!}Administrator/UpdateAccounts", accountsDto, _ct.Token);
                if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    accountsDto.Errors = (await httpResponseMessage!.Content.ReadFromJsonAsync<AccountsDto>())!.Errors;
                    return BadRequest(accountsDto);
                }
                
                // La chiamata è partita da ajax per cui, se tutto OK, ritornando Ok e true in ajax il reload della pagina è automatico
                return Ok(accountsDto);
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                return View("Index", accountsDto);
            }
        }

        [HttpPost("DeleteAccounts")]
        public async Task<IActionResult> DeleteAccountsAsync([FromBody] AccountsDto accountsDto)
        {
            try
            {
                _httpClient.SetTokens(accountsDto);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.GlobalParameters.Config.GetValue<string>("apiURL")!}Administrator/DeleteAccounts", accountsDto, _ct.Token);
                if (httpResponseMessage.StatusCode != HttpStatusCode.OK) return BadRequest(await httpResponseMessage.Content.ReadFromJsonAsync<AccountsDto>());
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                accountsDto.Errors = [ex.Message];
                return BadRequest(accountsDto);
            }

            // Con il passaggio della modale il return true / false è già stato effettuato. Javascript non può gestire il reload della pagina nel caso in cui non ci sono stati errori.
            // Nessun reload della pagina, solo aggiornamento della tabella tramite javascript
            return Ok(accountsDto);
        }
    }
}
