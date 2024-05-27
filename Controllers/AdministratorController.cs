using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using WebAPP.Extensions;
using WebAPP.Infrastructure.Models;
using WebAPP.MiddlewareFactory;
using WebAPP.Utilities;

namespace WebAPP.Infrastructure.Controllers;

public class AdministratorController : Controller, IActionFilter
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

    [HttpPost("Accounts")]
    public async Task<IActionResult> AccountsAsync(Tokens token)
    {
        try
        {
            // Action chiamata da un bottone submit (che funziona correttamente senza [fromBody]) e da una ajax call (che fornisce il parametro token correttamente caricato solo se non si specifica stringify e il contentype)
            // Sembra quindi che, al contrario, se si specifica [fromBody] la chiamata ajax debba specificare il contentType e la funzione stringify nei data.
            _httpClient.SetTokens(token);
            var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.GlobalParameters.Config.GetValue<string>("apiURL")!}Administrator/Accounts", token);
            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest) throw new BadHttpRequestException(httpResponseMessage.Content.ReadAsStream().ToString() ?? string.Empty);
            var prova = await httpResponseMessage.Content.ReadFromJsonAsync<AccountsDto>();
            return await Task.Run(async () => View(prova));
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
